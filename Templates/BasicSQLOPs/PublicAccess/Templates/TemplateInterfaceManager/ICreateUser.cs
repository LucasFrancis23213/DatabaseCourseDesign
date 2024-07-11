using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.PublicAccess.Templates.TemplateInterfaceManager
{
    public interface ICreateUser
    {
        public bool UserCreation();
        public bool UserExists();
    }
}
