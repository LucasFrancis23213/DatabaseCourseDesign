using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class RegisterBLL
    {
        private RegisterDAL RegisterDAL;
        public RegisterBLL() 
        {
            RegisterDAL = new RegisterDAL();
        }

        public Tuple<bool, string> InsertUser(Users user)
        {
            if (string.IsNullOrEmpty(user.User_Name) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Contact))
            {
                return new Tuple<bool, string>(false, "User details are incomplete");
            }

            var result = RegisterDAL.InsertUser(user.User_Name, user.Password, user.Contact, user.Status);

            return result;
        }
    }
}
