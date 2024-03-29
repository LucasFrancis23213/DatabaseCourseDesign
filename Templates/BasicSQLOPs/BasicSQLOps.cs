using System;
using System.Diagnostics;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;

namespace Templates
{
    public class BasicSQLOps:IBasicSQLOps
    {
        private Connection DatabaseConnector;

        //构造函数，在实现数据库操作之前先建立数据库连接
        public BasicSQLOps(Connection connection)
        {
            this.DatabaseConnector = connection;
        }
        //模板函数：插入操作
        public virtual bool InsertOperation(string TableName,string ColumnName,object Value)
        {
            if (DatabaseConnector.IsConnected)
            {
                string InsertSQL = $"INSERT INTO {TableName}({ColumnName}) VALUES (:Value);";
                using (OracleCommand cmd = new OracleCommand(InsertSQL))
                {
                    try
                    {
                        cmd.Parameters.Add(new OracleParameter("Value", Value ?? DBNull.Value));
                        int AffectedNum = cmd.ExecuteNonQuery();
                        Debug.WriteLine($"第{AffectedNum}行被插入");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"插入报错：{ex}");
                        return false;
                    }
                }
            }
            else
            {
                Debug.WriteLine("插入操作，数据库未连接");
                return false;
            }
        }
        //删除操作
        public virtual bool DeleteOperation(string TableName,string ColumnName,object Value)
        {
            if (DatabaseConnector.IsConnected)
            {
                string DeleteSQL = $"DELETE FROM {TableName} WHERE {ColumnName}=:Value";
                using(OracleCommand cmd = new OracleCommand(DeleteSQL))
                {
                    try
                    {
                        //这样写可以避免插入风险，不直接在sql语句里指定插入的值
                        cmd.Parameters.Add(new OracleParameter("Value", Value ?? DBNull.Value));
                        int AffectedRow=cmd.ExecuteNonQuery();
                        Debug.WriteLine($"第{AffectedRow}行被删除");
                        return true;
                    }
                    catch(Exception ex)
                    {
                        Debug.Write($"删除失败,报错为：{ex}");
                        return false;
                    }
                }
            }
            else
            {
                Debug.WriteLine("删除操作，数据库未连接");
                return false;
            }
        }

        //查询操作
        public virtual QueryResult QuerySQL(string TableName,string SelectColumn,string ConditionColumn,object Value)
        {
            if (DatabaseConnector.IsConnected)
            {
                QueryResult QueryResult = new QueryResult();
                string query = $"SELECT {SelectColumn} FROM {TableName} WHERE {ConditionColumn}=:Value";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    int AffectedRow = 0;
                    try
                    {
                        cmd.Parameters.Add(new OracleParameter("Value", Value ?? DBNull.Value));
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SingleResult SingleResult = new SingleResult()
                                {
                                    QueryName = SelectColumn,
                                    Result = reader[SelectColumn] ?? DBNull.Value
                                };
                                QueryResult.Results.Add(SingleResult);
                                AffectedRow = cmd.ExecuteNonQuery();
                            }
                        }
                        return QueryResult;
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine($"查询失败,报错为：{ex},最后一个被删除的为第{AffectedRow}行");
                        return null;
                    }
                }
            }
            else
            {
                Debug.WriteLine("查询操作，数据库未连接");
                return null;
            }
        }
        //更新操作
        public virtual bool UpdateSQL(string TableName,string UpdateColumn,object UpdateValue,string ConditionColumn,object ConditionValue)
        {
            if (DatabaseConnector.IsConnected)
            {
                string update = $"UPDATE {TableName} SET {UpdateColumn}= :UpdateValue WHERE {ConditionColumn}=: ConditionValue";
                try
                {
                    using (OracleCommand cmd = new OracleCommand(update))
                    {
                        cmd.Parameters.Add(new OracleParameter("UpdateValue", UpdateValue ?? DBNull.Value));
                        cmd.Parameters.Add(new OracleParameter("ConditionValue", ConditionValue ?? DBNull.Value));
                        int AffectedRow = cmd.ExecuteNonQuery() ;
                        Debug.WriteLine($"更新了第{AffectedRow}行");
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"更新操作，报错为：{ex}");
                    return false;
                }
            }
            else
            {
                Debug.WriteLine("更新操作，数据库未连接");
                return false;
            }
        }

    }
}

