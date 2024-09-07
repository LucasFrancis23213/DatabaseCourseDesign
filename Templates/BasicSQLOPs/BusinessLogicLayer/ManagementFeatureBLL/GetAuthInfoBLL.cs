using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class GetAuthInfoBLL
    {
        private UserAuthDAL UserAuthDAL;
        public GetAuthInfoBLL()
        {
            UserAuthDAL = new();
        }

        public Tuple<bool, string> GetAuthInfo(int? UserID)
        {
            if (UserID is not null && UserID <= 0)
                return Tuple.Create(false, "传入参数不合法");

            return UserAuthDAL.GetAuthInfo(UserID);
        }
    }
}
