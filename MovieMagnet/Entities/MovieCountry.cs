using MovieMagnet.Movies;
using MovieMagnet.ProductionCountries;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.MovieCountries;

public class MovieCountry : AuditedAggregateRoot<long>
{
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long CountryId { get; set; }
    public ProductionCountry Country { get; set; }
}