using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MovieMagnet.Entities;
using MovieMagnet.Services;

namespace MovieMagnet.Services.Users;
    public class TokenService : MovieMagnetAppService
    {
        private readonly string _secretKey;
        private readonly string _issuer;

        public TokenService(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:Key"];
            _issuer = configuration["Jwt:Issuer"];

            if (string.IsNullOrEmpty(_secretKey) || string.IsNullOrEmpty(_issuer))
            {
                throw new ApplicationException("JWT configuration not found.");
            }
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }