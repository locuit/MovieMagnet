using Volo.Abp.Application.Services;

namespace MovieMagnet.Services.Dtos.Users;
public interface IPasswordService : IApplicationService
{
    Task<string> HashPasswordAsync(string plainPassword);
    Task<bool> ComparePasswordAsync(string hashedPassword, string plainPassword);
}