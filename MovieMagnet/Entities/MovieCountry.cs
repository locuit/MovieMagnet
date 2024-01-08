using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class MovieCountry : Entity<long>
{
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long CountryId { get; set; }
    public ProductionCountry Country { get; set; }
}