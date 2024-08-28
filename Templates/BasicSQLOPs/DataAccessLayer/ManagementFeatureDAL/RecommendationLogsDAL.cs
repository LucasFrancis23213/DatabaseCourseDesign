using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class RecommendationLogsDAL : BaseLogDAL<Recommendation_Logs>
    {
        protected override string TableName => "RECOMMENDATION_LOGS";

        protected override List<string> ColumnNames => ["USER_ID", "RECOMMENDATION_TYPE", "RECOMMENDATION_TIME", "USER_FEED_BACK"];

        protected override Func<OracleDataReader, Recommendation_Logs> MapFromReader => reader => new Recommendation_Logs
        {
            User_ID = reader.GetInt32(0),
            Recommendation_Type = reader.GetString(1),
            Recommendation_Time = reader.GetDateTime(2),
            User_Feedback = reader.GetString(3),
            Log_ID = reader.GetInt32(4)
        };

        public Tuple<bool, string> GetLogs(QueryRecommendationLogsArgs args)
        {
            var parameters = new Dictionary<(string ColumnName, string Operator), object>
            {
                { ("USER_ID", "="), args.UserID },
                { ("RECOMMENDATION_TYPE", "="), args.RecommendationType },
                { ("RECOMMENDATION_TIME", ">="), args.StartTime },
                { ("RECOMMENDATION_TIME", "<="), args.EndTime },
                { ("USER_FEED_BACK", "="), args.UserFeedback },
                { ("LOG_ID", "="), args.LogID }
            };

            return GetLogsAux(parameters);
        }

        public Tuple<bool, string> InsertNewLog(RecommendationLogsInsertUtil args)
        {
            var values = new Dictionary<string, object>
            {
                { "USER_ID", args.UserID },
                { "RECOMMENDATION_TYPE", args.RecommendationType },
                { "RECOMMENDATION_TIME", args.RecommendationTime },
                { "USER_FEED_BACK", args.UserFeedback }
            };
            return InsertNewLogAux(values);
        }
    }
}
