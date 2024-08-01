using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class QueryAPIAccessLogsArgs
    {
        public string? APIName {  get; set; }
        public int? AccessorID {  get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set;}
        public string? Result {  get; set; }
        public int? AccessID { get; set; }
    }
}
