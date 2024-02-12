using DevExpress.Spreadsheet;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UpdatePanel_Auto_Refresh_Default : System.Web.UI.Page
{
    Worksheet worksheet;
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string[] arr = new string[] { "grand total", "sub total", "loss", "cost per case fob bangkok", "remark" };
    string[] SubType = Regex.Split(@"raw materials;ingredients;packaging;primary;secondary;labour & overhead;upcharge;up charge;margin", ";");//;
    List<string> mylist; List<string> mynote;
    string FilePath
    {
        get { return Session["FilePath"] == null ? String.Empty : Session["FilePath"].ToString(); }
        set { Session["FilePath"] = value; }
    }
    string strcom = "103"; string user = "999"; string _Currency = "THB";
    protected void Page_Load(object sender, EventArgs e)
    {
        bool IsPageRefresh = false;
        //if (!IsPostBack)
        //{
        //    ViewState["ViewStateId"] = System.Guid.NewGuid().ToString();
        //    Session["SessionId"] = ViewState["ViewStateId"].ToString();
        //}
        //else
        //{
        //    if (ViewState["ViewStateId"].ToString() != Session["SessionId"].ToString())
        //    {
        //        IsPageRefresh = true;
        //    }

        //    Session["SessionId"] = System.Guid.NewGuid().ToString();
        //    ViewState["ViewStateId"] = Session["SessionId"].ToString();
        //}
    }
    public void runtest()
    {
        //if (!IsPostBack)
        //{       
        //    FilePath = DateTime.Now.ToString("yyyyMMddHHmmss");
        //    CreateIfMissing(FilePath);
        //    SetInitialRow();
        //}


        //this section of code checks if the page postback is due to genuine submit by user or by pressing "refresh"

        ExpireAllCookies();
        FilePath = DateTime.Now.ToString("yyyyMMddHHmmss");
        CreateIfMissing(FilePath);
        SetInitialRow();
    }
    private void CreateIfMissing(string path)
    {
        var dir = Server.MapPath(@"~/ConvertedFiles/" + path);
        bool folderExists = Directory.Exists(dir);
        if (!folderExists)
            Directory.CreateDirectory(dir);
    }
    public void SetInitialRow()
    {
        var values = new[] { "P018", "P017", "P020", "P021", "P022" };
        var myvalues = new[] { "can", "pouch" };
        var dir = Server.MapPath(@"~/ExcelFiles"); bool testrun = true;
        List<string> listFiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
          .Where(file => new string[] { ".xls", ".xlsx" }
          .Contains(Path.GetExtension(file)))
          .ToList();
        string[] files = Directory.GetFiles(Server.MapPath(@"~/ExcelFiles"));
        foreach (string file in listFiles)
        {
            //foreach (string file in files)
            Spreadsheet.Document.LoadDocument(file);
            //worksheet = Spreadsheet.Document.Worksheets[0];
            IWorkbook workbook = Spreadsheet.Document; int i = 1;
            if (testrun)
                i = workbook.Worksheets.Count;
            for (int w = 0; w < i; w++)
            {
                worksheet = Spreadsheet.Document.Worksheets[w];
                if (values.Any(worksheet.Name.Contains) && strcom != "103") goto jumptoexitsheet;
                hfgetvalue["NewID"] = string.Format("{0}", cs.GetNewID());
                DataTable dt = new DataTable(); bool step = false;
                dt.Clear();
                dt.Columns.Add("subtype");
                dt.Columns.Add("Component");
                dt.Columns.Add("Name");
                dt.Columns.Add("Material");

                dt.Columns.Add("PriceOfUnit");
                dt.Columns.Add("yield");
                dt.Columns.Add("result");
                dt.Columns.Add("LBOh");
                dt.Columns.Add("BaseUnit");
                dt.Columns.Add("PriceOfCarton");
                dt.Columns.Add("Per");
                dt.Columns.Add("Remark");

                int n = 0; string mysubtype = ""; string myComponent = ""; int mycount = 0; int myinserttable = 0; string myfolio = "";
                //insert
                mylist = new List<string>();
                mynote = new List<string>();
                DataTable t = cs.builditems("select * from MasPrimary"); string myPackaging = "";
                foreach (DataRow dr in t.Rows)
                {
                    if (worksheet["B7"].Value.ToString().ToLower().Contains(dr["Name"].ToString().ToLower()))
                    {
                        myPackaging = dr["Name"].ToString();
                        break;
                    }
                }
                if (testrun)
                {
                    string c = "";
                    if (worksheet.Cells["A3"].Value.ToString().ToLower().Trim().Contains("uspn") || worksheet.Cells["A1"].Value.ToString().ToLower().Trim().Contains("tum"))
                        c = string.Format("{0}", "H");
                    else
                        c = string.Format("{0}", "G");
                    var namesheet = "";
                    if (worksheet.Name.Length > 1)
                        namesheet = worksheet.Name.Substring(0, 2) == "GP" || worksheet.Name.Substring(0, 2) == "PF" ? worksheet.Name : "";
                    var costno = worksheet.Cells[c + "9"].Value.ToString();
                    if (namesheet == worksheet["B6"].Value.ToString())
                        namesheet = "";
                    string converted = convert_Number(worksheet["B8"].Value.ToString());
                    SqlParameter[] param = { new SqlParameter("@Company", strcom),// worksheet["G3"].Value.ToString().ToLower().Contains("scc")?"103":"102"),
                new SqlParameter("@Id",string.Format("{0}",0)),
                new SqlParameter("@RequestNo",string.Format("{0}",0)),
                new SqlParameter("@CreateBy", string.Format("{0}",user)),
                new SqlParameter("@NewID", hfgetvalue["NewID"].ToString()),
                new SqlParameter("@Remark",string.Format("{0}","")),
                new SqlParameter("@StatusApp", string.Format("{0}", 4)),
                new SqlParameter("@MarketingNumber",string.Format("{0}", worksheet["B6"].Value.ToString() == ""? costno :"")),
				//new SqlParameter("@MarketingNumber",string.Format("{0}", "")),
                new SqlParameter("@RDNumber",worksheet.Cells[c + "10"].Value.ToString()),
                new SqlParameter("@CanSize",worksheet["B7"].Value.ToString()),
                new SqlParameter("@Netweight",string.Format("{0}|Grams", converted)),
                new SqlParameter("@Packaging",myPackaging==""?"can":myPackaging.ToString()),
                new SqlParameter("@ExchangeRate",worksheet["H11"].Value.ToString()),
                new SqlParameter("@Customer", worksheet["B4"].Value.ToString()),
                new SqlParameter("@PackSize",worksheet["B10"].Value.ToString())};
                    var myresult = cs.GetRelatedResources("spinsertCosting", param);
                    foreach (DataRow dr in myresult.Rows)
                    {
                        myfolio = dr["Id"].ToString();
                        using (SqlConnection con = new SqlConnection(strConn))
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = @"spinsertFormulaHeader";
                            cmd.Parameters.AddWithValue("@RequestNo", dr["Id"].ToString());
                            cmd.Parameters.AddWithValue("@Costper", "0");
                            cmd.Parameters.AddWithValue("@Id", "0");
                            cmd.Parameters.AddWithValue("@Formula", string.Format("{0}", "1"));
                            cmd.Parameters.AddWithValue("@Code", worksheet["B6"].Value.ToString().Trim());
                            cmd.Parameters.AddWithValue("@Customer", worksheet["B4"].Value.ToString());
                            cmd.Parameters.AddWithValue("@Name", worksheet["B5"].Value.ToString());
                            cmd.Parameters.AddWithValue("@RefSamples", worksheet.Cells[c + "10"].Value.ToString());
                            //cmd.Parameters.AddWithValue("@CostNo", string.Format("{0}", costno == ""?namesheet: worksheet.Cells[c + "9"].Value));
                            cmd.Parameters.AddWithValue("@CostNo", string.Format("{0}", worksheet["B6"].Value.ToString() == "" ? costno : ""));
                            //cmd.Parameters.AddWithValue("@CostNo", string.Format("{0}", ""));
                            cmd.Parameters.AddWithValue("@Revised", string.Format("{0}", 0));
                            cmd.Parameters.AddWithValue("@Mark", string.Format("{0}", "X"));
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                //Range range = worksheet.Range["A101:I200"];
                //worksheet.Range["A1:I100"].CopyFrom(range, PasteSpecial.All & ~PasteSpecial.Formulas);
                worksheet.Range["A1:C100"].CopyFrom(worksheet.Range["A1:C100"], PasteSpecial.Values);
                bool lboh = true; int l = 0; int ixx = 0; bool a = false; int xx = 0;
                do
                {
                    ++ixx;
                    if (worksheet.Cells["A" + ixx].Value.ToString().ToLower().Trim().Contains("loss"))
                        mylist.Add(string.Format("{0}", worksheet.Cells["B" + ixx].Value.ToString()));
                    if (worksheet.Cells["A" + ixx].Value.ToString().ToLower().Trim().Contains("labour & overhead") && l == 0)
                        l = ixx;
                    if (worksheet.Cells["A" + ixx].Value.ToString().ToLower().Trim().Contains("remark"))
                    {
                        int ix = ixx;
                        do
                        {
                            mynote.Add(string.Format("{0}", worksheet.Cells["B" + ix].Value.ToString()));
                            ++ix;
                            a = true;
                        } while (!string.IsNullOrEmpty(worksheet.Cells["A" + ix].Value.ToString()));
                    }
                    if (ixx > 500) goto jumptoexitsheet;
                }
                while (a == false);
                do
                {
                    Console.WriteLine(n);
                    // Add one to number.
                    n++;
                    string input = worksheet.Cells["G" + n].Value.ToString(); //DataTable myresult; SqlParameter[] param;
                    double output;
                    bool result = double.TryParse(input, out output);
                    if (result == false)
                        myComponent = worksheet.Cells["A" + n].Value.ToString();
                    if (worksheet.Cells["A" + n].Value.ToString().ToLower().Trim().StartsWith("description"))
                        step = true;
                    //int index = Array.IndexOf(SubType, worksheet.Cells["A" + n].Value.ToString().ToLower().Trim());
                    if (step)
                    {
                        foreach (string ar in SubType)
                        {
                            //var index = Array.FindIndex(SubType, x => x.Contains(worksheet.Cells["A" + n].Value.ToString().ToLower().Trim()));
                            //if (index > -1)
                            if (worksheet.Cells["A" + n].Value.ToString().ToLower().Trim().Contains(ar))
                            {
                                var index = Array.FindIndex(SubType, x => x.Contains(ar));
                                if (ar.Contains("packaging"))
                                {
                                    switch (mycount)
                                    {
                                        case 0:
                                            mysubtype = "primary packaging";
                                            break;
                                        case 1:
                                            mysubtype = "secondary packaging";
                                            break;
                                    }
                                    ++mycount;
                                }
                                else if (ar.Contains("up charge"))
                                {
                                    mysubtype = "upcharge";
                                    break;
                                }
                                else
                                    mysubtype = SubType[index];
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(mysubtype))
                            mysubtype = "raw materials";
                    }
                    //jumptoexit:
                    if (step && (result))
                    {
                        bool c = true;
                        foreach (string s in arr)
                            if (worksheet.Cells["A" + n].Value.ToString().ToLower().Trim().StartsWith(s))
                                c = false;
                        if (c)
                        {
                            if (mysubtype.Contains("labour & overhead")) goto jumptoexit;
                            DataRow _ravi = dt.NewRow();//
                            double total; double BaseUnit; double Yield; double Packsize;
                            if (lboh)
                            {
                                ++l;
                                _ravi["LBOh"] = worksheet.Cells["G" + l].Value.ToString(); ;// worksheet.Name.ToString();
                                int lx = l + 1;
                                if (worksheet.Cells["A" + lx].Value.ToString() == "")
                                    lboh = false;
                            }
                            if (xx < mynote.Count)
                                _ravi["Remark"] = mynote[xx]; ++xx;
                            _ravi["Name"] = worksheet.Cells["A" + n].Value.ToString();
                            _ravi["Material"] = worksheet.Cells["B" + n].Value.ToString();
                            if (mysubtype.Contains("ingredients"))
                                _ravi["subtype"] = "Ingredient";
                            else
                                _ravi["subtype"] = mysubtype.ToString() == "" || mysubtype.Contains("raw materials") ? "Raw Material" : mysubtype.ToString();
                            _ravi["Component"] = myComponent.ToString();

                            if (worksheet.Cells["A3"].Value.ToString().ToLower().Trim().Contains("uspn"))
                            {
                                _ravi["result"] = mysubtype.Contains("packaging") ? worksheet.Cells["F" + n].Value.ToString() : worksheet.Cells["D" + n].Value.ToString();
                                _ravi["PriceOfUnit"] = worksheet.Cells["C" + n].Value.ToString();
                                _ravi["yield"] = worksheet.Cells["E" + n].Value.ToString();
                                if (mysubtype.ToString().ToLower().Trim().Contains("upcharge"))
                                {
                                    _ravi["result"] = "1";
                                    _ravi["PriceOfUnit"] = worksheet.Cells["G" + n].Value.ToString();
                                    _ravi["PriceOfCarton"] = _ravi["PriceOfUnit"];
                                }
                                else if (mysubtype.ToString().ToLower().Trim().Contains("margin"))
                                {
                                    double per;
                                    double.TryParse(worksheet.Cells["B" + n].Value.ToString(), out per);
                                    _ravi["Per"] = Convert.ToDouble(per * 100).ToString();
                                    _ravi["Material"] = "";
                                    _ravi["result"] = "";
                                    _ravi["Component"] = mysubtype.ToString();
                                    _ravi["PriceOfCarton"] = "0";
                                }
                            }
                            else if (worksheet.Cells["A1"].Value.ToString().ToLower().Trim().Contains("tum"))
                            {
                                if (mysubtype.ToString().ToLower().Trim().Contains("margin"))
                                {
                                    double per;
                                    double.TryParse(worksheet.Cells["B" + n].Value.ToString(), out per);
                                    _ravi["Per"] = Convert.ToDouble(per * 100).ToString();
                                    _ravi["Material"] = "";
                                    _ravi["result"] = "";
                                    _ravi["Component"] = mysubtype.ToString();
                                    _ravi["PriceOfCarton"] = "0";
                                }
                                else
                                    _ravi["result"] = mysubtype.Contains("packaging") ? worksheet.Cells["F" + n].Value.ToString() : worksheet.Cells["D" + n].Value.ToString();
                                var _loss = worksheet.Cells["G" + n].Formula.ToString().Split('/');
                                if (_loss.Length == 2)
                                {
                                    double _yield;
                                    double.TryParse(worksheet.Cells["E" + n].Value.ToString(), out _yield);
                                    _ravi["yield"] = string.Format("{0}", _yield * Convert.ToDouble(_loss[1]));
                                    worksheet.Cells["G" + n].Formula = _loss[0];
                                }
                                else
                                    _ravi["yield"] = worksheet.Cells["E" + n].Value.ToString();
                                _Currency = "THB";
                                if (worksheet.Cells["G" + n].Formula.ToString().Contains("*$H$11") || worksheet.Cells["G" + n].Formula.ToString().Contains("*H11"))
                                    _Currency = "USD";
                                //    double _PriceOfUnit;double _Exchage;
                                //    double.TryParse(worksheet.Cells["C" + n].Value.ToString(), out _PriceOfUnit);
                                //    double.TryParse(worksheet.Cells["H11"].Value.ToString(), out _Exchage);
                                //_ravi["PriceOfUnit"] = Convert.ToDouble(_PriceOfUnit * _Exchage).ToString();                                   
                                _ravi["PriceOfUnit"] = worksheet.Cells["C" + n].Value.ToString();
                                //_ravi["result"] = worksheet.Cells["D" + n].Value.ToString();
                            }
                            else
                            {
                                _ravi["result"] = mysubtype.Contains("packaging") ? worksheet.Cells["E" + n].Value.ToString() : worksheet.Cells["C" + n].Value.ToString();
                                _ravi["PriceOfUnit"] = worksheet.Cells["F" + n].Value.ToString();
                                _ravi["yield"] = worksheet.Cells["D" + n].Value.ToString();
                                if (mysubtype.ToString().ToLower().Trim().Contains("upcharge"))
                                {
                                    _ravi["result"] = "1";
                                    _ravi["PriceOfUnit"] = worksheet.Cells["G" + n].Value.ToString();
                                    _ravi["PriceOfCarton"] = _ravi["PriceOfUnit"];
                                }
                                else if (mysubtype.ToString().ToLower().Trim().Contains("margin"))
                                {
                                    double per;
                                    double.TryParse(worksheet.Cells["B" + n].Value.ToString(), out per);
                                    _ravi["Per"] = Convert.ToDouble(per * 100).ToString();
                                    _ravi["Material"] = "";
                                    _ravi["result"] = "";
                                    _ravi["Component"] = mysubtype.ToString();
                                    _ravi["PriceOfCarton"] = "0";
                                }
                            }
                            if (mysubtype.Contains("packaging"))
                            {
                                double.TryParse(_ravi["PriceOfUnit"].ToString(), out BaseUnit);
                                double.TryParse(_ravi["result"].ToString(), out total);
                                double r = (total * BaseUnit);
                                _ravi["PriceOfCarton"] = r.ToString();
                                //if (mysubtype.Contains("packaging"))
                                myinserttable = 1;
                            }
                            else if (mysubtype.Contains("raw materials") || mysubtype.Contains("ingredients") || mysubtype.ToLower().Contains("raw material"))
                            {
                                double.TryParse(worksheet.Cells["B10"].Value.ToString(), out Packsize);
                                double.TryParse(_ravi["PriceOfUnit"].ToString(), out BaseUnit);
                                double.TryParse(_ravi["Yield"].ToString(), out Yield);
                                double.TryParse(_ravi["result"].ToString(), out total);
                                double r = ((total / 1000) * Packsize / (Yield / 100));
                                if (double.IsNaN(r) || double.IsInfinity(r))
                                    _ravi["BaseUnit"] = "";
                                else
                                {
                                    _ravi["BaseUnit"] = BaseUnit.ToString();
                                    _ravi["PriceOfCarton"] = Convert.ToDouble(r * BaseUnit).ToString();
                                }
                            }
                            if (testrun)
                                switch (myinserttable)
                                {
                                    case 0:
                                        SqlParameter[] param = new SqlParameter[] {
                                        new SqlParameter("@Id", string.Format("{0}", 0)),
                                            new SqlParameter("@Component", _ravi["subtype"].ToString()),
                                            new SqlParameter("@SubType", _ravi["Component"].ToString().Contains("1. RAW MATERIALS :")?"":_ravi["Component"].ToString()),
                                            new SqlParameter("@Description", _ravi["Name"].ToString()),
                                            new SqlParameter("@Material", string.Format("{0}", _ravi["Material"].ToString())),
                                            new SqlParameter("@Result", _ravi["result"].ToString()),
                                            new SqlParameter("@Yield", _ravi["yield"].ToString()),
                                            new SqlParameter("@RawMaterial", string.Format("{0}", "")),
                                            new SqlParameter("@Name", string.Format("{0}", "")),
                                            new SqlParameter("@PriceOfUnit", string.Format("{0}", _ravi["PriceOfUnit"].ToString())),
                                            new SqlParameter("@Currency", string.Format("{0}", _Currency)),
                                            new SqlParameter("@Unit", string.Format("{0}", "KG")),
                                            new SqlParameter("@ExchangeRate", string.Format("{0}", "1")),
                                            new SqlParameter("@BaseUnit", string.Format("{0}", _ravi["BaseUnit"].ToString())),
                                            new SqlParameter("@PriceOfCarton", string.Format("{0}", _ravi["PriceOfCarton"].ToString())),
                                            new SqlParameter("@Formula", string.Format("{0}", "1")),
                                            new SqlParameter("@IsActive", string.Format("{0}", "0")),
                                            new SqlParameter("@RequestNo", myfolio.ToString()),
                                            new SqlParameter("@user", string.Format("{0}", user)),
                                            new SqlParameter("@Remark", string.Format("{0}", "")),
                                            new SqlParameter("@LBOh", string.Format("{0}", "")),
                                            new SqlParameter("@Mark", string.Format("{0}", "X")),
                                            new SqlParameter("@LBRate", string.Format("{0}", _ravi["LBOh"].ToString()))};
                                        var myresult = cs.GetRelatedResources("spInsertFormula", param);
                                        break;
                                    case 1:
                                        double d; string strLoss = "0";
                                        int myc = mylist.Count; string str = "";
                                        if (myc > 2)
                                            str = mylist[_ravi["Component"].ToString() == "primary packaging" ? 1 : 2];
                                        if (Double.TryParse(str, out d)) // if done, then is a number
                                        {
                                            if (!string.IsNullOrEmpty(_ravi["BaseUnit"].ToString()))
                                            {
                                                double ro = (Convert.ToDouble(_ravi["BaseUnit"]) * (d / 100));
                                                if (!double.IsNaN(ro) || !double.IsInfinity(ro))
                                                    strLoss = ro.ToString("F4");
                                            }
                                        }
                                        param = param = new SqlParameter[]{
                                        new SqlParameter("@Id", string.Format("{0}", 0)),
                                        new SqlParameter("@Component", _ravi["subtype"].ToString()),
                                        new SqlParameter("@SAPMaterial", string.Format("{0}", _ravi["Material"].ToString())),
                                        new SqlParameter("@Description", _ravi["Name"].ToString()),
                                        new SqlParameter("@Quantity", _ravi["result"].ToString()),
                                        new SqlParameter("@PriceUnit", _ravi["PriceOfUnit"].ToString()),
                                        new SqlParameter("@Amount", _ravi["PriceOfCarton"].ToString()==""?"0":_ravi["PriceOfCarton"].ToString()),
                                        new SqlParameter("@Per", _ravi["Per"].ToString()),
                                        new SqlParameter("@Loss", strLoss==""?"0":strLoss.ToString()),
                                        new SqlParameter("@SellingUnit", string.Format("{0}", "THB")),
                                        new SqlParameter("@Formula",string.Format("{0}", "1")),
                                        new SqlParameter("@CreateBy", string.Format("{0}", user)),
                                        new SqlParameter("@RequestNo", myfolio.ToString()),
                                        new SqlParameter("@Mark", string.Format("{0}", "X"))};
                                        myresult = cs.GetRelatedResources("spinsertCostingItems", param);
                                        break;
                                }
                            dt.Rows.Add(_ravi);
                        }
                    }
                jumptoexit:
                    worksheet.Cells["K" + n].Value = n;
                } while (worksheet.Cells["A" + n].Value.ToString().ToLower().Trim().StartsWith("remark") == false);
                testgrid.DataSource = dt;
                testgrid.DataBind();
            jumptoexitsheet:
                Response.Write("X");
            }
            Console.WriteLine(file);
            //myfile = "c:/my documents/my images/cars/a.jpg";
            //File.Move(file, Path.ChangeExtension(file, ".blah"));
            //string from =  Server.MapPath(@"~/ExcelFiles/" + file);
            string to = file.Replace("ExcelFiles", "ConvertedFiles/" + FilePath);
            if (testrun)
                File.Move(file, to); // Try to move
            //Response.Write("sucess");
            //Response.Redirect("~/UpdatePanel_Auto_Refresh/GetExcelFiles.aspx");
            if (Request.Cookies["YourCookies"] != null)
            {
                Response.Cookies["YourCookiesCookies"].Expires = DateTime.Now.AddDays(-1);
                Response.Redirect(Request.Url.AbsoluteUri);
            }
        }
        Console.Read();
    }
    private void ExpireAllCookies()
    {
        if (HttpContext.Current != null)
        {
            int cookieCount = HttpContext.Current.Request.Cookies.Count;
            for (var i = 0; i < cookieCount; i++)
            {
                var cookie = HttpContext.Current.Request.Cookies[i];
                if (cookie != null)
                {
                    var expiredCookie = new HttpCookie(cookie.Name)
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        Domain = cookie.Domain
                    };
                    HttpContext.Current.Response.Cookies.Add(expiredCookie); // overwrite it
                }
            }

            // clear cookies server side
            HttpContext.Current.Request.Cookies.Clear();
        }
    }

    protected void btSubmit_Click(object sender, EventArgs e)
    {
        strcom = CmbCompany.Text;
        user = strcom == "103" ? "CP4090166" : "mp001688";
        runtest();
    }
    string convert_Number(string str)
    {
        StringBuilder sb = new StringBuilder();
        sb.Clear();
        foreach (char c in str)
        {
            //if ((c >= '0' && c <= '9') || c == ' ' || c == '-')
            if (c >= '0' && c <= '9')
            {
                sb.Append(c);
            }
            else
                break;
        }
        return sb.ToString();
    }
}