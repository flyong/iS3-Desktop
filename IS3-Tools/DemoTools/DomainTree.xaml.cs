using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using IS3.Core;

namespace DemoTools
{
    /// <summary>
    ///     DomainTree.xaml 的交互逻辑
    /// </summary>
    public partial class DomainTree : Window
    {
        private DataSet ds;
        private readonly List<string> TableList = new List<string>();

        public DomainTree()
        {
            InitializeComponent();
            DatabaseBox.ItemsSource = GetStringList("Master", "select name from dbo.sysdatabases");
        }


        private void DominTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(DominTreeView.SelectedItem is KeyValuePair<string, DGObjects>))
            {
                PropertyBox.ItemsSource = null;
                return;
            }
            var item = (KeyValuePair<string, DGObjects>) DominTreeView.SelectedItem;
            var type = item.Value.rowView2Obj.Values.ToList()[0].GetType();
            var propertys = type.GetProperties();
            var propertyList = new List<string>();
            foreach (var pi in propertys)
                propertyList.Add(pi.Name);
            PropertyBox.ItemsSource = propertyList;
            //ConfigList.ItemsSource = item.Value;
        }

        private void DomainTree_MouseRinghtButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = (DependencyObject) sender;
            while (true)
            {
                if (treeViewItem is TreeViewItem)
                {
                    (treeViewItem as TreeViewItem).Focus();
                    e.Handled = true;
                    return;
                }
                treeViewItem = VisualTreeHelper.GetParent(treeViewItem);
            }
        }

        private void Database_Changed(object sender, SelectionChangedEventArgs e)
        {
            var databse = DatabaseBox.SelectedItem.ToString();

            TableBox.ItemsSource = null;
            TableBox.ItemsSource = GetStringList(databse, "SELECT Name FROM SysObjects Where XType='U' ORDER BY Name");
        }

        private List<string> GetStringList(string database, string sqlcommand)
        {
            string ServerIP = Globals.iS3Service.DataService.ServerIP;
            string ServerUser= Globals.iS3Service.DataService.ServerUser;
            string ServerPWD = Globals.iS3Service.DataService.ServerPWD;
            var sqlConnection =
                new SqlConnection("Server = "+ServerIP+";Database = " + database + ";User ID = "+ServerUser+";Password = "+ServerPWD+";");
            sqlConnection.Open();
            var sqlCommand = new SqlCommand(sqlcommand, sqlConnection);
            var sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            ds = new DataSet();
            sqlDataAdapter.Fill(ds);
            var StringList = new List<string>();
            foreach (DataRow item in ds.Tables[0].Rows)
                StringList.Add(item.ItemArray[0].ToString());
            sqlConnection.Close();
            return StringList;
        }
    }
}