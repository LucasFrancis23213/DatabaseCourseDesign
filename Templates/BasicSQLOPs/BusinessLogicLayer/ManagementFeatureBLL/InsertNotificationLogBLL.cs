using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class InsertNotificationLogBLL
    {
        private NotificationLogsDAL NotificationLogsDAL;

        public InsertNotificationLogBLL()
        {
            NotificationLogsDAL = new();
        }

        public Tuple<bool, string> InsertNotificationLog(NotificationLogsInsertUtil NewLog)
        {
            if (NewLog.User_ID <= 0 || 
                string.IsNullOrEmpty(NewLog.Status) ||
                string.IsNullOrEmpty(NewLog.Notification_Type) ||
                NewLog.Send_Date == default)
            {
                return Tuple.Create(false, "传入参数不完整");
            }

            return NotificationLogsDAL.InsertNewLog(NewLog);
        }
    }
}
