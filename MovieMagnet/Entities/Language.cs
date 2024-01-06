using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace MovieMagnet.Entities;

public class Language : Entity<long>
{
    public string Name { get; set; }
    public string Code { get; set; }
}