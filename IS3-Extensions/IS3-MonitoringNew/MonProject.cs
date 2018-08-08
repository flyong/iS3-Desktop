using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iS3.Core;
namespace iS3.Monitoring
{
    public class MonProject:DGObject
    {
        public string Description { get; set; }
        public string Type { get; set; }
        public string UnitID { get; set; }
        public string PerInfoID { get; set; }
        public string MonInstInfoIDs { get; set; }
        public string PlanFile { get; set; }
        public string Remark { get; set; }
        public List<MonGroup> MonGroupList { get; set; }
    }
}
