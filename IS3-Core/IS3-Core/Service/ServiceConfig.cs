using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS3.Core.Service
{
    public static class ServiceConfig
    {
        //http://139.196.73.142:8011/api/geology/borehole?project=SHML12
        public static string IP = "139.196.73.142";
        public static string PortNum = "8011";
        public static string BaseURL = string.Format("http://{0}:{1}/", IP, PortNum);
        public static string TokenURL = BaseURL + "/token";
        //获取列表格式
        public static string DGObjectListFormat = "api/{0}/{1}?project={2}";
        //获取单个数据格式
        public static string DGObjectByIDFormat = "api/{0}/{1}?project={2}&id={3}";
    }
}
