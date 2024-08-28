using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class RecommendationLogsInsertUtil
    {
        public int UserID { get; set; }
        public string RecommendationType { get; set; }
        public DateTime RecommendationTime { get; set; }
        public string UserFeedback { get; set; }
    }
}
