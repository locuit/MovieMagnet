using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos.Movies;

namespace MovieMagnet.Services.Dtos.Utils;

public interface IUtilsService
{
    public Task<string> GetPosterPath(string imdbId);

    public Task<List<Movie>> SavePosterPathToDb(List<Movie> movies);
    public Task<Movie> SavePosterPathToDb(Movie movies);

    public List<MovieDto> MapToMovieDto(List<Movie> movies);
    public MovieDto MapToMovieDto(Movie movie);
}
