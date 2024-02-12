using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UpdatePanel_Auto_Refresh_ExportToExcel : System.Web.UI.Page
{
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    MyDataModule cs = new MyDataModule();
    protected void Page_Load(object sender, EventArgs e)
    {
        ExportTo("");
    }
    void ExportTo(string Data)
    {
        var workbook = new XLWorkbook(); int col=2;
        //col = 4;
        //string[] array = { "zpm1", "", "X", "103","ex","203" };
        var worksheet = workbook.Worksheets.Add("VK11_20180604_095920_");
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
        SqlParameter[] param = { new SqlParameter("@user",string.Format("{0}",user_name))};
        Results = cs.GetRelatedResources("spGetDataSendToSAP", param);
        foreach (DataRow row in Results.Rows){
            var i = col++;
            worksheet.Cell("D"+i).Value = "zpm1";
            worksheet.Cell("F" + i).Value = "X";
            worksheet.Cell("G" + i).Value = "103";
            worksheet.Cell("H" + i).Value = "EX";
            worksheet.Cell("I" + i).Value = "203";
            worksheet.Cell("J" + i).Value = row["SoldTo"].ToString();
            worksheet.Cell("K" + i).Value = row["ShipTo"].ToString();
            worksheet.Cell("L" + i).Value = row["Incoterm"].ToString();
            worksheet.Cell("M" + i).Value = row["PaymentTerm"].ToString();
            worksheet.Cell("N" + i).Value = row["Currency"].ToString();
            worksheet.Cell("O" + i).Value = row["SalesUnit"].ToString();
            worksheet.Cell("P" + i).Value = row["Code"].ToString();
            worksheet.Cell("Q" + i).Value = row["OfferPrice"].ToString();
            worksheet.Cell("R" + i).Value = row["Currency"].ToString();
            worksheet.Cell("S" + i).Value = row["PricingUnit"].ToString();
            worksheet.Cell("T" + i).Value = row["SalesUnit"].ToString();
            worksheet.Cell("U" + i).Value = string.Format("'{0}",row["RequestDate"].ToString());
            worksheet.Cell("V" + i).Value = string.Format("'{0}", row["RequireDate"].ToString());
            worksheet.Cell("W" + i).Value = row["CostNo"].ToString();
        }
        //workbook.SaveAs(@"C:\temp\HelloWorld.xlsx");
        workbook.SaveAs(@"C:\\Users\fo5910155\Documents\Winshuttle\Studio\Data\VK11_20180604_095920_.xlsx");
        MinPrice(Results);
        //Server.MapPath("~/App_Data/Documents/HelloWorld.xlsx"));
        Context.Response.Write(Data);
    }
    void MinPrice(DataTable dt)
    {
        var workbook = new XLWorkbook(); int col = 2;
        var worksheet = workbook.Worksheets.Add("VK11_20180604_094847");
        //MinPrice
        foreach (DataRow row in dt.Rows)
        {
            var i = col++;
            worksheet.Cell("D" + i).Value = "zpm2";
            worksheet.Cell("F" + i).Value = "X";
            worksheet.Cell("G" + i).Value = "103";
            worksheet.Cell("H" + i).Value = "EX";
            worksheet.Cell("I" + i).Value = row["Currency"].ToString();
            worksheet.Cell("J" + i).Value = row["SalesUnit"].ToString();
            worksheet.Cell("K" + i).Value = row["Code"].ToString();
            worksheet.Cell("L" + i).Value = row["MinPrice"].ToString();
            worksheet.Cell("M" + i).Value = row["Currency"].ToString();
            worksheet.Cell("N" + i).Value = row["PricingUnit"].ToString();
            worksheet.Cell("O" + i).Value = row["SalesUnit"].ToString();
            worksheet.Cell("P" + i).Value = string.Format("'{0}", row["RequestDate"].ToString());
            worksheet.Cell("Q" + i).Value = string.Format("'{0}", row["RequireDate"].ToString());
            worksheet.Cell("R" + i).Value = row["CostNo"].ToString();
        }
        //workbook.SaveAs(@"C:\temp\MinPrice.xlsx");
        workbook.SaveAs(@"C:\\Users\fo5910155\Documents\Winshuttle\Studio\Data\VK11_20180604_094847.xlsx");
    }
}