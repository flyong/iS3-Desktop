using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iS3.Core
{
    public interface ILogin
    {
        event EventHandler<UserLogin> UserLoginFinished;
    }
}
