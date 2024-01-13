using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class Language : Entity<long>
{
    public string Name { get; set; }
    public string Code { get; set; }
}