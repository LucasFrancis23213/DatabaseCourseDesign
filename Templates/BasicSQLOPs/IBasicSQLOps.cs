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
        QueryResult QuerySQL(string TableName, string SelectColumn, string ConditionColumn, object Value);
        bool UpdateSQL(string TableName, string UpdateColumn, object UpdateValue, string ConditionColumn, object ConditionValue);

    }
}
