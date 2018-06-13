using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using IS3.Core;

namespace IS3.UIDesign
{
    public class EntryPoint:UIDesigns
    {
        public override string name() { return "iS3.UIDesign"; }
        public override string provider() { return "Tongji iS3 team"; }
        public override string version() { return "1.0"; }
        test _test;
        public override FrameworkElement UIItem()
        {
            _test = new test();
            _test._loginClickHandler += _loginClickListener;
            return _test;
        }

        private void _loginClickListener(object sender, test._LoginClickArgs e)
        {
            if (LoginClickTrigger != null)
            {
                LoginClickArgs args = new LoginClickArgs();
                args.LoginName = e.LoginName;
                args.LoginPassword = e.LoginPassword;
                LoginClickTrigger(this, args);
            }
        }
        public override event EventHandler<LoginClickArgs> LoginClickTrigger;
    }
}
