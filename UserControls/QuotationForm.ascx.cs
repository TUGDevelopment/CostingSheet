//using DevExpress.Data;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
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

public partial class UserControls_QuotationForm : MasterUserControl
{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    const string PageSizeSessionKey = "ed5e843d-cff7-47a7-815e-832923f7fb09";
    HttpContext c = HttpContext.Current;
    ASPxSpreadsheet Spreadsheet = new ASPxSpreadsheet();
    public DataTable tcustomer
    {
        get { return Page.Session["tcustomer"] == null ? null : (DataTable)Page.Session["tcustomer"]; }
        set { Page.Session["tcustomer"] = value; }
    }
    //private DataTable tCalculation
    //{
    //    get { return Page.Session["CustomTable"] == null ? null : (DataTable)Page.Session["CustomTable"]; }
    //    set { Page.Session["CustomTable"] = value; }
    //}
    string FilePath
    {
        get { return Page.Session["sessionFile"] == null ? null : Page.Session["sessionFile"].ToString(); }
        set { Page.Session["sessionFile"] = value; }
    }
    //private DataTable _dt
    //{
    //    get { return Page.Session["seGetMyData"] == null ? null : (DataTable)Page.Session["seGetMyData"]; }
    //    set { Page.Session["seGetMyData"] = value; }
    //}
    protected string SearchText { get { return Utils.GetSearchText(Page); } }
    string[] myarray = new[] { "RequestNo", "SubID"};
    protected void Page_Load(object sender, EventArgs e)
    {
        //formLayout.FindItemOrGroupByName("Uploadfile").Visible = false;
        if (!this.IsPostBack)
            SetInitialRow();
    }
    public void SetInitialRow()
    {
        //if (!string.IsNullOrEmpty(hfid["hidden_value"].ToString()))
        hfid["hidden_value"] = string.Empty;
        hfpara["value"] = string.Empty;
        hftype["type"] = string.Format("{0}", 0);
        //user_name.Replace("fo5910155", "MO581192");
        username["user_name"] = user_name;
        usertp["usertype"] = string.Format("{0}", cs.GetData(user_name, "usertype"));
        editor["Name"] = cs.IsMemberOfRole(string.Format("{0}", 0));
        approv["approv"] = string.Format("{0}", "");
        hfStatusApp["StatusApp"] = string.Format("{0}", 0);
        //hfBu["BU"] = string.Format("{0}", cs.GetData(user_name, "bu"));
        //Update();
    }
    //protected void PreviewPanel_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    var text = string.Format("<div align='center'><h1>Access denied</h1><br/>You are not authorized to access this page.</div>", e.Parameter);
    //    PreviewPanel.Controls.Add(new LiteralControl(text));
    //}
    public override void Update()
    {
        //gridData.DataSource = dsgv;
        //DataTable dt = ((DataView)dsgv.Select(DataSourceSelectArguments.Empty)).Table;
        gridData.DataBind();
    }
    string buildsum(DataRow _dr)
    {
        Spreadsheet.Document.LoadDocument(Server.MapPath("~/App_Data/Documents/Example.xlsx"));
        Spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;
        Worksheet _ws = Spreadsheet.Document.Worksheets[0];
        DataTable dtproduct = cs.GetProductFormula(_dr["ID"].ToString());
        _ws.Cells["A1"].Value = string.Format("{0}", _dr["CanSize"]);
        _ws.Cells["B3"].Value = string.Format("{0}", _dr["PackSize"]);
        _ws.Cells["B4"].Value = string.Format("{0}", _dr["ExchangeRate"]);
        int c = 7;
        int totalrm = c;
        string strSQl = @"select CONCAT('[',SAPMaterial,']',Description) as name,* from TranshCosting where Requestno='" + _dr["ID"].ToString() + "'";
        DataTable dth = cs.builditems(strSQl);
        if (dth.Rows.Count > 0)
        {
            for (int o = 1; o <= dtproduct.Rows.Count; o++)
            {
                c++;
                DataRow _drp = dtproduct.Rows[o - 1];
                _ws.Cells["D" + c].Formula = string.Format("= {0}", cs.GetSinglePrice(_drp, _dr));
            }
            c++;
            _ws.Cells["D" + c].Formula = string.Format("=SUM(D{0}:D{1})", totalrm, c - 1);
            _ws.Cells["D" + c].NumberFormat = "#,###.##";
            totalrm = c;
            c++;
            DataRow[] _dth = dth.Select("component='Secondary Packaging'");
            int total = c;
            foreach (var _d in _dth)
            {
                c++;
                _ws.Cells["D" + c].Formula = string.Format("= {0}", _d["Amount"]);
            }
            c++;
            DataRow[] dtloss = dth.Select("component = 'Loss'");
            foreach (var _r in dtloss)
            {
                _ws.Cells["C" + c].Value = string.Format("{0}%", _r["Per"]);
                _ws.Cells["D" + c].Formula = string.Format("=SUM(D{0}:D{1})*$C${2}", total, c - 1, c);
                _ws.Cells["D" + c].NumberFormat = "0.##";
            }
            c++;
            _ws.Cells["D" + c].Formula = string.Format("=SUM(D{0}:D{1})", total, c - 1);
            total = c;
            c++;
            DataRow[] dtu = dth.Select("component ='Upcharge'");
            int totalup = c;
            foreach (var _u in dtu)
            {
                c++;
                _ws.Cells["D" + c].Formula = string.Format("={0}", _u["Amount"]);
            }
            c++;
            _ws.Cells["D" + c].Formula = string.Format("=SUM(D{0}:D{1})", totalup, c - 1);
            totalup = c;
            c++;
            _ws.Cells["A" + c].Font.Bold = true;
            c++;
            _ws.Cells["D" + c].Formula = string.Format("=SUM(D{0}+D{1})", total, totalup);
            _ws.Cells["D" + c].FillColor = System.Drawing.Color.Beige;
            c++;
            _ws.Cells["D" + c].Formula = string.Format("=+D{0}/$B$4", c - 1);
            c++;
            _ws.Cells["D" + c].Formula = string.Format("=+D{0}+D{1}", c - 1, totalrm);
            c++;
            c++;
            DataRow[] dtmargin = dth.Select("component = 'Margin'");
            foreach (var _r2 in dtmargin)
            {
                _ws.Cells["C" + c].FillColor = System.Drawing.Color.Beige;
                _ws.Cells["C" + c].Value = string.Format("{0}%", _r2["Per"]);
                _ws.Cells["D" + c].Formula = string.Format("=+D{0}*$C${1}", c + 1, c);
            }
            c++;
            _ws.Cells["D" + c].Formula = string.Format("=+D{0}+D{1}", c - 1, c - 3);
            _ws.Cells["D" + c].Font.Bold = true;
            c++;
            _ws.Cells["D" + c].Formula = string.Format("= D{0}/$B$3", c - 1);
            _ws.Cells["D" + c].Font.Bold = true;
            //c++;
            //_ws.Cells["D" + c].Formula = string.Format("=D{0}*$B$4", c - 1);
            //_ws.Cells["D" + c].Font.Bold = true;
        }
        _ws.Calculate();

        return _ws.Cells["D" + c].Value.ToString();
    }
        protected void gv_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        long id;
        if (!long.TryParse(args[1], out id))
            return;
        if (args[0] == "Clear")
        {
            foreach(DataRow found in tcustomer.Rows)
            found["Mark"] = "D";
            tcustomer.AcceptChanges();
        }

        if (args[0] == "new")
        {
            //    DataTable dt = (DataTable)gv.DataSource;
            //    string strSQL = string.Format(@"Costing='{0}' and Customer = '{1}' and ShipTo = '{2}' and Mark = 'X' ",
            //            CmbCostingNo.Text, CmbCustomer.Value, CmbShipTo.Value);
            //    DataRow r = dt.Select(strSQL).FirstOrDefault();
            if (tcustomer != null)
            {            
                string[] param = args[2].Split(',');
                foreach (string c in param)
                {
                    if (c.ToString().Substring(0, 1).Equals("S"))
                    {
                        string key = c.ToString().Substring(1, c.Length - 1);
                        DataRow drx = tcustomer.Select("CostingNo=" + key).FirstOrDefault();
                        if (drx == null)
                        {
                            int max = Convert.ToInt32(tcustomer.AsEnumerable()
                            .Max(row => row["ID"]));
                            max = max + 1;
                            DataRow dr = tcustomer.NewRow();
                            //,isnull((select b.VarietyPack from TransCostingHeader b where b.id=a.requestno),'') VarietyPack
                            if (c.Contains("_V"))
                            {
                                //DataTable dt2 = cs.builditems(string.Format("select b.* from TransCostingHeader b where b.id={0}", c.ToString().Replace("_V","")));
                                //foreach(DataRow dr2 in dt2.Rows)
                                //dr["MinPrice"] = string.Format("{0}", GetCostPrice(dr2));
                                //tcustomer.Rows.Add(dr);
                            }
                            else
                            {
                                string strSQL = string.Format(@"select a.* from TransFormulaHeader a where a.Id={0}", key.ToString());
                                DataTable dt = cs.builditems(strSQL);
                                foreach (DataRow xdr in dt.Rows)
                                {
                                    dr["CostNo"] = string.Format("{0}", xdr["CostNo"]);
                                    dr["Name"] = string.Format("{0}", xdr["Name"]);
                                    dr["Code"] = string.Format("{0}", xdr["Code"]);
                                    dr["RefSamples"] = string.Format("{0}", xdr["RefSamples"]);
                                    dr["ExchangeRate"] = GetExchangeRate(dt.Rows[0]["RequestNo"].ToString());
                                    dr["RequestType"] = string.Format("{0}", c.ToString().Substring(0, 1));
                                    dr["MinPrice"] = string.Format("{0}", GetMinPrice(dt.Select("Id=" + key).FirstOrDefault()));
                                    if (CmbCurrency.Value.ToString() == "THB")
                                        dr["MinPrice"] = Convert.ToDecimal(dr["ExchangeRate"]) * Convert.ToDecimal(dr["MinPrice"]);
                                    dr["ID"] = max;
                                    dr["RequestNo"] = xdr["RequestNo"].ToString();
                                    dr["CostingNo"] = xdr["ID"].ToString();
                                    string data = "StatusApp,Diff,PercentDiff,OfferPrice,OfferPrice_Adjust";
                                    string[] words = data.Split(',');
                                    foreach (string word in words)
                                    {
                                        dr[word] = "0";
                                    };
                                    tcustomer.Rows.Add(dr);
                                }
                            }
                        }
                    }
                    else
                    {
                        string valkey = c.ToString().Substring(1, c.Length - 1);
                        int maxval = Convert.ToInt32(tcustomer.AsEnumerable()
                            .Max(row => row["ID"]));
                        maxval = maxval + 1;
                        DataRow drval = tcustomer.NewRow();
                        string strSQL = string.Format(@"select a.* from TransCostingHeader a where a.Id={0}", valkey.ToString());
                        DataTable dt = cs.builditems(strSQL);
                        foreach (DataRow xdr in dt.Rows)
                        {
                            drval["CostNo"] = string.Format("{0}", xdr["MarketingNumber"]);
                            drval["Name"] = string.Format("{0}", "");
                            drval["Code"] = string.Format("{0}", xdr["VarietyPack"]);
                            drval["RefSamples"] = string.Format("{0}", xdr["RDNumber"]);
                            drval["CostingNo"] = xdr["ID"].ToString();
                            drval["RequestType"] = string.Format("{0}", c.ToString().Substring(0, 1));
                            drval["ID"] = maxval;
                            drval["StatusApp"] = "0";
                            drval["Diff"] = "0";
                            drval["PercentDiff"] = "0";
                            drval["OfferPrice"] = "0";
                            drval["OfferPrice_Adjust"] = "0";
                            drval["ExchangeRate"] = xdr["ExchangeRate"];
                            drval["RequestNo"] = xdr["ID"];
                            drval["MinPrice"] = buildsum(xdr);
                            //drval["ExchangeRate"] = GetExchangeRate(dt.Rows[0]["RequestNo"].ToString());
                            tcustomer.Rows.Add(drval);
                        }

                    }
                }
                
                //dr = bDataCustom(dr, max, "X");
            }
        }
        if (args[0] == "reload")
        {
            GetReloadCustom(args[1].ToString());
            //tcustomer.Merge(t);
            //tsubContainers = cs.builditems(string.Format(@"
            //    select *,'' as Mark from TransQuotationItems where RequestNo={0}", id.ToString()));
        }
        if (args[0] == "changed")
        {
            if (tcustomer != null)
                foreach (DataRow _rw in tcustomer.Rows)
                {
                    _rw["ExchangeRate"] = seExchangeRate.Text;
                }
        }

        //if(args[0] == "buildExch")

        gvcalculator(g, args, id);
        
    }
    void buildExch()
    {
        if (tcustomer != null)
            foreach (DataRow dr in tcustomer.Rows)
            {
                if (!CmbCurrency.Text.Contains("USD"))
                {
                    double _ExchangeRate = 0, _OfferPrice = 0, _MinPrice = 0;
                    _OfferPrice = (Convert.ToDouble(dr["OfferPrice"]) * Convert.ToDouble(dr["ExchangeRate"]));
                    _MinPrice = (Convert.ToDouble(dr["MinPrice"]) * Convert.ToDouble(dr["ExchangeRate"]));
                    double.TryParse(string.Format("{0}",seExchangeRate.Value), out _ExchangeRate);
                    if (_ExchangeRate != 0)
                    {
                        dr["OfferPriceExch"] = Convert.ToDouble(_OfferPrice / _ExchangeRate).ToString("F2");
                        dr["MinPriceExch"] = Convert.ToDouble(_MinPrice / _ExchangeRate).ToString("F2");
                    }
                    //dr["ExchangeRate"] = seExchangeRate.Value;
                }
                else
                {
                    dr["OfferPriceExch"] = dr["OfferPrice"];
                    dr["MinPriceExch"] = dr["MinPrice"];
                }
                if (CmbCurrency.Text.Contains("JPY")){
                    NumberFormatInfo nfi = new CultureInfo("ja-JP", false).NumberFormat;
                    dr["OfferPriceExch"] = Convert.ToInt64(Math.Floor(Convert.ToDouble(dr["OfferPriceExch"])));
                    dr["MinPriceExch"] = Convert.ToInt64(Math.Floor(Convert.ToDouble(dr["MinPriceExch"])));
                }
            }
    }
    string buildVariety(string Id)
    {
        ASPxSpreadsheet spreadsheet = new ASPxSpreadsheet();
        spreadsheet.Document.LoadDocument(Server.MapPath("~/App_Data/Documents/Example.xlsx"));
        spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;
        Worksheet worksheet = spreadsheet.Document.Worksheets[1];
        IWorkbook workbook = spreadsheet.Document;
        int c = 7;
        int totalrm = c;
        string strSQl = @"select CONCAT('[',SAPMaterial,']',Description) as name,* from TranshCosting where Requestno='" + Id.ToString() + "'";
        DataTable dth = cs.builditems(strSQl);
        if (dth.Rows.Count > 0)
        {

        }
            return "0";
    }
    string GetExchangeRate(string Id) {
        string strExchangeRate = "";
        DataTable dt = cs.builditems(@"select top 1 ExchangeRate from TransCostingHeader where id = '" + Id + "'");
        foreach (DataRow rw in dt.Rows){
            strExchangeRate = string.Format("{0}", rw["ExchangeRate"]);
        }
        return strExchangeRate;
        }
    void GetReloadCustom(string Keys)
    {
        tcustomer = new DataTable();
        //spGetQuotaCustomer
        SqlParameter[] param = {new SqlParameter("@RequestNo",Keys.ToString()),
                    new SqlParameter("@Id","0")};
        tcustomer = cs.GetRelatedResources("spGetQuotationItems", param);
        //tcustomer = cs.builditems(string.Format(@"
        //        select *,'' as Mark from transcustomer where RequestNo={0}", id.ToString()));
        tcustomer.PrimaryKey = new DataColumn[] { tcustomer.Columns["ID"] };
        foreach (DataRow dr in tcustomer.Rows)
        {
            DataTable dt = new DataTable();
            dt = cs.builditems(string.Format(@"
                select *,'' as Mark from TransQuotationItems where SubID={0} and RequestNo={1}", Keys.ToString(), dr["ID"]));
            string[] arr = { "MinPrice", "Overprice", "Extracost", "subContainers", "OfferPrice" };
            foreach (string s in arr)
            {
                List<string> list = new List<string>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                        list.Add(string.IsNullOrEmpty(r[s].ToString()) ? "0" : r[s].ToString());
                    dr[s] = String.Join("|", list.ToArray());
      
                }
            }
        }
    }
     
    
    DataTable GetTable()
    {
        // Here we create a DataTable with four columns.
        DataTable table = new DataTable();
        table.Columns.Add("RowID", typeof(int));
        table.Columns.Add("MinPrice", typeof(string));
        table.Columns.Add("Overprice", typeof(string));
        table.Columns.Add("Extracost", typeof(string));
        table.Columns.Add("SubContainers", typeof(string));
        table.Columns.Add("OfferPrice", typeof(string));
        table.Columns.Add("SubID", typeof(string));
        table.Columns.Add("RequestNo", typeof(string));
        return table;
    }
    protected void CmbDisponsition_Callback(object sender, CallbackEventArgsBase e)
    {
        if (string.IsNullOrEmpty(e.Parameter)) return;
        SqlParameter[] param = {new SqlParameter("@Id",e.Parameter.ToString()),
                        new SqlParameter("@table","TransQuotationHeader"),
                        new SqlParameter("@username",user_name.ToString())};
        CmbDisponsition.DataSource = cs.GetRelatedResources("spGetStatusApproval", param);
        CmbDisponsition.DataBind();
    }

    protected void gridData_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        var args = e.Parameters.Split('|');
        long id;
        if (!long.TryParse(args[2], out id))
            return;
        var result = new Dictionary<string, string>();
        if (args[1] == "EditDraft" || args[1] == "save")
        {
            SqlParameter[] param = {new SqlParameter("@Id",id.ToString()),
                        new SqlParameter("@username",user_name.ToString())
                };
            DataTable dt = cs.GetRelatedResources("spselectQuotaHeader", param);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                result["RequestNo"] = dr["RequestNo"].ToString();
                result["editor"] = dr["editor"].ToString();
                result["UniqueColumn"] = dr["UniqueColumn"].ToString();
                result["StatusApp"] = dr["StatusApp"].ToString();
                result["ID"] = dr["ID"].ToString();
                result["Brand"] = string.Format("{0}", dr["Brand"]);
                result["Commission"] = string.Format("{0}", dr["Commission"]);
                result["Incoterm"] = string.Format("{0}", dr["Incoterm"]);
                result["Route"] = string.Format("{0}", dr["Route"]);
                result["Size"] = string.Format("{0}", dr["Size"]);
                result["PaymentTerm"] = string.Format("{0}", dr["PaymentTerm"]);
                result["Interest"] = string.Format("{0}", dr["Interest"]);
                result["Freight"] = string.Format("{0}", dr["Freight"]);
                result["Insurance"] = string.Format("{0}", dr["Insurance"]);
                result["Remark"]= string.Format("{0}", dr["Remark"]);
                result["Customer"]= string.Format("{0}", dr["Customer"]);
                result["ShipTo"]= string.Format("{0}", dr["ShipTo"]);
                result["Currency"] = string.Format("{0}", dr["Currency"]);
                result["ExchangeRate"] = string.Format("{0}", dr["ExchangeRate"]);
                e.Result = result;
            }
        }
        if (args[1] == "New")
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
                result["editor"] = String.Format("{0}", 0);
                e.Result = result;
            }
        }
    }
    //DataTable _tsubContainers(DataRow dr)
    //{
    //    //spGetQuotationItems
    //    SqlParameter[] param = {new SqlParameter("@RequestNo",dr["Folio"].ToString()),
    //            //new SqlParameter("@Id",dr["CostingNo"].ToString())
    //    };
    //    DataTable dt = cs.GetRelatedResources("spGetQuotationItems", param);
    //    dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
    //    return dt;
    //}
    //string calculation(string p, string symbol)
    //{
    //    if (CustomTable == null)
    //        return "";
    //    object sumObject; double loss; double _r;
    //    string strSQL = "";
    //    strSQL = @"MasPFLoss Where PackageType='" + p.ToString() + "' order  BY CASE SubType ";
    //    strSQL += "When 'Raw Material & Ingredient' then 0 ";
    //    strSQL += "When 'Primary Packaging' then 1 ";
    //    strSQL += "When 'Secondary Packaging' then 2 ";
    //    strSQL += "When 'Damage & Loss' then 3 ";
    //    strSQL += "ELSE 99 END";
    //    DataTable Myresult = cs.GetElement(strSQL);

    //    myCollection = new List<string>();
    //    //sumObject = CustomTable.Compute("Sum(Convert(PriceOfCarton, 'System.Int32'))", string.Format("Formula={0}",symbol));
    //    sumObject = CustomTable.AsEnumerable()
    //               .Where(r => r.Field<int>("Formula") == Convert.ToInt32(symbol))
    //               .Sum(x => Convert.ToDouble(x["PriceOfCarton"]));

    //    //double _test = 0;
    //    //DataRow[] test = CustomTable.Select(string.Format("Formula='{0}'", symbol.ToString()));
    //    //foreach (DataRow dr in test)
    //    //    _test += Convert.ToDouble(dr["PriceOfCarton"].ToString() + dr["LBRate"].ToString());

    //    myCollection.Add(sumObject.ToString());//total
    //    if (CustomTable == null) return "0";
    //    var result = CustomTable.Select(string.Format(@"
    //    mark in ('R') and Formula='{0}'", symbol.ToString()));
    //    _r = result.AsEnumerable()
    //           .Sum(x => Convert.ToDouble(x["PriceOfCarton"]));

    //    loss = Myresult.AsEnumerable()
    //               .Where(r => r.Field<string>("SubType") == "Raw Material & Ingredient")
    //               .Sum(x => Convert.ToDouble(x["Loss"]));
    //    myCollection.Add((Convert.ToDouble(_r.ToString()) * (loss / 100)).ToString());//raw+ing

    //    var values = new[] { "Primary Packaging", "Secondary Packaging" };// Secondary Packaging
    //    for (int i = 0; i < values.Length; i++)
    //    {
    //        loss = 1;
    //        loss = Myresult.AsEnumerable()
    //                .Where(r => r.Field<string>("SubType").ToLower() == values[i].ToString().ToLower())
    //                .Sum(x => Convert.ToDouble(x["Loss"]));

    //        _r = new double();
    //        result = CustomTable.Select(string.Format("Component in ('{0}') and Formula='{1}'", values[i], symbol.ToString()));
    //        _r = result.AsEnumerable()
    //                .Sum(x => Convert.ToDouble(x["PriceOfCarton"]));
    //        myCollection.Add((Convert.ToDouble(_r.ToString()) * (loss / 100)).ToString());//Packaging
    //    }
    //    //,"margin"
    //    double _margin = 0;
    //    result = CustomTable.Select(string.Format("Component in ('Margin') and Formula='{0}'", symbol.ToString()));
    //    foreach (DataRow dr in result)
    //        _margin = Convert.ToDouble(dr["Per"].ToString());
    //    double _total = 0;
    //    foreach (string c in myCollection)
    //    {
    //        _total += Convert.ToDouble(c);
    //    }
    //    _total = _total / (Convert.ToDouble(100 - _margin) / 100);
    //    return _total.ToString("F",CultureInfo.InvariantCulture);
    //}
    DataTable GetLoss2(string Key, string Id, string utype)
    {
        SqlParameter[] p = { new SqlParameter("@PackageType", Key.ToString()),
        new SqlParameter("@To",cs.GetValidTo(Id)),
        new SqlParameter("@u",string.Format("{0}", utype))};
        return cs.GetRelatedResources("spGetLoss", p);
    }
    string GetCostPrice(DataRow row)
    {
        decimal r = 0;
        DataTable MyTable = reload(row);
        if (MyTable != null)
        {
            string[] SubType = Regex.Split("Raw Material & Ingredient;Primary Packaging;Secondary Packaging", ";");
            string[] valueType = Regex.Split("RmIng;PrimaryPkg", ";");
            string[] arr = Regex.Split("RM;Ing;UpCharge;LOH;RmIng;PrimaryPkg", ";");
            foreach (DataRow rw in MyTable.Rows)
            {
                for (int i = 0; i <= arr.Length - 1; i++)
                    rw[arr[i]] = (Convert.ToDecimal(rw[arr[i]]) / Convert.ToDecimal(rw["PackSizeh"])) * Convert.ToDecimal(rw["PackSize"]);
                decimal _rm, _totalrm;
                var _Packaging = row["Packaging"].ToString();
                var tableloss = GetLoss2(_Packaging, row["RequestNo"].ToString(), string.Format("{0}", row["UserType"]));
                for (int i = 0; i <= valueType.Length - 1; i++)
                {
                    _rm = Convert.ToDecimal(rw[valueType[i]]);
                    var _drloss = tableloss.Select("SubType='" + SubType[i] + "'").FirstOrDefault();
                    decimal _loss = 0;
                    decimal.TryParse(_drloss["Loss"].ToString(), out _loss);
                    _totalrm = _rm * (Convert.ToDecimal(_loss) / 100);
                    rw["Loss"] = Convert.ToDecimal(rw["Loss"]) + _totalrm;
                    rw["FOB"] = Convert.ToDecimal(rw["FOB"]) + _totalrm + Convert.ToDecimal(rw[valueType[i]]);
                }
                decimal totalCount = 0;
                totalCount = Convert.ToDecimal(rw["FOB"]) + Convert.ToDecimal(rw["LOH"]) + Convert.ToDecimal(rw["UpCharge"]);
                object sumObject = string.Format(@"{0}", rw["PerMargin"].ToString() == "" ? "0" : rw["PerMargin"].ToString());

                decimal _MinPrice = (totalCount / ((100 - Convert.ToDecimal(sumObject)) / 100));
                rw["Margin"] = _MinPrice * (Convert.ToDecimal(sumObject) / 100);

                List<decimal> list1 = new List<decimal>() { Convert.ToDecimal(rw["Ing"]),
                    Convert.ToDecimal(rw["RM"]),
                    Convert.ToDecimal(rw["PrimaryPkg"]),0,//Convert.ToDecimal(rw["SecondaryPkg"]), 
                    Convert.ToDecimal(rw["LOH"]),
                    Convert.ToDecimal(rw["UpCharge"]),
                    Convert.ToDecimal(rw["Loss"]),
                    Convert.ToDecimal(rw["Margin"])};
                r = list1.Sum();
                if (r > 0)
                {
                    if (rw["Rate"].ToString() != "")
                        r = r / Convert.ToDecimal(rw["Rate"]);
                }
            }
        }
        return r.ToString("F", CultureInfo.InvariantCulture);
    }
    DataTable reload(DataRow _drow)
    {
        ServiceCS mycs = new ServiceCS();
        DataTable dt = new DataTable();
        SqlParameter[] param = {new SqlParameter("@Id",_drow["ID"].ToString()),
                new SqlParameter("@username",user_name.ToString())};
        dt = cs.GetRelatedResources("spGetFormulaItems", param);
        var result = cs._createData().Clone();
        result.Clear();
                double[] doarray = new double[7];
            DataRow _r = result.NewRow();
            _r["RequestNo"] = _drow["RequestNo"].ToString();
            _r["Formula"] = _drow["Formula"].ToString();
        var values = new[] { "ingredient", "solid portion", "solution" };
        foreach (DataRow row in dt.Rows)
        {
            if (row["Component"].ToString().ToLower().Contains("raw material"))
                doarray[0] += Convert.ToDouble(row["PriceOfCarton"]);
            else if (row["Component"].ToString().ToLower().Contains("primary packaging"))
                doarray[3] += Convert.ToDouble(row["PriceOfCarton"]);
            else if (row["Component"].ToString().ToLower().Contains("secondary packaging"))
                doarray[4] += Convert.ToDouble(row["PriceOfCarton"]);
            else if (row["Component"].ToString().ToLower().Contains("upcharge"))
                doarray[5] += Convert.ToDouble(row["PriceOfCarton"]);
            else if (row["Component"].ToString().ToLower().Contains("margin"))
                _r["PerMargin"] = row["Per"];
            else if (row["Component"].ToString().ToLower().Contains("labor & overhead") || row["Component"].ToString().ToLower().Contains("loh"))
            {
                //string sum = dt.Compute("sum(GUnit)", "LBRate<>'' and Formula ='" + x.Key.ToString() + "'").ToString();
                //if (!string.IsNullOrEmpty(row["GUnit"].ToString()) && !string.IsNullOrEmpty(row["PriceOfCarton"].ToString()))
                //doarray[6]+= (Convert.ToDouble(row["PriceOfCarton"]) * Convert.ToDouble(row["GUnit"]))/ Convert.ToDouble(sum);
                //doarray[6] += Convert.ToDouble(row["PriceOfCarton"]);
            }
            else //if (row["Component"].ToString().ToLower() == "" || values.Any(row["Component"].ToString().ToLower().Contains))
            {
                doarray[1] += Convert.ToDouble(row["PriceOfCarton"]);
                doarray[2] += Convert.ToDouble(row["PriceOfCartonusd"]);
            }
            _r["Rate"] = row["Rate"];
                //if (row["Component"].ToString() != "Ingredient")
                //{
                //    DataRow dr = CustomTable.NewRow();
                //    dr = row;
                //    dr["RowID"] = CustomTable.Rows.Count + 9001;
                //    CustomTable.ImportRow(dr);
                //}
            }
            _r["Margin"] = 0;
            _r["Loss"] = 0;
            _r["FOB"] = 0;
            _r["RM"] = doarray[0];
            _r["Ing"] = doarray[1];
            _r["RmIng"] = doarray[0] + doarray[1];
            //_r["LOH"] = doarray[6];
			double LBRate = 0;//Labor & Overhead;
            DataTable dtFormula = cs.GetMergeDataSource(Convert.ToInt32(_drow["Formula"]),
                "3", _drow["RequestNo"].ToString());
        //DataRow _rw = dtFormula.Select("").FirstOrDefault();
            double _sumLBRate = 0;
            foreach (DataRow row in dtFormula.Rows)
            {
                if (row["SellingUnit"].ToString() == "%")
                {
                    object totalLBRate = dtFormula.Compute("Sum(LBRate)", "title in ('LOH','Labor & Overhead')").ToString();
                    _sumLBRate += Convert.ToDouble(totalLBRate) * (Convert.ToDouble(row["LBRate"])/100);
                }
                else
                    _sumLBRate += Convert.ToDouble(row["LBRate"]);
            }
            //object _sumLBRate = dtFormula.Compute("Sum(LBRate)", "").ToString();
            double.TryParse(_sumLBRate.ToString(), out LBRate);
            _r["LOH"] = LBRate;
            _r["PrimaryPkg"] = doarray[3];
            _r["SecondaryPkg"] = doarray[4];
            _r["UpCharge"] = string.Format("{0}", doarray[5]);
            double loh = 0;
            if (_r.IsNull("LOH"))
                _r["LOH"] = string.Format("{0}", loh);
            //if (doarray[0] > 0)
            //{
            //    DataRow dr = CustomTable.NewRow();
            //    dr["RowID"] = CustomTable.Rows.Count + 1;
            //    dr["Component"] = "Ingredient";
            //    dr["mark"] = "R";
            //    dr["Formula"] = x.Key.ToString();
            //    dr["PriceOfCarton"] = doarray[1].ToString();
            //    dr["PriceOfCartonusd"] = doarray[2].ToString("F",CultureInfo.InvariantCulture);
            //    CustomTable.Rows.Add(dr);
            //}
            result.Rows.Add(_r);
        return result;
    }
    
    List<string> myCollection = new List<string>();
    void postItems(string Keys)
    {
        //sold & shipto
        //DataTable data = new DataTable();
        //data = (DataTable)gv.DataSource;
        //if(data!=null)
        //foreach (DataRow row in data.Rows)
        //{
        //    SqlParameter[] param = { new SqlParameter("@Id", string.Format("{0}", row["ID"])),
        //                new SqlParameter("@SoldTo", string.Format("{0}", row["SoldTo"])),
        //                new SqlParameter("@ShipTo", string.Format("{0}", row["ShipTo"])),
        //                new SqlParameter("@Mark", string.Format("{0}", row["Mark"])),
        //                new SqlParameter("@SubID", string.Format("{0}", args[0])),
        //                new SqlParameter("@RequestNo", string.Format("{0}", args[1]))};
        //    cs.GetExecuteNonQuery("spInsertQuotaCustomer", param);
        //}
        //customer
        //DataTable customer = new DataTable();
        //customer = (DataTable)gvimport.DataSource;
        //if(_dt!=null)
        //foreach (DataRow row in _dt.Rows)
        //{
        //    SqlParameter[] param = new SqlParameter[] {new SqlParameter("@ID",string.Format("{0}", row["ID"])),
        //                new SqlParameter("@ProductName", string.Format("{0}", row["ProductName"])),
        //                new SqlParameter("@Mark", string.Format("{0}", row["Mark"])),
        //                new SqlParameter("@Material", string.Format("{0}", row["Material"])),
        //                new SqlParameter("@ShipTo", string.Format("{0}", row["ShipTo"])),
        //                new SqlParameter("@StatusApp", string.Format("{0}", row["StatusApp"])),
        //                new SqlParameter("@RD_ref", string.Format("{0}", row["RD_ref"])),
        //                new SqlParameter("@CostingNo", string.Format("{0}", row["CostingNo"])),
        //                new SqlParameter("@OfferPrice", string.Format("{0}", row["OfferPrice"])),
        //                new SqlParameter("@SubID", string.Format("{0}", Keys)),
        //                new SqlParameter("@RequestNo", string.Format("{0}", 0)),
        //                new SqlParameter("@Customer", string.Format("{0}", row["Customer"]))};
        //    cs.GetExecuteNonQuery("spinsertQuotationCustom", param);
        //}
    }
    void ApproveStep(string keys)
    {
        if (string.IsNullOrEmpty(CmbDisponsition.Text)) return;
        if (tcustomer.Rows.Count==0) return;
        //tsubContainers
        bool selected = false;
        //DataTable table = gvimport.DataSource as DataTable;
        DataRow result = tcustomer.Select(string.Format(@"StatusApp='{0}'",'A')).FirstOrDefault();
        if (result !=null)
            selected = true;
        using (SqlConnection con = new SqlConnection(strConn))
        {
            var value = hfStatusApp["StatusApp"].ToString();
            string status = "8";
            if (value.ToString() == "8")
                status = "9";
            if (value.ToString() == "9")
                status = "4";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spApproveStep";
            cmd.Parameters.AddWithValue("@Id", keys.ToString());
            cmd.Parameters.AddWithValue("@User", user_name.ToString());
            if (CmbDisponsition.Value.ToString() == "3")
                cmd.Parameters.AddWithValue("@StatusApp", CmbDisponsition.Value);
            else
                cmd.Parameters.AddWithValue("@StatusApp", selected==true? status : CmbDisponsition.Value);
            cmd.Parameters.AddWithValue("@table", "TransQuotationHeader");
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
                List<string> list = new List<string>();
                string[] split = dr["MailTo"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split)
                {
                    if (s.Trim() != "")
                        list.Add(cs.GetData(s, "email"));
                }
                string statusapp = " was " + CmbDisponsition.Text;
                string sender = String.Join(",", list.ToArray()); //string MailCc = "";
                if (selected == false)
                    statusapp = " was complete "; 
                list = new List<string>();
                split = dr["MailCc"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split)
                {
                    if (s.Trim() != "")
                        list.Add(cs.GetData(s, "email"));
                }
                string MailCc = String.Join(",", list.ToArray());
                string _link = "Quotation Request No.:" + dr["RequestNo"].ToString();
                _link += "<br/> Customer Name : " + CmbCustomer.Text;
                _link += "<br/> Payment Term : " + CmbPaymentTerm.Text;
                _link += "<br/> Incoterm : " + CmbIncoterm.Text;
                _link += "<br/> Create By : " + cs.GetData(dr["Requester"].ToString(), "fn");
                //_link += "<br/> Customer Name :" + dr["Assignee"].ToString();
                if (!string.IsNullOrEmpty(CmbReason.Text))
                    _link += "<br/> Reason :" + CmbReason.Text;
                _link += "<br/> Comment :" + mComment.Text;
                _link += "<br/> The document link --------><a href=" + cs.GetSettingValue() + "/Default.aspx?viewMode=QuotationForm&ID=" + keys.ToString();
                _link += " style='color: rgb(0,255,0)'><font color='#0000FF'>click</font></a>";
                cs.sendemail(sender, MailCc, _link, "GPC:Quotation :" + dr["RequestNo"].ToString() + statusapp);
                //cs.sendemail("voravut.somboornpong@thaiunion.com", MailCc, _link, "test system costing sheet No.:" + dr["RequestNo"].ToString() + statusapp);
            }
            con.Close();
        }
    }
    //string revised(string valuechange, string keys)
    //{
    //    string myfolio;
    //    using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        SqlCommand cmd = new SqlCommand();
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.CommandText = "spcopyCostingRevised";
    //        cmd.Parameters.AddWithValue("@Id", keys.ToString());
    //        cmd.Parameters.AddWithValue("@Requester", user_name.ToString());
    //        cmd.Parameters.AddWithValue("@Per", valuechange.ToString());
    //        cmd.Parameters.AddWithValue("@ExchangeRate", seExchangeRate.Text);
    //        cmd.Connection = con;
    //        con.Open();
    //        var getValue = cmd.ExecuteScalar();
    //        con.Close();
    //        myfolio = getValue.ToString();
    //    }
    //    return myfolio;
    //}
    protected void gridData_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        if (args[0] == "delete")
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                //cmd.CommandText = "Update TransTechnical set StatusApp=5 where ID=@ID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spDelQuota";
                cmd.Parameters.AddWithValue("@ID", args[1].ToString());
                cmd.Parameters.AddWithValue("@user", user_name.ToString());
                cmd.Parameters.AddWithValue("@StatusApp", "-1");
                cmd.Parameters.AddWithValue("@tablename", string.Format("{0}", "TransQuotationHeader"));
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        if (args[0] == "Action")
        {
            for (int i = 0; i < g.VisibleRowCount; i++)
            {
                string subject = "GPC:Quotation :" + g.GetRowValues(i, "RequestNo") + "{0}";
                object SubID = g.GetRowValues(i, g.KeyFieldName);
                string status = "8";
                object value = g.GetRowValues(i, "StatusApp");
                if (value.ToString() == "8")
                {
                    status = "9";
                    subject = string.Format(subject, " was approve");
                }
                if (value.ToString() == "9")
                {
                    status = "4";
                    subject = string.Format(subject, " was accept");
                }
                var sel = g.Selection.IsRowSelected(i);
                if (sel)
                {
                    using (SqlConnection con = new SqlConnection(strConn))
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spApproveStep";
                        cmd.Parameters.AddWithValue("@Id", SubID.ToString());
                        cmd.Parameters.AddWithValue("@User", user_name.ToString());
                        cmd.Parameters.AddWithValue("@StatusApp", status);
                        cmd.Parameters.AddWithValue("@table", "TransQuotationHeader");
                        cmd.Parameters.AddWithValue("@remark", "");
                        cmd.Parameters.AddWithValue("@Assign", "");
                        cmd.Parameters.AddWithValue("@reason", "");
                        cmd.Connection = con;
                        con.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                        oAdapter.Fill(dt);
                        foreach (DataRow dr in dt.Rows)
                        {
                            List<string> list = new List<string>();
                            string[] split = dr["MailTo"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in split)
                            {
                                if (s.Trim() != "")
                                    list.Add(cs.GetData(s, "email"));
                            }
                            string MailTo = String.Join(",", list.ToArray()); //string MailCc = "";
                            list = new List<string>();
                            split = dr["MailCc"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in split)
                            {
                                if (s.Trim() != "")
                                    list.Add(cs.GetData(s, "email"));
                            }
                            string MailCc = String.Join(",", list.ToArray());
                            string _link = "Quotation Request No.:" + dr["RequestNo"].ToString();
                            _link += "<br/> Customer Name : " + g.GetRowValues(i, "Customer").ToString();
                            _link += "<br/> Payment Term : " + g.GetRowValues(i, "PaymentTerm").ToString();
                            _link += "<br/> Incoterm : " + g.GetRowValues(i, "Incoterm").ToString();
                            _link += "<br/> Create By : " + cs.GetData(dr["Requester"].ToString(), "fn");
                            _link += "<br/> Comment :";
                            _link += "<br/> The document link --------><a href=" + cs.GetSettingValue() + "/Default.aspx?viewMode=QuotationForm&ID=" + SubID.ToString();
                            _link += " style='color: rgb(0,255,0)'><font color='#0000FF'>click</font></a>";
                            cs.sendemail(MailTo, MailCc, _link, subject);
                        }
                        con.Close();
                    }
                }
            }
        }
        if (args[0] == "Copied")
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spcopyQuotation";
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", args[1]));
                cmd.Parameters.AddWithValue("@Requester", user_name.ToString());
                cmd.Connection = con;
                con.Open();
                var getValue = cmd.ExecuteScalar();
                con.Close();
                string _form = "EditDraft";
                gridData.JSProperties["cpKeyValue"] = (getValue == null) ? string.Empty : string.Format("{0}|{1}", getValue, _form);
            }
        }
        if (args[0] == "post")
        {
            string value = "";
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spInsertQuotaHeader";
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", args[1]));
                cmd.Parameters.AddWithValue("@User", user_name.ToString());
                cmd.Parameters.AddWithValue("@Commission", string.Format("{0}", CmbCommission.Value));
                cmd.Parameters.AddWithValue("@Brand", string.Format("{0}", CmbBrand.Value));
                cmd.Parameters.AddWithValue("@Incoterm", string.Format("{0}", CmbIncoterm.Value));
                cmd.Parameters.AddWithValue("@Route", string.Format("{0}", CmbRoute.Value));
                cmd.Parameters.AddWithValue("@Size", string.Format("{0}", CmbSize.Value));
                cmd.Parameters.AddWithValue("@PaymentTerm", string.Format("{0}", CmbPaymentTerm.Value));
                cmd.Parameters.AddWithValue("@Interest", string.Format("{0}", tbinterest.Text));
                cmd.Parameters.AddWithValue("@Freight", string.Format("{0}", tbFreight.Text));
                cmd.Parameters.AddWithValue("@Insurance", string.Format("{0}", tbInsurance.Text));
                cmd.Parameters.AddWithValue("@Remark", string.Format("{0}", mNotes.Text));
                cmd.Parameters.AddWithValue("@Customer", string.Format("{0}", CmbCustomer.Value));
                cmd.Parameters.AddWithValue("@ShipTo", string.Format("{0}", CmbShipTo.Value));
                cmd.Parameters.AddWithValue("@Currency", string.Format("{0}", CmbCurrency.Value));
                cmd.Parameters.AddWithValue("@StatusApp", string.Format("{0}", 0));
                cmd.Parameters.AddWithValue("@ExchangeRate", string.Format("{0}", seExchangeRate.Value));
                
                cmd.Connection = con;
                con.Open();
                var getValue = cmd.ExecuteScalar();
                con.Close();
                value = getValue.ToString();
                //gridData.JSProperties["cpKeyValue"] = (getValue == null) ? string.Empty : getValue.ToString();
            }
            foreach (DataRow c in tcustomer.Rows)
            {               
                SqlParameter[] param = { new SqlParameter("@Id", string.Format("{0}", c["ID"])),
                    new SqlParameter("@subContainers", string.Format("{0}", c["subContainers"])),
                    new SqlParameter("@Formula", string.Format("{0}", 0)),
                    new SqlParameter("@Code_Adjust", string.Format("{0}", c["Code"])),
                    new SqlParameter("@CostingNo", string.Format("{0}", c["CostingNo"])),
                    new SqlParameter("@ExchangeRate", string.Format("{0}", c["ExchangeRate"])),
                    new SqlParameter("@MinPrice", string.Format("{0}", c["MinPrice"].ToString())),
                    new SqlParameter("@OfferPrice", string.Format("{0}", c["OfferPrice"])), 
                    new SqlParameter("@Mark", string.Format("{0}", c["Mark"])),
                    new SqlParameter("@RequestType", string.Format("{0}", c["RequestType"])),
                    new SqlParameter("@SubID", string.Format("{0}", value)),
                    new SqlParameter("@StatusApp", string.Format("{0}", c["StatusApp"])),
                    new SqlParameter("@Extracost", string.Format("{0}", c["Extracost"])),
                    new SqlParameter("@Overprice", string.Format("{0}", c["Overprice"])),
                    new SqlParameter("@RequestNo", string.Format("{0}", c["RequestNo"])),
                    new SqlParameter("@OfferPrice_Adjust",string.Format("{0}",c["OfferPrice_Adjust"]))};
                cs.GetExecuteNonQuery("spinsertQuotationItems", param);
            }
            postItems(string.Format("{0}", value));
            ApproveStep(value.ToString());
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
    //protected void ASPxCallback1_Callback(object source, CallbackEventArgs e)
    //{
    //    if (string.IsNullOrEmpty(e.Parameter))
    //        return;
    //    var args = e.Parameter.Split('|');
    //    string labelID = e.Parameter;
    //    if (tsubContainers == null) return;
    //    tsubContainers.Rows[Convert.ToInt32(args[1]) - 1]["Name"] = args[2];
    //    tsubContainers.AcceptChanges();
    //    string result = "from server";
    //    e.Result = result;
    //}
    //void namelist(string Folio)
    //{
    //    dsnamelist.SelectParameters.Clear();
    //    dsnamelist.SelectParameters.Add("Id", string.Format("{0}", Folio.ToString()));
    //    dsnamelist.DataBind();
    //    Tablelist = ((DataView)dsnamelist.Select(DataSourceSelectArguments.Empty)).Table;
    //    Tablelist.PrimaryKey = new DataColumn[] { Tablelist.Columns["ID"] };
    //}
    protected void gridData_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            var editor = cs.IsMemberOfRole(string.Format("{0}", 0));
            if (string.Format("{0}", editor) != "0") return;
            var item = e.CreateItem("Clone", "Clone");
            //var item = e.CreateItem("Revised", "Revised");
            item.Image.Url = @"~/Content/Images/Copy.png";
            e.Items.Add(item);

            GridViewContextMenuItem test = e.CreateItem("Export", "ExportToXLS");
            test.Image.Url = @"~/Content/Images/if_sign-out_59204.png";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), test);
            //item = e.CreateItem("Export", "Export");
            //item.BeginGroup = true;
            //item.Image.Url = @"~/Content/Images/if_sign-out_59204.png";
            //e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), item);
            //AddMenuSubItem(item, "PDF", "ExportToPDF", "~/Content/Images/pdf.gif", true);
            //AddMenuSubItem(item, "XLS", "ExportToXLS", "~/Content/Images/excel.gif", true);
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
    //string _calcu(string f,string[] args){
    //    tCalculation = new DataTable(); int NextRowID = 1;
    //    var Results = new DataTable();//spGetHistory
    //    //SqlParameter[] param = { new SqlParameter("@Id", args[1]),
    //    //    new SqlParameter("@Param",string.Format("{0}",args[0]))};
    //    //Results = cs.GetRelatedResources("spGetCalculation", param);
    //    Results = _createtable();
    //    //if (Results.Rows.Count > 0)
    //    foreach (DataRow row in tsubContainers.Rows)
    //    {
    //        double level1 = Convert.ToDouble(row["MinPrice"]); double level2 = 0;
    //        if (CmbIncoterm.Text.Contains("FOB"))
    //            return level1.ToString("F",CultureInfo.InvariantCulture);
    //        int i = Convert.ToInt32(row["Formula"]);
    //        Results.Rows.Add(NextRowID++, 0, "", "FOB Price", string.Format("{0}", row["MinPrice"]), i);
    //        foreach (DataColumn c in tsubContainers.Columns)
    //            if (c.ColumnName.Contains("Commission") || c.ColumnName.Contains("Interest") || c.ColumnName.Contains("Overprice") || c.ColumnName.Contains("Extracost"))
    //            {
    //                level1 += Convert.ToDouble(row[c.ColumnName]);
    //                Results.Rows.Add(NextRowID++, 1, "", c.ColumnName, string.Format("{0:F4}", row[c.ColumnName]), i);
    //            }
    //        int x = 1;// NextRowID;
    //        Results.Rows.Add(NextRowID++, 1, "SumTotal", "CIF Price", level1.ToString("F",CultureInfo.InvariantCulture), i);
    //        //Results.Rows.Add(NextRowID++, x, "", "CIF Price", string.Format("{0}", level1));
    //        double tfreight = 0;
    //        foreach (DataColumn c in tsubContainers.Columns)
    //            if (c.ColumnName.Contains("Freight"))
    //            {
    //                double tcont = 0; double _Freight = 0;
    //                if (!string.IsNullOrEmpty(tbFreight.Text))
    //                    if (double.TryParse(row["subContainers"].ToString(), out tcont) && double.TryParse(tbFreight.Value.ToString(), out _Freight))
    //                    if (tcont != 0) {
    //                        if (double.IsNaN(_Freight) || double.IsInfinity(_Freight))
    //                            _Freight = 0;
    //                        tfreight = _Freight / tcont;
    //                    }
    //                level2 += level1 + tfreight;
    //                Results.Rows.Add(NextRowID++, x, "", c.ColumnName, tfreight.ToString("F",CultureInfo.InvariantCulture), i);
    //            }
    //            else if (c.ColumnName.Contains("Insurance"))
    //            {
    //                double _Insurance,tinsu = 0;
    //                if(double.TryParse(tbInsurance.Text, out _Insurance))
    //                tinsu = ((level1 * 1.1) + tfreight) * Convert.ToDouble(_Insurance/100);
    //                level2 += tinsu;
    //                Results.Rows.Add(NextRowID++, x, "", c.ColumnName, tinsu.ToString("F",CultureInfo.InvariantCulture), i);//=((H70*110%)+H72)*T7
    //            }
    //    }
    //    //int max = Convert.ToInt32(CustomTable.AsEnumerable()
    //    //        .Max(r => r["Formula"]));
    //    //for (int u = 1; u <= max; u++)
    //    //{
    //    //    if (u != i)
    //    //    {
    //    //        int ro = u - 1;
    //    //        var fob_price = tsubContainers.Rows[ro]["MinPrice"].ToString();
    //    //        Results.Rows.Add(NextRowID++, 0, "test", "", 0, u);
    //    //    }
    //    //}
    //    tCalculation = (DataTable)Results;
    //    tCalculation.PrimaryKey = new DataColumn[] { tCalculation.Columns["ID"] };
    //    string totalsum = tCalculation.Compute("Sum(Price)", "Formula in('" + f.ToString() + "')").ToString();
    //    return totalsum;
    //}
    protected void gv_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        DataRow found = tcustomer.Rows.Find(e.Keys[0]);
        //tcustomer.Rows.Remove(found);
        found["Mark"] = "D";
        tcustomer.AcceptChanges();
        e.Cancel = true;
    }
    protected void gv_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.KeyFieldName = "ID";
        g.DataSource = tcustomer;// buildLoadgv;
        g.ForceDataRowType(typeof(DataRow));
    }
    private DataTable buildLoadgv
    {
        get
        {
            if (tcustomer == null)
                return tcustomer;
            var view = tcustomer.DefaultView;
            //CreateGridColumns(view);
            return tcustomer;
        }
    }
    protected void gridData_DataBound(object sender, EventArgs e)
    {
        //    ASPxGridView grid = sender as ASPxGridView;
        //    for (int i = 0; i < grid.Columns.Count; i++)
        //        grid.Columns[i].Width = Unit.Percentage(100 / grid.Columns.Count);
        ASPxGridView g = sender as ASPxGridView;
        int i = Convert.ToInt32(editor["Name"].ToString() == "0" ? 0 : 1);
        g.Columns[i].Visible = true;
    }
void CreateGridColumns(DataView view)
    {
        var table = view.Table;
        gv.Columns.Clear();
        gv.TotalSummary.Clear();
        var values = new[] { "ID", "Mark" };
        foreach (DataColumn c in table.Columns)
        {
            gv.Columns.Remove(gv.Columns[c.ColumnName]);
            GridViewDataColumn gridColumn = gv.Columns[c.ColumnName] as GridViewDataColumn;
            if (c.ColumnName.Contains("SoldTo") || c.ColumnName.Contains("ShipTo"))
            {
                //dsCustomer.SelectParameters.Clear();
                //dsCustomer.SelectParameters.Add("Param", string.Format("{0}", c.ColumnName.Contains("SoldTo")?"AG":"WE"));
                //dsCustomer.SelectParameters.Add("SalesOrg", string.Format("{0}", CmbCompany.Value));
                //dsCustomer.DataBind();
                //var result = ((DataView)dsCustomer.Select(DataSourceSelectArguments.Empty)).Table;

                GridViewDataComboBoxColumn CBCol = new GridViewDataComboBoxColumn();
                CBCol.FieldName = c.ColumnName;
                CBCol.PropertiesComboBox.Columns.Clear();
                CBCol.PropertiesComboBox.Columns.Add(new ListBoxColumn("Code"));
                CBCol.PropertiesComboBox.Columns.Add(new ListBoxColumn("Name"));
                CBCol.PropertiesComboBox.ValueField = "Code";
                CBCol.PropertiesComboBox.TextFormatString = "{0}{1}";
                CBCol.PropertiesComboBox.EnableCallbackMode = true;
                CBCol.PropertiesComboBox.CallbackPageSize = 10;
                CBCol.PropertiesComboBox.DataSource = dsCustomer;
                if (c.ColumnName.Contains("SoldTo")) { 
                CBCol.PropertiesComboBox.ClientSideEvents.Validation = "onValidation";
                    CBCol.PropertiesComboBox.ClientSideEvents.SelectedIndexChanged = "OnChanged";
                    }
                gv.Columns.Add(CBCol);
                gv.Columns[c.ColumnName].Width = Unit.Pixel(180);
            }
            else if (values.Any(c.ColumnName.Contains))
                AddTextColumn(c.ColumnName);
        }
        gv.KeyFieldName = table.Columns["ID"].ColumnName;
        gv.Columns["ID"].Visible = false;
    }
    private void AddTextColumn(string fieldName)
    {
        GridViewDataTextColumn c = new GridViewDataTextColumn();
        c.FieldName = fieldName;
        if (fieldName == "Mark" || fieldName == "SubID")
        {
            //c.HeaderStyle.CssClass = "hide";
            //c.EditCellStyle.CssClass = "hide";
            //c.CellStyle.CssClass = "hide";
            //c.FilterCellStyle.CssClass = "hide";
            //c.FooterCellStyle.CssClass = "hide";
            //c.GroupFooterCellStyle.CssClass = "hide";          
            c.EditFormSettings.Visible = DevExpress.Utils.DefaultBoolean.False;
        }
        gv.Columns.Add(c);
        gv.Columns[fieldName].Width = Unit.Pixel(70);
    }
 
    protected void gv_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (g.IsNewRowEditing) return;
        if (e.Column.FieldName == "RequestNo")
        {
            e.Editor.ReadOnly = true;
        }
        //if (e.Column.FieldName == "ShipTo")
        //{
        //    ASPxComboBox combo = (ASPxComboBox)e.Editor;
        //    combo.DataSource = dsCustomer;// ValuetoReturn.Distinct();
        //    combo.ValueField = "Code";
        //    combo.TextFormatString = "{0}{1}";
        //    combo.Columns.Add(new ListBoxColumn("Code"));
        //    combo.Columns.Add(new ListBoxColumn("Name"));
        //    combo.EnableCallbackMode = true;
        //    //if (e.Column.FieldName == "SoldTo")
        //    //combo.ClientSideEvents.Validation = "onValidation";
        //    combo.ClientSideEvents.SelectedIndexChanged = "OnChanged";
        //    combo.DataBindItems();
        //}
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
        //DataTable dataTable = new DataTable();
        //Session.Remove("dataTable");
        //dataTable.Clear();
        //_dataTable = new DataTable();
        //_dataTable = _createtablecustom("0");
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
            //Worksheet sheet = book.Worksheets.ActiveWorksheet;
            ASPxSpreadsheet spreadsheet = new ASPxSpreadsheet();
            spreadsheet.Document.LoadDocument(FilePath);
            spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;
            int i = 0;
            foreach (Worksheet sheet in spreadsheet.Document.Worksheets)
            {
                i++;
                CellRange range = sheet.GetUsedRange();
                DataTable table = sheet.CreateDataTable(range, false);
                DataTableExporter exporter = sheet.CreateDataTableExporter(range, table, false);
                exporter.CellValueConversionError += exporter_CellValueConversionError;
                exporter.Export();
                //clear data
                foreach (DataRow row in tcustomer.Rows){
                    row["OfferPrice_Adjust"] = "0";
                    row["StatusApp"] = "";
                }
                foreach (DataRow dr in table.Rows)
                {
                    var value = string.Format("{0}", dr["Column8"].ToString().Trim());
                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        DataRow drow = tcustomer.Select("CostingNo='" + value.ToString() +"'").FirstOrDefault();
                        if (drow != null) {
                            var v = float.Parse(dr["Column7"].ToString().Replace("$", ""), CultureInfo.InvariantCulture.NumberFormat);
                            drow["OfferPrice_Adjust"] = string.Format("{0}", v.ToString());
                        }
                    }
                }
            }
        }
    }
    private DataTable GetTableFromExcel(DataTable table)
    {
        DataTable dtDataTable = new DataTable();
        return dtDataTable;
    }
    void book_InvalidFormatException(object sender, SpreadsheetInvalidFormatExceptionEventArgs e)
    {

    }
    void exporter_CellValueConversionError(object sender, CellValueConversionErrorEventArgs e)
    {
        e.Action = DataTableExporterAction.Continue;
        e.DataTableValue = null;
    }

    //protected void gvimport_DataBinding(object sender, EventArgs e)
    //{
    //    ASPxGridView g = sender as ASPxGridView;
    //    g.KeyFieldName = "ID";
    //    g.DataSource = _dt;
    //    g.ForceDataRowType(typeof(DataRow));
    //}
    void gethide(ASPxGridView grid)
    {
        foreach (GridViewDataColumn c in grid.Columns)
            if (myarray.Any(c.FieldName.Contains)){
                c.HeaderStyle.CssClass = "hide";
                c.EditCellStyle.CssClass = "hide";
                c.CellStyle.CssClass = "hide";
                c.FilterCellStyle.CssClass = "hide";
                c.FooterCellStyle.CssClass = "hide";
                c.GroupFooterCellStyle.CssClass = "hide";
            }
    }
    DataTable buildCalculation(string ID)
    {
        SqlParameter[] param = { new SqlParameter("@Id", string.Format("{0}", hfid["hidden_value"].ToString())),
            new SqlParameter("@Param",string.Format("{0}",ID.ToString()))};
        var Results = cs.GetRelatedResources("spGetCalculation", param);
        return Results;
    }
    DataTable _createtable(string ID)
    {
        var Results = new DataTable();
        if (string.IsNullOrEmpty(ID.ToString())) return Results;
        Results = buildCalculation(ID);
        //if (Convert.ToInt32(Results.Rows[0][0]) > 0)
        //    return Results;
        if (string.IsNullOrEmpty(ID.ToString()))
            return Results;
        //DataRow[] selected = Results.Select("Formula = "+ActivePageSymbol);
        foreach (DataColumn dc in Results.Columns){
            double totalCount = 0,minprice = 0;
            if (cs.IsNumeric(dc.ToString()))
            {
            //DataRow rows = tsubContainers.Select("Formula='" + dc.ToString() + "'").FirstOrDefault();
                foreach (DataRow dr in Results.Rows)

                {
                    switch (dr["Name"].ToString())
                    {
                        case "MinPrice":
                            //string keys = Keys.ToString();
                            //minprice = dr[dc].ToString() != "0" ? Convert.ToDouble(dr[dc]) : 
                            //    Convert.ToDouble(GetMinPrice(string.Format("{0}", dc), ID.ToString()));
                            dr[dc] = minprice;
                            totalCount += minprice;
                            break;
						case "OfferPrice":
                            dr[dc] = minprice;
                            break;
                    }
                }
            }
        }
        return Results;
    }
    string GetMinPrice(DataRow row)
    {
        decimal r = 0;
        DataTable MyTable = reload(row);
        if (MyTable != null)
        {
            string[] SubType = Regex.Split("Raw Material & Ingredient;Primary Packaging;Secondary Packaging", ";");
            string[] valueType = Regex.Split("RmIng;PrimaryPkg;SecondaryPkg", ";");
            foreach (DataRow rw in MyTable.Rows)
            {
                decimal _rm, _totalrm;
                for (int i = 0; i <= valueType.Length - 1; i++)
                {
                    _rm = Convert.ToDecimal(rw[valueType[i]]);
                    _totalrm = _rm * (Convert.ToDecimal(GetLoss(SubType[i], row["RequestNo"].ToString())) / 100);
                    rw["Loss"] = Convert.ToDecimal(rw["Loss"]) + _totalrm;
                    rw["FOB"] = Convert.ToDecimal(rw["FOB"]) + _totalrm + Convert.ToDecimal(rw[valueType[i]]);
                }
                decimal totalCount = 0;
                totalCount = Convert.ToDecimal(rw["FOB"]) + Convert.ToDecimal(rw["LOH"]) + Convert.ToDecimal(rw["UpCharge"]);
                object sumObject = string.Format(@"{0}", rw["PerMargin"].ToString()=="" ? "0" : rw["PerMargin"].ToString());

                decimal _MinPrice = (totalCount / ((100 - Convert.ToDecimal(sumObject)) / 100));
                rw["Margin"] = _MinPrice * (Convert.ToDecimal(sumObject) / 100);
                List<decimal> list1 = new List<decimal>() { Convert.ToDecimal(rw["Ing"]),
                    Convert.ToDecimal(rw["RM"]),
                    Convert.ToDecimal(rw["PrimaryPkg"]),
                    Convert.ToDecimal(rw["SecondaryPkg"]), Convert.ToDecimal(rw["LOH"]),
                    Convert.ToDecimal(rw["UpCharge"]),
                    Convert.ToDecimal(rw["Loss"]),
                    Convert.ToDecimal(rw["Margin"])};
                r = list1.Sum();
                if (r > 0)
                    if (rw["Rate"].ToString() != "")
                    {
                        r = r / Convert.ToDecimal(rw["Rate"]);
                    }
            }
        }

        return r.ToString("F",
                  CultureInfo.InvariantCulture);
    }
    string GetLoss(string PackType, string id)
    {
        string str = "";
        DataTable dt = cs.builditems(@"select Packaging,[To] from TransCostingHeader where id = '" + id + "'");
        foreach(DataRow rw in dt.Rows) { 
        str = (string)cs.ReadItems(@"select Loss from maspfloss " +
        "where SubType='" + PackType + "' and ('" + (DateTime)rw["To"] + "' between Validfrom and Validto ) and PackageType in ('" + rw["Packaging"].ToString() + "')");
        }
        return str==""?"0": str;
    }
    //string ExchangeRate(string Currency)
    //{
    //    string cr = string.Format("{0}", 1);
    //    if (Currency == "THB") return cr;
    //    cr = "1";// seExchangeRate.Text;
    //    return cr;
    //}
    //DataRow _found(DataRow found)
    //{
    //    string[] array;
    //    if (found["mark"].ToString() != "R") goto jumptoexit;
    //    if (!string.IsNullOrEmpty(found["PriceOfUnit"].ToString()))
    //        if (string.IsNullOrEmpty(found["Currency"].ToString()))
    //            found["Currency"] = "THB";

    //    string strBaseUnit = found["BaseUnit"].ToString();
    //    if (!string.IsNullOrEmpty(found["Currency"].ToString()))
    //    {
    //        var Currency = string.Format("{0}", 1); //ExchangeRate(found["Currency"].ToString());
    //        if (found["Currency"].ToString() == "THB")
    //            Currency = found["ExchangeRate"].ToString();
    //        if (!string.IsNullOrEmpty(Currency))
    //        {
    //            found["ExchangeRate"] = Currency.ToString(); double da = 0;
    //            if (!string.IsNullOrEmpty(found["PriceOfUnit"].ToString()))
    //                da = double.Parse(found["ExchangeRate"].ToString(), CultureInfo.InvariantCulture) * double.Parse(found["PriceOfUnit"].ToString());
    //            if (!string.IsNullOrEmpty(found["Unit"].ToString()))
    //                switch (found["Unit"].ToString())
    //                {
    //                    case "Ton":
    //                    case "MT":
    //                        da = da / 1000;
    //                        break;
    //                }
    //            found["BaseUnit"] = da.ToString("F",
    //              CultureInfo.InvariantCulture);
    //        }
    //    }
    //    array = Regex.Split("PriceOfCarton", ";");
    //    foreach (string arr in array)
    //    {
    //        int co = 0; string c = "GUnit";
    //        //foreach (string c in myCollection)
    //        //{
    //        co++;
    //        var name = string.Format(arr, co);
    //        double OutVal; double BaseUnit; double Yield; double Packsize;
    //        if (double.TryParse(found[c].ToString(), out OutVal))
    //        {
    //            double total = OutVal / 1000;
    //            //table.Rows[i][name] = table.Rows[i][c].ToString();
    //            //var test = table.Rows[i][c].ToString();
    //            double.TryParse(found["PackSize"].ToString(), out Packsize);
    //            double.TryParse(found["BaseUnit"].ToString(), out BaseUnit);
    //            double.TryParse(found["Yield"].ToString(), out Yield);
    //            double r = (total * BaseUnit * Packsize / (Yield / 100));
    //            if (double.IsNaN(r) || double.IsInfinity(r))
    //                found[name] = "0";
    //            else
    //                found[name] = r.ToString("F",CultureInfo.InvariantCulture);
    //        }
    //        //}
    //    }
    //    jumptoexit:
    //    if (!string.IsNullOrEmpty(found["PriceOfCarton"].ToString())) {double da = 0;
    //        found["ExchangeRate"] = found["ExchangeRate"];
    //        da =  double.Parse(found["PriceOfCarton"].ToString()) / double.Parse(found["ExchangeRate"].ToString(), CultureInfo.InvariantCulture);
    //        found["PriceOfCartonusd"] = da.ToString("F",CultureInfo.InvariantCulture);
    //    }
    //    return found;
    //}
     
    void gvcalculator(ASPxGridView g, string[] args,long id)
    {
        string[] stringArray = { "updated", "subContainers", "sub", "new" };
        int index = Array.IndexOf(stringArray, args[0]);
        if (index > -1)
        {
            if (tcustomer == null) return;
            if (tcustomer.Rows.Count == 0) return;
            foreach (DataRow dr in tcustomer.Rows)
            {
                double commission, Overprice, Extracost, minprice = 0;
                double totalCount = 0, OfferPrice = 0;
                double Freight = 0, _Freight = 0, _Insurance = 0, _Containers = 0, interest = 0;
                //double doubleValue = 0;

                minprice = Convert.ToDouble(dr["MinPrice"]);
                string strOverprice = string.Format("{0}", dr["Overprice"]);
                string strExtracost = string.Format("{0}", dr["Extracost"]);
                string Containers = dr["SubContainers"].ToString();//fix row container
                if (double.TryParse(CmbCommission.Text, out commission))
                    totalCount += Convert.ToDouble(String.Format("{0:#,##0.####}", (minprice * (Convert.ToDouble(commission) / 100))));
                if (double.TryParse(tbinterest.Text, out interest))
                    totalCount += Convert.ToDouble(String.Format("{0:#,##0.####}", (minprice * (Convert.ToDouble(interest) / 100))));
                if (double.TryParse(strOverprice, out Overprice))
                    totalCount += Convert.ToDouble(Overprice.ToString("F", CultureInfo.InvariantCulture));
                if (double.TryParse(strExtracost, out Extracost))
                    totalCount += Convert.ToDouble((minprice * (Convert.ToDouble(Extracost) / 100)));
                if (!string.IsNullOrEmpty(tbFreight.Text) || !string.IsNullOrEmpty(Containers))
                    if (double.TryParse(Containers, out _Containers))
                    {
                        double.TryParse(tbFreight.Text, out Freight);
                        double.TryParse(tbinterest.Text, out interest);
                        if (_Containers > 0)
                            _Freight = (Convert.ToDouble(Freight) / Convert.ToDouble(_Containers));
                        double A = 0, B = 0, interrest = 0, _commission = 0;
                        double.TryParse(CmbCommission.Text, out _commission);
                        interrest = (minprice + _commission + _Freight) * (Convert.ToDouble(interest) / 100);
                        A = minprice + _Freight + interrest;
                        if (double.TryParse(tbInsurance.Text, out _Insurance))
                            //B = Convert.ToDouble(string.Format("{0:#,##0.####}", A * (_Insurance == 0 ? 0 : Convert.ToDouble(1.005/100))));
                            B = Convert.ToDouble(string.Format("{0:#,##0.####}", A * (_Insurance == 0 ? 0 : Convert.ToDouble(0.5 / 100))));
                        OfferPrice = minprice + totalCount + interrest + _Freight + Convert.ToDouble(B);
                    }
                if (!string.IsNullOrEmpty(OfferPrice.ToString()) || !string.IsNullOrEmpty(totalCount.ToString()))
                {
                    dr["OfferPrice"] = OfferPrice > 0 ? OfferPrice.ToString("F", CultureInfo.InvariantCulture) : (minprice + totalCount).ToString("F", CultureInfo.InvariantCulture);
                }
            }
            
        }
        buildExch();
        g.DataBind();
    }
    
    protected void cb_Callback(object source, CallbackEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Parameter))
            return;
        string[] args = e.Parameter.Split('|'); //bool selected = true;
        long id;
        if (!long.TryParse(args[1], out id))
            return;
        //if (args[0] == "Name" || args[0] == "RefSamples" || args[0] == "subContainers")
        //{
        //    DataRow row = tsubContainers.Select(string.Format("Formula={0}", args[1].ToString())).FirstOrDefault();
        //    if (row != null)
        //    {
        //        DataRow dr = tsubContainers.Rows.Find(row[0]);
        //        dr[args[0]] = string.Format("{0}", args[2]);
        //    }
        //}
    }
    //void namelist(string Folio)
    //{
    //    dsnamelist.SelectParameters.Clear();
    //    dsnamelist.SelectParameters.Add("Id", string.Format("{0}", Folio.ToString()));
    //    dsnamelist.DataBind();
    //    //DataTable Tablelist = new DataTable();
    //    Tablelist = ((DataView)dsnamelist.Select(DataSourceSelectArguments.Empty)).Table;
    //    Tablelist.PrimaryKey = new DataColumn[] { Tablelist.Columns["ID"] };
    //    //ViewState.Add("networksListVS", Tablelist);
    //    TimeSpan ts = new TimeSpan(0, 0, 10);
    //    Cache.Insert("Employee", Tablelist, null, Cache.NoAbsoluteExpiration, ts);
    //}
 
    Hashtable copiedValues = null;
    string[] copiedFields = new string[] { "RequestNo", "Customer", "ShipTo", "Currency", "ExchangeRate", "Commission",
    "PaymentTerm","Interest","Incoterm","Route","Size","Freight","Insurance","Overprice","Extracost","SubContainers","Mark","MinPrice","OfferPrice","Remark"};
    protected void gv_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        if (e.ButtonID == "Clone"){
            copiedValues = new Hashtable();
            foreach (string fieldName in copiedFields)
                copiedValues[fieldName] = g.GetRowValues(e.VisibleIndex, fieldName);
            g.AddNewRow();
            object ID= g.GetRowValues(e.VisibleIndex, g.KeyFieldName);
            string[] arr = { e.ButtonID, ID.ToString() };
            g.JSProperties["cpClone"] = String.Join("|", arr);
        }
        if (e.ButtonID == "remove") {
            object keyValue = g.GetRowValues(e.VisibleIndex, g.KeyFieldName);
            DataRow found = tcustomer.Rows.Find(keyValue);
            found["Mark"] = "D";
            tcustomer.AcceptChanges();
            g.DataBind();
            }
    }
    protected void gvcal_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        DataTable table = g.DataSource as DataTable;
        //DataRow dr = table.Rows.Find(e.Keys[0]);
        //string log = "";
        DataRow dr = table.Select(string.Format(@"RowID='{0}' and Name in ('{1}', '{2}', '{3}')", 
            e.Keys[0], "subContainers", "Overprice", "Extracost")).FirstOrDefault();
        if (dr !=null)
        foreach (object item in dr.ItemArray)
        {
            //foreach (DataColumn column in table.Columns)
            //{
            //    if (!column.ColumnName.Contains("RowID") && !column.ColumnName.Contains("Name"))
            //        dr[column.ColumnName] = e.NewValues[column.ColumnName];
            //}
            for (int i = 0; i < e.NewValues.Count; i++)
            {
                string columnName = e.NewValues.Keys.OfType<string>().Skip(i).First();
                    dr[columnName] = e.NewValues[columnName];
            }
        }
        //var values = new[] { "subContainers", "Overprice","Extracost" };
        //DataRow[] result = table.Select(string.Format("Name in ('{0}','{1}','{2}')", "subContainers", "Overprice", "Extracost");
        //foreach(DataRow r in result)
        //foreach (DataColumn column in result.Columns)
        //{
        //if (!column.ColumnName.Contains("RowID") && !column.ColumnName.Contains("Name")){
        //if (values.Any(dr["Name"].ToString().Contains)) { 
        //    DataRow dr = tCalculation.Select(string.Format("Name = '{0}'",column.ColumnName.ToString())).FirstOrDefault();
        //        r[column.ColumnName] = e.NewValues[column.ColumnName];
        //       }
        //    }
        //}
        //g.DataBind();
        g.CancelEdit();
        e.Cancel = true; 
    }
    //protected void CmbCustomer_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    ASPxComboBox comb = sender as ASPxComboBox;
    //    if (string.IsNullOrEmpty(e.Parameter)) return;
    //    string[] param = e.Parameter.Split('|');
    //    //if (string.IsNullOrEmpty(comb.Items[0].Value.ToString()))
    //    bCustomer("AG", Convert.ToInt32(param[1]));
    //    comb.DataBind();
    //}
    //protected void CmbShipTo_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    ASPxComboBox comb = sender as ASPxComboBox;
    //    if (string.IsNullOrEmpty(e.Parameter)) return;
    //    string[] param = e.Parameter.Split('|');
    //    bCustomer("WE", Convert.ToInt32(param[1]));
    //    comb.DataBind();
    //}
    //void bCustomer(string Partner,int Id)
    //{
    //    dsCustomer.SelectParameters.Clear();
    //    dsCustomer.SelectParameters.Add("Id", string.Format("{0}", Id.ToString()));
    //    dsCustomer.SelectParameters.Add("Param", string.Format("{0}", Partner));
    //}

    //protected void detailGrid_DataBinding(object sender, EventArgs e)
    //{
    //    ASPxGridView g = (ASPxGridView)sender;
    //    var Results = _createtable();
    //    gvcal.Columns.Clear();
    //    GridViewBandColumn bandColumn = new GridViewBandColumn();
    //    bandColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
    //    var values = new[] { "RowID", "Name" };
    //    foreach (DataColumn c in Results.Columns)
    //    {
    //        var str = c.ColumnName;
    //        if (values.Any(c.ColumnName.Contains))
    //        {
    //            GridViewDataTextColumn tc = new GridViewDataTextColumn();
    //            tc.FieldName = c.ColumnName;
    //            tc.Width = Unit.Pixel(270);
    //            gvcal.Columns.Add(tc);
    //        }
    //        else
    //        {
    //            GridViewDataSpinEditColumn se = new GridViewDataSpinEditColumn();
    //            se.FieldName = c.ColumnName;
    //            se.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
    //            bandColumn.Columns.Add(se);
    //        }
    //    }
    //    bandColumn.Caption = "Formula";
    //    gvcal.Columns.Add(bandColumn);
    //    if (Results.Rows.Count > 0)
    //    {
    //        gvcal.KeyFieldName = Results.Columns[0].ColumnName;
    //        gvcal.Columns[0].Visible = false;
    //    }
    //    g.DataSource = Results;
    //}

    //protected void gv_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    //{
    //    if (e.MenuType == GridViewContextMenuType.Rows)
    //    {
    //        e.Items.Clear();
    //        var editor = cs.IsMemberOfRole(string.Format("{0}", 0));
    //        if (string.Format("{0}", editor) != "0") return;
    //        var item = e.CreateItem("ExportToXLS", "ExportToXLS");
    //        //var item = e.CreateItem("Revised", "Revised");
    //        item.Image.Url = @"~/Content/Images/if_sign-out_59204.png";
    //        e.Items.Add(item);

    //        //item = e.CreateItem("Export", "Export");
    //        //item.BeginGroup = true;
    //        //item.Image.Url = @"~/Content/Images/if_sign-out_59204.png";
    //        //e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), item);
    //        //AddMenuSubItem(item, "PDF", "ExportToPDF", "~/Content/Images/pdf.gif", true);
    //        //AddMenuSubItem(item, "XLS", "ExportToXLS", "~/Content/Images/excel.gif", true);
    //    }
    //}

    protected void gv_ContextMenuItemClick(object sender, ASPxGridViewContextMenuItemClickEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        switch (e.Item.Name)
        {
            case "ExportToXLS":
                //exportto();
                break;
        }
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
        ws.Cells["G1"].Value = string.Format("FOB Bangkok/ctn({0})", CmbCurrency.Text);
        //DataTable table = gv.DataSource as DataTable;
        if (tcustomer != null)
        {
            int i = 2;
            foreach (DataRow r in tcustomer.Rows)
            {
                ws.Cells["A" + i].Value = string.Format("{0}", r["Name"]);
                ws.Cells["B" + i].Value = string.Format("{0}", r["Code"]);
                ws.Cells["E" + i].Value = string.Format("{0}", r["RefSamples"]);
                //ws.Cells["G" + i].Value = string.Format("{0}", CmbIncoterm.Text.ToString() == "FOB" ? r["MinPriceExch"] : r["OfferPriceExch"]);
                ws.Cells["F" + i].Value = string.Format("{0}", r["CostNo"]);
                ws.Cells["C" + i].Value = string.Format("{0}", CmbCustomer.Value);
                ws.Cells["D" + i].Value = string.Format("{0}", CmbShipTo.Value);
                ws.Cells["H" + i].Value = string.Format("{0}", r["CostingNo"]);
                
                i++;
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
    }
    // protected void btn_Click(object sender, EventArgs e)
    //{
    //    exportto();
    //}
    protected void CmbCustomer_Callback(object sender, CallbackEventArgsBase e)
    {
        ASPxComboBox cb = sender as ASPxComboBox;
        cb.DataBind();
    }

    //protected void CmbPaymentTerm_Callback(object sender, CallbackEventArgsBase e)
    //{
    //    ASPxComboBox comb = sender as ASPxComboBox;
    //    if (string.IsNullOrEmpty(e.Parameter)) return;
    //    var args = e.Parameter.Split('|');
    //    SqlParameter[] param = { new SqlParameter("@Id", args[0].ToString()) };
    //    DataTable result = cs.GetRelatedResources("spGetCustomer", param);
    //    if (result.Rows.Count > 0)
    //        foreach (DataRow dr in result.Rows)
    //        {
    //            var a = comb.Items.FindByValue(dr["TermOfPayment"].ToString());
    //            if (!string.IsNullOrEmpty(a.ToString()))
    //                comb.SelectedItem.Value = a; 
    //        }

    //}

    protected void gv_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            if (string.Format("{0}", cs.IsMemberOfRole(string.Format("{0}", 0))) != "0")
                return;
            var item = e.CreateItem("Clear", "Clear");
            item.Image.Url = @"~/Content/Images/Delete.gif";
            e.Items.Add(item);

            GridViewContextMenuItem test = e.CreateItem("Export", "ExportToXLS");
            test.Image.Url = @"~/Content/Images/if_sign-out_59204.png";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), test);
        }
    }

    protected void gv_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        var result = new Dictionary<string, string>();
        var args = e.Parameters.Split('|');
        if (args[0] == "cost")
        {
            SqlParameter[] param = { new SqlParameter("@ID", string.Format("{0}", args[1])) };
            DataTable dt = cs.GetRelatedResources("spGetMapCostingMat", param);
            foreach (DataRow dr in dt.Rows)
            {
                result["Code"] = string.Format("{0}", dr["Material"]);
                result["Name"] = string.Format("{0}", dr["Description"].ToString());
                e.Result = result;
            }
        }
        long id;
        if (!long.TryParse(args[1], out id))
            return;
        if (args[0] == "reload" || args[0]== "Clone")
        {
            //DataTable table = g.DataSource as DataTable;
            DataRow dr = tcustomer.Rows.Find(id);
            if (dr != null) { 
                //result["PaymentTerm"] = dr["PaymentTerm"].ToString();
                foreach (DataColumn column in tcustomer.Columns){
                var n = column.ColumnName;
                result[n] = string.Format("{0}", dr[n]);
                }
                e.Result = result;
            }
        }
        if (args[0] == "tot" || args[0] == "sub")
        {
            if (CmbRoute.Value!=null || CmbSize.Value!=null)
            {
                SqlParameter[] param = {new SqlParameter("@Route",string.Format("{0}",CmbRoute.Value)),
                    new SqlParameter("@Customer",string.Format("{0}",CmbCustomer.Value)),
                    new SqlParameter("@ShipTo",string.Format("{0}",CmbShipTo.Value)),
                    new SqlParameter("@Container",string.Format("{0}",CmbSize.Value))};
                    DataTable dt = cs.GetRelatedResources("spGetselectFreight", param);

                if(dt.Rows.Count==0)
                {
                    result["Insurance"] = "1.0011";
                    result["Freight"] = "0";
                    e.Result = result;
                }
                else    
                foreach (DataRow dr in dt.Rows)
                {
                    double _Insurance = 1;
                    if (CmbPaymentTerm.Value != null)
                    {
                        if (CmbPaymentTerm.Value.ToString().Substring(1, 1) == "I")
                            _Insurance = 1.0011;
                    }
                    result["Insurance"] = "1.0011";
                    result["Freight"] = string.Format("{0}", Convert.ToDouble(dr["MKTCost"]) * _Insurance);
                    e.Result = result;
                }
            }
        }
        
        //if (args[0] == "cost")
        //{
        //    result["Code"] = string.Format("{0}", args[1]);
        //    result["Name"] = GetDescrip(args[1].ToString());
        //    e.Result = result;
        //}
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
    protected void gv_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (e.NewValues["RequestNo"] == null || e.NewValues["Customer"] == null){
            e.Errors[g.Columns["RequestNo"]] = "Value can't be null.";
        }
        if (string.IsNullOrEmpty(e.RowError) && e.Errors.Count > 0) e.RowError = "Please, correct all errors.";
    }

    protected void btexport_Click(object sender, EventArgs e)
    {
        exportto();
    }
 

    protected void gv_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
    {
        if (copiedValues == null) return;
        foreach (string fieldName in copiedFields)
            e.NewValues[fieldName] = copiedValues[fieldName];
    }
    protected void gv_AfterPerformCallback(object sender, ASPxGridViewAfterPerformCallbackEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        if (e.CallbackName.Equals("CANCELEDIT"))
        {
            g.JSProperties["cpUpdatedMessage"] = "Canceledit";
            // custom logic goes here  
        }
    }

    protected void PreviewPanel_Callback(object sender, CallbackEventArgsBase e)
    {
        //var text = string.Format("<div align='center'><h1>Access denied</h1><br/>You are not authorized to access this page.</div>", e.Parameter);
        //PreviewPanel.Controls.Add(new LiteralControl(text));
    }

    protected void gv_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        //double totalCount = 0, totalOfferPrice=0;
        var values = new[] { "ID", "RowID", "RequestNo", "Name", "PercentDiff", "CostNo", "Code", "CostingNo", "MinPrice", "OfferPrice", "Mark", "Diff" };
        foreach (var args in e.UpdateValues)
        {
            DataRow dr = tcustomer.Rows.Find(args.Keys["ID"]);
            //double minprice = Convert.ToDouble(dr["MinPrice"]);
            if (dr != null)
                foreach (DataColumn column in tcustomer.Columns)
                {
                    //        if(column.ColumnName == "Overprice")
                    //        {
                    //            double Overprice = 0;
                    //            dr[column.ColumnName] = args.NewValues[column.ColumnName];
                    //            if (double.TryParse(args.NewValues[column.ColumnName].ToString(), out Overprice))
                    //                totalCount += Convert.ToDouble(Overprice.ToString("F", CultureInfo.InvariantCulture));
                    //        }
                    //        else if (column.ColumnName == "Extracost")
                    //        {
                    //            double Extracost = 0;
                    //            if (double.TryParse(args.NewValues[column.ColumnName].ToString(), out Extracost))
                    //            totalCount += Convert.ToDouble((minprice * (Convert.ToDouble(Extracost) / 100)));
                    //        }
                    //        else if (column.ColumnName== "SubContainers")
                    //        {
                    //            dr[column.ColumnName] = args.NewValues[column.ColumnName];
                    //            double Containers = 0;
                    //            if (!string.IsNullOrEmpty(tbFreight.Text) || !string.IsNullOrEmpty(args.NewValues[column.ColumnName].ToString()))
                    //                if (double.TryParse(args.NewValues[column.ColumnName].ToString(), out Containers))
                    //                {
                    //                    double Freight = 0, interest = 0, totalFreight=0;
                    //                    double.TryParse(tbFreight.Text, out Freight);
                    //                    double.TryParse(tbinterest.Text, out interest);
                    //                    if (Containers > 0)
                    //                        totalFreight = (Convert.ToDouble(Freight) / Convert.ToDouble(Containers));
                    //                    double A = 0, B = 0, interrest = 0, totalcommission = 0, Insurance=0;
                    //                    double.TryParse(CmbCommission.Value.ToString(), out totalcommission);
                    //                    interrest = (minprice + totalcommission + totalFreight) * (Convert.ToDouble(interest) / 100);
                    //                    A = minprice + totalFreight + interrest;
                    //                    if (double.TryParse(tbInsurance.Text, out Insurance))
                    //                        B = Convert.ToDouble(string.Format("{0:#,##0.##}", A * (Insurance == 0 ? 0 : Convert.ToDouble(0.5 / 100))));
                    //                    totalOfferPrice = minprice + totalCount + interrest + totalFreight + Convert.ToDouble(B);
                    //                }

                    //        }
                    //        else 
                    if (!values.Any(column.ColumnName.Equals))
                        {
                        if (args.NewValues[column.ColumnName] != null)
                            dr[column.ColumnName] = args.NewValues[column.ColumnName];
                        }

                    }
            //dr["OfferPrice"] = totalOfferPrice.ToString("F", CultureInfo.InvariantCulture);
        }
        String[] array = { "updated" };
        gvcalculator(g, array, 0);
        e.Handled = true;
    }
    protected void gv_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
    {
        var values = new[] { "OfferPrice" };
        if (e.DataColumn.FieldName == "Mark" || e.DataColumn.FieldName == "StatusApp")
        {
            var g = sender as ASPxGridView;
            DataRow row = g.GetDataRow(e.VisibleIndex);
            int index = e.VisibleIndex;
            //e.Cell.ForeColor = Color.Black;
            if (g.VisibleRowCount != 0 && row != null) { 
                if (g.GetRowValues(index, "Mark").ToString() == "D")
                    e.Cell.BackColor = Color.Coral;
                else if (g.GetRowValues(e.VisibleIndex, "Mark").ToString() == "X")
                    e.Cell.BackColor = Color.LightGreen;

            if (g.GetRowValues(e.VisibleIndex, "StatusApp").ToString() == "A")
                e.Cell.BackColor = Color.Red;
            }
        }
    }
    public decimal FindDifference(decimal nr1, decimal nr2)
    {
        return Math.Abs(nr1 - nr2);
    }
    protected void gv_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        DataTable table = (DataTable)g.DataSource;
        if (table != null)
        {
            foreach (DataRow drow in table.Rows)
            {
                if (drow["OfferPrice_Adjust"].ToString() != "0")
                {
                    //if (Convert.ToDouble(drow["OfferPrice_Adjust"]) < Convert.ToDouble(drow["OfferPrice"]))
                    //{
                        double diff = (Convert.ToDouble(drow["OfferPriceExch"]) - Convert.ToDouble(drow["OfferPrice_Adjust"]));
                        if (diff > 0.04)
                        {
                            drow["StatusApp"] = string.Format("{0}", 'A'); // if eq 'A' approve 
                        }
                            drow["Diff"] = Convert.ToDecimal(drow["OfferPrice_Adjust"].ToString()) - Convert.ToDecimal(drow["OfferPriceExch"].ToString());
                            drow["PercentDiff"] = Convert.ToDecimal(Convert.ToDecimal(drow["Diff"].ToString()) / Convert.ToDecimal(drow["OfferPriceExch"].ToString()) * 100).ToString("F");
                    //}
                    //else
                    //{
                    //    string data = "StatusApp,Diff,PercentDiff";
                    //    string[] words = data.Split(',');
                    //    foreach (string word in words)
                    //    {
                    //        drow[word] = "0";
                    //    }
                    //}
                }
            }
        }
    }

    protected void gridData_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
    {
        //var edit = cs.IsMemberOfRole(string.Format("{0}", 0)); //approv["Name"].ToString();
        // disable the selction checkbox
        ASPxGridView g = sender as ASPxGridView;
        var value = g.GetRowValues(e.VisibleIndex, "StatusApp");
        if (value != null)
        {
            if (e.ButtonType == ColumnCommandButtonType.SelectCheckbox)
                e.Enabled = value.Equals("0")? false:true;
        }
    }
}
