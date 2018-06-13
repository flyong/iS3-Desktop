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

namespace IS3.UIDesign
{
    /// <summary>
    /// test.xaml 的交互逻辑
    /// </summary>
    public partial class test : UserControl
    {
        public test()
        {
            InitializeComponent();
            System.Drawing.Image img = Properties.Resource1.LoginBackground;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);
            IntPtr hBitmap = bmp.GetHbitmap();
            System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            backgroundImage.Source = WpfBitmap;
        }
        public EventHandler<_LoginClickArgs> _loginClickHandler;
        private void Btn_Login_Click(object sender, RoutedEventArgs e)
        {
            if ((TB_LoginName.Text == null) || (TB_LoginPassword.Password == null)) { MessageBox.Show("用户或密码不能为空"); return; }
            if (_loginClickHandler != null)
            {
                _LoginClickArgs _args = new _LoginClickArgs();
                _args.LoginName = TB_LoginName.Text.ToString();
                _args.LoginPassword = TB_LoginPassword.Password.ToString();
                _loginClickHandler(this, _args);
            }
        }
        public class _LoginClickArgs
        {
            public string LoginName { get; set; }
            public string LoginPassword { get; set; }
        }
    }
}
