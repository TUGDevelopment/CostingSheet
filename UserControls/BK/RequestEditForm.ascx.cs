using DevExpress.Spreadsheet;
//using DevExpress.Spreadsheet.Export;
using DevExpress.Web;
//using DevExpress.Web.ASPxTreeList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Web.Caching;
using System.Text.RegularExpressions;
using System.Globalization;
using DevExpress.Web.ASPxSpreadsheet;
using ClosedXML.Excel;
using System.Collections;
using WebApplication;
using System.Text;

public partial class UserControls_RequestEditForm : MasterUserControl
{
    MyDataModule cs = new MyDataModule();
    ServiceCS myservice = new ServiceCS();
    myClass myClass = new myClass();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
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
    protected string SearchText { get { return Utils.GetSearchText(Page); } }
    public List<TransFileDetails> FileDetails
    {
        get
        {
            var obj = this.Session["myList"];
            if (obj == null) { obj = this.Session["myList"] = new List<TransFileDetails>(); }
            return (List<TransFileDetails>)obj;
        }

        set
        {
            this.Session["myList"] = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!this.IsPostBack)
            SetInitialRow();
    }
    public void SetInitRoot()
    {
        Session["DataSource"] = DataHelper.Init();
    }
    void Delete(string Id)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            string query = "DELETE TransRequestForm WHERE ID=@ID";
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("@ID", Id.ToString());
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
    public void SetInitialRow()
    {
        //if (!string.IsNullOrEmpty(hfid["hidden_value"].ToString()))
        hftype["type"] = string.Format("{0}", 0);
        //user_name.Replace("fo5910155", "MO581192");
        username["username"] = user_name;
        editor["Name"] = cs.IsMemberOfRole(string.Format("{0}", 0));
        hfGeID["GeID"] = string.Format("{0}", 0);
        hfCostID["CostID"] = string.Format("{0}", 0);
        hfTrfID["TrfID"] = string.Format("{0}", 0);
        //Update();
    }
    void ApproveStep(string keys)
    {
        if (CmbDisponsition.Value != null)
        {
            DataTable dt = new DataTable();
            SqlParameter[] param = {
                new SqlParameter("@Id", keys.ToString()),
                new SqlParameter("@User", user_name),
                new SqlParameter("@StatusApp", string.Format("{0}", CmbDisponsition.Value)),
                new SqlParameter("@table", string.Format("{0}", "TransRequestForm")),
                new SqlParameter("@remark", mComment.Text),
                new SqlParameter("@assign", string.Format("{0}", CmbAssignee.Value)),
                new SqlParameter("@reason", string.Format("{0}", CmbReason.Text))
            };
            dt = cs.GetRelatedResources("spApproveStep", param);
            foreach (DataRow dr in dt.Rows)
            {
                sendmail(dr, keys);
            }
        }
    }
    void sendmail(DataRow dr, string keys)
    {
        List<string> myList = new List<string>();
        List<string> list = new List<string>();
        List<string> listsender = new List<string>();
        string _text = "";
        string[] split = dr["MailTo"].ToString().Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
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
        split = dr["MailCc"].ToString().Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in split)
        {
            if (s.Trim() != "")
                if (!myList.Contains(s))
                {
                    list.Add(cs.GetData(s, "email"));
                    myList.Add(s);
                }
        }
        list.Add(cs.GetData(user_name.ToString(), "email"));
        _text = string.Format("Dear {0} <br/> ", String.Join(",", listsender.ToArray()));
        //if (!args[6].Contains("1"))
        //list.Add(cs.GetData(user_name, "email"));
        string MailCc = String.Join(",", list.ToArray());

        int i = (CmbDisponsition.Value.ToString() == "7") ? 3 : 2;
        //string statusapp = dr["StatusApp"].ToString();// " was "+ CmbDisponsition.Text;
        string form = "EditDraft";
        //if (dr["RequestNo"].ToString() != "")
        //    form = (dr["RequestNo"].ToString().Substring(3, 1) == "1") ? "Edit" : form;
        //string subject = string.Format(@"{0}:{1} Request No.: " + dr["RequestNo"].ToString() + " V.{2} ,"
        //    +  dr["Subject"], string.Format("{0:00}", dr["Revised"]));
        string subject = string.Format(@"{0}:{1} Request No.: " + dr["RequestNo"].ToString() + " V.{2} ,"
            + CmbMarketingNo.Text + "/", tbRefSamples.Text.ToString(), dr["Subject"], string.Format("{0:00}", dr["Revised"]));
        string _link = "Request No.:" + dr["RequestNo"].ToString();
        _link += "<br/> Request By : " + cs.GetData(dr["Requester"].ToString(), "fn");
        if (!string.IsNullOrEmpty(CmbReason.Text) && CmbDisponsition.Text == "Reject")
            _link += "<br/> Reason :" + CmbReason.Text;
        _link += "<br/> Sender : " + cs.GetData(user_name.ToString(), "fn");
        _link += "<br/> Comment :" + mComment.Text;
        _link += @"<br/> The document link --------><a href=" + cs.GetSettingValue()
            + "/Default.aspx?viewMode=RequestEditForm&ID=" + keys.ToString() + "&form=" + form ;
        _link += " style='color: rgb(0,255,0)'><font color='#0000FF'>click</font></a>";
        cs.sendemail(sender, MailCc, string.Format("{0}{1}", _text,_link), subject);
    }
 
    protected void gridData_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        var args = e.Parameters.Split('|');
        long id;
        if (!long.TryParse(args[2], out id))
            return;
        var result = new Dictionary<string, string>();
        if (args[0] == "GetResult")
        {
            result["Name"] = "";
            StringBuilder sb = new StringBuilder();
            //spGetHierarchy
            SqlParameter[] param = {new SqlParameter("@ProductGroup",string.Format("{0}", cmbProductGroup.Value)),
                new SqlParameter("@RawMaterials",string.Format("{0}",cmbRawMaterial.Value)),
                new SqlParameter("@Division",string.Format("{0}",CmbDivision.Value)),
                new SqlParameter("@Style",string.Format("{0}",cmbStyle.Value)),
                new SqlParameter("@Media",string.Format("{0}",cmbSizeMedia.Value)),
                new SqlParameter("@Customer",string.Format("{0}",cmbZone.Value)),
                new SqlParameter("@Nutrition",string.Format("{0}",CmbNutrition.Value)),
                new SqlParameter("@PetType",string.Format("{0}",CmbPetType.Value)),
                new SqlParameter("@Container",string.Format("{0}",CmbContainerLid.Value))};
            DataTable dt = myClass.GetRelatedResources("spGetHierarchy", param);
            //if (args[1] == "Zone")
            //{
            //    result["Name"] += cmbZone.Value.ToString();
            //}
            foreach (DataRow row in dt.Rows)
            {
                sb.Append(row[0] + " ");
            }
            result["Name"] = string.Join("", sb);
            object[] array = { "2", cmbProductGroup.Value, cmbRawMaterial.Value,
            cmbStyle.Value, cmbTopping.Value, cmbSizeMedia.Value, CmbContainerLid.Value,
            cmbGrade.Value, cmbZone.Value, tbPackingStyle.Text};
            result["result"] = string.Join("", array);
            result["Code"] = string.Join("", new string[] { string.Format("{0}",cmbProductGroup.Value),
                string.Format("{0}",CmbContainerLid.Value), string.Format("{0}",cmbGrade.Value),string.Format("{0}",cmbZone.Value)," ",
                string.Format("{0}",cmbRawMaterial.Value),string.Format("{0}",cmbStyle.Value),string.Format("{0}",cmbTopping.Value),
                string.Format("{0}",cmbSizeMedia.Value)});
            e.Result = result;
        }
        if (args[0] == "cost" || args[0] == "renew")
        {
            SqlParameter[] param = {new SqlParameter("@Id", args[1].Replace("S", "")) };
            DataTable dt = cs.GetRelatedResources("spGetCostnoRequestForm", param);
            foreach (DataRow rw in dt.Rows)
            {

                result["Code"] = string.Format("{0}", rw["TRFRequestNo"]);
                result["TRFID"] = string.Format("{0}", rw["RequestNo"]);

                result["Name"] = string.Format("{0}", rw["CostNo"]);
                result["CostID"] = string.Format("{0}", rw["CostID"]);
                result["RefSamples"] = string.Format("{0}", rw["RefSamples"]);
            }
            e.Result = result;
        }
        if (args[0] == "doc")
        {
            if (args[1].Length == 18)
            {
                var param = args[1];

                result["RawMaterial"] = string.Format("{0}", param.Substring(2, 2));
                result["StyleofPack"] = string.Format("{0}", param.Substring(4, 1));
                result["MediaType"] = string.Format("{0}", param.Substring(5, 3));
                result["NW"] = string.Format("{0}", param.Substring(8, 3));
                result["ContainerLid"] = string.Format("{0}", param.Substring(11, 2));
                result["Grade"] = string.Format("{0}", param.Substring(13, 1));

                result["Zone"] = string.Format("{0}", param.Substring(14, 2));
                var dtgroup = ((DataView)dsProductGroup.Select(DataSourceSelectArguments.Empty)).Table;
                DataRow _r = dtgroup.Select("ProductGroup='" + param.Substring(1, 1)+"'").FirstOrDefault();
                if (_r != null)
                    result["Group"] = string.Format("{0}", _r["Name"]);
                e.Result = result;
            }
            else
            {
                string[] arr = { "RawMaterial", "StyleofPack", "MediaType", "NW", "ContainerLid", "Grade", "Zone" };
                foreach (string i in arr){
                    result[i] = "";
                }
                result["Group"]= string.Format("{0}", args[1]);
                e.Result = result;
            }
            
        }
        if (args[1] == "New")
        {
            //BuildSelect_FileDetails();
            //hfgetvalue["NewID"] = string.Format("{0}", cs.GetNewID());
            result["NewID"] = (string)cs.GetNewID();
            result["editor"] = cs.IsMemberOfRole(string.Format("{0}", 0));
            //result["ExchangeRate"] = string.Format("{0}", exchangerate());
            e.Result = result;
        }
        if (args[1] == "EditDraft" || args[1] == "save")
        {
            SqlParameter[] param = {new SqlParameter("@Id",id.ToString()),
                        new SqlParameter("@username",user_name.ToString())};
            DataTable dt = cs.GetRelatedResources("spSelRequestForm", param);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                result["TRFID"] = dr["RequestNo"].ToString();
                result["CostID"] = dr["Costno"].ToString();
                result["editor"] = dr["editor"].ToString();
                result["UniqueColumn"] = dr["UniqueColumn"].ToString();
                result["StatusApp"] = dr["StatusApp"].ToString();
                result["ID"] = dr["ID"].ToString();
                result["DocumentNo"] = string.Format("{0}", dr["DocumentNo"]);
                result["Costno"] = string.Format("{0}", dr["Costingno"]);
                //result["Nutrition"] = string.Format("{0}", dr["Nutrition"]);
                result["Code"] = string.Format("{0}", dr["Code"]);
                result["ProductGroup"] = string.Format("{0}", dr["ProductGroup"]);
                result["RawMaterial"] = string.Format("{0}", dr["RawMaterial"]);
                result["StyleofPack"] = string.Format("{0}", dr["StyleofPack"]);
                result["MediaType"] = string.Format("{0}", dr["MediaType"]);
                result["Remark"] = string.Format("{0}", dr["Remark"]);
                result["NW"] = string.Format("{0}", dr["NW"]);
                result["ContainerLid"] = string.Format("{0}", dr["ContainerLid"]);
                result["Grade"] = string.Format("{0}", dr["Grade"]);
                result["RequestDate"] = string.Format("{0}", dr["RequestDate"]);
                result["Zone"] = string.Format("{0}", dr["Zone"]);
                result["RequestNo"] = string.Format("{0}", dr["trf_requestno"]);
                result["NewID"] = string.Format("{0}", dr["NewID"]);
                result["Group"] = string.Format("{0}", dr["ProductGroup"]);
                result["Assignee"] = string.Format("{0}", dr["Assignee"]);
                result["Customer"] = string.Format("{0}", dr["Customer"]);

                result["Destination"] = string.Format("{0}", dr["Destination"]);
                result["RefSamples"] = string.Format("{0}", dr["RefSamples"]);
                result["Country"] = string.Format("{0}", dr["Country"]);
                result["PetType"] = string.Format("{0}", dr["PetType"]);
                result["Division"] = string.Format("{0}", dr["Division"]);
                result["Nutrition"] = string.Format("{0}", dr["Nutrition"]);
                result["PackingStyle"] = string.Format("{0}",dr["PackingStyle"]);
                result["Eventlog"] = string.Format("{0}", DisplayEventlog(e));
                e.Result = result;
            }
        }
    }
    public string DisplayEventlog(ASPxGridViewCustomDataCallbackEventArgs e)
    {
        var args = e.Parameters.Split('|');
        int id;
        var text = string.Format(NotFoundFormat, args[2]);
        if (int.TryParse(args[2], out id))
        {
            StringBuilder sb = new StringBuilder();
            string strSQL = @"select requester+''+isnull('|'+assignee,'')assignee,RequestNo" +
                " ,b.Title  from TransRequestForm a left join MasStatusApp b on b.Id = a.StatusApp where a.Id='" + id + "' and b.levelApp in (5)";
            DataTable dt = cs.builditems(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                var arr = dr["assignee"].ToString().Split('|'); string s = "";
                sb.Append(string.Format("StatusApp :{0}",dr["Title"].ToString()));
                for (int i = 0; i < arr.Length; i++)
                {
                    if (!string.IsNullOrEmpty(arr[i]))
                        s += cs.GetData(arr[i], "fn") + ",";
                }
                sb.Append(string.Format("<div><b><u>Request By : {0}</u></b></div>", s));
            }
            var Results = new DataTable();//spGetHistory
            SqlParameter[] param = { new SqlParameter("@Id", id),
                new SqlParameter("@tablename",string.Format("{0}","TransRequestForm")),
                new SqlParameter("@user",string.Format("{0}",cs.CurUserName))};
            Results = cs.GetRelatedResources("spGetHistory", param);

            if (Results.Rows.Count > 0)
            {

                foreach (DataRow dr in Results.Rows)
                {
                    //string[] printer = { "5", "6", "7" };
                    //if (string.Format("{0}",dr["StatusApp"])=="3")
                    var status = dr["StatusApp"].ToString();
                    var Title = string.Format("{0}", (new[] { "5", "6", "7" }).Contains(status) ? biuldAssignee(dr["Reason"].ToString().Replace('|', ',')) : dr["Reason"]);
                    sb.Append(string.Format(PreviewFormat, cs.GetData(dr["Username"].ToString(), "fn"), dr["Title"], Title, dr["CreateOn"], dr["Remark"]));
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
            }
            else
                sb.Append("<br/>");
            text = sb.ToString();
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
    protected void gridData_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        if (args[0] == "Approve")
        {
            string fieldCollection = string.Empty;
            List<object> fieldvalues = g.GetSelectedFieldValues(new string[] { "ID" });
            int count = g.Selection.Count;
            foreach (int auxRecId in g.GetSelectedFieldValues("ID"))
            {
                int Key = auxRecId;
                using (SqlConnection con = new SqlConnection(strConn))
                {
                    string query = "update TransRequestForm set StatusApp=5 WHERE ID=@ID and StatusApp=0";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", auxRecId.ToString());
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            if (count > 0)
            {
                List<string> lstSend = new List<string>();
                List<string> MailTo = new List<string>();
                List<string> MailCc = new List<string>();
                string _link = string.Format("Dear {0} <br/> ", String.Join(",", lstSend.ToArray()));

                cs.sendemail(String.Join(",", MailTo.ToArray()), String.Join(",", MailCc.ToArray()), _link, "Request Material Accepted by " + cs.GetData(user_name, "fn"));
            }
        }
        if (args[0] == "post")
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                //
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spInsertRequestForm";
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", args[1]));
                cmd.Parameters.AddWithValue("@Requester", cs.CurUserName.ToString());
                cmd.Parameters.AddWithValue("@RequestNo", string.Format("{0}", hfTrfID["TrfID"]));
                cmd.Parameters.AddWithValue("@Costno", string.Format("{0}", hfCostID["CostID"]));
                cmd.Parameters.AddWithValue("@Result", string.Format("{0}", beresult.Text));
                cmd.Parameters.AddWithValue("@assignee", 0); 
                cmd.Parameters.AddWithValue("@ProductGroup", string.Format("{0}", cmbProductGroup.Value));
                cmd.Parameters.AddWithValue("@StatusApp", 0);
                cmd.Parameters.AddWithValue("@RawMaterial", string.Format("{0}", cmbRawMaterial.Value));
                cmd.Parameters.AddWithValue("@StyleofPack", string.Format("{0}", cmbStyle.Value));
                cmd.Parameters.AddWithValue("@MediaType", string.Format("{0}", cmbTopping.Value));
                cmd.Parameters.AddWithValue("@NW", string.Format("{0}", cmbSizeMedia.Value));
                cmd.Parameters.AddWithValue("@ContainerLid", string.Format("{0}", CmbContainerLid.Value));
                cmd.Parameters.AddWithValue("@Remark", string.Format("{0}", mNotes.Text));
                cmd.Parameters.AddWithValue("@Grade", string.Format("{0}", cmbGrade.Value));
                cmd.Parameters.AddWithValue("@Zone", string.Format("{0}", cmbZone.Value));
                cmd.Parameters.AddWithValue("@CreateBy", string.Format("{0}", cs.CurUserName.ToString()));

                cmd.Parameters.AddWithValue("@Destination", string.Format("{0}", CmbDestination.Value));
                cmd.Parameters.AddWithValue("@Country", string.Format("{0}", tbCountry.Text));
                cmd.Parameters.AddWithValue("@RefSamples", string.Format("{0}", tbRefSamples.Text));
 
                cmd.Parameters.AddWithValue("@PetType", string.Format("{0}", CmbPetType.Value));
                cmd.Parameters.AddWithValue("@Division", string.Format("{0}", CmbDivision.Value));
                cmd.Parameters.AddWithValue("@Nutrition", string.Format("{0}", CmbNutrition.Value));
                cmd.Parameters.AddWithValue("@PackingStyle", string.Format("{0}", tbPackingStyle.Text));
                
                cmd.Parameters.AddWithValue("@RequestDate", defrom.Value);
                cmd.Parameters.AddWithValue("@DocumentNo", "");

                cmd.Connection = con;
                con.Open();
                var getValue = cmd.ExecuteScalar();
                con.Close();
                ApproveStep(getValue.ToString());
            }
        }
        if (args[0] == "Delete" && args.Length > 1)
        {
            Delete(args[1]);
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
        Update();
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
        //var NewID = hfgetvalue["NewID"].ToString();
        string parameter = e.Parameters;
        var args = e.Parameters.Split('|');
        if (args[0] == "load" || args[0] == "Assignee")
        {
                 lookup.DataBind();
        }
    }
    protected void gridData_ContextMenuItemClick(object sender, ASPxGridViewContextMenuItemClickEventArgs e)
    {

    }

    protected void gridData_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
    {

    }

    public override void Update()
    {
        //gridData.DataSource = dsgv;
        //DataTable dt = ((DataView)dsgv.Select(DataSourceSelectArguments.Empty)).Table;
        gridData.DataBind();
    }

    protected void cmbProductGroup_Callback(object sender, CallbackEventArgsBase e)
    {
 
    }

    protected void cmbTopping_Callback(object sender, CallbackEventArgsBase e)
    {

        FillCityCombo(e.Parameter);
    }

    protected void FillCityCombo(string countryName)
    {
        if (string.IsNullOrEmpty(countryName)) return;
        var args = countryName.Split('|');
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlParameter[] param = { new SqlParameter("@group", (string)args[0]) };
            var Results = myClass.GetRelatedResources("spGetStruMediaType", param);
            cmbTopping.DataSource = Results;
            cmbTopping.DataBind();
            if (args.Length > 1)
                cmbTopping.Value = (string)args[1];
        }
    }
    protected void CmbReason_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        if (string.IsNullOrEmpty(e.Parameter)) return;
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
                        new SqlParameter("@table","TransRequestForm"),
                        new SqlParameter("@username",user_name)};
        DataTable table = new DataTable();
        table = cs.GetRelatedResources("spGetStatusApproval", param);
        CmbDisponsition.DataSource = table;
        CmbDisponsition.DataBind();
    }
    protected void cmbPackingStyle_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        if (string.IsNullOrEmpty(e.Parameter)) return;
        var args = e.Parameter.Split('|');
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlParameter[] param = { new SqlParameter("@group", (string)args[0]) };
            var Results = myClass.GetRelatedResources("spGetProductStyle", param);
            combobox.DataSource = Results;
            combobox.DataBind();
            if (args.Length > 1)
                combobox.Value = (string)args[1];
        }
    }

    protected void cmbZone_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        if (string.IsNullOrEmpty(e.Parameter)) return;
        var args = e.Parameter.Split('|');
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlParameter[] param = { new SqlParameter("@group", (string)args[0]) };
            var Results = myClass.GetRelatedResources("spGetCustomerBrand", param);
            combobox.DataSource = Results;
            combobox.DataBind();
            if (args.Length > 1)
                combobox.Value = (string)args[1];
        }
    }

    protected void CmbContainerLid_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        if (string.IsNullOrEmpty(e.Parameter)) return;
        var args = e.Parameter.Split('|');
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlParameter[] param = { new SqlParameter("@group", (string)args[0]) };
            var Results = myClass.GetRelatedResources("spGetStruContainerLid", param);
            combobox.DataSource = Results;
            combobox.DataBind();
            if (args.Length > 1)
                combobox.Value = (string)args[1];
        }
    }

    protected void cmbSizeMedia_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        if (string.IsNullOrEmpty(e.Parameter)) return;
        var args = e.Parameter.Split('|');
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlParameter[] param = { new SqlParameter("@group", (string)args[0]) };
            var Results = myClass.GetRelatedResources("spGetStruCanSize", param);
            combobox.DataSource = Results;
            combobox.DataBind();
            if (args.Length > 1)
                combobox.Value = (string)args[1];
        }
    }

    protected void cmbRawMaterial_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        if (string.IsNullOrEmpty(e.Parameter)) return;
        var args = e.Parameter.Split('|');
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlParameter[] param = { new SqlParameter("@group", (string)args[0]) };
            var Results = myClass.GetRelatedResources("spGetTopping", param);
            combobox.DataSource = Results;
            combobox.DataBind();
            if(args.Length>1)
            combobox.Value = (string)args[1];
        }
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
    void GenerateDataSource(Guid gcrecord)
    {
        //if (!string.IsNullOrEmpty(hfgetvalue["NewID"].ToString())) {
        //    var gcrecord = string.Format("{0}", hfgetvalue["NewID"]);
        Session["DataSource"] = DataHelper.CreateDataSource(gcrecord.ToString(),
        user_name, "TransRequestForm");
        fileManager.DataBind();
        //}
    }
    protected void cmbGrade_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox combobox = (ASPxComboBox)sender;
        if (string.IsNullOrEmpty(e.Parameter)) return;
        var args = e.Parameter.Split('|');
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlParameter[] param = { new SqlParameter("@group", (string)args[0]) };
            var Results = myClass.GetRelatedResources("spGetGrade", param);
            combobox.DataSource = Results;
            combobox.DataBind();
            if (args.Length > 1)
                combobox.Value = (string)args[1];
        }
    }

    protected void gridData_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            //e.Items.Add(e.CreateItem("Copy", "Copy"));
            //e.Items.Add(e.CreateItem("Revised", "Revised"));
            var item = e.CreateItem("Revised", "Revised");
            item.Image.Url = @"~/Content/Images/Edit.gif";
            e.Items.Add(item);
            GridViewContextMenuItem test = e.CreateItem("Copied", "Copied");
            test.Image.Url = @"~/Content/Images/Copy.png";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), test);
            //var item = e.CreateItem("Quotation", "Quotation");
            //item.Image.Url = @"~/Content/Images/excel.gif";
            //e.Items.Add(item);

            //item = e.CreateItem("File calculation sheet", "calculation");
            //item.BeginGroup = true;
            //item.Image.Url = @"~/Content/Images/excel.gif";
            //e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), item);
        }
    }

    //public void gridView_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    //{
    //    if (e.Parameters != "ValueChanged") return;

    //    ASPxGridView grid = sender as ASPxGridView;
    //    var selectedValues = cmbProductGroup.Value;
    //    using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        SqlParameter[] param = { new SqlParameter("@group", (string)selectedValues) };
    //        var Results = cs.GetRelatedResources("spGetCanSize", param);
    //        grid.DataSource = Results;
    //        grid.DataBind();

    //    }
    //}

    //protected void GridLookup_Init(object sender, EventArgs e)
    //{
    //    ASPxGridLookup gridLookup = sender as ASPxGridLookup;
    //    gridLookup.GridView.CustomCallback += new ASPxGridViewCustomCallbackEventHandler(gridView_CustomCallback);
    //}
}
public class TransFileDetails
{
    public int ID { get; set; }
    //public string Component { get; set; }
    public string Result { get; set; }
    public string Name { get; set; }
    public Byte[] Attached { get; set; }
    public string RequestNo { get; set; }
}