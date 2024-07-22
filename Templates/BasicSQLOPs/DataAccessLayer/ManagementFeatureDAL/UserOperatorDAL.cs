namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class UserOperatorDAL : BaseDAL
    {
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

        /***********************************************************************
        * 修改用户信息
        * 输入参数: 
        *     UserID - 用户ID
        *     UserName - 用户名
        *     Password - 用户密码
        *     Contact - 用户联系方式
        * 返回值: 包含操作是否成功和修改结果的元组
        ***********************************************************************/
        public Tuple<bool, string> UpdateUserInfo(int UserID, string UserName, string Password, string Contact)
        {
            return DoQuery(UpdateUserInfoGenerator(UserID, UserName, Password, Contact));
        }

        private Func<Tuple<bool, string>> UpdateUserInfoGenerator(int UserID, string UserName, string Password, string Contact)
        {
            return () =>
            {
                List<string> UpdateColumn = ["User_Name", "Password_", "Contact"];
                List<object> UpdateValue = [UserName, Password, Contact];

                for (int i = 0; i < UpdateColumn.Count; i++)
                {
                    var (IsSucceeded, Message) = BasicSQLOps.UpdateOperation("Users", UpdateColumn[i], UpdateValue[i], "USER_ID", UserID);
                    if (!IsSucceeded)
                    {
                        return new Tuple<bool, string>(IsSucceeded, Message);
                    }
                }
                return new Tuple<bool, string>(true, string.Empty);
            };
        }

        private Func<Tuple<bool, string>> GetUserInfoGenerator(string UserName)
        {
            return () =>
            {
                Tuple<bool, string> QueryResult = BasicSQLOps.QueryOperation("Users", "User_Name", UserName);
                return QueryResult;
            };
        }

        private Func<Tuple<bool, string>> DeleteUserGenerator(string UserName)
        {
            return () =>
            {
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
        }

        private Func<Tuple<bool, string>> InsertUserGenerator(string UserName, string Password, string Contact)
        {
            return () =>
            {
                List<string> ColumnNames = ["User_Name", "Password_", "Contact"];
                List<object> Values = [UserName, Password, Contact];

                Tuple<bool, string> QueryResult = BasicSQLOps.InsertOperation("Users", ColumnNames, Values);
                return QueryResult;
            };
        }
    }
}
