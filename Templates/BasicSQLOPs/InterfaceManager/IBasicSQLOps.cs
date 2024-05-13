using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.InterfaceManager
{
    public interface IBasicSQLOps
    {
        bool InsertOperation(string TableName, string ColumnName, object Value);
        bool DeleteOperation(string TableName, string ConditionColumn, object Value);
        string QueryOperation(string TableName, string ConditionColumn, object Value);
        bool UpdateOperation(string TableName, string UpdateColumn, object UpdateValue, string ConditionColumn, object ConditionValue);

    }
}
