using Tensorflow;
using Tensorflow.Keras.Engine;
using static Tensorflow.Binding;
using Microsoft.Data.Analysis;
using MovieMagnet.Services.Dtos.Recommendations;
using Tensorflow.NumPy;
using Tensorflow.Train;

namespace MovieMagnet.Services.Recommendations;

public class RecommendationService : MovieMagnetAppService, IRecommendationService
{
    // I want build the model here and then use it to make predictions
    // I want to use the model to make predictions in the MovieService
    
    // credits = pd.read_csv('../input/the-movies-dataset/credits.csv')
    //     keywords = pd.read_csv('../input/the-movies-dataset/keywords.csv')
    // movies = pd.read_csv('../input/the-movies-dataset/movies_metadata.csv').\
    // drop(['belongs_to_collection', 'homepage', 'imdb_id', 'poster_path', 'status', 'title', 'video'], axis=1).\
    // drop([19730, 29503, 35587]) # Incorrect data type
    //
    //     movies['id'] = movies['id'].astype('int64')
    //
    // df = movies.merge(keywords, on='id').\
    // merge(credits, on='id')
    //
    // df['original_language'] = df['original_language'].fillna('')
    // df['runtime'] = df['runtime'].fillna(0)
    // df['tagline'] = df['tagline'].fillna('')
    //
    // df.dropna(inplace=True)
    
    // ratings_df = pd.read_csv('../input/the-movies-dataset/ratings_small.csv')
    //
    //     ratings_df['date'] = ratings_df['timestamp'].apply(lambda x: datetime.fromtimestamp(x))
    // ratings_df.drop('timestamp', axis=1, inplace=True)
    //
    // ratings_df = ratings_df.merge(df[['id', 'original_title', 'genres', 'overview']], left_on='movieId',right_on='id', how='left')
    // ratings_df = ratings_df[~ratings_df['id'].isna()]
    // ratings_df.drop('id', axis=1, inplace=True)
    // ratings_df.reset_index(drop=True, inplace=True)
    //
    // ratings_df.head()
    
    // movies_df = df[['id', 'original_title']]
    // movies_df.rename(columns={'id':'movieId'}, inplace=True)
    // movies_df.head()
    
    // ratings_df['userId'] = ratings_df['userId'].astype(str)
    //
    // ratings = tf.data.Dataset.from_tensor_slices(dict(ratings_df[['userId', 'original_title', 'rating']]))
    // movies = tf.data.Dataset.from_tensor_slices(dict(movies_df[['original_title']]))
    //
    // ratings = ratings.map(lambda x: {
    //     "original_title": x["original_title"],
    //     "userId": x["userId"],
    //     "rating": float(x["rating"])
    // })
    //
    // movies = movies.map(lambda x: x["original_title"])
    
    // print('Total Data: {}'.format(len(ratings)))
    //
    // tf.random.set_seed(42)
    //     shuffled = ratings.shuffle(100_000, seed=42, reshuffle_each_iteration=False)
    //
    // train = ratings.take(35_000)
    //     test = ratings.skip(35_000).take(8_188)
    
    // movie_titles = movies.batch(1_000)
    //     user_ids = ratings.batch(1_000).map(lambda x: x["userId"])
    //
    // unique_movie_titles = np.unique(np.concatenate(list(movie_titles)))
    // unique_user_ids = np.unique(np.concatenate(list(user_ids)))
    //
    // print('Unique Movies: {}'.format(len(unique_movie_titles)))
    // print('Unique users: {}'.format(len(unique_user_ids)))
    
    
  //   class MovieModel(tfrs.models.Model):
  //
  // def __init__(self, rating_weight: float, retrieval_weight: float) -> None:
  //   # We take the loss weights in the constructor: this allows us to instantiate
  //   # several model objects with different loss weights.
  //
  //   super().__init__()
  //
  //   embedding_dimension = 64
  //
  //   # User and movie models.
  //   self.movie_model: tf.keras.layers.Layer = tf.keras.Sequential([
  //     tf.keras.layers.StringLookup(
  //       vocabulary=unique_movie_titles, mask_token=None),
  //     tf.keras.layers.Embedding(len(unique_movie_titles) + 1, embedding_dimension)
  //   ])
  //   self.user_model: tf.keras.layers.Layer = tf.keras.Sequential([
  //     tf.keras.layers.StringLookup(
  //       vocabulary=unique_user_ids, mask_token=None),
  //     tf.keras.layers.Embedding(len(unique_user_ids) + 1, embedding_dimension)
  //   ])
  //
  //   # A small model to take in user and movie embeddings and predict ratings.
  //   # We can make this as complicated as we want as long as we output a scalar
  //   # as our prediction.
  //   self.rating_model = tf.keras.Sequential([
  //       tf.keras.layers.Dense(256, activation="relu"),
  //       tf.keras.layers.Dense(128, activation="relu"),
  //       tf.keras.layers.Dense(1),
  //   ])
  //
  //   # The tasks.
  //   self.rating_task: tf.keras.layers.Layer = tfrs.tasks.Ranking(
  //       loss=tf.keras.losses.MeanSquaredError(),
  //       metrics=[tf.keras.metrics.RootMeanSquaredError()],
  //   )
  //   self.retrieval_task: tf.keras.layers.Layer = tfrs.tasks.Retrieval(
  //       metrics=tfrs.metrics.FactorizedTopK(
  //           candidates=movies.batch(128).map(self.movie_model)
  //       )
  //   )
  //
  //   # The loss weights.
  //   self.rating_weight = rating_weight
  //   self.retrieval_weight = retrieval_weight
  //
  // def call(self, features: Dict[Text, tf.Tensor]) -> tf.Tensor:
  //   # We pick out the user features and pass them into the user model.
  //   user_embeddings = self.user_model(features["userId"])
  //   # And pick out the movie features and pass them into the movie model.
  //   movie_embeddings = self.movie_model(features["original_title"])
  //   
  //   return (
  //       user_embeddings,
  //       movie_embeddings,
  //       # We apply the multi-layered rating model to a concatentation of
  //       # user and movie embeddings.
  //       self.rating_model(
  //           tf.concat([user_embeddings, movie_embeddings], axis=1)
  //       ),
  //   )
  //
  // def compute_loss(self, features: Dict[Text, tf.Tensor], training=False) -> tf.Tensor:
  //
  //   ratings = features.pop("rating")
  //
  //   user_embeddings, movie_embeddings, rating_predictions = self(features)
  //
  //   # We compute the loss for each task.
  //   rating_loss = self.rating_task(
  //       labels=ratings,
  //       predictions=rating_predictions,
  //   )
  //   retrieval_loss = self.retrieval_task(user_embeddings, movie_embeddings)
  //
  //   # And combine them using the loss weights.
  //   return (self.rating_weight * rating_loss
  //           + self.retrieval_weight * retrieval_loss)
  
  // model = MovieModel(rating_weight=1.0, retrieval_weight=1.0)
  // model.compile(optimizer=tf.keras.optimizers.Adagrad(0.1))
  //
  // cached_train = train.shuffle(100_000).batch(1_000).cache()
  // cached_test = test.batch(1_000).cache()
  //
  // model.fit(cached_train, epochs=3)
  
  
    // convert code tensorflow code to c# code
    // read csv file using csvhelper
    
    public void CreateModel()
    {
        // adult,belongs_to_collection,budget,genres,homepage,id,imdb_id,original_language,original_title,overview,popularity,poster_path,production_companies,production_countries,release_date,revenue,runtime,spoken_languages,status,tagline,title,video,vote_average,vote_count
        //     False,"{'id': 10194, 'name': 'Toy Story Collection', 'poster_path': '/7G9915LfUQ2lVfwMEEhDsn3kT4B.jpg', 'backdrop_path': '/9FBwqcd9IRruEDUrTdcaafOMKUq.jpg'}",30000000,"[{'id': 16, 'name': 'Animation'}, {'id': 35, 'name': 'Comedy'}, {'id': 10751, 'name': 'Family'}]",http://toystory.disney.com/toy-story,862,tt0114709,en,Toy Story,"Led by Woody, Andy's toys live happily in his room until Andy's birthday brings Buzz Lightyear onto the scene. Afraid of losing his place in Andy's heart, Woody plots against Buzz. But when circumstances separate Buzz and Woody from their owner, the duo eventually learns to put aside their differences.",21.946943,/rhIRbceoE9lR4veEXuwCC2wARtG.jpg,"[{'name': 'Pixar Animation Studios', 'id': 3}]","[{'iso_3166_1': 'US', 'name': 'United States of America'}]",1995-10-30,373554033,81.0,"[{'iso_639_1': 'en', 'name': 'English'}]",Released,,Toy Story,False,7.7,5415

        var path = Directory.GetCurrentDirectory();

        var movies = DataFrame.LoadCsv(path + "/Services/Recommendations/Data/movies_metadata.csv");
        
        string[] columnsMoviesToRemove = {"belongs_to_collection", "homepage", "imdb_id", "poster_path", "status", "title", "video"};
        
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
        
        // Merge DataFrames
        
        var df = movies.Merge(keywords, new[] { "id" }, new[] { "id" }, joinAlgorithm: JoinAlgorithm.Inner);
        df.Columns.Remove("id_right");

        df.Columns.RenameColumn("id_left", "id");
        
        var ratingsDf = DataFrame.LoadCsv(path + "/Services/Recommendations/Data/ratings_small.csv");
        
        convertedIds.Clear();
        convertedIds.AddRange(from object? item in ratingsDf["movieId"] select Convert.ToInt64(item));
        
        ratingsDf.Columns.Remove("movieId");
        ratingsDf.Columns.Add(new PrimitiveDataFrameColumn<long>("movieId", convertedIds));
        // Assuming the timestamp is a Unix timestamp
        var timestamps = ratingsDf["timestamp"].Cast<float>();
        var dates = timestamps.Select(timestamp => DateTimeOffset.FromUnixTimeSeconds((long)timestamp).DateTime).ToList();
        
        // Remove the old timestamp column
        ratingsDf.Columns.Remove("timestamp");
        
        // Add the new date column
        ratingsDf.Columns.Add(new PrimitiveDataFrameColumn<DateTime>("date", dates));
        
        // Merge DataFrames
        
        // ratings_df = ratings_df.merge(df[['id', 'original_title', 'genres', 'overview']], left_on='movieId',right_on='id', how='left')
        // ratings_df = ratings_df[~ratings_df['id'].isna()]
        
        // formatDf = df[['id', 'original_title', 'genres', 'overview']];
        var selectedColumns = new List<DataFrameColumn>
        {
            df["id"],
            df["original_title"],
            df["genres"],
            df["overview"]
        };

        // Create a new DataFrame with only the selected columns
        var formatDf = new DataFrame(selectedColumns);
        
        // Merge the ratings DataFrame with the format DataFrame
        ratingsDf = ratingsDf.Merge(formatDf, new[] { "movieId" }, new[] { "id" }, joinAlgorithm: JoinAlgorithm.Inner);
        ratingsDf = ratingsDf.DropNulls();
        ratingsDf.Columns.Remove("id");
        
        Console.WriteLine("huhu");
        
        // Movies DataFrame
        selectedColumns = new List<DataFrameColumn>
        {
            df["id"],
            df["original_title"]
        };
        
        var moviesDf = new DataFrame(selectedColumns);
        
        moviesDf.Columns.RenameColumn("id", "movieId");
        
        // var userIdTensor = tf.convert_to_tensor(ratingsDf.Columns["userId"].Cast<string>().ToArray());
    
        convertedIds.Clear();
        convertedIds.AddRange(from object? item in ratingsDf["userId"] select Convert.ToInt64(item));
        
        ratingsDf.Columns.Remove("userId");
        
        ratingsDf.Columns.Add(new PrimitiveDataFrameColumn<long>("userId", convertedIds));

        var userIdTensor = tf.convert_to_tensor(ratingsDf.Columns["userId"].Cast<long>().ToArray());
        // Convert original_title to string and create a tensor
        var originalTitleTensor = tf.convert_to_tensor(ratingsDf.Columns["original_title"].Cast<string>().ToArray());

        // Convert rating to float and create a tensor
        var ratingTensor = tf.convert_to_tensor(ratingsDf.Columns["rating"].Cast<float>().ToArray());

        // Now, you can create an NDArray with these tensors
        var featuresArray = np.array(new [] { userIdTensor, originalTitleTensor, ratingTensor });
        
        var ratingsDataset = tf.data.Dataset.from_tensor_slices(featuresArray);
        Console.WriteLine("haha");
        var moviesDataset = tf.data.Dataset.from_tensor_slices(tf.convert_to_tensor(moviesDf["original_title"].Cast<string>().ToArray()));
        
        // ratings = ratings.map(lambda x: {
        //     "original_title": x["original_title"],
        //     "userId": x["userId"],
        //     "rating": float(x["rating"])
        // })
        
        Console.WriteLine("Total Data: {0}", ratingsDataset.cardinality());
        Console.WriteLine("Unique Movies: {0}", moviesDataset.cardinality());
    }
}