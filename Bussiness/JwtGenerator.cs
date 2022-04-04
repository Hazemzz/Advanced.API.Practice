using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Business
{
    public class JwtGenerator
    {
        private readonly SymmetricSecurityKey _key;
        public JwtGenerator(IConfiguration configuration)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["token"]));
        }

        public ClaimsPrincipal DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler()
                .ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = _key,
                    SaveSigninToken = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out var securityToken);

            return tokenHandler;
        }

        public async Task<string> Generate(string userName)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName)
            };

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            var descriptor = new SecurityTokenDescriptor()
            {
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = credentials,
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.UtcNow.AddMinutes(30)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}
