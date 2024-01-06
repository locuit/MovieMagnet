using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class Keyword : Entity<long>
{
    public string Name { get; set; }
    
    public virtual ICollection<MovieKeyword> MovieKeywords { get; set; }
}