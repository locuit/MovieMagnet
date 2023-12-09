using System;
using System.Collections.Generic;
using MovieMagnet.MovieKeywords;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.Keywords;

public class Keyword : AuditedAggregateRoot<long>
{
    public string Name { get; set; }
    
    public virtual ICollection<MovieKeyword> MovieKeywords { get; set; }
}