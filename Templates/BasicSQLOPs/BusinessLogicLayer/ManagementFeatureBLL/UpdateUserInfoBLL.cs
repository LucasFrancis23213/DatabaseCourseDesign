using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class UpdateUserInfoBLL
    {
        private UserOperatorDAL UserOperatorDAL;

        public UpdateUserInfoBLL()
        {
            UserOperatorDAL = new UserOperatorDAL();
        }

        public Tuple<bool, string> UpdateUserInfo(Users NewInfo)
        {
            if (string.IsNullOrEmpty(NewInfo.User_Name) || string.IsNullOrEmpty(NewInfo.Password) || string.IsNullOrEmpty(NewInfo.Contact))
            {
                return new Tuple<bool, string>(false, "User details are incomplete");
            }

            NewInfo.Password = PasswordEncryptor.EncryptPassword(NewInfo.Password);

            return UserOperatorDAL.UpdateUserInfo(NewInfo.User_ID, NewInfo.User_Name, NewInfo.Password, NewInfo.Contact);
        }
    }
}