using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Services;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Mail;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using ClosedXML.Excel;
using System.Web;
using DevExpress.Web.ASPxSpreadsheet;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using CsvHelper;
using System.Collections.Generic;

/// <summary>
/// Summary description for ServiceCS
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class ServiceCS : System.Web.Services.WebService
{
    private readonly string ConnLab = ConfigurationManager.ConnectionStrings["LabConnectionString"].ConnectionString;
    private readonly string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    private readonly string CurUserName = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    private readonly string filepatch = @"C:\Users\wshuttleadm\Documents\Winshuttle\Studio";
    private readonly MyDataModule cs = new MyDataModule();
    private readonly ASPxSpreadsheet Spreadsheet = new ASPxSpreadsheet();
    private Worksheet worksheet;

    public ServiceCS()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }
    

    void GetLabResources(string StoredProcedure, object[] Parameters)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConnLab))
            {
                using (SqlCommand cmd = new SqlCommand(StoredProcedure, conn))
                {
                    conn.Open();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(Parameters);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
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
    
    [WebMethod]
    public void XX(string d)
    {
        foreach (int i in cs.Integers())
        {
            Context.Response.Write(i.ToString());
        }
        string ii =  "10_";
        Context.Response.Write(ii + d);
    }
    [WebMethod]
    public void GetData()
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select * from ulogin";
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            Context.Response.Write(JsonConvert.SerializeObject(dt));
        }
    }
    public byte[] ReadFile(string sPath)
    {
        //Initialize byte array with a null value initially.
        byte[] data = null;

        //Use FileInfo object to get file size.
        FileInfo fInfo = new FileInfo(sPath);
        long numBytes = fInfo.Length;

        //Open FileStream to read file
        FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);

        //Use BinaryReader to read file stream into byte array.
        BinaryReader br = new BinaryReader(fStream);

        //When you use BinaryReader, you need to supply number of bytes to read from file.
        //In this case we want to read entire file. So supplying total number of bytes.
        data = br.ReadBytes((int)numBytes);

        //Close BinaryReader
        br.Close();

        //Close FileStream
        fStream.Close();

        return data;
    }
    

    [WebMethod]
    public void jobalertemail()
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spalertemail";
            cmd.Connection = con;
            con.Open();
            //cmd.ExecuteNonQuery();
            var getValue = cmd.ExecuteScalar();
            if(getValue.ToString()=="0")
            {
                MailMessage msg = new MailMessage();
                msg.To.Add(new MailAddress("voravut.somboornpong@thaiunion.com"));
                msg.From = new MailAddress("wshuttleadm@thaiunion.com");
                msg.Subject = "job server fail";
                msg.Body = "X";
                msg.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("wshuttleadm@thaiunion.com", "WSP@ss2018");
                client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
                client.Host = "smtp.office365.com";
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Send(msg);
            }
            con.Close();
        }
        Context.Response.Write("success");
    }
    string GetLoss(string PackType, string i){
        string str = string.Format("{0}", cs.ReadItems(@"select b.Loss from TransCostingHeader a left join maspfloss b 
        on a.Packaging = b.PackageType and (a.[To] between Validfrom and Validto ) " +
        "where b.SubType='" + PackType + "' and a.id='" + i + "'"));
        return str == "" ? "0" : str;
    }
	[WebMethod]
	public void GetCostData(string ddd){
        SqlParameter[] param = { new SqlParameter("Keyword", 'Z'),
             new SqlParameter("UserNo", CurUserName.ToString())};
        var MyTable = cs.GetRelatedResources("spSummaryCosting", param);
        MyTable = PostDataSap(MyTable);
        var values = new[] { "Incoterm", "SoldTo", "ShipTo", "PaymentTerm", "Currency", "SalesUnit", "MinPrice","Code", "subID","OfferPrice", "PricingUnit" };
            for (int i = 0; i <= values.Length - 1; i++)
            {
                MyTable.Columns.Add(values[i], typeof(System.String));
            }
        foreach(DataRow row in MyTable.Rows)
        {
            row["Incoterm"] = "FOB";
            double MinPrice = Convert.ToDouble(row["FOB"].ToString())/ Convert.ToDouble(row["ExchangeRate"].ToString());
            row["MinPrice"] = MinPrice.ToString("F");
            row["Code"] = row["MaterialCode"].ToString();
            row["Currency"] = "USD".ToString();
            row["SalesUnit"] = "CAR";
            row["subID"] = 0;
            row["OfferPrice"] = "";
            row["PricingUnit"] = 1;
        }
        CreateToExport(MyTable);
        Context.Response.Write("success");
    }
    [WebMethod]
    public DataTable PostDataSap(DataTable MyTable)
    {
        //SqlCommand sqlComm = new SqlCommand();
        //sqlComm.CommandText = @"update TransQuotationItems set IsActive=4 WHERE ID=@ID";
        //sqlComm.Parameters.AddWithValue("@ID", Data.ToString());
        //DataTable dt = GetDataDb(sqlComm);
        //Context.Response.Write(JsonConvert.SerializeObject(dt));
        string[] SubType = Regex.Split("Raw Material & Ingredient;Primary Packaging;Secondary Packaging", ";");
        string[] valueType = Regex.Split("RmIng;PrimaryPkg;SecondaryPkg", ";");
        foreach (DataRow rw in MyTable.Rows)
        {
            decimal _rm, _totalrm;
            for (int i = 0; i <= valueType.Length - 1; i++)
            {
                _rm = Convert.ToDecimal(rw[valueType[i]]);
                _totalrm = _rm * (Convert.ToDecimal(GetLoss(SubType[i], rw["RequestNo"].ToString())) / 100);
                rw["Loss"] = Convert.ToDecimal(rw["Loss"]) + _totalrm;
                rw["FOB"] = Convert.ToDecimal(rw["FOB"]) + _totalrm + Convert.ToDecimal(rw[valueType[i]]);
            }
            //string _LOHvalue = _LOH(rw["Formula"].ToString(),rw["RequestNo"].ToString());
            string _LOHvalue = rw["LOH"].ToString();
            decimal totalCount = Convert.ToDecimal(rw["FOB"]) + Convert.ToDecimal(_LOHvalue) + Convert.ToDecimal(rw["UpCharge"]);
            object sumObject = rw["PerMargin"].ToString();
            decimal _FOB = (totalCount / ((100 - Convert.ToDecimal(sumObject)) / 100));
            rw["Margin"] = _FOB * (Convert.ToDecimal(sumObject) / 100);
            //rw["FOB"] = Convert.ToDecimal(rw["FOB"]) + Convert.ToDecimal(rw["Margin"]);
            List<decimal> list1 = new List<decimal>() { Convert.ToDecimal(rw["Ing"]),
                    Convert.ToDecimal(rw["RM"]),
                    Convert.ToDecimal(rw["PrimaryPkg"]),
                    Convert.ToDecimal(rw["SecondaryPkg"]), Convert.ToDecimal(_LOHvalue),
                    Convert.ToDecimal(rw["UpCharge"]),
                    Convert.ToDecimal(rw["Loss"]),
                    Convert.ToDecimal(rw["Margin"])};
            rw["FOB"] = list1.Sum();
            rw["Loss"] = Convert.ToDecimal(rw["Loss"]).ToString("F2");
            rw["FOB"] = Convert.ToDecimal(rw["FOB"]).ToString("F2");
            rw["Margin"] = Convert.ToDecimal(rw["Margin"]).ToString("F2");
            rw["Ing"] = Convert.ToDecimal(rw["Ing"]).ToString("F2");
            rw["Fishprice"] = String.Format("{0:#,##0.##}", Convert.ToDecimal(rw["Fishprice"]));
            rw["RM"] = String.Format("{0:#,##0.##}", Convert.ToDecimal(rw["RM"]));
            rw["SecondaryPkg"] = Convert.ToDecimal(rw["SecondaryPkg"]).ToString("F2");
            rw["PrimaryPkg"] = Convert.ToDecimal(rw["PrimaryPkg"]).ToString("F2");
            rw["FOBUsd"] = Convert.ToDecimal(Convert.ToDecimal(rw["FOB"])/ Convert.ToDecimal(rw["ExchangeRate"])).ToString("F2");
            rw["LOH"] = Convert.ToDecimal(_LOHvalue.ToString());
        }
        return MyTable;
    }
    //public string _LOH(string _Formula,string _ID)
    //{
    //using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        SqlCommand cmd = new SqlCommand();
    //        cmd.CommandText = "select dbo.fnc_GetLOHDL(@Formula, @RequestNo) as 'value'";
    //        cmd.Parameters.AddWithValue("@Formula", _Formula);
    //        cmd.Parameters.AddWithValue("@RequestNo", _ID);
    //        cmd.Connection = con;
    //        con.Open();
    //        var getValue = cmd.ExecuteScalar();
    //        con.Close();
    //        return string.Format("{0}", getValue);
    //    }
    //}
    [WebMethod]
    public DataSet GetQuery(string Data)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spGetDataSendToSAP";
            cmd.Parameters.AddWithValue("@user", CurUserName);
            cmd.Connection = con;
            con.Open();
            DataSet oDataset = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(oDataset);
            con.Close();
            return oDataset;
        }
    }
    [WebMethod]
    public void GetUnblockSO()
    {
        DataTable dtsale = builditems(@"select VBAP_VBELN from TransSalesDeliveryBlock group by VBAP_VBELN");
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[] { new DataColumn(@"Sales Document VBAK-VBELN"),
            new DataColumn(@"Delivery Block (Document Header)") }
        );
        foreach (DataRow rw in dtsale.Rows)
        {
            dt.Rows.Add(string.Format("{0}", rw["VBAP_VBELN"]),"Z4");
        }
        string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/VA02_UBLKSO_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv");
        MyToDataTable.ToCSV(dt, file);
    }
    [WebMethod]
    public void ExportTo(string Data)
    {
        //worksheet.Cell("D2").Value = "zpm1";
        //worksheet.Cell("F2").Value = "X";
        //worksheet.Cell("G2").Value = "103";
        //worksheet.Cell("H2").Value = "ex";
        //worksheet.Cell("I2").Value = "203";
        //for (int i = 3; i < 4; i++) {
        //worksheet.Range("C2:I2").CopyTo(worksheet.Range("C2:I2".Replace("2",i.ToString()))); 
        //worksheet.Cell("F"+i).Value = "X";
        //}

        var Results = new DataTable();//spGetHistory
        SqlParameter[] param = { new SqlParameter("@user", string.Format("{0}", CurUserName)) };
        Results = cs.GetRelatedResources("spGetDataSendToSAP", param);
        //CreateToExport(Results);
        
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[] { new DataColumn("IfColumn"), 
            new DataColumn(@"Condition Type RV13A-KSCHL"),
            new DataColumn(@"Sales Organization KOMG-VKORG"),
            new DataColumn(@"Distribution Channel KOMG-VTWEG"),
            new DataColumn(@"Sales group KOMG-VKGRP"),

            new DataColumn(@"Customer number KOMG-KUNNR"),
            new DataColumn(@"Ship-To Party KOMG-KUNWE"),
            new DataColumn(@"Incoterms (Part 1)KOMG-INCO1(01)"),
            new DataColumn(@"Terms of payment key KOMG-ZTERM(01)"),
            new DataColumn(@"SD Document Currency KOMG-WAERK(01)"),

            new DataColumn(@"Sales unit KOMG-VRKME(01)"),
            new DataColumn(@"Material Number KOMG-MATNR(01)"),
            new DataColumn(@"Condition amount or percentage where no scale exists KONP-KBETR(01)"),

            new DataColumn(@"Condition unit (currency or percentage) KONP-KONWA(01)"),
            new DataColumn(@"Condition Pricing Unit KONP-KPEIN(01)"),
            new DataColumn(@"Condition Unit KONP-KMEIN(01)"),
            new DataColumn(@"Validity start date of the condition record RV13A-DATAB(01)"),
            new DataColumn(@"Validity end date of the condition record RV13A-DATBI(01)"),
            new DataColumn(@"Long text line LV70T-LTX01(01)"),
            new DataColumn(@"AppID"),
            });
        foreach (DataRow row in Results.Rows)
        {
            string IfColumn = "";
            if (row["ShipTo"].ToString() != "" && row["Incoterm"].ToString() != "" && row["PaymentTerm"].ToString() != "")
                IfColumn = "11";
            else if (row["ShipTo"].ToString() != "" && row["Incoterm"].ToString() != "")
                IfColumn = "12";
            else if (row["PaymentTerm"].ToString() != "" && row["Incoterm"].ToString() != "")
                IfColumn = "14";
            else if (row["SoldTo"].ToString() != "" && row["Incoterm"].ToString() != "")
                IfColumn = "15";
            else if (row["SoldTo"].ToString() != "" && row["PaymentTerm"].ToString() != "")
                IfColumn = "16";
            else if (row["Code"].ToString() != "" && row["Incoterm"].ToString() != "")
                IfColumn = "17";

            dt.Rows.Add(string.Format("{0}", IfColumn),
            IfColumn == "17" ? @"zpm2" : @"zpm1", 
            string.Format("{0}", row["CostNo"].ToString().Substring(0, 2) == "PF" ? "103" : "102"),
            "EX", 
            "203", 
            row["SoldTo"].ToString(),
            row["ShipTo"].ToString(),
            row["Incoterm"].ToString(),
            row["PaymentTerm"].ToString(),

            row["Currency"].ToString(),
            row["SalesUnit"].ToString(),
            row["Code"].ToString(),
            row["OfferPrice"].ToString(),

            row["Currency"].ToString(),
            row["PricingUnit"].ToString(),
            row["SalesUnit"].ToString(),
            string.Format("{0}", row["RequestDate"].ToString()),
            string.Format("{0}", row["RequireDate"].ToString()),
            row["CostNo"].ToString(),
            row["ID"].ToString());

        }
        string[] ColumnsToBeDeleted = { "11", "12", "13", "14", "15","16","17","18" };
        if (dt.Rows.Count > 0)
            foreach (string ColName in ColumnsToBeDeleted)
            {
                var dtclone = new DataTable();

                //dtclone.Clear();
                //dtclone = dt.Clone();
                if (dt.Select("IfColumn='" + ColName + "'").Length > 0)
                {
                    if (ColName == "11")
                    {
                        dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
                    }
                    else if (ColName == "12")
                    {
                        dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
                        dtclone.Columns.Remove(@"Terms of payment key KOMG-ZTERM(01)");
                    }
                    else if (ColName == "13")
                    {
                        dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
                        dtclone.Columns.Remove(@"Incoterms (Part 1)KOMG-INCO1(01)");
                    }
                    else if (ColName == "14")
                    {
                        dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
                        dtclone.Columns.Remove(@"Ship-To Party KOMG-KUNWE");
                    }
                    else if (ColName == "15")
                    {
                        dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
                        dtclone.Columns.Remove(@"Ship-To Party KOMG-KUNWE");
                        dtclone.Columns.Remove(@"Terms of payment key KOMG-ZTERM(01)");
                    }
                    else if (ColName == "16")
                    {
                        dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
                        dtclone.Columns.Remove(@"Ship-To Party KOMG-KUNWE");
                        dtclone.Columns.Remove(@"Incoterms (Part 1)KOMG-INCO1(01)");
                    }
                    else if (ColName == "17")
                    {
                        dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
                        dtclone.Columns.Remove(@"Ship-To Party KOMG-KUNWE");
                        dtclone.Columns.Remove(@"Incoterms (Part 1)KOMG-INCO1(01)");
                        dtclone.Columns.Remove(@"Customer number KOMG-KUNNR");
                        dtclone.Columns.Remove(@"Terms of payment key KOMG-ZTERM(01)");
                    }
                    string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/VK11_"  + ColName + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv");
                    MyToDataTable.ToCSV(dtclone, file);
                }
            }
        
        //MinPrice(Results);
        //Server.MapPath("~/App_Data/Documents/HelloWorld.xlsx"));
        //Context.Response.Write(JsonConvert.SerializeObject(Results));
        Context.Response.Write("success");
    }
    public void CreateToExport(DataTable Results)
    {
        int i = 1;
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("VK11_20180604_095920_");
        foreach (DataRow row in Results.Rows)
        {
            //if (!row["Currency"].ToString().Contains("USD") && row["PriceUpload"].ToString() == "0")
            //{
            //    double _ExchangeRate = 0, _OfferPrice = 0, _MinPrice = 0;
            //    _OfferPrice = (Convert.ToDouble(row["OfferPrice"]) * Convert.ToDouble(row["Rate"]));
            //    _MinPrice = (Convert.ToDouble(row["MinPrice"]) * Convert.ToDouble(row["Rate"]));
            //    double.TryParse(string.Format("{0}", row["ExchangeRate"].ToString()), out _ExchangeRate);
            //    if (_ExchangeRate != 0)
            //    {
            //        row["OfferPriceExch"] = Convert.ToDouble(_OfferPrice / _ExchangeRate).ToString("F2");
            //        row["MinPriceExch"] = Convert.ToDouble(_MinPrice / _ExchangeRate).ToString("F2");
            //    }
            //    if (row["Currency"].ToString().Contains("JPY"))
            //    {
            //        NumberFormatInfo nfi = new CultureInfo("ja-JP", false).NumberFormat;
            //        row["OfferPriceExch"] = Convert.ToInt64(Math.Floor(Convert.ToDouble(row["OfferPriceExch"])));
            //        row["MinPriceExch"] = Convert.ToInt64(Math.Floor(Convert.ToDouble(row["MinPriceExch"])));
            //    }
            //    //dr["ExchangeRate"] = seExchangeRate.Value;
            //}
            //else
            //{
            //    row["OfferPriceExch"] = row["OfferPrice"];
            //    row["MinPriceExch"] = row["MinPrice"];
            //}
            //if (row["Incoterm"].ToString() == "FOB" && row["MinPrice"].ToString()== ".00")
            //    row["MinPrice"] = cs.GetMinPrice(row["Formula"].ToString(), row["RequestNo"].ToString());
            if (row["ShipTo"].ToString() != "" && row["Incoterm"].ToString() != "" && row["PaymentTerm"].ToString() != "")
            {
                i++;
                worksheet.Cell("A" + i).Value = string.Format("{0}", 11);
                worksheet.Cell("B" + i).Value = row["subID"].ToString();
                worksheet.Cell("C" + i).Value = row["ID"].ToString();
                worksheet.Cell("D" + i).Value = "zpm1";
                worksheet.Cell("F" + i).Value = "X";
                worksheet.Cell("G" + i).Value = string.Format("{0}", row["CostNo"].ToString().Substring(0, 2) == "PF" ? "103" : "102");
                worksheet.Cell("H" + i).Value = "EX";
                worksheet.Cell("I" + i).Value = "203";
                worksheet.Cell("J" + i).Value = row["SoldTo"].ToString();
                worksheet.Cell("K" + i).Value = row["ShipTo"].ToString();
                worksheet.Cell("L" + i).Value = row["Incoterm"].ToString();
                worksheet.Cell("M" + i).Value = row["PaymentTerm"].ToString();
                worksheet.Cell("N" + i).Value = row["Currency"].ToString();
                worksheet.Cell("O" + i).Value = row["SalesUnit"].ToString();
                worksheet.Cell("P" + i).Value = row["Code"].ToString();
                worksheet.Cell("Q" + i).Value = row["OfferPriceExch"].ToString();
                worksheet.Cell("R" + i).Value = row["Currency"].ToString();
                worksheet.Cell("S" + i).Value = row["PricingUnit"].ToString();
                worksheet.Cell("T" + i).Value = row["SalesUnit"].ToString();
                //DateTime dt = DateTime.ParseExact(row["RequestDate"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //string s = dt.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
                //worksheet.Cell("U" + i).Value = string.Format("'{0}", s);
                //worksheet.Cell("V" + i).Value = string.Format("'{0}", Convert.ToDateTime(row["RequireDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                worksheet.Cell("U" + i).Value = string.Format("{0}", row["RequestDate"].ToString());
                worksheet.Cell("V" + i).Value = string.Format("{0}", row["RequireDate"].ToString());
                worksheet.Cell("W" + i).Value = row["CostNo"].ToString();
            }
            else if (row["ShipTo"].ToString() != "" && row["Incoterm"].ToString() != "")
            {
                i++;
                worksheet.Cell("A" + i).Value = string.Format("{0}", 12);
                worksheet.Cell("B" + i).Value = row["subID"].ToString();
                worksheet.Cell("C" + i).Value = row["ID"].ToString();
                worksheet.Cell("D" + i).Value = "zpm1";
                worksheet.Cell("F" + i).Value = "X";
                worksheet.Cell("G" + i).Value = string.Format("{0}", row["CostNo"].ToString().Substring(0, 2) == "PF" ? "103" : "102");
                worksheet.Cell("H" + i).Value = "EX";
                worksheet.Cell("I" + i).Value = "203";
                worksheet.Cell("J" + i).Value = row["SoldTo"].ToString();
                worksheet.Cell("K" + i).Value = row["ShipTo"].ToString();
                worksheet.Cell("L" + i).Value = row["Incoterm"].ToString();
                worksheet.Cell("N" + i).Value = row["Currency"].ToString();
                worksheet.Cell("O" + i).Value = row["SalesUnit"].ToString();
                worksheet.Cell("P" + i).Value = row["Code"].ToString();
                worksheet.Cell("Q" + i).Value = row["OfferPriceExch"].ToString();
                worksheet.Cell("R" + i).Value = row["Currency"].ToString();
                worksheet.Cell("S" + i).Value = row["PricingUnit"].ToString();
                worksheet.Cell("T" + i).Value = row["SalesUnit"].ToString();
                worksheet.Cell("U" + i).Value = string.Format("'{0}", row["RequestDate"].ToString());
                worksheet.Cell("V" + i).Value = string.Format("'{0}", row["RequireDate"].ToString());
                worksheet.Cell("W" + i).Value = row["CostNo"].ToString();
            }
            else if (row["PaymentTerm"].ToString() != "" && row["Incoterm"].ToString() != "")
            {
                i++;
                worksheet.Cell("A" + i).Value = string.Format("{0}", 14);
                worksheet.Cell("B" + i).Value = row["subID"].ToString();
                worksheet.Cell("C" + i).Value = row["ID"].ToString();
                worksheet.Cell("D" + i).Value = "zpm1";
                worksheet.Cell("F" + i).Value = "X";
                worksheet.Cell("G" + i).Value = string.Format("{0}", row["CostNo"].ToString().Substring(0, 2) == "PF" ? "103" : "102");
                worksheet.Cell("H" + i).Value = "EX";
                worksheet.Cell("I" + i).Value = "203";
                worksheet.Cell("J" + i).Value = row["SoldTo"].ToString();
                worksheet.Cell("L" + i).Value = row["Incoterm"].ToString();
                worksheet.Cell("M" + i).Value = row["PaymentTerm"].ToString();
                worksheet.Cell("N" + i).Value = row["Currency"].ToString();
                worksheet.Cell("O" + i).Value = row["SalesUnit"].ToString();
                worksheet.Cell("P" + i).Value = row["Code"].ToString();
                worksheet.Cell("Q" + i).Value = row["OfferPriceExch"].ToString();
                worksheet.Cell("R" + i).Value = row["Currency"].ToString();
                worksheet.Cell("S" + i).Value = row["PricingUnit"].ToString();
                worksheet.Cell("T" + i).Value = row["SalesUnit"].ToString();
                worksheet.Cell("U" + i).Value = string.Format("'{0}", row["RequestDate"].ToString());
                worksheet.Cell("V" + i).Value = string.Format("'{0}", row["RequireDate"].ToString());
                worksheet.Cell("W" + i).Value = row["CostNo"].ToString();
            }
            else if (row["SoldTo"].ToString() != "" && row["Incoterm"].ToString() != "")
            {
                i++;
                worksheet.Cell("A" + i).Value = string.Format("{0}", 15);
                worksheet.Cell("B" + i).Value = row["subID"].ToString();
                worksheet.Cell("C" + i).Value = row["ID"].ToString();
                worksheet.Cell("D" + i).Value = "zpm1";
                worksheet.Cell("F" + i).Value = "X";
                worksheet.Cell("G" + i).Value = string.Format("{0}", row["CostNo"].ToString().Substring(0, 2) == "PF" ? "103" : "102");
                worksheet.Cell("H" + i).Value = "EX";
                worksheet.Cell("I" + i).Value = "203";
                worksheet.Cell("J" + i).Value = row["SoldTo"].ToString();
                worksheet.Cell("L" + i).Value = row["Incoterm"].ToString();
                worksheet.Cell("N" + i).Value = row["Currency"].ToString();
                worksheet.Cell("O" + i).Value = row["SalesUnit"].ToString();
                worksheet.Cell("P" + i).Value = row["Code"].ToString();
                worksheet.Cell("Q" + i).Value = row["OfferPrice"].ToString();
                worksheet.Cell("R" + i).Value = row["Currency"].ToString();
                worksheet.Cell("S" + i).Value = row["PricingUnit"].ToString();
                worksheet.Cell("T" + i).Value = row["SalesUnit"].ToString();
                worksheet.Cell("U" + i).Value = string.Format("'{0}", row["RequestDate"].ToString());
                worksheet.Cell("V" + i).Value = string.Format("'{0}", row["RequireDate"].ToString());
                worksheet.Cell("W" + i).Value = row["CostNo"].ToString();
            }
            else if (row["SoldTo"].ToString() != "" && row["PaymentTerm"].ToString() != "")
            {
                i++;
                worksheet.Cell("A" + i).Value = string.Format("{0}", 16);
                worksheet.Cell("B" + i).Value = row["subID"].ToString();
                worksheet.Cell("C" + i).Value = row["ID"].ToString();
                worksheet.Cell("D" + i).Value = "zpm1";
                worksheet.Cell("F" + i).Value = "X";
                worksheet.Cell("G" + i).Value = string.Format("{0}", row["CostNo"].ToString().Substring(0, 2) == "PF" ? "103" : "102");
                worksheet.Cell("H" + i).Value = "EX";
                worksheet.Cell("I" + i).Value = "203";
                worksheet.Cell("J" + i).Value = row["SoldTo"].ToString();
                worksheet.Cell("M" + i).Value = row["PaymentTerm"].ToString();
                worksheet.Cell("N" + i).Value = row["Currency"].ToString();
                worksheet.Cell("O" + i).Value = row["SalesUnit"].ToString();
                worksheet.Cell("P" + i).Value = row["Code"].ToString();
                worksheet.Cell("Q" + i).Value = row["OfferPriceExch"].ToString();
                worksheet.Cell("R" + i).Value = row["Currency"].ToString();
                worksheet.Cell("S" + i).Value = row["PricingUnit"].ToString();
                worksheet.Cell("T" + i).Value = row["SalesUnit"].ToString();
                worksheet.Cell("U" + i).Value = string.Format("{0}", row["RequestDate"].ToString());
                worksheet.Cell("V" + i).Value = string.Format("{0}", row["RequireDate"].ToString());
                worksheet.Cell("W" + i).Value = row["CostNo"].ToString();
            }
            else if (row["Code"].ToString() != "" && row["Incoterm"].ToString() != "")
            {
                i++;
                worksheet.Cell("A" + i).Value = string.Format("'{0}", 17);
                worksheet.Cell("B" + i).Value = row["subID"].ToString();
                worksheet.Cell("C" + i).Value = row["ID"].ToString();
                worksheet.Cell("D" + i).Value = "zpm2";
                worksheet.Cell("F" + i).Value = "X";
                worksheet.Cell("G" + i).Value = string.Format("{0}", row["CostNo"].ToString().Substring(0, 2) == "PF" ? "103" : "102");
                worksheet.Cell("H" + i).Value = "EX";
                worksheet.Cell("I" + i).Value = "203";
                worksheet.Cell("N" + i).Value = row["Currency"].ToString();
                worksheet.Cell("O" + i).Value = row["SalesUnit"].ToString();
                worksheet.Cell("P" + i).Value = row["Code"].ToString();
                worksheet.Cell("Q" + i).Value = row["subID"].ToString() == "" || row["subID"].ToString() == "0" ? 
                    cs.GetMinPrice(row["Formula"].ToString(), row["RequestNo"].ToString()) : row["MinPriceExch"].ToString();
                worksheet.Cell("R" + i).Value = row["Currency"].ToString();
                worksheet.Cell("S" + i).Value = string.Format("{0}", row["PricingUnit"].ToString());
                worksheet.Cell("T" + i).Value = row["SalesUnit"].ToString();

                worksheet.Cell("U" + i).Value = string.Format("{0}", row["RequestDate"].ToString());
                worksheet.Cell("V" + i).Value = string.Format("{0}", row["RequireDate"].ToString());
                worksheet.Cell("W" + i).Value = row["CostNo"].ToString();
                //IXLRange range = worksheet.Range("A" + i + ":W" + i);
                //i++;
                //worksheet.Cell("A" + i).Value = range;
                //worksheet.Cell("A" + i).Value = "18";
                //worksheet.Cell("I" + i).Value = "";
            }
        }
        //workbook.SaveAs(@"C:\temp\VK11_table.xlsx");
        workbook.SaveAs(@"\\192.168.1.212\Data\VK11_table.xlsx");
    }
    [WebMethod]
    public void GetUpdateTOCSV(string data)
    {
        var dir = HttpContext.Current.Server.MapPath("~/ExcelFiles");//D:\SAPInterfaces\Inbound
        var filePaths = Directory.GetFiles(dir, "*_result*.csv");
        foreach (string s in filePaths)
        {
            using (var reader = new StreamReader(s))

            using (var csv = new CsvReader(reader))
            {
                while (csv.Read())
                {//This will advance the reader to the next record.

                    //You can use an indexer to get by position or name. 
                    //This will return the field as a string

                    // By position
                    var field = csv[0];
                    var AppID = csv["AppID"];
                    // By header name

                    var Condition = csv["Condition TypeRV13A-KSCHL"];
                    if (csv["Result"] == "Condition records saved")
                    {
                        using (SqlConnection con = new SqlConnection(strConn))
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spUpdateQuotationfromJob";
                            cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", 0));
                            cmd.Parameters.AddWithValue("@subID", string.Format("{0}", Condition.ToString() == "zpm1" ? "0" : AppID.ToString()));
                            cmd.Parameters.AddWithValue("@ID", AppID.ToString());
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                //var records = csv.GetRecords<dynamic>();
                //foreach (var item in records)
                //{
                //    var da = item.Result;
                //    var condition = item[@"Condition Type RV13A-KSCHL"];  
                //}
                //foreach (dynamic record in records.ToList())
                //{
                //    var data = record["IfColumn"];
                //}
            }
            try
            { 
                File.Move(s, HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }
        }
    }
    [WebMethod]
   public void GetReadExcelupdateJob(string Data)
    {
        DevExpress.Spreadsheet.Worksheet worksheet;
        DevExpress.Web.ASPxSpreadsheet.ASPxSpreadsheet Spreadsheet = new DevExpress.Web.ASPxSpreadsheet.ASPxSpreadsheet();
        //string path = @"C:\Users\fo5910155\Documents\Winshuttle\Studio\Data\VK11_table.xlsx";
        string path = @"\\192.168.1.212\Data\VK11_20180604_095920_.xlsx";
        Spreadsheet.Document.LoadDocument(path);
        //worksheet = Spreadsheet.Document.Worksheets[0];
        DevExpress.Spreadsheet.IWorkbook workbook = Spreadsheet.Document;
        worksheet = Spreadsheet.Document.Worksheets[0];
        string value = "", result="",MinPrice= "";
        bool log_email = false;
        int i = 1;
        do
        {
            i++;
            value = worksheet["A" + i].Value.ToString();
            MinPrice = worksheet["Q" + i].Value.ToString();
            result = worksheet["X" + i].Value.ToString();
            if (result == "Condition records saved")
            {
                string subID = worksheet["B" + i].Value.ToString().Replace("'", "");
                string ID = worksheet["C" + i].Value.ToString().Replace("'", "");
                //spUpdateQuotationfromJob
                if (!string.IsNullOrEmpty(MinPrice))
                    MinPrice = MinPrice.Replace("'", "");
                using (SqlConnection con = new SqlConnection(strConn))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spUpdateQuotationfromJob";
                    cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", MinPrice));
                    cmd.Parameters.AddWithValue("@subID", string.Format("{0}", string.IsNullOrEmpty(subID.ToString())?"0":subID.ToString()));
                    cmd.Parameters.AddWithValue("@ID",ID.ToString());
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            else
                log_email = true;
            //Context.Response.Write(value);
        } while (value != "");
        //} while (i < 4);
        //Context.Response.Write(workbook.Worksheets.Count);
        //if (log_email)
        //    sendemailswhuttle();
    }
    [WebMethod]
    public void sendemailswhuttle()
    {
        try
        {
            string MailSubject = string.Format(@"run Job wshuttle fail : {0}",DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
            string MailTo = "Voravut.Somboornpong@Thaiunion.com";
            //string AppLocation = "";  
            //AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);  
            //AppLocation = AppLocation.Replace("file:\\", "");  
            //string file = AppLocation + "\\ExcelFiles\\DataFile.xlsx";  
            string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/VK11_"+ DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
            using (new UserImpersonation("Costing.WebBase@thaiunion.com", "thaiunion.co.th", "AAaa123*"))            {
                File.Copy(@"\\\\192.168.1.212\Data\VK11_20180604_095920_.xlsx", file);
            }
            //NetworkCredential theNetworkCredential = new NetworkCredential(@"thaiunion\service.webbase", "Tuna@40wb*");
            //CredentialCache theNetCache = new CredentialCache();
            //theNetCache.Add(new Uri(file), "Basic", theNetworkCredential);
            //string[] theFolders = Directory.GetDirectories(@"\\192.168.1.212\Data\VK11_20180604_095920_.xlsx");
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient();
            //SmtpClient SmtpServer = new SmtpClient("smtp.office365.com");
            mail.From = new MailAddress("Costing.WebBase@thaiunion.com");
            //mail.To.Add(MailTo); // Sending MailTo 
            if (string.IsNullOrEmpty(MailTo)) return;
            string[] words = MailTo.Split(',');
            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                    mail.To.Add(new MailAddress(word));
            }
            List<string> li = new List<string>();
            li.Add("Maneerat.Kladthong@thaiunion.com");
            //li.Add("Wasana.Phuangkhajorn@thaiunion.com");
            //li.Add("saihacksoft@gmail.com");  
            //li.Add("saihacksoft@gmail.com");  
            //li.Add("saihacksoft@gmail.com");  
            mail.CC.Add(string.Join<string>(",", li)); // Sending CC  
            //mail.Bcc.Add(string.Join < string > (",", li)); // Sending Bcc   
            mail.Subject = MailSubject; // Mail Subject  
            mail.Body = "job wshuttle fail <br/> *This is an automatically generated email, please do not reply*";
            mail.IsBodyHtml = true;
            mail.Attachments.Add(new Attachment(file));
            //SmtpServer.Port = 587; //PORT  
            //SmtpServer.EnableSsl = true;
            //SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            //SmtpServer.UseDefaultCredentials = false;
            //SmtpServer.Credentials = new NetworkCredential("Costing.WebBase@thaiunion.com", "AAaa123*");
            //SmtpServer.Send(mail);

            SmtpServer.Host = "192.168.1.39";
            SmtpServer.Port = 25;
            SmtpServer.Send(mail);
            SmtpServer.Dispose();

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    [WebMethod]
    public void UserAD()
    {
        Context.Response.Write("UserAD :"+string.Format("{0}", cs.GetUserAD().ToString()));
    }
    [WebMethod]
    public void again_sendmail(string MailID)
    {
        try { 
        string strSQL = @"select * from MailData where MailID="+ MailID;
        DataTable dt = cs.builditems(strSQL);
        foreach(DataRow row in dt.Rows)
        cs.sendemail(row["To"].ToString(),
           string.Format("{0}", row["Cc"]),
           string.Format("{0}", row["Body"]), 
           string.Format("{0}",row["Subject"]));
        }
        catch (Exception ex)
        {
            Context.Response.Write("Exception Executing Stored Procedure:" + ex.Message);
        }
    }
    [WebMethod]
    public void sendMailUsingOffice365SMTP()
    {
        //Creating a new mail message
        MailMessage message = new MailMessage();

        //Adding recipients
        message.To.Add(new MailAddress("voravut.somboornpong@thaiunion.com", "somboornpogn,voravut"));
        //Adding Subject
        message.Subject = "This is the Subject line";

        //Adding sender
        message.From = new MailAddress("Costing.WebBase@thaiunion.com", "Costing.WebBase");

        //Adding body text
        message.Body = "This is the message body";

        //Configuring SMTP
        SmtpClient smtp = new System.Net.Mail.SmtpClient("outlook.office365.com");
        smtp.EnableSsl = true;
        smtp.Port = 587;
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

        //Setting credentials
        //IMPORTANT!!! You must use your tenant crendential using the email address format and NOT the domain username
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new System.Net.NetworkCredential("Costing.WebBase@thaiunion.com", "AAaa123*");

        //Sending message
        smtp.Send(message);
    }
    public void ExportDataSetToExcel(DataSet ds)
    {
        //string AppLocation = "";
        //AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        //AppLocation = AppLocation.Replace("file:\\", "");
        //string file = AppLocation + "\\ExcelFiles\\DataFile.xlsx";
        string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/DataFile.xlsx");
        using (XLWorkbook wb = new XLWorkbook())
        {
            wb.Worksheets.Add(ds.Tables[0]);
            wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            wb.Style.Font.Bold = true;
            wb.SaveAs(file);
        }
    }
    [WebMethod]
    public void GetDataReport()
    {
        DataSet ds = null;
        using (SqlConnection con = new SqlConnection(strConn))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("spGetProblemReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                ds = new DataSet();
                da.Fill(ds);
                ExportDataSetToExcel(ds);
                smpcgmail("ExportDataSetToExcel");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                ds.Dispose();
            }
        }
    }
    [WebMethod]
    public void smpcgmail(string MailSubject)   
    {  
        try  
        {
            string MailTo = "Jutharut.Suwankanoknark@thaiunion.com,Thipwal.Khao-orn@thaiunion.com,Tuenjai.Saelee @thaiunion.com";
            //string AppLocation = "";  
            //AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);  
            //AppLocation = AppLocation.Replace("file:\\", "");  
            //string file = AppLocation + "\\ExcelFiles\\DataFile.xlsx";  
            string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/DataFile.xlsx");
            MailMessage mail = new MailMessage();  
            SmtpClient SmtpServer = new SmtpClient("smtp.office365.com");  
            mail.From = new MailAddress("Costing.WebBase@thaiunion.com");
            //mail.To.Add(MailTo); // Sending MailTo 
            if (string.IsNullOrEmpty(MailTo)) return;
            string[] words = MailTo.Split(',');
            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                    mail.To.Add(new MailAddress(word));
            }
            List < string > li = new List < string > ();
            li.Add("Wandee.Ardkhongharn@thaiunion.com");
            li.Add("Wasana.Phuangkhajorn@thaiunion.com");  
            //li.Add("saihacksoft@gmail.com");  
            //li.Add("saihacksoft@gmail.com");  
            //li.Add("saihacksoft@gmail.com");  
            mail.CC.Add(string.Join < string > (",", li)); // Sending CC  
            //mail.Bcc.Add(string.Join < string > (",", li)); // Sending Bcc   
            mail.Subject = MailSubject; // Mail Subject  
            mail.Body = "NCP Report *This is an automatically generated email, please do not reply*";  
            System.Net.Mail.Attachment attachment;  
            attachment = new System.Net.Mail.Attachment(file); //Attaching File to Mail  
            mail.Attachments.Add(attachment);  
            SmtpServer.Port = 587; //PORT  
            SmtpServer.EnableSsl = true;  
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;  
            SmtpServer.UseDefaultCredentials = false;  
            SmtpServer.Credentials = new NetworkCredential("Costing.WebBase@thaiunion.com", "AAaa123*");
            SmtpServer.Send(mail);  
        } catch (Exception ex)  
        {  
            throw ex;  
        }  
    }
    [WebMethod]
    public void testselectMaterial(string Keyword,string n)
    {
        SqlParameter[] param = {new SqlParameter("@Keyword", Keyword),
        new SqlParameter("@user", CurUserName.ToString()),
        new SqlParameter("@n", n)};
        DataTable table = new DataTable();
        table = SQLResources("spselectMaterial", param);
        Context.Response.Write(JsonConvert.SerializeObject(table));
    }
    [WebMethod]
    public void testxxx(string data)
    {
        string strSQL = @"select a.ID'XX',RDNumber from TransCostingHeader a left join TransFormulaHeader b on a.ID=b.RequestNo
    where Company = 102 and b.RefSamples='' and a.ID='"+ data.ToString() + "' group by a.ID, RDNumber";
        foreach (DataRow dr in cs.builditems(strSQL).Rows)
        {
            string rd = dr["RDNumber"].ToString(); List<string> mylist = new List<string>(); string c;
            int ed = rd.ToString().IndexOf(")");
            //int n = i + 1;int z=0;List<string> mylist= new List<string>();int x = i;
            int i = rd.ToString().IndexOf("(");
            if (i > 0)
            {
                c = rd.ToString().Substring(0, i);

                int n = i + 1; int x = i;
                string[] myarr = Regex.Split(rd.ToString().Substring(n, ed - n), ",");
                foreach (string ar in myarr)
                {
                    int index = ar.ToString().IndexOf("-"); string[] mysplit;
                    if (index > 0)
                    {
                        mysplit = Regex.Split(ar.ToString(), "-");
                        int s = Convert.ToInt32(mysplit[0]);
                        while (s <= Convert.ToInt32(mysplit[1]))
                        {
                            mylist.Add(s.ToString()); ++s;
                        }
                    }
                    else
                        mylist.Add(ar);
                }
                int o = 0;
                foreach (string ar in mylist)
                {
                    ++o;
                    SqlCommand sqlComm = new SqlCommand();
                    sqlComm.CommandText = @"update TransFormulaHeader set RefSamples=@RefSamples WHERE RequestNo=@ID and Formula=@f";
                    sqlComm.Parameters.AddWithValue("@ID", dr["XX"].ToString());
                    sqlComm.Parameters.AddWithValue("@RefSamples", string.Format("{0}{1}", c,ar.ToString()));
                    sqlComm.Parameters.AddWithValue("@f", o.ToString());
                    DataTable dt = GetDataDb(sqlComm);
                    Context.Response.Write(JsonConvert.SerializeObject(dt));
                }
            }
            else
            {
                c = rd.ToString();
                SqlCommand sqlComm = new SqlCommand();
                sqlComm.CommandText = @"update TransFormulaHeader set RefSamples=@RefSamples WHERE RequestNo=@ID and Formula=@f";
                sqlComm.Parameters.AddWithValue("@ID", dr["XX"].ToString());
                sqlComm.Parameters.AddWithValue("@RefSamples", c.ToString());
                sqlComm.Parameters.AddWithValue("@f", string.Format("{0}", 1));
                DataTable dt = GetDataDb(sqlComm);
                Context.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
    }
         [WebMethod]
    public void selectMateriallist(string data)
    {
        string datapath = "~/XlsTables/" + data + ".json";//DataTable t=new DataTable();
        using (StreamReader sr = new StreamReader(Server.MapPath(datapath)))
        {
            string json = sr.ReadToEnd();
            dynamic dynJson = JsonConvert.DeserializeObject(json);
            foreach (var item in dynJson)
            {
                SqlParameter[] param = {new SqlParameter("@Keyword", item.Keyword.ToString()),
                new SqlParameter("@user", CurUserName.ToString()),
                new SqlParameter("@n", item.n.ToString())};
                DataTable table = new DataTable();
                table = SQLResources("spselectMaterial", param);
                Context.Response.Write(JsonConvert.SerializeObject(table));
                //Context.Response.Write(item.Keyword.ToString());
            }
        }
    }
    public DataTable SQLResources(string StoredProcedure, object[] Parameters)
    {
        var Results = new DataTable();
        String strConnString = ConfigurationManager.
        ConnectionStrings["CostingConnectionString"].ConnectionString;
        try
        {
            using (SqlConnection conn = new SqlConnection(strConnString))
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
            throw ex;
        }
        return Results;
    }
    [WebMethod]
    public void testGetFishScrapes(string Date, string Size, string Line)
    {
        SqlParameter[] param = {new SqlParameter("@Date",Date),
            new SqlParameter("@Size",Size),
            new SqlParameter("@LineId",Line)};
        DataTable table = new DataTable();
        table = testResources("spGetFishScrapes", param);
        Context.Response.Write(JsonConvert.SerializeObject(table));
    }
    public DataTable testResources(string StoredProcedure, object[] Parameters)
    {
        var Results = new DataTable();
        String strConnString = System.Configuration.ConfigurationManager.
            ConnectionStrings["TunaConnectionString"].ConnectionString;
        try
        {
            using (SqlConnection conn = new SqlConnection(strConnString))
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
            Context.Response.Write("Exception Executing Stored Procedure:" + ex.Message);
        }

        return Results;
    }
    [WebMethod]
    public void InsertSaleitem(string data)
    {
        string datapath = "~/Content/" + data + ".json"; bool first = true;
        using (StreamReader sr = new StreamReader(Server.MapPath(datapath)))
        {
            string json = sr.ReadToEnd();
            dynamic dynJson = JsonConvert.DeserializeObject(json);
            foreach (var item in dynJson)
            {
                if (first)
                {
                    first = false;
                    SqlCommand sqlComm = new SqlCommand();
                    sqlComm.CommandText = @"DELETE FROM tbPLCR WHERE RequestNo=@ID";
                    sqlComm.Parameters.AddWithValue("@ID", item.ID.ToString());
                    DataTable dt = GetData(sqlComm);
                    Context.Response.Write(JsonConvert.SerializeObject(dt));
                }
                SqlParameter[] param = {new SqlParameter("@ID",item.ID.ToString()),
                        new SqlParameter("@CODES",item.CODES.ToString()),
                        new SqlParameter("@old_code",item.old_code.ToString()),
                        new SqlParameter("@BATCH",item.BATCH.ToString()),
                        new SqlParameter("@QTY",item.QTY.ToString()),
                        new SqlParameter("@BU",item.BU.ToString()),
                        new SqlParameter("@RK",item.RK.ToString()),
                        new SqlParameter("@Detail",item.Detail.ToString()),
                        new SqlParameter("@LC",item.LC.ToString()),
                        new SqlParameter("@DatePack",item.DatePack.ToString()),
                        new SqlParameter("@QC",item.QC.ToString()),
                        new SqlParameter("@PLANT",item.PLANT.ToString())};
                DataTable table = new DataTable();
                table = GetRelatedResources("spInsertSaleitem", param);
                Context.Response.Write(JsonConvert.SerializeObject(table));
            }
        }
    }
    [WebMethod]
    public void savedataOrder(string data)
    {
        string datapath = "~/Content/" + data + ".json";
        using (StreamReader sr = new StreamReader(Server.MapPath(datapath)))
        {
            string json = sr.ReadToEnd();
            dynamic dynJson = JsonConvert.DeserializeObject(json);
            foreach (var item in dynJson)
            {
                SqlParameter[] param = {new SqlParameter("@ID",item.ID.ToString()),
                new SqlParameter("@Salesorder", item.Salesorder.ToString()),
                new SqlParameter("@Item", item.Item.ToString()),
                new SqlParameter("@Agent", item.Agent.ToString()),
                new SqlParameter("@Brand", item.Brand.ToString()),
                new SqlParameter("@Port", item.Port.ToString()),
                new SqlParameter("@KeyDate", item.KeyDate.ToString()),
                new SqlParameter("@Country", item.Country.ToString()),
                new SqlParameter("@Plant", item.Plant.ToString()),
                new SqlParameter("@Quantity", item.Quantity.ToString()),
                new SqlParameter("@P", item.P.ToString()),

                new SqlParameter("@UserID", item.UserID.ToString()),
                new SqlParameter("@LastProduct", item.LastProduct.ToString()),
                new SqlParameter("@OrderOpen", item.OrderOpen.ToString()),
                new SqlParameter("@MaxCode", item.MaxCode.ToString()),
                new SqlParameter("@Incubation", item.Incubation.ToString()),
                new SqlParameter("@ShelfLife", item.ShelfLife.ToString()),
                new SqlParameter("@DelComplate", item.DelComplate.ToString())};
                DataTable table = new DataTable();
                table = GetRelatedResources("spinsertsaleorder", param);
                Context.Response.Write(JsonConvert.SerializeObject(table));
            }
        }
    }
    [WebMethod]
    public void selectdata(string data)
    {
        data = data.Replace("@", "%");
        SqlCommand cmd = new SqlCommand(data);
        DataTable dt = GetData(cmd);
        Context.Response.Write(JsonConvert.SerializeObject(dt));
    }
    [WebMethod]
    public void selectorder(string Salesdoc, string item)
    {
            SqlParameter[] param = {new SqlParameter("@saleorder",Salesdoc),
                        new SqlParameter("@item",item)};
            DataTable table = new DataTable();
            table = GetRelatedResources("spselectorder", param);
            Context.Response.Write(JsonConvert.SerializeObject(table));
     }
    [WebMethod]
    public void selectdeatil(string Id)
    {
        SqlParameter[] param = {new SqlParameter("@ID",Id)};
        DataTable table = new DataTable();
        table = GetRelatedResources("spselectdeatil", param);
        Context.Response.Write(JsonConvert.SerializeObject(table));
    }

    [WebMethod]
    public void getdeatil(string Id)
    {
        SqlParameter[] param = { new SqlParameter("@ID", Id) };
        DataTable table = new DataTable();
        table = GetRelatedResources("spgetdeatil", param);
        Context.Response.Write(JsonConvert.SerializeObject(table));
    }
    [WebMethod]
    public void selectbatch(string data,string batch)
    {
        data = data.Replace("@", "%");
        SqlParameter[] param = { new SqlParameter("@bc_code", data),
        new SqlParameter("@batch",batch)};
        DataTable table = new DataTable();
        table = GetRelatedResources("spselectbatch", param);
        Context.Response.Write(JsonConvert.SerializeObject(table));
    }
    [WebMethod]
    public void selectall(string data)
    {
        string datapath = "~/Content/" + data + ".json";//DataTable t=new DataTable();
        using (StreamReader sr = new StreamReader(Server.MapPath(datapath)))
        {
            string json = sr.ReadToEnd();
            dynamic dynJson = JsonConvert.DeserializeObject(json);
            foreach (var item in dynJson)
            {
                SqlParameter[] param = { new SqlParameter("@data", item.where.ToString()),
					new SqlParameter("@FrDt",item.FrDt.ToString()),
					new SqlParameter("@By",item.By.ToString()),
					new SqlParameter("@Material",item.Material.ToString()),
					new SqlParameter("@Batch",item.Batch.ToString()),
					new SqlParameter("@username",item.username.ToString()),
					new SqlParameter("@Salesorder",item.Salesorder.ToString()),
					new SqlParameter("@Items",item.Items.ToString())};
                DataTable table = new DataTable();
                table = GetRelatedResources("spselectall", param);
                Context.Response.Write(JsonConvert.SerializeObject(table));
            }
        }
    }
    public DataTable GetRelatedResources(string StoredProcedure, object[] Parameters)
    {
        var Results = new DataTable();
        String strConnString = System.Configuration.ConfigurationManager.
            ConnectionStrings["WareHouseConnectionString"].ConnectionString;
        try
        {
            using (SqlConnection conn = new SqlConnection(strConnString))
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
            Context.Response.Write("Exception Executing Stored Procedure:" + ex.Message);
        }

        return Results;
    }
    public DataTable GetDataDb(SqlCommand cmd)
    {
        DataTable dt = new DataTable();
        String oConn =  ConfigurationManager.
             ConnectionStrings["CostingConnectionString"].ConnectionString;
        SqlConnection con = new SqlConnection(oConn);
        SqlDataAdapter sda = new SqlDataAdapter();
        cmd.CommandType = CommandType.Text;
        cmd.Connection = con;
        try
        {
            con.Open();
            sda.SelectCommand = cmd;
            sda.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            con.Close();
            sda.Dispose();
            con.Dispose();
        }
    }
    public DataTable GetData(SqlCommand cmd)
    {
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strConn);
        SqlDataAdapter sda = new SqlDataAdapter();
        cmd.CommandType = CommandType.Text;
        cmd.Connection = con;
        try
        {
            con.Open();
            sda.SelectCommand = cmd;
            sda.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            con.Close();
            sda.Dispose();
            con.Dispose();
        }
    }
    public DataTable TestData(SqlCommand cmd)
    {
        DataTable dt = new DataTable();
        String strConnString = System.Configuration.ConfigurationManager.
             ConnectionStrings["TestDataConnectionString"].ConnectionString;
        SqlConnection con = new SqlConnection(strConnString);
        SqlDataAdapter sda = new SqlDataAdapter();
        cmd.CommandType = CommandType.Text;
        cmd.Connection = con;
        try
        {
            con.Open();
            sda.SelectCommand = cmd;
            sda.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            con.Close();
            sda.Dispose();
            con.Dispose();
        }
    }
    //[WebMethod]
    //public void Getshape(string data)
    //{
    //    string datapath = "~/Content/" + data + ".json";
    //    using (StreamReader sr = new StreamReader(Server.MapPath(datapath)))
    //    {
    //        string json = sr.ReadToEnd();
    //        dynamic dynJson = JsonConvert.DeserializeObject(json);
    //        foreach (var item in dynJson)
    //        {
    //            using (SqlConnection con = new SqlConnection(strConn))
    //            {
    //                string query = "insert into tblShape values(@Name,@ContentType,@Top,@Left,@Height,@Width,null)";
    //                using (SqlCommand cmd = new SqlCommand(query))
    //                {
    //                    cmd.Connection = con;
    //                    cmd.Parameters.AddWithValue("@Name", item.Name.ToString());
    //                    cmd.Parameters.AddWithValue("@ContentType",item.ContentType.ToString());
    //                    cmd.Parameters.AddWithValue("@Top", item.Top.ToString());
    //                    cmd.Parameters.AddWithValue("@Left", item.Left.ToString());
    //                    cmd.Parameters.AddWithValue("@Height", item.Height.ToString());
    //                    cmd.Parameters.AddWithValue("@Width", item.Width.ToString());
    //                    con.Open();
    //                    cmd.ExecuteNonQuery();
    //                    con.Close();
    //                }
    //            }

    //        }
    //    }
    //    Context.Response.Write("success");
    //}
    [WebMethod]
    public void Getdelete(string data)
    {
        string datapath = "~/Content/" + data + ".json";
        using (StreamReader sr = new StreamReader(Server.MapPath(datapath)))
        {
            string json = sr.ReadToEnd();
            dynamic dynJson = JsonConvert.DeserializeObject(json);
            foreach (var item in dynJson)
            {
                using (SqlConnection con = new SqlConnection(strConn))
                {
                    string query = "update tblncp set Active=1 where Id=@Id";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", (int)item.Name);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    //SqlCommand cmd = new SqlCommand();
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.CommandText = "spdelete";
                    //cmd.Parameters.AddWithValue("@Id", item.Name.ToString());
                    //cmd.Connection = con;
                    //con.Open();
                    //DataTable dt = new DataTable();
                    //SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                    //oAdapter.Fill(dt);
                    //con.Close();
            }
            //SqlConnection sqlConn = new SqlConnection(strConn);
            //SqlCommand sqlComm = new SqlCommand();
            //sqlComm = sqlConn.CreateCommand();
            //sqlComm.CommandText = @"DELETE FROM tblncp WHERE ID='@Id'";
            //sqlComm.Parameters.AddWithValue("@Id", item.Name);
            //sqlConn.Open();
            //sqlComm.ExecuteNonQuery();
            //sqlConn.Close();

        }
        }
        Context.Response.Write("success");
    }
    [WebMethod]
    public void Getcopy(string sName,string suser,string num)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spcopy";
            cmd.Parameters.AddWithValue("@Id", sName);
            cmd.Parameters.AddWithValue("@user", suser);
            cmd.Parameters.AddWithValue("@num", Int32.Parse(num)); 
            cmd.Connection = con;
            con.Open();
            //DataTable dt = new DataTable();
            //SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            //oAdapter.Fill(dt);
            //con.Close();
            //Context.Response.Write(JsonConvert.SerializeObject(dt));
            cmd.ExecuteNonQuery();
            con.Close();
            Context.Response.Write("success");
        }
    }
    [WebMethod]
    public void GetHistory(string sName)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spHistory";
            cmd.Parameters.AddWithValue("@Id", sName);
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            Context.Response.Write(JsonConvert.SerializeObject(dt));
        }
    }
    //[WebMethod]
    //public int SaveDocument(Byte[] filebyte)
    //{
    //    int adInteger; 
    //    using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        using (SqlCommand cmd = new SqlCommand("INSERT INTO masmaterial(docbinaryarray)  VALUES(@docbinaryarray);SELECT SCOPE_IDENTITY();",con))
    //        {
    //            cmd.CommandType = CommandType.Text;
    //            cmd.Parameters.AddWithValue("@docbinaryarray", filebyte);
    //            con.Open();
    //            adInteger = (int)cmd.ExecuteScalar();

    //            if (con.State == System.Data.ConnectionState.Open) con.Close();
    //            return adInteger;
    //        }
    //    }
    //}
    [WebMethod]
    public void Getselectsummaryall(string data)
    {
        string datapath = "~/Content/" + data + ".json";//DataTable t=new DataTable();
        using (StreamReader sr = new StreamReader(Server.MapPath(datapath)))
        {
            string json = sr.ReadToEnd();
            dynamic dynJson = JsonConvert.DeserializeObject(json);
            foreach (var item in dynJson)
            {
                //Context.Response.Write(item.FrDt +','+ item.where+','+ item.By+','+ item.Material+','+ item.Batch+','+ item.username+','+ item.decision+','+ item.FirstDecision);
                using (SqlConnection con = new SqlConnection(strConn))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spselectsummaryall";
                    cmd.Parameters.AddWithValue("@FrDt", string.Format("{0}", item.FrDt));
                    cmd.Parameters.AddWithValue("@where", string.Format("{0}", item.where));
                    cmd.Parameters.AddWithValue("@By", string.Format("{0}", item.By));
                    cmd.Parameters.AddWithValue("@Material", string.Format("{0}", item.Material));
                    cmd.Parameters.AddWithValue("@Batch", string.Format("{0}", item.Batch));
                    cmd.Parameters.AddWithValue("@username", string.Format("{0}", item.username));
                    cmd.Parameters.AddWithValue("@decision", string.Format("{0}", item.decision));
                    cmd.Parameters.AddWithValue("@FirstDecision", string.Format("{0}", item.FirstDecision));
                    //cmd.Parameters.Add(new SqlParameter("@FrDt", string.Format("{0}",item.FrDt)));
                    //cmd.Parameters.Add(new SqlParameter("@where", item.where));
                    //cmd.Parameters.Add(new SqlParameter("@By", item.By));
                    //cmd.Parameters.Add(new SqlParameter("@Material", item.Material));
                    //cmd.Parameters.Add(new SqlParameter("@Batch", item.Batch));
                    //cmd.Parameters.Add(new SqlParameter("@username", item.username));
                    //cmd.Parameters.Add(new SqlParameter("@decision", item.decision));
                    //cmd.Parameters.Add(new SqlParameter("@FirstDecision", item.FirstDecision));
                    cmd.Connection = con;
                    con.Open();
                    DataTable dt = new DataTable();
                    SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                    oAdapter.Fill(dt);
                    con.Close();
                    //t= dt;
                    Context.Response.Write(JsonConvert.SerializeObject(dt));
                }
            }
        }
        //return t;
    }
    //[WebMethod]
    //public void Getselectsummaryall(string FrDt, string where,string By,string Batch,string Material,string username,string decision,string FirstDecision)
    //{
    //    using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        SqlCommand cmd = new SqlCommand();
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.CommandText = "spselectsummaryall";
    //        cmd.Parameters.AddWithValue("@FrDt", FrDt);
    //        cmd.Parameters.AddWithValue("@where", where);
    //        cmd.Parameters.AddWithValue("@By", By);
    //        cmd.Parameters.AddWithValue("@Material", Material);
    //        cmd.Parameters.AddWithValue("@Batch", Batch);
    //        cmd.Parameters.AddWithValue("@username", username);
    //        cmd.Parameters.AddWithValue("@decision", decision);
    //        cmd.Parameters.AddWithValue("@FirstDecision", FirstDecision);
    //        cmd.Connection = con;
    //        con.Open();
    //        DataTable dt = new DataTable();
    //        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
    //        oAdapter.Fill(dt);
    //        con.Close();
    //        Context.Response.Write(JsonConvert.SerializeObject(dt));
    //    }
    //}
    [WebMethod]
    public void GetselectMaterial(string n, string Keyword)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spselectMaterial";
            cmd.Parameters.AddWithValue("@n", n);
            cmd.Parameters.AddWithValue("@user", CurUserName.ToString());
            cmd.Parameters.AddWithValue("@Keyword", Keyword);
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            Context.Response.Write(JsonConvert.SerializeObject(dt));
        }
    } 
   [WebMethod]
    public void savebyte(string data)
    {
        string datapath = "~/Content/" + data + ".json";
        using (StreamReader sr = new StreamReader(Server.MapPath(datapath)))
        {
            string json = sr.ReadToEnd();
            //string sp = "spInsertMultipleRows";
            List<Objattachment> ro = JsonConvert.DeserializeObject<List<Objattachment>>(json);
            //byte[] bytes = br.ReadBytes((Int32)fs.Length);
            using (SqlConnection con = new SqlConnection(strConn))
            {
                string query = "insert into tblFiles values (@Name,@ContentType,@Data,@MatDoc,@ActiveBy)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Name", ro[0].Name);
                    cmd.Parameters.AddWithValue("@ContentType", ro[0].ContentType);
                    cmd.Parameters.AddWithValue("@Data", ro[0].Data);
                    cmd.Parameters.AddWithValue("@MatDoc", ro[0].MatDoc);
                    cmd.Parameters.AddWithValue("@ActiveBy", ro[0].ActiveBy);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

        }
        Context.Response.Write("success");
    }
    [WebMethod]
    public void InsertProblem(string data)
    {
        string datapath = "~/Content/" + data + ".json"; bool first = true;
        using (StreamReader sr = new StreamReader(Server.MapPath(datapath)))
        {
            string json = sr.ReadToEnd();
            dynamic dynJson = JsonConvert.DeserializeObject(json);
            foreach (var item in dynJson)
            {
                    using (SqlConnection con = new SqlConnection(strConn))
                {
                    if (first)
                    {
                        first = false;
                        SqlCommand sqlComm = new SqlCommand();
                    sqlComm = con.CreateCommand();
                    sqlComm.CommandText = @"DELETE FROM tblDetail WHERE RequestNo=@ID";
                    sqlComm.Parameters.AddWithValue("@ID", item.ID.ToString());
                    con.Open();
                    sqlComm.ExecuteNonQuery();
                    con.Close();
                }
                    string query = "insert into tblDetail values (@ID,@Problem,@Detail)";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ID", string.Format("{0}", item.ID.ToString()));
                        cmd.Parameters.AddWithValue("@Problem", string.Format("{0}", item.Problem.ToString()));
                        cmd.Parameters.AddWithValue("@Detail", string.Format("{0}", item.Detail.ToString()));
                        //con.Open();
                        //cmd.ExecuteNonQuery();
                        //con.Close();
                        con.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                        oAdapter.Fill(dt);
                        con.Close();
                        Context.Response.Write(JsonConvert.SerializeObject(dt));
                    }
                }
            }
        }
    }
    private DataTable GetGridData()
    {
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[3] { new DataColumn("ID", typeof(int)),
                            new DataColumn("Component", typeof(string)),
                            new DataColumn("Code", typeof(string)) });
        dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
        return dt;
    }
    [WebMethod]
    public void formatbatchsap(string FrDt,string keydate,string Shift, string Line)
    {
        //YMDSLYMDN
        string name = ""; string b="";
        string[] array = Regex.Split("3YMD;SL-;M(M);D;S", ";");
        foreach (string arr in array)
        {
            DateTime date;
            if(arr=="3YMD")
            date= DateTime.ParseExact(FrDt, "yyyyMMdd", null, DateTimeStyles.None);
            else
            date = DateTime.ParseExact(keydate, "yyyyMMdd", null, DateTimeStyles.None);
            name = string.Format("{0}", arr);
            b += Structurename(name,date,Shift,Line);
        }
        Context.Response.Write(b);
    }
    [WebMethod]
    public void nameformat(string customer, string FrDt, string Shift, string Line)
    {
        DataTable table = GetGridData();
        string Id = ""; string strQuery = "";
        strQuery = " select Id from tblCustomer where CustomerCode = '" + customer.Substring(14, 2).ToString() + "' and ProductGroup in (select case when ProductType = 'F'then '0'"; 
        strQuery += " when ProductType = 'H' then '1' end 'ProductType' from tblProductGroup where ProductGroup like '%" + customer.Substring(1, 1).ToString() + "%')";
        foreach (DataRow row in builditems(strQuery).Rows)
        {
            Id = row["Id"].ToString();
        }
        string copy = ""; 
        DateTime date = DateTime.ParseExact(FrDt, "yyyyMMdd", null, DateTimeStyles.None);
        strQuery = "select * from tblBatchFormat where CustomerId='" + Id.ToString() + "'"; 
        strQuery+=    " and PackagingType in (select DataType from tblPackaging where Name = '" + customer.Substring(11, 1).ToString() + "')";
        DataTable dt = builditems(strQuery);
        foreach (DataRow dr in dt.Rows)
        {
            copy = Structurename(dr["Name"].ToString(), date,Shift,Line);
        }
        Context.Response.Write(copy);
    }
    public string Structurename(string name,DateTime date,string Shift,string Line)
    {
        string strQuery = "";
        string copy = ""; string yourInt = ""; int pos = 1;
        copy = name.ToString();
        string s = "P|PP|PPP|PPPP|Y|YY|YYYY|M|MM|M(M)|MMM|D|D(D)|DD|S|L|LL|T|O|JJJ|L|LL|S";
        string[] words = s.Split('|');
        foreach (string word in words)
        {
            if (name.Contains(word))
            {
                int index = name.IndexOf(word);
                //Console.WriteLine(word);
                while (index != -1)
                {
                    switch (word)
                    {
                        case "Y":
                        case "YY":
                        case "YYYY":
                            //case when '" + word.ToString() + "'='Y' then Id " + 
                            strQuery = "select case '" + word.ToString() + "' when 'Y' then convert(nvarchar,id)  when 'YY' then format(cast('" + date + "' as date),'yy') when 'YYYY' then [SName] end 'Id'" +
                                " from tblyear where sname='" + date.Year + "'";
                            foreach (DataRow row in builditems(strQuery).Rows)
                            {
                                copy = Getpositionstuff(copy, index + pos, (int)word.Length, row["Id"].ToString());
                            }
                            break;
                        case "M":
                        case "MMM":
                            foreach (DataRow row in builditems("select case when '" + word.ToString() + "' ='MMM' then Title else sname end 'sname' from tblmonth where Id='" + date.Month + "'").Rows)
                            {
                                copy = Getpositionstuff(copy, index + pos, (int)word.Length, row["sname"].ToString());
                            }
                            break;
                        case "MM":
                            yourInt = string.Format("{0:D2}", date.Month);
                            copy = Getpositionstuff(copy, index + pos, (int)word.Length, yourInt);
                            break;
                        case "M(M)":
                            foreach (DataRow row in builditems("select Mtext from tblmonth where Id='" + date.Month + "'").Rows)
                                copy = Getpositionstuff(copy, index + pos, (int)word.Length, row["Mtext"].ToString());
                            pos = (-3) + pos;
                            break;
                        case "D":
                            strQuery = "select case '" + word.ToString() + "' when 'D' then name when 'D(D)' then convert(nvarchar,id) when 'DD' then format(cast('" + date + "' as date),'dd') end 'name'" +
                                " from tblDate where Id='" + date.Day + "'";
                            foreach (DataRow row in builditems(strQuery).Rows)
                            {
                                copy = Getpositionstuff(copy, index + pos, (int)word.Length, row["name"].ToString());
                            }
                            break;
                        case "D(D)":
                            copy = Getpositionstuff(copy, index + pos, (int)word.Length, date.Day.ToString());
                            pos = (-3) + pos;
                            break;
                        case "DD":
                            yourInt = string.Format("{0:D2}", date.Day);
                            copy = Getpositionstuff(copy, index + pos, (int)word.Length, yourInt);
                            break;
                        case "JJJ":
                            strQuery = "select datepart(dy, '" + date + "') as 'Julian'";
                            foreach (DataRow row in builditems(strQuery).Rows)
                            {
                                yourInt = string.Format("{0:D3}", row["Julian"]);
                                copy = Getpositionstuff(copy, index + pos, (int)word.Length, yourInt);
                            }
                            break;
                        case "S":
                            copy = Getpositionstuff(copy, index + pos, (int)word.Length, Shift.ToString());
                            break;
                        //case "DD":
                        //    break;
                        case "L":
                        case "LL":
                            strQuery = "select case '" + word + "' when 'L' then Lname when 'LL' then L2 end 'name' from tblline where Lname='" + Line + "'";
                            foreach (DataRow row in builditems(strQuery).Rows)
                            {
                                copy = Getpositionstuff(copy, index + pos, (int)word.Length, row["name"].ToString());
                            }
                            break;
                    }
                    index = name.IndexOf(word, index + 1);
                }
            }
        }
            return copy;
    }
    [WebMethod]
    public string Getpositionstuff(string oldData,int stuff,int n,string str)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sppositionstuff";
            cmd.Parameters.AddWithValue("@oldData", oldData);
            cmd.Parameters.AddWithValue("@n", n);
			cmd.Parameters.AddWithValue("@str", str);
			cmd.Parameters.AddWithValue("@stuff", stuff);
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            string s = null;
            foreach (DataRow dr in dt.Rows)
            {
                s = dr["Name"].ToString();
            }
            con.Close();
            //Context.Response.Write(JsonConvert.SerializeObject(dt));
            return s;
        }
    }
    [WebMethod]
    public void Getulogin(string username,string password)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spulogin";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            Context.Response.Write(JsonConvert.SerializeObject(dt));
        }
    }
  
    public DataTable builditems(string data)
    {
        using (SqlConnection oConn = new SqlConnection(strConn))
        {
            oConn.Open();
            string strQuery = data;
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, oConn);
            // Fill the dataset.
            oAdapter.Fill(dt);
            oConn.Close();
            return dt;
        }
    }
    [WebMethod]
    public string GetDetails(string name, int age)
    {
        //return string.Format("Name: {0}{2}Age: {1}{2}TimeStamp: {3}", name, age, Environment.NewLine, DateTime.Now.ToString());
		string NowPeriodDate = null;
		System.Globalization.CultureInfo USCulture = new System.Globalization.CultureInfo("en-US", true);
        NowPeriodDate = DateTime.Now.ToString("yyyyMMdd", USCulture);
        return NowPeriodDate;
    }
    [WebMethod()]
    public void GetExport(string Id)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spexport";
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            Context.Response.Write(JsonConvert.SerializeObject(dt));
        }
    }
    //[WebMethod()]
    //public string[] QueryData(string strtable)
    //{
    //    int i = 0;
    //    string sql = null;
    //    SqlConnection oConn = new SqlConnection(strConn);
    //    oConn.Open();
    //    sql = "SELECT * FROM " + strtable;
    //    System.Data.DataSet oDataset = new System.Data.DataSet();
    //    SqlDataAdapter oAdapter = new SqlDataAdapter(sql, oConn);
    //    oAdapter.Fill(oDataset);
    //    string[] s = new string[oDataset.Tables[0].Rows.Count];
    //    for (i = 0; i <= oDataset.Tables[0].Rows.Count - 1; i++)
    //    {
    //        s[i] = oDataset.Tables[0].Rows[i].ItemArray[0].ToString();
    //    }
    //    oConn.Close();
    //    return s;
    //}
    [WebMethod()]
    public DataSet GetDataSet(string sName)
    {
        var args = sName.ToString().Split('|');
        switch (args[0])
        {
            case "Problem":
                sName = "(select Title from tblProblem  a where a.Location like '@'+(select location from tbltype where  substring(Title,1,3) =  '" + args[1] + "')+'@')#a";
                break;
            case "Location":
                sName = "(select value from dbo.FNC_SPLIT((select isnull(b.Title,'')Title from tbltype a left join tbllocation b on b.Id=a.location where substring(a.Title,1,3) = '" + args[1] + "'),';'))#a";
                break;
        }
                using (SqlConnection oConn = new SqlConnection(strConn))
        {
            oConn.Open();
            sName = sName.Replace("@", "%");
            string strQuery = "select * from " + sName;
            DataSet oDataset = new DataSet();
            SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, oConn);
            // Fill the dataset.
            oAdapter.Fill(oDataset);
            oConn.Close();
            return oDataset;
        }
    }
    [WebMethod()]
    public void Getjson(string sName)
    {
        using (SqlConnection oConn = new SqlConnection(strConn))
        {
            oConn.Open();
            sName = sName.Replace("@","%");
            string strQuery = "select * from " + sName;
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, oConn);
            // Fill the dataset.
            oAdapter.Fill(dt);
            oConn.Close();
            Context.Response.Write(JsonConvert.SerializeObject(dt)); 
        }
    }
  [WebMethod()]
    public void savedocument(string data)
    {
        string datapath = "~/Content/" + data + ".json";
        using (StreamReader sr = new StreamReader(Server.MapPath(datapath)))
        {
            string json = sr.ReadToEnd();
            string sp = "spsavedocument";
            List<RootObject> ro = JsonConvert.DeserializeObject<List<RootObject>>(json);
            //Context.Response.Write(ro[0].Requester);
            using (SqlConnection con = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = new SqlCommand(sp))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        DataTable dt = new DataTable();
                        cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@Id", ro[0].ID);
                        cmd.Parameters.AddWithValue("@ncptype", ro[0].ncptype);
                        cmd.Parameters.AddWithValue("@ncpid", ro[0].ncpid);
                        cmd.Parameters.AddWithValue("@Problem", ro[0].Problem);
                        cmd.Parameters.AddWithValue("@FirstDecision", ro[0].FirstDecision);
                        cmd.Parameters.AddWithValue("@Decision", ro[0].Decision);
                        cmd.Parameters.AddWithValue("@KeyDate", ro[0].KeyDate);
                        cmd.Parameters.AddWithValue("@Location", ro[0].Location);
                        cmd.Parameters.AddWithValue("@Plant", ro[0].Plant);
						cmd.Parameters.AddWithValue("@MaterialType", ro[0].MaterialType);
                        cmd.Parameters.AddWithValue("@BatchCode", ro[0].BatchCode);
                        cmd.Parameters.AddWithValue("@Product", ro[0].Product);
                        cmd.Parameters.AddWithValue("@Batchsap", ro[0].Batchsap);
                        cmd.Parameters.AddWithValue("@Active", ro[0].Active);
                        cmd.Parameters.AddWithValue("@Material", ro[0].Material);
                        cmd.Parameters.AddWithValue("@ProductionDate", ro[0].ProductionDate);
                        cmd.Parameters.AddWithValue("@Quantity", ro[0].Quantity);
                        cmd.Parameters.AddWithValue("@Shift", ro[0].Shift);
                        cmd.Parameters.AddWithValue("@HoldQuantity", ro[0].HoldQuantity);
                        cmd.Parameters.AddWithValue("@Action", ro[0].Action);
                        cmd.Parameters.AddWithValue("@Remark", ro[0].Remark);
                        cmd.Parameters.AddWithValue("@Approve", ro[0].Approve);
                        cmd.Parameters.AddWithValue("@Approvefinal", ro[0].Approvefinal);
						cmd.Parameters.AddWithValue("@user", ro[0].user);
                        cmd.Parameters.AddWithValue("@LinesNo", ro[0].LinesNo);
						cmd.Parameters.AddWithValue("@ShiftOption", ro[0].ShiftOption);
                        cmd.Parameters.AddWithValue("@Times", ro[0].Times);
                        cmd.Parameters.AddWithValue("@ResultDecision", ro[0].ResultDecision);
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);
						//System.IO.File.Delete(Server.MapPath(datapath));
						//string Path = Server.MapPath(datapath);
						//	if (File.Exists(Path))
						//	{
						//		File.Delete(Path);
						//	}
                        Context.Response.Write(JsonConvert.SerializeObject(dt));
                    }
                }
            }
        }
	}
    [WebMethod]
    public void Getpersonal(string sName,string sCondition,string sCurrent)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spGetpersonal";
            cmd.Parameters.AddWithValue("@name", sName);
			cmd.Parameters.AddWithValue("@current", sCurrent);
			cmd.Parameters.AddWithValue("@condition", sCondition);
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            Context.Response.Write(JsonConvert.SerializeObject(dt));
        }
    }
      [WebMethod()]
    public string saveCreateroot(string data)
    {
        string sp = "spCreateDocument";
        string datapath = "~/Content/" + data + ".json";
        using (StreamReader sr = new StreamReader(Server.MapPath(datapath)))
        {
            string json = sr.ReadToEnd();
            dynamic dynJson = JsonConvert.DeserializeObject(json);
            foreach (var item in dynJson)
            {
                //Console.WriteLine("{0} {1} {2} {3}\n", item.Id, item.Material,
                //    item.ProductName, item.user);
                sp = item.Material;
            }
            //List<CreateDocument> ro = JsonConvert.DeserializeObject<List<CreateDocument>>(json);
            ////Context.Response.Write(ro[0].Requester);
            //using (SqlConnection con = new SqlConnection(strConn))
            //{
            //    using (SqlCommand cmd = new SqlCommand(sp))
            //    {
            //        using (SqlDataAdapter sda = new SqlDataAdapter())
            //        {
            //            DataTable dt = new DataTable();
            //            cmd.CommandType = CommandType.StoredProcedure;
            //            cmd.Parameters.AddWithValue("@CreateBy", ro[0].CreateBy);
            //            cmd.Parameters.AddWithValue("@Code", ro[0].Code);
            //            cmd.Parameters.AddWithValue("@Condition", ro[0].Condition);
            //            cmd.Connection = con;
            //            sda.SelectCommand = cmd;
            //            sda.Fill(dt);
            //            Context.Response.Write(JsonConvert.SerializeObject(dt));
            //        }
            //    }
            //}
        }
        return sp;
    }
    [WebMethod]
    public void savechangeresult(string data)
    {
		string datapath = "~/Content/" + data + ".json";
        using (StreamReader sr = new StreamReader(Server.MapPath(datapath)))
        {
            string json = sr.ReadToEnd();
            dynamic dynJson = JsonConvert.DeserializeObject(json);
            foreach (var item in dynJson)
            {
                using (SqlConnection con = new SqlConnection(strConn))
                {
                    string query = "insert into tblChangeResult values (@MatDoc,@Name,@Result,@ActiveBy,format(Getdate(),'dd-MMM-yyyy HH:mm:ss'))";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Name", item.Name.ToString());
                        cmd.Parameters.AddWithValue("@Result", item.Result.ToString());
                        cmd.Parameters.AddWithValue("@MatDoc", item.Matdoc.ToString());
                        cmd.Parameters.AddWithValue("@ActiveBy", item.Activeby.ToString());
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
        }
        Context.Response.Write("success");
    }
}

//public class CreateDocument
//{
//public string CreateBy { get; set; }
//public string Condition { get; set; }
//public string Code { get; set; }
//}


public class Objattachment
{
    public int MatDoc { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public byte[] Data { get; set; }
    public string ActiveBy { get; set; }
}
 
public class RootObject
{
	public string ID { get; set; }
    public string ncptype { get; set; }
    public string ncpid { get; set; }
    public string Problem { get; set; }
    public string FirstDecision { get; set; }
    public string Decision { get; set; }
    public string KeyDate { get; set; }
    public string Location { get; set; }
    public string Plant { get; set; }
    public string BatchCode { get; set; }
    public string Product { get; set; }
    public string Batchsap { get; set; }
    public string Active { get; set; }
    public string Material { get; set; }
	public string MaterialType {get; set;}
    public string ProductionDate { get; set; }
    public string Quantity { get; set; }
    public string Shift { get; set; }
    public string HoldQuantity { get; set; }
    public string Action { get; set; }
    public string Remark { get; set; }
    public string Approve { get; set; }
    public string Approvefinal { get; set; }
	public string user { get; set; }
    public string LinesNo { get; set; }
	public string ShiftOption { get; set; }
    public string Times { get; set; }
    public string ResultDecision { get; set; }
}
