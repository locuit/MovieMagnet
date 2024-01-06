using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class MovieKeyword : Entity<long>
{
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long KeywordId { get; set; }
    public Keyword Keyword { get; set; }
}