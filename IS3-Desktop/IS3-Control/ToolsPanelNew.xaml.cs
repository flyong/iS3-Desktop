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
    /// ToolsPanelNew.xaml 的交互逻辑
    /// </summary>
    public partial class ToolsPanelNew : UserControl
    {
        public Dictionary<string, ToolTreeItem> m_Tree = new Dictionary<string, ToolTreeItem>();
        public ToolsPanelNew()
        {
            InitializeComponent();
        }

    }

}
