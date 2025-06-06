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
        public OperationType OperationValue { get; set; }
        public int Mode { get; set; }
        public DatabaseContext _databaseContext;

        public ModelHelper(DatabaseContext databaseContext, ProcedureType procedure, OperationType operationType, int mode)
        {
            Procedure = procedure;
            OperationValue = operationType;
            Mode = mode;
            _databaseContext = databaseContext;
        }

        public async Task<ModelResult<T>> ExecuteAsync<T>(Dictionary<string, object?> databaseParams) where T : class, new()
        {
            var ret = new ModelResult<T>();

            using var command = _databaseContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = Procedure.ToString();
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Operation", SqlDbType.NVarChar, 1) { Value = ((char)OperationValue).ToString() });
            command.Parameters.Add(new SqlParameter("@Mode", SqlDbType.Int) { Value = Mode });

            if (databaseParams != null)
            {
                foreach (var param in databaseParams)
                {
                    command.Parameters.Add(new SqlParameter(param.Key, param.Value ?? DBNull.Value));
                }
            }

            var resultCodeParam = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var messageParam = new SqlParameter("@Message", SqlDbType.VarChar, 150) { Direction = ParameterDirection.Output };
            command.Parameters.Add(resultCodeParam);
            command.Parameters.Add(messageParam);

            var returnValue = new SqlParameter("@return_value", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            command.Parameters.Add(returnValue);

            await _databaseContext.Database.OpenConnectionAsync();

            var resultList = new List<T>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var instance = new T();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var prop = typeof(T).GetProperty(reader.GetName(i));
                        if (prop != null && !await reader.IsDBNullAsync(i))
                        {
                            prop.SetValue(instance, reader.GetValue(i));
                        }
                    }
                    resultList.Add(instance);
                }
            }

            ret.ResultCode = (int)(resultCodeParam.Value ?? -1);
            ret.Message = messageParam.Value?.ToString() ?? "";
            if (ret.IsSuccess())
                ret.Data = resultList;

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
