namespace SQLOperation.PublicAccess.Templates.TemplateInterfaceManager
{
    public interface IBasicSQLOps
    {
        Tuple<bool, string> InsertOperation(string TableName, List<string> ColumnName, List<object> Value);
        Tuple<bool, string> DeleteOperation(string TableName, string ConditionColumn, object Value);
        Tuple<bool, string> QueryOperation(string TableName, string ConditionColumn, object Value);
        Tuple<bool, string> UpdateOperation(string TableName, string UpdateColumn, object UpdateValue, string ConditionColumn, object ConditionValue);

    }
}
