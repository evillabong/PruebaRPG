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
using WebApi.Security.Jwt;

namespace WebApi.DependencyInjection
{
    public class AuthenticationManager : IAuthenticationManager
    {
        IConfiguration _configuration;
        DatabaseContext _dbContext;
        IJwtService jwtService;
        public AuthenticationManager(IConfiguration configuration, DatabaseContext dbContext, IJwtService jwtService)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            this.jwtService = jwtService;
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
                var jwt = jwtService.CreateJwtToken(claims);

                return new LoginResult
                {
                    Jti = jwt.Jti,
                    Token = jwt.Token,
                    ExpireAt = jwt.Expiration
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
