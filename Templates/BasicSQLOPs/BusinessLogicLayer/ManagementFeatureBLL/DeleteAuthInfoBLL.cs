using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class DeleteAuthInfoBLL
    {
        private UserAuthDAL UserAuthDAL;
        public DeleteAuthInfoBLL()
        {
            UserAuthDAL = new();
        }
        public Tuple<bool, string> DeleteAuthInfo(int UserID)
        {
            if (UserID <= 0)
            {
                return Tuple.Create(false, "传入参数不合法");
            }

            return UserAuthDAL.DeleteAuthInfo(UserID);
        }
    }
}
