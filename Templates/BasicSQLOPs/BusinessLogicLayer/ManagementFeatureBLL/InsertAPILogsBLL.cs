using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class InsertAPILogsBLL
    {
        private APILogsDAL APILogsDAL;
        public InsertAPILogsBLL()
        {
            APILogsDAL = new();
        }

        public Tuple<bool, string> InsertLog(APIAccessLogsInsertUtil NewLog)
        {
            if (string.IsNullOrEmpty(NewLog.API_Name) ||
                string.IsNullOrEmpty(NewLog.Access_Result) ||
                NewLog.Accessor_ID <= 0 ||
                NewLog.Access_Time == default
                )
            {
                return Tuple.Create(false, "日志信息不完整");
            }

            return APILogsDAL.InsertNewLog(NewLog);
        }
    }
}