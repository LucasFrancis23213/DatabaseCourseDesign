using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class InsertUserOpsLogsBLL
    {
        private UserOpsLogsDAL UserOpsLogsDAL;
        public InsertUserOpsLogsBLL()
        {
            UserOpsLogsDAL = new UserOpsLogsDAL();
        }

        public Tuple<bool, string> InsertLog(UserOpsLogsInsertUtil NewLog)
        {
            UserOperatorDAL UserChecker = new();
            if (!UserChecker.CheckUserID(NewLog.User_ID))
            {
                return Tuple.Create(false, "用户不存在");
            }

            if (string.IsNullOrEmpty(NewLog.Action_Type) || NewLog.Occurrence_Time == default)
            {
                return Tuple.Create(false, "日志信息不完整");
            }

            return UserOpsLogsDAL.InsertNewLog(NewLog);
        }
    }
}