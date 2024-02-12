using ClosedXML.Excel;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using DevExpress.Web;
using DevExpress.Web.ASPxSpreadsheet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_UploadcostForm : MasterUserControl
{
    MyDataModule cs = new MyDataModule();
    ServiceCS myservice = new ServiceCS();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    const string NotFoundMessageFormat = "<h1>Can't find message with the key={0}</h1>";
    SelectAllCheckboxEdit check = new SelectAllCheckboxEdit();
	protected string SearchText { get { return Utils.GetSearchText(Page); } }
    private DataTable _dataTable
    {
        get
        {
            object o = Page.Session["dt"];
            if (o == null) return null;
            return (DataTable)o;
        }
        set
        {
            Page.Session["dt"] = value;
        }
    }
    string fExpr
    {
        get { return Session["fExpr"] == null ? String.Empty : Session["fExpr"].ToString(); }
        set { Session["fExpr"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            SetInitialRow();
    }

    public override void Update()
    {
        gridData.DataBind();
    }

    public void SetInitialRow()
    {
        hfGeID["GeID"] = string.Format("{0}", 0);
        hfStatusApp["StatusApp"] = string.Format("{0}", 0);
        hfBU["BU"] = string.Format("{0}", cs.GetData(user_name, "bu"));
        hfgetvalue["NewID"] = string.Format("{0}", cs.GetNewID());
        hfuser["user_name"] = user_name.ToString();
        editor["Name"] = cs.IsMemberOfRole(string.Format("{0}", 0));
        hftablename["tablename"] = "TransTechnical";
        hfRequestType["Type"] = string.Format("{0}", "");
        usertp["usertype"] = cs.GetData(user_name, "usertype").ToString();
        hftype["type"] = string.Format("{0}", 0);
        approv["approv"] = string.Format("{0}", "");
		hfRole["Role"] = string.Format("{0}", cs.CheckRole());
        Initialfilter();
        Update();
    }
    void Initialfilter()
    {
        DateTime today = DateTime.Today;
        DateTime b = DateTime.Now.AddYears(-1);
        //filter.FilterExpression = string.Format("[CreateOn] Between(#{0}#, #{1}#)",
        //        b.ToString("yyyy-MM-dd"), today.ToString("yyyy-MM-dd"));
        filter.FilterExpression = string.Format("RequestType in ({0})", 1);
        StringBuilder sb = new StringBuilder();
        sb.Append(filter.GetFilterExpressionForMsSql());
        fExpr = sb.ToString();
        hfKeyword["Keyword"] = fExpr;
    }
    protected void CmbDisponsition_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
    {

    }
    string BuildUserType(string type)
    {
        string result = "";
        if (type.Contains("1") && CmbCompanyEdit.Value.ToString() == "102")
            result = "1";
        else if (type.Contains("2") && CmbCompanyEdit.Value.ToString() == "101")
        {
            result = "2";
        }
        else
            result = type;
        return result;
    }
    string GetCount()
    {
        if (_dataTable == null) return "0";
        return _dataTable.Rows.Count > 0 ? "2" : "0";
    }
    string getuserType()
    {
        string value = usertp["usertype"].ToString() == "0" ? "0" : "1";
        return value;
    }
    void postdata(long[] keys)
    {
        //ASPxGridView g = glAssignee.GridView;
        //      StringBuilder sb = new StringBuilder();
        //      if (!string.IsNullOrEmpty(glAssignee.Text)){
        //          String r = g.GetRowValues(g.FocusedRowIndex, new String[] { "user_name" }).ToString();
        //              sb.Append(r);
        //          }
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spInsertRequestDes";
            cmd.Parameters.AddWithValue("@ID", keys[0].ToString());
            cmd.Parameters.AddWithValue("@user", hfuser["user_name"]);
            cmd.Parameters.AddWithValue("@Company", string.Format("{0}", CmbCompanyEdit.Value));
            cmd.Parameters.AddWithValue("@RequestNo", string.Format("{0}", tbRequestNoEdit.Text));
            cmd.Parameters.AddWithValue("@Remark", tbRemarkEdit.Text);
            cmd.Parameters.AddWithValue("@From", deValidfromEdit.Value);
            cmd.Parameters.AddWithValue("@To", deValidtoEdit.Value);
            cmd.Parameters.AddWithValue("@Assignee", GetAssignee(glAssignee.GridView, glAssignee.Text).ToString());
            cmd.Parameters.AddWithValue("@StatusApp", string.Format("{0}", GetCount()));
            cmd.Parameters.AddWithValue("@NewID", string.Format("{0}", hfgetvalue["NewID"]));
            cmd.Parameters.AddWithValue("@Destination", CmbCustomPriceEdit.Value);
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                var _status = "";
                if (dr["StatusApp"].ToString() == "2")
                {
                    List<string> listFolio = new List<string>();
                    listFolio.Add(dr["ID"].ToString());
                    testdata(listFolio);
                    if (_dataTable.Rows.Count > 0)
                        _status = dr["StatusApp"].ToString();
                }
                else
                    _status = dr["StatusApp"].ToString();
                if (!string.IsNullOrEmpty(_status) && _status != "4")
                    _alertmail(dr, string.Format("{0}", _status));
            }
            con.Close();
        }
    }
    void _alertmail(DataRow dr, string status)
    {
        DataTable dt = new DataTable();
        SqlParameter[] param = {new SqlParameter("@Id",dr["ID"].ToString()),
                        new SqlParameter("@User",hfuser["user_name"]),
                        new SqlParameter("@StatusApp",status),
                        new SqlParameter("@table",string.Format("{0}", "TransTechnical")),
                        new SqlParameter("@AppStatus","")};
        dt = cs.GetRelatedResources("spsender", param);
        foreach (DataRow row in dt.Rows)
            if (string.Format("{0}", dr["RequestNo"].ToString().Substring(3, 1)) == "1")
            {
                List<string> myList = new List<string>();
                List<string> list = new List<string>();
                List<string> listsender = new List<string>();
                string[] split = row["MailTo"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split)
                {
                    if (s.Trim() != "")
                    {
                        list.Add(cs.GetData(s, "email"));
                        listsender.Add(cs.GetData(s, "fn"));
                        myList.Add(s);
                    }
                }
                string sender = String.Join(",", list.ToArray());
                list = new List<string>();
                split = row["MailCc"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split)
                {
                    if (s.Trim() != "")
                        if (!myList.Contains(s))
                            list.Add(cs.GetData(s, "email"));
                }
                string form = (dr["RequestNo"].ToString().Substring(3, 1) == "1") ? "Edit" : "EditDraft";
                string MailCc = String.Join(",", list.ToArray());
                //string subject = string.Format(_text, cs.GetSubject(usertp["usertype"].ToString()), dr["RequestNo"].ToString(), "");
                string subject = string.Format(@"Request No.: {0}" , dr["RequestNo"].ToString());
                string _link = "Request No.: " + dr["RequestNo"].ToString();
                //_link += "<br/> Customer Name : " + tbCustomerEdit.Text;
                string ass = string.Format("{0}", GetAssignee(glAssignee.GridView, glAssignee.Text));//CmbAssignee
                _link += "<br/> Assignee : " + cs.GetData(ass, "fn");
                _link += string.Format("<br/> Valid from: {0} To {1}", dr["Form"], dr["To"]);
                _link += "<br/> Request By : " + cs.GetData(hfuser["user_name"].ToString(), "fn");
                _link += "<br/> Sender : " + cs.GetData(user_name.ToString(), "fn");
                _link += "<br/> The document link --------><a href=" + cs.GetSettingValue();
                _link += @"/Default.aspx?viewMode=RequestDesForm&ID=" + dr["ID"].ToString() + "&form=" + form + "&UserType=" + getuserType();
                _link += " style='color: rgb(0,255,0)'><font color='#0000FF'>click</font></a>";
                cs.sendemail(sender, MailCc, _link, subject);
            }
    }
    string GetAssignee(ASPxGridView g, string s)
    {
        string result = "";
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(s))
        {
            List<object> fieldValues = g.GetSelectedFieldValues(new string[] { "user_name" });
            if (fieldValues.Count > 0)
                foreach (object item in fieldValues)
                    sb.Append(item.ToString() + ",");
        }
        if (!string.IsNullOrEmpty(sb.ToString()))
        {
            result = sb.ToString();
            result = result.Substring(0, (result.Length - 1));
        }
        return result;
    }

    protected bool TryParseKeyValues(IEnumerable<string> stringKeys, out long[] resultKeys)
    {
        resultKeys = null;
        var list = new List<long>();
        foreach (var sKey in stringKeys)
        {
            long key;
            if (!long.TryParse(sKey, out key))
                return false;
            list.Add(key);
        }
        resultKeys = list.ToArray();
        return true;
    }

    protected void gridData_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        long[] keys = null;
        if (!TryParseKeyValues(args.Skip(1), out keys))
            return;
        if (args[0] == "Edit" || args[0] == "unread" || args[0] == "Post")
            postdata(keys);
        if (args[0] == "Delete" && args.Length > 1)
        {
            Delete(keys);
        }
		if (args[0] == "Pending" || args[0] == "All")
        {
            dsgv.DataBind();
        }
		if (args[0] == "Search")
        {
            string filter = args[0] == "Search" ? SearchText : string.Empty;
            //gridData.SearchPanelFilter = filter;
            //gridData.ExpandAll();
            if (filter == "")
            {
                gridData.FilterExpression = "";
            }
            else
            {
                string fExpr = "";
                foreach (GridViewColumn column in gridData.Columns)
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
                gridData.FilterExpression = fExpr;
            }
        }
        g.DataBind();
    }
    protected void gridData_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            if (string.Format("{0}", hfRole["Role"]) == "0")
            {
                var item = e.CreateItem("Export", "ExportToXLS");
                item.Image.Url = @"~/Content/Images/if_sign-out_59204.png";
                e.Items.Add(item);

                GridViewContextMenuItem assign = e.CreateItem("Assign", "Assign");
                assign.Image.Url = @"~/Content/Images/icons8-plus-16.png";
                e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), assign);
            }
        }
    }
    void Delete(long[] keys)
    {
        if (hfStatusApp["StatusApp"].ToString() == "2")
        {
            string strSQL = string.Format(@"update TransTechnical set Statusapp=-1 where ID={0}; select ID," +
                    "RequestNo,StatusApp,format(RequestDate, 'dd-MM-yyy') as 'Form'," +
                    "format(RequireDate, 'dd-MM-yyy') as 'To' from TransTechnical where ID = {0}", keys[0].ToString());
            DataTable dt = cs.builditems(strSQL);
            foreach (DataRow dr in dt.Rows)
                _alertmail(dr, "-1");
        }
        else
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                //cmd.CommandText = "Update TransTechnical set StatusApp=5 where ID=@ID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spDelTechnical";
                cmd.Parameters.AddWithValue("@ID", keys[0].ToString());
                cmd.Parameters.AddWithValue("@user", hfuser["user_name"].ToString());
                cmd.Parameters.AddWithValue("@StatusApp", "-1");
                cmd.Parameters.AddWithValue("@tablename", string.Format("{0}", "TransTechnical"));
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
    }
    protected void gridData_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        var args = e.Parameters.Split('|');
        long id;
        if (!long.TryParse(args[2], out id))
            return;
        var result = new Dictionary<string, string>();
        DataTable dt = new DataTable();
        if (args[1] == "print")
        {
            result["print"] = "0";
            result["view"] = "0";
            result["KeyValue"] = String.Format("{0}", id);
            e.Result = result;
        }
        if (args[1] == "ExportToXLS")
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spEditPriceReport";
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", id));
                cmd.Connection = con;
                con.Open();
                var getValue = cmd.ExecuteScalar();
                con.Close();
                con.Dispose();
                if (getValue.ToString() != "")
                    result["print"] = (getValue == null) ? string.Empty : getValue.ToString();
                var View = tbRequestNoEdit.Text != "" ? tbRequestNoEdit.Text.Substring(3, 1) : "1";
                result["view"] = String.Format("{0}", View);
                result["KeyValue"] = String.Format("{0}", id);
                e.Result = result;
            }
        }
        if (args[1] == "read" || args[1] == "unread" || args[1] == "new")
        {

            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetNewIDRequest";
                cmd.Connection = con;
                con.Open();
                var getValue = cmd.ExecuteScalar();
                con.Close();
                result["NewID"] = (string)getValue;
                result["RequestType"] = (string)args[1];
                result["editor"] = String.Format("{0}", 0);
                e.Result = result;
            }
        }
        if (args[1] == "Edit")
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spselectRequestDes";
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@username", hfuser["user_name"]);
                cmd.Connection = con;
                con.Open();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
            }
            foreach (DataRow dr in dt.Rows)
            {
				//string strSQL = string.Format("select Assignee from TransTechnical where Id={0}", id);
    //            dtassign = cs.builditems(strSQL);

                result["ID"] = dr["ID"].ToString();
                result["RequestNo"] = dr["RequestNo"].ToString();
                result["Remark"] = dr["Remark"].ToString();
                result["Company"] = dr["Company"].ToString();
                //result["RequestType"] = dr["RequestType"].ToString();
                result["Validfrom"] = dr["Validfrom"].ToString();
                result["Validto"] = dr["Validto"].ToString();
                result["StatusApp"] = dr["StatusApp"].ToString();
                result["CreateOn"] = dr["CreateOn"].ToString();
                result["editor"] = String.Format("{0}", dr["editor"]);
                List<string> list = new List<string>();
                string[] split = dr["assignee"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split)
                {
                    if (s.Trim() != "")
                        list.Add(cs.GetData(s, "fn"));
                }
                result["assignee"] = String.Join(",", list.ToArray()); //string.Format("{0}", dr["assignee"]);
                result["assigneeID"] = String.Format("{0}", dr["assignee"]);
                result["NewID"] = dr["UniqueColumn"].ToString();
                result["Destination"] = dr["Destination"].ToString();
                //result["Customer"] = dr["Customer"].ToString();
                //result["UserType"] = dr["UserType"].ToString();
                e.Result = result;
            }
        }
    }
    private DataTable dtassign
    {
        get { return Page.Session["dtassign"] == null ? null : (DataTable)Page.Session["dtassign"]; }
        set { Page.Session["dtassign"] = value; }
    }
    protected void glAssignee_DataBound(object sender, EventArgs e)
    {
        ASPxGridLookup lookup = (ASPxGridLookup)(sender);
        if(dtassign!=null)
        foreach(DataRow r in dtassign.Rows)
        {
            var arr = r["Assignee"].ToString().Split(',');
            for (int i = 0; i < arr.Length; i++)
            lookup.GridView.Selection.SelectRowByKey(arr[i]);
        }
    }
    const string sessionKey = "CE6907BD-E867-4cbf-97E2-F1EB702F433";
    public string ActivePageSymbol
    {
        get
        {
            if (Session[sessionKey] == null)
                Session[sessionKey] = string.Format("{0}", 1);
            return (string)Session[sessionKey];
        }
        set { Session[sessionKey] = value; }
    }
    protected void Grid_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string editor = cs.IsMemberOfRole(string.Format("{0}", 2));
        if (string.Format("{0}", editor) == "0")
        {
            if (g.Columns.IndexOf(g.Columns["CommandColumn"]) != -1)
                return;
            GridViewCommandColumn col = new GridViewCommandColumn();
            col.Name = "CommandColumn";
            col.ShowEditButton = true;
            col.ShowNewButtonInHeader = true;
            col.ButtonRenderMode = GridCommandButtonRenderMode.Image;
            GridViewCommandColumnCustomButton but = new GridViewCommandColumnCustomButton();
            but.ID = "EditCost";
            but.Text = "Upload";
            but.Image.ToolTip = "Update Costing";
            but.Image.IconID = "navigation_up_16x16";

            col.CustomButtons.Add(but);
            g.Columns.Add(col);

            GridViewCommandColumn colsel = new GridViewCommandColumn();
            colsel.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            colsel.Width = Unit.Pixel(45);
            colsel.Name = "Command";
            colsel.ShowSelectCheckbox = true;
            colsel.HeaderCaptionTemplate = check;
            colsel.VisibleIndex = 0;
            g.Columns.Add(colsel);
        }
        int rowCnt = g.VisibleRowCount;
        int pageSize = g.SettingsPager.PageSize;
        if (rowCnt != 0 && rowCnt != pageSize)
        {
            if (_dataTable != null)
            {
                // if (g.VisibleRowCount > 5)
                //	g.Settings.VerticalScrollableHeight = 220;
                //else
                //	g.Settings.VerticalScrollableHeight = g.VisibleRowCount * 23;
                //string editor = cs.IsMemberOfRole(string.Format("{0}", 2));
                //g.Columns[0].Visible = (string.Format("{0}", editor) == "0");
                //g.Columns[1].Visible = (string.Format("{0}", editor) == "0");

                //for (int i = 0; i < g.VisibleRowCount; i++)
                //{
                //    var id = g.GetRowValues(i, g.KeyFieldName).ToString();
                //    DataRow found = _dataTable.AsEnumerable()
                //                    .SingleOrDefault(r => r.Field<int>("ID") == Convert.ToInt32(id));
                //    foreach (DataRow dr in cs.builditems("select Result from TransEditCosting where ID=" + id).Rows)
                //        found["Result"] = dr["Result"];
                //}
            }
        }
        g.FilterExpression = "[Series] = '" + ActivePageSymbol + "'";
    }
    string FilePath
    {
        get { return Page.Session["sessionFile"] == null ? String.Empty : Page.Session["sessionFile"].ToString(); }
        set { Page.Session["sessionFile"] = value; }
    }
    protected void Upload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
    {
        //string dirVirtualPath = Server.MapPath("~/XlsTables/");
        //string dirVirtualPath = @"\\192.168.1.193\XlsTables";
        string dirVirtualPath = @"C:\\temp";
        string dirPhysicalPath = dirVirtualPath;
        if (!Directory.Exists(dirPhysicalPath))
        {
            Directory.CreateDirectory(dirPhysicalPath);
        }
        ActivePageSymbol = "1".ToString();
        string fileName = e.UploadedFile.FileName;
        FilePath = Path.Combine(dirPhysicalPath, fileName);
        e.UploadedFile.SaveAs(FilePath);
        if (!string.IsNullOrEmpty(FilePath))
        {
            //Workbook book = new Workbook();
            //book.InvalidFormatException += book_InvalidFormatException;
            //book.LoadDocument(FilePath);
            ASPxSpreadsheet spreadsheet = new ASPxSpreadsheet();
            spreadsheet.Document.LoadDocument(FilePath);
            spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;
            int NextRowID = 0; int i = 0;
            _dataTable.Rows.Clear();
            foreach (Worksheet sheet in spreadsheet.Document.Worksheets)
            {
                i++;
                CellRange range = sheet.GetUsedRange();
                DataTable table = sheet.CreateDataTable(range, false);
                DataTableExporter exporter = sheet.CreateDataTableExporter(range, table, false);
                exporter.CellValueConversionError += exporter_CellValueConversionError;
                exporter.Export();
                string VarietyPack = "";
                foreach (DataRow dr in table.Rows)
                {
                    //dataTable = table;
                    double num;
                    string value = dr["Column2"].ToString().Trim();
                    if (double.TryParse(dr["Column1"].ToString(), out num))
                        if (!string.IsNullOrEmpty(value))
                        {
                            DataRow rw = _dataTable.NewRow();
                            object[] array = new object[11];
                            rw["ID"] = NextRowID++;
                            rw["RowID"] = num.ToString();
                            if (value.Length == 18 && value.Substring(0, 1) == "3")
                            {
                                rw["Code"] = dr["Column2"];
                                SqlParameter[] param = new SqlParameter[] {
                                        new SqlParameter("@param", string.Format("{0}", value)) };
                                DataTable myresult = cs.GetRelatedResources("spgetselectcost", param);
                                foreach (DataRow row in myresult.Rows)
                                    rw["CostingSheet"] = string.Format("{0}", row["MarketingNumber"]);
                            }
                            else
                                if (GetExcelUserType(value.Substring(0, 2)) != "0")
                                rw["CostingSheet"] = string.Format("{0}", value);
                            else if (getuserType() != "0")
                                rw["CostingSheet"] = string.Format("{0}", value);
                            rw["Result"] = string.Format("{0}", 0);
                            rw["Series"] = i;
                            if (table.Columns.Contains("Column3"))
                            {
                                rw["SellingUnit"] = string.Format("{0}", dr["Column4"]).ToString();
                                if (dr["Column3"].ToString() != "")
                                {
                                    rw["VarietyPack"] = string.Format("{0}", dr["Column3"]).ToString();
                                    rw["Units"] = string.Format("{0}", dr["Column5"]).ToString();
                                    rw["TotalPack"] = string.Format("{0}", dr["Column6"]).ToString();
                                    rw["PackingStyle"] = string.Format("{0}", dr["Column7"]).ToString();

                                    VarietyPack = string.Format("{0}", dr["Column3"]).ToString();

                                }
                                else
                                {
                                    rw["VarietyPack"] = string.Format("{0}", VarietyPack).ToString();
                                }
                            }
                            //if (!string.IsNullOrEmpty(string.Format("{0}", array[2])) || !string.IsNullOrEmpty(string.Format("{0}", array[3])))
                            _dataTable.Rows.Add(rw);

                        }
                }
            }
            //Worksheet sheet = book.Worksheets.ActiveWorksheet;
            //Range range = sheet.GetUsedRange();
            //DataTable table = sheet.CreateDataTable(range, false);
            //DataTableExporter exporter = sheet.CreateDataTableExporter(range, table, false);
            //exporter.CellValueConversionError += exporter_CellValueConversionError;
            //exporter.Export();
        }
    }
    string GetExcelUserType(string value)
    {
        return cs.ReadItems("select count(ID)ID from Masplant where NamingCode='" + value + "'");
    }
    void book_InvalidFormatException(object sender, SpreadsheetInvalidFormatExceptionEventArgs e)
    {

    }
    void exporter_CellValueConversionError(object sender, CellValueConversionErrorEventArgs e)
    {
        e.Action = DataTableExporterAction.Continue;
        e.DataTableValue = null;
    }

    protected void Grid_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
    {
        var values = new[] { "ID", "RequestNo" };
        foreach (var args in e.InsertValues)
        {
            int NextRowID = Convert.ToInt32(_dataTable.AsEnumerable()
            .Max(row => row["ID"]));
            DataRow rw = _dataTable.NewRow();

            NextRowID++;
            rw["ID"] = NextRowID;
            //rw["RowID"] = rw["ID"];
            foreach (DataColumn column in _dataTable.Columns)
            {
                if (!values.Any(column.ColumnName.Contains))
                {
                    switch (column.ColumnName)
                    {
                        case "Mark":
                            rw[column.ColumnName] = "X";
                            break;
                        case "Series":
                            rw[column.ColumnName] = ActivePageSymbol == null ? "0" : ActivePageSymbol;
                            break;
                        case "SiteId":
                            rw[column.ColumnName] = "0";
                            break;
                        default:
                            rw[column.ColumnName] = args.NewValues[column.ColumnName];
                            break;
                    }
                }
            }
            _dataTable.Rows.Add(rw);
        }
        foreach (var args in e.UpdateValues)
        {
            var arr = new[] { "ID", "RequestNo", "Series", "SiteId" };
            DataRow dr = _dataTable.Rows.Find(args.Keys["ID"]);
            foreach (DataColumn column in _dataTable.Columns)
            {
                if (!arr.Any(column.ColumnName.Contains))
                {
                    dr[column.ColumnName] = args.NewValues[column.ColumnName];
                }
            }
        }
        e.Handled = true;
    }

    protected void Grid_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
    {
        GridViewCommandColumn C = e.Column as GridViewCommandColumn;
        //string editor = cs.IsMemberOfRole(string.Format("{0}", 2));
        //C.Visible = (string.Format("{0}", editor) == "0");
    }
    protected void Grid_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.KeyFieldName = "ID";
        g.DataSource = _dataTable;
        g.ForceDataRowType(typeof(DataRow));
    }
    //private DataTable LoadGrid(ASPxGridView g)
    //{
    //    if (_dataTable == null)
    //        _reload();
    //    //string edit = cs.IsMemberOfRole(string.Format("{0}", 2));
    //    if (_dataTable != null)// && edit != "0")
    //    {
    //        int max = Convert.ToInt32(_dataTable.AsEnumerable()
    //                        .Max(row => row["Series"]));
    //        g.Templates.PagerBar = new MyPagerBarTemplate(ActivePageSymbol, max);
    //        g.SettingsBehavior.AllowSelectByRowClick = false;
    //        g.SettingsDataSecurity.AllowEdit = true;
    //        //string edit = cs.IsMemberOfRole(string.Format("{0}", 2));
    //        //Grid.Columns[0].Visible = (string.Format("{0}", edit) == "0");
    //    }
    //    return _dataTable;
    //}
    protected void btn_Click(object sender, EventArgs e)
    {
        //ExportExcel();
        using (XLWorkbook wb = new XLWorkbook())
        {
            wb.Worksheets.Add(_dataTable, "Customers");

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=SqlExport.xlsx");
            using (MemoryStream MyMemoryStream = new MemoryStream())
            {
                wb.SaveAs(MyMemoryStream);
                MyMemoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
    }
    protected void Grid_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //dataTable
        var values = new[] { "Code", "CostingSheet" };

        DataRow dr = _dataTable.Rows.Find(e.Keys[0]);
        var r = string.Format("{0}", dr["SiteId"]);
        if (r == "" || string.Format("{0}", dr["SiteId"]) == "0")
        {
            foreach (DataColumn column in _dataTable.Columns)
            {
                if (!column.ColumnName.Contains("ID"))
                {
                    switch (column.ColumnName)
                    {
                        case "Series":
                            dr[column.ColumnName] = ActivePageSymbol == null ? "0" : ActivePageSymbol;
                            break;
                        case "SiteId":
                            dr[column.ColumnName] = "0";
                            break;
                        default:
                            if (values.Any(column.ColumnName.Contains))
                                dr[column.ColumnName] = e.NewValues[column.ColumnName];
                            break;
                    }
                }
            }
            //update
            buildchange(dr);
        }
        g.CancelEdit();
        e.Cancel = true;
    }
    void buildchange(DataRow dr)
    {
        List<string> listFolio = new List<string>();
        listFolio.Add(dr["RequestNo"].ToString());
        updateEditCosting(dr, listFolio);
    }
    protected void Grid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        DataRow row = _dataTable.NewRow();
        int NextRowID = _dataTable.Rows.Count;
        NextRowID++;
        row["ID"] = NextRowID;
        row["RequestNo"] = _dataTable.Rows[0]["RequestNo"];
        foreach (DataColumn column in _dataTable.Columns)
        {
            if (!column.ColumnName.Contains("ID"))
            {
                switch (column.ColumnName)
                {
                    case "Mark":
                        row[column.ColumnName] = "X";
                        break;
                    case "Series":
                        row[column.ColumnName] = ActivePageSymbol == null ? "0" : ActivePageSymbol;
                        break;
                    case "SiteId":
                        row[column.ColumnName] = "0";
                        break;
                    //case "Unit":
                    //    row[column.ColumnName] = "KG";
                    //    break;
                    default:
                        row[column.ColumnName] = e.NewValues[column.ColumnName];
                        break;
                }
            }
        }
        //insert 
        buildchange(row);
        _dataTable.Rows.InsertAt(row, 0);
        g.CancelEdit();
        e.Cancel = true;
    }
    public DataTable GetRelatedResources(string StoredProcedure, object[] Parameters)
    {
        var Results = new DataTable();
        try
        {
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = new SqlCommand(StoredProcedure, conn))
                {
                    conn.Open();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(Parameters);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(Results);
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write("Exception Executing Stored Procedure:" + ex.Message);
        }

        return Results;
    }
    private DataTable GetGridData()
    {
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[9] { new DataColumn("ID", typeof(int)),
                            new DataColumn("Material", typeof(string)),
                            new DataColumn("CostingSheet", typeof(string)),
                            new DataColumn("Result", typeof(string)),
                            new DataColumn("Series",typeof(int)),
        new DataColumn("Mark",typeof(string)),
        new DataColumn("StatusApp",typeof(string)),
        new DataColumn("SiteId",typeof(string)),
        new DataColumn("RequestNo",typeof(string))});
 
        dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
        return dt;
    }

    void testdata(List<string> myresult)
    {
        //deleteEditCosting(myresult[0].ToString());
        foreach (DataRow dr in _dataTable.Rows)
        {
            updateEditCosting(dr, myresult);
        }
    }
    void updateEditCosting(DataRow dr, List<string> myresult)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spInsertRequestDesItems";
            cmd.Parameters.AddWithValue("@ID", string.Format("{0}", dr["ID"]));
            cmd.Parameters.AddWithValue("@RequestNo", string.Format("{0}", myresult[0]));
            cmd.Parameters.AddWithValue("@CostingSheet", string.Format("{0}", dr["CostingSheet"]));
            cmd.Parameters.AddWithValue("@Material", string.Format("{0}", dr["Material"]).Trim());
            cmd.Parameters.AddWithValue("@Series", string.Format("{0}", dr["Series"]));
            cmd.Parameters.AddWithValue("@Result", string.Format("{0}", dr["Result"]));
            cmd.Parameters.AddWithValue("@user", string.Format("{0}", hfuser["user_name"]));
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    void deleteEditCosting(string myfolio)
    {
        //using (SqlConnection con = new SqlConnection(strConn))
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.CommandText = "spdeleteEditCosting";
        //    cmd.Parameters.AddWithValue("@Id", myfolio.ToString());
        //    cmd.Parameters.AddWithValue("@user", hfuser["user_name"]);
        //    cmd.Connection = con;
        //    con.Open();
        //    cmd.ExecuteNonQuery();
        //    con.Close();
        //    con.Dispose();
        //}
    }
    void _reload()
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            string Keys = hfGeID["GeID"].ToString();
            dsresult.SelectParameters.Clear();
            dsresult.SelectParameters.Add("ID", string.Format("{0}", string.Format("{0}", Keys)));
            dsresult.SelectParameters.Add("user", string.Format("{0}", hfuser["user_name"]));
            dsresult.DataBind();
            DataTable table = new DataTable();
            DataView dvsqllist = (DataView)dsresult.Select(DataSourceSelectArguments.Empty);
            if (dvsqllist != null)
            {
                table = dvsqllist.Table;
                _dataTable = table;
                _dataTable.PrimaryKey = new DataColumn[] { _dataTable.Columns["ID"] };
            }
            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandText = "spGetEditCosting";
            //cmd.Parameters.AddWithValue("@ID", param[1]);
            //cmd.Parameters.AddWithValue("@user", hfuser["user_name"]);
            //cmd.Connection = con;
            //con.Open();
            //SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            //oAdapter.Fill(table);
            //con.Close();
            //con.Dispose();
            //dataTable = table;
            //dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["ID"] };
            //Session["CustomTable"] = dataTable;
        }
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
    protected void Grid_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
    {
        var values = new[] { "Code", "CostingSheet", "Result" };
        //if (values.Any(e.DataColumn.FieldName.Contains))
        if (e.DataColumn.FieldName == "StatusApp")
        {
            var g = sender as ASPxGridView;
            DataRow row = g.GetDataRow(e.VisibleIndex);
            int index = e.VisibleIndex;
            //e.Cell.ForeColor = Color.Black;
            if (g.VisibleRowCount != 0 && row != null)
            {
                if (g.GetRowValues(e.VisibleIndex, "Mark").ToString() == "X")
                    e.Cell.BackColor = Color.LightGreen;
                else
                    e.Cell.BackColor = Color.Coral;
            }
        }
        if (hfStatusApp["StatusApp"].ToString() != "2" && approv["approv"].ToString() != "0")
            if (values.Any(e.DataColumn.FieldName.Contains))
                e.Cell.Attributes.Add("onclick", "event.cancelBubble = true");
    }
    protected void Grid_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            var item = e.CreateItem("Remove", "Remove");
            item.Image.Url = @"~/Content/Images/Remove.gif";
            item.Image.Width = Unit.Percentage(18);
            e.Items.Add(item);

            item = e.CreateItem("Export", "Export");
            item.BeginGroup = true;
            item.Image.Url = @"~/Content/Images/if_sign-out_59204.png";
            //e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Refresh), item);
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), item);
            AddMenuSubItem(item, "PDF", "PDF", "~/Content/Images/pdf.gif", true);
            AddMenuSubItem(item, "XLS", "XLS", "~/Content/Images/excel.gif", true);
        }
    }
    private static void AddMenuSubItem(GridViewContextMenuItem parentItem, string text, string name, string iconID, bool isPostBack)
    {
        var exportToXlsItem = parentItem.Items.Add(text, name);
        exportToXlsItem.Image.Url = iconID;
    }
    protected void Grid_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var result = new Dictionary<string, string>();
        var args = e.Parameters.Split('|');
        if (args[0] == "CostingSheet")
        {
            var table = new DataTable();
            table = cs.builditems(string.Format(@"select * from TransRequestForm where ID={0}", args[1]));
            foreach (DataRow rw in table.Rows)
            {
                result["Code"] = string.Format("3{0}{1}{2}{3}{4}{5}{6}{7}00", rw["ProductGroup"], rw["RawMaterial"], rw["StyleofPack"], rw["MediaType"],
                        rw["NW"], rw["ContainerLid"], rw["Grade"], rw["Zone"]);
                //result["CostNo"] = dr["CostNo"].ToString();
            }
            e.Result = result;
        }
    }
    protected void Grid_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //bool isOddRow = e.VisibleIndex % 2 == 0;
        string Result = string.Format("{0}", g.GetRowValues(e.VisibleIndex, "Result"));
        bool isOddRow = string.Format("{0}", Result) != "0" && !string.IsNullOrEmpty(Result);
        if (e.ButtonID == "EditCost" && isOddRow)
            e.Image.Url = "~/Content/Images/Edit.gif";
    }
    protected void gridData_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //bool isOddRow = e.VisibleIndex % 2 == 0;
        bool isOddRow = string.Format("{0}", g.GetRowValues(e.VisibleIndex, "RequestType")) == "1";
        //var  type=  g.GetRowValues(e.VisibleIndex, "RequestType");
        if (e.ButtonID == "btnDetails" && isOddRow)
        {
            e.Image.Url = "~/Content/Images/if_stock_zoom-object_21726.png";
            e.Image.ToolTip = "Display";
        }
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
        string result = string.Empty;
        var itemCount = gridData.GetTotalSummaryValue(gridData.TotalSummary["RequestNo"]);
        if (itemCount == null)
        {
            result = "0";
        }
        else
        {
            result = itemCount.ToString();
        }
        gridData.SettingsPager.Summary.Text = "Page {0} of {1} (" + result + " items)";
    }
    object GetDataSource(int count)
    {
        List<object> result = new List<object>();
        for (int i = 0; i < count; i++)
            result.Add(new { ID = i, City = "City_" + i });
        return result;
    }
    protected void Grid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string[] param = e.Parameters.Split('|'); //bool selected = true;
        switch (param[0])
        {
            case "AddRow":
                _dataTable = GetGridData();
                break;
            case "symbol":
                ActivePageSymbol = param[1].ToString();
                //_dataTable = (DataTable)g.DataSource;
                break;
            case "reload":
                hfGeID["GeID"] = string.Format("{0}", param[1]);
                _reload();
                break;
            case "calcu":
                string test = param[1];
                break;
        }
        string editor = cs.IsMemberOfRole(string.Format("{0}", 2));
        //if (_dataTable != null && editor != "0")
        //{
        //    int max = Convert.ToInt32(_dataTable.AsEnumerable()
        //   .Max(row => row["Series"]));
        //    g.AutoFilterByColumn(g.Columns["Series"], ActivePageSymbol);
        //    g.Templates.PagerBar = new MyPagerBarTemplate(ActivePageSymbol, max);
        //}
        g.DataBind();
    }
}
class SelectAllCheckboxEdit : ITemplate
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
        //box.Init += cbAll_Init;
        box.ClientSideEvents.CheckedChanged = "OnAllCheckedChanged";
        container.Controls.Add(box);
    }
    //protected void cbAll_Init(object sender, EventArgs e)
    //{
    //    ASPxCheckBox chk = sender as ASPxCheckBox;
    //    ASPxGridView grid = (chk.NamingContainer as GridViewHeaderTemplateContainer).Grid;
    //    chk.Checked = (grid.Selection.Count == grid.VisibleRowCount);
    //}
}
public class updateItems
{

}