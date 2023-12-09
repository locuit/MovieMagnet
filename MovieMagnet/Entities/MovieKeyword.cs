using MovieMagnet.Keywords;
using MovieMagnet.Movies;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.MovieKeywords;

public class MovieKeyword : AuditedAggregateRoot<long>
{
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public long KeywordId { get; set; }
    public Keyword Keyword { get; set; }
}