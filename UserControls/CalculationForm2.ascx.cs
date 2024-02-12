using ClosedXML.Excel;
//using DevExpress.CodeParser;
//using DevExpress.Data;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using DevExpress.Web;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxSpreadsheet;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Data.Linq;
//using DevExpress.Mvvm.POCO;
//using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtension;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
//using System.ComponentModel;

public partial class UserControls_CalculationForm : MasterUserControl
{
    ServiceCS myservice = new ServiceCS();
    MyDataModule cs = new MyDataModule();
   
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    //string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    string approverole = @"Margin,Pacifical,MSC,TG EU,Other";
    HttpContext Context = HttpContext.Current;
    ASPxSpreadsheet spreadsheet = new ASPxSpreadsheet();
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
    string FilePath
    {
        get { return this.Session["sessionFile"] == null ? String.Empty : this.Session["sessionFile"].ToString(); }
        set { this.Session["sessionFile"] = value; }
    }
    string separa
    {
        get { return this.Session["separa"] == null ? String.Empty : this.Session["separa"].ToString(); }
        set { this.Session["separa"] = value; }
    }
    //public List<StdItems> listItems
    //{
    //    get { return Session["list"] == null ? null : (List<StdItems>)Session["list"]; }
    //    set { Session["list"] = value; }
    //}
    public List<StdItems> listItems
    {
        get
        {
            var obj = this.Session["myList"];
            if (obj == null) { obj = this.Session["myList"] = new List<StdItems>(); }
            return (List<StdItems>)obj;
        }

        set
        {
            this.Session["myList"] = value;
        }
    }
    //public DataTable _UpChargedt
    //{
    //    get { return Page.Session["UpChargedt"] == null ? null : (DataTable)Page.Session["UpChargedt"]; }
    //    set { Page.Session["UpChargedt"] = value; }
    //}
    //public DataTable _utilizedt
    //{
    //    get { return Page.Session["utilizedt"] == null ? null : (DataTable)Page.Session["utilizedt"]; }
    //    set { Page.Session["utilizedt"] = value; }
    //}
    //public DataTable tcustomer
    //{
    //    get { return Page.Session["tcustomer"] == null ? null : (DataTable)Page.Session["tcustomer"]; }
    //    set { Page.Session["tcustomer"] = value; }
    //}
    public DataTable _dt
    {
        get { return Page.Session["setableKey"] == null ? null : (DataTable)Page.Session["setableKey"]; }
        set { Page.Session["setableKey"] = value; }
    }
    public DataTable USP_Select_FileDetails
    {
        get { return Page.Session["USP_Select_FileDetails"] == null ? null : (DataTable)Page.Session["USP_Select_FileDetails"]; }
        set { Page.Session["USP_Select_FileDetails"] = value; }
    }
    //public DataTable _dtcomponent
    //{
    //    get { return Page.Session["component"] == null ? null : (DataTable)Page.Session["component"]; }
    //    set { Page.Session["component"] = value; }
    //}
    public DataTable _CurrentTableCol
    {
        get { return Page.Session["col"] == null ? null : (DataTable)Page.Session["col"]; }
        set { Page.Session["col"] = value; }
    }
    //private DataTable TempTabel
    //{
    //    get
    //    {
    //        DataTable dt = new DataTable();
    //        if (ViewState["CurrentTabele"] == null)
    //        {
    //            dt.Columns.AddRange(new DataColumn[9]{new DataColumn("ID",typeof(int)),
    //                                        new DataColumn("LastWriteTime",typeof(DateTime)),
    //            new DataColumn("LastUpdateBy",typeof(string)),
    //            new DataColumn("Result",typeof(string)),
    //            new DataColumn("Name",typeof(string)),
    //            new DataColumn("Notes",typeof(string)),
    //            new DataColumn("Attached",typeof(byte[])),
    //            new DataColumn("SubID",typeof(string)),
    //            new DataColumn("Request",typeof(string))});
    //            ViewState["CurrentTabele"] = dt;
    //        }
    //        return (DataTable)ViewState["CurrentTabele"];
    //    }
    //    set
    //    {
    //        ViewState["CurrentTabele"] = value;
    //    }
    //} 
    EdiFormTemplate template = new EdiFormTemplate();
    protected string SearchText { get { return Utils.GetSearchText(Page); } }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            SetInitialRow();
        //gv.Templates.EditForm = template;
        //template.Grid = gv;
    }
    public void SetInitialRow()
    {
        Session.Clear();
        hfuser["user_name"] = cs.CurUserName;
        hfStatusApp["StatusApp"] = string.Format("{0}", 0);
        hfgetvalue["NewID"] = string.Format("{0}", cs.GetNewID());
        editor["Name"] = cs.IsMemberOfRole(string.Format("{0}", 0));
        hftype["type"] = string.Format("{0}", 0);
        Page.Session["BU"] = string.Format("{0}", cs.GetData(cs.CurUserName, "bu"));
        hGeID["GeID"] = string.Format("{0}", 0);
        hCommission["Commis"] = "";
        hfUpchargeGroup["Upcharge"] = "";
        hfRefAtt["RefAtt"] = "";
        hfid["ID"] = string.Format("{0}", 0);
        _CurrentTableCol = buildcolumnTable();
        approv["approv"] = "";
 
        //BuildSelect_FileDetails();
        //if (hf.Contains("isNewClicked"))
        //    if ((bool)hf.Get("isNewClicked") == true)
        //        gv.SettingsEditing.Mode = GridViewEditingMode.EditForm;
        //    else
        //        gv.SettingsEditing.Mode = GridViewEditingMode.Batch;
        //if (gridData.Columns["Command"] == null)
        //{
        //    AddColumn(gridData, 0);
        //}
        //deValidityDate.Border.BorderColor = System.Drawing.Color.Red;
    }
    private void AddColumn(ASPxGridView grid, int index)
    {
        GridViewCommandColumn col = new GridViewCommandColumn();
        col.Name = "Command";
        col.ShowSelectCheckbox = true;
        col.HeaderCaptionTemplate = check;
        col.VisibleIndex = index;
        col.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        col.Width = Unit.Pixel(45);
        grid.Columns.Insert(index, col);
    }

    string GetDescrip(string Code)
    {
        string name = "";
        SqlParameter[] param = { new SqlParameter("@Code", string.Format("{0}", Code)) };
        DataTable result = cs.GetRelatedResources("spGetDescription", param);
        if (result.Rows.Count > 0)
            name = result.Rows[0]["name"].ToString();
        return name.ToString();
    }
    //protected void gv_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
    //{
    //    ASPxGridView g = (ASPxGridView)sender;
    //    var values = new[] { "Upcharge" };
    //    string[] valueType = Regex.Split("Material;From;To;Commission;OverPrice;OverType;Pacifical;MSC;Margin;SubContainers", ";");
    //    DataRow dr = tcustomer.Rows.Find(e.Keys[0]);
    //    //foreach (DataColumn column in tcustomer.Columns)
    //    //{
    //    //    if (values.Any(column.ColumnName.Contains))
    //    for (int i = 0; i <= valueType.Length - 1; i++)
    //    {
    //        if (valueType[i] == "Material")
    //        {
    //            dr["Description"] = GetDescrip(e.NewValues[valueType[i]].ToString());
    //            dr[valueType[i]] = e.NewValues[valueType[i]];
    //        }
    //        else
    //        if (valueType[i].ToString() == "From")
    //            dr[valueType[i]] = defrom.Value;
    //        else if (valueType[i].ToString() == "To")
    //            dr[valueType[i]] = deto.Value;
    //        else
    //            dr[valueType[i]] = e.NewValues[valueType[i]];
    //    }
    //    //}
    //    if (_dt != null)
    //        if (_dt.Columns.Count > 0)
    //        {
    //            DataRow rw = _dt.Select("Name='FOB price(18 - digits)' and Calcu=8").FirstOrDefault();
    //            if (rw != null)
    //            {
    //                dr["MinPrice"] = rw["Result"].ToString();
    //                dr["OfferPrice"] = _dt.Compute("Max(Result)", "Name='Offer Price' and Calcu=8").ToString();
    //            }
    //            var valuesupcha = new[] { "Secondary Packaging", "FOB price(18 - digits)" };
    //            DataRow[] result = _dt.Select(string.Format("Calcu='{0}'", 6));
    //            foreach (DataRow _rw in result)
    //            {
    //                if (!valuesupcha.Any(_rw["Name"].ToString().Contains))
    //                {
    //                    DataRow rwupcharge = _UpChargedt.Select(string.Format("UpCharge in ('{0}') and SubID ='{1}' and RequestNo ='{2}'",
    //                        _rw["Name"], e.Keys[0], hGeID["GeID"])).FirstOrDefault();
    //                    if (rwupcharge != null)
    //                    {
    //                        rwupcharge["UpchargeGroup"] = _rw["Component"];
    //                        rwupcharge["UpCharge"] = _rw["Name"];
    //                        rwupcharge["Quantity"] = _rw["Quantity"];
    //                        rwupcharge["StdPackSize"] = _rw["Unit"];
    //                    }
    //                    else
    //                    {
    //                        DataRow _rwupcharge = _UpChargedt.NewRow();
    //                        int NextRowID = Convert.ToInt32(_UpChargedt.AsEnumerable()
    //                            .Max(row => row["ID"]));
    //                        NextRowID++;
    //                        _UpChargedt.Rows.Add(NextRowID,
    //                            _rw["Component"],
    //                            _rw["Name"],
    //                            _rw["Price"],
    //                            _rw["Quantity"],
    //                            _rw["Currency"],
    //                            _rw["Result"],
    //                            _rw["Unit"],
    //                            e.Keys[0],
    //                            hGeID["GeID"]);
    //                    }
    //                }
    //            }
    //        }
    //    //utilizedt
    //    List<string> list = new List<string>();
    //    foreach (DataRow rwuti in _utilizedt.Rows)
    //    {
    //        list.Add(rwuti["Result"].ToString());
    //    }
    //    if (list.Count > 0)
    //        dr["Utilize"] = String.Join("|", list.ToArray());

    //    g.CancelEdit();
    //    e.Cancel = true;

    //    g.JSProperties["cpUpdatedMessage"] = "successfully";
    //}
    DataRow updated(DataRow dr, int ID, string X)
    {

        return dr;
    }
    //protected void gv_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
    //{
    //    ASPxGridView g = (ASPxGridView)sender;
    //    int NextRowID = Convert.ToInt32(tcustomer.AsEnumerable()
    //                .Max(row => row["ID"]));
    //    DataRow rw = tcustomer.NewRow();
    //    NextRowID++;
    //    rw["ID"] = NextRowID;
    //    separa = rw["ID"].ToString();
    //    var values = new[] { "ID", "RowID", "RequestNo" };
    //    foreach (DataColumn column in tcustomer.Columns)
    //    {
    //        if (!values.Any(column.ColumnName.Contains))
    //        {
    //            if (column.ColumnName.ToString() == "From")
    //                rw[column.ColumnName] = defrom.Value;
    //            else if (column.ColumnName.ToString() == "To")
    //                rw[column.ColumnName] = deto.Value;
    //            else
    //                rw[column.ColumnName] = e.NewValues[column.ColumnName];
    //        }
    //    }
    //    tcustomer.Rows.Add(rw);
    //    e.Cancel = true;
    //    g.CancelEdit();
    //}
    DataTable exchangerate()
    {
        SqlParameter[] param = { new SqlParameter("@from", DateTime.Now),
            new SqlParameter("@to", DateTime.Now.ToString("yyyy-MM-dd"))};
        var dt = cs.GetRelatedResources("spTunaStdExchangeRat", param);
        return dt;
    }

    protected void gvitems_CustomDataCallback(object sender, DevExpress.Web.ASPxGridViewCustomDataCallbackEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var result = new Dictionary<string, string>();
        var args = e.Parameters.Split('|');
        if (args[0] == "PackSize")
        {
            var obj = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(args[1]));
            var _dtx = MyToDataTable.ToDataTable(obj.cal);
            result["Unit"] = GetPackSize(_dtx).ToString();
            e.Result = result;
        }
            if (args[0] == "Upcharg")
        {
            string strSQL = string.Format("select * from StandardUpcharge where Id='{0}'", args[1]);
            DataTable zdt = cs.builditems(strSQL);
            var rcus = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(args[2]));
            var _dtx = MyToDataTable.ToDataTable(rcus.cal);
            String strPackSize = GetPackSize(_dtx).ToString();
            if (zdt.Rows.Count > 0)
                foreach (DataRow dr in zdt.Rows)
                {
                    result["UpchargeGroup"] = dr["UpchargeGroup"].ToString();
                    result["UpCharge"] = dr["Upcharge"].ToString();
                    result["Value"] = dr["Value"].ToString();
                    result["StdPackSize"] = dr["StdPackSize"].ToString();
                    result["Unit"] = strPackSize.ToString();
                    result["Result"] = ((Convert.ToDouble(dr["Value"]) / Convert.ToDouble(dr["StdPackSize"])) * Convert.ToDouble(strPackSize)).ToString();
                    e.Result = result;
                }

        }
    }
    protected void gv_CustomDataCallback(object sender, DevExpress.Web.ASPxGridViewCustomDataCallbackEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var result = new Dictionary<string, string>();
        var args = e.Parameters.Split('|');
        if (args[0] == "_Margin" || args[0] == "_MSC" || args[0] == "_Pacifical")
        {
            result["Announcement_Fish_price"] = "";
            var licus1 = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(args[1]));
            DataTable fishgroup = getfishgroup(licus1.Material);
            foreach (DataRow rw in fishgroup.Rows) {
                if (args[0] == "_Margin")
                {
                    result["Announcement_Fish_price"] = licus1.diff_price;
                    //licus1.Margin = string.Format("{0}", args[2]);
                    //buildcalcu(licus1);
                    //result["Result"] = GetMargin(rw["SHD"].ToString());
                    //result["Result"] = Convert.ToDouble(licus1.Equivalent_Fish_price) - Convert.ToDouble(licus1.Announcement_Fish_price) > 70 ?  "0":string.Format("{0}", args[2]) ;
                }
                else
                {
                    DataTable table = GetCertificate(rw["FishGroup"].ToString());
                    if (table.Rows.Count > 0)
                    {
                        foreach (DataRow rw2 in table.Rows)
                        {
                            if (rw2["Certificate_fee"].ToString() == "_MSC")
                                result["Result"] = rw2["free"].ToString();
                            if (rw2["Certificate_fee"].ToString() == "_Pacifical")
                                result["Result"] = rw2["free"].ToString();
                        }
                    }
                    else result["Result"] = "0";
                }
            }
            //if (args[0] == "_Margin" && !string.IsNullOrEmpty(licus1.Bidprice))
            //{
            //    buildcalcu(licus1);
            //    DataRow _xdr = _dt.Select(string.Format("Name='Equivalent fish price' and Calcu={0}", 8)).FirstOrDefault();
            //    licus1.Announcement_Fish_price = Convert.ToDecimal(_xdr["Result"]).ToString("F2");
            //    result["Announcement_Fish_price"] = licus1.Announcement_Fish_price;
            //}
            e.Result = result;
        }
        if (args[0] == "zone")
        {
            var dtzone = ((DataView)dsCustomer.Select(DataSourceSelectArguments.Empty)).Table;
            if (dtzone != null)
            {
                DataRow _r = dtzone.Select("Code='" + args[1].ToString() + "'").FirstOrDefault();
                if (_r != null)
                {
                    result["zone"] = _r["zone"].ToString();
                    e.Result = result;
                }
            }
        }
        if (args[0] == "download")
        {
            var licus3 = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(args[1]));
            if (licus3 != null)
            {

                List<TransStdFileDetails> listFileDetails = licus3.FileDetails;
                TransStdFileDetails liresult = listFileDetails.Find(x => x.SubID == args[1].ToString() && x.Result == "Margin");
                if (liresult != null) {
                    //result["Attached"] = liresult.Attached.ToString();
                    result["Attached"] = liresult.ID.ToString();
                    e.Result = result;
                }
                //ExportToResponse(result.Name, result.Attached, MimeTypes.GetContentType(result.Attached.ToString()),true);
                //Content/UploadControl/d0frlbjx.xls
            }
        }
        if (args[0] == "Attached")
        {
            
            DataRow file = USP_Select_FileDetails.Select(string.Format("ID={0}", args[1].ToString())).FirstOrDefault();
            result["Attached"] = file["Attached"].ToString();
            e.Result = result;
        }
        //if (args[0] == "zterm")
        //{
        //    SqlParameter[] param = { new SqlParameter("@Code", args[1].ToString()) };
        //    DataTable zdt = cs.GetRelatedResources("spSelectPayment", param);
        //    if (zdt.Rows.Count > 0)
        //        foreach (DataRow dr in zdt.Rows)
        //        {
        //            result["LeadTime"] = dr["LeadTime"].ToString();
        //            result["knvv_zterm"] = dr["knvv_zterm"].ToString();
        //            e.Result = result;
        //        }

        //    else
        //    {
        //        result["LeadTime"] = "0";
        //        result["knvv_zterm"] = "0";
        //        e.Result = result;
        //    }
        //}
        if (args[0] == "tot" || args[0] == "sub" ||args[0] == "Freight")
        {
            string[] param = new string[] { args[3], args[4], CmbCustomer.Value.ToString(), CmbShipTo.Value.ToString() };
            DataTable dt = GetselectFreight(param);
            //int cou = dt.Rows.Count;
            //if (cou > 0)
            //{
            if (dt.Rows.Count == 0)
            {
                result["Insurance"] = "0.0011";
                result["Freight"] = "0";
                e.Result = result;
            }
            else
                foreach (DataRow dr in dt.Rows)
                {
                    //double minprice = Convert.ToDouble(tbMinPrice.Text);
                    double _Insurance = 1;
                    if (args[5] != "")
                    {
                        if (args[5].ToString().Substring(1, 1) == "I")
                            //double.TryParse(tbInsurance.Text, out Insurance);
                            _Insurance = 0.0011;
                    }
                    result["Insurance"] = "0.0011";
                    result["Freight"] = string.Format("{0}", Convert.ToDouble(dr["MKTCost"]) * _Insurance);
                    e.Result = result;
                }
        }
        if (args[0] == "reload")
        {
            //tcustomer = new DataTable();
            List<StdItems> listItems = new List<StdItems>();
            //tcustomer = buildLoadgv;
        }
        if (args[0] == "MSC")
        {
            DataTable fishgroup = getfishgroup(args[1]);
            foreach (DataRow rw in fishgroup.Rows)
            {
                result["Margin"] = GetMargin(rw["SHD"].ToString());
                DataTable table = GetCertificate(rw["FishGroup"].ToString());
                //if (table.Rows.Count > 0)
                //    foreach (DataRow rw in table.Rows){
                //        result["value"] = string.Format(@"{0}", rw["free"]);
                //    }
                //else
                result["SubContainers"] = "0";
                result["MSC"] = "0";
                result["Pacifical"] = "0";
                result["Commission"] = "0";
                result["OverPrice"] = "0";
                result["OverType"] = "USD";
                DataTable _table = GetCommission();
                foreach (DataRow _rw in _table.Rows)
                {
                    result["Commission"] = string.Format(@"{0}", _rw["Commission"]);
                    result["OverPrice"] = string.Format(@"{0}", _rw["OverPrice"]);
                    result["OverType"] = string.Format(@"{0}", _rw["Unit"]);
                }
                foreach (DataRow rw2 in table.Rows)
                {
                    if (rw2["Certificate_fee"].ToString() == "MSC")
                        result["MSC"] = rw2["free"].ToString();
                    if (rw2["Certificate_fee"].ToString() == "Pacifical")
                        result["Pacifical"] = rw2["free"].ToString();
                }
            }
            //result["value"] = table.Rows.Count > 0 ? table.Rows[0]["free"].ToString() : "0";
            result["Description"] = GetDescrip(args[1].ToString());
            e.Result = result;
        }
        if (args[0] == "build" && args[1].ToString() != "")
        {
            separa = args[1].ToString();
            //DataRow dr = tcustomer.Rows.Find(args[1]);
            var obj = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(args[1]));
            if (obj != null)
            {
                result["Material"] = string.Format(@"{0}", obj.Material);
                result["From"] = Convert.ToDateTime(obj.From).ToString("dd-MM-yyyy");
                result["To"] = Convert.ToDateTime(obj.To).ToString("dd-MM-yyyy");
                result["ID"] = string.Format("{0}", obj.ID);
                result["SubContainers"] = obj.SubContainers.ToString();
                result["MSC"] = obj.MSC.ToString();
                result["Description"] = GetDescrip(obj.Material.ToString());

                //foreach (DataRow _rwx in exchangerate().Rows){
                //    result["Rate"] = _rwx["To"].ToString();
                //    result["Currency"] = _rwx["Currency"].ToString();
                //}
            }
            e.Result = result;
        }
        if (args[0] == "post")
        {
            result["OfferPrice"] = _dt.Compute("Max(Result)", "Name='Offer Price' and Calcu=8").ToString();
            e.Result = result;
        }
        if (args[0].Equals("approve"))
        {
            var _t = GetLevel(cs.CurUserName);
            if (_t.Rows.Count > 0)
            {
                List<StdItems> SortedList = listItems.OrderBy(o => o.IsAccept).ToList();
                foreach (var _r in SortedList)
                {
                    //_r.IsAccept = checkapprove(_r);
                    if (string.Format("{0}", _r.isapprove) == "" && string.Format("{0}", _r.IsAccept).Contains("level"))
                    {
                        result["approve"] = string.Format("{0}", "level1");
                        break;
                    }
                    else if (string.Format("{0}", _r.isapprove) != "" && string.Format("{0}", _r.IsAccept).Contains("level2"))
                    {
                        result["approve"] = string.Format("{0}", "level2");
                        break;
                    }
                }

                //foreach (var _r in listItems)
                //{
                //    var _FileDetails = USP_Select_FileDetails.Select("Result = 'ValidityDate'");
                //    var _f = _r.FileDetails.Find(x => x.Result == "Freight" || x.Result == "Margin");
                //    if (_f != null)
                //        result["approve"] = "level1";//AM
                //    else if (_FileDetails.Length > 0)
                //        result["approve"] = "level2";//M
                //    else
                //        result["approve"] = checkapprove(_r);
                //    if (result.Count > 0)
                //        break;
                //}
                //if(string.IsNullOrEmpty( _t.Rows[0]["l1"].ToString()))
                //string.Format("{0}", approv["approv"])=="0" && 
                if (CmbDisponsition.Value.Equals("4"))
                {
                    for (int i = 0; i < gv.VisibleRowCount; i++)
                    {
                        //var selectedRowsKeys = gv.GetSelectedFieldValues(gv.KeyFieldName);
                        object fieldValue = gv.GetRowValues(i, "Notes");
                        object SubID = gv.GetRowValues(i, gv.KeyFieldName);
                        var sel = gv.Selection.IsRowSelected(i);
                        if (!string.IsNullOrEmpty(fieldValue.ToString()) && sel == false)
                        {
                            result["approve"] = "reject";
                        }
                    }
                }
            }
            e.Result = result;
        }
        //else
        //e.Result = string.Format("{0}", e.Parameters);
    }
    //
    DataTable getfishgroup(string _separa)
    {
        //string result = "";
        DataTable dt = new DataTable();
        if (!string.IsNullOrEmpty(_separa))
        {
            dt = cs.GetFillWeight(_separa);
            //if (dt.Rows.Count > 0)
            //    result = dt.Rows[0]["FishGroup"].ToString();
        }
        return dt;
    }
    DataTable GetLevel(string Assignee)
    {
            SqlParameter[] param = { new SqlParameter("@user", string.Format("{0}", Assignee))};
        return cs.GetRelatedResources("spbuildApprove", param);
    }
    //void insert(string levelApp, string RequestNo, string Activeby, string fn)
    //{

    //        using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        SqlCommand cmd = new SqlCommand();
    //        cmd.CommandText = "spinsertstdApprove";
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.Parameters.AddWithValue("@RequestNo", RequestNo);
    //        cmd.Parameters.AddWithValue("@levelApp", levelApp);
    //        cmd.Parameters.AddWithValue("@fn", fn);
    //        cmd.Parameters.AddWithValue("@Activeby", Activeby);
    //        cmd.Connection = con;
    //        con.Open();
    //        cmd.ExecuteNonQuery();
    //        con.Close();
    //    }
    //}
    DataTable GetCommission()
    {
        DataTable dt = new DataTable();
        if (!string.IsNullOrEmpty(CmbCustomer.Value.ToString()))
        {
            SqlParameter[] param = { new SqlParameter("@Customer", string.Format("{0}", CmbCustomer.Value)) };
            dt = cs.GetRelatedResources("spGetCommission", param);
        }
        return dt;
    }
    DataTable GetCertificate(string fishgroup)
    {
        DataTable dt = new DataTable();
        if (!string.IsNullOrEmpty(fishgroup))
        {
            SqlParameter[] param = { new SqlParameter("@FishGroup", string.Format("{0}", fishgroup)) };
            dt = cs.GetRelatedResources("spGetCertificate", param);
        }
        return dt;
    }
    string GetMargin(string _separa)
    {
        string result = "0";
        DataTable dt = new DataTable();
        if (!string.IsNullOrEmpty(_separa))
        {
            SqlParameter[] param = { new SqlParameter("@Zone", string.Format("{0}", cmbZone.Value)),
            new SqlParameter("@SHD", string.Format("{0}", _separa)),
            new SqlParameter("@validDate", string.Format("{0}", deto.Value))};
            dt = cs.GetRelatedResources("spstdtunaMargin", param);
            if (dt.Rows.Count > 0)
                result = dt.Rows[0]["Margin"].ToString();
        }
        return result;
    }
    protected void gv_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
    {
        //((ASPxPopupControl)e.EditForm.NamingContainer).ClientSideEvents.PopUp = string.Format(@"function(s, e) {{
        //     s.SetWidth(gv.GetWidth());
        //}}");
    }
    //DataTable GetTable()
    //{
    //    // Here we create a DataTable with four columns.
    //    int Id = Convert.ToInt32(hGeID["GeID"]);
    //    SqlParameter[] param = { new SqlParameter("@Param", Id.ToString()),
    //    new SqlParameter("@Code", CmbMaterial.Text.ToString())};
    //    var table = cs.GetRelatedResources("spGetTunaCalculation", param);
    //    table.PrimaryKey = new DataColumn[] { table.Columns["RowID"] };
    //    return table;
    //}

    //protected void gvcal_DataBinding(object sender, EventArgs e)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
    //    g.KeyFieldName = "RowID";
    //    g.DataSource = cs._dataTable;
    //    g.ForceDataRowType(typeof(DataRow));
    //}

    //protected void gvcal_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
    //    if (string.IsNullOrEmpty(e.Parameters))
    //        return;
    //    var args = e.Parameters.Split('|');
    //    if (args[0] == "reload"){
    //        this.Page.Session.Remove("sessionKey");
    //        cs._dataTable = GetTable();
    //    }
    //    if (args[0] == "sub")
    //    {
    //        if (cs._dataTable != null)
    //        {
    //            DataRow r = cs._dataTable.Select("Name='"+ args[2]+"'").FirstOrDefault();
    //            if (r != null)
    //            {
    //                r["Result"] = args[1];
    //                cs._dataTable.AcceptChanges();
    //            }
    //        }
    //    }
    //    g.DataBind();
    //}
    protected void CmbCustomer_Callback(object sender, CallbackEventArgsBase e)
    {

    }
    protected void CmbPaymentTerm_Callback(object sender, CallbackEventArgsBase e)
    {

    }

    //protected void gvcal_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    //{
    //    if (string.IsNullOrEmpty(e.Parameters))
    //        return;
    //    var result = new Dictionary<string, string>();
    //    var args = e.Parameters.Split('|');
    //    if (args[0] == "reload")
    //    {
    //         result["ExchangeRate"] = "";           
    //        //
    //        SqlParameter[] param = { new SqlParameter("@d", DateTime.Now.ToString("yyyy-MM-dd"))};
    //        var table = cs.GetRelatedResources("spTunaStdExchangeRat", param);
    //        foreach(DataRow row in table.Rows) {
    //            result["ExchangeRate"] = string.Format("{0}", row["Rate"]);
    //            result["Currency"] = string.Format("{0}", row["Currency"]); 
    //        }
    //        e.Result = result;
    //    }
    //}

    protected void gv_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;

        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        long id;
        if (!long.TryParse(args[1], out id))
            return;
        gv.Toolbars[0].Items[0].Enabled = (tbRequestNo.Text.Substring(1, 1) == "#" ? true : false);
        gv.Toolbars.Owner.LayoutChanged();
        if (args[0] == "recalup")
        {
            var rw = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(id));

        }
        //if (args[0] == "Valid")
        //{
        //    var _b = (CmbIncoterm.Value.Equals("CFR") || CmbIncoterm.Value.Equals("CIF"));
        //    g.SettingsEditing.BatchEditSettings.AllowValidationOnEndEdit = !_b;
        //}
        if (args[0] == "Change")
        {
            var rw = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(id));
            List<StdItems> obj = (from item in listItems
                                  where item.ID != Convert.ToInt32(id)
                                  select item).ToList(); 
                                  //listItems.Select(x => x.Material == rw.Material.Substring(2,3)).ToList();
            foreach (var item in obj) {

                SqlParameter[] param = { new SqlParameter("@code", string.Format("{0}",rw.Material)),
                new SqlParameter("@SapCodedigit", string.Format("{0}",item.Material))};
                var tablediff = cs.GetRelatedResources("spCheckDiffCode", param);
                if (string.Format("{0}", tablediff.Rows[0]["Result"]) == "0")
                {
                    //spCheckDiffCode
                    item.Announcement_Fish_price = string.Format("{0}", args[2]);
                    item.Apply = "Apply";
                    item.Bidprice = "";
                    _dt = GetCalcu(item);
                    buildcalcu(item);

                    DataRow _arow = _dt.Select("Name='Equivalent margin' and Calcu=9").FirstOrDefault();
                    string _text = "Name in ('FOB price(18 - digits)')  and Calcu={0}";
                    object sumObject = _dt.Compute(@"Sum(Result)", string.Format(_text, 8));
                    object sumMargin = _dt.Compute(@"Sum(Result)", string.Format("Name in ('Margin')", 8));
                    object conObject = _dt.Compute(@"Sum(Result)", string.Format(_text, 9));
                    if (!Convert.IsDBNull(sumMargin))
                        item.Equivalent_Margin = Convert.ToDecimal(Convert.ToDecimal((Convert.ToDecimal(conObject) - (Convert.ToDecimal(sumObject) -
                        Convert.ToDecimal(sumMargin))) / Convert.ToDecimal(conObject)) * 100).ToString("F4");
                    else
                        item.Equivalent_Margin = "0";
                }

            }
        }
        if (args[0] == "Rate")
        {
            foreach (var item in listItems)
            {
                _dt = GetCalcu(item);
                //if (_dt != null)
                item.cal = cs.ConvertDataTable<TransCal>(_dt);
                buildcalcu(item);

            }
        }
        if (args[0] == "recal")
        {
            foreach (var item in listItems)
            {
                //SqlParameter[] param = { new SqlParameter("@RequestNo", string.Format("{0}", hGeID["GeID"])),
                //new SqlParameter("@SubID", string.Format("{0}", item.ID))};
                //_dt = cs.GetRelatedResources("spGetTunaCal", param); ;
                //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                //_dt = GetCalcu(Convert.ToInt32(item.ID));
                //item.cal = cs.ConvertDataTable<TransCal>(_dt);
                buildcalcu(item);
            }
        }
        if (args[0] == "reutilize")
        {
            foreach (var t in listItems)
            {
                DataTable _utilizedt = Getutilize(Convert.ToDateTime(defrom.Value).Date,
                Convert.ToDateTime(deto.Value).Date);
                t.Utilize = (from DataRow dr in _utilizedt.Rows
                             select new TransUtilize()
                             {
                                 RowID = Convert.ToInt32(dr["RowID"]),
                                 Result = dr["Result"].ToString(),
                                 MonthName = dr["MonthName"].ToString(),
                                 Cost = dr["Cost"].ToString(),
                                 SubID = dr["SubID"].ToString(),
                                 RequestNo = dr["RequestNo"].ToString()
                             }).ToList();
                //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                _dt = GetCalcu(t);
                //if (_dt !=null)
                t.cal = cs.ConvertDataTable<TransCal>(_dt);
                buildcalcu(t);
            }
        }
        if (args[0] == "del")
        {
            var rw = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(id));
            listItems.Remove(rw);
            deleteItems(id);
        }
        if (args[0] == "delatt")
        {
            var listdelatt = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(id));
            List<TransStdFileDetails> delatt = listdelatt.FileDetails;
            TransStdFileDetails resultdelatt = delatt.Find(x => x.SubID == id.ToString() && x.Result == args[2].ToString());
            //DataRow dr = USP_Select_FileDetails.Rows.Find(args[1]);
            if (resultdelatt != null)
            {
                delatt.Remove(resultdelatt);
                listdelatt.FileDetails = delatt;
            }
        }
        if (args[0] == "ref")
        {
            var dr = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(id));
            //_dt = GetCalcu(dr);
            //dr.cal = cs.ConvertDataTable<TransCal>(_dt);
            buildcalcu(dr);
        }
        if (args[0] == "new")
        {

            //    //DataTable dt = (DataTable)g.DataSource;
            //    string strSQL = string.Format(@"Material='{0}' and Customer = '{1}' and ShipTo = '{2}' and Mark = 'X' ",
            //            CmbMaterial.Text, CmbCustomer.Value, CmbShipTo.Value);
            //    DataRow r = tcustomer.Select(strSQL).FirstOrDefault();
            //    if (r == null)
            //    {
            //        int max = Convert.ToInt32(tcustomer.AsEnumerable()
            //.Max(row => row["ID"]));
            //        max = max + 1;
            //        DataRow dr = tcustomer.NewRow();
            //        dr = BuildCustom(dr, max, "X");
            //        tcustomer.Rows.Add(dr);
            //        //gv.DataSource = tcustomer;
            //    }
        }
        if (args[0] == "editcomment")
        {
            var licus3 = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(id));
            List<TransStdFileDetails> list3 = licus3.FileDetails;
            TransStdFileDetails result3 = list3.Find(x => x.SubID == id.ToString() && x.Result == args[3].ToString());
            //DataRow dr = USP_Select_FileDetails.Rows.Find(args[1]);
            if (result3 == null)
            {
                var values = new[] { "MSC", "Pacifical", "Margin" };
                if (values.Any(args[3].ToString().Equals))
                {

                    TransStdFileDetails f3 = new TransStdFileDetails();
                    int maxAge = cs.FindMaxValue(list3, t => t.ID);
                    maxAge++;
                    f3.ID = maxAge;
                    f3.Result = args[3].ToString();
                    f3.Name = "";
                    f3.Notes = args[2].ToString();
                    f3.Attached = null;
                    f3.SubID = args[1].ToString();
                    f3.RequestNo = "";
                    list3.Add(f3);
                    licus3.FileDetails = list3;
                }
            }
            else
            {
                list3.Where(c => c.SubID == id.ToString() && c.Result == args[3].ToString())
                    .Select(c => { c.Notes = args[2].ToString(); return c; }).ToList();
                licus3.FileDetails = list3;
            }
            //addFileDetails("", args[2].ToString(), args[3].ToString(), args[1].ToString(), "");
            //}
            //else
            //dr["Notes"] = args[2].ToString();
        }
        if (args[0] == "RefAtt")
        {
            string GeID = String.Format("{0}", hGeID["GeID"]);
            //            int max = Convert.ToInt32(USP_Select_FileDetails.AsEnumerable()
            //.Max(row => row["ID"]));
            //            max = max + 1;
            var RefAtt = hfRefAtt["RefAtt"].ToString().Replace(',', ';').Split(';');
            if (!string.IsNullOrEmpty(RefAtt[0].ToString()))
                foreach (string value in RefAtt)
                {
                    //update 
                    var rcus = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(value));
                    if (rcus != null)
                    {
                        List<TransStdFileDetails> listFileDetails = rcus.FileDetails;
                        TransStdFileDetails result = listFileDetails.Find(x => x.SubID == value && x.Result == "Margin");
                        //DataRow[] xdr = USP_Select_FileDetails.Select(string.Format("SubID={0} and Result='{1}'", value, "Margin"));
                        //if (result.Length > 0)
                        //{
                        //    foreach (DataRow row in xdr)
                        //    {

                        var licus = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(id));
                        if (licus != null)
                        {
                            List<TransStdFileDetails> list2 = licus.FileDetails;
                            TransStdFileDetails f = new TransStdFileDetails();
                            int maxAge = cs.FindMaxValue(list2, t => t.ID);
                            maxAge++;
                            f.ID = maxAge;
                            f.Result = result.Result;
                            f.Name = result.Name;
                            f.Notes = args[2].ToString();
                            f.Attached = result.Attached;
                            f.SubID = id.ToString();
                            f.RequestNo = "";
                            list2.Add(f);
                            licus.Notes = args[2].ToString();
                            licus.Attached = string.Format("{0}", result.Name);
                            licus.FileDetails = list2;
                        }
                    }
                    //    }
                    //}
                }
        }
        if (args[0] == "download")
        {
            //var licus3 = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(id));
            //if (licus3 != null) {
                //downloadfile(id.ToString());
                //List<TransStdFileDetails> listFileDetails = licus3.FileDetails;
                //TransStdFileDetails result = listFileDetails.Find(x => x.SubID == id.ToString() && x.Result == "Margin");
                //ExportToResponse(result.Name, result.Attached, MimeTypes.GetContentType(result.Attached.ToString()),true);
                //Content/UploadControl/d0frlbjx.xls
            //}
        }
        if (args[0] == "comment")
        {
            //    foreach(DataRow dr in USP_Select_FileDetails.Rows)
            //    {
            //        if(dr["Notes"].ToString() == "X" && dr["Name"].ToString() == args[4].ToString())
            //        {
            //            dr["Notes"] = args[2].ToString();
            //            dr["Result"] = args[3].ToString();
            //        }
            //    }
            //    GridView1.DataSource = USP_Select_FileDetails;
            //    GridView1.DataBind();
        }
        if (args[0] == "reload" || args[0] == "Clone")
        {
            listItems = new List<StdItems>();
            string SubId = args[1];
            listItems = buildgv(SubId, args[0]);
            foreach (var item in listItems) {
                buildcalcu(item);
            }
            //+++++++++++
            //tcustomer.PrimaryKey = new DataColumn[] { tcustomer.Columns["ID"] };
            //UpCharge
            if (args[0] == "Clone") {
                BuildAttached("0");
                USP_Select_FileDetails = new DataTable();
                List<TransStdFileDetails> FileDetails = new List<TransStdFileDetails>();
                foreach (var r in listItems)
                {
                    r.FileDetails = FileDetails;
                    r.Notes = "";
                    r.Attached = "";
                }
            } else {
                BuildAttached(args[1]);
            }

            //foreach (DataRow dr in tcustomer.Rows)
            //{
            //    DataTable dt = new DataTable();
            //    dt = cs.builditems(string.Format(@"
            //    select *,'' as Mark from TransTunaStdItems where SubID={0} and RequestNo={1}", args[1].ToString(), dr["ID"]));
            //    string[] arr = { "MinPrice", "Overprice", "Extracost", "subContainers", "OfferPrice" };
            //    foreach (string s in arr)
            //    {
            //        List<string> list = new List<string>();
            //        if (dt.Rows.Count > 0)
            //        {
            //            foreach (DataRow r in dt.Rows)
            //                list.Add(string.IsNullOrEmpty(r[s].ToString()) ? "0" : r[s].ToString());
            //            dr[s] = String.Join("|", list.ToArray());
            //        }
            //    }
            //}

        }
        g.DataBind();
    }
    public void ExportToResponse(string fileName, string Attached, string fileType, bool inline)
    {
        //Response.Clear();
        //string UploadDirectory = "~/Content/UploadControl/";
        string filePath = MapPath(UploadDirectory + Attached.ToString());
        //Response.ContentType = fileType;
        //Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
        //Response.TransmitFile(Server.MapPath(UploadDirectory + Attached.ToString()));
        //Response.End();

        Response.ContentType = fileType;
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
        Response.WriteFile(filePath);
        Response.End();
    }
    void BuildAttached(string Keys)
    {
        SqlParameter[] param = { new SqlParameter("@ID", Keys.ToString()) };
        USP_Select_FileDetails = cs.GetRelatedResources("spGetstdAttached", param);
        USP_Select_FileDetails.PrimaryKey = new DataColumn[] { USP_Select_FileDetails.Columns["ID"] };
        foreach (var r in listItems)
        {
            DataRow[] xdr = USP_Select_FileDetails.Select(string.Format("SubID='{0}'", r.ID.ToString()));
            r.FileDetails = (from DataRow dr in xdr
                             select new TransStdFileDetails()
                             {
                                 ID = Convert.ToInt32(dr["ID"]),
                                 Result = dr["Result"].ToString(),
                                 Name = dr["Name"].ToString(),
                                 Notes = dr["Notes"].ToString(),
                                 Attached = !string.IsNullOrEmpty(dr["Attached"].ToString()) ? (byte[])dr["Attached"] : null,
                                 SubID = dr["SubID"].ToString(),
                                 RequestNo = dr["RequestNo"].ToString()
                             }).ToList();
        }
    }
    //protected void Page_Init(object sender, EventArgs e)
    //{
    //    BuildSelect_FileDetails();
    //}
    void BuildSelect_FileDetails()
    {
        if (USP_Select_FileDetails == null) return;
        if (USP_Select_FileDetails.Rows.Count == 0) return;
        DataRow[] xdr = USP_Select_FileDetails.Select(string.Format("SubID='{0}'", 0));
        if (xdr.Any())
        {
            DataTable xdt1 = xdr.CopyToDataTable();
            gvFiles.DataSource = xdt1;
            gvFiles.DataBind();
            //AddCheckBox(gvFiles, xdt1);

        }
    }
    private void AddCheckBox(GridView g,DataTable xdt1)
    {
        int num = xdt1.Rows.Count;
        for (int i = 0; i < num; i++)
        {
            //    switch (string.Format("{0}", xdt1.Rows[i]["Result"])) {
            //        case "ExchangeRate":
            //            seExchangeRate.ForeColor = Color.Red;
            //            //Control ctrl = LoadControl("seExchangeRate"); 
            //            //ctrl.ForeColor = Color.Red; 
            //            break;
            //        case "Freight":
            //            tbFreight.ForeColor = Color.Red;
            //            break;
            //        case "ValidityDate":
            //            deValidityDate.Border.BorderColor = System.Drawing.Color.Red; 
            //            //ASPxDateEdit date = this.FindControl("startDE") as ASPxDateEdit;
            //            //var dateValue = date.Value;
            //            break;
            //    }
            foreach (GridViewRow row in g.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox cb = new CheckBox();
                    //cb.Checked = true;
                    row.Cells[i].Controls.Add(cb);
                }
            }
        }
    }
    //void BuildUpCharge(string Keys)
    //{
    //    SqlParameter[] param = { new SqlParameter("@SubID", Keys.ToString()),
    //    new SqlParameter("@ID",hGeID["GeID"])};
    //    _UpChargedt = cs.GetRelatedResources("spGetstdUpCharge2", param);
    //    _UpChargedt.PrimaryKey = new DataColumn[] { _UpChargedt.Columns["ID"] };
    //}
    //void BuildUpCharge2(string Keys)
    //{
    //    //foreach(DataRow dr in tcustomer.Rows) { 
    //    //DataRow dr = tcustomer.Rows.Find(Keys);
    //    var obj = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(Keys));
    //    SqlParameter[] param = { new SqlParameter("@ID", Keys.ToString()) };
    //    var _UpChargedt = cs.GetRelatedResources("spGetstdUpCharge", param);
    //    _UpChargedt.PrimaryKey = new DataColumn[] { _UpChargedt.Columns["ID"] };

    //    //string[] columnNames = _UpChargedt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
    //    List<TransUpCharge> uplist = new List<TransUpCharge>();
    //        uplist = (from DataRow dr in _UpChargedt.Rows
    //                                      select new TransUpCharge()
    //                                      {
    //                                          ID = Convert.ToInt32(dr["ID"]),
    //                                          UpCharge = dr["UpCharge"].ToString(),
    //                                          Price = dr["Price"].ToString(),
    //                                          Currency = dr["Currency"].ToString()
    //                                      }).ToList();
    //    obj.Upcharge  = uplist;
    //    //}
    //}
    protected void gv_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //g.KeyFieldName = "ID";
        //g.DataSource = tcustomer;
        //g.ForceDataRowType(typeof(DataRow));
        g.DataSource = listItems;
    }
    List<StdItems> buildgv(string Id,string step)
    {
        SqlParameter[] param = { new SqlParameter("@ID", Id.ToString()) };
        DataTable dt = cs.GetRelatedResources("spGetTunaStdItems2", param);
        List<StdItems> stdList = new List<StdItems>();
        stdList = (from DataRow dr in dt.Rows
                   select new StdItems()
                   {
                       ID = Convert.ToInt32(dr["ID"]),
                       Material = dr["Material"].ToString(),
                       Description = GetDescrip(dr["Material"].ToString()),
                       From = Convert.ToDateTime(dr["From"]),
                       To = Convert.ToDateTime(dr["To"]),
                       RawMaterial = dr["RawMaterial"].ToString(),
                       SubContainers = dr["SubContainers"].ToString(),
                       SecPackaging = dr["SecPackaging"].ToString(),
                       Media = dr["Media"].ToString(),
                       Packaging = dr["Packaging"].ToString(),
                       LOHCost = dr["LOHCost"].ToString(),
                       PackingStyle = dr["PackingStyle"].ToString(),
                       Commission = dr["Commission"].ToString(),
                       OverPrice = dr["OverPrice"].ToString(),
                       OverType = dr["OverType"].ToString(),
                       Pacifical = dr["Pacifical"].ToString(),
                       FishGroup = string.Format("{0}",  dr["FishGroup"]),
                       EUGEN = string.Format("{0}", dr["EUGEN"]),
                       Margin = string.Format("{0}", step == "Clone"?"0":dr["Margin"]),
                       MSC = dr["MSC"].ToString(),
                       MinPrice = Convert.ToDecimal(dr["MinPrice"].ToString() == "" ? "0" : dr["MinPrice"].ToString()).ToString("F4"),
                       OfferPrice = Convert.ToDecimal(dr["OfferPrice"].ToString()).ToString("F4"),
                       RequestNo = dr["RequestNo"].ToString(),
                       Mark = dr["Mark"].ToString(),
                       Notes = dr["Notes"].ToString(),
                       Attached = dr["Attached"].ToString(),
                       Equivalent_Fish_price = dr["Equivalent_Fish_price"].ToString(),
                       Announcement_Fish_price = dr["Announcement_Fish_price"].ToString(),
                       Authorized_price = dr["Authorized_price"].ToString(),
                       Bidprice = string.Format("{0}", step == "Clone" ? "0" : dr["Bidprice"]),
                       marginBid = string.Format("{0}",  "0"),
                       Yield = dr["Yield"].ToString(),
                       FillWeight = dr["FillWeight"].ToString(),
                       PackSize = dr["PackSize"].ToString(),
                       //IsAccept = Convert.ToBoolean(dr["IsAccept"].ToString()==""?1:0),
                       IsAccept = string.Format("{0}", step == "Clone" ? "0" : dr["IsAccept"].ToString()),
                       cal = GetCal(dr["ID"].ToString(), Id, dr["Material"].ToString()),
                       totalUpcharge = string.Format("{0}", dr["Upcharge"].ToString()),
                       //Components = GetComponent(dr["ID"].ToString(), Id),
                       Upcharge = GetUpcharge(dr["ID"].ToString(), Id, dr["Material"].ToString()),
                       Utilize = GetUtilizes(dr["ID"].ToString(), Id),
                       isapprove = dr["Utilize"].ToString(),
                       diff_price = string.Format("{0}",0)
                   }).ToList();
        if (step == "Clone") {
            foreach (var rw in stdList)
                rw.cal = cs.ConvertDataTable<TransCal>(GetCalcu(rw));
        }
        return stdList;
    }
    //List<TransStdComponent> GetComponent(string Keys, String SubId)
    //{
    //    SqlParameter[] param = { new SqlParameter("@ID", Keys.ToString()) };
    //    DataTable dt = cs.GetRelatedResources("spGetStdComponent", param);
    //    dt.PrimaryKey = new DataColumn[] { USP_Select_FileDetails.Columns["ID"] };

    //    return (from DataRow dr in dt.Rows
    //            select new TransStdComponent()
    //            {
    //                ID = Convert.ToInt32(dr["ID"]),
    //                Component = dr["Component"].ToString(),
    //                Result = dr["Result"].ToString(),
    //                Price = dr["Price"].ToString(),
    //                Unit = dr["Unit"].ToString(),
    //                SubID = dr["SubID"].ToString(),
    //                RequestNo = dr["RequestNo"].ToString()
    //            }).ToList();
    //}
    List<TransUtilize> GetUtilizes(string ID, String SubId)
    {
        SqlParameter[] p = { new SqlParameter("@ID", ID.ToString()),
                    new SqlParameter("@RequestNo",String.Format("{0}", SubId))};
        DataTable _utilizedt = cs.GetRelatedResources("spGetUtilize2", p);
        if (_utilizedt.Rows.Count == 0)
            _utilizedt = Getutilize(Convert.ToDateTime(defrom.Value).Date,
                        Convert.ToDateTime(deto.Value).Date);

        return (from DataRow dr in _utilizedt.Rows
                select new TransUtilize()
                {
                    RowID = Convert.ToInt32(dr["RowID"]),
                    Result = dr["Result"].ToString(),
                    MonthName = dr["MonthName"].ToString(),
                    Cost = dr["Cost"].ToString(),
                    SubID = dr["SubID"].ToString(),
                    RequestNo = dr["RequestNo"].ToString()
                }).ToList();
    }
    List<TransUpCharge> GetUpcharge(string SubId,string ID,  string Material)
    {
        //upcharge
        SqlParameter[] u = { new SqlParameter("@SubID", SubId.ToString()),
			        new SqlParameter("@Code",String.Format("{0}", Material)),
                    new SqlParameter("@ID",String.Format("{0}", ID))};
        DataTable _Upcharge = cs.GetRelatedResources("spGetstdUpCharge3", u);
        return (from DataRow dr in _Upcharge.Rows
                select new TransUpCharge()
                {
                    RowID = Convert.ToInt32(dr["ID"]),
                    UpchargeGroup = dr["UpchargeGroup"].ToString(),
                    UpCharge = dr["UpCharge"].ToString(),
                    Price = dr["Price"].ToString(),
                    Quantity = dr["Quantity"].ToString(),
                    Currency = dr["Currency"].ToString(),
                    Result = dr["Result"].ToString(),
                    stdPackSize = dr["stdPackSize"].ToString(),
                    SubID = dr["SubID"].ToString(),
                    RequestNo = dr["RequestNo"].ToString()
                }).ToList();
    }
    List<TransCal> GetCal(string SubId,string ID,  string Material)
    {
        SqlParameter[] param = { new SqlParameter("@RequestNo", string.Format("{0}", ID)),
                new SqlParameter("@SubID", string.Format("{0}", SubId))};
        DataTable table = cs.GetRelatedResources("spGetTunaCal", param);
        return cs.ConvertDataTable<TransCal>(table);
    }
    //private DataTable buildLoadgv
    //{
    //    get
    //    {
    //        if (tcustomer == null)
    //            return tcustomer;
    //        var view = tcustomer.DefaultView;
    //        return tcustomer;
    //    }
    //}
    //protected void gvcal_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
    //{
    //    var values = new[] { "3","9", "12", "13","14" };
    //    ASPxGridView g = sender as ASPxGridView;
    //    if ((DataTable)g.DataSource != null)
    //    {
    //        var dt = (DataTable)g.DataSource;
    //        DataRow row = g.GetDataRow(e.VisibleIndex);
    //        int index = e.VisibleIndex;
    //        var data = g.GetRowValues(index, "OrderIndex").ToString();
    //        if (data.ToString() == "2")
    //            e.Cell.BackColor = Color.LightGreen;
    //        if (values.Any(data.ToString().Equals)){ //(data.ToString() == "3")
    //            e.Cell.BackColor = Color.Gray;
    //            e.Cell.ForeColor = Color.White;
    //            }
    //    }
    //}

    //protected void gvutilize_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
    //    if (string.IsNullOrEmpty(e.Parameters))
    //        return;
    //    var args = e.Parameters.Split('|');
    //    if (args[0] == "Edit")
    //    {
    //        //this.Page.Session.Remove("utilizedt");
    //        if (_utilizedt == null)
    //            load_utilizedt("0");
    //        if (separa != "")
    //        {
    //            DataRow[] dtr = _utilizedt.Select(string.Format("SubID='{0}'", separa));
    //            foreach (var drow in dtr)
    //            {
    //                drow.Delete();
    //            }
    //            _utilizedt.AcceptChanges();
    //            DataTable dataTable2 = Getutilize(Convert.ToDateTime(defrom.Value).Date,
    //                Convert.ToDateTime(deto.Value).Date);
    //            DataRow dr = null;
    //            foreach (DataRow rw in dataTable2.Rows)
    //            {
    //                dr = _utilizedt.NewRow(); // have new row on each iteration
    //                dr = rw;
    //                int NextRowID = Convert.ToInt32(_utilizedt.AsEnumerable()
    //                    .Max(row => row["ID"]));
    //                NextRowID++;
    //                //dr["ID"] = NextRowID;
    //                _utilizedt.Rows.Add(NextRowID, Convert.ToDouble(dr["Result"]), dr["MonthName"], dr["Cost"], dr["SubID"]);
    //            }
    //            //_utilizedt.Merge(dataTable2);
    //        }
    //    }
    //    if (args[0] == "reload")
    //    {
    //        load_utilizedt(args[1].ToString());
    //        //TransUtilize
    //    }
    //    if (_utilizedt != null)
    //    {
    //        DataTable dt = (DataTable)g.DataSource;
    //        //int Index = gv.EditingRowVisibleIndex;
    //        //object keyValue = gv.GetRowValues(Index, "ID");
    //        if (dt != null)
    //        {
    //            //g.Templates.PagerBar = new MyPagerBarTemplate(ActivePageSymbol, max);
    //            g.AutoFilterByColumn(g.Columns["SubID"], separa.ToString());
    //        }
    //        //    int max = Convert.ToInt32(_utilizedt.AsEnumerable()
    //        //.Max(row => row["Formula"]));


    //    }
    //    g.DataBind();
    //}
    //void load_utilizedt(string ID)
    //{
    //    if (ID != "")
    //    {
    //        string stutilize = "";
    //        if (_utilizedt != null)
    //        {
    //            DataRow[] result = _utilizedt.Select(string.Format("SubID='{0}'", ID));
    //            if (result.Length > 0){
    //                stutilize = "X";
    //            }
    //        }
    //        if (stutilize == "")
    //        {
    //            SqlParameter[] param = { new SqlParameter("@ID", ID.ToString()),
    //        new SqlParameter("@RequestNo",String.Format("{0}", hGeID["GeID"]))};
    //            _utilizedt = cs.GetRelatedResources("spGetUtilize2", param);
    //            _utilizedt.PrimaryKey = new DataColumn[] { _utilizedt.Columns["ID"] };
    //            if (_utilizedt.Rows.Count == 0)
    //            {
    //                // load_utilizedt("0");
    //                DataRow[] dtr = _utilizedt.Select(string.Format("SubID='{0}'", ID));
    //                foreach (var drow in dtr)
    //                {
    //                    drow.Delete();
    //                }
    //                _utilizedt.AcceptChanges();
    //                DataTable dataTable2 = Getutilize(Convert.ToDateTime(defrom.Value).Date,
    //                    Convert.ToDateTime(deto.Value).Date);
    //                DataRow dr = null;
    //                foreach (DataRow rw in dataTable2.Rows)
    //                {
    //                    dr = _utilizedt.NewRow(); // have new row on each iteration
    //                    dr = rw;
    //                    int NextRowID = Convert.ToInt32(_utilizedt.AsEnumerable()
    //                        .Max(row => row["RowID"]));
    //                    NextRowID++;
    //                    //dr["ID"] = NextRowID;
    //                    _utilizedt.Rows.Add(NextRowID, Convert.ToDouble(dr["Result"]), dr["MonthName"], dr["Cost"], dr["SubID"]);
    //                }
    //                //_utilizedt.Merge(dataTable2);
    //            }
    //        }
    //    }
    //}
    void DelUtilize(string myfolio)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            //if (_utilizedt == null) return;
            //DataRow dr = _utilizedt.Select(string.Format(@"SubID={0} and RequestNo = {1} and mark='X'",
            //    SubID.ToString(), myfolio.ToString())).FirstOrDefault();
            //if (dr != null)
            //{
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.CommandText = "DELETE TransUtilize WHERE RequestNo = @Id";
            cmd.Parameters.AddWithValue("@Id", myfolio.ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            //}
        }
    }
    DataTable Getutilize(DateTime from, DateTime dtto)
    {
        //DateTime from = Convert.ToDateTime(defrom.Value).Date;
        //DateTime to = Convert.ToDateTime(defrom.Value);
        int monthDiff = cs.GetMonthsBetween(from, Convert.ToDateTime(dtto).Date);
        int monthNumber = Convert.ToInt32(from.ToString("MM", CultureInfo.InvariantCulture));
        int i = 0;
        monthDiff++;
        using (DataTable table = new DataTable())
        {
            table.Columns.Add("RowID", typeof(int));
            table.Columns.Add("Result", typeof(double));
            table.Columns.Add("MonthName", typeof(string));
            table.Columns.Add("Cost", typeof(string));
            table.Columns.Add("SubID", typeof(string));
            table.Columns.Add("RequestNo", typeof(string));
            table.Columns.Add("Mark", typeof(string));
            table.Columns.Add("Calcu", typeof(string));
            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = table.Columns["ID"];
            table.PrimaryKey = keyColumns;
            decimal initial = 100;
            decimal total = GetTotal(Convert.ToInt32( initial), monthDiff);
            while (i < monthDiff)
            {
                i++;
                string monthName = new DateTimeFormatInfo().GetAbbreviatedMonthName(monthNumber);
                if (i == monthDiff && table.Rows.Count>0) {
                    object Result = table.Compute("Sum(Result)", "");
                    table.Rows.Add(i, 100 - Convert.ToDecimal(Result), monthName, "", separa.ToString(), "0");
                } else if (i == monthDiff) {
                    table.Rows.Add(i, 100 , monthName, "", separa.ToString(), "0");
                } else { 
                    table.Rows.Add(i, total <= initial ? total : initial, monthName, "", separa.ToString(), "0");
                }
                monthNumber++;
                if (monthNumber > 12)
                    monthNumber = monthNumber - 12;
                initial = initial - total;
            }

            return table;
        }
    }
    //public void UpdateData(GridDataItem model)
    //{
    //    DataRow dr = _utilizedt.Rows.Find(model.Id);
    //    var values = new[] { "Result" };
    //    foreach (DataColumn column in _utilizedt.Columns)
    //    {
    //        if (values.Any(column.ColumnName.Contains))
    //            dr[column.ColumnName] = model.Result;
    //    }
    //}
    protected void ASPxCallback_Callback(object source, CallbackEventArgs e)
    {
        try
        {
            //UpdateData(JsonConvert.DeserializeObject<GridDataItem>(e.Parameter));
            e.Result = "OK";
        }
        catch (Exception ex)
        {
            e.Result = ex.Message;
        }
    }
    //int GetTotal(int rowTotal, int rowPerPage)
    //{
    //    float pageTotal = 0;
    //    int pTotal = 0;
    //    if (rowTotal % rowPerPage == 0)
    //    {
    //        pTotal = rowTotal / rowPerPage;
    //    }
    //    else
    //    {
    //        pageTotal = rowTotal / rowPerPage;
    //        pTotal = (int)Math.Floor(pageTotal) + 1;
    //    }
    //    return pTotal;
    //}
    decimal GetTotal(int rowTotal, int rowPerPage)
    {
        return rowTotal / rowPerPage;
    }
    protected void gvitems_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        if (args[0] == "ref")
        {
            var dr = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(args[1]));
            _dt = GetCalcu(dr);
            //if(_dt !=null)
            dr.cal = cs.ConvertDataTable<TransCal>(_dt);
            buildcalcu(dr);
        }
        if (args[0] == "clear")
        {
            Page.Session.Remove("setableKey");
            _dt = new DataTable();
        }
        if (args[0] == "Insert")
        {
            insertsGrid(args[1], args[2]);
        }
        if (args[0] == "Attach")
        {
            if (args[1].ToString() == "") return;
            var rcus = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(args[1]));
            List<TransStdFileDetails> FileDetails = rcus.FileDetails;
            int iCount = cs.FindMaxValue(FileDetails, x => x.ID);
            TransStdFileDetails usprw = new TransStdFileDetails();
            iCount++;
            usprw.ID = iCount;
            FileDetails.Add(usprw);
            rcus.FileDetails = FileDetails;
        }
        if (args[0] == "reload")
        {
            Page.Session.Remove("setableKey");
            //load_utilizedt(args[2].ToString());
            separa = args[2].ToString();
            //BuildUpCharge2(args[2]);
            var rcus = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(args[2]));
            //_dt = GetCalcu(Convert.ToInt32(args[2]));
            //SqlParameter[] param = { new SqlParameter("@RequestNo", string.Format("{0}", hGeID["GeID"])),
            //    new SqlParameter("@SubID", string.Format("{0}", rcus.ID))};
            //_dt = cs.GetRelatedResources("spGetTunaCal", param); 
            if(rcus.cal == null) 
                //_dt = GetCalcu(rcus);
                rcus.cal = cs.ConvertDataTable<TransCal>(GetCalcu(rcus));
            buildcalcu(rcus);
            //buildcalcu();
            //CreateGridView();
        }
        if (args[0] == "OverType")
        {
            var value = args[1];
            var rowsToUpdate = _dt.AsEnumerable().Where(r => r.Field<string>("Name") == value).FirstOrDefault();
            if (rowsToUpdate != null)
                rowsToUpdate.SetField("BaseUnit", args[2].ToString());
        }
        if (args[0] == "updated")
        {
            if (_dt != null)
            {
                var value = args[1];
                var rowsToUpdate = _dt.AsEnumerable().Where(r => r.Field<string>("Name") == value).FirstOrDefault();
                if (rowsToUpdate != null)
                {
                    switch (value)
                    {
                        case "OverPrice":
                            rowsToUpdate.SetField("Price", args[2].ToString());
                            break;
                        default:
                            rowsToUpdate.SetField("Quantity", args[2].ToString());
                            break;
                    }
                }
            }
            //if(args[1]== "Margin" || args[1] == "MSC" || args[1] == "Pacifical")
            //{
            //    byte[] FileData = null;
            //    addFileDetails(args[1],"X", args[3].ToString(), FileData);
            //}
        }
        if (args[0] == "FOB")
        {
            if (_dt != null)
            {
                string[] valueType = Regex.Split("Route;Insurance", ";");
                for (int i = 0; i <= valueType.Length - 1; i++)
                {
                    string value = valueType[i].ToString();
                    var rowsToUpdate = _dt.AsEnumerable().Where(r => r.Field<string>("Name") == value);
                    foreach (var _row in rowsToUpdate)
                    {
                        _row.SetField("Quantity", 0);
                        _row.SetField("Result", 0);
                        _row.AcceptChanges();
                    }
                }
            }
        }
        if (args[0] == "symbol")
        {
            ActivePageSymbol = args[1].ToString();

        }
        if (args[0] == "post")
        {
            string Id = String.Format("{0}", hGeID["GeID"]);
            string SubID = args[1].ToString();
            //if (_dtcomponent.Select(string.Format("SubID='{0}'", SubID)).Length == 0)
            //{
            //    int max = Convert.ToInt32(_dtcomponent.AsEnumerable()
            //    .Max(row => row["ID"]));
            //    DataRow[] xdr = _dt.Select("Calcu in (1,2,3,4,5) and Component <> ''");
            //    foreach (DataRow dr in xdr)
            //    {   
            //        string name = dr["Name"].ToString();
            //        string Result = dr["Price"].ToString();
            //        max = max + 1;
            //        _dtcomponent.Rows.Add(new object[] { max, dr["Component"].ToString(),
            //            name, Result,dr["Unit"].ToString(), SubID, Id });
            //        }
            //}
        }
        //if (_dt != null)
        //{
        //    g.AutoFilterByColumn(g.Columns["Calcu"], ActivePageSymbol);
        //}
        //buildcalcu();
        g.Columns.Clear();
        g.AutoGenerateColumns = true;
        g.KeyFieldName = String.Empty;
        g.DataBind();
    }
    //void buildComponent()
    //{
    //        SqlParameter[] param = {
    //                    new SqlParameter("@tablename", "(select top 0 * from TransStdComponent)#a")};
    //    _dtcomponent = cs.GetRelatedResources("spGetElement", param);
    //    _dtcomponent.PrimaryKey = new DataColumn[] { _dtcomponent.Columns["ID"] };

    //}
    const string sessionKey = "CE6907BD-E867-4cbf-97E2-F1EB702F433";
    public string ActivePageSymbol
    {
        get
        {
            if (this.Session[sessionKey] == null)
                this.Session[sessionKey] = string.Format("{0}", 1);
            return (string)this.Session[sessionKey];
        }
        set { this.Session[sessionKey] = value; }
    }



    decimal GetResultdt(DataTable dt, string strSQL)
    {
        decimal result = 0;
        DataRow _row = dt.Select(strSQL).FirstOrDefault();
        if (_row != null)
        {
            result = Convert.ToDecimal(_row["Result"].ToString());
        }
        return result;
    }
    public static double RoundUp(double input, int places)
    {
        double multiplier = Math.Pow(10, Convert.ToDouble(places));
        return Math.Ceiling(input * multiplier) / multiplier;
    }
    DataTable GetCalcu(StdItems rcus)
    {
        //var rcus = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(keyValue));
        //DataRow rcus = tcustomer.Rows.Find(keyValue);
        //if (keyValue != null)
        //    Code = string.Format("{0}", keyValue);
        int Id = Convert.ToInt32(hGeID["GeID"]);
        SqlParameter[] param = { new SqlParameter("@Param", Id.ToString()),
        new SqlParameter("@Code", rcus.Material.ToString())};
        var dt = cs.GetRelatedResources("spGetTunaCalculation", param);
        dt.PrimaryKey = new DataColumn[] { dt.Columns["RowID"] };
        string _Currency = "";
        decimal _seExchangeRate=0;
        if (seExchangeRate.Value == null)
        {
            foreach (DataRow _rwx in exchangerate().Rows)
                _seExchangeRate = Convert.ToDecimal(_rwx["Rate"].ToString());
        }
        else
            _seExchangeRate = Convert.ToDecimal(seExchangeRate.Value);
        using (DataTable table = new DataTable())
        {
            //try
            //{
            table.Columns.Add("RowID", typeof(int));
            table.Columns.Add("Component", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Currency", typeof(string));
            table.Columns.Add("Result", typeof(double));
            table.Columns.Add("Calcu", typeof(int));
            table.Columns.Add("Quantity", typeof(string));
            table.Columns.Add("Price", typeof(string));
            table.Columns.Add("Unit", typeof(string));
            table.Columns.Add("BaseUnit", typeof(string));
            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = table.Columns["RowID"];
            table.PrimaryKey = keyColumns;
            int i = 0;
            object totalfillweight = 0;
            decimal _Yield = 0, _FillWeight = 0, _sumFillWeight = 0;
            string GroupStyle = "", Grading = "";
            string stdPackSize = dt.Compute("Max(Result)", "OrderIndex = 9").ToString();
            string _pkgSize = dt.Compute("Max(Result)", "OrderIndex = 14").ToString();
            DataTable _dtFillWeight = cs.GetFillWeight(rcus.Material.ToString());
            rcus.PackSize = stdPackSize;
            if (_dtFillWeight.Rows.Count == 0) return table;
            if (rcus.Utilize != null)
            {
                decimal totlpertuni = 0, _sumraw = 0;
                string stdprice = "";
                foreach (DataRow _rw in _dtFillWeight.Rows)
                {
                    decimal _culcu = 0;
                    GroupStyle = _rw["SHD"].ToString();
                    Grading = _rw["Grading"].ToString();
                    List<TransUtilize> strutilize = rcus.Utilize;
                    //int[] arr = strutilize.Split('|').Select(Int32.Parse).ToArray();
                    string _sumuti = strutilize.Count.ToString();
                    //string _sumuti = _utilizedt.Compute("Sum(Result)", "").ToString();
                    _FillWeight = Convert.ToDecimal(_rw["Result"].ToString());
                    _Yield = Convert.ToDecimal(_rw["Yield"].ToString()) / 100;
                    totlpertuni = 0;
                    //DataTable dataTable2 = Getutilize(Convert.ToDateTime(defrom.Value).Date,
                    //    Convert.ToDateTime(deto.Value).Date);
                    int index = 0;
                    //foreach (DataRow _r in dataTable2.Rows)
                    foreach (var _r in strutilize)
                    {
                        //object From = gv.GetRowValues(Index, "From");
                        //object To = gv.GetRowValues(Index, "To");
                        object From = defrom.Value;
                        object To = deto.Value;
                        var args = cs.GetAvgFishPrice(rcus.Material.ToString(), _r.MonthName, _rw["FishGroup"], _rw["FishCert"], _rw["SHD"],
                            Convert.ToDateTime(From), Convert.ToDateTime(To), rcus.ID.ToString()).Split('|');
                        if (args[0].ToString() == "") goto JumpEx;
                        if (args.Length > 1)
                        {
                            stdprice = args[0].ToString();
                            _r.Cost = args[0].ToString();
                        }
                        if (!string.IsNullOrEmpty(_r.Result.ToString()) && _sumuti != "0")
                        {
                            //decimal _value = Convert.ToDecimal(stdprice) * ((Convert.ToDecimal(_r.Result.ToString())/100) / Convert.ToDecimal(_sumuti));
                            decimal _value = Convert.ToDecimal(stdprice) * (Convert.ToDecimal(_r.Result.ToString()) / 100);
                            stdprice = _value.ToString("F4");
                            totlpertuni += _value;
                            //totlpertuni += Convert.ToDecimal(_r["Result"].ToString());
                        }
                        _culcu += Convert.ToDecimal(_FillWeight / 1000 / _Yield * Convert.ToDecimal(stdprice) / 1000 * Convert.ToDecimal(stdPackSize));
                        _Currency = string.Format("{0}",args[2]);
                        index++;
                    }
                    _sumraw += _culcu;
                    _sumFillWeight += _FillWeight;
                    table.Rows.Add(i++, "RM", string.Format("{0} {1} {2}", _rw["Name"], _rw["FishCert"], _rw["SHD"]), _rw["Result"], _culcu.ToString("F4"), 1,
                        _Yield, totlpertuni.ToString("F"), _Currency + "/Case","");
                    table.Rows.Add(i++, "Raw material", string.Format("{0} {1}", _rw["Name"], _rw["FishCert"]), _Currency + "/Case", _culcu.ToString("F4"), 8,
                        _FillWeight.ToString("F"), totlpertuni.ToString("F"), _Yield,"");
                    rcus.FillWeight = _FillWeight.ToString("F");
                    rcus.Yield = _Yield.ToString("F");
                    rcus.RawMaterial = totlpertuni.ToString("F");
                }
                string name = string.Format(@"Cost Per Case ({0})", stdPackSize);
                totalfillweight = _dtFillWeight.Compute("Sum(Result)", "");//calculator raw material fillweight
                //totalfillweight = Convert.ToDecimal(totalfillweight) / Convert.ToDecimal(_dtFillWeight.Rows.Count);
                table.Rows.Add(i++, "", name, "", _sumraw.ToString("F4"), 1, "", "", _Currency + "/Case","");
                //table.Rows.Add(i++, "", "Raw material", _Currency + "/Case", _sumraw.ToString("F4"), 8, _sumFillWeight.ToString("F4"), totlpertuni.ToString("F4"), "");
            }
            //Media(ing)
            decimal _summedia = 0, _sumoilcase = 0;
            foreach (DataRow _rw in cs.GetMedia(rcus.Material.ToString(), "Media", Convert.ToDateTime(defrom.Value),
                Convert.ToDateTime(deto.Value)).Rows)
            {
                decimal _oilloss = Getloss(_rw["GroupDescription"].ToString());
                //Decimal convertunit = _rw["Unitofmeasurement"].ToString() == "Case" ? 1 : 1000;
                decimal _sumoil = 0;
                if (_rw["Unitofmeasurement"].ToString() == "Case") {
                    _sumoil = Convert.ToDecimal(_rw["Result"].ToString());
                    _sumoilcase += _sumoil;
                }
                else
                {
                    _sumoil = Convert.ToDecimal(_rw["Price"].ToString()) / 1000 * Convert.ToDecimal(_rw["MediaWeight"].ToString()) * (1 + _oilloss / 100);
                    _summedia += _sumoil;
                }
                //decimal _sumoil = Convert.ToDecimal(_rw["Price"].ToString()) * Convert.ToDecimal(_rw["MediaWeight"].ToString()) * (1 + _oilloss / 100);
                table.Rows.Add(i++, string.Format("{0}", _rw["GroupDescription"]), _rw["CodeName"], _oilloss.ToString(), _sumoil.ToString("F4"), 2,
                    _rw["MediaWeight"].ToString(), _rw["Price"].ToString(),
                    _rw["Currency"] + "/" + _rw["Unitofmeasurement"],
                    string.Format("{0}/{1}",
                    _rw["Currency"].ToString(), _rw["Unitofmeasurement"].ToString() == "Case" ? "Case" : "Can"));
                _Currency = string.Format("{0}",_rw["Currency"]);

            }
            decimal CalCurrency = _Currency == "USD" ? 1 : Convert.ToDecimal(_seExchangeRate);
            decimal _media = Convert.ToDecimal(_summedia * Convert.ToDecimal(stdPackSize) / CalCurrency) + _sumoilcase;
            table.Rows.Add(i++, "Media", string.Format("Media Cost Per Case ({0})", stdPackSize), "", _media.ToString("F4"), 2, "", "", "", "USD/Case");
            table.Rows.Add(i++, "", "Media", "USD/Case", _media.ToString("F4"), 8, "", "", "", "");
            rcus.Media = _media.ToString("F4");
            decimal _primary = 0;
            foreach (DataRow _rw in cs.GetMedia(rcus.Material.ToString(), "PKG"
                , Convert.ToDateTime(defrom.Value),
                Convert.ToDateTime(deto.Value)).Rows)
            {
                DataRow _result = dt.Select("OrderIndex = '6'").FirstOrDefault();
                //decimal _primary = Convert.ToDecimal(_rw["Price"]) * (1 + (Convert.ToDecimal(_result["Result"])/100)) * (Convert.ToDecimal(stdPackSize) / Convert.ToDecimal(seExchangeRate.Value));
                decimal _priloss = Getloss(_rw["GroupDescription"].ToString());
                decimal _sumpri = Convert.ToDecimal(_rw["Price"]) * (1 + _priloss / 100);
                table.Rows.Add(i++, _rw["GroupDescription"], _rw["Description"], _rw["Currency"] + "/" + _rw["Unit"],
                    _sumpri.ToString("F4"), 3, _priloss.ToString(), _rw["Price"], 0,
                    _rw["Currency"] + "/" + _rw["Unitofmeasurement"]);
                _primary += _sumpri;
            }

            decimal _Packaging = Convert.ToDecimal(_primary * Convert.ToDecimal(stdPackSize) / Convert.ToDecimal(_seExchangeRate));
            table.Rows.Add(i++, "Packaging", string.Format("Primary Packaging per case ({0})", stdPackSize), "",
                String.Format("{0:0.00}", _Packaging.ToString("F4")), 3, "", "", "", "USD/Case");
            table.Rows.Add(i++, "", "Primary Packaging", "USD/Case", _Packaging.ToString("F4"), 8, "", "", "", "");
            rcus.Packaging = _Packaging.ToString("F4");
            decimal _sumLOHCost = 0;
            foreach (DataRow _rw in cs.GetMedia(rcus.Material.ToString(), "Labor", Convert.ToDateTime(defrom.Value),
                Convert.ToDateTime(deto.Value)).Rows)
            {
                if (_rw["Title"].ToString() == "Laborcost")
                {
                    decimal _Labor = Convert.ToDecimal(_rw["Cost"]) * Convert.ToDecimal(totalfillweight) * Convert.ToDecimal(stdPackSize);
                    table.Rows.Add(i++, "DL",
                        string.Format("{0}  by {1}", _rw["Title"], Grading.ToString()),
                        _rw["Currency"] + "/" + _rw["Unit"],
                        _Labor.ToString("F5"), 4, totalfillweight, Convert.ToDecimal(_rw["Cost"]).ToString("F5"), 0,
                        _rw["Currency"] + "/Case");
                    _sumLOHCost += _Labor;
                }
                else if (_rw["Title"].ToString() == "Cold storage")
                {
                    decimal _ColdLabor = 0;
                    if (GroupStyle == "SHD")
                        _ColdLabor = ((Convert.ToDecimal(totalfillweight) / 1000) / Convert.ToDecimal(0.42)) * Convert.ToDecimal(stdPackSize);
                    else
                        _ColdLabor = ((Convert.ToDecimal(totalfillweight) / 1000) / _Yield) * Convert.ToDecimal(stdPackSize);
                    decimal _resultcold = _ColdLabor * Convert.ToDecimal(_rw["Cost"]);
                    table.Rows.Add(i++, "OOH",
                        _rw["Title"].ToString(),
                        _rw["Currency"] + "/" + _rw["Unit"],
                        _resultcold.ToString("F4"), 4, _ColdLabor.ToString("F4"), Convert.ToDecimal(_rw["Cost"]), 0,
                        _rw["Currency"] + "/Case");
                    _sumLOHCost += _resultcold;
                }
                else
                {
                    //DataRow _rwx = GetMedia(Code.ToString(), "Style").Select("").FirstOrDefault();
                    decimal _LOHCost = Convert.ToDecimal(_rw["Cost"]) / Convert.ToDecimal(_rw["StdPackSize"]) * Convert.ToDecimal(stdPackSize);
                    table.Rows.Add(i++, _rw["Title"], string.Format("{0} {2} {3} pack size {1}", _rw["Title"], stdPackSize, _rw["PackagingType"], _rw["Size"]),
                        _rw["Currency"] + "/" + _rw["Unit"], _LOHCost.ToString(), 4, "", _rw["Cost"], "", _rw["Currency"] + "/Case");
                    _sumLOHCost += _LOHCost;
                }
            }
            decimal _LOH = Convert.ToDecimal(_sumLOHCost / Convert.ToDecimal(_seExchangeRate));
            if (stdPackSize != _pkgSize)
            {
                decimal _additional = Convert.ToDecimal("0.15");
                _LOH += _additional;
                table.Rows.Add(i++, "FOH", "Additional cost", "", _additional.ToString("F4"), 4, "", "", "", "USD/Case");
            }
            table.Rows.Add(i++, "LOH", string.Format("LOH per pack {0}", stdPackSize), "", _LOH.ToString("F4"), 4, "", "", "", "USD/Case");
            table.Rows.Add(i++, "", string.Format("LOH per pack {0}", stdPackSize), "USD/Case", _LOH.ToString("F4"), 8, "", "", "", "");
            rcus.LOHCost = _LOH.ToString("F4");
            object sumObject = 0;
            //sumObject = table.Compute("Sum(Result)", "Component in ('LOH','Media','Packaging')");
            //table.Rows.Add(i++, "Total Price", string.Format("16 - digit price per pack {0}", stdPackSize), "USD/Case", sumObject, 8, 0, 0, 0);

            //Packing Style
            decimal _totalPrice = 0; decimal _totalStyle = 0; decimal _PackSize = 0; decimal _Margin = 0; decimal _sumprice = 0;
            string _UnitStyle = "";
            foreach (DataRow _rw in cs.GetMedia(rcus.Material.ToString(), "Style2", Convert.ToDateTime(defrom.Value),Convert.ToDateTime(deto.Value)).Rows)
            {
                //table.Rows.Add(i++, "","Pack size","", _rw["PackSize"], 5, stdPackSize, 0);
                //table.Rows.Add(i++, "","Packing LOH cost per case", "", _rw["LaborCost"],5, stdPackSize, 0);
                //table.Rows.Add(i++, "","Secondary Packaging cost per case", "", _rw["SecPKGCost"], 5, stdPackSize, 0);
                decimal LaborCost;
                decimal.TryParse(_rw["LaborCost"].ToString(), out LaborCost);
                decimal _sumStyle = LaborCost;
                //_totalStyle = _sumStyle / Convert.ToDecimal(seExchangeRate.Value);
                _totalStyle += _sumStyle;
                table.Rows.Add(i++, rcus.Material.Substring(16, 2), _rw["name"], _rw["PackSize"], _sumStyle.ToString("F4"), 5, LaborCost, _sumStyle, "USD/" + _rw["Unit"],"USD");
                _PackSize = Convert.ToDecimal(_rw["PackSize"]);
                _UnitStyle = string.Format("{0}", _rw["Unit"]);
                //table.Rows.Add(i++, "Cansize", _rw["Size"],"",0, 5, 0, 0, 0);
                //table.Rows.Add(i++,"Packing LOH cost per case", _rw["LaborCost"],"",0, 5, 0, 0, _rw["PackSize"]);
                //table.Rows.Add(i++,"Secondary Packaging cost per case", _rw["PackSize"],"",0, 5, 0, 0, _rw["PackSize"]);
                //table.Rows.Add(i++, "Packing Style", string.Format("Pack size {0}", _rw["PackSize"]), "", _totalStyle.ToString("F4"), 5, "", "", "USD/" + _rw["Unit"]);
                }
                table.Rows.Add(i++, "", "Packing Style", "USD/" + _UnitStyle, _totalStyle.ToString("F4"), 8, "", "", "", "");
                rcus.PackingStyle = _totalStyle.ToString("F4");
                decimal _sumSecPKGCost = 0;
                foreach (DataRow _xrw in GetSecPKGCost(rcus.Material.ToString()).Rows)
                {
                    //string _UnitUpcharge = _xrw["Currency"].ToString() + "/" + _xrw["Unit"].ToString();
                    //string _UnitSecPKGCost = "THB/Case";
                    decimal _SecAmount = Convert.ToDecimal(Convert.ToDecimal(_xrw["Amount"]) / Convert.ToDecimal(_seExchangeRate));
                    table.Rows.Add(i++, "Packaging", "Secondary Packaging per case", _PackSize, _SecAmount.ToString("F4"), 5, "", Convert.ToDecimal(_xrw["Amount"]).ToString("F4"),
                        "USD/Case", "THB");
                    //"USD/" + _xrw["Unit"].ToString());
                    _sumSecPKGCost += _SecAmount;
                }
                decimal _totalSecPKGCost = Convert.ToDecimal(_sumSecPKGCost);
                table.Rows.Add(i++, "", "Secondary Packaging", "USD/Case", _totalSecPKGCost.ToString("F4"), 8, "", "", "", "");
                rcus.SecPackaging = _totalSecPKGCost.ToString("F4");
                //sumObject = table.Compute("Sum(Result)", "Component='Total Price'");
                //upcharge
                decimal _totupcharge = 0;//, _Quantityupch = 0, _Priceupch = 0;
                List<TransUpCharge> listUp = rcus.Upcharge;
                if (listUp != null)
                {
                    //DataRow[] result = _UpChargedt.Select(string.Format("SubID='{0}'", SubId));
                    foreach (var _row in listUp)
                    {
                        //if (hGeID["GeID"].ToString() == "0") { 
                        table.Rows.Add(i++, _row.UpchargeGroup,
                            _row.UpCharge,
                            string.Format("{0}", _row.Currency),
                            _row.Result, 6,
                            _row.Quantity,
                            _row.Price,
                             _row.stdPackSize, _PackSize);
                        //}
                        //else { 
                        //table.Rows.Add(_row.ID, _row.UpchargeGroup,
                        //    _row.UpCharge,
                        //    _row.Currency,
                        //    _row.Result,6,
                        //    _row.stdPackSize,
                        //    _row.Price,
                        //    _row.Quantity,_PackSize);
                        //}
                        //_Priceupch += Convert.ToDecimal(_row.Price.ToString());
                        //_Quantityupch += Convert.ToDecimal(_row.Quantity.ToString());
                        _totupcharge += Convert.ToDecimal(_row.Result);
                    }
                }
                else
                {
                    //upcharge
                    //foreach (DataRow _rwup in cs.GetMedia(rcus.Material.ToString(), "upcharge", Convert.ToDateTime(defrom.Value), Convert.ToDateTime(deto.Value)).Rows)
                    foreach(var _rwup in rcus.Upcharge)
                    {
                        double resuftup = Convert.ToDouble(_rwup.Result); //((Convert.ToDouble(_rwup["Value"]) / Convert.ToDouble(_rwup["StdPackSize"])) * Convert.ToDouble(_PackSize));
                        //table.Rows.Add(i++,"UpCharge", _rwup["UpCharge"], _rwup["Currency"], _rwup["Value"], 6, 1,
                        table.Rows.Add(i++, "UpCharge", _rwup.UpCharge, _rwup.Currency, _rwup.Price, 6, 1,
                            resuftup.ToString(), _PackSize);
                        _totupcharge += Convert.ToDecimal(resuftup);
                    }
                }
                table.Rows.Add(i++, "", "Upcharge", "USD/Case", _totupcharge.ToString("F4"), 8, "", "", "", "");
                rcus.totalUpcharge = _totupcharge.ToString("F4");
                sumObject = table.Compute("Sum(Result)", "Component in ('LOH','Media','Packaging')");
                _sumprice = Convert.ToDecimal(sumObject);
                if (sumObject != System.DBNull.Value)
                {
                    //foreach (DataRow _rwx in GetMedia(Code.ToString(), "Margin").Rows)
                    //{
                    //    _Margin = Convert.ToDecimal(_rwx["Margin"]);
                    //    decimal _totalmargin = _sumprice * (Convert.ToDecimal(_rwx["Margin"]) / 100);
                    //    table.Rows.Add(800, "", "Margin", "USD/Case", _totalmargin.ToString("F4"), 8, _Margin, 0, 0);
                    //}
                    table.Rows.Add(800, "", "Margin", "USD/Case", 0, 8, _Margin, 0, 0, "");
                }


            //Route 
            double _SubContainers = 0, sumFreight = 0;
            double Freight = 0;
            //object SubContainers = string.Format("{0}", rcus.SubContainers.ToString());
            //if (rcus.SubContainers != null)

            if (double.TryParse(string.Format("{0}", rcus.SubContainers), out _SubContainers))
            {
                double.TryParse(tbFreight.Text, out Freight);
                sumFreight = (Convert.ToDouble(Freight) / Convert.ToDouble(_SubContainers));
                table.Rows.Add(i++, "Route", CmbRoute.Text, "USD/Case",
                    sumFreight.ToString("F4"), 7, _SubContainers, Freight, 0, "");
                //_row.SetField("Result", sumFreight.ToString("F4"));
            }
            else
            {
                table.Rows.Add(i++, "Route", CmbRoute.Text, "USD/Case",
                        sumFreight.ToString("F4"), 7, _SubContainers, Freight, 0, "");
            }
            //table.Rows.Add(i++, "", "Upcharge1(Packing style per can)", "", 0, 8, 0, 0);
            //table.Rows.Add(i++, "", "Upcharge2(Packing style per case)", "", 0, 8, 0, 0);

            int summary = 802;
            //sumObject = table.Compute("Sum(Result)", "Name in ('Margin', 'Secondary Packaging')");
            if (sumObject != System.DBNull.Value)
            {
                _totalPrice = ((_sumprice / Convert.ToDecimal(stdPackSize) * Convert.ToDecimal(_PackSize)) + _totalStyle) * (1 + Convert.ToDecimal(_Margin) / 100);
                table.Rows.Add(summary++, "", "FOB price(18 - digits)", "USD/Case", Convert.ToDecimal(_totalPrice).ToString("F4"), 8, 0, 0, 0, "");
            }

            //table.Rows.Add(i++, "", "Margin", "", 0, 10, 0, 0, 0);
            //table.Rows.Add(i++, "", "FOB price", "", 0, 11, 0, 0, 0);
            string _currency = "USD/pack size";
            table.Rows.Add(summary++, "", "Value Commission", _currency, 0, 8, 0, 0, 0, "");
            table.Rows.Add(summary++, "", "OverPrice", _currency, 0, 8, 0, 0, 0, "");
            table.Rows.Add(summary++, "", "FOB Price", _currency, 0, 8, 0, 0, 0, "");

            //table.Rows.Add(i++, "", "Payment Term", "", 0, 16, 0, 0, 0);
            //table.Rows.Add(i++, "", "FOB Price 2", "", 0, 17, 0, 0, 0);

            table.Rows.Add(summary++, "", "Route", "", 0, 8, 0, 0, 0, "");
            table.Rows.Add(summary++, "", "Insurance", "", 0, 8, 0, 0, 0, "");
            table.Rows.Add(summary++, "", "Interest", "", 0, 8, 0, 0, 0, "");
            table.Rows.Add(summary++, "", "CIF Price", _currency, 0, 8, 0, 0, 0, "");

            //table.Rows.Add(summary++, "", "Pacifical", "", 0, 8, 0, 0, 0);
            //table.Rows.Add(summary++, "", "MSC", "", 0, 8, 0, 0, 0);

            table.Rows.Add(summary++, "", "MSC&Pacifical", "", 0, 8, 0, 0, 0, "");
            table.Rows.Add(summary++, "", "Offer Price", _currency, 0, 8, 0, 0, 0, "");
            table.Rows.Add(summary++, "", "Equivalent fish price", "USD/TON", 0, 8, 0, 0, 0, "");


                    string[] valueType = Regex.Split("Value Commission;OverPrice;Margin;Upcharge;Route", ";");
                    for (int ii = 0; ii <= valueType.Length - 1; ii++)
                    {
                        string value = valueType[ii].ToString();
                        var rowsToUpdate = table.AsEnumerable().Where(r => r.Field<string>("Name") == value);
                        //DataRow []res = table.Select(string.Format("Name='{0}'", valueType[ii]));
                        foreach (var _row in rowsToUpdate)
                        {
                            switch (value)
                            {
                                //case "Route":
                                //    double _SubContainers = 0, sumFreight = 0;
                                //    double Freight = 0;
                                //    if (double.TryParse(dr["SubContainers"].ToString(), out _SubContainers))
                                //    {
                                //        double.TryParse(tbFreight.Text, out Freight);
                                //        sumFreight = (Convert.ToDouble(Freight) / Convert.ToDouble(_SubContainers));
                                //        //table.Rows.Add(i++, "Route", CmbRoute.Text, "USD/Case",
                                //        //    sumFreight.ToString("F4"), 7, _SubContainers, Freight, 0);
                                //        //_row.SetField("Result", sumFreight.ToString("F4"));
                                //    }
                                //    break;
                                case "Margin":
                                    if (decimal.TryParse(rcus.Margin.ToString(), out _Margin))
                                    {
                                        //decimal _totalmargin = _sumprice * (_Margin / 100);
                                        //table.Rows.Add(800, "", "Margin", "USD/Case", _totalmargin.ToString("F4"), 8, _Margin, 0, 0);
                                        //_row.SetField("Result" , _totalmargin.ToString("F4"));
                                        _row.SetField("Quantity", _Margin.ToString("F4"));
                                    }
                                    break;
                                case "OverPrice":
                                    _row.SetField("Price", rcus.OverPrice.ToString());
                                    _row.SetField("BaseUnit", rcus.OverType.ToString());
                                    break;
                                //case "MSC":
                                //    _row.SetField("Quantity", dr.MSC.ToString());
                                //    break;
                                //case "Pacifical":
                                //    _row.SetField("Quantity", dr.Pacifical.ToString());
                                //    break;
                                case "Commission":
                                case "Value Commission":
                                    _row.SetField("Quantity", rcus.Commission.ToString());
                                    break;
                                case "Upcharge":
                                    //DataRow[] dtUpCharge = _UpChargedt.Select(string.Format("SubID='{0}'", SubId));
                                    //foreach (DataRow _rw in dtUpCharge)
                                    //{
                                    //    table.Rows.Add(_rw["ID"],
                                    //        _rw["UpchargeGroup"],
                                    //        _rw["Upcharge"],
                                    //        _rw["Currency"],
                                    //        _rw["Result"],
                                    //        6,
                                    //        _rw["Quantity"],
                                    //        _rw["Price"],
                                    //        _rw["StdPackSize"], GetPackSize(table).ToString());
                                    //}
                                    break;
                                case "Route":
                                    //++++++++++++++++++++++++++++++++++
                                    double NumberContainer;
                                    //var text = SubContainers.ToString();
                                    if (Double.TryParse(rcus.SubContainers, out NumberContainer))
                                    {
                                        double interest = 0, XFreight = 0, Insurance = 0;
                                        double XsumFreight = 0, minprice = 0;
                                        object objminprice = table.Compute("Sum(Result)", "Name in ('FOB price(18 - digits)','Value Commission') and Calcu=8");
                                        if (!string.IsNullOrEmpty(tbFreight.Text) || !string.IsNullOrEmpty(rcus.SubContainers))
                                            double.TryParse(tbFreight.Text, out XFreight);
                                        double.TryParse(tbinterest.Text, out interest);
                                        XsumFreight = (Convert.ToDouble(XFreight) / Convert.ToDouble(NumberContainer));
                                        double A = 0, B = 0, interrest = 0;
                                        object _commission = table.Compute("Sum(Result)", "Name in ('Value Commission') and Calcu=8");
                                        double.TryParse(objminprice.ToString(), out minprice);
                                        double.TryParse(tbinterest.Text, out interest);
                                        interrest = (Convert.ToDouble(minprice) + Convert.ToDouble(_commission) + XsumFreight) * (Convert.ToDouble(interest) / 100);
                                        A = Convert.ToDouble(minprice) + XsumFreight + interrest;
                                        if (double.TryParse(tbInsurance.Text, out Insurance))
                                            B = Convert.ToDouble(string.Format("{0:#,##0.####}", A * (Insurance == 0 ? 0 : Convert.ToDouble(0.0011))));
                                        //B = Convert.ToDouble(string.Format("{0:#,##0.####}", A * (_Insurance==0?0:Convert.ToDouble(0.5/100))));
                                        //OfferPrice = minprice + totalCount + interrest + XsumFreight + Convert.ToDouble(B);
                                        DataRow _result = table.Select("Component = 'Route'").FirstOrDefault();
                                        if (_result != null)
                                        {
                                            _result["Quantity"] = NumberContainer;
                                            _result["Result"] = XsumFreight.ToString("F4");
                                        }
                                        //var rowsToUpdate = _dt.AsEnumerable().Where(r => r.Field<string>("Component") == "Route").FirstOrDefault();
                                        //if (rowsToUpdate == null)
                                        //{
                                        //    _dt.Rows.Add(NextRowID++, "Route", CmbRoute.Text, "", XsumFreight.ToString("F4"), 7, NumberContainer, Freight, 0, "USD/Case");
                                        //}
                                        //else
                                        //{
                                        //    rowsToUpdate.SetField("Quantity", NumberContainer.ToString("F4"));
                                        //}
                                        DataRow[] _dr = table.Select("Name in ('Route','Insurance') and Calcu in (8,9)");
                                        foreach (DataRow _r in _dr)
                                        {
                                            switch (_r["Name"].ToString())
                                            {
                                                case "Route":
                                                    _r["Result"] = XsumFreight.ToString("F4");
                                                    _r.SetField("Quantity", NumberContainer.ToString("F4"));
                                                    _r.SetField("Price", XFreight.ToString("F4"));
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            _row.AcceptChanges();
                        }
                    }
 
            //Converter
            DataTable dt_clone = new DataTable();
            //dt.TableName = "CloneTable";
            dt_clone = table.Clone();
            DataRow[] _drarr = table.Select(string.Format("Calcu='{0}'", 8));
            int conv_int = 900;
            string[] values = Regex.Split("Equivalent fish price", ";");
            foreach (DataRow _dr in _drarr)
            {
                if (!values.Any(_dr["Name"].ToString().Equals))
                {
                    conv_int++;
                    DataRow newrow = dt_clone.NewRow();
                    newrow.ItemArray = _dr.ItemArray;
                    newrow[0] = conv_int;
                    newrow["Calcu"] = 9;
                    if (_dr["Name"].ToString() == "Margin")
                    {
                        string[] sarr = Regex.Split("Result;Quantity;Price", ";");
                        for (int j = 0; j <= sarr.Length - 1; j++)
                        {
                            newrow[sarr[j]] = 0;
                        }
                    }
                    dt_clone.Rows.Add(newrow);
                    //dt_clone.Rows.Add(conv_int, "", string.Format("{0}", _dr["Name"]),"", _dr["Result"],9,"", 
                    //        "", "", "");
                }
            }
            table.Merge(dt_clone);
            conv_int++;
            table.Rows.Add(conv_int++, "", "Equivalent margin", "", 0, 9, 0, 0, 0, "");
        //}
        //catch (Exception e)
        //{
        //    Console.WriteLine(e.Message);
        //}
        JumpEx:
            return table;
        }
    }
    DataTable GetSecPKGCost(string value)
    {

        SqlParameter[] param = {
            new SqlParameter("@Material", string.Format("{0}",value)),
        new SqlParameter("@Customer", string.Format("{0}", CmbCustomer.Value)),
        new SqlParameter("@toDate", string.Format("{0}", deto.Value)),
        new SqlParameter("@ShipTo", string.Format("{0}",CmbShipTo.Value))};
        var Results = cs.GetRelatedResources("spStdSecPKGCost2", param);

        return Results;
    }
    decimal Getloss(string value)
    {
        SqlParameter[] param = { new SqlParameter("@table",
           string.Format("(select * from StdLossPrimaryPKG where LossType='{0}')#a",value)) };
        var Results = cs.GetRelatedResources("usp_query", param);
        if (Results.Rows.Count == 0) return 0;
        DataRow row = Results.Rows[0];
        return Convert.ToDecimal(row["Loss1stPKG"]);
    }
    //double GetSumutilize()
    //{
    //    object sumObject;
    //    if (_utilizedt == null) return 0;
    //    sumObject = _utilizedt.Compute("Sum(Result)", string.Empty);
    //    return (double)sumObject;
    //}
    protected void cpRefAtt_Callback(object sender, CallbackEventArgsBase e)
    {
        string[] arg = e.Parameter.Split('|');
        switch (arg[0])
        {
            case "reload":
                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(int));
                //table.Columns.Add("Material", typeof(string));
                table.Columns.Add("Description", typeof(string));
                DataColumn[] keyColumns = new DataColumn[1];
                keyColumns[0] = table.Columns["ID"];
                table.PrimaryKey = keyColumns;
                foreach (var t in listItems)
                {
                    if (!string.IsNullOrEmpty(t.Notes))
                        table.Rows.Add(t.ID, t.Material + " " + t.Description);
                }

                cbAtt.DataSource = table;
                cbAtt.DataBind();
                cbAtt.SelectedIndex = -1;
                break;
        }
    }
    protected void gvitems_DataBinding(object sender, EventArgs e)
    {
        //try
        //{
            ASPxGridView g = sender as ASPxGridView;
        //    g.KeyFieldName = "RowID";
        //    g.DataSource = ReCreateColumns;
        //    g.ForceDataRowType(typeof(DataRow));
        //}
        //catch (Exception ex)
        //{
        //    throw ex;
        //}
        if (GetIndex >= 0)
        {
            ReCreateColumns();
            GetCurrentTable(g);
        }
    }
    DataTable buildcolumnTable()
    {
        DataTable dt = new DataTable();
        dt.Clear();
        dt.Columns.Add("Name");
        dt.Columns.Add("Marks");
        dt.Rows.Add(new object[] { "MonthName;Result;Cost;Calcu", 0 });
        dt.Rows.Add(new object[] { "Component;Name;Currency;Quantity;Price;Result;Unit;Calcu", 1 });
        dt.Rows.Add(new object[] { "Component;Name;Currency;Quantity;Price;Unit;Result;BaseUnit;Calcu", 2 });
        dt.Rows.Add(new object[] { "Component;Name;Quantity;Price;Currency;Result;BaseUnit;Calcu", 3 });
        dt.Rows.Add(new object[] { "Component;Name;Quantity;Price;Currency;Result;BaseUnit;Calcu", 4 });
        dt.Rows.Add(new object[] { "Component;Name;Currency;Price;BaseUnit;Result;Unit;Calcu", 5 });
        //dt.Rows.Add(new object[] { "Component;Name;Quantity;Price;Unit;Result;Currency;Calcu", 6 });
        dt.Rows.Add(new object[] { "UpchargeGroup;UpCharge;Quantity;Price;stdPackSize;Result;Currency;Calcu", 6 });
        dt.Rows.Add(new object[] { "Component;Name;Quantity;Price;Result;BaseUnit;Calcu", 7 });
        dt.Rows.Add(new object[] { "Component;Name;Quantity;Price;Result;Currency;Calcu", 8 });
        dt.Rows.Add(new object[] { "Component;Name;Quantity;Price;Result;Currency;Calcu", 9 });
        dt.Rows.Add(new object[] { "ID;Result;Notes;Name", 10 });
        dt.Rows.Add(new object[] { "ID;Remark;OldData;Reason;CreateOn", 11 });
        return dt;
    }
    Worksheet buildsum(StdItems rcus, int tabindex)
    {
        string path = Server.MapPath(@"~/App_Data/Documents/Quotation_cal.xlsx");
        spreadsheet.Document.LoadDocument(path);
        spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;
        Worksheet worksheet = spreadsheet.Document.Worksheets[1];
        IWorkbook workbook = spreadsheet.Document;
        DataRow dr = _dt.Select(string.Format("Component in ('Raw material') and Calcu ={0}", tabindex)).FirstOrDefault();
        if (!string.IsNullOrEmpty(dr["Currency"].ToString())) {
            worksheet.Cells["D7"].Value = string.Format("{0}", dr["Quantity"]);
            worksheet.Cells["C7"].Value = string.Format("{0}", dr["Unit"]);
            worksheet.Cells["B7"].Value = string.Format("{0}", rcus.PackSize=="0"? GetPackSize(_dt).ToString() : rcus.PackSize.ToString());
        }
        double _Bidprice = 0,_pkgstyle = 0, _secpkg = 0, _sumoblect=0;
        double.TryParse(rcus.Bidprice, out _Bidprice);
        if (_Bidprice > 0)
        worksheet.Cells["E7"].Value = string.Format("{0}",  dr["Price"]);
        else
        worksheet.Cells["E7"].Value = string.Format("{0}", rcus.Announcement_Fish_price != null && tabindex == 9 && GetIndex.Equals(-1) ? rcus.Announcement_Fish_price : dr["Price"]);
        object sumoblect = _dt.AsEnumerable().Where(y => y.Field<string>("Component") == "Media").Sum(x => x.Field<double>("Result")).ToString();
        double.TryParse(sumoblect.ToString(), out _sumoblect);
        //worksheet.Cells["F8"].Formula = string.Format("={0}", _dt.Compute("Sum(Result)", "Component in ('Media')"));
        worksheet.Cells["F8"].Formula = string.Format("={0}", Convert.ToDecimal(_sumoblect));
        worksheet.Cells["F9"].Formula = string.Format("={0}", _dt.Compute("Sum(Result)", "Component in ('Packaging') and (Name like 'Primary Packaging%')"));
        worksheet.Cells["F10"].Formula = string.Format("={0}", _dt.Compute("Sum(Result)", "(Name like 'LOH per pack%') and Calcu=" + tabindex));
        object objresult = _dt.Compute("Sum(Result)", "name like 'Packing style%' and Calcu=" + tabindex);
        double.TryParse(objresult.ToString(), out _pkgstyle);
        //if (!Convert.IsDBNull(objresult))
            worksheet.Cells["F11"].Formula = string.Format("={0}", Convert.ToDecimal(_pkgstyle));
        object objsecpkg = _dt.Compute("Sum(Result)", "Component in ('Packaging') and (Name like 'Secondary Packaging%')");
        double.TryParse(objsecpkg.ToString(), out _secpkg);
        //if (!Convert.IsDBNull(objsecpkg))
        worksheet.Cells["F12"].Formula = string.Format("={0}", Convert.ToDecimal(_secpkg));
        //'Secondary Packaging','Media','Primary Packaging','Packing Style','UpCharge
        double _Margin = 0, _Commission = 0, _OverPrice = 0;
        double.TryParse(tabindex == 8 ? rcus.Margin.ToString() : "0", out _Margin);
        double.TryParse(rcus.Commission.ToString(), out _Commission);
        worksheet.Cells["D14"].Value = string.Format("{0}", _Margin);
        worksheet.Cells["D16"].Formula = string.Format("={0}%", _Commission);
        
        worksheet.Cells["F7"].NumberFormat = "0.0000";
        Cell cell = worksheet.Cells["F17"];
        cell.NumberFormat = "0.00##";
        worksheet.Cells["F14"].NumberFormat = "0.###################################################################################################################################################################################################################################################################################################################################################";
        worksheet.Cells["F15"].NumberFormat = "0.###################################################################################################################################################################################################################################################################################################################################################";
         
        double.TryParse(rcus.OverPrice.ToString(), out _OverPrice);
        if (rcus.OverType.Contains("%")) {
            worksheet.Cells["E17"].Value = _OverPrice.ToString();
            worksheet.Cells["F17"].Formula = string.Format("={0}", "(F25-F19-F20)*E17%");
            worksheet.Cells["O15"].Formula = string.Format("={0}", "(O7-O13-O12)*E17%");
        }
        else {
            worksheet.Cells["E17"].Value = _OverPrice.ToString();
            worksheet.Cells["F17"].Formula = string.Format("={0}", _OverPrice);
            worksheet.Cells["O15"].Formula = string.Format("={0}",  "F17");
        }
        //string name = cs.ReadItems("select top 1 name from MasFishSpecies where sapcode = substring('" + rcus.Material.ToString() + "', 3, 2)");
        //worksheet.Cells["A7"].Value = string.Format("{0}", name);
        //worksheet.Cells["A10"].Value = string.Format("LOH per pack {0}", dr["PackSize"]);
        decimal _totupcharge = 0, _Quantityupch = 0, _Priceupch = 0;
        List<TransUpCharge> listUp = rcus.Upcharge;
        if (listUp != null)
        {
            foreach (var _row in listUp)
            {
                _Priceupch += Convert.ToDecimal(_row.Price.ToString());
                _Quantityupch += Convert.ToDecimal(_row.Quantity.ToString());
                _totupcharge += Convert.ToDecimal(_row.Result);
            }
        }
        double _SubContainers = 0, sumFreight = 0;
        double Freight = 0;
        if (!string.IsNullOrEmpty(rcus.SubContainers.ToString()))
        {
            if (double.TryParse(rcus.SubContainers.ToString(), out _SubContainers))
            {
                if (!string.IsNullOrEmpty(tbFreight.Text.ToString()))
                    double.TryParse(string.Format("{0}", tbFreight.Value.ToString()), out Freight);
                sumFreight = (Convert.ToDouble(Freight) / Convert.ToDouble(_SubContainers));
                worksheet.Cells["D19"].Value = _SubContainers.ToString();
                worksheet.Cells["E19"].Value = Freight.ToString();
            }
        }
        else
        {
            worksheet.Cells["F19"].Formula = string.Format("={0}", "0");
            worksheet.Cells["D19"].Formula = string.Format("={0}", "0");
            worksheet.Cells["E19"].Formula = string.Format("={0}", "0");
        }
        string _Incoterm = string.Format("{0}", CmbIncoterm.Value);
        worksheet.Cells["F20"].Formula = string.Format("={0}", "0");
        if (!string.IsNullOrEmpty(_Incoterm))
        {
            if (_Incoterm.ToString().Equals("FOB"))
                worksheet.Cells["F20"].Formula = string.Format("={0}", "0");
            else
                worksheet.Cells["F20"].Formula = string.Format("={0}", "F25*D20%");
        }
        double _interest = 0;
        worksheet.Cells["D21"].Formula = string.Format("={0}", "0");
        object objinterest = _dt.Compute("Sum(Result)", "Name in ('Interest')");
        if (double.TryParse(string.Format("{0}", tbinterest.Value), out _interest))
            worksheet.Cells["D21"].Formula = string.Format("={0}", _interest.ToString());

        //if (tbinterest.Value.ToString() == "0")
        //    worksheet.Cells["F21"].Formula = string.Format("=0");
        //else
        //    worksheet.Cells["F21"].Formula = string.Format("=F25*{0}%", tbinterest.Value.ToString());

        worksheet.Cells["D13"].Value = string.Format("{0}", "");
        worksheet.Cells["E13"].Value = string.Format("{0}", "");
        worksheet.Cells["F13"].Formula = string.Format("={0}", _totupcharge);
        worksheet.Cells["D23"].Formula = string.Format("={0}", rcus.Pacifical==""?"0": rcus.Pacifical);
        worksheet.Cells["D24"].Formula = string.Format("={0}", rcus.MSC == "" ? "0" : rcus.MSC);
        if (rcus.Pacifical.ToString() == "0")
            worksheet.Cells["F23"].Formula = string.Format("=0");
        else
            worksheet.Cells["F23"].Formula = string.Format("={0}", "F25*D23%");
        if (rcus.MSC.ToString() == "0")
            worksheet.Cells["F24"].Formula = string.Format("=0");
        else
            worksheet.Cells["F24"].Formula = string.Format("={0}", "F25*D24%");
        decimal TotalObject = 0;
        for (int i = 8; i <= 13; i++)
        {
            string cellvalue = worksheet.Cells["F" + i].Value.ToString();
            TotalObject += Convert.ToDecimal(cellvalue);
        }
        string a = worksheet.Cells["F15"].Value.ToString();
        if (!a.ToString().Contains("#VALUE!") && !a.ToString().Contains("#NUM!"))
        {
            object sumObject = worksheet.Cells["F15"].Value.ToString();
            decimal _ResultPrice = Convert.ToDecimal(sumObject) - Convert.ToDecimal(TotalObject);
            decimal _FillWeight = Convert.ToDecimal(worksheet.Cells["D7"].Value.ToString());
            decimal _Yield = Convert.ToDecimal(worksheet.Cells["C7"].Value.ToString());
            object _packsize = Convert.ToDecimal(worksheet.Cells["B7"].Value.ToString());
            if (_packsize.ToString() != "0")
                worksheet.Cells["F26"].Value = Convert.ToDecimal(_ResultPrice * 1000 / _FillWeight * 1000 *
                    _Yield / Convert.ToDecimal(_packsize.ToString()));
            else
                worksheet.Cells["F26"].Value = "0";
            //object objinterest = Convert.ToDecimal(worksheet.Cells["F20"].Value.ToString());
            //object objcif = Convert.ToDecimal(worksheet.Cells["F22"].Value.ToString());
            worksheet.Cells["F16"].Formula = string.Format("={0}", "(F25-F19-F20)*D16");
            worksheet.Cells["F21"].Formula = string.Format("={0}", "F25*D21%");
            worksheet.Cells["F24"].Formula = string.Format("={0}", "F25*D24%");
        }
        // ... Excel worksheet calculation,
        if (rcus.Bidprice != null)
            worksheet.Cells["O7"].Value = string.Format("{0}", rcus.Bidprice);
        worksheet.Calculate();

        return worksheet;
    }
    private static string ToLongString(double input)
    {
        string strOrig = input.ToString();
        string str = strOrig.ToUpper();

        // if string representation was collapsed from scientific notation, just return it:
        if (!str.Contains("E")) return strOrig;

        bool negativeNumber = false;

        if (str[0] == '-')
        {
            str = str.Remove(0, 1);
            negativeNumber = true;
        }

        string sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        char decSeparator = sep.ToCharArray()[0];

        string[] exponentParts = str.Split('E');
        string[] decimalParts = exponentParts[0].Split(decSeparator);

        // fix missing decimal point:
        if (decimalParts.Length == 1) decimalParts = new string[] { exponentParts[0], "0" };

        int exponentValue = int.Parse(exponentParts[1]);

        string newNumber = decimalParts[0] + decimalParts[1];

        string result;

        if (exponentValue > 0)
        {
            result =
                newNumber +
                GetZeros(exponentValue - decimalParts[1].Length);
        }
        else // negative exponent
        {
            result =
                "0" +
                decSeparator +
                GetZeros(exponentValue + decimalParts[0].Length) +
                newNumber;

            result = result.TrimEnd('0');
        }

        if (negativeNumber)
            result = "-" + result;

        return result;
    }

    private static string GetZeros(int zeroCount)
    {
        if (zeroCount < 0)
            zeroCount = Math.Abs(zeroCount);

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < zeroCount; i++) sb.Append("0");

        return sb.ToString();
    }
    StdItems buildcalcu(StdItems dr)
    {
        if (dr.cal!= null)
        {
            _dt = MyToDataTable.ToDataTable(dr.cal);
            //g.AutoFilterByColumn(g.Columns["Calcu"], string.Format("{0}", ActivePageSymbol));
            //((GridViewDataColumn)g.Columns[1]).SortAscending();
            //if (_dt != null)
            if (_dt.Rows.Count > 0)
            {

                var values = new[] { "Secondary Packaging","FOB price(18 - digits)","Value Commission","OverPrice","FOB Price",
                            "CIF Price","Insurance","Margin","Interest","Route","Upcharge","MSC&Pacifical","Offer Price","Equivalent fish price"};
                const string sentence = "8;9";
                string[] words = Regex.Split(sentence, @";");
                foreach (string value in words)
                {

                    //Console.WriteLine("WORD: " + value);
                    int tabindex = Convert.ToInt32(value);
                    Worksheet ws = buildsum(dr, tabindex);
                    if (!ws.Cells["F7"].Value.ToString().Contains("#VALUE!"))
                    {
                        double _Bidprice = 0;
                        double _margin = 0;
                        double.TryParse(dr.Bidprice, out _Bidprice);
                        double.TryParse(dr.Margin, out _margin);
                        if (_Bidprice>0) {
                            Object a = ws.Cells["O19"].Value;
                            CellRange sourceRange = ws["O19:O19"];
                            a= sourceRange[0].Value.ToObject();
                            double _marginBid = Convert.ToDouble(a);
                            dr.marginBid = Convert.ToDecimal(a).ToString();
                            //dr.Announcement_Fish_price = Convert.ToDecimal(ws.Cells["Q33"].Value.ToString()).ToString("F4");
                        }
                        CellRange range = ws["A6:G26"];
                        DataTable table = ws.CreateDataTable(range, false);
                        DataTableExporter exporter = ws.CreateDataTableExporter(range, table, false);
                        exporter.CellValueConversionError += exporter_CellValueConversionError;
                        exporter.Export();

                        for (int i = 0; i <= values.Length - 1; i++)
                        {
                            //object sumObject;
                            DataRow _xdr = _dt.Select(string.Format("Name='{0}' and Calcu={1}", values[i], tabindex)).FirstOrDefault();
                            //DataRow dr = _dt.Select("Name='Secondary Packaging'").FirstOrDefault();
                            if (_xdr != null)
                            {
                                switch (_xdr["Name"].ToString())
                                {
                                    //case "Secondary Packaging":
                                    //    sumObject = _dt.Compute("Sum(Result)", "Name <> 'Secondary Packaging per case' and Calcu=5");
                                    //    if (sumObject != System.DBNull.Value)
                                    //    {
                                    //        _xdr["Result"] = Convert.ToDecimal(sumObject);
                                    //    }
                                    //    break;
                                    case "FOB price(18 - digits)":
                                        //sumObject = _dt.Compute(@"Sum(Result)", "(Name in ('Secondary Packaging','Media'," +
                                        //    "'Primary Packaging','Packing Style','UpCharge') or Component in ('Raw material') or (Name like 'LOH per pack%')) and Calcu=" + tabindex);
                                        //if (sumObject != System.DBNull.Value && tabindex != 9)
                                        //{
                                        //    var rowsToUpdate = _dt.AsEnumerable().Where(r => r.Field<string>("Name") == "Margin" && tabindex == r.Field<int>("Calcu")).FirstOrDefault();
                                        //    decimal PerMargin = 0, _sumprice = Convert.ToDecimal(sumObject);
                                        //    decimal.TryParse(rowsToUpdate["Quantity"].ToString(), out PerMargin);
                                        //    decimal _totalmargin = (_sumprice / ((100 - PerMargin) / 100));
                                        //    decimal _Margin = _totalmargin * (PerMargin / 100);
                                        //    //decimal _totalmargin = _sumprice * (Convert.ToDecimal(PerMargin) / 100);
                                        //    rowsToUpdate["Result"] = _Margin.ToString("F4");
                                        //    _xdr["Result"] = Convert.ToDecimal(_sumprice + _Margin).ToString("F4");
                                        //}
                                        //else
                                        //    _xdr["Result"] = Convert.ToDecimal(sumObject);
                                        _xdr["Result"] = decimal.Parse(ws.Cells["F15"].Value.ToString());
                                        break;
                                    case "FOB Price":
                                        //object totalsumObject = _dt.Compute("Sum(Result)",
                                        //    "Name in ('Value Commission','FOB price(18 - digits)','OverPrice') and Calcu=" + tabindex);
                                        //_xdr["Result"] = totalsumObject.ToString();
                                        _xdr["Result"] = decimal.Parse(ws.Cells["F18"].Value.ToString());
                                        break;
                                    //case "Insurancexxx":
                                    //    if (CmbIncoterm.Value != null)
                                    //        if (CmbIncoterm.Value.ToString() != "FOB")
                                    //        {
                                    //            object _sumObject = _dt.Compute("Sum(Result)", "Name in ('CIF Price') and Calcu=" + tabindex);
                                    //            double _Insurance = 0.11;
                                    //            if (_Insurance > 0)
                                    //            {
                                    //                decimal _sumInsurance = (Convert.ToDecimal(_sumObject) / ((100 - Convert.ToDecimal(_Insurance)) / 100));
                                    //                _xdr["Result"] = _sumInsurance * (Convert.ToDecimal(_Insurance) / 100);
                                    //            }
                                    //        }
                                    //        else
                                    //            _xdr["Result"] = 0;
                                    //    DataRow drx = _dt.Select("Name in ('CIF Price') and Calcu=" + tabindex).FirstOrDefault();
                                    //    {
                                    //        drx["Result"] = Convert.ToDecimal(drx["Result"]) + Convert.ToDecimal(_xdr["Result"]);
                                    //    }
                                    //    break;
                                    case "Insurance":
                                        _xdr["Result"] = decimal.Parse(ws.Cells["F20"].Value.ToString());
                                        _xdr["Quantity"] = decimal.Parse(ws.Cells["D20"].Value.ToString());
                                        //if (CmbIncoterm.Value.ToString() != "FOB")
                                        //{
                                        //    if (ws.Cells["F20"].Value.ToString() == "#VALUE!" ||
                                        //       ws.Cells["F20"].Value.ToString() == "#NUM!")
                                        //    {
                                        //        object _sumtotal = _dt.Compute("Sum(Result)", "Name in ('FOB price(18 - digits)','Route','Interest','Pacifical') and Calcu=" + tabindex);
                                        //        decimal _sumInsurance = (Convert.ToDecimal(_sumtotal) / (1 - (Convert.ToDecimal(0.11) / 100)));
                                        //        _xdr["Result"] = _sumInsurance - (Convert.ToDecimal(_sumtotal));
                                        //    } else {

                                        //        _xdr["Result"] = decimal.Parse(ws.Cells["F20"].Value.ToString());
                                        //    }
                                        //}
                                        //else
                                        //    _xdr["Result"] = 0;
                                        //DataRow drs = _dt.Select("Name in ('CIF Price') and Calcu=" + tabindex).FirstOrDefault();
                                        //{
                                        //    drs["Result"] = Convert.ToDecimal(drs["Result"]) + Convert.ToDecimal(_xdr["Result"]);
                                        //}
                                        break;
                                    case "Interest":
                                        decimal _Interest = 0;
                                        if (decimal.TryParse(tbinterest.Text, out _Interest))
                                        {
                                            //object _sumObject = _dt.Compute("Sum(Result)", "Name in ('FOB price(18 - digits)') and Calcu=" + tabindex);
                                            //decimal _totalprice = Convert.ToDecimal(_sumObject) / (1 - (_Interest / 100)) - Convert.ToDecimal(_sumObject);
                                            //_xdr["Result"] = Convert.ToDouble(_totalprice).ToString("F4");

                                            //object _sumObject = _dt.Compute("Sum(Result)", "Name in ('CIF Price') and Calcu=" + tabindex);
                                            //decimal _sumInterest = (Convert.ToDecimal(_sumObject) / ((100 - _Interest) / 100));
                                            //_xdr["Result"] = _sumInterest * (Convert.ToDecimal(_Interest) / 100);
                                            _xdr["Quantity"] = decimal.Parse(ws.Cells["D21"].Value.ToString());
                                            _xdr["Result"] = decimal.Parse(ws.Cells["F21"].Value.ToString());
                                            //DataRow drwx = _dt.Select("Name in ('CIF Price') and Calcu=" + tabindex).FirstOrDefault();
                                            //{
                                            //    drwx["Result"] = Convert.ToDecimal(drwx["Result"]) + Convert.ToDecimal(_xdr["Result"]);
                                            //}
                                        }
                                        break;
                                    case "CIF Price":
                                        //object _cifsumObject = _dt.Compute("Sum(Result)", "Name in ('FOB Price','Route','Insurance','Interest') and Calcu=" + tabindex);
                                        //_xdr["Result"] = _cifsumObject.ToString();
                                        //if (Double.IsNaN(Convert.ToDouble(_xdr["Result"])))
                                        //    _xdr["Result"] = 0;
                                        _xdr["Result"] = decimal.Parse(ws.Cells["F22"].Value.ToString());
                                        break;
                                    case "Offer Price":
                                        _xdr["Result"] = decimal.Parse(ws.Cells["F25"].Value.ToString());

                                        //object _OffersumObject = _dt.Compute("Sum(Result)", "Name in ('CIF Price','MSC&Pacifical') and Calcu=" + tabindex);
                                        ////"Insurance"
                                        //double _sumofferprice = Convert.ToDouble(_OffersumObject);
                                        //_xdr["Result"] = _sumofferprice.ToString("F4");
                                        break;
                                    case "Value Commission":
                                        _xdr["Result"] = Decimal.Parse(ws.Cells["F16"].Value.ToString()).ToString("F4");
                                        //DataRow drw = _dt.Select("Name='FOB price(18 - digits)' and Calcu=" + tabindex).FirstOrDefault();
                                        //if (drw != null)
                                        //{
                                        //    decimal Value_Commission = 0;
                                        //    if (decimal.TryParse(_xdr["Quantity"].ToString(), out Value_Commission))
                                        //    {
                                        //        decimal _commition = Convert.ToDecimal(Value_Commission) / 100;
                                        //        if (_commition > 0)
                                        //        {
                                        //                decimal _valuecomm = Convert.ToDecimal(drw["Result"]) / (1 - _commition) - Convert.ToDecimal(drw["Result"]);
                                        //                _xdr["Result"] = _valuecomm.ToString("F4"); 
                                        //        }
                                        //        else
                                        //            _xdr["Result"] = "0";
                                        //    }
                                        //}
                                        break;
                                    case "Margin":
                                        object sumObject = Convert.ToDecimal(ws.Cells["F15"].Value.ToString());
                                        Cell cellMargin = ws.Cells["D14"];
                                        cellMargin.NumberFormat = "0.00##";
                                        _xdr["Result"] = (Convert.ToDecimal(cellMargin.Value.ToString()) / 100) * Convert.ToDecimal(sumObject);
                                        _xdr.SetField("Quantity", cellMargin.Value.ToString());
                                        break;
                                    case "OverPrice":
                                        Cell cell = ws.Cells["F17"];
                                        cell.NumberFormat = "0.00##";
                                        _xdr["Result"] = Double.Parse(cell.Value.ToString()).ToString("F4");
                                        ////sumObject = _dt.Compute("Sum(Result)", "Name in ('Value Commission','FOB price(18 - digits)') and Calcu=" + tabindex);
                                        //sumObject = _dt.Compute("Sum(Result)", "Name in ('FOB price(18 - digits)') and Calcu=" + tabindex);
                                        //if (_xdr["Price"].ToString() == "")
                                        //    _xdr["Price"] = "0";
                                        ////int Index = gv.EditingRowVisibleIndex;
                                        ////object baseunit = string.Format("{0}", gv.GetRowValues(Index, "OverType"));
                                        //decimal _OverPrice = 0;
                                        //decimal.TryParse(_xdr["Price"].ToString(), out _OverPrice);
                                        ////decimal _OverPrice = Convert.ToDecimal(_xdr["Price"]);
                                        //if (_OverPrice > 0)
                                        //{
                                        //    if (_xdr["BaseUnit"].ToString() == "%")
                                        //    {
                                        //            decimal _sumOverPrice = (Convert.ToDecimal(sumObject) / ((100 - _OverPrice) / 100));
                                        //            _xdr["Result"] = _sumOverPrice * (Convert.ToDecimal(_OverPrice) / 100); //=(F97/(100%-H101))-F97
                                        //        //(F15/(100-E17)*100)-F15
                                        //    }
                                        //    else
                                        //        _xdr["Result"] = _OverPrice.ToString("F4");

                                        //}
                                        //else
                                        //    _xdr["Result"] = "0";
                                        break;
                                    //case "Pacifical":
                                    //case "MSC":
                                    //    sumObject = _dt.Compute("Sum(Result)", "Name in ('CIF Price') and Calcu=" + tabindex);
                                    //    decimal Quantity = 0;
                                    //    decimal.TryParse(_xdr["Quantity"].ToString(), out Quantity);
                                    //    decimal _sumPrice = Quantity / 100;

                                    //    if (_sumPrice > 0)
                                    //    {
                                    //        decimal _totalprice = Convert.ToDecimal(sumObject) / (1 - _sumPrice) - Convert.ToDecimal(sumObject);
                                    //        _xdr["Result"] = _totalprice.ToString("F4");

                                    //    }
                                    //    else
                                    //        _xdr["Result"] = "0";
                                    //    break;
                                    case "MSC&Pacifical":
                                        //sumObject = _dt.Compute("Sum(Result)", "Name in ('CIF Price') and Calcu=" + tabindex);
                                        //sumObject = Double.Parse(ws.Cells["F25"].Value.ToString()).ToString("F4");
                                        decimal MSC = 0, Pacifical = 0, Result=0;
                                        decimal.TryParse(dr.Pacifical.ToString(), out Pacifical);
                                        decimal.TryParse(dr.MSC.ToString(), out MSC);
                                        Object a = ws.Cells["F23"].Value;
                                        CellRange sourceRange = ws["F23"];
                                        a = sourceRange[0].Value.ToObject();
                                        double _MSC = Convert.ToDouble(a);
                                        CellRange _cellRange = ws["F24"];
                                        Object _obj = _cellRange[0].Value.ToObject(); 
                                        double _Pacifical = Convert.ToDouble(_obj);
                                        //_xdr["Result"] = (Convert.ToDecimal(sumObject) / (1 - (MSC / 100) - (Pacifical / 100))) - Convert.ToDecimal(sumObject);
                                        _xdr["Result"] = Convert.ToDouble(_MSC + _Pacifical).ToString("F4");
                                        _xdr["Quantity"] = Convert.ToDecimal(Pacifical + MSC).ToString();
                                        break;
                                    case "Equivalent fish price":
                                        if (tabindex != 9) {
                                            decimal.TryParse(ws.Cells["Q33"].Value.ToString(), out Result);
                                                _xdr["Result"] = string.Format("{0}", Convert.ToDecimal(Result)); //recalculate();
                                            //if (_margin == 0)
                                            //    dr.Announcement_Fish_price = dr.Equivalent_Fish_price;
                                            //else 
                                            if (_Bidprice > 0 || string.IsNullOrEmpty(dr.Apply))
                                                dr.Announcement_Fish_price = string.Format("{0}", Convert.ToDecimal(_xdr["Result"]).ToString("F2"));
                                                //dr.Announcement_Fish_price = string.Format("{0}", Convert.ToDecimal(ws.Cells["Q33"].Value.ToString()));
                                                }
                                        break;
                                    case "Upcharge":
                                        //if (Double.IsNaN(Convert.ToDouble(_xdr["Result"]))) { 
                                        if(decimal.TryParse(ws.Cells["F13"].Value.ToString(), out Result)) { 
                                            _xdr["Result"] = string.Format("{0}", Convert.ToDecimal(Result)); 
                                        }
                                        dr.totalUpcharge =  ws.Cells["F13"].Value.ToString();
                                        break;
                                    case "Route":
                                        if (Double.IsNaN(Convert.ToDouble(_xdr["Result"])))
                                            _xdr["Result"] = 0;

                                        break;
                                }
                            }
                        }
                        if (tabindex == 9)
                        {
                            DataRow _arow = _dt.Select("Name='Equivalent margin' and Calcu=9").FirstOrDefault();
                            string _text = "Name in ('FOB price(18 - digits)')  and Calcu={0}";
                            object sumObject = _dt.Compute(@"Sum(Result)", string.Format(_text, 8));
                            object sumMargin = _dt.Compute(@"Sum(Result)", string.Format("Name in ('Margin')", 8));
                            object conObject = _dt.Compute(@"Sum(Result)", string.Format(_text, 9));
                            if (!Convert.IsDBNull(sumMargin) && _arow != null)
                                _arow["Result"] = Convert.ToDecimal((Convert.ToDecimal(conObject) - (Convert.ToDecimal(sumObject) -
                                Convert.ToDecimal(sumMargin))) / Convert.ToDecimal(conObject)) * 100;
                            else _arow["Result"] = "0";
                        }
                    }
                }
            }
            //}

            DataRow rw = _dt.Select("Name='Offer Price' and Calcu=8").FirstOrDefault();
            if (rw != null)
            {
                //int Index = gv.FocusedRowIndex;
                //object keyValue = gv.GetRowValues(Index, gv.KeyFieldName);
                //var dr = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(keyValue));
                //DataRow dr = tcustomer.Select("ID="+ keyValue).FirstOrDefault();
                //dr.MinPrice = rw["Result"].ToString();
                //string a = _dt.Compute("Max(Result)", "Name='Offer Price' and Calcu=8").ToString();
                //dr.OfferPrice = Double.Parse(a.ToString()).ToString("F4");
                //if (dr.OfferPrice.ToString() == "0")
                dr.OfferPrice = Convert.ToDecimal(rw["Result"].ToString()).ToString("F4");

            }
            DataRow fishprice = _dt.Select("Component='Raw material' and Calcu=8").FirstOrDefault();
            if (fishprice != null)
                dr.Equivalent_Fish_price = Convert.ToDecimal(fishprice["Price"].ToString()).ToString("F2");
        dr.cal = cs.ConvertDataTable<TransCal>(_dt);
        }

        return dr;
    }
    protected void tcDemos_ActiveTabChanged(object source, TabControlEventArgs e)
    {
        if (e.Tab.Index == 1)
        {
            var grid = (ASPxGridView)e.Tab.FindControl("gvitems");
            //...  
        }
    }
    //private void EnsureColumns()
    //{
    //    if (gvitems.Columns.Count == 0)
    //        ReCreateColumns();
    //    else
    //    {
    //        DataTable table = GetCurrentTable;
    //        // if the grid has other columns than the assigned data source, 
    //        // recreate columns
    //        if (gvitems.Columns[table.Columns[0].ColumnName] == null)
    //            ReCreateColumns();
    //    }
    //}
    int GetIndex
    {
        get {
            var tab = tcDemos.ActiveTabIndex;
            if (tab == 0 || tab == 1) return -1;
            return tab - 2;
        }
    }
    private void ReCreateColumns()
    {
        gvitems.Columns.Clear();
        if (_CurrentTableCol == null)
            _CurrentTableCol = buildcolumnTable();
        bool first = true;
        var index = GetIndex;
        DataRow[] result = _CurrentTableCol.Select(string.Format("Marks='{0}'", index));
        foreach (DataRow data in result)
        {
            string[] args = data["Name"].ToString().Split(';');
            for (int x = 0; x < args.Length; x++)
            {
                GridViewDataTextColumn column = new GridViewDataTextColumn();
                column.FieldName = args[x];
                column.CellStyle.CssClass = "truncated";
                if (args[x] == "Result")
                    column.PropertiesTextEdit.DisplayFormatString = "{0:0.0000}";
                // set additional column properties
                if (index == 0)
                {
                    column.Caption = args[x];
                }
                else if (index == 1)
                {
                    if (args[x] == "Currency")
                        column.Caption = "FillWeight";
                    else if (args[x] == "Quantity")
                        column.Caption = "Yield";
                    else if (args[x] == "Price")
                        column.Caption = "Fish price";
                    else if (args[x] == "Result")
                        column.Caption = "Fish cost";
                    else
                        column.Caption = args[x];
                }
                else if (index == 2)
                {
                    if (args[x] == "Currency")
                        column.Caption = "Loss";
                    else if (args[x] == "Quantity")
                        column.Caption = "FillWeight";
                    else if (args[x] == "Price")
                        column.Caption = "Media price";
                    else if (args[x] == "Result")
                        column.Caption = "Media cost";
                    else
                        column.Caption = args[x];
                }
                else if (index == 3)
                {
                    if (args[x] == "Currency")
                        column.Caption = "Unit";
                    else if (args[x] == "Quantity")
                        column.Caption = "Loss";
                    else if (args[x] == "Price")
                        column.Caption = "Primary Packaging";
                    else if (args[x] == "Result")
                        column.Caption = "Packaging cost";
                    else
                        column.Caption = args[x];
                }
                else if (index == 4)
                {
                    if (args[x] == "Currency")
                        column.Caption = "Unit";
                    else if (args[x] == "Price")
                        column.Caption = "LOH price";
                    else if (args[x] == "Result")
                        column.Caption = "LOH cost";
                    else if (args[x] == "Quantity")
                        column.Caption = "Fill weight and KG/Case";
                    else
                        column.Caption = args[x];
                }
                else if (index == 5)
                {
                    if (args[x] == "Currency")
                        column.Caption = "Pack size/Unit";
                    else if (args[x] == "Price")
                        column.Caption = "Secondary Packaging price";
                    else if (args[x] == "BaseUnit")
                        column.Caption = "Currency";
                    else if (args[x] == "Result")
                        column.Caption = "Packing style cost";
                    else
                        column.Caption = args[x];
                }
                else if (index == 6)
                {
                    if (first)
                    {
                        //GridViewCommandColumn command = new GridViewCommandColumn();
                        //g.Columns.Add(command);
                        GridViewCommandColumn col = new GridViewCommandColumn();
                        col.Name = "CommandColumn";
                        col.ShowEditButton = true;
                        col.ShowNewButtonInHeader = true;
                        //col.VisibleIndex = 0;
                        col.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        col.HeaderTemplate = new CustomDataColumnTemplate("6");
                        col.ButtonRenderMode = GridCommandButtonRenderMode.Image;
                        GridViewCommandColumnCustomButton but = new GridViewCommandColumnCustomButton();
                        but.ID = "colDel";
                        but.Image.ToolTip = "Remove";
                        but.Image.Url = "~/Content/Images/Cancel.gif";
                        col.Width = Unit.Pixel(45);
                        col.CustomButtons.Add(but);
                        gvitems.Columns.Add(col);
                        first = false;
                    }
                    if (args[x] == "UpCharge")
                    {
                        var de = new GridViewDataButtonEditColumn();
                        var redirectButton = new EditButton();
                        //redirectButton.Visible = true;
                        //redirectButton.Image.Url = ELLIPSIS_BUTTON_IMAGE;
                        //redirectButton.Image.Width = ELLIPSIS_BUTTON_WIDTH;
                        redirectButton.ImagePosition = ImagePosition.Top;
                        redirectButton.Position = ButtonsPosition.Right;
                        de.PropertiesButtonEdit.Buttons.Add(redirectButton);
                        de.PropertiesButtonEdit.ClientSideEvents.ButtonClick = "function(s,e){OnButtonClick('Upcharg');}";
                        de.Caption = args[x];
                        de.FieldName = args[x];
                        gvitems.Columns.Add(de);
                        goto JumpToContinue;
                    }
                    if (args[x] == "stdPackSize")
                        column.Caption = "PackSize";
                    else if (args[x] == "Quantity")
                        column.Caption = "StdPackSize";
                    else if (args[x] == "Price")
                        column.Caption = "Upcharge price";
                    else if (args[x] == "Result")
                        column.Caption = "Upcharge cost";
                    else
                        column.Caption = args[x];
                }
                else if (index == 7)
                {
                    if (args[x] == "Quantity")
                        column.Caption = "cases per fcl";
                    else if (args[x] == "Price")
                        column.Caption = "Route price";
                    else if (args[x] == "BaseUnit")
                        column.Caption = "Currency";
                    else if (args[x] == "Result")
                        column.Caption = "Route cost";
                    else
                        column.Caption = args[x];
                }
                else if (index.Equals(10)) {
                    if (first)
                    {
                        GridViewCommandColumn col = new GridViewCommandColumn();
                        col.Name = "CommandColumn";
                        col.ShowEditButton = false;
                        col.ShowNewButtonInHeader = false;
                        //col.VisibleIndex = 0;
                        col.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        col.HeaderTemplate = new CustomDataColumnTemplate("10");
                        col.ButtonRenderMode = GridCommandButtonRenderMode.Image;
                        
                        col.Width = Unit.Pixel(85);
                        //GridViewCommandColumnCustomButton edbut = new GridViewCommandColumnCustomButton();

                        //edbut.ID = "edcolUpload";
                        //edbut.Image.ToolTip = "edit";
                        //edbut.Image.Url = "~/Content/Images/icons8-edit-16.png";
                        //col.CustomButtons.Add(edbut);
                        //gvitems.Columns.Add(col);

                        GridViewCommandColumnCustomButton but = new GridViewCommandColumnCustomButton();

                        but.ID = "decolUpload";
                        but.Image.ToolTip = "remove";
                        but.Image.Url = "~/Content/Images/Cancel.gif";
                        col.CustomButtons.Add(but);
                        gvitems.Columns.Add(col);
                        first = false;
                    }
                    if (args[x].Equals("Result"))
                    {
                        GridViewDataComboBoxColumn Comb = new GridViewDataComboBoxColumn();
                        Comb.FieldName = args[x];
                        Comb.PropertiesComboBox.Columns.Clear();
                        Comb.PropertiesComboBox.Columns.Add(new ListBoxColumn("Title"));
                        Comb.PropertiesComboBox.ValueField = "Title";
                        //Comb.PropertiesComboBox.ClientSideEvents.SelectedIndexChanged = "function(s,e){buildpComment(s.GetText(), 'tbFreight');}";
                        //Comb.PropertiesComboBox.TextField = "Title";
                        //Comb.PropertiesComboBox.TextFormatString = "{0}";
                        string strSQL = @"select LTRIM(RTRIM(value))'Title' from dbo.FNC_SPLIT('" + approverole + "',',')";
                        Comb.PropertiesComboBox.DataSource = cs.builditems(@strSQL);
                        gvitems.Columns.Add(Comb);
                        gvitems.Columns["Result"].Width = Unit.Pixel(170);
                        //goto JumpToContinue;
                    }
                    else if (args[x].Equals("Notes"))
                    {
                        GridViewDataHyperLinkColumn hy = new GridViewDataHyperLinkColumn();
                        hy.FieldName = "ID"; 
                        hy.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        hy.UnboundExpression = "[ID]";
                        hy.Caption = args[x];
                        hy.PropertiesHyperLinkEdit.TextField = args[x].ToString();
                        hy.PropertiesHyperLinkEdit.NavigateUrlFormatString = "~/popupControls/DownloadFile.aspx?Id={0}";
                        //hy.PropertiesHyperLinkEdit.NavigateUrlFormatString = "javascript:downloadfile('{0}');";
             
                        gvitems.Columns.Add(hy);
                        //goto JumpToContinue;
                    }
                    else if (args[x].Equals("Name"))
                    {
                        GridViewDataTextColumn url = new GridViewDataTextColumn();
                        url.FieldName = args[x];
                        url.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        url.UnboundExpression = "[ID]";
                        url.Caption = args[x];
                        url.EditItemTemplate = new ButtonTemplate(gvitems,listItems);
                        gvitems.Columns.Add(url);
                        //goto JumpToContinue;
                    }
                }
                else
                {
                    if (args[x] == "Quantity")
                        column.Caption = "FW / Quantity";
                    else
                        column.Caption = args[x];

                }
                //column.Caption = args[x];
                if (args[x] == "Calcu")
                    column.Width = Unit.Pixel(0);
                if (gvitems.Columns.IndexOf(gvitems.Columns[args[x]]) == -1)
                    gvitems.Columns.Add(column);
            JumpToContinue:;
            }
        }
    }
    void GetCurrentTable(ASPxGridView g)
    {
        //int editrow = gv.EditingRowVisibleIndex;
        //if (editrow == -1) return null;
        var list = new List<dynamic>();
        if (listItems == null) return;
        int FocusedRowIndex = gv.FocusedRowIndex;
        object keyValue = gv.GetRowValues(FocusedRowIndex, gv.KeyFieldName);
        if (keyValue == null) return;
        //var rcus = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(keyValue));

        //int[] arr = strutilize.Split('|').Select(Int32.Parse).ToArray();

        //int arri = 0;
        //foreach (var _r in _utilizedt)
        //{
        //    _r["Result"] = arr[arri];
        //    arri++;
        //}
        var index = GetIndex;
        switch (index) {
            case 0:
                //if(_utilizedt != null) { 
                //gvitems.KeyFieldName = _utilizedt.Columns[0].ColumnName;
                //gvitems.Columns[0].Visible = false;
                //if (rcus == null) g.DataSource = null;
                //if (rcus.Utilize.Count > 0)
                //    g.DataSource = rcus.Utilize;
                //else
                //{
                //DataTable _utilizedt = Getutilize(Convert.ToDateTime(defrom.Value).Date,
                //Convert.ToDateTime(deto.Value).Date);
                var rcus = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(keyValue));
                if (rcus == null) return;
                g.KeyFieldName = "RowID";
                g.DataSource = rcus.Utilize;
                break;
            case 6:
                var obj = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(keyValue));
                if (obj == null) return;
                g.KeyFieldName = "RowID";
                g.DataSource = obj.Upcharge;
                break;
            case 10:
                g.KeyFieldName = "ID";
                //g.DataSource = USP_Select_FileDetails;
                g.DataSource = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(keyValue)).FileDetails;
                break;
            case 11:
                g.KeyFieldName = "ID";
                //g.DataSource = USP_Select_FileDetails;
                string GeID = String.Format("select * from StdHistory where Requestno='{0}' and isnull(SubID,'')=(case when isnull(SubID,'') ='' then isnull(SubID,'') else '{1}' end)", hGeID["GeID"], keyValue);
                g.DataSource = cs.builditems(GeID);
                break;
            case 99:
                if (USP_Select_FileDetails == null) return;
                //int Index = gv.FocusedRowIndex;
                //object keyValue = gv.GetRowValues(Index, gv.KeyFieldName);
                DataRow[] xdr = USP_Select_FileDetails.Select(string.Format("SubID={0}", keyValue));
                if (xdr.Length == 0)
                {
                    g.DataSource = null;
                    return;
                }
                DataTable xdt1 = xdr.CopyToDataTable();
                g.KeyFieldName = xdt1.Columns[0].ColumnName;
                g.DataSource = xdt1;
                break;
            default:
                var _rw = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(keyValue));
                if (_rw == null) return;
               
                var _dtx = MyToDataTable.ToDataTable(_rw.cal);
                if (_dtx == null) return;
                if (_dtx.Rows.Count > 0)
                {
                    g.KeyFieldName = _dtx.Columns[0].ColumnName;
                    //if(index == 6)
                    //    gvitems.Columns[1].Visible = false;
                    //else
                    //gvitems.Columns[0].Visible = false;
                }
                DataRow[] dr = _dtx.Select(string.Format("Calcu={0}", index));
                if (dr.Length == 0)
                {
                    DataTable dtclone = _dtx.Clone();
                    g.DataSource = dtclone;
                }
                else {
                    DataTable dt1 = dr.CopyToDataTable();
                    //dt.Rows.Add(dr);
                    int co = dt1.Rows.Count;
                    g.DataSource = dt1;
                }
                break;
        }
    }
    private void AddCommandColumn(ASPxGridView g)
    {
        GridViewCommandColumn c = new GridViewCommandColumn();
        g.Columns.Add(c);
        c.Name = "Editor";
        c.Caption = "Editor";
        c.VisibleIndex = 0;
        c.Width = 50;
        c.ShowNewButtonInHeader = true;
        c.ShowDeleteButton = true;
        c.ButtonRenderMode = GridCommandButtonRenderMode.Image;
        c.HeaderTemplate = new CustomDataColumnTemplate("0");
        g.SettingsCommandButton.NewButton.Image.Url = "~/Content/images/icons8-plus-math-filled-16.png";
    }
    protected void gvitems_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
    {
        var values = new[] { "7", "11", "13", "14" };
        ASPxGridView g = sender as ASPxGridView;
        if ((DataTable)g.DataSource != null)
        {
            var dt = (DataTable)g.DataSource;
            DataRow row = g.GetDataRow(e.VisibleIndex);
            int index = e.VisibleIndex;
            //var data = g.GetRowValues(index, "Calcu").ToString();
            //if (values.Any(data.ToString().Equals)){ //(data.ToString() == "3")
            //    e.Cell.BackColor = Color.LightGreen;
            //    e.Cell.ForeColor = Color.White;
            //    }
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
    string builPackSize(string Material)
    {
        return cs.ReadItems(
                        @"select top 1 PackSize from StdPackingStyle where sapcodedigit = substring('" + 
                        Material.ToString() + "',17,2)");
    }
    public List<StdItems> GetTableFromExcel(DataTable table)
    {
        List<StdItems> listexcel = new List<StdItems>();
        DataTable dataTable2 = Getutilize(Convert.ToDateTime(defrom.Value).Date,
            Convert.ToDateTime(deto.Value).Date);
        //int maxAge = listItems.Max(t => t.ID);
        //var dt = new DataTable();
        //dt = tcustomer.Clone();
        //int NextRowID = Convert.ToInt32(dt.AsEnumerable()
        //        .Max(row => row["ID"]));
        //int NextRowID = dt.Rows.Count;
        int NextRowID = 0, RowID = 0, ID = 0;
        //int RowID = Convert.ToInt32(_UpChargedt.AsEnumerable()
        //        .Max(row => row["ID"]));
        string strPackSizeMat = "";
        for (int i = 0; i < table.Rows.Count; i++)
        {
            if (i > 1)
            {
                // first record
                if (table.Rows[i]["Column1"].ToString() != "")
                {
                    DataTable strMat = GetSecPKGCost(table.Rows[i]["Column1"].ToString());
                    if (strMat.Rows.Count > 0)
                    {
                        StdItems _row = new StdItems();
                        //DataRow _row = dt.NewRow();
                        NextRowID++;
                        ID = NextRowID;
                        _row.ID = ID;
                        _row.Material = table.Rows[i]["Column1"].ToString();
                        _row.Description = GetDescrip(_row.Material.ToString());
                        //_row.From = Convert.ToDateTime(table.Rows[i]["Column2"]);
                        //_row.To = Convert.ToDateTime(table.Rows[i]["Column3"]);
                        _row.Commission = table.Rows[i]["Column2"].ToString().Replace("%","");
                        _row.OverPrice = table.Rows[i]["Column3"].ToString();
                        _row.OverType = table.Rows[i]["Column4"].ToString();
                        if (string.Format("{0}", _row.Commission) == "" && string.Format("{0}", _row.OverPrice) == "")
                        {
                            DataTable _table = GetCommission();
                            foreach (DataRow _rw in _table.Rows)
                            {
                                _row.Commission = string.Format(@"{0}", _rw["Commission"]);
                                _row.OverPrice = string.Format(@"{0}", _rw["OverPrice"]);
                                _row.OverType = string.Format(@"{0}", _rw["Unit"]);
                            }
                        }
                        _row.SubContainers = table.Rows[i]["Column5"].ToString();

                        _row.Pacifical = "0";
                        _row.MSC = "0";

                        DataTable fishgroup = getfishgroup(_row.Material);
                        foreach (DataRow rw in fishgroup.Rows)
                        {
                            _row.Margin = GetMargin(rw["SHD"].ToString());
                            foreach (DataRow rw2 in GetCertificate(rw["FishGroup"].ToString()).Rows)
                            {
                                if (rw2["Certificate_fee"].ToString() == "MSC")
                                    _row.MSC = rw2["free"].ToString();
                                if (rw2["Certificate_fee"].ToString() == "Pacifical")
                                    _row.Pacifical = rw2["free"].ToString();
                            }
                        }
                        _row.Utilize = (from DataRow dr in dataTable2.Rows
                                        select new TransUtilize()
                                        {
                                            RowID = Convert.ToInt32(dr["RowID"]),
                                            Result = dr["Result"].ToString(),
                                            MonthName = dr["MonthName"].ToString(),
                                            Cost = dr["Cost"].ToString(),
                                            SubID = dr["SubID"].ToString(),
                                            RequestNo = dr["RequestNo"].ToString()
                                        }).ToList();
                        _row.Mark = "X";
                        strPackSizeMat = builPackSize(_row.Material.ToString());
                        //DataRow dr = null;
                        //if (_utilizedt == null)
                        //    _utilizedt = dataTable2.Clone();
                        //foreach (DataRow rw in dataTable2.Rows)
                        //{
                        //    dr = _utilizedt.NewRow(); // have new row on each iteration
                        //    dr = rw;
                        //    int MaxRow = Convert.ToInt32(_utilizedt.AsEnumerable()
                        //        .Max(row => row["RowID"]));
                        //    MaxRow++;
                        //    _utilizedt.Rows.Add(MaxRow, Convert.ToDouble(dr["Result"]), dr["MonthName"], dr["Cost"], NextRowID, "X");
                        //}
                        List<TransUpCharge> _UpCharge = new List<TransUpCharge>();
                        _UpCharge.AddRange(buildUpCharge(_row, NextRowID, strPackSizeMat, _UpCharge));
                        RowID = cs.FindMaxValue(_UpCharge, t => t.RowID);
                        //for (int ii = i; ii < table.Rows.Count; ii++)
                        //{
                        //    if (table.Rows[ii]["Column1"].ToString() != "")
                        //    {
                        //        if (table.Rows[i]["Column1"].ToString() != table.Rows[ii]["Column1"].ToString())
                        //            break;
                        //    }
                        _UpCharge.AddRange(_Insertupcharge(i, RowID, table, strPackSizeMat, NextRowID));
                            //List<string> list = new List<string>();
                            //foreach (DataColumn column in table.Columns)
                            //{
                            //    if (column.ToString() == "Column3")
                            //        if (!string.IsNullOrEmpty(table.Rows[i][column].ToString()))
                            //            list.Add(table.Rows[i][column].ToString());
                            //}
                            //if (list.Count > 0)
                            //    row["Upcharge"] = String.Join("|", list.ToArray());
                            //   }

                        _row.Upcharge = _UpCharge;
                        listexcel.Add(_row);
                    }
                    else
                        table.Rows[i]["Column2"] = "X";
                       // MessageBox.Show(Page,"Here is my message");
                }
                //else
                //{
                //    if (strPackSizeMat != "")
                //    {
                //        var rcus = listexcel.FirstOrDefault(x => x.ID == ID);
                //        if (table.Rows[i]["Column7"].ToString() != "")
                //        {
                //            string strSQl = string.Format(@"select * from StandardUpcharge where UpchargeGroup='{0}' and Upcharge='{1}'",
                //                table.Rows[i]["Column6"].ToString(), table.Rows[i]["Column7"].ToString());
                //            DataTable _checkupcharge = cs.builditems(strSQl);
                //            if (_checkupcharge.Rows.Count > 0)
                //            {
                //                TransUpCharge _rmas = new TransUpCharge();
                //                //DataRow _rmas = _UpChargedt.NewRow();
                //                RowID++;
                //                _rmas.ID = RowID;
                //                _rmas.UpchargeGroup = "";
                //                _rmas.UpCharge = table.Rows[i]["Column7"].ToString();
                //                _rmas.Quantity = strPackSizeMat;
                //                _rmas.Price = _checkupcharge.Rows[0]["Value"].ToString();
                //                _rmas.Result = _checkupcharge.Rows[0]["Value"].ToString();
                //                _rmas.stdPackSize = _checkupcharge.Rows[0]["StdPackSize"].ToString();
                //                _rmas.SubID = NextRowID.ToString();
                //                rcus.Upcharge.Add(_rmas);
                //            }
                //        }
                //        if (table.Rows[i]["Column9"].ToString() != "")
                //        {
                //            TransUpCharge _roth = new TransUpCharge();
                //            //DataRow _roth = _UpChargedt.NewRow();
                //            RowID++;
                //            _roth.ID = RowID;
                //            _roth.UpchargeGroup = "";
                //            _roth.UpCharge = table.Rows[i]["Column9"].ToString();
                //            _roth.Quantity = strPackSizeMat;
                //            _roth.Price = table.Rows[i]["Column10"].ToString();
                //            _roth.Result = _roth.Price.ToString();
                //            _roth.stdPackSize = table.Rows[i]["Column11"].ToString();
                //            _roth.SubID = NextRowID.ToString();
                //            rcus.Upcharge.Add(_roth);
                //        }
                //    }
                //}
                //insert update
            }
        }
        return listexcel;
    }
   List<TransUpCharge>_Insertupcharge(int i,int RowID,DataTable table, string strPackSizeMat,int NextRowID)
    {
        int checkmat = 0;
        List<TransUpCharge> _UpCharge = new List<TransUpCharge>();
        for (int ii = i; ii < table.Rows.Count; ii++)
        {
            if (table.Rows[ii]["Column1"].ToString() != "")
            {
                if(checkmat>0) //if (table.Rows[i]["Column1"].ToString() != table.Rows[ii]["Column1"].ToString())
                    break;
                checkmat++;
            }

            if (table.Rows[ii]["Column7"].ToString() != "")
            {
                string strSQl = string.Format(@"select * from StandardUpcharge where UpchargeGroup='{0}' and Upcharge='{1}'",
                    table.Rows[ii]["Column6"].ToString(), table.Rows[ii]["Column7"].ToString());
                DataTable _checkupcharge = cs.builditems(strSQl);
                if (_checkupcharge.Rows.Count > 0)
                {
                    TransUpCharge _rmas = new TransUpCharge();
                    //DataRow _rmas = _UpChargedt.NewRow();
                    RowID++;
                    _rmas.RowID = RowID;
                    _rmas.UpchargeGroup = "";
                    _rmas.Currency = "";
                    _rmas.UpCharge = string.Format("{0}", table.Rows[ii]["Column7"]);
                    _rmas.Quantity = string.Format("{0}", _checkupcharge.Rows[0]["StdPackSize"]);
                    _rmas.Price = string.Format("{0}", _checkupcharge.Rows[0]["Value"]);
                    _rmas.stdPackSize = string.Format("{0}", strPackSizeMat);
                    _rmas.Result = string.Format("{0}", ((Convert.ToDouble(_rmas.Price) / Convert.ToDouble(_rmas.Quantity)) * Convert.ToDouble(_rmas.stdPackSize)));
                    _rmas.SubID = NextRowID.ToString();
                    _UpCharge.Add(_rmas);
                }
            }
            if (table.Rows[ii]["Column9"].ToString() != "")
            {
                TransUpCharge _roth = new TransUpCharge();
                //DataRow _roth = _UpChargedt.NewRow();
                RowID++;
                _roth.RowID = RowID;
                _roth.UpchargeGroup = "";
                _roth.UpCharge = string.Format("{0}", table.Rows[ii]["Column9"]);
                _roth.Quantity = string.Format("{0}", table.Rows[ii]["Column11"]);
                _roth.Price = string.Format("{0}", table.Rows[ii]["Column10"]);

                _roth.stdPackSize = string.Format("{0}", strPackSizeMat);
                _roth.Result = string.Format("{0}", ((Convert.ToDouble(_roth.Price) / Convert.ToDouble(_roth.Quantity)) * Convert.ToDouble(_roth.stdPackSize)));
                _roth.SubID = NextRowID.ToString();
                _UpCharge.Add(_roth);
            }
        }
        return _UpCharge;
    }
    protected void LoadNewValues(StdItems item, OrderedDictionary values)
    {
        //item.ID = Convert.ToInt32(values["ID"]);
        item.Material = Convert.ToString(values["Material"]);
        item.Description = Convert.ToString(values["Description"] == null ?
           GetDescrip(item.Material) : values["Description"]);
        //item.C4 = Convert.ToBoolean(values["C4"]);
        item.Apply = string.Format("{0}", values["Apply"]);
        item.SubContainers = values["SubContainers"] == null ? "" : values["SubContainers"].ToString();
        if(!string.Format("{0}", item.SubContainers).Equals(""))
        {
            double _SubContainers = 0, sumFreight = 0;
            double Freight = 0;
            List<TransCal> ldata = item.cal;
            var data = ldata.Find(x => x.Component == "Route" && x.Calcu==7);
            //if (data == null)
            //{
            //    int max = ldata.Max(t => t.RowID);
            //    max++;
            //    data = new TransCal();
            //    data.RowID = max;
            //    data.Calcu = 7;
            //    data.Name = "Route";
            //    data.BaseUnit = "";
            //    data.Component = data.Name;
            //    data.Unit = "";
            //    data.Currency = "USD/Case";
            //    data.Quantity = string.Format("{0}", item.SubContainers);
            //    if (double.TryParse(string.Format("{0}", item.SubContainers), out _SubContainers))
            //    {
            //        double.TryParse(tbFreight.Text, out Freight);
            //        sumFreight = (Convert.ToDouble(Freight) / Convert.ToDouble(_SubContainers));
            //        data.Price = string.Format("{0}", Freight);
            //        data.Result = Convert.ToDouble(sumFreight.ToString("F4"));
            //    }
            //    ldata.Add(data);
            //    item.cal = ldata;
            //}
            if (data != null)
            {
                data.Quantity = string.Format("{0}", item.SubContainers);
                if (double.TryParse(string.Format("{0}", item.SubContainers), out _SubContainers))
                {
                    double.TryParse(tbFreight.Text, out Freight);
                    sumFreight = (Convert.ToDouble(Freight) / Convert.ToDouble(_SubContainers));
                    data.Price = string.Format("{0}", Freight);
                    data.Result = Convert.ToDouble(sumFreight.ToString("F4"));
                    //
                    var sumdata = ldata.Find(x => x.Name == "Route" && x.Calcu == 8);
                    //foreach (var sumdata in lsumdata)
                    //{
                    if (sumdata != null)
                    {
                        sumdata.Quantity = data.Quantity;
                        sumdata.Price = string.Format("{0}", Freight);
                        sumdata.Result = Convert.ToDouble(sumFreight.ToString("F4"));
                    }
                    //}
                }
            }
        }

        item.Commission = string.Format("{0}", values["Commission"]);
        item.OverPrice = string.Format("{0}", values["OverPrice"]);
        item.OverType = string.Format("{0}", values["OverType"]);
        //item.IsAccept = Convert.ToBoolean(values["IsAccept"].ToString());
        item.Pacifical = string.Format("{0}", values["Pacifical"] == null ? "" : values["Pacifical"].ToString());
        item.Margin = string.Format("{0}", values["Margin"] == null ? "0" : values["Margin"]);
        item.MSC = string.Format("{0}", values["MSC"] == null ? "" : values["MSC"].ToString());
        item.MinPrice = values["MinPrice"] == null ? "" : values["MinPrice"].ToString();
        item.OfferPrice = values["OfferPrice"] == null ? "" : values["OfferPrice"].ToString();
        item.Authorized_price = values["Authorized_price"] == null ? "" : values["Authorized_price"].ToString();
        if (!string.IsNullOrEmpty(string.Format("{0}", values["Announcement_Fish_price"]).ToString())) {
            decimal Announcement_Fish_price = Convert.ToDecimal(values["Announcement_Fish_price"].ToString());
            item.Announcement_Fish_price = Convert.ToDecimal(values["Announcement_Fish_price"].ToString()).ToString("F2");
        }
        if (!string.IsNullOrEmpty(string.Format("{0}", values["Bidprice"]).ToString()))
        {
            item.Bidprice = Convert.ToDecimal(values["Bidprice"].ToString()).ToString("F2");
        }
        if (cs.ReadItems(@"select convert(nvarchar(max),count(*)) c from StdTunaSpeCase where substring(MatCode,1,16)=substring('" +
            values["Material"] + "',1,16)").ToString() != "0")
        {
            item.Mark = "TG";
            List<TransStdFileDetails> list3 = item.FileDetails;
            TransStdFileDetails result3 = list3.Find(x => x.SubID == item.ID.ToString() && x.Result == "TG EU");
            if (result3 != null)
                item.Mark = "TGA";
            //        TransStdFileDetails f3 = new TransStdFileDetails();
            //        int maxAge = cs.FindMaxValue(list3,t => t.ID);
            //        maxAge++;
            //        f3.ID = maxAge;
            //        f3.Result = "TG EU";
            //        f3.Name = "";
            //        f3.SubID = item.ID.ToString();
            //        f3.RequestNo = "";
            //        list3.Add(f3);
            //    item.FileDetails = list3;

        }
        else
            item.Mark = values["Mark"] == null ? "" : values["Mark"].ToString();
    }
    string valid()
    {
        if (string.IsNullOrEmpty(string.Format("{0}", CmbCustomer.Value)))
            return "1";
        else if (string.IsNullOrEmpty(string.Format("{0}", CmbShipTo.Value)))
            return "1";
        else if (string.IsNullOrEmpty(string.Format("{0}", CmbBillTo.Value)))
            return "1";
        else if (string.IsNullOrEmpty(string.Format("{0}", CmbPaymentTerm.Value)))
            return "1";
        else if (string.IsNullOrEmpty(string.Format("{0}", cmbZone.Value)))
            return "1";
        else
            return "0";
    }
    StdItems getcalcu(StdItems item)
    {
        _dt = GetCalcu(item);
        item.cal = cs.ConvertDataTable<TransCal>(_dt);
        buildcalcu(item);

        return item;
    }
    protected void Upload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
    {
        if (valid()=="0")
        {
            //string dirVirtualPath = @"\\192.168.1.193\XlsTables";
            string msgbox = "";
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
                DataTable _table = sheet.CreateDataTable(range, false);
                DataTableExporter exporter = sheet.CreateDataTableExporter(range, _table, false);
                exporter.CellValueConversionError += exporter_CellValueConversionError;
                exporter.Export();
                //if (_utilizedt != null)
                //    _utilizedt.Rows.Clear();
                //if (_UpChargedt != null)
                //    _UpChargedt.Rows.Clear();
                //this.Session.Remove("utilizedt");
                //this.Session.Remove("UpChargedt");
                //tcustomer = new DataTable();
                listItems = new List<StdItems>();
                listItems = GetTableFromExcel(_table);
                foreach (var item in listItems)
                {
                    _dt = GetCalcu(item);
                    //if (_dt != null)
                    //{
                        item.cal = cs.ConvertDataTable<TransCal>(_dt);
                        buildcalcu(item);
                    
                        item.FileDetails = new List<TransStdFileDetails>();
                    //}
                }
                //tcustomer.PrimaryKey = new DataColumn[] { tcustomer.Columns["ID"] };
                //string strSQL = @"MasExchangeRat where (cast(getdate() as date) between cast(Validfrom as date) ";
                //strSQL+="and cast(validto as date)) and Company='" + CmbCompany.Text + "'";
                //SqlParameter[] param = { new SqlParameter("@Id", CmbCostingNo.Value) };
                //var myresult = cs.GetRelatedResources("spExchangeRat", param); string ExchangeRate = "0";
                //if (myresult.Rows.Count > 0)
                //    ExchangeRate = myresult.Rows[0]["Rate"].ToString();
                //Session.Remove("CustomTable");
                //dataTable = new DataTable();
                //dataTable = GetTableFromExcel(table);
                //dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["RowID"] };
                //string Name = ""; string RefSamples = ""; string Code = "";
                //string[] arr = { CanSize, RDNumber, ExchangeRate.ToString(), Name, RefSamples, Code };
                //e.CallbackData = string.Join("|", arr);
                //for (int i = 0; i < dataTable.Rows.Count; i++)
                //{
                //    DataRow row = _found(dataTable.Rows[i]);
                //}
                var row = _table.Select("Column2='X'");
                foreach (DataRow dr in row)
                {
                    msgbox = msgbox + "\n" + dr["Column1"].ToString();
                    //if (dr["Column1"].ToString() != "" && dr["Column1"].ToString() != "Product Code")
                    //{
                    //    string strMat = cs.ReadItems(
                    //    @"select distinct a.Material as 'c' from StdSecPKGCost a where a.Material = '" + dr["Column1"].ToString() + "'");
                    //    if (string.IsNullOrEmpty(strMat))
                    //    {
                    //        msgbox = msgbox + "\n" + dr["Column1"].ToString();
                    //    }
                    //}
                }
            }
        if (msgbox.Length > 0)
            e.CallbackData = msgbox;
        }
    }
    //void buildSelect_FileDetails()
    //{
    //    if (USP_Select_FileDetails == null)
    //    {
    //        SqlParameter[] param = {
    //                    new SqlParameter("@tablename", "(select top 0 * from TransStdFileDetails)#a")};
    //        DataTable table = cs.GetRelatedResources("spGetElement", param);
    //        table.PrimaryKey = new DataColumn[] { table.Columns["ID"] };
    //        USP_Select_FileDetails = table.Clone();
    //    }
    //}
    protected void gridData_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        var args = e.Parameters.Split('|');
        long id;
        if (!long.TryParse(args[2], out id))
            return;

        var result = new Dictionary<string, string>();
        DataTable dt = new DataTable();
        if (args[1] == "save")
        {
            //
            result["Mark"] = "";
            var obj = listItems.FirstOrDefault(x => x.Mark == "TG");
            if (obj != null)
                result["Mark"] = obj.Mark;
            e.Result = result;
        }
        else if (args[1] == "New")
        {
            //BuildSelect_FileDetails();
            //hfgetvalue["NewID"] = string.Format("{0}", cs.GetNewID());
            result["NewID"] = (string)cs.GetNewID();
            result["editor"] = cs.IsMemberOfRole(string.Format("{0}", 0));
            //result["ExchangeRate"] = string.Format("{0}", exchangerate());
            foreach (DataRow _rwx in exchangerate().Rows)
            {
                result["ExchangeRate"] = _rwx["Rate"].ToString();
                result["Currency"] = _rwx["Currency"].ToString();
            }
            e.Result = result;
        }
        if (args[1] == "IsPaid" || args[1] == "Comment")
        {
            //bool value = false;
        }
        if (args[1] == "ExportToXLS" || args[1] == "Quotation" || args[1] == "calculation")
        {
            result["print"] = String.Format("{0}", "4");
            result["view"] = String.Format("{0}", "4");
            result["command"] = String.Format("{0}", args[1]);
            result["KeyValue"] = String.Format("{0}", hGeID["GeID"]);
            e.Result = result;
        }
        if (args[1] == "EditDraft" || args[1] == "Clone")
        {
            //BuildSelect_FileDetails();
            SqlParameter[] param = {new SqlParameter("@RequestNo",args[2].ToString()),
                    new SqlParameter("@value","Customer".ToString()),
                    new SqlParameter("@username",cs.CurUserName.ToString())};
            dt = cs.GetRelatedResources("spGetTunaCustomer", param);
            foreach (DataRow rw in dt.Rows)
            {

                if (args[1] == "EditDraft")
                {
                    result["NewID"] = string.Format("{0}", rw["UniqueColumn"]);
                    result["RequestNo"] = string.Format("{0}", rw["RequestNo"]);
                    result["editor"] = rw["editor"].ToString();
                    result["approv"] = string.Format("{0}", rw["approv"]);
                    result["Currency"] = string.Format("{0}", rw["Currency"]);
                    result["ExchangeRate"] = string.Format("{0}", rw["ExchangeRate"]);
                }
                else
                {
                    result["NewID"] = (string)cs.GetNewID();
                    result["editor"] = cs.IsMemberOfRole(string.Format("{0}", 0));
                    
                    //result["ExchangeRate"] = string.Format("{0}", exchangerate());
                    foreach (DataRow _rwx in exchangerate().Rows)
                    {
                        result["ExchangeRate"] = _rwx["Rate"].ToString();
                        result["Currency"] = _rwx["Currency"].ToString();
                    }
                }
                result["StatusApp"] = string.Format("{0}", rw["StatusApp"]);
                result["ID"] = string.Format("{0}", rw["ID"]);
                result["_Freight"] = string.Format("{0}", rw["_Freight"]);
                result["_ValidityDate"] = string.Format("{0}", rw["_ValidityDate"]);
                result["_ExchangeRate"] = string.Format("{0}", rw["_ExchangeRate"]);
                result["Customer"] = string.Format("{0}", rw["Customer"]);
                result["ShipTo"] = string.Format("{0}", rw["ShipTo"]);
                result["BillTo"] = string.Format("{0}", rw["BillTo"]);
                result["From"] = string.Format("{0}", rw["From"]);
                result["To"] = string.Format("{0}", rw["To"]);
                result["ValidityDate"] = string.Format("{0}", rw["ValidityDate"]);
                result["Incoterm"] = string.Format("{0}", rw["Incoterm"]);
                result["Zone"] = string.Format("{0}", rw["Zone"]);
                result["PaymentTerm"] = string.Format("{0}", rw["PaymentTerm"]);

                result["Insurance"] = string.Format("{0}", rw["Insurance"]);
                result["Interest"] = string.Format("{0}", rw["Interest"]);
                result["Size"] = string.Format("{0}", rw["Size"]);
                result["Freight"] = string.Format("{0}", rw["Freight"]);
                result["Route"] = string.Format("{0}", rw["Route"]);
                result["Remark"] = string.Format("{0}", rw["Remark"]);
                result["FreightOld"] = "";
                result["Eventlog"] = string.Format("{0}", DisplayEventlog(e));
                if (rw["Incoterm"].ToString() == "CIF" || rw["Incoterm"].ToString() == "CRF")
                {
                    string[] value = new string[] { rw["Route"].ToString(), rw["Size"].ToString(), rw["Customer"].ToString()
                        , rw["ShipTo"].ToString() };
                    DataTable dtFreight = GetselectFreight(value);
                    foreach (DataRow dr in dtFreight.Rows)
                    {
                        string FreightOld = string.Format("{0}", Convert.ToDouble(dr["MKTCost"]) * 0.0011);
                        if(args[1] == "Clone")
                            result["FreightOld"] = string.Format("{0}", FreightOld);
                        else if (rw["Freight"].ToString() != FreightOld)
                            result["FreightOld"] = string.Format("{0}", FreightOld);
                    }
                }
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
            string strSQL = @"select CreateBy,RequestNo" +
                " ,b.Title  from TransTunaStd a left join MasStatusApp b on b.Id = a.StatusApp where a.Id='" + id + "' and b.levelapp in (4)";
            DataTable dt = cs.builditems(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                var arr = dr["CreateBy"].ToString().Split('|'); string s = "";
                sb.Append(string.Format("StatusApp :{0}", dr["Title"].ToString()));
                for (int i = 0; i < arr.Length; i++)
                {
                    s += cs.GetData(arr[i], "fn") + ",";
                }
                sb.Append(string.Format("<div><b><u>Create By : {0}</u></b></div>", s));
            }
            var Results = new DataTable();//spGetHistory
            SqlParameter[] param = { new SqlParameter("@Id", id),
                new SqlParameter("@tablename",string.Format("{0}","TransTunaStd")),
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
    void exportto()
    {
        string name = "offer";
        ASPxSpreadsheet spreadsheet = new ASPxSpreadsheet();
        string path = Server.MapPath(@"~/App_Data/Documents/offer5_.xlsx");
        spreadsheet.Document.LoadDocument(path);
        XLWorkbook wb = new XLWorkbook();
        Worksheet ws;
        ws = spreadsheet.Document.Worksheets[0];
        DataTable _table = gv.DataSource as DataTable;
        int i = 2;
        foreach (DataRow r in _table.Rows)
        {
            int a = 0;

            string[] arrs = r["MinPrice"].ToString().Split('|');
            string[] args = r["OfferPrice"].ToString().Split('|');
            for (int x = 0; x < args.Length; x++)
            {
                a++;
                //string strcost = string.Concat(r["RequestNo"], a.ToString("00"));
                DataTable dt = new DataTable();
                dt = cs.builditems(string.Format(@"
                    select * from TransFormulaHeader where Formula={0} and RequestNo={1}", a.ToString(), r["RequestNo"]));

                ws.Cells["A" + i].Value = string.Format("{0}", dt.Rows[0]["Name"]);
                ws.Cells["B" + i].Value = string.Format("{0}", dt.Rows[0]["Code"]);
                ws.Cells["E" + i].Value = string.Format("{0}", dt.Rows[0]["RefSamples"]);
                ws.Cells["G" + i].Value = string.Format("{0}", r["Incoterm"].ToString() == "FOB" ? arrs[x] : args[x]);
                ws.Cells["F4" + i].Value = string.Format("{0}", dt.Rows[0]["CostNo"]);
                ws.Cells["C" + i].Value = r["Customer"].ToString();
                ws.Cells["D" + i].Value = r["ShipTo"].ToString();
                i++;
            }
        }
        var st = new MemoryStream();
        spreadsheet.Document.SaveDocument(st, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
        Response.Clear();
        Response.ContentType = "application/force-download";
        String fileName = String.Format(name + "_{0}.xlsx", DateTime.Now.ToString("yyyyMMddhhmmss"));
        Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
        Response.BinaryWrite(st.ToArray());
        Response.End();
    }
    protected void btexport_Click(object sender, EventArgs e)
    {
        exportto();
    }
    //void downloadfile(string Id)
    //{
    //            try
    //            {
    //                DataTable dt = cs.builditems("SELECT * FROM transstdFileDetails where ID=" + Id);
    //                foreach (DataRow dr in dt.Rows)
    //                {
    //                    char[] charsToTrim = { '*', ' ', '\'', ',' };
    //                    string result = dr["Name"].ToString().Trim(charsToTrim);
    //                    Response.Clear();
    //                    Response.Buffer = true;
    //                    Response.Charset = "";
    //                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
    //                    Response.ContentType = MimeTypes.GetContentType(dr["Name"].ToString()); ;
    //                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + result.Replace(",", "_"));
    //                    Response.BinaryWrite((byte[])dr["Attached"]);
    //                    Response.Flush();
    //                    Response.End();
    //                }
    //            }
    //            catch (Exception Ex)
    //            {
    //                throw new ApplicationException(Ex.Message);
    //            }
    //}
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

    //protected void gvitems_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
    //{
    //    ASPxGridView g = (ASPxGridView)sender;
    //    e.Cancel = true;
    //    g.CancelEdit();
    //}

    void insertsGrid(string p, string text)
    {

        //if (_dt == null) return;
        //if (_dt.Rows.Count == 0) return;
        //DataRow rw = _dt.NewRow();
        //int iMax = 600;
        //foreach (DataRow oneNewrow in _dt.Select("Calcu = " + p.ToString()))
        //{
        //    if (Convert.ToInt32(oneNewrow["RowID"]) != 699)
        //        if (iMax < Convert.ToInt32(oneNewrow["RowID"]))
        //            iMax = Convert.ToInt32(oneNewrow["RowID"]);
        //}
        //iMax++;
        switch (p.ToString())
        {
            //case "6":
            //    //if (_UpChargedt == null){
            //    //    _UpChargedt = new DataTable(_dt.TableName);
            //    //    _UpChargedt = _dt.Clone();
            //    //}
            //    double UpChargePrice, totalUpChargePrice = 0, UpChargeUnit = 0;
            //    if (Double.TryParse(tbUpChargePrice.Text, out UpChargePrice))
            //    {
            //        //if (tbUpChargeUnit.Text.ToLower() == "can")
            //        //{
            //        //    DataRow _result = _dt.Select("Component = 'Packing Style'").FirstOrDefault();
            //        Double.TryParse(tbUpChargeUnit.Text, out UpChargeUnit);

            //        //DataRow _r = _dt.Select("Component in ('Packaging') and Calcu=5 and name like '%Secondary Packaging per case%'").FirstOrDefault();
            //        string packsize = GetPackSize(_dt).ToString();
            //        totalUpChargePrice = ((UpChargePrice / UpChargeUnit) * Convert.ToDouble(packsize.ToString())) * 1;
            //        //}
            //        _dt.Rows.Add(iMax++, hfUpchargeGroup["Upcharge"] + "UpCharge", CmbUpCharge.Text, "USD/Case", totalUpChargePrice.ToString("F4"), 6,
            //            1,
            //            tbUpChargePrice.Text, tbUpChargeUnit.Text, packsize.ToString());
            //        DataRow[] _rowdt = _dt.Select("Name = 'Upcharge' and Calcu in (8,9)");
            //        foreach (DataRow _result in _rowdt)
            //        {
            //            _result["Result"] = Convert.ToDecimal(_result["Result"]) + Convert.ToDecimal(totalUpChargePrice);
            //            _result["Quantity"] = 1;
            //            _result["Price"] = Convert.ToDecimal(_result["Price"]) + Convert.ToDecimal(tbUpChargePrice.Text);
            //        }
            //    }
            //    break;
            case "6":
                int iMax = Convert.ToInt32(_dt.AsEnumerable()
                        .Max(row => row["RowID"]));
                DataRow rw = _dt.NewRow();
                iMax++;
                rw["RowID"] = iMax;
                rw["Calcu"] = GetIndex;
                rw["Component"] = "UpCharge";
                _dt.Rows.Add(rw);
                break;
            case "7":
                int NextRowID = Convert.ToInt32(_dt.AsEnumerable()
                    .Max(row => row["RowID"]));
                NextRowID++;
                double NumberContainer;
                if (Double.TryParse(text, out NumberContainer))
                {
                    if (NumberContainer == 0)
                    {
                        //delete
                        var rows = _dt.Select("Component = 'Route'");
                        foreach (var row in rows)
                            row.Delete();
                        //upsummary
                        var uprows = _dt.Select("Name = 'Route'");
                        foreach (var row in uprows)
                        {
                            row["Result"] = "0";
                            row.AcceptChanges();
                        }
                    }
                    else
                    {
                        double interest = 0, Freight = 0, Insurance = 0;
                        double sumFreight = 0, minprice = 0;
                        object objminprice = _dt.Compute("Sum(Result)", "Name in ('FOB price(18 - digits)','Value Commission') and Calcu=8");
                        if (!string.IsNullOrEmpty(tbFreight.Text) || !string.IsNullOrEmpty(text))
                            double.TryParse(tbFreight.Text, out Freight);
                        double.TryParse(tbinterest.Text, out interest);
                        sumFreight = (Convert.ToDouble(Freight) / Convert.ToDouble(text));
                        double A = 0, B = 0, interrest = 0;
                        object _commission = _dt.Compute("Sum(Result)", "Name in ('Value Commission') and Calcu=8");
                        double.TryParse(objminprice.ToString(), out minprice);
                        double.TryParse(tbinterest.Text, out interest);
                        interrest = (Convert.ToDouble(minprice) + Convert.ToDouble(_commission) + sumFreight) * (Convert.ToDouble(interest) / 100);
                        A = Convert.ToDouble(minprice) + sumFreight + interrest;
                        if (double.TryParse(tbInsurance.Text, out Insurance))
                            B = Convert.ToDouble(string.Format("{0:#,##0.####}", A * (Insurance == 0 ? 0 : Convert.ToDouble(0.0011))));
                        //B = Convert.ToDouble(string.Format("{0:#,##0.####}", A * (_Insurance==0?0:Convert.ToDouble(0.5/100))));
                        //OfferPrice = minprice + totalCount + interrest + sumFreight + Convert.ToDouble(B);
                        DataRow _result = _dt.Select("Component = 'Route'").FirstOrDefault();
                        if (_result != null)
                        {
                            _result["Quantity"] = NumberContainer;
                            _result["Result"] = sumFreight.ToString("F4");
                        }
                        var rowsToUpdate = _dt.AsEnumerable().Where(r => r.Field<string>("Component") == "Route").FirstOrDefault();
                        if (rowsToUpdate == null)
                        {
                            _dt.Rows.Add(NextRowID++, "Route", CmbRoute.Text, "", sumFreight.ToString("F4"), 7, NumberContainer, Freight, 0, "USD/Case");
                        }
                        else
                        {
                            rowsToUpdate.SetField("Quantity", NumberContainer.ToString("F4"));
                        }
                        DataRow[] _dr = _dt.Select("Name in ('Route','Insurance') and Calcu in (8,9)");
                        foreach (DataRow _r in _dr)
                        {
                            switch (_r["Name"].ToString())
                            {
                                case "Route":
                                    _r["Result"] = sumFreight.ToString("F4");
                                    break;
                            }
                        }
                    }
                }
                break;
        }
    }
    DataTable GetselectFreight(string[] args)
    {
        SqlParameter[] param = {new SqlParameter("@Route",args[0].ToString()),
                new SqlParameter("@Container",args[1].ToString()),
                new SqlParameter("@Customer",args[2].ToString()),
                new SqlParameter("@ShipTo",args[3].ToString())};
        DataTable dt = cs.GetRelatedResources("spGetselectFreight", param);
        return dt;
    }
    protected void gvitems_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //bool isOddRow = e.VisibleIndex % 2 == 0;
        string Result = string.Format("{0}", g.GetRowValues(e.VisibleIndex, "Component"));
        //bool isOddRow = string.Format("{0}", Result) != "Upcharge" && !string.IsNullOrEmpty(Result);
        bool isOddRow = string.Format("{0}", Result) != "Upcharge";
        if (e.ButtonID == "EditCost" && isOddRow)
        {
            //e.Image.Url = "~/Content/Images/Refresh.png";
            e.Visible = DevExpress.Utils.DefaultBoolean.False;
            e.Image.ToolTip = "";
        }
    }

    void post(DataRow dr)
    {
        string ID = string.Format("{0}", dr["ID"]);
        DelUtilize(ID);
        deleteUpCharge(ID.ToString());
        deleteFileDetails(ID.ToString());
        if(hfStatusApp["StatusApp"].ToString().Equals("8") || hfStatusApp["StatusApp"].ToString().Equals("9"))
        for (int i = 0; i < gv.VisibleRowCount; i++)
        {
                object SubID = gv.GetRowValues(i, gv.KeyFieldName);
                var sel = gv.Selection.IsRowSelected(i);
                if (sel)
                {
                    var obj = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(SubID));
                    if (obj != null)
                        obj.isapprove = hfStatusApp["StatusApp"].ToString();               
                }

        }
            foreach (var rw in listItems)
        {
            //
            //var _table = cs.builditems("select * from transTunaStdItems where id='" + rw.ID.ToString() + "'");
            //foreach(DataRow _rw in _table.Rows)
            //{
            //}
            //if(hfStatusApp["StatusApp"].ToString().Equals("0") || hfStatusApp["StatusApp"].ToString().Equals("-1"))
            using (SqlConnection con = new SqlConnection(strConn))
            {
                string query = "spInsertTunaStdItems";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", rw.ID.ToString());
                    cmd.Parameters.AddWithValue("@Material", rw.Material.ToString());
                    cmd.Parameters.AddWithValue("@Name", rw.Description.ToString());
                    List<TransUtilize> _Utilize = new List<TransUtilize>();
                    //var array = rw.Utilize.ToArray(); 

                    cmd.Parameters.AddWithValue("@Utilize", string.Format("{0}", rw.isapprove));
                    cmd.Parameters.AddWithValue("@From", defrom.Value);
                    cmd.Parameters.AddWithValue("@To", deto.Value);
                    cmd.Parameters.AddWithValue("@subContainers", rw.SubContainers);
                    cmd.Parameters.AddWithValue("@RawMaterial", string.Format("{0}", rw.RawMaterial));
                    cmd.Parameters.AddWithValue("@Media", string.Format("{0}", rw.Media));
                    cmd.Parameters.AddWithValue("@Packaging", string.Format("{0}", rw.Packaging));
                    cmd.Parameters.AddWithValue("@LOHCost", string.Format("{0}", rw.LOHCost));
                    cmd.Parameters.AddWithValue("@PackingStyle", string.Format("{0}", rw.PackingStyle));

                    cmd.Parameters.AddWithValue("@Upcharge", string.Format("{0}", rw.totalUpcharge));
                    cmd.Parameters.AddWithValue("@PackSize", string.Format("{0}", rw.PackSize));
                    cmd.Parameters.AddWithValue("@Yield", string.Format("{0}", rw.Yield));
                    cmd.Parameters.AddWithValue("@FillWeight", string.Format("{0}", rw.FillWeight));
                    cmd.Parameters.AddWithValue("@SecPackaging", string.Format("{0}", rw.SecPackaging));

                    cmd.Parameters.AddWithValue("@RequestNo", ID.ToString());
                    cmd.Parameters.AddWithValue("@Commission", rw.Commission);
                    cmd.Parameters.AddWithValue("@OverPrice", rw.OverPrice);
                    cmd.Parameters.AddWithValue("@OverType", rw.OverType);
                    cmd.Parameters.AddWithValue("@Pacifical", rw.Pacifical);
                    cmd.Parameters.AddWithValue("@MSC", rw.MSC);
                    cmd.Parameters.AddWithValue("@Margin", rw.Margin);
                    cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", rw.MinPrice));
                    cmd.Parameters.AddWithValue("@OfferPrice", string.Format("{0}", rw.OfferPrice));
                    cmd.Parameters.AddWithValue("@IsAccept", string.Format("{0}", rw.IsAccept));
                    cmd.Parameters.AddWithValue("@SpecialFishPrice", string.Format("{0}", rw.Announcement_Fish_price));
                    cmd.Parameters.AddWithValue("@Authorized_price", string.Format("{0}", rw.Authorized_price));
                    //if(rw.Apply == "Apply")
                    //{
                    //    cmd.Parameters.AddWithValue("@Bidprice", string.Format("{0}", rw.OfferPrice));
                    //}else{
                        
                    //    cmd.Parameters.AddWithValue("@Bidprice", string.Format("{0}", rw.Bidprice));
                    //}
                    cmd.Parameters.AddWithValue("@Bidprice", string.Format("{0}", 0));
                    cmd.Parameters.AddWithValue("@MarginBid", string.Format("{0}", rw.marginBid));
                    cmd.Parameters.AddWithValue("@Mark", rw.Mark);
                    con.Open();
                    //cmd.ExecuteNonQuery();
                    var getValue = cmd.ExecuteScalar();
                    con.Close();
                    //DataRow[] rwUpCharge = _UpChargedt.Select(string.Format("SubID ='{0}'", rw["ID"].ToString()));
                    List<TransUpCharge> rwUpCharge = rw.Upcharge;
                    if (rwUpCharge != null)
                    {
                        foreach (var row in rwUpCharge)
                        {
                            SqlParameter[] param = new SqlParameter[] {new SqlParameter("@ID",string.Format("{0}", row.RowID)),
                        new SqlParameter("@UpchargeGroup", string.Format("{0}", row.UpchargeGroup)),
                        new SqlParameter("@Upcharge", string.Format("{0}", row.UpCharge)),
                        new SqlParameter("@Price", string.Format("{0}", row.Price)),
                        new SqlParameter("@Quantity", string.Format("{0}", row.Quantity)),
                        new SqlParameter("@Currency", string.Format("{0}", row.Currency)),
                        new SqlParameter("@Result", string.Format("{0}", row.Result)),
                        new SqlParameter("@stdPackSize", string.Format("{0}", row.stdPackSize)),
                        new SqlParameter("@Mark", string.Format("{0}", "")),//Header
                        new SqlParameter("@SubID", string.Format("{0}", getValue.ToString())),
                        new SqlParameter("@RequestNo", string.Format("{0}", ID))};
                            cs.GetExecuteNonQuery("spinsertstdUpCharge", param);
                        }
                    }
                    
                    List<TransCal> rwCal = rw.cal;
                    if (rw.cal != null)
                    {
                        SqlCommand sqlComm = new SqlCommand();
                        sqlComm.CommandText = @"DELETE FROM transtunacal WHERE SubID=@ID and RequestNo=@RequestNo";
                        sqlComm.Parameters.AddWithValue("@ID", getValue.ToString());
                        sqlComm.Parameters.AddWithValue("@RequestNo", ID.ToString());
                        DataTable table = cs.GetSqlCommand(sqlComm);
                        //DataRow[] rwutilize = _utilizedt.Select(string.Format("SubID ='{0}'", rw["ID"].ToString()));
                        foreach (var _rw in rwCal)
                        {
                            SqlParameter[] param = new SqlParameter[] {new SqlParameter("@RowID",string.Format("{0}", _rw.RowID)),
                        new SqlParameter("@Component", string.Format("{0}", _rw.Component)),
                        new SqlParameter("@Name", string.Format("{0}", _rw.Name)),
                        new SqlParameter("@Currency", string.Format("{0}", _rw.Currency)),
                        new SqlParameter("@Result", string.Format("{0}", _rw.Result)),
                        new SqlParameter("@Calcu", string.Format("{0}", _rw.Calcu)),
                        new SqlParameter("@Quantity", string.Format("{0}", _rw.Quantity)),
                        new SqlParameter("@Price", string.Format("{0}", _rw.Price)),
                        new SqlParameter("@Unit", string.Format("{0}", _rw.Unit)),
                        new SqlParameter("@BaseUnit", string.Format("{0}", _rw.BaseUnit)),

                        new SqlParameter("@Mark", string.Format("{0}", "")),//Header
                        new SqlParameter("@SubID", string.Format("{0}", getValue.ToString())),
                        new SqlParameter("@RequestNo", string.Format("{0}", ID))};
                            cs.GetExecuteNonQuery("spInsertTunaCal", param);
                        }
                    }
                    List<TransUtilize> rwutilize = rw.Utilize;
                    if (rw.Utilize != null)
                    {
                        //DataRow[] rwutilize = _utilizedt.Select(string.Format("SubID ='{0}'", rw["ID"].ToString()));
                        foreach (var row in rwutilize)
                        {
                            SqlParameter[] param = new SqlParameter[] {new SqlParameter("@ID",string.Format("{0}", row.RowID)),
                        new SqlParameter("@Result", string.Format("{0}", row.Result)),
                        new SqlParameter("@Monthname", string.Format("{0}", row.MonthName)),
                        new SqlParameter("@Cost", string.Format("{0}", row.Cost)),
                        new SqlParameter("@Mark", string.Format("{0}", "")),//Header
                        new SqlParameter("@SubID", string.Format("{0}", getValue.ToString())),
                        new SqlParameter("@RequestNo", string.Format("{0}", ID))};
                            cs.GetExecuteNonQuery("spInsertUtilize", param);
                        }
                    }

                    List<TransStdFileDetails> FileDetails = rw.FileDetails;
                    foreach (var r in FileDetails)
                    {
                        //byte[] FileData = null;
                        SqlParameter[] param = new SqlParameter[] {new SqlParameter("@Name", string.Format("{0}",r.Name.ToString())),
                        new SqlParameter("@Result", string.Format("{0}",r.Result)),
                        new SqlParameter("@Notes", string.Format("{0}",r.Notes)),
                        new SqlParameter("@IsApprove",  string.Format("{0}", r.IsApprove)),
                        new SqlParameter("@Attached", (byte[])r.Attached),
                        new SqlParameter("@user", cs.CurUserName.ToString()),
                        new SqlParameter("@SubID", string.Format("{0}", getValue.ToString())),
                        new SqlParameter("@RequestNo", string.Format("{0}", ID))};
                        cs.GetExecuteNonQuery("spInsertStdFileDetails", param);
                    }
                    
                }
            }
        }
        //if (_dtcomponent != null)
        //    foreach (DataRow row in _dtcomponent.Rows)
        //    {
        //        SqlParameter[] param = new SqlParameter[] {new SqlParameter("@Result", string.Format("{0}", row["Result"])),
        //                new SqlParameter("@Component", string.Format("{0}", row["Component"])),
        //                new SqlParameter("@Price", string.Format("{0}", row["Price"])),
        //                new SqlParameter("@Unit", string.Format("{0}", "")),
        //                new SqlParameter("@SubID", string.Format("{0}", row["SubID"].ToString())),
        //                new SqlParameter("@RequestNo", string.Format("{0}", ID))};
        //        cs.GetExecuteNonQuery("spInsertComponent", param);
        //    }
        if (USP_Select_FileDetails != null)
            foreach (DataRow row in USP_Select_FileDetails.Rows)
            {
                var values = new[] { "MSC", "Pacifical", "Margin" };
                if (!values.Any(row["Result"].ToString().Equals))
                {
                    //byte[] FileData = null;
                    SqlParameter[] param = new SqlParameter[] {new SqlParameter("@Name", string.Format("{0}",row["Name"])),
                        new SqlParameter("@Result", string.Format("{0}",row["Result"])),
                        new SqlParameter("@Notes", string.Format("{0}",row["Notes"])),
                        new SqlParameter("@IsApprove", ""),
                        //new SqlParameter("@Attached", (byte[])FileData),
                        new SqlParameter("@Attached", row["Attached"]),
                        new SqlParameter("@user", string.Format("{0}",cs.CurUserName)),
                        new SqlParameter("@SubID", string.Format("{0}", row["SubID"])),
                        new SqlParameter("@RequestNo", string.Format("{0}", ID))};
                    cs.GetExecuteNonQuery("spInsertStdFileDetails", param);
                }
            }
    }
    void updatesel(string Id)
    {
        for (int i = 0; i < gv.VisibleRowCount; i++)
        {
            //var selectedRowsKeys = gv.GetSelectedFieldValues(gv.KeyFieldName);
            object keys = gv.GetRowValues(i, gv.KeyFieldName);
            var sel = gv.Selection.IsRowSelected(i);
            if (sel == true)
            {
                DataRow[] xdr = USP_Select_FileDetails.Select(string.Format("SubID={0}", keys.ToString()));
                foreach(var usp in xdr) { 
                    if(usp["Result"].Equals("Margin"))
                        usp["IsApprove"] = "0";
                }
                using (SqlConnection con = new SqlConnection(strConn))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "Update TransStdFileDetails set IsApprove=0,LastUpdateBy=@LastUpdateBy where SubID=@SubID and RequestNo=@ID";
                    cmd.Parameters.AddWithValue("@SubID", keys.ToString());
                    cmd.Parameters.AddWithValue("@LastUpdateBy", cs.CurUserName.ToString());
                    cmd.Parameters.AddWithValue("@ID", Id.ToString());
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
    }
    public decimal FindDifference(decimal nr1, decimal nr2)
    {
        return Math.Abs(nr1 - nr2);
    }
    protected void gvitems_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        if (e.ButtonID == "colDel")
        {
            object keyValue = g.GetRowValues(e.VisibleIndex, g.KeyFieldName);


            //DataRow found = _UpChargedt.Rows.Find(keyValue);
            //found["UpchargeGroup"] = found["UpchargeGroup"].ToString() == "D" ? "" : "D";
            //_UpChargedt.AcceptChanges();
            var item = listItems.First(i => i.ID == Convert.ToInt32(separa));

            DataRow rw = _dt.Rows.Find(keyValue);
            rw.Delete();
            //rw["Component"] = rw["Component"].ToString() == "Del" ? "" : "Del";
            List<TransUpCharge> listup = item.Upcharge;
            //var itemToRemove = listup.SingleOrDefault(r => r.ID == Convert.ToInt32(keyValue));
            //if (itemToRemove != null)
            //{
            //    listup.Remove(itemToRemove);
            //    item.Upcharge = listup;
            //}
            DataRow[] rowupcharge = _dt.Select("Calcu in (6)");
            int co = rowupcharge.Length;
            if (co > 0)
            {
                item.Upcharge = (from DataRow xdr in rowupcharge
                                 select new TransUpCharge()
                                 {
                                     RowID = Convert.ToInt32(xdr["RowID"]),
                                     UpchargeGroup = xdr["Component"].ToString(),
                                     UpCharge = xdr["Name"].ToString(),
                                     Price = xdr["Price"].ToString(),
                                     Quantity = xdr["Unit"].ToString(),
                                     Currency = xdr["Currency"].ToString(),
                                     Result = xdr["Result"].ToString(),
                                     stdPackSize = xdr["BaseUnit"].ToString()
                                     //SubID = xdr["SubID"].ToString(),
                                     //RequestNo = xdr["RequestNo"].ToString()
                                 }).ToList();
            }
            else
            {
                item.Upcharge = new List<TransUpCharge>();
            }

            //buildcalcu(rcus);
            ////GridData.Remove(item);
            ////int Index = gv.EditingRowVisibleIndex;
            ////object SubID = gv.GetRowValues(Index, gv.KeyFieldName);
            //DataRow rwupcharge = _UpChargedt.Select(string.Format("UpCharge in ('{0}') and SubID ='{1}' and RequestNo ='{2}'",
            //rw["Name"], keyValue, hGeID["GeID"])).FirstOrDefault();
            //if (rwupcharge != null)
            //    _UpChargedt.Rows.Remove(rwupcharge);
            ////deleteUpCharge(keyValue);
            //_dt.Rows.Remove(rw);
            //_dt.AcceptChanges();
            //decimal _totupcharge = 0, _Quantityupch = 0, _Priceupch = 0;
            //DataRow[] arw = _dt.Select(string.Format("Calcu ='{0}'", 6));
            //foreach (DataRow r in arw)
            //{
            //    _Priceupch += Convert.ToDecimal(r["Price"].ToString());
            //    _Quantityupch += Convert.ToDecimal(r["Quantity"].ToString());
            //    _totupcharge += Convert.ToDecimal(r["Result"].ToString());
            //}
            //DataRow _result = _dt.Select("Name = 'Upcharge' and Calcu=8").FirstOrDefault();
            //_result["Result"] = _totupcharge.ToString("F4");
            //_result["Quantity"] = _Quantityupch.ToString("F4");
            //_result["Price"] = _Priceupch.ToString("F4");
        }
        if (e.ButtonID == "colUpload")
        {

        }
        g.JSProperties["cpKeyValue"] = e.ButtonID.ToString();
        //g.DataBind();
    }

    protected void gridData_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        if (args[0] == "Checked")
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Update TransTunaStd set Flag=@Flag where Id=@ID";
                cmd.Parameters.AddWithValue("@Flag", string.Format("{0}", args[2]=="false"?"0":"1"));
                cmd.Parameters.AddWithValue("@ID", string.Format("{0}", args[1]));
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        if (args[0] == "post")
        {
            //

            //string value = "";
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spInsertTunaStd";
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", args[1]));
                cmd.Parameters.AddWithValue("@User", cs.CurUserName.ToString());
                cmd.Parameters.AddWithValue("@Incoterm", string.Format("{0}", CmbIncoterm.Value));
                cmd.Parameters.AddWithValue("@Route", string.Format("{0}", CmbRoute.Value));
                cmd.Parameters.AddWithValue("@Size", string.Format("{0}", CmbSize.Value));
                cmd.Parameters.AddWithValue("@Quantity", "");
                cmd.Parameters.AddWithValue("@PaymentTerm", string.Format("{0}", CmbPaymentTerm.Value));
                cmd.Parameters.AddWithValue("@StatusApp", 0);
                cmd.Parameters.AddWithValue("@Interest", tbinterest.Text);
                cmd.Parameters.AddWithValue("@Freight", tbFreight.Text);
                cmd.Parameters.AddWithValue("@Insurance", tbInsurance.Text);
                cmd.Parameters.AddWithValue("@Currency", string.Format("{0}", CmbCurrency.Value));
                cmd.Parameters.AddWithValue("@ExchangeRate", seExchangeRate.Text);
                cmd.Parameters.AddWithValue("@Remark", mNotes.Text);
                cmd.Parameters.AddWithValue("@Customer", string.Format("{0}", CmbCustomer.Value));
                cmd.Parameters.AddWithValue("@ShipTo", string.Format("{0}", CmbShipTo.Value));
                cmd.Parameters.AddWithValue("@BillTo", string.Format("{0}", CmbBillTo.Value));
                cmd.Parameters.AddWithValue("@zone", string.Format("{0}", cmbZone.Value));
                cmd.Parameters.AddWithValue("@from", defrom.Value);
                cmd.Parameters.AddWithValue("@to", deto.Value);
                cmd.Parameters.AddWithValue("@ValidityDate", deValidityDate.Value);
                cmd.Parameters.AddWithValue("@RequestNo", "");
                cmd.Parameters.AddWithValue("@NewID", hfgetvalue["NewID"]);
                //cmd.Connection = con;
                //con.Open();
                //var getValue = cmd.ExecuteScalar();
                //con.Close();
                //value = getValue.ToString();
                cmd.Connection = con;
                con.Open();
                DataTable dtstd = new DataTable();// string Folio = "";
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dtstd);
                con.Close();
                foreach (DataRow dr in dtstd.Rows)
                {
                    post(dr);
                    ApproveStep(dr);
                }
                //gridData.JSProperties["cpKeyValue"] = (getValue == null) ? string.Empty : getValue.ToString();
                con.Dispose();
            }
        }
        if (args[0] == "Delete")
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
    void ApproveStep(DataRow r)
    {
        string keys = string.Format("{0}", r["ID"]);
        string step = "";
        if (CmbDisponsition.Value != null)
        {
            updatesel(string.Format("{0}", keys));

            if (CmbDisponsition.Value.Equals("3")) {
                step = "3";
            } 
            else if (CmbDisponsition.Value.Equals("4") && string.Format("{0}", r["Statusapp"]).Equals("9"))
            {
                step = "4";
            }
            else if (CmbDisponsition.Value.Equals("1"))
            {
                var xdr = listItems.FirstOrDefault(x => string.Format("{0}", x.IsAccept).Contains("level"));
                if (xdr == null)
                    step = "4";
                else if(xdr != null)
                {
                    string _sublevel = cs.ReadItems(@"select convert(nvarchar(max), count(empid))c from MasApprovAssign where empid = '" +
                            CmbAssignee.Value.ToString() + "' and Sublevel = '4'");
                    if (_sublevel.Equals("1"))
                    {
                        step = "9";
                    }
                    else
                    {
                        _sublevel = cs.ReadItems(@"select convert(nvarchar(max), count(empid))c from MasApprovAssign where empid = '" +
                        cs.CurUserName.ToString() + "' and Sublevel = '15'");
                        if (_sublevel.Equals("1"))
                            step = "9";
                        else if (_sublevel.Equals("0"))
                            step = "8";
                    }
                }
            }
            else if (CmbDisponsition.Value.Equals("4") && string.Format("{0}", r["Statusapp"]).Equals("8"))
            {
                var xdr = listItems.FirstOrDefault(x => string.Format("{0}", x.IsAccept).Contains("level2"));
                if (xdr != null)
                    step = "9";
                else if (xdr == null)
                    step = "4";
            }
                                                       //    {
                                                       //    List<string> AuthorList = new List<string>();
                                                       //    var objItems = listItems.Where(x => !string.IsNullOrEmpty(x.Notes)).ToList();
                                                       //    foreach(var _r in objItems)
                                                       //    {
                                                       //            string _level = checkapprove(_r, string.Format("{0}", r["Statusapp"]));
                                                       //    if (AuthorList.Find(x=>x.Equals(_level.ToString()))==null && !string.IsNullOrEmpty(_level)) {
                                                       //            AuthorList.Add(_level);
                                                       //        }
                                                       //    }
                                                       //    //foreach (string s in AuthorList)
                                                       //    //{
                                                       //    //    var table = GetLevel(CmbAssignee.Value.ToString()).AsEnumerable().FirstOrDefault();
                                                       //    //    //.AsEnumerable().Where(a => a["EMPID"].Equals(cs.CurUserName)).FirstOrDefault();
                                                       //    //    insert(s.Substring(1,1).ToString(), string.Format("{0}", keys), table[s].ToString(), s);
                                                       //    //}
                                                       //    //var dt = cs.builditems(@"select Statusapp from TransTunaStd where Id=" + keys);
                                                       //    //DataRow row = dt.Select("Id=" + keys).FirstOrDefault();
                                                       //    //string statusapp = row["Statusapp"].ToString();
                                                       //    var values = new[] { "ExchangeRate", "Freight", "Margin" };
                                                       //    var xdr = listItems.FirstOrDefault(x => string.Format("{0}", x.IsAccept).Contains("level") && string.Format("{0}", x.isapprove)=="");
                                                       //    if (xdr != null)
                                                       //    {
                                                       //        step = "8";
                                                       //        //string strSQL = @"select Sublevel from MasApprovAssign where EmpId in ('" + cs.CurUserName + "')";
                                                       //        //DataTable dtsub = cs.builditems(@strSQL);
                                                       //        ////check level
                                                       //        //if (dtsub.Select("Sublevel=15").Length > 0){
                                                       //        //    step = "9";
                                                       //        //}
                                                       //    }
                                                       //    else if (step.ToString() == "")
                                                       //    {
                                                       //        var obj = listItems.FirstOrDefault(x => string.Format("{0}", x.IsAccept) == "level2");
                                                       //        if (obj != null)
                                                       //            step = "9";
                                                       //    }
                                                       //}
         else
            {
                step = CmbDisponsition.Value.ToString();
            }
        }
        if (string.IsNullOrEmpty(CmbDisponsition.Text)) return;

        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spApproveStep";
            cmd.Parameters.AddWithValue("@Id", keys.ToString());
            cmd.Parameters.AddWithValue("@User", cs.CurUserName.ToString());
            cmd.Parameters.AddWithValue("@StatusApp", step.ToString());
            cmd.Parameters.AddWithValue("@table", "TransTunaStd");
            cmd.Parameters.AddWithValue("@remark", mComment.Text);
            cmd.Parameters.AddWithValue("@Assign", CmbAssignee.Value);
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
                List<string> list = new List<string>();
                List<string> lstValue = new List<string>();
                List<string> lstSend = new List<string>();

                if (!string.IsNullOrEmpty(CmbAssignee.Text.ToString()) && CmbDisponsition.Value.Equals("1"))
                    dr["MailTo"] = CmbAssignee.Value.ToString();
                if (CmbDisponsition.Value.Equals("3"))
                    dr["MailTo"] = dr["MailTo"].ToString();//dr["Requester"].ToString();

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
                string sender = String.Join(",", list.ToArray());
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
                //list.Add(cs.GetData(CurUserName, "email"));
                string MailCc = String.Join(",", list.ToArray());
                string _link = string.Format("Dear {0} <br/> ", String.Join(",", lstSend.ToArray()));
                _link += "<br/> Requester : " + cs.GetData(dr["Requester"].ToString(), "fn");
                if (step.Equals("9") && dr["Requester"].ToString() != cs.CurUserName)
                    _link += "<br/> Reviewer " + cs.GetData(cs.CurUserName, "fn");
                else if (step.Equals("3")) {
                   DataTable _Reviewer = cs.builditems( @"select distinct(m.Username) from  
                MasHistory m left join MasApprovAssign s on s.EMPID = m.Username where tablename = 'TransTunaStd' and RequestNo = "+ keys.ToString() + " and Sublevel = '15'");
                    if (_Reviewer.Rows.Count > 0)
                        _link += "<br/> Reviewer " + cs.GetData(cs.CurUserName, "fn");
                }

                _link += "<br/> Quotation request no. :" + dr["RequestNo"].ToString();
                //_link += "<br/> Date request : " + DateTime.ParseExact(dr["requestdate"].ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);  
                _link += "<br/> Customer name : " + string.Format("{0}", CmbCustomer.Text);
                _link += "<br/> Payment term : " + string.Format("{0}", CmbPaymentTerm.Text);
                _link += "<br/> Incoterm : " + string.Format("{0}", CmbIncoterm.Text);
                if (!string.IsNullOrEmpty(CmbReason.Text))
                    _link += "<br/> Reason :" + CmbReason.Text;
                _link += "<br/> Comment :" + mComment.Text;
                _link += @"<br/> The document link --------><a href=" + cs.GetSettingValue() + "/Default.aspx?viewMode=CalculationForm2&ID="
                    + keys.ToString() + "&UserType=" + hftype["type"];
                _link += " style='color: rgb(0,255,0)'><font color='#0000FF'>click</font></a>";

                string subject = string.Format(@"Human Food | Quotation :{0} ", dr["RequestNo"].ToString());
                if (string.Format("{0}", dr["Statusapp"]).Equals("8"))
                    subject = string.Format(subject + "Send to Review");
                else if (string.Format("{0}", dr["Statusapp"]).Equals("9"))
                    subject = string.Format(subject + "Send to Approve");
                else if (string.Format("{0}", dr["Statusapp"]).Equals("3") || string.Format("{0}", dr["Statusapp"]).Equals("-1"))
                    subject = string.Format(subject + "was rejected");
                else if (string.Format("{0}", dr["Statusapp"]).Equals("4"))
                    subject = string.Format(subject + "was approved");
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
    public override void Update()
    {
        //gridData.DataSource = dsgv;
        gridData.DataBind();
    }
    string checkapprove(StdItems _r,string statusapp)
    {
        
        double _margin;
        if (Double.TryParse(_r.Margin.ToString(), out _margin))
        {
            var _t = cs.GetFillWeight(_r.Material);
            foreach (DataRow _dr in _t.Rows)
            {
                if (_margin < 0)
                {
                    //if (_dr["skjgroup"].ToString().Equals("STD SKJ") && _dr["SHD"].ToString().Equals("SHD"))
                    //{
                    //    if ((-7.4 < _margin && _margin < 0) && statusapp.Equals("0") )
                    //        return "level1";
                    //    if (-7.4 >= _margin)
                    //        return "level2";
                    //}
                    //else if (_dr["skjgroup"].ToString().Equals("STD SKJ") && _dr["SHD"].ToString().Equals("None"))
                    //{
                    //    if ((-5.7 < _margin && _margin <= -1.8)&& statusapp.Equals("0"))
                    //        return "level1";
                    //    if (-5.7 >= _margin )
                    //        return "level2";
                    //}
                    //else if (_dr["skjgroup"].ToString().Equals("Non STD SKJ") && _dr["SHD"].ToString().Equals("None") && -2.5 >= _margin )
                    //{
                    //    return "level2";
                    //}
                    if ((_margin < 0) && statusapp.Equals("0"))
                        return "level1";
                }
            }
        }
        
        return "";
    }
    void Delete(string keys)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "Update TransTechnical set StatusApp=5 where ID=@ID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spDelTunaStd";
            cmd.Parameters.AddWithValue("@ID", keys.ToString());
            cmd.Parameters.AddWithValue("@user", hfuser["user_name"].ToString());
            cmd.Parameters.AddWithValue("@StatusApp", "-1");
            cmd.Parameters.AddWithValue("@tablename", string.Format("{0}", "TransTunaStd"));
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    protected void gvitems_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e)
    {
        foreach (var args in e.InsertValues)
        {
            if (GetIndex == 6)
            {
                object keys = gv.GetRowValues(gv.FocusedRowIndex, gv.KeyFieldName);
                var obj = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(keys));
                //List<TransUpCharge> list3 = obj.Upcharge;
                TransUpCharge f3 = new TransUpCharge();
                int iMax = cs.FindMaxValue(obj.Upcharge, x => x.RowID);
                iMax++;
                f3.RowID = iMax;
                f3.SubID = keys.ToString();
                f3.UpchargeGroup = "UpCharge";
                f3.UpCharge = "";
                f3.Currency = "USD";
                f3.RequestNo = "";
                f3.Result = "0";
                f3.Price = "0";
                f3.Quantity = "0";
                f3.stdPackSize = "0";
                f3.Calcu = GetIndex.ToString();
                obj.Upcharge.Add(f3);

                //TransCal cal = new TransCal();
                //int iMax = cs.FindMaxValue(obj.cal, x => x.RowID);
                //iMax++;
                //cal.RowID = iMax;
                //cal.Component = "UpCharge";
                //cal.Calcu = GetIndex;
                //cal.BaseUnit = "";

                //obj.cal.Add(cal);
                //buildcalcu(obj);
            }
  
        }
        foreach (var args in e.UpdateValues)
        {
            int FocusedRowIndex = gv.FocusedRowIndex;
            object keyValue = gv.GetRowValues(FocusedRowIndex, gv.KeyFieldName);
            int ID = Convert.ToInt32(keyValue);
            var rcus = listItems.FirstOrDefault(x => x.ID == ID);
            if (GetIndex == 9)
            {
                DataRow dr = _dt.Rows.Find(args.Keys["RowID"]);
                foreach (DataColumn column in _dt.Columns)
                {
                    if (column.ColumnName.Equals("Price"))
                    {
                        dr[column.ColumnName] = args.NewValues[column.ColumnName];
                        decimal _FillWeight = Convert.ToDecimal(dr["Quantity"]);
                        decimal _Yield = Convert.ToDecimal(dr["Unit"]);
                        string packsize = GetPackSize(_dt).ToString();
                        dr["Result"] = Convert.ToDecimal(_FillWeight / 1000 / _Yield *
                            Convert.ToDecimal(args.NewValues[column.ColumnName]) / 1000 * Convert.ToDecimal(packsize));
                    }
                }
            }
            if (GetIndex == 0)
            {
                //if (_utilizedt != null)
                //{
                var values = new[] { "Result" };
                bool mark = false;
                decimal intpercent = 100;
                //int FocusedRowIndex = gv.FocusedRowIndex;
                //object keyValue = gv.GetRowValues(FocusedRowIndex, gv.KeyFieldName);
                //DataRow rcus = tcustomer.Rows.Find(keyValue);
                var rw = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(separa));
                //string strutilize = rw["Utilize"].ToString();
                //int[] arr = strutilize.Split('|').Select(Int32.Parse).ToArray();
                DataTable _utilizedt = new DataTable();
                if (rw.Utilize == null)
                    _utilizedt = Getutilize(Convert.ToDateTime(defrom.Value).Date,
                                Convert.ToDateTime(deto.Value).Date);
                //DataRow[] result = _utilizedt.Select(string.Format("SubID='{0}'", separa));
                else
                    _utilizedt = MyToDataTable.ToDataTable(rw.Utilize);
                int c = _utilizedt.Rows.Count;
                foreach (DataRow dr in _utilizedt.Rows)
                {
                    //DataRow dr = _utilizedt.Rows.Find(e.Keys[0]);
                    foreach (DataColumn column in _utilizedt.Columns)
                    {
                        if (column.ColumnName == "Mark")
                            dr["Mark"] = "X";
                        else if (column.ColumnName == "Result")
                        {
                            if (dr["RowID"].ToString() == args.Keys["RowID"].ToString())
                            {
                                mark = true;
                                if (values.Any(column.ColumnName.Contains))
                                {
                                    dr[column.ColumnName] = args.NewValues[column.ColumnName].ToString();
                                    c = c - 1;
                                    intpercent = intpercent - Convert.ToDecimal(dr[column.ColumnName]);
                                }
                            }
                            else
                            {
                                if (mark)
                                {
                                    dr[column.ColumnName] = intpercent / c;
                                }
                                else
                                {
                                    c = c - 1;
                                    string strvalue = dr[column.ColumnName].ToString();
                                    decimal value = 0;
                                    decimal.TryParse(strvalue.ToString(), out value);
                                    intpercent = intpercent - value;
                                }
                            }
                        }
                    }
                }
                rw.Utilize = (from DataRow dr in _utilizedt.Rows
                              select new TransUtilize()
                              {
                                  RowID = Convert.ToInt32(dr["RowID"]),
                                  Result = dr["Result"].ToString(),
                                  MonthName = dr["MonthName"].ToString(),
                                  Cost = dr["Cost"].ToString(),
                                  SubID = dr["SubID"].ToString(),
                                  Calcu = GetIndex.ToString(),
                                  RequestNo = dr["RequestNo"].ToString()
                              }).ToList();
                ///update 
                //List<string> list = new List<string>();
                //foreach (DataRow dr in _utilizedt.Rows)
                //{
                //    list.Add(dr["Result"].ToString());
                //}
                //if (list.Count > 0)
                //{

                //    if (rw != null)
                //        rw["Utilize"] = String.Join("|", list.ToArray());
                //}
                //}
            }
            if (GetIndex == 6)
            {
                ////if (_UpChargedt == null){
                ////    _UpChargedt = new DataTable(_dt.TableName);
                ////    _UpChargedt = _dt.Clone();
                ////}
                //var _cal = rcus.cal;
                //var udr = _cal.Find(x => x.RowID == Convert.ToInt32(args.Keys["RowID"]));

                var _cal = rcus.Upcharge;
                var udr = _cal.Find(x => x.RowID == Convert.ToInt32(args.Keys["RowID"]));

                //foreach (DataColumn column in _dt.Columns)
                //{
                //    if (!column.ColumnName.Equals("RowID"))
                //    {
                //        if (column.ColumnName == "Unit")
                //        {
                //            dr[column.ColumnName] = GetPackSize(_dt).ToString();
                //        }
                //        else
                //            dr[column.ColumnName] = args.NewValues[column.ColumnName];
                //        //decimal _FillWeight = Convert.ToDecimal(dr["Quantity"]);
                //        //decimal _Yield = Convert.ToDecimal(dr["Unit"]);
                //        //string packsize = GetPackSize(_dt).ToString();
                //        //dr["Result"] = Convert.ToDecimal(_FillWeight / 1000 / _Yield *
                //        //    Convert.ToDecimal(args.NewValues[column.ColumnName]) / 1000 * Convert.ToDecimal(packsize));
                //    }
                //}
                udr.UpchargeGroup = string.Format("{0}", args.NewValues["UpchargeGroup"]);
                udr.UpCharge = string.Format("{0}", args.NewValues["UpCharge"]);
                udr.Price = string.Format("{0}", args.NewValues["Price"]);
                udr.Quantity = string.Format("{0}", args.NewValues["Quantity"]);
                udr.Currency = string.Format("{0}", args.NewValues["Currency"]);//udr.Result = string.Format("{0}", args.NewValues["Result"]);
                udr.stdPackSize = string.Format("{0}", args.NewValues["stdPackSize"]);
                if (udr.Price != "0")
                {
                    udr.Result = ((Convert.ToDouble(udr.Price) / Convert.ToDouble(udr.Quantity)) * Convert.ToDouble(udr.stdPackSize)).ToString();
                }
                else
                    udr.Result = "0";
                rcus.Upcharge = _cal;
                //udr.Component = string.Format("{0}", args.NewValues["Component"]); 
                //udr.Name = string.Format("{0}", args.NewValues["Name"]);
                //udr.Price = string.Format("{0}", args.NewValues["Price"]);
                //udr.Quantity = string.Format("{0}", args.NewValues["Quantity"]);
                //udr.Currency = string.Format("{0}", args.NewValues["Currency"]);
                //udr.Result =  Convert.ToDouble(args.NewValues["Result"]);
                //udr.Unit = string.Format("{0}", args.NewValues["Unit"]);
                //udr.BaseUnit = "";
                //rcus.cal = _cal;

                //if (dr != null) {
                //    dr["Result"] = ((Convert.ToDecimal(dr["Price"]) / Convert.ToDecimal(dr["Quantity"])) * Convert.ToDecimal(dr["Unit"]));
                //}
                //DataRow[] rowupcharge = _dt.Select("Calcu in (6)");
                //int co = rowupcharge.Length;
                //if (co > 0) {
                //    rcus.Upcharge = (from DataRow xdr in rowupcharge
                //                     select new TransUpCharge()
                //                     {
                //                         ID = Convert.ToInt32(xdr["RowID"]),
                //                         UpchargeGroup = xdr["Component"].ToString(),
                //                         UpCharge = xdr["Name"].ToString(),
                //                         Price = xdr["Price"].ToString(),
                //                         Quantity = xdr["Quantity"].ToString(),
                //                         Currency = xdr["Currency"].ToString(),
                //                         Result = xdr["Result"].ToString(),
                //                         stdPackSize = xdr["Unit"].ToString()
                //                         //SubID = xdr["SubID"].ToString(),
                //                         //RequestNo = xdr["RequestNo"].ToString()
                //                     }).ToList();
                //}
                ////+++++++++++++++++++
                //double UpChargePrice, totalUpChargePrice = 0, UpChargeUnit = 0;
                //if (Double.TryParse(dr["Price"].ToString(), out UpChargePrice))
                //{
                //    //if (tbUpChargeUnit.Text.ToLower() == "can")
                //    //{
                //    //    DataRow _result = _dt.Select("Component = 'Packing Style'").FirstOrDefault();
                //    Double.TryParse(dr["Unit"].ToString(), out UpChargeUnit);
                //    //DataRow _r = _dt.Select("Component in ('Packaging') and Calcu=5 and name like '%Secondary Packaging per case%'").FirstOrDefault();
                //    string packsize = GetPackSize(_dt).ToString();
                //    totalUpChargePrice = ((UpChargePrice / UpChargeUnit) * Convert.ToDouble(packsize.ToString())) * 1;
                //    //}
                //    DataRow[] _rowdt = _dt.Select("Name = 'Upcharge' and Calcu in (8,9)");
                //    foreach (DataRow _result in _rowdt)
                //    {
                //        _result["Result"] = Convert.ToDecimal(_result["Result"]) + Convert.ToDecimal(totalUpChargePrice);
                //        //                        _result["Quantity"] = 1;
                //        //                        _result["Price"] = Convert.ToDecimal(_result["Price"]) + Convert.ToDecimal(dr["Price"].ToString());
                //    }
                //}
                ////++++++++++++++

            }
            if (GetIndex.Equals(10))
            {
                var flist = rcus.FileDetails;
                var fdr = flist.Find(x => x.ID == Convert.ToInt32(args.Keys["ID"]));
                fdr.Result = string.Format("{0}", args.NewValues["Result"]);
                fdr.Notes = string.Format("{0}", args.NewValues["Notes"]);
                //fdr.Name = string.Format("{0}", args.NewValues["Name"]);
                //fdr.Attached = !string.IsNullOrEmpty(string.Format("{0}", args.NewValues["Attached"])) ? (byte[])args.NewValues["Attached"] : null;
                fdr.RequestNo = "";
                fdr.SubID = string.Format("{0}", keyValue);// args.Keys["ID"].ToString();
                rcus.FileDetails = flist;
            }
            //_dt = GetCalcu(rcus.ID);
            buildcalcu(rcus);
            if(Convert.ToDouble(rcus.Equivalent_Fish_price) - Convert.ToDouble(rcus.Announcement_Fish_price) > 75 )
            {
                var _f = rcus.FileDetails.Find(x => x.SubID == Convert.ToInt32(args.Keys["ID"]).ToString() && x.Result == "Margin");
                if (_f == null)
                {
                    if (rcus.Material.Substring(4, 1).Equals("S"))
                        rcus.diff_price = (Convert.ToDouble(rcus.Equivalent_Fish_price) - Convert.ToDouble(rcus.Announcement_Fish_price) > 300) ?"X": "" ;
                    else
                        rcus.diff_price = _f == null ? "X" : "";
                }
            }
                //? "1" : "0";
        }

        foreach (var args in e.DeleteValues) {

            var rw = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(args.Keys["ID"]));
            if (GetIndex == -1)
            {
                //DataRow rw = tcustomer.Rows.Find(args.Keys["ID"]);
                rw.Mark = rw.Mark.ToString() == "Del" ? "" : "Del";
                separa = "";
                listItems.Remove(rw);
            }
            if (GetIndex == 10)
            {
                var flist3 = rw.FileDetails;
                var fdr3 = flist3.Find(x => x.ID == Convert.ToInt32(args.Keys["ID"]));
                if(fdr3 != null)
                    rw.Notes = "";
            }
            //deleteItems(args.Keys["ID"]);
            //tcustomer.AcceptChanges();
        }
        if (GetIndex == 0 || GetIndex == 6)
            gvitems.JSProperties["cpKeyValue"] = "Submitted";
        e.Handled = true;
    }
    string GetPackSize(DataTable table)
    {
        DataRow _r = table.Select("Component in ('Packaging') and Calcu=5 and name like '%Secondary Packaging per case%'").FirstOrDefault();
        return _r == null ? "0" : _r["Currency"].ToString();
    }
    protected void gvitems_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.FilterExpression = "[Calcu] = '" + ActivePageSymbol + "'";

    }
    string recalculate()
    {
        decimal result = 0;
        //Creating another DataTable to copy
        //DataTable dt_copy = new DataTable();
        //_dt.TableName = "CopyTable";
        //dt_copy = _dt.Copy();
        //object sumObject = _dt.Compute(@"Sum(Result)", "(Component in ('RM'))and Calcu=8");
        //int Index = gv.EditingRowVisibleIndex;
        //object keyValue = gv.GetRowValues(Index, "Material");
        //DataTable _dtfillweight = GetFillWeight(keyValue.ToString());
        //DataRow[] _rarray = _dt.Select("Component in ('RM') and Calcu=1 and name like '%None%'");
        DataRow[] _rarray = _dt.Select("Component in ('Raw material') and Calcu=8");
        foreach (DataRow _rw in _rarray)
        {
            object TotalObject = _dt.Compute(@"Sum(Result)", "(Name in ('Secondary Packaging','Media'," +
                                        "'Primary Packaging','Packing Style','UpCharge') or (Name like 'LOH per pack%')) and Calcu=8");
            object sumObject = _dt.Compute(@"Sum(Result)", "(name in ('FOB price(18 - digits)'))and Calcu=8");
            decimal _ResultPrice = Convert.ToDecimal(sumObject) - Convert.ToDecimal(TotalObject);
            decimal _FillWeight = Convert.ToDecimal(_rw["Quantity"].ToString());
            decimal _Yield = Convert.ToDecimal(_rw["Unit"].ToString());
            //decimal _ResultPrice = Convert.ToDecimal(_rw["Result"]);
            DataRow _r = _dt.Select("Component in ('Packaging') and Calcu=5 and name like '%Secondary Packaging per case%'").FirstOrDefault();
            if (_r != null)
            {
                //J13*1000/I14*1000*I15/I16
                result = Convert.ToDecimal(_ResultPrice * 1000 / _FillWeight * 1000 * _Yield / Convert.ToDecimal(_r["Currency"].ToString()));
            }

        }


        //object totalsumObject = dt_copy.Compute("Sum(Result)", "Name in ('Value Commission','FOB price(18 - digits)','OverPrice') and Calcu=8");
        //Decimal _FOBPrice = Convert.ToDecimal(sumObject) - _margin;

        //if (sumObject != System.DBNull.Value){   
        //}
        return result.ToString("F4");
    }
    //void GetUpdateItems()
    //{
    //    if (_dt == null) return;
    //    var values = new[] { "Value Commission", "OverPrice", "Pacifical", "MSC", "Margin", "Interest" };
    //    for (int i = 0; i <= values.Length - 1; i++)
    //    {
    //        //object sumObject;
    //        DataRow _dr = _dt.Select(string.Format("Name='{0}'", values[i])).FirstOrDefault();
    //        //if (sumObject != System.DBNull.Value)

    //        if (_dr != null)
    //        {

    //        }
    //        //Value Commission
    //        //insertsGrid(args[1]);
    //    }
    //}
    //protected void gvitems_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
    //    if (_dt != null)
    //    {
    //        DataRow dr = _dt.Rows.Find(e.Keys["RowID"]);
    //        foreach (DataColumn column in _dt.Columns)
    //        {
    //            if (column.ColumnName.ToString() != "RowID")
    //            {
    //                dr[column.ColumnName] = e.NewValues[column.ColumnName];
    //            }
    //        }
    //        //dr["Data"] = e.NewValues["Data"];
    //    }
    //    g.CancelEdit();
    //    e.Cancel = true;
    //}
    protected void ASPxGridView1_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
    {
        if (e.RowType == GridViewRowType.Data)
            e.Row.Attributes["data"] = e.GetValue("UpchargeGroup").ToString();
        //e.Row.Attributes["Upcharge"] = e.GetValue("Upcharge").ToString();
        //e.Row.Attributes["Value"] = e.GetValue("Value").ToString();
        //e.Row.Attributes["Currency"] = e.GetValue("Currency").ToString();
        //e.Row.Attributes["Unit"] = e.GetValue("Unit").ToString();
    }
    public class CustomDataColumnTemplate : ITemplate
    {
        string index;
        public CustomDataColumnTemplate(string myindex)
        {
            this.index = myindex;
        }
        public void InstantiateIn(Control container)
        {
            GridViewHeaderTemplateContainer c = (GridViewHeaderTemplateContainer)container;
            ASPxButton btn = new ASPxButton();
            btn.RenderMode = ButtonRenderMode.Link;
            btn.AutoPostBack = false;
            btn.ID = "button" + c.ItemIndex;

            c.Controls.Add(btn);
            //btn.Text = "Button";
            btn.Image.Url = "~/Content/Images/icons8-plus-math-filled-16.png";
            //btn.Click += ASPxButton1_Click;
            //string data = index == "10" ? "Attach" : "Insert";
            btn.ClientSideEvents.Click = @"function(s,e){ insertSelection(" + index + ",'Insert',s); }";
        }
        protected void ASPxButton1_Click(object sender, EventArgs e)
        {

        }
    }
    public class CustomDownload : ITemplate
    {        public void InstantiateIn(Control container)
        {
            GridViewHeaderTemplateContainer c = (GridViewHeaderTemplateContainer)container;
            ASPxButton btn = new ASPxButton();
            btn.RenderMode = ButtonRenderMode.Link;
            btn.AutoPostBack = false;
            btn.ID = "button" + c.ItemIndex;

            c.Controls.Add(btn);
            btn.Image.Url = "~/Content/Images/icons8-plus-math-filled-16.png";
            btn.Click += ASPxButton1_Init; 
        }
        protected void ASPxButton1_Init(object sender, EventArgs e)
        {
            ASPxButton button = (ASPxButton)sender;
            GridViewDataItemTemplateContainer container = (GridViewDataItemTemplateContainer)button.NamingContainer;
            button.ClientSideEvents.Click = string.Format("function(s, e) {{ window.open = 'popupControls/DownloadFile.aspx?Id={0}'; }}", container.KeyValue);
        }
    }
    protected void gv_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
    {
        if (e.DataColumn.FieldName == "Mark")
        {
            var g = sender as ASPxGridView;
            DataRow row = g.GetDataRow(e.VisibleIndex);
            int index = e.VisibleIndex;
            //e.Cell.ForeColor = Color.Black;
            if (g.VisibleRowCount != 0 && row != null)
            {
                if (g.GetRowValues(index, "Mark").ToString() == "Del")
                    e.Cell.BackColor = Color.Coral;
                else if (g.GetRowValues(e.VisibleIndex, "Mark").ToString() == "X")
                    e.Cell.BackColor = Color.LightGreen;
                else
                    e.Cell.BackColor = Color.White;
            }
        }
    }
    protected void gv_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e)
    {
        if (e.RowType != GridViewRowType.Data) return;
        string value = (string)e.GetValue("Mark");
        if (value.ToString() == "TG")
        {
            int Keys = 0;
            int.TryParse(e.KeyValue.ToString(), out Keys);
            if (listItems != null)
            {
                var obj = listItems.FirstOrDefault(x => x.ID == Keys);
                List<TransStdFileDetails> list3 = obj.FileDetails;
                TransStdFileDetails result3 = list3.Find(x => x.SubID == Keys.ToString() && x.Result == "TG EU");
                if (result3 != null)
                    e.Row.ForeColor = System.Drawing.Color.Green;
                else
                    e.Row.ForeColor = System.Drawing.Color.Red;
            }
        }
        //    e.Row.ForeColor = System.Drawing.Color.Red;
        //else if (value.ToString() == "TGA")
        //    e.Row.ForeColor = System.Drawing.Color.Green;
        else
            e.Row.ForeColor = System.Drawing.Color.Black; 
    }
    //protected void gv_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e)
    //{
    //    if (listItems != null)
    //    {
    //        var dr = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(e.KeyValue));
    //        if (dr != null)
    //        {
    //            if (dr.Mark.ToString() == "TG")
    //                e.Row.BackColor = Color.Red;
    //            else
    //                e.Row.BorderColor = Color.Green;
    //        }
    //    }
    //}
    void deleteItems(object keyValue)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spDelTunaStdItems";
            cmd.Parameters.AddWithValue("@Id", keyValue.ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    void deleteUpCharge(object keyValue)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.CommandText = "DELETE TransUpCharge WHERE RequestNo = @Id";
            cmd.Parameters.AddWithValue("@Id", keyValue.ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    void deleteFileDetails(object keyValue)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.CommandText = "DELETE TransStdFileDetails WHERE RequestNo = @Id";
            cmd.Parameters.AddWithValue("@Id", keyValue.ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    //protected void gv_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
    //{
    //    ASPxGridView g = (ASPxGridView)sender;
    //    DataRow rw = tcustomer.Rows.Find(e.Keys[0]);
    //    rw["Mark"] = rw["Mark"].ToString() == "Del" ? "" : "Del";
    //    separa = "";
    //    tcustomer.Rows.Remove(rw);
    //    deleteItems(e.Keys[0]);
    //    tcustomer.AcceptChanges();
    //    e.Cancel = true;
    //}
    protected void ASPxHyperLink_Load(object sender, EventArgs e)
    {
        ASPxHyperLink hpl = (ASPxHyperLink)sender;
        //GridViewDataItemTemplateContainer c = (GridViewDataItemTemplateContainer)hpl.NamingContainer;
        //if (!String.IsNullOrWhiteSpace(FileList[c.VisibleIndex].FileName) && !String.IsNullOrWhiteSpace(FileList[c.VisibleIndex].Url))
        //{
        //    hpl.Text = FileList[c.VisibleIndex].FileName;
        //    hpl.NavigateUrl = FileList[c.VisibleIndex].Url;
        //}
    }
    //public List<MySavedObjects> FileList
    //{
    //    get
    //    {
    //        List<MySavedObjects> list = Session["list"] as List<MySavedObjects>;
    //        if (list == null)
    //        {
    //            list = new List<MySavedObjects>();
    //            for (int i = 0; i < ASPxGridView1.VisibleRowCount; i++)
    //            {
    //                list.Add(new MySavedObjects() { RowNumber = i });
    //            }
    //            Session["list"] = list;
    //        }
    //        return list;
    //    }
    //}
    protected void gridData_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {
        if (e.ButtonID != "Clone") return;
        ASPxGridView g = (ASPxGridView)sender;
        object keyValue = g.GetRowValues(e.VisibleIndex, g.KeyFieldName);
        //using (SqlConnection con = new SqlConnection(strConn))
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.CommandText = "spcopyTunaStd";
        //    cmd.Parameters.AddWithValue("@Id", keyValue);
        //    cmd.Parameters.AddWithValue("@Requester", hfuser["user_name"].ToString());
        //    cmd.Connection = con;
        //    con.Open();
        //    var getValue = cmd.ExecuteScalar();
        //    con.Close();
        //    g.JSProperties["cpKeyValue"] = (getValue == null) ? string.Empty : getValue.ToString();
        //}
        g.JSProperties["cpKeyValue"] = "Clone|" +string.Format("{0}", (keyValue == null) ? string.Empty : keyValue.ToString());
    }
    //protected void gv_InitNewRow(object sender, ASPxDataInitNewRowEventArgs e)
    //{
    //    ASPxGridView g = (ASPxGridView)sender;
    //        int NextRowID = Convert.ToInt32(tcustomer.AsEnumerable()
    //        .Max(row => row["Id"]));
    //    NextRowID++;
    //    separa = NextRowID.ToString();
    //}
    //void init()
    //{
    //    int NextRowID = Convert.ToInt32(tcustomer.AsEnumerable()
    //             .Max(row => row["ID"]));
    //    DataRow rw = tcustomer.NewRow();
    //    NextRowID++;
    //    rw["ID"] = NextRowID;
    //    rw["From"] = defrom.Value;
    //    rw["To"] = deto.Value;
    //    separa = rw["ID"].ToString();
    //    //var values = new[] { "ID", "RowID", "RequestNo" };
    //    //foreach (DataColumn column in tcustomer.Columns)
    //    //{
    //    //    if (!values.Any(column.ColumnName.Contains))
    //    //    {
    //    //        if (column.ColumnName.ToString() == "From")
    //    //            rw[column.ColumnName] = defrom.Value;
    //    //        else if (column.ColumnName.ToString() == "To")
    //    //            rw[column.ColumnName] = deto.Value;
    //    //    }
    //    //}
    //    tcustomer.Rows.Add(rw);
    //}
    //protected void cp_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    switch (e.Parameter)
    //    {
    //        case "Update":
    //            gv.UpdateEdit();
    //            break;
    //        case "Cancel":
    //            gv.CancelEdit();
    //            break;
    //    }
    //}

    protected void gv_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e)
    {
        if (e.Column.FieldName == "MinPrice" || e.Column.FieldName == "OfferPrice" )
            e.DisplayText = string.Format("{0:N2}", e.Value);

    }
    protected string GetCheckBoxChecked(object value)
    {
        return (bool)value ? "checked" : "";
    }

    //protected void btnUpload_Click(object sender, EventArgs e)
    //{
    //    UploadFile();
    //}
    private string GetFileExtension(string fileExtension)
    {
        switch (fileExtension.ToLower())
        {
            case ".docx":
            case ".doc":
                return "Microsoft Word Document";
            case ".xlsx":
            case ".xls":
                return "Microsoft Excel Document";
            case ".txt":
                return "Text Document";
            case ".jpg":
            case ".png":
                return "Image";
            default:
                return "Unknown";
        }
    }

    //Save File in folder and File details and Path to database  
    //private void UploadFile()
    //{
    //    string FileName = string.Empty;
    //    string FileSize = string.Empty;
    //    string extension = string.Empty;
    //    string FilePath = string.Empty;

    //    if (FileUpload1.HasFile)
    //    {
    //        extension = Path.GetExtension(FileUpload1.FileName);
    //        FileName = FileUpload1.PostedFile.FileName;
    //        FileSize = FileName.Length.ToString() + " Bytes";
    //        //strFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + FileUpload1.FileName;  
    //        FileUpload1.PostedFile.SaveAs(Server.MapPath(@"~/Application/FileUploads/" + FileName.Trim()));
    //        FilePath = @"~/Application/FileUploads/" + FileName.Trim().ToString();
    //    }
    //    else
    //    {
    //        lblMsg.Text = "Plase upload the file";
    //        lblMsg.ForeColor = Color.Red;
    //        return;
    //    }
    //    using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        SqlCommand cmd = new SqlCommand("USP_Insert_FileDetails", con);
    //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
    //        cmd.Parameters.AddWithValue("@FileName", FileName);
    //        cmd.Parameters.AddWithValue("@FileSize", FileSize);
    //        cmd.Parameters.AddWithValue("@FileType", GetFileExtension(extension));
    //        cmd.Parameters.AddWithValue("@FilePath", FilePath);
    //        con.Open();
    //        int result = cmd.ExecuteNonQuery();
    //        if (result > 0)
    //        {
    //            lblMsg.Text = "File Uploaded Successfully ";
    //            lblMsg.ForeColor = Color.Green;
    //            BindGridView();
    //        }
    //        con.Close();
    //    }
    //}
    //private void BindGridView()
    //{
    //    using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        SqlDataAdapter adp = new SqlDataAdapter("USP_Select_FileDetails", con);
    //        adp.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
    //        if (con.State != ConnectionState.Open)
    //        {
    //            con.Open();
    //        }
    //        DataSet ds = new DataSet();
    //        adp.Fill(ds);
    //        gvFiles.DataSource = ds;
    //        gvFiles.DataBind();
    //        con.Close();
    //    }
    //}
    protected void gvFiles_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            //case "EditOffer":
            //    Response.Clear();
            //    Response.ContentType = "application/octet-stream";
            //    Response.AppendHeader("Content-Disposition", "filename=" + e.CommandArgument);
            //    Response.TransmitFile(Server.MapPath("~/Application/FileUploads/") + e.CommandArgument);
            //    Response.End();
            //    Break;
            case "Delete":
                var id =  e.CommandArgument;
                break;
        }
    }
    const string UploadDirectory = "~/Content/UploadControl/";

    protected void UploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
    {
        string text = pComment.HeaderText;
        string fileName = e.UploadedFile.FileName;        
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
        byte[] FileData = myservice.ReadFile(resultFilePath);
        var param = hfcomment["comment"].ToString().Split('|');
        var values = new[] { "MSC", "Pacifical", "Margin","TG EU","Other"};
        if (values.Any(param[1].ToString().Equals))
        {
        var obj = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(param[2]));
            List<TransStdFileDetails> list3 = obj.FileDetails;
            TransStdFileDetails result3 = list3.Find(x => x.SubID == param[2].ToString() && x.Result == param[1].ToString());
            //DataRow dr = USP_Select_FileDetails.Rows.Find(args[1]);
            if (result3 == null)
            {
                    TransStdFileDetails f3 = new TransStdFileDetails();
                    //int maxAge = list3.Max(t => t.ID);
                int maxAge = cs.FindMaxValue(list3, x => x.ID);
                maxAge++;
                f3.ID = maxAge;
                    f3.Result = param[1].ToString();
                    f3.Name = name;
                    f3.Notes = param[0].ToString();
                    f3.Attached = FileData;
                    f3.SubID = param[2].ToString();
                    f3.RequestNo = "";
                    list3.Add(f3);
                obj.FileDetails = list3;
            }
            else
            {
                list3.Where(c => c.SubID == param[2].ToString() && c.Result == param[1].ToString())
                    .Select(c => { c.Notes = param[0].ToString(); 
                        c.Name = name;
                        c.Attached = FileData; return c; }).ToList();
                obj.FileDetails = list3;
            }
            if (param[1].ToString().Contains("TG EU"))
                obj.Mark = "TGA";
            if (param[1].ToString() == "Margin")
            {
                obj.Notes = param[0].ToString();
                obj.Attached = name.ToString();
            }
        }
        else
            addFileDetails(name, param[0], param[1], param[2], FileData);
    
            //BuildSelect_FileDetails();
            //gvFiles.DataSource = USP_Select_FileDetails;
            //gvFiles.DataBind();
            e.CallbackData = name + "|" + url + "|" + sizeText;
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
            //        cmd.Parameters.AddWithValue("@table", "TransCostingHeader");
            //        con.Open();
            //        cmd.ExecuteNonQuery();
            //        con.Close();
            //    }
            //}
        }
    protected void CmbDisponsition_Callback(object sender, CallbackEventArgsBase e)
    {
        if (string.IsNullOrEmpty(e.Parameter)) return;


            SqlParameter[] param = {new SqlParameter("@Id",(string)e.Parameter),
                        new SqlParameter("@table","TransTunaStd"),
                        new SqlParameter("@username",hfuser["user_name"].ToString())};
            CmbDisponsition.DataSource = cs.GetRelatedResources("spGetStatusApproval", param);
            CmbDisponsition.DataBind();

    }
    void addFileDetails(string name,string notes,
        string Component,
        string SubID, Byte[] FileData)
    {
        string Id = String.Format("{0}", hGeID["GeID"]);
        int max = Convert.ToInt32(USP_Select_FileDetails.AsEnumerable()
.Max(row => row["ID"]));
        max = max + 1;
        var values = new[] { "MSC", "Pacifical", "Margin" };
        SubID = values.Any(Component.Equals) ? SubID : "0";
        //var RefAtt = String.Concat(hfRefAtt["RefAtt"].ToString().Replace(',', ';'),SubID).Split(';');
        //foreach (string value in RefAtt)
        //{
        if (Component == "Margin")
        {
            var rcus = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(SubID));
            if (rcus != null)
            {
                rcus.Notes = notes;
                rcus.Attached = name.ToString();
            }
        }
        if (USP_Select_FileDetails.Rows.Count == 0)
            BuildAttached("0");
        DataRow[] xdr = USP_Select_FileDetails.Select(string.Format("SubID={0} and Result='{1}'", SubID, Component));
            if (xdr.Length > 0)
            {
            foreach (DataRow row in xdr)
            {
                row["Notes"] = notes;
                row["Name"] = name.ToString();
                row["Attached"] = FileData.ToString();
            }
            }
            else
            {
                USP_Select_FileDetails.Rows.Add(new object[] { max, DateTime.Now, cs.CurUserName, Component, name, notes, FileData, SubID, Id });
            }
        //}
    }
   List<TransUpCharge> buildUpCharge(StdItems _row, int NextRowID,string strPackSizeMat, List<TransUpCharge> UpCharge)
    {
        List<TransUpCharge> _UpCharge = new List<TransUpCharge>();
        if (_row.Material.ToString()=="")
            return _UpCharge;
        int RowID  = cs.FindMaxValue(UpCharge, t => t.RowID); 
        foreach (DataRow _rwup in cs.GetMedia(_row.Material.ToString(), "upcharge", Convert.ToDateTime(defrom.Value), Convert.ToDateTime(deto.Value)).Rows)
        {
            TransUpCharge _roth = new TransUpCharge();
            RowID++;
            _roth.RowID = RowID;
            _roth.UpchargeGroup = "Upcharge";
            _roth.UpCharge = string.Format("{0}", _rwup["UpCharge"]);
            _roth.Quantity = string.Format("{0}", 1);
            _roth.Price = string.Format("{0}", _rwup["Value"]);
            _roth.Currency = string.Format("{0}/{1}", _rwup["Currency"], _rwup["Unit"]);
            _roth.stdPackSize = string.Format("{0}", _rwup["StdPackSize"]);
            _roth.Result = string.Format("{0}", ((Convert.ToDouble(_rwup["Value"]) / Convert.ToDouble(_rwup["StdPackSize"])) * Convert.ToDouble(strPackSizeMat)));
            _roth.SubID = NextRowID.ToString();
            _UpCharge.Add(_roth);
        }
        return _UpCharge;
    }
    protected void gv_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e)
    {
        foreach (var args in e.InsertValues)
        {
            //int NextRowID = Convert.ToInt32(tcustomer.AsEnumerable()
            //            .Max(row => row["ID"]));
            int NextRowID  = cs.FindMaxValue(listItems, t => t.ID);
            //DataRow rw = tcustomer.NewRow();
            StdItems rw = new StdItems();
            NextRowID++;
            rw.ID = NextRowID;
            separa = rw.ID.ToString();
            var values = new[] { "ID", "RowID", "RequestNo" };
            DataTable _utilizedt = Getutilize(Convert.ToDateTime(defrom.Value).Date,
                Convert.ToDateTime(deto.Value).Date);
            rw.Utilize = (from DataRow dr in _utilizedt.Rows
                         select new TransUtilize()
                         {
                             RowID = Convert.ToInt32(dr["RowID"]),
                             Result = dr["Result"].ToString(),
                             MonthName = dr["MonthName"].ToString(),
                             Cost = dr["Cost"].ToString(),
                             SubID = dr["SubID"].ToString(),
                             Calcu = "0",
                             RequestNo = dr["RequestNo"].ToString()
                         }).ToList();

            rw.FileDetails = new List<TransStdFileDetails>();
            LoadNewValues(rw, args.NewValues);
            rw.PackSize = builPackSize(rw.Material.ToString());
            rw.Upcharge = buildUpCharge(rw, NextRowID,rw.PackSize,rw.Upcharge);
            if (rw.Mark.Equals("TG"))
                gv.JSProperties["cpKeyValue"] = string.Format("{0} EU|{1}", rw.Mark,rw.ID);
            //foreach (DataColumn column in tcustomer.Columns)
            //{
            //    if (!values.Any(column.ColumnName.Contains))
            //    {
            //        if (column.ColumnName.ToString() == "From")
            //            rw[column.ColumnName] = defrom.Value;
            //        else if (column.ColumnName.ToString() == "To")
            //            rw[column.ColumnName] = deto.Value;
            //        else if(column.ColumnName == "Utilize")
            //        {
            //            DataTable dataTable2 = Getutilize(Convert.ToDateTime(defrom.Value).Date,
            //                    Convert.ToDateTime(deto.Value).Date);
            //            List<string> list = new List<string>();
            //            foreach (DataRow rwuti in dataTable2.Rows)
            //            {
            //                list.Add(rwuti["Result"].ToString());
            //            }
            //            if (list.Count > 0)
            //                rw["Utilize"] = String.Join("|", list.ToArray());
            //        }
            //        else
            //            rw[column.ColumnName] = args.NewValues[column.ColumnName];
            //    }
            //}
            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            _dt = GetCalcu(rw);
            rw.cal = cs.ConvertDataTable<TransCal>(_dt);
            listItems.Add(rw);
            buildcalcu(rw);
        }
        foreach (var args in e.UpdateValues)
        {
            var values = new[] { "Upcharge" };
            //string[] valueType = Regex.Split("Material;From;To;Commission;OverPrice;OverType;Pacifical;MSC;Margin;SubContainers", ";");
            var dr = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(args.Keys["ID"]));
            if (dr.Material != string.Format("{0}", args.NewValues["Material"]))
            {
                dr.Material = string.Format("{0}", args.NewValues["Material"]);
                dr.cal = new List<TransCal>();
                _dt = GetCalcu(dr);
                //if (_dt != null)
                dr.cal = cs.ConvertDataTable<TransCal>(_dt);
            }
            LoadNewValues(dr, args.NewValues);
            separa = dr.ID.ToString();
            //foreach (DataColumn column in tcustomer.Columns)
            //{
            //    if (values.Any(column.ColumnName.Contains))

            //for (int i = 0; i <= valueType.Length - 1; i++)
            //{
            //    if (valueType[i] == "Material")
            //    {
            //        string de = args.NewValues[valueType[i]].ToString();
            //        dr.Name = GetDescrip(de);
            //        dr[valueType[i]] = args.NewValues[valueType[i]];
            //    }
            //    else
            //    if (valueType[i].ToString() == "From")
            //        dr[valueType[i]] = defrom.Value;
            //    else if (valueType[i].ToString() == "To")
            //        dr[valueType[i]] = deto.Value;
            //    else
            //        dr[valueType[i]] = args.NewValues[valueType[i]];
            //}
            //utilizedt
            //if (_utilizedt == null)
            //{
            //    DataTable dataTable2 = Getutilize(Convert.ToDateTime(defrom.Value).Date,
            //            Convert.ToDateTime(deto.Value).Date);
            //    _utilizedt = dataTable2.Clone();
            //}
            //List<string> list = new List<string>();
            //foreach (DataRow rwuti in _utilizedt.Rows)
            //{
            //    list.Add(rwuti["Result"].ToString());
            //}
            //if (list.Count > 0)
            //    dr["Utilize"] = String.Join("|", list.ToArray());
            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //_dt = GetCalcu(dr);
            //dr.cal = cs.ConvertDataTable<TransCal>(_dt);
            buildcalcu(dr);
            double _Bidprice =0;
            double _omargin = 0;
            double.TryParse(string.Format("{0}", args.NewValues["Bidprice"]), out _Bidprice);
            double _Announcement_Fish_price = 0;
            double.TryParse(string.Format("{0}", args.NewValues["Announcement_Fish_price"]), out _Announcement_Fish_price);
            if (_Bidprice > 0)
            {
                if (!Convert.IsDBNull(dr.marginBid))
                {
                    decimal _margin = Convert.ToDecimal(Convert.ToDecimal(dr.marginBid).ToString());
                    dr.Equivalent_Margin = Convert.ToDecimal(_margin * 100).ToString("F4");
                }
                else dr.Equivalent_Margin = "0";
                //dr.Apply = "";
                //if (Convert.ToDouble(dr.OfferPrice).Equals(_Bidprice))
                //    dr.Apply = "";
                //else  if (!dr.Equivalent_Margin.Equals(dr.Margin))
                //    dr.Apply = "Apply";
            }
            else if (_Announcement_Fish_price > 0 && _Bidprice == 0)
            {
                DataRow _arow = _dt.Select("Name='Equivalent margin' and Calcu=9").FirstOrDefault();
                string _text = "Name in ('FOB price(18 - digits)')  and Calcu={0}";
                object sumObject = _dt.Compute(@"Sum(Result)", string.Format(_text, 8));
                object sumMargin = _dt.Compute(@"Sum(Result)", string.Format("Name in ('Margin')", 8));
                object conObject = _dt.Compute(@"Sum(Result)", string.Format(_text, 9));
                if (!Convert.IsDBNull(sumMargin))
                    dr.Equivalent_Margin = Convert.ToDecimal(Convert.ToDecimal((Convert.ToDecimal(conObject) - (Convert.ToDecimal(sumObject) -
                    Convert.ToDecimal(sumMargin))) / Convert.ToDecimal(conObject)) * 100).ToString("F4");
                else dr.Equivalent_Margin = "0";
                //dr.Apply = "";
                //if (!dr.Equivalent_Margin.Equals(dr.Margin))
                //    dr.Apply = "Apply";
            }
            double.TryParse(string.Format("{0}", args.NewValues["Margin"]), out _omargin);
            if (_omargin < 0)
                dr.IsAccept = checkapprove(dr, "0");
            if (dr.Apply.Equals(""))
            {
                TransStdFileDetails result3 = dr.FileDetails.Find(x => x.Result == "Margin");

                if (FindDifference(Convert.ToDecimal(dr.Equivalent_Fish_price), Convert.ToDecimal(dr.Announcement_Fish_price)) > 75)
                    if (result3 == null)
                        if (dr.Material.Substring(4, 1).Equals("S"))
                        {
                            if (Convert.ToDouble(dr.Equivalent_Fish_price) - Convert.ToDouble(dr.Announcement_Fish_price) > 300)
                                gv.JSProperties["cpKeyValue"] = string.Format("{0}|{1}", "Margin", args.Keys["ID"]);
                        }
                        else
                        {
                            gv.JSProperties["cpKeyValue"] = string.Format("{0}|{1}", "Margin", args.Keys["ID"]);
                        }
            }
            //}
            //if (_dt != null)
            //    if (_dt.Columns.Count > 0)
            //    {
            //        var valuesupcha = new[] { "Secondary Packaging", "FOB price(18 - digits)" };
            //        DataRow[] result = _dt.Select(string.Format("Calcu='{0}'", 6));
            //        foreach (DataRow _rw in result)
            //        {
            //            if (!valuesupcha.Any(_rw["Name"].ToString().Contains))
            //            {
            //                DataRow rwupcharge = _UpChargedt.Select(string.Format("UpCharge in ('{0}') and SubID ='{1}' and RequestNo ='{2}'",
            //                    _rw["Name"], args.Keys["ID"], hGeID["GeID"])).FirstOrDefault();
            //                if (rwupcharge != null)
            //                {
            //                    rwupcharge["UpchargeGroup"] = _rw["Component"];
            //                    rwupcharge["UpCharge"] = _rw["Name"];
            //                    rwupcharge["Quantity"] = _rw["Quantity"];
            //                    rwupcharge["StdPackSize"] = _rw["Unit"];
            //                }
            //                else
            //                {
            //                    DataRow _rwupcharge = _UpChargedt.NewRow();
            //                    int NextRowID = Convert.ToInt32(_UpChargedt.AsEnumerable()
            //                        .Max(row => row["ID"]));
            //                    NextRowID++;
            //                    _UpChargedt.Rows.Add(NextRowID,
            //                        _rw["Component"],
            //                        _rw["Name"],
            //                        _rw["Price"],
            //                        _rw["Quantity"],
            //                        _rw["Currency"],
            //                        _rw["Result"],
            //                        _rw["Unit"],
            //                        args.Keys["RowID"],
            //                        hGeID["GeID"]);
            //                }
            //            }
            //        }
            //    }
        }
        foreach (var args in e.DeleteValues)
        {
            var rw = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(args.Keys["ID"]));
            //DataRow rw = tcustomer.Rows.Find(args.Keys["ID"]);
            rw.Mark = rw.Mark.ToString() == "Del" ? "" : "Del";
            separa = "";
            listItems.Remove(rw);
            //deleteItems(args.Keys["ID"]);
            //tcustomer.AcceptChanges();
        }
        //buildcalcu();
        
        e.Handled = true;
    }

    protected void cpUpdateFiles_Callback(object sender, CallbackEventArgsBase e)
    {
        string[] param = e.Parameter.Split('|');
        switch (param[0])
        {
            case "reload":
                BuildSelect_FileDetails();
                break;
        }
    }
    protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string item = e.Row.Cells[0].Text;
            foreach (Button button in e.Row.Cells[4].Controls.OfType<Button>())
            {
                if (button.CommandName == "Delete")
                {
                    button.Attributes["onclick"] = "if(!confirm('Do you want to delete " + item + "?')){ return false; };";
                }
            }
        }
    }
    protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int index = Convert.ToInt32(e.RowIndex);
        USP_Select_FileDetails.Rows[index].Delete();
    }
    //protected void ASPxCallbackPanel1_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    if (hf.Contains("isNewClicked"))
    //    {
    //        if ((bool)hf.Get("isNewClicked") == true)
    //        {
    //            gv.SettingsEditing.Mode = GridViewEditingMode.EditForm;
    //            hf.Set("isNewClicked", false);
    //            gv.AddNewRow();
    //        }
    //        else
    //            gv.SettingsEditing.Mode = GridViewEditingMode.Batch;
    //    }
    //}


    protected void gv_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
    {
        if(e.Column.FieldName == "Checked")
            e.Editor.ReadOnly = true;
        if (e.Column.FieldName != "Material") return;
        (e.Editor as ASPxButtonEdit).ClientEnabled = tbRequestNo.Text.Contains("#")? true: false;

        if (e.Column.FieldName == "Equivalent" && !string.Format("{0}", hfStatusApp["StatusApp"]).Equals("0"))
        {
            e.Column.Visible = false;

        }
    }

    protected void lbUpcharge_Callback(object sender, CallbackEventArgsBase e)
    {
        lbUpcharge.DataBind();
    }

    protected void CmbAssignee_Callback(object sender, CallbackEventArgsBase e)
    {
        if (string.IsNullOrEmpty(e.Parameter)) return;
        string[] arg = e.Parameter.Split('|');
        SqlParameter[] param = {new SqlParameter("@Id",string.Format("{0}",hGeID["GeID"])),
                        new SqlParameter("@table",string.Format("{0}",arg[0])),
                        new SqlParameter("@zone",string.Format("{0}",cmbZone.Value)),
                        new SqlParameter("@username",hfuser["user_name"].ToString())};
        CmbAssignee.DataSource = cs.GetRelatedResources("spGetLevelApprove", param);
        CmbAssignee.DataBind();
    }
    //protected void gv_ContextMenuItemVisibility(object sender, ASPxGridViewContextMenuItemVisibilityEventArgs e)
    //{
    //    ASPxGridView g = (ASPxGridView)sender;
    //    if (e.MenuType == GridViewContextMenuType.Rows)
    //    {
    //        GridViewContextMenuItem menuItemSelected = e.Items.Find(item => item.Name == "OnlySelectedRows") as GridViewContextMenuItem;
    //        GridViewContextMenuItem menuItemSelectedAndDiscontinued = e.Items.Find(item => item.Name == "OnlySelectedAndDiscontinuedRows") as GridViewContextMenuItem;
    //        for (int i = 0; i < g.VisibleRowCount; i++)
    //        {
    //            e.SetVisible(menuItemSelected, i, g.Selection.IsRowSelected(i));
    //            e.SetEnabled(menuItemSelectedAndDiscontinued, i, g.Selection.IsRowSelected(i) && (bool)g.GetRowValues(i, "Discontinued"));
    //        }
    //    }
    //}

    protected void gv_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
    {
        ASPxGridView g = (sender as ASPxGridView);
        if (e.VisibleIndex == -1) return;
        //bool isOddRow = e.VisibleIndex % 2 == 0;
        //if (isOddRow)
        //{  // some condition
        //   // hide the Edit button
        //    if (e.ButtonType == ColumnCommandButtonType.Edit)
        //        e.Visible = false;

        //    // disable the selction checkbox
        //    if (e.ButtonType == ColumnCommandButtonType.SelectCheckbox)
        //        e.Enabled = false;
        //}
        bool ischeck = false;//change 28/04/2023 by voravut
        if (e.ButtonType == ColumnCommandButtonType.SelectCheckbox && ischeck)
        {
            object fieldValue = (sender as ASPxGridView).GetRowValues(e.VisibleIndex, "Notes");

            e.Enabled = false;
            object key = (sender as ASPxGridView).GetRowValues(e.VisibleIndex, "ID");
            if(!string.IsNullOrEmpty(string.Format("{0}", key)))
            {
                var rcus = listItems.FirstOrDefault(x => x.ID == Convert.ToInt32(key));
                if (rcus != null)
                {
                    if (hfStatusApp["StatusApp"].ToString().Equals("9"))
                    {
                        e.Enabled = rcus.IsAccept.Equals("level2") ? true : false;
                    }
                    else if (hfStatusApp["StatusApp"].ToString().Equals("8"))
                    {
                        e.Enabled = rcus.IsAccept.Contains("level") ? true : false;
                    }
                    //   if (Convert.ToBoolean(string.Format("{0}", rcus.IsAccept) == "" ? 1 : 0) )
                    //   (sender as ASPxGridView).Selection.SelectRowByKey(int.Parse(key.ToString()));

                    //DataRow xdr = USP_Select_FileDetails.Select(string.Format("SubID={0} and Result='{1}' and LastUpdateBy='{2}' and IsApprove='0'", 
                    //    key.ToString(), "Margin", cs.CurUserName)).FirstOrDefault();
                    //change 28/04/2023 by voravut
                    //if (string.Format("{0}", rcus.isapprove) != "")
                    //    (sender as ASPxGridView).Selection.SetSelection(e.VisibleIndex, true);
                }
            }
        }
        //if (e.ButtonType == ColumnCommandButtonType.SelectCheckbox)
        //    if (!Convert.IsDBNull(g.GetRowValues(e.VisibleIndex, "Notes"))){
        //       e.Visible = false;  
        //       //e.Enabled = true;
        //    }
    }

    protected void gv_HtmlCommandCellPrepared(object sender, ASPxGridViewTableCommandCellEventArgs e)
    {
        ASPxGridView grid = (sender as ASPxGridView);
        if (e.CommandCellType == GridViewTableCommandCellType.Data)
        {
            if (!System.Convert.IsDBNull(grid.GetRowValuesByKeyValue(e.KeyValue, "Notes")))
            {
                ((WebControl)e.Cell.Controls[0]).Enabled = false;
            }
        }
    }

    protected void gridData_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            //e.Items.Add(e.CreateItem("Copied", "Copied"));
            //e.Items.Add(e.CreateItem("Revised", "Revised"));
            var item = e.CreateItem("Quotation", "Quotation");
            item.Image.Url = @"~/Content/Images/excel.gif";
            e.Items.Add(item);

            item = e.CreateItem("File calculation sheet", "calculation");
            item.BeginGroup = true;
            item.Image.Url = @"~/Content/Images/excel.gif";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), item);
        }
    }
    private static void AddMenuSubItem(GridViewContextMenuItem parentItem, string text, string name, string iconID, bool isPostBack)
    {
        var exportToXlsItem = parentItem.Items.Add(text, name);
        exportToXlsItem.Image.Url = iconID;
    }

    protected void gv_RowValidating(object sender, ASPxDataValidationEventArgs e)
    {
        var SubContainers = e.NewValues["SubContainers"];
        if (CmbIncoterm.Value.Equals("CFR") || CmbIncoterm.Value.Equals("CIF") && SubContainers.ToString()=="")
        {
            AddError(e.Errors, gv.Columns["SubContainers"], "Invalid Case per fcl.");
            e.RowError = "Correct validation errors";
        }
    }
    void AddError(Dictionary<GridViewColumn, string> errors, GridViewColumn column, string errorText)
    {
        if (errors.ContainsKey(column)) return;
        errors[column] = errorText;
    }


    protected void gv_DataBound(object sender, EventArgs e)
    {
        gv.Columns["FishGroup"].Visible = string.Format("{0}", hfStatusApp["StatusApp"]).Equals("0") ? false : true;
        gv.Columns["EUGEN"].Visible = string.Format("{0}", hfStatusApp["StatusApp"]).Equals("0") ? false : true;  
        gv.Columns["Apply"].Visible = string.Format("{0}", hfStatusApp["StatusApp"]).Equals("0") ? true : false;
        //gv.Columns["Equivalent_Margin"].Visible = string.Format("{0}", hfStatusApp["StatusApp"]).Equals("0") ? true : false;
    }

    protected void cb_Init(object sender, EventArgs e)
    {
        ASPxCheckBox chbx = (sender as ASPxCheckBox);
        GridViewDataItemTemplateContainer gvditc = chbx.NamingContainer as GridViewDataItemTemplateContainer;
        chbx.JSProperties["cpLibID"] = gvditc.KeyValue.ToString();
    }
}

class tempEditButton : ITemplate
{
    //private List<string> data()
    //{
    //    MyDataModule mycla = new MyDataModule();
    //    DataTable table = mycla.builditems("select Name from MasUserType");
    //    List<string> list = table.AsEnumerable()
    //                   .Select(r => r.Field<string>("Name"))
    //                   .ToList();
    //    return list;
    //    //return new List<string>(new string[] { "101", "102", "103", "901" });
    //}
    public void InstantiateIn(Control container)
    {
        ASPxButtonEdit bu = new ASPxButtonEdit();
        //GridViewDataItemTemplateContainer gridContainer = (GridViewDataItemTemplateContainer)container;
        bu.ID = "button";
        
        bu.Border.BorderWidth = 0;
        container.Controls.Add(bu);
    }
}
public class GridDataItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Result { get; set; }
    public string StatusApp { get; set; }
}
public class StdItems
{
    public int ID { get; set; }
    public string Material { get; set; }
    public string Description { get; set; }
    public List<TransUtilize> Utilize { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string RawMaterial { get; set; }
    public string PackSize { get; set; }
    public string Yield { get; set; }
    public string FillWeight { get; set; }
    public string SubContainers { get; set; }
    public string Media { get; set; }
    public string Packaging { get; set; }
    public string LOHCost { get; set; }
    public string PackingStyle { get; set; }
    public string SecPackaging { get; set; }
    public List<TransUpCharge> Upcharge { get; set; }
    public string totalUpcharge { get; set; }
    public string Commission { get; set; }
    public string OverPrice { get; set; }
    public string OverType { get; set; }
    public string Pacifical { get; set; }
    public string MSC { get; set; }
    public string Margin { get; set; }
    public string MinPrice { get; set; }
    public string OfferPrice { get; set; }
    public string RequestNo { get; set; }
    public string Mark { get; set; }
    public string IsAccept { get; set; }
    public string Notes { get; set; }
    public string Attached { get; set; }
    public string Equivalent_Fish_price { get; set; }
    public string Announcement_Fish_price { get; set; }
    public string Authorized_price { get; set; }
    public string Equivalent_Margin { get; set; }
    public string Apply { get; set; }
    public string Bidprice { get; set; }
    public string marginBid { get; set; }
    //public bool IsActive { get; set; }
    public List<TransStdFileDetails> FileDetails { get; set; }
    public List<TransCal> cal { get; set; }
    public string isapprove { get; set; }
    public string diff_price { get; set; }
    public string FishGroup { get; set; }
    public string EUGEN { get; set; }
}    
public class TransUpCharge
{
    public int RowID { get; set; }
    public string UpchargeGroup { get; set; }
    public string UpCharge { get; set; }
    public string Price { get; set; }
    public string Quantity { get; set; }
    public string Currency { get; set; }
    public string Result { get; set; }
    public string stdPackSize { get; set; }
    public string SubID { get; set; }
    public string RequestNo { get; set; }
    public string Calcu { get; set; }
}
public class TransCal
{
    public int RowID { get; set; }
    public string Component { get; set; }
    public string Name { get; set; }
    public string Currency { get; set; }
    public double Result { get; set; }
    public int Calcu { get; set; }
    public string Quantity { get; set; }
    public string Price { get; set; }
    public string Unit { get; set; }
    public string BaseUnit { get; set; }
}
public class TransUtilize
{
    public int RowID { get; set; }
    public string Result { get; set; }
    public string MonthName { get; set; }
    public string Cost { get; set; }
    public string SubID { get; set; }
    public string RequestNo { get; set; }
    public string Calcu { get; set; }
}
public class TransStdFileDetails
{
    public int ID { get; set; }
    //public string Component { get; set; }
    public string Result { get; set; }
    public string Name { get; set; }
    public string Notes { get; set; }
    public Byte[] Attached { get; set; }
    public string IsApprove { get; set; }
    public string SubID { get; set; }
    public string RequestNo { get; set; }
}

public class StdHistory
{
    public int ID { get; set; }
    public string UserName { get; set; }
    public string Remark { get; set; }
    public DateTime CreateOn { get; set; }
    public string RequestNo { get; set; }
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
        box.ID = "ASPxCheckBox1";
        box.Checked = _isSelected;
        box.ClientSideEvents.CheckedChanged = "function(s,e){grid.PerformCallback(s.GetChecked());}";
        container.Controls.Add(box);
    }
}
class ButtonTemplate : ITemplate
{
    ASPxGridView g = new ASPxGridView();
    List<StdItems> litems = new List<StdItems>();
    public ButtonTemplate(ASPxGridView ASPxGridView1, List<StdItems> listItems)
    {
        this.g = ASPxGridView1;
        this.litems = listItems;
    }
    public void InstantiateIn(Control _container)
    {
        GridViewEditItemTemplateContainer container = _container as GridViewEditItemTemplateContainer;
        DevExpress.Web.ASPxUploadControl ASPxUploadControl1 = new DevExpress.Web.ASPxUploadControl();
        ASPxUploadControl1.ID = "ASPxUploadControl1";
        ASPxUploadControl1.FileUploadComplete += ASPxUploadControl1_FileUploadComplete;
        ASPxUploadControl1.UploadMode = UploadControlUploadMode.Auto;
        ASPxUploadControl1.FileUploadMode = UploadControlFileUploadMode.OnPageLoad;
        ASPxUploadControl1.AutoStartUpload = true;
        ASPxUploadControl1.ClientSideEvents.FileUploadComplete = "ASPxUploadControl1_OnFileUploadComplete";
        ASPxUploadControl1.CssClass = "navButtons prevButton";
        container.Controls.Add(ASPxUploadControl1);
    }
    protected void ASPxUploadControl1_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
    {
        if (e.IsValid)
        {
            string fileName = g.EditingRowVisibleIndex + e.UploadedFile.FileName;
            int editingRowVisibleIndex = g.EditingRowVisibleIndex;
            int FocusedRowIndex = g.FocusedRowIndex;
            var rowValue = g.GetRowValues(FocusedRowIndex, g.KeyFieldName).ToString();
            var rowSubID = g.GetRowValues(FocusedRowIndex, "SubID").ToString();
            var obj = litems.FirstOrDefault(x => x.ID == Convert.ToInt32(rowSubID));
            if (obj != null)
            {
                List<TransStdFileDetails> list3 = obj.FileDetails;
                //string fileName = e.UploadedFile.FileName;
                string path = "~/Content/UploadControl/" + fileName;
                byte[] FileData = e.UploadedFile.FileBytes;
                list3.Where(c => c.ID == Convert.ToInt32(rowValue))
                .Select(c =>
                {
                    c.Name = e.UploadedFile.FileName;
                    c.Attached = FileData;
                    return c;
                }).ToList();
                obj.FileDetails = list3;
            }
            //string resultFilePath = MapPath(resultFileUrl);
            //e.UploadedFile.SaveAs(Server.MapPath(path), true);
            //FileList[ASPxGridView1.EditingRowVisibleIndex].Url = Page.ResolveUrl(path);
            //FileList[ASPxGridView1.EditingRowVisibleIndex].FileName = fileName;
            //Session["list"] = FileList;
            e.CallbackData = fileName;
        }
    }
}