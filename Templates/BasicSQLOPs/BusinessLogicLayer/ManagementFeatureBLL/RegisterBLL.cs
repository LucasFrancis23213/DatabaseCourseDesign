using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class RegisterBLL
    {
        private UserOperatorDAL UserOperatorDAL;
        public RegisterBLL() 
        {
            UserOperatorDAL = new UserOperatorDAL();
        }

        public Tuple<bool, string> InsertUser(Users user)
        {
            if (string.IsNullOrEmpty(user.User_Name) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Contact))
            {
                return new Tuple<bool, string>(false, "User details are incomplete");
            }

            var result = UserOperatorDAL.InsertUser(user.User_Name, user.Password, user.Contact);

            return result;
        }
    }
}
