using Volo.Abp.Domain.Entities;

namespace MovieMagnet.Entities;

public class User : Entity<long>
{
    public string Username { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Avatar { get; set; }
    
    public virtual ICollection<Rating> Ratings { get; set; }

    public virtual ICollection<UserWatchList> UserWatchList { get; set; }
    
    public virtual ICollection<MovieWatching> MovieWatchings { get; set; }
}