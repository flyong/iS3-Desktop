using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using IS3.Core;
namespace IS3.Servers
{
    public class DataServiceWebService : IDataService
    {
        public string ServerIP { get; set; }
        public string ServerUser { get; set; }
        public string ServerPWD { get; set; }
        public DbServiceType type { get { return DbServiceType.WebService; } }

        public string _constrPrefix { get; set; }

        private string _tableNamePrex = "";
        public string TableNamePrex { get { return _tableNamePrex; } set { _tableNamePrex = value; } }

        public void initializeDataService(string ServerIP, string ServerUser, string ServerPwd, string dbFilePath)
        {
            throw new NotImplementedException();
        }

        public bool CanConnect()
        {
            return false;
        }


        public bool CreateNewDbFile(string path)
        {
            return false;
        }

        public bool CreateNewDbTable()
        {
            return false;
        }

        public DataSet ExecuteDatable(string connectionStr, string sql)
        {
            return null;
        }

        public void initializeAdapter()
        {

        }

        public void initializeDataService(string dbFilePath)   
        {
            
        }

        public bool InsertRecord()
        {
            return false;
        }

        public void onClose()
        {

        }

        public DataSet QueryRecord()
        {
            return null;
        }

        public DataSet Query(string tableNameSQL, string orderSQL, string conditionSQL)
        {
            return null;
        }
        public DataSet Query(string tableNameSQL, string orderSQL, string conditionSQL,int Times)
        {
            return null;
        }
        public void initializeDataService(string dbFilePath, string id, string name)
        {

        }

        public DataSet QueryByField(string tableNameSQL, string fieldName, List<int> records)
        {
            throw new NotImplementedException();
        }
    }
}
