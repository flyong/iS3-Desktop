using Newtonsoft.Json.Linq;
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

namespace IS3_Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AuthConfig_Click(object sender, RoutedEventArgs e)
        {
            string result=AuthConfigTool.auth("linxiaodong", "lxd");
            JObject obj = JObject.Parse(result);
            string token = "Bearer "+ obj["access_token"].ToString();

            new LoadFile().Load(null, null, null);
        }
    }
}
