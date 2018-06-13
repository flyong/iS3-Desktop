using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Data.Common;

using IS3.Core;
using IS3.Core.Shape;

namespace IS3.Core.Serialization
{
    public class DbDataLoader
    {
        #region Member Variables
        // Default table name for attached documents.
        string _defDocTable = "Document";
        //protected DbContext _dbContext;
        ShapeObject nullShape;
        #endregion

        #region Constructor
        //public DbDataLoader(DbContext dbContext)
        //{
        //    _dbContext = dbContext;
        //    nullShape.Type = GeometryType.Null;
        //    nullShape.Data = null;
        //}
        public DbDataLoader()
        {
            //_dbContext = dbContext;
            nullShape.Type = GeometryType.Null;
            nullShape.Data = null;
        }
        #endregion

        #region Read Helper Functions
        public double? ReadDouble(DbDataReader reader, int index)
        {
            double d = double.NaN;

            if (reader.IsDBNull(index))
                return 0;

            Object obj = reader.GetValue(index);
            if (obj.GetType() == typeof(Decimal))
            {
                d = Decimal.ToDouble((Decimal)obj);
            }
            else if (obj.GetType() == typeof(double))
            {
                d = (double)obj;
            }
            else if (obj.GetType() == typeof(float))
            {
                d = (float)obj;
            }
            else if (obj.GetType() == typeof(string))
            {
                d = double.Parse((string)obj);
            }
            else
                return null;
            return d;
        }
        public double? ReadDouble(DbDataReader reader, string name)
        {
            int ordina = reader.GetOrdinal(name);
            return ReadDouble(reader, ordina);
        }

        public int? ReadInt(DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return 0;

            int i;
            Object obj = reader.GetValue(index);


            if (obj.GetType() == typeof(int) ||
                obj.GetType() == typeof(Int16) ||
                obj.GetType() == typeof(Int32) ||
                obj.GetType() == typeof(Int64))
                i = Convert.ToInt32(obj);
            else if (obj.GetType() == typeof(string))
                i = int.Parse((string)obj);
            else
                return null;

            return i;
        }
        public int? ReadInt(DbDataReader reader, string name)
        {
            int ordina = reader.GetOrdinal(name);
            return ReadInt(reader, ordina);
        }

        public DateTime? ReadDateTime(DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            Object obj = reader.GetValue(index);
            if (obj.GetType() == typeof(DateTime))
                return (DateTime)obj;
            else
                return Convert.ToDateTime(obj);
        }
        public DateTime? ReadDateTime(DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return ReadDateTime(reader, ordinal);
        }

        public TimeSpan? ReadTimeSpan(DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            Object obj = reader.GetValue(index);
            if (obj.GetType() == typeof(TimeSpan))
                return (TimeSpan)obj;
            else
                return null;
        }
        public TimeSpan? ReadTimeSpan(DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return ReadTimeSpan(reader, ordinal);
        }

        public string ReadString(DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;

            string str;
            Object obj = reader.GetValue(index);
            if (obj.GetType() == typeof(string))
                str = (string)obj;
            else
                return obj.ToString();

            return str;
        }
        public string ReadString(DbDataReader reader, string name)
        {
            int ordina = reader.GetOrdinal(name);
            return ReadString(reader, ordina);
        }

        public bool IsDbColumnExist(DataRow row, string colName)
        {
            if (!row.Table.Columns.Contains(colName))
            {
                string error = string.Format(
                    "[{0}] does not exist in table [{1}]!!!!",
                    colName, row.Table.TableName);
                ErrorReport.Report(error);
                return false;
            }
            return true;
        }
        public bool IsDbNull(DataRow row, string colName)
        {
            if (row[colName].GetType() == typeof(System.DBNull))
                return true;
            else
                return false;
        }
        public double? ReadDouble(DataRow row, string colName)
        {
            if (!IsDbColumnExist(row, colName))
                return null;
            else if (IsDbNull(row, colName))
                return null;
            else
                if (row[colName].ToString() == "")
                return null;
            else
                return Convert.ToDouble(row[colName]);
        }

        public int? ReadInt(DataRow row, string colName)
        {
            if (!IsDbColumnExist(row, colName))
                return null;
            else if (IsDbNull(row, colName))
                return null;
            else
            if (row[colName].ToString() == "")
                return null;
            else
                return Convert.ToInt32(row[colName]);
        }

        public DateTime? ReadDateTime(DataRow row, string colName)
        {
            if (!IsDbColumnExist(row, colName))
                return null;
            else if (IsDbNull(row, colName))
                return null;
            else
            if (row[colName].ToString() == "")
                return null;
            else
            {
                return Convert.ToDateTime(row[colName]);
            }
        }

        public TimeSpan? ReadTimeSpan(DataRow row, string colName)
        {
            if (!IsDbColumnExist(row, colName))
                return null;
            else if (IsDbNull(row, colName))
                return null;
            else
            if (row[colName].ToString() == "")
                return null;
            else
            {
                object obj = row[colName];
                if (obj.GetType() == typeof(TimeSpan))
                    return (TimeSpan)obj;
                else
                    return null;
            }
        }
        
        public string ReadString(DataRow row, string colName)
        {
            if (!IsDbColumnExist(row, colName))
                return null;
            else if (IsDbNull(row, colName))
                return null;
            else
                return Convert.ToString(row[colName]);
        }

        public byte[] ReadBytes(DataRow row, string colName)
        {
            if (!IsDbColumnExist(row, colName))
                return null;
            else if (IsDbNull(row, colName))
                return null;
            else
                return (byte[])(row[colName]);
        }
        #endregion

        public virtual bool ReadRawData(
            DGObjects objs,
            string tableNameSQL,
            string orderSQL,
            string conditionSQL)
        {
            //确认所在数据库
            string databaseName = objs.definition.DatabaseName;
            if (databaseName.Substring(databaseName.Length - 3, 3).ToUpper() == "MDB")
            {
                Globals.iS3Service.SetNowDataService(DbServiceType.MDB);
                Globals.iS3Service.DataService.initializeDataService(databaseName);
            }
            else
            {
                Globals.iS3Service.SetNowDataService(DbServiceType.SQLSERVER);
                Globals.iS3Service.DataService.initializeDataService(databaseName);
            }
            objs.rawDataSet = Globals.iS3Service.DataService.Query(tableNameSQL,orderSQL, conditionSQL);
            // Add a field 'IsSelected' to the first DataTable, so we can 
            // set and track the selection state of each row
            if (objs.rawDataSet.Tables != null
                && objs.rawDataSet.Tables.Count > 0)
            {
                DataTable dt = objs.rawDataSet.Tables[0];
                DataColumn column = null;
                if (dt.Columns.Contains("IsSelected"))
                    column = dt.Columns["IsSelected"];
                else
                    column = dt.Columns.Add("IsSelected", typeof(bool));
                foreach (DataRow row in dt.Rows)
                {
                    row.SetField(column, false);
                }
            }

            return true;
        }

        public virtual ShapeObject ReadShape(DataRow row)
        {
            DataTable table = row.Table;
            if (!table.Columns.Contains("Shape"))
                return nullShape;
            else
                return ShapeBuilder.BuildObject(ReadBytes(row, "Shape"));
        }

        // Read object (i.e., Name list) from the specified table.
        // REQUIREMENT:
        //      1. The specified table must have a 'Name' column.
        //
        public virtual bool ReadDGObjects(DGObjects objs, string tableNameSQL,
            string defNamesSQL, string orderSQL, string conditionSQL)
        {
            ReadRawData(objs, tableNameSQL, orderSQL, conditionSQL);

            DataTable table = objs.rawDataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                DGObject dgObj = new DGObject(row);

                try
                {
                    if (table.Columns.Contains("ID"))
                        dgObj.id = Convert.ToInt32(row["ID"]);

                    if (table.Columns.Contains("Name"))
                        dgObj.name = Convert.ToString(row["Name"]);

                    if (table.Columns.Contains("FullName"))
                        dgObj.fullName = ReadString(row, "FullName");

                    if (table.Columns.Contains("Description"))
                        dgObj.description = ReadString(row, "Description");
                    
                    dgObj.shape = ReadShape(row);
                }
                catch (FormatException ex)
                {
                    string str = ex.ToString();
                    ErrorReport.Report(str);
                    continue;
                }
                objs[dgObj.id.ToString() + ":" + dgObj.name] = dgObj;
            }

            return true;
        }

        public virtual bool ReadDGObjects(DGObjects objs, string tableNameSQL,
            List<int> IDs)
        {
            string condition = WhereSQL(IDs);
            return ReadDGObjects(objs, tableNameSQL, null, null, condition);
        }

        // Read attachments for the entities.
        // REQUIREMENT:
        //      1. The specified table 'Documents' must exists.
        //      2. 'DocName', 'Url', 'Name' and 'Category' columns
        //         must exists in 'Documents' table.
        //
        //public virtual List<Attachment> ReadAttachments(string category,
        //    string name)
        //{
        //    string docTable = _dbContext.TableNamePrefix + _defDocTable;
        //    List<Attachment> attachments = new List<Attachment>();
        //    string strCmd =
        //        "SELECT [DocName],[Url] FROM [" +
        //        docTable +
        //        "] WHERE [Category]='"
        //        + category + "' AND [Name]='" + name + "'";

        //    try
        //    {
        //        if (_dbContext.IsTableExist("Documents"))
        //        {
        //            DbDataReader reader =
        //                _dbContext.ExecuteCommand(strCmd);
        //            while (reader.Read())
        //            {
        //                Attachment attach = new Attachment();
        //                attach.Name = ReadString(reader, 0);
        //                attach.Url = ReadString(reader, 1);
        //                attachments.Add(attach);
        //            }
        //            reader.Close();
        //        }
        //    }
        //    catch (DbException ex)
        //    {
        //        string str = ex.ToString();
        //        ErrorReport.Report(str);
        //    }

        //    return attachments;
        //}
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
    }
}