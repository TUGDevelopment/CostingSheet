using DevExpress.Data;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using DevExpress.Web;
using DevExpress.Web.Data;
//using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
//using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using DevExpress.Web.ASPxTreeList;
//using System.Collections.ObjectModel;
using System.Text;
//using System.Web.UI.HtmlControls;
using WebApplication;
using System.Collections.Specialized;
using DevExpress.Web.ASPxSpreadsheet;

public partial class UserControls_CostingEditForm : MasterUserControl
{
    MyDataModule cs = new MyDataModule();
    ServiceCS myservice = new ServiceCS();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    SelectAllCheckbox check = new SelectAllCheckbox();
    const string PreviewFormat =
        "<div class='MailPreview'>" +
            "<div class='Subject'></div>" +
            "<div class='Info'>" +
                "<div>From: {0}</div>" +
                "<div>{1}: {2}</div>" +
                "<div>Date: {3:g}</div>" +
            "</div>" +
            "<div class='Separator'></div>" +
            "<div class='Body'>{4}</div>" +
        "</div>",
    ReplyFormat = "Hi,<br/><br/><br/><br/>Thanks,<br/>Thomas Hardy<br/><br/><br/>----- Original Message -----<br/>Subject: {0}<br/>From: {1}<br/>To: {2}<br/>Date: {3:g}<br/>{4}",
    NotFoundFormat = "<h1>Can't find message with the key={0}</h1>";
    const string sessionKey = "CE6907BD-E867-4cbf-97E2-F1EB702F433";

    HttpContext context = HttpContext.Current;
    //string sessionFile, seGetMyData, CustomTable, sessionlist; 
    string FilePath
    {
        get { return Page.Session["sessionFile"] == null ? String.Empty : Page.Session["sessionFile"].ToString(); }
        set { Page.Session["sessionFile"] = value; }
    }
    private DataTable _dt
    {
        get { return Page.Session["seGetMyData"] == null ? null : (DataTable)Page.Session["seGetMyData"]; }
        set { Page.Session["seGetMyData"] = value; }
    }
    private DataTable _dataTable
    {
        get { return Page.Session["CustomTable"] == null ? null : (DataTable)Page.Session["CustomTable"]; }
        set { Page.Session["CustomTable"] = value; }
    }
    public DataTable tcustomer
    {
        get { return Page.Session["tcustomer"] == null ? null : (DataTable)Page.Session["tcustomer"]; }
        set { Page.Session["tcustomer"] = value; }
    }
    public List<TransCosting> listItems
    {
        get
        {
            var obj = this.Session["myList"];
            if (obj == null) { obj = this.Session["myList"] = new List<TransCosting>(); }
            return (List<TransCosting>)obj;
        }

        set
        {
            this.Session["myList"] = value;
        }
    }
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    string[] SubType = Regex.Split("raw material;ingredient;primary packaging;secondary packaging", ";");
    string[] arrSubType = Regex.Split("0;Primary Packaging;Secondary Packaging;Margin;Upcharge;Product Detail;Attached file", ";");
    List<string> listFolio;
    const string InsertRowsSessionKey = "InsertRows";
    const string PageSizeSessionKey = "ed5e843d-cff7-47a7-815e-832923f7fb09";
    //List<dynamic> data;

    //private DataTable Tablelist
    //{
    //    get { return context.Session["sessionlist"] == null ? null : (DataTable)context.Session["sessionlist"]; }
    //    set { context.Session["sessionlist"] = value; }
    //}
    //private List<string> _myList;
    //public ReadOnlyCollection<string> PublicReadOnlyList { get { return _myList.AsReadOnly(); } }
    //private List<string> _myList;
    //public ReadOnlyCollection<string> PublicReadOnlyList { get { return _myList.AsReadOnly(); } }
    protected string SearchText { get { return Utils.GetSearchText(Page); } }
    protected void Page_Load(object sender, EventArgs e)
    {
        //formLayout.FindItemOrGroupByName("Uploadfile").Visible = false;
        if (!IsPostBack)
        {
            SetInitialRow();
        }

    }
    //protected void Page_Init(object sender, EventArgs e) { 
    //CreateGrid();
    //}
    //
    void buildFindULevel()
    {
        SqlParameter[] param = { new SqlParameter("@user_name", (string)cs.CurUserName) };
        var Results = cs.GetRelatedResources("spGetFindULevel", param);
        foreach(DataRow rw in Results.Rows)
        hfsublevel["sublevel"] = string.Format("{0}", rw["Sublevel"]);
    }
    void ubuild()
    {
        usertp["usertype"] = string.Format("{0}", cs.GetData(user_name, "usertype"));
    }
    public DataTable _CurrentTableCol
    {
        get { return Page.Session["col"] == null ? null : (DataTable)Page.Session["col"]; }
        set { Page.Session["col"] = value; }
    }
    public void SetInitRoot()
    {
        Session["DataSource"] = DataHelper.Init();
    }
    public void SetInitialRow()
    {
        //string userSession = Guid.NewGuid().ToString();
        //Session["UserSession"] = userSession;
        Session.Remove("sessionKey");
        //Session.Remove("seGetMyData");
        SetInitRoot();
        hfgetvalue["NewID"] = string.Format("{0}", cs.GetNewID());
        hfid["hidden_value"] = string.Empty;
        hfFolio["Folio"] = string.Empty;
        hflit["hflit"] = string.Empty;
        hftype["type"] = string.Format("{0}", 0);
        hfRequestNo["ID"] = string.Format("{0}", 0);
        hfuser["user_name"] = user_name;
        hftablename["tablename"] = "TransCostingHeader";
        ubuild();
        buildFindULevel();
        approv["approv"] = string.Format("{0}", "");
        editor["Name"] = cs.IsMemberOfRole(string.Format("{0}", 2));
        hfStatusApp["StatusApp"] = string.Format("{0}", 0);
        Session["BU"] = string.Format("{0}", cs.GetData(user_name, "bu"));
        _CurrentTableCol = GetCurrentTable();
        
        Update();
    }
    protected void grid_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
    {
        ASPxTextBox editor = e.Editor as ASPxTextBox;
        if (editor != null)
        {
            editor.ClientSideEvents.Init = "function(s, e)  {  s.ValueChanged.ClearHandlers(); s.KeyDown.ClearHandlers();   s.KeyDown.AddHandler( function(s, e) {  if(e.htmlEvent.keyCode ==13) ASPxClientUtils.PreventEventAndBubble(e.htmlEvent); } ) ;  } ";
            editor.ClientInstanceName = "filterRow_" + e.Column.FieldName;
        }
    }
    DataTable GetCurrentTable()
    {
        DataTable dt = new DataTable();
        dt.Clear();
        dt.Columns.Add("Name");
        dt.Columns.Add("Marks");
        dt.Rows.Add(new object[] { "ID;Component;SubType;Description;SAPMaterial;GUnit;Yield;RawMaterial;Name;PriceOfUnit;Currency;Unit;ExchangeRate;BaseUnit;PriceOfCarton;Remark;LBOh;LBRate;Formula", "0" });
        dt.Rows.Add(new object[] { "ID;SubType;Code;Name;Quantity;PriceUnit;Amount;Per;ExchangeRate;Currency;Loss;Formula", "1;2;3;4;5;6;7;8" });
        dt.Rows.Add(new object[] { "ID;SubType;Code;Name;Quantity;PriceUnit;Amount;Per;Currency;Loss;Formula", "10" });
        dt.Rows.Add(new object[] { "ID;Code;Name;RefSamples;NetWeight;Packaging;FixedFillWeight;PackSize;Formula;RequestForm", "9" });
        return dt;
    }
    protected void CmbCountry_Callback(object sender, CallbackEventArgsBase e)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlParameter[] param = { new SqlParameter("@group", (string)hfRequestNo["ID"]) };
            var Results = cs.GetRelatedResources("spGetCountry", param);
            //CmbCountry.Columns.Clear();
            //CmbCountry.Columns.Add(new ListBoxColumn("Code"));
            //CmbCountry.Columns.Add(new ListBoxColumn("Name"));
            //CmbCountry.Columns.Add(new ListBoxColumn("Zone"));
            CmbCountry.DataSource = Results;
            CmbCountry.DataBind();
        }
    }

    public override void Update()
    {
        //gridData.DataSource = dsgv;
        //DataTable dt = ((DataView)dsgv.Select(DataSourceSelectArguments.Empty)).Table;
        gridData.DataBind();
    }
    private DataTable GetDataView(ASPxGridView grid)
    {
        DataTable dt = new DataTable();
        foreach (GridViewColumn col in grid.VisibleColumns)
        {
            GridViewDataColumn dataColumn = col as GridViewDataColumn;
            if (dataColumn == null) continue;
            dt.Columns.Add(dataColumn.FieldName);
        }
        for (int i = 0; i < grid.VisibleRowCount; i++)
        {
            DataRow row = dt.Rows.Add();
            foreach (DataColumn col in dt.Columns)
                row[col.ColumnName] = grid.GetRowValues(i, col.ColumnName);
        }
        return dt;
    }
    void copyCosting(string[] args)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spcopyCosting";
            cmd.Parameters.AddWithValue("@Id", string.Format("{0}", args[2]));
            cmd.Parameters.AddWithValue("@Requester", user_name.ToString());
            cmd.Parameters.AddWithValue("@UserType", string.Format("{0}", usertp["usertype"]));
            //cmd.Parameters.AddWithValue("@CostingNo", string.Format("{0}", args[1]));
            cmd.Connection = con;
            con.Open();
            var getValue = cmd.ExecuteScalar();
            con.Close();
            con.Dispose();
            gridData.JSProperties["cpKeyValue"] = (getValue == null) ? string.Empty : getValue.ToString();
        }
    }
    string buildExchangeRate(string ID)
    {
        string exchange = cs.ReadItems(@"select ExchangeRate from TransCostingHeader where ID='"
                    + ID.ToString() + "'");
        return exchange;
    }
    protected void gridData_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        if (args[0] == "Action")
        {
            for (int i = 0; i < g.VisibleRowCount; i++)
            {
                var sel = g.Selection.IsRowSelected(i);
                if (sel)
                {
                    object SubID = g.GetRowValues(i, g.KeyFieldName);
                    object value = g.GetRowValues(i, "StatusApp");
                    using (SqlConnection con = new SqlConnection(strConn))
                    {
                        //ApproveStep
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spApproveStep";
                        cmd.Parameters.AddWithValue("@Id", SubID.ToString().Split('|')[0]);
                        cmd.Parameters.AddWithValue("@User", user_name.ToString());
                        cmd.Parameters.AddWithValue("@StatusApp", value.ToString() == "0" && args[1] == "Approve" ? "4" : value.ToString());
                        cmd.Parameters.AddWithValue("@table", "TransCostingHeader");
                        cmd.Parameters.AddWithValue("@remark", "");
                        cmd.Parameters.AddWithValue("@Assign", "");
                        cmd.Parameters.AddWithValue("@reason", "");
                        //cmd.Connection = con;
                        //con.Open();
                        //cmd.ExecuteNonQuery();
                        cmd.Connection = con;
                        con.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                        oAdapter.Fill(dt);
                        foreach (DataRow dr in dt.Rows)
                        {
                            List<string> myList = new List<string>();
                            string name = dr["NamingCode"].ToString();
                            List<string> list = new List<string>();
                            List<string> lstValue = new List<string>();
                            List<string> lstSend = new List<string>();
                            string[] split = dr["MailTo"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in split)
                            {
                                if (s.Trim() != "")
                                {
                                    if (!myList.Contains(s))
                                    {
                                        list.Add(cs.GetData(s, "email"));
                                        lstSend.Add(cs.GetData(s, "fn"));
                                        myList.Add(s);
                                    }
                                }
                            }
                            //string statusapp = " was " + CmbDisponsition.Text;
                            string MailTo = String.Join(",", list.ToArray()); //string MailCc = "";
                            list = new List<string>();
                            split = dr["MailCc"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in split)
                            {
                                if (s.Trim() != "")
                                    if (!myList.Contains(s))
                                    {
                                        list.Add(cs.GetData(s, "email"));
                                        myList.Add(s);
                                    }
                            }
                            string MailCc = String.Join(",", list.ToArray());
                            string _link = string.Format("Dear {0} <br/> ", String.Join(",", lstSend.ToArray()));
                            _link += "<br/> Costing No.:" + dr["RequestNo"].ToString();
                            _link += "<br/> Request No.:" + dr["CreateBy"].ToString();
                            _link += "<br/> Create By : " + cs.GetData(dr["Requester"].ToString(), "fn");
                            _link += "<br/> Customer Name : " + g.GetRowValues(i, "Customer").ToString();
                            _link += "<br/> Sender : " + cs.GetData(user_name.ToString(), "fn");
                            _link += "<br/> Comment :";
                            _link += @"<br/> The document link --------><a href=" + cs.GetSettingValue() + "/Default.aspx?viewMode=CostingEditForm&ID="
                                + SubID.ToString() + "&UserType=" + usertp["usertype"];
                            _link += " style='color: rgb(0,255,0)'><font color='#0000FF'>click</font></a>";
                            string subject = string.Format(@"{0}:{1}:{2} / Request No.: " + dr["CreateBy"].ToString() + " V.{3} ,"
                            + g.GetRowValues(i, "Customer").ToString() + "/", name.ToString(), dr["Subject"], dr["RequestNo"].ToString(), string.Format("{0:00}", dr["Revised"]));
                            cs.sendemail(MailTo, MailCc, _link, subject);
                        }
                        con.Close();
                    }
                }
            }
        }
        if (args[0] == "Pending" || args[0] == "All")
        {
            dsgv.DataBind();
        }
        if (args[0] == "Request")
        {
            //if (g.Selection.Count > 1)
            //{
            //    //Obtain key field values of the selected rows
            //    List<object> selectedRowKeys = g.GetSelectedFieldValues("ID");
            //    for (int i = 0; i < selectedRowKeys.Count; i++)
            //    {

            //    }
            //}
            List<string> myList = new List<string>();
            List<string> lists = new List<string>();
            List<Object> selectItems = g.GetSelectedFieldValues("ID");
            foreach (object selectItemId in selectItems)
            {
                //var a = selectItemId;
                //table.Rows.Remove(table.Rows.Find(selectItemId));
            
            List<string> listsSubID = new List<string>();
                //for (int i = 0; i < g.VisibleRowCount; i++)
                //{
                //    var sel = g.Selection.IsRowSelected(i);
                //    if (sel)
                    {
                    object SubID = selectItemId;//g.GetRowValues(i, g.KeyFieldName);
                        //object value = g.GetRowValues(i, "RequestNo");
                        //listsSubID.Add(SubID.ToString());
                        //if (!myList.Contains(value.ToString()))
                        //    myList.Add(value.ToString());

                        SqlParameter[] param = { new SqlParameter("@Id", string.Format("{0}", SubID).Split(';')[0]) };
                        DataTable dt = cs.GetRelatedResources("spGetCostnoRequestForm", param);
                        foreach (DataRow rw in dt.Rows)
                        {

                        hfid["hidden_value"] = string.Format("{0}", rw["RequestNo"]);
                            using (SqlConnection con = new SqlConnection(strConn))
                            {
                                SqlCommand cmd = new SqlCommand();
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandText = "spInsertDocumentNo";
                                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", 0));
                                cmd.Parameters.AddWithValue("@Requester", cs.CurUserName.ToString());
                                cmd.Parameters.AddWithValue("@RequestNo", string.Format("{0}", rw["RequestNo"]));
                                cmd.Parameters.AddWithValue("@Costno", string.Format("{0}", SubID));
                                cmd.Parameters.AddWithValue("@Assignee", string.Format("{0}", CmbReceiver.Value));
                            
                                cmd.Parameters.AddWithValue("@Destination", string.Format("{0}", CmbCountry.Value));
                                cmd.Parameters.AddWithValue("@Country", string.Format("{0}", textBoxCountry.Text));
                                cmd.Connection = con;
                                con.Open();
                                var getValue = cmd.ExecuteScalar();
                                con.Close();
                                lists.Add(string.Format("{0}", getValue));
                            }
                        }
                    }
                //}
                    //ASPxCheckBox chk = g.FindRowCellTemplateControl(i, null, "cbAll") as ASPxCheckBox;
                    //if (chk.Checked)
                    //{
                    //    bool isSelected = Convert.ToBoolean(g.GetRowValues(i, "ID"));
                    //    object SubID = g.GetRowValues(i, g.KeyFieldName);
                    //    var sel = g.Selection.IsRowSelected(i);
                    //}
                }
            buildsendemail(lists, string.Format("{0}", hfid["hidden_value"]));
            //if (myList.Count() > 1)
            //{

            //}
            //else {
            //g.JSProperties["cpKeyValue"] = "success";
            //}

        }
        if (args[0] == "SaveMail" || args[0] == "SendMail")
        {
            //Main(new String[] { "Jesper" });
            string myfolio = null;
            int max = Convert.ToInt32(_dataTable.AsEnumerable()
            .Max(row => row["Formula"]));
            if (hfStatusApp["StatusApp"].ToString() == "0")
            {
                double d;
                string exchange = buildExchangeRate(args[1].ToString());
                if (double.TryParse(exchange, out d))
                    if (Double.Parse(seExchangeRate.Value.ToString()) != d)
                    {
                        var valuechange = (Double.Parse(seExchangeRate.Value.ToString()) > d) ? 0 : 1;
                        myfolio = revised(valuechange.ToString(), args[1], seExchangeRate.Text); goto jumpexit;
                    }
                for (int i = 1; i <= max; i++)
                {
                    DataRow[] result = _dataTable.Select("SubType = 'Margin' and Formula='" + i.ToString() + "'");
                    foreach (DataRow row in result)
                    {
                        string per = cs.ReadItems("select Per from TransCosting where ID='" + row["ID"].ToString() + "'");
                        if (double.TryParse(per, out d))
                            if (Double.Parse(row["Per"].ToString()) != d)
                            {
                                var valuechange = (Double.Parse(row["Per"].ToString()) > d) ? 0 : 1;
                                myfolio = revised(valuechange.ToString(), args[1], seExchangeRate.Text); goto jumpexit;
                                //using (SqlConnection con = new SqlConnection(strConn))
                                //{
                                //    SqlCommand cmd = new SqlCommand();
                                //    cmd.CommandType = CommandType.StoredProcedure;
                                //    cmd.CommandText = "spcopyCostingRevised";
                                //    cmd.Parameters.AddWithValue("@Id", hfFolio["Folio"].ToString());
                                //    cmd.Parameters.AddWithValue("@Requester", user_name.ToString());
                                //    cmd.Parameters.AddWithValue("@Per", valuechange.ToString());
                                //    cmd.Connection = con;
                                //    con.Open();
                                //    var getValue = cmd.ExecuteScalar();
                                //    con.Close();
                                //    myfolio = getValue.ToString();
                                //}
                                //Revised = 1;
                                //break;
                            }
                    }
                }
            }
        jumpexit:
            savedata(myfolio == null ? args[1] : myfolio);
        }
        if (args[0] == "Revised")
        {
            var dt = cs.builditems(string.Format(@"select * from TransCostingHeader where ID={0}", args[1]));
            foreach (DataRow r in dt.Rows)
            {
                string getValue = revised("5", args[1], r["ExchangeRate"].ToString());
                gridData.JSProperties["cpKeyValue"] = (getValue == null) ? string.Empty : getValue.ToString();
            }
        }
        if (args[0] == "Copied")
        {
            copyCosting(args);
        }
        if (args[0] == "New")
        {
            NewID();
        }
        if (args[0] == "Delete" && args.Length > 1)
        {
            Delete(args[1]);
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
        Update();
    }
    void buildsendemail(List<string> lists, string _Request)
    {
        if (lists.Count > 0)
        {
            List<string> lstSend = new List<string>();
            List<string> MailTo = new List<string>();
            List<string> MailCc = new List<string>();
            List<string> listRequest = new List<string>();
            SqlParameter[] param = {
                            new SqlParameter("@Id", String.Format("{0}",_Request))};
            var dtreq = GetRelatedResources("spGetRequestNoti", param);
            if (dtreq.Rows.Count == 0)
            {
                return;
            }
            foreach (DataRow _rw in dtreq.Rows)
            {
                if (_rw["Statusapp"].ToString().Equals("5") && _rw["email"].ToString() != "")
                {
                    lstSend.Add(cs.GetData(_rw["user_name"].ToString(), "fn"));
                    MailTo.Add(_rw["email"].ToString());
                }
                if (_rw["Statusapp"].ToString().Equals("1"))
                {
                    MailCc.Add(_rw["email"].ToString());
                }
            }
            MailCc.Add(cs.GetData(user_name.ToString(), "email"));
            string _link = string.Format("Dear {0} <br/> ", String.Join(",", lstSend.ToArray()));
            string _body = "";
            foreach (var _list in lists)
            {
                foreach (DataRow _rw in getrequester(_list.ToString()).Rows)
                {
                    _body = _body + string.Format("TRF : {0}, Request No : {1}, RD Ref : {2}, Customer : {3}", _rw["RequestNo"], _rw["DocumentNo"], _rw["RefSamples"], tbCustomer.Text);
                }
                _link += @"<br/> The document link --------><a href=" + cs.GetSettingValue() + "/Default.aspx?viewMode=RequestEditForm&ID="
                    + _list.ToString();
                _link += " style='color: rgb(0,255,0)'><font color='#0000FF'>click</font></a>";
            }

            cs.sendemail(String.Join(",", MailTo.ToArray()), String.Join(",", MailCc.ToArray()), _link, "Request Create Material by CD" + _body);
        }
    }

    void Delete(string Id)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spDelDocument";
            cmd.Parameters.AddWithValue("@ID", Id.ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    DataTable getrequester(string Id)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spGetRequester";
            cmd.Parameters.AddWithValue("@RequestNo", Id.ToString());
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            return dt;
        }
    }
    void NewID()
    {
        //testgrid.DataSource = null;
        //testgrid.DataBind();
    }
    void savedata(string Id)
    {
        //string qty = ((TextBox)gridData.FindRowCellTemplateControl(i, gridData.Columns["Discount"] as GridViewDataColumn, "txtQty")).Text;
        //myfun.FireStoredProcedure("mystored", new SqlParameter("@come_date", gridData.GetRowValues(i, "come_date").ToString()));
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spinsertCosting";
            cmd.Parameters.AddWithValue("@Id", Id.ToString());
            cmd.Parameters.AddWithValue("@RequestNo", hfid["hidden_value"].ToString());
            cmd.Parameters.AddWithValue("@MarketingNumber", tbMarketingNo.Text);
            cmd.Parameters.AddWithValue("@RDNumber", tbReference.Text);
            cmd.Parameters.AddWithValue("@Company", string.Format("{0}", CmbCompany.Value));
            cmd.Parameters.AddWithValue("@PackSize", tbPackSize.Text);
            cmd.Parameters.AddWithValue("@CreateBy", user_name.ToString());
            cmd.Parameters.AddWithValue("@NewID", hfgetvalue["NewID"].ToString());
            cmd.Parameters.AddWithValue("@Remark", mNotes.Text);
            cmd.Parameters.AddWithValue("@CanSize", tbCanSize.Text);
            cmd.Parameters.AddWithValue("@NetWeight", tbNetweight.Text + "|" + CmbNetUnit.Text);
            //cmd.Parameters.AddWithValue("@Completed", string.Format("{0}", cbCompleted.Checked == true ? 1 : 0));
            cmd.Parameters.AddWithValue("@Packaging", string.Format("{0}", CmbPackaging.Text));
            cmd.Parameters.AddWithValue("@StatusApp", string.Format("{0}", 2));
            cmd.Parameters.AddWithValue("@ExchangeRate", seExchangeRate.Text);
            cmd.Parameters.AddWithValue("@Customer", tbCustomer.Text);
            cmd.Parameters.AddWithValue("@From", defrom.Value);
            cmd.Parameters.AddWithValue("@To", deto.Value);
            cmd.Parameters.AddWithValue("@UserType", string.Format("{0}", usertp["usertype"]));
            cmd.Parameters.AddWithValue("@VarietyPack", string.Format("{0}", tbVarietyPack.Text));
            //cmd.Connection = con;
            //con.Open();
            //cmd.ExecuteNonQuery();
            //con.Close();
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();// string Folio = "";
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            listFolio = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                listFolio = new List<string>();
                listFolio.Add(dr["ID"].ToString());
                inserthead(dr["ID"].ToString());
                //Session["Folio"] = dr["ID"].ToString();
                if (new[] { "2", "-1" }.Any(dr["StatusApp"].ToString().Equals))
                {
                    testdata(tcustomer, listFolio);
                    insertname(dr["ID"].ToString());
                    List<FileSystemData> listdata = (List<FileSystemData>)Session["DataSource"];
                    SqlCommand sqlComm = new SqlCommand();
                    sqlComm.CommandText = @"DELETE FROM FileSystem WHERE GCRecord=@ID";
                    sqlComm.Parameters.AddWithValue("@ID", Guid.Parse(dr["UniqueColumn"].ToString()));
                    DataTable _dtx = myservice.GetData(sqlComm);
                    foreach (var d in listdata)
                    {
                        if (d.IsFolder == false)
                        {
                            savefile(d.Name.ToString(), d.Data, d.LastUpdateBy);
                        }
                    }
                    Session.Remove("DataSource");
                    savetestgrid(dr["ID"].ToString());
                }
                ApproveStep(dr["ID"].ToString());
                //if (Id == "0")

                //                DataTable table = (DataTable)Session[seGetMyData];
                //               foreach (DataRow row in table.Rows)
                //Response.Write("");
                con.Dispose();
            }

            //try
            //{
            //    HttpContext.Current.Response.Redirect("~/Default.aspx?viewMode=EntryEditForm&ID=" + ID.ToString());
            //}
            //catch (ApplicationException)
            //{
            //    HttpContext.Current.Response.RedirectLocation =
            //                         System.Web.VirtualPathUtility.ToAbsolute("~/Default.aspx?viewMode=EntryEditForm&ID=" + ID.ToString());
            //}
            //string TARGET_URL = "~/Default.aspx?viewMode=EntryEditForm&Folio=" + Folio.ToString();
            //if (Page.IsCallback)
            //    ASPxWebControl.RedirectOnCallback(TARGET_URL);
            //else
            //    Response.Redirect(TARGET_URL);
            //Response.Redirect("~/Default.aspx?viewMode=EntryEditForm&ID="+ID.ToString());
        }
    }
    void inserthead(string keys)
    {
        foreach (var rw in listItems)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spinserthCosting";

                cmd.Parameters.AddWithValue("@ID", string.Format("{0}", rw.ID));
                cmd.Parameters.AddWithValue("@RequestNo", string.Format("{0}", keys));
                cmd.Parameters.AddWithValue("@Component", string.Format("{0}", rw.SubType));
                cmd.Parameters.AddWithValue("@SAPMaterial", string.Format("{0}", rw.Code));
                cmd.Parameters.AddWithValue("@Description", string.Format("{0}", rw.Name));
                cmd.Parameters.AddWithValue("@Quantity", string.Format("{0}", rw.Quantity));
                cmd.Parameters.AddWithValue("@PriceUnit", rw.PriceUnit);
                cmd.Parameters.AddWithValue("@Amount", string.Format("{0}", rw.Amount));
                cmd.Parameters.AddWithValue("@Per", string.Format("{0}", rw.Per));
                cmd.Parameters.AddWithValue("@SellingUnit", rw.Currency);
                cmd.Parameters.AddWithValue("@Loss", string.Format("{0}", rw.Loss));
                cmd.Parameters.AddWithValue("@Formula", string.Format("{0}", rw.Formula));
                cmd.Parameters.AddWithValue("@CreateBy", hfuser["user_name"].ToString());
                cmd.Parameters.AddWithValue("@Mark", string.Format("{0}", rw.Mark));
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
    void insertname(string keys)
    {
        if (keys == "0" || keys == "") return;
        if (_dt == null) return;
        if (_dt.Rows.Count == 0) return;
        foreach (DataRow c in _dt.Rows)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spinsertFormulaHeader";
                cmd.Parameters.AddWithValue("@name", c["Name"].ToString());
                cmd.Parameters.AddWithValue("@Customer", string.Format("{0}", ""));
                cmd.Parameters.AddWithValue("@Code", string.Format("{0}", c["Code"]));
                cmd.Parameters.AddWithValue("@RefSamples", string.Format("{0}", c["RefSamples"]));
                cmd.Parameters.AddWithValue("@formula", c["formula"].ToString());
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", c["ID"]));
                cmd.Parameters.AddWithValue("@IsActive", string.Format("{0}", 0));
                //cmd.Parameters.AddWithValue("@IsActive", string.Format("{0}", (Convert.ToBoolean(c["IsActive"])==true)?0:1));
                cmd.Parameters.AddWithValue("@RequestNo", keys.ToString());
                cmd.Parameters.AddWithValue("@Costper", string.Format("{0}", 0));
                cmd.Parameters.AddWithValue("@CostNo", string.Format("{0}", 0));
                cmd.Parameters.AddWithValue("@PID", string.Format("{0}", c["PID"]));
                cmd.Parameters.AddWithValue("@SellingUnit", string.Format("{0}", c["SellingUnit"]));
                cmd.Parameters.AddWithValue("@ref", string.Format("{0}", c["ref"]));
                cmd.Parameters.AddWithValue("@nw", string.Format("{0}", c["NetWeight"]));
                cmd.Parameters.AddWithValue("@PackSize", string.Format("{0}", c["PackSize"]));

                cmd.Parameters.AddWithValue("@Packaging", string.Format("{0}", c["Packaging"]));
                cmd.Parameters.AddWithValue("@Revised", string.Format("{0}", 0));
                cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", "0"));

                //cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", c["MinPrice"].ToString() == "0"? cs.GetMinPrice(c["formula"].ToString(), c["ID"].ToString()): c["MinPrice"]));
                cmd.Parameters.AddWithValue("@Mark", string.Format("{0}", c["Mark"]));
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
    string chdevlopment(string s)
    {
        if (usertp["usertype"].ToString() == "0") return "0";
        string result = cs.ReadItems(string.Format(@"select convert(nvarchar(max),count(*))'c' 
            from MasApprovAssign where empid='{0}' and Sublevel in (3,4)", s));
        return result;
    }
    void ApproveStep(string keys)
    {
        if (string.IsNullOrEmpty(CmbDisponsition.Text)) return;
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spApproveStep";
            cmd.Parameters.AddWithValue("@Id", keys.ToString());
            cmd.Parameters.AddWithValue("@User", user_name.ToString());
            cmd.Parameters.AddWithValue("@StatusApp", CmbDisponsition.Value);
            cmd.Parameters.AddWithValue("@table", "TransCostingHeader");
            cmd.Parameters.AddWithValue("@remark", mComment.Text);
            cmd.Parameters.AddWithValue("@Assign", "");
            cmd.Parameters.AddWithValue("@reason", CmbReason.Text = (CmbDisponsition.Value.ToString() != "3") ? "" : CmbReason.Text);
            //cmd.Connection = con;
            //con.Open();
            //cmd.ExecuteNonQuery();
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                List<string> myList = new List<string>();
                string name = dr["NamingCode"].ToString();
                List<string> list = new List<string>();
                List<string> lstValue = new List<string>();
                List<string> lstSend = new List<string>();
                string[] split = dr["MailTo"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split)
                {
                    if (s.Trim() != "")
                    {
                        if (!myList.Contains(s))
                        {
                            list.Add(cs.GetData(s, "email"));
                            lstSend.Add(cs.GetData(s, "fn"));
                            myList.Add(s);
                        }
                    }
                }
                //string statusapp = " was " + CmbDisponsition.Text;
                string sender = String.Join(",", list.ToArray()); //string MailCc = "";
                list = new List<string>();
                split = dr["MailCc"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split)
                {
                    if (s.Trim() != "")
                        if (!myList.Contains(s))
                        {
                            list.Add(cs.GetData(s, "email"));
                            myList.Add(s);
                        }
                }
                //list.Add(cs.GetData(user_name, "email"));
                string MailCc = String.Join(",", list.ToArray());
                string _link = string.Format("Dear {0} <br/> ", String.Join(",", lstSend.ToArray()));
                _link += "<br/> Costing No.:" + dr["RequestNo"].ToString();
                _link += "<br/> Request No.:" + dr["CreateBy"].ToString();
                _link += "<br/> Create By : " + cs.GetData(dr["Requester"].ToString(), "fn");
                _link += "<br/> Customer Name : " + tbCustomer.Text.ToString();
                if (!string.IsNullOrEmpty(CmbReason.Text))
                    _link += "<br/> Reason :" + CmbReason.Text;

                _link += "<br/> Sender : " + cs.GetData(user_name.ToString(), "fn");
                _link += "<br/> Comment :" + mComment.Text;
                _link += @"<br/> The document link --------><a href=" + cs.GetSettingValue() + "/Default.aspx?viewMode=CostingEditForm&ID="
                    + keys.ToString() + "&UserType=" + usertp["usertype"];
                _link += " style='color: rgb(0,255,0)'><font color='#0000FF'>click</font></a>";
                string subject = string.Format(@"{0}:{1}:{2} / Request No.: " + dr["CreateBy"].ToString() + " V.{3} ,"
                + tbCustomer.Text + "/", name.ToString(), dr["Subject"], dr["RequestNo"].ToString(), string.Format("{0:00}", dr["Revised"]));
                //string.Format("costing Request No.:{0}{1}/{2}", dr["RequestNo"].ToString(), statusapp, tbCustomer.Text)
                //if (usertp["usertype"].ToString() == "1"){
                //    if (lstValue.Count() > 0)
                //        cs.emlSend(String.Join(",", lstValue.ToArray()), MailCc, _link, subject, dr["UniqueColumn"].ToString());
                //    if(lstSend.Count()>0)
                //        cs.sendemail(String.Join(",", lstSend.ToArray()), MailCc, _link, subject);
                //}
                //else
                cs.sendemail(sender, MailCc, _link, subject);
            }
            con.Close();
            //try
            //{
            //    //HttpContext.Current.Response.Write("Message Sent Succesfully");
            //}
            //catch (Exception ex)
            //{
            //    HttpContext.Current.Response.Write(ex.ToString());
            //}
        }
    }
    protected void gridData_CustomGroupDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e)
    {
        if (e.Column.FieldName == "Company")
            e.DisplayText = HttpUtility.HtmlEncode(e.Value);
    }

    void NotifyUser(string message)
    {
        Page.ClientScript.RegisterStartupScript(GetType(), "alert",
          @"<script type=""text/javascript"">alert('" + message + "');</script>");
    }

    protected void gridData_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        var args = e.Parameters.Split('|');
        var result = new Dictionary<string, string>();
        DataTable dt = new DataTable();
        string strSQL = "";
        long id;
        if (!long.TryParse(args[2], out id))
            return;

        if (args[1] == "RequestNo")
        {
            if (tcustomer.Rows.Count > 0) return;
            result["view"] = args[1].ToString();
            result["ID"] = id.ToString();
            Page.Session.Remove("seGetMyData");
            strSQL = string.Format(@"select top 1 * from TransTechnical where ID='{0}'", id);
            dt = cs.builditems(strSQL);
            _dt = new DataTable();
            namelist(string.Format(@"{0}", 0));
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
                //insert to formula

                tcustomer = new DataTable();
                tcustomer = ConvertToDatatable("0");
                tcustomer.PrimaryKey = new DataColumn[] { tcustomer.Columns["ID"] };
                testgrid_reload("0");
                int iFormula = 0;
                //namelist(id.ToString());
                string query = string.Format(@"select t.*,''Mark from TransProductList t where t.RequestNo='{0}'
                                 and t.id in (select c.formula from TransCusFormula c where c.RequestNo='{0}' group by c.formula)",
                id.ToString());
                var Tablelist = cs.builditems(query); int nomax = 0;
                foreach (DataRow drow in Tablelist.Rows)//package
                {
                    iFormula++; nomax++;
                    DataRow _rwx = _dt.NewRow();
                    _rwx["ID"] = nomax;
                    _rwx["formula"] = iFormula.ToString();
                    _rwx["Costno"] = "";
                    _rwx["RefSamples"] = "";
                    _rwx["MinPrice"] = "";
                    _rwx["Mark"] = "X";
                    _rwx["Name"] = drow["Name"];
                    _rwx["NetWeight"] = drow["NetWeight"];
                    _rwx["Packaging"] = dr["Packaging"].ToString();
                    _rwx["FixedFillWeight"] = drow["FixedFillWeight"];
                    _rwx["PID"] = drow["ID"];
                    _rwx["SellingUnit"] = "";
                    _rwx["Ref"] = "";
                    //_rwx["Unit"] = "";
                    _rwx["PackSize"] = drow["PackSize"];
                    _dt.Rows.Add(_rwx);
                    //
                    SqlParameter[] param = {
                            new SqlParameter("@Id", id.ToString()),
                            new SqlParameter("@type", "C"),
                            new SqlParameter("@formula", drow["ID"].ToString())};
                    var dtformula = GetRelatedResources("spGetCusFormula", param);
                    DataRow[] row = dtformula.Select();

                    foreach (DataRow _r in row)
                    {
                        //foreach(DataRow _row in cs.builditems("select * from MasOHSGA where Company"))
                        if (_r["Component"].ToString().Equals("3") || _r["Component"].ToString().Equals("4") || _r["Component"].ToString().Equals("5"))
                        {
                            //DataTable testdt = _dataTable.Clone();
                            //DataRow _ravi = testdt.NewRow();
                            string SubType = "";
                            if (_r["Component"].ToString().Equals("5"))
                                SubType = "Labor";//_r["SubType"].ToString();
                            else
                                SubType = cs.ReadItems(@"select value from dbo.FNC_SPLIT('Raw Material,Ingredient,Primary Packaging,Secondary Packaging,Labor,Semi,BCDL',',') where idx="
                                + _r["Component"].ToString());

                            _dataTable.Rows.Add(_r["Id"], string.Format("{0}", SubType),
                    _r["Material"].ToString(),
                    _r["Description"].ToString() == "" ? _r["Name"].ToString() : _r["Description"].ToString(),
                    _r["Result"].ToString(),
                    _r["PriceOfUnit"].ToString().Equals("") ? (Convert.ToDouble(_r["PriceOfCarton"].ToString()) / Convert.ToDouble(_r["Result"].ToString())).ToString() : _r["PriceOfUnit"].ToString(),
                    _r["Component"].ToString().Equals("5") ? (Convert.ToDouble(_r["PriceOfCarton"].ToString())) :
                    Convert.ToDouble(_r["Result"].ToString()) * Convert.ToDouble(_r["PriceOfUnit"].ToString()) * Convert.ToDouble(drow["PackSize"].ToString()), "",
                    _r["Currency"].ToString(),
                    _r["Component"].ToString().Equals("5") ? 0 :
                    (Convert.ToDecimal(_r["PriceOfUnit"].ToString()) * (Convert.ToDecimal(getloss2(dr, 3).ToString()) / 1000)),
                    iFormula, "X"); 
                        }
                        else //if (_r["Component"].ToString().Equals("1") || _r["Component"].ToString().Equals("2"))
                        {
                            DataRow _row = tcustomer.NewRow();
                            _row["ID"] = _r["Id"];
                            _row["Component"] = "";
                            switch (_r["Component"].ToString())
                            {
                                case "1":
                                    _row["Component"] = "Raw Material";
                                    break;
                                case "2":
                                    _row["Component"] = "Ingredient";
                                    break;
                                case "5":
                                    _row["Component"] = string.Format("(0)", _row["SubType"]);
                                    break;
                            }
                            _row["SubType"] = _row["Component"];
                            _row["Description"] = _r["Description"].ToString() == "" ? _r["Name"] : _r["Description"];
                            _row["SapMaterial"] = _r["Material"];
                            _row["GUnit"] = _r["Result"];
                            _row["Yield"] = _r["Yield"];
                            //_row["RawMaterial"] = _r["RawMaterial"];
                            //_row["Name"] = _r["Name"];
                            _row["PriceOfUnit"] = _r["AdjustPrice"];
                            //,[AdjustPrice]
                            _row["Currency"] = "THB";
                            _row["Unit"] = _r["Unit"];
                            _row["ExchangeRate"] = "1";
                            _row["BaseUnit"] = _r["AdjustPrice"];
                            _row["PriceOfCarton"] = _r["PriceOfCarton"];
                            _row["Formula"] = iFormula;
                            _row["IsActive"] = _r["IsActive"];
                            _row["Remark"] = _r["Remark"];
                            _row["LBOh"] = _r["LBOh"];
                            _row["LBRate"] = _r["LBRate"];
                            //_row["RequestNo"] = _r["RequestNo"];
                            _row["aValidate"] = "";
                            _row["Mark"] = "X";
                            tcustomer.Rows.Add(_row);
                        }
                    }
                    //tcustomer.PrimaryKey = new DataColumn[] { tcustomer.Columns["ID"] };
                }
            }
            e.Result = result;
        }
        if (args[1] == "PackageCode" || args[1] == "SecPackageCode")
        {
            DateTime from = Convert.ToDateTime(defrom.Value).Date;
            int monthDiff = cs.GetMonthsBetween(from, Convert.ToDateTime(deto.Value).Date);
            int monthNumber = Convert.ToInt32(from.ToString("MM", CultureInfo.InvariantCulture));
            result["view"] = args[1].ToString();
            strSQL = string.Format(@"select top 1 Material,[Description],case when {0} < 7 then PriceStd1
		    else PriceStd2 end as 'Price' ,Currency,Unit from MasPriceStd where ID='{1}'", monthDiff, id);
            dt = cs.builditems(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                result["ID"] = dr["Material"].ToString();
                result["Name"] = dr["Description"].ToString();
                result["Rate"] = dr["Price"].ToString();
                result["Unit"] = dr["Currency"].ToString();
            }
            e.Result = result;
        }
        if (args[1] == "Margin")
        {
            result["view"] = args[1].ToString();
            strSQL = string.Format(@"select top 1 * from MasMargin where ID='{0}'", id);
            dt = cs.builditems(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                result["ID"] = dr["MarginCode"].ToString();
                result["Name"] = dr["MarginName"].ToString();
                result["Rate"] = dr["MarginRate"].ToString();
            }
            e.Result = result;
        }
        if (args[1] == "DL")
        {
            result["view"] = args[1].ToString();
            strSQL = string.Format(@"select top 1 * from MasDL where ID='{0}'", id);
            dt = cs.builditems(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                result["Name"] = dr["LBName"].ToString();
                result["Rate"] = dr["LBRate"].ToString();
                result["Unit"] = dr["Unit"].ToString();
            }
            e.Result = result;
        }
        if (args[1] == "SGA" || args[1] == "OH" || args[1] == "OHRate")
        {
            result["view"] = args[1].ToString();
            strSQL = string.Format(@"select top 1 * from MasOHSGA where ID='{0}'", id);
            dt = cs.builditems(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                if (_dt != null)
                {
                    DataRow[] xdr = _dt.Select(string.Format("Formula={0}", ActivePageSymbol));
                    if (xdr.Length > 0)
                    {
                        double NetWeight = Convert.ToDouble(xdr[0]["NetWeight"].ToString());
                        result["Q"] = Convert.ToDouble((NetWeight / 1000) * Convert.ToDouble(xdr[0]["PackSize"].ToString())).ToString();
                    }
                }
                result["Name"] = dr["Name"].ToString();
                result["Rate"] = dr["Rate"].ToString();
                result["Unit"] = dr["Unit"].ToString();
            }
            e.Result = result;
        }
        if (args[1] == "New")
        {
            hfgetvalue["NewID"] = string.Format("{0}", cs.GetNewID());
            result["NewID"] = (string)cs.GetNewID();
            result["editor"] = cs.IsMemberOfRole(string.Format("{0}", 2));
            e.Result = result;
        }
        //if (args[1] == "EditCost")
        //{
        //    using (SqlConnection con = new SqlConnection(strConn))
        //    {
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = "spselectMapCosting";
        //        cmd.Parameters.AddWithValue("@Code", args[2]);
        //        cmd.Parameters.AddWithValue("@Costing", args[3]);
        //        cmd.Parameters.AddWithValue("@key", string.Format("{0}", 1));
        //        cmd.Connection = con;
        //        con.Open();
        //        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
        //        oAdapter.Fill(dt);
        //        con.Close();
        //    }
        //}
        if (args[1] == "EditDraft" || args[1] == "save")
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spselectCostingHeader";
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@username", user_name.ToString());
                cmd.Connection = con;
                con.Open();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
            }
            if (dt.Rows.Count == 0)
            {
                result["StatusApp"] = string.Format("{0}", 2);
                e.Result = result;
            }
            foreach (DataRow dr in dt.Rows)
            {
                result["RequestNo"] = (string)dr["RequestNo"];
                result["Marketingnumber"] = (string)dr["Marketingnumber"];
                result["RDNumber"] = (string)dr["RDNumber"];
                result["PackSize"] = (string)dr["PackSize"];
                result["Company"] = (string)dr["Company"];
                result["ID"] = (string)dr["ID"];
                result["Folio"] = (string)dr["Folio"];
                result["NewID"] = (string)dr["UniqueColumn"];
                result["Remark"] = (string)dr["Remark"];
                result["CanSize"] = (string)dr["CanSize"];
                result["Packaging"] = (string)dr["Packaging"];
                result["ExchangeRate"] = (string)dr["ExchangeRate"];
                result["RefSamples"] = dr["RefSamples"].ToString();
                result["Name"] = dr["Name"].ToString();
                result["Code"] = dr["Code"].ToString();
                string values = dr["NetWeight"].ToString();
                string[] array = values.Split('|');//Regex.Split(dr["NetWeight"], "|");
                result["NetWeight"] = array[0];
                result["NetUnit"] = array[1];
                result["StatusApp"] = (string)dr["StatusApp"];
                result["editor"] = dr["editor"].ToString();
                result["Eventlog"] = string.Format("{0}", DisplayEventlog(e));
                result["Customer"] = dr["Customer"].ToString();
                result["From"] = dr["From"].ToString();
                result["To"] = dr["To"].ToString();
                result["UserType"] = dr["UserType"].ToString();
                result["VarietyPack"] = string.Format("{0}", dr["VarietyPack"]);
                e.Result = result;
            }
        }
    }
    DataTable Getformula(string id)
    {
        SqlParameter[] param = {
                            new SqlParameter("@Id", id.ToString())};
        return GetRelatedResources("spGetProductList", param);
        //if (t.Rows.Count > 0)
        //    return t.Rows[0]["c"].ToString();
        //else
        //    return ""; 
    }
    public string DisplayEventlog(ASPxGridViewCustomDataCallbackEventArgs e)
    {
        var args = e.Parameters.Split('|');
        int id;
        var text = string.Format(NotFoundFormat, args[2]);
        if (int.TryParse(args[2], out id))
        {
            StringBuilder sb = new StringBuilder();
            string strSQL = @"select CreateBy,RequestNo,RIGHT(CONCAT('00', Revised), 2)'Revised'" +
                " ,b.Title  from TransCostingHeader a left join MasStatusApp b on b.Id = a.StatusApp where a.Id='" + id + "' and b.levelapp in (1,2)";
            DataTable dt = cs.builditems(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                var arr = dr["CreateBy"].ToString().Split('|'); string s = "";
                sb.Append(string.Format("StatusApp :{0}, Revised :{1}</b>",
                     dr["Title"].ToString(), dr["Revised"].ToString()));
                for (int i = 0; i < arr.Length; i++)
                {
                    s += cs.GetData(arr[i], "fn") + ",";
                }
                sb.Append(string.Format("<div><b><u>Create By : {0}</u></b></div>", s));
            }
            var Results = new DataTable();//spGetHistory
            SqlParameter[] param = { new SqlParameter("@Id", id),
                new SqlParameter("@tablename",string.Format("{0}","TransCostingHeader")),
                new SqlParameter("@user",string.Format("{0}",hfuser["user_name"]))};
            Results = cs.GetRelatedResources("spGetHistory", param);
            if (Results.Rows.Count > 0)
            {
                foreach (DataRow dr in Results.Rows)
                {
                    //string[] printer = { "5", "6", "7" };
                    //if (string.Format("{0}",dr["StatusApp"])=="3")
                    var status = dr["StatusApp"].ToString();
                    var Title = string.Format("{0}", (new[] { "5", "6", "7" }).Contains(status) ? cs.GetData(dr["Reason"].ToString(), "fn") : dr["Reason"]);
                    sb.Append(string.Format(PreviewFormat, cs.GetData(dr["Username"].ToString(), "fn"), dr["Title"], Title, dr["CreateOn"], dr["Remark"]));
                    sb.Append("<br/>");
                }
            }
            else
                sb.Append("<br/>");
            text = sb.ToString();
        }
        //litText.Text = text;
        return text;
    }
    //protected void FormPanel_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    if (string.IsNullOrEmpty(e.Parameter)) return;
    //string strSQL = @"select ID,RequestNo,MarketingNumber,format(RequestDate,'dd-MM-yyyy')'RequireDate',PackSize ";
    //strSQL= strSQL+"from TransTechnical Where Company in (select value from dbo.FNC_SPLIT('" + e.Parameter + "',';')) ";
    //strSQL = strSQL + "and StatusApp <> '5'";
    //CmbCostingNo.DataSource = cs.builditems(strSQL);
    //CmbCostingNo.DataBind();
    //}
    //protected void tkbPackSize_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    ASPxTokenBox tkb = sender as ASPxTokenBox;
    //    if (string.IsNullOrEmpty(e.Parameter)) return;
    //    string[] param = e.Parameter.Split('|'); //bool selected = true;
    //    string strtext = cs.ReadItems(@"select isnull(PackSize," +
    //    "(select top 1 PackSize from TransCostingHeader where RequestNo=a.ID)) from TransTechnical a where a.ID='" +
    //    param[1] + "'");
    //    string strSQL = @"select LTRIM(RTRIM(value))'value' from dbo.FNC_SPLIT('" + strtext + "',',')";
    //    DataTable dt = cs.builditems(strSQL);
    //    tkb.DataSource = dt;
    //    tkb.DataBind();
    //}
    //protected void CmbPackSize_Callback(object sender, CallbackEventArgsBase e)
    //{
    //	if (string.IsNullOrEmpty(e.Parameter)) return;
    //	string[] param = e.Parameter.Split('|'); //bool selected = true;
    //	string strtext = cs.ReadItems(@"select isnull(PackSize," + 
    //	"(select top 1 PackSize from TransCostingHeader where RequestNo=a.ID)) from TransTechnical a where a.ID='" + param[1] + "'");
    //	string strSQL = @"select LTRIM(RTRIM(value))'value' from dbo.FNC_SPLIT('" + strtext + "',',')";
    //	DataTable dt = cs.builditems(strSQL);
    //	CmbPackSize.DataSource = dt;
    //	CmbPackSize.DataBind();
    //	if (dt.Rows.Count == 1)
    //		CmbPackSize.SelectedIndex = 0;
    //}
    //protected void grid_DataBinding(object sender, EventArgs e)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
    //    //(sender as ASPxGridView).DataSource = LoadGrid;
    //    g.KeyFieldName = "RowID";
    //    g.DataSource = LoadGrid;
    //    g.ForceDataRowType(typeof(DataRow));
    //}
    void namelist(string Folio)
    {
        dsnamelist.SelectParameters.Clear();
        dsnamelist.SelectParameters.Add("Id", string.Format("{0}", Folio.ToString()));
        dsnamelist.DataBind();
        _dt = ((DataView)dsnamelist.Select(DataSourceSelectArguments.Empty)).Table;
        _dt.PrimaryKey = new DataColumn[] { _dt.Columns["ID"] };
    }
    protected void grid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        
        string[] parameters = e.Parameters.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        string[] param = e.Parameters.Split('|'); //bool selected = true;
        switch (param[0])
        {
            case "Labor":
            case "changed":
                //LaborOverhead();
                if (tcustomer != null)
                    for (int i = 0; i < tcustomer.Rows.Count; i++)
                    {
                        DataRow dr = _found(tcustomer.Rows[i]);
                    }
                break;
            //case "Code":
            //case "Name":
            //case "RefSamples":
            //    if (Tablelist.Rows.Count == 0) return;
            //    DataRow dr = Tablelist.Select(string.Format("Formula={0}", param[1].ToString())).FirstOrDefault();
            //    if (dr != null)
            //    {
            //            dr[param[0]] = string.Format("{0}", param[2]);
            //    }
            //        break;
            case "reload":
                g.Selection.UnselectAll();
                //Session.Remove(sessionKey);
                //Session.Remove("sessionlist");
                string Folio = param[1];
                //namelist(Folio);
                //string datapath = "~/XlsTables/" + Folio + ".json";
                //FileInfo sFile = new FileInfo(Server.MapPath(datapath));
                //bool fileExist = sFile.Exists;
                //DataTable table = new DataTable();
                //using (SqlConnection con = new SqlConnection(strConn))
                //{
                //    SqlCommand cmd = new SqlCommand();
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    cmd.CommandText = "spGetFormula";
                //    cmd.Parameters.AddWithValue("@data", Folio);
                //    cmd.Parameters.AddWithValue("@user", user_name.ToString());
                //    cmd.Connection = con;
                //    con.Open();
                //    SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                //    oAdapter.Fill(table);
                //    con.Close();
                //    con.Dispose();
                //    dataTable = table;
                //    dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["RowID"] };
                //    //Session[CustomTable] = dataTable;
                //}
                //grid.ClientSideEvents.SelectionChanged = String.Format("function (s, e) {{ testgrid.PerformCallback('priority|' + {0});}}", param[1]);
                tcustomer = ConvertToDatatable(Folio);
                tcustomer.PrimaryKey = new DataColumn[] { tcustomer.Columns["ID"] };
                //update++
                listItems = new List<TransCosting>();
                string strSQL = "select * from TranshCosting where requestno='" + Folio + "'";
                listItems = (from DataRow dr in cs.builditems(strSQL).Rows
                             select new TransCosting()
                             {
                                 ID = Convert.ToInt32(dr["ID"]),
                                 SubType = dr["Component"].ToString(),
                                 Code = dr["SAPMaterial"].ToString(),
                                 Name = string.Format("{0}", dr["SAPMaterial"])==""? dr["Description"].ToString() : GetDescrip(dr["SAPMaterial"].ToString()),
                                 //Name = string.Format("{0}",  dr["Description"].ToString() ),
                                 Quantity = string.Format("{0}", dr["Quantity"]),
                                 PriceUnit = string.Format("{0}", dr["PriceUnit"]),
                                 Amount = dr["Amount"].ToString(),
                                 Per = dr["Per"].ToString(),
                                 Currency = dr["SellingUnit"].ToString(),
                                 Formula = Convert.ToInt32(dr["Formula"]),
                                 RequestNo = dr["RequestNo"].ToString()
                             }).ToList();
                _update("raw|" + Folio);
                //++product code 
                //Session.Remove("setableKey");
                _dt = new DataTable();
                namelist(param[1]);
                //component 
                //Session.Remove("seGetMyData");
                //Session.Remove(sessionKey);
                testgrid_reload(param[1]);
                break;
            //StreamReader stRead = new StreamReader(Server.MapPath(datapath));
            //dataTable = JsonConvert.DeserializeObject<DataTable>(stRead.ReadToEnd());

            //grid.Columns.Clear();
            //if (fileExist)
            //   return dataTable;
            //foreach (DataColumn column in dataTable.Columns)
            //{
            //    if (column.ColumnName.Contains("Carton"))
            //        grid.TotalSummary.Add(SummaryItemType.Custom, column.ColumnName);
            //}
            //grid.CustomSummaryCalculate += grid_CustomSummaryCalculate;
            //int index = 3;
            //if (int.TryParse(e.Parameters, out index))
            //grid.SettingsEditing.Mode = GridViewEditingMode.EditForm;
            //Grid.DataSource = null;
            //Grid.DataBind();
            //}else
            //grid.SettingsEditing.Mode = GridViewEditingMode.Inline;
            //LoadGrid();
            //DataTable table = GetDataView(Grid);
            //if (table.Rows.Count > 0)
            //{
            //    string datapath = "~/XlsTables/" + CmbCostingNo.Text + "_.json";
            //    cs.WriteJason(table, Server.MapPath(datapath));
            //Session["Flag"] = true;
            //grid.SettingsEditing.Mode =  GridViewEditingMode.Batch;
            case "savedata":
                Session["Flag"] = null;
                //grid.SettingsEditing.Mode = GridViewEditingMode.Inline;
                break;
            //grid.Columns.Clear();
            //grid.TotalSummary.Clear();
            case "ChangeValidTo":
                //raw material,ingredient
                for (int i = 0; i < tcustomer.Rows.Count; i++)
                {
                    DataRow row = _found(tcustomer.Rows[i]);
                }
                //
                foreach (DataRow _xdr in _dataTable.Rows)
                {
                    if (_xdr["Code"].ToString().Length > 0)
                    {
                        string mytext = _xdr["Code"].ToString().Substring(1, 1); int n;
                        bool isNumeric = int.TryParse(mytext, out n);
                        if (_xdr["Code"].ToString().StartsWith("5122"))
                            isNumeric = false;
                        var Results = buildpackage(isNumeric, _xdr["Code"].ToString());

                        if (Results.Rows.Count > 0)
                        {
                            _xdr["PriceUnit"] = Convert.ToDouble(Results.Rows[0]["Price"]);
                            _xdr["Amount"] = Convert.ToDouble(_xdr["Quantity"]) * Convert.ToDouble(_xdr["PriceUnit"]);
                        }
                    }
                }
                break;
            case "symbol":
                ActivePageSymbol = param[1].ToString();
                //tcustomer = (DataTable)g.DataSource;
                break;
            case "Insert":
                _dataTable = insertsGrid(Convert.ToInt32(ActivePageSymbol), param[1]);
                break;
            case "copy":

                int max = Convert.ToInt32(tcustomer.AsEnumerable()
                   .Max(row => row["Formula"]));
                for (int i = 1; i <= max; i++)
                {
                    _dataTable = insertsGrid(i, param[1]);
                }
                break;
            case "AddRow":
                //throw new MyException("Data modifications are not allowed.");
                //    data = new List<dynamic>();
                //var data = context.Session[InsertRowsSessionKey] as List<dynamic>;
                g.Selection.UnselectAll();
                //Session.Remove(sessionKey);
                //Session.Remove("setableKey");
                tcustomer = new DataTable();
                tcustomer = cs.builditems("select top 0 convert(nvarchar(max),Id) as 'RowID'" +
                  ", Component" +
                  ", SubType" +
                  ", Description" +
                  ", Material as 'SAPMaterial'" +
                  ", convert(float,Result) as 'GUnit'" +
                  ", Yield" +
                  ", RawMaterial" +
                  ", Name" +
                  ", PriceOfUnit" +
                  ", Currency" +
                  ", Unit" +
                  ", ExchangeRate" +
                  ", BaseUnit" +
                  ", PriceOfCarton,Remark,LBOh,LBRate" +
                  ", convert(int,Formula)'Formula',IsActive,'' as aValidate from TransFormula");

                _dataTable = new DataTable();
                _dt = new DataTable();
                //dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["RowID"] };
                break;
            //case "cbCheck":
            //    var state = Convert.ToBoolean(param[1]);
            //    //for (int i = 0 * grid.PageIndex; i < grid.SettingsPager.PageSize; i++)
            //    //{
            //    //    grid.Selection.SetSelection(i, state);
            //    //}
            //    var headerCb = grid.FindHeaderTemplateControl(grid.Columns[1], "SelectAllCheckBox") as ASPxCheckBox;
            //    headerCb.Checked = state;
            //    DataRow[] rows = dataTable.Select("formula='"+ ActivePageSymbol + "'");
            //    for (int i = 0; i < rows.Length; i++)
            //    {

            //        rows[i]["IsActive"] = string.Format("{0}",(state == true ) ? 1 : 0); 
            //    }
            //    break;

            //case "Remove":
            case "Request":
                List<Object> selectItems = gridData.GetSelectedFieldValues("ID");
                List<string> lists = new List<string>();
                string _RequestNo = "";
                foreach (object selectItemId in selectItems)
                {
                    _RequestNo = cs.ReadItems(@"select b.RequestNo from TransFormulaHeader a left join TransCostingHeader b on b.ID = a.RequestNo where a.ID = 173411");
                    using (SqlConnection con = new SqlConnection(strConn))
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spInsertDocumentNo";
                        cmd.Parameters.AddWithValue("@Id", string.Format("{0}", 0));
                        cmd.Parameters.AddWithValue("@Requester", cs.CurUserName.ToString());
                        cmd.Parameters.AddWithValue("@RequestNo", string.Format("{0}", _RequestNo));
                        cmd.Parameters.AddWithValue("@Costno", string.Format("{0}", selectItemId));
                        cmd.Parameters.AddWithValue("@Assignee", string.Format("{0}", CmbReceiver.Value));
                        cmd.Parameters.AddWithValue("@Destination", string.Format("{0}", CmbCountry.Value));
                        cmd.Parameters.AddWithValue("@Country", string.Format("{0}", textBoxCountry.Text));
                        cmd.Connection = con;
                        con.Open();
                        var getValue = cmd.ExecuteScalar();
                        con.Close();
                        lists.Add(string.Format("{0}", getValue));
                    }
                }
               
                for (int i = 0; i < grid.VisibleRowCount; i++)
                {
                    ASPxCheckBox chk = grid.FindRowCellTemplateControl(i, null, "cbAll") as ASPxCheckBox;
                    
                    if (chk.Checked)
                    {
                        bool isSelected = Convert.ToBoolean(grid.GetRowValues(i, "ID"));
                        object SubID = grid.GetRowValues(i, grid.KeyFieldName);
                        var sel = grid.Selection.IsRowSelected(i);
                        _RequestNo = string.Format("{0}", hfid["hidden_value"]);
                        using (SqlConnection con = new SqlConnection(strConn))
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spInsertDocumentNo";
                            cmd.Parameters.AddWithValue("@Id", string.Format("{0}", 0));
                            cmd.Parameters.AddWithValue("@Requester", cs.CurUserName.ToString());
                            cmd.Parameters.AddWithValue("@RequestNo", string.Format("{0}", hfid["hidden_value"]));
                            cmd.Parameters.AddWithValue("@Costno", string.Format("{0}", SubID));
                            cmd.Parameters.AddWithValue("@Assignee", string.Format("{0}", CmbReceiver.Value));
                            cmd.Parameters.AddWithValue("@Destination", string.Format("{0}", CmbCountry.Value));
                            cmd.Parameters.AddWithValue("@Country", string.Format("{0}", textBoxCountry.Text));
                            cmd.Connection = con;
                            con.Open();
                            var getValue = cmd.ExecuteScalar();
                            con.Close();
                            lists.Add(string.Format("{0}", getValue));
                        }
                        DataRow dr1 = _dt.Rows.Find(SubID);
                        if (dr1 != null)
                            dr1["RequestForm"] = string.Format("{0}", Convert.ToInt32(dr1["RequestForm"]) + 1);
                    }
                }
                buildsendemail(lists, _RequestNo);
                
                break;
            case "Delete":

                for (int i = 0; i < g.VisibleRowCount; i++)
                {
                    ASPxCheckBox chk = g.FindRowCellTemplateControl(i, null, "cbAll") as ASPxCheckBox;
                    if (chk.Checked)
                    {
                        var keyValue = g.GetRowValues(i, "ID").ToString();
                        if (tcDemos.ActiveTabIndex.Equals(10))
                        {
                            var dr = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(keyValue));
                            listItems.Remove(dr);
                            using (SqlConnection con = new SqlConnection(strConn))
                            {
                                SqlCommand cmd = new SqlCommand();
                                cmd = new SqlCommand();
                                cmd.CommandText = "DELETE TranshCosting WHERE Id = @Id";
                                cmd.Parameters.AddWithValue("@Id", keyValue.ToString());
                                cmd.Connection = con;
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                        if (new[] { 1, 2, 3, 4, 5, 6, 7, 8 }.Any(tcDemos.ActiveTabIndex.Equals))
                        {
                            DataRow found = _dataTable.Rows.Find(keyValue);
                            if (found != null)
                            {

                                string code = found["Code"].ToString();
                                _dataTable.Rows.Remove(found);
                                deleteCosting(keyValue);

                                //if (!string.IsNullOrEmpty(code.ToString()))
                                //{
                                //    DataRow[] selected = _dataTable.Select(string.Format("Code='{0}'", code));
                                //    foreach (DataRow row in selected) { 
                                //            _dataTable.Rows.Remove(row);
                                //            deleteCosting(row[0]);
                                //    }
                                //}
                            }
                        }
                        if (tcDemos.ActiveTabIndex.Equals(9))
                        {
                            DataRow dr1 = _dt.Rows.Find(keyValue);
                            string _req= string.Format("reload|{0}" , dr1["RequestNo"]);
                            _dt.Rows.Remove(dr1);
                            Delformula(string.Format("{0}", keyValue));//by formula
                            g.JSProperties["cpKeyValue"] = _req;
                        }
                            if (tcDemos.ActiveTabIndex.Equals(0))
                        {
                            DataRow cust = tcustomer.Rows.Find(keyValue);
                            if (cust != null)
                            {
                                tcustomer.Rows.Remove(cust);
                                //
                                using (SqlConnection con = new SqlConnection(strConn))
                                {
                                    SqlCommand cmd = new SqlCommand();
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = "delete TransFormula where ID=@Id";
                                    cmd.Parameters.AddWithValue("@Id", keyValue.ToString());
                                    cmd.Connection = con;
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                }
                                //deleteformula(keyValue);
                            }
                        }
                    }
                }
                break;
        }
        //ASPxCheckBox cb = (ASPxCheckBox)grid.FindHeaderTemplateControl(grid.Columns[1], "SelectAllCheckBox");
        //if (cb != null)
        //{
        //    cb.Checked = selected;
        //}
        //ActivePageSymbol = e.Parameters;
        //dataTable=(DataTable)context.Session[CustomTable];
        //if (tcustomer != null)
        //{
        //DataRow[] result = dataTable.Select();
        //for (int i = 0; i < result.Length; i++)
        //    if (result[i]["IsActive"].ToString() == "1")
        //        //grid.Selection.SetSelection(i, true);
        //grid.Selection.SelectRow(i);
        //int max = Convert.ToInt32(tcustomer.AsEnumerable()
        //.Max(row => row["Formula"]));

        //g.Templates.PagerBar = new MyPagerBarTemplate(ActivePageSymbol, max);
        //g.Templates.PagerBar = new combobox(ActivePageSymbol, max);
        //g.AutoFilterByColumn(g.Columns["Formula"], ActivePageSymbol);
        //}
        //int[] numbers = { 4 };
        //foreach (int i in numbers)
        g.Toolbars[0].Items[4].ClientVisible = (tcDemos.ActiveTabIndex == 9);
        g.DataBind();
    }
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
    public string TabChanged
    {
        get
        {
            if (Session["TabChanged"] == null)
                Session["TabChanged"] = string.Format("{0}", 1);
            return (string)Page.Session["TabChanged"];
        }
        set { Page.Session["TabChanged"] = value; }
    }
    List<string> myCollection = new List<string>();
    private DataTable LoadGrid
    {
        get
        {
            //return calcutable(dataTable);
            //dataTable = ViewState[CustomTable] as DataTable;
            //dataTable = (DataTable)context.Session[CustomTable]; 
            //if (dataTable != null)
            //    return dataTable;
            //    return dataTable;
            //DataTable table;
            //bool columnCreated = false; bool columnName = false; bool columnCalculate = false;
            //if (!string.IsNullOrEmpty(FilePath))
            //{
            //    dataTable = GetTableFromExcel();
            //}
            if (tcustomer == null)
                return tcustomer;
            string dataint = hfgetvalue["NewID"].ToString();
            myCollection = new List<string>();
            foreach (DataColumn column in tcustomer.Columns)
                if (column.ColumnName.Contains("GUnit"))
                    myCollection.Add(column.ColumnName);
            //dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["RowID"] };
            //var view = tcustomer.DefaultView;
            CreateGridColumns();

            //context.Session[CustomTable] = view.Table;
            //return view.Table;
            return tcustomer;
        }
    }
    void CreateGridColumns()
    {
        //var table = view.Table;
        var myResult = tcustomer.AsEnumerable()
                   .Select(s => new
                   {
                       id = s.Field<string>("Component"),
                   })
                   .Distinct().ToList();
        //DataTable dt = new DataTable();
        //dt.Clear();
        //dt.Columns.Add("Name");
        //DataRow _ravi = dt.NewRow();
        //_ravi["Name"] = "ravi";
        //dt.Rows.Add(_ravi);

        //Grid.Columns.Clear();
        //Grid.TotalSummary.Clear();

        //foreach (DataColumn column in table.Columns)
        //{
        //    Grid.Columns.Remove(Grid.Columns[column.ColumnName]);
        //    GridViewDataColumn gridColumn = Grid.Columns[column.ColumnName] as GridViewDataColumn;

        //    bool columnCreated = false;
        //    //GridViewDataComboBoxColumn cb;
        //    if (column.ColumnName.Contains("Yield")|| column.ColumnName.Contains("PriceOfUnit")) // some condition
        //    {
        //        //Grid.Columns.Remove(Grid.Columns[column.ColumnName]);
        //        CreateGridColumn(Grid, column.ColumnName);
        //        //    cb = new GridViewDataComboBoxColumn();
        //        //    Grid.Columns.Add(cb);
        //        //    //cb.Caption = Grid.Columns[2].Caption;
        //        //    //cb.FieldName = ((GridViewDataColumn)Grid.Columns[2]).FieldName;
        //        //    //cb.PropertiesComboBox.DataSource = dsMaterial;
        //        //    //cb.PropertiesComboBox.TextField = "Name";
        //        //    //cb.PropertiesComboBox.ValueField = "ID";
        //    }
        //    else if (column.ColumnName.Contains("Component") || column.ColumnName.Contains("SubType"))
        //    {
        //        GridViewDataComboBoxColumn Comb = new GridViewDataComboBoxColumn();
        //        Comb.FieldName = "Component";
        //        Comb.PropertiesComboBox.Columns.Clear();
        //        Comb.PropertiesComboBox.Columns.Add(new ListBoxColumn("id"));
        //        Comb.PropertiesComboBox.ValueField = "id";
        //        Comb.PropertiesComboBox.TextFormatString = "{0}";
        //        Comb.PropertiesComboBox.DataSource = myResult;
        //        Grid.Columns.Add(Comb);
        //        Grid.Columns["Component"].Width = Unit.Pixel(170);
        //    }
        //    else if (column.ColumnName.Contains("Currency"))
        //    {
        //        GridViewDataComboBoxColumn CmbCurrency = new GridViewDataComboBoxColumn();
        //        CmbCurrency.FieldName = "Currency";
        //        CmbCurrency.PropertiesComboBox.Columns.Clear();
        //        CmbCurrency.PropertiesComboBox.Columns.Add(new ListBoxColumn("value"));
        //        CmbCurrency.PropertiesComboBox.ValueField = "value";
        //        CmbCurrency.PropertiesComboBox.TextFormatString = "{0}";
        //        CmbCurrency.PropertiesComboBox.DataSource = cs.builditems("select value from dbo.FNC_SPLIT('THB;USD',';')");
        //        Grid.Columns.Add(CmbCurrency);
        //    }
        //    else if (column.ColumnName.Contains("SAPMaterial"))
        //    {
        //        GridViewDataComboBoxColumn CBCol = new GridViewDataComboBoxColumn();
        //        CBCol.FieldName = "SAPMaterial";
        //        CBCol.PropertiesComboBox.Columns.Clear();
        //        CBCol.PropertiesComboBox.Columns.Add(new ListBoxColumn("RawMaterial"));
        //        CBCol.PropertiesComboBox.Columns.Add(new ListBoxColumn("Description"));
        //        CBCol.PropertiesComboBox.Columns.Add(new ListBoxColumn("Yield"));
        //        CBCol.PropertiesComboBox.Columns.Add(new ListBoxColumn("Material"));
        //        CBCol.PropertiesComboBox.Columns.Add(new ListBoxColumn("Name"));
        //        CBCol.PropertiesComboBox.ValueField = "Material";
        //        CBCol.PropertiesComboBox.TextFormatString = "{0}";
        //        CBCol.PropertiesComboBox.EnableCallbackMode = true;
        //        CBCol.PropertiesComboBox.CallbackPageSize = 10;
        //        CBCol.PropertiesComboBox.DataSource = dsYield;
        //        Grid.Columns.Add(CBCol);
        //        Grid.Columns["SAPMaterial"].Width = Unit.Pixel(170);
        //    }
        //    else if (gridColumn == null)
        //    //if (gridColumn == null)
        //    {
        //        gridColumn = new GridViewDataColumn(column.ColumnName);
        //        columnCreated = true;
        //    }
        //    //    gridColumn.DataItemTemplate = new CustomDataItemTemplate();
        //    if (columnCreated)
        //        Grid.Columns.Add(gridColumn);
        //}
        //Grid.KeyFieldName = "Col_0";
        //Grid.KeyFieldName = table.Columns["RowID"].ColumnName;
        //Grid.Columns["RowID"].Visible = false;
        //Grid.SettingsEditing.Mode = GridViewEditingMode.Batch;
        if (tcustomer != null)
        {
            int max = Convert.ToInt32(tcustomer.AsEnumerable()
                            .Max(row => row["Formula"]));
            grid.Templates.PagerBar = new MyPagerBarTemplate(ActivePageSymbol, max);
            //grid.Templates.PagerBar = new combobox(ActivePageSymbol, max);
            grid.SettingsBehavior.AllowSelectByRowClick = false;
            grid.SettingsDataSecurity.AllowEdit = true;
        }
    }
    void AddCommandColumn()
    {
        //if (!this.IsPostBack)
        //{
        GridViewCommandColumn command = new GridViewCommandColumn();
        grid.Columns.Add(command);
        command.ShowEditButton = true;
        //command.ShowUpdateButton = true;
        //command.ShowDeleteButton = true;
        //cm.ShowNewButtonInHeader = true;
        //foreach (DataColumn column in dataTable.Columns)
        //{
        //    if (column.ColumnName.Contains("Carton"))
        //int co = 0;
        //foreach (string c in myCollection)
        //{
        //co++;
        //var name = string.Format("PriceOfCarton", co);
        //grid.TotalSummary.Add(SummaryItemType.Custom, name);
        //}
        //grid.CustomSummaryCalculate += grid_CustomSummaryCalculate;
        //}
    }
    private void CreateGridColumn(ASPxGridView grid, string caption)
    {
        if (string.IsNullOrEmpty(caption)) return;
        //GridViewDataColumn col = new GridViewDataColumn(fieldName, caption);
        GridViewDataSpinEditColumn col = new GridViewDataSpinEditColumn();
        col.FieldName = caption;
        grid.Columns.Add(col);
    }
    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (!IsPostBack)
            FilePath = String.Empty;
    }
    protected void Upload_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
    {
        //string dirVirtualPath = @"\\192.168.1.193\XlsTables";
        string dirVirtualPath = @"C:\\temp";
        string dirPhysicalPath = dirVirtualPath;// Server.MapPath(dirVirtualPath);
        if (!Directory.Exists(dirPhysicalPath))
        {
            Directory.CreateDirectory(dirPhysicalPath);
        }

        string fileName = e.UploadedFile.FileName;
        //string fileFullPath = Path.Combine(dirPhysicalPath, fileName);
        FilePath = Path.Combine(dirPhysicalPath, fileName);
        //FilePath = string.Format(Server.MapPath("~/XlsTables/{0}") , e.UploadedFile.FileName);
        e.UploadedFile.SaveAs(FilePath);
        if (!string.IsNullOrEmpty(FilePath))
        {
            //Workbook book = new Workbook();
            //book.InvalidFormatException += book_InvalidFormatException;
            //book.LoadDocument(FilePath);
            ASPxSpreadsheet spreadsheet = new ASPxSpreadsheet();
            spreadsheet.Document.LoadDocument(FilePath);
            spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;
            Worksheet worksheet = spreadsheet.Document.Worksheets.ActiveWorksheet;
            //IWorkbook workbook = spreadsheet.Document;

            File.Delete(FilePath);
            //Worksheet sheet = book.Worksheets.ActiveWorksheet;
            string CanSize = string.Format("{0}", worksheet.Cells["C5"].Value);
            string RDNumber = string.Format("{0}", worksheet.Cells["C6"].Value);
            CellRange range = worksheet.GetUsedRange(); 
            DataTable table = worksheet.CreateDataTable(range, false);
            DataTableExporter exporter = worksheet.CreateDataTableExporter(range, table, false);
            exporter.CellValueConversionError += exporter_CellValueConversionError;
            exporter.Export();
            //string CanSize = table.Rows[4][2].ToString();
            //string RDNumber = table.Rows[5][2].ToString();

            //string strSQL = @"MasExchangeRat where (cast(getdate() as date) between cast(Validfrom as date) ";
            //strSQL+="and cast(validto as date)) and Company='" + CmbCompany.Text + "'";
            SqlParameter[] param = { new SqlParameter("@Id", hfRequestNo["ID"]) };
            var myresult = cs.GetRelatedResources("spExchangeRat", param); string ExchangeRate = "0";
            if (myresult.Rows.Count > 0)
                ExchangeRate = myresult.Rows[0]["Rate"].ToString();
            Page.Session.Remove("seGetMyData");
            tcustomer = new DataTable();
            tcustomer = GetTableFromExcel(table);
            tcustomer.PrimaryKey = new DataColumn[] { tcustomer.Columns["ID"] };
            //context.Session["CustomTable"] = null;
            //string Name = ""; string RefSamples = ""; string Code = "";
            //DataRow[] rows = Tablelist.Select("Formula=1");
            //for (int i = 0; i < rows.Length; i++)
            //{
            //    Name = string.Format("{0}", rows[i]["Name"]);
            //    RefSamples = string.Format("{0}", rows[i]["RefSamples"]);
            //    Code = string.Format("{0}", rows[i]["Code"]);
            //}
            string[] arr = { CanSize, RDNumber, ExchangeRate.ToString() };
            e.CallbackData = string.Join("|", arr);
            for (int i = 0; i < tcustomer.Rows.Count; i++)
            {
                DataRow row = _found(tcustomer.Rows[i]);
            }

            
        }
    }
    DataRow _found(DataRow found)
    {
        //---
        string[] array;

        //DataView dv = (DataView)dsLaborOverhead.Select(DataSourceSelectArguments.Empty);
        //if (dv == null) return found;
        //DataTable dtLBOh = dv.ToTable(); //((DataView)dsLabor.Select(DataSourceSelectArguments.Empty)).Table; 
        float sumOf = 0;// Convert.ToDouble(table.Compute("SUM(GUnit)", "Formula = " + found["Formula"]+ "And Component='Raw Material'"));
        object calc = tcustomer.Compute("SUM(GUnit)", "Formula = " + found["Formula"]);// "And Component='Raw Material'");
        if (!float.TryParse(calc.ToString(), out sumOf))
            return found;
        // var sumOf = table.AsEnumerable()
        //.Where(dr => dr.Field<string>("Formula").Equals(found["Formula"]) && dr.Field<string>("Component").Equals("Raw Material"))
        //.Sum(dr => dr.Field<double>("GUnit"));
        DataTable dtLBOh = new DataTable();
        if (!string.IsNullOrEmpty(found["LBOh"].ToString()) && dtLBOh != null)
        {
            DataRow drLBOh = dtLBOh.AsEnumerable()
                        .SingleOrDefault(r => r.Field<string>("LBCode") == found["LBOh"].ToString());
            if (drLBOh != null)
            {
                //var LBRate = (Convert.ToDouble(found["GUnit"]) / sumOf) * ((Convert.ToDouble(drLBOh["LBRate"]) / Convert.ToDouble(drLBOh["PackSize"])) *
                //    Convert.ToDouble(CmbPackSize.Text));
                var LBRate = (Convert.ToDouble(drLBOh["LBRate"]));
                found["LBRate"] = LBRate.ToString("F4");
            }
        }
        string value = "";
        var result = (found["SAPMaterial"] == null) ? string.Empty : found["SAPMaterial"].ToString();
        string comp = (CmbCompany.Value == null) ? string.Empty : CmbCompany.Value.ToString();
        if (!string.IsNullOrEmpty(result))
        {
            value = string.Format("{0}", comp.ToString() + "|" + found["SAPMaterial"] + "|" + hfRequestNo["ID"]);
            DataTable data = GetrequestRate(value);
            if (data != null)
                foreach (DataRow dr in data.Rows)
                {
                    string[] rowValueItems = Regex.Split("PriceOfUnit;Currency;Unit;ExchangeRate;BaseUnit", ";");
                    foreach (string row in rowValueItems)
                    {
                        found[row] = dr[row];
                    }
                }
        }
        if (!string.IsNullOrEmpty(found["PriceOfUnit"].ToString()))
            if (string.IsNullOrEmpty(found["Currency"].ToString()))
                found["Currency"] = "THB";

        string strBaseUnit = found["BaseUnit"].ToString();
        if (!string.IsNullOrEmpty(found["Currency"].ToString()))
        {
            var Currency = ExchangeRate(found["Currency"].ToString());
            if (!string.IsNullOrEmpty(Currency))
            {
                found["ExchangeRate"] = Currency.ToString();
                double da = 0;
                if (!string.IsNullOrEmpty(found["PriceOfUnit"].ToString()))
                    da = double.Parse(found["ExchangeRate"].ToString(), CultureInfo.InvariantCulture) * double.Parse(found["PriceOfUnit"].ToString());
                found["BaseUnit"] = da.ToString("F4");
            }
        }
        if (!string.IsNullOrEmpty(found["Unit"].ToString()))
        {
            double BaseUnit = 0;
            if (double.TryParse(found["BaseUnit"].ToString(), out BaseUnit))
                switch (found["Unit"].ToString())
                {
                    case "Ton":
                    case "MT":
                        BaseUnit = BaseUnit / 1000;
                        break;
                }
            found["BaseUnit"] = BaseUnit.ToString("F4");
        }
        array = Regex.Split("PriceOfCarton", ";");
        foreach (string arr in array)
        {
            int co = 0; string c = "GUnit";
            //foreach (string c in myCollection)
            //{
            co++;
            var name = string.Format(arr, co);
            double OutVal; double BaseUnit; double Yield; double Packsize = 0;
            if (double.TryParse(found[c].ToString(), out OutVal))
            {
                double total = OutVal / 1000;
                //table.Rows[i][name] = table.Rows[i][c].ToString();
                //var test = table.Rows[i][c].ToString();
                if (CmbCompany.Text == "101")
                {
                    var rw = _dt.Select("Formula='" + found["Formula"] + "'").FirstOrDefault();
                    if (rw != null)
                    {
                        double.TryParse(rw["Packaging"].ToString(), out Packsize);
                    }
                }
                else
                    double.TryParse(tbPackSize.Text.ToString(), out Packsize);
                double.TryParse(found["BaseUnit"].ToString(), out BaseUnit);
                double.TryParse(found["Yield"].ToString(), out Yield);
                double r = (total * BaseUnit * Packsize / (Yield / 100));
                if (double.IsNaN(r) || double.IsInfinity(r))
                    found[name] = "0";
                else
                    found[name] = r.ToString("F4");
            }
            //}
        }
        return found;
        //---
    }
    DataTable buildpackage(bool isNumeric, string Code)
    {
        SqlParameter[] param = { new SqlParameter("@Id", (string)hfRequestNo["ID"]),
                        new SqlParameter("@Company",CmbCompany.Value),
                        new SqlParameter("@Material",Code.ToString()),
                        new SqlParameter("@From", defrom.Value),
                        new SqlParameter("@To", deto.Value),
                    new SqlParameter("@Type",isNumeric==true?1:0) };
        return cs.GetRelatedResources("spselectPackage", param);
    }
    private DataTable GetTableFromExcel(DataTable table)
    {
        //Workbook book = new Workbook();
        //book.InvalidFormatException += book_InvalidFormatException;
        //book.LoadDocument(FilePath);
        //Worksheet sheet = book.Worksheets.ActiveWorksheet;
        //Range range = sheet.GetUsedRange();
        //DataTable table = sheet.CreateDataTable(range, false);
        //DataTableExporter exporter = sheet.CreateDataTableExporter(range, table, false);
        //exporter.CellValueConversionError += exporter_CellValueConversionError;
        //exporter.Export();
        //var foundRows = table.Select("Column1 like '%Remark%'");
        _dataTable.Rows.Clear();
        FilePath = String.Empty;
        var list = new List<int>();
        bool b = false;
        var str = new List<string>();
        int mx = 0;
        bool sol = false;
        bool r = false;
        //Tablelist = ((DataView)dsnamelist.Select(DataSourceSelectArguments.Empty)).Table;
        //Page.Session.Remove("setableKey");
        if (_dt == null)
        {
            namelist(string.Format("{0}", 0));
            foreach (DataRow _r in _dt.Rows)
            {
                _r["PackSize"] = string.Format("{0}", tbPackSize.Text);
                _r["Packaging"] = string.Format("{0}", CmbPackaging.Text);
                _r["NW"] = string.Format("{0}", tbNetweight.Text);
            }
        }
        var dt = new DataTable();
        DataTable testdt = _dataTable.Clone();
        int NextRowID = 0;
        dt.Columns.Add("Component", typeof(string)).SetOrdinal(0);
        dt.Columns.Add("SubType", typeof(string)).SetOrdinal(1);
        for (int i = 0; i < table.Rows.Count; i++)
        {

            //pack Primary Secondary
            string name = table.Rows[i][0].ToString().ToLower().Trim();
            if (new[] { "primary", "secondary", "upcharge", "margin", "loh"}.Any(name.StartsWith))
            {
                double dPrice = 0, dAmount = 0;
                if (!string.IsNullOrEmpty(table.Rows[i][3].ToString()))
                {
                    string mytext = table.Rows[i][3].ToString().Substring(1, 1); int n;
                    bool isNumeric = int.TryParse(mytext, out n);
                    if (table.Rows[i][3].ToString().StartsWith("5122"))
                        isNumeric = false;
                    //SqlParameter[] param = { new SqlParameter("@Id", (string)hfRequestNo["ID"]),
                    //    new SqlParameter("@Company",CmbCompany.Value),
                    //    new SqlParameter("@Material",table.Rows[i][3].ToString()),
                    //    new SqlParameter("@From", defrom.Value),
                    //    new SqlParameter("@To", deto.Value),
                    //new SqlParameter("@Type",isNumeric==true?1:0) };
                    var Results = buildpackage(isNumeric, table.Rows[i][3].ToString());//cs.GetRelatedResources("spselectPackage", param);
                    if (Results.Rows.Count > 0)
                    {
                        dPrice = Convert.ToDouble(Results.Rows[0]["Price"]);
                        dAmount = Convert.ToDouble(dPrice) * Convert.ToDouble(table.Rows[i][2]);
                    }
                    //else
                    //{
                    //    dAmount = Convert.ToDouble(table.Rows[i][4].ToString()) * Convert.ToDouble(table.Rows[i][2]);
                    //}
                    double d;
                    Double.TryParse((getloss(CmbPackaging.Text, isNumeric == true ? 2 : 3, "")), out d);
                    DataRow _ravi = testdt.NewRow();
                    string unit = "THB";
                    //if (new[] { "primary", "secondary" }.Any(name.StartsWith))
                    //{
                    //    unit = CmbSellingUnit.Text;
                    //}
                    //else if (name.StartsWith("upcharge"))
                    //{
                    //    unit = CmbUpChargeCurrency.Text;
                    //}
                    testdt.Rows.Add(NextRowID++,
                        (isNumeric == true ? "Primary Packaging" : "Secondary Packaging"),
                        table.Rows[i][3].ToString(),
                        table.Rows[i][1].ToString(),
                        table.Rows[i][2].ToString(),
                        dPrice,
                        dAmount, "",
                        "1",
                        unit
                        ,
                        (dAmount * (d / 100)), 1, "X");
                }
                else if (table.Rows[i][0].ToString().Equals("LOH"))
                {
                    string _strcol = "";
                    int Column = 4;
                    int formu = 1;
                    do
                    {
                        _strcol = table.Rows[i][Column].ToString();
                        if (_strcol != "")
                        {
                            _dataTable.Rows.Add(NextRowID++, table.Rows[i][0].ToString(),
                            string.Format("{0}", table.Rows[i][3].ToString()),
                            "",
                            string.Format("{0}", table.Rows[i][2].ToString()),
                            _strcol.ToString(),
                            Convert.ToDouble(_strcol) * Convert.ToDouble(string.Format("{0}", table.Rows[i][2])),
                            "",
                            "1",
                            "THB",
                            0, formu, "X");
                        }
                        formu++;
                        Column++;
                    } while (_strcol != "");
                }
                else if (table.Rows[i][0].ToString().Equals("MARGIN"))
                {
                    string _strcol = "";
                    int Column = 4;
                    int formu = 1;
                    do
                    {
                        _strcol = table.Rows[i][Column].ToString();
                        if (_strcol != "")
                        {
                            _dataTable.Rows.Add(NextRowID++, table.Rows[i][0].ToString(),
                            table.Rows[i][3].ToString(),
                            "",
                            table.Rows[i][2].ToString(),
                            0,
                            0,
                            _strcol.ToString(),
                            "",
                            "",
                            0, formu, "X");
                        }
                        formu++;
                        Column++;
                    } while (_strcol != "");
                }
                else
                {
                    string value = Array.Find(arrSubType,
            element => element.ToLower().StartsWith(name, StringComparison.Ordinal));
                    double.TryParse(table.Rows[i][5].ToString(), out dAmount);
                    //dAmount = Convert.ToDouble(table.Rows[i][5].ToString());
                    dAmount = Convert.ToDouble(table.Rows[i][4].ToString()) * Convert.ToDouble(table.Rows[i][2]);
                    string Per = name.StartsWith("margin") ? table.Rows[i][4].ToString() : "";
                    testdt.Rows.Add(NextRowID++, value,
                        "",
                        table.Rows[i][1].ToString(),
                        table.Rows[i][2].ToString(),
                        name.StartsWith("margin") ? "" : table.Rows[i][4].ToString(),
                        //table.Rows[i][5].ToString(), name.StartsWith("margin")? table.Rows[i][4].ToString() : "",
                        dAmount,
                        Per,
                        "",
                        "THB",
                        0, 1, "X");
                }
            }
            if (table.Rows[i][0].ToString().ToLower().Trim().StartsWith("remark"))
                r = true;
            if (r == true && table.Rows[i][1].ToString() != "")
            {
                int index = 0;
                int.TryParse(table.Rows[i][1].ToString(), out index);
                _dt.Rows.Add(i, index, table.Rows[i][2].ToString(), "", table.Rows[6][index + 3].ToString(), "", "X", "",0, string.Format("{0}", tbNetweight.Text)
                    , string.Format("{0}", tbPackSize.Text), "", "", "", "", string.Format("{0}", tbNetweight.Text), string.Format("{0}", CmbPackaging.Text));
            }
            var result = (table.Rows[i][0] == null) ? string.Empty : table.Rows[i][0].ToString();
            string Solution = table.Rows[i][1].ToString();
            if (Solution.Contains("Solution"))
            {
                b = false; sol = true;
            }
            if (!string.IsNullOrEmpty(result))
            {
                if ((string)result == "No")
                {
                    int inial = i;
                    int strx = ++inial;
                    string _strNo = "";
                    do
                    {
                        _strNo = table.Rows[strx]["Column1"].ToString();
                        if (_strNo != "") goto Ex;
                        if (!string.IsNullOrEmpty(table.Rows[strx]["Column2"].ToString()))
                        {
                            str.Add(table.Rows[strx]["Column2"].ToString());
                            b = true;
                        }
                    Ex:
                        strx++;
                    } while (_strNo == "");
                    //                    for (int x = ++inial; x < table.Columns.Count; x++)
                    //                    {
                    //                        if (!string.IsNullOrEmpty(table.Rows[x]["Column2"].ToString()))
                    //                        {
                    //                            str.Add(table.Rows[x]["Column2"].ToString());
                    //                            b = true;
                    //                            break;
                    //                        }
                    //                    }

                    for (int x = 0; x < table.Columns.Count; x++)
                    {
                        var value = (table.Rows[i][x] == null) ? string.Empty : table.Rows[i][x].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            dt.Columns.Add("Col_" + x, typeof(string));
                            list.Add(x);
                        }
                    }
                }
                double num;
                string candidate = (string)result;
                if (double.TryParse(candidate, out num))
                {
                    if (num > 0 && b == false)
                    {
                        int n = i - 1;
                        str.Add(table.Rows[n]["Column2"].ToString());
                        b = true; mx++;
                    }
                    DataRow _ravi = dt.NewRow();
                    foreach (var item in list)
                    {
                        var tresult = (table.Rows[i][item] == null) ? string.Empty : table.Rows[i][item].ToString();
                        if (!string.IsNullOrEmpty(tresult))
                            _ravi["Col_" + item] = table.Rows[i][item];
                    }
                    dt.Rows.Add(_ravi);
                    string word = table.Rows[i]["Column4"].ToString();
                    //Component
                    int ro = (int)num - 1;
                    if (!string.IsNullOrEmpty(word))
                    {
                        if ((int)word.Length > 8)
                            dt.Rows[ro][0] = "Raw Material";
                        else
                            if (str.Count > 1)
                            dt.Rows[ro][0] = str[1];
                        else
                            dt.Rows[ro][0] = str[0];
                    }
                    dt.Rows[ro][1] = str[mx];//subType
                }
            }
            if (b == true && string.IsNullOrEmpty(result) && sol == true)
            {
                b = false; //mx++;
            }
        }
        NextRowID = 0; int myi = 0; 
        myi = Convert.ToInt32(_dataTable.AsEnumerable()
                        .Max(row => row["ID"]));
        var max = _dataTable.AsEnumerable().Max(row => row["ID"]);

        var mylist = new List<dynamic>();
        DataTable dtDataTable = ConvertToDatatable("");
        int col = dt.Columns.Count; int co = 0;//GUnit1
        //DataRow[] rowtable = testdt.Select("subtype<>'LOH'");
        for (int c = 6; c < col; c++)
        {
            co++;
            
            foreach (DataRow drow in testdt.Rows)//package
            {
                myi++;
                drow["ID"] = myi;
                drow["formula"] = co;
                //t.Rows.Add(drow);
                _dataTable.ImportRow(drow);
            }
            var name = string.Format("GUnit{0}", co);
            dt.Columns[c].ColumnName = name;
            dt.AcceptChanges();
            foreach (DataRow row in dt.Rows)
            {
                double OutVal;
                if (double.TryParse(row[name].ToString(), out OutVal))
                //if (!string.IsNullOrEmpty(row[name].ToString()))
                {
                    //mylist.Add(new
                    //{
                    //    RowID = row["Col_0"],
                    //    Component = row["Component"],
                    //    SubType = row["SubType"],
                    //    Description = row["Col_1"],
                    //    SAPMaterial = row["Col_3"],
                    //    Result = row[name],
                    //    Yield="",
                    //    RawMaterial="",
                    //    Name="",
                    //    Price="",
                    //    Currency="",
                    //    Unit="",
                    //    ExchangeRate="",
                    //    BaseUnit="",
                    //    PriceOgCarton = "",
                    //    Formula=co
                    //});
                    object[] array = new object[22];
                    array[0] = NextRowID++;
                    array[1] = row["Component"];
                    array[2] = row["SubType"];
                    array[3] = row["Col_1"];
                    array[4] = row["Col_3"];
                    array[5] = OutVal;//row[name];
                    array[6] = string.Format("{0}", row["Col_2"]);
                    array[11] = "KG";
                    array[15] = co;
                    string Description = "";
                    if (!string.IsNullOrEmpty(row["Col_3"].ToString()))
                    {
                        string strSQL = @"select Name from MasMaterial where Code='" + row["Col_3"].ToString() + "'";
                        Description = cs.ReadItems(strSQL);
                        array[20] = Description.ToUpper() == string.Format("{0}", row["Col_1"].ToString().ToUpper()) ? "" : "X";
                    }
                    else
                        array[20] = "";
                    if (string.Format("{0}", row["Col_2"]) == "")
                    {
                        dsMaterial.SelectParameters.Clear();
                        dsMaterial.SelectParameters.Add("Company", CmbCompany.Value.ToString());
                        dsMaterial.SelectParameters.Add("RawMaterial", row["Col_3"].ToString());
                        dsMaterial.DataBind();
                        DataView dvsqllist = (DataView)dsMaterial.Select(DataSourceSelectArguments.Empty);
                        if (dvsqllist != null)
                        {
                            DataTable master = dvsqllist.Table;
                            DataRow[] rows = master.Select("Company = '" + CmbCompany.Value.ToString() + "' and RawMaterial = '" + (string)row["Col_3"] + "'");
                            foreach (DataRow dr in rows)
                            {
                                string[] ay = Regex.Split("Yield;Material;Name", ";"); int a = 6;
                                foreach (string arr in ay)
                                {
                                    array[a] = dr[arr]; a++;
                                }
                            }
                        }
                    }
                    if (string.Format("{0}", array[6]) == "")
                        array[6] = 100;
                    array[21] = "X";
                    dtDataTable.Rows.Add(array);
                }
            }
        }
        //string rowValue = "Yield;RawMaterial;Description;StdFishPrice;Currency;Unit;ExchangeRat;BaseUnit";
        //string[] rowValueItems = rowValue.Split(';');
        //string a = "Std{0}Carton";
        //foreach (string row in rowValueItems)
        //{
        //    if (cs.HasColumn(dt, row) == false)
        //    {
        //        dt.Columns.Add(row, typeof(string));
        //    }
        //}
        //co = 0;
        //foreach (string c in myCollection)
        //{
        //    co++;
        //    var name = string.Format(a, co);
        //    if (cs.HasColumn(dt, name) == false)
        //    {
        //        dt.Columns.Add(name, typeof(string));
        //    }
        //}
        return dtDataTable;
    }
    public DataTable ConvertToDatatable(string data)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spGetFormula";
            cmd.Parameters.AddWithValue("@data", data);
            cmd.Parameters.AddWithValue("@user", hfuser["user_name"].ToString());
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            return dt;
        }
    }
    public DataTable GetrequestRate(string Id)
    {
        string[] param = Id.Split('|');
        if (defrom.Value != null || deto.Value != null)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetRequestRate";
                cmd.Parameters.AddWithValue("@Id", Id.ToString());
                cmd.Parameters.AddWithValue("@From", defrom.Value);
                cmd.Parameters.AddWithValue("@To", deto.Value);
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
                return dt;
            }
        }
        else
            return null;
    }
    void book_InvalidFormatException(object sender, SpreadsheetInvalidFormatExceptionEventArgs e)
    {

    }
    void exporter_CellValueConversionError(object sender, CellValueConversionErrorEventArgs e)
    {
        e.Action = DataTableExporterAction.Continue;
        e.DataTableValue = null;
    }

    //protected void grid_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
    //{
    //    ASPxGridView gridView = (ASPxGridView)sender;
    //    if (e.Column.FieldName == "Std2Carton")
    //    {
    //        //decimal a = Convert.ToDecimal(e.GetListSourceFieldValue("sql_database_column_5"));
    //        //decimal b = Convert.ToDecimal(e.GetListSourceFieldValue("sql_database_column_6"));
    //        e.Value = 1;
    //    }

    //}

    //protected void grid_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
    //{
    //    e.Cell.Attributes["OfferPrice"] = e.DataColumn.FieldName;
    //}
    //protected void CmbCostingNo_Init(object sender, EventArgs e)
    //{
    //    ASPxGridLookup lookup = (ASPxGridLookup)sender;
    //    ASPxGridView gridView = lookup.GridView;
    //    gridView.CustomCallback += new ASPxGridViewCustomCallbackEventHandler(gridView_CustomCallback);
    //}

    //void gridView_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    //{
    //    ASPxGridView gridView = (ASPxGridView)sender;
    //        string parameter = e.Parameters;
    //    if (string.IsNullOrEmpty(parameter)) return;
    //        dsCostingNo.SelectCommand = string.Format("{0} WHERE StatusApp <> '5' and Company in (select value from dbo.FNC_SPLIT('{1}',';'))", dsCostingNo.SelectCommand, parameter);
    //        gridView.DataBind();
    //}

    protected void CmbCostingNo_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox comb = sender as ASPxComboBox;
        if (string.IsNullOrEmpty(e.Parameter)) return;
        string[] param = e.Parameter.Split('|');
        if (param[0] == "Build" || param[0] == "reload")
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetRequestNo";
                cmd.Parameters.AddWithValue("@Company", param[1].ToString());
                cmd.Parameters.AddWithValue("@User", user_name.ToString());
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
                con.Dispose();
                //context.Session["CustomTable"] = dataTable;
                //string strSQL = @"select ID,RequestNo,MarketingNumber,format(RequestDate,'dd-MM-yyyy')'RequireDate',PackSize, Packaging, ";
                //strSQL = strSQL + "RIGHT(CONCAT('00',Revised), 2)'Revised' from TransTechnical Where Company in (select value from dbo.FNC_SPLIT('" + param[1].ToString() + "',';')) ";
                //strSQL = strSQL + "and StatusApp = '4'";
                //comb.DataSource = cs.builditems(strSQL);
                comb.DataSource = dt;
                comb.DataBind();
            }
        }
        if (param[0] == "copy")
        {

        }
    }
    //protected void grid_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    //{
    //    var args = e.Parameters.Split('|');
    //    int id;
    //    if (!int.TryParse(args[0], out id))
    //        return;

    //    LoadGrid();
    //}

    //protected void grid_DataBound(object sender, EventArgs e)
    //{
    //    DataTable table = (DataTable)context.Session["gridDataTable"];
    //    //foreach (DataColumn column in table.Columns)
    //    //{
    //    //    //    for (int i = 0; i < table.Columns.Count; i++)
    //    //    //{
    //    //if (column.ColumnName=="Col_3") { 
    //    Grid.Columns.Remove(Grid.Columns["Col_3"]);

    //    GridViewDataComboBoxColumn CBCol = new GridViewDataComboBoxColumn();
    //    CBCol.FieldName = "Col_3";
    //    CBCol.PropertiesComboBox.Columns.Clear();
    //    CBCol.PropertiesComboBox.Columns.Add(new ListBoxColumn("SAPMaterial"));
    //    CBCol.PropertiesComboBox.Columns.Add(new ListBoxColumn("Name"));
    //    CBCol.PropertiesComboBox.ValueField = "SAPMaterial";
    //    CBCol.PropertiesComboBox.TextFormatString = "{1}";
    //    CBCol.PropertiesComboBox.DataSource = GetCBData();
    //    //CBCol.PropertiesComboBox.ValueType = Type.GetType("System.Int32");
    //    //CBCol.PropertiesComboBox.TextField = "Name";
    //    Grid.Columns.Add(CBCol);
    //    Grid.Columns["Col_3"].Width = Unit.Pixel(170);
    //    //    }
    //    //    //if (column.ColumnName.Contains("GUnit1"))
    //    //    if (column.ColumnName=="GUnit1")
    //    //    {
    //    //grid.Columns(3)
    //    Grid.Columns.Remove(Grid.Columns[3]);

    //    GridViewDataSpinEditColumn GUnit1 = new GridViewDataSpinEditColumn();
    //    GUnit1.FieldName = "GUnit1";
    //    Grid.Columns.Add(GUnit1);
    //    //    }
    //    //}
    //}
    private DataTable GetCBData(string strSQL)
    {
        DataTable dt = new DataTable();
        dt = cs.builditems(strSQL);
        return dt;
    }

    private DataTable GetGridData()
    {
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[15] { new DataColumn("ID", typeof(int)),
                            new DataColumn("Component", typeof(string)),
                            new DataColumn("Code", typeof(string)),
                            new DataColumn("Name", typeof(string)),
                            new DataColumn("Quantity", typeof(string)),
                            new DataColumn("PriceUnit", typeof(string)),
                            new DataColumn("Amount", typeof(string)),
                            new DataColumn("Per", typeof(string)),
                            new DataColumn("SellingUnit", typeof(string)),
                            new DataColumn("NW", typeof(string)),
                            //new DataColumn("Unit", typeof(string)),
                            new DataColumn("Ref", typeof(string)),
                            new DataColumn("Packaging",typeof(string)),
                            new DataColumn("PackSize",typeof(string)),
                            new DataColumn("Loss",typeof(string)),
                            new DataColumn("Formula",typeof(int))});
        dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
        return dt;
    }
    private DataTable insertsGrid(int i, string p)
    {
        DataTable dt = _dataTable;
        int NextRowID = Convert.ToInt32(_dataTable.AsEnumerable()
                .Max(row => row["ID"])) + 1; //(int)dt.Rows.Count;
                                             //string keyvalue = hfid["hidden_value"].ToString();
                                             //string PackType = cs.ReadItems(@"select Packaging from TransTechnical where ID='" + keyvalue + "'").ToString();
                                             //dt.Rows[0]["FieldName"].ToString(); 
        NextRowID++;
        var data = new List<dynamic>();
        double d; string str;
        //var tcname = tcDemos.ActiveTab.Name;
        object[] array = new object[22];
        //switch (test.Substring(2, 1))
        switch (p.ToString())
        {
            case "0":
                int iMax = Convert.ToInt32(tcustomer.AsEnumerable()
                .Max(row => row["ID"])) + 1;
                DataRow _row = tcustomer.NewRow();
                _row["ID"] = iMax;
                _row["Component"] = "";
                _row["Currency"] = "THB";
                _row["ExchangeRate"] = "1";
                _row["Formula"] = i;
                _row["aValidate"] = "";
                _row["Mark"] = "X";
                tcustomer.Rows.Add(_row);
                break;
            case "1":
                str = (string)getloss(CmbPackaging.Text, 2, "");
                if (Double.TryParse(str, out d)) // if done, then is a number
                {
                    if (tbQuantity.Text != "")
                        dt.Rows.Add(NextRowID++,
                            SubType[2],
                            CmbPackageCode.Value,
                            tbDescription.Text,
                            tbQuantity.Text,
                            tbPriceRate.Text,
                            tbAmount.Text, "",
                            CmbSellingUnit.Text.Equals("THB")?"1":seExchangeRate.Text,
                            CmbSellingUnit.Text,
                            (Convert.ToDouble(tbAmount.Text) * (d / 100)), i, "X");
                }
                break;
            case "2":
                str = (string)getloss(CmbPackaging.Text, 3, "");
                if (Double.TryParse(str, out d)) // if done, then is a number
                {
                    if (tbSecQuantity.Text != "")
                        dt.Rows.Add(NextRowID++, SubType[3],
                        CmbSecPackageCode.Value,
                        tbSecDescription.Text,
                        tbSecQuantity.Text,
                        tbSecPriceRate.Text,
                        tbSecAmount.Text, "",
                        CmbSecSellingUnit.Text.Equals("THB")?"1": seExchangeRate.Text,
                        CmbSecSellingUnit.Text,
                        (Convert.ToDouble(tbSecAmount.Text) * (d / 100)), i, "X");
                }
                break;
            case "3":
                dt.Rows.Add(NextRowID++, "Margin", CmbMargin.Text, tbMarginName.Text, 1, 0,
                    0, tbMarginRate.Text, "","", 0, i, "X");
                break;
            case "4":
                double Quantity;
                if (Double.TryParse(tbUpChargeQuantity.Text, out Quantity))
                {
                    double _amount = 0;
                    _amount = Convert.ToDouble(Quantity * Convert.ToDouble(tbUpChargePrice.Text) * (CmbUpChargeCurrency.Text.Equals("THB") ? 1 : Convert.ToDouble(seExchangeRate.Text)));
                    dt.Rows.Add(NextRowID++, "Upcharge", "", tbUpCharge.Text, tbUpChargeQuantity.Text, tbUpChargePrice.Text,
                _amount, "", CmbUpChargeCurrency.Text.Equals("THB")?"1": seExchangeRate.Text, CmbUpChargeCurrency.Text, 0, i, "X");
                }
                break;
            case "5":
                dt.Rows.Add(NextRowID++, "Labor&Overhead", "", cmbLaborOverhead.Text, tbResultLOH.Text, 0,
                    0, "","", cmbLOHType.Value, 0, i, "X");
                break;
            case "6":
                double LaborQuantity;
                if (Double.TryParse(tbLaborQuantity.Text, out LaborQuantity))
                    dt.Rows.Add(NextRowID++, "DL", "", cmbLabor.Text, tbLaborQuantity.Text, tbLaborRate.Text,
                    LaborQuantity * Convert.ToDouble(tbLaborRate.Text), "","", cmbLaborType.Value, 0, i, "X");
                break;
            case "7":
                double OHQuantity;
                if (Double.TryParse(tbOHQuantity.Text, out OHQuantity))
                    dt.Rows.Add(NextRowID++, "OH", "", cmbOHRate.Text, tbOHQuantity.Text, tbOHRate.Text,
                    OHQuantity * Convert.ToDouble(tbOHRate.Text), "","", cmbOHType.Value, 0, i, "X");
                break;
            case "8":
                double SGAQuantity;
                if (Double.TryParse(tbSGAQuantity.Text, out SGAQuantity))
                    dt.Rows.Add(NextRowID++, "SGA", "", cmbSGA.Text, tbSGAQuantity.Text, tbSGARate.Text,
                    SGAQuantity * Convert.ToDouble(tbSGARate.Text), "","", cmbSGAType.Value, 0, i, "X");
                break;
            case "10":
                int MaxRowID = cs.FindMaxValue(listItems, t => t.ID);
                //DataRow rw = tcustomer.NewRow();
                TransCosting rw = new TransCosting();
                MaxRowID++;
                rw.ID = MaxRowID;
                rw.Mark = "X";
                listItems.Add(rw);
                break;
        }
        //context.Session[seGetMyData] = dt;
        //context.Session[InsertRowsSessionKey] = data;
        return dt;
    }
    string getloss(string PackType, int i, string BU)
    {
        return (string)cs.ReadItems(@"select Loss from maspfloss where ('" + deto.Value + "' between Validfrom and Validto )" +
        " and isnull(BU,'') = case when isnull(BU,'')='' then isnull(BU,'') else '" + PackType + "' end and PackageType='" + PackType + "' and SubType='" + SubType[i] + "'");
    }
    string getloss2(DataRow dr, int i)
    {
        return (string)cs.ReadItems(@"select Loss from maspfloss where ('" + Convert.ToDateTime(dr["RequireDate"]).Date + "' between Validfrom and Validto )" +
        " and isnull(BU,'') = case when isnull(BU,'')='' then isnull(BU,'') else '" + dr["Packaging"].ToString() + "' end and PackageType='" + dr["Packaging"].ToString() + "' and SubType='" + SubType[i] + "'");
    }
    //private void BindGrid(string param)
    //{
    //    DataTable dt = (DataTable)context.Session[seGetMyData];
    //    //if (context.Session[seGetMyData] == null)
    //    //    dt = GetGridData();
    //    //else
    //    //    dt = (DataTable)context.Session[seGetMyData];
    //    if(param=="AddRow")
    //    dt = insertsGrid();
    //    testgrid.Columns.Clear();
    //    testgrid.AutoGenerateColumns = true; // needed otherwise rebind does not re-generate columns, despite it being a property of the grid in markup.
    //    //testgrid.KeyFieldName = "RowID";
    //    testgrid.DataSource = dt;
    //    testgrid.DataBind();

    //    //GridViewDataColumn idCol = (GridViewDataColumn)testgrid.Columns["RowID"];
    //    //idCol.ReadOnly = true;
    //    //idCol.EditFormSettings.Visible = DevExpress.Utils.DefaultBoolean.False;
    //}
    private void AddColumns()
    {
        grid.Columns.Clear();
        //AddCommandColumn();
        var index = tcDemos.ActiveTabIndex;
        var values = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        if (_CurrentTableCol == null)
            _CurrentTableCol = GetCurrentTable();
        //DataRow[] result = _CurrentTableCol.Select(string.Format("Marks='{0}'", index));
        foreach (DataRow data in _CurrentTableCol.Rows)
        {
            if (data["Marks"].ToString().Split(';').Any(index.ToString().Equals))
            {
                string[] args = data["Name"].ToString().Split(';');
                for (int x = 0; x < args.Length; x++)
                {
                    switch (args[x])

                    {
                        case "Component":
                            GridViewDataComboBoxColumn Comb = new GridViewDataComboBoxColumn();
                            if (tcustomer != null)
                            {
                                var ValuetoReturn = (from Rows in tcustomer.AsEnumerable()
                                                     select Rows[args[x]]).Distinct().ToList();
                                ValuetoReturn.Add(string.Empty);
                                ValuetoReturn.Add("Raw Material");

                                var sortedTable = ValuetoReturn.Distinct().OrderBy(q => q.ToString());
                                Comb.PropertiesComboBox.DataSource = sortedTable;
                            }
                            Comb.FieldName = args[x];
                            grid.Columns.Add(Comb);
                            grid.Columns[args[x]].Width = Unit.Pixel(170);
                            break;
                        case "SubType":

                            GridViewDataComboBoxColumn Compo = new GridViewDataComboBoxColumn();
                            if (index == 0)
                            {
                                if (tcustomer != null)
                                {
                                    var ValuetoReturn = (from Rows in tcustomer.AsEnumerable()
                                                         select Rows[args[x]]).Distinct().ToList();
                                    ValuetoReturn.Add(string.Empty);
                                    ValuetoReturn.Add("Raw Material");

                                    var sortedTable = ValuetoReturn.Distinct().OrderBy(q => q.ToString());
                                    Compo.PropertiesComboBox.DataSource = sortedTable;
                                }
                            }
                            else
                            {
                                string strSQL = @"select value from dbo.FNC_SPLIT('Primary Packaging;Secondary Packaging;Margin;Upcharge;Labor&Overhead;Labor;OH;SG&A;Loss',';')";
                                Compo.PropertiesComboBox.DataSource = cs.builditems(strSQL);
                                Compo.PropertiesComboBox.Columns.Add(new ListBoxColumn("value"));
                                Compo.PropertiesComboBox.ValueField = "value";
                                Compo.PropertiesComboBox.TextFormatString = "{0}";
                            }
                            Compo.FieldName = args[x];
                            grid.Columns.Add(Compo);
                            grid.Columns[args[x]].Width = Unit.Pixel(170);
                            break;
                        case "LBOh":

                            var deLBOh = new GridViewDataButtonEditColumn();
                            var redirectButton = new EditButton();
                            redirectButton.ImagePosition = ImagePosition.Top;
                            redirectButton.Position = ButtonsPosition.Right;
                            deLBOh.PropertiesButtonEdit.Buttons.Add(redirectButton);
                            deLBOh.PropertiesButtonEdit.ClientSideEvents.ButtonClick = "function(s, e) { OnButtonClick('LBOh'); }";
                            deLBOh.FieldName = args[x];
                            grid.Columns.Add(deLBOh);
                            break;
                        case "ID":
                            GridViewDataTextColumn column = new GridViewDataTextColumn();
                            column.FieldName = args[x];
                            column.Caption = args[x];
                            column.DataItemTemplate = check;
                            column.Width = Unit.Pixel(45);
                            column.FixedStyle = GridViewColumnFixedStyle.Left;
                            grid.Columns.Add(column);
                            break;
                        //    GridViewDataTextColumn column = new GridViewDataTextColumn();
                        //    column.FieldName = args[x];
                        //    column.Caption = args[x];
                        //    column.DataItemTemplate = check;
                        //    column.Width = Unit.Pixel(45);
                        //    column.FixedStyle = GridViewColumnFixedStyle.Left;
                        //    grid.Columns.Add(column);
                        //    break;
                        case "Packaging":

                            GridViewDataComboBoxColumn cb2 = new GridViewDataComboBoxColumn();
                            cb2.FieldName = args[x];
                            cb2.Caption = args[x];
                            cb2.Width = Unit.Pixel(45);
                            cb2.PropertiesComboBox.DataSource = dsPrimary;
                            cb2.PropertiesComboBox.Columns.Add(new ListBoxColumn("Name"));
                            cb2.PropertiesComboBox.TextFormatString = "{0}";
                            cb2.PropertiesComboBox.ValueField = "Name";
                            grid.Columns.Add(cb2);
                            break;
                        //else if (args[x] == "Unit" && index.Equals(9))
                        //{
                        //    GridViewDataComboBoxColumn cbunit = new GridViewDataComboBoxColumn();
                        //    cbunit.FieldName = args[x];
                        //    cbunit.Caption = args[x];
                        //    cbunit.Width = Unit.Pixel(45);
                        //    cbunit.PropertiesComboBox.DataSource = cs.builditems("select * from dbo.FNC_SPLIT('Grams,Ounces,Lbs,KG',',')");
                        //    cbunit.PropertiesComboBox.TextField = "value";
                        //    grid.Columns.Add(cbunit);
                        //}
                        case "SAPMaterial":

                            var de1 = new GridViewDataButtonEditColumn();
                            var redirectButton1 = new EditButton();
                            redirectButton1.ImagePosition = ImagePosition.Top;
                            redirectButton1.Position = ButtonsPosition.Right;
                            de1.PropertiesButtonEdit.Buttons.Add(redirectButton1);
                            de1.PropertiesButtonEdit.ClientSideEvents.ButtonClick = "function(s, e) { OnButtonClick('SAPMaterial'); }";
                            de1.FieldName = args[x];
                            grid.Columns.Add(de1);
                            break;
                        case "RawMaterial":

                            var deraw = new GridViewDataButtonEditColumn();
                            var rerawdirectButton = new EditButton();
                            rerawdirectButton.ImagePosition = ImagePosition.Top;
                            rerawdirectButton.Position = ButtonsPosition.Right;
                            deraw.PropertiesButtonEdit.Buttons.Add(rerawdirectButton);
                            deraw.PropertiesButtonEdit.ClientSideEvents.ButtonClick = "function(s, e) { OnButtonClick('RawMaterial'); }";
                            deraw.FieldName = args[x];
                            grid.Columns.Add(deraw);
                            break;
                        case "Code":

                            var de2 = new GridViewDataButtonEditColumn();
                            var redirectButton2 = new EditButton();
                            redirectButton2.ImagePosition = ImagePosition.Top;
                            redirectButton2.Position = ButtonsPosition.Right;
                            de2.PropertiesButtonEdit.Buttons.Add(redirectButton2);
                            de2.PropertiesButtonEdit.ClientSideEvents.ButtonClick = "function(s, e) { OnButtonClick2(''); }";
                            de2.FieldName = args[x];
                            grid.Columns.Add(de2);
                            break;
                        case "Currency":

                            GridViewDataComboBoxColumn cbunit = new GridViewDataComboBoxColumn();
                            cbunit.FieldName = args[x];
                            cbunit.Caption = args[x];
                            cbunit.Width = Unit.Pixel(45);
                            cbunit.PropertiesComboBox.DataSource = dsCurrency;
                            cbunit.PropertiesComboBox.Columns.Add(new ListBoxColumn("value"));
                            cbunit.PropertiesComboBox.TextFormatString = "{0}";
                            cbunit.PropertiesComboBox.ValueField = "value";
                            cbunit.PropertiesComboBox.ClientSideEvents.SelectedIndexChanged = "rate_SelectedIndexChaged";
                            grid.Columns.Add(cbunit);
                            break;
                        case "Formula":

                            GridViewDataTextColumn f = new GridViewDataTextColumn();
                            f.EditFormSettings.Visible = DevExpress.Utils.DefaultBoolean.False;
                            f.Width = Unit.Pixel(0);
                            f.FieldName = args[x];
                            f.Caption = args[x];
                            grid.Columns.Add(f);
                            break;
                        default:

                            GridViewDataTextColumn column2 = new GridViewDataTextColumn();
                            column2.FieldName = args[x];
                            column2.Caption = args[x];
                            grid.Columns.Add(column2);
                            break;
                    }
                }
            }
        }
        GridViewDataColumn col = new GridViewDataColumn();
        col.Visible = false;
        col.EditFormSettings.Visible = DevExpress.Utils.DefaultBoolean.True;
        //ButtonTemplate template = new ButtonTemplate();
        //col.EditItemTemplate = template;
        //grid.Columns.Add(col);

        //GridViewCommandColumn colsel = new GridViewCommandColumn();
        //colsel.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        //colsel.Width = Unit.Pixel(45);
        //colsel.Name = "Command";
        //colsel.ShowSelectCheckbox = true;
        //colsel.HeaderCaptionTemplate = check;

        //colsel.FixedStyle = GridViewColumnFixedStyle.Left;
        //g.Columns.Add(colsel);
        //g.Columns["Command"].VisibleIndex = 0;
        grid.KeyFieldName = "ID";
        var lis = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        var arr = new[] { 10 };
        int TabIndex = tcDemos.ActiveTabIndex;
        //            g.Columns[0].Visible = false;
        grid.FilterExpression = String.Empty;
        if (tcustomer != null && lis.Any(TabIndex.Equals))
        {
            int max = Convert.ToInt32(tcustomer.AsEnumerable()
            .Max(row => row["Formula"]));
            grid.Templates.PagerBar = new MyPagerBarTemplate(ActivePageSymbol, max);
            grid.AutoFilterByColumn(grid.Columns["Formula"], ActivePageSymbol);
        }
        else if (arr.Any(TabIndex.Equals))
        {
            grid.AutoFilterByColumn(grid.Columns["Formula"], "0");
        }
    }
    private DataTable GetTable
    {
        get
        {
            //You can store a DataTable in the session state
            //if (t != null)
            //    return t;
            var values = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var index = tcDemos.ActiveTabIndex;
            if (index == 0 && tcustomer != null)
            {
                if (tcustomer.Rows.Count == 0) return null;
                //grid.Templates.PagerBar = LoadTemplate("CustomPager.ascx");
                DataRow[] dr = tcustomer.Select(string.Format("Formula={0}", ActivePageSymbol));
                if (dr.Length == 0) return null;
                DataTable dt1 = dr.CopyToDataTable();
                //dt.Rows.Add(dr);
                int co = dt1.Rows.Count;
                return dt1;
            }
            else if (values.Any(index.Equals))
            {
                if (_dataTable == null) return null;
                //if (_dataTable.Rows.Count == 0) return _dataTable;
                //{
                //    _dataTable = new DataTable();
                //    if (string.IsNullOrEmpty(hfFolio["Folio"].ToString()))
                //        _dataTable = GetGridData();
                //}
                //int max = Convert.ToInt32(_dataTable.AsEnumerable()
                //.Max(row => row["Formula"]));
                //grid.Templates.PagerBar = new MyPagerBarTemplate(ActivePageSymbol, max);
                if (_dataTable.Rows.Count == 0) return null;
                DataRow[] dr = _dataTable.Select(string.Format("Formula={0}", ActivePageSymbol));
                if (dr.Length == 0) return null;
                DataTable dt1 = dr.CopyToDataTable();
                int co = dt1.Rows.Count;
                return dt1;

            }
            else if (index == 9)
                return _dt;
            else if (index == 10)
                return MyToDataTable.ToDataTable(listItems);
            else
                return null;
        }
    }
    void savetestgrid(string myfolio)
    {
        if (myfolio == "0") return;
        //        using (SqlConnection con = new SqlConnection(strConn))
        //        {
        //            SqlCommand cmd = new SqlCommand();
        //            cmd = new SqlCommand();
        //            cmd.CommandText = "DELETE TransCosting WHERE RequestNo = @Id";
        //            cmd.Parameters.AddWithValue("@Id", myfolio.ToString());
        //            cmd.Connection = con;
        //            con.Open();
        //            cmd.ExecuteNonQuery();
        //            con.Close();
        //        }
        foreach (DataRow row in _dataTable.Rows)
        {
            //var c = testgrid.GetRowValues(i, "Component");
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spinsertCostingItems";
                cmd.Parameters.AddWithValue("@ID", row["ID"].ToString());
                cmd.Parameters.AddWithValue("@RequestNo", myfolio.ToString());
                cmd.Parameters.AddWithValue("@Component", row["SubType"]);
                cmd.Parameters.AddWithValue("@SAPMaterial", string.Format("{0}", row["Code"]));
                cmd.Parameters.AddWithValue("@Description", row["Name"]);
                cmd.Parameters.AddWithValue("@Quantity", row["Quantity"]);
                cmd.Parameters.AddWithValue("@PriceUnit", row["PriceUnit"]);
                cmd.Parameters.AddWithValue("@Amount", row["Amount"]);
                cmd.Parameters.AddWithValue("@ExchangeRate", row["ExchangeRate"]);
                
                cmd.Parameters.AddWithValue("@Per", row["Per"]);
                cmd.Parameters.AddWithValue("@SellingUnit", row["Currency"]);
                cmd.Parameters.AddWithValue("@Loss", row["Loss"]);
                cmd.Parameters.AddWithValue("@Formula", row["Formula"]);
                cmd.Parameters.AddWithValue("@CreateBy", hfuser["user_name"].ToString());
                cmd.Parameters.AddWithValue("@Mark", row["Mark"].ToString());
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
    protected void GridView1_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters)) return;
        string[] param = e.Parameters.Split('|');
        if (param[0] == "copy")
        {
            ods.SelectParameters.Clear();
            ods.SelectParameters.Add("Id", param[1].ToString());
            ods.DataBind();
            //DataView dvsqllist = (DataView)ods.Select(DataSourceSelectArguments.Empty);
            //if (dvsqllist != null)
            //{
            //    DataTable dt = dvsqllist.Table;
            //    g.DataSource = dt;
            //    g.DataBind();
            //}
        }
        g.DataBind();
    }
    void testgrid_reload(string Key)
    {
        //Page.Session.Remove("seGetMyData");
        //Page.Session.Remove(sessionKey);
        //Page.Session.Remove("setableKey");
        if (Key == "0")
            _dataTable = new DataTable();
        SqlParameter[] p = { new SqlParameter("@param", Key.ToString()) };
        _dataTable = cs.GetRelatedResources("spGetCostingItem", p);
        _dataTable.PrimaryKey = new DataColumn[] { _dataTable.Columns["ID"] };
        //update++
        _update("foo|" + Key);
    }
    void _update(string Keys)
    {
        if (string.IsNullOrEmpty(Keys)) return;
        string[] param = Keys.Split('|');
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spGetUpdateIsactive";
            cmd.Parameters.AddWithValue("@param", param[1]);
            cmd.Connection = con;
            con.Open();
            var getValue = cmd.ExecuteScalar();
            con.Close();
            if (getValue.ToString() == "0")
                return;
        }
        switch (param[0])
        {
            case "raw":
                for (int i = 0; i < tcustomer.Rows.Count; i++)
                {
                    DataRow row = _found(tcustomer.Rows[i]);
                }
                break;
            case "foo":
                foreach (DataRow found in _dataTable.Rows)
                {
                    string value = "";
                    var result = (found["Code"] == null) ? string.Empty : found["Code"].ToString();
                    string comp = (CmbCompany.Value == null) ? string.Empty : CmbCompany.Value.ToString();
                    if (!string.IsNullOrEmpty(result))
                    {
                        value = string.Format("{0}", comp.ToString() + "|" + found["Code"] + "|" + hfRequestNo["ID"]);
                        DataTable data = GetrequestRate(value);
                        if (data != null)
                            foreach (DataRow dr in data.Rows)
                            {
                                string[] rowValueItems = Regex.Split("PriceOfUnit", ";");
                                foreach (string row in rowValueItems)
                                {
                                    double number = 1.5362;
                                    double _priceUnit = 0;
                                    double.TryParse(found["PriceUnit"].ToString(), out _priceUnit);

                                    double rounded = Math.Round(_priceUnit);
                                    //rounds number to 2

                                    double rounded_2 = Math.Round(_priceUnit, 2);
                                    found["PriceUnit"] = dr[row];
                                    
                                    found["Amount"] = Convert.ToDouble(found["Quantity"]) * Math.Round(_priceUnit, 2);
                                    //found["Mark"] = "X";
                                }
                            }
                    }
                }
                break;
        }
    }
    protected void testgrid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //DataTable table = (DataTable)context.Session[seGetMyData];
        if (string.IsNullOrEmpty(e.Parameters)) return;
        string[] param = e.Parameters.Split('|');
        if (param[0] == "copy")
        {
            int max = Convert.ToInt32(tcustomer.AsEnumerable()
               .Max(row => row["Formula"]));
            for (int i = 1; i <= max; i++)
            {
                _dataTable = insertsGrid(i, param[1]);
            }
        }
        if (param[0] == "AddRow")
        {
            //throw new MyException("Data modifications are not allowed.");
            //    data = new List<dynamic>();
            //var data = context.Session[InsertRowsSessionKey] as List<dynamic>;
            _dataTable = insertsGrid(Convert.ToInt32(ActivePageSymbol), param[1]);
        }
        if (param[0] == "reload" || param[0] == "priority")
        {
            //Session.Clear();
            Session.Remove("seGetMyData");
            Session.Remove(sessionKey);
            if (param[1] == "0")
                _dataTable = new DataTable();
            //t.Rows.Clear();
            //int NextRowID = 0;
            //if (t== null)
            //t = GetGridData();
            //else
            //    table = (DataTable)context.Session[seGetMyData];
            //DataTable table = new DataTable();
            //string strSQL = "select ID,Component,SAPMaterial as 'Code',Description as 'Name',"+
            //    " Quantity,PriceUnit,Amount,Per,SellingUnit,Loss from TransCosting where RequestNo='" + param[1] + "'";
            //t  = cs.builditems(strSQL);
            //string strSQL = "select ID,Component,SAPMaterial as 'Code',Description as 'Name'," +
            //" Quantity,PriceUnit,Amount,Per,SellingUnit,Loss,Formula,'' as Mark from TransCosting where RequestNo='" + param[1].ToString() + "'";
            //t = cs.builditems(strSQL);
            //t.PrimaryKey = new DataColumn[] { t.Columns["ID"] };
            testgrid_reload(param[1]);
            //t = GetTable;
        }
        if (param[0] == "symbol")
        {
            ActivePageSymbol = param[1].ToString();
            //_dataTable = (DataTable)g.DataSource;
        }
        if (param[0] == "TabChanged")
        {
            TabChanged = param[1].ToString();
        }
        if (param[0] == "Remove" || param[0] == "Delete")
        {
            var list = new List<dynamic>();
            List<object> fieldValues = g.GetSelectedFieldValues(new string[] { "ID" });
            if (fieldValues.Count == 0)
                return;
            else
            {
                foreach (object item in fieldValues)
                {
                    list.Add(item);
                    DataRow found = _dataTable.Rows.Find(item);
                    if (found != null)
                    {
                        string code = found["Code"].ToString();
                        _dataTable.Rows.Remove(found);
                        deleteCosting(item);
                        //if (!string.IsNullOrEmpty(code.ToString()))
                        //{
                        //    DataRow[] selected = _dataTable.Select(string.Format("Code='{0}'", code));
                        //    foreach (DataRow row in selected) { 
                        //            _dataTable.Rows.Remove(row);
                        //            deleteCosting(row[0]);
                        //    }
                        //}
                    }
                }
            }
            string joinRowID = string.Join(",", list);
        }
        if (param[0] == "ChangeValidTo")
        {
            foreach (DataRow _xdr in _dataTable.Rows)
            {
                if (_xdr["Code"].ToString().Length > 0)
                {
                    string mytext = _xdr["Code"].ToString().Substring(1, 1); int n;
                    bool isNumeric = int.TryParse(mytext, out n);
                    if (_xdr["Code"].ToString().StartsWith("5122"))
                        isNumeric = false;
                    var Results = buildpackage(isNumeric, _xdr["Code"].ToString());

                    if (Results.Rows.Count > 0)
                    {
                        _xdr["PriceUnit"] = Convert.ToDouble(Results.Rows[0]["Price"]);
                        _xdr["Amount"] = Convert.ToDouble(_xdr["Quantity"]) * Convert.ToDouble(_xdr["PriceUnit"]);
                    }
                }
            }
        }
        if (param[0] == "DeleteXX")
        {
            //List<Object> selectItems = testgrid.GetSelectedFieldValues("RowID");
            //foreach (object selectItemId in selectItems)
            //{
            //    table.Rows.Remove(table.Rows.Find(selectItemId));
            //}
            //DataRow[] drr = t.Select("RowID=' " + param[1] + " ' ");
            //foreach (var row in drr)
            //    row.Delete();
            DataRow[] addedRows = _dataTable.Select("Formula='" + ActivePageSymbol + "'");
            foreach (DataRow _ddr in addedRows)
            {
                DataRow found = _dataTable.Rows.Find(_ddr["ID"]);
                if (found["Mark"].ToString() == "X")
                    _dataTable.Rows.Remove(found);
                else
                    found["Mark"] = "D";
            }
            //t.Rows.Clear();
            //context.Session[seGetMyData] = table;
            //BindGrid(param[0]);
            //testgrid.DataBind();
            //testgrid.Selection.UnselectAll();
        }
        //if (param[0] == "DblClick")
        //{
        //int index = 5;
        //int.TryParse(param[1], out index);
        //DataRow found = t.Rows.Find(param[1]);
        //GridViewEditingMode mode = (GridViewEditingMode)Enum.Parse(typeof(GridViewEditingMode), "Batch");
        //var commandColumn = g.Columns[0] as GridViewCommandColumn;
        //commandColumn.Visible = !object.Equals(mode, GridViewEditingMode.Batch);
        //g.SettingsEditing.Mode = (GridViewEditingMode)index;
        //tbMarginName.Text = found["Name"].ToString();
        //TabPage tp = ((TabbedLayoutGroup)(formLayout.Items[2])).PageControl.TabPages.FindByText("Item1");
        //TabbedLayoutGroup group = (TabbedLayoutGroup)formLayout.Items[0];
        //group.PageControl.ActiveTabIndex = 2;
        //}
        if (param[0] == "post")
        {
            //DataTable data = (DataTable)context.Session[seGetMyData];
            //context.Session[InsertRowsSessionKey] = new List<dynamic>();
            //var data = context.Session[InsertRowsSessionKey] as List<dynamic>;
            if (_dataTable != null)
            {
                //change Margin
                //int Revised = 0;
                string myfolio = null;
                //clear
                //if (string.IsNullOrEmpty(myfolio))
                savetestgrid(myfolio == null ? param[2] : myfolio);
                //myfolio = context.Session["Folio"].ToString();
            }
        }
        //context.Session[seGetMyData] = table;
        //testgrid.Columns.Clear();
        if (_dataTable != null)
        {
            int i = Convert.ToInt32(TabChanged == null ? "0" : TabChanged);
            int max = Convert.ToInt32(_dataTable.AsEnumerable()
                .Max(row => row["Formula"]));
            //g.Templates.PagerBar = new MyPagerBarTemplate("2", max);
            g.Templates.PagerBar = new MyPagerBarTemplate(ActivePageSymbol, max);
            //g.Templates.PagerBar = new combobox(ActivePageSymbol, max);
            //g.AutoFilterByColumn(g.Columns["Formula"], ActivePageSymbol);
            //g.AutoFilterByColumn(g.Columns["Component"], string.Format("{0}", arrSubType[i]));
        }
        //(testgrid.Columns["myCommandColumn"] as GridViewColumn).Visible = false;
        //testgrid.DataSource = t;
        g.DataBind();
        //Update();
    }
    void deleteCosting(object keyValue)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.CommandText = "DELETE TransCosting WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", keyValue.ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    string revised(string valuechange, string keys, string NewExchangeRate)
    {
        string myfolio;
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spcopyCostingRevised";
            cmd.Parameters.AddWithValue("@Id", keys.ToString());
            cmd.Parameters.AddWithValue("@Requester", hfuser["user_name"].ToString());
            cmd.Parameters.AddWithValue("@Per", valuechange.ToString());
            cmd.Parameters.AddWithValue("@ExchangeRate", NewExchangeRate);
            cmd.Connection = con;
            con.Open();
            var getValue = cmd.ExecuteScalar();
            con.Close();
            myfolio = getValue.ToString();
        }
        return myfolio;
    }
    void deleteformula(string myfolio)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spdeleteformula";
            cmd.Parameters.AddWithValue("@Id", myfolio.ToString());
            cmd.Parameters.AddWithValue("@user", hfuser["user_name"].ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            con.Dispose();
        }
    }
    //protected void testgrid_DataBound(object sender, EventArgs e)
    //{
    //    ASPxGridView grid = sender as ASPxGridView;
    //    if (grid.Columns.IndexOf(grid.Columns["CommandColumn"]) != -1)
    //        return;
    //    GridViewCommandColumn col = new GridViewCommandColumn();
    //    col.Name = "CommandColumn";
    //    col.ShowDeleteButton = true;
    //    col.VisibleIndex = 0;
    //    grid.Columns.Add(col);
    //}

    protected void grid_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e)
    {
        var values = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        foreach (var args in e.InsertValues)
        {
            if (tcDemos.ActiveTabIndex == 10)
            {
                //int MaxRowID = cs.FindMaxValue(listItems, t => t.ID);
                ////DataRow rw = tcustomer.NewRow();
                //TransCosting rw = new TransCosting();
                //MaxRowID++;
                //rw.ID = MaxRowID;
                //listItems.Add(rw);
            }
            else
            {
                DataRow row = tcustomer.NewRow();
                int NextRowID = tcustomer.Rows.Count;
                NextRowID++;
                row["ID"] = NextRowID;
                foreach (DataColumn column in tcustomer.Columns)
                {
                    if (!column.ColumnName.Contains("ID"))
                    {
                        switch (column.ColumnName)
                        {
                            case "Mark":
                                row[column.ColumnName] = "X";
                                break;
                            case "Formula":
                                row[column.ColumnName] = ActivePageSymbol;
                                break;
                            case "Currency":
                                row[column.ColumnName] = "THB";
                                break;
                            case "Unit":
                                row[column.ColumnName] = "KG";
                                break;
                            case "GUnit":
                            case "Yield":
                                row[column.ColumnName] = args.NewValues[column.ColumnName] == null ? "0" : args.NewValues[column.ColumnName];
                                break;
                            default:
                                row[column.ColumnName] = args.NewValues[column.ColumnName];
                                break;
                        }
                    }
                }
                tcustomer.Rows.Add(row);
            }
        }

        foreach (var args in e.UpdateValues)
        {
            if (tcDemos.ActiveTabIndex.Equals(10))
            {
                var dr = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(args.Keys["ID"]));
                LoadNewValues(dr, args.NewValues);
            }
            else if (tcDemos.ActiveTabIndex.Equals(9))
            {
                DataRow dr1 = _dt.Rows.Find(args.Keys["ID"]);
                if (dr1 != null)
                {
                    foreach (DataColumn column in _dt.Columns)
                    {

                        if (! new[] { "RequestForm","ID" }.Any(column.ColumnName.Equals))
                        {
                            if (column.ColumnName == "Formula")
                            {
                                int f = 0;
                                if (int.TryParse(ActivePageSymbol, out f))
                                    dr1[column.ColumnName] = f;
                            }
                            else
                            {
                                dr1[column.ColumnName] = args.NewValues[column.ColumnName];
                            }
                        }
                    }
                }
            }
            else if (values.Any(tcDemos.ActiveTabIndex.Equals))
            {
                DataRow dr2 = _dataTable.Rows.Find(args.Keys["ID"]);
                if (dr2 != null)
                {
                    foreach (DataColumn column in _dataTable.Columns)
                    {
                        if (!column.ColumnName.Contains("ID"))
                        {
                            switch (column.ColumnName)
                            {
                                case "Formula":
                                    int f = 0;
                                    if (int.TryParse(ActivePageSymbol, out f))
                                        dr2[column.ColumnName] = f;
                                    break;

                                case "Amount":
                                    dr2[column.ColumnName] = "0";
                                    if (Convert.ToString(args.NewValues["PriceUnit"]) != "")
                                    {
                                        decimal _Quantity = 0;
                                        decimal.TryParse(Convert.ToString(args.NewValues["Quantity"]), out _Quantity);
                                        dr2[column.ColumnName] = Convert.ToString(_Quantity * Convert.ToDecimal(args.NewValues["PriceUnit"]));
                                    }
                                    break;
                                default:
                                    dr2[column.ColumnName] = args.NewValues[column.ColumnName];
                                    break;
                            }

                        }
                    }
                }
            }
            else if (tcDemos.ActiveTabIndex.Equals(0))
            {
                DataRow[] tdr = tcustomer.Select("ID=" + args.Keys["ID"].ToString());// + "and Component in ('Raw Material','Ingredient')");
                foreach (DataRow dr in tdr)
                {
                    if (dr != null)
                    {
                        foreach (DataColumn column in tcustomer.Columns)
                        {
                            if (!column.ColumnName.Contains("ID"))
                            {
                                switch (column.ColumnName)
                                {
                                    case "Formula":
                                        int f = 0;
                                        if (int.TryParse(ActivePageSymbol, out f))
                                            dr[column.ColumnName] = f;
                                        break;
                                    default:
                                        dr[column.ColumnName] = args.NewValues[column.ColumnName];
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
        foreach (var args in e.DeleteValues)
        {
            if (tcDemos.ActiveTabIndex.Equals(0))
            {
                DataRow[] tfound = tcustomer.Select("ID=" + args.Keys["ID"].ToString());// + "and Component in ('Raw Material','Ingredient')");
                foreach (DataRow found in tfound)
                    tcustomer.Rows.Remove(found);
            }
            else if (tcDemos.ActiveTabIndex.Equals(9))
            {
                DataRow dr1 = _dt.Rows.Find(args.Keys["ID"]);
                _dt.Rows.Remove(dr1);
                Delformula(string.Format("{0}", args.Keys["ID"]));//by formula
            }
        }

        e.Handled = true;
    }
    void Delformula(string keyValue)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spDelformula";
            cmd.Parameters.AddWithValue("@Id", string.Format("{0}", keyValue));
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    protected void LoadNewValues(TransCosting item, OrderedDictionary values)
    {
        //item.ID = Convert.ToInt32(values["ID"]);
        item.SubType = Convert.ToString(values["SubType"]);
        item.Code = Convert.ToString(values["Code"]);
        item.Name = Convert.ToString(values["Name"]);

        item.Per = Convert.ToString(values["Per"]);
        item.PriceUnit = Convert.ToString(values["PriceUnit"]);
        item.Currency = Convert.ToString(values["Currency"]);
        item.Quantity = Convert.ToString(values["Quantity"]);
        if (Convert.ToString(values["PriceUnit"]) != "")
        {
            decimal _Quantity = 0;
            decimal.TryParse(Convert.ToString(values["Quantity"]), out _Quantity);
            item.Amount = Convert.ToString(_Quantity * Convert.ToDecimal(item.PriceUnit));
        }
    }
    protected void testgrid_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e)
    {

        foreach (var args in e.DeleteValues)
        {
            DataRow found = _dataTable.Rows.Find(args.Keys["Id"]);
            _dataTable.Rows.Remove(found);
        }
        foreach (var args in e.UpdateValues)
        {
            DataRow found = _dataTable.Rows.Find(args.Keys["Id"]);
            foreach (DataColumn column in _dataTable.Columns)
            {
                if (!column.ColumnName.Contains("ID"))
                {
                    found[column.ColumnName] = args.NewValues[column.ColumnName];
                }
            }
        }
        e.Handled = true;
    }
    protected void testgrid_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
    {
        ////ASPxGridView grid = sender as ASPxGridView;
        //int index = (sender as ASPxGridView).FindVisibleIndexByKeyValue(e.Keys[0]);
        //DataTable dt = (DataTable)context.Session[seGetMyData];
        //dt.Rows[index].Delete();
        //context.Session[seGetMyData] = dt;
        //BindGrid();
        ASPxGridView grid = sender as ASPxGridView;
        //DataTable table = (DataTable)context.Session[seGetMyData];
        DataRow found = _dataTable.Rows.Find(e.Keys[0]);
        _dataTable.Rows.Remove(found);
        //context.Session[seGetMyData] = t;
        e.Cancel = true;
    }
    protected void grid_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //DataTable table = LoadGrid;
        //DataRow dr = tcustomer.Rows.Find(e.Keys[0]);
        //foreach (DataColumn column in tcustomer.Columns)
        //{
        //    if (!column.ColumnName.Contains("ID"))
        //    {
        //        dr[column.ColumnName] = e.NewValues[column.ColumnName];
        //    }
        //}
        //dr = _found(dr);
        //UpdateData(g);
        //test(table);
        //calculatecolumn = true;
        g.CancelEdit();
        e.Cancel = true;
    }

    protected void grid_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
    {
        int id = (int)e.Keys[0];
        DataRow[] tfound = tcustomer.Select("ID=" + id.ToString());// + "and Component in ('Raw Material','Ingredient')");
        foreach (DataRow dr in tfound)
        {
            tcustomer.Rows.Remove(dr);
        }
        ASPxGridView g = sender as ASPxGridView;
        UpdateData(g);
        e.Cancel = true;
    }
    private void UpdateData(ASPxGridView g)
    {
        //context.Session["CustomTable"] = dataTable;
        g.DataBind();
    }
    void testdata(DataTable table, List<string> myresult)
    {
        if (table != null)
        {
            table.Select(string.Format("[IsActive] = '{0}'", 1)).ToList<DataRow>().ForEach(r => r["IsActive"] = 0);
            var mylist = new List<dynamic>();
            List<object> fieldValues = grid.GetSelectedFieldValues(new string[] { "ID" });
            foreach (var item in fieldValues)
            {
                DataRow found = table.Rows.Find(item);
                //found["IsActive"] = string.Format("{0}", 1);
                //mylist.Add(item.ToString());
                table.Select(string.Format("[Formula] = '{0}'", found["Formula"])).ToList<DataRow>().ForEach(r => r["IsActive"] = 1);
            }
            //context.Session["Folio"] = Folio[0];
            //string datapath = "~/XlsTables/"+ Folio[0] +".json";
            //cs.WriteJason(table, Server.MapPath(datapath));
            if (table != null)
            {
                //deleteformula(myresult[0].ToString());
                foreach (DataRow dr in table.Rows)
                {
                    using (SqlConnection con = new SqlConnection(strConn))
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spInsertFormula";
                        cmd.Parameters.AddWithValue("@Id", dr["ID"].ToString());
                        cmd.Parameters.AddWithValue("@Component", dr["Component"].ToString());
                        cmd.Parameters.AddWithValue("@SubType", dr["SubType"].ToString());
                        cmd.Parameters.AddWithValue("@Description", dr["Description"].ToString());
                        cmd.Parameters.AddWithValue("@Material", string.Format("{0}", dr["SAPMaterial"].ToString()));
                        cmd.Parameters.AddWithValue("@Result", dr["GUnit"].ToString());
                        cmd.Parameters.AddWithValue("@Yield", dr["Yield"].ToString());
                        cmd.Parameters.AddWithValue("@RawMaterial", string.Format("{0}", dr["RawMaterial"].ToString()));
                        cmd.Parameters.AddWithValue("@Name", dr["Name"].ToString());
                        cmd.Parameters.AddWithValue("@PriceOfUnit", dr["PriceOfUnit"].ToString());
                        cmd.Parameters.AddWithValue("@Currency", dr["Currency"].ToString());
                        cmd.Parameters.AddWithValue("@Unit", dr["Unit"].ToString());
                        cmd.Parameters.AddWithValue("@ExchangeRate", dr["ExchangeRate"].ToString());
                        cmd.Parameters.AddWithValue("@BaseUnit", dr["BaseUnit"].ToString());
                        cmd.Parameters.AddWithValue("@PriceOfCarton", dr["PriceOfCarton"].ToString());
                        cmd.Parameters.AddWithValue("@Formula", dr["Formula"].ToString());
                        cmd.Parameters.AddWithValue("@IsActive", dr["IsActive"].ToString());
                        cmd.Parameters.AddWithValue("@RequestNo", myresult[0].ToString());
                        cmd.Parameters.AddWithValue("@user", hfuser["user_name"].ToString());
                        cmd.Parameters.AddWithValue("@Remark", dr["Remark"].ToString());
                        cmd.Parameters.AddWithValue("@LBOh", dr["LBOh"].ToString());
                        cmd.Parameters.AddWithValue("@LBRate", dr["LBRate"].ToString());
                        cmd.Parameters.AddWithValue("@Mark", dr["Mark"].ToString());
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
        }
    }
    //    protected void CmbMargin_Callback(object sender, CallbackEventArgsBase e)
    //    {
    //        ASPxComboBox comb = sender as ASPxComboBox;
    //        if (string.IsNullOrEmpty(e.Parameter)) return;
    //        string[] param = e.Parameter.Split('|');
    //        if (param[0] == "TextChanges")
    //        {
    //            //DataTable dt = cs.builditems("select * from MasPrice where ID="+param[1].ToString());
    //            //foreach(DataRow dr in dt.Rows)
    //            //{
    //            //    tbDescription.Text = dr["Name"].ToString();
    //            //}
    //        }
    //        else if(param[0]== "reload")
    //        {
    //            dsMargin.SelectParameters.Clear();
    //            dsMargin.SelectParameters.Add("CostingNo", param[2].ToString());
    //            dsMargin.SelectParameters.Add("Company", string.Format("{0}", CmbCompany.Text));
    //			dsMargin.SelectParameters.Add("NetWeight", string.Format("{0}", tbNetweight.Text));
    //            comb.DataBind();
    //        }
    //        else
    //        {
    //            dsMargin.SelectParameters.Clear();
    //            dsMargin.SelectParameters.Add("Company", param[1].ToString());
    //            dsMargin.SelectParameters.Add("CostingNo", string.Format("{0}",CmbCostingNo.Value));
    //            comb.DataBind();
    //        }
    //    }
    protected void CmbMargin_Callback(object sender, CallbackEventArgsBase e)
    {
        //        ASPxComboBox comb = sender as ASPxComboBox;
        //        if (string.IsNullOrEmpty(e.Parameter)) return;
        //        string[] param = e.Parameter.Split('|');
        //        dsMargin.SelectParameters.Clear();
        //        if (param[0] == "TextChanges")
        //        {
        //            //DataTable dt = cs.builditems("select * from MasPrice where ID="+param[1].ToString());
        //            //foreach(DataRow dr in dt.Rows)
        //            //{
        //            //    tbDescription.Text = dr["Name"].ToString();
        //            //}
        //        }
        //        if(param[0]== "reload")
        //        {
        //            dsMargin.SelectParameters.Add("CostingNo", param[2].ToString());
        //            dsMargin.SelectParameters.Add("Company", string.Format("{0}", CmbCompany.Text));
        //        }
        //        else
        //        {
        //            dsMargin.SelectParameters.Add("Company", param[1].ToString());
        //            dsMargin.SelectParameters.Add("CostingNo", string.Format("{0}",CmbCostingNo.Value));
        //        }
        //        dsMargin.SelectParameters.Add("Packaging", string.Format("{0}", CmbPackaging.Text));
        //        dsMargin.SelectParameters.Add("NetWeight", string.Format("{0}", tbNetweight.Text));
        //        comb.DataBind();
        //ASPxComboBox comb = sender as ASPxComboBox;
        //if (string.IsNullOrEmpty(e.Parameter)) return;
        //string[] param = e.Parameter.Split('|');
        //dsMargin.SelectParameters.Clear();
        //dsMargin.SelectParameters.Add("Company", string.Format("{0}", CmbCompany.Text));
        //dsMargin.SelectParameters.Add("CostingNo", string.Format("{0}", hfRequestNo["ID"]));
        //dsMargin.SelectParameters.Add("Packaging", string.Format("{0}", CmbPackaging.Text));
        //dsMargin.SelectParameters.Add("NetWeight", string.Format("{0}", tbNetweight.Text));
        //dsMargin.SelectParameters.Add("UserType", string.Format("{0}", usertp["usertype"].ToString()));
        //comb.DataBind();
    }
    //protected void CmbLaborOverhead_Callback(object sender, CallbackEventArgsBase e)
    //{
    //ASPxComboBox comb = sender as ASPxComboBox;
    //if (string.IsNullOrEmpty(e.Parameter)) return;
    //string[] param = e.Parameter.Split('|');
    //dsLabor.SelectParameters.Clear();
    //dsLabor.SelectParameters.Add("Company", param[1].ToString());
    //comb.DataBind();
    //}
    protected void CmbPackageCode_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox comb = sender as ASPxComboBox;
        if (string.IsNullOrEmpty(e.Parameter)) return;
        string[] param = e.Parameter.Split('|');
        if (param[0] == "TextChanges")
        {
            //DataTable dt = cs.builditems("select * from MasPrice where ID="+param[1].ToString());
            //foreach(DataRow dr in dt.Rows)
            //{
            //    tbDescription.Text = dr["Name"].ToString();
            //}
        }
        else
        {
            FillCityCombo("1", comb, e);
            //dsPackage.SelectParameters.Clear();
            //dsPackage.SelectParameters.Add("Company", param[1].ToString());
            //comb.DataBind();
        }
    }

    protected void FillCityCombo(string thisContractCode, ASPxComboBox comb, CallbackEventArgsBase e)
    {
        //string[] param = e.Parameter.Split('|');
        //dsPackage.SelectParameters.Clear();
        //dsPackage.SelectParameters.Add("Id", param[0].ToString());
        //dsPackage.SelectParameters.Add("Company", param[1].ToString());
        //dsPackage.SelectParameters.Add("Type", String.Format("{0}", thisContractCode));
        //dsPackage.DataBind();
        //DataTable bt = new DataTable();
        //DataView dv = (DataView)dsPackage.Select(DataSourceSelectArguments.Empty);
        //if (dv != null)
        //{
        //    bt = dv.ToTable();
        //    comb.DataBind();
        //}
    }
    protected void CmbSecPackageCode_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox comb = sender as ASPxComboBox;
        if (string.IsNullOrEmpty(e.Parameter)) return;
        //string[] param = e.Parameter.Split('|');
        FillCityCombo("0", comb, e);
    }
    //Dictionary<int, List<decimal>> dict;
    //protected void testgrid_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
    //{
    //    if (e.SummaryProcess == CustomSummaryProcess.Start)
    //        dict = new Dictionary<int, List<decimal>>();
    //    if (e.SummaryProcess == CustomSummaryProcess.Calculate)
    //    {
    //        int RowID = Convert.ToInt32(e.GetValue("ID"));
    //        List<decimal> list;
    //        if (!dict.TryGetValue(RowID, out list))
    //        {
    //            list = new List<decimal>();
    //            dict.Add(RowID, list);
    //        }
    //        string loss = e.GetValue("Loss").ToString();
    //        list.Add(Convert.ToDecimal(e.GetValue("Loss")));
    //    }
    //    if (e.SummaryProcess == CustomSummaryProcess.Finalize)
    //    {
    //        e.TotalValue = CalculateTotal();
    //        //string value = CalculateTotal(99).ToString();
    //        //e.TotalValue = String.Format("{0}", value);
    //    }
    //}
    protected void testgrid_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
    {
        ASPxSummaryItem item = (ASPxSummaryItem)e.Item;
        if (e.SummaryProcess == CustomSummaryProcess.Start)
        {
            totalCount = 0;
        }
        if (e.SummaryProcess == CustomSummaryProcess.Calculate)
        {
            totalCount += Convert.ToDecimal(e.FieldValue);
        }
        if (e.SummaryProcess == CustomSummaryProcess.Finalize)
        {
            e.TotalValue = totalCount;
        }
    }
    //private object CalculateTotal()
    //{
    //    IEnumerator en = dict.GetEnumerator();
    //    en.Reset();
    //    float result = 0;
    //    while (en.MoveNext())
    //    {
    //        KeyValuePair<int, List<float>> current = ((KeyValuePair<int, List<float>>)en.Current);
    //        //float sum = 0;
    //        for (int i = 0; i < current.Value.Count; i++)
    //            //    sum += current.Value[i];
    //            //result += sum / current.Value.Count;
    //            //if (i == c)
    //            //    result += current.Value[i];
    //            //else if (c == 99)
    //            result += current.Value[i];
    //    }
    //    return result;
    //}
    float floatValue = 0;
    protected void grid_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
    {
        //DataTable dt = (DataTable)context.Session["CustomTable"];
        //foreach (DataColumn column in dt.Columns)
        //{
        //    if (column.ColumnName.Contains("Carton"))
        //    {
        //        string col = column.ColumnName;
        //        if (((ASPxSummaryItem)e.Item).FieldName == col)
        //        {
        //            // Initialization.
        //            if (e.SummaryProcess == CustomSummaryProcess.Start)
        //                floatValue = 0;
        //            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
        //            {
        //                var name = e.GetValue(col) == DBNull.Value ? "0" : e.GetValue(col).ToString();        
        //                if (!string.IsNullOrEmpty(name))
        //                 floatValue = float.Parse(name);
        //            }
        //            // Finalize.
        //            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
        //            {
        //                if (((ASPxSummaryItem)e.Item).FieldName == col)
        //                    e.TotalValue = floatValue;
        //            }
        //        }
        //    }
        //}
        //int c = 0;
        ////DataTable dt = (DataTable)context.Session["CustomTable"];
        ////    foreach (DataColumn column in dt.Columns)
        ////    {
        ////        if (column.ColumnName.Contains("Carton"))
        ////        {
        //string ColumnName = "Std2Carton";
        //        if (e.SummaryProcess == CustomSummaryProcess.Start)
        //            dict = new Dictionary<int, List<float>>();
        //        if (e.SummaryProcess == CustomSummaryProcess.Calculate)
        //        {
        //            int RowID = Convert.ToInt32(e.GetValue("Col_0"));
        //            List<float> list;
        //            if (!dict.TryGetValue(RowID, out list))
        //            {
        //                list = new List<float>();
        //                dict.Add(RowID, list);
        //            }
        //            //list.Add(Convert.ToInt32(e.GetValue("Loss")));
        //            var name = e.GetValue(ColumnName) == DBNull.Value ? "0" : e.GetValue(ColumnName).ToString();
        //            //float floatValue = float.Parse(name);
        //            if (string.IsNullOrEmpty(name))
        //                name = "0";
        //            list.Add(float.Parse(name));
        //        }
        //        if (e.SummaryProcess == CustomSummaryProcess.Finalize)
        //        {
        //    //e.TotalValue = CalculateTotal();
        //string keyvalue = hfid["hidden_value"].ToString();
        if (e.SummaryProcess == CustomSummaryProcess.Finalize)
        {
            string keyvalue = (string.IsNullOrEmpty(hfid["hidden_value"].ToString())) ? string.Empty : hfid["hidden_value"].ToString();
            if (!string.IsNullOrEmpty(keyvalue))
            {
                //string Packaging = cs.ReadItems(@"select Packaging from TransTechnical where ID='" + keyvalue + "'").ToString();//dt.Rows[0]["FieldName"].ToString(); 
                string strloss = (string)cs.ReadItems(@"select Loss from maspfloss where PackageType='" +
                    CmbPackaging.Text + "' and SubType='Raw Material & Ingredient'");
                double loss;
                double.TryParse(strloss.ToString(), out loss);
                double value = (Convert.ToDouble(loss) / 100);
                //            //string result = value.ToString(); 
                //            //e.TotalValue = String.Format("Of raw materials + ingredients= {0}", value);
                //            //e.TotalValue = String.Format("{0}<br />", value);
                //    if (((ASPxSummaryItem)e.Item).FieldName == ColumnName)
                //                e.TotalValue = String.Format("{0:$#,##0.000;($#,##0.000);Zero}<br />", value);
                //        }
                //c++;
                //    }
                //}
                int co = 0;
                //dataTable = (DataTable)context.Session["CustomTable"];
                if (tcustomer == null) return;
                ActivePageSymbol = (ActivePageSymbol == "0" || ActivePageSymbol == "") ? string.Format("{0}", 1) : ActivePageSymbol.ToString();
                //DataTable dt = (DataTable)context.Session["CustomTable"];
                foreach (string c in myCollection)
                {
                    co++;
                    var name = string.Format("PriceOfCarton", co);
                    if (((ASPxSummaryItem)e.Item).FieldName == name)
                    {
                        decimal d = 0;
                        decimal total = tcustomer.Select("Formula = '" + ActivePageSymbol.ToString() + "'").AsEnumerable()
                         .Where(r => decimal.TryParse(r.Field<string>(name), out d))
                         //.Where(y => y.Field<string>("Formula") == ActivePageSymbol)
                         .Sum(r => d);
                        total = total * Convert.ToDecimal(value);
                        e.TotalValue = String.Format("%Loss = {0}<br />", total.ToString("F4"));
                    }
                    if (((ASPxSummaryItem)e.Item).FieldName == "LBRate")
                    {
                        decimal xx = 0;

                        //var a = e.GetValue("GUnit");
                        //var b = e.GetValue("LBRate");
                        string i = ActivePageSymbol.ToString();
                        DataRow[] foundRows = tcustomer.Select("LBRate<>'' and formula = '" + i.ToString() + "'");
                        foreach (DataRow row in foundRows)
                        {
                            if (!string.IsNullOrEmpty(row["GUnit"].ToString()) && !string.IsNullOrEmpty(row["LBRate"].ToString()))
                                xx += Convert.ToDecimal(row["GUnit"]) * Convert.ToDecimal(row["LBRate"]);
                        }
                        decimal xsum = 0;
                        string textsum = tcustomer.Compute("Sum(GUnit)", "LBRate<>'' and Formula in('" + ActivePageSymbol.ToString() + "')").ToString();
                        decimal.TryParse(textsum, out xsum);
                        if (xsum != 0)
                            e.TotalValue = String.Format("LBOh = {0}<br />", (xx / xsum).ToString("F4"));
                    }
                }
            }
        }
    }
    //protected void grid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
    //{
    //    ASPxGridView grid = (ASPxGridView)sender;
    //    if (e.Column.FieldName == "SAPMaterial")
    //    {
    //        ASPxComboBox cmb = (ASPxComboBox)e.Editor;
    //        var Id = grid.GetRowValues(e.VisibleIndex, "RowID");
    //        cmb.Columns.Add(new ListBoxColumn("RawMaterial"));
    //        cmb.Columns.Add(new ListBoxColumn("Description"));
    //        cmb.Columns.Add(new ListBoxColumn("Yield"));
    //        cmb.Columns.Add(new ListBoxColumn("Material"));
    //        cmb.Columns.Add(new ListBoxColumn("Name"));
    //        cmb.EnableCallbackMode = true;
    //        cmb.CallbackPageSize = 10;
    //        cmb.DataSource = dsYield;
    //        cmb.DataBindItems();
    //    }
    //    if (e.Column.FieldName.Contains("Component"))
    //    {
    //        if(dataTable != null){
    //            var myResult = dataTable.AsEnumerable()
    //            .Select(s => new {
    //                id = s.Field<string>("Component"),
    //            })
    //            .Distinct().ToList();
    //            ASPxComboBox CmbComponent = (ASPxComboBox)e.Editor;
    //            CmbComponent.DataSource = myResult;
    //            CmbComponent.DataBindItems(); }
    //    }
    //    if (e.Column.FieldName.Contains("Currency"))
    //    {
    //        ASPxComboBox CmbCurrency = (ASPxComboBox)e.Editor;
    //        CmbCurrency.Columns.Add(new ListBoxColumn("value"));
    //        CmbCurrency.ValueField = "value";
    //        CmbCurrency.TextFormatString = "{0}";
    //        CmbCurrency.DataSource = cs.builditems("select value from dbo.FNC_SPLIT('THB;USD',';')");
    //        CmbCurrency.DataBindItems();
    //    }
    //}

    protected void grid_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //for (int i = 0; i < Math.Min(g.VisibleRowCount, g.Columns.Count); i++)
        //{
        //    var  id = g.GetRowValues(i,g.KeyFieldName).ToString();
        //    double OutVal; double BaseUnit; double Yield; double Packsize;
        //    if (double.TryParse(g.GetRowValues(i, "GUnit").ToString(), out OutVal))
        //    {
        //        string te = g.GetRowValues(i, "Description").ToString();
        //    }     
        //}
        //DataTable dtLBOh = ((DataView)dsLabor.Select(DataSourceSelectArguments.Empty)).Table;
        //---
        //DataTable dtLBOh = new DataTable();
        //DataView dv = (DataView)dsLabor.Select(DataSourceSelectArguments.Empty);
        //if (dv != null)
        //dtLBOh = dv.ToTable();
        int rowCnt = g.VisibleRowCount;
        int pageSize = g.SettingsPager.PageSize; //string[] array;
        if (rowCnt != 0 && rowCnt != pageSize)
        {
            DataTable table = (DataTable)g.DataSource;
            if (table != null)
            {
                //g.Columns[0].Visible = (string.Format("{0}", editor["Name"]) == "0");
                if (g.VisibleRowCount > 30)
                    g.Settings.VerticalScrollableHeight = 550;
                else
                    g.Settings.VerticalScrollableHeight = g.VisibleRowCount * 33;
                //        //for (int i = 0; i < Math.Min(g.VisibleRowCount, g.Columns.Count); i++)
                for (int i = 0; i < g.VisibleRowCount; i++)
                {
                    var id = g.GetRowValues(i, g.KeyFieldName).ToString();
                    //            //DataRow found = table.Rows.Find(id);
                    //            //g.Columns[0].HeaderStyle.BackColor = Color.Coral;
                    //DataRow found = table.AsEnumerable()
                    //                .SingleOrDefault(r => r.Field<int>("RowID") == Convert.ToInt32(id));
                    DataRow found = table.Select("RowID='" + id + "'").FirstOrDefault();

                    //            float sumOf = 0;// Convert.ToDouble(table.Compute("SUM(GUnit)", "Formula = " + found["Formula"]+ "And Component='Raw Material'"));
                    //            object calc = table.Compute("SUM(GUnit)", "Formula = " + found["Formula"]);// "And Component='Raw Material'");
                    //            if(!float.TryParse(calc.ToString(), out sumOf))
                    //                return;
                    //            // var sumOf = table.AsEnumerable()
                    //            //.Where(dr => dr.Field<string>("Formula").Equals(found["Formula"]) && dr.Field<string>("Component").Equals("Raw Material"))
                    //            //.Sum(dr => dr.Field<double>("GUnit"));
                    //            if (!string.IsNullOrEmpty(found["LBOh"].ToString()) && dtLBOh!=null)
                    //            {
                    //                DataRow drLBOh = dtLBOh.AsEnumerable()
                    //                            .SingleOrDefault(r => r.Field<string>("LBCode") == found["LBOh"].ToString());
                    //                if (drLBOh != null)
                    //                {
                    //                    var LBRate = (Convert.ToDouble(found["GUnit"]) / sumOf) * ((Convert.ToDouble(drLBOh["LBRate"]) / Convert.ToDouble(drLBOh["PackSize"])) * 
                    //                        Convert.ToDouble(CmbPackSize.Text));
                    //                    found["LBRate"] = LBRate.ToString("F4");
                    //                }
                    //            }
                    if (found["IsActive"].ToString() == "1")
                        g.Selection.SelectRow(i);
                    //            //for (int i = 0; i < rowCnt; i++)
                    //            //{
                    //            string value = "";
                    //            var result = (found["SAPMaterial"] == null) ? string.Empty : found["SAPMaterial"].ToString();
                    //            string comp = (CmbCompany.Value == null) ? string.Empty : CmbCompany.Value.ToString();
                    //            if (!string.IsNullOrEmpty(result))
                    //            {
                    //                //dsMaterial.SelectParameters.Clear();
                    //                //dsMaterial.SelectParameters.Add("Company", comp.ToString());
                    //                //dsMaterial.SelectParameters.Add("RawMaterial", result.ToString());
                    //                //dsMaterial.DataBind();
                    //                //DataView dvsqllist = (DataView)dsMaterial.Select(DataSourceSelectArguments.Empty);
                    //                //if (dvsqllist != null)
                    //                //{
                    //                //    DataTable dt = dvsqllist.Table;
                    //                //    DataRow[] r = dt.Select("Company = '" + comp.ToString() + "' and RawMaterial = '" + (string)result + "'");
                    //                //    foreach (DataRow dr in r)
                    //                //    {
                    //                //        array = Regex.Split("Name;Yield", ";");
                    //                //        foreach (string arr in array)
                    //                //        {
                    //                //            found[arr] = dr[arr];
                    //                //        }
                    //                //        found["RawMaterial"] = dr["Material"];
                    //                //    }
                    //                //}
                    //                //if (found["RawMaterial"].ToString() == "4130241")
                    //                //    System.Diagnostics.Debug.WriteLine(found["RawMaterial"].ToString());
                    //                //if (found["SAPMaterial"].ToString() == "4100333")
                    //                //{
                    //                //var test = "";
                    //                //}

                    //                value = string.Format("{0}", comp.ToString() + "|" + found["SAPMaterial"] + "|" + CmbCostingNo.Value);
                    //                DataTable data = GetrequestRate(value);
                    //                foreach (DataRow dr in data.Rows)
                    //                {
                    //                    string[] rowValueItems = Regex.Split("PriceOfUnit;Currency;Unit;ExchangeRate;BaseUnit", ";");
                    //                    foreach (string row in rowValueItems)
                    //                    {
                    //                        found[row] = dr[row];
                    //                    }
                    //                }
                    //            }
                    //            string strBaseUnit = found["BaseUnit"].ToString();
                    //            //if (string.IsNullOrEmpty(strBaseUnit) && !string.IsNullOrEmpty(found["Currency"].ToString()))
                    //            if (!string.IsNullOrEmpty(found["Currency"].ToString()))
                    //            {
                    //                var Currency = ExchangeRate(found["Currency"].ToString());
                    //                if (!string.IsNullOrEmpty(Currency))
                    //                {
                    //                    found["ExchangeRate"] = Currency.ToString();
                    //                    double da = double.Parse(found["ExchangeRate"].ToString(), CultureInfo.InvariantCulture) * double.Parse(found["PriceOfUnit"].ToString());
                    //                    if (!string.IsNullOrEmpty(found["Unit"].ToString()))
                    //                        switch (found["Unit"].ToString())
                    //                            {
                    //                            case "Ton":case "MT":
                    //                                da=da/1000;
                    //                                break;
                    //                        }
                    //                        found["BaseUnit"] = da.ToString("F4");
                    //                }
                    //            }
                    //            array = Regex.Split("PriceOfCarton", ";");
                    //            foreach (string arr in array)
                    //            {
                    //                int co = 0;
                    //                foreach (string c in myCollection)
                    //                {
                    //                    co++;
                    //                    var name = string.Format(arr, co);
                    //                    double OutVal; double BaseUnit; double Yield; double Packsize;
                    //                    if (double.TryParse(found[c].ToString(), out OutVal))
                    //                    {
                    //                        double total = OutVal / 1000;
                    //                        //table.Rows[i][name] = table.Rows[i][c].ToString();
                    //                        //var test = table.Rows[i][c].ToString();
                    //                        double.TryParse(CmbPackSize.Text.ToString(), out Packsize);
                    //                        double.TryParse(found["BaseUnit"].ToString(), out BaseUnit);
                    //                        double.TryParse(found["Yield"].ToString(), out Yield);
                    //                        double r = (total * BaseUnit * Packsize / (Yield / 100));
                    //                        if (double.IsNaN(r) || double.IsInfinity(r))
                    //                            found[name] = "0";
                    //                        else
                    //                            found[name] = r.ToString("F4");
                    //                    }
                    //                }
                    //            }
                }
                //        //for (int i = 0; i < pageSize - rowCnt; i++)
                //        //{
                //        //    int rowIndex = rowCnt + i + 1;
                //        //    DataRow row = table.NewRow();
                //        //    for (int j = 0; j < g.Columns.Count; j++)
                //        //    {
                //        //        row[j] = "";
                //        //    }
                //        //    table.Rows.Add(row);
                //        //}
                //        dataTable = table;
                //        //context.Session["CustomTable"] = table;
            }
        }
        //   //---
        g.FilterExpression = "[Formula] = '" + ActivePageSymbol + "'";
    }
    string ExchangeRate(string Currency)
    {
        string cr = string.Format("{0}", 1);
        if (Currency == "THB") return cr;
        cr = seExchangeRate.Text;
        //using (SqlConnection con = new SqlConnection(strConn))
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.CommandText = "spExchangeRat";
        //    cmd.Parameters.AddWithValue("@KeyDate", DateTime.Now.ToString("yyyy-MM-dd"));
        //    cmd.Parameters.AddWithValue("@Company", CmbCompany.Text);
        //    cmd.Connection = con;
        //    con.Open();
        //    var getValue = cmd.ExecuteScalar();
        //    con.Close();
        //    cr = (string)getValue;
        //}
        return cr;
    }
    //DataTable LaborOverhead()
    //{
    //    DataTable dt = new DataTable();
    //    dsLaborOverhead.SelectParameters.Clear();
    //    dsLaborOverhead.SelectParameters.Add("Company", CmbCompany.Text);
    //    dsLaborOverhead.SelectParameters.Add("Packaging", string.Format("{0}", CmbPackaging.Text));
    //    dsLaborOverhead.SelectParameters.Add("NetWeight", string.Format("{0}", tbNetweight.Text));
    //    dsLaborOverhead.SelectParameters.Add("PackSize", string.Format("{0}", tbPackSize.Text));
    //    dsLaborOverhead.DataBind();
    //    DataView dvsqllist = (DataView)dsLaborOverhead.Select(DataSourceSelectArguments.Empty);
    //    if (dvsqllist != null){
    //        int c = dvsqllist.Table.Rows.Count;
    //        dt = dvsqllist.Table;
    //    }
    //    return dt;
    //    }
    protected void grid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (e.Column.FieldName == "SAPMaterial")
        {
            //e.Editor.ClientInstanceName = "SAPMaterial";
            //ASPxComboBox cmb = (ASPxComboBox)e.Editor;
            //dsRawMaterial.SelectParameters.Clear();
            //dsRawMaterial.SelectParameters.Add("Company", CmbCompany.Text);
            //dsRawMaterial.DataBind();

        }
        if (e.Column.FieldName == "LBOh")
        {
            //e.Editor.ClientInstanceName = "LBOh";
            //ASPxComboBox comboBox = (ASPxComboBox)e.Editor;
            //LaborOverhead();
            //comboBox.DataBind();
            //comboBox.Items.Insert(0, new ListEditItem(string.Empty, null));
        }
        if (e.Column.FieldName == "Component" || e.Column.FieldName == "SubType")
        {
            if (tcustomer != null)
            {
                // var myResult = dataTable.AsEnumerable()
                //.Select(s => new
                //{
                //    id = s.Field<string>(e.Column.FieldName),
                //})
                //.Distinct().ToList();
                ASPxComboBox CmbComponent = (ASPxComboBox)e.Editor;
                var ValuetoReturn = (from Rows in tcustomer.AsEnumerable()
                                     select Rows[e.Column.FieldName]).Distinct().ToList();
                ValuetoReturn.Add(string.Empty);
                ValuetoReturn.Add("Raw Material");
                CmbComponent.ValueType = typeof(System.String);
                var sortedTable = ValuetoReturn.Distinct().OrderBy(q => q.ToString());
                CmbComponent.DataSource = sortedTable;// ValuetoReturn.Distinct();
                CmbComponent.DataBindItems();
            }
        }
        if (e.Column.FieldName == "RawMaterial")
        {
            //var comb = (ASPxComboBox)e.Editor;
            //comb.ClientInstanceName = "RawMaterial";
            //comb.Callback += Combo_OnCallback;
            //var SAPMaterial = g.GetRowValues(e.VisibleIndex, "SAPMaterial");
            //if (e.KeyValue != DBNull.Value && e.KeyValue != null && SAPMaterial != null && SAPMaterial != DBNull.Value)
            //{
            //    FillMaterialCombo(comb, SAPMaterial.ToString());
            //}
            //else
            //{
            //    comb.DataSourceID = null;
            //    comb.Items.Clear();
            //}
            //if (g.IsNewRowEditing) return;
            //if (editor["Name"].ToString() != "0")
            //    e.Editor.ReadOnly = true;
        }
    }
    protected void OnItemRequestedByValue(object source, ListEditItemRequestedByValueEventArgs e)
    {
        if (e.Value == null)
            //    return;
            //ASPxComboBox editor = source as ASPxComboBox;
            //dsMaterial.SelectParameters.Clear();
            //dsMaterial.SelectParameters.Add("Company", CmbCompany.Text);
            //dsMaterial.SelectParameters.Add("RawMaterial", e.Value.ToString());
            //dsMaterial.DataBind();
            //editor.DataSource = dsMaterial;
            //editor.DataBind();
            return;
    }
    protected void SQL_ItemsRequestedByFilterCondition(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
    {
        ASPxComboBox editor = source as ASPxComboBox;
        //IQueryable<City> query;
        //var take = e.EndIndex - e.BeginIndex + 1;
        //var skip = e.BeginIndex;
        //object countryValue = GetCurrentCountry();
        //DataView dvsqllist = (DataView)dsLaborOverhead.Select(DataSourceSelectArguments.Empty);
        //if (dvsqllist != null)
        //{
        //    int c = dvsqllist.Table.Rows.Count;
        //    DataTable dt = dvsqllist.Table;
        //    editor.DataSource = dt;
        //    editor.DataBind();
        //}
    }
    void Combo_OnCallback(object source, CallbackEventArgsBase e)
    {
        FillMaterialCombo(source as ASPxComboBox, e.Parameter);
    }
    protected void FillMaterialCombo(ASPxComboBox cmb, string rawValue)
    {
        //if (string.IsNullOrEmpty(rawValue)) return;
        //dsMaterial.SelectParameters.Clear();
        //dsMaterial.SelectParameters.Add("Company", CmbCompany.Text);
        //dsMaterial.SelectParameters.Add("RawMaterial", rawValue.ToString());
        //dsMaterial.DataBind();
        //cmb.DataSourceID = null;
        //DataView dvsqllist = (DataView)dsMaterial.Select(DataSourceSelectArguments.Empty);
        //if (dvsqllist != null)
        //{
        //    int c = dvsqllist.Table.Rows.Count;
        //    DataTable dt = dvsqllist.Table;
        //    cmb.DataSource = dt;
        //    cmb.DataBindItems();
        //}
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
            DataTable dt = dvsqllist.Table;
            editor.DataSource = dt;
            editor.DataBind();
        }
        //editor.DataSource = dsMaterial;
        //editor.DataBind();
    }
    private string GetCurrentCountry()
    {
        object id = null;
        if (hf.TryGet("CurrentCountry", out id))
            return Convert.ToString(id);
        return Convert.ToString(id);
    }

    protected void gridData_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {
        if (e.ButtonID != "Clone") return;
        ASPxGridView g = (ASPxGridView)sender;
        object keyValue = g.GetRowValues(e.VisibleIndex, "ID");
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spcopyCosting";
            cmd.Parameters.AddWithValue("@Id", keyValue);
            cmd.Parameters.AddWithValue("@Requester", hfuser["user_name"].ToString());
            cmd.Connection = con;
            con.Open();
            var getValue = cmd.ExecuteScalar();
            con.Close();
            g.JSProperties["cpKeyValue"] = (getValue == null) ? string.Empty : getValue.ToString();
        }
    }
    protected void CmbDisponsition_Callback(object sender, CallbackEventArgsBase e)
    {
        if (string.IsNullOrEmpty(e.Parameter)) return;
        SqlParameter[] param = {new SqlParameter("@Id",(string)e.Parameter),
                        new SqlParameter("@table","TransCostingHeader"),
                        new SqlParameter("@username",hfuser["user_name"].ToString())};
        CmbDisponsition.DataSource = GetRelatedResources("spGetStatusApproval", param);
        CmbDisponsition.DataBind();
    }
    //string IsValidate = "";
    protected void grid_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
    {
        //var values = new[] { "GUnit", "BaseUnit", "PriceOfCarton" };
        var values = new[] { "BaseUnit", "PriceOfCarton" };
        if (editor["Name"].ToString() == "0")
        {
            if (values.Any(e.DataColumn.FieldName.Contains))
                e.Cell.Attributes.Add("onclick", "event.cancelBubble = true");
        }
        else
            e.Cell.Attributes.Add("onclick", "event.cancelBubble = true");
        values = new[] { "Component", "SubType", "Description", "SAPMaterial" };
        if (values.Any(e.DataColumn.FieldName.Contains))
            e.Cell.ForeColor = Color.Black;
        //if (e.DataColumn.FieldName == "aValidate")
        //    IsValidate= string.Format("{0}",e.CellValue);
        if (e.DataColumn.FieldName == "Description")
        {
            var gv = sender as ASPxGridView;
            DataRow row = gv.GetDataRow(e.VisibleIndex);
            int index = e.VisibleIndex;
            //e.Cell.ForeColor = Color.Black;
            if (gv.VisibleRowCount != 0 && row != null && tbMarketingNo.Text == "")
                if (gv.GetRowValues(e.VisibleIndex, "aValidate").ToString() == "X")
                    e.Cell.BackColor = Color.Coral;
                else
                    e.Cell.BackColor = Color.LightGreen;
        }
    }
    DataTable GetElement(string Element)
    { //spGetElement
        var Results = new DataTable();
        SqlParameter[] param = { new SqlParameter("@tablename", (string)Element) };
        Results = GetRelatedResources("spGetElement", param);
        return Results;
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

    protected void grid_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //g.KeyFieldName = "ID";
        //g.DataSource = GetTable;//_dataTable;
        //g.ForceDataRowType(typeof(DataRow));

        g.DataSource = GetTable;
        AddColumns();
    }
    //protected void cbCheck_Load(object sender, EventArgs e)
    //{
    //    ASPxCheckBox cb = sender as ASPxCheckBox;

    //    GridViewDataItemTemplateContainer container = cb.NamingContainer as GridViewDataItemTemplateContainer;
    //    cb.ClientInstanceName = String.Format("cbCheck{0}", container.VisibleIndex);
    //    if (dataTable != null)
    //    {
    //        DataRow[] rows = dataTable.Select("formula='" + ActivePageSymbol + "'");
    //        for (int i = 0; i < rows.Length; i++)
    //        {
    //           cb.Checked = rows[i]["IsActive"].ToString() == "1" ? true : false ;
    //        }
    //    }
    //    //cb.Checked = Grid.Selection.IsRowSelected(container.VisibleIndex);

    //    cb.ClientSideEvents.CheckedChanged = String.Format("function (s, e) {{ Grid.SelectRowOnPage ({0}, s.GetChecked()); }}", container.VisibleIndex);
    //}

    protected void PreviewPanel_Callback(object sender, CallbackEventArgsBase e)
    {
        //int id;
        var text = string.Format("<div align='center'><h1>Access denied</h1><br/>You are not authorized to access this page.</div>", e.Parameter);
        //if (int.TryParse(e.Parameter, out id))
        //{
        //    var message = DemoModel.DataProvider.Messages.FirstOrDefault(m => m.ID == id);
        //    if (message != null)
        //    {
        //        DemoModel.DataProvider.MarkMessagesAs(true, new int[] { id });
        //        var subject = message.Subject;
        //        if (message.IsReply)
        //            subject = "Re: " + subject;
        //        text = string.Format(PreviewMessageFormat, subject, message.From, message.To, message.Date, message.Text);
        //    }
        //}
        PreviewPanel.Controls.Add(new LiteralControl(text));
    }
    private DataTable dataTable;
    private DataTable CustomDataSourse
    {
        get
        {
            if (dataTable != null)
                return dataTable;

            dataTable = ViewState["CustomTable"] as DataTable;
            if (dataTable != null)
                return dataTable;


            dataTable = new DataTable("CustomDTable");
            dataTable.Columns.Add("ID", typeof(Int32));
            dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns[0] };
            dataTable.Columns.Add("Data", typeof(string));

            dataTable.Rows.Add(0, "Data1");
            dataTable.Rows.Add(1, "Data2");
            dataTable.Rows.Add(2, "Data3");
            dataTable.Rows.Add(3, "Data4");
            dataTable.Rows.Add(4, "Data5");
            ViewState["CustomTable"] = dataTable;

            return dataTable;
        }
    }
    private void CreateGrid()
    {

        grid.KeyFieldName = "ID";
        grid.DataBind();
        if (!this.IsPostBack)
        {
            GridViewCommandColumn c = new GridViewCommandColumn();
            grid.Columns.Add(c);
            c.ShowEditButton = true;
            c.ShowUpdateButton = true;
            c.ShowNewButtonInHeader = true;
        }

    }
    //protected void GridView1_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    //{
    //    ASPxGridView grid = sender as ASPxGridView;
    //    string[] param = e.Parameters.Split('|'); //bool selected = true;
    //    DataTable dt = new DataTable();
    //    if (param[0] == "symbol")
    //    {
    //        ActivePageSymbol = param[1].ToString();
    //        dt = (DataTable)grid.DataSource;
    //    }
    //    if (param[0] == "reload")
    //    {
    //        Session.Remove(sessionKey);
    //        string Folio = param[1];
    //        using (SqlConnection con = new SqlConnection(strConn))
    //        {
    //            SqlCommand cmd = new SqlCommand();
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.CommandText = "spselectformuladisplay";
    //            cmd.Parameters.AddWithValue("@Id", Folio);
    //            cmd.Connection = con;
    //            con.Open();
    //            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
    //            oAdapter.Fill(dt);
    //            con.Close();
    //            con.Dispose();
    //        }
    //    }
    //    if (dt != null)
    //    {
    //        //DataRow[] result = dataTable.Select();
    //        //for (int i = 0; i < result.Length; i++)
    //        //    if (result[i]["IsActive"].ToString() == "1")
    //        //        //grid.Selection.SetSelection(i, true);
    //        //grid.Selection.SelectRow(i);
    //        int max = Convert.ToInt32(dt.AsEnumerable()
    //        .Max(row => row["Formula"]));
    //        grid.AutoFilterByColumn(grid.Columns["Formula"], ActivePageSymbol);
    //        grid.Templates.PagerBar = new MyPagerBarTemplate(ActivePageSymbol, max);
    //    }
    //    context.Session["table"] = dt;
    //    grid.DataBind();
    //}
    //protected void GridView1_DataBound(object sender, EventArgs e)
    //{
    //    //ASPxGridView g = sender as ASPxGridView;
    //    //int rowCnt = g.VisibleRowCount;
    //    //int pageSize = g.SettingsPager.PageSize; string[] array;
    //    //if (rowCnt != 0 && rowCnt != pageSize)
    //    //{
    //    //    DataTable dt = (DataTable)g.DataSource;
    //    //    if (dt != null)
    //    //    {
    //    //        for (int i = 0; i < Math.Min(g.VisibleRowCount, g.Columns.Count); i++)
    //    //        {

    //    //            var id = g.GetRowValues(i, g.KeyFieldName).ToString();
    //    //            //DataRow found = table.Rows.Find(id);
    //    //            DataRow found = dt.AsEnumerable()
    //    //                            .SingleOrDefault(r => r.Field<string>("Id") == id);
    //    //            if (found["IsActive"].ToString() == "1")
    //    //                g.Selection.SelectRow(i);
    //    //        }
    //    //    }
    //    //}
    // }
    decimal totalCount;
    //List<string> OrderNo; 
    //protected void GridView1_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
    //{
    //    ASPxSummaryItem item = (ASPxSummaryItem)e.Item;
    //    if (e.SummaryProcess == CustomSummaryProcess.Start)
    //    {
    //        totalCount = 0;
    //    }
    //    if (e.SummaryProcess == CustomSummaryProcess.Calculate)
    //    {
    //            totalCount+=Convert.ToDecimal(e.FieldValue); 
    //    }
    //    if (e.SummaryProcess == CustomSummaryProcess.Finalize)
    //    {
    //        e.TotalValue = totalCount;
    //    }
    //}

    //protected void GridView1_DataBinding(object sender, EventArgs e)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
    //    g.KeyFieldName = "Id";
    //    g.DataSource = display;
    //    g.ForceDataRowType(typeof(DataRow));
    //}
    //private DataTable display
    //{
    //    get
    //    {
    //        DataTable dt = (DataTable)context.Session["table"];
    //        return dt;
    //    }

    //}

    protected void testgrid_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        if (e.ButtonID != "remove") return;
        object keyValue = g.GetRowValues(e.VisibleIndex, g.KeyFieldName);
        DataRow found = _dataTable.Rows.Find(keyValue);
        //t.Rows.Remove(found);
        tcustomer.Rows.Remove(found);
        deleteCosting(keyValue);
        //found["Mark"] = found["Mark"].ToString() == "D" ? "" : "D";
        //_dataTable.AcceptChanges();
        g.DataBind();
    }

    protected void testgrid_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
    {
        var values = new[] { "Component", "Code", "Name" };
        //if (values.Any(e.DataColumn.FieldName.Contains))
        if (e.DataColumn.FieldName.Contains("Per") == false)
            e.Cell.Attributes.Add("onclick", "event.cancelBubble = true");
    }
    protected void testgrid_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string editor = cs.IsMemberOfRole(string.Format("{0}", 2));
        if (string.Format("{0}", editor) == "0")
        {
            if (g.Columns.IndexOf(g.Columns["Command"]) != -1)
                return;
            GridViewCommandColumn colsel = new GridViewCommandColumn();
            colsel.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            colsel.Width = Unit.Pixel(45);
            colsel.Name = "Command";
            colsel.ShowSelectCheckbox = true;
            colsel.HeaderCaptionTemplate = check;
            colsel.ShowNewButtonInHeader = true;
            colsel.FixedStyle = GridViewColumnFixedStyle.Left;
            g.Columns.Add(colsel);
            g.Columns["Command"].VisibleIndex = 0;
        }
        int rowCnt = g.VisibleRowCount;
        int pageSize = g.SettingsPager.PageSize;
        if (rowCnt != 0 && rowCnt != pageSize)
        {
            DataTable table = (DataTable)g.DataSource;
            if (table != null)
            {
                if (g.VisibleRowCount > 5)
                    g.Settings.VerticalScrollableHeight = 320;
                else
                    g.Settings.VerticalScrollableHeight = g.VisibleRowCount * 33;
                //string keyvalue = hfid["hidden_value"].ToString();
                //string PackType = cs.ReadItems(@"select Packaging from TransTechnical where ID='" + keyvalue + "'").ToString();
            }
        }
        g.FilterExpression = "[Formula] = '" + ActivePageSymbol + "'";
    }
    protected void testgrid_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
    {
        //DataTable table = GetTable;
        DataRow found = _dataTable.Rows.Find(e.Keys[0]);
        foreach (DataColumn column in _dataTable.Columns)
        {
            if (!column.ColumnName.Contains("ID"))
            {
                found[column.ColumnName] = e.NewValues[column.ColumnName];
            }
        }
        ASPxGridView g = sender as ASPxGridView;
        UpdateData(g);
        //test(table);
        //calculatecolumn = true;
        g.CancelEdit();
        e.Cancel = true;
    }

    protected void CmbCompany_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox comb = sender as ASPxComboBox;
        if (string.IsNullOrEmpty(e.Parameter)) return;
        string[] param = e.Parameter.Split('|');
        dsCompany.SelectParameters.Clear();
        dsCompany.SelectParameters.Add("ID", param[1].ToString());
        dsCompany.SelectParameters.Add("BU", Session["BU"].ToString());
        comb.DataBind();
        if (param[0] == "#TextChanged#")
            comb.Text = param[1].ToString();
    }

    //protected void popup_WindowCallback(object source, PopupWindowCallbackArgs e)
    //{
    //    var KeyValue = e.Parameter.ToString();
    //hflit["hflit"] = KeyValue.ToString();
    //DataBind(KeyValue);
    //litText.Text = e.Parameter.ToString();
    //using (SqlConnection con = new SqlConnection(strConn))
    //{
    //    SqlCommand cmd = new SqlCommand();
    //    cmd.CommandType = CommandType.StoredProcedure;
    //    cmd.CommandText = "spGetCostingSheet";
    //    cmd.Parameters.AddWithValue("@Id", e.Parameter.ToString());
    //    cmd.Connection = con;
    //    con.Open();
    //    DataTable dt = new DataTable();
    //    SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
    //    oAdapter.Fill(dt);
    //    con.Close();
    //    con.Dispose();
    //}
    //}

    //protected void TreeList_CustomJSProperties(object sender,TreeListCustomJSPropertiesEventArgs e)
    //{
    //    ASPxTreeList treeList = sender as ASPxTreeList;
    //    Hashtable nameTable = new Hashtable();
    //    foreach (TreeListNode node in treeList.GetVisibleNodes())
    //        nameTable.Add(node.Key, string.Format("{0} {1}", node["MarketingNumber"], node["Material"]));
    //    e.Properties["cpEmployeeNames"] = nameTable;
    //}
    //protected void TreeList_CustomDataCallback(object sender, TreeListCustomDataCallbackEventArgs e)
    //{
    //    string key = e.Argument.ToString();
    //}

    //protected void TreeList_DataBinding(object sender, EventArgs e)
    //{
    //    ASPxTreeList treeList = sender as ASPxTreeList;
    //    if (string.IsNullOrEmpty(hflit["hflit"].ToString())) return;
    //    string test = hflit["hflit"].ToString();
    //    ods.SelectParameters.Clear();
    //    ods.SelectParameters.Add("Id", hflit["hflit"].ToString());
    //    ods.DataBind();
    //    DataView dvsqllist = (DataView)ods.Select(DataSourceSelectArguments.Empty);
    //    if (dvsqllist != null)
    //    {
    //        DataTable dt = dvsqllist.Table;
    //        treeList.DataSource = dt;
    //    }
    //}

    //protected void TreeList_CustomCallback(object sender, TreeListCustomCallbackEventArgs e)
    //{
    //    string[] parameter = e.Argument.Split('|');
    //}

    protected void gridData_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        int i = Convert.ToInt32(editor["Name"].ToString() == "0" ? 0 : 1);
        g.Columns[i].Visible = true;
        //g.Columns[0].Visible = (string.Format("{0}", editor["Name"]) == "0");
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
        int itemCount = 0;
        //try
        //{
        //    itemCount = (int)gridData.GetTotalSummaryValue(gridData.TotalSummary["RequestNo"]);
        //}
        //catch(Exception e)
        //{
        //    Console.WriteLine(e.Message);
        //}
        gridData.SettingsPager.Summary.Text = "Page {0} of {1} (" + itemCount.ToString() + " items)";
    }
    object GetDataSource(int count)
    {
        List<object> result = new List<object>();
        for (int i = 0; i < count; i++)
            result.Add(new { ID = i, City = "City_" + i });
        return result;
    }
    //protected void CmbPackaging_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    if (string.IsNullOrEmpty(e.Parameter)) return;
    //    string[] param = e.Parameter.Split('|'); //bool selected = true;
    //    string strtext = cs.ReadItems(@"select isnull(Packaging," +
    //    "(select top 1 Packaging from TransCostingHeader where RequestNo=a.ID)) from TransTechnical a where a.ID='" + param[1] + "'");
    //    string strSQL = @"select LTRIM(RTRIM(value))'value' from dbo.FNC_SPLIT('" + strtext + "',';')";
    //    DataTable dt = cs.builditems(strSQL);
    //    CmbPackaging.DataSource = dt;
    //    CmbPackaging.DataBind();
    //    if (dt.Rows.Count == 1)
    //        CmbPackaging.SelectedIndex = 0;
    //}

    protected void gridData_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            if (string.Format("{0}", cs.IsMemberOfRole(string.Format("{0}", 2))) != "0")
                return;
            var Copied = e.CreateItem("Copied", "Copied");
            Copied.Image.Url = @"~/Content/Images/Copy.png";
            e.Items.Add(Copied);
            var item = e.CreateItem("Revised", "Revised");
            item.Image.Url = @"~/Content/Images/Edit.gif";
            e.Items.Add(item);

            var itemAttach = e.CreateItem("Attach", "Attach");
            itemAttach.Image.Url = @"~/Content/Images/icons8-attach-16.png";
            e.Items.Add(itemAttach);
            itemAttach.BeginGroup = true;

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

    protected void grid_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
    {
        // hide the Edit button
        if (e.ButtonType == ColumnCommandButtonType.SelectCheckbox)
        {
            ASPxGridView gv = sender as ASPxGridView;
            int ID = Convert.ToInt32(gv.GetDataRow(e.VisibleIndex)["Id"]);
            //if (PreviouslySelectedListContains(ID))
            //{
            gv.Selection.SetSelection(e.VisibleIndex, true);
            //}
        }
        if (e.ButtonType == ColumnCommandButtonType.Edit || e.ButtonType == ColumnCommandButtonType.Cancel || e.ButtonType == ColumnCommandButtonType.Update)
            e.Visible = false;
        var edit = cs.IsMemberOfRole(string.Format("{0}", 0)); //approv["Name"].ToString();
        // disable the selction checkbox
        if (edit != "0")
        {
            if (e.ButtonType == ColumnCommandButtonType.SelectCheckbox)
                e.Enabled = false;
            GridViewCommandColumn SelectColumn = e.Column as GridViewCommandColumn;
            SelectColumn.SelectAllCheckboxMode = GridViewSelectAllCheckBoxMode.None;
        }
    }

    protected void grid_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var result = new Dictionary<string, string>();
        var args = e.Parameters.Split('|');
        if (args[0] == "PackSize")
        {
            result["PackSize"] = "";
            var rw = _dt.Select("Formula='" + ActivePageSymbol + "'").FirstOrDefault();
            if (rw != null)
                result["PackSize"] = rw["Packaging"].ToString();
            e.Result = result;
        }
        else if (args[0] == "symbol")
        {
            if (_dt == null)
                namelist(hfFolio["Folio"].ToString());
            //DataRow[] rows = Tablelist.Select("Formula='" + ActivePageSymbol + "'");
            //for (int i = 0; i < rows.Length; i++)
            //if (Tablelist.Rows.Count == 0) return;
            //result["Name"] = string.Format("{0}", Tablelist.Rows[Convert.ToInt32(args[1]) - 1]["Name"]);
            //result["RefSamples"] = string.Format("{0}", Tablelist.Rows[Convert.ToInt32(args[1]) - 1]["RefSamples"]);
            //result["Code"] = string.Format("{0}", Tablelist.Rows[Convert.ToInt32(args[1]) - 1]["Code"]);
            //result["Costper"] = string.Format("{0}", calculatecostper());
            //e.Result = result;
        }
        else if (args[0] == "LBOh")
        {
            var tableLBOh = cs.builditems(string.Format("select LBCode,((LBRate / convert(float,PackSize))* {0})LBRate from MasLaborOverhead where ID='{1}'",
                    tbPackSize.Text, args[1]));
            foreach (DataRow dr in tableLBOh.Rows)
            {
                result["LBCode"] = dr["LBCode"].ToString();
                result["Rate"] = dr["LBRate"].ToString();
            }
            e.Result = result;
        }
        else if (args[0] == "RawMaterial")
        {
            var table = new DataTable();//spGetHistory
            SqlParameter[] param = { new SqlParameter("@Material", args[1]),
                new SqlParameter("@RawMaterial",string.Format("{0}",args[2])),
                new SqlParameter("@Company",string.Format("{0}",CmbCompany.Value))};
            table = cs.GetRelatedResources("spGetMaterialYield", param);
            foreach (DataRow dr in table.Rows)
            {
                result["Name"] = dr["Name"].ToString();
                result["Yield"] = dr["Yield"].ToString();

            }
            e.Result = result;
        }
        else if (args[0] == "SAPMaterial")
        {
            string comp = (CmbCompany.Value == null) ? string.Empty : CmbCompany.Value.ToString();
            if (!string.IsNullOrEmpty(args[1]))
            {
                var value = string.Format("{0}", comp.ToString() + "|" + args[1] + "|" + hfRequestNo["ID"]);
                DataTable data = GetrequestRate(value);
                string[] rowValueItems = Regex.Split("Description;PriceOfUnit;Currency;Unit;ExchangeRate;BaseUnit", ";");
                if (data != null)
                    if (data.Rows.Count > 0)
                    {
                        foreach (DataRow dr in data.Rows)
                        {
                            foreach (string row in rowValueItems)
                                result[row] = string.Format("{0}", dr[row].ToString());
                        }
                    }
            }
            e.Result = result;
        }
        else
            e.Result = string.Format("{0}", e.Parameters);
    }
    //protected void cb_Callback(object source, CallbackEventArgs e)
    //{
    //    if (string.IsNullOrEmpty(e.Parameter))
    //        return;
    //    var args = e.Parameter.Split('|');
    //    //string labelID = e.Parameter;
    //    if (Tablelist == null) return;
    //    //Tablelist.Rows[Convert.ToInt32(args[1]) - 1]["Name"] = args[2];
    //    //Tablelist.AcceptChanges();
    //    string result = "from server";
    //    switch (args[0])
    //    {
    //        case "Code":
    //        case "Name":
    //        case "RefSamples":
    //            if (Tablelist.Rows.Count == 0) return;
    //            DataRow dr = Tablelist.Select(string.Format("Formula={0}", args[1].ToString())).FirstOrDefault();
    //            if (dr != null)
    //            {
    //                dr[args[0]] = string.Format("{0}", args[2]);
    //            }
    //            break;
    //    }
    //    e.Result = result;
    //}
    protected void treeList_DataBinding(object sender, EventArgs e)
    {
        ASPxTreeList treeList = sender as ASPxTreeList;

    }

    protected void treeList_CustomCallback(object sender, TreeListCustomCallbackEventArgs e)
    {
        ASPxTreeList treeList = sender as ASPxTreeList;
        if (string.IsNullOrEmpty(e.Argument))
            return;
        var args = e.Argument.Split('|');
        if (args[0] == "reload")
        {
            var Results = new DataTable();//spGetHistory
            SqlParameter[] param = { new SqlParameter("@Id", args[1]),
                new SqlParameter("@Param",string.Format("{0}",args[2]))};
            Results = cs.GetRelatedResources("spCreatePackSize", param);
            if (Results.Rows.Count > 0)
            {
                treeList.DataSource = (DataTable)Results;
                treeList.DataBind();
            }
        }
    }

    protected void treeList_CustomDataCallback(object sender, TreeListCustomDataCallbackEventArgs e)
    {

    }

    protected void treeList_CustomJSProperties(object sender, TreeListCustomJSPropertiesEventArgs e)
    {
        ASPxTreeList treeList = sender as ASPxTreeList;
        Hashtable nameTable = new Hashtable();
        foreach (TreeListNode node in treeList.GetVisibleNodes())
            nameTable.Add(node.Key, string.Format("{0}", node["PackSize"]));
        e.Properties["cpEmployeeNames"] = nameTable;
    }
    protected void grid_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        if (e.ButtonID != "EditCost") return;
        object keyValue = g.GetRowValues(e.VisibleIndex, g.KeyFieldName);
        DataRow[] tfound = tcustomer.Select("ID=" + keyValue.ToString());// + "and Component in ('Raw Material','Ingredient')");
        foreach (DataRow found in tfound)
        {
            //DataRow found = tcustomer.Rows.Find(keyValue);
            //t.Rows.Remove(found);
            found["Mark"] = found["Mark"].ToString() == "D" ? "" : "D";
            tcustomer.AcceptChanges();
        }
        g.DataBind();
    }
    protected void grid_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //bool isOddRow = e.VisibleIndex % 2 == 0;
        string Result = string.Format("{0}", g.GetRowValues(e.VisibleIndex, "Mark"));
        bool isOddRow = string.Format("{0}", Result) != "" && !string.IsNullOrEmpty(Result);
        if (e.ButtonID == "EditCost" && isOddRow)
        {
            if (Result == "D")
            {
                e.Image.Url = "~/Content/Images/Refresh.png";
                e.Image.ToolTip = "Return";
            }
        }
    }
    //protected void grid_RowInserting(object sender, ASPxDataInsertingEventArgs e)
    //{
    //    ASPxGridView g = (ASPxGridView)sender;
    //    DataRow row = tcustomer.NewRow();
    //    int NextRowID = tcustomer.Rows.Count;
    //    NextRowID++;
    //    row["ID"] = NextRowID;
    //    foreach (DataColumn column in tcustomer.Columns)
    //    {
    //        if (!column.ColumnName.Contains("ID"))
    //        {
    //            switch (column.ColumnName)
    //            {
    //                case "Mark":
    //                    row[column.ColumnName] = "X";
    //                    break;
    //                case "Formula":
    //                    row[column.ColumnName] = ActivePageSymbol;
    //                    break;
    //                case "Currency":
    //                    row[column.ColumnName] = "THB";
    //                    break;
    //                case "Unit":
    //                    row[column.ColumnName] = "KG";
    //                    break;
    //                default:
    //                    row[column.ColumnName] = e.NewValues[column.ColumnName];
    //                    break;
    //            }
    //        }
    //    }
    //    tcustomer.Rows.InsertAt(_found(row), 0);
    //    e.Cancel = true;
    //    g.CancelEdit();
    //}
    protected void gvCode_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //(sender as ASPxGridView).DataSource = LoadGrid;
        g.KeyFieldName = "ID";
        g.DataSource = _dt;
        g.ForceDataRowType(typeof(DataRow));
    }

    //protected void gvCode_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
    //    string[] param = e.Parameters.Split('|'); //bool selected = true;
    //    switch (param[0])
    //    {
    //        case "reload":
    //            Page.Session.Remove("setableKey");
    //            _dt = new DataTable();
    //            namelist(param[1]);
    //            break;
    //    }
    //    g.DataBind();
    //}

    protected void gvCode_DataBound(object sender, EventArgs e)
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
            but.ID = "colDel";
            but.Image.ToolTip = "Remove";
            but.Image.Url = "~/Content/Images/Cancel.gif";
            col.Width = Unit.Pixel(45);
            col.CustomButtons.Add(but);
            g.Columns.Add(col);
            g.Columns["CommandColumn"].VisibleIndex = 0;
        }
        if (g.VisibleRowCount > 5)
            g.Settings.VerticalScrollableHeight = 220;
        else
            g.Settings.VerticalScrollableHeight = g.VisibleRowCount * 23;
        //if(hfStatusApp["StatusApp"].ToString() == "0")
        //    g.Columns[0].Visible = false;
        //else
        //    g.Columns[1].Visible = false;
    }
    protected void gvCode_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e)
    {
        foreach (var args in e.UpdateValues)
        {
            if (_dt != null)
            {
                var values = new[] { "ID", "RequestNo" };
                DataRow dr = _dt.Rows.Find(args.Keys["ID"]);
                foreach (DataColumn column in _dt.Columns)
                {
                    if (!values.Any(column.ColumnName.Contains))
                    {
                        dr[column.ColumnName] = args.NewValues[column.ColumnName];
                    }
                }
                //dr["Data"] = e.NewValues["Data"];
            }
        }
        e.Handled = true;
    }
    protected void gvCode_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //DataTable dt = (DataTable)g.DataSource;//LoadGrid;
        if (_dt != null)
        {
            var values = new[] { "ID", "RequestNo" };
            DataRow dr = _dt.Rows.Find(e.Keys[0]);
            foreach (DataColumn column in _dt.Columns)
            {
                if (!values.Any(column.ColumnName.Contains))
                {
                    dr[column.ColumnName] = e.NewValues[column.ColumnName];
                }
            }
            //dr["Data"] = e.NewValues["Data"];
        }
        g.CancelEdit();
        e.Cancel = true;
    }
    protected void GroupCheckBox_Load(object sender, EventArgs e)
    {

    }
    protected void testgrid_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //bool isOddRow = e.VisibleIndex % 2 == 0;
        string Result = string.Format("{0}", g.GetRowValues(e.VisibleIndex, "Mark"));
        bool isOddRow = string.Format("{0}", Result) != "" && !string.IsNullOrEmpty(Result);
        if (e.ButtonID == "remove" && isOddRow)
        {
            if (Result == "D")
            {
                e.Image.Url = "~/Content/Images/Refresh.png";
                e.Image.ToolTip = "Return";
            }
        }
    }
    protected void OnFileDownloading(object source, FileManagerFileDownloadingEventArgs e)
    {
        if (e.File.Extension.Contains("pdf") || e.File.Extension.Contains("xls"))
        {
            string Id = e.File.Id;
            string contentType = MimeTypes.GetContentType(e.File.FullName);
            using (SqlConnection con = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandText = "SELECT * FROM FileSystem where ID=@Id";
                        cmd.Parameters.AddWithValue("@Id", Id);
                        cmd.Connection = con;
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            char[] charsToTrim = { '*', ' ', '\'', ',' };
                            string result = dr["Name"].ToString().Trim(charsToTrim);
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Response.ContentType = contentType;
                            Response.AppendHeader("Content-Disposition", "attachment; filename=" + result.Replace(",", "_"));
                            Response.BinaryWrite((byte[])dr["Data"]);
                            Response.Flush();
                            Response.End();
                        }
                    }
                    catch (Exception Ex)
                    {
                        throw new ApplicationException(Ex.Message);
                    }
                }
            }
            //e.InputStream;
        }
        e.Cancel = true;
    }
    const string UploadDirectory = "~/Content/UploadControl/";
    protected void UploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
    {
        string resultExtension = Path.GetExtension(e.UploadedFile.FileName);
        string resultFileName = Path.ChangeExtension(Path.GetRandomFileName(), resultExtension);
        string resultFileUrl = UploadDirectory + resultFileName;
        string resultFilePath = MapPath(resultFileUrl);
        //Save the uploaded file
        e.UploadedFile.SaveAs(resultFilePath);
        string name = e.UploadedFile.FileName;
        string url = ResolveClientUrl(resultFileUrl);
        long sizeInKilobytes = e.UploadedFile.ContentLength / 1024;
        string sizeText = sizeInKilobytes.ToString() + " KB";
        string Id = string.Format("{0}", hfid["hidden_value"]);
        byte[] FileData = myservice.ReadFile(resultFilePath);

        //byte[] data = e.UploadedFile.FileBytes;

        //ArtsDataSource.InsertParameters["Name"].DefaultValue = name;
        //ArtsDataSource.InsertParameters["ParentID"].DefaultValue = "24"; /* Files folder */
        //ArtsDataSource.InsertParameters["IsFolder"].DefaultValue = "false";
        //AccessDataSource1.InsertParameters["Data"].DefaultValue = data;
        //ArtsDataSource.InsertParameters["LastWriteTime"].DefaultValue = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        //ArtsDataSource.Insert();
        e.CallbackData = name + "|" + url + "|" + sizeText;

    }
    void savefile(string name, byte[] FileData, string user)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            string query = "spinsertFileSystem";
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", name.ToString());
                cmd.Parameters.AddWithValue("@IsFolder", 0);
                cmd.Parameters.AddWithValue("@ParentID", 1);
                cmd.Parameters.AddWithValue("@Data", FileData);
                cmd.Parameters.AddWithValue("@LastWriteTime", DateTime.Now.ToString());
                cmd.Parameters.AddWithValue("@GCRecord", String.Format("{0}", hfgetvalue["NewID"]));
                cmd.Parameters.AddWithValue("@LastUpdateBy", String.Format("{0}", user));
                cmd.Parameters.AddWithValue("@table", "TransCostingHeader");
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
    protected void OnCustomCallback(object sender, CallbackEventArgsBase e)
    {
        ASPxFileManager fm = (ASPxFileManager)sender;
        string[] param = e.Parameter.Split('|'); //bool selected = true;
        //int id;
        //if (!int.TryParse(param[1], out id))
        //    return;
        switch (param[0])
        {
            case "load":
                //ArtsDataSource.SelectParameters.Clear();
                //.SelectParameters.Add("GCRecord", string.Format("{0}", hfgetvalue["NewID"]));
                //ArtsDataSource.SelectParameters.Add("username", hfuser["user_name"].ToString());
                Session["GCRecord"] = string.Format("{0}", hfgetvalue["NewID"]);
                break;
        }

        fm.DataBind();
    }

    protected void fileManager_DataBinding(object sender, EventArgs e)
    {
        ASPxFileManager fm = (ASPxFileManager)sender;
        var edit = approv["approv"];
        FileManagerFolderAccessRule fr = new FileManagerFolderAccessRule();
        //if (edit.ToString() == "0")
        //{
        fr.Edit = edit.ToString() == "0" ? Rights.Allow : Rights.Deny;
        fr.Role = edit.ToString() == "0" ? "Admin" : "";
        fr.Path = "";
        fr.Browse = Rights.Allow;
        fr.Upload = Rights.Allow;
        fm.SettingsPermissions.AccessRules.Add(fr);
        fm.SettingsEditing.AllowDownload = true;
        //}
        //else
        //    fm.SettingsPermissions.AccessRules.Add(new FileManagerFolderAccessRule { Path = "", Edit = Rights.Deny, Browse = Rights.Deny });
    }
    string GetDescrip(string Code)
    {
        string name = "";
        SqlParameter[] param = { new SqlParameter("@Code", string.Format("{0}", Code)) };
        DataTable result = cs.GetRelatedResources("spGetTitleRawMaterial", param);
        if (result.Rows.Count > 0)
            name = result.Rows[0]["name"].ToString();
        return name.ToString();
    }
    protected void testgrid_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        var result = new Dictionary<string, string>();
        string[] param = e.Parameters.Split('|'); //bool selected = true;
        int id;
        if (!int.TryParse(param[1], out id))
            return;
        switch (param[0])
        {
            case "GetBatch":
                var Keys = g.GetRowValues(id, g.KeyFieldName).ToString();
                DataRow found = _dataTable.AsEnumerable()
                                .SingleOrDefault(r => r.Field<int>("ID") == Convert.ToInt32(Keys));
                //DataRow found = table.Rows.Find(id);
                double d;
                int index = Array.IndexOf(SubType, found["Component"].ToString().ToLower().Trim());
                if (index > -1)
                {
                    string str = (string)getloss(CmbPackaging.Text, index, "");
                    if (Double.TryParse(str, out d)) // if done, then is a number
                    {
                        double Amount;
                        if (Double.TryParse(found["Amount"].ToString(), out Amount))
                        {
                            double ro = (Amount * (d / 100));
                            if (!double.IsNaN(ro) || !double.IsInfinity(ro))
                                result["Loss"] = ro.ToString("F4");
                        }
                    }
                    double OutVal;
                    if (double.TryParse(found["Quantity"].ToString(), out OutVal))
                        result["Amount"] = Convert.ToSingle(Convert.ToDouble(found["PriceUnit"]) * OutVal).ToString();
                }
                e.Result = result;
                break;
        }
    }

    //protected void grid_InitNewRow(object sender, ASPxDataInitNewRowEventArgs e)
    //{
    //    if (tcustomer != null)
    //    {
    //        int NextRowID = tcustomer.Rows.Count;
    //        NextRowID++;
    //        e.NewValues["RowID"] = NextRowID;
    //        e.NewValues["Formula"] = ActivePageSymbol;
    //        e.NewValues["GUnit"] = 0;
    //        e.NewValues["Yield"] = 0;
    //    }
    //}

    protected void cpUpdateFiles_Callback(object sender, CallbackEventArgsBase e)
    {
        string[] param = e.Parameter.Split('|');
        switch (param[0])
        {
            //case "load":
            //    fileManager.DataBind();
            //    break;
            case "reload":
                GenerateDataSource(Guid.Parse(param[1]));
                break;
        }
    }
    void GenerateDataSource(Guid gcrecord)
    {
        //if (!string.IsNullOrEmpty(hfgetvalue["NewID"].ToString())){
        //    var gcrecord = string.Format("{0}", hfgetvalue["NewID"]);
        Session["DataSource"] = DataHelper.CreateDataSource(gcrecord.ToString(),
            user_name, "TransCostingHeader");
        fileManager.DataBind();
        //}
    }

    protected void CmbReceiver_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        var args = e.Parameter.Split('|');
        if (args[0] == "Add" || args[0] == "load")
        {
            List<Object> selectItems = gridData.GetSelectedFieldValues("ID");
            if (selectItems.Count > 0) {
                args[1] = cs.ReadItems(@"select b.RequestNo from TransFormulaHeader a inner join TransCostingHeader b on b.ID=a.RequestNo where a.id=" + selectItems[0]);
                    }
             
            var Results = new DataTable();
            SqlParameter[] param = { new SqlParameter("@id", args[1]),
                new SqlParameter("@utype",string.Format("{0}", usertp["usertype"]))};
            
            Results = GetRelatedResources("spGetAssignee", param);
            combobox.DataSource = Results;
            combobox.DataBind();
        }
    }
}
public class MyException : Exception
{
    public MyException(string message)
        : base(message)
    {
    }
}
public class CustomComparer : IEqualityComparer<DataRow>
{
    #region IEqualityComparer<DataRow> Members

    public bool Equals(DataRow x, DataRow y)
    {
        return ((string)x["Name"]).Equals((string)y["Name"]);
    }

    public int GetHashCode(DataRow obj)
    {
        return ((string)obj["Name"]).GetHashCode();
    }

    #endregion
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
        //box.Init += cbAll_Init;
        //box.ClientSideEvents.CheckedChanged = "OnAllCheckedChanged";
        container.Controls.Add(box);
    }
    protected void cbAll_Init(object sender, EventArgs e)
    {
        ASPxCheckBox chk = sender as ASPxCheckBox;
        //ASPxGridView grid = (chk.NamingContainer as GridViewHeaderTemplateContainer).Grid;
        GridViewDataItemTemplateContainer container = (GridViewDataItemTemplateContainer)chk.NamingContainer;
        //chk.Checked = (grid.Selection.Count == grid.VisibleRowCount);
        string key = string.Format("{0}|{1}", container.Column.FieldName, container.KeyValue);
        chk.ClientSideEvents.CheckedChanged = string.Format("function(s, e) {{ grid.PerformCallback('{0}|' + s.GetChecked()); }}", key);
    }
}

class combobox : ITemplate
{
    string ActivePageSymbol;
    int max;
    public combobox(string ActivePageSymbol, int max)
    {
        this.ActivePageSymbol = ActivePageSymbol;
        this.max = max;
    }
    protected void combo_Init(object sender, EventArgs e)
    {
        ASPxComboBox combo = sender as ASPxComboBox;
        combo.SelectedIndex = combo.Items.IndexOf(combo.Items.FindByValue(ActivePageSymbol));
    }
    public void InstantiateIn(Control container)
    {
        Func<int, string> Space = (n) => ("".PadRight(n));
        ASPxComboBox combo = new ASPxComboBox();
        combo.ClientInstanceName = "ClientPageSymbol";
        //combo.Init += combo_Init;
        //combo.Callback += combo_Callback;
        combo.ClientSideEvents.SelectedIndexChanged = "combo_SelectedIndexChanged";
        combo.Caption = Space(1) + string.Format("_Formula ({0}) ", max);
        combo.Width = Unit.Pixel(70);
        combo.DropDownWidth = Unit.Pixel(70);
        for (int i = 1; i <= max; i++)
            combo.Items.Add(i.ToString(), i);
        int index = combo.Items.IndexOf(combo.Items.FindByValue(ActivePageSymbol));
        combo.SelectedIndex = index;
        //if(max > 0)
        //combo.Value = ActivePageSymbol;
        container.Controls.Add(combo);
    }
    protected void combo_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combo = sender as ASPxComboBox;
        combo.Value = ActivePageSymbol;
    }
}
class ButtonTemplate : ITemplate
{
    public void InstantiateIn(Control _container)
    {
        GridViewEditItemTemplateContainer container = _container as GridViewEditItemTemplateContainer;
        ASPxButton buttonPrev = new ASPxButton();
        buttonPrev.ID = "buttonPrev";
        buttonPrev.Init += buttonPrev_Init;

        buttonPrev.Text = "<";
        buttonPrev.CssClass = "navButtons prevButton";
        buttonPrev.AutoPostBack = false;
        container.Controls.Add(buttonPrev);

        ASPxButton buttonNext = new ASPxButton();
        buttonNext.ID = "buttonNext";
        buttonNext.Init += buttonNext_Init;

        buttonNext.Text = ">";
        buttonNext.CssClass = "navButtons nextButton";
        buttonNext.AutoPostBack = false;
        container.Controls.Add(buttonNext);
    }
    protected void buttonPrev_Init(object sender, EventArgs e)
    {
        ASPxButton button = sender as ASPxButton;
        GridViewDataItemTemplateContainer container = button.NamingContainer as GridViewDataItemTemplateContainer;
        button.ClientSideEvents.Click = string.Format("function(s, e){{ grid.StartEditRow({0}-1); }}", container.VisibleIndex);
    }

    protected void buttonNext_Init(object sender, EventArgs e)
    {
        ASPxButton button = sender as ASPxButton;
        GridViewDataItemTemplateContainer container = button.NamingContainer as GridViewDataItemTemplateContainer;
        button.ClientSideEvents.Click = string.Format("function(s, e){{ grid.StartEditRow({0}+1); }}", container.VisibleIndex);
    }
}
public class TransCosting
{
    public int ID { get; set; }
    public string SubType { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    //public string SAPMaterial { get; set; }
    //public string Description { get; set; }
    public string Quantity { get; set; }
    public string PriceUnit { get; set; }
    public string Amount { get; set; }
    public string Per { get; set; }
    public string Currency { get; set; }
    public string Loss { get; set; }
    public int Formula { get; set; }
    public string RequestNo { get; set; }
    public string Mark { get; set; }
}