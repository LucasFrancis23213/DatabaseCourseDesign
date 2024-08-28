using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Utilities;
using System.Data;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class SystemLogsDAL: BaseLogDAL<System_Logs>
    {
        protected override string TableName => "SYSTEM_LOGS";

        protected override List<string> ColumnNames => ["OPERATION_TYPE", "OPERATION_DETAILS", "USER_ID"];

        protected override Func<OracleDataReader, System_Logs> MapFromReader => reader => new System_Logs
        {
            User_ID = reader.GetInt32(0),
            Operation_Type = reader.GetString(1),
            Operation_Details = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty,
            System_Log_ID = reader.GetInt32(3)
        };

        public Tuple<bool, string> GetSystemLogs(QuerySystemLogsArgs args)
        {
            var parameters = new Dictionary<(string ColumnName, string Operator), object>
            {
                { ("SYSTEM_LOG_ID", "="), args.SystemLogID },
                { ("OPERATION_TYPE", "="), args.OperationType },
                { ("USER_ID", "="), args.UserID }
            };

            return GetLogsAux(parameters);
        }

        public Tuple<bool, string> InsertNewLog(SystemLogsInsertUtil args)
        {
            var values = new Dictionary<string, object>
            {
                { "OPERATION_TYPE", args.OperationType },
                { "OPERATION_DETAILS", args.OperationDetails },
                { "USER_ID", args.UserID }
            };
            return InsertNewLogAux(values);
        }
    }
}