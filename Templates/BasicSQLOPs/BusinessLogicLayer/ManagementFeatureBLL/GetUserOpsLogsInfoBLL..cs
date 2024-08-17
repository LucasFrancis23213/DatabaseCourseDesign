using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;
using System.Configuration;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class GetUserOpsLogsInfoBLL
    {
        private UserOpsLogsDAL UserOpsLogsDAL;

        public GetUserOpsLogsInfoBLL()
        {
            UserOpsLogsDAL = new();
        }

        public Tuple<bool, string> GetUserOpsLogs(QueryUserOpsLogsArgs InputArgs)
        {
            if (InputArgs.ActivityLogID <= 0)
            {
                return Tuple.Create(false, "ActivityLogID不合法");
            }
            if (InputArgs.UserID <= 0)
            {
                return Tuple.Create(false, "UserID不合法");
            }
            if (InputArgs.StartTime is not null && InputArgs.StartTime is not DateTime)
            {
                return Tuple.Create(false, "开始时间不合法");
            }
            if (InputArgs.EndTime is not null && InputArgs.EndTime is not DateTime)
            {
                return Tuple.Create(false, "结束时间不合法");
            }

            return UserOpsLogsDAL.GetTargetLogs(InputArgs);
        }

    }
}