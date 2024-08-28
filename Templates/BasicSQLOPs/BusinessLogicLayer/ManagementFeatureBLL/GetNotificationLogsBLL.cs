using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class GetNotificationLogsBLL
    {
        private NotificationLogsDAL NotificationLogsDAL;
        public GetNotificationLogsBLL()
        {
            NotificationLogsDAL = new();
        }

        public Tuple<bool, string> GetLogs(QueryNotificationLogsArgs args)
        {
            if (args.NotificationID <= 0)
            {
                return Tuple.Create(false, "通知ID不合法");
            }
            if (args.UserID <= 0)
            {
                return Tuple.Create(false, "用户ID不合法");
            }
            if (args.StartDate is not null && args.StartDate is not DateTime)
            {
               return Tuple.Create(false, "开始时间不合法");
            }
            if (args.EndDate is not null && args.EndDate is not DateTime)
            {
                return Tuple.Create(false, "结束时间不合法");
            }
            
            return NotificationLogsDAL.GetLogs(args);
        }
    }
}
