using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace IS3.Core
{
    public interface IService
    {
        IPrivilegeService PrivilegeService { get; set; }

        #region DataService Management 
        Dictionary<DbServiceType,IDataService> dataServiceDict { get; set; }
        IDataService DataService { get; set; }
        bool IsDataServiceConnect(DbServiceType dbServiceType);
        void SetNowDataService(DbServiceType dbServiceType);
        #endregion
        IFileService FileService { get; set; }
        IMapService MapService { get; set; }
    }
}
