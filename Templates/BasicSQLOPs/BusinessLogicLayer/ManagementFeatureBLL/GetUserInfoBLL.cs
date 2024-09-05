using Newtonsoft.Json;
using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class GetUserInfoBLL
    {
        private UserOperatorDAL UserOperatorDAL;
        public GetUserInfoBLL() 
        { 
            UserOperatorDAL = new UserOperatorDAL();
        }

        public Tuple<bool, string> GetInfo(int? UserID, string? UserName, bool IsAdmin = false)
        {
            if (!IsAdmin && UserID is null && UserName is null)
                return new Tuple<bool, string>(false, "未找到用户");

            Tuple<bool, string> QueryResult = UserOperatorDAL.GetUserInfo(UserID, UserName);

            if (!QueryResult.Item1)
                return new Tuple<bool, string>(false, QueryResult.Item2);

            var users = JsonConvert.DeserializeObject<List<Users>>(QueryResult.Item2);

            if (users == null || users.Count <= 0)
                return new Tuple<bool, string>(false, "无法将Users类型转换为Json类型");

            List<UserAccessibleInfo> Resultlist = [];

            foreach (var user in users)
            {
                if (!IsAdmin && user.Is_Deleted == 1)
                    continue;

                Resultlist.Add(new UserAccessibleInfo()
                {
                    UserName = user.User_Name,
                    UserID = user.User_ID,
                    Contact = user.Contact,
                    IsDeleted = user.Is_Deleted,
                    Avatar = user.Avatar,
                });
            }

            return new Tuple<bool, string>(true, JsonConvert.SerializeObject(Resultlist));
        }
    }
}
