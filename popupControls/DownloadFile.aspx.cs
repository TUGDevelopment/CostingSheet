using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class popupControls_downloadfile : System.Web.UI.Page
{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        getdownload(string.Format("{0}", Request.QueryString["Id"]));
    }
     void getdownload(string Id)
    {
        DataTable dt = cs.builditems("SELECT * FROM transstdFileDetails where ID=" + Id);
        foreach (DataRow dr in dt.Rows)
        {
            //string contentType = MimeTypes.GetContentType(dr["Name"].ToString());
            //Response.Clear();
            //Response.Buffer = true;
            //Response.Charset = "";
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.ContentType = contentType;
            //Response.AppendHeader("Content-Disposition", "attachment; filename=" + dr["Name"]);
            //Response.BinaryWrite((byte[])dr["Attached"]);
            //Response.Flush();
            //Response.End();
            char[] charsToTrim = { '*', ' ', '\'', ',' };
            string result = dr["Name"].ToString().Trim(charsToTrim);
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = MimeTypes.GetContentType(dr["Name"].ToString()); ;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + result.Replace(",", "_"));
            Response.BinaryWrite((byte[])dr["Attached"]);
            Response.Flush();
            Response.End();
        }
    }
}