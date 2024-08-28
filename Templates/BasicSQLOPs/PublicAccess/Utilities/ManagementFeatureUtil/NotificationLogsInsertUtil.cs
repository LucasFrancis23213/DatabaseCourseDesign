using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class NotificationLogsInsertUtil
    {
        public int User_ID { get; set; }
        public string Notification_Type { get; set; }
        public DateTime Send_Date { get; set; }
        public string Status { get; set; }
    }
}
