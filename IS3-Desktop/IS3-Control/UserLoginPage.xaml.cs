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
using System.Collections.Specialized;
using System.Configuration;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using iS3.Core.Service;
namespace iS3.Control
{
    /// <summary>
    /// UserLoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class UserLoginPage : UserControl,ILogin
    {
        private XDocument xml = XDocument.Load(Runtime.configurationPath);
        private string defaultUserName;
        private string defaultPassword;

        public event EventHandler<UserLogin> UserLoginFinished;

        public UserLoginPage()
        {
            InitializeComponent();

            //读取xml配置到界面
            InitialLogin();
        }
        void InitialLogin()
        {
            DBAddress_TB.Text = xml.Root.Element("ipaddress").Value;
            DBPort_TB.Text = xml.Root.Element("portID").Value;
            defaultUserName = xml.Root.Element("DName").Value;
            defaultPassword = xml.Root.Element("DPass").Value;
        }

        #region 按钮事件

        //登陆验证
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!((LoginNameTB.Text == defaultUserName) || (LoginPasswordTB.Password == defaultPassword)))
            {
                MessageBox.Show("账号或密码错误");
                return;
            }

            XDocument xml = XDocument.Load(Runtime.configurationPath);
            ServiceConfig.IP = xml.Root.Element("ipaddress")?.Value;
            ServiceConfig.PortNum = xml.Root.Element("portID")?.Value;
            Globals.userID = 0;

            if (UserLoginFinished != null)
            {
                UserLoginFinished(this, null);
            }
        }
        private void DBConfig_Click(object sender, RoutedEventArgs e)
        {
            LoginElement.Visibility = Visibility.Hidden;
            DBConfigElement.Visibility = Visibility.Visible;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            hideUI();
        }

        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            xml.Root.Element("ipaddress").SetValue(DBAddress_TB.Text);
            xml.Root.Element("portID").SetValue(DBPort_TB.Text);
            xml.Save(Runtime.configurationPath);

            hideUI();
        }
        void hideUI()
        {
            LoginElement.Visibility = Visibility.Visible;
            DBConfigElement.Visibility = Visibility.Hidden;
        }
        #endregion
    }
}
