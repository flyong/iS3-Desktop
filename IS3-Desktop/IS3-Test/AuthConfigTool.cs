using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IS3_Test
{
    public class AuthConfigTool
    {
        public static string auth(string username,string password)
        {
            string result = "";

            string serviceAddress = "http://139.196.73.142:8011/token";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string strContent = string.Format("grant_type=password&password={1}&username={0}",username,password);
            byte[] data = Encoding.UTF8.GetBytes(strContent);
            request.ContentLength = data.Length;

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            Stream stream = resp.GetResponseStream();

            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
       public static void post(string token)
        {
            string result = "";

            string serviceAddress = "http://139.196.73.142:8011/api/project/info/LXD";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);

            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", token);

            string strContent = @"{""ProjectName"": ""iS3Demo""}";

            using (StreamWriter dataStream = new StreamWriter(request.GetRequestStream()))
            {
                dataStream.Write(strContent);
                dataStream.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();

            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
        }
    }
}
