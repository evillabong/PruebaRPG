using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.Security;

namespace WebApi.Interfaces
{
    public interface IJwtService
    {
        JwtResult CreateJwtToken(List<Claim>? claims = null);
        JwtSecurityToken ValidateJwtToken(string token);
    }
}
