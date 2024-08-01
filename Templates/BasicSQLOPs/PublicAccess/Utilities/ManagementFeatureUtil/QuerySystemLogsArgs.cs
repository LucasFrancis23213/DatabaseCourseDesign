using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class QuerySystemLogsArgs
    {
        public int? SystemLogID {  get; set; }
        public int? UserID { get; set; }
        public string? OperationType {  get; set; }
    }
}
