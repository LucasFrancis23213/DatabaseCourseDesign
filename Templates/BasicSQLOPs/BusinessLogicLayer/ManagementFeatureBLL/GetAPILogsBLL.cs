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
            string? APIName = InputArgs.APIName;
            int? AccessorID = null;
            DateTime? StartTime = null;
            DateTime? EndTime = null;
            string? Result = InputArgs.Result;
            int? AccessID = null;

            if (InputArgs.AccessID > 0)
            {
                AccessID = InputArgs.AccessID;
            }
            if (InputArgs.AccessorID > 0)
            {
                AccessorID = InputArgs.AccessorID;
            }
            if (InputArgs.StartTime != default)
            {
                StartTime = InputArgs.StartTime;
            }
            if (InputArgs.EndTime != default)
            {
                EndTime = InputArgs.EndTime;
            }

            return APILogsDAL.GetAPIAccessLogs(AccessID, APIName, Result, AccessorID, StartTime, EndTime);
        }
    }
}
