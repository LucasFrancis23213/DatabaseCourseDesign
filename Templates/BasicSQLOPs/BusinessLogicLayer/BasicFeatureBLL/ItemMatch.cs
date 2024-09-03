using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.InterfaceManager.BasicFeatureInterface;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace SQLOperation.BusinessLogicLayer.BasicFeatureBLL
{
    
    public class ItemMatch:IItemMatch
    {
        private Connection conn;
        private OracleConnection OracleConnection;
        //private BasicSQLOps SQLOps;
        public ItemMatch(string Uid = "ADMIN", string Password = "123456", string DataSource = "121.36.200.128:1521/ORCL")
        {
            conn = new Connection(Uid, Password, DataSource);
            OracleConnection = conn.GetOracleConnection();
            //SQLOps = new BasicSQLOps(conn);
        }
        //增加一条物品声明记录
        public Tuple<bool, string> ItemClaimProcessBasic(Item_Claim_Processes item){
            var Names = new List<string>
            {
                "Process_ID",
                "Item_ID",
                "Claimant_User_ID",
                "Status",
                "Application_Date",
            };
            var values = new List<object>
            {
                item.Process_ID,
                item.Item_ID,
                item.Claimant_User_ID,
                item.Status,
                item.Application_Date,
            };
            BasicSQLOps basic = new BasicSQLOps(conn);
            var result = basic.InsertOperation("Item_Claim_Processes", Names, values);
            bool isSuccess = result.Item1; // 获取是否成功插入
            string errorReason = result.Item2; // 获取出错误原因
            if (isSuccess)
                return new Tuple<bool, string>(true, string.Empty);
            //插入失败
            else
            {
                return new Tuple<bool, string>(false, errorReason);
            }
        }
        //增加一条匹配记录
        private Tuple<bool, string> MatchRecordBasic(Match_Records item)
        {
            var Names = new List<string>
            {
                "Record_ID",
                "Lost_Item_ID",
                "Found_Item_ID",
                "Match_Date",
                "Processing_Status",
            };
            var values = new List<object>
            {
                item.Record_ID,
                item.Record_ID,
                item.Found_Item_ID,
                item.Match_Date,
                item.Processing_Status,
            };
            BasicSQLOps basic = new BasicSQLOps(conn);
            var result = basic.InsertOperation("Match_Record", Names, values);
            bool isSuccess = result.Item1; // 获取是否成功插入
            string errorReason = result.Item2; // 获取出错误原因
            if (isSuccess)
                return new Tuple<bool, string>(true, string.Empty);
            //插入失败
            else
            {
                return new Tuple<bool, string>(false, errorReason);
            }
        }
        //增加一条历史变更
        private Tuple<bool, string> ItemStatusHistoryBasic(Item_Status_History item)
        {
            var Names = new List<string>
            {
                "History_ID",
                "Change_Date",
                "Item_ID",
                "Preview_Status",
                "New_Status",
            };
            var values = new List<object>
            {
                item.History_ID,
                item.Change_Date,
                item.Item_ID,
                item. Preview_Status,
                item.New_Status,
            };
            BasicSQLOps basic = new BasicSQLOps(conn);
            var result = basic.InsertOperation("Item_Status_history", Names, values);
            bool isSuccess = result.Item1; // 获取是否成功插入
            string errorReason = result.Item2; // 获取出错误原因
            if (isSuccess)
                return new Tuple<bool, string>(true, string.Empty);
            //插入失败
            else
            {
                return new Tuple<bool, string>(false, errorReason);
            }
        }
        //增加一条交换记录
        private Tuple<bool, string> ItemExchangeBasic(Item_Exchanges item)
        {
            var Names = new List<string>
            {
                "Exchange_ID",
                "Lost_Item_ID",
                "Found_Item_ID",
                "Initiator_User_ID",
                "Transaction_Type",
                "Responder_User_ID",
                "Exchange_Status",
                "Creation_time"
            };
            var values = new List<object>
            {
                item. Exchange_ID,
                item. Lost_Item_ID,
                item.Found_Item_ID,
                item.Initiator_User_ID,
                item.Transaction_Type,
                item.Responder_User_ID,
                item.Exchange_Status,
                item.Creation_Time,
            };
            BasicSQLOps basic = new BasicSQLOps(conn);
            var result = basic.InsertOperation("Item_Exchanges", Names, values);
            bool isSuccess = result.Item1; // 获取是否成功插入
            string errorReason = result.Item2; // 获取出错误原因
            if (isSuccess)
                return new Tuple<bool, string>(true, string.Empty);
            //插入失败
            else
            {
                return new Tuple<bool, string>(false, errorReason);
            }
        }
        //增加一条协议记录
        public Tuple<bool, string> ItemReturnAgreementBasic(Item_Return_Agreements item)
        {
            var Names = new List<string>
            {
                "Agreement_ID",
                "Item_ID",
                "From_User_ID",
                "To_User_ID",
                "Agreement_Content",
                "Exchange_Status",
                "Exchange_Status",
                "Creation_time"
            };
            var values = new List<object>
            {
                item. Agreement_ID,
                item. Item_ID,
                item.From_User_ID,
                item.To_User_ID,
                item.Agreement_Content,
                item.Exchange_Status,
                item.Creation_Time,
            };
            BasicSQLOps basic = new BasicSQLOps(conn);
            var result = basic.InsertOperation("Item_Return_Agreements", Names, values);
            bool isSuccess = result.Item1; // 获取是否成功插入
            string errorReason = result.Item2; // 获取出错误原因
            if (isSuccess)
                return new Tuple<bool, string>(true, string.Empty);
            //插入失败
            else
            {
                return new Tuple<bool, string>(false, errorReason);
            }
        }
        //查找一条记录
        public Tuple<bool, string> QueryItem(string TableName, Dictionary<string, object> index)
        {

            if (index == null)
            {
                if (TableName == "Match_Records" || (TableName == "Item_Status_History") || (TableName == "Item_Claim_Processes")) { }
                else
                {
                    string errorReason = "不合法的TableName值！";
                    return new Tuple<bool, string>(false, errorReason);
                }
                string ErrorReason = string.Empty;
                if (OracleConnection.State == ConnectionState.Open)
                {

                    string DeleteSQL = $"SELECT * FROM {TableName.ToUpper()}";
                    using (OracleCommand cmd = new OracleCommand(DeleteSQL, OracleConnection))
                    {
                        try
                        {
                            // 添加参数
                            foreach (var condition in index)
                            {
                                cmd.Parameters.Add(new OracleParameter(condition.Key.ToUpper(), condition.Value ?? DBNull.Value));
                            }
                            int AffectedRow = cmd.ExecuteNonQuery();
                            Debug.WriteLine($"共{AffectedRow}行被查找");
                            if (AffectedRow == 0)
                            {
                                return new Tuple<bool, string>(false, "没有查找到相应内容");
                            }
                            return new Tuple<bool, string>(true, ErrorReason);
                        }
                        catch (Exception ex)
                        {
                            ErrorReason = ex.Message;
                            Debug.Write($"查找失败,报错为：{ErrorReason}");
                            return new Tuple<bool, string>(false, ErrorReason);
                        }
                    }
                }
                else
                {
                    ErrorReason = "数据库未连接";
                    Debug.WriteLine("查找操作，", ErrorReason);
                    return new Tuple<bool, string>(false, ErrorReason);
                }
            }
            else
            {
                if (TableName == "Match_Records" || (TableName == "Item_Status_History") || (TableName == "Item_Claim_Processes")) { }
                else
                {
                    string errorReason = "不合法的TableName值！";
                    return new Tuple<bool, string>(false, errorReason);
                }
                string ErrorReason = string.Empty;
                if (OracleConnection.State == ConnectionState.Open)
                {
                    string ConditionString = string.Join(" AND ", index.Keys.Select(key => $"{key.ToUpper()} = :{key.ToUpper()}"));
                    string DeleteSQL = $"SELECT * FROM {TableName.ToUpper()} WHERE {ConditionString}";

                    using (OracleCommand cmd = new OracleCommand(DeleteSQL, OracleConnection))
                    {
                        try
                        {
                            // 添加参数
                            foreach (var condition in index)
                            {
                                cmd.Parameters.Add(new OracleParameter(condition.Key.ToUpper(), condition.Value ?? DBNull.Value));
                            }
                            int AffectedRow = cmd.ExecuteNonQuery();
                            Debug.WriteLine($"共{AffectedRow}行被查找");
                            if (AffectedRow == 0)
                            {
                                return new Tuple<bool, string>(false, "没有查找到相应内容");
                            }
                            return new Tuple<bool, string>(true, ErrorReason);
                        }
                        catch (Exception ex)
                        {
                            ErrorReason = ex.Message;
                            Debug.Write($"查找失败,报错为：{ErrorReason}");
                            return new Tuple<bool, string>(false, ErrorReason);
                        }
                    }
                }
                else
                {
                    ErrorReason = "数据库未连接";
                    Debug.WriteLine("查找操作，", ErrorReason);
                    return new Tuple<bool, string>(false, ErrorReason);
                }
            }
        }
        //删除一条物品声明记录
        public Tuple<bool, string> DeleteItem(string TableName,Dictionary<string, object> index)
        {
            if (index == null)
            {
                string errorReason = "删除索引不能为空！";
                return new Tuple<bool, string>(false, errorReason);
            }

            else
            {
                string ErrorReason = string.Empty;
                if (OracleConnection.State == ConnectionState.Open)
                {
                    string ConditionString = string.Join(" AND ", index.Keys.Select(key => $"{key.ToUpper()} = :{key.ToUpper()}"));
                    string DeleteSQL = $"DELETE FROM {TableName.ToUpper()} WHERE {ConditionString}";

                    using (OracleCommand cmd = new OracleCommand(DeleteSQL, OracleConnection))
                    {
                        try
                        {
                            // 添加参数
                            foreach (var condition in index)
                            {
                                cmd.Parameters.Add(new OracleParameter(condition.Key.ToUpper(), condition.Value ?? DBNull.Value));
                            }
                            int AffectedRow = cmd.ExecuteNonQuery();
                            Debug.WriteLine($"共{AffectedRow}行被删除");
                            if (AffectedRow == 0)
                            {
                                return new Tuple<bool, string>(false, "删除无效");
                            }
                            return new Tuple<bool, string>(true, ErrorReason);
                        }
                        catch (Exception ex)
                        {
                            ErrorReason = ex.Message;
                            Debug.Write($"删除失败,报错为：{ErrorReason}");
                            return new Tuple<bool, string>(false, ErrorReason);
                        }
                    }
                }
                else
                {
                    ErrorReason = "数据库未连接";
                    Debug.WriteLine("删除操作，", ErrorReason);
                    return new Tuple<bool, string>(false, ErrorReason);
                }
            }
        }
        //更新记录
        public Tuple<bool, string> UpdateItem(string TableName, Dictionary<string, object> UpdateColumns, Dictionary<string, object> index)
        {
            if (OracleConnection.State == ConnectionState.Open)
            {
                string ErrorReason = string.Empty;
                string update = $"UPDATE {TableName.ToUpper()} SET ";

                // 构建 SET 子句
                List<string> updateClauses = new List<string>();
                foreach (var kvp in UpdateColumns)
                {
                    updateClauses.Add($"{kvp.Key.ToUpper()} = :{kvp.Key}");
                }
                update += string.Join(", ", updateClauses);

                // 构建 WHERE 子句
                string whereClause = " WHERE ";
                List<string> conditionClauses = new List<string>();
                foreach (var kvp in index)
                {
                    conditionClauses.Add($"{kvp.Key.ToUpper()} = :{kvp.Key}");
                }
                whereClause += string.Join(" AND ", conditionClauses);
                update += whereClause;

                try
                {
                    using (OracleCommand cmd = new OracleCommand(update, OracleConnection))
                    {
                        // 添加更新列的参数
                        foreach (var kvp in UpdateColumns)
                        {
                            cmd.Parameters.Add(new OracleParameter($":{kvp.Key}", kvp.Value ?? DBNull.Value));
                        }

                        // 添加条件列的参数
                        foreach (var kvp in index)
                        {
                            cmd.Parameters.Add(new OracleParameter($":{kvp.Key}", kvp.Value ?? DBNull.Value));
                        }

                        int AffectedRow = cmd.ExecuteNonQuery();
                        Debug.WriteLine($"更新了{AffectedRow}行");

                        return new Tuple<bool, string>(true, $"更新了{AffectedRow}行");
                    }
                }
                catch (Exception ex)
                {
                    ErrorReason = $"更新操作，报错为：{ex.Message}";
                    Debug.WriteLine(ErrorReason);
                    return new Tuple<bool, string>(false, ErrorReason);
                }
            }
            else
            {
                Debug.WriteLine("更新操作，数据库未连接");
                return new Tuple<bool, string>(false, "数据库未连接");
            }
        }
        //更新一条记录状态（item_state_history）
        static Item_Status_History GetLatestRecord(OracleConnection conn, string itemId)
        {
            string query = @"
            SELECT History_ID, Change_Date, Item_ID, Preview_Status, New_Status
            FROM Item_Status_History
            WHERE Item_ID = :itemId
            ORDER BY Change_Date DESC
            FETCH FIRST 1 ROWS ONLY";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Parameters.Add(":itemId", OracleDbType.Int32).Value = itemId;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Item_Status_History
                        {
                            History_ID = reader.GetInt32(0).ToString(),
                            Change_Date = reader.GetDateTime(1),
                            Item_ID = reader.GetInt32(2).ToString(),
                            Preview_Status = reader.GetString(3),
                            New_Status = reader.GetString(4)
                        };
                    }
                }
            }

            return null;
        }
        //看似是个update 实则还是插入新纪录
        public Tuple<bool, string> UpdateHistory(OracleConnection conn, string itemId,string historyId, string newStatus)
        {
            
            Item_Status_History latestRecord = GetLatestRecord(conn, itemId);
            Item_Status_History my=new Item_Status_History
            {
                History_ID = historyId,
                Change_Date = DateTime.Now,
                Item_ID = itemId,
                Preview_Status = latestRecord.New_Status,
                New_Status = newStatus
            };
            //插入新的记录
            Tuple<bool, string> NewQuery = ItemStatusHistoryBasic(my);
            return NewQuery;
        }
        //查找
        public Tuple<bool, string> AutoMatch(List<string> Status, List<Match_Records> MatchRecord)
        {
            int n = 0;
            try
            {
               
                foreach (Match_Records item in MatchRecord)
                {
                    //在确认已经完成点击这是我的之后进行操作
                    if (Status[n] == "审核通过")
                    {
                        //匹配记录写在数据库
                        var basicExcel = MatchRecordBasic(item);
                        bool isSuccess1 = basicExcel.Item1; // 获取是否成功插入
                        string errorReason1 = "表单写入数据库过程中" + basicExcel.Item2; // 获取出错误原因
                       
                    }
                    else
                    {
                        string errorReason = "当前声明还没有审核通过，无法进行匹配";
                        return new Tuple<bool, string>(false, errorReason);
                    }
                    
                }//end of foreach
                return new Tuple<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                //其他未处理的异常
                return new Tuple<bool, string>(false, "自动匹配发生异常: " + ex.Message);
            }

        }
        public Tuple<bool, string> ExchangeItem(List<string> Status,List<Item_Exchanges> exchangeItems)
        {
            int n = 0;
            try
            {

                foreach (Item_Exchanges item in exchangeItems)
                {
                    //在确认已经阅读完毕协议之后进行操作
                    if (Status[n] == "已接受")
                    {
                        //匹配记录写在数据库
                        var basicExcel = ItemExchangeBasic(item);
                        bool isSuccess1 = basicExcel.Item1; // 获取是否成功插入
                        string errorReason1 = "表单写入数据库过程中" + basicExcel.Item2; // 获取出错误原因

                    }
                    else
                    {
                        string errorReason = "还没有完成协议阅读！";
                        return new Tuple<bool, string>(false, errorReason);
                    }

                }//end of foreach
                return new Tuple<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                //其他未处理的异常
                return new Tuple<bool, string>(false, "交换物品过程发生异常: " + ex.Message);
            }
        }
       
    }
}
