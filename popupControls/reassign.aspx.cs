using DevExpress.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_reassign : System.Web.UI.Page
{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    private DataTable result
    {
        get { return Page.Session["result"] == null ? null : (DataTable)Page.Session["result"]; }
        set { Page.Session["result"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Clear();
        var Id = Request.QueryString["Id"].ToString();
        var table = Request.QueryString["table"].ToString();
        string strSQL = "select usertype,";
        strSQL += table == "TransTechnical" ? "Assignee," : "'' as Assignee,";
        strSQL += string.Format(@"company,CONVERT(nvarchar(max),UniqueColumn)'NewID' from " + table + " where Id={0}", Id);
        result = cs.builditems(strSQL);
        foreach (DataRow r in result.Rows)
        {
            Page.Session["usertype"] = string.Format("{0}", r["usertype"]);
            Page.Session["BU"] = string.Format("{0}", r["company"]);
            Page.Session["NewID"] = r["NewID"];
            Page.Session["username"] = user_name;
            Page.Session["tablename"] = table;
        }
    }
    protected void cp_Callback(object sender, CallbackEventArgsBase e)
    {
        
        Int32 count = lbChoosen.Items.Count;
        List<String> listTubes = new List<String>();
        ArrayList arrTubes = new ArrayList();
        Int32 p = Convert.ToInt32(lbChoosen.Rows);
        if (lbChoosen.Items.Count >= 1)
        {
            for (int i = 0; i < lbChoosen.Items.Count; i++)
            {
                arrTubes.Add(lbChoosen.Items[i].Value);
                listTubes.Add(lbChoosen.Items[i].Text);
            }
        }
        else
        {

        }
        var Id = Request.QueryString["Id"].ToString();
        string Assign = String.Join(",", arrTubes.ToArray());
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spUpdateAssinee";
            cmd.Parameters.AddWithValue("@ID", Id.ToString());
            cmd.Parameters.AddWithValue("@Assign", Assign.ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}