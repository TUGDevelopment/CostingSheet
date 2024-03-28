using DevExpress.Pdf;
using DevExpress.Spreadsheet;
//using OfficeOpenXml;
//using OfficeOpenXml.Drawing;
//using DevExpress.XtraPrinting;
//using DevExpress.XtraReports.Web;
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
using System.Net;
using System.Text.RegularExpressions;
//using System.Text.RegularExpressions;
using System.Web;
//using System.Web.UI;
//using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
//using GemBox.Spreadsheet;
using System.Text;

public partial class MailMerge : System.Web.UI.Page
{
    Worksheet worksheet;
    MyDataModule cs = new MyDataModule();
    myClass myClass = new myClass();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    public string CurUserName = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    private DataTable dataTable; string p; string costingsheet; double PackSize = 0; string strCompany = "";
    List<string> mylist = new List<string>();
    string RequireDate
    {
        get { return Session["RequireDate"] == null ? null : Session["RequireDate"].ToString(); }
        set { Session["RequireDate"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!IsPostBack)
        //{}
		int id;
        if (!int.TryParse(Request.QueryString["Id"], out id))
            return;
        decimal calc_sec = 0, calc_loh=0, calc_margin=0;
        hfusername["user_name"] = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
        switch (Request.QueryString["view"].ToString())
        {
            case "0":
                DataTable dt = _selectData(id);
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["usertype"].ToString() != "0")
                    {
                        Spreadsheet.Visible = true;
                        Spreadsheet.Document.LoadDocument(Server.MapPath("~/App_Data/Documents/F9MKXX03_New_Request_5_09.09.2019V3.xlsx"));
                        Worksheet ws = Spreadsheet.Document.Worksheets["NEW SAMPLE REQUEST FORM"];
                        ws.Cells["E4"].Value = dr["Company"].ToString();
                        ws.Cells["AC4"].Value = dr["RequestNo"].ToString();
                        //ws.Cells["AC6"].Value = dr["Validfrom"].ToString();
                        //ws.Cells["AC8"].Value = dr["Validto"].ToString();
                        ws.Cells["AC6"].Value = dr["CreateOn"].ToString();
                        ws.Cells["AC8"].Value = dr["SampleDate"].ToString();
                        ws.Cells["AC10"].Value = cs.GetData(dr["Requester"].ToString(), "fn");
                        ws.Cells["I4"].Value = dr["PetFoodType"].ToString();
                        ws.Cells["E6"].Value = dr["ProductCat"].ToString();
                        ws.Cells["F14"].Value = dr["Customer"].ToString();
                        ws.Cells["F16"].Value = dr["Destination"].ToString();
                        ws.Cells["F18"].Value = dr["Country"].ToString();
                        ws.Cells["D22"].Value = dr["Packaging"].ToString().ToUpper();
                        ws.Cells["I22"].Value = dr["PackType"].ToString().ToUpper();
                        ws.Cells["D24"].Value = dr["Material"].ToString().ToUpper();
                        ws.Cells["I24"].Value = dr["PackLacquer"].ToString().ToUpper();
                        ws.Cells["D26"].Value = dr["PackLid"].ToString().ToUpper();
                        ws.Cells["I26"].Value = dr["PackDesign"].ToString().ToUpper();
                        //ws.Pictures.AddPicture(GetImageCheckbox(dr["Requestfor"].ToString(), "New Sample"), ws.Cells["E8"]);
                        string values = dr["PhysicalSample"].ToString();
                        string[] array = values.Split('|');
                        //result["Physical"] = array[0].ToString();
                        ws.Cells["T14"].Value = array[1].ToString();
                        //result["Sample"] = array[2].ToString();
                        ws.Cells["T16"].Value = array[3].ToString();
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["Requestfor"].ToString(), "New Sample"), ws.Cells["D8"]);
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["Requestfor"].ToString(), "New Sample – TU Proposal"), ws.Cells["P8"]);
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["Requestfor"].ToString(), "New Sample - Customer Inquiry"), ws.Cells["P10"]);
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["Requestfor"].ToString(), "New sample for non-commercial"), 660, 310);
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["Requestfor"].ToString(), "Adjust Current Item"), ws.Cells["D10"]);
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["Requestfor"].ToString(), "Product Specification"), 660, 390);
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["Requestfor"].ToString(), "Costing"), 1060, 390);
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["CompliedWith"].ToString(), "Buyer's Specification"), 1365, 485);
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["CompliedWith"].ToString(), "Company's Specification"), 1735, 485);

                        ws.Cells["I28"].Value = dr["PackColor"].ToString().ToUpper();
                        ws.Cells["D28"].Value = dr["Drainweight"].ToString();
                        ws.Cells["J38"].Value = dr["PackSize"].ToString();

                        ws.Pictures.AddPicture(GetImageCheckbox(dr["CustomerRequest"].ToString(), "0"), 1080, 965);//FDA
                        //ws.Pictures.AddPicture(GetImageCheckbox(dr["CustomerRequest"].ToString(), "1"), ws.Cells["V12"]);//SID No. (For USA)
                        //ws.Pictures.AddPicture(GetImageCheckbox(dr["CustomerRequest"].ToString(), "2"), ws.Cells["V12"]);
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["CustomerRequest"].ToString(), "3"), 1230, 965);//Kosher
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["CustomerRequest"].ToString(), "4"), 1510, 965);//Halal
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["CustomerRequest"].ToString(), "6"), 1730, 965);//Halal w/ Movement
                        string PrdOther = dr["PrdRequirement"].ToString();
                        string[] prdarray = PrdOther.Split('|');
                        ws.Pictures.AddPicture(GetImageCheckbox(prdarray[0].ToString(), "9"), 1085, 1625); //Other Requirements
                        ws.Cells["R36"].Value = prdarray[1];
                        ws.Pictures.AddPicture(GetImageCheckbox(prdarray[0].ToString(), "0"), 1080, 1105);//MSC
                        ws.Pictures.AddPicture(GetImageCheckbox(prdarray[0].ToString(), "10"), 1510, 1105);//RSPO

                        ws.Pictures.AddPicture(GetImageCheckbox(prdarray[0].ToString(), "2"), 1230, 1105);//Pole & Line                        
                        ws.Pictures.AddPicture(GetImageCheckbox(prdarray[0].ToString(), "11"), ws.Cells["Q30"]);//Vegan
                        string[] arrInner = dr["Inner"].ToString().Split('|');
                        if (arrInner != null || arrInner.Length > 1)
                        {
                            ws.Pictures.AddPicture(GetImageCheckbox(arrInner[0].ToString(), "0"), ws.Cells["C38"]);//Inner Box
                            ws.Pictures.AddPicture(GetImageCheckbox(arrInner[0].ToString(), "1"), ws.Cells["C39"]);//Sleeve Box
                            ws.Pictures.AddPicture(GetImageCheckbox(arrInner[0].ToString(), "2"), ws.Cells["C40"]);//Shrink
                            ws.Pictures.AddPicture(GetImageCheckbox(arrInner[0].ToString(), "3"), ws.Cells["C41"]);//Other
                            ws.Cells["D41"].Value = arrInner[1].ToString();
                        }
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["Outer"].ToString(), "0"), ws.Cells["G38"]);//STD Carton
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["Outer"].ToString(), "1"), ws.Cells["G39"]);//Perforated Carton
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["Outer"].ToString(), "2"), ws.Cells["G40"]);//Tray
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["Outer"].ToString(), "3"), ws.Cells["G41"]);//Tray&Hood/U-Shape
                        ws.Pictures.AddPicture(GetImageCheckbox(";", "X"), 2840, 70);//Require document for cert
                        ws.Pictures.AddPicture(GetImageCheckbox(";", "X"), 2385, 1315);//Approved
                        ws.Pictures.AddPicture(GetImageCheckbox(";", "X"), 2385, 1395);//Disapproved 

                        ws.Pictures.AddPicture(GetImageCheckbox(";", "X"), 2210, 565);//Accept to make sample
                        ws.Pictures.AddPicture(GetImageCheckbox(";", "X"), 2210, 635);// Not accept and return to Markeing with 5 days
                        values = dr["Ingredient"].ToString();
                        array = values.Split('|');
                        ws.Cells["W26"].Value = array[1];
                        ws.Pictures.AddPicture(GetImageCheckbox(array[0].ToString(), "3"), ws.Cells["Q38"]);//Dairy Free
                        ws.Pictures.AddPicture(GetImageCheckbox(array[0].ToString(), "4"), ws.Cells["T38"]);//Gluten Free
                        ws.Pictures.AddPicture(GetImageCheckbox(array[0].ToString(), "5"), 1680, 1105);//Other Certifications

                        ws.Pictures.AddPicture(GetImageCheckbox(array[0].ToString(), "0"), 1085, 1315);//Natural
                        ws.Pictures.AddPicture(GetImageCheckbox(array[0].ToString(), "1"), 1085, 1395);//Non_GMO
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["CustomerRequest"].ToString(), "4"), 1085, 1475);//SID No
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["CustomerRequest"].ToString(), "7"), 1085, 1555);//Specific Supplier

                        ws.Pictures.AddPicture(GetImageCheckbox(dr["CustomerRequest"].ToString(), "3"), ws.Cells["Q32"]);//Valid IT
                        ws.Pictures.AddPicture(GetImageCheckbox(dr["CustomerRequest"].ToString(), "1"), ws.Cells["Q34"]);//Approved Plant no. (TH/EST/FCE for USA)

                        ws.Pictures.AddPicture(GetImageCheckbox(";", "X"), ws.Cells["T30"]);
                        ws.Pictures.AddPicture(GetImageCheckbox(array[0].ToString(), "2"), ws.Cells["T32"]);//Non-China Origin

                        DataTable listdt = cs.builditems(string.Format(@"select ROW_NUMBER() OVER(ORDER BY Id) AS RowID ,
                                *,''Mark from TransProductList where RequestNo={0}", id));
                        for (int index = 0; index < listdt.Rows.Count; index++)
                        {
                            string member = "", col = Convert.ToDouble(46 + index).ToString();
                            member = listdt.Rows[index]["Name"].ToString();
                            ws.Cells["D" + col].Value = member;
                            ws.Cells["X" + col].Value = listdt.Rows[index]["NetWeight"].ToString();
                            ws.Cells["AA" + col].Value = listdt.Rows[index]["DW"].ToString();
                            ws.Cells["AB" + col].Value = listdt.Rows[index]["DWType"].ToString();
                            ws.Cells["AC" + col].Value = listdt.Rows[index]["FixedFillWeight"].ToString();
                            ws.Cells["AC" + col].Value = listdt.Rows[index]["FixedFillWeight"].ToString();
                            ws.Cells["AD" + col].Value = listdt.Rows[index]["PW"].ToString();
                            ws.Cells["AE" + col].Value = listdt.Rows[index]["TargetPrice"].ToString();
                            ws.Cells["AF" + col].Value = listdt.Rows[index]["SaltContent"].ToString();
                        }
                        string[] narray = string.Format("{0}", dr["Notes"]).Split('\n');
                        int inputrow = 59;
                        foreach (string value in narray)
                        {
                            ws.Cells["C" + inputrow].Value = string.Format("{0}", value);
                            inputrow++;
                        }
                    }
                    else
                    {
                        string fileName = Server.MapPath("~/App_Data/Documents/F9MKXX09_0_05062015_(3).pdf");
                        FormFillManual(fileName);
                    }
                }
                break;
            case "1":
            case "2":
            case "3":
                string Id = (Request.QueryString["Id"] == null) ? "4" : Request.QueryString["Id"].ToString();
                string strTec = ",(select bb.CustomPrice from TransTechnical bb where bb.ID=aa.RequestNo) as CustomPrice";
                DataTable comp = cs.builditems(@"select aa.* " + strTec + " from TransCostingHeader aa where aa.ID='" + Id + "'");
                DataRow _dr = comp.Select().FirstOrDefault();
                Spreadsheet.Visible = true;
                if (string.Format("{0}", _dr["Company"]) == "101")
                {
                    Spreadsheet.Document.LoadDocument(Server.MapPath(@"~/App_Data/Documents/Cost sheet print out(07.06.2021).xlsx"));
                    Spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;
                    worksheet = Spreadsheet.Document.Worksheets[0];
                    IWorkbook workbook = Spreadsheet.Document;
                    p = (string)_dr["Packaging"];
                    build_detail(Id, _dr);
                    if (workbook.Worksheets.Count > 1)
                        workbook.Worksheets.RemoveAt(0);
                }
                else
                {
                    //Spreadsheet.Document.Worksheets.ActiveWorksheet.Import(GetMailMergeDataSource(), true, 0, 0);
                    Spreadsheet.Document.LoadDocument(Server.MapPath("~/App_Data/Documents/Example.xlsx"));
                    //Spreadsheet.Open(Server.MapPath("~/App_Data/Documents/Example.xlsx"));
                    Spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;
                    worksheet = Spreadsheet.Document.Worksheets[0];
                    IWorkbook workbook = Spreadsheet.Document;
                    if (workbook.Worksheets.Count > 1)
                    {
                        int max = workbook.Worksheets.Count; int sheet = max;
                        for (int i = 1; i <= max; i++)
                        {
                            sheet = sheet - 1;
                            workbook.Worksheets.RemoveAt(sheet);
                        }
                    }

                    if (Request.QueryString["Type"] == "0")
                    {
                        //PrepareHeaderCells(Id);
                        body(Id, _dr);
                    }
                    else
                        bodyAll(Id, Request.QueryString["Type"]);
                    if (workbook.Worksheets.Count > 1)
                        workbook.Worksheets.RemoveAt(0);
                    if (string.Format("{0}", _dr["VarietyPack"]) != "")
                    {
                        var _v = string.Format("{0}", _dr["VarietyPack"]);
                        workbook.Worksheets.Add(_v);
                        Worksheet _ws = Spreadsheet.Document.Worksheets[_v];
                        _ws.Cells["A1"].ColumnWidth = 800;
                        _ws.Cells["B1"].ColumnWidth = 800;
                        _ws.Cells["C1"].ColumnWidth = 600;
                        _ws.Range["D1:D40"].Style.NumberFormat = "##0.00";
                        _ws.Cells["A1"].Value = string.Format("{0}", _dr["CanSize"]);
                        _ws.Cells["A1"].Font.Name = "Calibri Light";
                        _ws.Cells["A1"].Font.Bold = true;
                        _ws.Cells["A1"].Borders.SetOutsideBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
                        _ws.Cells["A3"].Value = string.Format("Packing, {0}/carton", _dr["Packaging"]);
                        _ws.Cells["A4"].Value = "THB: USD FX Rate";
                        _ws.Cells["D6"].Value = "'(USD/carton)";
                        _ws.Cells["C6"].Value = "'Unit";
                        _ws.Range["D6:E6"].Merge();
                        _ws.Cells["D6"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                        _ws.Cells["B3"].Value = string.Format("{0}", _dr["PackSize"]);
                        _ws.Cells["B4"].Value = string.Format("{0}", _dr["ExchangeRate"]);
                        _ws.Cells["A7"].Value = string.Format("1) RM Components({0}{1}/Pack)", _dr["PackSize"], _dr["Packaging"]);
                        int c = 7;
                        int totalrm = c;

                        DataTable dtproduct = cs.GetProductFormula(Id);
                        string strSQl = @"select CONCAT('[',SAPMaterial,']',Description) as name,* from TranshCosting where Requestno='"
                        + Id.ToString() + "'";
                        DataTable dth = cs.builditems(strSQl);
                        if (dth.Rows.Count > 0)
                        {
                            for (int o = 1; o <= dtproduct.Rows.Count; o++)
                            {
                                c++;
                                DataRow _drp = dtproduct.Rows[o - 1];
                                _ws.Cells["B" + c].Value = _drp["Name"].ToString();
                                _ws.Cells["C" + c].Formula = string.Format("= {0}", _drp["PackSize"]); //
                                _ws.Cells["D" + c].Formula = string.Format("= {0}", cs.GetSinglePrice(_drp, _dr));
                                _ws.Cells["E" + c].Value = "THB";
                            }

                            c++;
                            _ws.Cells["C" + c].Value = "Sub-total RM THB";
                            _ws.Cells["D" + c].Formula = string.Format("=SUM(D{0}:D{1})", totalrm, c - 1);
                            _ws.Cells["D" + c].NumberFormat = "#,###.##";
                            _ws.Cells["E" + c].Value = "THB";
                            totalrm = c;
                            c++;
                            _ws.Cells["A" + c].Value = "2) Packaging";
                            DataRow[] _dth = dth.Select("component='Secondary Packaging'");
                            int total = c;
                            foreach (var _d in _dth)
                            {
                                c++;
                                _ws.Cells["B" + c].Value = _d["Name"].ToString();
                                _ws.Cells["C" + c].Formula = string.Format("= {0}", _d["Quantity"]);
                                _ws.Cells["D" + c].Formula = string.Format("= {0}", _d["Amount"]);

                                _ws.Cells["E" + c].Value = "THB";
                            }

                            c++;
                            DataRow[] dtloss = dth.Select("component = 'Loss'");
                            foreach (var _r in dtloss)
                            {
                                _ws.Range["B" + c + ":D" + c].FillColor = System.Drawing.Color.Beige;
                                _ws.Cells["B" + c].Value = "2.4)Packaging Loss";
                                _ws.Cells["C" + c].Value = string.Format("{0}%", _r["Per"]);
                                _ws.Cells["D" + c].Formula = string.Format("=SUM(D{0}:D{1})*$C${2}", total, c - 1, c);
                                _ws.Cells["D" + c].NumberFormat = "0.##";
                                _ws.Cells["E" + c].Value = "THB";
                            }

                            c++;
                            _ws.Cells["C" + c].Value = "Sub-total THB";
                            _ws.Cells["D" + c].Formula = string.Format("=SUM(D{0}:D{1})", total, c - 1);
                            _ws.Cells["E" + c].Value = "THB";
                            calc_sec = Convert.ToDecimal(_ws.Cells["D" + c].Value) / Convert.ToDecimal(_dr["ExchangeRate"]);
                            total = c;
                            c++;
                            _ws.Cells["A" + c].Value = "3) Labour/OH/Margin";

                            DataRow[] dtu = dth.Select("component ='Upcharge'");
                            int totalup = c;
                            foreach (var _u in dtu)
                            {
                                c++;
                                _ws.Cells["B" + c].Value = _u["Description"].ToString();
                                _ws.Cells["D" + c].Formula = string.Format("={0}", _u["Amount"]);
                                _ws.Cells["E" + c].Value = "THB";
                            }


                            c++;
                            _ws.Cells["C" + c].Value = "Sub-total THB";
                            _ws.Cells["D" + c].Formula = string.Format("=SUM(D{0}:D{1})", totalup, c - 1);
                            _ws.Cells["E" + c].Value = "THB";
                            calc_loh = Convert.ToDecimal(_ws.Cells["D" + c].Value) / Convert.ToDecimal(_dr["ExchangeRate"]);
                            totalup = c;
                            c++;
                            _ws.Cells["A" + c].Value = "4) Conversion THB components to THB";
                            _ws.Cells["A" + c].Font.Bold = true;
                            c++;
                            _ws.Cells["C" + c].Value = "Sub-total THB";
                            _ws.Cells["D" + c].Formula = string.Format("=SUM(D{0}+D{1})", total, totalup);
                            _ws.Cells["D" + c].FillColor = System.Drawing.Color.Beige;
                            _ws.Cells["E" + c].Value = "THB";
                            c++;
                            _ws.Cells["C" + c].Value = "Sub-total THB";
                            _ws.Cells["D" + c].Formula = string.Format("=+D{0}", c - 1);
                            _ws.Cells["E" + c].Value = "THB";
                            c++;
                            _ws.Cells["A" + c].Value = "5) Total Case Price";
                            _ws.Cells["B" + c].Value = "1) + 4)";
                            _ws.Cells["C" + c].Value = "Grand-total (THB)";
                            _ws.Cells["D" + c].Formula = string.Format("=+D{0}+D{1}", c - 1, totalrm);
                            _ws.Cells["E" + c].Value = "THB";
                            c++;
                            c++;
                            DataRow[] dtmargin = dth.Select("component = 'Margin'");
                            foreach (var _r2 in dtmargin)
                            {
                                _ws.Cells["A" + c].Value = "6) MARGIN";
                                _ws.Cells["C" + c].FillColor = System.Drawing.Color.Beige;
                                _ws.Cells["C" + c].Value = string.Format("{0}%", _r2["Per"]);
                                _ws.Cells["D" + c].Formula = string.Format("=+D{0}*$C${1}", c + 1, c);
                                _ws.Cells["E" + c].Value = "THB";
                            }
                            int fixc = c;
                            c++;
                            _ws.Cells["C" + c].Value = "FOB Price";
                            _ws.Cells["D" + c].Formula = string.Format("=+D{0}+D{1}", c - 1, c - 3);
                            _ws.Cells["D" + c].Font.Bold = true;
                            _ws.Cells["E" + c].Value = "THB";
                            c++;
                            _ws.Cells["D" + c].Formula = string.Format("= D{0}/$B$4", c - 1);
                            _ws.Cells["D" + c].Font.Bold = true;
                            _ws.Cells["E" + c].Value = "USD";
                            c++;
                            _ws.Cells["D" + c].Formula = string.Format("=D{0}*$B$4", c - 1);
                            _ws.Cells["D" + c].Font.Bold = true;
                            _ws.Cells["E" + c].Value = "THB";


                            calc_margin = Convert.ToDecimal(string.Format("{0}", _ws.Cells["D" + fixc].Value)) / Convert.ToDecimal(_dr["ExchangeRate"]);
                        }
                    }
                    //----------------------------------------------------------------------------
                    workbook.Worksheets.Add("SUM");
                    Worksheet _ws2 = Spreadsheet.Document.Worksheets["SUM"];
                    _ws2.Cells["A1"].Value = "Web base";
                    _ws2.Cells["A1"].FillColor = System.Drawing.Color.Beige;
                    _ws2.Cells["A1"].ColumnWidth = 800;
                    _ws2.Cells["A2"].Value = "REF. #";
                    _ws2.Cells["A3"].Value = "NET WEIGHT  :";
                    _ws2.Cells["A4"].Value = "SUB TOTAL 1 - RAW MATERIALS";
                    _ws2.Cells["A5"].Value = "SUB TOTAL 2 - INGREDIENTS";
                    _ws2.Cells["A6"].Value = "SUB TOTAL 3 - Primary PACKAGING";
                    _ws2.Cells["A7"].Value = "SUB TOTAL 4 - Secondary PACKAGING";
                    _ws2.Cells["A8"].Value = "SUB TOTAL 5 - LABOUR & OVERHEAD";
                    _ws2.Cells["A9"].Value = "SUB TOTAL 6 - UPCHARGE";

                    _ws2.Cells["A10"].Value = "Of  raw mat , ing";
                    _ws2.Cells["A11"].Value = "Of  primary packaging";
                    _ws2.Cells["A12"].Value = "Of secondary packaging";
                    _ws2.Cells["A13"].Value = "MARGIN";
                    _ws2.Cells["A14"].Value = "COST PER CASE FOB BANGKOK";
                    _ws2.Cells["A15"].Value = "USD / CTN";
                    _ws2.Cells["A16"].Value = "COST PER CASE FOB BANGKOK";

                    _ws2.Cells["A19"].Value = "MARGIN";
                    _ws2.Cells["A20"].Value = "EXCHANGE RATE : USD 1 = ";

                    DataTable dtproduct2 = cs.GetProductFormula(Id);
                    for (int o = 1; o <= dtproduct2.Rows.Count; o++)
                    {
                        DataRow row = dtproduct2.Rows[o - 1];
                        string _n = cs.GetExcelColumnName(o + 1);
                        _ws2.Cells[_n + "1"].Value = dtproduct2.Rows[o - 1]["CostNo"].ToString();
                        _ws2.Cells[_n + "1"].ColumnWidth = 300;
                        _ws2.Cells[_n + "3"].Value = string.Format("{0}", dtproduct2.Rows[o - 1]["NW"]);
                        _ws2.Cells[_n + "1"].FillColor = System.Drawing.Color.Beige;
                        for (int o2 = 1; o2 <= 9; o2++)
                            _ws2.Cells[_n + o2].Font.Color = System.Drawing.Color.Red;
                        DataTable MyTable = cs.reload(row);
                        if (MyTable != null)
                        {
                            string[] SubType = Regex.Split("Raw Material & Ingredient;Primary Packaging;Secondary Packaging", ";");
                            string[] valueType = Regex.Split("RmIng;PrimaryPkg;SecondaryPkg", ";");
                            foreach (DataRow rw in MyTable.Rows)
                            {
                                List<decimal> list = new List<decimal>();
                                decimal _rm, _totalrm;
                                var _Packaging = row["Packaging"].ToString();
                                var tableloss = cs.GettableLoss(_Packaging, Convert.ToDateTime(_dr["To"]), string.Format("{0}", row["UserType"]));
                                for (int i = 0; i <= valueType.Length - 1; i++)
                                {
                                    _rm = Convert.ToDecimal(rw[valueType[i]]);
                                    var _drloss = tableloss.Select("SubType='" + SubType[i] + "'").FirstOrDefault();

                                    decimal _loss = 0;
                                    decimal.TryParse(_drloss["Loss"].ToString(), out _loss);
                                    _totalrm = _rm * (Convert.ToDecimal(_loss) / 100);
                                    list.Add(_loss);
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
                                Convert.ToDecimal(rw["PrimaryPkg"]),
                                Convert.ToDecimal(rw["SecondaryPkg"]), Convert.ToDecimal(rw["LOH"]),
                                Convert.ToDecimal(rw["UpCharge"]),
                                Convert.ToDecimal(rw["Loss"]),
                                Convert.ToDecimal(rw["Margin"])};
                                _ws2.Cells[_n + "4"].Formula = string.Format("={0}", rw["RM"]);
                                _ws2.Cells[_n + "5"].Formula = string.Format("={0}", rw["Ing"]);
                                _ws2.Cells[_n + "6"].Formula = string.Format("={0}", rw["PrimaryPkg"]);
                                _ws2.Cells[_n + "7"].Formula = string.Format("={0}", rw["SecondaryPkg"]);
                                _ws2.Cells[_n + "8"].Formula = string.Format("={0}", rw["LOH"]);
                                _ws2.Cells[_n + "9"].Formula = string.Format("={0}", rw["UpCharge"]);
                                int desiredIndex = 2;
                                _ws2.Cells[_n + "10"].Formula = string.Format("= SUM({0}4:{0}5)*{1}%", _n, list.Count > desiredIndex && list[desiredIndex] != null ? list[0] : 0);
                                _ws2.Cells[_n + "11"].Formula = string.Format("={0}6*{1}%", _n, list[1]);
                                _ws2.Cells[_n + "12"].Formula = string.Format("={0}7*{1}%", _n, list[2]);
                                _ws2.Cells[_n + "13"].Formula = string.Format("={0}14*{0}19", _n);
                                _ws2.Cells[_n + "14"].Formula = string.Format("=SUM({0}4:{0}13)", _n);
                                _ws2.Cells[_n + "15"].Formula = string.Format("={0}14/{0}20", _n);
                                _ws2.Cells[_n + "16"].Formula = string.Format("={0}", rw["Rate"].ToString() != "" ? list1.Sum() / Convert.ToDecimal(rw["Rate"]) : list1.Sum());
                                _ws2.Cells[_n + "16"].NumberFormat = "$#,##0.00";
                                _ws2.Cells[_n + "19"].Formula = string.Format("={0}%", rw["PerMargin"].ToString() == "" ? "0" : rw["PerMargin"]);
                                _ws2.Cells[_n + "19"].NumberFormat = "###,##%";
                                _ws2.Cells[_n + "20"].Formula = string.Format("={0}", _dr["ExchangeRate"]);
                            }
                        }
                    }
                    //_ws2.Range["D1:D40"].Style.NumberFormat = "##0.00";
                    if (string.Format("{0}", _dr["CustomPrice"]).Equals("1"))
                    {
                        Workbook book1 = new Workbook();
                        book1.LoadDocument(Server.MapPath("~/App_Data/Documents/CALC Template Examples.xlsx"));
                        //FileInfo template = new FileInfo(Server.MapPath("~/App_Data/Documents/CALC Template Examples.xlsx"));
                        //var package = new ExcelPackage(template);
                        //var wbook = package.Workbook;
                        //Spreadsheet.Document.LoadDocument(Server.MapPath("~/App_Data/Documents/CALC Template Examples.xlsx"));
                        //worksheet = Spreadsheet.Document.Worksheets[0];
                        //IWorkbook wbook = Spreadsheet.Document;
                        //var wsheet = wbook.Worksheets.First();
                        workbook.Worksheets.Add("CALC");
                        //workbook.Append(book1);
                        //workbook.Worksheets["Sheet1_Copy"].CopyFrom((Worksheet)wsheet);
                        Worksheet _wsheets = Spreadsheet.Document.Worksheets["CALC"];
                        workbook.Worksheets["CALC"].CopyFrom(book1.Worksheets[0]);
                        DataTable dtx = cs.builditems(@"select * from TransFormulaHeader where Requestno='" + Id.ToString() + "'");
                        int rowx = 7;
                        foreach (DataRow _rw in dtx.Rows)
                        {

                            _wsheets.Cells["H" + rowx].Value = string.Format("{0}", _rw["Name"]);
                            _wsheets.Cells["I" + rowx].Value = string.Format("{0}", _rw["Costno"]);
                            _wsheets.Cells["J" + rowx].Value = string.Format("{0}", _rw["RefSamples"]);

                            string[] sourceArray = string.Format("{0}", _dr["Netweight"]).Split('|');
                            if (sourceArray.Length > 1)
                                _wsheets.Cells["L" + rowx].Formula = string.Format("={0}", Convert.ToDouble(sourceArray[0]) / 28.35);
                            _wsheets.Cells["L" + rowx].NumberFormat = "#,##0.00";
                            _wsheets.Cells["M" + rowx].Formula = string.Format("{0}", _rw["PackSize"].ToString() != "" ? _rw["PackSize"] : _dr["PackSize"]);
                            DataTable Tablex = cs.reload(_rw);
                            DataRow rw = Tablex.Select().FirstOrDefault();
                            string[] SubType = Regex.Split("Raw Material & Ingredient;Primary Packaging;Secondary Packaging", ";");
                            string[] valueType = Regex.Split("RmIng;PrimaryPkg;SecondaryPkg", ";");
                            List<decimal> list = new List<decimal>();
                            decimal _rm, _totalrm;
                            var _Packaging = _dr["Packaging"].ToString();
                            var tableloss = cs.GettableLoss(_Packaging, Convert.ToDateTime(_dr["To"]), string.Format("{0}", _dr["UserType"]));
                            for (int i = 0; i <= valueType.Length - 1; i++)
                            {
                                _rm = Convert.ToDecimal(rw[valueType[i]]);
                                var _drloss = tableloss.Select("SubType='" + SubType[i] + "'").FirstOrDefault();

                                decimal _loss = 0;
                                decimal.TryParse(_drloss["Loss"].ToString(), out _loss);
                                _totalrm = _rm * (Convert.ToDecimal(_loss) / 100);
                                list.Add(_loss);
                                rw["Loss"] = Convert.ToDecimal(rw["Loss"]) + _totalrm;
                                rw["FOB"] = Convert.ToDecimal(rw["FOB"]) + _totalrm + Convert.ToDecimal(rw[valueType[i]]);
                            }
                            decimal totalCount = 0;
                            totalCount = Convert.ToDecimal(rw["FOB"]) + Convert.ToDecimal(rw["LOH"]) + Convert.ToDecimal(rw["UpCharge"]);
                            object sumObject = string.Format(@"{0}", rw["PerMargin"].ToString() == "" ? "0" : rw["PerMargin"].ToString());

                            decimal _MinPrice = (totalCount / ((100 - Convert.ToDecimal(sumObject)) / 100));
                            rw["Margin"] = _MinPrice * (Convert.ToDecimal(sumObject) / 100);
                            _wsheets.Cells["N" + rowx].Formula = string.Format("={0}/{1}", rw["RM"], Convert.ToDouble(_dr["ExchangeRate"]));
                            _wsheets.Cells["O" + rowx].Formula = string.Format("={0}/{1}", rw["Ing"], Convert.ToDouble(_dr["ExchangeRate"]));
                            _wsheets.Cells["P" + rowx].Formula = string.Format("={0}/{1}", rw["PrimaryPkg"], Convert.ToDouble(_dr["ExchangeRate"]));
                            _wsheets.Cells["Q" + rowx].Formula = string.Format("=({0}/{1})+({2})", rw["SecondaryPkg"], Convert.ToDouble(_dr["ExchangeRate"]), calc_sec);
                            _wsheets.Cells["R" + rowx].Formula = string.Format("=({0}/{1})+({2})", rw["LOH"], Convert.ToDouble(_dr["ExchangeRate"]), calc_loh);
                            _wsheets.Cells["S" + rowx].Formula = string.Format("={0}/{1}", rw["UpCharge"], Convert.ToDouble(_dr["ExchangeRate"]));
                            _wsheets.Cells["T" + rowx].Formula = string.Format("=({0}/{1})+({2})", (Convert.ToDecimal(rw["Margin"]) + Convert.ToDecimal(rw["Loss"])), Convert.ToDouble(_dr["ExchangeRate"]), calc_margin);

                            rowx++;
                            _wsheets.Rows[rowx].Insert();
                            _wsheets.Rows[rowx].CopyFrom(_wsheets.Rows[rowx + 1]);
                        }
                    }
                    //_wsheets.Cells["A3"].Value = wsheet.Cells["A3"].Value.ToString();

                }
                //worksheet.InsertCells(worksheet.Range["A29:I29"], InsertCellsMode.EntireRow);
                //workbook.Worksheets.Add("Sheet1_Copy");
                //workbook.Worksheets["Sheet1_Copy"].CopyFrom(workbook.Worksheets["Sheet1"]);
                break;

            case "4":
                Spreadsheet.Visible = true;
                Spreadsheet.Document.LoadDocument(Server.MapPath("~/App_Data/Documents/Quotation_template.xlsx"));
                Spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;
                worksheet = Spreadsheet.Document.Worksheets[0];
                IWorkbook wb = Spreadsheet.Document;
                if (Request.QueryString["command"].Contains("Quotation"))
                {
                    wb.Worksheets.RemoveAt(1);
                    string strText = "select top 1 b.Name from MasCustomer b where b.Code=TransTunaStd.BillTo";
                    string strSQL = String.Format("Select *, ({0}) as Name,getdate() as 'CurrentDate' from TransTunaStd where Id= {1}", strText, id.ToString());
                    DataTable dtstd = cs.builditems(strSQL);
                    if (dtstd.Rows.Count > 0)
                    {
                        worksheet.Cells["D02"].Value = "Page 1";
                        worksheet.Cells["D22"].Value = String.Format("{0:MM/dd/yyyy HH:mm}", dtstd.Rows[0]["CurrentDate"]);
                        worksheet.Cells["C01"].Value = string.Format("Offer price for {0} on {1}", dtstd.Rows[0]["Name"], DateTime.Now.ToString("dd/MM/yyyy"));
                        worksheet.Cells["C02"].Value = string.Format("Request no. {0}", dtstd.Rows[0]["RequestNo"]);
                        worksheet.Cells["D11"].Value = string.Format("{0}", cs.ReadItems("select Value from MasPaymentTerm where Code='" + dtstd.Rows[0]["PaymentTerm"] + "'"));
                        worksheet.Cells["D12"].Value = string.Format("{0}", cs.ReadItems("select Code +':' + name as t from MasIncoterm where code='" + dtstd.Rows[0]["Incoterm"] + "'"));
                        worksheet.Cells["D15"].Value = string.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", dtstd.Rows[0]["From"], dtstd.Rows[0]["To"]);
                        worksheet.Cells["D16"].Value = String.Format("{0:MM/dd/yyyy}", dtstd.Rows[0]["ValidityDate"]);
                        strText = "select top 1 b.CustomTitle from StdCustomTitle b where b.Material=TransTunaStdItems.Material";
                        DataTable dtstditems = cs.builditems(String.Format(@"Select OfferPrice,({0}) as CustomTitle,Material,Name from TransTunaStdItems where RequestNo= {1}",
                            strText, id.ToString()));
                        int c = 4;
                        int num = 1;
                        for (int o = 0; o < dtstditems.Rows.Count; o++)
                        {
                            DataRow dr = dtstditems.Rows[o];
                            worksheet.Rows[c].Insert();
                            worksheet.Rows[c].CopyFrom(worksheet.Rows[c + 1]);
                            worksheet.Cells["B" + c].Value = num;
                            Cell cell = worksheet.Cells["B" + c];
                            cell.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Left;
                            worksheet.Cells["C" + c].Value = string.Format("{0}", dr["CustomTitle"]);
                            //worksheet.Cells["D" + c].Value = Rounding.RoundDown(Convert.ToDecimal(dr["OfferPrice"].ToString()), 2);
                            if (!string.IsNullOrEmpty(dr["OfferPrice"].ToString()))
                                worksheet.Cells["D" + c].Value = Math.Round(Convert.ToDecimal(dr["OfferPrice"].ToString()), 2).ToString();
                            worksheet.Cells["D" + c].NumberFormat = "$#,##0.00";

                            worksheet.Cells["D" + c].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Right;
                            c++;
                            num++;
                        }
                    }
                }
                else if (Request.QueryString["command"].Contains("calculation"))
                {
                    buildsum(id.ToString());
                    wb.Worksheets.RemoveAt(0);
                }
                break;
            case "5":
                Spreadsheet.Visible = true;
                DataTable dtRequest = cs.builditems("select a.*,b.Company from TransRequestForm a inner join TransTechnical b on b.ID=a.RequestNo where a.ID=" + id);

                foreach (DataRow ro in dtRequest.Rows)
                {
                    if (string.Format("{0}", ro["Company"]).Equals("1031"))
                        Spreadsheet.Document.LoadDocument(Server.MapPath("~/App_Data/Documents/SS - แบบฟอร์มขอ Code ผลิตภัณฑ์.xlsx"));
                    else
                        Spreadsheet.Document.LoadDocument(Server.MapPath("~/App_Data/Documents/F3RPRD42 แบบฟอร์มขอ Code ผลิตภัณฑ์.xlsx"));
                    Spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;
                    Worksheet _wsheet = Spreadsheet.Document.Worksheets[0];
                    IWorkbook _wb = Spreadsheet.Document;

                    SqlParameter[] ph = { new SqlParameter("@ID", string.Format("{0}", id)),
                        new SqlParameter("@ProductGroup", string.Format("{0}", ro["ProductGroup"])),
                        new SqlParameter("@Zone", string.Format("{0}", ro["Zone"])),
                        new SqlParameter("@MediaType", string.Format("{0}", ro["MediaType"])),
                        new SqlParameter("@NW", string.Format("{0}", ro["NW"])),
                        new SqlParameter("@Division", string.Format("{0}", ro["Division"])),
                        new SqlParameter("@ContainerLid", string.Format("{0}", ro["ContainerLid"])),
                        new SqlParameter("@Nutrition", string.Format("{0}", ro["Nutrition"])),
                        new SqlParameter("@PetType", string.Format("{0}", ro["PetType"])),
                        new SqlParameter("@StyleOfPack", string.Format("{0}", ro["StyleOfPack"])),
                        new SqlParameter("@RawMaterial", string.Format("{0}", ro["RawMaterial"])),
                        new SqlParameter("@Grade", string.Format("{0}", ro["Grade"]))
                    };
                    DataTable _dt = myClass.GetRelatedResources("spGetRequestFormReport", ph);
                    foreach (DataRow rw in _dt.Rows)
                    {
                        var requester = cs.builditems(@"select (select concat(u.FirstName,' ',u.LastName ) from ulogin u where u.user_name = requester)'requester',customer from TransTechnical where ID=" +
                            string.Format("{0}", ro["RequestNo"]));
                        foreach (DataRow _trow in requester.Rows)
                        {

                            _wsheet.Cells["I5"].Value = string.Format("{0}", _trow["customer"]);
                            _wsheet.Cells["E5"].Value = string.Format("{0}", _trow["requester"]);
                        }
                        var cost = cs.ReadItems(@"select Name from TransFormulaHeader where ID=" + string.Format("{0}", ro["Costno"]));
                        _wsheet.Cells["D4"].Value = string.Format("{0}", cost);
                        //_wsheet.Cells["E3"].Value = string.Format("{0}", ro["DocumentNo"]);
                        var Code = string.Format("3{0}{1}{2}{3}{4}{5}{6}{7}00", ro["ProductGroup"], ro["RawMaterial"], ro["StyleofPack"], ro["MediaType"],
                            ro["NW"], ro["ContainerLid"], ro["Grade"], ro["Zone"]);
                        _wsheet.Cells["D22"].Value = string.Join("", new string[] { string.Format("{0}",ro["ProductGroup"]),
                        string.Format("{0}",ro["ContainerLid"]), string.Format("{0}",ro["Grade"]),string.Format("{0}",ro["Zone"])," ",
                        string.Format("{0}",ro["RawMaterial"]),string.Format("{0}",ro["StyleofPack"]),string.Format("{0}",ro["MediaType"]),
                        string.Format("{0}",ro["NW"])});
                        //_wsheet.Pictures.AddPicture(GetImageCheckbox(rw["ProductGroup"].ToString(), "G"), 100, 780);
                        _wsheet.Cells["D11"].Value = string.Format("{0}", ro["Remark"]);
                        _wsheet.Cells["D6"].Value = string.Format("{0}", rw["ProductGroupName"]);
                        _wsheet.Cells["F7"].Value = string.Format("{0}", rw["BrandName"]);
                        _wsheet.Cells["H9"].Value = string.Format("{0}", rw["RawMaterialName"]);
                        _wsheet.Cells["D8"].Value = string.Format("{0}", rw["PetTypeName"]);

                        _wsheet.Cells["F10"].Value = string.Format("{0}", rw["StyleName"]);
                        _wsheet.Cells["E11"].Value = string.Format("{0}", rw["CanSizeName"]);
                        _wsheet.Cells["D12"].Value = string.Format("{0}", rw["NutritionName"]);
                        _wsheet.Cells["G13"].Value = string.Format("{0}:{1}", ro["Grade"], rw["ZoneName"]);
                        _wsheet.Cells["G15"].Value = string.Format("{0}", rw["ContainerLidName"]);

                        _wsheet.Cells["D19"].Value = string.Format("{0}", ro["RefSamples"]);
                        _wsheet.Cells["D14"].Value = string.Format("{0}", rw["DivisionName"]);
                        _wsheet.Cells["D16"].Value = string.Format("{0}", rw["MediaTypeName"]);
                        _wsheet.Cells["D17"].Value = string.Format("{0}", ro["PackingStyle"]);
                        _wsheet.Cells["F3"].Value = String.Format("{0:dd/MM/yyyy}", ro["RequestDate"]);
                        StringBuilder sb = new StringBuilder();
                        //spGetHierarchy
                        SqlParameter[] param = {new SqlParameter("@ProductGroup",string.Format("{0}", ro["ProductGroup"])),
                        new SqlParameter("@RawMaterials",string.Format("{0}",ro["RawMaterial"])),
                        new SqlParameter("@Division",string.Format("{0}",ro["Division"])),
                        new SqlParameter("@Style",string.Format("{0}",ro["StyleofPack"])),
                        new SqlParameter("@Media",string.Format("{0}",ro["NW"])),
                        new SqlParameter("@Customer",string.Format("{0}",ro["Zone"])),
                        new SqlParameter("@Nutrition",string.Format("{0}",ro["Nutrition"])),
                        new SqlParameter("@PetType",string.Format("{0}",ro["PetType"])),
                        new SqlParameter("@Container",string.Format("{0}",ro["ContainerLid"]))};
                        DataTable mydt = myClass.GetRelatedResources("spGetHierarchy", param);
                        //if (args[1] == "Zone")
                        //{
                        //    result["Name"] += cmbZone.Value.ToString();
                        //}
                        foreach (DataRow row in mydt.Rows)
                        {
                            sb.Append(row[0] + " ");
                        }
                        _wsheet.Cells["E18"].Value = string.Join("", sb);

                    }
                }
                //SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                //var workbook2 =  ExcelFile.Load(Server.MapPath("~/App_Data/Documents/F3RPRD42 แบบฟอร์มขอ Code ผลิตภัณฑ์ - Rev 0.xls"));

                //var sb = new StringBuilder();

                //// Iterate through all worksheets in an Excel workbook.
                //foreach (var worksheet2 in workbook2.Worksheets)
                //{
                //    //var worksheet2 = workbook2.Worksheets.Add("Form Controls");
                //    var checkBox = worksheet2.FormControls.AddCheckBox("Simple check box", "B2", 100, 15, LengthUnit.Point);
                //    checkBox.CellLink = worksheet2.Cells["A2"];
                //    checkBox.Checked = true;
                //}
                //worksheet.Cells["A4"].Value = "VALUE A";
                //worksheet.Cells["A5"].Value = "VALUE B";
                //worksheet.Cells["A6"].Value = "VALUE C";
                //worksheet.Cells["A7"].Value = "VALUE D";
                //var comboBox = worksheet2.FormControls.AddComboBox("B4", 100, 20, LengthUnit.Point);
                //comboBox.InputRange = worksheet2.Cells.GetSubrange("A4:A7");
                //comboBox.SelectedIndex = 2;

                //var scrollBar = worksheet2.FormControls.AddScrollBar("B9", 100, 20, LengthUnit.Point);
                //scrollBar.CellLink = worksheet2.Cells["A9"];
                //scrollBar.MinimumValue = 10;
                //scrollBar.MaximumValue = 50;
                //scrollBar.CurrentValue = 20;
                //HttpResponseBase responseBase = new HttpResponseWrapper(this.Response);
                //workbook2.Save(responseBase, "Spreadsheet.xlsx");

                break;
            case "6":
                IWorkbook wbook = Spreadsheet.Document;
                DataTable dtcusf = cs.GetCusFormula(id.ToString());
                SqlParameter[] paramcus = {new SqlParameter("@Id",id.ToString()),
                        //new SqlParameter("@param",args[1].ToString()),
                        new SqlParameter("@username",cs.CurUserName.ToString())};
                var dthd = cs.GetRelatedResources("spGetCusFormulaHeader", paramcus);
                foreach (DataRow rows in dthd.Rows)
                {
                    if (string.Format("{0}", rows["Company"]).Contains("101"))
                    {
                        Spreadsheet.Document.LoadDocument(Server.MapPath("~/App_Data/Documents/Food cost template sent v3.xlsx"));
                        Spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;
                        Worksheet wsheet = Spreadsheet.Document.Worksheets[0];
                        wsheet.Cells["C5"].Value = string.Format("{0}", rows["RequestNo"]);
                        wsheet.Cells["C6"].Value = string.Format("{0}", rows["Code"]);
                        wsheet.Cells["G8"].Value = string.Format("{0}", rows["Name"]);

                    }
                    else
                    {
                        Spreadsheet.Document.LoadDocument(Server.MapPath("~/App_Data/Documents/New experiment sheet 01-12-22 v2.xlsx"));
                        Spreadsheet.Document.DocumentSettings.Calculation.Iterative = true;
                        Worksheet wsheet = Spreadsheet.Document.Worksheets[0];


                        //List<formula> stdList = new List<formula>();
                        wsheet.Cells["B5"].Value = string.Format("{0}", rows["RequestNo"]);
                        wsheet.Cells["B6"].Value = string.Format("{0}", rows["Customer"]);
                        wsheet.Cells["B7"].Value = string.Format("{0}", rows["Code"]);
                        wsheet.Cells["B8"].Value = string.Format("{0}", rows["ScheduledProcess"]);
                        wsheet.Cells["B9"].Value = string.Format("{0}", rows["Primary1pt"]);
                        wsheet.Cells["F9"].Value = string.Format("{0}", rows["MatCode1"]);
                        wsheet.Cells["I9"].Value = string.Format("{0}", rows["pkgSupplier1"]);
                        wsheet.Cells["B10"].Value = string.Format("{0}", rows["Primary2pt"]);
                        wsheet.Cells["F10"].Value = string.Format("{0}", rows["MatCode2"]);
                        wsheet.Cells["I10"].Value = string.Format("{0}", rows["pkgSupplier2"]);
                        wsheet.Cells["F7"].Value = string.Format("{0}", rows["RefSamples"]);
                        wsheet.Cells["F7"].Value = string.Format("{0}", rows["RefSamples"]);
                        wsheet.Cells["F8"].Value = string.Format("{0}", rows["NetWeight"]).Split('|')[0];
                        wsheet.Cells["I8"].Value = string.Format("{0}", rows["FW"]);
                        wsheet.Cells["E6"].Value = string.Format("{0}", rows["Name"]);
                        wsheet.Cells["I7"].Value = string.Format("{0}", rows["rDate"]);

                        wsheet.Name = string.Format("{0}", rows["RequestNo"]);


                        int cu = 13;
                        //var listComponent = (from r in dtcusf.AsEnumerable()
                        //                    select r["Component"]).Distinct().ToList();
                        //foreach(var name in listComponent)
                        //{
                        //wsheet.Cells["B" + cu].Value = string.Format("{0}", name);
                        //cu++;
                        string x = "";
                        int j = 1;
                        var groupeh = from DataRow dr in dtcusf.Rows where (int)dr["ParentID"] == 0 orderby dr["Component"] group dr by dr["Component"];
                        foreach (var k in groupeh)
                        {
                            //if (j > 0)
                            //{
                            //cu = cu + 3;
                            Worksheet wsheet2 = Spreadsheet.Document.Worksheets[1];
                            wsheet.Range[string.Format("A{0}:J{0}", cu)].Insert();
                            wsheet.Range[string.Format("A{0}:J{0}", cu)].CopyFrom(wsheet2.Range["A13:J13"]);
                            wsheet.Range[string.Format("A{0}:J{0}", cu)].Style = wsheet2.Range["A13:J13"].Style;
                            wsheet.Cells["A" + cu].Value = string.Format("{0} {1}",j, k.Key);
                            //}
                            j++;
                            cu++;
                            //x = (string)(k.ElementAt(0)["Component"]) + Environment.NewLine;
                            DataRow[] listportion = dtcusf.Select("ParentID = 0 and Component = '" + k.Key + "'");
                            foreach (DataRow dr in listportion)
                            {

                                wsheet.Rows[cu].Insert();
                                wsheet.Rows[cu].CopyFrom(wsheet2.Rows["14"]);
                                wsheet.Range[string.Format("A{0}:J{0}", cu)].Style = wsheet2.Range["A14:J14"].Style;
                                wsheet.Cells["A" + cu].Value = string.Format("{0}", dr["Description"]);
                                wsheet.Cells["F" + cu].Formula = string.Format("{0:N2}", dr["Result"]);//string.Format("{0}", dr["Result"]);
                                wsheet.Cells["C" + cu].Formula = string.Format("{0:N2}%", dr["Yield"]);
                                wsheet.Cells["D" + cu].Value = string.Format("{0}", dr["Material"]);
                                wsheet.Cells["B" + cu].Value = string.Format("{0}", dr["Note"]);
                                wsheet.Cells["E" + cu].Value = string.Format("{0}", dr["IDNumber"]);
                                cu++;
                            }
                            cu++;
                            //wsheet.Range[string.Format("A{0}:J{0}",cu)].Insert();
                            //wsheet.Range[string.Format("A{0}:J{0}", cu)].CopyFrom(wsheet2.Range["A15:J15"]);
                            //wsheet.Range[string.Format("A{0}:J{0}", cu)].Style = wsheet2.Range["A15:J15"].Style;

                            //wsheet.Cells[string.Format("E{0}", cu + 1)].Value = cu + 1;
                            //wsheet.Cells[string.Format("F{0}", cu + 1)].Formula = string.Format("=SUM(F14:F{0})", cu);
                            //cu = cu + 3;
                        }
                            //DataRow[] listdr = dtcusf.Select("(Component like '%Solution%' or Component like '%Topping%') and ParentID = 0");
                        //foreach (DataRow dr in listdr)
                        //{
                        //    wsheet.Rows[cu].Insert();
                        //    wsheet.Rows[cu].CopyFrom(wsheet.Rows[cu - 1]);
                        //    wsheet.Cells["A" + cu].Value = string.Format("{0}", dr["Description"]);
                        //    wsheet.Cells["B" + cu].Value = string.Format("{0}", dr["Note"]);
                        //    wsheet.Cells["E" + cu].Value = string.Format("{0}", dr["IDNumber"]);
                        //    wsheet.Cells["F" + cu].Formula = string.Format("{0:N2}", dr["Result"]);
                        //    wsheet.Cells["C" + cu].Formula = string.Format("{0:N2}%", dr["Yield"]);
                        //    wsheet.Cells["D" + cu].Value = string.Format("{0}", dr["Material"]);
                        //    cu++;
                        //}

                        //cu = cu + 3;
                        //var tParentID = dtcusf.AsEnumerable().GroupBy(r => r.Field<int>("ParentID")).Select(g => g.First()).CopyToDataTable();
                        //var grouped = from DataRow dr in dtcusf.Rows where (int)dr["ParentID"] > 0 orderby dr["ParentID"] group dr by dr["ParentID"];
                        
                        //foreach (var k in grouped)
                        //{
                        //    x = (int)(k.ElementAt(0)["ParentID"]) + Environment.NewLine;
                        //    //if (j > 0)
                        //    //{
                        //        cu = cu + 3;
                        //        Worksheet wsheet2 = Spreadsheet.Document.Worksheets[1];
                        //        wsheet.Range[string.Format("A{0}:J{1}", cu, cu + 4)].Insert();
                        //        wsheet.Range[string.Format("A{0}:J{1}", cu, cu + 4)].CopyFrom(wsheet2.Range["A20:J23"]);
                        //    //}

                        //    //j++;
                        //    //wsheet.Cells["A" + cu].Value = string.Format("{0}", x);
                        //    cu = cu + 1;
                        //    DataRow[] tParentID = dtcusf.Select("ParentID > 0 and ParentID = " + string.Format("{0}", x));
                        //    var hdr = dtcusf.Select("ID = " + string.Format("{0}", x)).FirstOrDefault();
                        //    wsheet.Cells[string.Format("A{0}", cu - 1)].Value = string.Format("{0}", hdr["Description"]);
                        //    foreach (DataRow dr in tParentID)
                        //    {
                        //        wsheet.Rows[cu].Insert();
                        //        wsheet.Rows[cu].CopyFrom(wsheet.Rows[cu - 1]);
                        //        wsheet.Cells["A" + cu].Value = string.Format("{0}", dr["Description"]);
                        //        wsheet.Cells["B" + cu].Value = string.Format("{0}", dr["Note"]);
                        //        wsheet.Cells["H" + cu].Formula = string.Format("{0:N2}", dr["Portion"]);
                        //        //wsheet.Cells["F" + cu].Formula = string.Format("{0:N2}", dr["Result"]);
                        //        wsheet.Cells["C" + cu].Formula = string.Format("{0:N2}%", dr["Yield"]);
                        //        wsheet.Cells["D" + cu].Value = string.Format("{0}", dr["Material"]);
                        //        cu++;
                        //    }
                        //}
                        ////}
                        wbook.Worksheets.RemoveAt(2);
                        wbook.Worksheets.RemoveAt(1);
                    }
                }
                break;
        }
    }

    void build_detail(string id,DataRow _dr)
    {
        /*string strSQl = @"select *,isnull(Code,'') as 'Material'
        ,isnull((select top 1 z.UserType from TransCostingHeader z where Id=a.Requestno),0)UserType
	    ,isnull((select top 1 a.NetWeight from TransProductList a where Id=pid),0)NetWeight
        ,isnull((select top 1 a.FixedFillWeight from TransProductList a where Id=pid),0)FixedFillWeight
        ,isnull((select top 1 a.PackSize from TransProductList a where Id=pid),0)PackSize from TransFormulaHeader a where a.Requestno='" + id.ToString() + "'";*/
        SqlParameter[] param = { new SqlParameter("@id", string.Format("{0}", id)) };
        DataTable dtproduct = GetRelatedResources("spGetBuildDetail", param);

        //DataTable dtproduct = cs.builditems(strSQl);
        IWorkbook workbook = Spreadsheet.Document;
        //StreamReader stRead = new StreamReader(Server.MapPath(datapath));
        //dataTable = JsonConvert.DeserializeObject<DataTable>(stRead.ReadToEnd());
        //var myResult = dataTable.AsEnumerable()
        //.Select(s => new {
        //    id = s.Field<string>("Component"),
        //})
        //.Distinct().ToList();
        DataTable Myresult = cs.GettableLoss(p, Convert.ToDateTime( _dr["To"]), string.Format("{0}", dtproduct.Rows[0]["UserType"]));
        foreach (DataRow row in Myresult.Rows)
        {
            //worksheet.Cells["B" + irow].NumberFormat = "###,##%";
            if (string.Format("{0}", row["SubType"]) == "Raw Material & Ingredient"){
                worksheet.Cells["B17"].Formula = string.Format("{0:N2}%", row["Loss"]);
                worksheet.Cells["B21"].Formula = string.Format("{0:N2}%", row["Loss"]);
            }
            else if (string.Format("{0}", row["SubType"]) == "Primary Packaging")
                worksheet.Cells["B25"].Formula = string.Format("{0:N2}%", row["Loss"]);
            else if (string.Format("{0}", row["SubType"]) == "Secondary Packaging")
                worksheet.Cells["B29"].Formula = string.Format("{0:N2}%", row["Loss"]);

        }
        dataTable = new DataTable();
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spselectformula";
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Connection = con;
            con.Open();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dataTable);
            con.Close();
            con.Dispose();
        }
        for (int s = 1; s <= dtproduct.Rows.Count; s++)
        {
            string fixrow = "16";
            var foundRows = dtproduct.Select("formula = '" + s.ToString() + "'").FirstOrDefault();
            if (foundRows != null)
            {
                string cnumber = foundRows["CostNo"].ToString();
                foreach (Worksheet sheet in workbook.Worksheets)
                {
                    if (sheet.Name.Equals(cnumber))
                        cnumber = string.Format("{0}_{1}", cnumber, s);
                }
                workbook.Worksheets.Add(cnumber);
                workbook.Worksheets[cnumber].CopyFrom(workbook.Worksheets[0]);
                worksheet = Spreadsheet.Document.Worksheets[cnumber];
            }
            var _rw = foundRows;
            int irow = 16;
            build_Header(id, foundRows);
            Worksheet ws = Spreadsheet.Document.Worksheets[s];
            string _margin = cs.ReadItems(@"select Per from TransCosting Where Component = 'margin' and Formula='" 
                + s.ToString() + "' and  RequestNo = '" + id + "'");
            worksheet.Cells["B41"].Formula = string.Format("{0:N2}%", _margin);
            ws.Cells["B5"].Value = string.Format("{0}", _rw["Name"]);
            DataRow[] mytable = dataTable.Select("Component = 'Raw Material' and Formula='" + s.ToString() + "'");
            foreach (DataRow row in mytable)
            {
                worksheet.Rows[irow].Insert();
                worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
                double Yield = 0;
                double.TryParse(row["Yield"].ToString(), out Yield);
                worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                if (row["Description"].ToString().Contains("product"))
                {
                    worksheet.Cells["G" + irow].Formula = "=" + (string)row["PriceOfCarton"];
                    //worksheet.Cells["G" + irow].Value = Convert.ToDouble(row["PriceOfCarton"]).ToString("C2", CultureInfo.CreateSpecificCulture("th-TH"));
                }
                else
                {
                    worksheet.Cells["G" + irow].Formula = string.Format("{0}", "=(F" + irow + "*E" + irow + ")*$B$11");
                    worksheet.Cells["B" + irow].Value = (string)row["Material"];
                }
   
                    worksheet.Cells["A" + irow].Value = (string)row["Description"];
                    worksheet.Cells["C" + irow].Value = Convert.ToDouble(row["Result"]);
                    worksheet.Cells["D" + irow].Formula = "=" + Yield + "%";
                
                Cell cell = worksheet.Cells["F" + irow];
                cell.Value = Convert.ToDouble(row["PriceOfUnit"]);
                cell.NumberFormat = "#,###.##";
                irow++;
                if (mytable.Length == (irow - 16))
                    worksheet.Range["A" + irow + ":M" + irow].ClearContents();
            }
            worksheet.Cells["G" + (irow + 1)].Formula = string.Format("{0}", "=SUM(G" + fixrow + ":G" + (irow) + ")*B" + (irow + 1));
            DataTable dt = new DataTable();
            dt = cs.GetMergeDataSource(s, "2", id);
            irow = irow + 4; fixrow = string.Format("{0}", irow); double BaseUnit;
            int RowCount = dt.Rows.Count;
            foreach (DataRow row in dt.Rows)
            {
                int RowIndex = dt.Rows.IndexOf(row);
                //if (dt.Rows.IndexOf(row) != 0)
                //if ((RowCount - 1) == RowIndex)
                worksheet.Rows[irow].Insert();
                worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);

                //CellRange sourceCell = worksheet.Range["A" + fixrow + ":M" + fixrow];
                //worksheet.Range["A" + irow + ":M" + irow].CopyFrom(sourceCell);

                DevExpress.Spreadsheet.CellRange range1 = worksheet.Range["A" + (irow+1) + ":F" + (irow + 1)];
                range1.ClearContents();
                worksheet.Range["A" + irow + ":B" + irow].Borders.SetAllBorders(Color.White, BorderLineStyle.Thin);
                worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;

                if (row["userlevel"].ToString() == "0")
                {
                    worksheet.Cells["A" + irow].Value = (string)row["SubType"];
                    double.TryParse(row["PriceOfCarton"].ToString(), out BaseUnit);
                    //Std1Carton += BaseUnit;
                    worksheet.Cells["G" + irow].Formula = string.Format("={0}", BaseUnit.ToString());
                }else{
                    worksheet.Cells["A" + irow].Value = (string)row["Description"]+ irow;
                    worksheet.Cells["B" + irow].Value = (string)row["Material"];
                    worksheet.Cells["C" + irow].Value = Convert.ToDouble(row["Result"]);
                    double Yield = 0;
                    double.TryParse(row["Yield"].ToString(), out Yield);
                    if (Yield > 0)
                    {
                        worksheet.Cells["D" + irow].Formula = "="+ Yield + "%";
                        worksheet.Cells["E" + irow].Formula = "=C" + irow + "/1000*$B$11/D" + irow;
                    }
                    if (row["BaseUnit"].ToString() != "")
                        worksheet.Cells["F" + irow].Value = Convert.ToDouble(row["BaseUnit"]);
                    if (string.IsNullOrEmpty(row["PriceOfCarton"].ToString()))
                        worksheet.Cells["F" + irow].Value = Convert.ToDouble(row["PriceOfCarton"]);
                    else
                        worksheet.Cells["G" + irow].Formula = string.Format("={0}", ("E" + irow + "* F" + irow).ToString());
                }
                irow++;
                if (dt.Rows.Count == (irow - 16))
                    worksheet.Range["A" + irow + ":M" + irow].ClearContents();

            }
            worksheet.Cells["G" + (irow + 1)].Formula = string.Format("{0}", "=SUM(G" + fixrow + ":G" + (irow) + ")*B" + (irow + 1));
            worksheet.Cells["I" + (irow + 1)].Formula = string.Format("{0}", "=SUM(I" + fixrow + ":I" + (irow) + ")*B" + (irow + 1));

            string strSQL = @"select *,case when Component in ('Semi','BCDL','OH','DL') then 'Labor' else 
            Component end 'SubType' from TransCosting Where RequestNo = '" + id + "'";
            dt = cs.builditems(strSQL);
            var result = dt.Select("Component = 'Primary Packaging' and Formula='" + s.ToString() + "'");
            fixrow = string.Format("{0}", irow + 5);
            irow += 4; fixrow = irow.ToString();
            int px = 0;
            foreach (DataRow row in result)
            {
                px++;
                worksheet.Rows[irow].Insert();
                worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
                
                worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                worksheet.Range["G" + irow + ":I" + irow].Borders.SetOutsideBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
                worksheet.Cells["A" + irow].Value = (string)row["Description"];
                worksheet.Cells["B" + irow].Value = (string)row["SAPMaterial"];
                worksheet.Cells["E" + irow].Value = Convert.ToDouble(row["Quantity"]);

                double PriceUnit = 0;
                double.TryParse(row["PriceUnit"].ToString(), out PriceUnit);
                if (PriceUnit > 0)
                    worksheet.Cells["F" + irow].Value = PriceUnit;
                worksheet.Cells["G" + irow].Formula = string.Format("{0}", "=E" + irow + " * F" + irow + "*$B$11" );
                //worksheet.Cells["H" + irow].Formula = string.Format("{0}", "=G" + irow + "/$H$11");
                if ( Convert.ToInt32(result.Length) == px)
                irow++;
            }
            worksheet.Cells["G" + (irow + 1)].Formula = string.Format("{0}", "=SUM(G" + fixrow + ":G" + irow + ")*B" + (irow + 1));
            worksheet.Cells["I" + (irow + 1)].Formula = string.Format("{0}", "=SUM(I" + fixrow + ":I" + irow + ")*B" + (irow + 1));
            worksheet.Cells["G" + (irow + 2)].Formula = string.Format("{0}", "=SUM(G" + fixrow + ":G" + (irow + 1) + ")");
            worksheet.Cells["I" + (irow + 2)].Formula = string.Format("{0}", "=SUM(I" + fixrow + ":I" + (irow + 1) + ")");
            result = dt.Select("Component = 'Secondary Packaging' and Formula='" + s.ToString() + "'");
            irow += 4; fixrow = irow.ToString();
            //worksheet.ClearContents(worksheet["A" + irow + ":I63"]);
            foreach (DataRow row in result)
            {
                worksheet.Rows[irow].Insert();
                worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);

                worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                worksheet.Range["G" + irow + ":I" + irow].Borders.SetOutsideBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
                worksheet.Cells["A" + irow].Value = (string)row["Description"];
                worksheet.Cells["B" + irow].Value = (string)row["SAPMaterial"];
                worksheet.Cells["E" + irow].Value = Convert.ToDouble(row["Quantity"]);

                double PriceUnit = 0;
                double.TryParse(row["PriceUnit"].ToString(), out PriceUnit);
                if (PriceUnit > 0)
                    worksheet.Cells["F" + irow].Value = PriceUnit;
                worksheet.Cells["G" + irow].Formula = string.Format("{0}", "=E" + irow + " * F" + irow + "*$B$11");
                //worksheet.Cells["H" + irow].Formula = string.Format("{0}", "=G" + irow + "/$H$11");
                irow++;
            }
            worksheet.Cells["G" + (irow + 1)].Formula = string.Format("{0}", "=SUM(G" + fixrow + ":G" + irow + ")*B" + (irow + 1));
            worksheet.Cells["I" + (irow + 1)].Formula = string.Format("{0}", "=SUM(I" + fixrow + ":I" + irow + ")*B" + (irow + 1));
            worksheet.Cells["G" + (irow + 2)].Formula = string.Format("{0}", "=SUM(G" + fixrow + ":G" + (irow + 1) + ")");
            worksheet.Cells["I" + (irow + 2)].Formula = string.Format("{0}", "=SUM(I" + fixrow + ":I" + (irow + 1) + ")");

            result = dt.Select("Component in ('DL','OH','Labor') and Formula='" + s.ToString() + "'");
            irow += 4; fixrow = irow.ToString();
            foreach (DataRow row in result)
            {
                worksheet.Rows[irow].Insert();
                worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
                worksheet.Range["A" + (irow+1) + ":F" + (irow + 1)].ClearContents();
                worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                worksheet.Cells["A" + irow].Value = (string)row["Description"];

                double PriceUnit = 0;
                PriceUnit = Convert.ToDouble(row["PriceUnit"].ToString(), CultureInfo.InvariantCulture);
                worksheet.Cells["C" + irow].Value = PriceUnit;
                worksheet.Cells["F" + irow].Value = Convert.ToDouble(row["Quantity"]);
                if (row["Component"].ToString().Equals("BCDL"))
                    worksheet.Cells["G" + irow].Formula = "= (F" + irow + " * C" + irow + "*$B$9 * $B$11/1000)";
                else if (row["Component"].ToString().Equals("Semi"))
                    worksheet.Cells["G" + irow].Formula = "= (F" + irow + " * C" + irow + "*$B$10 * $B$11/1000)";
                else
                    worksheet.Cells["G" + irow].Formula = "= F" + irow + " * C" + irow;
                irow++;
            }
            string[] SubType = Regex.Split("G;H;I;J;K;L", ";");
            foreach (string sub in SubType) 
                worksheet.Cells[sub + (irow + 1)].Formula = string.Format("{0}", "=SUM("+ sub + fixrow + ":"+sub + irow + ")");
            //worksheet.Cells["H" + (irow + 1)].Formula = string.Format("{0}", "=SUM(H" + fixrow + ":H" + irow + ")");
            //worksheet.Cells["I" + (irow + 1)].Formula = string.Format("{0}", "=SUM(I" + fixrow + ":I" + irow + ")");
            //worksheet.Cells["J" + (irow + 1)].Formula = string.Format("{0}", "=SUM(J" + fixrow + ":J" + irow + ")");
            //worksheet.Cells["K" + (irow + 1)].Formula = string.Format("{0}", "=SUM(K" + fixrow + ":K" + irow + ")");
            //worksheet.Cells["L" + (irow + 1)].Formula = string.Format("{0}", "=SUM(L" + fixrow + ":L" + irow + ")");

            result = dt.Select("Component in ('SGA','SG&A') and Formula='" + s.ToString() + "'");
            irow += 3; fixrow = irow.ToString();
            foreach (DataRow row in result)
            {
                worksheet.Rows[irow].Insert();
                worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
                worksheet.Range["A" + (irow + 1) + ":F" + (irow + 1)].ClearContents();
                worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                worksheet.Cells["A" + irow].Value = (string)row["Description"];

                double PriceUnit = 0;
                PriceUnit = Convert.ToDouble(row["PriceUnit"].ToString(), CultureInfo.InvariantCulture);
                worksheet.Cells["C" + irow].Value = PriceUnit;
                worksheet.Cells["F" + irow].Value = Convert.ToDouble(row["Quantity"]);
                if (row["Component"].ToString().Equals("BCDL"))
                    worksheet.Cells["G" + irow].Formula = "= (F" + irow + " * C" + irow + "*$B$9 * $B$11/1000)";
                else if (row["Component"].ToString().Equals("Semi"))
                    worksheet.Cells["G" + irow].Formula = "= (F" + irow + " * C" + irow + "*$B$10 * $B$11/1000)";
                else
                    worksheet.Cells["G" + irow].Formula = "= F" + irow + " * C" + irow;
                irow++;
            }
            foreach (string sub in SubType)
                worksheet.Cells[sub + (irow + 1)].Formula = string.Format("{0}", "=SUM(" + sub + fixrow + ":" + sub + irow + ")");

            result = dt.Select("Component in ('Upcharge') and Formula='" + s.ToString() + "'");
            irow += 3; fixrow = irow.ToString();
            foreach (DataRow row in result)
            {
                worksheet.Rows[irow].Insert();
                worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
                worksheet.Range["A" + (irow + 1) + ":F" + (irow + 1)].ClearContents();
                worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                worksheet.Cells["A" + irow].Value = (string)row["Description"];

                double PriceUnit = 0;
                PriceUnit = Convert.ToDouble(row["PriceUnit"].ToString(), CultureInfo.InvariantCulture);
                worksheet.Cells["C" + irow].Value = PriceUnit;
                worksheet.Cells["F" + irow].Value = Convert.ToDouble(row["Quantity"]);
                if (row["Component"].ToString().Equals("BCDL"))
                    worksheet.Cells["G" + irow].Formula = "= (F" + irow + " * C" + irow + "*$B$9 * $B$11/1000)";
                else if (row["Component"].ToString().Equals("Semi"))
                    worksheet.Cells["G" + irow].Formula = "= (F" + irow + " * C" + irow + "*$B$10 * $B$11/1000)";
                else
                    worksheet.Cells["G" + irow].Formula = "= F" + irow + " * C" + irow;
                irow++;
            }
            foreach (string sub in SubType)
                worksheet.Cells[sub + (irow + 1)].Formula = string.Format("{0}", "=SUM(" + sub + fixrow + ":" + sub + irow + ")");
            //string[] SubType = Regex.Split("Labor;SGA;SG&A;Upcharge", ";");
            //foreach (string sub in SubType)
            //{
            //    px = 0;
            //    result = dt.Select("SubType in ('" + sub + "') and Formula='" + s.ToString() + "'");
            //    if (sub.Equals("Labor"))
            //        irow += 4;
            //    else if (sub.Equals("SGA")|| sub.Equals("SG&A"))
            //        irow += 3;
            //    else if (sub.Equals("Upcharge"))
            //        irow += 3;
            //    fixrow = irow.ToString();
            //    //worksheet.ClearContents(worksheet["A" + irow + ":I71"]);
            //    //foreach (DataRow row in result)
            //    //{
            //    //    px++;
            //    //    double d = 0;
            //    //    if (Double.TryParse(row["Quantity"].ToString(), out d))
            //    //    { // if done, then is a number
            //    //        worksheet.Rows[irow].Insert();
            //    //        worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
            //    //        worksheet.Rows[irow].ClearContents();
            //    //        if (sub.Equals("Labor"))
            //    //        {
            //    //            DevExpress.Spreadsheet.CellRange range4 = worksheet.Range["A" + irow + ":F" + irow];
            //    //            Formatting range4Formatting = range4.BeginUpdateFormatting();
            //    //            Borders range4Borders = range4Formatting.Borders;
            //    //            range4Borders.BottomBorder.Color = System.Drawing.Color.White;
            //    //            range4.EndUpdateFormatting(range4Formatting);
            //    //        }

            //    //        worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
            //    //        worksheet.Cells["A" + irow].Value = (string)row["Description"];

            //    //        double PriceUnit = 0;
            //    //        PriceUnit = Convert.ToDouble(row["PriceUnit"].ToString(), CultureInfo.InvariantCulture);
            //    //        //double.TryParse(row["PriceUnit"].ToString(), out PriceUnit);
            //    //        //if (PriceUnit > 0)
            //    //        worksheet.Cells["C" + irow].Value = PriceUnit;

            //    //        worksheet.Cells["F" + irow].Value = Convert.ToDouble(row["Quantity"]);
            //    //        if (row["Component"].ToString().Equals("BCDL"))
            //    //            worksheet.Cells["G" + irow].Formula = "= (F" + irow + " * C" + irow + "*$B$9 * $B$11/1000)";
            //    //        else if (row["Component"].ToString().Equals("Semi"))
            //    //            worksheet.Cells["G" + irow].Formula = "= (F" + irow + " * C" + irow + "*$B$10 * $B$11/1000)";
            //    //        else
            //    //            worksheet.Cells["G" + irow].Formula = "= F" + irow + " * C" + irow;
            //    //        //worksheet.Cells["G" + irow].Formula = string.Format("={0}",row["PriceUnit"]);
            //    //        //worksheet.Cells["H" + irow].Formula = "=G" + irow + "/$H$11";
            //    //        //if (Convert.ToInt32(result.Length) == px)
            //    //            irow++;
            //    //    }
            //    //}
            //    //if (sub.Equals("Labor")) { 
            //    //    //worksheet.Cells["G" + (irow+1)].Formula = string.Format("{0}", "=SUM(G" + fixrow + ":G" + (irow)+")");
            //    //    worksheet.Cells["G" + (irow + 2)].Formula = string.Format("{0}", "=SUM(G" + fixrow + ":G" + (irow + 1) + ")");
            //    //    worksheet.Cells["H" + (irow + 2)].Formula = string.Format("{0}", "=SUM(H" + fixrow + ":H" + (irow + 1) + ")");
            //    //    worksheet.Cells["I" + (irow + 2)].Formula = string.Format("{0}", "=SUM(I" + fixrow + ":I" + (irow + 1) + ")");
            //    //    worksheet.Cells["J" + (irow + 2)].Formula = string.Format("{0}", "=SUM(J" + fixrow + ":J" + (irow + 1) + ")");
            //    //    worksheet.Cells["K" + (irow + 2)].Formula = string.Format("{0}", "=SUM(K" + fixrow + ":K" + (irow + 1) + ")");
            //    //    worksheet.Cells["L" + (irow + 2)].Formula = string.Format("{0}", "=SUM(L" + fixrow + ":L" + (irow + 1) + ")");
            //    //}
            //}
        }
    }
    void buildsum(string id)
    {
       DataTable dtheader= cs.builditems(
            String.Format("Select * from TransTunaStd where ID= {0}", id.ToString()));
        DataRow drh = dtheader.Rows[0];
        DataTable dtstditems = cs.builditems(
            String.Format("Select * from TransTunaStdItems where RequestNo= {0}", id.ToString()));
        //int c = 4;
        int num = 1;
        for (int o = 0; o < dtstditems.Rows.Count; o++)
        {
            DataRow dr = dtstditems.Rows[o];
            worksheet = Spreadsheet.Document.Worksheets[1];
            IWorkbook workbook = Spreadsheet.Document;
            string sheetnum = string.Format("{0}_{1}", num, string.Format("{0}", dr["Material"]));
            if (o > 0)
            {
                workbook.Worksheets.Add(sheetnum);
                workbook.Worksheets[sheetnum].CopyFrom(workbook.Worksheets[1]);
                worksheet = Spreadsheet.Document.Worksheets[sheetnum];
            }
            else
            {
                worksheet = Spreadsheet.Document.Worksheets[1];
                worksheet.Name = sheetnum;
            }

            worksheet.Cells["B3"].Value = string.Format("{0}", dr["Material"]);
            worksheet.Cells["B4"].Value = string.Format("{0}", dr["Name"]);

            if (!string.IsNullOrEmpty(dr["FillWeight"].ToString())) { 
                worksheet.Cells["D7"].Value = string.Format("{0}", dr["FillWeight"]);
                worksheet.Cells["C7"].Value = string.Format("{0}", dr["Yield"]);
                worksheet.Cells["B7"].Value = string.Format("{0}", dr["PackSize"]);
            }
            else
            {
                DataTable _dtFillWeight = cs.GetFillWeight(dr["Material"].ToString());
                foreach (DataRow _rw in _dtFillWeight.Rows)
                {
                    worksheet.Cells["D7"].Value = string.Format("{0}", _rw["Result"]);
                    worksheet.Cells["C7"].Value = string.Format("{0}", Convert.ToDouble(_rw["Yield"])/100);
                    var dtPackSize = cs.builditems(@"select StdPackSize from StdOverheadCost where CanSize = substring('"+ 
                        dr["Material"].ToString()+"', 9, 3)");
                    DataRow r = dtPackSize.Select().FirstOrDefault();
                    worksheet.Cells["B7"].Value = string.Format("{0}", r["StdPackSize"]);
                    dr["PackSize"] = r["StdPackSize"];
                }
            }
            worksheet.Cells["E7"].Value = string.Format("{0}", dr["RawMaterial"]);

            worksheet.Cells["F8"].Formula = string.Format("={0}", dr["Media"]);
            worksheet.Cells["F9"].Formula = string.Format("={0}", dr["Packaging"]);
            worksheet.Cells["F10"].Formula = string.Format("={0}", dr["LOHCost"]);
            double _pkgstyle = 0;
            double.TryParse(dr["PackingStyle"].ToString(), out _pkgstyle);
            worksheet.Cells["F11"].Formula = string.Format("={0}", _pkgstyle);
            worksheet.Cells["F12"].Formula = string.Format("={0}", dr["SecPackaging"].ToString()==""?"0": dr["SecPackaging"]);
            double _Margin = 0, _Commission=0, _OverPrice=0;
            double.TryParse(dr["Margin"].ToString(), out _Margin);
            double.TryParse(dr["Commission"].ToString(), out _Commission);
            worksheet.Cells["D14"].Value = string.Format("{0}", _Margin);
            worksheet.Cells["D14"].Value = string.Format("{0}", _Margin);
            worksheet.Cells["D16"].Formula = string.Format("={0}%", _Commission);
            //worksheet.Cells["F16"].Formula = string.Format("={0}", "(F25-F19-F20)*D16");
            double.TryParse(dr["OverPrice"].ToString(), out _OverPrice);
            if (dr["OverType"].ToString() == "%")
            {
                worksheet.Cells["E17"].Value = _OverPrice.ToString();
                worksheet.Cells["F17"].Formula = string.Format("={0}", "(F25-F19-F20)*E17%");
            }else { 
                worksheet.Cells["E17"].Value = _OverPrice.ToString();
                worksheet.Cells["F17"].Formula = string.Format("={0}", _OverPrice);
            }
            string name = cs.ReadItems("select top 1 name from MasFishSpecies where sapcode = substring('" + dr["Material"] + "', 3, 2)");
            worksheet.Cells["A7"].Value = string.Format("{0}", name);
            worksheet.Cells["A10"].Value = string.Format("LOH per pack {0}", dr["PackSize"]);

            decimal _totupcharge = 0, _Quantityupch = 0, _Priceupch = 0;
            DataTable listUp = cs.builditems(string.Format("select * from TransUpCharge where RequestNo= {0} and " +
                "SubID='{1}'", id.ToString(),dr["ID"].ToString()));
            foreach (DataRow _row in listUp.Rows)
            {
                _Priceupch += Convert.ToDecimal(_row["Price"].ToString());
                _Quantityupch += Convert.ToDecimal(_row["Quantity"].ToString());
                _totupcharge += Convert.ToDecimal(_row["Result"]);
            }
            double _SubContainers = 0, sumFreight = 0;
            double Freight = 0;
            if (!string.IsNullOrEmpty(dr["SubContainers"].ToString()))
            {
                if (double.TryParse(dr["SubContainers"].ToString(), out _SubContainers))
                {
                    double.TryParse(drh["Freight"].ToString(), out Freight);
                    sumFreight = (Convert.ToDouble(Freight) / Convert.ToDouble(_SubContainers));
                    worksheet.Cells["D19"].Value = _SubContainers.ToString();
                    worksheet.Cells["E19"].Value = Freight.ToString();
                    //                    worksheet.Cells["F19"].Value = sumFreight.ToString("F");
                }
            }
            else
            {
                worksheet.Cells["F19"].Formula = string.Format("={0}", "0");
                worksheet.Cells["D19"].Formula = string.Format("={0}", "0");
                worksheet.Cells["E19"].Formula = string.Format("={0}", "0");

            }
            if (drh["Incoterm"].ToString() == "FOB")
                worksheet.Cells["F20"].Formula = string.Format("={0}", "0");
            else
                worksheet.Cells["F20"].Formula = string.Format("={0}", "F25*0.11%");
            worksheet.Cells["D21"].Formula = string.Format("={0}", drh["Interest"].ToString());

            worksheet.Cells["D13"].Value = string.Format("{0}", "");
            worksheet.Cells["E13"].Value = string.Format("{0}", "");
            worksheet.Cells["F13"].Formula = string.Format("={0}", _totupcharge);
            //Cell cell = worksheet.Cells["F13"];
            //cell.NumberFormat = "#,###.####";
            worksheet.Cells["D23"].Formula = string.Format("={0}", dr["Pacifical"]);
            worksheet.Cells["D24"].Formula = string.Format("={0}", dr["MSC"]);
            if (dr["Pacifical"].ToString() == "0")
                worksheet.Cells["F23"].Formula = string.Format("=0");
            else
                worksheet.Cells["F23"].Formula = string.Format("={0}", "F25*D23%");
            if (dr["MSC"].ToString() == "0")
                worksheet.Cells["F24"].Formula = string.Format("=0");
            else
                worksheet.Cells["F24"].Formula = string.Format("={0}", "F25*D24%");
            decimal TotalObject = 0;
                //object TotalObject = _dt.Compute(@"Sum(Result)", "(Name in ('Secondary Packaging','Media'," +
                //                            "'Primary Packaging','Packing Style','UpCharge') or (Name like 'LOH per pack%')) and Calcu=8");
            for (int i = 8; i <= 13; i++)
            {
                string cellvalue = worksheet.Cells["F" + i].Value.ToString();
                TotalObject += Convert.ToDecimal(cellvalue);
            }
            string a = worksheet.Cells["F15"].Value.ToString();
            if(a.ToString()!= "#VALUE!") { 
            object sumObject = Convert.ToDecimal(worksheet.Cells["F15"].Value.ToString());
                decimal _ResultPrice = Convert.ToDecimal(sumObject) - Convert.ToDecimal(TotalObject);
                decimal _FillWeight = Convert.ToDecimal(worksheet.Cells["D7"].Value.ToString());
                decimal _Yield = Convert.ToDecimal(worksheet.Cells["C7"].Value.ToString());
                //decimal _ResultPrice = Convert.ToDecimal(_rw["Result"]);
                //J13*1000/I14*1000*I15/I16
                worksheet.Cells["F26"].Value = Convert.ToDecimal(_ResultPrice * 1000 / _FillWeight * 1000 * _Yield / Convert.ToDecimal(dr["PackSize"].ToString()));
            }
            worksheet.Cells["F16"].Formula = string.Format("={0}", "(F25-F19-F20)*D16");
            worksheet.Cells["F21"].Formula = string.Format("={0}", "F25*D21%");
            worksheet.Cells["F24"].Formula = string.Format("={0}", "F25*D24%");
            num++;
        }
    }
    DataTable GetSecPKGCost(string value, string Customer, string ShipTo)
    {

        SqlParameter[] param = {
            new SqlParameter("@Material", value),
        new SqlParameter("@Customer", Customer),
        new SqlParameter("@ShipTo", ShipTo)};
        var Results = cs.GetRelatedResources("spStdSecPKGCost", param);

        return Results;
    }
    decimal stdGetloss(string value)
    {
        SqlParameter[] param = { new SqlParameter("@table",
           string.Format("(select * from StdLossPrimaryPKG where LossType='{0}')#a",value)) };
        var Results = cs.GetRelatedResources("usp_query", param);
        if (Results.Rows.Count == 0) return 0;
        DataRow row = Results.Rows[0];
        return Convert.ToDecimal(row["Loss1stPKG"]);
    }
    //string GetLoss2(string PackType, string id)
    //{
    //    string str = "";
    //    DataTable dt = cs.builditems(@"select Packaging,[To] from TransCostingHeader where id = '" + id + "'");
    //    foreach (DataRow rw in dt.Rows)
    //    {
    //        str = (string)cs.ReadItems(@"select Loss from maspfloss " +
    //        "where SubType='" + PackType + "' and ('" + (DateTime)rw["To"] + "' between Validfrom and Validto ) and PackageType in ('" + rw["Packaging"].ToString() + "')");
    //    }
    //    return str == "" ? "0" : str;
    //}
    DataTable _selectData(long id)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spselectrequest";
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@username", hfusername["user_name"]);
            cmd.Connection = con;
            con.Open();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
        }
        return dt;
    }
    string GetImageCheckbox(string arr,string result)
    {
        string pic = Server.MapPath("~/Content/uncheckbox16.png");
        string[] values = arr.Split(';');
        if (values.Any(result.Equals)){
            pic = Server.MapPath("~/Content/checkbox16.png");
        }
        return pic;
    }
    private void FormFillManual(string pathForPdf)
    {
        PdfDocumentProcessor doc = new PdfDocumentProcessor();
        doc.LoadDocument(pathForPdf);
        PdfFormData data = doc.GetFormData();
        //string message = "";
        foreach (string name in data.GetFieldNames())
        {
            object value = data[name].Value;
            //message += Environment.NewLine;
            //message += string.Format("name: {0},{1}", name, value);
        }
        //string path = HttpContext.Current.Server.MapPath("~/ErrorLog/ErrorLog.txt");
        //using (StreamWriter writer = new StreamWriter(path, true))
        //{
        //    writer.WriteLine(message);
        //    writer.Close();
        //}
        string KeyValue = (Request.QueryString["Id"] == null) ? "4" : Request.QueryString["Id"].ToString();
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spselectrequest";
            cmd.Parameters.AddWithValue("@Id", KeyValue);
            cmd.Parameters.AddWithValue("@username", hfusername["user_name"].ToString());
            cmd.Connection = con;
            con.Open();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            con.Dispose();
        }
        foreach (DataRow dr in dt.Rows)
        {
            string strtext = @"form1[0].#subform[0].";
            data["tbProductType"].Value = dr["ProductType"].ToString();//PRODUCT TYPE
            data[strtext + "TextField7[0]"].Value = dr["RequestNo"].ToString(); //REQUEST#
            var arr = dr["CustomerRequest"].ToString().Split(';');
            //bool has = arr.Contains(var);
            for (int i = 0; i < arr.Length; i++)
            {
                string s = arr[i];
                switch (s)
                {
                    case "0":
                        data[strtext + "CheckBox1[0]"].Value = "1";
                        break;
                    case "1":
                        data[strtext + "CheckBox2[0]"].Value = "1";
                        break;
                    case "2":
                        data[strtext + "CheckBox3[0]"].Value = "1";
                        break;
                    case "3":
                        data[strtext + "CheckBox4[0]"].Value = "1";
                        break;
                }
            }
            data[strtext + "TextField2[0]"].Value = dr["CreateOn"].ToString();
            data[strtext + "TextField3[0]"].Value = cs.GetData(dr["Requester"].ToString(), "fn");
            data["tbProposedFactory"].Value = dr["Company"].ToString();
            data["tbPetCategory"].Value = dr["PetCategory"].ToString();
            data["tbPetFoodType"].Value = dr["petfoodtype"].ToString();//petfoodtype
            data["tbCompliedWith"].Value = dr["CompliedWith"].ToString();
            data["tbRequestFor"].Value = dr["Requestfor"].ToString();
            data["tbNutrientProfile"].Value = dr["NutrientProfile"].ToString();
            data["tbProductDetail"].Value = dr["ProductNote"].ToString();//Other

            data[strtext + "TextField1[0]"].Value = dr["Customer"].ToString();
            data[strtext + "TextField1[1]"].Value = dr["Brand"].ToString();
            data[strtext + "TextField1[2]"].Value = dr["Destination"].ToString();
            data[strtext + "TextField1[3]"].Value = dr["Country"].ToString();
            data[strtext + "TextField1[4]"].Value = dr["ESTVolume"].ToString();
            data[strtext + "TextField1[5]"].Value = dr["ESTLaunching"].ToString();
            data[strtext + "TextField1[6]"].Value = dr["ESTFOB"].ToString();

            var arrVet = dr["VetOther"].ToString().Split('|');
            data[strtext + "CheckBox2[1]"].Value = string.Format("{0}", arrVet[0] == null || arrVet[0] == "" ? "Off" : "1");
            data[strtext + "TextField1[7]"].Value = string.Format("{0}", arrVet[1]);
            var arrPhy = dr["PhysicalSample"].ToString().Split('|');
            data[strtext + "CheckBox2[2]"].Value = string.Format("{0}", arrPhy[0] == null || arrPhy[0] == "" ? "Off" : "1");
            data[strtext + "TextField3[1]"].Value = string.Format("{0}", arrPhy[1]);
            data[strtext + "#field[23]"].Value = string.Format("{0}", arrPhy[2] == null || arrPhy[2] == "" ? "Off" : "1");
            data[strtext + "TextField8[0]"].Value = string.Format("{0}", arrPhy[3]);
            data["tbProductStyle"].Value = dr["ProductStyle"].ToString();
            data["tbMedia"].Value = dr["Media"].ToString();
            data["tbChunkType"].Value = dr["ChunkType"].ToString();
            var args = dr["NetWeight"].ToString().Split('|');
            data["tbNetWeight"].Value = args[0].ToString();
            data["tbUnit"].Value = args[1].ToString();
            data["tbPackaging"].Value = dr["Packaging"].ToString();
            data["tbType"].Value = dr["PackType"].ToString();
            data["tbMaterial"].Value = dr["Material"].ToString();
            data["tbDesign"].Value = dr["PackDesign"].ToString();
            data["tbLid"].Value = dr["PackLid"].ToString();
            data["tbColor"].Value = dr["PackColor"].ToString();
            data["tbLacquer"].Value = dr["PackLacquer"].ToString();
            data["tbShape"].Value = dr["PackShape"].ToString();
            var arrText = dr["Ingredient"].ToString().Split('|');

            arr = arrText[0].ToString().Split(';');
            for (int a = 0; a < arr.Length; a++)
            {
                string s = arr[a];
                switch (s)
                {
                    case "0":
                        data[strtext + "CheckBox1[1]"].Value = "1"; break;
                    case "1":
                        data[strtext + "CheckBox1[2]"].Value = "1"; break;
                    case "2":
                        data[strtext + "CheckBox1[3]"].Value = "1"; break;
                    case "3":
                        data[strtext + "CheckBox1[4]"].Value = "1"; break;
                    case "4":
                        data[strtext + "CheckBox1[5]"].Value = "1"; break;
                    case "5":
                        data[strtext + "TextField2[2]"].Value = arrText[1]; break;
                }
            }
            arrText = dr["Claims"].ToString().Split('|');
            arr = arrText[0].ToString().Split(';');
            for (int a = 0; a < arr.Length; a++)
            {
                string s = string.Format("{0}", arr[a]);
                switch (s)
                {
                    case "0":
                        data[strtext + "CheckBox1[6]"].Value = "1"; break;
                    case "1":
                        data[strtext + "CheckBox1[7]"].Value = "1"; break;
                    case "2":
                        data[strtext + "CheckBox1[8]"].Value = "1"; break;
                    case "3":
                        data[strtext + "TextField2[3]"].Value = arrText[1]; break;
                }
            }
            data[strtext + "TextField2[4]"].Value = dr["PackSize"].ToString();
            data[strtext + "TextField2[5]"].Value = dr["Drainweight"].ToString();
            data[strtext + "TextField5[0]"].Value = dr["Notes"].ToString();
        }
        doc.ApplyFormData(data);
        string testpdf = Server.MapPath("~/App_Data/Documents/_F9MKXX09_0_05.06.2015_.pdf");
        doc.SaveDocument(testpdf);
        doc.CloseDocument();
        WebClient User = new WebClient();
        Byte[] FileBuffer = User.DownloadData(testpdf);
        if (FileBuffer != null)
        {
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", FileBuffer.Length.ToString());
            Response.BinaryWrite(FileBuffer);
        }
    }
    void bodyAll(string Id, string Series)
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            if (Series == "3") {
                cmd.CommandText = "spGetQuotaCustomer";
                cmd.Parameters.AddWithValue("@requestno", Id);
                cmd.Parameters.AddWithValue("@value", "Customer");
            }
            else{ 
            cmd.CommandText = "spselectEditCosting";
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.Parameters.AddWithValue("@Series", Series);
            cmd.Parameters.AddWithValue("@username", hfusername["user_name"].ToString());}
            cmd.Connection = con;
            con.Open();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            con.Dispose();
        }
        foreach (DataRow dr in dt.Rows)
        {
            string KeyValue = Series == "3" ? dr["requestno"].ToString(): dr["ID"].ToString();
            //PrepareHeaderCells(KeyValue);
            body(KeyValue, dr);
        }
    }
    protected void ASPxSpreadsheet1_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
    {
        if (e.Parameter == "SaveAsAuthorizedTemplate")
        {
            string path = Server.MapPath("~/App_Data/Authorized Templates");
            Spreadsheet.SaveCopy(Path.Combine(path, "authorized_template.xlsx"));
        }
    }
    void build_Header(string Folio, DataRow foundRows)
    {
        DataTable dt = new DataTable();
        //string strSQL = "select a.Company,a.MarketingNumber,a.RDNumber,a.PackSize,b.NetWeight,a.PackSize,";
        //strSQL= strSQL+"b.RequestNo from TransCostingHeader a left join TransTechnical b on b.ID = a.RequestNo where a.ID = '"+ Folio + "'";
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spselectCostingReport";
            cmd.Parameters.AddWithValue("@Id", Folio);
            cmd.Parameters.AddWithValue("@username", hfusername["user_name"].ToString());
            cmd.Connection = con;
            con.Open();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            con.Dispose();
        }
        foreach (DataRow dr in dt.Rows)
        {
            Spreadsheet.Document.Options.Save.CurrentFileName = dr["MarketingNumber"].ToString() + ".xlsx";
            worksheet.Cells["B11"].Value = Convert.ToDouble(foundRows["PackSize"]);
            worksheet.Cells["H12"].Value = Convert.ToDouble(dr["ExchangeRate"]);
            worksheet.Cells["B4"].Value = (string)dr["Customer"];
            string[] array = foundRows["NetWeight"].ToString().Split('|');
            worksheet.Cells["B9"].Value = Convert.ToDouble(array[0]);
            decimal _FixedFillWeight = 0;
            decimal.TryParse(foundRows["FixedFillWeight"].ToString(), out _FixedFillWeight);
            worksheet.Cells["B10"].Value = _FixedFillWeight.ToString();

            //worksheet.Cells["A1"].Value = ((string)dr["Company"] == "102" ? "TUM 3 ,GLOBAL PET CARE PET FOOD" : "Songkla Canning Public Company Limited");
            worksheet.Cells["G4"].Value = DateTime.Now.ToString("dd-MM-yyyy");
            worksheet.Cells["G4"].Value = DateTime.Now.ToString("dd-MM-yyyy");
            worksheet.Cells["B6"].Value = Convert.ToDateTime(dr["RequestDate"]) .ToString("dd-MM-yyyy");
            worksheet.Cells["G6"].Value = Convert.ToDateTime(dr["RequireDate"]).ToString("dd-MM-yyyy");
            
            worksheet.Cells["G7"].Value = (string)dr["createby"];
            worksheet.Cells["G8"].Value = (string)dr["Requester"];
            worksheet.Cells["B7"].Value = (string)dr["CanSize"];
            worksheet.Cells["B38"].Value = (string)dr["Remark"];
            worksheet.Cells["A3"].Value = string.Format("PRODUCT COSTING SHEET ({0})", dr["UserType"].ToString().ToUpper()); 
            worksheet.Cells["G11"].Value =  dr["MarketingNumber"].ToString() + Convert.ToDouble(foundRows["formula"]).ToString("00");
        }
    }
    //void build_Header(string Folio)
    //{
    //    DataTable dt = new DataTable();
    //    //string strSQL = "select a.Company,a.MarketingNumber,a.RDNumber,a.PackSize,b.NetWeight,a.PackSize,";
    //    //strSQL= strSQL+"b.RequestNo from TransCostingHeader a left join TransTechnical b on b.ID = a.RequestNo where a.ID = '"+ Folio + "'";
    //    using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        SqlCommand cmd = new SqlCommand();
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.CommandText = "spselectCostingReport";
    //        cmd.Parameters.AddWithValue("@Id", Folio);
    //        cmd.Parameters.AddWithValue("@username", hfusername["user_name"].ToString());
    //        cmd.Connection = con;
    //        con.Open();
    //        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
    //        oAdapter.Fill(dt);
    //        con.Close();
    //        con.Dispose();
    //    }
    //    foreach (DataRow dr in dt.Rows)
    //    {
    //        Spreadsheet.Document.Options.Save.CurrentFileName = dr["MarketingNumber"].ToString() + ".xlsx";
    //        worksheet.Cells["B11"].Value = Convert.ToDouble(dr["PackSize"]);
    //        worksheet.Cells["H12"].Value = Convert.ToDouble(dr["ExchangeRate"]);
    //        worksheet.Cells["B4"].Value = (string)dr["Customer"];
    //        string[] array = dr["NetWeight"].ToString().Split('|');
    //        worksheet.Cells["B9"].Value = Convert.ToDouble(array[0]);
    //    }
    //}
    void PrepareHeaderCells(string Folio, DataRow foundRows)
    {
        DataTable dt = new DataTable();
        //string strSQL = "select a.Company,a.MarketingNumber,a.RDNumber,a.PackSize,b.NetWeight,a.PackSize,";
        //strSQL= strSQL+"b.RequestNo from TransCostingHeader a left join TransTechnical b on b.ID = a.RequestNo where a.ID = '"+ Folio + "'";
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spselectCostingReport";
            cmd.Parameters.AddWithValue("@Id", Folio);
            cmd.Parameters.AddWithValue("@username", hfusername["user_name"].ToString());
            cmd.Connection = con;
            con.Open();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            con.Dispose();
        }
        foreach (DataRow dr in dt.Rows)
        {
            Spreadsheet.Document.Options.Save.CurrentFileName = dr["MarketingNumber"].ToString() + ".xlsx";
            worksheet.Cells["A1"].Value = ((string)dr["Company"] == "102" ? "TUM 3 ,GLOBAL PET CARE PET FOOD" : "i-Tail Corporation Public Company Limited");
            worksheet.Cells["H1"].Value = ((string)dr["Company"] == "103" ? "F3ACXX26-0-06/09/21" : "F3ACXX26 -2-30/08/17");
            worksheet.Cells["G5"].Value = ((string)dr["Company"] == "102" ? "TUM, BKK" : "SCC, BKK");
            worksheet.Cells["G4"].Value = DateTime.Now.ToString("dd-MM-yyyy"); ;
            worksheet.Cells["G6"].Value = (string)dr["Requester"];
            worksheet.Cells["G7"].Value = (string)dr["createby"];
            worksheet.Cells["B7"].Value = (string)dr["CanSize"];
            worksheet.Cells["B10"].Value = Convert.ToDouble(dr["PackSize"]);//Convert.ToDouble(foundRows["PackSize"]);
            worksheet.Cells["H11"].Value = Convert.ToDouble(dr["ExchangeRate"]);
            worksheet.Cells["B38"].Value = (string)dr["Remark"];
            double.TryParse(dr["PackSize"].ToString(), out PackSize);
            //worksheet.Cells["G10"].Value = (string)dr["MarketingNumber"];
            strCompany = dr["Company"].ToString();
            //if (strCompany.ToString() == "103")
            //    worksheet.Cells["G10"].Value = dr["RDNumber"].ToString();
            //else
            //{
            //    string rd = dr["RDNumber"].ToString();
            //    int ed = rd.ToString().IndexOf(")");
            //    //int n = i + 1;int z=0;List<string> mylist= new List<string>();int x = i;
            //    int i = rd.ToString().IndexOf("(");

            //    int n = i + 1; int x = i;
            //    string[] myarr = Regex.Split(rd.ToString().Substring(n, ed - n), ",");
            //    foreach (string ar in myarr)
            //    {
            //        int index = ar.ToString().IndexOf("-"); string[] mysplit;
            //        if (index > 0)
            //        {
            //            mysplit = Regex.Split(ar.ToString(), "-");
            //            int s = Convert.ToInt32(mysplit[0]);
            //            while (s <= Convert.ToInt32(mysplit[1]))
            //            {
            //                mylist.Add(s.ToString()); ++s;
            //            }
            //        }
            //        else
            //            mylist.Add(ar);
            //    }

            //    if (i > 0)
            //        worksheet.Cells["G10"].Value = dr["RDNumber"].ToString().Substring(0, i);
            //    else
            //        worksheet.Cells["G10"].Value = dr["RDNumber"].ToString();
            //}
            worksheet.Cells["B4"].Value = (string)dr["Customer"];
            string[] array = dr["NetWeight"].ToString().Split('|');
            worksheet.Cells["B8"].Value = Convert.ToDouble(array[0]);
            p = (string)dr["Packaging"];
            costingsheet = (string)dr["MarketingNumber"];
            //worksheet.Cells["G45"].Value = (string)dr["RequireDate"] + " Shipment";
            RequireDate = (string)dr["To"] + " Shipment";
        }
    }
    public Worksheet GetWorksheetByName(Workbook workbook, string name)
    {
        return workbook.Worksheets.OfType<Worksheet>().FirstOrDefault(ws => ws.Name == name);
    }
    //List<string> myCollection = new List<string>();
    void body(string Folio, DataRow _dr)
    {
        //string Folio = (Request.QueryString["Id"] == null) ? "4" : Request.QueryString["Id"].ToString();
        dataTable = new DataTable();
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spselectformula";
            cmd.Parameters.AddWithValue("@Id", Folio);
            cmd.Connection = con;
            con.Open();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dataTable);
            con.Close();
            con.Dispose();
        }
        //DataTable dataTable;
        //string datapath = "~/XlsTables/" + Folio + ".json";
        //FileInfo sFile = new FileInfo(Server.MapPath(datapath));
        //bool fileExist = sFile.Exists;
        string strSQl = @"select *,isnull(Code,'') as 'Material' from TransFormulaHeader where Requestno='" + Folio.ToString() + "'";
        DataTable dtproduct = cs.builditems(strSQl);
        //StreamReader stRead = new StreamReader(Server.MapPath(datapath));
        //dataTable = JsonConvert.DeserializeObject<DataTable>(stRead.ReadToEnd());
        var myResult = dataTable.AsEnumerable()
        .Select(s => new {
            id = s.Field<string>("Component"),
        })
        .Distinct().ToList();
        //var itemToRemove = myResult.Single(r => r.id == "");
        //myResult.Remove(itemToRemove);
        int max = Convert.ToInt32(dataTable.AsEnumerable()
                .Max(row => row["Formula"]));
        //myCollection = new List<string>();
        //foreach (DataColumn column in dataTable.Columns)
        //    if (column.ColumnName.Contains("GUnit"))
        //        myCollection.Add(column.ColumnName);
        int wscount = Spreadsheet.Document.Worksheets.Count;
        //bool createworksheet = false;
        //if (max != wscount || max == 1)
        //{
        //    createworksheet = true;
        //}
        IWorkbook workbook = Spreadsheet.Document;
        if (Request.QueryString["view"].ToString() == "2" && wscount == 1)
        {
            Worksheet ws;
            workbook.Worksheets.Add("All");
            ws = Spreadsheet.Document.Worksheets["All"];
            ws.Cells["A1"].Value = "Product name/ description";
            ws.Cells["B1"].Value = "Code 18 digit";
            ws.Cells["C1"].Value = "Customer";
            ws.Cells["D1"].Value = "Ship To";
            ws.Cells["E1"].Value = "RD ref.no.";
            ws.Cells["F1"].Value = "Costing no.";
            ws.Cells["G1"].Value = "Offer price";

            ws.Cells["A1"].ColumnWidth = 800;
            ws.Cells["B1"].ColumnWidth = 650;
            ws.Cells["C1"].ColumnWidth = 450;
            ws.Cells["D1"].ColumnWidth = 290;
            ws.Cells["E1"].ColumnWidth = 290;

            //int _rng = dtproduct.Rows.Count + 1;
            ws.Range["A1:G1"].FillColor = System.Drawing.ColorTranslator.FromHtml("#f2faeb"); //System.Drawing.Color.GreenYellow;
            ws.Range["A1:G1"].Font.Bold = true;
            ws.Range["A1:G1"].Borders.SetOutsideBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
            DataTable _dt = cs.builditems(@"select * from TransQuotation where SubId = '" + Request.QueryString["Id"].ToString() + "'");
            int c = 1;
            foreach (DataRow _row in _dt.Rows)
            {
                //DataRow _row = _dt.Select(string.Format("SubId={0}", Folio.ToString())).FirstOrDefault();
                SqlParameter[] param = { new SqlParameter("@Id",_row["SubID"].ToString()),
                new SqlParameter("@requestno",_row["RequestNo"].ToString())};
                var myresult = cs.GetRelatedResources("spGetQuotationItems", param);
                for (int o = 1; o <= myresult.Rows.Count; o++)
                {
                    c++;
                    ws.Cells["A" + c].Value = myresult.Rows[o - 1]["Name"].ToString();
                    ws.Cells["B" + c].Value = myresult.Rows[o - 1]["Code"].ToString();
                    ws.Cells["C" + c].Value = _row["Customer"].ToString();
                    ws.Cells["D" + c].Value = _row["ShipTo"].ToString();
                    ws.Cells["E" + c].Value = myresult.Rows[o - 1]["RefSamples"].ToString();
                    ws.Cells["F" + c].Value = myresult.Rows[o - 1]["CostNo"].ToString();
                    ws.Cells["G" + c].Value = _row["Incoterm"].ToString()=="FOB" ? myresult.Rows[o - 1]["MinPrice"].ToString() : myresult.Rows[o - 1]["OfferPrice"].ToString();
                    if (myresult.Rows.Count == o)
                    {
                        int _r = c + 2; int _r2 = _r + 1;
                        ws.Range["A" + _r].Font.Bold = true;
                        ws.Cells["A" + _r].Value = "condition:";
                        ws.Cells["A" + _r2].Value = _row["Remark"].ToString();
                    }
                }
            }
            ws.Range["A1:G" + c].Borders.SetOutsideBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
        }
        //int ws = 0;
        for (int i = 1; i <= max; i++)
        {
            string cnumber = "";
            //if (createworksheet){
            var foundRows = dtproduct.Select("formula = '" + i.ToString() + "'");
            for (int s = 0; s < foundRows.Length; s++)
            {
                cnumber = foundRows[s]["Costno"].ToString();
                foreach (Worksheet sheet in workbook.Worksheets)
                {
                    if (sheet.Name.Equals(cnumber))
                        cnumber = string.Format("{0}_{1}", cnumber, s);
                }
                workbook.Worksheets.Add(cnumber);
                workbook.Worksheets[cnumber].CopyFrom(workbook.Worksheets[0]);
                worksheet = Spreadsheet.Document.Worksheets[cnumber];

                worksheet.Cells["B5"].Value = foundRows[s]["name"].ToString();
                worksheet.Cells["B6"].Value = foundRows[s]["Material"].ToString();
                worksheet.Cells["G10"].Value = foundRows[s]["RefSamples"].ToString();
                if (string.Format("{0}", foundRows[s]["PackSize"]) != "") { 
                    worksheet.Cells["B8"].Value = string.Format("{0}", foundRows[s]["NW"]);
                    worksheet.Cells["B10"].Value = string.Format("{0}", foundRows[s]["PackSize"]);
                    worksheet.Cells["B10"].Value = string.Format("{0}", foundRows[s]["PackSize"]);
                }
            }
                //cnumber = costingsheet + string.Format("{0:00}", i);
            //}
            if (cnumber == "") return;

            PrepareHeaderCells(Folio, dtproduct.Select("formula = '" + i.ToString() + "'").FirstOrDefault());
            DataRow[] result; int irow = 0; int sum = 0; string fixrow = "14";
            //Session["CustomTable"] = dataTable;Quotation template
            //DataRow[] result = dataTable.Select("Size >= 230 AND Sex = 'm'");
            decimal xsum = 0;
            string textsum = "";
            string countFishprice = dataTable.Compute("Count(Fishprice)", "Mark=0 and Formula in('" + i.ToString() + "')").ToString();
            if (countFishprice == "1") { 
            textsum = dataTable.Compute("Max(Fishprice)", "Mark=0 and Formula in('" + i.ToString() + "')").ToString();
            decimal.TryParse(textsum, out xsum);
            if (xsum != 0)
                worksheet.Cells["D11"].Formula = String.Format("={0}", xsum).ToString();
            }
            worksheet.Cells["G9"].Value = (string)cnumber;

            //worksheet.Cells["G10"].Value = strCompany == "103" ? worksheet.Cells["G10"].Value : worksheet.Cells["G10"].Value + mylist[i-1].ToString();
            result = dataTable.Select("Component = 'Raw Material' and Formula='" + i.ToString() + "'"); irow = 14;
            var ValuetoReturn = (from Rows in result.AsEnumerable()
                                 select Rows["SubType"]).Distinct().ToList();
            //worksheet.Range["A15:A24"].ClearContents();
            //worksheet.ClearContents(worksheet["A15:I24"]);
            if (ValuetoReturn.Count == 0)
            {
                worksheet.Cells["G16"].Value = "";
                worksheet.Cells["H16"].Value = "";
                worksheet.Cells["I16"].Value = "";
            }
            var name = "";
            foreach (string c in ValuetoReturn)
            {
                DataRow[] mytable = dataTable.Select("SubType='" + c + "' and Component = 'Raw Material' and Formula='" + i.ToString() + "'");
                if (!string.IsNullOrEmpty(c.ToString()))
                {
                    var w = worksheet;
                    worksheet.Rows[irow].Insert();
                    worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);

                    irow++;
                    worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                    worksheet.Cells["A" + irow].Value = c.ToString();
                }
                foreach (DataRow row in mytable)
                {
                    name = "Result";// string.Format("Gunit{0}", i);
                    if (!string.IsNullOrEmpty(row[name].ToString()))
                    {
                        double Yield = 0;
                        double.TryParse(row["Yield"].ToString(), out Yield);
                        worksheet.Rows[irow].Insert();
                        worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
                        irow++;
                        worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                        worksheet.Cells["A" + irow].Value = (string)row["Description"];
                        worksheet.Cells["B" + irow].Value = (string)row["Material"];
                        worksheet.Cells["C" + irow].Value = Convert.ToDouble( row[name]);
                        worksheet.Cells["D" + irow].Value = Yield;
                        //=C16/1000*$B$10/D16%
                        worksheet.Cells["E" + irow].Formula = "=C" + irow + "/1000*$B$10/D" + irow + "%";
                        name = "PriceOfCarton"; string.Format("Std{0}Carton", i);
                        if (row["Mark"].ToString() == "0") {//&& textsum == row["Fishprice"].ToString()
                            Cell cell = worksheet.Cells["F" + irow];
                            if (Convert.ToInt32(countFishprice) > 1){
                                textsum = dataTable.Compute(@"Max(Fishprice)", "Material='"+ row["Material"].ToString() 
                                    + "' and Mark=0 and Formula in('" + i.ToString() + "')").ToString();
                                decimal.TryParse(textsum, out xsum);
                                if (xsum != 0)
                                {
                                    worksheet.Range["J" + irow + ":K" + irow].Font.Name = "Angsana New";
                                    worksheet.Range["J" + irow + ":K" + irow].Font.Size = 14;
                                    worksheet.Range["J" + irow + ":K" + irow].Font.Bold = true;
                                    worksheet.Cells["J" + irow].Formula = String.Format("={0}", xsum).ToString();
                                    worksheet.Cells["K" + irow].Value = "USD / TON";
                                }
                                cell.Formula = "=$J$" + irow + "/1000*H11";
                            }
                            else
                                cell.Formula ="=$D$11/1000*H11";
                            //cell.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Left;
                            cell.NumberFormat = "#,###.####";
                        }
                            else
                            if (row["BaseUnit"].ToString() != "")
                            worksheet.Cells["F" + irow].Value = Convert.ToDouble(row["BaseUnit"]);
                        worksheet.Cells["G" + irow].Formula = "=F" + irow + "*E" + irow;
                        worksheet.Cells["H" + irow].Formula = "=G" + irow + "/$H$11";
                        //worksheet.Cells["I" + irow].Formula = "=G" + irow + "/$G$31";
                    }
                }
                sum = irow + 2;
                worksheet.Cells["G" + sum].Formula = "=SUM(G15:G" + irow + ")";
                worksheet.Cells["H" + sum].Formula = "=SUM(H15:H" + irow + ")";
            }
            //worksheet.Cells["G25"].Formula = "= SUM(G15:G24)";
            //worksheet.Cells["H25"].Formula = "=SUM(H15:H24)";
            //worksheet.Cells["I25"].Formula = "=G25/$G$78";
            //worksheet.ClearContents(worksheet["A27:I32"]);
            irow = irow + 3; double LBRate = 0; double BaseUnit;
            int fsum = irow;
            DataTable dt = new DataTable();
            dt = cs.GetMergeDataSource(i, "2", Folio);
            foreach (DataRow row in dt.Rows)
            {
                worksheet.Rows[irow].Insert();
                worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
                irow++;
                worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                if (row["userlevel"].ToString()=="0")
                {

                            worksheet.Cells["A" + irow].Value = (string)row["SubType"];
                            double.TryParse(row["PriceOfCarton"].ToString(), out BaseUnit);
                            //Std1Carton += BaseUnit;
                            worksheet.Cells["G" + irow].Formula = string.Format("={0}", BaseUnit.ToString());
       
                        }else
 
                        {
                            worksheet.Cells["A" + irow].Value = (string)row["Description"];
                            worksheet.Cells["B" + irow].Value = (string)row["Material"];
                            worksheet.Cells["C" + irow].Value = Convert.ToDouble(row["Result"]);
                            double Yield = 0;
                            double.TryParse(row["Yield"].ToString(), out Yield);
                            if (Yield > 0)
                            {
                                worksheet.Cells["D" + irow].Value = Yield;
                                worksheet.Cells["E" + irow].Formula = "=C" + irow + "/1000*$B$10/D" + irow + "%";
                            }
                            if(row["BaseUnit"].ToString()!="")
                            worksheet.Cells["F" + irow].Value = Convert.ToDouble(row["BaseUnit"]);
                            if (string.IsNullOrEmpty(row["PriceOfCarton"].ToString()))
                                worksheet.Cells["F" + irow].Value = Convert.ToDouble(row["PriceOfCarton"]);
                            else
                                worksheet.Cells["G" + irow].Formula = string.Format("={0}", ("E" + irow + "* F" + irow).ToString());
       
                }

                worksheet.Cells["H" + irow].Formula = "=G" + irow + "/$H$11";
            }
            sum = irow + 2;
            if (dt.Rows.Count == 0)
            {
                worksheet.Cells["G" + sum].Value = "";
                worksheet.Cells["H" + sum].Value = "";
                worksheet.Cells["I" + sum].Value = "";
            }
            else
            {
                worksheet.Cells["G" + sum].Formula = "=SUM(G" + fsum + ":G" + irow + ")";
                worksheet.Cells["H" + sum].Formula = "=SUM(H" + fsum + ":H" + irow + ")";
            }
            //worksheet.Cells["G"+sum].Formula = "=SUM(G"+fsum+":G"+irow+")";
            //worksheet.Cells["H"+sum].Formula = "=SUM(H"+fsum+":H"+irow+")";
            string strSQL = "select * from TransCosting Where RequestNo = '" + Folio + "'";
            dt = cs.builditems(strSQL);
            result = dt.Select("Component = 'Primary Packaging' and Formula='" + i.ToString() + "'");
            fixrow = string.Format("{0}", irow + 3);
            irow += 3; fsum = irow;
            //worksheet.ClearContents(worksheet["A" + irow + ":I57"]);
            foreach (DataRow row in result)
            {
                worksheet.Rows[irow].Insert();
                worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
                irow++;
                worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                worksheet.Range["G" + irow + ":I" + irow].Borders.SetOutsideBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
                worksheet.Cells["A" + irow].Value = (string)row["Description"];
                worksheet.Cells["B" + irow].Value = (string)row["SAPMaterial"];
                worksheet.Cells["E" + irow].Value = Convert.ToDouble(row["Quantity"]);
                double PriceUnit = 0;
                double.TryParse(row["PriceUnit"].ToString(), out PriceUnit);
                if (PriceUnit > 0)
                    worksheet.Cells["F" + irow].Value =  PriceUnit;
                worksheet.Cells["G" + irow].Formula = string.Format("{0}", "=E" + irow + " * F" + irow);
                worksheet.Cells["H" + irow].Formula = string.Format("{0}", "=G" + irow + "/$H$11");
            }
            sum = irow + 2;
            worksheet.Cells["G" + sum].Formula = "=SUM(G" + fsum + ":G" + irow + ")";
            worksheet.Cells["H" + sum].Formula = "=SUM(H" + fsum + ":H" + irow + ")";
            result = dt.Select("Component = 'Secondary Packaging' and Formula='" + i.ToString() + "'");
            irow += 3; fsum = irow;
            //worksheet.ClearContents(worksheet["A" + irow + ":I63"]);
            foreach (DataRow row in result)
            {
                worksheet.Rows[irow].Insert();
                worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
                irow++;
                worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                worksheet.Range["G" + irow + ":I" + irow].Borders.SetOutsideBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
                worksheet.Cells["A" + irow].Value = (string)row["Description"];
                worksheet.Cells["B" + irow].Value = (string)row["SAPMaterial"];
                worksheet.Cells["E" + irow].Value = Convert.ToDouble(row["Quantity"]);
                double PriceUnit = 0;
                double.TryParse(row["PriceUnit"].ToString(), out PriceUnit);
                if (PriceUnit > 0)
                    worksheet.Cells["F" + irow].Value = PriceUnit;
                worksheet.Cells["G" + irow].Formula = string.Format("{0}", "=E" + irow + " * F" + irow);
                worksheet.Cells["H" + irow].Formula = string.Format("{0}", "=G" + irow + "/$H$11");
            }
            sum = irow + 2;
            worksheet.Cells["G" + sum].Formula = "=SUM(G" + fsum + ":G" + irow + ")";
            worksheet.Cells["H" + sum].Formula = "=SUM(H" + fsum + ":H" + irow + ")";

            DataTable table = cs.GetMergeDataSource(i, "3", Folio);//Labor & Overhead
            irow += 3; fsum = irow;
            int fix_rate = irow;
            foreach (DataRow row in table.Rows)
            {
                worksheet.Rows[irow].Insert();
                worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
                irow++;
                worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                worksheet.Cells["G" + irow].NumberFormat = "#,#";
                worksheet.Range["G" + irow + ":I" + irow].Borders.SetOutsideBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
                //worksheet.Cells["F" + irow].Value = PackSize.ToString();
                double.TryParse(row["LBRate"].ToString(), out LBRate);
                //                switch (row["userlevel"].ToString())
                //                {
                //                    case "0":

                if (row["Title"].ToString() == "Labor & Overhead") { 
                    fix_rate = irow;
                }
                worksheet.Cells["A" + irow].Value = string.Format("{0}", row["Title"].ToString());
                if (row["SellingUnit"].ToString() == "%") {
 
                    worksheet.Cells["F" + irow].NumberFormat = "###,##%";
                    worksheet.Cells["F" + irow].Formula = string.Format("={0}%", LBRate);
                    worksheet.Cells["G" + irow].Formula = "= G" + fix_rate + " * F" + irow; 
                }
                else{
                    worksheet.Cells["G" + irow].Formula = LBRate.ToString();//"= F" + irow + " * C" + irow;
                }
                worksheet.Cells["H" + irow].Formula = "=G" + irow + "/$H$11";
                //                        break;
                //                    case "1":
                //                        worksheet.Cells["A" + irow].Value = (string)row["LBName"];
                //                        worksheet.Cells["C" + irow].Value = LBRate / PackSize;
                //                        //worksheet.Cells["G" + irow].Formula = string.Format("={0}", string.Format("{0}", row["LBRate"].ToString()));
                //                        worksheet.Cells["G" + irow].Formula = "= F" + irow + " * C" + irow;
                //                        worksheet.Cells["H" + irow].Formula = "=G" + irow + "/$H$11";
                //                        break;
                //                }
            }
            //result = dt.Select("Component = 'Labor & Overhead' and Formula='" + i.ToString() + "'");
            //irow = irow + 3; fsum = irow;
            ////worksheet.ClearContents(worksheet["A" + irow + ":I67"]);
            //foreach (DataRow row in result)
            //{
            //    double.TryParse(row["Amount"].ToString(), out BaseUnit);
            //    Std1Carton += BaseUnit;
            //}
            //worksheet.Rows[irow].Insert();
            //worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
            //irow++;
            //worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
            //worksheet.Cells["A" + irow].Value = string.Format("{0}", "LABOUR & OVERHEAD");
            //worksheet.Cells["G" + irow].Formula = string.Format("={0}", Std1Carton);
            //worksheet.Cells["G" + irow].NumberFormat = "#,#";
            //worksheet.Cells["H" + irow].Formula = "=G" + irow + "/$H$11";
            //worksheet.Range["G" + irow + ":I" + irow].Borders.SetOutsideBorders(System.Drawing.Color.Black, BorderLineStyle.Thin);
            sum = irow + 2;
            worksheet.Cells["G" + sum].Formula = "=SUM(G" + fsum + ":G" + irow + ")";
            worksheet.Cells["H" + sum].Formula = "=SUM(H" + fsum + ":H" + irow + ")";

            result = dt.Select("Component = 'Upcharge' and Formula='" + i.ToString() + "'");
            irow += 3; fsum = irow;
            //worksheet.ClearContents(worksheet["A" + irow + ":I71"]);
            foreach (DataRow row in result)
            {
				double d=0;
                if (Double.TryParse(row["Quantity"].ToString(), out d)){ // if done, then is a number
                worksheet.Rows[irow].Insert();
                worksheet.Rows[irow].CopyFrom(worksheet.Rows[fixrow]);
                irow++;
                worksheet.Range["A" + irow + ":B" + irow].FillColor = System.Drawing.Color.White;
                worksheet.Cells["A" + irow].Value = (string)row["Description"];
                double PriceUnit = 0;
                    PriceUnit = Convert.ToDouble(row["PriceUnit"].ToString(), CultureInfo.InvariantCulture);
                    //double.TryParse(row["PriceUnit"].ToString(), out PriceUnit);
                    //if (PriceUnit > 0)
                    worksheet.Cells["C" + irow].Value = PriceUnit;
                    
                    worksheet.Cells["F" + irow].Value = Convert.ToDouble( row["Quantity"]);
                worksheet.Cells["G" + irow].Formula = "= F" + irow + " * C" + irow;
                //worksheet.Cells["G" + irow].Formula = string.Format("={0}",row["PriceUnit"]);
                worksheet.Cells["H" + irow].Formula = "=G" + irow + "/$H$11";
				}
            }
            sum = irow + 2;
            worksheet.Cells["G" + sum].Formula = "= SUM(G" + fsum + ":G" + irow + ")";
            worksheet.Cells["H" + sum].Formula = "=SUM(H" + fsum + ":H" + irow + ")";
            //loss
            irow = irow + 4;
            DataTable Myresult = cs.GettableLoss(p, Convert.ToDateTime(_dr["To"]), _dr["UserType"].ToString());
            foreach (DataRow row in Myresult.Rows)
            {
                //worksheet.Cells["B" + irow].NumberFormat = "###,##%";
                worksheet.Cells["B" + irow].Formula = string.Format("{0:N2}%", row["Loss"]);

                irow++;
            }
            result = dt.Select("Component = 'Margin' and Formula='" + i.ToString() + "'");
            //irow = irow + 6;
            foreach (DataRow row in result)
            {
                worksheet.Cells["A" + irow].Value = string.Format("{0}", "Margin");
                //worksheet.Cells["B" + irow].NumberFormat = "###,##%";
                worksheet.Cells["B" + irow].Formula = string.Format("{0:N2}%", row["Per"]);

                //worksheet.Cells["B" + irow].Value = string.Format("{0:N}", row["Per"]);
                worksheet.Cells["G" + irow].Formula = "=G" + (irow + 1) + "*B" + irow;
                worksheet.Cells["H" + irow].Formula = "=G" + irow + "/$H$11";
            }
            worksheet.Cells["G" + (irow + 1)].Formula = "=SUM(G" + (irow - 4) + ":G" + irow + ")";
            worksheet.Cells["H" + (irow + 1)].Formula = "=SUM(H" + (irow - 4) + ":H" + irow + ")";
            //ws++;
            worksheet.Cells["G" + (irow + 9)].Value = RequireDate.ToString();
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
    //protected void Page_PreRender(object sender, EventArgs e)
    //{
    //    HtmlMeta meta = new HtmlMeta();
    //    meta.HttpEquiv = "X-UA-Compatible";
    //    meta.Content = "IE=8";
    //    Page.Header.Controls.AddAt(0, meta);
    //}
    //protected void ReportViewer1_Unload(object sender, EventArgs e)
    //{
    //    ((ReportViewer)sender).Report = null;
    //}
}