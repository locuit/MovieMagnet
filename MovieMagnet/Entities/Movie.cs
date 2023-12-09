using System;
using System.Collections;
using System.Collections.Generic;
using MovieMagnet.MovieCompanies;
using MovieMagnet.MovieCountries;
using MovieMagnet.MovieGenres;
using MovieMagnet.MovieKeywords;
using MovieMagnet.ProductionCompanies;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.Movies;

public class Movie : AuditedAggregateRoot<long>
{
    public decimal Budget { get; set; }
    public required string Title { get; set; } 
    public required string Language { get; set; }
    public string? Overview { get; set; }
    public string? PosterPath { get; set; }
    public DateTime ReleaseDate { get; set; }
    public required string ImdbId { get; set; }
    public decimal Popularity { get; set; }
    public decimal Revenue { get; set; }
    public decimal Runtime { get; set; }
    public decimal VoteAverage { get; set; }
    public int VoteCount { get; set; }
    
    public virtual ICollection<MovieKeyword> MovieKeywords { get; set; }
    public virtual ICollection<MovieCompany> MovieCompanies { get; set; }
    public virtual ICollection<MovieCountry> MovieCountries { get; set; }
    public virtual ICollection<MovieGenre> MovieGenres { get; set; }
}