using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

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
            int? ActivityLogID = null;
            int? UserID = null;
            string ActionType = InputArgs.ActionType;
            DateTime? StartTime = null;
            DateTime? EndTime = null;

            if (InputArgs.ActivityLogID > 0)
            {
                ActivityLogID = InputArgs.ActivityLogID;
            }
            if (InputArgs.UserID > 0)
            {
                UserID = InputArgs.UserID;
            }
            if (InputArgs.StartTime != default)
            {
                StartTime = InputArgs.StartTime;
            }
            if (InputArgs.EndTime != default)
            {
                EndTime = InputArgs.EndTime;
            }

            return UserOpsLogsDAL.GetTargetLogs(ActivityLogID, UserID, ActionType, StartTime, EndTime);
        }

    }
}