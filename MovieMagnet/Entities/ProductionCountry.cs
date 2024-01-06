using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class ProductionCountry : Entity<long>
{
    public string Name { get; set; } 
    
    public virtual ICollection<MovieCountry> MovieCountries { get; set; }
}