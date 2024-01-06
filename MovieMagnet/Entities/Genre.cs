using Volo.Abp.Domain.Entities;
namespace MovieMagnet.Entities;


public class Genre : Entity<long>
{
    public string Name { get; set; }
    
    public virtual ICollection<MovieGenre> MovieGenres { get; set; }
}