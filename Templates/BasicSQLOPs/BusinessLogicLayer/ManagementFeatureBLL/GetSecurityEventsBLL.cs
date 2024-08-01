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
            int? EventID = null;
            string? EventType = InputArgs.EventType;
            string? Status = InputArgs.Status;
            DateTime? StartTime = default;
            DateTime? EndTime = default;

            if (InputArgs.EventID > 0)
            {
                EventID = InputArgs.EventID;
            }
            if (InputArgs.StartTime != default)
            {
                StartTime = InputArgs.StartTime;
            }
            if (InputArgs.EndTime != default)
            {
                EndTime = InputArgs.EndTime;
            }

            return SecurityEventsDAL.GetSecurityEvents(EventID, EventType, Status, StartTime, EndTime);
        }

    }
}
