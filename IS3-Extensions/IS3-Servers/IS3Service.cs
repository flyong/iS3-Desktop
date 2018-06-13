using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using IS3.Core;
using System.Data.OleDb;
using System.Reflection;

namespace IS3.Servers
{
    public class IS3Service : IService
    {
        public IPrivilegeService PrivilegeService { get; set; }
       
        public IFileService FileService { get; set; }
        public IMapService MapService { get; set; }
        #region DataService Managerment
        public IDataService DataService { get; set; }
        public List<IDataService> dataServices { get; set ; }
        public Dictionary<DbServiceType, IDataService> dataServiceDict { get; set; }

        public bool IsDataServiceConnect(DbServiceType dbServiceType)
        {
            if ((dataServiceDict != null) && (dataServiceDict.ContainsKey(dbServiceType)) && (dataServiceDict[dbServiceType].CanConnect()))
            {
                return true;
            }
            return false;
        }
        public void SetNowDataService(DbServiceType dbServiceType)
        {
            if (dataServiceDict != null)
            {
                DataService = dataServiceDict[dbServiceType];
            }
        }
        #endregion
    }
   
}

