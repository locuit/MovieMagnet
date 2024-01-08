using Volo.Abp.Application.Dtos;

namespace MovieMagnet.Services.Dtos.Movies;

public class MovieDto : EntityDto<long>
{
    public float Budget { get; set; }
    public string Title { get; set; } = null!;
    public string Language { get; set; } = null!;
    public string? Overview { get; set; }
    public string? PosterPath { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string ImdbId { get; set; } = null!;
    public float Popularity { get; set; }
    public float Revenue { get; set; }
    public float Runtime { get; set; }
    public float VoteAverage { get; set; }
    public int VoteCount { get; set; }
}