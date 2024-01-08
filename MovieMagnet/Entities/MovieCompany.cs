using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class MovieCompany : Entity<long>
{
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long CompanyId { get; set; }
    public ProductionCompany Company { get; set; }
}