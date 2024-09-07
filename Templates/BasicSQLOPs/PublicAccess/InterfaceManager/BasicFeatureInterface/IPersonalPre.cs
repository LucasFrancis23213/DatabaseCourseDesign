using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.InterfaceManager.BasicFeatureInterface
{
    public interface IPersonalPre
    {
        //用户正常创建之初就直接进行偏好和订阅的初始化
        //可能有未知修改，先不进行合并为一个操作
        public Tuple<bool, string> UserPreferencesBasic(User_Preferences item);
        public Tuple<bool, string> UserSubscriptionsBasic(User_Subscriptions item);
        //用户注销进行级联删除的时候直接删除这两个表，不再进行删除的写
        //查找只存在于管理员进行管理
        public Tuple<bool, string> QueryItem(string TableName, Dictionary<string, object> index);
        //修改由用户操作，较为重要
        public Tuple<bool, string> UpdateItem(string TableName, Dictionary<string, object> UpdateColumns, Dictionary<string, object> index);
    }
}
