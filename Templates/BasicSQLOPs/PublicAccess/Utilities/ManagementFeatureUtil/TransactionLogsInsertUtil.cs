using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class TransactionLogsInsertUtil
    {
        public int FromUserID { get; set; }
        public int ToUserID { get; set; }
        public string? ItemID { get; set; }
        public double Amount { get; set; }
        public string TransactionType { get; set; }
        public DateTime StartTime { get; set;}
        public DateTime? FinishTime { get; set; }
        public string Status { get; set; }
    }
}
