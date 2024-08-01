using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Templates.SQLManager;
using System.Data;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public abstract class BaseDAL
    {
        protected readonly BasicSQLOps BasicSQLOps;
        private readonly Connection conn;
        protected readonly OracleConnection OracleConnection;
        private static readonly string Uid = "ADMIN";
        private static readonly string Password = "123456";
        private static readonly string DataSource = "121.36.200.128:1521/ORCL";

        protected BaseDAL()
        {
            conn = new Connection(Uid, Password, DataSource);
            BasicSQLOps = new BasicSQLOps(conn);
            OracleConnection = conn.GetOracleConnection();
        }

        protected Tuple<bool, string> DoQuery(Func<Tuple<bool, string>> action)
        {
            try
            {
                if (OracleConnection.State != ConnectionState.Open)
                {
                    OracleConnection.Open();
                }

                return action();
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
