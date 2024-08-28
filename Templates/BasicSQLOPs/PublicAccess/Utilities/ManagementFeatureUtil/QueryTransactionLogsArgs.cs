using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class QueryTransactionLogsArgs
    {
        public int? FromUserID { get; set; }
        public int? ToUserID { get; set; }
        public string? ItemID { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
        public string? TransactionType { get; set; }
        public int? TransactionID { get; set; }
        public string? Status { get; set; }
        public DateTime? StartTimeBeg {  get; set; }
        public DateTime? StartTimeEnd {  get; set; }
        public DateTime? FinishTimeBeg { get; set; }
        public DateTime? FinishTimeEnd { get; set; }
    }
}
