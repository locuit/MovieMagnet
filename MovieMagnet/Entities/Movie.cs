using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class Movie : Entity<long>
{
    public decimal Budget { get; set; }
    public required string Title { get; set; } 
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
    
    public virtual ICollection<MovieKeyword> MovieKeywords { get; set; }
    public virtual ICollection<MovieCompany> MovieCompanies { get; set; }
    public virtual ICollection<MovieCountry> MovieCountries { get; set; }
    public virtual ICollection<MovieGenre> MovieGenres { get; set; }
    
    public virtual ICollection<Rating> Ratings { get; set; }

    public virtual ICollection<UserWatchList> UserWatchList { get; set; }
    
    public virtual ICollection<MovieWatching> MovieWatchings { get; set; }

}