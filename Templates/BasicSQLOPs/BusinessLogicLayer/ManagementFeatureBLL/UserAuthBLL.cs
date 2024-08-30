using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class UserAuthBLL
    {
        private UserAuthDAL UserAuthDAL;
        public UserAuthBLL()
        {
            UserAuthDAL = new();
        }

        public Tuple<bool, string> NewUserAuthed(Auth_Info NewAuth)
        {
            if (NewAuth == null)
            {
                return Tuple.Create(false, "BLL: 传入内容为空");
            }

            if (NewAuth.User_ID <= 0 || NewAuth.Auth_Date == default)
            {
                return Tuple.Create(false, "传入参数不完整");
            }

            return UserAuthDAL.NewUserAuthed(NewAuth);
        }
    }
}
