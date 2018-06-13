using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;
using System.Reflection;

using IS3.Core;
using IS3.Core.Serialization;
using IS3.ArcGIS.Graphics;
using IS3.ArcGIS.Geometry;

using IS3.Desktop.Properties;
namespace IS3.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainFrame _mainFrame;
        string _sysDataDir = @"..\..\Data";

        static IS3GraphicEngine _graphicEngine = new IS3GraphicEngine();
        static IS3GeometryEngine _geometryEngine = new IS3GeometryEngine();

        public MainFrame MainFrame
        {
            get { return _mainFrame; }
            set { _mainFrame = value; }
        }
        public Project Project
        {
            get
            {
                if (_mainFrame != null)
                    return _mainFrame.prj;
                else
                    return null;
            }
        }
        public string SysDataDir { get { return _sysDataDir; } }

        public App()
        {
            Startup += App_Startup;
            Exit += App_Exit;
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                string exeLocation = Assembly.GetExecutingAssembly().Location;
                string exePath = System.IO.Path.GetDirectoryName(exeLocation);
                DirectoryInfo di = System.IO.Directory.GetParent(exePath);
                string rootPath = di.FullName;
                string dataPath = rootPath + "\\Data";
                string tilePath = dataPath + "\\TPKs";
                Runtime.rootPath = rootPath;
                Runtime.dataPath = dataPath;
                Runtime.tilePath = tilePath;
                Runtime.servicePath = rootPath + "\\bin\\Servers";
                Runtime.configurationPath = rootPath + "\\IS3-Configuration\\DBconfig.xml";

                //ArcGISRuntime.Initialize();
                Runtime.initializeEngines(_graphicEngine, _geometryEngine);
                Globals.application = this;
                Globals.mainthreadID = Thread.CurrentThread.ManagedThreadId;
                Globals.iS3Service = ServiceImporter.LoadService(Runtime.servicePath);
                //test();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

                // Exit application
                this.Shutdown();
            }
        }

        void App_Exit(object sender, ExitEventArgs e)
        {
        }

    }

}
