using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class APILogsDAL : BaseLogDAL<API_Access_Logs>
    {
        protected override string TableName => "API_ACCESS_LOGS";

        protected override List<string> ColumnNames => ["API_NAME", "ACCESSOR_ID", "ACCESS_TIME", "ACCESS_RESULT"];

        protected override Func<OracleDataReader, API_Access_Logs> MapFromReader => reader => new API_Access_Logs
        {
            API_Name = reader.GetString(0),
            Accessor_ID = reader.GetString(1),
            Access_Time = reader.GetDateTime(2),
            Access_Result = reader.GetString(3),
            Access_ID = reader.GetInt32(4)
        };

        public Tuple<bool, string> GetLogs(QueryAPIAccessLogsArgs args)
        {
            var parameters = new Dictionary<(string ColumnName, string Operator), object>
            {
                { ("API_NAME", "="), args.APIName },
                { ("ACCESSOR_ID", "="), args.AccessorID },
                { ("ACCESS_TIME", ">="), args.StartTime },
                { ("ACCESS_TIME", "<="), args.EndTime },
                { ("ACCESS_RESULT", "="), args.Result },
                { ("ACCESS_ID", "="), args.AccessID }
            };

            return GetLogsAux(parameters);
        }

        public Tuple<bool, string> InsertNewLog(APIAccessLogsInsertUtil args)
        {
            var values = new Dictionary<string, object>
            {
                { "API_NAME", args.API_Name },
                { "ACCESSOR_ID", args.Accessor_ID },
                { "ACCESS_TIME", args.Access_Time },
                { "ACCESS_RESULT", args.Access_Result }
            };
            return InsertNewLogAux(values);
        }
    }
}