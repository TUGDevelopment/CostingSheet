using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class popupControls_selmaster : System.Web.UI.Page
{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            selectedDataSource = string.Format("{0}", Request.QueryString["view"]);
            gv.DataBind();
        }
    }
    private DataTable GetDataSource()
    {
        //DataTable table = new DataTable();
        //table.Clear();
        //table.Columns.Add("Name");
        //table.Columns.Add("Marks");
        object o = selectedDataSource;
        DataTable dt = new DataTable();
        SqlParameter[] param;
        switch (string.Format("{0}", o))
        {
            case "RawMaterial":
                param = new SqlParameter[] {
                new SqlParameter("@RawMaterial", Request.QueryString["RawMaterial"].ToString()),
                                    new SqlParameter("@Company", Request.QueryString["Company"].ToString())};
                dt = cs.GetRelatedResources("spGetMaterial", param);
                //table.Rows.Add(new object[] { @"ID;Material;Name;Yield", 0 });
                break;
            case "PackageCode":case "SecPackageCode":
                param = new SqlParameter[] {
                new SqlParameter("@Id", Request.QueryString["Id"].ToString()),
                new SqlParameter("@from", Request.QueryString["from"].ToString()),
                new SqlParameter("@to", Request.QueryString["to"].ToString()),
                new SqlParameter("@type", Request.QueryString["type"].ToString()),
                new SqlParameter("@Company", Request.QueryString["Company"].ToString())};
                dt = cs.GetRelatedResources("spGetPackage", param);
                break;
            case "CostingSheet":
                param = new SqlParameter[] {
                new SqlParameter("@t", Request.QueryString["type"].ToString()),
                new SqlParameter("@Company", Request.QueryString["Company"].ToString())};
                dt = cs.GetRelatedResources("spGetCostFormula", param);
                break;
            case "SAPMaterial":
                param = new SqlParameter[] {
                new SqlParameter("@Company", Request.QueryString["Company"].ToString()),
                new SqlParameter("@from", Request.QueryString["from"].ToString()),
                new SqlParameter("@to", Request.QueryString["to"].ToString()),
                new SqlParameter("@RequestNo", Request.QueryString["ID"].ToString())};
                dt = cs.GetRelatedResources("spGetRawMaterial", param);
                break;
            case "Labor":
                param = new SqlParameter[] {
                new SqlParameter("@Company", Request.QueryString["Company"].ToString()),
                new SqlParameter("@RequestNo", Request.QueryString["ID"].ToString())};
                dt = cs.GetRelatedResources("spGetLabor", param);
                break;
            case "LBOh":
                param = new SqlParameter[] {
                new SqlParameter("@Company", Request.QueryString["Company"].ToString()),
                new SqlParameter("@NetWeight", Request.QueryString["NetWeight"].ToString()),
                new SqlParameter("@Packaging", Request.QueryString["Packaging"].ToString()),
                new SqlParameter("@UserType", Request.QueryString["UserType"].ToString()),
                new SqlParameter("@PackSize", Request.QueryString["PackSize"].ToString())};
                dt = cs.GetRelatedResources("spGetLaborOverhead", param);
                break;
            case "Margin":
                param = new SqlParameter[] {
                new SqlParameter("@Company", Request.QueryString["Company"].ToString()),
                new SqlParameter("@ID", Request.QueryString["ID"].ToString()),
                new SqlParameter("@NetWeight", Request.QueryString["NetWeight"].ToString()),
                new SqlParameter("@Packaging", Request.QueryString["Packaging"].ToString()),
                new SqlParameter("@UserType", Request.QueryString["UserType"].ToString())};
                dt = cs.GetRelatedResources("spGetMargin", param);
                break;
            case "RequestNo":
                param = new SqlParameter[] {
                new SqlParameter("@Company", Request.QueryString["Company"].ToString()),
                new SqlParameter("@User", Request.QueryString["User"].ToString())};
                dt = cs.GetRelatedResources("spGetRequestNo", param);
                //table.Rows.Add(new object[] { @"ID;RequestNo;RequestDate;RequireDate;PackSize;Packaging;NetWeight;Customer;Product", 0 });
                break;
        }
        //gv.Columns.Clear();
        //foreach (DataRow data in table.Rows)
        //{
        //    string[] args = data["Name"].ToString().Split(';');
        //    for (int x = 0; x < args.Length; x++)
        //    {
        //        GridViewDataTextColumn tc = new GridViewDataTextColumn();
        //        tc.FieldName = args[x].ToString();
        //        gv.Columns.Add(tc);
        //    }
        //    gv.KeyFieldName = table.Columns[0].ColumnName;
        //    //gv.Columns[0].Visible = false;
        //}
        return dt;
    }
    string selectedDataSource
    {
        get { return Page.Session["selectedDataSource"] == null ? String.Empty : Page.Session["selectedDataSource"].ToString(); }
        set { Page.Session["selectedDataSource"] = value; }
    }
    protected void gv_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.KeyFieldName = "ID";
        g.DataSource = GetDataSource();
        g.ForceDataRowType(typeof(DataRow));
    }

    protected void gv_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //g.DataBind();
    }

    //protected void cp_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    gv.DataBind();
    //}
}