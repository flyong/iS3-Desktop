using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace iS3.Core
{
    public interface IPageTransition
    {
        void ShowPage(UserControl newPage);
    }
}
