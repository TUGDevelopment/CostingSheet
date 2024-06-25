using DevExpress.Web;
using DevExpress.Xpo.DB;
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
    string labConn = ConfigurationManager.ConnectionStrings["LabConnectionString"].ConnectionString;
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
        SqlParameter[] param = new SqlParameter[] { };
        switch (string.Format("{0}", o))
        {
            case "Matformu":case "Matdetail": case "1MatCode": case "2MatCode":
                param = new SqlParameter[] {
                new SqlParameter("@Company", Request.QueryString["Company"].ToString()),
                new SqlParameter("@Mattype", string.Format("{0}", o))};
                dt = cs.GetRelatedResources("spGetMatformu", param);
                //table.Rows.Add(new object[] { @"ID;Material;Name;Yield", 0 });
                break;
            case "PackingStyle":
                param = new SqlParameter[] {
                new SqlParameter("@TypeofPrimary", Request.QueryString["TypeofPrimary"].ToString())};
                dt = myPackagingClass.GetRelatedResources("spGetPackingStyle", param);
                break;
            case "RawMaterial":
                param = new SqlParameter[] {
                new SqlParameter("@RawMaterial", Request.QueryString["RawMaterial"].ToString()),
                                    new SqlParameter("@Company", Request.QueryString["Company"].ToString())};
                dt = cs.GetRelatedResources("spGetMaterial", param);
                //table.Rows.Add(new object[] { @"ID;Material;Name;Yield", 0 });
                break;
            case "SelMaterial":
                param = new SqlParameter[] {
                new SqlParameter("@Company", Request.QueryString["Company"].ToString()),
                new SqlParameter("@RequestNo", Request.QueryString["ID"].ToString())};
                dt = cs.GetRelatedResources("spSelMaterial", param);
                //table.Rows.Add(new object[] { @"ID;Material;Name;Yield", 0 });
                break;
            case "PackageCode":case "SecPackageCode":case "Packaging":
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
                new SqlParameter("@from",Request.QueryString["from"].ToString()),
                new SqlParameter("@to", Request.QueryString["to"].ToString()),
                new SqlParameter("@Company", Request.QueryString["Company"].ToString()),
                new SqlParameter("@RequestNo", Request.QueryString["ID"].ToString())};
                dt = cs.GetRelatedResources("spGetRawMaterial", param);
                break;
            case "Labor":
                param = new SqlParameter[] {
                new SqlParameter("@Company", Request.QueryString["Company"].ToString()),
                new SqlParameter("@RequestNo", Request.QueryString["ID"].ToString())};
                dt = cs.GetRelatedResources("spGetLabor", param);
                break;
            case "Semi":case "DL":
                param = new SqlParameter[] {
                new SqlParameter("@Company", Request.QueryString["Company"].ToString()),
                new SqlParameter("@RequestNo", Request.QueryString["ID"].ToString())};
                dt = cs.GetRelatedResources("spGetDL", param);
                break;
            case "BCDL":
                param = new SqlParameter[] {
                new SqlParameter("@Company", Request.QueryString["Company"].ToString().Substring(0,3)),
                new SqlParameter("@SubID", Request.QueryString["SubID"].ToString())};
                dt = cs.GetRelatedResources("spGetLB", param);
                break;
            //case "DL":
            //    param = new SqlParameter[] {
            //    new SqlParameter("@Company", Request.QueryString["Company"].ToString().Substring(0,3)),
            //    new SqlParameter("@SubID", Request.QueryString["ID"].ToString())};
            //    dt = cs.GetRelatedResources("spGetLB", param);
            //    break;
            case "OHRate":case "SGA":            
                param = new SqlParameter[] {
                new SqlParameter("@DataType", Request.QueryString["view"].ToString()=="OHRate"?"OH":"SGA"),
                new SqlParameter("@ID", Request.QueryString["ID"].ToString())};
                dt = cs.GetRelatedResources("spGetOHRate", param);
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

            case "RequestForm":
                param = new SqlParameter[] {
                new SqlParameter("@type", Request.QueryString["type"].ToString()),
                new SqlParameter("@user_name", cs.CurUserName.ToString())};
                dt = cs.GetRelatedResources("spGetRequestForm", param);
                break;

            case "Grade":
                param = new SqlParameter[] {
                new SqlParameter("@group", Request.QueryString["group"].ToString())};
                var Results = new DataTable();
                using (SqlConnection conn = new SqlConnection(labConn))
                {
                    using (SqlCommand cmd = new SqlCommand("select Code,Description from dbo.transGrade where dbo.fnc_checktype( productgroup, @group)>0 and ProductType='PF' and isactive in ('0','1') union select '-1',''", conn))
                    {
                        conn.Open();
                        cmd.Connection = conn;
                        cmd.Parameters.AddRange(param);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(Results);
                        conn.Close();
                        conn.Dispose();
                    }
                }
                dt = Results;
                break;    

            case "SizeMedia":
                param = new SqlParameter[] {
                new SqlParameter("@group", Request.QueryString["group"].ToString())};
                var res2 = new DataTable();
                using (SqlConnection conn = new SqlConnection(labConn))
                {
                    using (SqlCommand cmd = new SqlCommand("declare @n nvarchar(max)=(select name from dbo.tblProductGroup where ProductGroup=@group and GroupType='F');select Code,CanSize,Type,Media,NW,NutritionType  from  dbo.transCanSize  where dbo.fnc_checktype( productgroup, @group)>0 and Packaging = (case when @n like  '%non can%' then 'Non Can' else 'Can'end ) and isactive in ('0','1') and ProductType='PF'", conn))
                    {
                        conn.Open();
                        cmd.Connection = conn;
                        cmd.Parameters.AddRange(param);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(res2);
                        conn.Close();
                        conn.Dispose();
                    }
                }
                dt = res2;
                break;

            case "ContainerLid":
                param = new SqlParameter[] {
                new SqlParameter("@group", Request.QueryString["group"].ToString())};
                var res3 = new DataTable();
                using (SqlConnection conn = new SqlConnection(labConn))
                {
                    using (SqlCommand cmd = new SqlCommand("select Code, ContainerType, LidType from  dbo.transContainerLid  where dbo.fnc_checktype( productgroup, @group)>0 and isactive in ('0','1') and ProductType='PF'", conn))
                    {
                        conn.Open();
                        cmd.Connection = conn;
                        cmd.Parameters.AddRange(param);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(res3);
                        conn.Close();
                        conn.Dispose();
                    }
                }
                dt = res3;
                break;

            case "Topping":
                param = new SqlParameter[] {
                new SqlParameter("@group", Request.QueryString["group"].ToString())};
                var res4 = new DataTable();
                using (SqlConnection conn = new SqlConnection(labConn))
                {
                    using (SqlCommand cmd = new SqlCommand("select Code, Description, REPLACE(REPLACE( MediaType,'Topping and Special Requirement',''),'Topping & Special Requirement','') as MediaType from  dbo.transMediaType where dbo.fnc_checktype( productgroup, @group)>0 and isactive in ('0','1') and ProductType='PF'", conn))
                    {
                        conn.Open();
                        cmd.Connection = conn;
                        cmd.Parameters.AddRange(param);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(res4);
                        conn.Close();
                        conn.Dispose();
                    }
                }
                dt = res4;
                break;
            case "TransRawMaterial":
                param = new SqlParameter[] {
                new SqlParameter("@group", Request.QueryString["group"].ToString())};
                var res5 = new DataTable();
                using (SqlConnection conn = new SqlConnection(labConn))
                {
                    using (SqlCommand cmd = new SqlCommand("select Code, Description from dbo.transRawMaterial where dbo.fnc_checktype( productgroup, @group)>0\tand ProductType='PF'", conn))
                    {
                        conn.Open();
                        cmd.Connection = conn;
                        cmd.Parameters.AddRange(param);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(res5);
                        conn.Close();
                        conn.Dispose();
                    }
                }
                dt = res5;
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
    private void AddColumns()
    {
        gv.Columns.Clear();
        if (GetDataSource() == null) return;
        DataView dw = (DataView)GetDataSource().DefaultView;
        var values = new[] { "From", "To", "Validfrom", "Validto" };
        foreach (DataColumn c in dw.Table.Columns)
        {
            var str = c.ColumnName;
            if (c.ColumnName.Contains("tcode"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                gv.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("RequestNo"))
            {
                GridViewDataTextColumn tc = new GridViewDataTextColumn();
                tc.FieldName = c.ColumnName;
                tc.Width = Unit.Percentage(20);
                gv.Columns.Add(tc);
            }
            else
                AddTextColumn(c.ColumnName);
        }
        gv.KeyFieldName = dw.Table.Columns[0].ColumnName;
        gv.Columns[0].Visible = false;
    }
    private void AddTextColumn(string fieldName)
    {
        GridViewDataTextColumn c = new GridViewDataTextColumn();
        c.FieldName = fieldName;
        gv.Columns.Add(c);
    }
    //protected void cp_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    gv.DataBind();
    //}

}