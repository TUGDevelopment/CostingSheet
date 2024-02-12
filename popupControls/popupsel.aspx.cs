using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_popupsel : System.Web.UI.Page
{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    protected void Page_Load(object sender, EventArgs e)
    {
        selectedDataSource = string.Format("{0}", Request.QueryString["view"]);
    }
    protected void listBox_DataBinding(object sender, EventArgs e)
    {
        (sender as ASPxListBox).DataSource = GetDataSource();
    }
    private DataTable GetDataSource()
    {
        object o = selectedDataSource;
        DataTable dt = new DataTable();
        SqlParameter[] param;
        switch (string.Format("{0}", o))
        {
            case "Primary":
                param = new SqlParameter[] {
                new SqlParameter("@usertype", Request.QueryString["usertype"].ToString()) };
                dt = cs.GetRelatedResources("spGetPrimary", param);
                return dt;             
            case "Receiver":
                param = new SqlParameter[] {
                new SqlParameter("@Plant", Request.QueryString["Plant"].ToString()),
                new SqlParameter("@Category", Request.QueryString["Category"].ToString())};
                dt = cs.GetRelatedResources("spGetReceiver", param);
                return dt;
            default:
                string strSQL = "";
                if (string.Format("{0}", o) == "Color")
                    strSQL = "spGetColor";
                else if (string.Format("{0}", o) == "Design")
                    strSQL = "spGetDesign";
                else if (string.Format("{0}", o) == "Material")
                    strSQL = "spGetPackagType";
                else if (string.Format("{0}", o) == "PFLid")
                    strSQL = "spGetPFLid";
                else if (string.Format("{0}", o) == "Lacquer")
                    strSQL = "spGetLacquer";
                else if (string.Format("{0}", o) == "PFType")
                    strSQL = "spGetPFType";
                else if (string.Format("{0}", o) == "Shape")
                    strSQL = "spGetShape";
                param = new SqlParameter[] {
                new SqlParameter("@usertype", Request.QueryString["usertype"].ToString()),
                new SqlParameter("@Primary", Request.QueryString["Primary"].ToString())};
                dt = cs.GetRelatedResources(strSQL, param);
                return dt;
        }
    }
    string selectedDataSource
    {
        get { return Page.Session["selectedDataSource"] == null ? String.Empty : Page.Session["selectedDataSource"].ToString(); }
        set { Page.Session["selectedDataSource"] = value; }
    }
    protected void cp_Callback(object sender, CallbackEventArgsBase e)
    {
        listBox.SelectionMode = ListEditSelectionMode.Single;
        listBox.DataBind();
    }
}