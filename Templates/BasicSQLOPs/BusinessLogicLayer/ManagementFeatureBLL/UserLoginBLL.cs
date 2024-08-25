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
            var queryResult = UserOperatorDAL.GetUserInfo(null, UserName);

            if (!queryResult.Item1)
            {
                return Tuple.Create(false, queryResult.Item2);
            }

            var users = JsonSerializer.Deserialize<List<Users>>(queryResult.Item2);

            if (users == null || users.Count == 0)
            {
                return Tuple.Create(false, "无法将Users类型转换为Json类型");
            }

            var user = users.FirstOrDefault(u => u.Is_Deleted == 0);

            if (user is null)
            {
                return Tuple.Create(false, "User表没有符合要求的元素");
            }

            return user.Password == PasswordEncryptor.EncryptPassword(Password)
                ? Tuple.Create(true, "用户名与密码匹配，登录成功")
                : Tuple.Create(false, "用户名与密码不匹配");
        }
    }
}
