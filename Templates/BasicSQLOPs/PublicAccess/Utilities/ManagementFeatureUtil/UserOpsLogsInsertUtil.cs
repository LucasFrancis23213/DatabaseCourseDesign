using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class UserOpsLogsInsertUtil
    {
        public int User_ID { get; set; }
        public string Action_Type { get; set; }
        public DateTime Occurrence_Time { get; set; }
    }
}