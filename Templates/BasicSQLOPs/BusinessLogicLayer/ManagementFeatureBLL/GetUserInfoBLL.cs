using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;
using System.Text.Json;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class GetUserInfoBLL
    {
        private UserOperatorDAL UserOperatorDAL;
        public GetUserInfoBLL() 
        { 
            UserOperatorDAL = new UserOperatorDAL();
        }

        public Tuple<bool, UserAccessibleInfo, string> GetInfo(string UserName)
        {
            Tuple<bool, string> QueryResult = UserOperatorDAL.GetUserInfo(UserName);

            if (QueryResult.Item1)
            {
                var users = JsonSerializer.Deserialize<List<Users>>(QueryResult.Item2);
                if (users != null && users.Count > 0)
                {
                    UserAccessibleInfo Info = new()
                    {
                        UserName = UserName,
                        UserID = users[0].User_ID,
                        Contact = users[0].Contact
                    };
                    return new Tuple<bool, UserAccessibleInfo, string>(true, Info, string.Empty);
                }
                else
                {
                    return new Tuple<bool, UserAccessibleInfo, string>(false,new UserAccessibleInfo(), "无法将Users类型转换为Json类型");
                }
            }
            else
            {
                return new Tuple<bool, UserAccessibleInfo, string>(false,new UserAccessibleInfo(), QueryResult.Item2);
            }
        }
    }
}
