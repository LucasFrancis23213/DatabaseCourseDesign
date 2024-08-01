using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class QueryUserOpsLogsArgs
    {
        public int? ActivityLogID {  get; set; }
        public int? UserID {  get; set; }
        public string? ActionType {  get; set; }
        public DateTime? StartTime {  get; set; }
        public DateTime? EndTime { get; set; }
    }
}
