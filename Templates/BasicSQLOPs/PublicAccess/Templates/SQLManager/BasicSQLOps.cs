using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;
using System.Data.OracleClient;
using System.Data;
using System.Security.Cryptography;
using System.Text.Json;
using SQLOperation.PublicAccess.Templates.TemplateInterfaceManager;


namespace SQLOperation.PublicAccess.Templates.SQLManager
{
    public class BasicSQLOps : IBasicSQLOps
    {
        public static void Main(string[] args)
        {
            //dbORM db = new dbORM();
            //db.getInstance();
            string DataSource = "(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))) (CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = orclpdb)))";
            Connection connection = new Connection("cjh2251646", "123456", "localhost:1521/orclpdb");
        }

        private Connection DatabaseConnector;
        private OracleConnection OracleConnection;
        //构造函数，在实现数据库操作之前先建立数据库连接
        public BasicSQLOps(Connection connection)
        {
            DatabaseConnector = connection;
            OracleConnection = DatabaseConnector.GetOracleConnection();
            //Console.WriteLine("database connected? "+DatabaseConnector.OracleConnection.State);
        }
        //模板函数：插入操作
        //TableName:string类型，插入的数据所在表的名字
        //ColumnName:string类型，插入的列的名字 
        public virtual Tuple<bool,string> InsertOperation(string TableName, List<string> ColumnName, List<object> Value)
        {
            if (OracleConnection.State == ConnectionState.Open)
            {
                //string InsertSQL = $"INSERT INTO {TableName.ToUpper()}({ColumnName.ToUpper()}) VALUES (:Value)";
                if(ColumnName.Count != Value.Count)
                {
                    string ErrorReason = "列名和值的数量不匹配";
                    Debug.WriteLine(ErrorReason);
                    return new Tuple<bool, string>(false, ErrorReason);
                }

                string Columns = string.Join(",", ColumnName.Select(c => c.ToUpper()));
                string parameter = string.Join(",", ColumnName.Select((c, index) => $":Value{index}"));
                string InsertSQL = $"INSERT INTO {TableName.ToUpper()} ({Columns}) VALUES ({parameter})";
                using (OracleCommand cmd = new OracleCommand(InsertSQL, OracleConnection))
                {
                    try
                    {
                        //这种语句不支持表名/列名
                        //cmd.Parameters.Add(new OracleParameter(":Value", Value ?? DBNull.Value));
                        for (int i = 0; i < Value.Count; i++)
                        {
                            object value = Value[i] ?? DBNull.Value;
                            Type type = value.GetType();
                            Debug.WriteLine(type);
                            if (value is DateTime dateTimeValue)
                            {
                                // 将 DateTime 转换为 Oracle 兼容的字符串格式
                                string oracleDate = dateTimeValue.ToString("yyyy/M/d HH:mm:ss");
                                Debug.WriteLine(oracleDate);
                                Debug.WriteLine(DateTime.Now);
                                cmd.Parameters.Add(new OracleParameter($":Value{i}", OracleDbType.Date)).Value = dateTimeValue;
                                //cmd.Parameters.Add(new OracleParameter($":Value{i}", OracleDbType.Date)).Value = DateTime.Now;
                            }
                            else
                                cmd.Parameters.Add(new OracleParameter($":Value{i}", Value[i]));

                        }
                        int AffectedNum = cmd.ExecuteNonQuery();
                        Debug.WriteLine($"第{AffectedNum}行被插入");
                        return new Tuple<bool, string>(true, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        string ErrorReason = ex.Message;
                        Debug.WriteLine($"插入报错：{ex}");
                        return new Tuple<bool, string>(false, ErrorReason);
                    }
                }
            }
            else
            {
                string ErrorReason = "数据库未连接";
                Debug.WriteLine("插入操作，",ErrorReason);
                return new Tuple<bool, string>(false, ErrorReason);
            }
        }
        //删除操作
        public virtual new Tuple<bool, string> DeleteOperation(string TableName, string ConditionColumn, object Value)
        {
            string ErrorReason = string.Empty;
            if (OracleConnection.State == ConnectionState.Open)
            {
                string DeleteSQL = $"DELETE FROM {TableName.ToUpper()} WHERE {ConditionColumn.ToUpper()}=:Value";
                using (OracleCommand cmd = new OracleCommand(DeleteSQL, OracleConnection))
                {
                    try
                    {
                        //这样写可以避免插入风险，不直接在sql语句里指定插入的值
                        cmd.Parameters.Add(new OracleParameter("Value", Value ?? DBNull.Value));
                        int AffectedRow = cmd.ExecuteNonQuery();
                        Debug.WriteLine($"共{AffectedRow}行被删除");
                        return new Tuple<bool, string>(true,AffectedRow.ToString());
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

        //查询操作
        //TableName:表名，即想查询的数据在哪个表
        //SelectColumn:列名，即想查询数据库中的哪一列
        //ConditionColumn和Value:共同组成where部分的条件判断
        public virtual Tuple <bool,string> QueryOperation(string TableName, string ConditionColumn, object Value)
        {
            bool IsQuerySuccess = true;
            if (OracleConnection.State == ConnectionState.Open)
            {
                //RowList:存储查询到的所有结果
                List<Dictionary<string, object>> RowList = new List<Dictionary<string, object>>();
                //GetAllColumn:获取想要查询的表的全部列名的SQL语句
                //tips:这里的表名必！须！大！写！否则查不到
                string GetAllColumn = $"select column_name from user_tab_columns where table_name = '{TableName.ToUpper()}'";
                //ColumnName:存储所有列名的列表
                try
                {
                    List<string> ColumnName = new List<string>();
                    using (OracleCommand cmd = new OracleCommand(GetAllColumn, OracleConnection))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ColumnName.Add(reader.GetString(0));
                            }
                        }
                    }
                    if (ColumnName.Count > 0)
                    {
                        //将列表里的列名转换为符合sql标准的语句
                        string ColumnsToBeSelected = string.Join(',', ColumnName);
                        //query:符合用户输入条件的查询sql语句
                        string query = $"SELECT {ColumnsToBeSelected.ToUpper()} FROM {TableName.ToUpper()} WHERE {ConditionColumn.ToUpper()} = :Value";
                        using (OracleCommand cmd = new OracleCommand(query, OracleConnection))
                        {
                            cmd.Parameters.Add(":Value", Value ?? DBNull.Value);
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Dictionary<string, object> SingleMatch = new Dictionary<string, object>();
                                    foreach (string name in ColumnName)
                                    {
                                        SingleMatch[name] = reader[name];
                                    }
                                    RowList.Add(SingleMatch);
                                }
                            }
                        }
                        if (RowList.Count == 0)
                        {
                            IsQuerySuccess = false;
                            return new Tuple<bool, string>(IsQuerySuccess, $"{TableName}表没有符合要求的元素");
                        }
                        else
                        {
                            //加上这个能让输出美观一些
                            var option = new JsonSerializerOptions { WriteIndented = true };
                            string JsonFormatResult = JsonSerializer.Serialize(RowList, option);
                            return new Tuple<bool, string>(IsQuerySuccess,JsonFormatResult);
                        }
                    }
                    else
                    {
                        IsQuerySuccess = false;
                        string ErrorReason = "当前表没有任何列";
                        Debug.WriteLine(ErrorReason);
                        return new Tuple<bool, string>(IsQuerySuccess,ErrorReason);
                    }
                }

                catch (Exception ex)
                {
                    IsQuerySuccess = false;
                    Debug.WriteLine(ex.Message);
                    return new Tuple<bool, string>(IsQuerySuccess, ex.Message);
                }
            }
            else
            {
                IsQuerySuccess = false;
                string ErrorReason = "数据库未连接";
                Debug.WriteLine("查询操作，数据库未连接");
                return new Tuple<bool, string>(IsQuerySuccess,ErrorReason);
            }
        }
        //更新操作
        public virtual Tuple<bool,string> UpdateOperation(string TableName, string UpdateColumn, object UpdateValue, string ConditionColumn, object ConditionValue)
        {
            if (OracleConnection.State == ConnectionState.Open)
            {
                string ErrorReason = string.Empty;
                string update = $"UPDATE {TableName.ToUpper()} SET {UpdateColumn.ToUpper()}= :UpdateValue WHERE {ConditionColumn.ToUpper()}=:ConditionValue";
                try
                {
                    using (OracleCommand cmd = new OracleCommand(update, OracleConnection))
                    {
                        cmd.Parameters.Add(new OracleParameter(":UpdateValue", UpdateValue ?? DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter(":ConditionValue", ConditionValue ?? DBNull.Value));
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

