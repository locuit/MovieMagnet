using MovieMagnet.Genres;
using MovieMagnet.Movies;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.MovieGenres;

public class MovieGenre : AuditedAggregateRoot<long>
{ 
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long GenreId { get; set; }
    public Genre Genre { get; set; }
}