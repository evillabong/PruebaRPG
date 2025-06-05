using Model.Entities.Sql.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface ISupervisorManager
    {
        Task<ModelResult> ApproveRequest(int requestId, int status, double amount, DateTime awaitedAt, string? comment = null);
        Task<ModelResult<Request>> PendingRequest();
        Task<ModelResult> DeleteRequest(int requestId);
        Task<ModelResult<Audit>> ViewApprovedHistory();
    }
}
