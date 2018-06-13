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
using System.Collections.Specialized;
using System.Configuration;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;

namespace IS3.Desktop
{
    /// <summary>
    /// UserLoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class UserLoginPage : UserControl
    {
        private XDocument xml = XDocument.Load(Runtime.configurationPath);
        public UserLoginPage()
        {
            InitializeComponent();

            //读取xml配置到界面
            InitialLogin();
        }
        void InitialLogin()
        {
            DBAddress_TB.Text = xml.Root.Element("ipaddress")?.Value;
            DBUser_TB.Text = xml.Root.Element("user")?.Value;
            DBPW_TB.Text = xml.Root.Element("password")?.Value;
            DBName_TB.Text = xml.Root.Element("database")?.Value;
        }

        #region 按钮事件

        //登陆验证
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceImporter.UpdateMainConnect(Runtime.configurationPath);

            XDocument xml = XDocument.Load(Runtime.configurationPath);
            string key = "Data Source =" + xml.Root.Element("ipaddress")?.Value + "; Initial Catalog = " +
                         xml.Root.Element("database")?.Value + "; User Id = " + xml.Root.Element("user")?.Value +
                         "; Password = " + xml.Root.Element("password")?.Value + ";Connect Timeout=1";
            SqlConnection sqlConnection = new SqlConnection(key);
            try
            {
                sqlConnection.QuickOpen(1000);
            }
            catch (Exception)
            {
                MessageBox.Show("连接超时");
                sqlConnection.Close();
                sqlConnection.Dispose();
                return;
            }
            sqlConnection.Close();

            if ((LoginNameTB.Text == "") || (LoginPasswordTB.Password == ""))
            {
                MessageBox.Show("账号或密码不为空");
                return;
            }
            Globals.userID = Globals.iS3Service.PrivilegeService.CheckIfValidLoginInfo(LoginNameTB.Text, LoginPasswordTB.Password);
            if (Globals.userID > 0)
            {
                App app = Application.Current as App;
                IS3MainWindow mw = (IS3MainWindow)app.MainWindow;
                mw.SwitchToProjectListPage();
            }
            else
            {
                MessageBox.Show("密码错误");
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
            xml.Root.Element("user").SetValue(DBUser_TB.Text);
            xml.Root.Element("password").SetValue(DBPW_TB.Text);
            xml.Root.Element("database").SetValue(DBName_TB.Text);
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
    public static class SqlExtensions
    {
        public static void QuickOpen(this SqlConnection conn, int timeout)
        {
            // We'll use a Stopwatch here for simplicity. A comparison to a stored DateTime.Now value could also be used
            Stopwatch sw = new Stopwatch();
            bool connectSuccess = false;

            // Try to open the connection, if anything goes wrong, make sure we set connectSuccess = false
            Thread t = new Thread(delegate ()
            {
                try
                {
                    sw.Start();
                    conn.Open();
                    connectSuccess = true;
                }
                catch { }
            });

            // Make sure it's marked as a background thread so it'll get cleaned up automatically
            t.IsBackground = true;
            t.Start();

            // Keep trying to join the thread until we either succeed or the timeout value has been exceeded
            while (timeout > sw.ElapsedMilliseconds)
                if (t.Join(1))
                    break;

            // If we didn't connect successfully, throw an exception
            if (!connectSuccess)
                throw new Exception("Timed out while trying to connect.");
        }
    }
}
