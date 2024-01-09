using Volo.Abp.Application.Dtos;

namespace MovieMagnet.Services.Dtos.Movies;

public class MovieDto : EntityDto<long>
{
    public float Budget { get; set; }
    public string Title { get; set; } = null!;
    public string? Language { get; set; }
    public string? Overview { get; set; }
    public string? PosterPath { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? ImdbId { get; set; }
    public decimal Popularity { get; set; }
    public decimal Revenue { get; set; }
    public decimal Runtime { get; set; }
    public decimal VoteAverage { get; set; }
    public int VoteCount { get; set; }
}