using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templates
{
    public interface IBasicSQLOps
    {
        bool InsertOperation(string TableName, string ColumnName, object Value);
        bool DeleteOperation(string TableName, string ColumnName, object Value);
        QueryResult QueryOperation(string TableName, string SelectColumn, string ConditionColumn, object Value);
        bool UpdateOperation(string TableName, string UpdateColumn, object UpdateValue, string ConditionColumn, object ConditionValue);

    }
}
