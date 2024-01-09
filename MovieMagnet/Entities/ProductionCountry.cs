using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class ProductionCountry : Entity<long>
{
    [Required]
    [MaxLength(255)]
    public required string Name { get; set; }
    
    public virtual ICollection<MovieCountry> MovieCountries { get; set; }
}