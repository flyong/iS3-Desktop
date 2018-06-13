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
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using IS3.Core.Geometry;
using IS3.Core.Graphics;
using IS3.Monitoring;

namespace DemoTools
{
    /// <summary>
    /// TestUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TestUserControl : UserControl, IExteralUI
    {
        #region inteface
        public TestUserControl()
        {
            InitializeComponent();

        }
        string _name = "test";
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        LayoutAnchorable _parent;
        public LayoutAnchorable parent
        { get { return _parent; } set { _parent = value; } }
        public UserControl content
        {
            get { return this; }
            set { }
        }
        bool _isActive = false;
        public bool isActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        #endregion
        //定义全局变量
        Project _prj;
        Domain _MonitorDomain;
        IMainFrame _mainFrame;
        IView _inputView;

        //DGObject members
        DGObjectsCollection _allMons; //所有监测点对象
        List<string> _monLayerIDs; //监测图层ID
        Dictionary<string, IEnumerable<DGObject>> _selectedMonsDict; //选中的衬砌

        //graphics members
        ISpatialReference _spatialRef; //视图坐标系

        //result
        Dictionary<int, int> _MonGrade; //用来存储监测点监测状况
        Dictionary<int, IGraphicCollection> _MonGraphics; //分析结果存储

        public void initial()
        {
            //初始化全局变量
            _selectedMonsDict = new Dictionary<string, IEnumerable<DGObject>>();
            _MonGrade = new Dictionary<int, int>();
            _MonGraphics = new Dictionary<int, IGraphicCollection>();

            _mainFrame = Globals.mainframe;
            _prj = Globals.project;
            _MonitorDomain = _prj.getDomain(DomainType.Monitoring);
            _allMons = _MonitorDomain.getObjects("MonPoint");
            _monLayerIDs = new List<string>();
            foreach (DGObjects objs in _allMons)
                _monLayerIDs.Add(objs.definition.GISLayerName);

            Loaded += MonitorDemoTool_Loaded;
            Unloaded += MonitorDemoTool_Unloaded;
        }

        private void MonitorDemoTool_Unloaded(object sender, RoutedEventArgs e)
        {
            _inputView.addSeletableLayer("_ALL");
            _inputView.objSelectionChangedTrigger -=
                _inputView_objSelectionChangedListener;
        }

        private void MonitorDemoTool_Loaded(object sender, RoutedEventArgs e)
        {
            //设置input view combobox数据源
            List<IView> planViews = new List<IView>();
            foreach (IView view in _mainFrame.views)
            {
                if (view.eMap.MapType == EngineeringMapType.FootPrintMap)
                    planViews.Add(view);
            }
            InputCB.ItemsSource = planViews;
            if (planViews.Count > 0)
            {
                _inputView = planViews[0];
                InputCB.SelectedIndex = 0;
            }
            else
            {
                return;
            }

            //设置segmenglining listbox数据源
            _inputView_objSelectionChangedListener(null, null);
        }
        //视图对象选择监听事件
        void _inputView_objSelectionChangedListener(object sender,
            ObjSelectionChangedEventArgs e)
        {
            //设置MonPoint listbox数据源
            _selectedMonsDict = _prj.getSelectedObjs(_MonitorDomain, "MonPoint");
            List<DGObject> _mons = new List<DGObject>();
            foreach (var item in _selectedMonsDict.Values)
            {
                foreach (var obj in item)
                {
                    _mons.Add(obj);
                }
            }
            if (_mons != null && _mons.Count() > 0)
                SLLB.ItemsSource = _mons;
        }

        //input view cobombox选择事件
        private void InputCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //上一次选择的view
            _inputView.addSeletableLayer("_ALL");
            _inputView.objSelectionChangedTrigger -=
                    _inputView_objSelectionChangedListener;

            //新选择的view
            _inputView = InputCB.SelectedItem as IView;
            _inputView.removeSelectableLayer("_ALL");
            foreach (string layerID in _monLayerIDs)
                _inputView.addSeletableLayer(layerID);

            //为新的view添加对象选择监听事件
            _inputView.objSelectionChangedTrigger +=
                _inputView_objSelectionChangedListener;
        }


        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if ((TB_Max.Text == "") || (TB_Max.Text == null)) return;
            StartAnalysis();
            SyncToView();
        }

        //关闭按钮事件
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            isActive = false;
            parent.Hide();
        }
        void StartAnalysis()
        {
            double max = double.Parse(TB_Max.Text);
            //获取输入的view和复制坐标系
            IView view = InputCB.SelectedItem as IView;
            _spatialRef = view.spatialReference;

            //开始分析
            foreach (string MonLayerID in _selectedMonsDict.Keys)
            {
                //获取衬砌选中列表
                IEnumerable<DGObject> mons = _selectedMonsDict[MonLayerID];
                List<DGObject> monList = mons.ToList();
                IGraphicsLayer gLayer = _inputView.getLayer(MonLayerID);
                foreach (DGObject dg in monList)
                {

                    //获取单个监测点对象，计算监测状况
                    MonPoint mp = dg as MonPoint;

                    int grade = 0;
                    foreach (string key in mp.readingsDict.Keys)
                    {
                        List<MonReading> mrList = mp.readingsDict[key];
                        foreach (MonReading mr in mrList)
                        {
                            if (Math.Abs(mr.value) > max)
                            {
                                grade = 1;
                                break;
                            }
                        }
                        if (grade == 1) break;
                    }


                    //根据评估等级获取图形样式
                    ISymbol symbol = GetSymbol(grade);

                    //为了演示，采用了较复杂的方法
                    //<简便方法 可替换下面代码>
                    //IGraphicCollection gcollection = gLayer.getGraphics(sl);
                    //IGraphic g = gcollection[0];
                    //g.Symbol = symbol;
                    //IGraphicCollection gc = Runtime.graphicEngine.newGraphicCollection();
                    //gc.Add(g);
                    //_slsGraphics[sl.id] = gc;
                    //</简便方法>

                    //获取衬砌图形
                    IGraphicCollection gcollection = gLayer.getGraphics(dg);
                    IGraphic g = gcollection[0];
                    IPolygon ip = g.Geometry as IPolygon; //获取端点
                    IPointCollection ipc = ip.GetPoints();
                    //导入的监测点不是点类型，是个圆所以要转换，如果是点，自己可以转换为IMapPoint
                    double centerX = 0;
                    double centerY = 0;
                    foreach (IMapPoint point in ipc)
                    {
                        centerX += point.X;
                        centerY += point.Y;
                    }
                    if (ipc.Count > 0)
                    {
                        centerX = centerX / ipc.Count;
                        centerY = centerY / ipc.Count;
                    }
                    double offset = 2;
                    //新建新的点，注意复制坐标系
                    IMapPoint p1 = Runtime.geometryEngine.newMapPoint(centerX - offset, centerY - offset, _spatialRef);
                    IMapPoint p2 = Runtime.geometryEngine.newMapPoint(centerX - offset, centerY + offset, _spatialRef);
                    IMapPoint p3 = Runtime.geometryEngine.newMapPoint(centerX + offset, centerY + offset, _spatialRef);
                    IMapPoint p4 = Runtime.geometryEngine.newMapPoint(centerX + offset, centerY - offset, _spatialRef);
                    ////生成新的图形
                    g = Runtime.graphicEngine.newQuadrilateral(p1, p2, p3, p4);
                    g.Symbol = symbol;
                    IGraphicCollection gc = Runtime.graphicEngine.newGraphicCollection();
                    gc.Add(g);
                    _MonGraphics[dg.id] = gc; //保存结果
                }
            }
        }

        //在view中加载图形，和同步图形
        void SyncToView()
        {
            IView view = InputCB.SelectedItem as IView;

            //为图形赋值“Name”属性，以便图形和数据关联
            foreach (int monID in _MonGraphics.Keys)
            {
                MonPoint mp = _allMons[monID] as MonPoint;
                IGraphicCollection gc = _MonGraphics[monID];
                foreach (IGraphic g in gc)
                    g.Attributes["Name"] = mp.name;
            }

            //将图形添加到view中
            string layerID = "DemoLayer"; //图层ID
            IGraphicsLayer gLayer = getDemoLayer(view, layerID); //获取图层函数
            foreach (int id in _MonGraphics.Keys)
            {
                IGraphicCollection gc = _MonGraphics[id];
                gLayer.addGraphics(gc);
            }

            //使数据与图形关联
            List<DGObject> sls = _allMons.merge();
            gLayer.syncObjects(sls);

            //计算新建图形范围，并在地图中显示该范围
            IEnvelope ext = null;
            foreach (IGraphicCollection gc in _MonGraphics.Values)
            {
                IEnvelope itemExt = GraphicsUtil.GetGraphicsEnvelope(gc);
                if (ext == null)
                    ext = itemExt;
                else
                    ext = ext.Union(itemExt);
            }
            _mainFrame.activeView = view;
            view.zoomTo(ext);
        }

        //根据评估等级获取样式
        ISymbol GetSymbol(int grade)
        {
            ISimpleLineSymbol linesymbol = Runtime.graphicEngine.newSimpleLineSymbol(
                                Colors.Black, SimpleLineStyle.Solid, 1.0);
            Color color = Colors.Green;
            if (grade == 1)
                color = Colors.Red;
            return Runtime.graphicEngine.newSimpleFillSymbol(color, SimpleFillStyle.Solid, linesymbol);
        }

        //获取新建图层
        IGraphicsLayer getDemoLayer(IView view, string layerID)
        {
            IGraphicsLayer gLayer = view.getLayer(layerID);
            if (gLayer == null)
            {
                gLayer = Runtime.graphicEngine.newGraphicsLayer(
                    layerID, layerID);
                var sym_fill = GraphicsUtil.GetDefaultFillSymbol();
                var renderer = Runtime.graphicEngine.newSimpleRenderer(sym_fill);
                gLayer.setRenderer(renderer);
                gLayer.Opacity = 0.9;
                view.addLayer(gLayer);
            }
            return gLayer;
        }
    }
}
