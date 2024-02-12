using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

/// <summary>
/// Summary description for OrderByCollection
/// </summary>
public static class OrderByCollection
{
    public static DataTable ConvertTo<T>(IList<T> list)
    {
        DataTable table = CreateTable<T>();
        Type entityType = typeof(T);
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

        foreach (T item in list)
        {
            DataRow row = table.NewRow();

            foreach (PropertyDescriptor prop in properties)
            {
                row[prop.Name] = prop.GetValue(item);
            }

            table.Rows.Add(row);
        }

        return table;
    }

    public static IList<T> ConvertTo<T>(IList<DataRow> rows)
    {
        IList<T> list = null;

        if (rows != null)
        {
            list = new List<T>();

            foreach (DataRow row in rows)
            {
                T item = CreateItem<T>(row);
                list.Add(item);
            }
        }

        return list;
    }

    public static IList<T> ConvertTo<T>(DataTable table)
    {
        if (table == null)
        {
            return null;
        }

        List<DataRow> rows = new List<DataRow>();

        foreach (DataRow row in table.Rows)
        {
            rows.Add(row);
        }

        return ConvertTo<T>(rows);
    }

    public static T CreateItem<T>(DataRow row)
    {
        T obj = default(T);
        if (row != null)
        {
            obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in row.Table.Columns)
            {
                PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                try
                {
                    object value = row[column.ColumnName];
                    prop.SetValue(obj, value, null);
                }
                catch
                {
                    // You can log something here
                    throw;
                }
            }
        }

        return obj;
    }

    public static DataTable CreateTable<T>()
    {
        Type entityType = typeof(T);
        DataTable table = new DataTable(entityType.Name);
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

        foreach (PropertyDescriptor prop in properties)
        {
            table.Columns.Add(prop.Name, prop.PropertyType);
        }

        return table;
    }

    public static DataTable ListToDataTable<T>(IList<T> thisList)
    {
        DataTable dt = new DataTable();
        if (typeof(T).IsValueType || typeof(T).Equals(typeof(string)))
        {
            DataColumn dc = new DataColumn("CountryList");
            dt.Columns.Add(dc);

            foreach (T item in thisList)
            {
                DataRow dr = dt.NewRow();
                dr[0] = item;
                dt.Rows.Add(dr);
            }
        }
        else
        {
            PropertyInfo[] propertyInfo = typeof(T).GetProperties();
            foreach (PropertyInfo pi in propertyInfo)
            {
                DataColumn dc = new DataColumn(pi.Name, pi.PropertyType);
                dt.Columns.Add(dc);
            }

            for (int item = 0; item < thisList.Count(); item++)
            {
                DataRow dr = dt.NewRow();
                for (int property = 0; property < propertyInfo.Length; property++)
                {
                    dr[property] = propertyInfo[property].GetValue(thisList[item], null);
                }
                dt.Rows.Add(dr);
            }
        }
        dt.AcceptChanges();
        return dt;

    }
    public static DataTable ToDataTable<T>(IEnumerable<T> collection)
    {
        DataTable dt = new DataTable("DataTable");
        Type t = typeof(T);
        PropertyInfo[] pia = t.GetProperties();

        //Inspect the properties and create the columns in the DataTable
        foreach (PropertyInfo pi in pia)
        {
            Type ColumnType = pi.PropertyType;
            if ((ColumnType.IsGenericType))
            {
                ColumnType = ColumnType.GetGenericArguments()[0];
            }
            dt.Columns.Add(pi.Name, ColumnType);
        }

        //Populate the data table
        foreach (T item in collection)
        {
            DataRow dr = dt.NewRow();
            dr.BeginEdit();
            foreach (PropertyInfo pi in pia)
            {
                if (pi.GetValue(item, null) != null)
                {
                    dr[pi.Name] = pi.GetValue(item, null);
                }
            }
            dr.EndEdit();
            dt.Rows.Add(dr);
        }
        return dt;
    }
}