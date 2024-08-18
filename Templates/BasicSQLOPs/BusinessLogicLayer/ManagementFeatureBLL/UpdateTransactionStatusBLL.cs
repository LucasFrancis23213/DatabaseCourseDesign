using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class UpdateTransactionStatusBLL
    {
        private TransactionLogsDAL TransactionLogsDAL;

        public UpdateTransactionStatusBLL()
        {
            TransactionLogsDAL = new();
        }

        public Tuple<bool, string> UpdateTransactionStatus(int TransactionID, string NewStatus, DateTime? FinishTime)
        {
            if (TransactionID <= 0 || string.IsNullOrEmpty(NewStatus))
            {
                return Tuple.Create(false, "传入参数不完整");
            }

            return TransactionLogsDAL.UpdateLog(TransactionID, NewStatus, FinishTime);
        }
    }
}
