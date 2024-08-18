using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class InsertRecommendationLogBLL
    {
        private RecommendationLogsDAL RecommendationLogsDAL;

        public InsertRecommendationLogBLL()
        {
            RecommendationLogsDAL = new();
        }

        public Tuple<bool, string> InsertRecommendationLog(RecommendationLogsInsertUtil NewLog)
        {
            if (NewLog.UserID <= 0 ||
                string.IsNullOrEmpty(NewLog.RecommendationType) ||
                NewLog.RecommendationTime == default ||
                string.IsNullOrEmpty(NewLog.UserFeedback))
            {
                return Tuple.Create(false, "传入参数不完整");
            }

            return RecommendationLogsDAL.InsertNewLog(NewLog);
        }
    }
}
