using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Shared.Param;
using Shared.Result;
using WebApi.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [EnableCors(PolicyName = "CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        readonly IAuthenticationManager _userManager;

        public SecurityController(IAuthenticationManager userManager)
        {
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost(nameof(Shared.WebMethods.Security.Login))]
        public async Task<LoginResult> GetLoginAsync([FromBody] LoginParam param)
        {
            return await _userManager.GetLoginAsync(param);
        }
    }
}
