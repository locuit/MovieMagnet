using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.Users;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace MovieMagnet.Services.Users
{
    public class UserService : MovieMagnetAppService, IUserService
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly TokenService _tokenService;

        public UserService(IRepository<User, long> userRepository, IPasswordService passwordService, TokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<LoginUserDto> LoginAsync(LoginUserDto input)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Username == input.Username);
            if (user == null)
            {
                throw new UserFriendlyException("User not found");
            }

            if (!await _passwordService.ComparePasswordAsync(user.Password, input.Password))
            {
                throw new UserFriendlyException("Wrong password");
            }

            var token = _tokenService.GenerateJwtToken(user);
            return new LoginUserDto
            {
                Username = user.Username,
                Token = token
            };
        }

        public async Task LogoutAsync()
        {
            await Task.CompletedTask;
        }

        public async Task<RegisterUserDto> RegisterAsync(RegisterUserDto input)
        {
            Console.WriteLine(input);
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Username == input.Username);
            if (user != null)
            {
                throw new UserFriendlyException("Username already exists");
            }

            var hashedPassword = await _passwordService.HashPasswordAsync(input.Password);

            var newUser = new User
            {
                Username = input.Username,
                Fullname = input.Fullname,
                Email = input.Email,
                Password = hashedPassword,
                Avatar = input.Avatar
            };
            newUser = await _userRepository.InsertAsync(newUser);
            var userDto = new RegisterUserDto
            {
                Username = newUser.Username,
                Fullname = newUser.Fullname,
                Email = newUser.Email,
                Avatar = newUser.Avatar
            };
            return userDto;
        }
    }
}
