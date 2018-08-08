using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using iS3.Core.Service;
using Newtonsoft.Json.Linq;

namespace iS3.Core.Models
{
    public class AuthLogin
    {
        private string token;
        public string Token { get { return token; } set { token = value; } }

        public async Task<string> GetToken(string username, string password)
        {
            try
            {
                string result = "";

                string serviceAddress = ServiceConfig.TokenURL;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string strContent = string.Format("grant_type=password&password={1}&username={0}", username, password);
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
                JObject obj = JObject.Parse(result);
                Token = "Bearer " + obj["access_token"].ToString();
                return Token; ;
            }
            catch {
                return null;
            }
        }

        public  void  QueryByToken(string token)
        {
            string result = "";

            string serviceAddress = "http://47.98.187.240:8011/api/project/info/LXD";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);

            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization",token);

            string strContent = @"{""ProjectName"": ""test""}";

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