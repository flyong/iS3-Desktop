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
using IS3.Core;
namespace IS3.Control
{
    /// <summary>
    /// IS3MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class IS3MainWindow : Window, IS3PageHolder
    {
        public IS3MainWindow()
        {
            InitializeComponent();
            Loaded += IS3MainWindow_Loaded;
        }

        private void IS3MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

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
    }
}
