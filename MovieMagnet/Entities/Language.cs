using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.Languages;

public class Language : AuditedAggregateRoot<long>
{
    public string Name { get; set; }
    public string Code { get; set; }
}