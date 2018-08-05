using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IS3_Test
{
    public class LoadFile
    {
        public void Load(string filePath, string fileName,string token)
        {
            string url = "http://139.196.73.142:8011/api/file/iS3Demo?file=SHML12.py";
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";
            myRequest.Timeout = 4000;
            HttpWebResponse myResponse = null;

            myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();

            FileStream fs = new FileStream("E://SHML12.py", FileMode.Create);
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
