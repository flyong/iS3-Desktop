using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IS3.Core;
namespace IS3.Servers
{
    public class EntryPoint : Extensions
    {
        public override string name() { return "iS3.Servers"; }
        public override string provider() { return "Tongji iS3 team"; }
        public override string version() { return "1.0"; }
    }
}
