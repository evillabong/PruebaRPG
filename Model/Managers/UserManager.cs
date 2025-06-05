using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
using static Azure.Core.HttpHeader;

namespace Model.Managers
{
    public class UserManager : IUserManager
    {
        DatabaseContext _dbContext;
        ILogger _logger;
        IUserContext _userContext;
        public UserManager(DatabaseContext dbContext, ILogger logger, IUserContext userContext)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userContext = userContext;
        }

        public async Task<ModelResult> CreateRequest(
            string description,
            double amount,
            DateTime awaitedAt,
            string? comment = null)
        {
            var ret = new ModelResult();
            try
            {
                var helper = new ModelHelper(ProcedureType.RequestManager, OperationType.Create, 0);
                await helper.ExecuteNonQueryAsync(new Dictionary<string, object?>
                {
                    ["@Username"] = _userContext.GetUsername(),
                    ["@Description"] = description,
                    ["@Amount"] = amount,
                    ["@AwaitedAt"] = awaitedAt,                    
                    ["@Comment"] = comment
                });

                return ret;
            }
            catch (Exception ex)
            {
                ret.ResultCode = (int)ModelResultType.InternalError;
                ret.Message = "Fatal error.";
                _logger.LogInformation(ex, nameof(CreateRequest));
                return ret;
            }
        }

        public async Task<ModelResult<Request>> MyRequests()
        {
            try
            {
                var helper = new ModelHelper(ProcedureType.RequestManager, OperationType.Read, 0);
                var query = await helper.ExecuteAsync<Request>(new Dictionary<string, object?>
                {
                    ["@Username"] = _userContext.GetUsername()
                });
                return query;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, nameof(MyRequests));
                return new ModelResult<Request>()
                {
                    ResultCode = (int)ModelResultType.InternalError,
                    Message = "Fatal error."
                };
            }
        }
    }
}
