
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Versioning;
using System.Security.Claims;
using System.Text;
using Model.Extensions;
using WebApi.Interfaces;
using Shared.Types;
using Model.Entities.Sql.DataBase;

namespace WebApi.Security.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration configuration;
        private readonly JwtSecurityTokenHandler tokenHandler;
        public JwtService(IConfiguration configuration)
        {
            this.configuration = configuration;
            tokenHandler = new JwtSecurityTokenHandler();
        }

        public Task Authenticate()
        {
            return Task.CompletedTask;
        }


        public JwtResult CreateJwtToken(List<Claim>? claims = null)
        {
            claims = claims ?? new List<Claim>();
            var jti = Guid.NewGuid().ToString();

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, jti));

            var minutes = int.Parse(configuration["Security:MinutesTimeAlive"]!);
            DateTime expiration = DateTime.UtcNow;
            expiration = expiration.AddMinutes(minutes);

            var secretKey = configuration["Security:SecretKey"]!;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = null,
                Audience = null,
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)), SecurityAlgorithms.HmacSha512),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new JwtResult
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = expiration,
                Jti = jti
            };

        }
        public JwtSecurityToken ValidateJwtToken(string token)
        {
            var secretKey = configuration["Security:SecretKey"]!;

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };
            tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
            if (!(securityToken is JwtSecurityToken jwtToken && jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase)))
            {
                return null!;
            }
            return jwtToken;
        }
    }
}
