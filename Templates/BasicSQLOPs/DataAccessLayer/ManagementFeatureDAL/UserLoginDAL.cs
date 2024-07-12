using Oracle.ManagedDataAccess.Client;
using System.Data;
using SQLOperation.PublicAccess.Templates.SQLManager;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class UserLoginDAL
    {
        private BasicSQLOps BasicSQLOps;
        private Connection conn;
        private OracleConnection OracleConnection;
        private static readonly string Uid = "ADMIN";
        private static readonly string Password = "123456";
        private static readonly string DataSource = "121.36.200.128:1521/ORCL";

        public UserLoginDAL() 
        {
            conn = new Connection(Uid, Password, DataSource);
            BasicSQLOps = new BasicSQLOps(conn);
            OracleConnection = conn.GetOracleConnection();
        }

        public Tuple<bool, string> GetUserInfo(string UserName)
        {
            try
            {
                if (OracleConnection.State != ConnectionState.Open)
                {
                    OracleConnection.Open();
                }
            
                Tuple<bool, string> QueryResult = BasicSQLOps.QueryOperation("Users", "User_Name", UserName);
                return QueryResult; 
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
            finally
            {
                if (OracleConnection.State == ConnectionState.Open) 
                {
                    OracleConnection.Close();
                }
            }
        }
    }
}
