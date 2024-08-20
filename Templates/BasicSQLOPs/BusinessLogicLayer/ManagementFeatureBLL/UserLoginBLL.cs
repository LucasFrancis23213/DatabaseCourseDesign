using System.Text.Json;
using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class UserLoginBLL
    {
        private UserOperatorDAL UserOperatorDAL;
        public UserLoginBLL()
        {
            UserOperatorDAL = new UserOperatorDAL();
        }

        public Tuple<bool, string> CheckPassword(string UserName, string Password)
        {
            Tuple<bool, string> QueryResult = UserOperatorDAL.GetUserInfo(null, UserName);
            Users Info;

            // get the target user's information
            if (QueryResult.Item1)
            { 
                var users = JsonSerializer.Deserialize<List<Users>>(QueryResult.Item2);
                if (users != null && users.Count > 0)
                {
                    Info = users[0];
                }
                else
                {
                    return new Tuple<bool, string>(false, "无法将Users类型转换为Json类型");
                }
            }
            else
            {
                return new Tuple<bool, string>(false, QueryResult.Item2);
            }

            if (Info.Password == PasswordEncryptor.EncryptPassword(Password))
            {
                return new Tuple<bool, string>(true, "用户名与密码匹配，登录成功");
            }
            else
            {
                return new Tuple<bool, string>(false, "用户名与密码不匹配");
            }
        }
    }
}
