using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class GetSecurityEventsBLL
    {
        private SecurityEventsDAL SecurityEventsDAL;

        public GetSecurityEventsBLL()
        {
            SecurityEventsDAL = new();
        }

        public Tuple<bool, string> GetSecurityEvents(QuerySecurityEventsArgs InputArgs)
        {
            if (InputArgs.EventID <= 0)
            {
                return Tuple.Create(false, "EventID不合法");
            }
            if (InputArgs.StartTime is not null && InputArgs.StartTime is not DateTime)
            {
                return Tuple.Create(false, "开始时间不合法");
            }
            if (InputArgs.EndTime is not null && InputArgs.EndTime is not DateTime)
            {
                return Tuple.Create(false, "结束时间不合法");
            }

            return SecurityEventsDAL.GetSecurityEvents(InputArgs);
        }

    }
}