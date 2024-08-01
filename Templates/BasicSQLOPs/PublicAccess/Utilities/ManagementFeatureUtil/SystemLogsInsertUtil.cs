using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class SystemLogsInsertUtil
    {
        public string OperationType {  get; set; }
        public string? OperationDetails {  get; set; }
        public int UserID {  get; set; }
    }
}
