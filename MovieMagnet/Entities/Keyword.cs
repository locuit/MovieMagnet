using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class Keyword : Entity<long>
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    
    public virtual ICollection<MovieKeyword> MovieKeywords { get; set; }
}