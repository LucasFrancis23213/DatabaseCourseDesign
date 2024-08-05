using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Utilities;
using System.Data;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class SystemLogsDAL : BaseDAL
    {
        /// <summary>
        /// Retrieves system logs.
        /// </summary>
        /// <param name="SystemLogID">The ID of the system log (nullable).</param>
        /// <param name="OperationType">The type of operation (nullable).</param>
        /// <param name="UserID">The ID of the user (nullable).</param>
        /// <returns>A tuple containing a boolean indicating success and the query result as a string.</returns>
        public Tuple<bool, string> GetSystemLogs(
            int? SystemLogID,
            string OperationType,
            int? UserID)
        {
            return DoQuery(QueryGenerator(SystemLogID, OperationType, UserID));
        }

        /// <summary>
        /// Inserts a new system log.
        /// </summary>
        /// <param name="NewInfo">An instance of System_Logs containing the information of the new log entry.</param>
        /// <returns>A tuple containing a boolean indicating success and the result of the insertion as a string.</returns>
        public Tuple<bool, string> InsertNewLog(SystemLogsInsertUtil NewInfo)
        {
            return DoQuery(InsertGenerator(NewInfo));
        }

        private Func<Tuple<bool, string>> InsertGenerator(SystemLogsInsertUtil NewInfo)
        {
            return () =>
            {
                List<string> ColumnNames = ["OPERATION_TYPE", "OPERATION_DETAILS", "USER_ID"];
                List<object> Values = [NewInfo.OperationType, NewInfo.OperationDetails, NewInfo.UserID];

                Tuple<bool, string> QueryResult = BasicSQLOps.InsertOperation("SYSTEM_LOGS", ColumnNames, Values);
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
            int? SystemLogID,
            string OperationType,
            int? UserID)
        {
            return () =>
            {
                if (OracleConnection.State == ConnectionState.Open)
                {
                    try
                    {
                        var query = "SELECT * FROM System_Logs WHERE 1=1";
                        var parameters = new List<OracleParameter>();

                        AddParameterIfValueExists(ref query, parameters, "System_Log_ID", SystemLogID);
                        AddParameterIfValueExists(ref query, parameters, "Operation_Type", OperationType);
                        AddParameterIfValueExists(ref query, parameters, "User_ID", UserID);

                        var command = new OracleCommand(query, OracleConnection);
                        command.Parameters.AddRange(parameters.ToArray());

                        var reader = command.ExecuteReader();
                        var result = new List<System_Logs>();
                        while (reader.Read())
                        {
                            var log = new System_Logs
                            {
                                User_ID = reader.GetInt32(0),
                                Operation_Type = reader.GetString(1),
                                Operation_Details = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty,
                                System_Log_ID = reader.GetInt32(3)
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
    }
}