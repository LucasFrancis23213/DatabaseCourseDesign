using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Utilities;
using System.Data;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class SecurityEventsDAL : BaseLogDAL<Security_Events>
    {
        /*/// <summary>
        /// Retrieves security event logs.
        /// </summary>
        /// <param name="EventID">The ID of the event (nullable).</param>
        /// <param name="EventType">The type of event (nullable).</param>
        /// <param name="Status">The status of the event (nullable).</param>
        /// <param name="StartTime">The start date for the event query (nullable).</param>
        /// <param name="EndTime">The end date for the event query (nullable).</param>
        /// <returns>A tuple containing a boolean indicating success and the query result as a string.</returns>
        public Tuple<bool, string> GetSecurityEvents(
            int? EventID,
            string EventType,
            string Status,
            DateTime? StartTime,
            DateTime? EndTime)
        {
            return DoQuery(QueryGenerator(EventID, EventType, Status, StartTime, EndTime));
        }

        /// <summary>
        /// Inserts a new security event log.
        /// </summary>
        /// <param name="NewInfo">An instance of Security_Events containing the information of the new event entry.</param>
        /// <returns>A tuple containing a boolean indicating success and the result of the insertion as a string.</returns>
        public Tuple<bool, string> InsertNewEvent(SecurityEventsInsertUtil NewInfo)
        {
            return DoQuery(InsertGenerator(NewInfo));
        }

        private Func<Tuple<bool, string>> InsertGenerator(SecurityEventsInsertUtil NewInfo)
        {
            return () =>
            {
                List<string> ColumnNames = ["EVENT_TYPE", "EVENT_DETAILS", "STATUS", "OCCURRENCE_TIME"];
                List<object> Values = [NewInfo.Event_Type, NewInfo.Event_Details, NewInfo.Status, NewInfo.Occurrence_Date];

                Tuple<bool, string> QueryResult = BasicSQLOps.InsertOperation("SECURITY_EVENTS", ColumnNames, Values);
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
            int? EventID,
            string EventType,
            string Status,
            DateTime? StartDate,
            DateTime? EndDate)
        {
            return () =>
            {
                if (OracleConnection.State == ConnectionState.Open)
                {
                    try
                    {
                        var query = "SELECT * FROM Security_Events WHERE 1=1";
                        var parameters = new List<OracleParameter>();

                        AddParameterIfValueExists(ref query, parameters, "Event_ID", EventID);
                        AddParameterIfValueExists(ref query, parameters, "Event_Type", EventType);
                        AddParameterIfValueExists(ref query, parameters, "Status", Status);
                        AddParameterIfValueExists(ref query, parameters, "Occurrence_Time", StartDate, ">=");
                        AddParameterIfValueExists(ref query, parameters, "Occurrence_Time", EndDate, "<=");

                        var command = new OracleCommand(query, OracleConnection);
                        command.Parameters.AddRange(parameters.ToArray());

                        var reader = command.ExecuteReader();
                        var result = new List<Security_Events>();
                        while (reader.Read())
                        {
                            var evt = new Security_Events
                            {
                                Event_Type = reader.GetString(0),
                                Event_Details = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty,
                                Status = reader.GetString(2),
                                Occurrence_Date = reader.GetDateTime(3),
                                Event_ID = reader.GetInt32(4)
                            };
                            result.Add(evt);
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
        protected override string TableName => "SECURITY_EVENTS";

        protected override List<string> ColumnNames => ["USER_ID", "ACTION_TYPE", "OCCURRENCE_TIME"];

        protected override Func<OracleDataReader, Security_Events> MapFromReader => reader => new Security_Events
        {
            Event_Type = reader.GetString(0),
            Event_Details = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty,
            Status = reader.GetString(2),
            Occurrence_Date = reader.GetDateTime(3),
            Event_ID = reader.GetInt32(4)
        };

        public Tuple<bool, string> GetSecurityEvents(QuerySecurityEventsArgs args)
        {
            var parameters = new Dictionary<(string ColumnName, string Operator), object>
            {
                { ("Event_ID", "="), args.EventID },
                { ("Event_Type", "="), args.EventType },
                { ("Status", "="), args.Status },
                { ("Occurrence_Time", ">="), args.StartTime },
                { ("Occurrence_Time", "<="), args.EndTime }
            };

            return GetLogsAux(parameters);
        }

        public Tuple<bool, string> InsertNewEvent(SecurityEventsInsertUtil args)
        {
            var values = new Dictionary<string, object>
            {
                { "Event_Type", args.Event_Type },
                { "Event_Details", args.Event_Details },
                { "Status", args.Status },
                { "Occurrence_Time", args.Occurrence_Date }
            };
            return InsertNewLogAux(values);
        }
    }
}