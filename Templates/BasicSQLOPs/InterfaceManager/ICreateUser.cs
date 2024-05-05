using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLOperation.InterfaceManager
{
    public interface ICreateUser
    {
        public bool UserCreation();
        public bool UserExists();
    }
}
