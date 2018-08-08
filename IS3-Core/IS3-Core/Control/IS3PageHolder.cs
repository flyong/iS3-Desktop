using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace iS3.Core
{
    public interface IS3PageHolder
    {
        void SetShow();
        void SetPageTransition(IPageTransition pageTransition);
        void ShowPage(UserControl userControl);
        void SwitchToMainFrame(string projectID);
        void SwitchToProjectListPage();

        void LoginFinished(object sender, UserLogin e);
    }
}
