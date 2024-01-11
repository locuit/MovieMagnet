using Microsoft.AspNetCore.Mvc;
using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.Users;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using MovieMagnet.Authorization;

namespace MovieMagnet.Services.Users
{
    
    [Authorize]
    public class UserService : MovieMagnetAppService, IUserService
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtUtils _jwtUtils;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserService(IRepository<User, long> userRepository, IPasswordService passwordService, IJwtUtils jwtUtils,IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtUtils = jwtUtils;
            _httpContextAccessor = httpContextAccessor;
        }
        
        [HttpPost("auth/login")]
        [AllowAnonymous]
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

            var token = _jwtUtils.GenerateJwtToken(user);
            return new LoginUserDto
            {
                Username = user.Username,
                Token = token
            };
        }

        [HttpPost("auth/logout")]
        [AllowAnonymous]
        public async Task LogoutAsync()
        {
            await Task.CompletedTask;
        }

        [HttpPost("auth/register")]
        [AllowAnonymous]
        public async Task<RegisterUserDto> RegisterAsync(RegisterUserDto input)
        {
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
        
        [HttpGet("/users/me")]
        [Authorize]
        public async Task<UserDto> GetMe()
        {
            var user = _httpContextAccessor.HttpContext?.Items["User"] as UserDto;
            if (user == null)
            {
                throw new UserFriendlyException("User not found");
            }
            return user;
        }
        
        public async Task<UserDto> GetById(long id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                throw new UserFriendlyException("User not found");
            }

            var userDto = new UserDto
            {
                Username = user.Username,
                Fullname = user.Fullname,
                Email = user.Email,
                Avatar = user.Avatar,
                Id = user.Id
            };
            return userDto;
        }
        
        
    }
}
