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

public partial class UserControls_EntryEditForm : MasterUserControl{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    private DataTable table
    {
        get { return Session["Customtb"] == null ? null : (DataTable)Session["Customtb"]; }
        set { Session["Customtb"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        SetInitialRow();
    }
    public void SetInitialRow()
    {
        int Folio;
        if (!int.TryParse(Request.QueryString["Folio"], out Folio))
        {
            var pageNameScript = string.Format("<script type='text/javascript'>Folio = '{0}';</script>", Folio);
            Page.Header.Controls.AddAt(0, new LiteralControl(pageNameScript));
        }
        username["user_name"] = user_name;//HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\",@"");
        Update();
    }
    public override void Update()
    {
        //gridData.DataSource = dsgv;
        gridData.DataBind();
    }
    protected void gridData_CustomDataCallback(object sender, DevExpress.Web.ASPxGridViewCustomDataCallbackEventArgs e)
    {
        var args = e.Parameters.Split('|');
        int id;
        if (!int.TryParse(args[2], out id))
            return;
        DataTable dt = new DataTable();
        if (args[1] == "EditDraft" || args[1] == "save")
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spMaterialDetail";
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Connection = con;
                con.Open();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
            }
            foreach (DataRow dr in dt.Rows)
            {
                var result = new Dictionary<string, string>();
                foreach (DataColumn dc in dt.Columns)
                {
                    var name = dc.ToString();
                    result[name] = dr[dc].ToString();
                }
                e.Result = result;
            }
        }
    }
    protected void gridData_Init(object sender, EventArgs e)
    {
        //DataSourceSelectArguments args = new DataSourceSelectArguments();
        //DataView view = (DataView)dsgv.Select(args);
        //DataTable dt = view.ToTable();
        //dsgv.SelectParameters.Clear();
        //dsgv.SelectParameters.Add("user_name", string.Format("{0}",hfusername["user_name"]));
        //int n;
        //n = dt.Rows.Count;
    }

    protected void gridData_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            var item = e.CreateItem("Revised", "Revised");
            item.Image.Url = @"~/Content/Images/Edit.gif";
            e.Items.Add(item);

            item = e.CreateItem("Export", "Export");
            item.BeginGroup = true;
            item.Image.Url = @"~/Content/Images/if_sign-out_59204.png";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), item);
            AddMenuSubItem(item, "PDF", "ExportToPDF", "~/Content/Images/pdf.gif", true);
            AddMenuSubItem(item, "XLS", "ExportToXLS", "~/Content/Images/excel.gif", true);
        }
   }
    private static void AddMenuSubItem(GridViewContextMenuItem parentItem, string text, string name, string iconID, bool isPostBack)
    {
        var exportToXlsItem = parentItem.Items.Add(text, name);
        exportToXlsItem.Image.Url = iconID;
    }

    protected void gridData_ContextMenuItemClick(object sender, ASPxGridViewContextMenuItemClickEventArgs e)
    {
        switch (e.Item.Name)
        {
            case "ExportToPDF":
                GridExporter.WritePdfToResponse();
                break;
            case "ExportToXLS":
                GridExporter.WriteXlsToResponse();
                break;
        }
    }

    protected void CmbDisponsition_Callback(object sender, CallbackEventArgsBase e)
    {

    }

    protected void CmbCompany_Callback(object sender, CallbackEventArgsBase e)
    {

    }
    protected void grid_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //(sender as ASPxGridView).DataSource = LoadGrid;
        g.KeyFieldName = "RowID";
        g.DataSource = LoadGrid;
        g.ForceDataRowType(typeof(DataRow));
    }

    private void AddTextColumn(string fieldName)
    {
        GridViewDataTextColumn c = new GridViewDataTextColumn();
        c.FieldName = fieldName;
        c.Width= new Unit(500, UnitType.Pixel);
        grid.Columns.Add(c);
    }
    private DataTable LoadGrid
    {
        get
        {
            if (table == null)
                return table;
            grid.Columns.Clear();
            //GridViewBandColumn bandColumn = new GridViewBandColumn();
            //bandColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            var values = new[] { "Description", "RowID" };
            foreach (DataColumn c in table.Columns)
            {
                var str = c.ColumnName;
                if (c.ColumnName.Contains("SAPMaterial"))
                {
                    GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                    cb.FieldName = c.ColumnName;
                    cb.PropertiesComboBox.Columns.Clear();
                    cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("RawMaterial"));
                    cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("Description"));
                    cb.PropertiesComboBox.ValueField = "RawMaterial";
                    cb.PropertiesComboBox.TextFormatString = "{0}";
                    cb.PropertiesComboBox.CallbackPageSize = 100;
                    cb.PropertiesComboBox.EnableCallbackMode = true;
                    cb.PropertiesComboBox.IncrementalFilteringDelay = 500;
                    grid.Columns.Add(cb);
                }
                else if (c.ColumnName.Contains("SubType") || c.ColumnName.Contains("Component"))
                {
                    GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                    cb.FieldName = c.ColumnName;
                    //cb.PropertiesComboBox.Columns.Clear();
                    //cb.PropertiesComboBox.TextField = "value";
                    //cb.PropertiesComboBox.TextFormatString = "{0}";
                    grid.Columns.Add(cb);
                }
                else if (values.Any(c.ColumnName.Contains))
                    AddTextColumn(c.ColumnName);
                else
                {
                    GridViewDataSpinEditColumn se = new GridViewDataSpinEditColumn();
                    se.FieldName = c.ColumnName;
                    se.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    //bandColumn.Columns.Add(se);
                    grid.Columns.Add(se);
                }
            }
            //bandColumn.Caption = "(gm./can)";
            //grid.Columns.Add(bandColumn);
            //grid.Columns["Material"].FixedStyle = GridViewColumnFixedStyle.Left;
            grid.KeyFieldName = table.Columns[0].ColumnName;
            grid.Columns[0].Visible = false;
            //grid.SettingsEditing.Mode = GridViewEditingMode.Batch;
            return table;
        }
    }
    void Revised(string keyValue)
    {
        //var args = keyValue.ToString().Split('|');
        var getValue = keyValue.ToString();//args[0];
        if (getValue.ToString() != "0")
            gridData.JSProperties["cpKeyValue"] = (getValue == null) ? string.Empty : getValue.ToString();
    }
    protected void grid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string[] args = e.Parameters.Split('|'); //bool selected = true;
        long id;
        if (!long.TryParse(args[1], out id))
            return;
        DataTable dt = new DataTable();
        switch (args[0])
        {
            case "reload":
                SqlParameter[] param = {new SqlParameter("@key",id.ToString()),
                        new SqlParameter("@user",user_name.ToString())};
                table = cs.GetRelatedResources("spmaterialFormula", param);
                table.PrimaryKey = new DataColumn[] { table.Columns["RowID"] };
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
        if (e.Column.FieldName == "Component" || e.Column.FieldName == "SubType")
        {
            if (table != null)
            {
                // var myResult = dataTable.AsEnumerable()
                //.Select(s => new
                //{
                //    id = s.Field<string>(e.Column.FieldName),
                //})
                //.Distinct().ToList();
                var ValuetoReturn = (from Rows in table.AsEnumerable()
                                     select Rows[e.Column.FieldName]).Distinct().ToList();
                foreach (var name in ValuetoReturn) { Console.WriteLine(name); }
                ValuetoReturn.Add(string.Empty);
                ValuetoReturn.Add("Raw Material");
                ASPxComboBox combo = (ASPxComboBox)e.Editor;
                combo.DataSource = ValuetoReturn.Distinct().OrderBy(q => q);
                combo.DataBindItems();
            }
        }
        var values = new[] { "SAPMaterial", "Component", "SubType" };
        if (values.Any(e.Column.FieldName.Contains))
        {
            ASPxComboBox cb = (ASPxComboBox)e.Editor;
            cb.ItemRequestedByValue += new ListEditItemRequestedByValueEventHandler(Cb_OnItemRequestedByValue_SQL);
            cb.ItemsRequestedByFilterCondition += new ListEditItemsRequestedByFilterConditionEventHandler(Cb_OnItemsRequestedByFilterCondition_SQL);
            ((ASPxComboBox)e.Editor).ClientSideEvents.ValueChanged = "function(s,e){completarArticulo(s); }";
            //if (!g.IsNewRowEditing) ((ASPxComboBox)e.Editor).ClientEnabled = false;
            //cb.DataBind();
        }
    }
    protected void Cb_OnItemsRequestedByFilterCondition_SQL(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
    {
        ASPxComboBox comboBox = (ASPxComboBox)source;
        //IList<Articulo> Lista_Articulos = new ArticuloService().TraerVarios(e.Filter, e.BeginIndex + 1, e.EndIndex + 1);
        //comboBox.DataSource = Lista_Articulos;
        //comboBox.DataBind();
        ASPxComboBox combo = (ASPxComboBox)source;
        dsRawMaterial.SelectParameters.Clear();
        dsRawMaterial.SelectParameters.Add("Company", CmbCompany.Text.Substring(0,3));
        dsRawMaterial.DataBind();
        DataView dvsqllist = (DataView)dsRawMaterial.Select(DataSourceSelectArguments.Empty);
        if (dvsqllist != null)
        {
            int c = dvsqllist.Table.Rows.Count;
            DataTable dt = dvsqllist.Table;
            combo.DataSource = dt;
            combo.DataBind();
        }
    }

    protected void Cb_OnItemRequestedByValue_SQL(object source, ListEditItemRequestedByValueEventArgs e)
    {
        long value = 0;
        if (!Int64.TryParse(e.Value.ToString(), out value))
            return;
        ASPxComboBox comboBox = (ASPxComboBox)source;
        //List<Articulo> Lista_Articulos = new List<Articulo>();
        //Articulo articulo = new ArticuloService().TraerUno(e.Value.ToString());
        //if (articulo != null) Lista_Articulos.Add(articulo);
        //comboBox.DataSource = Lista_Articulos;
        //comboBox.DataBind();
    }


    protected void grid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
    {
        //table = LoadGrid;
        ASPxGridView g = (ASPxGridView)sender;
        //DataRow row = table.NewRow();
        //row["RowID"] = e.NewValues["ID"];
        //row["SubType"] = e.NewValues["SubType"];
        //g.CancelEdit();
        //e.Cancel = true;
        //table.Rows.Add(row);
        e.Cancel = true;
        g.CancelEdit();
    }

    protected void grid_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
    {
        DataTable t = LoadGrid;
        DataRow found = t.Rows.Find(e.Keys[0]);
        foreach (DataColumn column in table.Columns)
        {
            if (!column.ColumnName.Contains("RowID"))
            {
                found[column.ColumnName] = e.NewValues[column.ColumnName];
            }
        }
        ASPxGridView g = sender as ASPxGridView;
        //test(table);
        //calculatecolumn = true;
        g.CancelEdit();
        e.Cancel = true;
    }

     protected void grid_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        e.Result = string.Format("{0}", e.Parameters);
    }

    protected void grid_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
    {

    }
}