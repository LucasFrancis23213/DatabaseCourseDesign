using System.Text.Json;
using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class UserLoginBLL
    {
        private readonly UserLoginDAL UserLoginDAL;
        public UserLoginBLL()
        {
            UserLoginDAL = new UserLoginDAL();
        }

        public Tuple<bool, Users, string> GetUserInfoUtil(string UserName)
        {
            Tuple<bool, string> QueryResult = UserLoginDAL.GetUserInfo(UserName);
            if (QueryResult.Item1)
            { 
                List<Users> users = JsonSerializer.Deserialize<List<Users>>(QueryResult.Item2);
                if (users != null && users.Count > 0)
                {
                    return new Tuple<bool, Users, string>(true, users[0], string.Empty);
                }
                else
                {
                    return new Tuple<bool, Users, string>(false, new Users(), "转换错误");
                }
            }
            else
            {
                return new Tuple<bool, Users, string>(false, new Users(), QueryResult.Item2);
            }
        }
    }
}
