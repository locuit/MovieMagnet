using System;
using System.Collections.Generic;
using MovieMagnet.MovieCompanies;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.ProductionCompanies;

public class ProductionCompany : AuditedAggregateRoot<long>
{
    public string Name { get; set; }
    
    public virtual ICollection<MovieCompany> MovieCompanies { get; set; }
}