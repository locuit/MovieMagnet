using MovieMagnet.Services.Dtos.Users;

namespace MovieMagnet.Services.Users
{
    public class PasswordService :MovieMagnetAppService, IPasswordService
    {
        public async Task<string> HashPasswordAsync(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);

        }

        public async Task<bool> ComparePasswordAsync(string hashedPassword, string plainPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}