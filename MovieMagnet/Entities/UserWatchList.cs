using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class UserWatchList : Entity<long>
{
    public long MovieId { get; set; }
    public long UserId { get; set; }
    public User User { get; set; }
    public Movie Movie { get; set; }
}