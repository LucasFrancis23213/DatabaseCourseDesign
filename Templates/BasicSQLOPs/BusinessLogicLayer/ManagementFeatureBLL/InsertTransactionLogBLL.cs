using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class InsertTransactionLogBLL
    {
        private TransactionLogsDAL TransactionLogsDAL;

        public InsertTransactionLogBLL()
        {
            TransactionLogsDAL = new();
        }

        public Tuple<bool, string> InsertTransactionLog(TransactionLogsInsertUtil NewLog)
        {
            if (NewLog.FromUserID <= 0 ||
                NewLog.ToUserID <= 0 ||
                (NewLog.Amount <= 0) ||
                string.IsNullOrEmpty(NewLog.TransactionType) ||
                NewLog.StartTime == default ||
                string.IsNullOrEmpty(NewLog.Status))
            {
                return Tuple.Create(false, "传入参数不完整");
            }

            return TransactionLogsDAL.InsertNewLog(NewLog);
        }
    }
}
