using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class DeleteUserBLL
    {
        private UserOperatorDAL UserOperatorDAL;
        private UserAuthDAL UserAuthDAL;
        public DeleteUserBLL()
        {
            UserOperatorDAL = new UserOperatorDAL();
            UserAuthDAL = new UserAuthDAL();
        }

        public Tuple<bool, string> DeleteUser(int UserID)
        {
            if (UserID < 0)
            {
                return new Tuple<bool, string>(false, "UserID不合法");
            }

            // delete auth info first
            var (succeeded, message) = UserAuthDAL.DeleteAuthInfo(UserID);
            if (!succeeded)
                return Tuple.Create(false, $"清除用户认证信息时: {message}");

            return UserOperatorDAL.DeleteUser(UserID);
        }
    }
}
