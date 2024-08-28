using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class TransactionLogsDAL : BaseLogDAL<Transaction_Logs>
    {
        protected override string TableName => "TRANSACTION_LOGS";

        protected override List<string> ColumnNames => 
            ["FROM_USER_ID", "TO_USER_ID", "ITEM_ID", "AMOUNT", "TRANSACTION_TYPE", "START_TIME", "FINISH_TIME", "STATUS"];

        protected override Func<OracleDataReader, Transaction_Logs> MapFromReader => reader => new Transaction_Logs
        {
            From_User_ID = reader.GetInt32(0),
            To_User_ID = reader.GetInt32(1),
            Item_ID = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty,
            Amount = reader.GetDouble(3),
            Transaction_Type = reader.GetString(4),
            Transaction_ID = reader.GetInt32(5),
            StartTime = reader.GetDateTime(6),
            FinishTime = !reader.IsDBNull(7) ? reader.GetDateTime(7) : default,
            Status = reader.GetString(8)
        };

        public Tuple<bool, string> GetTransactionLogs(QueryTransactionLogsArgs args)
        {
            var parameters = new Dictionary<(string ColumnName, string Operator), object>
            {
                { ("FROM_USER_ID", "="), args.FromUserID },
                { ("TO_USER_ID", "="), args.ToUserID },
                { ("ITEM_ID", "="), args.ItemID },
                { ("AMOUNT", ">="), args.MinAmount },
                { ("AMOUNT", "<="), args.MaxAmount },
                { ("TRANSACTION_TYPE", "="), args.TransactionType },
                { ("TRANSACTION_ID", "="), args.TransactionID },
                { ("START_TIME", ">="), args.StartTimeBeg },
                { ("START_TIME", "<="), args.StartTimeEnd },
                { ("FINISH_TIME", ">="), args.FinishTimeBeg },
                { ("FINISH_TIME", "<="), args.FinishTimeEnd },
                { ("STATUS", "="), args.Status }
            };

            return GetLogsAux(parameters);
        }

        public Tuple<bool, string> InsertNewLog(TransactionLogsInsertUtil args)
        {
            var values = new Dictionary<string, object>
            {
                { "FROM_USER_ID", args.FromUserID },
                { "TO_USER_ID", args.ToUserID },
                { "ITEM_ID", args.ItemID },
                { "AMOUNT", args.Amount },
                { "TRANSACTION_TYPE", args.TransactionType },
                { "START_TIME", args.StartTime },
                { "FINISH_TIME", args.FinishTime },
                { "STATUS", args.Status }
            };
            return InsertNewLogAux(values);
        }

        public Tuple<bool, string> UpdateLog(int TransactionID, string status, DateTime? FinishTime)
        {
            string[] UpdateColumn = ["STATUS", "FINISH_TIME"];
            object[] UpdateValue = [status, FinishTime];

            for (int i = 0; i < 2; i++)
            {
                if (UpdateValue[i] == null)
                {
                    continue;
                }

                var (IsSucceeded, Message) = BasicSQLOps.UpdateOperation(TableName, UpdateColumn[i], UpdateValue[i], "TRANSACTION_ID", TransactionID);

                // 连接错误导致的失败
                if (!IsSucceeded)
                {
                    return new Tuple<bool, string>(IsSucceeded, Message);
                }

                // 传入数据错误导致的失败
                if (Message != "更新了1行")
                {
                    return new Tuple<bool, string>(false, "未找到这个交易记录");
                }
            }
            return new Tuple<bool, string>(true, string.Empty);
        }
    }
}
