using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iS3.Core
{
    public interface IView3D:IView
    {
        void ExcuteCommand(string command);
    }
}
