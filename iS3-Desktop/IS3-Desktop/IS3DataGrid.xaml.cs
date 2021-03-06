﻿using System;
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
using System.ComponentModel;
using System.Globalization;
using System.Collections;

using IS3.Core;
using System.Collections.ObjectModel;
using System.Threading;

namespace IS3.Desktop
{
    public class ObjectValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return ObjectHelper.ObjectToString(value, true);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Interaction logic for IS3DataGrid.xaml
    /// </summary>
    /// 
    public partial class IS3DataGrid : UserControl, IViewHolder
    {
        public EventHandler<ObjSelectionChangedEventArgs> objSelectionChangedTrigger;
        public void DGObjectsSelectionChangedListener(object sender, DGObjectsSelectionChangedEventArgs e)
        {
            try
            {
                if (sender != this)
                {

                    GetData(e.newOne);
                }
            }
            catch (Exception ex)
            {

            }

        }
        public void ObjSelectionChangedListener(object sender, ObjSelectionChangedEventArgs e)
        {
            if (sender != this)
            {

            }
        }
        public async Task GetData(DGObjects objs)
        {
            DGObjectRepository repository = DGObjectRepository.Instance(
                 Globals.project.projDef.ID, objs.parent.name, objs.definition.Type);

            //layerName = Globals.project[tree.RefDomainName].GetDef(tree.Name).FirstOrDefault().GISLayerName;
            List<DGObject> objList = await repository.GetAllAsync();
            foreach (DGObject obj in objList)
            {
                obj.parent = objs;
            }
            //foreach (IView view in views)
            //{
            //    int count = view.syncObjects(layerName, objList);
            //}
            //await DGObjectDataGrid.Dispatcher.BeginInvoke(new Action(() =>
            // {
            //     
            // }));

            //this.DGObjectDataGrid.Dispatcher.Invoke(new updateDelegate(Update), objList);
           DGObjectDataGrid.ItemsSource = objList;
        }

        protected int _maxColWith = 300;
        //protected ObjectValueConverter _objectConverter
        //    = new ObjectValueConverter();

        public IS3DataGrid()
        {
            InitializeComponent();

            Loaded += IS3DataGrid_Loaded;
            _view = new IS3ViewNormal(this);

            _view.DGObjectsSelectionChangedTriggerInner += DGObjectsSelectionChangedListener;
            objSelectionChangedTrigger += view.objSelectionChangedListenerInner;
            view.objSelectionChangedTriggerInner += ObjSelectionChangedListener;
        }

        private void IS3DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //DGObjectDataGrid.ItemsSource = list;
        }

        private void DGObjectDataGrid_AutoGeneratingColumn(object sender,
            DataGridAutoGeneratingColumnEventArgs e)
        {
            // "Graphics" and "Attributes" are used internally.
            if (e.PropertyName == "Graphics" ||
                e.PropertyName == "Attributes" ||
                e.PropertyName == "IsSelected" ||
                e.PropertyName == "OBJECTID" ||
                e.PropertyName == "SHAPE" ||
                e.PropertyName == "Shape" ||
                e.PropertyName == "SHAPE_Length" ||
                e.PropertyName == "Shape_Length" ||
                e.PropertyName == "SHAPE_Area" ||
                e.PropertyName == "Shape_Area"
                )
            {
                e.Cancel = true;
                return;
            }

            //DataGridTextColumn tcol = e.Column as DataGridTextColumn;
            //if (tcol == null)
            //    return;

            //// Does the column data type contain the ICollection interface?
            //// If yes, we need the CollectionValueConverter to display data.
            //if (typeof(ICollection).IsAssignableFrom(e.PropertyType))
            //{
            //    Binding binding = tcol.Binding as Binding;
            //    binding.Converter = _objectConverter;
            //}
            //// Is the column data class type other than String?
            //// If yes, we need the ClassValueConverter to display data.
            //else if (e.PropertyType.IsClass && e.PropertyType.Name != "String")
            //{
            //    Binding binding = tcol.Binding as Binding;
            //    binding.Converter = _objectConverter;
            //}
        }

        private void DGObjectDataGrid_AutoGeneratedColumns(object sender,
            EventArgs e)
        {
            if (DGObjectDataGrid.Columns.Count == 0)
                return;

            try
            {
                DataGridColumn col =
                    DGObjectDataGrid.Columns.FirstOrDefault(
                    c => c.Header.ToString() == "ID");
                if (col != null)
                    col.DisplayIndex = 0;

                col = DGObjectDataGrid.Columns.FirstOrDefault(
                    c => c.Header.ToString() == "Name");
                if (col != null)
                    col.DisplayIndex = 1;

                col = DGObjectDataGrid.Columns.FirstOrDefault(
                    c => c.Header.ToString() == "FullName");
                if (col != null)
                    col.DisplayIndex = 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            foreach (DataGridColumn _iter in DGObjectDataGrid.Columns)
            {
                //_iter.MaxWidth = 300;
            }
        }
        DGObject _lastObj = null;
        protected IView _view;
        public IView view
        {
            get { return _view; }
        }

        private void DGObjectDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGObjectDataGrid.IsKeyboardFocusWithin == false)
                return;
            List<DGObject> addedObjs = new List<DGObject>();
            List<DGObject> removedObjs = new List<DGObject>();
            DGObject selectOne = DGObjectDataGrid.SelectedItem as DGObject;
            addedObjs.Add(selectOne);
            if (_lastObj != null)
            {
                removedObjs.Add(_lastObj);
            }
            if (objSelectionChangedTrigger != null)
            {
                Dictionary<string, IEnumerable<DGObject>> addedObjsDict = null;
                Dictionary<string, IEnumerable<DGObject>> removedObjsDict = null;
                if (addedObjs.Count > 0)
                {
                    addedObjsDict = new Dictionary<string, IEnumerable<DGObject>>();
                    addedObjsDict[selectOne.parent.definition.Name] = addedObjs;
                }
                if (removedObjs.Count > 0)
                {
                    removedObjsDict = new Dictionary<string, IEnumerable<DGObject>>();
                    removedObjsDict[_lastObj.parent.definition.Name] = removedObjs;
                }
                ObjSelectionChangedEventArgs args =
                    new ObjSelectionChangedEventArgs();
                args.addedObjs = addedObjsDict;
                args.removedObjs = removedObjsDict;
                objSelectionChangedTrigger(this, args);
            }
            _lastObj = selectOne;
        }

        public void setCoord(string coord)
        {

        }
    }
}