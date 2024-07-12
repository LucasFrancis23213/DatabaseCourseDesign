using SQLOperation.PublicAccess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseProject.DataAccessLayer.CommunityFeatureDAL
{
    // IMergedDAL接口定义
    public interface IMergedDAL
    {
        Tuple<bool, string> InsertTable(string TableName, List<string> ColumnName, List<object> Value);
        Tuple<bool, string> DeleteTable(string TableName, Dictionary<string, object> Conditions);
        Tuple<bool, string> QueryTable(string TableName, Dictionary<string, object> Conditions);
        Tuple<bool,string> UpdateTable(string TableName, Dictionary<string, object> UpdateColumns, Dictionary<string, object> ConditionColumns);
        Tuple<bool, string> InsertAndGetValue(string tableName, string returnValueColumn, List<string> columnNames, List<object> values);
    }


}
