using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

/// <summary>
/// Summary description for MyToDataTable
/// </summary>
public static class MyToDataTable
{
    public static DataTable ToDataTable<T>(this IList<T> data)
    {
        PropertyDescriptorCollection properties =
            TypeDescriptor.GetProperties(typeof(T));
        DataTable table = new DataTable();
        foreach (PropertyDescriptor prop in properties)
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        foreach (T item in data)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            table.Rows.Add(row);
        }
        return table;
    }
    public static void ToCSV(DataTable dtDataTable, string strFilePath)
    {
        StreamWriter sw = new StreamWriter(strFilePath, false);
        //headers
        for (int i = 0; i < dtDataTable.Columns.Count; i++)
        {
            sw.Write(dtDataTable.Columns[i]);
            if (i < dtDataTable.Columns.Count - 1)
            {
                sw.Write(",");
            }
        }
        sw.Write(sw.NewLine);
        foreach (DataRow dr in dtDataTable.Rows)
        {
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                if (!Convert.IsDBNull(dr[i]))
                {
                    string value = dr[i].ToString();
                    if (value.Contains(','))
                    {
                        value = String.Format("\"{0}\"", value);
                        sw.Write(value);
                    }
                    else
                    {
                        sw.Write(dr[i].ToString());
                    }
                }
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
        }
        sw.Close();
    }
    public static String getColumnNameFromIndex(int column)
    {
        column--;
        String col = Convert.ToString((char)('A' + (column % 26)));
        while (column >= 26)
        {
            column = (column / 26) - 1;
            col = Convert.ToString((char)('A' + (column % 26))) + col;
        }
        return col;
    }
    public static char NumberToChar(int number)
    {
        int offset = (int)'A' - 1;
        return (char)(number + offset);
    }
    public static int CharToNumber(char character)
    {
        return character - 'A' + 1; // " + 1" : You want to start from 1
    }
    public static string CharacterIncrement(int colCount)
    {
        int TempCount = 0;
        string returnCharCount = string.Empty;

        if (colCount <= 25)
        {
            TempCount = colCount;
            char CharCount = Convert.ToChar((Convert.ToInt32('A') + TempCount));
            returnCharCount += CharCount;
            return returnCharCount;
        }
        else
        {
            var rev = 0;

            while (colCount >= 26)
            {
                colCount = colCount - 26;
                rev++;
            }

            returnCharCount += CharacterIncrement(rev - 1);
            returnCharCount += CharacterIncrement(colCount);
            return returnCharCount;
        }
    }

 
public static string Increment(string s)
    {
        var chars = s.ToCharArray();
        int carry = 1;
        var i = s.Length - 1;
        while (i >= 0)
        {
            chars[i] += (char)carry;
            if (chars[i] <= 'z')
            {
                carry = 0;
                break;
            }
            chars[i] = 'a';
            carry = 1;
            i--;
        }
        var res = new string(chars);
        return (i == -1 && carry != 0) ? "a" + res : res;
    }
    public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
    {
        try
        {
            List<T> list = new List<T>();

            foreach (var row in table.AsEnumerable())
            {
                T obj = new T();

                foreach (var prop in obj.GetType().GetProperties())
                {
                    try
                    {
                        PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                        propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                    }
                    catch
                    {
                        continue;
                    }
                }

                list.Add(obj);
            }

            return list;
        }
        catch
        {
            return null;
        }
    }
}