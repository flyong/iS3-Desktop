using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IS3.Core;
using IS3.Servers.DataServiceHelp;


namespace IS3.Servers
{
    class DataServiceSqlServer:IDataService
    {

        #region inteface

        public DbServiceType type => DbServiceType.SQLSERVER;

        public string _constrPrefix { get; }
        private string _tableNamePrex = "";
        public string TableNamePrex { get { return _tableNamePrex; } set { _tableNamePrex = value; } }

        private string serverIP;

        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }

        public string ServerUser { get; set; }

        public string ServerPWD { get; set; }

        public bool CanConnect()
        {
            return true;
        }
        public void initializeDataService(string dbFilePath)
        {
            SqlServerHelp.SetConncetionString(dbFilePath);
        }
        public void initializeDataService(string dbFilePath, string id, string name)
        {

        }
        public void initializeDataService(string IP, string ServerUser, string ServerPwd, string dbFilePath)
        {
            SqlServerHelp.SetConncetionString(IP,dbFilePath, ServerUser, ServerPwd);
            ServerIP = IP;
            this.ServerUser = ServerUser;
            this.ServerPWD = ServerPwd;
        }

        public bool CreateNewDbFile(string path)
        {
            return true;
        }

        public bool CreateNewDbTable()
        {
            return true;
        }

        public bool InsertRecord()
        {
            return true;
        }

        public void onClose()
        {

        }

        public DataSet QueryRecord()
        {
            return new DataSet();
        }


        public DataSet Query(string tableNameSQL, string orderSQL, string conditionSQL, int Times)
        {
            return new DataSet();
        }

        public DataSet Query(string tableNameSQL, string orderSQL, string conditionSQL)
        {
            DataSet ds = new DataSet();
            string[] names = tableNameSQL.Split(_separator);
            string[] orders = null;
            string[] conditions = null;
            if (orderSQL != null)
                orders = orderSQL.Split(_separator);
            if (conditionSQL != null)
                conditions = conditionSQL.Split(_separator);
            for (int i = 0; i < names.Count(); ++i)
            {
                string tableName = TableNamePrex + names[i];
                string strCmd = "";

                if (conditions != null && i < conditions.Count())
                {
                    string cond = conditions[i];
                    strCmd = getSelectCmd(tableName, cond);
                }
                else
                    strCmd = "SELECT * FROM " + tableName + "";
                if (orders != null && i < orders.Count())
                {
                    strCmd += OrderSQL(orders[i]);
                }
                DataSet _ds = SqlServerHelp.Query(strCmd);
                if (_ds.Tables.Count > 0)
                {
                    DataTable dt = _ds.Tables[0];
                    _ds.Tables.RemoveAt(0);
                    dt.TableName = tableName;
                    ds.Tables.Add(dt);
                }
            }
            return ds;
        }
        #endregion
        #region function
        public char[] _separator = new char[] { ',' };

        #region function turn parameter to SQL 
        // Split '\n' seperated names into string array
        public string[] SplitNames(string names)
        {
            if (names == "*")
                return null;
            else
                return names.Split(new char[] { '\n', '\t', ',' });
        }

        // Convert names to SQL format, such as:
        //      ([Name]='1' OR [Name]='2')
        public string NamesToSQL(string[] names)
        {
            bool first = true;
            string sql = "(";
            foreach (string name in names)
            {
                if (first == false)
                    sql += " OR ";
                sql += "[Name]='" + name + "'";
                first = false;
            }
            sql += ")";
            return sql;
        }

        // Convert IDs to SQL, such as
        //      WHERE ID in (1,2)
        public string WhereSQL(List<int> IDs)
        {
            string sql = " WHERE ID in (";
            for (int i = 0; i < IDs.Count; ++i)
            {
                sql += IDs[i].ToString();
                if (i != IDs.Count - 1)
                    sql += ",";
            }
            sql += ")";
            return sql;
        }

        // Convert names and contitionSQL to SQL WHERE clause, such as
        //      WHERE [LineNo]=1 AND ([Name]='1' OR [Name]='2')
        public string WhereSQL(string[] names, string conditionSQL)
        {
            string where = "";
            bool bWhereAdded = false;
            if (conditionSQL != null && conditionSQL.Length > 0)
            {
                if (conditionSQL.Contains("WHERE") == false)
                    where += " WHERE ";
                where += conditionSQL;
                bWhereAdded = true;
            }

            bool IsNameEmpty = false;
            if (names == null)
                IsNameEmpty = true;
            else if (names.Length == 1 && names[0].Length == 0)
                IsNameEmpty = true;

            if (!IsNameEmpty)
            {
                if (bWhereAdded == false)
                    where += " WHERE ";
                else
                    where += " AND ";
                where += NamesToSQL(names);
            }

            return where;
        }

        public string OrderSQL(string orderSQL)
        {
            if (orderSQL != null && orderSQL.Length > 0)
                return " ORDER BY " + orderSQL;
            else
                return "";
        }

        // Get record count of a table
        int getRecordCount(string tableNameSQL)
        {
            return SqlServerHelp.GetRecordCount(tableNameSQL);
        }

        public string getSelectCmd(string tableNameSQL, string condition)
        {
            string cmd = "";
            int index = condition.IndexOf("@UniformSampling");
            if (index >= 0)
            {
                // SQL: select * from [table] where [id] mod N = 0
                int begin = condition.IndexOf("(", index);
                int end = condition.IndexOf(")", begin);
                string sampleStr = condition.Substring(begin + 1, end - begin - 1);
                int sample = int.Parse(sampleStr);

                int count = getRecordCount(tableNameSQL);
                int interval = count / sample;
                if (interval == 0)
                    interval = 1;

                cmd = "SELECT * FROM [" + tableNameSQL + "] WHERE [ID] MOD " + interval.ToString() + " = 0";
                return cmd;
            }

            index = condition.IndexOf("@Last");
            if (index >= 0)
            {
                // SQL: select * from (select top N * from [table] order by [time] desc)
                int begin = condition.IndexOf("(", index);
                int end = condition.IndexOf(")", begin);
                string sampleStr = condition.Substring(begin + 1, end - begin - 1);
                int sample = int.Parse(sampleStr);

                cmd = "SELECT * FROM (SELECT TOP " + sample.ToString() + " * FROM " + tableNameSQL + " ORDER BY [TIME] DESC)";
                return cmd;
            }

            cmd = "SELECT * FROM " + tableNameSQL + "";
            cmd += WhereSQL(null, condition);
            return cmd;
        }

        public DataSet QueryByField(string tableNameSQL, string fieldName, List<int> records)
        {
            throw new NotImplementedException();
        }



        #endregion

        #endregion

    }
}
