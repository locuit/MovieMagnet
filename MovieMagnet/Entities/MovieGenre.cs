using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class MovieGenre : Entity<long>
{ 
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long GenreId { get; set; }
    public Genre Genre { get; set; }
}