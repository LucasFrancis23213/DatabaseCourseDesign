using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class GetTransactionLogsBLL
    {
        private TransactionLogsDAL TransactionLogsDAL;
        public GetTransactionLogsBLL()
        {
            TransactionLogsDAL = new();
        }

        public Tuple<bool, string> GetLogs(QueryTransactionLogsArgs args)
        {
            if (args.TransactionID <= 0)
            {
                return Tuple.Create(false, "交易ID不合法");
            }
            if (args.FromUserID <= 0 && args.ToUserID <= 0)
            {
                return Tuple.Create(false, "用户ID不合法");
            }
            if (args.MinAmount < 0)
            {
                return Tuple.Create(false, "最小金额不合法");
            }
            if (args.MaxAmount < 0 || (args.MinAmount > args.MaxAmount))
            {
                return Tuple.Create(false, "最大金额不合法或小于最小金额");
            }
            if (args.StartTimeBeg is not null && args.StartTimeBeg is not DateTime)
            {
                return Tuple.Create(false, "开始时间区间起点不合法");
            }
            if (args.StartTimeEnd is not null && args.StartTimeEnd is not DateTime)
            {
                return Tuple.Create(false, "开始时间区间终点不合法");
            }
            if (args.FinishTimeBeg is not null && args.FinishTimeBeg is not DateTime)
            {
                return Tuple.Create(false, "结束时间区间起点不合法");
            }
            if (args.FinishTimeEnd is not null && args.FinishTimeEnd is not DateTime)
            {
                return Tuple.Create(false, "结束时间区间终点不合法");
            }

            return TransactionLogsDAL.GetTransactionLogs(args);
        }
    }
}
