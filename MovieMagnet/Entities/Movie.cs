using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class Movie : Entity<long>
{
    public float Budget { get; set; }
    
    [Required]
    [MaxLength(255)]
    public required string Title { get; set; } 
    
    [Required]
    [MaxLength(255)]
    public required string Language { get; set; }
    public string? Overview { get; set; }
    public string? PosterPath { get; set; }
    public DateTime ReleaseDate { get; set; }
    
    [Required]
    [MaxLength(255)]
    public required string ImdbId { get; set; }
    public float Popularity { get; set; }
    public float Revenue { get; set; }
    public float Runtime { get; set; }
    public float VoteAverage { get; set; }
    public int VoteCount { get; set; }
    
    public virtual ICollection<MovieKeyword> MovieKeywords { get; set; }
    public virtual ICollection<MovieCompany> MovieCompanies { get; set; }
    public virtual ICollection<MovieCountry> MovieCountries { get; set; }
    public virtual ICollection<MovieGenre> MovieGenres { get; set; }
}