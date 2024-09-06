using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class UpdateUserInfoBLL
    {
        private UserOperatorDAL UserOperatorDAL;

        public UpdateUserInfoBLL()
        {
            UserOperatorDAL = new UserOperatorDAL();
        }

        public Tuple<bool, string> UpdateUserInfo(UpdateUserInfoUtil NewInfo)
        {
            if (!string.IsNullOrEmpty(NewInfo.Password))
                NewInfo.Password = PasswordEncryptor.EncryptPassword(NewInfo.Password);

            return UserOperatorDAL.UpdateUserInfo(NewInfo.UserID, NewInfo.UserName, NewInfo.Password, NewInfo.Contact);
        }
    }
}