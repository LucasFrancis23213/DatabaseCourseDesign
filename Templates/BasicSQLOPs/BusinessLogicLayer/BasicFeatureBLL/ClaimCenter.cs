using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Templates.TemplateInterfaceManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Drawing;
using Newtonsoft.Json;
using System.Reflection;

namespace SQLOperation.BusinessLogicLayer.BasicFeatureBLL
{
    public class ClaimCenter : IClaimCenter
    {
        private Connection conn;
        private OracleConnection OracleConnection;

        public ClaimCenter(string Uid = "ADMIN", string Password = "123456", string DataSource = "121.36.200.128:1521/ORCL")
        {
            conn = new Connection(Uid, Password, DataSource);
            OracleConnection = conn.GetOracleConnection();
        }
        
        public void ReleaseSQLConn()
        {
            try
            {
                conn.DisconnectSQL();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("in ClaimCenter", ex.Message);
            }               
        }

        public Tuple<bool, string> AddReturnItem(string itemID)
        {
            // ReturnItem对应Lost_Items
            string ErrorReason = string.Empty;
            if (OracleConnection.State == ConnectionState.Open)
            {
                string UpdateSQL = $"UPDATE LOST_ITEMS SET LOST_STATUS = 'RETURNING' WHERE ITEM_ID = '{itemID}'";

                using (OracleCommand cmd = new OracleCommand(UpdateSQL, OracleConnection))
                {
                    try
                    {
                        int rowsUpdated = cmd.ExecuteNonQuery();
                        if (rowsUpdated > 0)
                        {
                            return new Tuple<bool, string>(true, ErrorReason);
                        }
                        else
                        {
                            ErrorReason = "未找到对应的记录进行更新";
                            return new Tuple<bool, string>(false, ErrorReason);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorReason = ex.Message;
                        Debug.Write($"更新失败,报错为：{ErrorReason}");
                        return new Tuple<bool, string>(false, ErrorReason);
                    }
                }
            }
            else
            {
                ErrorReason = "数据库未连接";
                Debug.WriteLine("更新操作失败：", ErrorReason);
                return new Tuple<bool, string>(false, ErrorReason);

            }


        }

        public Tuple<bool, string> AddClaimItem(string itemID)
        {
            // ClaimItem对应Found_Items
            string ErrorReason = string.Empty;
            if (OracleConnection.State == ConnectionState.Open)
            {
                string UpdateSQL = $"UPDATE FOUND_ITEMS SET MATCH_STATUS = 'CLAIMING' WHERE ITEM_ID = '{itemID}'";

                using (OracleCommand cmd = new OracleCommand(UpdateSQL, OracleConnection))
                {
                    try
                    {
                        int rowsUpdated = cmd.ExecuteNonQuery();
                        if (rowsUpdated > 0)
                        {
                            return new Tuple<bool, string>(true, ErrorReason);
                        }
                        else
                        {
                            ErrorReason = "未找到对应的记录进行更新";
                            return new Tuple<bool, string>(false, ErrorReason);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorReason = ex.Message;
                        Debug.Write($"更新失败,报错为：{ErrorReason}");
                        return new Tuple<bool, string>(false, ErrorReason);
                    }
                }
            }
            else
            {
                ErrorReason = "数据库未连接";
                Debug.WriteLine("更新操作失败：", ErrorReason);
                return new Tuple<bool, string>(false, ErrorReason);

            }
        }

        public Tuple<bool, string> QueryItem(int type, Dictionary<string, object> index)
        {
            string TableName = "";
            if (index == null)
            {
                string errorReason = "查询索引不能为空！";
                return new Tuple<bool, string>(false, errorReason);
            }
            else
            {
                if (type == 0)
                { TableName = "Lost_Items"; }
                else if (type == 1)
                { TableName = "Found_Items"; }
                else
                {
                    string errorReason = "不合法的type值，请输入0/1！";
                    return new Tuple<bool, string>(false, errorReason);
                }
                string ErrorReason = string.Empty;
                if (OracleConnection.State == ConnectionState.Open)
                {
                    string ConditionString = string.Join(" OR ", index.Keys.Select(key => $"{key.ToUpper()} = :{key.ToUpper()}"));
                    string QuerySQL = "";
                    if (type == 0)
                    {
                        QuerySQL = $"SELECT * FROM {TableName.ToUpper()} L INNER JOIN REWARD_OFFERS R ON L.item_id = R.item_id INNER JOIN ITEM_CLAIM_PROCESSES C ON L.item_id = C.item_id WHERE (C.{ConditionString}) AND C.STATUS = 'WORKING'";
                    }
                    else
                    {
                        QuerySQL = $"SELECT * FROM {TableName.ToUpper()} NATURAL JOIN ITEM_CLAIM_PROCESSES WHERE ({ConditionString}) AND STATUS = 'WORKING'";
                    }


                    using (OracleCommand cmd = new OracleCommand(QuerySQL, OracleConnection))
                    {
                        foreach (var condition in index)
                        {
                            cmd.Parameters.Add(new OracleParameter(condition.Key.ToUpper(), condition.Value ?? DBNull.Value));
                        }

                        try
                        {
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                var results = new List<Dictionary<string, object>>();
                                while (reader.Read())
                                {
                                    var row = new Dictionary<string, object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                    }
                                    results.Add(row);
                                }
                                return new Tuple<bool, string>(true, JsonConvert.SerializeObject(results));
                            }
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

        public Tuple<bool, string> SignReturnAgreement(string itemID, int userID)
        {
            string ErrorReason = string.Empty;
            if (OracleConnection.State == ConnectionState.Open)
            {
                string UpdateSQL = $"UPDATE ITEM_RETURN_AGREEMENTS SET UPDATED_FLAG = 1, UPDATE_VALUE = {userID} WHERE ITEM_ID = '{itemID}'";

                using (OracleCommand cmd = new OracleCommand(UpdateSQL, OracleConnection))
                {
                    try
                    {
                        int rowsUpdated = cmd.ExecuteNonQuery();
                        if (rowsUpdated > 0)
                        {
                            return new Tuple<bool, string>(true, ErrorReason);
                        }
                        else
                        {
                            ErrorReason = "未找到对应的记录进行更新";
                            return new Tuple<bool, string>(false, ErrorReason);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorReason = ex.Message;
                        Debug.Write($"更新失败,报错为：{ErrorReason}");
                        return new Tuple<bool, string>(false, ErrorReason);
                    }
                }
            }
            else
            {
                ErrorReason = "数据库未连接";
                Debug.WriteLine("更新操作失败：", ErrorReason);
                return new Tuple<bool, string>(false, ErrorReason);

            }
        }

        public Tuple<bool, Tuple<bool,string>> CheckSignStatus(string itemID, int userID)
        {
            bool SignStatus = false;
            string ErrorReason = string.Empty;
            if (OracleConnection.State == ConnectionState.Open)
            {
                string QuerySQL = $"SELECT A_USER_ID, B_USER_ID FROM ITEM_CLAIM_PROCESSES NATURAL JOIN ITEM_RETURN_AGREEMENTS WHERE ITEM_ID = '{itemID}'";

                using (OracleCommand cmd = new OracleCommand(QuerySQL, OracleConnection))
                {
                    try
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            var results = new List<Dictionary<string, object>>();
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                                results.Add(row);
                            }
                            Debug.WriteLine($"查询成功，结果为：{JsonConvert.SerializeObject(results)}");
                            SignStatus = results.Any(result =>
                                                    Convert.ToInt32(result["A_USER_ID"]) == Convert.ToInt32(userID) ||
                                                    Convert.ToInt32(result["B_USER_ID"]) == Convert.ToInt32(userID));

                            Debug.WriteLine($"SignStatus: {SignStatus}");

                            Tuple<bool, string> res = new Tuple<bool, string>(SignStatus, ErrorReason);
                            return new Tuple<bool, Tuple<bool, string>>(true, res);

                        }
                    }

                    catch (Exception ex)
                    {
                        ErrorReason = ex.Message;
                        Debug.Write($"查找失败,报错为：{ErrorReason}");
                        Tuple<bool, string> res = new Tuple<bool, string>(false, ErrorReason);
                        return new Tuple<bool, Tuple<bool, string>>(false, res);
                    }
                }
            }
            else
            {
                ErrorReason = "数据库未连接";
                Debug.WriteLine("查找操作，", ErrorReason);
                Tuple<bool, string> res = new Tuple<bool, string>(false, ErrorReason);
                return new Tuple<bool, Tuple<bool, string>>(false, res);
            }
        }

        public Tuple<bool, string> DeleteItem(string itemID)
        {
            string ErrorReason = string.Empty;
            if (OracleConnection.State == ConnectionState.Open)
            {
                string DeleteSQL = $"DELETE FROM ITEM_CLAIM_PROCESSES WHERE ITEM_ID = {itemID}";

                using (OracleCommand cmd = new OracleCommand(DeleteSQL, OracleConnection))
                {
                    try
                    {
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
}
