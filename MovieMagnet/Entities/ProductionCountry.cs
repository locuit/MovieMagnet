using System;
using System.Collections.Generic;
using MovieMagnet.MovieCountries;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.ProductionCountries;

public class ProductionCountry : AuditedAggregateRoot<long>
{
    public string Name { get; set; } 
    
    public virtual ICollection<MovieCountry> MovieCountries { get; set; }
}