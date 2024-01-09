using Volo.Abp.Application.Services;

namespace MovieMagnet.Services.Dtos.Users;

public interface IUserService : IApplicationService
{
    Task<LoginUserDto> LoginAsync(LoginUserDto input);
    Task LogoutAsync();
    Task<RegisterUserDto> RegisterAsync(RegisterUserDto input);
}