using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace iS3.Core.Service
{
    public class FileService
    {
        public static async Task Load(string filePath, string fileName)
        {
            string url = ServiceConfig.BaseURL + string.Format(ServiceConfig.FileGetFormat, "iS3Demo", fileName);
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";
            myRequest.Timeout = 4000;
            HttpWebResponse myResponse = null;

            myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();

            FileStream fs = new FileStream(filePath+fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(content);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();

        }
    }
}
