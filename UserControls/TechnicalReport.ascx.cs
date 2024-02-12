using DevExpress.Data.Filtering;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using System.IO;
//using DevExpress.DemoData.Model;

public partial class UserControls_TechnicalReport : MasterUserControl
{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    //string CurEditRoles = ConfigurationManager.AppSettings["CurEditRoles"].ToUpper();
    //DataTable _dataSource;
    //DataTable DataSource
    //{
    //    get
    //    {
    //        if (_dataSource == null)
    //        _dataSource = ((DataView)dsgv.Select(DataSourceSelectArguments.Empty)).Table;
    //        return _dataSource;
    //    }
    //    set
    //    {
    //        _dataSource = value;
    //    }
    //}
    private DataTable MyTable
    {
        get { return Session["MyTable"] == null ? null : (DataTable)Session["MyTable"]; }
        set { Session["MyTable"] = value; }
    }
    string _mode 
    {
        get
        { return Session["mode"] == null? Request.QueryString["mode"]: Session["mode"].ToString();}
        set
        {
            Session["mode"] = value;
        }
    }
    string fExpr
    {
        get { return Session["fExpr"] == null ? String.Empty : Session["fExpr"].ToString(); }
        set { Session["fExpr"] = value; }
    }
    //private readonly Type CurrentobjectType;
    protected string SearchText { get { return Utils.GetSearchText(Page); } }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            SetInitialRow();
    }
    public void SetInitialRow()
    {
        Session.Remove("fExpr");
        Session.Remove("MyTable");
        hfKeyword["Keyword"] = string.Format("{0}", "X");
        username["user_name"] = (Request.QueryString["user_name"] == null) ? user_name : Request.QueryString["user_name"].ToString(); ;
        string mode = (Request.QueryString["mode"] == null) ? "4" : Request.QueryString["mode"].ToString();
        string strSQL = string.Format(@"select count(*) from ulogin where isnull(IsResign,0)=0 and
        [user_name]='{0}'", user_name);
        string role = cs.ReadItems(strSQL);
        editor["Name"] = role=="1"?0:1;
        usertp["usertype"] = cs.GetData(user_name, "usertype").ToString();
        //usertp["usertype"] = string.Format("{0}", cs.GetData(user_name, "usertype"));
        //if (mode == "spSummaryCosting") {
        //    editor["Name"] = cs.IsMemberOfRole(string.Format("{0}", 2));
        //}
        //else
        //editor["Name"] = cs.IsMemberOfRole(string.Format("{0}", 0));
        //gridData.DataBind();
        //gridData.FilterExpression = CriteriaOperator.Parse("[StatusApp] = 0").ToString();
        //CreateFilterColumns();
        //filter.FilterExpression = gridData.FilterExpression = "CreateOn > 0";
        //Update();
        DateTime today = DateTime.Today;
        DateTime b = DateTime.Now.AddDays(-30);
        filter.FilterExpression = string.Format("[CreateOn] Between(#{0}#, #{1}#)",
                b.ToString("yyyy-MM-dd"), today.ToString("yyyy-MM-dd"));
    }
    void CreateFilterColumns()
    {
        int maxHierarchyDepth = 1;
        DropDownList ddl = new DropDownList();
        DataView dvsqllist = (DataView)CategoriesDataSource.Select(DataSourceSelectArguments.Empty);
        if (dvsqllist == null) return;
        ddl.DataSource = CategoriesDataSource;
        ddl.DataBind();

        filter.BindToSource(typeof(Product), true, maxHierarchyDepth);
        //var test = filter.Columns["StatusApp"];
        //var categoryColumn = (FilterControlComplexTypeColumn)filter.Columns["StatusApp"];
        FilterControlComplexTypeColumn filterColumn = new FilterControlComplexTypeColumn();
        filterColumn.PropertyName = "test";
        filterColumn.DisplayName = "test";
        filterColumn.Columns.Insert(0, CreateComboBoxColumn());
        //categoryColumn.Columns.Insert(0, CreateComboBoxColumn());

    }
    FilterControlColumn CreateComboBoxColumn()
    {
        var column = new FilterControlComboBoxColumn();
        column.PropertyName = "CategoryName";
        var comboBoxProperties = column.PropertiesEdit as ComboBoxProperties;
        comboBoxProperties.DataSourceID = "CategoriesDataSource";
        comboBoxProperties.TextField = "CategoryName";
        comboBoxProperties.ValueField = "CategoryName";
        comboBoxProperties.ValueType = typeof(string);
        return column;
    }
    public override void Update()
    {
        //dsgv.SelectParameters.Clear();
        //dsgv.SelectParameters.Add("Keyword", string.Format("{0}", fExpr == "" ? "X" : fExpr));
        //dsgv.SelectParameters.Add("UserNo", string.Format("{0}", HttpContext.Current.User.Identity.Name));
        //dsgv.DataBind();
        //gridData.DataBind();
    }
    protected void gridData_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            var item = e.CreateItem("Refresh", "Refresh");
            item.Image.Url = @"~/Content/Images/Refresh.png";
            e.Items.Add(item);
            //item = e.CreateItem("Charts", "Charts");
            //item.Image.Url = @"~/Content/Images/colorful_chart.png";
            //e.Items.Add(item);
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
    //protected void gridData_BeforePerformDataSelect(object sender, EventArgs e)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
    //    StringBuilder sb = new StringBuilder();
    //    string fExpr = ((ASPxGridView)sender).FilterExpression;
    //    if (fExpr != "")
    //    {
            //    foreach (GridViewColumn column in g.Columns)
            //    {
            //        var name = (column as GridViewDataColumn).FieldName;
            //        GridViewDataColumn col = g.Columns[name] as GridViewDataColumn;
            //        string filter = col.FilterExpression;
            //        string filterValue = string.Empty;
            //        CriteriaOperator criteria = CriteriaOperator.Parse(filter);
            //        // You can also check for other criteria operator types
            //        if (criteria is BinaryOperator)
            //        {
            //            sb.Append((sb.Length == 0 ? "where " : "and") + criteria.ToString());
            //            filterValue = ((criteria as BinaryOperator).RightOperand as OperandValue).Value.ToString();
            //        }
            //    }
            //    dsgv.SelectParameters.Clear();
            //    dsgv.SelectParameters.Add("Keyword", string.Format("{0}", sb.ToString()));
            //    dsgv.DataBind();
    //    }
    //}
    protected void gridData_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        var myresult = (DataTable)g.DataSource;
        var values = new[] { "Code", "ID","Formula", "RmIng" };
        if (myresult!=null)
            g.Columns[0].Visible = (string.Format("{0}", editor["Name"]) == "0");
        foreach (GridViewColumn column in gridData.Columns)
        {
            GridViewDataColumn idCol = column as GridViewDataColumn;
            //if (column is GridViewDataColumn)
            if (values.Any(idCol.FieldName.Contains))
                    idCol.Visible = false;
        }
        SetItemCount();
    }
    protected void PreRender(object sender, EventArgs e)
    {
        SetItemCount();
    }
    protected void BeforeGetCallbackResult(object sender, EventArgs e)
    {
        SetItemCount();
    }
    void SetItemCount()
    {
        //DataTable dt = (DataTable)gridData.DataSource;
        //if (dt == null) return;
        //var i = dt.Rows.Count;
        //if (MyTable == null) return;
        //int itemCount = (int)gridData.GetTotalSummaryValue(gridData.TotalSummary["RequestNo"]);
        //gridData.SettingsPager.Summary.Text = "Page {0} of {1} (" + itemCount.ToString() + " items)";
    }
    object GetDataSource(int count)
    {
        List<object> result = new List<object>();
        for (int i = 0; i < count; i++)
            result.Add(new { ID = i, City = "City_" + i });
        return result;
    }
    string test()
    {
        StringBuilder sb = new StringBuilder();
        foreach (GridViewColumn column in gridData.Columns)
        {
            var name = (column as GridViewDataColumn).FieldName;
            GridViewDataColumn col = gridData.Columns[name] as GridViewDataColumn;
            string filter = col.FilterExpression;
            string filterValue = string.Empty;
            CriteriaOperator criteria = CriteriaOperator.Parse(filter);
            // You can also check for other criteria operator types
            if (criteria is BinaryOperator)
            {
                sb.Append((sb.Length == 0 ? "where " : "and") + criteria.ToString());
                filterValue = ((criteria as BinaryOperator).RightOperand as OperandValue).Value.ToString();
            }
        }
        return string.Format("{0}",sb.ToString());  
        //dsgv.SelectParameters.Clear();
        //dsgv.SelectParameters.Add("Keyword", string.Format("{0}", fExpr == "" ? "X" : fExpr));
        //dsgv.SelectParameters.Add("UserNo", string.Format("{0}", HttpContext.Current.User.Identity.Name));
        //dsgv.DataBind();
        //Update();
    }

    //protected void dsgv_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
    //{
    //    SqlDataSource ods = sender as SqlDataSource;
    //    test();
    //    e.Command.Parameters[0].Value = string.Format("{0}", fExpr==""?"X": fExpr);
    //    e.Command.Parameters[1].Value = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    //}

    //protected void gridData_ProcessColumnAutoFilter(object sender, ASPxGridViewAutoFilterEventArgs e)
    //{
    //    if (e.Column.FieldName == "CreateOn" || e.Column.FieldName == "Company")
    //    {
    //        FilterDataSourceByDateColumns(e);
    //    }
    //}

    //private void FilterDataSourceByDateColumns(ASPxGridViewAutoFilterEventArgs e)
    //{
    //    CriteriaOperator newCriteriaOperator = UpdateCriteria(e.Criteria);
    //    string filterExpression = DevExpress.Data.Filtering.CriteriaToWhereClauseHelper.GetDataSetWhere(newCriteriaOperator);
    //    DataSource = DataSource.Select(filterExpression).CopyToDataTable<DataRow>(); ;
    //}

    //private CriteriaOperator UpdateCriteria(CriteriaOperator criteriaOperator)
    //{
    //    CriteriaOperator newCriteriaOperator = CriteriaOperator.Clone(criteriaOperator);

    //    string[] DateFieldsToChange = new string[] {
    //        "CreateOn",
    //        "Company",
    //    };

    //    foreach (string dateField in DateFieldsToChange)
    //    {
    //        // Update filter criteria here
    //    }

    //    return newCriteriaOperator;
    //}

    //protected void gridData_DataBinding(object sender, EventArgs e)
    //{
    //    ((ASPxGridView)sender).DataSource = DataSource;
    //}

    // void gridData_AfterPerformCallback(object sender, ASPxGridViewAfterPerformCallbackEventArgs e)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
        //fExpr = ((ASPxGridView)sender).FilterExpression;
        //if (g.FilterExpression != ""){
        //   string fExpr= test(); 
        //    gridData.JSProperties["cpFilterExpression"] = g.FilterExpression;}

            //g.JSProperties.Add("cpIsCustomCallback", null);
            //g.JSProperties["cpIsCustomCallback"] = e.CallbackName == "CUSTOMCALLBACK";}
    //}
    protected void gridData_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        var g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        //try
        //{
        //    if (string.IsNullOrEmpty(_mode)) return;
        //}
        //catch (Exception ex)
        //{
        //    throw ex;
        //}
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
                string fxExpr = "";
                foreach (GridViewColumn column in g.Columns)
                {
                    if (column is GridViewDataColumn)
                    {
                        if (fxExpr == "")
                        {
                            fxExpr = "[" + (column as GridViewDataColumn).FieldName + "] Like '%" + filter + "%'";
                        }
                        else
                        {
                            fxExpr = fxExpr + "OR " + "[" + (column as GridViewDataColumn).FieldName + "] Like '%" + filter + "%'";
                        }
                    }
                }
                g.FilterExpression = fxExpr;
            }
        }
        if (args[0] == "filter")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(filter.GetFilterExpressionForMsSql());
            fExpr = sb.ToString();
            //g.JSProperties["cpFilterExpression"] = sb.ToString();
            MyTable = new DataTable();
            _mode = (Request.QueryString["mode"] == null) ? "spSummaryReport" : Request.QueryString["mode"].ToString();
            SqlParameter[] param = { new SqlParameter("Keyword", fExpr == "" ? "X" : fExpr),
             new SqlParameter("UserNo", username["user_name"].ToString())};
            MyTable = cs.GetRelatedResources(_mode, param);

            if (_mode == "spSummaryCosting" || _mode == "spSummaryCustomer")
            {
                ServiceCS service = new ServiceCS();
                MyTable = service.PostDataSap(MyTable);
                if (_mode == "spSummaryCustomer")
                {
                    List<string> list = new List<string>();
                    DataTable dt = MyTable.DefaultView.ToTable(true, "RequestNo");
                    foreach (DataRow _r in dt.Rows)
                    {
                        list.Add(_r["RequestNo"].ToString());
                    }
                    List<string> distinct = list.Distinct().ToList();
                    string strRequestNo = String.Join(",", distinct.ToArray());
                    dt = new DataTable();
                    dt = BuildSec(strRequestNo);
                    if (dt.Rows.Count > 0)
                    {
                        var values = new[] { "requestno", "Formula" };
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (!values.Any(column.ColumnName.Contains))
                            {
                                MyTable.Columns.Add(column.ColumnName, typeof(System.String));
                            }
                        }
                        foreach (DataRow _rw in MyTable.Rows)
                        {
                            string strSQL = string.Format("requestno='{0}' and Formula = '{1}'", _rw["Requestno"], _rw["Formula"]);
                            DataRow _result = dt.Select(strSQL).FirstOrDefault();
                            if (_result != null)
                            {
                                foreach (DataColumn column in dt.Columns)
                                    _rw[column.ColumnName] = string.Format("{0}", _result[column.ColumnName]);
                            }
                        }
                    }
                }
            }
        }
        //if (args[0] == "filter")
        //    test();
        UpdateDataViewFilterExpression();
        //Update();
        //g.DataBind();
    }
    DataTable BuildSec(string value)
    {
        SqlParameter[] param = { new SqlParameter("ID", value),
             new SqlParameter("UserNo", username["user_name"].ToString())};
        DataTable dt = cs.GetRelatedResources("spSummaryCostSec", param);
        return dt;
    }

    protected void UpdateDataViewFilterExpression()
    {
        UpdateDataSourceFilterExpression();
        gridData.DataBind();
    }
    protected void UpdateDataSourceFilterExpression()
    {
        if (filter.IsFilterExpressionValid())
        {
            string filterExpression = filter.GetFilterExpressionForAccess();
            ASPxLabel1.Text = filterExpression;
            //if (!AccessDataSource1.SelectCommand.Contains(filterExpression))
            //    AccessDataSource1.SelectCommand = string.Format("{0} WHERE {1}", AccessDataSource1.SelectCommand, filterExpression);
        }
        else
            ASPxLabel1.Text = "Filter expression is not valid";
        if (string.IsNullOrEmpty(ASPxLabel1.Text))
            ASPxLabel1.Text = "Filter expression is empty";
    }
    //protected void gridData_DataBinding(object sender, EventArgs e)
    //{
    //    ((ASPxGridView)sender).DataSource = DataSource;
    //}
    //void Filter(object sender)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
    //    if (g.FilterExpression != "")
    //        g.JSProperties["cpFilterExpression"] = g.FilterExpression;
    //}

    protected void gridData_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //(sender as ASPxGridView).DataSource = LoadGrid;
        g.KeyFieldName = "ID";
        g.DataSource = LoadGrid;
        g.ForceDataRowType(typeof(DataRow));
        AddColumns();
    }
    private DataTable LoadGrid
    {
        get
        {
             if (MyTable == null)
                return MyTable;
            //Session["CustomTable"] = view.Table;
            //return view.Table;
            var view = MyTable.DefaultView;
            CreateGridColumns(view);
            return MyTable;
        }
    }
    void CreateGridColumns(DataView view)
    {
        var table = view.Table;
        gridData.Columns.Clear();
        //gridData.TotalSummary.Clear();
        var values = new[] { "Received", "Started","Completed", "CreateOn", "SendMKT", "R&D","SendCostMKT", "Marketing","Form","To", 
            "ActionDate", "AssignPIC", "RDMGRsendAC", "PICtoRDMGR" };
        var dataname = new[] { "RequestNo","CostingNo","MaterialCode", "Customer", "ProductName", "CanSize", "Country", "CostNo" };
        foreach (DataColumn c in table.Columns)
        {
            //gridData.Columns.Remove(gridData.Columns[c.ColumnName]);
            GridViewDataColumn gridColumn = gridData.Columns[c.ColumnName] as GridViewDataColumn;
            if (c.ColumnName.Contains("MarketingName") || c.ColumnName.Contains("By") || c.ColumnName.Equals("RD") || c.ColumnName.Equals("RDM"))
            {
                GridViewDataComboBoxColumn CBCol = new GridViewDataComboBoxColumn();
                CBCol.FieldName = c.ColumnName;
                CBCol.PropertiesComboBox.Columns.Clear();
                CBCol.PropertiesComboBox.Columns.Add(new ListBoxColumn("user_name"));
                CBCol.PropertiesComboBox.Columns.Add(new ListBoxColumn("fn"));
                CBCol.PropertiesComboBox.ValueField = "user_name";
                CBCol.PropertiesComboBox.TextFormatString = "{1}";
                CBCol.PropertiesComboBox.EnableCallbackMode = true;
                CBCol.PropertiesComboBox.CallbackPageSize = 10;
                CBCol.PropertiesComboBox.DataSource = dsuser;
                gridData.Columns.Add(CBCol);
                gridData.Columns[c.ColumnName].Width = Unit.Pixel(180);
            }
            else if (c.ColumnName.Contains("CD") || c.ColumnName.Contains("PMC")|| c.ColumnName.Contains("Name") || c.ColumnName.Contains("Requester")) {
                GridViewDataTokenBoxColumn tb = new GridViewDataTokenBoxColumn();
                tb.FieldName = c.ColumnName;
                tb.PropertiesTokenBox.ValueField = "user_name";
                tb.PropertiesTokenBox.TextField="fn";
                tb.PropertiesTokenBox.DataSource = dsuser;
                gridData.Columns.Add(tb);
                gridData.Columns[c.ColumnName].Width = Unit.Pixel(180);
            }
            else if (c.ColumnName.Contains("Customer") || c.ColumnName.Contains("ShipTo"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.ValueField = "Code";
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.DataSource = dsCustomer;
                gridData.Columns.Add(cb);
            }
            else if (c.ColumnName.Equals("StatusApp"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.ValueField = "Id";
                cb.PropertiesComboBox.TextField = "Title";
                cb.PropertiesComboBox.DataSource = dsStatusApp;
                gridData.Columns.Add(cb);
            }
            //else if (c.ColumnName.Contains("Received") || c.ColumnName.Contains("Started") || c.ColumnName.Contains("Completed") || c.ColumnName.Contains("CreateOn"))
            else if(values.Any(c.ColumnName.Contains))
            {
                GridViewDataDateColumn dt = new GridViewDataDateColumn();
                dt.FieldName = c.ColumnName;
                dt.PropertiesDateEdit.DisplayFormatString= "dd/MM/yyyy HH:mm:ss";
                gridData.Columns.Add(dt);
                gridData.Columns[c.ColumnName].Width = Unit.Pixel(150);
            }
            else if (dataname.Any(c.ColumnName.Contains))
            {
                GridViewDataColumn d = new GridViewDataColumn();
                d.FieldName = c.ColumnName;
                gridData.Columns.Add(d);
                gridData.Columns[c.ColumnName].Width = Unit.Pixel(150);
            }
            else 
                AddTextColumn(c.ColumnName);
        }
        gridData.KeyFieldName = table.Columns["ID"].ColumnName;
        gridData.Columns["ID"].Visible = false;
    }
    private void AddTextColumn(string fieldName)
    {
        GridViewDataTextColumn c = new GridViewDataTextColumn();
        c.FieldName = fieldName;
        gridData.Columns.Add(c);
        gridData.Columns[fieldName].Width = Unit.Pixel(70);
    }
    private void AddColumns()
    {
        //gridData.Columns[0].Visible = false;
    }
    protected void gridData_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
    {
        if (e.Column.FieldName == "StatusApp")
        {
            ASPxComboBox combo = (ASPxComboBox)e.Editor;
            combo.DataBind();
        }
    }

    protected void gridData_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e)
    {
        if (e.Column.FieldName == "Fishprice" || e.Column.FieldName == "UpCharge")
        {
            if (e.Value == null) return;
            string str = e.Value.ToString();
            double number;
            Double.TryParse(str, out number);
            e.DisplayText = string.Format("{0:F4}", number);
        }
    }

    protected void btn_Click(object sender, EventArgs e)
    {
        //GridExporter.WriteXlsToResponse();
        PrintingSystemBase ps = new PrintingSystemBase();
        PrintableComponentLinkBase lnk = new PrintableComponentLinkBase(ps);
        lnk.Component = GridExporter;

        CompositeLinkBase compositeLink = new CompositeLinkBase(ps);
        compositeLink.Links.AddRange(new object[] { lnk });
        compositeLink.CreateDocument();

        MemoryStream stream = new MemoryStream();
        compositeLink.PrintingSystemBase.ExportToXls(stream);
        WriteToResponse(gridData.ID, true, "xls", stream);
    }
    protected void WriteToResponse(string fileName, bool saveAsFile, string fileFormat, MemoryStream stream)
    {
        if (Page == null || Page.Response == null) return;
        string disposition = saveAsFile ? "attachment" : "inline";
        Page.Response.Clear();
        Page.Response.Buffer = false;
        Page.Response.AppendHeader("Content-Type", string.Format("application/{0}", fileFormat));
        Page.Response.AppendHeader("Content-Transfer-Encoding", "binary");
        Page.Response.AppendHeader("Content-Disposition", string.Format("{0}; filename={1}.{2}", disposition, HttpUtility.UrlEncode(fileName).Replace("+", "%20"), fileFormat));
        Page.Response.BinaryWrite(stream.ToArray());
        Page.Response.End();
    }
}