using System;
using System.Collections.Generic;
using MovieMagnet.MovieGenres;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.Genres;


public class Genre : Entity<long>
{
    public string Name { get; set; }
    
    public virtual ICollection<MovieGenre> MovieGenres { get; set; }
}