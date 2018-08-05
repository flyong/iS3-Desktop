using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IS3.Core
{
    public interface IPageTransition
    {
        void ShowPage(UserControl newPage);
    }
}
