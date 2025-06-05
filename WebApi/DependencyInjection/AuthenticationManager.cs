using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Model.Entities.Sql.DataBase;
using Model.Extensions;
using Shared.Interfaces;
using Shared.Param;
using Shared.Result;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Interfaces;

namespace WebApi.DependencyInjection
{
    public class AuthenticationManager : IAuthenticationManager
    {
        IConfiguration _configuration;
        DatabaseContext _dbContext;
        public AuthenticationManager(IConfiguration configuration, DatabaseContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }
        public async Task<LoginResult> GetLoginAsync(LoginParam param)
        {
            param.Password = param.Password.GetPasswordHash();
            var query = await _dbContext.Users
                    .Include(p => p.UserRoles)
                        .ThenInclude(p => p.Role)
                    .FirstOrDefaultAsync(p => p.Username == param.Username && p.Password == param.Password);
            if (query != null)
            {
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, param.Username));

                foreach (var role in query.UserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Role.Name!));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Security:SecretKey"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return new LoginResult
                {
                    Token = tokenString,
                    ExpireAt = token.ValidTo
                };
            }
            else
            {
                return new LoginResult
                {
                    ResultCode = (int)Shared.Types.ResultType.Unauthorized,
                    Message = "El nombre de usuario o contraseña pueden ser incorrectas"
                };
            }

        }
    }
}
