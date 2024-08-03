using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Utilities;
using System.Data;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;
using SQLOperation.PublicAccess.Templates.SQLManager;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class APILogsDAL : BaseDAL
    {
        /// <summary>
        /// Retrieves API access logs.
        /// </summary>
        /// <param name="AccessID">The ID of the access log (nullable).</param>
        /// <param name="APIName">The name of the API (nullable).</param>
        /// <param name="AccessorID">The ID of the accessor (nullable).</param>
        /// <param name="StartTime">The start time for the access query (nullable).</param>
        /// <param name="EndTime">The end time for the access query (nullable).</param>
        /// <returns>A tuple containing a boolean indicating success and the query result as a string.</returns>
        public Tuple<bool, string> GetAPIAccessLogs(
            int? AccessID,
            string APIName,
            string Result,
            int? AccessorID,
            DateTime? StartTime,
            DateTime? EndTime)
        {
            return DoQuery(QueryGenerator(AccessID, APIName, Result, AccessorID, StartTime, EndTime));
        }

        /// <summary>
        /// Inserts a new API access log.
        /// </summary>
        /// <param name="NewInfo">An instance of API_Access_Logs containing the information of the new log entry.</param>
        /// <returns>A tuple containing a boolean indicating success and the result of the insertion as a string.</returns>
        public Tuple<bool, string> InsertNewLog(APIAccessLogsInsertUtil NewInfo)
        {
            return DoQuery(InsertGenerator(NewInfo));
        }

        private Func<Tuple<bool, string>> InsertGenerator(APIAccessLogsInsertUtil NewInfo)
        {
            return () =>
            {
                List<string> ColumnNames = ["API_NAME", "ACCESSOR_ID", "ACCESS_TIME", "ACCESS_RESULT"];
                List<object> Values = [NewInfo.API_Name, NewInfo.Accessor_ID, NewInfo.Access_Time, NewInfo.Access_Result];

                Tuple<bool, string> QueryResult = BasicSQLOps.InsertOperation("API_ACCESS_LOGS", ColumnNames, Values);
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
            int? AccessID,
            string APIName,
            string Result,
            int? AccessorID,
            DateTime? StartTime,
            DateTime? EndTime)
        {
            return () =>
            {
                if (OracleConnection.State == ConnectionState.Open)
                {
                    try
                    {
                        var query = "SELECT * FROM API_Access_Logs WHERE 1=1";
                        var parameters = new List<OracleParameter>();

                        AddParameterIfValueExists(ref query, parameters, "Access_ID", AccessID);
                        AddParameterIfValueExists(ref query, parameters, "API_Name", APIName);
                        AddParameterIfValueExists(ref query, parameters, "Accessor_ID", AccessorID);
                        AddParameterIfValueExists(ref query, parameters, "Access_Result", Result);
                        AddParameterIfValueExists(ref query, parameters, "Access_Time", StartTime, ">=");
                        AddParameterIfValueExists(ref query, parameters, "Access_Time", EndTime, "<=");

                        var command = new OracleCommand(query, OracleConnection);
                        command.Parameters.AddRange(parameters.ToArray());

                        var reader = command.ExecuteReader();
                        var result = new List<API_Access_Logs>();
                        while (reader.Read())
                        {
                            var log = new API_Access_Logs
                            {
                                API_Name = reader.GetString(0),
                                Accessor_ID = reader.GetString(1),
                                Access_Time = reader.GetDateTime(2),
                                Access_Result = reader.GetString(3),
                                Access_ID = reader.GetInt32(4),
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