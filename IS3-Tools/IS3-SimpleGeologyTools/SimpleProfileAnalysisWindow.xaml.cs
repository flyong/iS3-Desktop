using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using IS3.Core;
using IS3.Core.Geometry;
using IS3.Core.Graphics;
using IS3.Core.Service;
using IS3.Geology;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IS3.SimpleGeologyTools
{
    #region Copyright Notice
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
    #endregion

    public class GeoProjSettings
    {
        public bool drawProjectionLine { get; set; }
        public bool drawBorehole { get; set; }
        public bool drawStratum { get; set; }
        public bool extendBorehole { get; set; }
        public bool clipInProjectionLine { get; set; }
        public double xOffset { get; set; }
        public double zScale { get; set; }
        public int interval { get; set; }

        public double scale { get; set; }   // New! map scale.
        public string mapID { get; set; }

        public GeoProjSettings()
        {
            drawProjectionLine = false;
            drawBorehole = true;
            drawStratum = true;
            extendBorehole = true;
            clipInProjectionLine = true;
            xOffset = 0.0;
            zScale = 1.0;
            interval = 10;

            scale = 1.0;
        }
    }

    /// <summary>
    /// Interaction logic for SimpleProfileAnalysisWindow.xaml
    /// </summary>
    public partial class SimpleProfileAnalysisWindow : Window
    {
        Project _prj;                       // the project
        Domain _geologyDomain;              // the geology domain of the project
        Domain _structureDomain;            // the structure domain of the project
        IMainFrame _mainFrame;              // the main frame
        IView _inputView;                   // the input view (boreholes, projeciton line)
        List<DGObject> _allBhs;        // all the boreholes
        List<DGObject> _allSts;        // all the strata
        List<DGObject> _allTunnels;    // all the tunnels
                                            // selected boreholes dictionay
        Dictionary<string, IEnumerable<DGObject>> _selectedBhsDict;
        Dictionary<string, IEnumerable<DGObject>> _selectedTunnelsDict; 
        List<string> _bhLayerIDs;           // borehole layer IDs
        List<string> _tunnelLayerIDs;       // tunnel layer IDs
        GeoProjSettings _settings;          // the analysis settings
        bool _initFailed;                   // set to true if initialization failed
        IPolyline _projLine;                // projection line
        

        public SimpleProfileAnalysisWindow()
        {
            InitializeComponent();
            
            _settings = new GeoProjSettings();
            SettingsHolder.DataContext = _settings;
            Loaded += SimpleProfileAnalysisWindow_Loaded;
            Unloaded += SimpleProfileAnalysisWindow_Unloaded;

            _mainFrame = Globals.mainframe;
            _prj = Globals.project;

            if (_mainFrame == null || _prj == null) { _initFailed = true; return; }

            _geologyDomain = _prj.getDomain(DomainType.Geology);
            _structureDomain = _prj.getDomain(DomainType.Structure);

            // set the input view
            _inputView = _mainFrame.views.Where(x => x.name== "BaseMap").FirstOrDefault();
            InputViewTB.DataContext = _inputView;

            // add a listener to object selection changed event
            _inputView.objSelectionChangedTriggerOuter += 
                _inputView_objSelectionChangedListener;
            // add a listener to drawing graphics changed event
            (_inputView as IView2D).drawingGraphicsChangedTrigger += 
                _inputView_drawingGraphicsChangedListener;
        }
        async Task LoadAsync()
        {
            _allBhs = await _geologyDomain.getObjects("Borehole");
            _allSts =await _geologyDomain.getObjects("Stratum");
            _bhLayerIDs = new List<string>();
            _bhLayerIDs.Add("Borehole");

            // add borehole layer as selectable layer
           ( _inputView as IView2D).removeSelectableLayer("_ALL");
            (_inputView as IView2D).addSeletableLayer("0");     // "0" is the drawing layer ID
            foreach (string layerID in _bhLayerIDs)
                (_inputView as IView2D).addSeletableLayer(layerID);
        }
        void SimpleProfileAnalysisWindow_Loaded(object sender,
            RoutedEventArgs e)
        {
            //Application curApp = Application.Current;
            //Window mainWindow = curApp.MainWindow;
            //this.Owner = mainWindow;

            // position window to the bottom-right
            //this.Left = mainWindow.Left +
            //    (mainWindow.Width - this.ActualWidth - 10);
            //this.Top = mainWindow.Top +
            //    (mainWindow.Height - this.ActualHeight - 10);

            if (_initFailed)
                return;

            // fill output view combobox
            List<IView> profileViews = new List<IView>();
            foreach (IView view in _mainFrame.views)
            {
                if (((view as IView2D)!=null) && ((view as IView2D).eMap.MapType == EngineeringMapType.GeneralProfileMap))
                    profileViews.Add(view);
            }
            OutputCB.ItemsSource = profileViews;
            if (profileViews.Count > 0)
                OutputCB.SelectedIndex = 0;
            LoadAsync();
        }

        void SimpleProfileAnalysisWindow_Unloaded(object sender,
            RoutedEventArgs e)
        {
            if (!_initFailed)
            {
                (_inputView as IView2D).addSeletableLayer("_ALL");
                // remove the listener to object selection changed event
                _inputView.objSelectionChangedTriggerOuter -=
                    _inputView_objSelectionChangedListener;
                // remove the listener to drawing graphics changed event
                (_inputView as IView2D).drawingGraphicsChangedTrigger -=
                    _inputView_drawingGraphicsChangedListener;
            }
        }
        List<DGObject> myList;
        async Task SeelctChange()
        {
            _selectedBhsDict = new Dictionary<string, IEnumerable<DGObject>>();
            myList= new List<DGObject>();
            string result = await Task.Run(() =>
               WebApiCaller.HttpGet(ServiceConfig.BaseURL +
               string.Format(ServiceConfig.DGObjectListFormat, "geology", "borehole", "TONGJI")));
            //
            JObject _obj = JObject.Parse(result);
            string data = _obj["data"].ToString();
            JArray objList = JArray.Parse(data);
            foreach (JToken token in objList)
            {
                var obj0 = JsonConvert.DeserializeObject<Borehole>(token.ToString());
                myList.Add(obj0);
            }
            _selectedBhsDict["Borehole"] = myList;
            _allBhs = new List<DGObject>();

            List<DGObject> _bhs = new List<DGObject>();
            foreach (var item in _selectedBhsDict.Values)
            {
                foreach (var obj in item)
                {
                    _bhs.Add(obj);
                }
            }
            if (_bhs != null && _bhs.Count() > 0)
            {
                BoreholeCB.ItemsSource = _bhs;
                BoreholeCB.SelectedIndex = 0;
            }

        }
        void _inputView_objSelectionChangedListener(object sender,
            ObjSelectionChangedEventArgs e)
        {
            // fill borehole combobox
            if (_prj == null) return;


            SeelctChange();
        }

        void _inputView_drawingGraphicsChangedListener(object sender,
            DrawingGraphicsChangedEventArgs e)
        {
            if (_inputView == null) return;
            if (RB2.IsChecked.Value)
            {
                List<Tuple<IGraphic, string>> polylines =
                getPolylines((_inputView as IView2D).drawingLayer);
                LineLB.ItemsSource = polylines;
                LineLB.SelectedIndex = LineLB.Items.Count - 1;
            }
        }

        List<Tuple<IGraphic,string>> getPolylines(IGraphicsLayer gLayer)
        {
            if (gLayer == null)
                return null;

            List<Tuple<IGraphic, string>> result = new List<Tuple<IGraphic, string>>();
            int i = 1;
            foreach (IGraphic g in gLayer.graphics)
            {
                IGeometry geom = g.Geometry;
                if (geom == null)
                    continue;
                if (geom.GeometryType == GeometryType.Polyline)
                {
                    Tuple<IGraphic, string> turple = new Tuple<IGraphic, string>(
                        g, "Polyline#" + i.ToString());
                    i++;
                    result.Add(turple);
                }
            }
            return result;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            //updata the line list box
            if (LineLB != null)
                LineLB.ItemsSource = null;
            _inputView_objSelectionChangedListener(null, null);
            _inputView_drawingGraphicsChangedListener(null, null);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (_initFailed)
                return;

            ProjectBoreholeResult result = StartAnalysis();
            AfterAnalysis(result);
            
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        ProjectBoreholeResult StartAnalysis()
        {
            string mapID = (_inputView as IView2D).eMap.MapID;
            _settings.mapID = mapID;

            // check all needed data is set up correctly
            if (_selectedBhsDict == null || _selectedBhsDict.Count() == 0)
                return null;
            IGraphic g = Runtime.graphicEngine.newGraphic();
            // get the project line
            if (RB2.IsChecked.Value)
            {
                Tuple<IGraphic, string> selectedLine =
                    LineLB.SelectedItem as Tuple<IGraphic, string>;
                if (selectedLine == null)
                    return null;
                g = selectedLine.Item1;
            }
            if (g == null)
                return null;
            // the graphic must be a polyline
            _projLine = g.Geometry as IPolyline;
            if (_projLine == null)
                return null;
            // get borehole (x,y) coordinates,
            // and generate a list of Tuple<Borehole, IMapPoint>.
            List<Tuple<Borehole, IMapPoint>> input =
                new List<Tuple<Borehole, IMapPoint>>();
            foreach (string bhLayerID in _selectedBhsDict.Keys)
            {
                IEnumerable<DGObject> bhs = _selectedBhsDict[bhLayerID];
                IGraphicsLayer gLayer = (_inputView as IView2D).getLayer(bhLayerID);
                if (gLayer == null)
                    continue;
                foreach (var obj in bhs)
                {
                    Borehole bh = obj as Borehole;
                    if (bh == null)
                        continue;
                    IGraphicCollection gc = gLayer.getGraphics(bh);
                    if (gc.Count == 0)
                        continue;
                     g = gc[0];
                    IMapPoint p = g.Geometry as IMapPoint;
                    if (p == null)
                        continue;
                    input.Add(new Tuple<Borehole, IMapPoint>(bh, p));
                }
            }

            // do the analysis
            ProjectBoreholeResult result =
                SimpleProfileAnalysis.ProjectBoreholes(input, _projLine, _settings);
            return result;
        }

        // sample code for generating graphics and a layer
        IGraphicsLayer testGraphicsLayer()
        {
            var sym_line =
                Runtime.graphicEngine.newSimpleLineSymbol(
                Colors.Blue, SimpleLineStyle.Solid, 1.0);
            var sym_fill =
                Runtime.graphicEngine.newSimpleFillSymbol(
                Colors.LightBlue, SimpleFillStyle.Solid, sym_line);
            var renderer =
                Runtime.graphicEngine.newSimpleRenderer(sym_fill);
            var p1 = Runtime.geometryEngine.newMapPoint(30, 30);
            var p2 = Runtime.geometryEngine.newMapPoint(30, 70);
            var p3 = Runtime.geometryEngine.newMapPoint(50, 50);
            var triangle =
                Runtime.graphicEngine.newTriangle(p1, p2, p3);
            var layer =
                Runtime.graphicEngine.newGraphicsLayer("test", "test");
            layer.setRenderer(renderer);
            layer.addGraphic(triangle);
            return layer;
        }
        IView outView;
        void AfterAnalysis(ProjectBoreholeResult result)
        {
            if (result == null)
                return;

            IView profileView = OutputCB.SelectedItem as IView;
            outView = profileView;
            (profileView as IView2D).eMap.profileLine = _projLine;

            // test code:
            //profileView.addLayer(testGraphicsLayer());

            // Assign name to each graphic objects,
            // so that the graphics layer can sync them with corresponding object.
            foreach(int bhID in result.bhGraphics.Keys)
            {
                Borehole bh = myList.Where(x=>x.id==bhID).FirstOrDefault() as Borehole;
                IGraphicCollection gc = result.bhGraphics[bhID];
                foreach (IGraphic g in gc)
                    g.Attributes["Name"] = bh.name;
            }
            foreach(int stID in result.stGraphics.Keys)
            {
                Stratum st = _allSts[stID] as Stratum;
                IGraphicCollection gc = result.stGraphics[stID];
                foreach (IGraphic g in gc)
                    g.Attributes["Name"] = st.name;
            }

            // add graphic to the view
            // boreholes displayed on top of strata
            HashSet<IGraphicsLayer> bhGraphicLayers = new HashSet<IGraphicsLayer>();
            HashSet<IGraphicsLayer> stGraphicLayers = new HashSet<IGraphicsLayer>();
            foreach (int id in result.stGraphics.Keys)
            {
                IGraphicCollection gc = result.stGraphics[id];
                DGObject obj = _allSts[id];
                string layerID = "Borehole";
                IGraphicsLayer gLayerSt = getStratumLayer(profileView, layerID);
                gLayerSt.addGraphics(gc);
                stGraphicLayers.Add(gLayerSt);
            }
            foreach (int id in result.bhGraphics.Keys)
            {
                IGraphicCollection gc = result.bhGraphics[id];
                DGObject obj = myList.Where(x => x.id == id).FirstOrDefault() as Borehole;
                string layerID = "Borehole";
                IGraphicsLayer gLayerBh = getBoreholeLayer(profileView, layerID);
                gLayerBh.addGraphics(gc);
                bhGraphicLayers.Add(gLayerBh);
            }

            //// sync objects with graphics
            //List<DGObject> bhs = _allBhs.merge();
            //List<DGObject> sts = _allSts.merge();
            //foreach (IGraphicsLayer gLayer in bhGraphicLayers)
            //    gLayer.syncObjects(bhs);
            //foreach (IGraphicsLayer gLayer in stGraphicLayers)
            //    gLayer.syncObjects(sts);

            _mainFrame.activeView = profileView;
           ( profileView as  IView2D).zoomTo(result.Extent);
        }

        IGraphicsLayer getBoreholeLayer(IView view, string layerID)
        {
            IGraphicsLayer gLayerBh = (view as IView2D).getLayer(layerID);
            if (gLayerBh == null)
            {
                gLayerBh = Runtime.graphicEngine.newGraphicsLayer(
                    layerID, layerID);
                var sym_fill = GraphicsUtil.GetDefaultFillSymbol();
                var renderer = Runtime.graphicEngine.newSimpleRenderer(sym_fill);
                gLayerBh.setRenderer(renderer);
                (view as IView2D).addLayer(gLayerBh);
            }
            return gLayerBh;
        }

        IGraphicsLayer getStratumLayer(IView view, string layerID)
        {
            IGraphicsLayer gLayerSt = (view as IView2D).getLayer(layerID);
            if (gLayerSt == null)
            {
                gLayerSt = Runtime.graphicEngine.newGraphicsLayer(
                    layerID, layerID);
                var sym_fill = GraphicsUtil.GetDefaultFillSymbol();
                var renderer = Runtime.graphicEngine.newSimpleRenderer(sym_fill);
                gLayerSt.setRenderer(renderer);
                gLayerSt.Opacity = 0.5;
                (view as IView2D).addLayer(gLayerSt);
            }
            return gLayerSt;
        }

    }
}
