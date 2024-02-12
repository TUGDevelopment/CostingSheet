using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class selcosting : System.Web.UI.Page
{
    MyDataModule cs = new MyDataModule();
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    SelectAllCheckbox check = new SelectAllCheckbox();
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["selectedDataSource"] = string.Format("{0}", Request.QueryString["view"]);
        Page.Session["user_name"] = user_name;
        Page.Session["value"] = 0;
        gv.DataBind();
    }

    protected bool GetChecked(int visibleIndex)
    {
        for (int i = 0; i < gv.GetChildRowCount(visibleIndex); i++)
        {
            bool isRowSelected = gv.Selection.IsRowSelectedByKey(gv.GetChildDataRow(visibleIndex, i)["ID"]);
            if (!isRowSelected)
                return false;
        }
        return true;
    }

    protected string GetCaptionText(GridViewGroupRowTemplateContainer container)
    {
        string captionText = !string.IsNullOrEmpty(container.Column.Caption) ? container.Column.Caption : container.Column.FieldName;
        return string.Format("{0} : {1} {2}", captionText, container.GroupText, container.SummaryText);
    }
    protected void gv_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        string[] parameters = e.Parameters.Split(';');
        int index = int.Parse(parameters[0]);
        bool isGroupRowSelected = bool.Parse(parameters[1]);
        for (int i = 0; i < gv.GetChildRowCount(index); i++)
        {
            DataRow row = gv.GetChildDataRow(index, i);
            gv.Selection.SetSelectionByKey(row["ID"], isGroupRowSelected);
        }
    }
    protected void gv_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var result = new Dictionary<string, string>();
        var args = e.Parameters.Split('|');
        string joinRowID = "";
        var list = new List<dynamic>();
        List<object> fieldValues = g.GetSelectedFieldValues(new string[] { "ID" });

        if (fieldValues.Count > 0)
            joinRowID = string.Join(",", fieldValues);
            result["Code"] = joinRowID;
        e.Result = result;
    }
        protected void gv_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e)
    {
        if (e.RowType == GridViewRowType.Group)
        {
            ASPxCheckBox checkBox = gv.FindGroupRowTemplateControl(e.VisibleIndex, "checkBox") as ASPxCheckBox;
            if (checkBox != null)
            {
                checkBox.ClientSideEvents.CheckedChanged = string.Format("function(s, e){{ gv.PerformCallback('{0};' + s.GetChecked()); }}", e.VisibleIndex);
                checkBox.Checked = GetChecked(e.VisibleIndex);
            }
        }
    }
    protected void gv_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (!Session["selectedDataSource"].Equals("new")) return;
        (g.Columns["RequestNo"] as GridViewDataColumn).GroupIndex = 0;
        string editor = cs.IsMemberOfRole(string.Format("{0}", 2));
        if (g.Columns.IndexOf(g.Columns["CommandColumn"]) != -1)
            return;
        GridViewCommandColumn colsel = new GridViewCommandColumn();
        colsel.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        colsel.Width = Unit.Pixel(45);
        colsel.Name = "CommandColumn";
        colsel.ShowSelectCheckbox = true;
        //colsel.HeaderCaptionTemplate = check;
        colsel.VisibleIndex = 0;
        g.Columns.Add(colsel);
    }

    protected void gv_DataBinding(object sender, EventArgs e)
    {
        (sender as ASPxGridView).DataSource = GetDataSource();
    }
    private object GetDataSource()
    {
        object o = Session["selectedDataSource"];
        //DataSourceType dsType = DataSourceType.Material;
        //if (o != null)
        //    dsType = (DataSourceType)o;

        switch (o.ToString())
        {
            case "new":case "renew":
                return dsGetcosting;
            case "cost":
                return dsGetCostingMat;
                //SqlDataSource1.SelectCommand = "SELECT * from t1_TransMapCosting";
                //return SqlDataSource1;
            case "ProductGroup":
                ds.SelectCommand = "select ProductGroup,Name from tblProductGroup where GroupType='F'";
                return ds;
            case "ContainerLid":
                ds.SelectCommand = "select Code,ContainerType,LidType from transContainerLid";
                return ds;
            case "Grade":
                ds.SelectCommand = "select Code,Description,IsActive from transGrade";
                return ds;
            case "Brand":
                ds.SelectCommand = "select Code,Description,IsActive from transCustomerBrand";
                return ds;
            case "Style":
                ds.SelectCommand = "SELECT ID,ProductGroup,Code,Description,IsActive from transProductStyle";
                return ds;
            case "RawMaterial":
                ds.SelectCommand = "SELECT ID,ProductGroup, ProductType,Code,Description,IsActive from transRawMaterial";
                return ds;
            case "MediaType":
                ds.SelectCommand = "SELECT ID,MediaType,ProductGroup,Code,Description,IsActive from transMediaType";
                ds.UpdateCommand = "Update transMediaType set ProductGroup=@ProductGroup,Code=@Code where Id=@id";
                ds.InsertCommand = "insert into transMediaType values(@ProductGroup,@Code,@MediaType)";
                ds.DeleteCommand = "Delete transMediaType Where ID=@ID";
                return ds;
            case "CanSize":
                ds.SelectCommand = "select ID,ProductGroup,Code,CanSize,PouchWidth,Type,NW,DWeight,Packaging from transCanSize";
                return ds;
            default:
                return null;
        }
    }
}
class SelectAllCheckbox : ITemplate
{
    private bool _isSelected;
    public bool IsSelected
    {
        get
        {
            return _isSelected;
        }
        set
        {
            _isSelected = value;
        }
    }
    public void InstantiateIn(Control container)
    {
        ASPxCheckBox box = new ASPxCheckBox();
        box.ID = "cbAll";
        box.Checked = _isSelected;
        box.ClientInstanceName = "cbAll";
        box.ToolTip = "Select all rows";
        box.Init += cbAll_Init;
        box.ClientSideEvents.CheckedChanged = "OnAllCheckedChanged";
        container.Controls.Add(box);
    }
    protected void cbAll_Init(object sender, EventArgs e)
    {
        ASPxCheckBox chk = sender as ASPxCheckBox;
        ASPxGridView grid = (chk.NamingContainer as GridViewHeaderTemplateContainer).Grid;
        chk.Checked = (grid.Selection.Count == grid.VisibleRowCount);
    }
}