using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iS3.Core
{
    public class ReflectionConvertHelper
    {
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            List<T> list = new List<T>();// 定义集合
            Type type = typeof(T);// 获得此模型的类型
            string fieldName = string.Empty;// 定义字段名称
            PropertyInfo[] propertys = type.GetProperties();// 获得此模型的公共属性
            foreach (DataRow dr in dt.Rows)
            {
                //新建一个模型
                object obj = type.Assembly.CreateInstance(type.FullName);
                foreach (PropertyInfo propertyInfo in propertys)
                {
                    fieldName = propertyInfo.Name;
                    if (dt.Columns.Contains(fieldName))
                    {
                        if (!propertyInfo.CanWrite) continue;
                        object value = dr[fieldName];
                        if (value != DBNull.Value)
                            propertyInfo.SetValue(obj, value, null);
                    }
                }
                list.Add((T)obj);
            }
            return list;
        }

        public static List<T> ConvertToList<T>(IDataReader reader)
        {
            List<T> list = new List<T>(); // 定义集合
            Type type = typeof(T); // 获得此模型的类型
            string fieldName = string.Empty; // 定义字段名称
            PropertyInfo[] propertys = type.GetProperties();// 获得此模型的公共属性
            while (reader.Read())
            {
                //新建一个模型
                object obj = type.Assembly.CreateInstance(type.FullName);
                foreach (PropertyInfo propertyInfo in propertys)
                {
                    fieldName = propertyInfo.Name;
                    if (ReaderExists(reader, fieldName))
                    {
                        if (!propertyInfo.CanWrite) continue;
                        object value = reader[fieldName];
                        if (value != DBNull.Value)
                            propertyInfo.SetValue(obj, value, null);
                    }
                }
                list.Add((T)obj);
            }
            return list;
        }

        public static T ConvertToModel<T>(IDataReader reader)
        {
            Type type = typeof(T);
            PropertyInfo[] proList = type.GetProperties();
            //新建一个模型
            object obj = type.Assembly.CreateInstance(type.FullName);
            string fieldName = string.Empty; // 定义字段名称
            if (reader.Read())
            {
                foreach (PropertyInfo propertyInfo in proList)
                {
                    fieldName = propertyInfo.Name;
                    if (ReaderExists(reader, propertyInfo.Name))
                    {
                        if (!propertyInfo.CanWrite) continue;
                        object value = reader[fieldName];
                        if (value != DBNull.Value)
                            propertyInfo.SetValue(obj, value, null);
                    }
                }
            }
            return (T)obj;
        }

        public static T ConvertToModel<T>(DataRow row)
        {
            Type type = typeof(T);
            PropertyInfo[] proList = type.GetProperties();
            //新建一个模型
            object obj = type.Assembly.CreateInstance(type.FullName);
            string fieldName = string.Empty; // 定义字段名称
            foreach (PropertyInfo propertyInfo in proList)
            {
                fieldName = propertyInfo.Name;
                if (row.Table.Columns.Contains(fieldName))
                {
                    if (!propertyInfo.CanWrite) continue;
                    object value = row[fieldName];
                    if (value != DBNull.Value)
                        propertyInfo.SetValue(obj, value, null);
                }
            }
            return (T)obj;
        }

        /// <summary>
        /// 验证reader是否存在某列
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private static bool ReaderExists(IDataReader reader, string columnName)
        {
            int count = reader.FieldCount;
            for (int i = 0; i < count; i++)
            {
                if (reader.GetName(i).Equals(columnName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
