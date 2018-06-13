using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Markup;
using System.Diagnostics;

using System.Reflection;

using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;

using IS3.Core;
using IS3.Python;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Controls;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace IS3.Desktop
{
    //************************  Notice  **********************************
    //** This file is part of iS3
    //**
    //** Copyright (c) 2015 Tongji University iS3 Team. All rights reserved.
    //**
    //** This library is free software; you can redistribute it and/or
    //** modify it under the terms of the GNU Lesser General Public
    //** License as published by the Free Software Foundation; either
    //** version 3 of the License, or (at your option) any later version.
    //**
    //** This library is distributed in the hope that it will be useful,
    //** but WITHOUT ANY WARRANTY; without even the implied warranty of
    //** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    //** Lesser General Public License for more details.
    //**
    //** In addition, as a special exception,  that plugins developed for iS3,
    //** are allowed to remain closed sourced and can be distributed under any license .
    //** These rights are included in the file LGPL_EXCEPTION.txt in this package.
    //**
    //**************************************************************************

    public partial class ProjectListPage : UserControl
    {
        static ProjectList Projects;
        //平常状态图标
        PictureMarkerSymbol _MarkerSymbol_Normal;
        //选中状态图标
        PictureMarkerSymbol _MarkerSymbol_Select;

        private bool _isHitTesting;
        private SpatialReference _srEMap;

        public ProjectListPage()
        {
            InitializeComponent();

            ProjectTitle.Text = "";

            _srEMap = Map.SpatialReference;

            MyMapView.Loaded += MyMapView_Loaded;
            MyMapView.MouseMove += MyMapView_MouseMove;
            MyMapView.MouseLeftButtonDown += MyMapView_MouseLeftButtonDown;
            MyMapView.MouseRightButtonDown += MyMapView_MouseRightButtonDown;
            MyMapView.NavigationCompleted += MyMapView_NavigationCompleted;

            _isHitTesting = true;

            //初始化图标
            InitializePictureMarkerSymbol();
            //初始化右键菜单
            initialContextMenu();
            AddContextMenu();
        }


        #region 右键菜单
        Graphic _selectGraphic;
        ContextMenu aMenu;
        void initialContextMenu()
        {
            aMenu = new ContextMenu();
            //MenuItem ConfigurationMenu = new MenuItem();
            //ConfigurationMenu.Header = "配置项目";
            //ConfigurationMenu.Click += ConfigurationMenu_Click;
            //aMenu.Items.Add(ConfigurationMenu);
            MenuItem ViewMenu = new MenuItem();
            ViewMenu.Header = "进入项目";
            ViewMenu.Click += ViewMenu_Click;
            aMenu.Items.Add(ViewMenu);
        }
        void AddContextMenu()
        {
            MyMapView.ContextMenu = aMenu;
        }
        void RemoveContextMenu()
        {
            MyMapView.ContextMenu = null;
        }
        private void ViewMenu_Click(object sender, RoutedEventArgs e)
        {
            string projectID = _selectGraphic.Attributes["ID"] as string;
            App app = Application.Current as App;
            IS3MainWindow mw = (IS3MainWindow)app.MainWindow;
            mw.SwitchToMainFrame(projectID);
        }
        #endregion

        #region GIS Map Operation
        private async void InitializePictureMarkerSymbol()
        {
            _MarkerSymbol_Normal = LayoutRoot.Resources["DefaultMarkerSymbol"]
                as PictureMarkerSymbol;
            _MarkerSymbol_Select = LayoutRoot.Resources["DefaultMarkerSymbol2"]
                as PictureMarkerSymbol;
            try
            {
                await _MarkerSymbol_Normal.SetSourceAsync(
                new Uri("pack://application:,,,/IS3.Desktop;component/Images/pin_red.png"));
                await _MarkerSymbol_Select.SetSourceAsync(
                    new Uri("pack://application:,,,/IS3.Desktop;component/Images/pIcon64.png"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        void MyMapView_NavigationCompleted(object sender, EventArgs e)
        {
            MyMapView.NavigationCompleted -= MyMapView_NavigationCompleted;
            _isHitTesting = false;
        }

        async void MyMapView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isHitTesting)
                return;

            try
            {
            
                _isHitTesting = true;

                Point screenPoint = e.GetPosition(MyMapView);
                
                //设置鼠标当前坐标
                MapPoint mapPoint = screenPoint2MapPoint(screenPoint);
                if (mapPoint == null)
                    return;
                setCoord(mapPoint);


                Graphic graphic = await ProjectGraphicsLayer.HitTestAsync(MyMapView, screenPoint);
                if (graphic != null)
                {
                    mapTip.DataContext = graphic;
                    mapTip.Visibility = System.Windows.Visibility.Visible;
                    ProjectTitle.Text = (string)graphic.Attributes["ID"];
                }
                else
                {
                    mapTip.Visibility = System.Windows.Visibility.Collapsed;
                    ProjectTitle.Text = "";
                }
            }
            catch
            {
                mapTip.Visibility = System.Windows.Visibility.Collapsed;
                ProjectTitle.Text = "";

            }
            finally
            {
                _isHitTesting = false;
            }
        }
        async void MyMapView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                RemoveContextMenu();
                //MyMapView.ContextMenu.Visibility = Visibility.Collapsed;

                _isHitTesting = true;

                Point screenPoint = e.GetPosition(MyMapView);
                Graphic graphic = await ProjectGraphicsLayer.HitTestAsync(MyMapView, screenPoint);
                if (graphic != null)
                {
                    _selectGraphic = graphic;
                    AddContextMenu();
                }
            }
            catch
            {
            }
            finally
            {
                _isHitTesting = false;
            }

        }

        async void MyMapView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AddContextMenu();
               // MyMapView.ContextMenu.Visibility = Visibility.Collapsed;

                _isHitTesting = true;

                Point screenPoint = e.GetPosition(MyMapView);
                Graphic graphic = await ProjectGraphicsLayer.HitTestAsync(MyMapView, screenPoint);
                if (graphic != null)
                {
                    _selectGraphic = graphic;
                    foreach (Graphic g in gLayer.Graphics)
                    {
                        g.Symbol = _MarkerSymbol_Normal;
                    }
                    _selectGraphic.Symbol = _MarkerSymbol_Select;
                    for (int i = 0; i < Projects.Locations.Count; i++)
                    {
                        if (Projects.Locations[i].ID.ToString() == graphic.Attributes["ID"].ToString())
                        {
                            projectBox.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                _isHitTesting = false;
            }
        }

        void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Projects == null)
            {
                LoadProjectList();
                // switch to the default project
                if (Projects != null)
                {
                    ProjectLocation loc =
                        Projects.Locations.ToList().Find(i => i.Default == true);
                    if ((loc != null))
                    {

                        App app = Application.Current as App;
                        IS3MainWindow mw = (IS3MainWindow)app.MainWindow;
                        mw.SwitchToMainFrame(loc.ID);
                    }
                    projectBox.ItemsSource = Projects.Locations;
                }
            }
            if (Projects != null)
            {
                Envelope projectExtent = new Envelope(Projects.XMin, Projects.YMin,
                    Projects.XMax, Projects.YMax);

                AddProjectsToMap();
                //Map.ZoomTo(ProjectExtent);
            }
        }
        public void LoadProjectList()
        {
            if (Projects != null)
                return;

            try
            {
                Projects = Globals.iS3Service.PrivilegeService.QueryAccessableProject(Globals.userID);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
            }
        }

        GraphicsLayer gLayer;
        private void AddProjectsToMap()
        {
            gLayer = Map.Layers["ProjectGraphicsLayer"] as GraphicsLayer;

            foreach (ProjectLocation loc in Projects.Locations)
            {
                Graphic g = new Graphic()
                {
                    Geometry = new MapPoint(loc.X, loc.Y),
                    Symbol = _MarkerSymbol_Normal,
                    //Symbol = _selectMarkerSymbol,
                };
                g.Attributes["ID"] = loc.ID;
                g.Attributes["DefinitionFile"] = loc.DefinitionFile;
                g.Attributes["Description"] = loc.Description;

                gLayer.Graphics.Add(g);
            }
        }

        public void setCoord(MapPoint mapPoint)
        {
            string format = "X = {0}, Y = {1}";
            string coord = string.Format(format,
                Math.Round(mapPoint.X, 2), Math.Round(mapPoint.Y, 2));
            MapCoordsTB.Text = coord;
        }
        MapPoint screenPoint2MapPoint(Point screenPoint)
        {
            MapPoint mapPoint = MyMapView.ScreenToLocation(screenPoint);
            if (mapPoint == null)
                return null;
            if (MyMapView.WrapAround)
                mapPoint = GeometryEngine.NormalizeCentralMeridian(mapPoint) as MapPoint;
            if (_srEMap != null)
            {
                // transform the map point to user defined spatial reference coordinate system
                Esri.ArcGISRuntime.Geometry.Geometry g = GeometryEngine.Project(mapPoint, _srEMap);
                mapPoint = g as MapPoint;
            }
            return mapPoint;
        }
        #endregion

        private void projectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectID = Projects.Locations[projectBox.SelectedIndex].ID;
            foreach (Graphic g in gLayer.Graphics)
            {
                if (g.Attributes["ID"].ToString() == selectID)
                {
                    g.Symbol = _MarkerSymbol_Select;
                }
                else
                {
                    g.Symbol = _MarkerSymbol_Normal;
                }
            }

        }

       

        private void AddProject_OnClick(object sender, RoutedEventArgs e)
        {
            AddPop.Width = AddProject.ActualWidth;
            AddPop.IsOpen = true;

        }

        private void AddProject_Cancel_Click(object sender, RoutedEventArgs e)
        {
            AddPop.IsOpen = false;
            if (newg != null)
            {
                gLayer.Graphics.Remove(newg);
            }
        }

        private void addProject_Commit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XDocument xml = XDocument.Load(Runtime.configurationPath);
                string key = "Data Source =" + xml.Root.Element("ipaddress")?.Value + "; Initial Catalog = " +
                             xml.Root.Element("database")?.Value + "; User Id = " + xml.Root.Element("user")?.Value +
                             "; Password = " + xml.Root.Element("password")?.Value + "; ";
                SqlConnection sqlConnection = new SqlConnection(key);
                sqlConnection.Open();


                SqlCommand sqlCommand = new SqlCommand(
                     "INSERT [dbo].[Sys_ProjectListInfo] ([Description], [X], [Y], [Name],[ID],[DefaultMap]) VALUES ('" +
                     ProjectDes.Text + "','" +
                     (newg.Geometry as MapPoint).X.ToString() + "','" + (newg.Geometry as MapPoint).Y.ToString() +
                     "','" + ProjectID.Text + "','" + (Projects.Locations.Count + 1) + "',0)", sqlConnection);
                if (sqlCommand.ExecuteNonQuery() > 0)
                {
                    newg.Attributes["ID"] = ProjectID.Text;
                    Projects.Locations.Add(new ProjectLocation()
                    {
                        Default = false,
                        DefinitionFile = "",
                        Description = ProjectDes.Text,
                        ID = ProjectID.Text,
                        X = (newg.Geometry as MapPoint).X,
                        Y = (newg.Geometry as MapPoint).Y
                    });
                    sqlCommand = new SqlCommand(
                        "INSERT [dbo].[CF_Privilege] ([PrivilegeID], [PrivilegeMaster], [PrivilegeMasterValue], [PrivilegeAccess], [PrivilegeAccessValue], [PrivilegeOperation]) VALUES (" + (Projects.Locations.Count) + ", N'User', N'1', N'ProjectListInfo', N'" +
                        (Projects.Locations.Count) + "', N'enabled')", sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand = new SqlCommand("Create DataBase " + ProjectID.Text + ";", sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                    Directory.CreateDirectory(Runtime.dataPath + "\\" + ProjectID.Text.Trim());
                    File.Copy(Runtime.dataPath + "\\Example\\Example.py", Runtime.dataPath + "\\" + ProjectID.Text.Trim() + "\\" + ProjectID.Text + ".py");
                    File.Copy(Runtime.dataPath + "\\Example\\Example.xml", Runtime.dataPath + "\\" + ProjectID.Text.Trim() + "\\" + ProjectID.Text + ".xml");
                }

                sqlConnection.Close();
                AddPop.IsOpen = false;
                MessageBox.Show("成功新增工程：" + ProjectID.Text);
                ProjectDes.Text = "";
                ProjectID.Text = "";
            }
            catch { }
          
        }

        private async void ProjectPin_Click(object sender, RoutedEventArgs e)
        {
            await drawGraphics();
        }
        Graphic newg;
        public async Task drawGraphics()
        {
            try
            {
                Geometry geom = await MyMapView.Editor.RequestShapeAsync(DrawShape.Point);
                MapPoint point = geom as MapPoint;
                newg = new Graphic()
                {
                    Geometry = new MapPoint(point.X, point.Y),
                    Symbol = _MarkerSymbol_Normal,
                };
                gLayer.Graphics.Add(newg);
            }
            catch
            { }

        }
    }
}
