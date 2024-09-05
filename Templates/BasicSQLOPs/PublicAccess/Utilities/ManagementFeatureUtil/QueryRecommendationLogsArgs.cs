using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class QueryRecommendationLogsArgs
    {
        public int? UserID { get; set; }
        public string? RecommendationType { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? UserFeedback { get; set; }
        public int? LogID { get; set; }
    }
}
