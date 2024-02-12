using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class src_selectedvalue : System.Web.UI.Page
{
    //enum DataSourceType
    //{
    //    Material,
    //    Upcharg
    //}
    public DataTable _CurrentTableCol
    {
        get { return Page.Session["colvalue"] == null ? null : (DataTable)Page.Session["colvalue"]; }
        set { Page.Session["colvalue"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["selectedDataSource"] = string.Format("{0}", Request.QueryString["Id"]);
        _CurrentTableCol = buildcolumnTable();
        gv.DataBind();
    }
    DataTable buildcolumnTable()
    {
        DataTable dt = new DataTable();
        dt.Clear();
        dt.Columns.Add("Name");
        dt.Columns.Add("Marks");
        dt.Rows.Add(new object[] { "Id;Material;Description", 1 });
        dt.Rows.Add(new object[] { "Id;UpchargeGroup;Upcharge;Value;Currency;Unit;StdPackSize", 4 });
        return dt;
    }
    //protected void Page_Init(object sender, EventArgs e)
    //{
    //    if (!this.IsPostBack)
    //        Session.Clear();

    //    gv.DataBind();
    //}
    protected void gv_DataBinding(object sender, EventArgs e)
    {
        ReCreateColumns();
        (sender as ASPxGridView).DataSource = GetDataSource();
    }
    private void ReCreateColumns()
    {
        gv.Columns.Clear();
        if (_CurrentTableCol == null)
            _CurrentTableCol = buildcolumnTable();
        
            var index = "";
        if (string.Format("{0}", Session["selectedDataSource"]).Equals("Material"))
            index ="1";
        else if (string.Format("{0}", Session["selectedDataSource"]).Equals("RawMaterial"))
            index = "1";
        else
            index = "4";
        DataRow[] result = _CurrentTableCol.Select(string.Format("Marks='{0}'", index));
        foreach (DataRow data in result)
        {
            string[] args = data["Name"].ToString().Split(';');
            for (int x = 0; x < args.Length; x++)
            {

                GridViewDataTextColumn column = new GridViewDataTextColumn();
                column.FieldName = args[x];
                column.CellStyle.CssClass = "truncated";
                if (args[x] == "UpchargeGroup")
                    column.Width = Unit.Pixel(120);
                if(args[x] == "Upcharge")
                    column.Width = Unit.Pixel(240);
                gv.Columns.Add(column);
                
            }
        }

        gv.KeyFieldName = "Id";
        gv.Columns[0].Visible = false;
    }
    //protected void gv_CustomCallback(object sender, DevExpress.Web.ASPxGridViewCustomCallbackEventArgs e)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
    //    Session["selectedDataSource"] = string.Format("{0}", Request.QueryString["Id"]);
    //    g.Columns.Clear();
    //    g.AutoGenerateColumns = true;
    //    g.KeyFieldName = String.Empty;
    //    g.DataBind();
    //}
    private object GetDataSource()
    {
        object o = Session["selectedDataSource"];
        //DataSourceType dsType = DataSourceType.Material;
        //if (o != null)
        //    dsType = (DataSourceType)o;

        switch (o.ToString())
        {
            case "Material":
                return dssapMaterial;
            case "RawMaterial":
                return dsRawMaterial;
            default:
                return dsUpcharge;
        }
    }
}