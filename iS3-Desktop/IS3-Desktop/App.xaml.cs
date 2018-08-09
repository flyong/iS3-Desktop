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

using iS3.Core;
using iS3.ArcGIS.Graphics;
using iS3.ArcGIS.Geometry;

using iS3.Desktop.Properties;
using System.Xml.Linq;

namespace iS3.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //MainFrame _mainFrame;
        string _sysDataDir = @"..\..\Data";

        static IS3GraphicEngine _graphicEngine = new IS3GraphicEngine();
        static IS3GeometryEngine _geometryEngine = new IS3GeometryEngine();

        public string SysDataDir { get { return _sysDataDir; } }

        public App()
        {
            Startup += App_Startup;
            Exit += App_Exit;
        }
        void Application_Startup(object sender, StartupEventArgs e)
        {
            Globals.iS3Core = new IS3RuntimeControl();

            XDocument xml = XDocument.Load(Runtime.configurationPath);
            string version = xml.Root.Element("Version").Value;
            if (version == "Normal")
            {
                //Normal Style
                Globals.iS3Core.SetPageHolder(new iS3.Control.IS3MainWindow());
                Globals.iS3Core.SetProjectList(new iS3.Control.ProjectListPage());
                Globals.iS3Core.SetMainFrame(new iS3.Control.MainFrame("TONGJI"));
            }
            else
            {
                //Telerik Style
                //Globals.iS3Core.SetPageHolder(new iS3.Control.IS3MainWindowTelerik());
                //Globals.iS3Core.SetProjectList(new iS3.Control.ProjectListPageTelerik());
                //Globals.iS3Core.SetMainFrame(new iS3.Control.MainFrameByTelerik("TONGJI"));
            }
            Globals.iS3Core.SetPageTransition(new iS3.Control.WpfPageTransitions.PageTransition());
            Globals.iS3Core.MainWindowShow();
            Globals.iS3Core.SetLogin(new iS3.Control.UserLoginPage());

            Globals.iS3Core.SetPageShow(PageType.LoginPage);
            
            try
            {
                string exeLocation = Assembly.GetExecutingAssembly().Location;
                string exePath = System.IO.Path.GetDirectoryName(exeLocation);
                DirectoryInfo di = System.IO.Directory.GetParent(exePath);
                string rootPath = di.FullName;
                string dataPath = rootPath + "\\Output\\Data";
                string tilePath = dataPath + "\\TPKs";
                Runtime.rootPath = rootPath;
                Runtime.dataPath = dataPath;
                Runtime.tilePath = tilePath;
                Runtime.servicePath = rootPath + "\\bin\\Servers";
                Runtime.configurationPath = rootPath + "\\IS3-Configuration\\ServiceConfig.xml";

                //ArcGISRuntime.Initialize();
                Runtime.initializeEngines(_graphicEngine, _geometryEngine);
                Globals.application = this;
                Globals.mainthreadID = Thread.CurrentThread.ManagedThreadId;
                //Globals.iS3Service = ServiceImporter.LoadService(Runtime.servicePath);
                //test();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

                // Exit application
                this.Shutdown();
            }
        }
        void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                string exeLocation = Assembly.GetExecutingAssembly().Location;
                string exePath = System.IO.Path.GetDirectoryName(exeLocation);
                DirectoryInfo di = System.IO.Directory.GetParent(exePath);
                string rootPath = di.FullName;
                string dataPath = rootPath + "\\Output\\Data";
                string tilePath = dataPath + "\\TPKs";
                Runtime.rootPath = rootPath;
                Runtime.dataPath = dataPath;
                Runtime.tilePath = tilePath;
                Runtime.servicePath = rootPath + "\\bin\\Servers";
                Runtime.configurationPath = rootPath + "\\IS3-Configuration\\ServiceConfig.xml";

                //ArcGISRuntime.Initialize();
                Runtime.initializeEngines(_graphicEngine, _geometryEngine);
                Globals.application = this;
                Globals.mainthreadID = Thread.CurrentThread.ManagedThreadId;
                //Globals.iS3Service = ServiceImporter.LoadService(Runtime.servicePath);
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
