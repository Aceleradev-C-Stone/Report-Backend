using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Report.Domain.Models;
using Report.Infra.Extensions;

namespace Report.Infra.Services
{
    public class TokenService : ITokenService
    {
        private const int SECONDS_TO_EXPIRE = 900;

        private static IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(
                _config.GetSection("Security").GetSection("TokenSecret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.GetName()),
                }),
                Expires = DateTime.UtcNow.AddSeconds(SECONDS_TO_EXPIRE),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int GetExpirationInSeconds() => SECONDS_TO_EXPIRE;
        public int GetExpirationInMinutes() => SECONDS_TO_EXPIRE / 60;
        public int GetExpirationInHours() => SECONDS_TO_EXPIRE / (60 * 60);
    }
}