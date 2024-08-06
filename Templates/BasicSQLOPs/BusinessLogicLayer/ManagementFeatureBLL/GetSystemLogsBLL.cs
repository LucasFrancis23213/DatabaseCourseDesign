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
            int? UserID = null;
            int? SystemLogID = null;
            string? OperationType = InputArgs.OperationType;

            if (InputArgs.UserID > 0)
            {
                UserID = InputArgs.UserID;
            }
            if (InputArgs.SystemLogID > 0)
            {
                SystemLogID = InputArgs.SystemLogID;
            }

            return SystemLogsDAL.GetSystemLogs(SystemLogID, OperationType, UserID);
        }
    }
}