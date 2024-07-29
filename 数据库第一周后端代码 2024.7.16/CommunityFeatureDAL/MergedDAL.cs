using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Templates.TemplateInterfaceManager;
using System.Data;
using System.Diagnostics;
using System.Text.Json;


namespace DatabaseProject.DataAccessLayer.CommunityFeatureDAL
{
    // 混合的DAL
    public class MergedDAL : IMergedDAL
    {
        private IBasicSQLOps basicSQLOps;
        private Connection connection;
        private OracleConnection oConn;

        public MergedDAL(Connection Connection)
        {
            basicSQLOps = new BasicSQLOps(Connection);
            connection = Connection;
            oConn = Connection.GetOracleConnection();

        }

        

        // 插入并返回指定列的值
        public Tuple<bool, string> InsertAndGetValue(string tableName, string returnValueColumn, List<string> columnNames, List<object> values)
        {
            if (oConn.State == ConnectionState.Open)
            {
                if (columnNames.Count != values.Count)
                {
                    string errorReason = "列名和值的数量不匹配";
                    Debug.WriteLine("列名和值的数量不匹配");
                    return new Tuple<bool, string>(false, errorReason);
                }

                try
                {
                    // 将所有列名转换为大写
                    columnNames = columnNames.Select(name => name.ToUpper()).ToList();
                    returnValueColumn = returnValueColumn.ToUpper();

                    // 构建 SQL INSERT 命令
                    string columns = string.Join(", ", columnNames);
                    string parameters = string.Join(", ", columnNames.Select((col, index) => $":p{index}"));

                    // 构建插入并返回指定列的 SQL 语句
                    string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters}) RETURNING {returnValueColumn} INTO :returnValue";

                    using (OracleCommand cmd = new OracleCommand(sql, oConn))
                    {
                        for (int i = 0; i < values.Count; i++)
                        {
                            cmd.Parameters.Add(new OracleParameter($":p{i}", values[i]));
                        }

                        // 添加返回值参数
                        // 假设返回值是 NUMBER 类型，转换为 int 输出
                        OracleParameter returnParameterInt = new OracleParameter(":returnValueInt", OracleDbType.Decimal, ParameterDirection.Output);

                        cmd.Parameters.Add(returnParameterInt);

                        // 执行命令
                        cmd.ExecuteNonQuery();

                        // 返回
                        string result = returnParameterInt.Value.ToString();
                        if (result == null)
                        {
                            return new Tuple<bool, string>(false, "获取失败");
                        }
                        else
                        {
                            return new Tuple<bool, string>(true, returnParameterInt.Value.ToString());
                        }


                    }

                }
                catch (Exception ex)
                {
                    string errorReason = $"插入报错：{ex}";
                    Debug.WriteLine($"插入报错：{ex}");
                    return new Tuple<bool, string>(false, errorReason);
                }


            }
            else
            {
                string errorReason = "数据库未连接成功";
                Debug.WriteLine("数据库未连接成功");
                return new Tuple<bool, string>(false, errorReason);
            }

        }

        //删除操作
        public Tuple<bool, string> DeleteTable(string TableName, Dictionary<string, object> Conditions)
        {
            string ErrorReason = string.Empty;
            if (oConn.State == ConnectionState.Open)
            {
                // 构建条件字符串，例如 "COLUMN1 = :COLUMN1 AND COLUMN2 = :COLUMN2"
                string ConditionString = string.Join(" AND ", Conditions.Keys.Select(key => $"{key.ToUpper()} = :{key.ToUpper()}"));
                string DeleteSQL = $"DELETE FROM {TableName.ToUpper()} WHERE {ConditionString}";

                using (OracleCommand cmd = new OracleCommand(DeleteSQL, oConn))
                {
                    try
                    {
                        // 添加参数
                        foreach (var condition in Conditions)
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

        // 构建条件字符串，支持 AND 和 OR
        private string BuildConditionString(Dictionary<string, object> conditions, string conditionType)
        {
            List<string> conditionParts = new List<string>();
            foreach (var condition in conditions)
            {
                conditionParts.Add($"{condition.Key.ToUpper()} = :{condition.Key.ToUpper()}");
            }

            string conditionString = string.Join($" {conditionType} ", conditionParts);
            return conditionString;
        }
        // 查询操作
        public Tuple<bool, string> QueryTable(string TableName, Dictionary<string, object> Conditions, string conditionType)
        {
            bool IsQuerySuccess = true;
            if (oConn.State == ConnectionState.Open)
            {
                // RowList: 存储查询到的所有结果
                List<Dictionary<string, object>> RowList = new List<Dictionary<string, object>>();
                // GetAllColumn: 获取想要查询的表的全部列名的SQL语句
                string GetAllColumn = $"SELECT column_name FROM user_tab_columns WHERE table_name = '{TableName.ToUpper()}'";
                // ColumnName: 存储所有列名的列表
                try
                {
                    List<string> ColumnName = new List<string>();
                    using (OracleCommand cmd = new OracleCommand(GetAllColumn, oConn))
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
                        // 将列表里的列名转换为符合SQL标准的语句
                        string ColumnsToBeSelected = string.Join(',', ColumnName);

                        // 构建条件字符串
                        string ConditionString = BuildConditionString(Conditions, conditionType);

                        // query: 符合用户输入条件的查询SQL语句
                        string query = $"SELECT {ColumnsToBeSelected.ToUpper()} FROM {TableName.ToUpper()} WHERE {ConditionString}";

                        using (OracleCommand cmd = new OracleCommand(query, oConn))
                        {
                            foreach (var condition in Conditions)
                            {
                                cmd.Parameters.Add(new OracleParameter($":{condition.Key.ToUpper()}", condition.Value ?? DBNull.Value));
                            }

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
                            // 不要报空错
                            IsQuerySuccess = true;
                            return new Tuple<bool, string>(IsQuerySuccess, null);
                        }
                        else
                        {
                            // 加上这个能让输出美观一些
                            var option = new JsonSerializerOptions { WriteIndented = true };
                            string JsonFormatResult = JsonSerializer.Serialize(RowList, option);
                            return new Tuple<bool, string>(IsQuerySuccess, JsonFormatResult);
                        }
                    }
                    else
                    {
                        IsQuerySuccess = false;
                        string ErrorReason = "当前表没有任何列";
                        Debug.WriteLine(ErrorReason);
                        return new Tuple<bool, string>(IsQuerySuccess, ErrorReason);
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
                return new Tuple<bool, string>(IsQuerySuccess, ErrorReason);
            }
        }
        // 查询操作，限制返回的行数
        // 通用查询操作，根据表名和自定义 WHERE 条件查询所有内容
        public Tuple<bool, string> QueryTableWithWhere(string TableName, string whereClause, OracleParameter[] parameters)
        {
            bool IsQuerySuccess = true;

            if (oConn.State == ConnectionState.Open)
            {
                // RowList: 存储查询到的所有结果
                List<Dictionary<string, object>> RowList = new List<Dictionary<string, object>>();

                try
                {
                    // 获取表的所有列名
                    List<string> ColumnName = new List<string>();
                    string GetAllColumn = $"SELECT column_name FROM user_tab_columns WHERE table_name = '{TableName.ToUpper()}'";

                    using (OracleCommand cmd = new OracleCommand(GetAllColumn, oConn))
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
                        string query;
                        if (whereClause == "1=1")
                        {
                            // 构建查询SQL语句，查询整个表
                            query = $"SELECT {string.Join(',', ColumnName)} FROM {TableName.ToUpper()}";
                        }
                        else
                        {
                            // 构建带有条件的查询SQL语句
                            query = $"SELECT {string.Join(',', ColumnName)} FROM {TableName.ToUpper()} WHERE {whereClause}";
                        }

                        // 使用参数化查询
                        using (OracleCommand cmd = new OracleCommand(query, oConn))
                        {
                            // 添加参数
                            if (parameters != null)
                            {
                                cmd.Parameters.AddRange(parameters);
                            }

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
                            // 不要报空错
                            IsQuerySuccess = true;
                            return new Tuple<bool, string>(IsQuerySuccess, null);
                        }
                        else
                        {
                            // 加上这个能让输出美观一些
                            var option = new JsonSerializerOptions { WriteIndented = true };
                            string JsonFormatResult = JsonSerializer.Serialize(RowList, option);
                            return new Tuple<bool, string>(IsQuerySuccess, JsonFormatResult);
                        }
                    }
                    else
                    {
                        IsQuerySuccess = false;
                        string ErrorReason = "当前表没有任何列";
                        Debug.WriteLine(ErrorReason);
                        return new Tuple<bool, string>(IsQuerySuccess, ErrorReason);
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
                return new Tuple<bool, string>(IsQuerySuccess, ErrorReason);
            }
        }

        // 多表查询
        // 查询操作，支持自定义 WHERE 和 FROM 子句，但 SELECT 子句固定为 *
        public Tuple<bool, string> QueryWithCustomFromAndWhere(string fromClause, string whereClause, OracleParameter[] parameters)
        {
            bool IsQuerySuccess = true;

            if (oConn.State == ConnectionState.Open)
            {
                // RowList: 存储查询到的所有结果
                List<Dictionary<string, object>> RowList = new List<Dictionary<string, object>>();

                try
                {
                    // 构建查询 SQL 语句
                    string query = $"SELECT * FROM {fromClause}";

                    // 添加 WHERE 子句
                    if (!string.IsNullOrEmpty(whereClause))
                    {
                        query += $" WHERE {whereClause}";
                    }

                    // 使用参数化查询
                    using (OracleCommand cmd = new OracleCommand(query, oConn))
                    {
                        // 添加参数
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            // 获取列名
                            var columnNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

                            while (reader.Read())
                            {
                                Dictionary<string, object> SingleMatch = new Dictionary<string, object>();
                                foreach (string name in columnNames)
                                {
                                    SingleMatch[name] = reader[name];
                                }
                                RowList.Add(SingleMatch);
                            }
                        }
                    }

                    if (RowList.Count == 0)
                    {
                        // 返回空结果
                        IsQuerySuccess = true;
                        return new Tuple<bool, string>(IsQuerySuccess, null);
                    }
                    else
                    {
                        // 序列化结果为 JSON 格式
                        var option = new JsonSerializerOptions { WriteIndented = true };
                        string JsonFormatResult = JsonSerializer.Serialize(RowList, option);
                        return new Tuple<bool, string>(IsQuerySuccess, JsonFormatResult);
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
                return new Tuple<bool, string>(IsQuerySuccess, ErrorReason);
            }
        }


        //更新操作
        public Tuple<bool, string> UpdateTable(string TableName, Dictionary<string, object> UpdateColumns, Dictionary<string, object> ConditionColumns)
        {
            if (oConn.State == ConnectionState.Open)
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
                foreach (var kvp in ConditionColumns)
                {
                    conditionClauses.Add($"{kvp.Key.ToUpper()} = :{kvp.Key}");
                }
                whereClause += string.Join(" AND ", conditionClauses);
                update += whereClause;

                try
                {
                    using (OracleCommand cmd = new OracleCommand(update, oConn))
                    {
                        // 添加更新列的参数
                        foreach (var kvp in UpdateColumns)
                        {
                            cmd.Parameters.Add(new OracleParameter($":{kvp.Key}", kvp.Value ?? DBNull.Value));
                        }

                        // 添加条件列的参数
                        foreach (var kvp in ConditionColumns)
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
