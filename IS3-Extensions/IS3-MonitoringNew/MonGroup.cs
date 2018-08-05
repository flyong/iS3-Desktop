using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IS3.Core;
namespace IS3.Monitoring
{
    public class MonGroup:DGObject
    {
        internal object monPntDict;

        public string Description { get; set; }
        public string MonGroupType { get; set; }
        public string MonProjectID { get; set; }
        public string RefSpecifications { get; set; }
        public string PerInfoIDs { get; set; }
        public string Remark { get; set; }
    }
}
