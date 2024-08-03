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

        public Tuple<bool, string> DeleteUser(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return new Tuple<bool, string>(false, "UserName is empty");
            }

            return UserOperatorDAL.DeleteUser(UserName);
        }
    }
}
