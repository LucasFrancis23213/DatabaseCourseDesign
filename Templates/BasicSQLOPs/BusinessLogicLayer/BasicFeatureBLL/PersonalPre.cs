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
    internal class PersonalPre:IPersonalPre
    {
        private Connection conn;
        private OracleConnection OracleConnection;
        //private BasicSQLOps SQLOps;
        public PersonalPre(string Uid = "ADMIN", string Password = "123456", string DataSource = "121.36.200.128:1521/ORCL")
        {
            conn = new Connection(Uid, Password, DataSource);
            OracleConnection = conn.GetOracleConnection();
            //SQLOps = new BasicSQLOps(conn);
        }
        //插入一条新的用户偏好选择表
        public Tuple<bool, string> UserPreferencesBasic(User_Preferences item)
        {
            var Names = new List<string>
            {
                "Preference_ID",
                "User_ID",
                "Preference_Type",
                "Preference_Value",
                "Release_Date",
            };
            var values = new List<object>
            {
                item.Preference_ID,
                item.User_ID,
                item.Preference_Type,
                item.Preference_Value,
                item.Release_Date,
            };
            BasicSQLOps basic = new BasicSQLOps(conn);
            var result = basic.InsertOperation("User_Preferences", Names, values);
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
        public Tuple<bool, string> UserSubscriptionsBasic(User_Subscriptions item)
        {
            var Names = new List<string>
            {
                "Subscription_ID",
                "User_ID",
                "Subsciption_Type",
                "Subsciption_Status",
                "Release_Date",
            };
            var values = new List<object>
            {
                item.Subscription_ID,
                item.User_ID,
                item.Subsciption_Type,
                item.Subsciption_Status,
                item.Release_Date,
            };
            BasicSQLOps basic = new BasicSQLOps(conn);
            var result = basic.InsertOperation("User_Subscriptions", Names, values);
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
        public Tuple<bool, string> QueryItem(string TableName, Dictionary<string, object> index)
        {
            if (index == null)
            {
                if (TableName == "User_Preferences" || (TableName == "User_Subscriptions")) { }
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
                if (TableName == "User_Preferences" || (TableName == "User_Subscriptions")) { }
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
    }
}
