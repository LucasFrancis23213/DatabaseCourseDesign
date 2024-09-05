using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class GetAPILogsBLL
    {
        private APILogsDAL APILogsDAL;

        public GetAPILogsBLL()
        {
            APILogsDAL = new();
        }

        public Tuple<bool, string> GetAPILogs(QueryAPIAccessLogsArgs InputArgs)
        {
            if (InputArgs.AccessID <= 0)
            {
                return Tuple.Create(false, "AccessID不合法");
            }
            if (InputArgs.AccessorID < 0)
            {
                // AccessorID == 0 留给测试用
                return Tuple.Create(false, "AccessorID不合法");
            }
            if (InputArgs.StartTime is not null && InputArgs.StartTime is not DateTime)
            {
                return Tuple.Create(false, "开始时间不合法");
            }
            if (InputArgs.EndTime is not null && InputArgs.EndTime is not DateTime)
            {
                return Tuple.Create(false, "结束时间不合法");
            }

            return APILogsDAL.GetLogs(InputArgs);
        }
    }
}