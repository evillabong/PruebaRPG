using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Entities.Sql.DataBase;
using Model.Interfaces;
using Model.Type;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Managers
{
    public class SupervisorManager : ISupervisorManager
    {
        DatabaseContext _dbContext;
        ILogger<SupervisorManager> _logger;
        IUserContext _userContext;
        public SupervisorManager(DatabaseContext dbContext, ILogger<SupervisorManager> logger, IUserContext userContext)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userContext = userContext;
        }

        public async Task<ModelResult> ApproveRequest(
            int requestId,
            int status,
            string? comment = null)
        {
            try
            {
                var helper = new ModelHelper(_dbContext, ProcedureType.RequestManager, OperationType.Update, 0);
                var ret = await helper.ExecuteNonQueryAsync(new Dictionary<string, object?>
                {
                    ["@Username"] = _userContext.GetUsername(),
                    ["@RequestId"] = requestId,
                    ["@Status"] = status,
                    ["@Comment"] = comment
                });

                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, nameof(ApproveRequest));
                return new ModelResult()
                {
                    ResultCode = (int)ModelResultType.InternalError,
                    Message = "Fatal error."
                };
            }
        }

        public async Task<ModelResult> DeleteRequest(int requestId)
        {
            try
            {
                var helper = new ModelHelper(_dbContext, ProcedureType.RequestManager, OperationType.Delete, 0);
                var ret = await helper.ExecuteNonQueryAsync(new Dictionary<string, object?>
                {
                    ["@Username"] = _userContext.GetUsername(),
                    ["@RequestId"] = requestId,
                });

                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, nameof(DeleteRequest));
                return new ModelResult()
                {
                    ResultCode = (int)ModelResultType.InternalError,
                    Message = "Fatal error."
                };
            }
        }

        public async Task<ModelResult<Request>> PendingRequest()
        {
            try
            {
                var helper = new ModelHelper(_dbContext, ProcedureType.RequestManager, OperationType.Read, 0);
                var query = await helper.ExecuteAsync<Request>(new Dictionary<string, object?>
                {
                    ["@Status"] = 0
                });
                return query;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, nameof(PendingRequest));
                return new ModelResult<Request>()
                {
                    ResultCode = (int)ModelResultType.InternalError,
                    Message = "Fatal error."
                };
            }
        }

        public async Task<ModelResult<Audit>> ViewApprovedHistory()
        {
            try
            {
                var helper = new ModelHelper(_dbContext, ProcedureType.RequestManager, OperationType.Read, 2);
                var query = await helper.ExecuteAsync<Audit>(new Dictionary<string, object?>
                {
                    //["@Username"] = _userContext.GetUsername()
                });
                return query;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, nameof(ViewApprovedHistory));
                return new ModelResult<Audit>()
                {
                    ResultCode = (int)ModelResultType.InternalError,
                    Message = "Fatal error."
                };
            }
        }
    }
}
