using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using SqlSugar;

namespace webapi
{
    public class dbORM
    {
        public SqlSugarClient getInstance()
        {
            SqlSugarClient db = new SqlSugarClient(connect_info);
            return db;
        }
        public static ConnectionConfig connect_info = new ConnectionConfig()
        {
            ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))" +
            "(CONNECT_DATA=(SERVICE_NAME=OralceMajorDatabase)));" +
            "Persist Security Info=True;User ID=system;Password=Ss504754572;",
            DbType = SqlSugar.DbType.Oracle,
            IsAutoCloseConnection = true,
            InitKeyType = SqlSugar.InitKeyType.Attribute
        };
    }
}