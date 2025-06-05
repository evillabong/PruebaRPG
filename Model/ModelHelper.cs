using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Model.Entities;
using Model.Entities.Sql.DataBase;
using Model.Interfaces;
using Model.Type;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ModelHelper
    {
        public ProcedureType Procedure { get; set; }
        public OperationType OperationType { get; set; }
        public int Mode { get; set; }

        public ModelHelper(ProcedureType procedure, OperationType operationType, int mode)
        {
            Procedure = procedure;
            OperationType = operationType;
            Mode = mode;
        }

        public async Task<ModelResult<T>> ExecuteAsync<T>(Dictionary<string, object?> databaseParams) where T : class
        {
            using var dbContext = new DatabaseContext();

            var ret = new ModelResult<T>();

            var resultCodeParam = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var messageParam = new SqlParameter("@Message", SqlDbType.VarChar, 150) { Direction = ParameterDirection.Output };

            var sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@Operation", OperationType), // "C", "R", "U", "D"
                new SqlParameter("@Mode", Mode)
            };

            if (databaseParams.Count > 0)
            {
                foreach (var entry in databaseParams)
                {
                    sqlParams.Add(new SqlParameter(entry.Key, entry.Value ?? DBNull.Value));
                }
            }
            sqlParams.Add(resultCodeParam);
            sqlParams.Add(messageParam);

            string paramList = string.Join(", ", sqlParams.Select(p => $"{p.ParameterName}{(p.Direction == ParameterDirection.Output ? " OUT" : "")}"));
            string sql = $"EXEC {Procedure} {paramList}";

            var data = await dbContext.Set<T>().FromSqlRaw(sql, sqlParams.ToArray()).ToListAsync();

            ret.ResultCode = (int)resultCodeParam.Value;
            ret.Message = (string)messageParam.Value;
            if (ret.IsSuccess())
                ret.Data = data;

            return ret;
        }
        public async Task<ModelResult> ExecuteNonQueryAsync(Dictionary<string, object?> databaseParams)
        {
            var ret = new ModelResult();
            var query = await ExecuteAsync<EmptyEntity>(databaseParams);
            ret.ResultCode = query.ResultCode;
            ret.Message = query.Message;
            return ret;

        }
    }

}
