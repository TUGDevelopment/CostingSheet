using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class selcosting : System.Web.UI.Page
{
    myClass myClass = new myClass();
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
        AddColumns();
    }
    protected void gv_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
    {
        ASPxGridView grid = sender as ASPxGridView;
        if (grid.IsNewRowEditing)
        {
            if (e.Column.FieldName == "ProductGroup")
            {
                e.Editor.Enabled = false;
            }
        }
        //if (grid.IsEditing)
        //{
        //    if (e.Column.FieldName == "BaseType")
        //    {
        //        e.Column.Visible = false;
        //    }
        //}
        //if (e.Column.FieldName == "ProductGroup")
        //{
        //           ASPxDropDownEdit dropDownEdit = (ASPxDropDownEdit)e.Editor;
        //            dropDownEdit.ClientInstanceName = "dropDownEdit";
        //            string[] indexes = dropDownEdit.Text.Split(',');

        //            ASPxListBox listBox = (ASPxListBox)dropDownEdit.FindControl("ASPxListBox1");
        //            listBox.ClientInstanceName = "checkListBox";
        //            listBox.ClientSideEvents.SelectedIndexChanged = String.Format("function(s,e){{OnListBoxSelectionChanged(s,e,{0});}}", dropDownEdit.ClientID);
        //            dropDownEdit.ClientSideEvents.TextChanged = String.Format("function(s,e){{ SynchronizeListBoxValues(s,e,{0});}}", listBox.ClientID);
        //            dropDownEdit.ClientSideEvents.DropDown = String.Format("function(s,e){{ SynchronizeListBoxValues(s,e,{0});}}", listBox.ClientID);
        //            if (listBox == null) return;
        //            foreach (ListEditItem item in listBox.Items)
        //            {
        //                if (indexes.Contains<string>(item.Value.ToString()))
        //                    item.Selected = true;
        //            }
        //}
    }
        private void AddColumns()
    {
        gv.Columns.Clear();
        if (GetDataSource() == null) return;
        DataSourceSelectArguments args = new DataSourceSelectArguments();
        SqlDataSource ds = (SqlDataSource)GetDataSource();
        DataView dw = (DataView)ds.Select(args);
        if (dw != null)
            foreach (DataColumn c in dw.Table.Columns)
            {
                if (c.ColumnName == "BaseType")
                {
                    GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                    cb.FieldName = c.ColumnName;
                    cb.PropertiesComboBox.Columns.Clear();
                    cb.PropertiesComboBox.TextField = "value";
                    cb.PropertiesComboBox.TextFormatString = "{0}";
                    cb.PropertiesComboBox.DataSource = cs.builditems("select value from dbo.FNC_SPLIT('Non-Fish Base;Fish Base',';') ");
                    gv.Columns.Add(cb);
                }
                else if (c.ColumnName == "ProductGroup")
                {
                    //GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                    //cb.FieldName = c.ColumnName;
                    //cb.PropertiesComboBox.Columns.Clear();
                    //cb.PropertiesComboBox.TextField = "Name";
                    //cb.PropertiesComboBox.ValueField = "ProductGroup";
                    //cb.PropertiesComboBox.TextFormatString = "{0}";
                    //cb.PropertiesComboBox.DataSource = dsProductGroup;
                    //gv.Columns.Add(cb);
                    GridViewDataDropDownEditColumn col = new GridViewDataDropDownEditColumn();
                    ((DropDownEditProperties)col.PropertiesEdit).DropDownWindowTemplate = new MyDropDownWindow();
                    col.FieldName = c.ColumnName;
                    col.Visible = false;
                    gv.Columns.Add(col);
                }
                else if (c.ColumnName == "CanSize")
                {
                    GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                    cb.FieldName = c.ColumnName;
                    cb.Caption = "Container Type";

                    cb.PropertiesComboBox.Columns.Clear();
                    cb.PropertiesComboBox.TextField = "Description";
                    cb.PropertiesComboBox.ValueField = "Code";
                    cb.PropertiesComboBox.TextFormatString = "{0}";
                    cb.PropertiesComboBox.DataSource = myClass.builditems("select Code,Description from MasfixdigitSize where IsActive='0' and ProductGroup='PF' "); ;
                    gv.Columns.Add(cb);
                }
                else if (c.ColumnName.Contains("ProductType"))
                {
                    GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                    cb.FieldName = c.ColumnName;
                    cb.PropertiesComboBox.Columns.Clear();
                    cb.PropertiesComboBox.TextField = "value";
                    cb.PropertiesComboBox.TextFormatString = "{0}";
                    cb.PropertiesComboBox.DataSource = cs.builditems("select value from dbo.FNC_SPLIT('PF;HF',';') ");
                    gv.Columns.Add(cb);
                }
                else if (c.ColumnName.Contains("NutritionType"))
                {
                    GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                    cb.FieldName = c.ColumnName;
                    cb.PropertiesComboBox.Columns.Clear();
                    cb.PropertiesComboBox.TextField = "value";
                    cb.PropertiesComboBox.TextFormatString = "{0}";
                    cb.PropertiesComboBox.DataSource = cs.builditems("select value from dbo.FNC_SPLIT('Complementary;Completed;',';') ");
                    gv.Columns.Add(cb);
                }
                else if (c.ColumnName.Contains("Packaging"))
                {
                    GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                    cb.FieldName = c.ColumnName;
                    cb.PropertiesComboBox.Columns.Clear();
                    cb.PropertiesComboBox.TextField = "value";
                    cb.PropertiesComboBox.TextFormatString = "{0}";
                    cb.PropertiesComboBox.DataSource = cs.builditems("select value from dbo.FNC_SPLIT('Can;Non Can',';') ");
                    gv.Columns.Add(cb);
                }
                else if (c.ColumnName.Contains("Type"))
                {
                    GridViewDataTextColumn tc = new GridViewDataTextColumn();
                    tc.FieldName = c.ColumnName;
                    tc.Caption = "Container Size";
                    //tc.ReadOnly = true;
                    gv.Columns.Add(tc);
                }
                else if (c.ColumnName.Contains("Code"))
                {
                    GridViewDataTextColumn tc = new GridViewDataTextColumn();
                    tc.FieldName = c.ColumnName;
                    //tc.ReadOnly = true;
                    gv.Columns.Add(tc);
                }
                else
                    AddTextColumn(c.ColumnName);
            }
    }
    private void AddTextColumn(string fieldName)
    {
        GridViewDataTextColumn c = new GridViewDataTextColumn();
        c.FieldName = fieldName;
        gv.Columns.Add(c);
    }
    private object GetDataSource()
    {
        object o = Session["selectedDataSource"];
        //DataSourceType dsType = DataSourceType.Material;
        //if (o != null)
        //    dsType = (DataSourceType)o;

        switch (o.ToString())
        {
            //case "new":case "renew":
            //    return dsGetcosting;
            //case "cost":
            //    return dsGetCostingMat;
            case "ProductGroup":
                ds.SelectCommand = "select ProductGroup,Name from tblProductGroup where GroupType='F'";
                return ds;
            case "ContainerLid":
                ds.SelectCommand = "select ID,Code,ContainerType,LidType from transContainerLid where ProductType='PF'";
                return ds;
            case "Grade":
                ds.SelectCommand = "select ID,Code,Description,IsActive from transGrade where ProductType='PF'";
                return ds;
            case "Brand":
                ds.SelectCommand = "select ID,Code,Description,IsActive from transCustomerBrand where ProductType='PF'";
                return ds;
            case "Style":
                ds.SelectCommand = "SELECT ID,ProductGroup,Code,Description,IsActive from transProductStyle where ProductType='PF'";
                return ds;
            case "RawMaterial":
                ds.SelectCommand = "SELECT ID,ProductGroup, ProductType,Code,Description,IsActive from transRawMaterial where ProductType='PF'";
                return ds;
            case "MediaType":
                ds.SelectCommand = "SELECT ID,MediaType,ProductGroup,Code,Description,IsActive from transMediaType where ProductType='PF'";
                ds.UpdateCommand = "Update transMediaType set ProductGroup=@ProductGroup,Code=@Code where Id=@id";
                ds.InsertCommand = "insert into transMediaType values(@ProductGroup,@Code,@MediaType)";
                ds.DeleteCommand = "Delete transMediaType Where ID=@ID";
                return ds;
            case "CanSize":
                ds.SelectCommand = "select ID,Packaging,ProductGroup,Code,CanSize,PouchWidth,Type,NW,DWeight,ProductType,'' as BaseType,Media,NutritionType from transCanSize where ProductType='PF'";
                ds.InsertCommand = "insert into transCanSize (ProductGroup,Code,CanSize,PouchWidth,Type,NW,DWeight,Packaging,ProductType,IsActive,Media,NutritionType) values(@ProductGroup,@Code,@CanSize,@PouchWidth,@Type,@NW,@DWeight,@Packaging,@ProductType,1,@Media,@NutritionType)";
                return ds;
            default:
                return null;
        }
    }



    protected void gv_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
    {
        //string a = MyToDataTable.Increment("AB");
 
        object o = Session["selectedDataSource"];
        switch (string.Format("{0}", o))
        {
            case "CanSize":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("ProductGroup", e.NewValues["ProductGroup"].ToString());
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("CanSize", e.NewValues["CanSize"].ToString());
                ds.SelectParameters.Add("PouchWidth", e.NewValues["PouchWidth"].ToString());
                ds.SelectParameters.Add("Type", e.NewValues["Zone"].ToString());
                ds.SelectParameters.Add("NW", e.NewValues["NW"].ToString());
                ds.SelectParameters.Add("NutritionType", e.NewValues["NutritionType"].ToString());
                ds.SelectParameters.Add("Media",e.NewValues["Media"].ToString());
                ds.SelectParameters.Add("DWeight", e.NewValues["DWeight"].ToString());
                ds.SelectParameters.Add("Packaging", e.NewValues["Packaging"].ToString());
                break;
        }
    }
    OrderedDictionary buildProductType(OrderedDictionary values)
    {
        SqlParameter[] param = {new SqlParameter("@package", string.Format("{0}", values["Packaging"])),
                        new SqlParameter("@type",string.Format("{0}",values["ProductType"])=="HF"?"H":"F"),//10
                        new SqlParameter("@fishbase", string.Format("{0}", values["BaseType"]).ToLower().Contains("Non-Fish Base")?"non fish": "fish")};
        DataTable sqldt = myClass.GetRelatedResources("spgetproductgroup2", param);
        if (sqldt.Rows.Count > 0)
        {
            DataRow _row = sqldt.Select().FirstOrDefault();
            values["ProductGroup"]= string.Format("{0}", _row["name"]);
             
        }

        return values;
    }
    OrderedDictionary buildCode(OrderedDictionary values)
    {

        //string a = MyToDataTable.Increment("X");
        SqlParameter[] param = { new SqlParameter("@ProductGroup", string.Format("{0}", values["ProductGroup"])),
        new SqlParameter("@Packaging", string.Format("{0}", values["Packaging"])),
        new SqlParameter("@CanSize", string.Format("{0}", values["CanSize"])),
        new SqlParameter("@ProductType", string.Format("{0}", values["ProductType"]))};
        DataTable result = myClass.GetRelatedResources("spGetCanSize", param);
        if (result.Rows.Count > 0)
        {
            values["CanSize"] = myClass.ReadItems("select Description from [MasfixdigitSize] where IsActive='0' and Code='" + string.Format("{0}"
                , values["CanSize"]) + "' and ProductGroup='PF' and Packaging='" + string.Format("{0}", values["Packaging"]) + "'");

            int a = 0;
            string id = result.Rows[0]["Code"].ToString();
            for (int i = id.Length - 1; i >= 0; i--)
            {
                if (string.Format("{0}", id[i]) == "Z")
                {
                    var text = string.Format("{0}", id[i]);
                    if (text.All(Char.IsNumber))
                    {
                        string d = (Convert.ToInt32(text) + 1).ToString();
                        values["Code"] = string.Format("{0}{1}2", values["CanSize"], d);
                        return values;
                    }
                    else
                    {
                        values["Code"] = string.Format("{0}{1}2", values["CanSize"], MyToDataTable.Increment(string.Format("{0}", id[1])));
                        return values;                
                    }
                }
                else if (string.Format("{0}", id[i]) == "9")
                {
                    values["Code"] = string.Format("{0}A", result.Rows[0]["Code"].ToString().Substring(0, 2));
                    return values;
                }
                else
                {
                    values["Code"] = result.Rows[0]["Code"].ToString().Substring(0, 2) + MyToDataTable.Increment(string.Format("{0}", id[i]));
                    return values;
                }
            }

        }
        return values;
    }

    protected void gv_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
    {
        object o = Session["selectedDataSource"];
        switch (string.Format("{0}", o))
        {
            case "CanSize":
                object StateID = e.NewValues["CanSize"];
                GridViewDataComboBoxColumn column = (GridViewDataComboBoxColumn)gv.Columns["CanSize"];
                buildCode(e.NewValues); //
                //e.NewValues["CanSize"] = column.PropertiesComboBox.Items.FindByValue(e.NewValues["CanSize"]).Text;
                ds.SelectParameters.Add("ProductType", string.Format("{0}", e.NewValues["ProductType"]));
                ds.SelectParameters.Add("Code", string.Format("{0}", e.NewValues["Code"]));
                ds.SelectParameters.Add("CanSize", string.Format("{0}", e.NewValues["CanSize"]));
                ds.SelectParameters.Add("PouchWidth", string.Format("{0}", e.NewValues["PouchWidth"]));
                ds.SelectParameters.Add("Type", string.Format("{0}", e.NewValues["Type"]));
                ds.SelectParameters.Add("ProductGroup", string.Format("{0}", buildProductType(e.NewValues)));
                ds.SelectParameters.Add("NW", string.Format("{0}", e.NewValues["NW"]));
                ds.SelectParameters.Add("NutritionType", e.NewValues["NutritionType"].ToString());
                ds.SelectParameters.Add("Media", e.NewValues["Media"].ToString());
                ds.SelectParameters.Add("DWeight", string.Format("{0}", e.NewValues["DWeight"]));
                ds.SelectParameters.Add("Packaging", string.Format("{0}", e.NewValues["Packaging"]));
                break;
        }
        gv.JSProperties["cpShowAlert"] = true;
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

class MyDropDownWindow : ITemplate
{
    void ITemplate.InstantiateIn(Control container)
    {
        ASPxListBox lb = new ASPxListBox();
        lb.ID = "ASPxListBox1";
        container.Controls.Add(lb);
        lb.Width = Unit.Percentage(100);
        lb.SelectionMode = ListEditSelectionMode.CheckColumn;
        //lb.ClientSideEvents.SelectedIndexChanged = "function(s,e) { OnSelectedIndexChanged(s,e); }";
        lb.DataBinding += lb_DataBinding;
    }
    void lb_DataBinding(object sender, EventArgs e)
    {
        ASPxListBox lb = (ASPxListBox)sender;
        lb.DataSource = GetDataSource();
    }
    private List<string> GetDataSource()
    {
        myClass myClass = new myClass();
        DataTable dt = myClass.builditems("select ProductGroup from tblProductGroup where GroupType='F'");
        //List<string> list = dt.AsEnumerable()
        //    .Select(r => r.Field<string>("code"))
        //               .ToList();
        List<string> list = dt.Rows.OfType<DataRow>().Select(dr => (string)dr["ProductGroup"]).ToList();
        return list;

    }
}