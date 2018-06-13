using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;

using IS3.Core;
namespace IS3.Servers
{
    public enum DbType { Unknown, MDB, XLS, SQLServer };

    public abstract class DbAdapter
    {
        #region Member Variables
        // Database file name
        protected string _dbFile;

        // This is a table name prefix.
        // When you export tables from SQL Server to MDB,
        // an additional 'dbo_' prefix will appear in MDB table names.
        // To deal with this issue, TableNamePrefix be added to each table name.
        // For example, Boreholes will be dbo_Boreholes.
        protected string _tableNamePrefix;

        protected DbType _dbType;

        // Connection string
        protected string _connStr;
        #endregion

        #region Public Properties
        public string TableNamePrefix
        {
            get { return _tableNamePrefix; }
            set { _tableNamePrefix = value; }
        }
        public DbType DatabaseType { get { return _dbType; } }
        public string ConnectionStr { get { return _connStr; } }
        #endregion

        #region Constructor
        public DbAdapter(string dbFile)
        {
            _dbFile = dbFile;
            _dbType = DbType.Unknown;
            _connStr = "Unknown file format";

            string dbTypeStr = DbFileExtension();
            dbTypeStr = dbTypeStr.ToUpper();
            if (dbTypeStr == "MDB")
            {
                _tableNamePrefix = "dbo_";
                _dbType = DbType.MDB;
            }
            else if (dbTypeStr == "XLS")
            {
                _dbType = DbType.XLS;
            }
        }
        #endregion

        public string DbFileExtension()
        {
            int i = _dbFile.LastIndexOf('.');
            int len = _dbFile.Length - i - 1;
            string dbTypeStr = _dbFile.Substring(i + 1, len);
            return dbTypeStr;
        }

        public abstract DbConnection NewConnection();
        public abstract DbDataReader ExcuteCommand(DbConnection conn,
            string strCmd);
        public abstract DbDataAdapter GetDbDataAdapter(DbConnection conn,
            string strCmd);
    }

    public class OdbcAdapter : DbAdapter
    {
        public OdbcAdapter(string _dbFile)
            : base(_dbFile)
        {
            if (_dbType == DbType.MDB)
            {
                _connStr =
                    "DSN=MS Access Database;DBQ=" + _dbFile;
            }
            else if (_dbType == DbType.XLS)
            {
                _connStr =
                    "DSN=Excel Files;DBQ=" + _dbFile;
            }
        }

        public override DbConnection NewConnection()
        {
            return new OdbcConnection(_connStr);
        }

        public override DbDataReader ExcuteCommand(DbConnection conn,
            string strCmd)
        {
            OdbcConnection odbcConn = conn as OdbcConnection;
            OdbcCommand cmd = new OdbcCommand(strCmd, odbcConn);
            return cmd.ExecuteReader();
        }

        public override DbDataAdapter GetDbDataAdapter(DbConnection conn,
            string strCmd)
        {
            return new OdbcDataAdapter(strCmd, conn as OdbcConnection);
        }
    }

    public class OleDbAdapter : DbAdapter
    {
        public OleDbAdapter(string _dbFile)
            : base(_dbFile)
        {
            if (_dbType == DbType.MDB || _dbType == DbType.XLS)
            {
                _connStr =
                    "Provider=Microsoft.Jet.OLEDB.4.0; Data Source="
                    + _dbFile;
            }
        }

        public override DbConnection NewConnection()
        {
            return new OleDbConnection(_connStr);
        }

        public override DbDataReader ExcuteCommand(DbConnection conn,
            string strCmd)
        {
            OleDbConnection oledbConn = conn as OleDbConnection;
            OleDbCommand cmd = new OleDbCommand(strCmd, oledbConn);
            return cmd.ExecuteReader();
        }

        public override DbDataAdapter GetDbDataAdapter(DbConnection conn,
            string strCmd)
        {
            return new OleDbDataAdapter(strCmd, conn as OleDbConnection);
        }
    }

    public class DbContext:Extensions
    {
        protected DbAdapter _adapter;
        protected DbConnection _connection;
        protected bool _isOpened;

        // option:
        //  0 - odbc connection
        //  1 - oledb connection
        public DbContext(string dbFileName, int option = 0)
        {
            if (option == 0)
                _adapter = new OdbcAdapter(dbFileName);
            else
                _adapter = new OleDbAdapter(dbFileName);

            _connection = _adapter.NewConnection();
        }

        ~DbContext()
        {
            Close();
        }

        public bool Open()
        {
            if (_isOpened)
                return true;
            try
            {
                _connection.Open();
            }
            catch (Exception)
            {
                string error = "Open database file failed. Connection string = '"
                    + _adapter.ConnectionStr + "'";
                //ErrorReport.Report(error);
                _isOpened = false;
                return false;
            }

            _isOpened = true;
            return true;
        }

        public bool Close()
        {
            if (!_isOpened)
                return false;

            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
                _isOpened = false;
            }

            return true;
        }

        public DbDataReader ExecuteCommand(string strCmd)
        {
            bool success = Open();
            if (!success)
                return null;

            return _adapter.ExcuteCommand(_connection, strCmd);
        }

        public DbDataAdapter GetDbDataAdapter(string strCmd)
        {
            return _adapter.GetDbDataAdapter(_connection, strCmd);
        }

        public bool IsTableExist(string tableName)
        {
            bool success = Open();
            if (!success)
                return false;

            DataTable dt = _connection.GetSchema("Tables");
            foreach (DataRow row in dt.Rows)
            {
                string currentTable = row["TABLE_NAME"].ToString();
                if (currentTable == tableName)
                    return true;
            }
            return false;
        }

        public string TableNamePrefix
        {
            get { return _adapter.TableNamePrefix; }
        }

        public override string ToString()
        {
            string str = string.Format("DbContext: adapter={0}, isOpened={1}",
                _adapter.GetType().ToString(), _isOpened);
            return str;
        }
    }
}
