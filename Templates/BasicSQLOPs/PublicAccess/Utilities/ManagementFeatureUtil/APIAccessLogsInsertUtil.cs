using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class APIAccessLogsInsertUtil
    {
        public string API_Name {  get; set; }
        public int Accessor_ID {  get; set; }
        public DateTime Access_Time {  get; set; }
        public string Access_Result {  get; set; }
    }
}
