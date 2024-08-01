using Oracle.ManagedDataAccess.Client;

namespace DatabaseProject.DataAccessLayer.CommunityFeatureDAL
{
    // IMergedDAL接口定义
    public interface IMergedDAL
    {
        Tuple<bool, string> DeleteTable(string TableName, Dictionary<string, object> Conditions);
        Tuple<bool, string> QueryTable(string TableName, Dictionary<string, object> Conditions, string conditionType);
        Tuple<bool, string> UpdateTable(string TableName, Dictionary<string, object> UpdateColumns, Dictionary<string, object> ConditionColumns);
        Tuple<bool, string> InsertAndGetValue(string tableName, string returnValueColumn, List<string> columnNames, List<object> values);
        Tuple<bool, string> QueryTableWithWhere(string TableName, string whereClause, OracleParameter[] parameters);

        Tuple<bool, string> QueryWithCustomFromAndWhere(string fromClause, string whereClause, OracleParameter[] parameters);

        Tuple<bool, string> QueryWithCustomSelect(string selectClause, string fromClause, string whereClause, OracleParameter[] parameters);
    }


}
