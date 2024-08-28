using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class GetRecommendationLogsBLL
    {
        private RecommendationLogsDAL RecommendationLogsDAL;
        public GetRecommendationLogsBLL()
        {
            RecommendationLogsDAL = new();
        }

        public Tuple<bool, string> GetLogs(QueryRecommendationLogsArgs args)
        {
            if (args.LogID <= 0)
            {
                return Tuple.Create(false, "日志ID不合法");
            }
            if (args.UserID <= 0)
            {
                return Tuple.Create(false, "用户ID不合法");
            }
            if (args.StartTime is not null && args.StartTime is not DateTime)
            {
                return Tuple.Create(false, "开始时间不合法");
            }
            if (args.EndTime is not null && args.EndTime is not DateTime)
            {
                return Tuple.Create(false, "结束时间不合法");
            }

            return RecommendationLogsDAL.GetLogs(args);
        }
    }
}
