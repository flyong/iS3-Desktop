using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IS3.Core
{
    public class iS3Property
    {
        public static Type GetType(string domain, string DGObject)
        {

            string dllName = string.Format("IS3.{0}", domain);
            if (!DllImport.assemblyDict.ContainsKey(dllName))
            {
                return null;
            }
            string nameSpace = string.Format("IS3.{0}.{1}", domain, DGObject);
            Assembly assembly = DllImport.assemblyDict[dllName];
            Type t = assembly.GetType(nameSpace);
            return t;
        }
        public static List<T> Convert<T>(List<DGObject> objs) where T : class
        {
            List<T> list = new List<T>();
            foreach (DGObject obj in objs)
            {
                list.Add(obj as T);
            }
            return list;
        }
    }
}
