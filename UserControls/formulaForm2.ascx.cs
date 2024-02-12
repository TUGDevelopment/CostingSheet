using System;
using System.Data;
using System.Configuration;
using System.Web;
//using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;
using DevExpress.Web;
//using DevExpress.XtraPrinting;
//using DevExpress.XtraPrintingLinks;
//using System.IO;
//using DevExpress.Web.ASPxGauges.Printing;
//using DevExpress.Web.ASPxTreeList;
//using DevExpress.Web.ASPxTreeList.Internal;
using System.Collections.Generic;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
//using System.Drawing;
public partial class UserControls_test : MasterUserControl
{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    private List<formula> table
    {
        get {
            var obj = this.Session["Customtb"];
            if (obj == null) { obj = this.Session["Customtb"] = new List<formula>(); }
            return   (List<formula>)obj; 
        }
        set { this.Session["Customtb"] = value; }
    }
    public List<User> Users
    {
        get
        {
            var obj = this.Session["myList"];
            if (obj == null) { obj = this.Session["myList"] = new List<User>(); }
            return (List<User>)obj;
        }

        set
        {
            this.Session["myList"] = value;
        }
    }
    public override void Update()
    {
        //gridData.DataSource = dsgv;
        //DataTable dt = ((DataView)dsgv.Select(DataSourceSelectArguments.Empty)).Table;
        grid.DataBind();
    }
    protected string SearchText { get { return Utils.GetSearchText(Page); } }
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
            SetInitialRow();
    }
    public void SetInitialRow()
    {
        hftype["type"] = string.Format("{0}", 0);
        username["user_name"] = user_name;
        editor["Name"] = cs.IsMemberOfRole(string.Format("{0}", 2));
        hfStatusApp["StatusApp"] = string.Format("{0}", 0);
        hfBu["BU"] = string.Format("{0}", cs.GetData(user_name, "bu"));
        hfGeID["GeID"] = string.Format("{0}", 0);
        hfgetvalue["NewID"] = string.Format("{0}", "");
        hfRequestNo["ID"] = string.Format("{0}", 0);
        hfid["ID"] = string.Format("{0}", 0);
        usertp["usertype"] = cs.GetData(user_name, "usertype").ToString();

    }
        Hashtable copiedValues = null;
    string[] copiedFields = new string[] { "FirstName", "Title", "Notes", "LastName", "BirthDate", "HireDate" };
    protected void grid_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {
        if (e.ButtonID != "Clone") return;
        copiedValues = new Hashtable();
        foreach (string fieldName in copiedFields)
        {
            copiedValues[fieldName] = grid.GetRowValues(e.VisibleIndex, fieldName);
        }
        grid.AddNewRow();

    }
    protected void grid_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
    {
        //if (copiedValues == null) return;
        //foreach (string fieldName in copiedFields)
        //{
        //    e.NewValues[fieldName] = copiedValues[fieldName];
        //}
        //DataTable dt = (DataTable)Session["gridDataTable"];
        if (table == null) return;
        int NextRowID = cs.FindMaxValue(table, x => x.Id);
        NextRowID++;
        e.NewValues["RowID"] = NextRowID;
        e.NewValues["Subype"] = "";
    }
    protected void CmbCompany_Callback(object sender, CallbackEventArgsBase e)
    {
        if (string.IsNullOrEmpty(e.Parameter))
            return;
        var args = e.Parameter.Split('|');
    }
    protected void CmbDisponsition_Callback(object sender, CallbackEventArgsBase e)
    {
        if (string.IsNullOrEmpty(e.Parameter)) return;
        SqlParameter[] param = {new SqlParameter("@Id",(string)e.Parameter),
                        new SqlParameter("@table","TransCusFormulaHeader"),
                        new SqlParameter("@username",user_name.ToString())};
        CmbDisponsition.DataSource = cs.GetRelatedResources("spGetStatusApproval", param);
        CmbDisponsition.DataBind();
    }

    protected void gridData_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string[] args = e.Parameters.Split('|'); //bool selected = true;
        //long id;
        //if (!long.TryParse(args[1], out id))
        //    return;
        DataTable dt = new DataTable();
        if (args[0] == "delete" && args.Length > 1)
        {
            Delete(string.Format("{0}", args[1]));
        }
        if (args[0]== "Revised")
        {
                Revised(args[1]);
        }
        if (args[0] == "post") {

            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spinsertCusFormulaHeader";
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", hfid["ID"]));
                cmd.Parameters.AddWithValue("@Name", string.Format("{0}", tbDestination.Text));
                cmd.Parameters.AddWithValue("@Customer", string.Format("{0}", tbCustomer.Text));
                cmd.Parameters.AddWithValue("@Code", string.Format("{0}", tbProduct.Text));
                cmd.Parameters.AddWithValue("@RefSamples", string.Format("{0}", tbReference.Text));
                cmd.Parameters.AddWithValue("@Formula", string.Format("{0}", 1));
                cmd.Parameters.AddWithValue("@IsActive", string.Format("{0}", 0));
                cmd.Parameters.AddWithValue("@CostNo", string.Format("{0}", tbRequestNo.Text.Equals("###########")?"": tbRequestNo.Text));
                cmd.Parameters.AddWithValue("@Costper", string.Format("{0}", usertp["usertype"]));
                cmd.Parameters.AddWithValue("@FW", string.Format("{0}", tbFW.Text));
                cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", 0));
                cmd.Parameters.AddWithValue("@UniqueColumn", string.Format("{0}", hfgetvalue["NewID"]));
                cmd.Parameters.AddWithValue("@NetWeight", string.Format("{0}|{1}", tbNetweight.Text,CmbUnit.Text));
                //cmd.Parameters.AddWithValue("@Packaging", string.Format("{0}", 0));
                cmd.Parameters.AddWithValue("@Company", string.Format("{0}", CmbCompany.Value));
                cmd.Parameters.AddWithValue("@ProductStyle", string.Format("{0}", tbProductStyle.Text));
                cmd.Parameters.AddWithValue("@Packaging", string.Format("{0}", CmbPackaging.Value));
                cmd.Parameters.AddWithValue("@Revised", string.Format("{0}", tbRevised.Text));
                cmd.Connection = con;
                con.Open();
                DataTable _dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(_dt);
                foreach (DataRow dr in _dt.Rows)
                {
                    saveitems(dr["ID"].ToString());
                }
            }

            
        }
        if (args[0] == "Search")
        {
            string filter = args[0] == "Search" ? SearchText : string.Empty;
            //gridData.SearchPanelFilter = filter;
            //gridData.ExpandAll();
            if (filter == "")
            {
                g.FilterExpression = "";
            }
            else
            {
                string fExpr = "";
                foreach (GridViewColumn column in g.Columns)
                {
                    if (column is GridViewDataColumn)
                    {
                        if (fExpr == "")
                        {
                            fExpr = "[" + (column as GridViewDataColumn).FieldName + "] Like '%" + filter + "%'";
                        }
                        else
                        {
                            fExpr = fExpr + "OR " + "[" + (column as GridViewDataColumn).FieldName + "] Like '%" + filter + "%'";
                        }
                    }
                }
                g.FilterExpression = fExpr;
            }
        }
        ApplyLayout(g);
        //g.GroupBy(g.Columns["RequestNo"]);
        g.DataBind();
    }
    void ApplyLayout(ASPxGridView g)
    {
        g.BeginUpdate();
        try
        {
            g.ClearSort();
            g.GroupBy(g.Columns["RequestNo"]);
        }
        finally
        {
            g.EndUpdate();
        }
        g.ExpandAll();
    }
    protected void gridData_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {

    }

    protected void gridData_CustomGroupDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e)
    {

    }

    protected void gridData_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        var args = e.Parameters.Split('|');
        long id;
        if (!long.TryParse(args[2], out id))
            return;
        ASPxGridView g = sender as ASPxGridView;
        var result = new Dictionary<string, string>();
        DataTable dt = new DataTable();
        //DataTable table = (DataTable)g.DataSource;
        if (args[1] == "New")
        {

            //hfgetvalue["NewID"] = string.Format("{0}", cs.GetNewID());
            result["NewID"] = (string)cs.GetNewID();
            result["editor"] = cs.IsMemberOfRole(string.Format("{0}", 2));
            e.Result = result;

        }
        if (args[1] == "print")
        {
            result["print"] = "0";
            result["view"] = "6";
            result["KeyValue"] = String.Format("{0}", id);
            e.Result = result;
        }
        if (args[1] == "RequestNo")
        {
            result["view"] = args[1].ToString();
            result["ID"] = id.ToString();
            string strSQL = string.Format(@"select * from TransTechnical where ID='{0}'", id);
            dt = cs.builditems(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                result["RequestNo"] = dr["RequestNo"].ToString();
                result["form"] = Convert.ToDateTime(dr["RequestDate"]).ToString("dd-MM-yyyy");
                result["to"] = Convert.ToDateTime(dr["RequireDate"]).ToString("dd-MM-yyyy");
                if (dr["NetWeight"].ToString().Contains("|"))
                {
                    var Net = dr["NetWeight"].ToString().Split('|');
                    result["NetWeight"] = Net[0].ToString();
                    result["WeightUnit"] = Net[1].ToString();
                }
                else
                {
                    result["NetWeight"] = "0";
                    result["WeightUnit"] = "Grams";
                }
                result["PackSize"] = dr["PackSize"].ToString();
                result["Packaging"] = dr["Packaging"].ToString();
                result["RequestType"] = dr["RequestType"].ToString();
                result["UserType"] = dr["UserType"].ToString();

                result["Customer"] = dr["Customer"].ToString();
                result["CanSize"] = dr["Drainweight"].ToString();
                result["RequestForm"] = "";
            }
            e.Result = result;
        }
        if (args[1] == "EditDraft" || args[1] == "save")
        {
            SqlParameter[] param = {new SqlParameter("@Id",id.ToString()),
                        //new SqlParameter("@param",args[1].ToString()),
                        new SqlParameter("@username",user_name.ToString())};
            dt = cs.GetRelatedResources("spGetCusFormulaHeader", param);
            foreach(DataRow rows in dt.Rows)
            {
                result["ID"] = string.Format("{0}", rows["ID"]);
                result["editor"] = string.Format("{0}",rows["editor"]);
                result["RequestNo"] = string.Format("{0}", rows["RequestNo"]);
                result["Company"] = string.Format("{0}", rows["Company"]);
                result["Code"] = string.Format("{0}", rows["Code"]);
                result["CostNo"] = string.Format("{0}", rows["CostNo"]);
                result["RefSamples"] = string.Format("{0}", rows["RefSamples"]);
                result["Customer"] = string.Format("{0}", rows["Customer"]);
                result["ProductStyle"] = string.Format("{0}", rows["ProductStyle"]);
                result["Packaging"] = string.Format("{0}",rows["Packaging"]);
                result["StatusApp"] = string.Format("{0}", rows["StatusApp"]);
                result["Revised"] = string.Format("{0}", rows["Revised"]);
                result["FW"] = string.Format("{0}", rows["FW"]);
                result["Destination"] = string.Format("{0}", rows["Name"]);
                result["NewID"] = string.Format("{0}", rows["UniqueColumn"]);
                string values = rows["NetWeight"].ToString();
                string[] array = values.Split('|');
                if (array.Length > 1) { 
                    result["NetWeight"] = array[0];
                    result["Unit"] = array[1];
                }
                e.Result = result;
            }
        }
    }
    protected void gridData_DataBound(object sender, EventArgs e)
    {

    }

    protected void gridData_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            //if (string.Format("{0}", editor["Name"].ToString()) != "0")
            //    return;
            var item = e.CreateItem("Revised", "Revised");
            item.Image.Url = @"~/Content/Images/Edit.gif";
            e.Items.Add(item);

            item = e.CreateItem("Export", "Export");
            item.BeginGroup = true;
            item.Image.Url = @"~/Content/Images/if_sign-out_59204.png";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), item);
            AddMenuSubItem(item, "PDF", "ExportToPDF", "~/Content/Images/pdf.gif", true);
            AddMenuSubItem(item, "XLS", "ExportToXLS", "~/Content/Images/excel.gif", true);

            GridViewContextMenuItem test = e.CreateItem("Copied", "Copied");
            test.Image.Url = @"~/Content/Images/Copy.png";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), test);

            GridViewContextMenuItem assign = e.CreateItem("Assign", "Assign");
            assign.Image.Url = @"~/Content/Images/icons8-plus-16.png";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), assign);

        }
    }
    private static void AddMenuSubItem(GridViewContextMenuItem parentItem, string text, string name, string iconID, bool isPostBack)
    {
        var exportToXlsItem = parentItem.Items.Add(text, name);
        exportToXlsItem.Image.Url = iconID;
    }
    protected void gridData_ContextMenuItemClick(object sender, ASPxGridViewContextMenuItemClickEventArgs e)
    {

    }

    protected void grid_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //(sender as ASPxGridView).DataSource = LoadGrid;
        g.KeyFieldName = "Id";
        g.DataSource = table;
        g.ForceDataRowType(typeof(DataRow));
        //AddCommandColumn();
        //((GridViewDataComboBoxColumn)g.Columns["SAPMaterial"]).PropertiesComboBox.DataSource = dsSAPMaterial;
    }
    //void AddCommandColumn()
    //{
    //    GridViewCommandColumn c = new GridViewCommandColumn();
    //    grid.Columns.Add(c);
    //    c.VisibleIndex = 0;
    //    c.Width = 45;
    //    c.ButtonRenderMode = GridCommandButtonRenderMode.Image;
    //    c.ShowEditButton = true;
    //    c.ShowNewButtonInHeader = true;
    //    c.ShowDeleteButton = true;
    //    c.ShowCancelButton = true;
    //    c.ShowUpdateButton = true;
    //    grid.SettingsCommandButton.NewButton.Image.Url = "~/Content/images/icons8-plus-math-filled-16.png";
    //    grid.SettingsCommandButton.DeleteButton.Image.Url = "~/Content/images/cancel.gif";
    //}
    //private void AddTextColumn(string fieldName)
    //{
    //    GridViewDataTextColumn c = new GridViewDataTextColumn();
    //    c.FieldName = fieldName;
    //    if (fieldName== "Description" || fieldName == "SubType")
    //    c.Width= Unit.Pixel(270);
    //    if (fieldName == "Mark")
    //    {
    //        c.HeaderStyle.CssClass = "hide";
    //        c.EditCellStyle.CssClass = "hide";
    //        c.CellStyle.CssClass = "hide";
    //        c.FilterCellStyle.CssClass = "hide";
    //        c.FooterCellStyle.CssClass = "hide";
    //        c.GroupFooterCellStyle.CssClass = "hide";
    //    }
    //    grid.Columns.Add(c);
    //}
    //private DataTable test
    //{
    //    get
    //    {   
    //        return table;
    //    }
    //}
    void saveitems(string keys)
    {
        foreach (var c in table)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spinsertCusFormula";
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", c.Id));
                cmd.Parameters.AddWithValue("@Component", string.Format("{0}", c.Component));
                cmd.Parameters.AddWithValue("@SubType", string.Format("{0}", c.SubType));
                cmd.Parameters.AddWithValue("@Description", string.Format("{0}", c.Description));
                cmd.Parameters.AddWithValue("@Material", string.Format("{0}", c.Material));
                cmd.Parameters.AddWithValue("@Result", string.Format("{0}", c.Result));
                cmd.Parameters.AddWithValue("@Yield", string.Format("{0}", c.Yield));
                cmd.Parameters.AddWithValue("@RawMaterial", string.Format("{0}", c.RawMaterial));
                cmd.Parameters.AddWithValue("@Name", string.Format("{0}", c.Name));
                cmd.Parameters.AddWithValue("@PriceOfUnit", string.Format("{0}", c.PriceOfUnit));
                cmd.Parameters.AddWithValue("@AdjustPrice", string.Format("{0}", c.AdjustPrice));
                cmd.Parameters.AddWithValue("@Currency", string.Format("{0}", c.Currency));
                cmd.Parameters.AddWithValue("@Unit", string.Format("{0}", c.Unit));
                cmd.Parameters.AddWithValue("@ExchangeRate", string.Format("{0}", c.ExchangeRate));
                cmd.Parameters.AddWithValue("@BaseUnit", string.Format("{0}", c.BaseUnit));
                cmd.Parameters.AddWithValue("@PriceOfCarton", string.Format("{0}", c.PriceOfCarton));
                cmd.Parameters.AddWithValue("@Formula", string.Format("{0}", 1));
                cmd.Parameters.AddWithValue("@IDNumber", string.Format("{0}", c.IDNumber));
                cmd.Parameters.AddWithValue("@IsActive", string.Format("{0}", c.IsActive));
                cmd.Parameters.AddWithValue("@Remark", string.Format("{0}", c.Remark));
                cmd.Parameters.AddWithValue("@LBOh", string.Format("{0}", c.LBOh));
                cmd.Parameters.AddWithValue("@LBRate", string.Format("{0}", c.LBRate));
                cmd.Parameters.AddWithValue("@RequestNo", string.Format("{0}", 0));
                cmd.Parameters.AddWithValue("@NW", string.Format("{0}", c.NW));
                cmd.Parameters.AddWithValue("@Batch", string.Format("{0}", c.Batch));
                cmd.Parameters.AddWithValue("@Portion", string.Format("{0}", c.Portion));
                cmd.Parameters.AddWithValue("@Note", string.Format("{0}", c.Note)); 
                cmd.Parameters.AddWithValue("@SubID", string.Format("{0}", keys));

                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
    //private DataTable LoadGrid
    //{
    //    get
    //    {
    //        if (table == null)
    //            return table;
    //        grid.Columns.Clear();
    //        //grid.TotalSummary.Clear();
    //        GridViewBandColumn bandColumn = new GridViewBandColumn();
    //        bandColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
    //        var values = new[] { "Description", "RowID","Yield","Name","Mark" };
    //        foreach (DataColumn c in table.Columns)
    //        {
    //            var str = c.ColumnName;
    //            if (c.ColumnName.Contains("SAPMaterial"))
    //            {
    //                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
    //                cb.FieldName = c.ColumnName;
    //                cb.PropertiesComboBox.Columns.Clear();
    //                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("RawMaterial"));
    //                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("Description"));
    //                cb.PropertiesComboBox.ValueField = "RawMaterial";
    //                cb.PropertiesComboBox.TextFormatString = "{0}";
    //                cb.PropertiesComboBox.CallbackPageSize = 10;
    //                cb.PropertiesComboBox.EnableCallbackMode = true;
    //                cb.PropertiesComboBox.IncrementalFilteringDelay = 500;
    //                cb.PropertiesComboBox.ClientInstanceName = "SAPMaterial";
    //                cb.PropertiesComboBox.ClientSideEvents.SelectedIndexChanged = "Combo_SelectedIndexChanged";
    //                grid.Columns.Add(cb);
    //            }
    //            else if (c.ColumnName.Contains("RawMaterial"))
    //            {
    //                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
    //                cb.FieldName = c.ColumnName;
    //                cb.PropertiesComboBox.Columns.Clear();
    //                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("Material"));
    //                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("Name"));
    //                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("Yield"));
    //                cb.PropertiesComboBox.ValueField = "Material";
    //                cb.PropertiesComboBox.TextFormatString = "{0}";
    //                cb.PropertiesComboBox.CallbackPageSize = 10;
    //                cb.PropertiesComboBox.EnableCallbackMode = true;
    //                cb.PropertiesComboBox.ItemsRequestedByFilterCondition += OnItemsRequestedByFilterCondition;
    //                cb.PropertiesComboBox.IncrementalFilteringDelay = 500;
    //                cb.PropertiesComboBox.ClientInstanceName = "RawMaterial";
    //                cb.PropertiesComboBox.ClientSideEvents.SelectedIndexChanged = "OnSelectedIndexChanged";
    //                cb.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
    //                grid.Columns.Add(cb);

    //            }
    //            else if (c.ColumnName.Contains("SubType") || c.ColumnName.Contains("Component"))
    //            {
    //                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
    //                cb.FieldName = c.ColumnName;
    //                if (c.ColumnName.Contains("Component"))
    //                    cb.Width = Unit.Pixel(270);
    //                 //cb.PropertiesComboBox.Columns.Clear();
    //                //cb.PropertiesComboBox.TextField = "value";
    //                //cb.PropertiesComboBox.TextFormatString = "{0}";
    //                grid.Columns.Add(cb);
    //            }
    //            else if (values.Any(c.ColumnName.Contains))
    //                AddTextColumn(c.ColumnName);
    //            else {
    //                GridViewDataSpinEditColumn se = new GridViewDataSpinEditColumn();
    //                se.FieldName = c.ColumnName;
    //                se.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
    //                bandColumn.Columns.Add(se);
    //            }
    //        }
    //        bandColumn.Caption = "(gm./can)";
    //        grid.Columns.Add(bandColumn);
    //        //grid.Columns["Material"].FixedStyle = GridViewColumnFixedStyle.Left;
    //        if (table.Rows.Count > 0) { 
    //        grid.KeyFieldName = table.Columns[0].ColumnName;
    //        grid.Columns[0].Visible = false;}
    //        //var name = string.Format("{0}", table.Columns[7].ColumnName);
    //        //grid.TotalSummary.Add(DevExpress.Data.SummaryItemType.Custom, name);
    //        //grid.CustomSummaryCalculate += Grid_CustomSummaryCalculate;
    //        //grid.SettingsEditing.Mode = GridViewEditingMode.Batch;
    //        return table;
    //    }
    //}
    decimal totalCount;
    protected void grid_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
    {
        if (e.IsGroupSummary)
        {
            e.TotalValue = string.Format("Total = {0}",0);
        }
        ASPxGridView g = sender as ASPxGridView;
        ASPxSummaryItem totalSummary = new ASPxSummaryItem();
        const string value = "0123456789";
        ASPxSummaryItem item = (ASPxSummaryItem)e.Item;
        if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
        {
            totalCount = 0;
        }
        if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
        {
            if (!string.IsNullOrEmpty(string.Format("{0}", e.FieldValue)))
                totalCount += Convert.ToDecimal(e.FieldValue);
        }
        if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            if (value.IndexOf(((ASPxSummaryItem)e.Item).FieldName) != -1)
            {
                e.TotalValue = String.Format("Total = {0}<br />", totalCount.ToString("F4"));}
    }
    void Revised(string keyValue)
    {
        //var args = keyValue.ToString().Split('|');
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spCopyFormulaRevised";
            cmd.Parameters.AddWithValue("@Id", keyValue);
            cmd.Parameters.AddWithValue("@Requester", user_name.ToString());
            cmd.Connection = con;
            con.Open();
            var getValue = cmd.ExecuteScalar();
            con.Close();
            //var getValue = keyValue.ToString();//args[0];
            if (getValue.ToString() != "0")
                gridData.JSProperties["cpKeyValue"] = (getValue == null) ? string.Empty : getValue.ToString();
        }
    }
    void Delete(string keys)
    {
        //if (hfStatusApp["StatusApp"].ToString() == "2")
        //{
        //    string strSQL = string.Format(@"update TransTechnical set Statusapp=-1 where ID={0}; select ID," +
        //            "RequestNo,StatusApp,format(RequestDate, 'dd-MM-yyy') as 'Form'," +
        //            "format(RequireDate, 'dd-MM-yyy') as 'To' from TransTechnical where ID = {0}", keys[0].ToString());
        //    DataTable dt = cs.builditems(strSQL);
        //    foreach (DataRow dr in dt.Rows)
        //        _alertmail(dr, "-1");
        //}else
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "Update TransTechnical set StatusApp=5 where ID=@ID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spDelCusFormulaHD";
            cmd.Parameters.AddWithValue("@ID", keys.ToString());
            cmd.Parameters.AddWithValue("@user", user_name.ToString());
            cmd.Parameters.AddWithValue("@StatusApp", "-1");
            cmd.Parameters.AddWithValue("@tablename", string.Format("{0}", "TransCusFormulaHeader"));
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    protected void grid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string[] args = e.Parameters.Split('|'); //bool selected = true;
        long id;
        if (!long.TryParse(args[1], out id))
            return;
        //DataTable dt = new DataTable();
        switch (args[0])
        {
            //case "delete":
            //    Delete(string.Format("{0}", id));
            //    break;
            case "New":
                // add detail
                var obj = table.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
                User u = new User();
                int iMax = cs.FindMaxValue(obj.User, x => x.Id);
                iMax++;
                u.Id = iMax;
                u.parentID = Convert.ToInt32(id);
                obj.User.Add(u);

                break;
            case "reload":
  
                DataTable dt = cs.GetCusFormula(id.ToString());
                //List<formula> stdList = new List<formula>();
                table = (from DataRow dr in dt.Rows
                           select new formula()
                           {
                               Id = Convert.ToInt32(dr["Id"]),
                               Material = dr["Material"].ToString(),
                               SubType = string.Format("{0}", dr["SubType"]),
                               Component = string.Format("{0}", dr["Component"]),
                               Description = string.Format("{0}", dr["Description"]),
                               Result = Convert.ToDecimal(dr["Result"]),
                               NW = Convert.ToDecimal(dr["NW"]),
                               Batch = Convert.ToDecimal(dr["Batch"]),
                               Portion = Convert.ToDecimal(dr["Portion"]),
                               Yield = string.Format("{0}", dr["Yield"]),
                               RawMaterial = string.Format("{0}", dr["RawMaterial"]),
                               Name = string.Format("{0}", dr["Name"]),
                               PriceOfUnit = string.Format("{0}", dr["PriceOfUnit"]),
                               AdjustPrice = string.Format("{0}", dr["AdjustPrice"]),
                               Currency = string.Format("{0}", dr["Currency"]),
                               Unit = string.Format("{0}", dr["Unit"]),
                               ExchangeRate = string.Format("{0}", dr["ExchangeRate"]),
                               BaseUnit = string.Format("{0}", dr["BaseUnit"]),
                               PriceOfCarton = string.Format("{0}", dr["PriceOfCarton"]),
                               Formula = 0,
                               IsActive = string.Format("{0}", dr["IsActive"]),
                               Remark = string.Format("{0}", dr["Remark"]),
                               LBOh = string.Format("{0}", dr["LBOh"]),
                               LBRate = string.Format("{0}", dr["LBRate"]),
                               RequestNo = string.Format("{0}", dr["RequestNo"]),
                               User = GetUser(string.Format("{0}", dr["RequestNo"])),
                           }).ToList();

                //table.PrimaryKey = new DataColumn[] { table.Columns["Id"] };
                //g.TotalSummary.Clear();
                //var name = string.Format("{0}", table.Columns[9].ColumnName);
                //g.TotalSummary.Add(DevExpress.Data.SummaryItemType.Custom, name);
                //load
                break;
            case "save":
                DataTable t = (DataTable)g.DataSource;
                if (table != null)
                {
                    for (int i = 0; i < g.VisibleRowCount; i++)
                    {
                        var Key = g.GetRowValues(i, g.KeyFieldName).ToString();
                    }
                }
                    break;
        }
        g.DataBind();
    }

    protected void grid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        var values = new[] { "Description", "Name"};
        if (values.Any(e.Column.FieldName.Contains))
            e.Editor.ReadOnly = true;
        if (e.Column.FieldName == "SubType")
        {
            if (table != null)
            {
                ASPxComboBox combo = (ASPxComboBox)e.Editor;
                var ValuetoReturn = (from Rows in table.AsEnumerable()
                                     select string.Format("{0}", Rows.Description)).Distinct().ToList();
                if (ValuetoReturn.Count > 0)
                {
                    List<string> ls = new List<string>();
                    ValuetoReturn.Add(string.Empty);
                    foreach (var name in ValuetoReturn)
                    {
                        ls.Add(string.Format("{0}", name));
                    }
                    //var sortedTable = ValuetoReturn.Distinct().OrderBy(q => q.ToString());
                    combo.DataSource = ls; 
                    combo.DataBindItems();
                }
                // var myResult = dataTable.AsEnumerable()
                //.Select(s => new
                //{
                //    id = s.Field<string>(e.Column.FieldName),
                //})
                //.Distinct().ToList();
                //List<string> ls = new List<string>();
                //var ValuetoReturn = (from Rows in table.AsEnumerable()
                //                     select Rows[e.Column.FieldName]).Distinct().ToList();
                //foreach (var name in ValuetoReturn) {
                //    ls.Add(string.Format("{0}",name));
                //    Console.WriteLine(name); }
                //ValuetoReturn.Add(string.Empty);
                //ValuetoReturn.Add("Raw Material");
                //ASPxComboBox cb = (ASPxComboBox)e.Editor;
                //cb.DataSource = ls;
                ////cb.DataSource = ValuetoReturn.Distinct().OrderBy(q => q);
                //cb.DataBindItems();
                ////cb.DataBind();
            }
        }
        else if (e.Column.FieldName == "SAPMaterial")
        {
            //dsSAPMaterial.SelectParameters.Clear();
            //dsSAPMaterial.SelectParameters.Add("Company", string.Format("{0}",CmbCompany.Text));
            //dsSAPMaterial.DataBind();
            ASPxComboBox combo = (ASPxComboBox)e.Editor;
            //SqlParameter[] param = { new SqlParameter("@Company", string.Format("{0}",CmbCompany.Text)),
            //    new SqlParameter("@RequestNo",string.Format("{0}",0))};
            //var Results = cs.GetRelatedResources("spGetRawMaterial", param);
            //if (Results != null)
            //    combo.DataSource = (DataTable)Results;
            //combo.DataBind();
            //DataView dvsqllist = (DataView)dsSAPMaterial.Select(DataSourceSelectArguments.Empty);
            //if (dvsqllist != null) 
            //combo.DataSource = dvsqllist.Table;
            //combo.DataBindItems();
        }
        //var values = new[] { "Material", "Component", "SubType" };
        //if (values.Any(e.Column.FieldName.Contains))
        //{
        //    ASPxComboBox cb = (ASPxComboBox)e.Editor;
        //    cb.ItemRequestedByValue += new ListEditItemRequestedByValueEventHandler(Cb_OnItemRequestedByValue_SQL);
        //    cb.ItemsRequestedByFilterCondition += new ListEditItemsRequestedByFilterConditionEventHandler(Cb_OnItemsRequestedByFilterCondition_SQL);
        //    ((ASPxComboBox)e.Editor).ClientSideEvents.ValueChanged = "function(s,e){completarArticulo(); }";
        //    //if (!g.IsNewRowEditing) ((ASPxComboBox)e.Editor).ClientEnabled = false;
        //    //cb.DataBind();
        //}
        //if (e.Column.FieldName == "RawMaterial")
        //{
        //    var comb = (ASPxComboBox)e.Editor;
        //    comb.ClientInstanceName = "RawMaterial";
        //    comb.ClientSideEvents.EndCallback = "Combo_EndCallback";

        //    if (g.IsNewRowEditing) return;
        //    if (editor["Name"].ToString() != "0")
        //        e.Editor.ReadOnly = true;
        //}
    }
    protected void Cb_OnItemsRequestedByFilterCondition_SQL(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
    {
        //ASPxComboBox comboBox = (ASPxComboBox)source;
        //IList<Articulo> Lista_Articulos = new ArticuloService().TraerVarios(e.Filter, e.BeginIndex + 1, e.EndIndex + 1);
        //comboBox.DataSource = Lista_Articulos;
        //comboBox.DataBind();
        //ASPxComboBox combo = (ASPxComboBox)source;
        //dsSAPMaterial.SelectParameters.Clear();
        //dsSAPMaterial.SelectParameters.Add("Company", CmbCompany.Text);
        //dsSAPMaterial.SelectParameters.Add("RequestNo", "0");
        //dsSAPMaterial.DataBind();
        //DataView dvsqllist = (DataView)dsSAPMaterial.Select(DataSourceSelectArguments.Empty);
        //if (dvsqllist != null)
        //{
        //    int c = dvsqllist.Table.Rows.Count;
        //    DataTable dt = dvsqllist.Table;
        //    combo.DataSource = dt;
        //    combo.DataBind();
        //}
    }

    protected void Cb_OnItemRequestedByValue_SQL(object source, ListEditItemRequestedByValueEventArgs e)
    {
        //long value = 0;
        //if (!Int64.TryParse(e.Value.ToString(), out value))
        //    return;
        ASPxComboBox comboBox = (ASPxComboBox)source;
        //List<Articulo> Lista_Articulos = new List<Articulo>();
        //Articulo articulo = new ArticuloService().TraerUno(e.Value.ToString());
        //if (articulo != null) Lista_Articulos.Add(articulo);
        //comboBox.DataSource = Lista_Articulos;
        //comboBox.DataBind();
    }
    private string GetCurrentCountry()
    {
        object id = null;
        if (hf.TryGet("CurrentCountry", out id))
            return Convert.ToString(id);
        return Convert.ToString(id);
    }
    protected void OnItemsRequestedByFilterCondition(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
    {
        ASPxComboBox editor = source as ASPxComboBox;
        //IQueryable<City> query;
        var take = e.EndIndex - e.BeginIndex + 1;
        var skip = e.BeginIndex;
        object countryValue = GetCurrentCountry();
        //if (countryValue > -1)
        //    query = entity.Cities.Where(city => city.CityName.Contains(e.Filter) && city.Country.CountryId == countryValue).OrderBy(city => city.CityId);
        //else
        //    query = entity.Cities.Where(city => city.CityName.Contains(e.Filter)).OrderBy(city => city.CityId);
        //query = query.Skip(skip).Take(take);
        //editor.DataSource = query.ToList();
        //editor.DataBind();

        dsMaterial.SelectParameters.Clear();
        dsMaterial.SelectParameters.Add("Company", CmbCompany.Text);
        dsMaterial.SelectParameters.Add("RawMaterial", countryValue.ToString());
        dsMaterial.DataBind();
        DataView dvsqllist = (DataView)dsMaterial.Select(DataSourceSelectArguments.Empty);
        if (dvsqllist != null)
        {
            int c = dvsqllist.Table.Rows.Count;
            //editor.BackColor= c == 0?Color.Coral:Color.Green;

            DataTable dt = dvsqllist.Table;
            editor.DataSource = dt;
            editor.DataBind();
        }
        //editor.DataSource = dsMaterial;
        //editor.DataBind();
    }
 
    //protected void grid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
    //{
    //    //table = LoadGrid;
    //    ASPxGridView g = (ASPxGridView)sender;
    //    DataRow row = table.NewRow();
    //    int NextRowID = table.Rows.Count ;
    //    NextRowID++;
    //    row["RowID"] = NextRowID;
    //    foreach (DataColumn column in table.Columns)
    //    {
    //        if (!column.ColumnName.Contains("RowID"))
    //        {
    //            row[column.ColumnName] = e.NewValues[column.ColumnName];
    //        }
    //    }

    //    //row["SubType"] = e.NewValues["SubType"];
    //    //row["Component"] = e.NewValues["Component"];
    //    //row["RawMaterial"] = string.Format("{0}", e.NewValues["RawMaterial"]);
    //    //g.CancelEdit();
    //    //e.Cancel = true;
    //    //table.Rows.Add(row);
    //    table.Rows.InsertAt(row, 0);
    //    e.Cancel = true;
    //    g.CancelEdit();
    //}

    //protected void grid_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
    //{
    //    ASPxGridView g = (ASPxGridView)sender;
    //    DataRow dr = table.Rows.Find(e.Keys[0]);
    //    foreach (DataColumn column in table.Columns)
    //    {
    //        if (!column.ColumnName.Contains("RowID"))
    //        {
    //            dr[column.ColumnName] = e.NewValues[column.ColumnName];
    //        }
    //    }
    //    g.DataBind();
    //    //test(table);
    //    //calculatecolumn = true;
    //    g.CancelEdit();
    //    e.Cancel = true;
    //}

    //protected void CmbPackSize_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    ASPxComboBox comb = sender as ASPxComboBox;
    //    if (string.IsNullOrEmpty(e.Parameter)) return;
    //    string[] param = e.Parameter.Split('|');
    //    if(param[0]== "AddRow") { 
    //    var item = new ListEditItem(param[1], 1);
    //    comb.Items.Add(item);
    //    comb.SelectedIndex = 0;}
    //}

    protected void grid_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var result = new Dictionary<string, string>();
        var args = e.Parameters.Split('|');
        if (args[0] == "Matformu")
        {
            var _table = cs.builditems(string.Format("SELECT * from MasMaterial where ID='{0}'", args[1]));
            foreach(DataRow _rw in _table.Rows)
            {
                result["Code"] = _rw["Code"].ToString();
                result["Name"] = string.Format("{0}", _rw["Name"]);
            }
            e.Result = result;
        }
        else
            e.Result = string.Format("{0}", e.Parameters);
    }

    protected void detail_BeforePerformDataSelect(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        int currentUserID = (int)g.GetMasterRowKeyValue();

        g.DataSource = table.Find(u => u.Id == currentUserID).User;
    }

    protected void grid_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
    {
        foreach (var args in e.InsertValues)
        {
            formula row =  new formula();
            int NextRowID = cs.FindMaxValue(table, x => x.Id); ;
            NextRowID++;
            row.Id = NextRowID;
            row.User = GetUser(string.Format("{0}", NextRowID));
            row.Formula = 1;
            row.Batch = 0;
            row.Portion = 0;
            row.NW = 0;
            row.Result = 0;
            table.Add(row);

        }
        foreach (var args in e.UpdateValues)
        {
            int FocusedRowIndex = grid.FocusedRowIndex;
            object keyValue = grid.GetRowValues(FocusedRowIndex, grid.KeyFieldName);
            int Id = Convert.ToInt32(keyValue);
            var dr1 = table.FirstOrDefault(x => x.Id == Id);
            dr1.Component = string.Format("{0}", args.NewValues["Component"]);
            dr1.Description = string.Format("{0}", args.NewValues["Description"]);
            dr1.Yield = string.Format("{0}", args.NewValues["Yield"]);
            dr1.Name = string.Format("{0}", args.NewValues["Name"]);
            //dr1.Result = string.Format("{0}", args.NewValues["Result"]);
            if ((string.Format("{0}", args.NewValues["SubType"]) != ""))
            {

            }
            else
            {
                if (string.Format("{0}", args.NewValues["Result"]) != "")
                {
                    decimal _result = 0;
                    decimal.TryParse(args.NewValues["Result"].ToString(), out _result);
                    dr1.Batch = Convert.ToDecimal(_result * Convert.ToDecimal(tbRevised.Text));
                }
            }
            dr1.NW = Convert.ToDecimal(args.NewValues["NW"]);
            if (string.Format("{0}", args.NewValues["NW"]) != "")
            {

                //=(F20/100)*I$12
                decimal _NW = 0;
                decimal.TryParse(args.NewValues["NW"].ToString(), out _NW);
                dr1.Result = Convert.ToDecimal((_NW / 100) * Convert.ToDecimal(tbFW.Text));

               

                //=E20*$B$15
                dr1.Batch = Convert.ToDecimal(Convert.ToDecimal(dr1.Result) * Convert.ToDecimal(tbRevised.Text));
                foreach (var c in table)
                {
                    //=F20/$F$22*100
                    decimal sumObject = table.Where(s => s.Component.Equals(c.Component)).Select(s => Convert.ToDecimal(s.NW)).Sum();
                   
                    if (sumObject>0)
                    c.Portion = Decimal.Parse(Convert.ToDecimal((c.NW / sumObject) * 100).ToString("f3")); 
                }

            }
            dr1.Material = string.Format("{0}", args.NewValues["Material"]);
            dr1.Formula = 1;
            dr1.Note = string.Format("{0}", args.NewValues["Note"]);
            dr1.IDNumber = string.Format("{0}", args.NewValues["IDNumber"]); 
            
            //dr1.Portion = Convert.ToDecimal(args.NewValues["Portion"]);
            //dr1.Batch = string.Format("{0}", args.NewValues["Batch"]);
            dr1.SubType = string.Format("{0}", args.NewValues["SubType"]);
        }

            e.Handled = true;
    }

    protected void cpUpdateFiles_Callback(object sender, CallbackEventArgsBase e)
    {

    }
    List<User> GetUser(string Id)
    {
        SqlParameter[] param = {new SqlParameter("@Id",Id.ToString()),
                        new SqlParameter("@username",user_name.ToString())};
        DataTable dt = cs.GetRelatedResources("spselectsummary2", param);
        List<User> stdList = new List<User>();
        stdList = (from DataRow dr in dt.Rows
                   select new User()
                   {
                       Id = Convert.ToInt32(dr["Id"]),
                       Material = dr["Material"].ToString(),
                       Yield = dr["Yield"].ToString()
                   }).ToList();
        return stdList;
    }  
    protected void grid_DetailRowGetButtonVisibility(object sender, ASPxGridViewDetailRowButtonEventArgs e)
    {
        formula currentUser = table.Find(u => u.Id == (int)grid.GetRowValues(e.VisibleIndex, "Id"));

        if (!currentUser.HasProjects())
            e.ButtonState = GridViewDetailRowButtonState.Hidden;
    }
}
public class formula
{
    public int Id { get; set; }
    public string Component { get; set; }
    public string SubType { get; set; }
    public string Description { get; set; }
    public string Note { get; set; }
    public string IDNumber { get; set; }
    public string Material { get; set; }
    public decimal Result { get; set; }
    public string Yield { get; set; }
    public string RawMaterial { get; set; }
    public string Name { get; set; }
    public string PriceOfUnit { get; set; }
    public string AdjustPrice { get; set; }
    public string Currency { get; set; }
    public string Unit { get; set; }
    public string ExchangeRate { get; set; }
    public string BaseUnit { get; set; }
    public string PriceOfCarton { get; set; }
    public int Formula { get; set; }
    public string IsActive { get; set; }
    public string Remark { get; set; }
    public string LBOh { get; set; }
    public string LBRate { get; set; }
    public decimal NW { get; set; }
    public decimal Portion { get; set; }
    public decimal Batch { get; set; }
    public string RequestNo { get; set; }
    public List<User> User { get; set; }

    public bool HasProjects()
    {
        return User.Count > 0;
    }
}
public class User
{
    public User()
    {
        Projects = new List<Project>();
    }

    //private string m_fullName;

    public int Id { get; set; }
    public string Material { get; set; }
    public string Yield { get; set; }
    public int parentID { get; set; } 
    //public string FirstName { get; set; }
    //public string LastName { get; set; }
    public List<Project> Projects { get; set; }

    //public string FullName
    //{
    //    get { return string.Format("{0}, {1}", this.LastName, this.FirstName); }
    //}

    public bool HasProjects()
    {
        return Projects.Count > 0;
    }
}
public class Project
{
    public Project()
    {
        Status = ProjectStatus.New;
    }

    public int ID { get; set; }
    public string Name { get; set; }
    public ProjectStatus Status { get; set; }
}
public enum ProjectStatus
{
    New,
    InProgress,
    Failed,
    Done
}