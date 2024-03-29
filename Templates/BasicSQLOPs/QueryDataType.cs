using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templates
{
    public class SingleResult
    {
        public string QueryName { get; set; }
        public object Result {  get; set; }
    }

    public class QueryResult
    {
        public List<SingleResult> Results { get; set; } = new List<SingleResult>();
    }

}
