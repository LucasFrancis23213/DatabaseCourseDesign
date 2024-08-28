using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class NotificationLogsDAL : BaseLogDAL<Notification_Logs>
    {
        protected override string TableName => "NOTIFICATION_LOGS";

        protected override List<string> ColumnNames => ["USER_ID", "NOTIFICATION_TYPE", "SEND_DATE", "STATUS"];

        protected override Func<OracleDataReader, Notification_Logs> MapFromReader => reader => new Notification_Logs
        {
            User_ID = reader.GetInt32(0),
            Notification_Type = reader.GetString(1),
            Send_Date = reader.GetDateTime(2),
            Status = reader.GetString(3),
            Notification_ID = reader.GetInt32(4)
        };

        public Tuple<bool, string> GetLogs(QueryNotificationLogsArgs args)
        {
            var parameters = new Dictionary<(string ColumnName, string Operator), object>
            {
                { ("USER_ID", "="), args.UserID },
                { ("NOTIFICATION_TYPE", "="), args.NotificationType },
                { ("SEND_DATE", ">="), args.StartDate },
                { ("SEND_DATE", "<="), args.EndDate },
                { ("STATUS", "="), args.Status },
                { ("NOTIFICATION_ID", "="), args.NotificationID }
            };

            return GetLogsAux(parameters);
        }

        public Tuple<bool, string> InsertNewLog(NotificationLogsInsertUtil args)
        {
            var values = new Dictionary<string, object>
            {
                { "USER_ID", args.User_ID },
                { "NOTIFICATION_TYPE", args.Notification_Type },
                { "SEND_DATE", args.Send_Date },
                { "STATUS", args.Status }
            };
            return InsertNewLogAux(values);
        }
    }
}
