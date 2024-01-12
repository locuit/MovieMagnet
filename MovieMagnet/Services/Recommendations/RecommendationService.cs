using Tensorflow;
using Tensorflow.Keras.Layers;
using static Tensorflow.Binding;
using Microsoft.Data.Analysis;
using Microsoft.EntityFrameworkCore;
using MovieMagnet.Authorization;
using MovieMagnet.Data;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.Movies;
using MovieMagnet.Services.Dtos.Recommendations;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.NumPy;
using Volo.Abp.Application.Dtos;

namespace MovieMagnet.Services.Recommendations;


public class RecommendationService : MovieMagnetAppService, IRecommendationService
{
    private const int EmbeddingDimension = 128;
    private const int DenseLayerSize = 256;
    private const int DenseLayerSize2 = 128;
    private const int Epochs = 5;
    private const int BatchSize = 2048;
    private const double TestRatio = 0.3;
    private NDArray _userArray;
    private NDArray _movieArray;
    private NDArray _ratingArray;
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MovieMagnetDbContext _dbContext;
    public RecommendationService(IHttpContextAccessor httpContextAccessor, MovieMagnetDbContext dbContext)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }


    [Authorize]
    public async Task<PagedResultDto<MovieDto>> RecommendMovies(int n)
    {
        tf.enable_eager_execution();

        var user = _httpContextAccessor.HttpContext?.Items["User"] as UserDto;
        var path = Directory.GetCurrentDirectory();
        var movies = DataFrame.LoadCsv(path + "/Services/Recommendations/Data/movies_metadata.csv");

        string[] columnsMoviesToRemove =
            { "belongs_to_collection", "homepage", "imdb_id", "poster_path", "status", "title", "video" };

        foreach (var column in columnsMoviesToRemove)
        {
            movies.Columns.Remove(column);
        }

        var convertedIds = (from object? item in movies["id"] select Convert.ToInt64(item)).ToList();

        movies.Columns.Remove("id");
        movies.Columns.Add(new PrimitiveDataFrameColumn<long>("id", convertedIds));

        var keywords = DataFrame.LoadCsv(path + "/Services/Recommendations/Data/keywords.csv");

        convertedIds.Clear();
        convertedIds.AddRange(from object? item in keywords["id"] select Convert.ToInt64(item));

        keywords.Columns.Remove("id");
        keywords.Columns.Add(new PrimitiveDataFrameColumn<long>("id", convertedIds));

        var df = movies.Merge(keywords, new[] { "id" }, new[] { "id" }, joinAlgorithm: JoinAlgorithm.Inner);
        df.Columns.Remove("id_right");
        df.Columns.RenameColumn("id_left", "id");

        var ratingsDf = DataFrame.LoadCsv(path + "/Services/Recommendations/Data/ratings_small.csv");

        convertedIds.Clear();
        convertedIds.AddRange(from object? item in ratingsDf["movieId"] select Convert.ToInt64(item));

        ratingsDf.Columns.Remove("movieId");
        ratingsDf.Columns.Add(new PrimitiveDataFrameColumn<long>("movieId", convertedIds));
        var timestamps = ratingsDf["timestamp"].Cast<float>();
        var dates = timestamps.Select(timestamp => DateTimeOffset.FromUnixTimeSeconds((long)timestamp).DateTime)
            .ToList();
        ratingsDf.Columns.Remove("timestamp");

        ratingsDf.Columns.Add(new PrimitiveDataFrameColumn<DateTime>("date", dates));

        var selectedColumns = new List<DataFrameColumn>
        {
            df["id"],
            df["original_title"],
            df["genres"],
            df["overview"]
        };

        var formatDf = new DataFrame(selectedColumns);

        ratingsDf = ratingsDf.Merge(formatDf, new[] { "movieId" }, new[] { "id" }, joinAlgorithm: JoinAlgorithm.Inner);
        ratingsDf = ratingsDf.DropNulls();
        ratingsDf.Columns.Remove("id");

        selectedColumns = new List<DataFrameColumn>
        {
            df["id"],
            df["original_title"]
        };

        var moviesDf = new DataFrame(selectedColumns);

        moviesDf.Columns.RenameColumn("id", "movieId");
        convertedIds.Clear();
        convertedIds.AddRange(from object? item in ratingsDf["userId"] select Convert.ToInt64(item));
        ratingsDf.Columns.Remove("userId");
        ratingsDf.Columns.Add(new PrimitiveDataFrameColumn<long>("userId", convertedIds));

        var ratingCount = (int)ratingsDf.Rows.Count;

        var shuffledDf = ratingsDf.Sample(ratingCount);

        var testSize = (int)(shuffledDf.Rows.Count * TestRatio);
        var trainDf = shuffledDf.Head((int)shuffledDf.Rows.Count - testSize);
        var testDf = shuffledDf.Tail(testSize);

        // Prepare training data
        var trainUserIds = np.array(trainDf["userId"].Cast<long>().ToArray());
        var trainMovieIds = np.array(trainDf["movieId"].Cast<long>().ToArray());
        var trainRatings = np.array(trainDf["rating"].Cast<float>().ToArray());

        // Prepare test data
        var testUserIds = np.array(testDf["userId"].Cast<long>().ToArray());
        var testMovieIds = np.array(testDf["movieId"].Cast<long>().ToArray());
        var testRatings = np.array(testDf["rating"].Cast<float>().ToArray());

        // Define the model
        var userInp = tf.keras.Input(shape: new Shape(1), name: "user_input");
        var movieInp = tf.keras.Input(shape: new Shape(1), name: "movie_input");

        // Embedding layers
        var numUsers = (int)np.unique(trainUserIds).Item1.shape[0];
        var numMovies = (int)np.unique(trainMovieIds).Item1.shape[0];

        var userEmbedding = new Embedding(new EmbeddingArgs
        {
            InputDim = numUsers,
            OutputDim = EmbeddingDimension,
            Name = "user_embedding",
            InputLength = 1
        }).Apply(userInp);

        var movieEmbedding = new Embedding(new EmbeddingArgs
        {
            InputDim = numMovies,
            OutputDim = EmbeddingDimension,
            Name = "movie_embedding",
            InputLength = 1
        }).Apply(movieInp);

        // Flatten the embedding layers
        var userEmbeddingFlat = tf.keras.layers.Flatten().Apply(userEmbedding);
        var movieEmbeddingFlat = tf.keras.layers.Flatten().Apply(movieEmbedding);

        // Concatenate flattened embeddings
        var concat = tf.keras.layers.Concatenate().Apply(new Tensors(userEmbeddingFlat, movieEmbeddingFlat));

        // Additional Dense layers
        var dense1 = tf.keras.layers.Dense(DenseLayerSize, activation: "relu").Apply(concat);
        var dropout1 = tf.keras.layers.Dropout(0.3f).Apply(dense1); // Dropout layer
        var dense2 = tf.keras.layers.Dense(DenseLayerSize2, activation: "relu").Apply(dropout1);
        var dropout2 = tf.keras.layers.Dropout(0.3f).Apply(dense2); // Dropout layer
        var ratingOutputLayer = tf.keras.layers.Dense(1).Apply(dropout2);

        // Build and compile the model
        var model = tf.keras.Model(new Tensors(userInp, movieInp), ratingOutputLayer);
        var optimizer = tf.keras.optimizers.Adam();
        var lossFunc = tf.keras.losses.MeanSquaredError();
        model.compile(optimizer, lossFunc);

        // Reshape data for training
        var trainInputs = new[] { trainUserIds.reshape(new Shape(-1, 1)), trainMovieIds.reshape(new Shape(-1, 1)) };
        var trainLabels = trainRatings.reshape(new Shape(-1, 1));

        // Train the model
        model.fit(trainInputs, trainLabels, batch_size: BatchSize, epochs: Epochs);

        // Reshape data for testing
        var testUserIdsReshaped = testUserIds.reshape(new Shape(-1, 1));
        var testMovieIdsReshaped = testMovieIds.reshape(new Shape(-1, 1));
        var testLabelsReshaped = testRatings.reshape(new Shape(-1, 1));

        var combinedTestInputs = new Tensors(testUserIdsReshaped, testMovieIdsReshaped);

        var predictedRatings = model.predict(combinedTestInputs);
        var predictions = predictedRatings.numpy();

        var mse = np.mean(np.square(predictions - testLabelsReshaped));
        var rmse = Math.Sqrt(mse);
        Console.WriteLine($"RMSE: {rmse}");
        Console.WriteLine($"MSE: {mse}");

        // Recommendations for a specific user
        var userIdArray = np.full((numMovies, 1), user?.Id ?? 123);
        var userIdsReshaped = userIdArray.reshape(new Shape(-1, 1));

        var movieIds = np.array(ratingsDf["movieId"].Cast<long>().Distinct().ToArray());

        var inputs = new Tensors(userIdsReshaped, movieIds.reshape(new Shape(-1, 1)));
        var userPredictions = model.predict(inputs).numpy();

        var recommendedMovieIds =
            np.argsort(userPredictions, axis: 0).ToArray<int>().Reverse().Take(n).ToArray().ToList();
        
        // console log
        foreach (var movieId in recommendedMovieIds)
        {
            Console.WriteLine(movieId);
        }

        var moviesQuery = _dbContext.Movies
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .Where(m => recommendedMovieIds.Contains((int)m.Id));
        
        var movieDtos = moviesQuery.Select(entry => new MovieDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Budget = entry.Budget,
            Language = entry.Language,
            Overview = entry.Overview,
            PosterPath = entry.PosterPath,
            ReleaseDate = entry.ReleaseDate,
            ImdbId = entry.ImdbId,
            Popularity = entry.Popularity,
            Revenue = entry.Revenue,
            Runtime = entry.Runtime,
            VoteAverage = entry.VoteAverage,
            VoteCount = entry.VoteCount,
            Genres = entry.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
        }).ToList();
        
        var result = new PagedResultDto<MovieDto>
        {
            TotalCount = await moviesQuery.CountAsync(),
            Items = movieDtos
        };
        
        return result;
    }
}