using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iS3.Core;
using Telerik.Windows;
using Telerik.Windows.Controls;

namespace iS3.Control
{
    /// <summary>
    /// IS3MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class IS3MainWindowTelerik : RadWindow, IS3PageHolder
    {
        public IS3MainWindowTelerik()
        {
            InitializeComponent();

        }
        private IPageTransition pageTransition;

        public void SwitchToMainFrame(string projectID)
        {
            
        }
        public void SwitchToProjectListPage()
        {
           
        }

        public void SetPageTransition(IPageTransition pageTransition)
        {
            this.pageTransition = pageTransition;
            holder.Children.Add(pageTransition as UserControl);
        }

        public void ShowPage(UserControl userControl)
        {
            pageTransition.ShowPage(userControl);
        }

        public void LoginFinished(object sender, UserLogin e)
        {
           
        }

        public void SetShow()
        {
            this.Show();
        }
    }
}
