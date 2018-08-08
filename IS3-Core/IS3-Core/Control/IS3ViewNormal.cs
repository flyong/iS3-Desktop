using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace iS3.Core
{
    public class IS3ViewNormal : IView
    {
        public IS3ViewNormal(UserControl _parent)
        {
            this._parent = _parent;
        }
        protected Project _prj;
        public Project prj
        {
            get { return _prj; }
            set { _prj = value; }
        }

        public ViewType type
        {
            get { return ViewType.None; }
        }

        public string name { get; set; }

        public ViewBaseType baseType
        {
            get { return ViewBaseType.Normal; }
        }
        protected UserControl _parent;
        public UserControl parent
        {
            get { return _parent; }
        }


        public event EventHandler<DGObjectsSelectionChangedEventArgs> DGObjectsSelectionChangedTriggerOuter;
        public event EventHandler<ObjSelectionChangedEventArgs> objSelectionChangedTriggerOuter;
        public event EventHandler<DGObjectsSelectionChangedEventArgs> DGObjectsSelectionChangedTriggerInner;
        public event EventHandler<ObjSelectionChangedEventArgs> objSelectionChangedTriggerInner;
        public virtual void highlightAll(bool on = true)
        {
            
        }

        public virtual void highlightObject(DGObject obj, bool on = true)
        {
            
        }

        public virtual void highlightObjects(IEnumerable<DGObject> objs, bool on = true)
        {
            
        }

        public virtual void highlightObjects(IEnumerable<DGObject> objs, string layerID, bool on = true)
        {

        }

        public virtual Task initializeView()
        {
            return null;
        }

        public void load()
        {
            Globals.mainframe.objSelectionChangedTrigger += objSelectionChangedListenerOuter;
            objSelectionChangedTriggerOuter += Globals.mainframe.objSelectionChangedListener;

            Globals.mainframe.dGObjectsSelectionChangedTrigger += DGObjectsSelectionChangedListenerOuter;
            DGObjectsSelectionChangedTriggerOuter += Globals.mainframe.DGObjectsSelectionChangedListener;
        }



        public virtual void onClose()
        {
            Globals.mainframe.objSelectionChangedTrigger -= objSelectionChangedListenerOuter;
            objSelectionChangedTriggerOuter -= Globals.mainframe.objSelectionChangedListener;

            Globals.mainframe.dGObjectsSelectionChangedTrigger -= DGObjectsSelectionChangedListenerOuter;
            DGObjectsSelectionChangedTriggerOuter -= Globals.mainframe.DGObjectsSelectionChangedListener;
        }

        public void DGObjectsSelectionChangedListenerOuter(object sender, DGObjectsSelectionChangedEventArgs e)
        {
            if ((sender as UserControl) == parent) return;
            if (DGObjectsSelectionChangedTriggerInner != null)
            {
                DGObjectsSelectionChangedTriggerInner(sender, e);
            }
        }

        public void objSelectionChangedListenerOuter(object sender, ObjSelectionChangedEventArgs e)
        {
            if ((sender as UserControl) == parent) return;
            if (objSelectionChangedTriggerInner != null)
            {
                objSelectionChangedTriggerInner(sender, e);
            }
        }

        public void DGObjectsSelectionChangedListenerInner(object sender, DGObjectsSelectionChangedEventArgs e)
        {
            if (DGObjectsSelectionChangedTriggerOuter != null)
            {
                DGObjectsSelectionChangedTriggerOuter(sender, e);
            }
        }

        public void objSelectionChangedListenerInner(object sender, ObjSelectionChangedEventArgs e)
        {
          
            if (objSelectionChangedTriggerOuter != null)
            {
                    objSelectionChangedTriggerOuter(sender, e);
            }
        }
    }
}
