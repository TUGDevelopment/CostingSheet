using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using DevExpress.Web;
//using DevExpress.Web.Internal;
//using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
//using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using FusionCharts.Charts;
//using System.Drawing;
//using ClosedXML.Excel;
using DevExpress.Web.ASPxTreeList;
//using WebApplication;
using System.Collections.Specialized;
using WebApplication;
using DevExpress.Web.ASPxSpreadsheet;

public partial class UserControls_WebUserControl : MasterUserControl{
    MyCustomProviderOptions ProviderOptions { get; set; }
    MyDataModule cs = new MyDataModule();
    ServiceCS myservice = new ServiceCS();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    const string
    PreviewFormat =
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
    SelectAllCheckbox check = new SelectAllCheckbox();
    HttpContext ct = HttpContext.Current;
    //string sessionFile, seGetMyData, CustomTable, sessionlist;
    //private DataTable Tablelist
    //{
    //    get { return Page.Session["sessionlist"] == null ? null : (DataTable)Page.Session["sessionlist"]; }
    //    set { Page.Session["sessionlist"] = value; }
    //}
    public List<FileSystem> listFile
    {
        get { return Session["list"] == null ? null : (List<FileSystem>)Session["list"]; }
        set { Session["list"] = value; }
    }
    public List<TransProductList> listItems
    {
        get
        {
            var obj = this.Session["myList"];
            if (obj == null) { obj = this.Session["myList"] = new List<TransProductList>(); }
            return (List<TransProductList>)obj;
        }
        set
        {
            this.Session["myList"] = value;
        }
    }
    
    public List<TransStyle> listStyle
    {
        get
        {
            var obj = this.Session["myListStyle"];
            if (obj == null) { obj = this.Session["myListStyle"] = new List<TransStyle>(); }
            return (List<TransStyle>)obj;
        }
        set
        {
            this.Session["myListStyle"] = value;
        }
    }
    public List<TransDestination> listDestination
    {
        get
        {
            var obj = this.Session["myListDestination"];
            if (obj == null) { obj = this.Session["myListDestination"] = new List<TransDestination>(); }
            return (List<TransDestination>)obj;
        }
        set
        {
            this.Session["myListDestination"] = value;
        }
    }
    //
    string FilePath
    {
        get { return Page.Session["sessionFile"] == null ? String.Empty : Page.Session["sessionFile"].ToString(); }
        set { Page.Session["sessionFile"] = value; }
    }
    //public List<Document> listdoc
    //{
    //    get
    //    {
    //        var obj = this.Session["myList"];
    //        if (obj == null) { obj = this.Session["myList"] = new List<Document>(); }
    //        return (List<Document>)obj;
    //    }

    //    set
    //    {
    //        this.Session["myList"] = value;
    //    }
    //}
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    const string NotFoundMessageFormat = "<h1>Can't find message with the key={0}</h1>";
	private DataTable ustable
    {
        get { return Page.Session["CustomTable"] == null ? null : (DataTable)Page.Session["CustomTable"]; }
        set { Page.Session["CustomTable"] = value; }
    }
    protected string SearchText { get { return Utils.GetSearchText(Page); } }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        SetInitialRow();
    }

    public override void Update()
    {
        gridData.DataBind();
    }
    public void SetInitRoot()
    {
        Session["DataSource"] = DataHelper.Init();
    }
    public void SetInitialRow()
    {
        //hfOuter["Outer"] = "";
        //hfInner["Inner"] = "";

        XmlDataSource1.DataFile = "~/App_Data/Platforms.xml";
        if (cs.GetData(user_name, "usertype").ToString().Contains("0")){
            XmlDataSource1.XPath = "//custom";
        }else
            XmlDataSource1.XPath = "//editform";

        hfSellingUnit["SellingUnit"] = "";
        hfReceiver["Receiver"] = "";
        hfIngredient["Ingredient"] = "";
        hfPrdRequirement["PrdRequirement"] = "";
        hfCustomerRe["CustomerRe"] = "";
        this.Session["username"] = user_name;
        hfBU["BU"] = string.Format("{0}", cs.GetData(user_name, "bu"));
        usertp["usertype"] = cs.GetData(user_name, "usertype").ToString();
        Clientusertp["ut"]= string.Format("{0}", "");
        Session.Remove("seGetMyData");
        Session.Remove("CustomTable");
        hfgetvalue["NewID"] = string.Format("{0}", "");
        hfuser["user_name"] = user_name.ToString();
        editor["Name"] = cs.IsMemberOfRole(string.Format("{0}", 0));
        hftablename["tablename"] = "TransTechnical";
        hfRequestType["Type"] = string.Format("{0}", "");
        hfview["view"] = string.Format("{0}", "");
        approv["approv"] = string.Format("{0}", "");
        hftype["type"] = string.Format("{0}", 0);
        hfGeID["GeID"] = string.Format("{0}", 0);
		hfStatusApp["StatusApp"] = string.Format("{0}", 0);
        hfRole["Role"] = string.Format("{0}", cs.CheckRole());
        lbEventLog.Attributes.Add("style", "height:140px;width:150%;overflow:scroll");
        SetInitRoot();
        Initialfilter();
        GetRole();
        Update();
    }
	void GetRole(){
        string[] array = { "1", "2", "3", "4" };
        string strsql = string.Format(@"select Sublevel from MasApprovAssign where EmpId like '{0}'"
        ,user_name.ToString());
        string role = cs.ReadItems(strsql);
        if (!string.IsNullOrEmpty(role)){
            int temp = Array.IndexOf(array, role);
            CmbAssignee.SelectionMode = temp >-1 ? GridLookupSelectionMode.Multiple : GridLookupSelectionMode.Single;//ASPxGridLookup
            CmbAssignee.DataBind();
        }
    }
    void Initialfilter()
    {
        DateTime today = DateTime.Today;
        DateTime b = DateTime.Now.AddYears(-1);
        //filter.FilterExpression = string.Format("[CreateOn] Between(#{0}#, #{1}#)",
        //        b.ToString("yyyy-MM-dd"), today.ToString("yyyy-MM-dd"));
        filter.FilterExpression = string.Format("RequestType in ({0})", "0,2,3");
        StringBuilder sb = new StringBuilder();
        sb.Append(filter.GetFilterExpressionForMsSql());
        fExpr = sb.ToString();
        hfKeyword["Keyword"] = fExpr;
    }

    string SectionParameter
    {
        get
        {
            return Page.Request.QueryString["Section"];
        }
    }
    DataTable _selectData(long id)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spselectrequest";
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@username", hfuser["user_name"]);
            cmd.Connection = con;
            con.Open();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
        }
        return dt;
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
                var View = tbRequestNo.Text != "" ? tbRequestNo.Text.Substring(3, 1) : "1";
                result["view"] = String.Format("{0}", View);
                result["KeyValue"] = String.Format("{0}", id);
                e.Result = result;
            }
        }
        if (args[1] == "new" || args[1] == "unread")
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
                result["editor"] = String.Format("{0}",0);
                e.Result = result;
            }
        }
        if (args[1] == "EditDraft" || args[1]== "save") { 
            dt = _selectData(id);
            if (dt.Rows.Count == 0)
            {
                result["StatusApp"] = string.Format("{0}", 0);
                e.Result = result;
            }
            switch (args[1])
            {
                case "save":
                    foreach (DataRow dr in dt.Rows)
                    {
                        result["RequestType"] = dr["RequestType"].ToString();
                        var param = hfpara["para"].ToString().Split('|');
                        result["StatusApp"] = param[0] == "Copied" ? "0" : dr["StatusApp"].ToString();
                        //result["StatusApp"] = dr["StatusApp"].ToString();
                        e.Result = result;
                    }
                        break;
                case "EditDraft":
                    foreach (DataRow dr in dt.Rows)
                    {
                        result["RequestType"] = dr["RequestType"].ToString();
                        result["RequestNo"] = dr["RequestNo"].ToString();
                        result["Marketingnumber"] = dr["Marketingnumber"].ToString();
                        result["Requestfor"] = dr["Requestfor"].ToString();
                        result["Company"] = dr["Company"].ToString();
                        result["Validfrom"] = dr["Validfrom"].ToString();
                        result["Validto"] = dr["Validto"].ToString();
                        result["PetCategory"] = dr["PetCategory"].ToString();
                        result["PetFoodType"] = dr["PetFoodType"].ToString();
                        result["CompliedWith"] = dr["CompliedWith"].ToString();
                        result["NutrientProfile"] = dr["NutrientProfile"].ToString();
                        result["ProductType"] = dr["ProductType"].ToString();
                        result["ProductStyle"] = dr["ProductStyle"].ToString();
                        result["Media"] = dr["Media"].ToString();
                        result["ChunkType"] = dr["ChunkType"].ToString();
                        result["PackSize"] = dr["PackSize"].ToString();
                        result["Packaging"] = dr["Packaging"].ToString();
                        result["Material"] = dr["Material"].ToString();
                        result["PackType"] = dr["PackType"].ToString();
                        result["PackDesign"] = dr["PackDesign"].ToString();
                        result["PackColor"] = dr["PackColor"].ToString();
                        result["PackLid"] = dr["PackLid"].ToString();
                        result["PackShape"] = dr["PackShape"].ToString();
                        result["PackLacquer"] = dr["PackLacquer"].ToString();
                        result["SellingUnit"] = dr["SellingUnit"].ToString();
                        result["Drainweight"] = dr["Drainweight"].ToString();
                        result["ProductNote"] = dr["ProductNote"].ToString();
                        result["Notes"] = dr["Notes"].ToString();
                        result["CreateOn"] = dr["CreateOn"].ToString();
                        result["Customer"] = dr["Customer"].ToString();
                        result["Customprice"] = dr["Customprice"].ToString();
                        result["Brand"] = dr["Brand"].ToString();
                        result["Destination"] = dr["Destination"].ToString();
                        result["ESTVolume"] = dr["ESTVolume"].ToString();
                        result["ESTLaunching"] = dr["ESTLaunching"].ToString();
                        result["ESTFOB"] = dr["ESTFOB"].ToString();
                        result["StatusApp"] = dr["StatusApp"].ToString();
						result["Concave"] = string.Format("{0}", dr["Concave"]);
                        string[] arrRequest = dr["CustomerRequest"].ToString().Split('|');
                        if (arrRequest != null && arrRequest.Length > 1)         {
                            result["CustomerRequest"] = arrRequest[0].ToString();
                            result["OtherRole"] = arrRequest[1].ToString();
                        }
                        string values = dr["NetWeight"].ToString();
                        string[] array = values.Split('|');//Regex.Split(dr["NetWeight"], "|");
                        result["NetWeight"] = array[0];
                        result["NetUnit"] = array[1];
                        values = dr["Ingredient"].ToString();
                        array = values.Split('|');
                        result["Ingredient"] = array[0];
                        result["IngredientOther"] = array[1];
                        values = dr["Claims"].ToString();
                        array = values.Split('|');
                        result["Claims"] = array[0];
                        result["ClaimsOther"] = array[1];
                        result["ClaimsText"] = "";
                        var Results = new DataTable();
                        SqlParameter[] param = { new SqlParameter("@claim", array[0].ToString()) };
                        Results = GetRelatedResources("spGetclaimText", param);
                        foreach (DataRow xdr in Results.Rows)
                            result["ClaimsText"]=xdr["Name"].ToString().Replace(";", ",");
                        values = dr["VetOther"].ToString();
                        array = values.Split('|');
                        result["Vet"] = array[0];
                        result["etc"] = array[1];
                        values = dr["PhysicalSample"].ToString();
                        array = values.Split('|');
                        result["Physical"] = array[0].ToString();
                        result["PhysicalUnit"] = array[1].ToString();
                        result["Sample"] = array[2].ToString();
                        result["SampleUnit"] = array[3].ToString();
                        result["NewID"] = dr["UniqueColumn"].ToString();
                        result["Country"] = dr["Country"].ToString();
                        result["editor"] = dr["editor"].ToString();
                        result["ID"] = dr["ID"].ToString();
                        result["Receiver"] = dr["Receiver"].ToString();
                        result["ReceiverName"] = cs.GetData(dr["Receiver"].ToString(),"fn");
                        result["copied"] = dr["copied"].ToString();
                        result["CopiedID"] = dr["CopiedID"].ToString();
                        List<string> list = new List<string>();
                        string[] split = dr["assignee"].ToString().Split(new Char[] { ',' }, 
                            StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in split){
                            if (s.Trim() != "")
                                list.Add(cs.GetData(s, "fn"));
                        }
                        result["SampleDate"] = dr["SampleDate"].ToString();
                        result["assignee"] = String.Join(",", list.ToArray());
                        result["assigneeID"] = String.Format("{0}", dr["assignee"]);
                        result["UserType"] = dr["UserType"].ToString();
                        //result["Outer"] = dr["Outer"].ToString();
						string[] arrOuter = dr["Outer"].ToString().Split('|');
                        if (arrOuter != null && arrOuter.Length > 1)
                        {
                            result["Outer"] = arrOuter[0].ToString();
                            result["OuterOther"] = arrOuter[1].ToString().Replace(';', ',');
                        }
                        string[] arrInner = dr["Inner"].ToString().Split('|');
                        if (arrInner != null && arrInner.Length > 1) { 
                            result["Inner"] = arrInner[0].ToString().Replace(';',',');
                            result["InnerOther"] = arrInner[1].ToString();
                        }
                        string[] arrPrd = dr["PrdRequirement"].ToString().Split('|');
                        if(arrPrd!=null && arrPrd.Length > 1) {
                            result["PrdRequirement"] = arrPrd[0].ToString();
                            result["OtherPrd"] = arrPrd[1].ToString();
                        }
                        result["Eventlog"] = string.Format("{0}", DisplayEventlog(e));
                        e.Result = result;
                    }
                    break;
            }
        }
    }
    void Copied(string keyValue)
    {
        //var args = keyValue.ToString().Split('|');
        var getValue = keyValue.ToString();//args[0];
        if (getValue.ToString() != "0")
        {
            long id;string _form = "EditDraft";
            if (!long.TryParse(getValue, out id))
                return;
            var result = (DataTable)_selectData(id);
            _form = result.Rows[0]["RequestType"].ToString()=="1"?"Edit":_form;
            gridData.JSProperties["cpKeyValue"] = (getValue == null) ? string.Empty : string.Format("{0}|{1}", getValue, _form);
        }
    }
    string fExpr
    {
        get { return Session["fExpr"] == null ? String.Empty : Session["fExpr"].ToString(); }
        set { Session["fExpr"] = value; }
    }
    protected void gridData_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
		ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        long[] keys = null;
        if (args[0] == "Assignee")
        {
            string words = args[2].Replace('\n',',');
            string[] arr = words.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (string s in arr)
            //{
            //    if (s.Trim() != "") { 

            //    }
            //}
            SqlParameter[] param = { new SqlParameter("@ID", args[1].ToString()),
                    new SqlParameter("@Assignee",string.Join(",", arr)),
                    new SqlParameter("@Type",7)};
                DataTable table = new DataTable();
                var result = GetRelatedResources("spAssignDocument", param);
            goto jumptoexit;
        }
        if (!TryParseKeyValues(args.Skip(1), out keys))
            return;
		if (args[0] == "Pending"|| args[0] == "All")
        {
            //hftype["type"] = string.Format("{0}", args[0]== "Pending"?0:1);
            dsgv.DataBind();
        }
        if (args[0] == "Copied")
        {
            Copied(args[1]);
            SetInitRoot();
        }
        if (args[0]== "Split TRF" || args[0] == "Split_TRF")
        {
            string keyValue = args[1] + "|0";
            keys[0] = Convert.ToInt64(SplitTechnical(keyValue));
        }
        if (args[0] == "SaveMail" || args[0]== "read")
        {
            savedata(keys);
        }
        if (args[0] == "Delete" && args.Length > 1)
        {
            Delete(keys);
        }
        if (args[0] == "filter")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(filter.GetFilterExpressionForMsSql());
            fExpr = sb.ToString();
            hfKeyword["Keyword"] = fExpr;
        }
            if (args[0] == "build")
        {

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
        jumptoexit:
            Update();
    }

    void Delete(long[] keys)
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
    void ApproveStep(string keys)
    {
        if (CmbDisponsition.Value!=null)
        {
            string assignee = "";
            switch(CmbDisponsition.Value.ToString())
            {
                case "1":
                    assignee = GetAssignee(CmbAssignee.GridView, CmbAssignee.Text);
                    break;
                case "5": case "3": case "7":
                    assignee = CmbAssignee.Value.ToString();
                    break;
            } 
            DataTable dt = new DataTable();
            SqlParameter[] param = {new SqlParameter("@Id",keys.ToString()),
                        new SqlParameter("@User",hfuser["user_name"]),
                        new SqlParameter("@StatusApp",string.Format("{0}",CmbDisponsition.Value)),
                        new SqlParameter("@table",string.Format("{0}", "TransTechnical")),
                        new SqlParameter("@remark",mComment.Text),
                        new SqlParameter("@assign",string.Format("{0}", CmbAssignee.Value==null?"":assignee)),
                        new SqlParameter("@reason",string.Format("{0}", CmbReason.Text))};
            dt = cs.GetRelatedResources("spApproveStep", param);
            //SqlCommand cmd = new SqlCommand();
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.CommandText = "spApproveStep";
            //    cmd.Parameters.AddWithValue("@Id", keys.ToString());
            //    cmd.Parameters.AddWithValue("@User", hfuser["user_name"]);
            //    cmd.Parameters.AddWithValue("@StatusApp", CmbDisponsition.Value);
            //    cmd.Parameters.AddWithValue("@table", string.Format("{0}", "TransTechnical"));
            //    cmd.Parameters.AddWithValue("@remark", mComment.Text);
            //    cmd.Parameters.AddWithValue("@reason", string.Format("{0}", (CmbDisponsition.Value.ToString() == "3") ? CmbReason.Text : CmbReason.Value));
            //    cmd.Connection = con;
            //    con.Open();
            //    this._dataTable dt = new DataTable();
            //    SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            //    oAdapter.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                //string[] arr = {keys.ToString(),
                //      dr["RequestNo"].ToString(),
                //      dr["MailTo"].ToString(),
                //      dr["MailCc"].ToString(),
                //      dr["Requester"].ToString(),
                //      dr["Assignee"].ToString(),
                //      dr["Subject"].ToString(),
                //      dr["Revised"].ToString(),
                //      dr["StatusApp"].ToString()};
                sendmail(dr, keys);
            }
            //        con.Close();
            //}
        }
    } 
    void sendmail(DataRow dr,string keys)
    {
        string utype = dr["UserType"].ToString();
        string name = dr["NamingCode"].ToString();
        //ArrayList list = new ArrayList();
		List<string> myList = new List<string>();
        List<string> list = new List<string>();
        List<string> listsender = new List<string>();
        //var args = keys.Split('|');
        string _text = "";
        string[] split = dr["MailTo"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in split)
        {
            if (s.Trim() != "") { 
                list.Add(cs.GetData(s,"email"));
                listsender.Add(cs.GetData(s, "fn"));
				myList.Add(s);
            }
        }
        string sender = String.Join(",", list.ToArray());
        list = new List<string>();
        split = dr["MailCc"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in split)
        {
            if (s.Trim() != "")
				if (!myList.Contains(s)){
					list.Add(cs.GetData(s,"email"));
					myList.Add(s);
			}
        }
        //Add sender  
        //Add 'Dear XXX'  
        //Add PM/CD Assignee
        switch (string.Format(@"{0}", dr["StatusApp"]))
        {
            case "2":
                _text = "Dear Costing Team<br/> ";
                break;
            default:
                _text = string.Format("Dear {0} <br/> ", String.Join(",", listsender.ToArray()));
                break;
        }
        //if (!args[6].Contains("1"))
        //list.Add(cs.GetData(user_name, "email"));
        string MailCc = String.Join(",", list.ToArray());
        int i = (CmbDisponsition.Value.ToString() == "7") ? 3 : 2;
        //string statusapp = dr["StatusApp"].ToString();// " was "+ CmbDisponsition.Text;
        string form  = (dr["RequestNo"].ToString().Substring(3,1) == "1") ? "Edit" : "EditDraft";
		string subject = string.Format(@"{0}:{1} Request No.: " + dr["RequestNo"].ToString() + " V.{2} ," 
            + tbCustomerName.Text + "/", name.ToString(), dr["Subject"], string.Format("{0:00}", dr["Revised"]));
        //subject += tbBrandName.Text + "/" + CmbProductStyle.Text + "/" + CmbPrimary.Text + "/" + tbNetweight.Text + "/" + CmbNetUnit.Text;
        string _link = "Request No.:" + dr["RequestNo"].ToString();
        _link += "<br/> Request Type : " + string.Format("{0}",cbRequestType.Text);
        _link += " < br/> Customer Name : " + tbCustomerName.Text;
        _link += "<br/> Brand Name : " + tbBrandName.Text;

        if (utype.ToString() != "0")
        {
            subject += tbBrandName.Text + "/" + CmbPrimary.Text;
            _link += "<br/> Packaging : " + CmbPrimary.Text;
        }
        else
        {
            subject += tbBrandName.Text + "/" + CmbProductStyle.Text + "/" + CmbPrimary.Text + "/" + tbNetweight.Text + "/" + CmbNetUnit.Text;
            _link += "<br/> Product Style : " + CmbProductStyle.Text;
            _link += "<br/> Packaging : " + CmbPrimary.Text;
            _link += "<br/> N/W : " + tbNetweight.Text + "/" + CmbNetUnit.Text;
            _link += "<br/> Destination : " + string.Format("{0}", tbDestination.Text);
            _link += "<br/> Pet Category : " + string.Format("{0}", CmbPetCategory.Text);
            _link += "<br/> Pet Food Type : " + string.Format("{0}", CmbPetFoodType.Text);
        }
        _link += "<br/> Request By : " + cs.GetData(dr["Requester"].ToString(),"fn");

        if (CmbDisponsition.Value.ToString() == "5")
        {
            string ass = string.Format("{0}", CmbAssignee.Value);//CmbAssignee
        _link += "<br/> Assignee : "+ cs.GetData(ass, "fn");
        }
        else if (CmbDisponsition.Value.ToString() == "1" && CmbReceiver.Text != ""){
            _link += "<br/> R&D : " + CmbReceiver.Text;
            _link += "<br/> PM/CD Assignee : " + CmbAssignee.Text.ToString();
        }
        if (!string.IsNullOrEmpty(CmbReason.Text) && CmbDisponsition.Text =="Reject")
        _link += "<br/> Reason :" + CmbReason.Text;
        _link += "<br/> Sender : " + cs.GetData(user_name.ToString(), "fn");
        _link += "<br/> Comment :" + mComment.Text;
        _link += @"<br/> The document link --------><a href="+ cs.GetSettingValue() 
            + "/Default.aspx?viewMode=CustomerEditForm&ID=" + keys.ToString() + "&form="+ form + "&UserType=" + utype; 
        _link += " style='color: rgb(0,255,0)'><font color='#0000FF'>click</font></a>";
        //cs.insertsendmail(sender, MailCc, _link, "system costing sheet Request No.:" + args[1].ToString() + statusapp);
        //cs.sendemail(sender, MailCc, string.Format("{0}{1}", _text,_link), subject);
    }
    void savedata(long[] keys)
    {
        var args = hfpara["para"].ToString().Split('|');
        var keyref = "";
        if (args[0] == "Revised")
        {
            string keyValue = args[1] + "|0";
            keys[0] = Convert.ToInt64(copyTechnical(keyValue));
        }
        //if (args[0] == "Split TRF" || args[0] == "Split_TRF")
        //{
        //    string keyValue = args[1] + "|0";
        //    keys[0] = Convert.ToInt64(SplitTechnical(keyValue));

        //}
        if (args[0] == "Copied")
        {
            keys[0] = 0; keyref = args[1];
        }
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spinsertTechnical";
            cmd.Parameters.AddWithValue("@RequestNo", keys[0].ToString());
            cmd.Parameters.AddWithValue("@RDNumber", keyref.ToString());
            cmd.Parameters.AddWithValue("@Requester", hfuser["user_name"]);
            cmd.Parameters.AddWithValue("@Company", cmbCompany.Value);
            cmd.Parameters.AddWithValue("@From", defrom.Value);
            cmd.Parameters.AddWithValue("@To", deto.Value);
            cmd.Parameters.AddWithValue("@PetCategory", string.Format("{0}",CmbPetCategory.Value));
            cmd.Parameters.AddWithValue("@PetFoodType", CmbPetFoodType.Text);
            cmd.Parameters.AddWithValue("@CompliedWith", CmbCompliedWith.Text);
            cmd.Parameters.AddWithValue("@NutrientProfile", CmbNutrientProfile.Text);
            cmd.Parameters.AddWithValue("@Requestfor", cbRequestType.Text);
            cmd.Parameters.AddWithValue("@Color", CmbColor.Text);
            cmd.Parameters.AddWithValue("@PrdRequirement", hfPrdRequirement["PrdRequirement"].ToString().Replace(',', ';') + '|' + tbOtherPrd.Text);
            cmd.Parameters.AddWithValue("@SecInner", 0);
            cmd.Parameters.AddWithValue("@SecOuter", 0);
            //cmd.Parameters.AddWithValue("@SecInner", GetCheckedItems(cbInner) + '|' + tbInnerOther.Text);
            //cmd.Parameters.AddWithValue("@SecOuter", GetCheckedItems(cbOuter) + '|' + tbOuterOther.Text);
            //cmd.Parameters.AddWithValue("@SecInner", hfInner["Inner"].ToString().Replace(',', ';') + '|' + tbInnerOther.Text);
            //cmd.Parameters.AddWithValue("@SecOuter", hfOuter["Outer"].ToString().Replace(',', ';') + '|' + tbOuterOther.Text);
            cmd.Parameters.AddWithValue("@ProductType", CmbProductType.Text);
            cmd.Parameters.AddWithValue("@ProductStyle",CmbProductStyle.Text);
            cmd.Parameters.AddWithValue("@Media", CmbMedia.Text);
            cmd.Parameters.AddWithValue("@ChunkType", CmbChunkType.Text);
            cmd.Parameters.AddWithValue("@NetWeight", tbNetweight.Text +"|"+ CmbNetUnit.Text);
            cmd.Parameters.AddWithValue("@Marketingnumber", "");
            cmd.Parameters.AddWithValue("@PackSize", tbPackSize.Text);
            cmd.Parameters.AddWithValue("@Primary", CmbPrimary.Text);
            cmd.Parameters.AddWithValue("@Material", CmbMaterial.Text);
            cmd.Parameters.AddWithValue("@PackageType", CmbPFType.Text);
            cmd.Parameters.AddWithValue("@Design", CmbDesign.Text);
            cmd.Parameters.AddWithValue("@Lid", CmbPFLid.Text);
            cmd.Parameters.AddWithValue("@PackagingShape", CmbShape.Text);
            cmd.Parameters.AddWithValue("@Lacquer", CmbLacquer.Text);
            cmd.Parameters.AddWithValue("@SellingUnit", CmbSellingUnit.Text);
            
            cmd.Parameters.AddWithValue("@Drainweight", tbSize.Text);
            cmd.Parameters.AddWithValue("@Notes", mNotes.Text);
            cmd.Parameters.AddWithValue("@Customer", tbCustomerName.Text);
            cmd.Parameters.AddWithValue("@Brand", tbBrandName.Text);
            cmd.Parameters.AddWithValue("@Destination", tbDestination.Text);
            cmd.Parameters.AddWithValue("@Country", tbCountry.Text);
            cmd.Parameters.AddWithValue("@ESTVolume", tbESTVolume.Text);
            cmd.Parameters.AddWithValue("@ESTLaunching", tbESTLaunching.Text);
            cmd.Parameters.AddWithValue("@ESTFOB", tbESTFOB.Text);
            cmd.Parameters.AddWithValue("@ProductNote", tbProductNote.Text);
            cmd.Parameters.AddWithValue("@CustomerRequest", hfCustomerRe["CustomerRe"].ToString().Replace(',',';') + '|' + tbOtherRole.Text);
            cmd.Parameters.AddWithValue("@Ingredient", hfIngredient["Ingredient"].ToString().Replace(',', ';') + "|" + tbIngredientOther.Text);
            cmd.Parameters.AddWithValue("@Concave", string.Format("{0}", rbConcave.Value));
            cmd.Parameters.AddWithValue("@Claims", GetCheckedItems(cbclaims)+";"+ GetClaims() + "|"+ tbclaimsOther.Text);
            cmd.Parameters.AddWithValue("@VetOther", GetCheckedItems(cbVet) + "|" + tbetc.Text);
            cmd.Parameters.AddWithValue("@PhysicalSample", GetCheckedItems(cbPhysical) + "|" + tbPhysicalUnit.Text +"|"+ GetCheckedItems(cbSample) +"|"+  tbSampleUnit.Text );
            cmd.Parameters.AddWithValue("@RequestType", string.Format("{0}", (CmbReceiver.Text == "") ? "2" : "0"));
            cmd.Parameters.AddWithValue("@Receiver", string.Format("{0}",cs.Getuser_name(CmbReceiver.Text, "user_name")));
            cmd.Parameters.AddWithValue("@NewID", string.Format("{0}", hfgetvalue["NewID"]));
            cmd.Parameters.AddWithValue("@customprice", string.Format("{0}", CmbCustomerPrice.Value));
            cmd.Parameters.AddWithValue("@StatusApp", string.Format("{0}", CmbDisponsition.Value));
            cmd.Parameters.AddWithValue("@Assignee", string.Format("{0}", 0));
            cmd.Parameters.AddWithValue("@usertype", string.Format("{0}", GetUserType()));
            cmd.Parameters.AddWithValue("@SampleDate", deSampleDate.Value);
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                //SaveFileToDB(dr["ID"].ToString());
                //if (!string.IsNullOrEmpty(CmbDisponsition.Text))
                //switch (CmbDisponsition.Value.ToString())
                //{
                //    case "5":
                //    case "6":
                //    case "7":
                //        string Id = dr["ID"].ToString();
                //        SqlParameter[] param = { new SqlParameter("@ID", Id),
                //        new SqlParameter("@Assignee",CmbReason.Value),
                //        new SqlParameter("@Type",CmbDisponsition.Value)};
                //        DataTable table = new DataTable();
                //        var result = GetRelatedResources("spAssignDocument", param);
                //        foreach (DataRow row in result.Rows)
                //        if(string.Format("{0}",CmbDisponsition.Value)!="6")
                //            ApproveStep(dr["ID"].ToString());
                //        else
                //        sendmail(string.Format("{0}|{1}|{2}|{3}|{4}",
                //            Id.ToString(),
                //            row["RequestNo"],
                //            row["Assignee"], 
                //            "",
                //            row["Requester"]));
                //        break;
                //    default:
                var leaveNo = hfgetvalue["NewID"].ToString();
                var newleavepath = @"~\memberpages\Leave\documents\" + leaveNo;
                //if (Directory.Exists(Server.MapPath(newleavepath)))
                //    Directory.Delete(newleavepath, true);
                List<FileSystemData> listdata = (List<FileSystemData>)Session["DataSource"];
                SqlCommand sqlComm = new SqlCommand();
                sqlComm.CommandText = @"DELETE FROM FileSystem WHERE GCRecord=@ID";
                sqlComm.Parameters.AddWithValue("@ID", Guid.Parse(dr["UniqueColumn"].ToString()));
                DataTable _dt = myservice.GetData(sqlComm);
                foreach (var d in listdata)
                {
                    if (d.IsFolder==false)
                    {
                        //if (!Directory.Exists(Server.MapPath(newleavepath)))
                        //{
                        //    Directory.CreateDirectory(Server.MapPath(newleavepath));
                        //}
                        //string resultFilePath = Server.MapPath(newleavepath) + @"\" + d.Name;
                        //byte[] FileData = myservice.ReadFile(resultFilePath);
                        savefile(d.Name.ToString(), d.Data, d.LastUpdateBy);
                    }
                }
                Page.Session.Remove("DataSource");
                insertname(dr["ID"].ToString());
                ApproveStep(dr["ID"].ToString());
                //        break;
                //}

            }
            con.Close();
        }
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
                cmd.Parameters.AddWithValue("@table", "TransTechnical");
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
    string GetClaims()
    {
        if (string.IsNullOrEmpty(ddelistclaim.Text)) return "";
        List<string> list = new List<string>();
        var Results = new DataTable();
        SqlParameter[] param = { new SqlParameter("@claim", (string)ddelistclaim.Text) };
        Results = GetRelatedResources("spGetclaim", param);
        foreach(DataRow dr in Results.Rows)
        {
            list.Add(dr["Name"].ToString().Replace(",",";"));
        }
        return String.Join(";", list.ToArray()); 
    }
    string GetUserType(){
        if (CmbPetCategory.Value==null || cmbCompany.Value == null) return "";
        string strSQL = string.Format(@"(select usertype from MasPetCategory where dbo.fnc_checktype({0}, ID) > 0 and dbo.fnc_checktype(Factory, {1}) > 0)"
          , CmbPetCategory.Value, cmbCompany.Value);
        return cs.ReadItems(strSQL);
    }
    void insertname(string keys)
    {
        //
        foreach (var des in listDestination)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spinsertDestination";
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", des.Id));
                cmd.Parameters.AddWithValue("@Country", string.Format("{0}", des.Country));
                cmd.Parameters.AddWithValue("@Zone", string.Format("{0}", des.Zone));
                cmd.Parameters.AddWithValue("@Mark", string.Format("{0}", des.Mark));
                cmd.Parameters.AddWithValue("@RequestNo", keys.ToString());
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        foreach (var c in listItems)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spinsertProductLst";
                cmd.Parameters.AddWithValue("@name", c.Name.ToString());
                cmd.Parameters.AddWithValue("@NetWeight", string.Format("{0}", c.NetWeight));
                cmd.Parameters.AddWithValue("@SaltContent", string.Format("{0}", c.SaltContent));
                cmd.Parameters.AddWithValue("@DWType", c.DWType.ToString());
                cmd.Parameters.AddWithValue("@Mark", c.Mark.ToString());
                cmd.Parameters.AddWithValue("@FixedFillWeight", string.Format("{0}", c.FixedFillWeight));
                cmd.Parameters.AddWithValue("@PW", c.PW.ToString());
                cmd.Parameters.AddWithValue("@TargetPrice", c.TargetPrice.ToString());
                cmd.Parameters.AddWithValue("@DW", c.DW.ToString());
                cmd.Parameters.AddWithValue("@Efficiency", c.Efficiency.ToString());
                cmd.Parameters.AddWithValue("@Yield", c.Yield.ToString());
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", c.Id));
                cmd.Parameters.AddWithValue("@RDCode", string.Format("{0}", c.RDCode));
                cmd.Parameters.AddWithValue("@PackSize", string.Format("{0}", c.PackSize));
                //cmd.Parameters.AddWithValue("@Country", string.Format("{0}", c.Country));
                //cmd.Parameters.AddWithValue("@Destination", string.Format("{0}", c.Destination));

                cmd.Parameters.AddWithValue("@RequestNo", keys.ToString());
                cmd.Connection = con;
                con.Open();
                var getValue = cmd.ExecuteScalar();//cmd.ExecuteNonQuery();
                con.Close();
                insertformula(getValue.ToString(), c.formula, keys.ToString());
            }
        }
    }
    void deleteUpCharge(string keyValue, string list)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spdelTransCusFormula";
            cmd.Parameters.AddWithValue("@Id", keyValue.ToString());
            cmd.Parameters.AddWithValue("@list", list.ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    void insertformula(string getValue, List<TransCusFormula> lst ,string keys) {
        List<string> list = new List<string>();
        foreach (var c in lst)
        {
            list.Add(c.ID.ToString());
        }
        if (list.Count > 0) {
            string del = String.Join(",", list.ToArray());
            deleteUpCharge(keys, del);
        }
        foreach (var c in lst)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spinsertCusFormulafortrf";
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", c.ID));
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
                cmd.Parameters.AddWithValue("@Formula", string.Format("{0}", getValue));

                cmd.Parameters.AddWithValue("@IsActive", string.Format("{0}", c.IsActive));
                cmd.Parameters.AddWithValue("@Remark", string.Format("{0}", c.Remark));
                cmd.Parameters.AddWithValue("@LBOh", string.Format("{0}", c.LBOh));
                cmd.Parameters.AddWithValue("@LBRate", string.Format("{0}", c.LBRate));
                cmd.Parameters.AddWithValue("@RequestNo", string.Format("{0}", keys));
                cmd.Parameters.AddWithValue("@SubID", string.Format("{0}", 0));
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
    string GetAssignee(ASPxGridView g,string s)
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
    public string GetCheckedItems(ASPxCheckBoxList cb)
    {
        string result="";
        StringBuilder sb = new StringBuilder();
        foreach(ListEditItem item in cb.Items)
        {
            if (item.Selected)
                sb.Append(item.Value + ";");
            if(!string.IsNullOrEmpty(sb.ToString()))
            {
                result = sb.ToString();
                result = result.Substring(0, (result.Length - 1));
            }
        }
        return result;
    }
    //protected void CmbPFType_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    ASPxComboBox combobox = (ASPxComboBox)sender;
    //    //string strSQL = @"select ID,Name from(Select * from MasPFType Where pf='1' and '" + e.Parameter;
    //    //strSQL += "' in (select value from dbo.FNC_SPLIT(tcode,',')))#a";
    //    FillCityCombo(dsPFType, combobox);
    //}

    protected void FillCityCombo(SqlDataSource ds, ASPxComboBox combobox)
    {
        DataTable dt = new DataTable();
        combobox.DataBind();
        DataView dv= (DataView)ds.Select(DataSourceSelectArguments.Empty);
        if (dv != null)
        {
            dt = dv.Table;
            if (dt.Rows.Count == 0)
            {
                combobox.Items.Insert(0, new ListEditItem("-"));
                combobox.SelectedIndex = 0;
            }
            else if (dt.Rows.Count == 1)
                combobox.SelectedIndex = 0;
        }
    }
    //protected void CmbDesign_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    ASPxComboBox combobox = (ASPxComboBox)sender;
    //    //string strSQL = @"Select ID,Name from(select * from MasDesign Where pf='1' and '" + e.Parameter;
    //    //strSQL += "' in (select value from dbo.FNC_SPLIT(tcode,',')))#a";
    //    FillCityCombo(dsDesign, combobox);
    //}

    //protected void CmbPFLid_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    ASPxComboBox combobox = (ASPxComboBox)sender;
    //    //string strSQL = @"select ID,Name from(Select * from MasPFLid Where pf='1' and '" + e.Parameter;
    //    //strSQL += "' in (select value from dbo.FNC_SPLIT(tcode,',')))#a";
    //    FillCityCombo(dsPFLid, combobox);
    //}

    //protected void popup_WindowCallback(object source, PopupWindowCallbackArgs e)
    //{
    //    litText.Text = e.Parameter.ToString();
    //}
    public string SplitTechnical(string keyValue)
    {
        var args = keyValue.ToString().Split('|');
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spSplitTechnical";
            cmd.Parameters.AddWithValue("@RequestNo", string.Format("{0}", args[0]));
            cmd.Parameters.AddWithValue("@Requester", hfuser["user_name"]);
            cmd.Parameters.AddWithValue("@RequestType", string.Format("{0}", args[1]));
            //cmd.Parameters.AddWithValue("@myid", string.Format("{0}", hfgetvalue["NewID"]));
            cmd.Parameters.AddWithValue("@usertype", string.Format("{0}", GetUserType()));
            cmd.Connection = con;
            con.Open();
            var getValue = cmd.ExecuteScalar();
            con.Close();
            string sql = "select * from TransTechnical where id=" + getValue.ToString();
            foreach (DataRow _r in cs.builditems(sql).Rows){
                hfgetvalue["NewID"] = string.Format("{0}", _r["UniqueColumn"]);
            }
            return string.Format("{0}", getValue);
        }
    }
    public string copyTechnical(string keyValue) {
        var args = keyValue.ToString().Split('|');
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spcopyTechnical";
            cmd.Parameters.AddWithValue("@RequestNo", string.Format("{0}", args[0]));
            cmd.Parameters.AddWithValue("@Requester", hfuser["user_name"]);
            cmd.Parameters.AddWithValue("@RequestType", string.Format("{0}", args[1]));
            cmd.Parameters.AddWithValue("@myid", string.Format("{0}", hfgetvalue["NewID"]));
            cmd.Parameters.AddWithValue("@usertype", string.Format("{0}", GetUserType()));
            cmd.Connection = con;
            con.Open();
            var getValue = cmd.ExecuteScalar();
            con.Close();
            return string.Format("{0}",getValue);
        }
    }
    protected void gridData_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {
        if (e.ButtonID != "Clone") return;
        ASPxGridView g = (ASPxGridView)sender;
        //string keyValue = g.GetRowValues(e.VisibleIndex, "RequestNo").ToString();
        object keyValue = g.GetRowValues(e.VisibleIndex, g.KeyFieldName);
        var args = keyValue.ToString().Split('|');
        //string keyValue = g.GetRowValues(g.FocusedRowIndex, "ID").ToString();
        //gridData.JSProperties["cpKeyValue"] = keyValue;
        //var getValue = copyTechnical(keyValue+"|1");
        //gridData.JSProperties["cpKeyValue"] = (getValue == null) ? string.Empty : getValue.ToString();
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

 
    DataTable GetElement(string Element)
    { //spGetElement
        var Results = new DataTable();
        SqlParameter[] param = {new SqlParameter("@tablename",(string)Element) };
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

    protected void CmbDisponsition_Callback(object sender, CallbackEventArgsBase e)
    {
        if (string.IsNullOrEmpty(e.Parameter)) return;
        //if (string.IsNullOrEmpty(e.Parameters))
        //    return;
        //var args = e.Parameters.Split('|');
        //DataTable table = new DataTable();
        //table.Columns.AddRange(new DataColumn[2] { new DataColumn("ID", typeof(int)),
        //          new DataColumn("Name",typeof(string)) });
        SqlParameter[] param = {new SqlParameter("@Id",(string)e.Parameter),
                        new SqlParameter("@table","TransTechnical"),
                        new SqlParameter("@username",hfuser["user_name"])};
        DataTable table = new DataTable();
        table= GetRelatedResources("spGetStatusApproval", param);
        CmbDisponsition.DataSource = table;
        CmbDisponsition.DataBind();
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
        string Id = string.Format("{0}", hfGeID["GeID"]);
        byte[] FileData = myservice.ReadFile(resultFilePath);
        savefile(name.ToString(), FileData, cs.CurUserName);
        //using (SqlConnection con = new SqlConnection(strConn))
        //{
        //    string query = "spinsertFileSystem";
        //    using (SqlCommand cmd = new SqlCommand(query))
        //    {
        //        cmd.Connection = con;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@Name", name.ToString());
        //        cmd.Parameters.AddWithValue("@IsFolder", 0);
        //        cmd.Parameters.AddWithValue("@ParentID", 1);
        //        cmd.Parameters.AddWithValue("@Data", FileData);
        //        cmd.Parameters.AddWithValue("@LastWriteTime", DateTime.Now.ToString());
        //        cmd.Parameters.AddWithValue("@GCRecord", String.Format("{0}", hfgetvalue["NewID"]));
        //        cmd.Parameters.AddWithValue("@LastUpdateBy", String.Format("{0}", hfuser["user_name"]));
        //        cmd.Parameters.AddWithValue("@table", "TransTechnical");
        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //        con.Close();
        //    }
        //}
        //byte[] data = e.UploadedFile.FileBytes;
        string strSQL = string.Format(@"select * from TransTechnical where UniqueColumn='{0}'", hfgetvalue["NewID"]);
		var result = cs.builditems(strSQL);
		foreach (DataRow r in result.Rows){
			if (r["Requestfor"].ToString().Contains("Product Specification") && name.Contains(".pdf") && hfRole["Role"].ToString().Contains("5")){
				cs._alertmail(r, name, "SPEC UPLOADED", GetUserType().ToString());
			}
		}
        e.CallbackData = name + "|" + url + "|" + sizeText;

    }
     protected void gridData_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //bool isOddRow = e.VisibleIndex % 2 == 0;
        bool isOddRow = string.Format("{0}", g.GetRowValues(e.VisibleIndex, "RequestType")) == "1";
        //var  type=  g.GetRowValues(e.VisibleIndex, "RequestType");
        if (e.ButtonID == "btnDetails" && isOddRow) { 
        e.Image.Url = "~/Content/Images/if_stock_zoom-object_21726.png";
        e.Image.ToolTip = "Display";}
    }

    protected void hyperLink_Init(object sender, EventArgs e)
    {
        ASPxHyperLink link = (ASPxHyperLink)sender;
        GridViewDataItemTemplateContainer templateContainer = (GridViewDataItemTemplateContainer)link.NamingContainer;
        int rowVisibleIndex = templateContainer.VisibleIndex;
        string ean13 = templateContainer.Grid.GetRowValues(rowVisibleIndex, "RequestNo").ToString();
        //string contentUrl = string.Format("{0}?EAN13={1}", Session["baseURL"], ean13);
        if (ean13.Substring(3, 1) == "0")
        {
            link.ImageUrl = "~/Content/images/MathIcons/3DPlot.png";
            link.NavigateUrl = "javascript:void(0);";
        }
        //else
        //    link.ImageUrl = "~/Content/images/Refresh.png";
        //link.Text = string.Format("More Info: EAN13 - {0}", ean13);
        //link.ClientSideEvents.Click = string.Format("function(s, e) {{ OnMoreInfoClick('{0}'); }}", contentUrl);
    }
    protected void gridData_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
    {
        if (e.RowType == GridViewRowType.Data)
        {
            //e.Row.Attributes.Add("onmouseover", "OnCellMouseOver(this, event)");
            //e.Row.Attributes.Add("onmouseout", "OnCellMouseOut()");
        }
    }
    bool _result(string keys)
    {
        var _b = false;
        DataView dvsqllist = (DataView)dsgv.Select(DataSourceSelectArguments.Empty);
        if (dvsqllist != null)
        {
            DataTable dt = dvsqllist.Table;
            DataRow[] mytable = dt.Select("ID='" + keys.ToString() + "'");
            foreach (DataRow dr in mytable)
                if (dr["RequestType"].ToString() == "0" || dr["RequestType"].ToString() == "3")
                    _b = true;
        }
         return _b;
    }
    protected void gridData_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
    {
        if(e.DataColumn.Caption == "More Info")
        {
            string value = e.GetValue("ID").ToString();
			if (_result(value) == false) return;
            e.Cell.Attributes.Add("onmouseover", "OnCellMouseOver("+ value + ", event)");
            e.Cell.Attributes.Add("onmouseout", "OnCellMouseOut()");
            //setTimeout('HidePictureWithDelay(\'' + e.item.name + '\')', 300);
            //e.Cell.Attributes.Add("onclick", "event.cancelBubble = true");
            ASPxGridView g = sender as ASPxGridView;
            e.Cell.Attributes["onclick"] = string.Format("OnCellClick({0}, {1}, {2}, {3}, {4}, event);",
            g.ClientInstanceName, e.VisibleIndex, e.KeyValue, e.GetValue("ID"), e.DataColumn.VisibleIndex);
        }
    }
 
    protected void gridData_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            if (string.Format("{0}", hfRole["Role"]) == "0" || hfRole["Role"].ToString() == "9")
            {
            //e.Items.Add(e.CreateItem("Copied", "Copied"));
            //e.Items.Add(e.CreateItem("Revised", "Revised"));
            var item = e.CreateItem("Revised", "Revised");
            item.Image.Url = @"~/Content/Images/Edit.gif";
            e.Items.Add(item);

            item = e.CreateItem("Export", "ExportToXLS");
            item.BeginGroup = true;
            item.Image.Url = @"~/Content/Images/if_sign-out_59204.png";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), item);
            //AddMenuSubItem(item, "PDF", "ExportToPDF", "~/Content/Images/pdf.gif", true);
            //AddMenuSubItem(item, "XLS", "ExportToXLS", "~/Content/Images/excel.gif", true);
            //e.Items.Add(e.CreateItem("Custom item for all rows", "AllRows"));
            //e.Items.Add(e.CreateItem("Custom item for selected rows only", "OnlySelectedRows"));
            //e.Items.Add(e.CreateItem("Custom item for selected rows with 'Discontinued=true' only", "OnlySelectedAndDiscontinuedRows"));

            GridViewContextMenuItem test = e.CreateItem("Copied", "Copied");
            test.Image.Url = @"~/Content/Images/Copy.png";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), test);

            GridViewContextMenuItem assign = e.CreateItem("Assign", "Assign");
            assign.Image.Url = @"~/Content/Images/icons8-plus-16.png";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), assign);

            GridViewContextMenuItem product = e.CreateItem("Product", "Product");
            product.Image.IconID = @"navigation_up_16x16";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), product);
            }
            else if(hfRole["Role"].ToString() == "5" || hfRole["Role"].ToString() == "1"|| hfRole["Role"].ToString() == "0")
            {
                var item = e.CreateItem("Attach Spec", "Attach Spec");
                item.Image.Url = @"~/Content/Images/icons8-attach-16.png";
                e.Items.Add(item);

                GridViewContextMenuItem formu = e.CreateItem("Upload Formula", "Upload Formula");
                formu.Image.Url = @"~/Content/Images/icons8-attach-16.png";
                e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), formu);

                GridViewContextMenuItem assign = e.CreateItem("Split TRF", "Split_TRF");
                assign.Image.Url = @"~/Content/Images/Copy.png";
                e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), assign);

                item = e.CreateItem("Export", "ExportToXLS");
                item.BeginGroup = true;
                item.Image.Url = @"~/Content/Images/if_sign-out_59204.png";
                e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), item);
            }           
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

    protected void CmbReason_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        if (string.IsNullOrEmpty(e.Parameter))return; 
        var args = e.Parameter.Split('|');
        switch (args[0])
        {
            case "3":
                DataView dvsqllist = (DataView)dsReason.Select(DataSourceSelectArguments.Empty);
                if (dvsqllist != null)
                {
                    CmbReason.DataSource = dvsqllist.Table;
                    CmbReason.DataBind();
                }
                break;
        }
    }
    protected void CmbPetCategory_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        var args = e.Parameter.Split('|');
        if (args[0] == "Add" || args[0] == "load")
        {
            SqlDataSource1.SelectCommand = @"SELECT [ID], [Name], usertype,isnull(Receiver,'')Receiver FROM MasPetCategory WHERE dbo.fnc_checktype(usertype,@UserType)>0 
            AND @factory in (select value from dbo.FNC_SPLIT(factory,';')) ORDER BY Name";
            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("factory", TypeCode.String, args[1].ToString());
            SqlDataSource1.SelectParameters.Add("UserType", TypeCode.String, usertp["usertype"].ToString());
            combobox.DataSource = SqlDataSource1;
            combobox.DataBind();
            if (args[2] != "")
                combobox.SelectedIndex = combobox.Items.IndexOf(combobox.Items.FindByValue(args[2]));
        }
    }
    protected void CmbReceiver_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        var args = e.Parameter.Split('|');
        if (args[0] == "Add" || args[0] == "load")
        {
            var Results = new DataTable();
            SqlParameter[] param = { new SqlParameter("@Plant", args[1]),
                new SqlParameter("@Category",string.Format("{0}",args[2]))};
            Results = GetRelatedResources("spGetReceiver", param);
            combobox.DataSource = Results;
            combobox.DataBind();
            if (args[3] != "") {
                combobox.SelectedIndex = combobox.Items.IndexOf(combobox.Items.FindByValue(hfReceiver["Receiver"]));
            }
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
        int itemCount = 0;
        try
        {
            itemCount = (int)gridData.GetTotalSummaryValue(gridData.TotalSummary["RequestNo"]);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        gridData.SettingsPager.Summary.Text = "Page {0} of {1} (" + itemCount.ToString() + " items)";
    }
    object GetDataSource(int count)
    {
        List<object> result = new List<object>();
        for (int i = 0; i < count; i++)
            result.Add(new { ID = i, City = "City_" + i });
        return result;
    }
	public string DisplayEventlog(ASPxGridViewCustomDataCallbackEventArgs e)
    {
        var args = e.Parameters.Split('|');
        int id;
        var text = string.Format(NotFoundFormat, args[2]);
        if (int.TryParse(args[2], out id))
        {
            StringBuilder sb = new StringBuilder();
            string strSQL = @"select requester+''+isnull('|'+assignee,'')assignee,RequestNo,RIGHT(CONCAT('00', Revised), 2)'Revised'"+
                " ,b.Title  from TransTechnical a left join MasStatusApp b on b.Id = a.StatusApp where a.Id='" + id +"' and b.levelApp in (0,2)";
            DataTable dt = cs.builditems(strSQL);
            foreach(DataRow dr in dt.Rows)
            {
            var arr = dr["assignee"].ToString().Split('|');string s="";
            sb.Append(string.Format("StatusApp :{0}, Revised :{1}</b>", 
                 dr["Title"].ToString(),dr["Revised"].ToString()));
            for (int i = 0; i < arr.Length; i++) {
                    if(!string.IsNullOrEmpty(arr[i]))
                    s += cs.GetData(arr[i],"fn")+","; 
                }
                sb.Append(string.Format("<div><b><u>Request By : {0}</u></b></div>",s));}
            var Results = new DataTable();//spGetHistory
            SqlParameter[] param = { new SqlParameter("@Id", id),
                new SqlParameter("@tablename",string.Format("{0}","TransTechnical")),
                new SqlParameter("@user",string.Format("{0}",hfuser["user_name"]))};
            Results = GetRelatedResources("spGetHistory", param);

		    if (Results.Rows.Count > 0)
            {

            foreach (DataRow dr in Results.Rows)
            {
                //string[] printer = { "5", "6", "7" };
                //if (string.Format("{0}",dr["StatusApp"])=="3")
                var status = dr["StatusApp"].ToString();
                var Title = string.Format("{0}", (new[] { "5", "6", "7" }).Contains(status) ? biuldAssignee(dr["Reason"].ToString().Replace('|',',')) : dr["Reason"]);
                sb.Append(string.Format(PreviewFormat, cs.GetData(dr["Username"].ToString(),"fn"), dr["Title"], Title, dr["CreateOn"], dr["Remark"]));
                sb.Append("<br/>");
            }
            //var message = DemoModel.DataProvider.Messages.FirstOrDefault(m => m.ID == id);
            //if (message != null)
            //{
            //    DemoModel.DataProvider.MarkMessagesAs(true, new int[] { id });
            //    var subject = message.Subject;
            //    if (message.IsReply)
            //        subject = "Re: " + subject;
            //    text = string.Format(PreviewFormat, subject, message.From, message.To, message.Date, message.Text);
            //}
			            }else
                sb.Append("<br/>");
            text= sb.ToString();
        }
        //litText.Text = text;
        return text;
    }
    string biuldAssignee(string assignee)
    {
        var arr = assignee.ToString().Split(','); string s = "";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < arr.Length; i++)
        {
            if (!string.IsNullOrEmpty(arr[i]))
                s += cs.GetData(arr[i], "fn") + ",";
        }
        sb.Append(s);
        return sb.ToString();
    }
    protected void fileManager_CustomCallback(object sender, CallbackEventArgsBase e)
    {
        ASPxFileManager fm = (ASPxFileManager)sender;
        string[] param = e.Parameter.Split('|'); //bool selected = true;
        //int id;
        //if (!int.TryParse(param[1], out id))
        //    return;
        switch (param[0]) {
            case "load":
                //ArtsDataSource.SelectParameters.Clear();
                //.SelectParameters.Add("GCRecord", string.Format("{0}", hfgetvalue["NewID"]));
                //ArtsDataSource.SelectParameters.Add("username", hfuser["user_name"].ToString());
                this.Session["GCRecord"] = string.Format("{0}", hfgetvalue["NewID"]);
                break;
        }

		ApplyFileManagerSettings();
        fm.DataBind();
    }
    void ApplyFileManagerSettings()
    {
		FileManagerFolderAccessRule fr = new FileManagerFolderAccessRule();
            fr.Edit = Rights.Allow;
            fr.Role = "Admin";
            fr.Path = "ReadOnly";            
            fr.Browse = Rights.Allow;
            fr.Upload = Rights.Allow;
        //fileManager.SettingsPermissions.AccessRules.Add(fr);
        //fileManager.SettingsEditing.AllowDownload = true;
        //else
        //    fm.SettingsPermissions.AccessRules.Add(new FileManagerFolderAccessRule { Path = "", Edit = Rights.Deny, Browse = Rights.Deny });
    }
    //protected void DeleteData(object sender, EventArgs e)
    //{

    //}
    //protected void DownloadData(object sender, EventArgs e)
    //{

    //}
    //void GenerateDataSource()
    //{
        //if (Session["DataSource"] == null)
        //Session["DataSource"] = DataHelper.CreateDataSource(hfgetvalue["NewID"].ToString(), user_name, "TransTechnical");
        //gvFiles.DataSource = listFile;
        //gvFiles.DataBind();
    //}
   public List<FileSystem> bfile()
    {
        SqlParameter[] param = { new SqlParameter("@GCRecord", string.Format("{0}", hfgetvalue["NewID"])),
        new SqlParameter("@username", string.Format("{0}",  hfuser["user_name"])),
        new SqlParameter("@tablename", "TransTechnical")};
        var Results = GetRelatedResources("spGetFileSystem", param);
      return   (from DataRow dr in Results.Rows
         select new FileSystem()
         {
             ID = Convert.ToInt32(dr["ID"]),
             ParentID = Convert.ToInt32(dr["Result"].ToString()),
             Name = dr["Name"].ToString(),
             //Notes = dr["Notes"].ToString(),
             //Attached = dr["Attached"].ToString(),
             //SubID = dr["SubID"].ToString(),
             //RequestNo = dr["RequestNo"].ToString()
         }).ToList();
    }
    protected void OnItemDeleting(object source, FileManagerItemDeleteEventArgs e)

		{
			if (e.Item is FileManagerFolder)
			{
				e.Cancel = true;
				e.ErrorText = "Folder's deleting is denied";
			}else if (e.Item is FileManagerFile)
			{
				var edit = approv["approv"];
				if (edit.ToString() == "0")
				{
                cs._del(e.Item.Id.ToString());
                    //using (SqlConnection con = new SqlConnection(strConn))
                    //{
                    //	SqlCommand cmd = new SqlCommand();
                    //	cmd.CommandType = CommandType.StoredProcedure;
                    //	cmd.CommandText = "spDeleteFileSystem";
                    //	cmd.Parameters.AddWithValue("@ID", e.Item.Id.ToString());
                    //	cmd.Connection = con;
                    //	con.Open();
                    //	cmd.ExecuteNonQuery();
                    //	con.Close();
                    //}
            }
				else
				{
					e.ErrorText = "File's deleting is denied";
				}
	 e.Cancel = true;
			}
		}
    void namelist(string Folio)
    {
        

        string strSQL = string.Format(@"select *,'' as Mark from 
            TransProductList where RequestNo='{0}'",
            Folio.ToString());
        var Tablelist = cs.builditems(strSQL);
        Tablelist.PrimaryKey = new DataColumn[] { Tablelist.Columns["Id"] };
        listItems = (from DataRow dr in Tablelist.Rows
                     select new TransProductList()
                     {
                         Id = Convert.ToInt32(dr["Id"]),
                         Name = dr["Name"].ToString(),
                         NetWeight = dr["NetWeight"].ToString(),
                         DW = dr["DW"].ToString(),
                         DWType = dr["DWType"].ToString(),
                         RDCode = dr["RDCode"].ToString(),
                         PackSize = dr["PackSize"].ToString(),
                         PW = dr["PW"].ToString(),
                         FixedFillWeight = dr["FixedFillWeight"].ToString(),
                         SaltContent = dr["SaltContent"].ToString(),
                         TargetPrice = dr["TargetPrice"].ToString(),
                         Efficiency = dr["Efficiency"].ToString(),
                         Yield = dr["Yield"].ToString(),
                         Mark = dr["Mark"].ToString(),
                         //Country = dr["Country"].ToString(),
                         //Destination = dr["Destination"].ToString(),
                         RequestNo = dr["RequestNo"].ToString()
                     }).ToList();
        foreach (var t in listItems)
        {
            DataTable dt = GetFormula(t.RequestNo,t.Id.ToString());
            t.formula = (from DataRow dr in dt.Rows
                         select new TransCusFormula()
                         {
                             ID = Convert.ToInt32(dr["ID"]),
                             Component = dr["Component"].ToString(),
                             SubType = dr["SubType"].ToString(),
                             Description = dr["Description"].ToString(),
                             Material = dr["Material"].ToString(),
                             Result = dr["Result"].ToString(),
                             Yield = dr["Yield"].ToString(),
                             RawMaterial = dr["RawMaterial"].ToString(),
                             Name = dr["Name"].ToString(),
                             PriceOfUnit = dr["PriceOfUnit"].ToString(),
                             AdjustPrice = dr["AdjustPrice"].ToString(),
                             Currency = dr["Currency"].ToString(),
                             Unit = dr["Unit"].ToString(),
                             ExchangeRate = dr["ExchangeRate"].ToString(),
                             BaseUnit = dr["BaseUnit"].ToString(),
                             PriceOfCarton = dr["PriceOfCarton"].ToString(),
                             Formula = Convert.ToInt32( dr["Formula"].ToString()),
                             IsActive = dr["IsActive"].ToString(),
                             Remark = dr["Remark"].ToString(),
                             LBOh = dr["LBOh"].ToString(),
                             LBRate = dr["LBRate"].ToString(),
                             RequestNo = dr["RequestNo"].ToString()
                         }).ToList();
            if (t.formula.Count > 0)
                t.Mark = "X";
        }
    }
    public DataTable GetFormula(string Id,string formula)
    {
        SqlParameter[] param = {
                            new SqlParameter("@Id", Id.ToString()),
                            new SqlParameter("@formula", formula.ToString())};
        var dt = GetRelatedResources("spGetCusFormula", param);
        dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
        return dt;
    }
    protected void gv_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.KeyFieldName = "Id";
        g.DataSource = listItems;
        g.ForceDataRowType(typeof(DataRow));
    }
    //private DataTable ReCreateColumns
    //{
    //    get
    //    {
    //        ASPxGridView g = gv;
    //        g.Columns.Clear();
    //        string textarray = "Id;RowID; Name; NetWeight; DW; FixedFillWeight; PW; TargetPrice; SaltContent; Efficiency";
    //        string[] args = textarray.ToString().Split(';');
    //        for (int x = 0; x < args.Length; x++)
    //        {
    //            GridViewDataTextColumn column = new GridViewDataTextColumn();
    //            column.FieldName = args[x];
    //            g.Columns.Add(column);
    //        }

    //        return Tablelist;
    //    }
    //}
    //protected void gv_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
    //{
    //    ASPxGridView g = (ASPxGridView)sender;
    //    DataRow row = Tablelist.NewRow();
    //    int NextRowID = Tablelist.Rows.Count;
    //    NextRowID++;
    //    row["Id"] = 9000 + NextRowID;
    //    var values = new[] { "Id", "RowID", "RequestNo" };
    //    foreach (DataColumn column in Tablelist.Columns)
    //    {
    //        if (!values.Any(column.ColumnName.Contains))
    //        {
    //            row[column.ColumnName] = e.NewValues[column.ColumnName];
    //        }
    //    }
    //    Tablelist.Rows.Add(row);
    //    e.Cancel = true;
    //    g.CancelEdit();
    //}

    //protected void gv_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
    //    //DataTable dt = (DataTable)g.DataSource;//_dataTable;
    //    if (Tablelist != null)
    //    {
    //        var values = new[] { "Id", "RowID", "RequestNo" };
    //        DataRow dr = Tablelist.Rows.Find(e.Keys[0]);
    //        foreach (DataColumn column in Tablelist.Columns)
    //        {
    //            if (!values.Any(column.ColumnName.Contains))
    //            {
    //                dr[column.ColumnName] = e.NewValues[column.ColumnName];
    //            }
    //        }
    //    }
    //    g.CancelEdit();
    //    e.Cancel = true;
    //}
    void loadItems(string Id)
    {
        this.Session.Remove("sessionlist");
        listItems = new List<TransProductList>();
        namelist(Id);
    }
    protected void gv_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string[] param = e.Parameters.Split('|'); //bool selected = true;
        switch (param[0])
        {
            case "reload":
                //this.Session.Remove("sessionlist");
                //listItems = new List<TransProductList>();
                //namelist(param[1]);
                loadItems(param[1]);
                gvformu.DataBind();
                break;
			case "SaveMail":
                break;
            case "Remove":
            case "Delete":
 
                    var list = new List<dynamic>();
                    List<object> fieldValues = g.GetSelectedFieldValues(new string[] { "Id" });
                    if (fieldValues.Count != 0)
                    {
                        foreach (object item in fieldValues)
                        {
                            list.Add(item);
                            var found = listItems.FirstOrDefault(x => x.Id == Convert.ToInt32(item));
                            if (found != null)
                            {
                            found.Mark = "D";
                            }
                        }
                    }
                    string joinRowID = string.Join(",", list);
                break;
        }
        g.DataBind();
    }

    protected void gv_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (g.VisibleRowCount > 5)
            g.Settings.VerticalScrollableHeight = 320;
        else
            g.Settings.VerticalScrollableHeight = g.VisibleRowCount * 23;
        if (!usertp["usertype"].ToString().Contains("0")){ 
			string str = usertp["usertype"].ToString();
			if (str.Contains('1')){
				g.Columns[1].Width = Unit.Pixel(0);
			}
			else { 
			g.Columns[1].Width = Unit.Pixel(0);
			g.Columns[5].Caption = "Unit";
			g.Columns[5].VisibleIndex = 3;
			g.Columns[6].Caption = "F/W (g)";
			g.Columns[7].Width = Unit.Pixel(0);
			g.Columns[9].Caption = "Volume";
			if (hfRole["Role"].ToString() == "6" || hfRole["Role"].ToString() == "2"){
					g.Columns[10].Width = Unit.Percentage(10);
				g.Columns[11].Width = Unit.Percentage(10);
				}
			}
		}
    }
    void book_InvalidFormatException(object sender, SpreadsheetInvalidFormatExceptionEventArgs e)
    {

    }
    void exporter_CellValueConversionError(object sender, CellValueConversionErrorEventArgs e)
    {
        e.Action = DataTableExporterAction.Continue;
        e.DataTableValue = null;
    }
    protected void UpCode_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
    {
        //string dirVirtualPath = Server.MapPath("~/XlsTables/");
        //string dirVirtualPath = @"\\192.168.1.193\XlsTables";
        string dirVirtualPath = @"C:\\temp";
        string dirPhysicalPath = dirVirtualPath;
        if (!Directory.Exists(dirPhysicalPath))
        {
            Directory.CreateDirectory(dirPhysicalPath);
        }
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
            Worksheet worksheet = spreadsheet.Document.Worksheets.ActiveWorksheet;

            int NextRowID = 0; int i = 0;
            NextRowID= cs.FindMaxValue(listItems, x => x.Id);
            foreach (Worksheet sheet in spreadsheet.Document.Worksheets)
            {
                i++;
                CellRange range = sheet.GetUsedRange();
                DataTable table = sheet.CreateDataTable(range, false);
                DataTableExporter exporter = sheet.CreateDataTableExporter(range, table, false);
                exporter.CellValueConversionError += exporter_CellValueConversionError;
                exporter.Export();
                foreach (DataRow dr in table.Rows)
                {
                    if (hfStatusApp["StatusApp"].ToString() == "0")
                    {
                        double num;
                        string value = dr["Column2"].ToString().Trim();
                        if (double.TryParse(dr["Column1"].ToString(), out num))
                            if (!string.IsNullOrEmpty(value))
                            {
                                NextRowID++;
                                TransProductList array = new TransProductList();
                                //array[0] = num;
                                array.Id = NextRowID;
                                array.Name = string.Format( dr["Column2"].ToString());
                                array.NetWeight = string.Format(dr["Column3"].ToString());
                                array.DW = string.Format("{0}", dr["Column4"]);
                                array.DWType = string.Format("{0}", dr["Column5"]);
                                array.PW = string.Format("{0}", dr["Column6"]);
                                array.FixedFillWeight = "0";
                                array.SaltContent = "0";
                                array.TargetPrice = "0";
                                array.Efficiency = "0";
                                    listItems.Add(array);
                            }

                    }
                    else
                    {
                        List<Object> selectItems = gv.GetSelectedFieldValues("ID");
                    }
                }
            }
        }
    }
    protected void CmbAssignee_Init(object sender, EventArgs e)
    {
        ASPxGridLookup lookup = (ASPxGridLookup)sender;
        ASPxGridView gridView = lookup.GridView;
        gridView.CustomCallback += new ASPxGridViewCustomCallbackEventHandler(lookup_CustomCallback);
    }

    void lookup_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView lookup = (ASPxGridView)sender;
        var NewID = hfgetvalue["NewID"].ToString();
        string parameter = e.Parameters;
        var args = e.Parameters.Split('|');
        if (args[0] == "load" || args[0]== "Assignee") { 
        lookup.DataBind();
		}
    }

    //protected void gv_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
    //{
    //    if (Tablelist != null) { 
    //    int NextRowID = Tablelist.Rows.Count;
    //    NextRowID++;
    //    e.NewValues["RowID"] = NextRowID;
    //    }
    //}
    protected void gvzone_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        if (e.ButtonID != "Remove") return;
        object keyValue = g.GetRowValues(e.VisibleIndex, g.KeyFieldName);
        var found = listDestination.FirstOrDefault(x => x.Id == Convert.ToInt32(keyValue));
        //t.Rows.Remove(found);
        found.Mark = found.Mark.ToString() == "D" ? "" : "D";
        listDestination.Remove(found);
        deleteDestination(keyValue);
        //Tablelist.AcceptChanges();
        g.DataBind();
    }
    protected void gv_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        if (e.ButtonID != "Edit") return;
        object keyValue = g.GetRowValues(e.VisibleIndex, g.KeyFieldName);
        var found = listItems.FirstOrDefault(x => x.Id == Convert.ToInt32(keyValue));
        //t.Rows.Remove(found);
        found.Mark = found.Mark.ToString() == "D" ? "" : "D";
        //Tablelist.AcceptChanges();
        g.DataBind();
    }
    protected void gv_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //bool isOddRow = e.VisibleIndex % 2 == 0;
        string Result = string.Format("{0}", g.GetRowValues(e.VisibleIndex, "Mark"));
        bool isOddRow = string.Format("{0}", Result) != "" && !string.IsNullOrEmpty(Result);
        if (e.ButtonID == "Edit" && isOddRow)
        {
            e.Image.Url = "~/Content/Images/Refresh.png";
            e.Image.ToolTip = "Return";
        }
    }
    //const string UploadDirectory = "~/Content/UploadControl/";

    protected void CmbCustomerPrice_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        FillCityCombo(dsCustomerPrice, combobox);
    }
   protected void cpUpdateRole_Callback(object sender, CallbackEventArgsBase e)
    {
        string[] param = e.Parameter.Split('|');
        switch (param[0])
        {
            case "reload":
                cbIngredient.DataBind();
                //var arr = param[1].ToString().Split(';');
                //for (int i = 0; i < arr.Length; i++)
                //    //int value = arr[i];
                //    if (arr[i] != "")
                //        cbIngredient.SelectedIndex = Convert.ToInt32(arr[i]);
                //tbIngredientOther.Text = param[2].ToString();
                break;
            case "build":
                cbIngredient.DataBind();
                if(param[1].ToString()!="")
                for (int i = 0; i < cbIngredient.Items.Count; i++) {
                    if (hfIngredient["Ingredient"].ToString().Contains(cbIngredient.Items[i].Value.ToString()))
                        cbIngredient.Items[i].Selected = true;
                }
                break;
        }
    }

    protected void dsComponent_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
    {
        var a = e.Command.Parameters;
    }
    void gridView_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        var NewID = hfgetvalue["NewID"].ToString();
        string[] param = e.Parameters.Split('|'); //bool selected = true;
        //switch (param[0])
        //{
        //    case "Add":
        //        string strSQL = @"select Company from MasPlant where Code='{0}' and usertype='{1}'";
        //        string Bu = cs.ReadItems(string.Format(strSQL, cmbCompany.Value, usertp["usertype"]));
        //        build_assign(Bu);
        //        break;
        //}
        g.DataBind();
    }
    protected void CmbAssign_Init(object sender, EventArgs e)
    {
        ASPxGridLookup lookup = (ASPxGridLookup)sender;
        ASPxGridView gridView = lookup.GridView;
        gridView.CustomCallback += new ASPxGridViewCustomCallbackEventHandler(gridView_CustomCallback);
    }

	string conditionTemplate;
    protected void combo_ItemRequestedByValue(object source, ListEditItemRequestedByValueEventArgs e)
    {
        string value = e.Value == null ? String.Empty : e.Value.ToString();
        if (e.Value != null)
        {
            conditionTemplate = "AND [ID] = {0}";
        }
        BindComboBox(source, value, 0, 1);
    }
    protected void combo_ItemsRequestedByFilterCondition(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
    {
        if (!String.IsNullOrEmpty(e.Filter))
        {
            conditionTemplate = "AND [Name] like '{0}%'";
        }
        BindComboBox(source, e.Filter, e.BeginIndex, e.EndIndex);
    }
    private void BindComboBox(object source, string value, int beginIndex, int endIndex)
    {
        ASPxComboBox cb = source as ASPxComboBox;
        cb.DataSource = GetDataSet(value, beginIndex, endIndex);
        cb.DataBind();
    }
    private IEnumerable GetDataSet(string value, int beginIndex, int endIndex)
    {
        const string commandTemplate =
            "SELECT * FROM (" +
            "    SELECT ROW_NUMBER() OVER (ORDER BY [ID]) AS rownumber, [ID], [Name], [usertype] " +
            "    FROM [MasPetCategory] {0} {1}" +
            ") AS foo " +
            "WHERE rownumber >= {2} AND rownumber <= {3}";

        string command;
        string utype = string.Format("WHERE dbo.fnc_checktype(usertype,'{0}')>0 {1}", usertp["usertype"], 
            string.Format(cmbCompany.Value == null ? "" : "AND {0} in (select value from dbo.FNC_SPLIT(factory,';'))", cmbCompany.Value));
        bool valueIsEmpty = String.IsNullOrEmpty(value);
        if (valueIsEmpty)
            command = String.Format(commandTemplate, utype, String.Empty, beginIndex, endIndex);
        else
            command = String.Format(commandTemplate, utype, String.Format(conditionTemplate, value), beginIndex, endIndex);

        SqlDataSource ds = new SqlDataSource(strConn, command);
        IEnumerable dataSet = ds.Select(DataSourceSelectArguments.Empty);
        //if (!valueIsEmpty) {
        //    Regex r = new Regex("^" + value, RegexOptions.IgnoreCase);
        //    foreach (DataRowView item in dataSet) {
        //        item.Row["ProductName"] = r.Replace(item.Row["ProductName"].ToString(), AddColor);
        //    }
        //}
        return dataSet;
    }
    private string AddColor(Match m)
    {
        return String.Format("<span style='background-color:Orange'>{0}</span>", m.Value);
    }
    protected void listBox_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxListBox lbox = (ASPxListBox)sender;
        lbox.DataBind();
    }

    protected void CmbPetFoodType_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        combobox.DataBind();
    }

    protected void CmbCompliedWith_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        combobox.DataBind();
    }

   
    void creatediv()
    {

    }

 
    protected void cpCustomerRole_Callback(object sender, CallbackEventArgsBase e)
    {
        string[] arg = e.Parameter.Split('|');
            switch (arg[0])
        {
            case "reload":
                //cbCustomer.DataSource = dt;
                cbCustomer.DataBind();
                break;
            case "build":
                //cbCustomer.DataSource = dt;
                cbCustomer.DataBind();
                //string[] arg = arg[1].Split(';'); 
				if (arg[1].ToString() != ""){
					for (int i = 0; i < cbCustomer.Items.Count; i++){
						if(hfCustomerRe["CustomerRe"].ToString().Contains(cbCustomer.Items[i].Value.ToString()))
						cbCustomer.Items[i].Selected = true;
					}
				}
                break;
        }
    }
    protected void cpPrdRequirement_Callback(object sender, CallbackEventArgsBase e)
    {
        string[] param = e.Parameter.Split('|');
        switch (param[0])
        {
            case "reload":
                cbPrdRequirement.DataBind();
                break;
            case "build":
                cbPrdRequirement.DataBind();
				if (param[1].ToString() != "") { 
					for (int i = 0; i < cbPrdRequirement.Items.Count; i++)
					{
						if (hfPrdRequirement["PrdRequirement"].ToString().Equals(cbPrdRequirement.Items[i].Value.ToString()))
							cbPrdRequirement.Items[i].Selected = true;
					}
				}
                break;
        }
    }
    protected void cmbCompany_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        string[] param = e.Parameter.Split('|');
        switch (param[0])
        {
            case "reload":

                break;
        }
    }
   protected void OnFileDownloading(object source, FileManagerFileDownloadingEventArgs e)
    {
        ASPxFileManager file = source as ASPxFileManager;
        if (e.File.Extension.ToLower().Contains("pdf") || e.File.Extension.ToLower().Contains("xls") || e.File.Extension.ToLower().Contains("png"))
        {
            foreach (var items in file.SelectedFiles) {
                getdownload(items.Id,MimeTypes.GetContentType(e.File.FullName));
            }
            //e.InputStream;
        }
        e.Cancel = true;
    }
    void getdownload(string Id, string contentType)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "SELECT * FROM FileSystem2 where ID=@Id";
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Connection = con;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
					char[] charsToTrim = { '*', ' ', '\'',',' };
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
        }
    }
   protected void CmbSellingUnit_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        var args = e.Parameter.Split('|');
        if (args[0] == "reload") { 
            combobox.DataBind();
            if (args[1] != "") {
                combobox.SelectedIndex = combobox.Items.IndexOf(combobox.Items.FindByText(hfSellingUnit["SellingUnit"].ToString()));
            }
        }
    }
     protected void Upformula_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
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
            Worksheet sheet = spreadsheet.Document.Worksheets.ActiveWorksheet;
            CellRange range = sheet.GetUsedRange();
            DataTable table = sheet.CreateDataTable(range, false);
            DataTableExporter exporter = sheet.CreateDataTableExporter(range, table, false);
            exporter.CellValueConversionError += exporter_CellValueConversionError;
            exporter.Export();
          
        }
    }
    protected void ordersGridView_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
    {

    }
    protected void callbackPanel_Callback(object source, DevExpress.Web.CallbackEventArgsBase e) { 
    }
    protected void gv_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
    {
        var values = new[] { "Id", "RowID", "RequestNo" };
        foreach (var args in e.InsertValues)
        {
            int NextRowID = cs.FindMaxValue(listItems, t => t.Id);
            TransProductList row = new TransProductList();
            NextRowID++;
            row.Id = NextRowID;
            LoadNewValues(row, args.NewValues);
            listItems.Add(row);
        }
        foreach (var args in e.UpdateValues)
        {
            var rcus = listItems.FirstOrDefault(x => x.Id == Convert.ToInt32(args.Keys["Id"]));
            LoadNewValues(rcus, args.NewValues);
        }
        e.Handled = true;
    }
    protected void LoadNewValues(TransProductList item, OrderedDictionary values)
    {
        item.Name = Convert.ToString(values["Name"]);
        item.NetWeight = Convert.ToString(values["NetWeight"]);
        item.DW = Convert.ToString(values["DW"]);
        item.DWType = Convert.ToString(values["DWType"]);
        item.PW = Convert.ToString(values["PW"]);
        item.FixedFillWeight = Convert.ToString(values["FixedFillWeight"]);
        item.SaltContent = Convert.ToString(values["SaltContent"]);
        item.TargetPrice = Convert.ToString(values["TargetPrice"]);
        item.Efficiency = Convert.ToString(values["Efficiency"]);
        item.Yield = Convert.ToString(values["Yield"]);
        item.RDCode = Convert.ToString(values["RDCode"]);
        item.PackSize = Convert.ToString(values["PackSize"]);
        item.Mark = Convert.ToString(values["Mark"]);
        item.CanSize = Convert.ToString(values["CanSize"]);
        //item.Country = Convert.ToString(values["Country"]);
        //item.Destination = Convert.ToString(values["Destination"]);
        if (item.formula == null)
            item.formula = new List<TransCusFormula>();
        item.RequestNo = Convert.ToString(values["RequestNo"]);
    }
    public class CustomDataColumnTemplate : ITemplate
    {
        private ASPxGridView _grid;
        public void InstantiateIn(Control container)
        {
            GridViewDataItemTemplateContainer c = (GridViewDataItemTemplateContainer)container;
            int key = Convert.ToInt32(c.KeyValue);
            ASPxButton btn = new ASPxButton();
            btn.ID = "button_" + c.VisibleIndex;
            c.Controls.Add(btn);
            btn.Text = "Button";

            _grid = c.Grid;
            btn.Click += ASPxButton1_Click;
        }

        protected void ASPxButton1_Click(object sender, EventArgs e)
        {
            ASPxButton s = sender as ASPxButton;
            string id = s.ID;
            int ind = Int32.Parse(id.Split('_')[1]);
            string st = _grid.GetRowValues(ind, "ProductID").ToString();
        }
    }


	protected void treeList_DataBound(object sender, EventArgs e)
    {
        ASPxTreeList tree = sender as ASPxTreeList;
        if (!Page.IsCallback && !IsPostBack)
        {
            tree.ExpandAll();
        }
        SetNodeSelectionSettings(tree);
    }

    void SetNodeSelectionSettings(ASPxTreeList treeList)
    {
        TreeListNodeIterator iterator = treeList.CreateNodeIterator();
        TreeListNode node;
        while (true)
        {
            node = iterator.GetNext();
            if (node == null) break;
            node.AllowSelect = !node.HasChildren;
        }
    }
   protected void treeList_CustomJSProperties(object sender, TreeListCustomJSPropertiesEventArgs e)
    {
        ASPxTreeList tree = sender as ASPxTreeList;
        Hashtable employeeNames = new Hashtable();
        foreach (TreeListNode node in tree.GetVisibleNodes())
            employeeNames.Add(node.Key, node["Name"]);
        e.Properties["cpEmployeeNames"] = employeeNames;

    }

    protected void treeList_CustomDataCallback(object sender, TreeListCustomDataCallbackEventArgs e)
    {
        ASPxTreeList tree = sender as ASPxTreeList;
        Hashtable Names = new Hashtable();
        //var nodes = tree.GetSelectedNodes();
        List<string> list = new List<string>();
        foreach (TreeListNode node in tree.GetSelectedNodes()) {
            //Names.Add(node.Key, node["Name"]);
            list.Add(node["Name"].ToString());
        }
        e.Result = String.Join(",", list.ToArray()); 
    }

    protected void treeList_CustomCallback(object sender, TreeListCustomCallbackEventArgs e)
    {
        string[] parameter = e.Argument.Split(';');
        ASPxTreeList tree = sender as ASPxTreeList;
 
        TreeListNodeIterator iterator = tree.CreateNodeIterator();
        TreeListNode node;
        while (true)
        {
            node = iterator.GetNext();
            if (node == null) break;
            int a = Array.IndexOf(parameter, node.Key.ToString());
            if (Array.IndexOf(parameter, node.Key.ToString()) ==-1)
                node.Selected = false;
            else
                node.Selected = true;
        }
    }
    protected void grid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {

    }
    protected void cpUpdateFiles_Callback(object sender, CallbackEventArgsBase e)
    {
        string[] param = e.Parameter.Split('|');
        switch (param[0])
        {
            //case "load":
            //    fileManager.Settings.RootFolder = @"Folder\";
            //    fileManager.DataBind();
            //    break;
            case "reload":
                GenerateDataSource(Guid.Parse(param[1].ToString()));
                break;
        }
    }

    protected void gvformu_DataBinding(object sender, EventArgs e)
    {
        (sender as ASPxGridView).DataSource = LoadGrid;

    }
    protected void cbPage_Init(object sender, EventArgs e)
    {
        ASPxComboBox cb = sender as ASPxComboBox;
        var listprd = listItems.Where(x => x.Mark != "").ToList();
        foreach (var i in listprd)
            cb.Items.Add(i.Name.ToString(), i.Id);
    }
    private List<TransCusFormula> LoadGrid
    {
        get
        {
            List<TransCusFormula> f = new List<TransCusFormula>();
            var index = TabList.ActiveTabIndex;
            List<TransCusFormula> listcus= new List<TransCusFormula>();
            var listprd = listItems.Where(x => x.Mark != "").ToList();
            if (listprd.Count == 0) return f;

                ActivePageSymbol = listprd.Where(x =>x.Id.ToString()== ActivePageSymbol).ToList().Count==0 ? listprd[0].Id.ToString() : ActivePageSymbol;
                int id = Convert.ToInt32(ActivePageSymbol);// Convert.ToInt32(cmbformu.Value == null ? 0 : cmbformu.Value);
                var t = listItems.FirstOrDefault(x => x.Id == id);
                if (listprd != null)
                {
                    int max = cs.FindMaxValue(listprd, x => x.Id);
                    //gvformu.Templates.PagerBar = new MyPagerBarTemplate(listprd[0].Id.ToString(), max);
                    //gvformu.Templates.PagerBar = new combobox(ActivePageSymbol, max);

                    gvformu.AutoFilterByColumn(gvformu.Columns["Formula"], ActivePageSymbol);
                    gvformu.SettingsBehavior.AllowSelectByRowClick = false;
                    gvformu.SettingsDataSecurity.AllowEdit = true;
                }
            
            if (t == null) {

                return f;
            } else
                return t.formula;
        }
    }
    protected void gvformu_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string[] param = e.Parameters.Split('|');
        if (param[0] == "Insert")
        {
            int id = Convert.ToInt32(ActivePageSymbol);
            var t = listItems.FirstOrDefault(x => x.Id == id);
            TransCusFormula f3 = new TransCusFormula();
            List<TransCusFormula> list3 = t.formula;
            int iMax = cs.FindMaxValue(list3, x => x.ID);
            iMax++;
            f3.ID = iMax;
            string strSQL = "";
            if (param[2].ToString() == "Packaging")
            {
                strSQL = string.Format(@"select top 1 material from MasPricePolicy where ID='{0}'", param[1].ToString());
                string Material = cs.ReadItems(strSQL);
                string value = string.Format("{0}", tbRequestNo.Value.ToString().Substring(0, 3) + "|" + Material + "|" + hfGeID["GeID"]);
                DataTable dttable = GetrequestRate(value);
                foreach(DataRow _r in dttable.Rows)
                {
                    f3.Component = "3";
                    f3.SubType = param[2].ToString();
                    f3.Material = _r["Material"].ToString();
                    f3.Name = _r["Description"].ToString();
                    f3.Result = Convert.ToDecimal("1").ToString("F4");
                    f3.Yield = "100";
                    f3.Unit = string.Format("{0}", _r["Unit"]);
                    f3.PriceOfUnit = string.Format("{0}",  _r["PriceOfUnit"]);
                    f3.AdjustPrice = string.Format("{0}", f3.PriceOfUnit);
                    f3.PriceOfCarton = string.Format("{0:n}", Convert.ToDecimal(f3.PriceOfUnit) * Convert.ToDecimal(f3.Result) * Convert.ToDecimal(t.PackSize));
                    f3.Formula = id;
                    list3.Add(f3);
                }
            }
            else
            {
                if (param[2].ToString() == "Semi")
                    strSQL = string.Format(@"select top 1 * from MasDL where ID='{0}'", param[1].ToString());
                if (param[2].ToString() == "BCDL")
                    strSQL = string.Format(@"select top 1 * from MasLaborOverhead where ID='{0}'", param[1].ToString());
                DataTable dt = cs.builditems(strSQL);
                foreach (DataRow dr in dt.Rows)
                {
                    f3.Component = "5";
                    f3.SubType = param[2].ToString();
                    f3.Material = dr["LBCode"].ToString();
                    f3.Name = dr["LBName"].ToString();
                    f3.Result = "1";
                    f3.PriceOfUnit = dr["LBRate"].ToString();
                    if (param[2].ToString() == "BCDL" && !string.IsNullOrEmpty(t.NetWeight))
                    {
                        decimal _NetWeight = Convert.ToDecimal(t.NetWeight);
                        decimal _PackSize = Convert.ToDecimal(t.PackSize);
                        f3.PriceOfCarton = Convert.ToDecimal(_NetWeight * (Convert.ToDecimal(dr["LBRate"].ToString()) / 1000) * _PackSize).ToString();
                    }
                    else if (param[2].ToString() == "Semi")
                    {
                        if (Clientusertp["ut"].ToString().Contains("5") || Clientusertp["ut"].ToString().Contains("6") || Clientusertp["ut"].ToString().Contains("7") || Clientusertp["ut"].ToString().Contains("8"))
                            f3.PriceOfCarton = Convert.ToDecimal(Convert.ToDecimal(t.NetWeight) * (Convert.ToDecimal(dr["LBRate"].ToString()) / 1000) * Convert.ToDecimal(t.PackSize)).ToString();
                        else
                            f3.PriceOfCarton = Convert.ToDecimal(Convert.ToDecimal(t.FixedFillWeight) * (Convert.ToDecimal(dr["LBRate"].ToString()) / 1000) *
                                Convert.ToDecimal(t.PackSize)).ToString();
                    }
                    f3.Formula = id;
                    list3.Add(f3);
                }
            }
            t.formula = list3;
        }
        if (param[0] == "reload")
        {
            if (param.Length > 2)
                loadItems(param[2].ToString());
            //ASPxComboBox cmb = g.FindPagerBarTemplateControl("cbPage", GridViewPagerBarPosition.Top) as ASPxComboBox;
            //cbPage_Init(this);
            //cmb.DataBind();
            double usertype = 0;
            if (double.TryParse(param[1].ToString(), out usertype))
            {
                if (usertype >= 5) { 
                    gvformu.Toolbars[0].Items[0].Text = "DL Frozen";
                    gvformu.Toolbars[0].Items[1].Visible = false;
                }
            }
        }
        if (param[0] == "del")
        {
            int key = Convert.ToInt32(ActivePageSymbol);
            var rw = listItems.FirstOrDefault(x => x.Id == Convert.ToInt32(key));
            var f = rw.formula;
            foreach(var _rx in f){
                if(_rx.ID == Convert.ToInt32(param[1])) { 
                    f.Remove(_rx); break;
                }
            }
            rw.formula = f;
        }
        if (param[0] == "symbol")
        {
            ActivePageSymbol = param[1].ToString();
        }
        if (listItems.Count >0)
        {
            var listprd = listItems.Where(x => x.Mark != "").ToList();
            int max = cs.FindMaxValue(listprd, x => x.Id);
            if(listprd.Count>0)
            ActivePageSymbol = listprd.Where(x => x.Id.ToString() == ActivePageSymbol).ToList().Count == 0 ? listprd[0].Id.ToString() : ActivePageSymbol;
            //g.Templates.PagerBar = new MyPagerBarTemplate(ActivePageSymbol, max);
        }
        g.DataBind();
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
    protected void gvformu_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        var listprd = listItems.Where(x => x.Mark != "").ToList();
        if (listprd.Count > 0)
        {
            int max = cs.FindMaxValue(listprd, x => x.Id);
            ActivePageSymbol = listprd.Where(x => x.Id.ToString() == ActivePageSymbol).ToList().Count == 0 ? listprd[0].Id.ToString() : ActivePageSymbol;
            g.FilterExpression = "[Formula] = '" + ActivePageSymbol + "'";
            g.JSProperties["cpPageIndex"] = ActivePageSymbol;
        }
        if (Clientusertp["ut"].ToString().Contains("5")){
            gvformu.Toolbars[0].Items[0].Text = "DL Frozen";
            gvformu.Toolbars[0].Items[1].Visible = false;
        }
        else{
            gvformu.Toolbars[0].Items[0].Text = "Semi";
            gvformu.Toolbars[0].Items[1].Visible = true;
        }
    }
    string GetBU(string CategoryID)
    {
        return cs.ReadItems("select isnull(Receiver,'')c from MasPetCategory where ID ='"+ CategoryID + "'");
    }
    protected void Upload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
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
        string bu = GetBU(CmbPetCategory.Value.ToString());
        e.UploadedFile.SaveAs(FilePath);
        if (!string.IsNullOrEmpty(FilePath))
        {
            //Workbook book = new Workbook();
            //book.InvalidFormatException += book_InvalidFormatException;
            //book.LoadDocument(FilePath);
            ASPxSpreadsheet spreadsheet = new ASPxSpreadsheet();
            spreadsheet.Document.LoadDocument(FilePath);
            spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;

            //int i = book.Worksheets.Count;
            //for (int w = 0; w < i; w++)
            foreach (Worksheet sheet in spreadsheet.Document.Worksheets)
            {
                //Worksheet sheet = book.Worksheets[w];
                CellRange range = sheet.GetUsedRange();
                DataTable table = sheet.CreateDataTable(range, false);
                DataTableExporter exporter = sheet.CreateDataTableExporter(range, table, false);
                exporter.CellValueConversionError += exporter_CellValueConversionError;
                exporter.Export();
                string item = "";
                if (bu.StartsWith("102"))
                {
                    if (sheet.Cells["E5"].Value.ToString() != "")
                    {
                        item = sheet.Cells["E5"].Value.ToString();
                    }
                    if (item.ToString() != "")
                    {

                        var t = listItems.FirstOrDefault(x => x.Name == item);
                        if (t != null)
                        {

                            t.RDCode = table.Rows[2]["Column13"].ToString();
                            t.CanSize = table.Rows[5]["Column7"].ToString();
                            t.PackSize = t.PackSize.ToString(); ;
                            List<TransCusFormula> listexcel = new List<TransCusFormula>();
                            string Component = "";
                            int irow = 0;
                            foreach (DataRow r in table.Rows)
                            {
                                irow++;
                                if (string.IsNullOrEmpty(r["Column1"].ToString()))
                                    Component = r["Column4"].ToString();
                                if (IsNumeric(r["Column1"].ToString()))
                                {
                                    TransCusFormula excel = new TransCusFormula();
                                    int NextRowID = cs.FindMaxValue(listexcel, x => x.ID);
                                    NextRowID++;
                                    excel.ID = NextRowID;
                                    excel.Material = string.Format("{0}", r["Column3"].ToString());
                                    excel.Name = string.Format("{0}", r["Column4"].ToString());
                                    if (excel.Name.ToString().ToLower().Equals("water"))
                                        excel.Yield = string.Format("{0}", 100);
                                    else
                                        excel.Yield = string.Format("{0}", r["Column2"].ToString().Replace("%", ""));
                                    excel.Formula = t.Id;
                                    decimal resutl = 0;
                                    //decimal.TryParse(r["Column8"].ToString(), out resutl);
                                    decimal.TryParse(sheet.Cells["H"+irow].Value.ToString(), out resutl);
                                    excel.Result = string.Format("{0}", resutl.ToString());
                                    excel.AdjustPrice = string.Format("{0}", 0);
                                    excel.PriceOfUnit = string.Format("{0}", 0);
                                    excel.Diff = string.Format("{0}", 0);
                                    if (excel.Material != "")
                                    {
                                        string value = string.Format("{0}", tbRequestNo.Value.ToString().Substring(0, 3) + "|" + excel.Material + "|" + hfGeID["GeID"]);
                                        DataTable data = GetrequestRate(value);
                                        foreach (DataRow dr in data.Rows)
                                        {
                                            decimal PriceOfUnit = 0, pack = 0;
                                            decimal.TryParse(dr["PriceOfUnit"].ToString(), out PriceOfUnit);
                                            decimal.TryParse(t.PackSize.ToString(), out pack);
                                            excel.PriceOfUnit = string.Format("{0}", PriceOfUnit);
                                            excel.AdjustPrice = string.Format("{0}", PriceOfUnit.ToString("F4"));
                                            excel.Unit = string.Format("{0}", dr["Unit"]);
                                            excel.PriceOfCarton = "0";
                                            if (excel.Yield != "")
                                                excel.PriceOfCarton = string.Format("{0}", resutl / (Convert.ToDecimal(excel.Yield) / 100) * PriceOfUnit * pack);
                                            //excel.PriceOfCarton = Convert.ToDouble(excel.PriceOfCarton).ToString("F4");
                                            if(excel.Material.Substring(0, 1) == "5"){
                                                excel.PriceOfCarton = String.Format("{0:n}",  Convert.ToDouble(excel.PriceOfCarton));
                                            }else
                                            excel.PriceOfCarton = String.Format("{0:n}", Convert.ToDouble(Convert.ToDouble(excel.PriceOfCarton) / 1000).ToString("F2"));
                                        }

                                        string count = cs.ReadItems(@"select CAST(count(*) as nvarchar)c from MasPricePolicy where Material = '"
                                            + r["Column3"].ToString() + "'");
                                        if (count != "0")
                                        {
                                            string a = excel.Material.Substring(0, 1);
                                            switch (a.ToString())
                                            {
                                                case "1":
                                                    excel.Component = "1";
                                                    break;
                                                case "4":
                                                    excel.Component = "2";
                                                    break;
                                                case "5":
                                                    if(Component.ToLower().Contains("primary packaging"))
                                                    excel.Component = "3";
                                                    else if(Component.ToLower().Contains("secondary packaging"))
                                                    excel.Component = "4";
                                                    break;

                                            }
                                        }
                                        else
                                        {
                                            excel.Material = string.Format("{0}", "");
                                        }

                                    }
                                    listexcel.Add(excel);
                                }
                            }
                            t.formula = listexcel;
                            t.Mark = "X";
                        }
                    }
                } else 
                {
                    if (sheet.Cells["D5"].Value.ToString() != "")
                    {
                        item = sheet.Cells["D5"].Value.ToString();
                    }
                    if (item.ToString() != "")
                    {
                        var t = listItems.FirstOrDefault(x => x.Name == item);
                        if (t != null)
                        {
                            t.PackSize = t.PackSize.ToString();
                            t.formula = GetTableFromExcel(table, t.Id,t);
                            t.Mark = "X";
                        }
                    }
                }  
            }
            string[] arr = { "Complete" };
            e.CallbackData = string.Join("|", arr);
        }
    }
    public List<TransCusFormula> GetTableFromExcel(DataTable table,int Id,  TransProductList t)
    {
        List<TransCusFormula> listexcel = new List<TransCusFormula>();
        string Component = "";
        foreach (DataRow r in  table.Rows)
        {
            if (string.IsNullOrEmpty(r["Column1"].ToString()))
                Component = r["Column4"].ToString();
            if (IsNumeric(r["Column1"].ToString()))
            {
                TransCusFormula excel = new TransCusFormula();
                int NextRowID = cs.FindMaxValue(listexcel, x => x.ID);
                NextRowID++;
                excel.ID = NextRowID;
                excel.Yield = string.Format("{0}", r["Column2"].ToString().Replace("%",""));
                excel.Material = string.Format("{0}", r["Column3"].ToString());
                excel.Name = string.Format("{0}", r["Column4"].ToString());
                excel.Formula = Id;

                double resutl = 0;
                double.TryParse(r["Column8"].ToString(), out resutl);
                excel.Result = string.Format("{0}", resutl.ToString("F4"));
                //excel.AdjustPrice = string.Format("{0}", AdjustPrice.ToString("F4"));
                excel.AdjustPrice = string.Format("{0}", 0);
                excel.PriceOfUnit = string.Format("{0}", 0);
                excel.Diff = string.Format("{0}", 0);
                
                excel.Component = string.Format("{0}", "");
                if (excel.Material != "")
                {
                    string count = cs.ReadItems(@"select CAST(count(*) as nvarchar)c 
                        from MasPricePolicy where Material = '" + r["Column3"].ToString() + "'");
                    if (count != "0")
                    {
                        string a = excel.Material.Substring(0, 1);
                        switch (a.ToString())
                        {
                            case "1":
                                excel.Component = "1";
                                break;
                            case "4":
                                excel.Component = "2";
                                break;
                            case "5":
                                if (Component.ToLower().Contains("primary packaging"))
                                    excel.Component = "3";
                                else if (Component.ToLower().Contains("secondary packaging"))
                                    excel.Component = "4";
                                break;

                        }
                    }else
                        excel.Material = string.Format("{0}", "");
                    string value = string.Format("{0}", tbRequestNo.Value.ToString().Substring(0, 3) + "|" + excel.Material + "|" + hfGeID["GeID"]);
                    DataTable data = GetrequestRate(value);
                    foreach (DataRow dr in data.Rows)
                    {
                        double PriceOfUnit = 0,pack =0;
                        double.TryParse(dr["PriceOfUnit"].ToString(), out PriceOfUnit);
                        double.TryParse(t.PackSize.ToString(), out pack);
                        excel.PriceOfUnit = string.Format("{0}", PriceOfUnit);
                        excel.AdjustPrice = string.Format("{0}", PriceOfUnit.ToString("F4"));
                        excel.Unit = string.Format("{0}", dr["Unit"]);
                        excel.PriceOfCarton = "0";
                        if (excel.Yield !="")
                        excel.PriceOfCarton = string.Format("{0}", resutl/(Convert.ToDouble(excel.Yield)/100) * PriceOfUnit * pack);
                        var row = listexcel.FirstOrDefault();
                        //excel.PriceOfCarton = Convert.ToDouble(excel.PriceOfCarton).ToString("F4");
                        if (r["Column3"].ToString().ToLower().Equals("co product") || r["Column3"].ToString().ToLower().Equals("by product")){
                            excel.PriceOfCarton = ((Convert.ToDouble(excel.Yield) * Convert.ToDouble(excel.PriceOfUnit)) * 
                                (Convert.ToDouble(row.Result) / 1000 * pack / Convert.ToDouble(row.Yield))).ToString();
                        }
                        //else if (r["Column3"].ToString().ToLower().Equals("by product"))
                        //{
                        //    excel.PriceOfCarton = ((Convert.ToDouble(excel.Yield) * Convert.ToDouble(excel.PriceOfUnit)) *
                        //        (Convert.ToDouble(t.FixedFillWeight) / 1000 * pack / Convert.ToDouble(_yield))).ToString();
                        //}
                        else if(excel.Material.Substring(0, 1) == "5"){
                            excel.PriceOfCarton = String.Format("{0:n}", Convert.ToDouble(excel.PriceOfCarton));
                        }
                        else
                            excel.PriceOfCarton = String.Format("{0:n}", Convert.ToDouble(Convert.ToDouble(excel.PriceOfCarton) / 1000).ToString("F2"));
                    }


                }
                listexcel.Add(excel);
            }
        }
        return listexcel;
    }
    public static bool IsNumeric(object Expression)
    {
        double retNum;

        bool isNum = Double.TryParse(Convert.ToString(Expression), 
            System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        return isNum;
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
    protected object GetGroupSummaryValue(GridViewGroupFooterCellTemplateContainer container)
    {
        var summaryItem = gvformu.GroupSummary.First(i => i.Tag == "GroupPriceOfUnit");
        return gvformu.GetGroupSummaryValue(container.VisibleIndex, summaryItem);
    }
    protected object GetTotalSummaryValue()
    {
        var summaryItem = gvformu.TotalSummary.First(i => i.Tag == "TotalPriceOfUnit");
        return gvformu.GetTotalSummaryValue(summaryItem);
    }
    //protected void cmbformu_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    ASPxComboBox combobox = (ASPxComboBox)sender;
    //    combobox.DataSource = listItems;
    //    combobox.DataBind();
    //    //combobox.SelectedIndex = 0;
    //}
 
    protected void gvformu_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //ASPxComboBox cb = g.FindPagerBarTemplateControl("cbPage", GridViewPagerBarPosition.Top) as ASPxComboBox;
        foreach (var args in e.InsertValues)
        {

        }
        foreach (var args in e.UpdateValues)
        {
            int id = Convert.ToInt32(args.NewValues["Formula"]);
            //string[] valueType = Regex.Split("Material;From;To;Commission;OverPrice;OverType;Pacifical;MSC;Margin;SubContainers", ";");
            var dr = listItems.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
            var drx = dr.formula.FirstOrDefault(x => x.ID == Convert.ToInt32(args.Keys["ID"]));
            LoadNewValues(drx, args.NewValues,dr.PackSize);
        }
        foreach (var args in e.DeleteValues)
        {
            int keyid = Convert.ToInt32(args.Keys["ID"]);
            var found = listItems.FirstOrDefault(x => x.Id == Convert.ToInt32(keyid));
            listItems.Remove(found);
        }
        e.Handled = true;
    }
    protected void LoadNewValues(TransCusFormula item, OrderedDictionary values, string Packaging)
    {
        //item.ID = Convert.ToInt32(values["ID"]);
        double pack = 0;
        double.TryParse(Packaging, out pack);
        item.Material = Convert.ToString(values["Material"]);
        item.Description = Convert.ToString(values["Description"] == null ?
           GetDescrip(item.Material) : values["Description"]);
        item.Result = values["Result"].ToString();
        item.Yield = values["Yield"].ToString();
        item.Component = values["Component"].ToString();
        item.AdjustPrice = values["AdjustPrice"].ToString();
        item.Remark = string.Format("{0}", values["Remark"]);
        item.Diff = "0";
        item.Unit = string.Format("{0}", values["Unit"]).ToString();
   
        if (!string.IsNullOrEmpty(item.AdjustPrice))
        {
            double PriceOfUnit = 0;
            if (double.TryParse(item.PriceOfUnit, out PriceOfUnit)) { 
            //if (!string.IsNullOrEmpty(item.PriceOfUnit)) {
                if (PriceOfUnit != 0)
                item.Diff = string.Format("{0}", PriceOfUnit - Convert.ToDouble(item.AdjustPrice));
            }
            if (item.Yield != "")
                item.PriceOfCarton = string.Format("{0}",
                    Convert.ToDouble(item.Result) / (Convert.ToDouble(item.Yield) / 100) * Convert.ToDouble(item.AdjustPrice) * pack);
            if (item.Component.Equals("3"))
                item.PriceOfCarton = string.Format("{0}",
                    Convert.ToDouble(item.Result) * Convert.ToDouble(item.AdjustPrice) * pack);
            if (item.Component.Equals("2") || item.Component.Equals("1"))
                if (!string.IsNullOrEmpty(item.PriceOfCarton))
                    item.PriceOfCarton = String.Format("{0:n}", Convert.ToDouble(Convert.ToDouble(item.PriceOfCarton) / 1000).ToString("F2"));
        }
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
    //protected void Page_Init(object sender, EventArgs e)
    //{
    //    GenerateDataSource();
    //}
    void GenerateDataSource(Guid gcrecord)
    {
        //if (!string.IsNullOrEmpty(hfgetvalue["NewID"].ToString())) {
        //    var gcrecord = string.Format("{0}", hfgetvalue["NewID"]);
            Session["DataSource"] = DataHelper.CreateDataSource(gcrecord.ToString(),
            user_name, "TransTechnical");
            fileManager.DataBind();
        //}
    }
    //public List<FileSystemData> CreateDataSource()
    //{
    //    List<FileSystemData> list = new List<FileSystemData>();
    //    var gcrecord = string.Format("{0}", "41D68E1D-8E4F-4985-8C01-05FF3C42B6B6");
    //    var user = string.Format("{0}", "fo5910155");
    //    SqlParameter[] param = { new SqlParameter("@GCRecord",string.Format("{0}", gcrecord)),
    //    new SqlParameter("@username", string.Format("{0}", user)),
    //    new SqlParameter("@tablename", string.Format("{0}", "TransTechnical"))};
    //    var _dt = GetRelatedResources("spGetFileSystem", param);
 
    //    _dt.PrimaryKey = new DataColumn[] { _dt.Columns["ID"] };
    //    FileSystemData item = new FileSystemData();

    //    list = (from DataRow dr in _dt.Rows
    //            select new FileSystemData()
    //            {
    //                Id = Convert.ToInt32(dr["ID"]),
    //                ParentId = Convert.ToInt32(dr["ParentId"]),
    //                Name = string.Format("{0}", dr["Name"]).ToString(),
    //                IsFolder = Convert.ToBoolean(dr["IsFolder"].ToString()),
    //                //Data = (byte[])dr["Data"],
    //                LastWriteTime = Convert.ToDateTime(dr["LastWriteTime"].ToString())
    //            }).ToList();
    //    return list;
    //}
    //protected void Page_Init(object sender, EventArgs e)
    //{
    //    fileManager.CustomFileSystemProvider = CreateCustomFileSystemProvider();
    //}
    protected void OnFileUploading2(object source, FileManagerFileUploadEventArgs e)
    {
        var leavefilepath = e.File.Folder.ToString();
        string dirVirtualPath = @"C:\\temp";
            string dirPath = e.File.Folder.FullName;
            string dirPhysicalPath = dirVirtualPath;// Server.MapPath(dirVirtualPath);
            if (!Directory.Exists(dirPhysicalPath))
            {
                Directory.CreateDirectory(dirPhysicalPath);
            }
            ASPxUploadControl FileUpload1 = new ASPxUploadControl();
            string fileName = e.FileName;
            //String TempFileName;
            HttpFileCollection MyFileCollection = Request.Files;
            FilePath = Path.Combine(dirPath, fileName);
            //FilePath = string.Format(Server.MapPath("~/XlsTables/{0}") , e.UploadedFile.FileName);

            if (!string.IsNullOrEmpty(FilePath))
            {
            }
            //string test = hffileManager["overwrite"].ToString();
            //ProviderOptions.AllowOverwrite = Convert.ToBoolean(hffileManager["overwrite"]);
    }
    protected void OnFileUploading(object source, FileManagerFileUploadEventArgs e)
    {
        var leaveNo = hfgetvalue["NewID"].ToString();
        var newleavepath = @"~\memberpages\Leave\documents\" + leaveNo;
        if (!Directory.Exists(Server.MapPath(newleavepath)))
        {
            Directory.CreateDirectory(Server.MapPath(newleavepath));
        }
        var newDocPath = Server.MapPath(newleavepath) + @"\" + e.File.Name;
         try
        {
            FileStream fs = new FileStream(newDocPath, FileMode.CreateNew);
            //List<FileSystemData> listdoc = new List<FileSystemData>();
            //listdoc.Add(FileToByteArray(FilePath));
            e.InputStream.CopyTo(fs);
            fs.Close(); //close the new file created by the FileStream
            e.Cancel = true; //cancelling the upload, prevents duplicate uploads
            e.ErrorText = "Success"; //shown when the upload is cancelled
                                     //fmLeaveDocs.Refresh(); //does not work. Causes an error.
        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString());
        };
    }
    public FileSystemData FileToByteArray(string fileName)
    {
        byte[] fileContent = null;
        System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
        long byteLength = new System.IO.FileInfo(fileName).Length;
        fileContent = binaryReader.ReadBytes((Int32)byteLength);
        fs.Close();
        fs.Dispose();
        binaryReader.Close();
        FileSystemData d = new FileSystemData();
        d.Name = fileName;
        d.Data = fileContent;
        return d;
    }
    private FileSystemProviderBase CreateCustomFileSystemProvider()
    {
        ProviderOptions = new MyCustomProviderOptions();
        MyCustomFileSystemProvider provider = new MyCustomFileSystemProvider(fileManager.Settings.RootFolder, ProviderOptions);

        return provider;
    }

    protected void OnFolderCreating(object source, FileManagerFolderCreateEventArgs e)
    {

    }

    protected void OnDataBinding(object sender, EventArgs e)
    {

    }

    protected void gvformu_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        var args = e.Parameters.Split('|');
        var result = new Dictionary<string, string>();
        DataTable dt = new DataTable();
        string strSQL = "";
        long id;
        if (!long.TryParse(args[2], out id))
            return;
        if (args[0] == "Semi")
        {
            result["view"] = args[1].ToString();
            strSQL = string.Format(@"select top 1 * from MasDL where ID='{0}'", args[1]);
            dt = cs.builditems(strSQL);
            foreach (DataRow dr in dt.Rows){
                result["LBCode"] = dr["LBCode"].ToString();
                result["Rate"] = dr["LBRate"].ToString();
            }
            e.Result = result;
        }
        if (args[0] == "BCDL")
        {
            result["view"] = args[1].ToString();
            strSQL = string.Format(@"select top 1 * from MasLaborOverhead where ID='{0}'", args[1]);
            dt = cs.builditems(strSQL);
            foreach (DataRow dr in dt.Rows){
                result["LBCode"] = dr["LBCode"].ToString();
                result["Rate"] = dr["LBRate"].ToString();
            }
            e.Result = result;
        }
        if (args[0] == "SelMaterial")
        {
            result["view"] = args[1].ToString();
            strSQL = string.Format(@"select top 1 material from MasPricePolicy where ID='{0}'", args[1]);
            string Material = cs.ReadItems(strSQL);
            string value = string.Format("{0}", tbRequestNo.Value.ToString().Substring(0, 3) + "|" + Material + "|" + hfGeID["GeID"]);
            dt = GetrequestRate(value);
            foreach (DataRow dr in dt.Rows)
            {
                result["Material"] = dr["Material"].ToString();
                result["Name"] = dr["Description"].ToString();
                result["Price"] = dr["PriceOfUnit"].ToString();
            }
            e.Result = result;
        }
    }

    protected void cbPage_Callback(object sender, CallbackEventArgsBase e)
    {
        if (string.IsNullOrEmpty(e.Parameter)) return;
        ASPxComboBox cb = sender as ASPxComboBox;
        var listprd = listItems.Where(x => x.Mark != "").ToList();
        //foreach (var i in listItems)
        //    cb.Items.Add(i.Name.ToString(), i.Id);
    }

    protected void fileManager_FileUploading(object source, FileManagerFileUploadEventArgs e)
    {
        if (e.File.Extension.ToLower().Contains("pdf") || e.File.Extension.ToLower().Contains("xls") || e.File.Extension.ToLower().Contains("png")|| Path.GetExtension(e.FileName) == ".jpg")
        {
            string a = e.FileName;
            string FileToCopy = e.File.Location;
            if (! string.IsNullOrEmpty(tbRequestNo.Text)){
                e.OutputStream = SavePostedFile(e.InputStream, e.File.Name);
            } else {
                var leaveNo = hfgetvalue["NewID"].ToString();
                var newleavepath = @"~\memberpages\Leave\documents\" + leaveNo;
                if (!Directory.Exists(Server.MapPath(newleavepath)))
                {
                    Directory.CreateDirectory(Server.MapPath(newleavepath));
                }
                var newDocPath = Server.MapPath(newleavepath) + @"\" + e.File.Name;
                FileStream fs = new FileStream(newDocPath, FileMode.CreateNew);
                e.InputStream.CopyTo(fs);
                   
                fs.Close(); //close the new file created by the FileStream
                e.Cancel = true; //cancelling the upload, prevents duplicate uploads
                                 //e.ErrorText = "Success"; //shown when the upload is cancelled
                                 //fmLeaveDocs.Refresh(); //does not work. Causes an error.
            }
        }
        //if (Path.GetExtension(e.FileName) == ".jpg")
        //{
        //    e.OutputStream = SavePostedFile(e.InputStream, e.FileName);
        //}
    }
    protected Stream SavePostedFile(Stream stream, string name)
    {
        //var leaveNo = hfgetvalue["NewID"].ToString();
        //var newleavepath = @"~\memberpages\Leave\documents\" + leaveNo;
        //if (!Directory.Exists(Server.MapPath(newleavepath)))
        //{
        //    Directory.CreateDirectory(Server.MapPath(newleavepath));
        //}
        //string resultFilePath = Server.MapPath(newleavepath) + @"\" + name;
        byte[] FileData = cs.ReadToEnd(stream);
            //savefile(name.ToString(), FileData);
            //var image = PhotoUtils.GetReducedImage(Image.FromStream(stream), 200, 150);
            //PhotoUtils.DrawWatermarkText(image, "25 Photos");
            MemoryStream ms = new MemoryStream();
        //image.Save(ms, ImageFormat.Jpeg);
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

    protected void gvzone_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        foreach (var args in e.InsertValues)
        {
            int NextRowID = cs.FindMaxValue(listDestination, t => t.Id);
            TransDestination row = new TransDestination();
            NextRowID++;
            row.Id = NextRowID;
            row.Country = Convert.ToString(args.NewValues["Country"]);
            
            listDestination.Add(row);
        }
        foreach (var args in e.UpdateValues)
        {
            var dr = listDestination.FirstOrDefault(x => x.Id == Convert.ToInt32(args.Keys["Id"]));
            dr.Zone = string.Format("{0}", args.NewValues["Zone"]);
            dr.Country = string.Format("{0}",  args.NewValues["Country"]);
 
        }
        foreach (var args in e.DeleteValues)
        {
            int keyid = Convert.ToInt32(args.Keys["Id"]);
            var found = listDestination.FirstOrDefault(x => x.Id == Convert.ToInt32(keyid));
            found.Mark = "D";
            deleteDestination(keyid);
            listDestination.Remove(found);
        }
        e.Handled = true;
    }
    void deleteDestination(object keyValue)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.CommandText = "DELETE TransDestination WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", keyValue.ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    protected void gvzone_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.KeyFieldName = "Id";
        g.DataSource = listDestination;
        g.ForceDataRowType(typeof(DataRow));
    }

    protected void gvzone_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string[] param = e.Parameters.Split('|'); //bool selected = true;
        switch (param[0])
        {
            case "reload":

                string strSQL = string.Format(@"select *,''Mark from TransDestination where RequestNo='{0}'",
                param[1].ToString());
                var TableDes = cs.builditems(strSQL);
                TableDes.PrimaryKey = new DataColumn[] { TableDes.Columns["Id"] };
                listDestination = (from DataRow dr in TableDes.Rows
                                   select new TransDestination()
                                   {
                                       Id = Convert.ToInt32(dr["Id"]),
                                       Mark = dr["Mark"].ToString(),
                                       Country = dr["Country"].ToString(),
                                       Zone = dr["Zone"].ToString(),
                                       RequestNo = dr["RequestNo"].ToString()
                                   }).ToList();
                break;
        }
        g.DataBind();
    }

    protected void gvstyle_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.KeyFieldName = "Id";
        g.DataSource = listStyle;
        g.ForceDataRowType(typeof(DataRow));
        //AddCommandColumn();
    }
    
    protected void gvstyle_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string[] param = e.Parameters.Split('|'); //bool selected = true;
        long id;
        if (!long.TryParse(param[1], out id))
            return;
        switch (param[0])
        {
            case "New":
                // add detail
                var obj = listStyle.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
                Detail u = new Detail();
                int iMax = cs.FindMaxValue(obj.Users, x => x.Id);
                iMax++;
                u.Id = iMax;
                u.parentID = Convert.ToInt32(id);
                obj.Users.Add(u);
                break;
            case "reload":

                string strSQL = string.Format(@"select *,''Mark from TransPackingStyle where RequestNo='{0}'",
                param[1].ToString());
                var TableDes = cs.builditems(strSQL);
                TableDes.PrimaryKey = new DataColumn[] { TableDes.Columns["Id"] };
                listStyle = (from DataRow dr in TableDes.Rows
                                   select new TransStyle()
                                   {
                                       Id = Convert.ToInt32(dr["Id"]),
                                       Mark = dr["Mark"].ToString(),
                                       Packing = dr["Packing"].ToString(),
                                       OD = dr["OD"].ToString(),
                                       PackagingType = dr["PackagingType"].ToString(),
                                       RequestNo = dr["RequestNo"].ToString()
                                   }).ToList();
                break;
        }
        g.DataBind();
    }
    protected void detail_BeforePerformDataSelect(object sender, EventArgs e)
    {
        //ASPxGridView g = sender as ASPxGridView;
        //int currentUserID = (int)g.GetMasterRowKeyValue();

        //g.DataSource = listStyle.Find(u => u.Id == currentUserID).User;
    }

    protected void gvstyle_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        foreach (var args in e.InsertValues)
        {
            int NextRowID = cs.FindMaxValue(listStyle, t => t.Id);
            TransStyle row = new TransStyle();
            NextRowID++;
            row.Id = NextRowID;
            row.RequestNo = Convert.ToString(args.NewValues["RequestNo"]);

            listStyle.Add(row);
        }
        foreach (var args in e.UpdateValues)
        {
            var dr = listStyle.FirstOrDefault(x => x.Id == Convert.ToInt32(args.Keys["Id"]));
            dr.OD = string.Format("{0}", args.NewValues["OD"]);
            dr.PackagingType = string.Format("{0}", args.NewValues["PackagingType"]);

        }
        foreach (var args in e.DeleteValues)
        {
            int keyid = Convert.ToInt32(args.Keys["Id"]);
            var found = listStyle.FirstOrDefault(x => x.Id == Convert.ToInt32(keyid));
            found.Mark = "D";
            RemoveStyle(keyid);
            listStyle.Remove(found);
        }
        e.Handled = true;
    }
    void RemoveStyle(object keyValue)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.CommandText = "DELETE TransPackingStyle WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", keyValue.ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
        

    protected void ordersGridView_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.KeyFieldName = "Id";
        g.DataSource = listStyle;
        g.ForceDataRowType(typeof(DataRow));
    }
    protected void ordersGridView_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string[] param = e.Parameters.Split('|');
        if (param[0] == "save")
        {
            int NextRowID = cs.FindMaxValue(listStyle, t => t.Id);
            TransStyle row = new TransStyle();
            NextRowID++;
            row.Id = NextRowID;
            row.Packing = PackingTextBox.Text;
            row.OD = ODTextBox.Text;
            row.StylePackage= string.Format("{0}", PackingStyleComboBox.Value);
            listStyle.Add(row);
        }
        if (param[0] == "reload")
        {

        }
        g.DataBind();
    }

    protected void detail_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
    {
        foreach (var args in e.InsertValues)
        {
            object keyValue = ordersGridView.GetRowValues(ordersGridView.FocusedRowIndex, ordersGridView.KeyFieldName);
            //int currentUserID = (int)ordersGridView.GetMasterRowKeyValue();
            var data = listStyle.Find(x => x.Id == Convert.ToInt32(keyValue));
            int NextRowID = cs.FindMaxValue(data.Users, t => t.Id);
            Detail user = new Detail();
            user.Id = NextRowID;
            user.parentID = data.Id;
            data.Users.Add(user);
        }
        e.Handled = true;
    }

    protected void detail_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string aa = ordersGridView.FocusedRowIndex.ToString();

        object keyValue = ordersGridView.GetRowValues(ordersGridView.FocusedRowIndex, ordersGridView.KeyFieldName);
        //int currentUserID = (int)ordersGridView.GetMasterRowKeyValue(ordersGridView.FocusedRowIndex, ordersGridView.KeyFieldName);
        //g.DataSource = listStyle.Find(u => u.Id == Convert.ToInt32(keyValue)).Users;
        g.DataSource = new List<Detail>();
    }
    protected void PasteToGridView(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Id", typeof(int)),
                    new DataColumn("Name", typeof(string)),
                    new DataColumn("Country",typeof(string)) });

        string copiedContent = Request.Form[txtCopied.UniqueID];
        foreach (string row in copiedContent.Split('\n'))
        {
            if (!string.IsNullOrEmpty(row))
            {
                dt.Rows.Add();
                int i = 0;
                foreach (string cell in row.Split('\t'))
                {
                    dt.Rows[dt.Rows.Count - 1][i] = cell;
                    i++;
                }
            }
        }
        GridView1.DataSource = dt;
        GridView1.DataBind();
        txtCopied.Text = "";
    }
    protected void detail_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
         

        g.DataBind();
    }
}
public class MyCustomFileSystemProvider : PhysicalFileSystemProvider
{
    MyCustomProviderOptions ProviderOptions { get; set; }

    public MyCustomFileSystemProvider(string rootFolder, MyCustomProviderOptions providerOptions)
        : base(rootFolder)
    {
        ProviderOptions = providerOptions;
    }
    public override bool Exists(FileManagerFile file)
    {
        if (ProviderOptions.AllowOverwrite)
            return false;
        return base.Exists(file);
    }
    public override void UploadFile(FileManagerFolder folder, string fileName, System.IO.Stream content)
    {
        base.UploadFile(folder, fileName, content);
        ProviderOptions.AllowOverwrite = false;
    }
}


public class MyCustomProviderOptions
{
    public bool AllowOverwrite { get; set; }
}
class SqlCommandHelper : IEnumerable<DynamicSqlRow>, IDisposable
{
    private SqlConnection connection;
    private SqlCommand command;

    public SqlCommandHelper(string connectionString, CommandType commandType, string commandText, params SqlParameter[] parameters)
    {
        connection = new SqlConnection(connectionString);
        command = new SqlCommand
        {
            CommandText = commandText,
            CommandType = commandType,
            Connection = connection
        };

        command.Parameters.AddRange(parameters);
    }

    public IEnumerator<DynamicSqlRow> GetEnumerator()
    {
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                yield return new DynamicSqlRow(reader);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        command.Dispose();
        connection.Dispose();
    }
}
class DynamicSqlRow : DynamicObject
{
    IDataReader reader;

    public DynamicSqlRow(IDataReader reader)
    {
        this.reader = reader;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        var row = reader[binder.Name];

        result = row is DBNull ? null : row;

        return true;
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
 public class TransCusFormula
{
    public int ID { get; set; }
    public string Component { get; set; }
    public string SubType { get; set; }
    public string Description { get; set; }
    public string Material { get; set; }
    public string Result { get; set; }
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
    //public string Semi { get; set; }
    public string LBOh { get; set; }
    public string LBRate { get; set; }
    //public string LBSemi { get; set; }
    //public string LBSemiRate { get; set; }
    //public string LBTotal { get; set; }
    public string RequestNo { get; set; }
    public string Diff { get; set; }
}
public class TransProductList
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Primary { get; set; }
    public string PrimarySize { get; set; }
    public string NetWeight { get; set; }
    public string DW { get; set; }
    public string DWType { get; set; }
    public string PW { get; set; }
    public string FixedFillWeight { get; set; }
    public string SaltContent { get; set; }
    public string TargetPrice { get; set; }
    public string Efficiency { get; set; }
    public string Yield { get; set; }
    public string RDCode { get; set; }
    public string PackSize { get; set; }
    public string Mark { get; set; }
    public List<TransCusFormula> formula { get; set; }
    public string CanSize { get; set; }
    //public string Destination { get; set; }
    //public string Country { get; set; }
    public string RequestNo { get; set; }
}
public class FileSystem
{
    public DateTime LastWriteTime { get; set; }
    public string Name { get; set; }
    public int ID { get; set; }
    public int ParentID { get; set; }
    public bool IsFolder { get; set; }
    public byte[] Data { get; set; }
    public int OptimisticLockField { get; set; }
    public Guid GCRecord { get; set; }
    public byte[] SSMA_TimeStamp { get; set; }
    public string LastUpdateBy { get; set; }
}
//public class FileSystemData
//{
//    public int? Id { get; set; }
//    public int? ParentId { get; set; }
//    public string Name { get; set; }
//    public bool IsFolder { get; set; }
//    public Byte[] Data { get; set; }
//    public DateTime? LastWriteTime { get; set; }
//}
public class Document
{
    public int DocId { get; set; }
    public string DocName { get; set; }
    public byte[] DocContent { get; set; }
}
public class TransDestination
{
    public int Id { get; set; }
    public string RequestNo { get; set; }
    public string Zone { get; set; }
    public string Country { get; set; }
    public string Mark { get; set; }
}
public class TransStyle
{
    public int Id { get; set; }
    public string RequestNo { get; set; }
    public string StylePackage { get; set; }
    public string CategoriesString { get; set; }
    public string Packing { get; set; }
    public string OD { get; set; }
    public string PackagingType { get; set; }
    public string PackSize { get; set; }
    public string Mark { get; set; }
    public List<Detail> Users { get; set; }

    public bool HasProjects()
    {
        return Users.Count > 0;
    }
}
public class Detail
{
    public Detail()
    {
        Projects = new List<ProjectDetail>();
    }

    //private string m_fullName;
    public string PackingStyle { get; set; }
    public int Id { get; set; }
    public string spec { get; set; }
    public string coating { get; set; }
    public string totalcolor { get; set; }
    public string note { get; set; }
    public int parentID { get; set; }
    //public string FirstName { get; set; }
    //public string LastName { get; set; }
    public List<ProjectDetail> Projects { get; set; }

    //public string FullName
    //{
    //    get { return string.Format("{0}, {1}", this.LastName, this.FirstName); }
    //}

    public bool HasProjects()
    {
        return Projects.Count > 0;
    }
}
public class ProjectDetail
{
    public ProjectDetail()
    {
        Status = ProjectStatus.New;
    }

    public int ID { get; set; }
    public string StepPrice { get; set; }
    public string PriceUnit { get; set; }
    //public string SAPCode { get; set; }
    public int parentID { get; set; }
    public ProjectStatus Status { get; set; }
}
public enum ProjectStatus
{
    New,
    InProgress,
    Failed,
    Done
}
