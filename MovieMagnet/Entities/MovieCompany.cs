using MovieMagnet.Movies;
using MovieMagnet.ProductionCompanies;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.MovieCompanies;

public class MovieCompany : AuditedAggregateRoot<long>
{
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long CompanyId { get; set; }
    public ProductionCompany Company { get; set; }
}