using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IS3.Core;
namespace IS3.Monitoring
{
    public class MonReading:DGObject
    {
        public int MonPointID { get; set; }
        public double Reading { get; set; }
        public double Value { get; set; }
        public DateTime DataTime { get; set; }
        public DateTime SysTime { get; set; }
        public string unit { get; set; }
    }
}
