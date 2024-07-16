using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.UserSecrets;
using SQLOperation.PublicAccess.Utilities;

namespace SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil
{
    public class UserAccessibleInfo
    {

        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Contact { get; set; }
    }
}
