using MovieMagnet.Movies;
using MovieMagnet.ProductionCompanies;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.MovieCompanies;

public class MovieCompany : Entity<long>
{
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long CompanyId { get; set; }
    public ProductionCompany Company { get; set; }
}