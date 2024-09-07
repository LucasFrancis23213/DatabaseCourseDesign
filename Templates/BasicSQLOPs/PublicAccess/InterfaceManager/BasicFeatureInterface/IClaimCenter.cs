using SQLOperation.PublicAccess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.BusinessLogicLayer.BasicFeatureBLL
{
    public interface IClaimCenter
    {
        //认领&归还若干操作
        public Tuple<bool, string> AddReturnItem(string itemID);

        public Tuple<bool, string> AddClaimItem(string itemID);

        public Tuple<bool, string> QueryItem(int type, Dictionary<string, object> index);

        public Tuple<bool, string> SignReturnAgreement(string itemID, int userID);

        public Tuple<bool, Tuple<bool, string>> CheckSignStatus(string itemID, int userID);

        public Tuple<bool, string> DeleteItem(string itemID);

        public void ReleaseSQLConn();
    }
}
