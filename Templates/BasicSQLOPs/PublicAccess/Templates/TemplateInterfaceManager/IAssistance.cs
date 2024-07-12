using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Templates.TemplateInterfaceManager
{
    public interface IAssistance
    {
        bool CheckColumnExists(string ColumnName, string TableName);

    }
}
