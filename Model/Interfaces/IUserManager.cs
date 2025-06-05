using Model.Entities.Sql.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IUserManager
    {
        Task<ModelResult> CreateRequest(string description, double amount, DateTime awaitedAt, string? comment = null);
        Task<ModelResult<Request>> MyRequests();
    }
}
