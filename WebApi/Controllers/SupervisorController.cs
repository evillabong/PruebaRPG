using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Model.Interfaces;
using Model.Managers;
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
        public ISupervisorManager _supervisorManager;

        public SupervisorController(ISupervisorManager supervisorManager)
        {
            _supervisorManager = supervisorManager;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleType.Supervisor))]
        [HttpGet(nameof(Shared.WebMethods.Supervisor.PendingRequest))]
        public async Task<PendingRequestResult> PendingRequest()
        {
            var result = new PendingRequestResult();
            var model = await _supervisorManager.PendingRequest();

            result.SetResult(model.ResultCode, model.Message);

            if (model.IsSuccess())
            {
                if (model.Data.Count > 0)
                {
                    result.PendingRequest = model.Data.Select(p => new Shared.Base.RequestBase
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
            return result;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleType.Supervisor))]
        [HttpPost($"{nameof(Shared.WebMethods.Supervisor.ApprovedRequest)}")]
        public async Task<ApprovedRequestResult> ApprovedRequest([FromBody] ApprovedRequestParam param)
        {
            var result = new ApprovedRequestResult();

            var model = await _supervisorManager.ApproveRequest(param.RequestId, param.Status, param.Comment);
            result.SetResult(model.ResultCode, model.Message);

            return result;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleType.Supervisor))]
        [HttpPost($"{nameof(Shared.WebMethods.Supervisor.DeleteRequest)}")]
        public async Task<DeleteRequestResult> DeleteRequest([FromBody] DeleteRequestParam param)
        {
            var result = new DeleteRequestResult();

            var model = await _supervisorManager.DeleteRequest(param.RequestId);
            result.SetResult(model.ResultCode, model.Message);

            return result;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleType.Supervisor))]
        [HttpGet($"{nameof(Shared.WebMethods.Supervisor.ViewApprovedHistory)}")]
        public async Task<ViewApprovedHistoryResult> ViewApprovedHistory()
        {
            var result = new ViewApprovedHistoryResult();

            var model = await _supervisorManager.ViewApprovedHistory();
            result.SetResult(model.ResultCode, model.Message);
            if (result.IsSuccess())
            {
                if (model.Data.Count > 0)
                {
                    result.Audits = model.Data.Select(p => new Shared.Base.AuditBase
                    {
                        Id = p.Id,
                        Action = p.Action,
                        Detail = p.Detail,
                        UserId = p.UserId,
                        Username = p.Username,
                        CreatedAt = p.CreatedAt
                    }).ToList();
                }

            }
            return result;
        }
    }
}
