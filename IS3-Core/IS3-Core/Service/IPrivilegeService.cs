using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS3.Core
{
    public interface IPrivilegeService
    {
        void SetDataService(IDataService dataService, string ServerIP, string ServerUser, string ServerPwd,
            string path);
        bool SetServicePath(string _path);
        int CheckIfValidLoginInfo(string userName, string password);
        ProjectList QueryAccessableProject(int UserID);
    }
}
