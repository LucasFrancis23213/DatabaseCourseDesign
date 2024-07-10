using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Templates.TemplateInterfaceManager
{
    public interface IBasicSQLOps
    {
        bool InsertOperation(string TableName, List<string> ColumnName, List<object> Value);
        bool DeleteOperation(string TableName, string ConditionColumn, object Value);
        string QueryOperation(string TableName, string ConditionColumn, object Value);
        bool UpdateOperation(string TableName, string UpdateColumn, object UpdateValue, string ConditionColumn, object ConditionValue);

    }
}
