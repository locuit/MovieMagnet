using MovieMagnet.Entities;
using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class ProductionCompany : Entity<long>
{
    public string Name { get; set; }
    
    public virtual ICollection<MovieCompany> MovieCompanies { get; set; }
}