using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class Genre : Entity<long>
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    
    public virtual ICollection<MovieGenre> MovieGenres { get; set; }
}