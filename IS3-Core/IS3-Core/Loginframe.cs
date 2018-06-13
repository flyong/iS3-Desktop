using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS3.Core
{
    public interface ILoginframe
    {
        event EventHandler<LoginClickArgs> LoginClickTrigger;

    }
    public class LoginClickArgs
    {
        private string _loginName;
        public string LoginName
        {
            get { return _loginName; }
            set { _loginName = value; }
        }
        private string _loginPassword;
        public string LoginPassword
        {
            get { return _loginPassword; }
            set { _loginPassword = value; }
        }
    }
}
