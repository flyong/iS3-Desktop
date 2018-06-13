using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace IS3.Core
{
    public class LoginClickArgs
    {
        public string LoginName { get; set; }
        public string LoginPassword { get; set; }
    }
    public  class UIDesigns:Extensions
    {
        public override string name() { return "Unknown UIDesigns"; }

        // Summary:
        //     Get treeItems of the tool, called immediately after loaded.
        public virtual FrameworkElement UIItem()
        {
            return null;
        }
        public virtual event EventHandler<LoginClickArgs> LoginClickTrigger;

    }
    public interface IExteralUI
    {
        string name { get; set; }
        LayoutAnchorable parent { get; set; }
        UserControl content { get; set; }
        bool isActive { get; set; }

    }
}
