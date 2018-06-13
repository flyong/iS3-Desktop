using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IS3.Core;
using System.Data;

namespace IS3.Servers
{
    public class PrivilegeService : IPrivilegeService
    {
        private IDataService main_dataService;
        public void SetDataService(IDataService dataService,string ServerIP,string ServerUser,string ServerPwd,string path)
        {
            Type _type = dataService.GetType();
            main_dataService = Activator.CreateInstance(_type) as IDataService;
            main_dataService.TableNamePrex = "";
            main_dataService.initializeDataService(ServerIP,ServerUser,ServerPwd,path);
        }
        public int CheckIfValidLoginInfo(string userName, string password)
        {
            string tableNameSQL = "CF_User";
            string conditionSQL = "LoginName = '" + userName + "' and LoginPassword = '" + password + "'";
            DataSet ds = main_dataService.Query(tableNameSQL, null, conditionSQL);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return 0;
            }
            else
            {
                return int.Parse(ds.Tables[0].Rows[0]["UserID"].ToString());
            }
        }


        public ProjectList QueryAccessableProject(int UserID)
        {
            ProjectList _list = new ProjectList();
            _list.XMin = 13500000;
            _list.XMax = 13560000;
            _list.YMin = 3640000;
            _list.YMax = 3690000;
            _list.Locations = new System.Collections.ObjectModel.ObservableCollection<ProjectLocation>();
            string tableNameSQL = "CF_Privilege";
            string conditionSQL = "PrivilegeMaster = 'User' and PrivilegeMasterValue = '"
                            + UserID.ToString() + "' and PrivilegeAccess='ProjectListInfo' and PrivilegeOperation='enabled'";
            DataSet ds = main_dataService.Query(tableNameSQL, null, conditionSQL);
            string tableNameSQL2 = "Sys_ProjectListInfo";
            string conditionSQL2 = ""; ;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string accessableProjectID = dr["PrivilegeAccessValue"].ToString();
                if (conditionSQL2 == "")
                {
                    conditionSQL2 += " WHERE ID='" + accessableProjectID + "' ";
                }
                else
                {
                    conditionSQL2 += " or ID='" + accessableProjectID + "' ";
                }
            }

            DataSet ds2 = main_dataService.Query(tableNameSQL2, null, conditionSQL2);
            foreach (DataRow dr in ds2.Tables[0].Rows)
            {
                ProjectLocation _location = new ProjectLocation();
                _location.ID = dr["Name"].ToString();
                _location.X = Convert.ToDouble(dr["X"].ToString());
                _location.Y = Convert.ToDouble(dr["Y"].ToString());
                _location.Description = dr["Description"].ToString();
                _location.DefinitionFile = dr["DefinitionFile"].ToString();
                _location.Default = bool.Parse(dr["DefaultMap"].ToString());
                _list.Locations.Add(_location);
            }
            return _list;
        }



        public bool SetServicePath(string _path)
        {
            return true;
        }

        public void SetDataService(IDataService dataService, string _path)
        {
            throw new NotImplementedException();
        }
    }
}
