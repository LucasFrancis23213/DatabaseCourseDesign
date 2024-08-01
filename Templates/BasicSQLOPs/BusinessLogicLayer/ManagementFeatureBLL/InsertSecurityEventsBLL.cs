using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class InsertSecurityEventsBLL
    {
        private SecurityEventsDAL SecurityEventsDAL;

        public InsertSecurityEventsBLL()
        {
            SecurityEventsDAL = new();
        }

        public Tuple<bool, string> InsertLog(SecurityEventsInsertUtil NewLog)
        {
            if (NewLog == null || string.IsNullOrEmpty(NewLog.Event_Type) || string.IsNullOrEmpty(NewLog.Status))
            {
                return Tuple.Create(false, "传入信息不完整");
            }

            return SecurityEventsDAL.InsertNewEvent(NewLog);
        }
    }
}
