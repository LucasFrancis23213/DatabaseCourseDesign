using Oracle.ManagedDataAccess.Client;
using System.Data;
using SQLOperation.PublicAccess.Templates.SQLManager;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class RegisterDAL
    {
        private Connection conn;
        private OracleConnection OracleConnection;
        private static readonly string Uid = "ADMIN";
        private static readonly string Password = "123456";
        private static readonly string DataSource = "121.36.200.128:1521/ORCL";

        public RegisterDAL()
        {
            conn = new Connection(Uid, Password, DataSource);
            OracleConnection = conn.GetOracleConnection();
        }

        public Tuple<bool, string> InsertUser(string UserName, string Password, string Contact, string Status = "false")
        {
            try
            {
                string query = "INSERT INTO Users (User_Name, Password, Contact, Status) VALUES (@UserName, @Password, @Contact, @Status)";
                using OracleCommand command = new OracleCommand(query, OracleConnection);

                command.Parameters.Add(new OracleParameter("UserName", UserName));
                command.Parameters.Add(new OracleParameter("Password", Password));
                command.Parameters.Add(new OracleParameter("Contact", Contact));
                command.Parameters.Add(new OracleParameter("Status", Status));

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
