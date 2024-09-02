using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Utilities;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class UserAuthDAL : BaseDAL
    {
        public Tuple<bool, string> NewUserAuthed(Auth_Info NewAuth)
        {
            return DoQuery(InsertGenerator(NewAuth));
        }

        public Tuple<bool, string> DeleteAuthInfo(int UserID)
        {
            return DoQuery(DeleteGenerator(UserID));
        }

        public Tuple<bool, string> GetAuthInfo(int? UserID)
        {
            return DoQuery(GetInfoGenerator(UserID));
        }

        private Func<Tuple<bool, string>> InsertGenerator(Auth_Info NewAuth)
        {
            return () =>
            {
                List<string> ColumnNames = ["User_ID", "Auth_Date"];
                List<object> Values = [NewAuth.User_ID, NewAuth.Auth_Date];
                return BasicSQLOps.InsertOperation("AUTH_INFO", ColumnNames, Values);
            };
        }

        private Func<Tuple<bool, string>> DeleteGenerator(int UserID)
        {
            return () =>
            {
                return BasicSQLOps.DeleteOperation("AUTH_INFO", "User_ID", UserID);
            };
        }

        private Func<Tuple<bool, string>> GetInfoGenerator(int? UserID)
        {
            if (UserID is not null)
            {
                return () =>
                {
                    return BasicSQLOps.QueryOperation("AUTH_INFO", "User_ID", UserID);
                };
            }

            return () =>
            {
                var query = "SELECT * FROM AUTH_INFO";
                using OracleCommand cmd = new(query, OracleConnection);
                using OracleDataReader reader = cmd.ExecuteReader();
                var result = new List<Auth_Info>();
                while (reader.Read())
                {
                    result.Add(new Auth_Info
                    {
                        User_ID = reader.GetInt32(0),
                        Auth_Date = reader.GetDateTime(1),
                    });
                }
                string jsonResult = JsonConvert.SerializeObject(result);
                return new Tuple<bool, string>(true, jsonResult);
            };
        }
    }
}
