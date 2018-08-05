using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IS3.Core;
using IS3.Core.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IS3.Monitoring
{
    public class MonPoint:DGObject
    {
        public string MonPointType { get; set; }
        public string XCoordinate { get; set; }
        public string YCoordinate { get; set; }
        public string ZCoordinate { get; set; }
        public int MonGroupID { get; set; }
        public string SensorName { get; set; }
        public string IniValue { get; set; }
        public string STime { get; set; }
        public string PerInfoID { get; set; }
        public string Remark { get; set; }
        public List<MonReading> MonDatas { get; set; }
        public override async Task<List<FrameworkElement>> chartViews(IEnumerable<DGObject> objs, double width, double height)
        {
            List<FrameworkElement> charts = new List<FrameworkElement>();
            FrameworkElement chart =
                FormsCharting.getMonPointChart(objs, width, height);
            charts.Add(chart);
            return charts;
        }
    }
}
