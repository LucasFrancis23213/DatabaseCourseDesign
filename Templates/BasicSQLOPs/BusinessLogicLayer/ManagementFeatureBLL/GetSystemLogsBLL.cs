using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class GetSystemLogsBLL
    {
        private SystemLogsDAL SystemLogsDAL;

        public GetSystemLogsBLL()
        {
            SystemLogsDAL = new();
        }

        public Tuple<bool, string> GetSystemLogs(QuerySystemLogsArgs InputArgs)
        {
            if (InputArgs.UserID <= 0)
            {
                return Tuple.Create(false, "UserID不合法");
            }
            if (InputArgs.SystemLogID <= 0)
            {
                return new Tuple<bool, string>(false, "SystemLogID不合法");
            }

            return SystemLogsDAL.GetSystemLogs(InputArgs);
        }
    }
}