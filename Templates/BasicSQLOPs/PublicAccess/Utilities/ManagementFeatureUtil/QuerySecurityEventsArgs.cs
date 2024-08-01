using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class QuerySecurityEventsArgs
    {
        public int? EventID { get; set; }
        public string? EventType { get; set; }
        public string? Status {  get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set;}
    }
}
