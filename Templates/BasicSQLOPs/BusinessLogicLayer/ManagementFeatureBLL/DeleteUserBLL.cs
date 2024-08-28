using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class DeleteUserBLL
    {
        private UserOperatorDAL UserOperatorDAL;
        public DeleteUserBLL()
        {
            UserOperatorDAL = new UserOperatorDAL();
        }

        public Tuple<bool, string> DeleteUser(int UserID)
        {
            if (UserID < 0)
            {
                return new Tuple<bool, string>(false, "UserID不合法");
            }

            return UserOperatorDAL.DeleteUser(UserID);
        }
    }
}
