using Volo.Abp.Application.Dtos;

namespace MovieMagnet.Services.Dtos;

public class UserDto : EntityDto<long>
{
    public string Username { get; set; } = null!;
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } = null!;
    public string Avatar { get; set; }
}

public class LoginUserDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    
    public string? Token { get; set; }
}

public class RegisterUserDto
{
    public string Username { get; set; } = null!;
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } = null!;
    public string Avatar { get; set; }
}