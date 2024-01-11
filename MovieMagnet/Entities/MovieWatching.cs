using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class MovieWatching : Entity<long>
{
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long UserId { get; set; }
    public User User { get; set; }
    public string lastViewMoment { get; set; }
}