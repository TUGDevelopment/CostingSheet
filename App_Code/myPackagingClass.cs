using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for myClass
/// </summary>
public static class myPackagingClass
{
    //HttpContext Context = HttpContext.Current;
    public static string strConn = ConfigurationManager.ConnectionStrings["PKGConnectionString"].ConnectionString;
    public static string CurUserName = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    public static DataTable GetRelatedResources(string StoredProcedure, object[] Parameters)
    {
        var Results = new DataTable();
        using (SqlConnection conn = new SqlConnection(strConn))
        {
            using (SqlCommand cmd = new SqlCommand(StoredProcedure, conn))
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(Parameters);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Results);
                conn.Close();
                conn.Dispose();
            }
        }
        return Results;
    }
    public static DataTable builditems(string data)
    {
        using (SqlConnection oConn = new SqlConnection(strConn))
        {
            oConn.Open();
            string strQuery = data;
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, oConn);
            // Fill the dataset.
            oAdapter.Fill(dt);
            oConn.Close();
            oConn.Dispose();
            return dt;
        }
    }
    public static string ReadItems(string strQuery)
    {
        string result = "";
        // (ByVal FieldName As String, ByVal TableName As String, ByVal Cur As String, ByVal Value As String) As String
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strConn);
        SqlDataAdapter sda = new SqlDataAdapter();
        SqlCommand cmd = new SqlCommand(strQuery);
        cmd.CommandType = CommandType.Text;
        cmd.Connection = con;
        con.Open();
        sda.SelectCommand = cmd;
        sda.Fill(dt);
        con.Close();
        con.Dispose();
        StringBuilder sb = new StringBuilder();
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
            {
                sb.Append(row[0] + ",");
            }
            if (result.Length < 2)
            {
                result = sb.ToString();
                result = result.Substring(0, (result.Length - 1));
            }
        }
        return result;
    }
}