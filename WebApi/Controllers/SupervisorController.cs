using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Model.Interfaces;
using Shared.Param.Supervisor;
using Shared.Param.User;
using Shared.Result.Supervisor;
using Shared.Result.User;
using Shared.Types;

namespace WebApi.Controllers
{
    [EnableCors(PolicyName = "CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class SupervisorController
    {
        public IUserManager _userManager;

        public SupervisorController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = nameof(RoleType.Supervisor))]
        [HttpPost(nameof(Shared.WebMethods.Supervisor.PendingRequest))]
        public async Task<PendingRequestResult> PendingRequest()
        {
            var result = await _userManager.CreateRequest(
                dto.Description,
                dto.Amount,
                dto.AwaitedAt,
                dto.Comment);

            return new CreateRequestResult
            {
                ResultCode = result.ResultCode,
                Message = result.Message,
            };
        }

        [Authorize(Roles = nameof(RoleType.Supervisor))]
        [HttpGet($"{nameof(Shared.WebMethods.User.MyRequests)}")]
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
