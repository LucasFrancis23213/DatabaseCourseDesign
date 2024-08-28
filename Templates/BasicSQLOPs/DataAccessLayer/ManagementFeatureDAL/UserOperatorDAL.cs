using Newtonsoft.Json;
using Renci.SshNet.Messages;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;
using System.Diagnostics;

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
        /// <param id="UserID">The userID of the user.</param>
        /// <param name="UserName">The username of the user.</param>
        /// <returns>A tuple containing a boolean indicating success and the query result as a string.</returns>
        public Tuple<bool, string> GetUserInfo(int? UserID, string? UserName)
        {
            return DoQuery(GetUserInfoGenerator(UserID, UserName));
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param id="UserID">The username of the user to be deleted.</param>
        /// <returns>A tuple containing a boolean indicating success and the result of the deletion as a string.</returns>
        public Tuple<bool, string> DeleteUser(int UserID)
        {
            return DoQuery(DeleteUserGenerator(UserID));
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
        public Tuple<bool, string> UpdateUserInfo(int UserID, string? UserName, string? Password, string? Contact)
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

        private Func<Tuple<bool, string>> UpdateUserInfoGenerator(int UserID, string? UserName, string? Password, string? Contact)
        {
            return () =>
            {
                List<string> UpdateColumn = ["User_Name", "Password_", "Contact"];
                List<string> UpdateValue = [UserName, Password, Contact];

                for (int i = 0; i < UpdateColumn.Count; i++)
                {
                    if (string.IsNullOrEmpty(UpdateValue[i]))
                    {
                        continue;
                    }

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

        private Func<Tuple<bool, string>> GetUserInfoGenerator(int? UserID, string? UserName)
        {
            return () =>
            {
                Tuple<bool, string>? QueryResultID = null;
                Tuple<bool, string>? QueryResultName = null;

                if (UserID is not null || UserID >= 0)
                {
                    QueryResultID = BasicSQLOps.QueryOperation("Users", "User_ID", UserID);
                }
                if (!string.IsNullOrEmpty(UserName))
                {
                    QueryResultName = BasicSQLOps.QueryOperation("Users", "User_Name", UserName);
                }

                bool SucceedID = QueryResultID is not null && QueryResultID.Item1;
                bool SucceedName = QueryResultName is not null && QueryResultName.Item1;

                if (!SucceedID && SucceedName)
                {
                    return QueryResultName;
                }
                else if (SucceedID && !SucceedName)
                {
                    return QueryResultID;
                }
                else if (SucceedID && SucceedName)
                {
                    try
                    {
                        var usersFromName = JsonConvert.DeserializeObject<List<Users>>(QueryResultName.Item2);

                        // 检查列表中是否有用户与 ID 匹配
                        if (usersFromName.Any(u => u.User_ID == UserID))
                        {
                            return QueryResultID;
                        }
                        else
                        {
                            return Tuple.Create(false, "用户名与用户ID不匹配");
                        }
                    }
                    catch (JsonException ex)
                    {
                        // 处理 JSON 反序列化错误
                        return Tuple.Create(false, "JSON反序列化错误: " + ex.Message);
                    }
                }
                else
                {
                    return Tuple.Create(false, "未找到用户");
                }
            };
        }

        private Func<Tuple<bool, string>> DeleteUserGenerator(int UserID)
        {
            return () =>
            {
                var (IsSucceeded, Message) = BasicSQLOps.UpdateOperation("Users", "IS_DELETED", 1, "USER_ID", UserID);

                if (!IsSucceeded)
                {
                    return new Tuple<bool, string>(IsSucceeded, Message);
                }

                if (Message != "更新了1行")
                {
                    return new Tuple<bool, string>(false, "未找到用户");
                }

                return Tuple.Create(true, "删除成功");
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