using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace IS3.Core
{
    /// <summary>
    /// 
    /// </summary>
    public enum DbServiceType { MDB, SQLSERVER, MYSQL, WebService };

    /// <summary>
    /// interface for DataService Conncet such as mdb,sqlServer,mysql,webservice
    /// help you get the data from your own source
    /// </summary>
    public interface IDataService
    {
        #region Property

        // Summary:
        //     ConnectionStr for this serivice
        string _constrPrefix { get; }
        string TableNamePrex { get; set; }
        string ServerIP { get; set; }
        string ServerUser { get; set; }
        string ServerPWD { get; set; }
        // Summary:
        //     define this dataService type { MDB, SQLSERVER, MYSQL, WebService }
        DbServiceType type { get; }
        #endregion

        #region basic work
        // Summary:
        //     initail DataService
        void initializeDataService( string dbFilePath);
        void initializeDataService(string dbFilePath,string id,string name);
        void initializeDataService(string ServerIP, string ServerUser, string ServerPwd,string dbFilePath);

        // Summary:
        //     check if this dataService can connect
        bool CanConnect();

        // Summary:
        //     Close the DataService Conncet
        void onClose();
        #endregion

        #region Operation

        /// <summary>
        /// query the dataset from your dataservice
        /// </summary>
        /// <param name="tableNameSQL">for example: "Mon_Segment,Mon_SegmentData" </param>
        /// <param name="orderSQL">for example: "[ID],[time]" </param>
        /// <param name="conditionSQL">for example: ",@Last(1000)"</param>
        /// <param name="Times"> the waiting time for querying</param>
        /// <returns></returns>
        DataSet Query(string tableNameSQL, string orderSQL, string conditionSQL, int Times);
        DataSet Query(string tableNameSQL, string orderSQL, string conditionSQL);
        DataSet QueryByField(string tableNameSQL, string fieldName, List<int> records);
        bool CreateNewDbFile( string path );
        bool CreateNewDbTable();
        DataSet QueryRecord();

        bool InsertRecord();
        #endregion
    }
}
