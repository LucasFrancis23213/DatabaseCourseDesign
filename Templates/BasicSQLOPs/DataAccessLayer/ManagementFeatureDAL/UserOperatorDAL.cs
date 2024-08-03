using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Templates.SQLManager;
using System.Data;

namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class UserOperatorDAL
    {
        private BasicSQLOps BasicSQLOps;
        private Connection conn;
        private OracleConnection OracleConnection;
        private static readonly string Uid = "ADMIN";
        private static readonly string Password = "123456";
        private static readonly string DataSource = "121.36.200.128:1521/ORCL";

        public UserOperatorDAL()
        {
            conn = new Connection(Uid, Password, DataSource);
            BasicSQLOps = new BasicSQLOps(conn);
            OracleConnection = conn.GetOracleConnection();
        }

        /***********************************************************************
         * 获取用户信息
         * 输入参数: UserName - 用户名
         * 返回值: 包含操作是否成功和查询结果的元组
         ***********************************************************************/
        public Tuple<bool, string> GetUserInfo(string UserName)
        {
            return DoQuery(GetUserInfoGenerator(UserName));
        }

        /***********************************************************************
         * 删除用户
         * 输入参数: UserName - 用户名
         * 返回值: 包含操作是否成功和删除结果的元组
         ***********************************************************************/
        public Tuple<bool, string> DeleteUser(string UserName)
        {
            return DoQuery(DeleteUserGenerator(UserName));
        }

        /***********************************************************************
         * 插入用户
         * 输入参数: 
         *     UserName - 用户名
         *     Password - 用户密码
         *     Contact - 用户联系方式
         * 返回值: 包含操作是否成功和插入结果的元组
         ***********************************************************************/
        public Tuple<bool, string> InsertUser(string UserName, string Password, string Contact)
        {
            return DoQuery(InsertUserGenerator(UserName, Password, Contact));
        }




        // 以下为私有工具函数，Generator 中进行真正运行工具的定义，DoQuery 为所有操作的接口

        private Tuple<bool, string> DoQuery(Func<Tuple<bool, string>> action)
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

        private Func<Tuple<bool, string>> GetUserInfoGenerator(string UserName)
        {
            Func<Tuple<bool, string>> GetUserInfoHandler = delegate {
                Tuple<bool, string> QueryResult = BasicSQLOps.QueryOperation("Users", "User_Name", UserName);
                return QueryResult;
            };

            return GetUserInfoHandler;
        }

        private Func<Tuple<bool, string>> DeleteUserGenerator(string UserName)
        {
            Func<Tuple<bool, string>> DeleteUserHandler = delegate {
                Tuple<bool, string> DeleteResult = BasicSQLOps.DeleteOperation("Users", "User_Name", UserName);

                if (!DeleteResult.Item1)
                { 
                    return DeleteResult; 
                }
                else
                {
                    int AffectedRow = int.Parse(DeleteResult.Item2);
                    if (AffectedRow == 0)
                    {
                        return new Tuple<bool, string>(false, "数据库中不存在此行");
                    }

                    return new Tuple<bool, string>(true, "");
                }
            };

            return DeleteUserHandler;
        }

        private Func<Tuple<bool, string>> InsertUserGenerator(string UserName, string Password, string Contact)
        {

            Func<Tuple<bool, string>> DeleteUserHandler = delegate {

                List<string> ColumnNames = ["User_Name", "Password_", "Contact"];
                List<object> Values = [UserName, Password, Contact];

                Tuple<bool, string> QueryResult = BasicSQLOps.InsertOperation("Users", ColumnNames, Values);
                return QueryResult;
            };

            return DeleteUserHandler;
        }
    }
}
