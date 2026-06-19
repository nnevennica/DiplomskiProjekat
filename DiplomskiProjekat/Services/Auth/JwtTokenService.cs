using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiplomskiProjekat.Models.Auth;
using Microsoft.IdentityModel.Tokens;

namespace DiplomskiProjekat.Services.Auth
{
    public class JwtTokenService : IJwtTokenService
    {

        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(User user)
        {
            var key = _config["Jwt:Key"] ?? throw new Exception("Missing Jwt:Key");
            var issuer = _config["Jwt:Issuer"] ?? "MeteoApi";
            var audience = _config["Jwt:Audience"] ?? "MeteoClient";
            var expiresMinutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 60;

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("city", user.City)
        };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
