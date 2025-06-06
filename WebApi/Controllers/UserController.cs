using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Interfaces;
using Shared.Param.User;
using Shared.Result.User;
using Shared.Types;

namespace WebApi.Controllers
{
    [EnableCors(PolicyName = "CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        public IUserManager _userManager;

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost(nameof(Shared.WebMethods.User.CreateRequest))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleType.User))]
        public async Task<CreateRequestResult> CreateRequest([FromBody] CreateRequestParam param)
        {
            var result = await _userManager.CreateRequest(
                param.Description,
                param.Amount,
                param.AwaitedAt,
                param.Comment);

            return new CreateRequestResult
            {
                ResultCode = result.ResultCode,
                Message = result.Message,
            };
        }

        [HttpGet($"{nameof(Shared.WebMethods.User.MyRequests)}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleType.User))]
        public async Task<MyRequestResult> MyRequests()
        {
            var ret = new MyRequestResult();

            var model = await _userManager.MyRequests();
            ret.ResultCode = model.ResultCode;
            ret.Message = model.Message;
            if (model.IsSuccess())
            {
                if (model.Data.Count > 0)
                {
                    ret.Requests = model.Data.Select(p => new Shared.Base.RequestBase
                    {
                        Id = p.Id,
                        UserId = p.UserId,
                        Amount = p.Amount,
                        Comment = p.Comment,
                        AwaitedAt = p.AwaitedAt,
                        CreatedAt = p.CreatedAt,
                        Description = p.Description,
                        UserName = p.Username,
                        ResponsedAt = p.ResponsedAt,
                        Status = p.Status
                    }).ToList();
                }
            }
            return ret;
        }
    }
}
