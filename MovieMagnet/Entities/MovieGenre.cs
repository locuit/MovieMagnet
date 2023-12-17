using MovieMagnet.Genres;
using MovieMagnet.Movies;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.MovieGenres;

public class MovieGenre : Entity<long>
{ 
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long GenreId { get; set; }
    public Genre Genre { get; set; }
}