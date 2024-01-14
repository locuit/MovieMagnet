using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos.Movies;
using MovieMagnet.Services.Dtos.Utils;
using Newtonsoft.Json;

namespace MovieMagnet.Services.Utils;

public class Json
{
    public string Poster { get; set; }
}

public class UtilsService : IUtilsService
{
    private HttpClient _httpClient;

    public UtilsService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<string> GetPosterPath(string imdbId)
    {
        var res = await _httpClient.GetAsync($"https://www.omdbapi.com/?i={imdbId}&apikey=75343f68");

        var jsonString = await res.Content.ReadAsStringAsync();
        // Store the poster path in the dictionary using IMDb ID as the key
        var json = JsonConvert.DeserializeObject<Json>(jsonString);

        if (json != null)
        {
            return json.Poster;
        }

        return "";
    }

    public async Task<List<Movie>> SavePosterPathToDb(List<Movie> movies)
    {
        var listTask = new List<Task>();
        var moviePosters = new Dictionary<string, string>();

        foreach (var movie in movies)
        {
            if (movie.PosterPath != null && !movie.PosterPath.Contains("https"))
            {
                listTask.Add(GetPosterPath(movie.ImdbId!)
                .ContinueWith(posterTask =>
                {
                    // Ensure the posterTask is completed successfully
                    if (posterTask.Status == TaskStatus.RanToCompletion)
                    {
                        moviePosters[movie.ImdbId!] = posterTask.Result;
                    }
                }));
            }
        }

        await Task.WhenAll(listTask);

        // Map the poster paths back to the original movies
        foreach (var movie in movies)
        {
            // Retrieve the poster path from the dictionary using IMDb ID
            if (moviePosters.TryGetValue(movie.ImdbId!, out var posterPath))
            {
                // Assign the poster path to the original movie
                movie.PosterPath = posterPath;
            }
        }

        return movies;
    }

    public async Task<Movie> SavePosterPathToDb(Movie movie)
    {
        if (movie.PosterPath != null && !movie.PosterPath.Contains("https"))
        {
            var posterPath = await GetPosterPath(movie.ImdbId!);

            movie.PosterPath = posterPath;
        }

        return movie;
    }

    public List<MovieDto> MapToMovieDto(List<Movie> movies)
    {
        List<MovieDto> moviesDto = new() { };
        movies.ForEach(x =>
        {
            decimal? movieRating = x.Ratings is { Count: > 0 } ? x.Ratings.Average(r => r.Score) : null;
            int movieRatingCount = x.Ratings is { Count: > 0 } ? x.Ratings.Count() : 0;

            moviesDto.Add(new MovieDto()
            {
                Id = x.Id,
                Budget = x.Budget,
                Title = x.Title,
                Language = x.Language,
                Overview = x.Overview,
                PosterPath = x.PosterPath,
                ReleaseDate = x.ReleaseDate,
                ImdbId = x.ImdbId,
                Popularity = x.Popularity,
                Revenue = x.Revenue,
                Runtime = x.Runtime,
                VoteAverage = movieRating,
                VoteCount = movieRatingCount,
                Genres = x.MovieGenres.Select(mg => mg.Genre.Name).ToArray(),
            });
        });
        return moviesDto;
    }
    public MovieDto MapToMovieDto(Movie x)
    {
        decimal? movieRating = x.Ratings is { Count: > 0 } ? x.Ratings.Average(r => r.Score) : null;
        int movieRatingCount = x.Ratings is { Count: > 0 } ? x.Ratings.Count() : 0;

        var moviesDto = new MovieDto()
        {
            Id = x.Id,
            Budget = x.Budget,
            Title = x.Title,
            Language = x.Language,
            Overview = x.Overview,
            PosterPath = x.PosterPath,
            ReleaseDate = x.ReleaseDate,
            ImdbId = x.ImdbId,
            Popularity = x.Popularity,
            Revenue = x.Revenue,
            Runtime = x.Runtime,
            VoteAverage = movieRating,
            VoteCount = movieRatingCount,
            Genres = x.MovieGenres.Select(mg => mg.Genre.Name).ToArray(),
        };

        return moviesDto;
    }
}
