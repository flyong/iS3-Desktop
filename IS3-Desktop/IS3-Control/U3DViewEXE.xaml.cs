using System;
using System.Collections.Generic;
using System.IO;
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

namespace iS3.Control
{
    /// <summary>
    /// U3DView.xaml 的交互逻辑
    /// </summary>
    public partial class U3DViewEXE : UserControl
    {
        public UnityPanel panel;
        public U3DViewEXE(EngineeringMap emap)
        {
            InitializeComponent();
            panel = new UnityPanel(emap);
            FormHost.Child = panel;
        }
        public void set()
        {
            FormHost.Child = panel;
        }


        private void FormHost_OnGotFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("------------------------------------------->Focus");
            UnityPanel.main.ActivateUnityWindow();
        }

        private void FormHost_OnLostFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("------------------------------------------->LostFocus");
            UnityPanel.main.DeactivateUnityWindow();
        }

        private void FormHost_OnMouseEnter(object sender, MouseEventArgs e)
        {
            UnityPanel.main.ActivateUnityWindow();
        }

        private void FormHost_OnMouseLeave(object sender, MouseEventArgs e)
        {
            UnityPanel.main.DeactivateUnityWindow();
        }
    }
}
