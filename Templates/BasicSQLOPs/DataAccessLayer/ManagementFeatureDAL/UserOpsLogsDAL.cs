using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Utilities;
using System.Data;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    /*public class UserOpsLogsDAL : BaseDAL
    {
        /// <summary>
        /// Retrieves user operation logs.
        /// </summary>
        /// <param name="ActivityLogID">The ID of the activity log (nullable).</param>
        /// <param name="UserID">The ID of the user (nullable).</param>
        /// <param name="ActionType">The type of action performed by the user.</param>
        /// <param name="StartTime">The start time for the log query (nullable).</param>
        /// <param name="EndTime">The end time for the log query (nullable).</param>
        /// <returns>A tuple containing a boolean indicating success and the query result as a string.</returns>
        public Tuple<bool, string> GetTargetLogs(
            int? ActivityLogID,
            int? UserID,
            string ActionType,
            DateTime? StartTime,
            DateTime? EndTime)
        {
            return DoQuery(QueryGenerator(ActivityLogID, UserID, ActionType, StartTime, EndTime));
        }

        /// <summary>
        /// Inserts a new user operation log.
        /// </summary>
        /// <param name="NewInfo">An instance of User_Activity_Logs containing the information of the new log entry.</param>
        /// <returns>A tuple containing a boolean indicating success and the result of the insertion as a string.</returns>
        public Tuple<bool, string> InsertNewLog(UserOpsLogsInsertUtil NewInfo)
        {
            return DoQuery(InsertGenerator(NewInfo));
        }

        private Func<Tuple<bool, string>> InsertGenerator(UserOpsLogsInsertUtil NewInfo)
        {
            return () =>
            {
                List<string> ColumnNames = ["USER_ID", "ACTION_TYPE", "OCCURRENCE_TIME"];
                List<object> Values = [NewInfo.User_ID, NewInfo.Action_Type, NewInfo.Occurrence_Time];

                Tuple<bool, string> QueryResult = BasicSQLOps.InsertOperation("USER_ACTIVITY_LOGS", ColumnNames, Values);
                return QueryResult;
            };
        }

        private void AddParameterIfValueExists<T>(ref string query, List<OracleParameter> parameters, string columnName, T? value, string operatorSymbol = "=") where T : struct
        {
            if (value.HasValue)
            {
                query += $" AND {columnName} {operatorSymbol} :{columnName}";
                parameters.Add(new OracleParameter($":{columnName}", value.Value));
            }
        }

        private void AddParameterIfValueExists(ref string query, List<OracleParameter> parameters, string columnName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                query += $" AND {columnName} = :{columnName}";
                parameters.Add(new OracleParameter($":{columnName}", value));
            }
        }

        private Func<Tuple<bool, string>> QueryGenerator(
        int? activityLogID,
        int? userID,
        string actionType,
        DateTime? startTime,
        DateTime? endTime)
        {
            return () =>
            {
                if (OracleConnection.State == ConnectionState.Open)
                {
                    try
                    {
                        var query = "SELECT * FROM User_Activity_Logs WHERE 1=1";
                        var parameters = new List<OracleParameter>();

                        AddParameterIfValueExists(ref query, parameters, "Activity_Log_ID", activityLogID);
                        AddParameterIfValueExists(ref query, parameters, "User_ID", userID);
                        AddParameterIfValueExists(ref query, parameters, "Action_Type", actionType);
                        AddParameterIfValueExists(ref query, parameters, "Occurrence_Time", startTime, ">=");
                        AddParameterIfValueExists(ref query, parameters, "Occurrence_Time", endTime, "<=");

                        var command = new OracleCommand(query, OracleConnection);
                        command.Parameters.AddRange(parameters.ToArray());

                        var reader = command.ExecuteReader();
                        var result = new List<User_Activity_Logs>();
                        while (reader.Read())
                        {
                            var log = new User_Activity_Logs
                            {
                                User_ID = reader.GetInt32(0),
                                Action_Type = reader.GetString(1),
                                Occurrence_Time = reader.GetDateTime(2),
                                Activity_Log_ID = reader.GetInt32(3),
                            };
                            result.Add(log);
                        }

                        // Assuming success if we got this far
                        var jsonResult = JsonConvert.SerializeObject(result);
                        return Tuple.Create(true, jsonResult);
                    }
                    catch (Exception ex)
                    {
                        // Return failure with exception message
                        return Tuple.Create(false, ex.Message);
                    }
                }
                else
                {
                    return Tuple.Create(false, "Connection Error");
                }
            };
        }
    }*/

    public class UserOpsLogsDAL : BaseLogDAL<User_Activity_Logs>
    {
        protected override string TableName => "USER_ACTIVITY_LOGS";

        protected override List<string> ColumnNames => ["USER_ID", "ACTION_TYPE", "OCCURRENCE_TIME"];

        protected override Func<OracleDataReader, User_Activity_Logs> MapFromReader => reader => new User_Activity_Logs
        {
            User_ID = reader.GetInt32(0),
            Action_Type = reader.GetString(1),
            Occurrence_Time = reader.GetDateTime(2),
            Activity_Log_ID = reader.GetInt32(3),
        };

        public Tuple<bool, string> GetTargetLogs(QueryUserOpsLogsArgs args)
        {
            var parameters = new Dictionary<(string ColumnName, string Operator), object>
            {
                { ("USER_ID", "="), args.UserID },
                { ("ACTION_TYPE", "="), args.ActionType },
                { ("OCCURRENCE_TIME", ">="), args.StartTime },
                { ("OCCURRENCE_TIME", "<="), args.EndTime },
                { ("ACTIVITY_LOG_ID", "="), args.ActivityLogID }
            };

            return GetLogsAux(parameters);
        }

        public Tuple<bool, string> InsertNewLog(UserOpsLogsInsertUtil args)
        {
            var values = new Dictionary<string, object>
            {
                { "USER_ID", args.User_ID },
                { "ACTION_TYPE", args.Action_Type },
                { "OCCURRENCE_TIME", args.Occurrence_Time }
            };
            return InsertNewLogAux(values);
        }
    }
}