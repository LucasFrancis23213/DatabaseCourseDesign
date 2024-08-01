namespace SQLOperation.DataAccessLayer.ManagementFeatureDAL
{
    public class UserOperatorDAL : BaseDAL
    {
        /// <summary>
        /// Get user ID by user name.
        /// </summary>
        /// <param ID="UserID">The userID of the user.</param>
        /// <returns>A boolean indicating existence.</returns>
        public bool CheckUserID(int UserID)
        {
            int MaxCheckTime = 5;
            bool IsSucceed;
            string Message;

            do
            {
                (IsSucceed, Message) = DoQuery(CheckUserIDGenerator(UserID));
            } while (!IsSucceed && Message != "Users表没有符合要求的元素" && --MaxCheckTime > 0);

            return IsSucceed;
        }

        /// <summary>
        /// Retrieves user information.
        /// </summary>
        /// <param name="UserName">The username of the user.</param>
        /// <returns>A tuple containing a boolean indicating success and the query result as a string.</returns>
        public Tuple<bool, string> GetUserInfo(string UserName)
        {
            return DoQuery(GetUserInfoGenerator(UserName));
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="UserName">The username of the user to be deleted.</param>
        /// <returns>A tuple containing a boolean indicating success and the result of the deletion as a string.</returns>
        public Tuple<bool, string> DeleteUser(string UserName)
        {
            return DoQuery(DeleteUserGenerator(UserName));
        }

        /// <summary>
        /// Inserts a new user.
        /// </summary>
        /// <param name="UserName">The username of the new user.</param>
        /// <param name="Password">The password of the new user.</param>
        /// <param name="Contact">The contact information of the new user.</param>
        /// <returns>A tuple containing a boolean indicating success and the result of the insertion as a string.</returns>
        public Tuple<bool, string> InsertUser(string UserName, string Password, string Contact)
        {
            return DoQuery(InsertUserGenerator(UserName, Password, Contact));
        }

        /// <summary>
        /// Updates user information.
        /// </summary>
        /// <param name="UserID">The ID of the user.</param>
        /// <param name="UserName">The new username of the user.</param>
        /// <param name="Password">The new password of the user.</param>
        /// <param name="Contact">The new contact information of the user.</param>
        /// <returns>A tuple containing a boolean indicating success and the result of the update as a string.</returns>
        public Tuple<bool, string> UpdateUserInfo(int UserID, string UserName, string Password, string Contact)
        {
            return DoQuery(UpdateUserInfoGenerator(UserID, UserName, Password, Contact));
        }

        private Func<Tuple<bool, string>> CheckUserIDGenerator(int UserID)
        {
            return () =>
            {
                Tuple<bool, string> QueryResult = BasicSQLOps.QueryOperation("Users", "User_ID", UserID);
                return QueryResult;
            };
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

                    // 连接错误导致的失败
                    if (!IsSucceeded)
                    {
                        return new Tuple<bool, string>(IsSucceeded, Message);
                    }

                    // 传入数据错误导致的失败
                    if (Message != "更新了1行")
                    {
                        return new Tuple<bool, string>(false, "未找到用户");
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
