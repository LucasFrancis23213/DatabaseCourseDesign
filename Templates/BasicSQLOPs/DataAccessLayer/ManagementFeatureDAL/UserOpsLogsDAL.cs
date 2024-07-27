using Oracle.ManagedDataAccess.Client;
using System.Data;
using SQLOperation.PublicAccess.Templates.SQLManager;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class UserOpsLogsDAL
    {
        private Connection conn;
        private OracleConnection OracleConnection;
        private static readonly string Uid = "ADMIN";
        private static readonly string Password = "123456";
        private static readonly string DataSource = "121.36.200.128:1521/ORCL";

        public UserOpsLogsDAL()
        {
            conn = new Connection(Uid, Password, DataSource);
            OracleConnection = conn.GetOracleConnection();
        }

        public Tuple<bool, string> InsertUserOpsLogs(string UserID, string ActionType, string OccurrenceTime)
        {
            try
            {
                string query = "INSERT INTO Users (User_ID, Action_Type, Occurrence_Time) VALUES (@UserID, @ActionType, @OccurrenceTime)";
                using OracleCommand command = new OracleCommand(query, OracleConnection);

                command.Parameters.Add(new OracleParameter("UserID", UserID));
                command.Parameters.Add(new OracleParameter("ActionType", ActionType));
                command.Parameters.Add(new OracleParameter("OccurrenceTime", OccurrenceTime));

                if (OracleConnection.State != ConnectionState.Open)
                {
                    OracleConnection.Open();
                }

                int result = command.ExecuteNonQuery();
                if (result < 0)
                {
                    return new Tuple<bool, string>(false, "Error inserting data into Database");
                }
                else
                {
                    return new Tuple<bool, string>(true, string.Empty);
                }
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
