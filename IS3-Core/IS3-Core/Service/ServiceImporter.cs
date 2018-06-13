using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IS3.Core
{

    public static class ServiceImporter
    {

        /// <summary>
        /// import Service dll
        /// </summary>
        public static IService LoadService(string _servicePath)
        {
            List<Assembly> _loadedServer = new List<Assembly>();
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (!Directory.Exists(_servicePath)) return null;

            // try to load *.dll in bin\tools\
            var files = Directory.EnumerateFiles(_servicePath, "*.dll",
                SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                string shortName = Path.GetFileName(file);
                if (allAssemblies.Any(x => x.ManifestModule.Name == shortName))
                    continue;

                // Assembly.LoadFile doesn't resolve dependencies, 
                // so don't use Assembly.LoadFile
                Assembly assembly = Assembly.LoadFrom(file);
                if (assembly != null)
                    _loadedServer.Add(assembly);
            }
            IService _service = null;
            foreach (Assembly assembly in _loadedServer)
            {
                // Summary
                //      load IService
                var types = from type in assembly.GetTypes()
                            where type.GetInterfaces().Contains(typeof(IService))
                            select type;
                foreach (var type in types)
                {
                    object obj = Activator.CreateInstance(type);
                    _service = obj as IService;
                }

                //Summary
                //    load IDataService
                var datatypes = from type in assembly.GetTypes()
                                where type.GetInterfaces().Contains(typeof(IDataService))
                                select type;
                _service.dataServiceDict = new Dictionary<DbServiceType, IDataService>();
                foreach (var type in datatypes)
                {
                    object obj = Activator.CreateInstance(type);
                    IDataService m_dataservice = obj as IDataService;
                    _service.dataServiceDict[m_dataservice.type] = m_dataservice;
                }
                //Summary
                //    load IPrivilegeService
                var Privilegetypes = from type in assembly.GetTypes()
                                     where type.GetInterfaces().Contains(typeof(IPrivilegeService))
                                     select type;
                foreach (var type in Privilegetypes)
                {
                    object obj = Activator.CreateInstance(type);
                    IPrivilegeService m_PrivilegeService = obj as IPrivilegeService;
                    _service.PrivilegeService = m_PrivilegeService;
                }
            }
            return _service;
        }
        /// <summary>
        /// 更新主服务器配置数据
        /// </summary>
        /// <param name="DBconfigFile"></param>
        public static void UpdateMainConnect(string DBconfigFile)
        {
            XDocument xml = XDocument.Load(DBconfigFile);
            //设置主数据库位置
            var ipaddress = xml.Root.Element("ipaddress")?.Value;
            if ((ipaddress.Length > 3) && (ipaddress.Substring(ipaddress.Length - 3, 3).ToUpper() == "MDB"))
            {
                Globals.iS3Service.SetNowDataService(DbServiceType.MDB);
            }
            else
            {
                Globals.iS3Service.SetNowDataService(DbServiceType.SQLSERVER);
            }
            //
            var user = xml.Root.Element("user")?.Value;
            var password = xml.Root.Element("password")?.Value;
            var database = xml.Root.Element("database")?.Value;
            Globals.iS3Service.PrivilegeService.SetDataService(Globals.iS3Service.DataService, ipaddress, user, password,database);
        }


    }


}
