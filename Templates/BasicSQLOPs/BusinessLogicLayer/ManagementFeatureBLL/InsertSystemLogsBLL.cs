using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class InsertSystemLogsBLL
    {
        private SystemLogsDAL SystemLogsDAL;
        public InsertSystemLogsBLL()
        {
            SystemLogsDAL = new();
        }

        public Tuple<bool, string> InsertLog(SystemLogsInsertUtil NewLog)
        {
            UserOperatorDAL UserChecker = new();
            if (!UserChecker.CheckUserID(NewLog.UserID))
            {
                return Tuple.Create(false, "用户不存在");
            }

            if (string.IsNullOrEmpty(NewLog.OperationType))
            {
                return Tuple.Create(false, "日志信息不完整");
            }

            return SystemLogsDAL.InsertNewLog(NewLog);
        }
    }
}
