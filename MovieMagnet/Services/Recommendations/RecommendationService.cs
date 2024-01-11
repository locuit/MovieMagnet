using Tensorflow;
using Tensorflow.Keras.Layers;
using static Tensorflow.Binding;
using Microsoft.Data.Analysis;
using MovieMagnet.Services.Dtos.Recommendations;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.NumPy;

namespace MovieMagnet.Services.Recommendations;


public class RecommendationService : MovieMagnetAppService, IRecommendationService
{
    private const int EmbeddingDimension = 64;
    private NDArray _userArray;
    private NDArray _movieArray;
    private NDArray _ratingArray;
  
  public void PrepareData()
  {
      tf.enable_eager_execution();

      // adult,belongs_to_collection,budget,genres,homepage,id,imdb_id,original_language,original_title,overview,popularity,poster_path,production_companies,production_countries,release_date,revenue,runtime,spoken_languages,status,tagline,title,video,vote_average,vote_count
      //     False,"{'id': 10194, 'name': 'Toy Story Collection', 'poster_path': '/7G9915LfUQ2lVfwMEEhDsn3kT4B.jpg', 'backdrop_path': '/9FBwqcd9IRruEDUrTdcaafOMKUq.jpg'}",30000000,"[{'id': 16, 'name': 'Animation'}, {'id': 35, 'name': 'Comedy'}, {'id': 10751, 'name': 'Family'}]",http://toystory.disney.com/toy-story,862,tt0114709,en,Toy Story,"Led by Woody, Andy's toys live happily in his room until Andy's birthday brings Buzz Lightyear onto the scene. Afraid of losing his place in Andy's heart, Woody plots against Buzz. But when circumstances separate Buzz and Woody from their owner, the duo eventually learns to put aside their differences.",21.946943,/rhIRbceoE9lR4veEXuwCC2wARtG.jpg,"[{'name': 'Pixar Animation Studios', 'id': 3}]","[{'iso_3166_1': 'US', 'name': 'United States of America'}]",1995-10-30,373554033,81.0,"[{'iso_639_1': 'en', 'name': 'English'}]",Released,,Toy Story,False,7.7,5415
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
      var dates = timestamps.Select(timestamp => DateTimeOffset.FromUnixTimeSeconds((long)timestamp).DateTime).ToList();
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

      // Merge the ratings DataFrame with the format DataFrame
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
      
      var userIds = np.array(ratingsDf["userId"].Cast<long>().ToArray());
      var movieIds = np.array(ratingsDf["movieId"].Cast<long>().ToArray());
      var ratings = np.array(ratingsDf["rating"].Cast<float>().ToArray());


      var uniqueMovieTitles = np.unique(movieIds);
      var uniqueUserIds = np.unique(userIds);
      
        var numUsers = (int)uniqueUserIds.Item1.shape[0];
        var numMovies = (int)uniqueMovieTitles.Item1.shape[0];
        
        var userInp = tf.keras.Input(shape: new Shape(1), name: "user_input");
        var movieInp = tf.keras.Input(shape: new Shape(1), name: "movie_input");
        
        var userEmbedding = new Embedding(new EmbeddingArgs
        {
            InputDim = numUsers,
            OutputDim = EmbeddingDimension,
            Name = "user_embedding"
        }).Apply(userInp);
        
        var movieEmbedding = new Embedding(new EmbeddingArgs
        {
            InputDim = numMovies,
            OutputDim = EmbeddingDimension,
            Name = "movie_embedding"
        }).Apply(movieInp);
        
        var userEmbeddingFlat = tf.keras.layers.Flatten().Apply(userEmbedding);
        var movieEmbeddingFlat = tf.keras.layers.Flatten().Apply(movieEmbedding);
        
        var concat = tf.keras.layers.Concatenate().Apply(new Tensors(userEmbeddingFlat, movieEmbeddingFlat));
        
        var dense1 = tf.keras.layers.Dense(256, activation: "relu").Apply(concat);
        var dense2 = tf.keras.layers.Dense(128, activation: "relu").Apply(dense1);
        var ratingOutputLayer = tf.keras.layers.Dense(1).Apply(dense2);
        
        var model = tf.keras.Model(new Tensors(userInp, movieInp), ratingOutputLayer);
        var optimizer = tf.keras.optimizers.Adam();
        var lossFunc = tf.keras.losses.MeanSquaredError();
        model.compile(optimizer, lossFunc);
        
        // Reshape the input arrays
        var userIdsReshaped = userIds.reshape(new Shape(-1, 1));
        var movieIdsReshaped = movieIds.reshape(new Shape(-1, 1));
        
        var trainInputs = new[] { userIdsReshaped, movieIdsReshaped };
        var trainLabels = ratings.reshape(new Shape(-1, 1));

        model.fit(trainInputs, trainLabels, batch_size: 512, epochs: 5);
        
        var allMovieIds = np.array(moviesDf["movieId"].Cast<long>().ToArray());
        
        var userIdArray = np.full(new Shape(movieIds.shape[0]), 123);
        var userIdsReshapedNew = userIdArray.reshape(new Shape(-1, 1));
        
        var inputs = new Tensors(userIdsReshapedNew, movieIdsReshaped);
        
        var predictedRatings = model.predict(inputs);
        var predictions = predictedRatings.numpy();
        
        const int topN = 10;
        var recommendedMovieIds = np.argsort(predictions, axis: 0).ToArray<int>().Reverse().Take(topN).ToArray();
        
        Console.WriteLine($"Recommended movies: {recommendedMovieIds}");
  }


  // public void TrainModel(int numUsers, int numMovies, int embeddingDimension, int epochs)
  // {
  //     _model = new MovieModel(new MovieArgs
  //     {
  //           NumUsers = numUsers,
  //           NumMovies = numMovies,
  //           EmbeddingDimension = embeddingDimension
  //     });
  //     
  //     var optimizer = tf.keras.optimizers.Adam();
  //       var trainDataset = tf.data.Dataset.zip(_userIdDataset, _movieIdDataset).batch(1024);
  //       var testDataset = tf.data.Dataset.zip(_userIdDataset, _movieIdDataset).batch(1024);
  //     var lossFunc = tf.keras.losses.BinaryCrossentropy();
  //     var cachedTrain = trainDataset.cache();
  //     
  //       var cachedTest = testDataset.cache();
  // }

  // public List<int> RecommendMovies(int userId, int topN = 10)
  // {
  //     if (_model == null)
  //     {
  //         throw new InvalidOperationException("Model has not been trained.");
  //     }
  //
  //     var allMovieIds = _movies.Columns["id"].Cast<int>().ToArray();
  //
  //     var userIds = Enumerable.Repeat(userId, allMovieIds.Length).ToArray();
  //
  //     var userIdTensor = tf.convert_to_tensor(userIds, dtype: tf.int32);
  //     var movieIdTensor = tf.convert_to_tensor(allMovieIds, dtype: tf.int32);
  //
  //     // make predictions using the model
  //     var predictions = _model.predict(new Tensors(userIdTensor, movieIdTensor));
  //
  //     // Assuming the second output of the model is the watch likelihood
  //     var watchLikelihoods = predictions[1].ToArray<float>();
  //
  //     // Get top N recommended movie IDs based on the watch likelihoods
  //     var recommendedMovieIds = allMovieIds.Zip(watchLikelihoods, (movieId, likelihood) => new { movieId, likelihood })
  //         .OrderByDescending(x => x.likelihood)
  //         .Take(topN)
  //         .Select(x => x.movieId)
  //         .ToList();
  //
  //     return recommendedMovieIds;
  // }
  
  // public void Run()
  // {
  //     var (numUsers, numMovies, embeddingDimension, epochs) = PrepareData();
  //     TrainModel(numUsers, numMovies, embeddingDimension, epochs);
  //     var recommendedMovies = RecommendMovies(1);
  //     Console.WriteLine($"Recommended movies: {recommendedMovies}");
  // }
}