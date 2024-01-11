using Tensorflow;
using Tensorflow.Common.Types;
using Tensorflow.Keras;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.Keras.Engine;
using static Tensorflow.Binding;

namespace MovieMagnet.TensorflowModels
{
    public class MovieArgs : ModelArgs
    {
        public int NumUsers;
        public int NumMovies;
        public int EmbeddingDimension;
    }

    public class MovieModel : Model
    {
        private readonly ILayer userEmbedding;
        private readonly ILayer movieEmbedding;
        private readonly ILayer dense1;
        private readonly ILayer dense2;
        private readonly ILayer ratingOutputLayer;
        private readonly ILayer watchOutputLayer;

        public MovieModel(MovieArgs args) : base(args)
        {
            userEmbedding = tf.keras.layers.Embedding(args.NumUsers + 1, args.EmbeddingDimension);
            movieEmbedding = tf.keras.layers.Embedding(args.NumMovies + 1, args.EmbeddingDimension);

            dense1 = tf.keras.layers.Dense(256, activation: "relu");
            dense2 = tf.keras.layers.Dense(128, activation: "relu");
            ratingOutputLayer = tf.keras.layers.Dense(1);
            watchOutputLayer = tf.keras.layers.Dense(1, activation: "sigmoid");
        }
        
        public Tensors Call(Tensors userEmbed, Tensors movieEmbed)
        {
            var userEmbedFlat = tf.keras.layers.Flatten().Apply(userEmbed);
            var movieEmbedFlat = tf.keras.layers.Flatten().Apply(movieEmbed);
            var concat = tf.keras.layers.Concatenate().Apply(new Tensors(userEmbedFlat, movieEmbedFlat));
            var dense1Out = dense1.Apply(concat);
            var dense2Out = dense2.Apply(dense1Out);
            var ratingOut = ratingOutputLayer.Apply(dense2Out);
            var watchOut = watchOutputLayer.Apply(dense2Out);
            
            return (ratingOut, watchOut);
        }
    }
}
