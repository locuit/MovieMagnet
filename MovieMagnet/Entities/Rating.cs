using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class Rating : Entity<long>
{
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long UserId { get; set; }
    public User User { get; set; }
    public decimal Score { get; set; }
    public string Timestamp { get; set; }
}