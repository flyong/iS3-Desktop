using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

using IS3.Core;
using IS3.Core.Geometry;


using System.Diagnostics;
using System.Threading;
using System.Windows.Controls;

namespace IS3.Control
{
    public class U3DViewModel : IView3D
    {
        protected UnityPanel _parent;

        public Project prj
        {
            get { return Globals.project; }
        }
        protected EngineeringMap _map;
        private UnityPanel unityPanel;
        private Process process;

        public EngineeringMap eMap
        {
            get { return _map; }
            set { _map = value; }
        }

        public ViewType type
        {
            get { return ViewType.General3DView; }
        }

        public string name { get { return "Unity3D"; } }

        public UserControl parent => throw new NotImplementedException();
        public ViewBaseType baseType
        {
            get { return ViewBaseType.D3; }
        }


        public event EventHandler viewLoaded;
        public event EventHandler<DGObjectsSelectionChangedEventArgs> DGObjectsSelectionChangedTriggerOuter;
        public event EventHandler<ObjSelectionChangedEventArgs> objSelectionChangedTriggerOuter;
        public event EventHandler<DGObjectsSelectionChangedEventArgs> DGObjectsSelectionChangedTriggerInner;
        public event EventHandler<ObjSelectionChangedEventArgs> objSelectionChangedTriggerInner;

       
        public U3DViewModel(UnityPanel parent)
        {
            _parent = parent;

        }

        public U3DViewModel()
        {
        }

        public async Task<bool> Load3DScene(string filePath)
        {
                try
                {
                process = new Process();
                process.StartInfo.FileName = "TONGJI";
                process.StartInfo.Arguments = _parent.GetArguments();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                process.WaitForInputIdle();

                _parent.EnumChildWindows();
            }
            catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ".\nCheck if Container.exe is placed next to Child.exe.");
                }
            return true;
        }

        public async Task initializeView()
        {
            await Load3DScene("");
        }

        public void onClose()
        {
            try
            {
               
            }
            catch (Exception)
            {

            }
        }

        public void highlightObject(DGObject obj, bool on = true)
        {

        }

        public void highlightObjects(IEnumerable<DGObject> objs, bool on = true)
        {

        }

        public void highlightObjects(IEnumerable<DGObject> objs, string layerID, bool on = true)
        {

        }

        public void highlightAll(bool on = true)
        {

        }

        public IMapPoint screenToLocation(Point screenPoint)
        {
            return null;
        }

        public Point locationToScreen(IMapPoint mapPoint)
        {
            return new Point();
        }

        public int syncObjects()
        {
            return 0;
        }

        public async Task loadPredefinedLayers(string filePath)
        {
           await Load3DScene(filePath);
           
        }

        public void objSelectionChangedListener(object sender, ObjSelectionChangedEventArgs e)
        {

        }



        public void Update()
        {
        }

        public void ExcuteCommand(string command)
        {
          
        }

        public void load()
        {
           
        }


        public void DGObjectsSelectionChangedListenerOuter(object sender, DGObjectsSelectionChangedEventArgs e)
        {

        }

        public void objSelectionChangedListenerOuter(object sender, ObjSelectionChangedEventArgs e)
        {

        }

        public void DGObjectsSelectionChangedListenerInner(object sender, DGObjectsSelectionChangedEventArgs e)
        {

        }

        public void objSelectionChangedListenerInner(object sender, ObjSelectionChangedEventArgs e)
        {

        }
    }
    }
    
