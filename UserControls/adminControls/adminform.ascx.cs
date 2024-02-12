using DevExpress.Web;
using System;
using System.IO;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Text;

public partial class UserControls_adminControls_adminform : MasterUserControl
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
    string strcom = "103";
    string user = "999";
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    public DataTable _dataTable
    {
        get { return Context.Session["sessionKey"] == null ? null : (DataTable)Context.Session["sessionKey"]; }
        set
        {
            Context.Session["sessionKey"] = value;
        }
    }
    string selectedDataSource
    {
        get { return Session["selectedDataSource"] == null ? String.Empty : Session["selectedDataSource"].ToString(); }
        set { Session["selectedDataSource"] = value; }
    }
    protected string SearchText { get { return Utils.GetSearchText(Page); } }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            Session.Clear();
            this.Session["user_name"] = user_name;
            //selectedDataSource = string.Format("{0}", Request.QueryString["role"]);
            //string strSQL = @"select replace(Bu,';','|') as 'Bu' from ulogin where [user_name]='" + user_name + "'";
            hBu["BU"] = cs.GetData(user_name, "BU");
            usertp["usertype"] = string.Format("{0}", cs.GetData(user_name, "usertype"));
            editor["Name"] = cs.GetData(user_name, "userlevel") == "2" ? true : false;
            uploader["uploader"] = "";
        }
        Update();
    }
    public override void Update()
    {
        //gridData.DataSource = dsgv;
        //DataTable dt = ((DataView)dsgv.Select(DataSourceSelectArguments.Empty)).Table;
        grid.DataBind();
    }
    protected void PreviewPanel_Callback(object sender, CallbackEventArgsBase e)
    {
        var text = string.Format("<div align='center'><h1>Access denied</h1><br/>You are not authorized to access this page.</div>", e.Parameter);
        PreviewPanel.Controls.Add(new LiteralControl(text));
    }
    protected void testGrid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        object o = selectedDataSource;
        if (o == null) return;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        if (args[0] == "new")
            _dataTable = GetTable(o);
        if (args[0]=="upCost")
            _dataTable = GetGridData();
        if (args[0] == "post")
        {
            if (o.ToString() == "upCost"){
                strcom = CmbCompany.Text;
                user = strcom == "103" ? "CP4090166" : "mp001688";
                runtest();
            }
            else{
                if (_dataTable != null)
                {
                    truncate();
                    foreach (DataRow dr in _dataTable.Rows)
                    {
                        using (SqlConnection con = new SqlConnection(strConn))
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spInsertYield";
                            cmd.Parameters.AddWithValue("@Company", dr["Company"].ToString());
                            cmd.Parameters.AddWithValue("@Material", dr["Material"].ToString());
                            cmd.Parameters.AddWithValue("@Name", dr["Name"].ToString());
                            cmd.Parameters.AddWithValue("@RawMaterial", dr["RawMaterial"].ToString());
                            cmd.Parameters.AddWithValue("@Description", dr["Description"].ToString());
                            cmd.Parameters.AddWithValue("@Yield", dr["Yield"].ToString());
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
        }
        g.DataBind();
    }
    void truncate()
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format(@"delete MasYield where Company 
            in (select distinct value from dbo.FNC_SPLIT({0},'|'))", hBu["BU"].ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            con.Dispose();
        }
    }
     public string GetFilePath()
    {
        string _file = "/" + FilePath;
        return HttpContext.Current.Server.MapPath(@"~/ExcelFiles" + _file);
    }
    protected void Upload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
    {
        object o = selectedDataSource;
        string dirVirtualPath = @"C:\\temp";
        string dirPhysicalPath = dirVirtualPath;// Server.MapPath(dirVirtualPath);
        if (o.ToString() != "Upload") return;

    }
    //void BuildupCost()
    //{
    //    //var _uploader = uploader["uploader"];
    //    object o = selectedDataSource;
    //    string dirVirtualPath = @"C:\\temp";
    //    string dirPhysicalPath = dirVirtualPath;// Server.MapPath(dirVirtualPath);
    //    if (o.ToString()== "upCost")
    //    {
    //        testTable.Rows.Clear();
    //        FilePath = DateTime.Now.ToString("yyyyMMddHHmmss");
    //        dirVirtualPath = GetFilePath();
    //        dirPhysicalPath = dirVirtualPath;// Server.MapPath(dirVirtualPath);
    //        if (!Directory.Exists(dirPhysicalPath))
    //        {
    //            Directory.CreateDirectory(dirPhysicalPath);
    //        }
    //        string _fileName = e.UploadedFile.FileName;
    //        string _FilePath = Path.Combine(dirPhysicalPath, _fileName);
    //        e.UploadedFile.SaveAs(_FilePath);
    //        if (!string.IsNullOrEmpty(_FilePath))
    //        {
    //            object[] array = new object[7];
    //            long NextRowID = 1;
    //            string[] filePaths = Directory.GetFiles(GetFilePath());
    //            List<ListItem> files = new List<ListItem>();
    //            foreach (string filePath in filePaths)
    //            {
    //                files.Add(new ListItem(Path.GetFileName(filePath)));
    //                array[0] = string.Format("{0}", NextRowID++);
    //                array[1] = (Path.GetFileName(filePath)).ToString();
    //                testTable.Rows.Add(array);
    //            }
    //            //testGrid.DataSource = files;
    //            //testGrid.DataBind();
    //        }
    //        goto JumpTo;
    //    }
    //    //string dirVirtualPath = @"\\192.168.1.193\XlsTables";
    //    if (!Directory.Exists(dirPhysicalPath))
    //    {
    //        Directory.CreateDirectory(dirPhysicalPath);
    //    }
    //    string fileName = e.UploadedFile.FileName;
    //    FilePath = Path.Combine(dirPhysicalPath, fileName);
    //    e.UploadedFile.SaveAs(FilePath);
    //    if (!string.IsNullOrEmpty(FilePath))
    //    {
    //        Workbook book = new Workbook();
    //        book.InvalidFormatException += book_InvalidFormatException;
    //        book.LoadDocument(FilePath);
    //        int i = 0;//int NextRowID = 0; 
    //        testTable.Rows.Clear();
    //        long NextRowID = 1; 
    //        foreach (Worksheet sheet in book.Worksheets)
    //        {
    //            i++;
    //            Range range = sheet.GetUsedRange();
    //            DataTable table = sheet.CreateDataTable(range, false);
    //            DataTableExporter exporter = sheet.CreateDataTableExporter(range, table, false);
    //            exporter.CellValueConversionError += exporter_CellValueConversionError;
    //            exporter.Export();
    //            //testTable = table;
    //            foreach (DataRow dr in table.Rows)
    //            {
    //                if (dr[0].ToString() != "Company" && !string.IsNullOrEmpty(dr[0].ToString()))
    //                {
    //                    //DataRow _ravi = testTable.NewRow();
    //                    //_ravi["ID"] = string.Format("{0}", NextRowID++);
    //                    //_ravi["Company"] = dr["Column1"];
    //                    //_ravi["Yield"] = dr["Column7"];
    //                    //testTable.Rows.Add(_ravi);
    //                    object[] array = new object[7];
    //                    array[0] = string.Format("{0}", NextRowID++);
    //                    for (int a = 0; a < 6; a++)
    //                    {
    //                        array[a + 1] = string.Format("{0}", dr[a].ToString().Trim());
    //                    }
    //                    testTable.Rows.Add(array);
    //                }
    //            }
    //            if (string.Format("{0}", o.ToString()) == "Yield")
    //            {
    //                int n = 0;var Code="";
    //                foreach (DataRow dr in testTable.Rows) // search whole table
    //                {
    //                    DataRow[] rows = getmaterial(dr["RawMaterial"].ToString()).Select();
    //                    if (rows.Length > 0)
    //                    dr["RawMaterial"] = string.Format("{0}",rows[0]["code"].ToString());
    //                    if (string.Format("{0}",Code) != dr["RawMaterial"].ToString())
    //                    {
    //                        n = 0; Code = dr["RawMaterial"].ToString();
    //                    }
    //                    else
    //                        n++;
    //                    dr["Material"] = Code + n.ToString();
    //                    //dr["Material"] = "cde";
    //                }
    //            }
    //        }
    //    }
    //    JumpTo:
    //    testGrid.DataBind();
    //}
    public void runtest()
    {
        ExpireAllCookies();
        //FilePath = DateTime.Now.ToString("yyyyMMddHHmmss");
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
    private DataTable GetGridData()
    {
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[10] { new DataColumn("ID", typeof(int)),
                            new DataColumn("Files", typeof(string)),
                            new DataColumn("Code", typeof(string)),
                            new DataColumn("CostingSheet", typeof(string)),
                            new DataColumn("Result", typeof(string)),
                            new DataColumn("Series",typeof(string)),
        new DataColumn("Mark",typeof(string)),
        new DataColumn("StatusApp",typeof(string)),
        new DataColumn("SiteId",typeof(string)),
        new DataColumn("RequestNo",typeof(string))});
        dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
        return dt;
    }
    public void SetInitialRow()
    {
        var values = new[] { "P018", "P017", "P020", "P021", "P022" };
        var myvalues = new[] { "can", "pouch" };
        //var dir = Server.MapPath(@"~/ExcelFiles");bool testrun = true;
        var dir = GetFilePath(); bool testrun = true;
        List<string> listFiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
          .Where(file => new string[] { ".xls", ".xlsx" }
          .Contains(Path.GetExtension(file)))
          .ToList();
        //string[] files = Directory.GetFiles(Server.MapPath(@"~/ExcelFiles"));
        string[] files = Directory.GetFiles(GetFilePath());
        int NextRowID = 1;
        _dataTable.Rows.Clear();
        //DataTable dataTable = new DataTable();
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
                if (values.Any(worksheet.Name.Contains)) goto jumptoexitsheet;
                         object[] array = new object[10];
                array[0] = NextRowID++;
                array[1] = file.ToString();
                array[5] = worksheet.Name.ToString();
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
                    var namesheet = worksheet.Name;
                    var costno = worksheet.Cells[c + "9"].Value.ToString();
                    if (namesheet == worksheet["B6"].Value.ToString())
                        namesheet = "";
                    string converted = convert_Number(worksheet["B8"].Value.ToString());
                    string _value = string.Format("{0}", costno == "" ? namesheet : worksheet.Cells[c + "9"].Value);
                    if (string.Format("{0}", _value) != "")
                    {
                        var _split = _value.ToString().Substring(0, 2);
                        if (_split.ToString() == "GP" || _split.ToString() == "PF")
                        {
                            var _r = cs.ReadItems(@"select count(costno) from TransFormulaHeader where costno='"+ _value +"'");
                            if (_r.ToString() != "0")
                                goto jumptoexitsheet;
                        }
                        else
                            _value = "";
                    }
                    // worksheet["G3"].Value.ToString().ToLower().Contains("scc")?"103":"102"),
                    SqlParameter[] param = { new SqlParameter("@Company", strcom),
                new SqlParameter("@Id",string.Format("{0}",0)),
                new SqlParameter("@RequestNo",string.Format("{0}",0)),
                new SqlParameter("@CreateBy", string.Format("{0}",user)),
                new SqlParameter("@NewID", hfgetvalue["NewID"].ToString()),
                new SqlParameter("@Remark",string.Format("{0}","")),
                new SqlParameter("@StatusApp", string.Format("{0}", 4)),
                new SqlParameter("@MarketingNumber",string.Format("{0}", _value)),
                new SqlParameter("@RDNumber",worksheet.Cells[c + "10"].Value.ToString()),
                new SqlParameter("@CanSize",worksheet["B7"].Value.ToString()),
                new SqlParameter("@Netweight",string.Format("{0}|Grams", converted)),
                new SqlParameter("@Packaging",myPackaging==""?"can":myPackaging.ToString()),
                new SqlParameter("@ExchangeRate",worksheet["H11"].Value.ToString()),
                new SqlParameter("@PackSize",worksheet["B10"].Value.ToString())};
                    var myresult = cs.GetRelatedResources("spinsertCosting", param);
                    foreach (DataRow dr in myresult.Rows)
                    {
                        myfolio = dr["Id"].ToString();
                        array[9] = myfolio.ToString();
                        array[4] = dr["MarketingNumber"].ToString();
                        array[2] = worksheet["B6"].Value.ToString();
                        using (SqlConnection con = new SqlConnection(strConn))
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = @"spinsertFormulaHeader";
                            cmd.Parameters.AddWithValue("@RequestNo", dr["Id"].ToString());
                            cmd.Parameters.AddWithValue("@Costper", "0");
                            cmd.Parameters.AddWithValue("@Id", "0");
                            cmd.Parameters.AddWithValue("@Formula", string.Format("{0}", "1"));
                            cmd.Parameters.AddWithValue("@Code", worksheet["B6"].Value.ToString());
                            cmd.Parameters.AddWithValue("@Customer", worksheet["B4"].Value.ToString());
                            cmd.Parameters.AddWithValue("@Name", worksheet["B5"].Value.ToString());
                            cmd.Parameters.AddWithValue("@RefSamples", worksheet.Cells[c + "10"].Value.ToString());
                            cmd.Parameters.AddWithValue("@CostNo", string.Format("{0}", _value));
                            cmd.Parameters.AddWithValue("@Revised", string.Format("{0}", 0));
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
                    //Console.WriteLine(n);
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
                                            new SqlParameter("@Currency", string.Format("{0}", "THB")),
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
                _dataTable.Rows.Add(array);
                jumptoexitsheet:
                Response.Write("+");
            }
            //Console.WriteLine(file);
            //myfile = "c:/my documents/my images/cars/a.jpg";
            //File.Move(file, Path.ChangeExtension(file, ".blah"));
            //string from =  Server.MapPath(@"~/ExcelFiles/" + file);
            //string to = file.Replace("ExcelFiles", "ConvertedFiles" );
            //if (testrun)
            //    File.Move(file, to); // Try to move
            ////Response.Write("sucess");
            ////Response.Redirect("~/UpdatePanel_Auto_Refresh/GetExcelFiles.aspx");
            //if (Request.Cookies["YourCookies"] != null)
            //{
            //    Response.Cookies["YourCookiesCookies"].Expires = DateTime.Now.AddDays(-1);
            //    Response.Redirect(Request.Url.AbsoluteUri);
            //}
        }
        //Console.Read();
        //testgrid.DataSource = dt;
        //testgrid.DataBind();
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
     string convert_Number(string str)
    {
        StringBuilder sb = new StringBuilder();
        sb.Clear();
        foreach (char c in str)
        {
            if (c >= '0' && c <= '9')//if ((c >= '0' && c <= '9') || c == ' ' || c == '-')
                sb.Append(c);
            else
                break;
        }
        return sb.ToString();
    }
    DataTable getmaterial(string Id)
    {
        SqlParameter[] param = { new SqlParameter("@Material", Id)};
        DataTable table = new DataTable();
        var result = cs.GetRelatedResources("spGetOldMaterial", param);
        return result;
    }
    protected void testGrid_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.KeyFieldName = "ID";
        g.DataSource = LoadGrid;
        g.ForceDataRowType(typeof(DataRow));
    }
    private DataTable LoadGrid
    {
        get
        {
            if (_dataTable == null) return _dataTable;
            if (_dataTable != null)
            {
                testGrid.SettingsBehavior.AllowSelectByRowClick = false;
                testGrid.SettingsDataSecurity.AllowEdit = true;
            }
            int i = _dataTable.Rows.Count;
            //DataView dv = (DataView)dsMasYield.Select(DataSourceSelectArguments.Empty);
            //if (dv != null)
            //{
            //    testTable = dv.ToTable();
            //}
            return _dataTable;
        }
    }
    DataTable GetTable(object o)
    {
        // Here we create a DataTable with four columns.
        //Creating another DataTable to clone
        DataTable dt_clone = new DataTable(); 
        if (o == null) return dt_clone;
        switch (string.Format("{0}", o))
        {
            case "Yield":
            //default:
                //strSQL = @"select top 0 * from masYield";
                DataView dv = (DataView)dsMasYield.Select(DataSourceSelectArguments.Empty);
                DataTable dt = new DataTable();
                dt = dv.ToTable();
                dt.TableName = "CloneTable";
                dt_clone = dt.Clone();
                break;
        }
        //dt = cs.builditems(strSQL);
        //DataTable dt = new DataTable();
        //dt.Columns.AddRange(new DataColumn[6] { new DataColumn("ID", typeof(int)),
        //                    new DataColumn("RowID", typeof(string)),
        //                    new DataColumn("Code", typeof(string)),
        //                    new DataColumn("CostingSheet", typeof(string)),
        //                    new DataColumn("Result", typeof(string)),
        //                    new DataColumn("Series",typeof(string))});
        dt_clone.PrimaryKey = new DataColumn[] { dt_clone.Columns["ID"] };
        return dt_clone;
    }
    void book_InvalidFormatException(object sender, SpreadsheetInvalidFormatExceptionEventArgs e)
    {

    }
    void exporter_CellValueConversionError(object sender, CellValueConversionErrorEventArgs e)
    {
        e.Action = DataTableExporterAction.Continue;
        e.DataTableValue = null;
    }
    protected void grid_CustomCallback(object sender, DevExpress.Web.ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        DataRow dr = g.GetDataRow(g.FocusedRowIndex);
        //Session["selectedDataSource"] = Int32.Parse(e.Parameters);
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        if (args[0] == "Search")
        {
            string filter = args[0] == "Search" ? SearchText : string.Empty;
            //gridData.SearchPanelFilter = filter;
            //gridData.ExpandAll();
            if (filter == "")
            {
                grid.FilterExpression = "";
            }
            else
            {
                string fExpr = "";
                foreach (GridViewColumn column in grid.Columns)
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
                grid.FilterExpression = fExpr;
            }
        }
        if (args[0] == "reload")
        {
            selectedDataSource = string.Format("{0}", args[1].ToString());
        }
        if (args[0]=="post")
        {
            //var post = "XX";
            //post= string.Format("{0}", args[1].ToString()); 
        }
        //else
        //{
        //    grid.Columns.Clear();
        //    grid.AutoGenerateColumns = false;
        //}
        //int newPageSize;
        //if (e != null)
        //{
        //    if (!int.TryParse(e.Parameters, out newPageSize)) return;
        //    g.SettingsPager.PageSize = newPageSize;
        //    Session["gvPageSize"] = newPageSize;
        //}
        g.Columns.Clear();
        g.AutoGenerateColumns = false;
        g.DataBind();
    }

    protected void grid_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        g.DataSource = GetDataSource();
        AddColumns();
        
    }
    private void AddColumns()
    {
        grid.Columns.Clear();
        DataView dw = (DataView)GetDataSource().Select(DataSourceSelectArguments.Empty);
        if (dw == null)
            return;
        foreach (DataColumn c in dw.Table.Columns)
        {
            var str = c.ColumnName;
            //GridViewDataColumn col = new GridViewDataTextColumn();
            if (c.ColumnName.Contains("BU"))
            {
                GridViewDataDropDownEditColumn col = new GridViewDataDropDownEditColumn();
                col = new GridViewDataDropDownEditColumn();
                ((DropDownEditProperties)col.PropertiesEdit).DropDownWindowTemplate = new MyDropDownWindow();
                col.FieldName = c.ColumnName;
                //ASPxListBox listBox = de;
                //if (listBox != null)
                //{
                //    listBox.DataSource = lstColumns;
                //    listBox.TextField = "Name";
                //    listBox.ValueField = "Name";
                //    listBox.DataBind();
                //}
                grid.Columns.Add(col);

            }else if (c.ColumnName.Contains("Company"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.ValueField = "Code";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = dsCompany;
                grid.Columns.Add(cb);
            } else if (c.ColumnName.Contains("usertype"))
        {
            GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
            cb.FieldName = c.ColumnName;
            cb.PropertiesComboBox.Columns.Clear();
            cb.PropertiesComboBox.TextField = "Name";
            cb.PropertiesComboBox.ValueField = "Id";
            cb.PropertiesComboBox.TextFormatString = "{0}";
            cb.PropertiesComboBox.DataSource = dsusertype;
            grid.Columns.Add(cb);
        }
        else if (c.ColumnName=="userlevel"|| c.ColumnName=="sublevel")
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.ValueField = "Id";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = c.ColumnName.Contains("sublevel") ? dsSublevel:dsUserlevel;
                grid.Columns.Add(cb);
            }
            //else if (c.ColumnName.Contains("Sublevel"))
            //{
            //    GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
            //    cb.FieldName = c.ColumnName;
            //    cb.PropertiesComboBox.Columns.Clear();
            //    cb.PropertiesComboBox.TextField = "Name";
            //    cb.PropertiesComboBox.ValueField = "Id";
            //    cb.PropertiesComboBox.TextFormatString = "{0}";
            //    cb.PropertiesComboBox.DataSource=dsSublevel;
            //    grid.Columns.Add(cb);
            //}
            else if (c.ColumnName.Contains("PackageType") || c.ColumnName.Contains("tcode"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("empid"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("user_name"));
                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("firstname"));
                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("lastname"));
                cb.PropertiesComboBox.TextField = "user_name";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = dsulogin;
                cb.PropertiesComboBox.EnableCallbackMode = true;
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("Currency") || c.ColumnName.Contains("SubType") || c.ColumnName.Contains("Unit") || c.ColumnName.Contains("CurrencyTh"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "value";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("From") || c.ColumnName.Contains("To")|| c.ColumnName.Contains("Validfrom") || c.ColumnName.Contains("Validto"))
            {
                GridViewDataDateColumn colID = new GridViewDataDateColumn();
                colID.FieldName = c.ColumnName;
                colID.PropertiesDateEdit.EditFormat = EditFormat.Date;
                colID.PropertiesDateEdit.EditFormatString = "dd/MM/yyyy";
                colID.PropertiesEdit.DisplayFormatString = "dd/MM/yyyy";
                colID.PropertiesDateEdit.AllowNull = true;
                colID.PropertiesDateEdit.AllowUserInput = true;
                colID.PropertiesDateEdit.ConvertEmptyStringToNull = true;
                colID.PropertiesDateEdit.NullDisplayText = "";
                colID.PropertiesDateEdit.NullText = "";
                colID.PropertiesEdit.NullDisplayText = "";
                grid.Columns.Add(colID);
            }
            //else if (c.ColumnName.Contains("Customer"))
            //{
            //    GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
            //    cb.FieldName = c.ColumnName;
            //    cb.PropertiesComboBox.Columns.Clear();
            //    cb.PropertiesComboBox.TextField = "Name";
            //    cb.PropertiesComboBox.TextFormatString = "{0}";
            //    grid.Columns.Add(cb);
            //}
            else if (c.ColumnName.Contains("Customer") || c.ColumnName.Contains("ExchangeType"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.ValueField = "ID";
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = dsCustomerPrice;
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("MarginCode") || c.ColumnName.Contains("PercentMargin") || c.ColumnName.Contains("LBCode")) {
                GridViewDataTextColumn tc = new GridViewDataTextColumn();
                tc.FieldName = c.ColumnName;
                tc.EditFormSettings.Visible = DevExpress.Utils.DefaultBoolean.False;
                grid.Columns.Add(tc);
            }
            //else if (c.ColumnName.Contains("Material"))
            //{
            //    GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
            //    cb.FieldName = c.ColumnName;
            //    cb.PropertiesComboBox.Columns.Clear();
            //    cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("Material"));
            //    cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("Description"));
            //    cb.PropertiesComboBox.ValueField = "Material";
            //    cb.PropertiesComboBox.EnableCallbackMode = true;
            //    cb.PropertiesComboBox.CallbackPageSize = 10;
            //    cb.PropertiesComboBox.TextFormatString = "{0}";
            //    grid.Columns.Add(cb);
            //}
            else if (c.ColumnName.Contains("IsResign") || c.ColumnName.Contains("IsActive"))
            {
                GridViewDataCheckColumn cc = new GridViewDataCheckColumn();
                cc.FieldName = c.ColumnName;
                grid.Columns.Add(cc);
            }
            else
                AddTextColumn(c.ColumnName);
        }
        grid.KeyFieldName = dw.Table.Columns[0].ColumnName;
        grid.Columns[0].Visible = false;
        AddCommandColumn();
    }
    private void AddTextColumn(string fieldName)
    {
        GridViewDataTextColumn c = new GridViewDataTextColumn();
        c.FieldName = fieldName;
        if(fieldName.Contains("Email") || fieldName.Contains("Position")|| fieldName.Contains("Description"))
        c.Width = Unit.Pixel(300);
        grid.Columns.Add(c);
    }
    private void AddCommandColumn()
    {
        object o = selectedDataSource;
        if (o.ToString() == "Policy") return;
        SqlDataSource ds = (SqlDataSource)grid.DataSource;
        bool showColumn = !(String.IsNullOrEmpty(ds.UpdateCommand) && String.IsNullOrEmpty(ds.InsertCommand) &&
            String.IsNullOrEmpty(ds.DeleteCommand));
        if (showColumn)
        {
            GridViewCommandColumn c = new GridViewCommandColumn();
            grid.Columns.Add(c);
            c.Width = 50;
            c.VisibleIndex = 0;
            c.ShowEditButton = !String.IsNullOrEmpty(ds.UpdateCommand);
            //c.ShowNewButtonInHeader = !String.IsNullOrEmpty(ds.InsertCommand);
            //c.ShowDeleteButton = !String.IsNullOrEmpty(ds.DeleteCommand);
            c.ShowCancelButton = true;
            c.ShowUpdateButton = true;
            //c.ButtonRenderMode = GridCommandButtonRenderMode.Image;
        }
        //grid.SettingsCommandButton.EditButton.Image.Url = "~/Content/images/Edit.gif";
        //grid.SettingsCommandButton.UpdateButton.Image.Url = "~/Content/images/update.png";
        //grid.SettingsCommandButton.CancelButton.Image.Url = "~/Content/images/cancel.png";
    }
    private SqlDataSource GetDataSource()
    {
        object o = selectedDataSource;
        if (o == null)
            return dsulogin;
        switch (string.Format("{0}", o))
        {
            case "EditCost":
                return dsFormulaByCode;
            case "upCost":
                List<string> _list = new List<string>();
                foreach (DataRow dr in _dataTable.Rows)
                    _list.Add(dr["RequestNo"].ToString());
                var _value = String.Join("|", _list.ToArray());
                ds.SelectCommand = string.Format(@"select b.ID,MarketingNumber,b.RequestNo,isnull(Code,'') as 'Code' 
                from TransCostingHeader a left join TransFormulaHeader b on a.ID=b.RequestNo
                where a.id in (select distinct value from dbo.FNC_SPLIT('{0}','|'))", _value);//Id
                return ds;
            case "ExchangeRat":case "MasExchangeRat":
                return dsExchangeRat;
            case "Reason":
                return dsReason;
            case "Role":
                return dsApprovAssign;
            case "Policy":case "MasPricePolicy":
                return dsMasPricePolicy;
            case "Standard": case "MasPriceStd":
                //return dsMasPriceStd;
                ds.SelectCommand = string.Format("select * from MasPriceStd where Company in (select distinct value from dbo.FNC_SPLIT({0},'|'))", hBu["BU"].ToString());
                ds.InsertCommand = "Insert into MasPriceStd values (@Company,@Material,@Description,@PriceStd1,@PriceStd2,@Currency,@Unit,@From,@To,0)";
                ds.UpdateCommand = @"Update MasPriceStd set Company=@Company,Material=@Material,Description=@Description,PriceStd1=@PriceStd1,
                PriceStd2=@PriceStd2,Currency=@Currency,Unit=@Unit,[From]=@From,[To]=@To,IsActive=@IsActive where ID=@ID";
                ds.DeleteCommand = "Delete MasPriceStd Where ID=@ID";
                return ds; 
            case "MasPrice":
                return dsMasPrice;
            case "Margin":
                ds.SelectCommand = string.Format(@"select a.*,b.Name as tcode from MasMargin a left join MasPrimary b on b.LBOh=SUBSTRING(MarginCode,1,2) 
                where a.Company in (select distinct value from dbo.FNC_SPLIT({0},'|'))", hBu["BU"].ToString());
                ds.UpdateCommand = "Update MasMargin set MarginName=@MarginName,MarginRate=@MarginRate where ID=@ID";
                ds.InsertCommand = "spinsertMargin"; ds.InsertCommandType = SqlDataSourceCommandType.StoredProcedure;
                ds.DeleteCommand = "Delete MasMargin Where ID=@ID";
                return ds;
            case "Labor":
                ds.SelectCommand = string.Format(@"select a.*,b.Name as tcode from MasLaborOverhead a left join MasPrimary b on b.LBOh=SUBSTRING(LBCode,1,2) 
                where a.Company in (select distinct value from dbo.FNC_SPLIT({0},'|'))", hBu["BU"].ToString());
                ds.UpdateCommand = @"Update MasLaborOverhead set LBName=@LBName,LBRate=@LBRate,
                LBType=@LBType,Currency=@Currency,PackSize=@PackSize,Unit=@Unit,LBMin=@LBMin,LBMax=@LBMax,isactive=@isactive where ID=@ID";
                //ds.InsertCommand = "Insert into MasLabor values (@Company,@LBCode,@LBName,@LBRate@Currency,@PackSiz)";
                ds.InsertCommand = "spinsertLaborOverhead"; ds.InsertCommandType = SqlDataSourceCommandType.StoredProcedure;
                ds.DeleteCommand = "Delete MasLaborOverhead Where ID=@ID";
                return ds;
            case "Loss":
                ds.SelectCommand = "select * from MasPFLoss";
                ds.UpdateCommand = "Update MasPFLoss set PackageType=@PackageType,Loss=@Loss,SubType=@SubType where Id=@ID";
                ds.InsertCommand = "Insert into MasPFLoss values (@PackageType,@Loss,@SubType)";
                ds.DeleteCommand = "Delete MasPFLoss Where ID=@ID";
                return ds;
            case "Company":
                ds.SelectCommand = "select ID,Code,Name,fn from MasCompany";
                ds.UpdateCommand = "Update MasCompany set Code=@Code,Name=@Name,fn=@fn where ID=@ID";
                ds.DeleteCommand = "Delete MasCompany Where ID=@ID";
                ds.InsertCommand = "insert into MasCompany values(@Code,@Name,@fn)";
                return ds;
            case "Yield":
                return dsMasYield;
            case "Workflow":
                ds.SelectCommand = "select * from MasWFapp";
                ds.UpdateCommand = "Update MasWFapp set Company=@Company,Userstatus=@Userstatus,fn=@fn,StatusApp=@StatusApp where ID=@ID";
                ds.InsertCommand = "Insert into MasWFapp values(@Company,@Userstatus,@fn,@StatusApp)";
                ds.DeleteCommand = "Delete MasWFapp Where ID=@ID";
                return ds;
            default:
                return dsulogin;
        }
    }
    protected void grid_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;

        foreach (GridViewColumn column in g.Columns)
        {
            if (column is GridViewDataColumn)
            {
                ((GridViewDataColumn)column).Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            }
        }
    }

    protected void grid_CellEditorInitialize(object sender, DevExpress.Web.ASPxGridViewEditorEventArgs e)
    {
        //if (grid.IsNewRowEditing) return;
        //if (e.Column.FieldName == "firstname" || e.Column.FieldName == "lastname" || e.Column.FieldName == "empid")
        //    e.Editor.ReadOnly = true;
            var values = new[] { "Company", "Currency", "Unit", "SubType", "PackageType", "Customer", "tcode", "empid", "CurrencyTh", "ExchangeType" };
        if (grid.IsNewRowEditing)
        {
            if (e.Column.FieldName == "IsActive" || e.Column.FieldName == "IsResign")
            {
                ASPxCheckBox cb = (ASPxCheckBox)e.Editor;
                cb.Enabled = false;
            }
        }

        if (e.Column.FieldName == "BU")
        {
            ASPxDropDownEdit dropDownEdit = (ASPxDropDownEdit)e.Editor;
            dropDownEdit.ClientInstanceName = "dropDownEdit";
            string[] indexes = dropDownEdit.Text.Split(',');

            ASPxListBox listBox = (ASPxListBox)dropDownEdit.FindControl("ASPxListBox1");
            listBox.ClientInstanceName = "checkListBox";
            listBox.ClientSideEvents.SelectedIndexChanged= String.Format("function(s,e){{OnListBoxSelectionChanged(s,e,{0});}}", dropDownEdit.ClientID);
            dropDownEdit.ClientSideEvents.TextChanged = String.Format("function(s,e){{ SynchronizeListBoxValues(s,e,{0});}}", listBox.ClientID);
            dropDownEdit.ClientSideEvents.DropDown = String.Format("function(s,e){{ SynchronizeListBoxValues(s,e,{0});}}", listBox.ClientID);
            if (listBox == null) return;
            foreach (ListEditItem item in listBox.Items)
            {
                if (indexes.Contains<string>(item.Value.ToString()))
                    item.Selected = true;
            }
        }
        if (values.Any(e.Column.FieldName.Contains))
        {
            ASPxComboBox combo = (ASPxComboBox)e.Editor;
            dsCompany.SelectParameters.Clear();
            dsCompany.DataBind();
            switch (e.Column.FieldName)
            {
                case "Customer":case "ExchangeType":
                    combo.DataSource = dsCustomerPrice;
                    break;
                case "SubType":
                    combo.DataSource = dsSubType;
                    break;
                case "Currency":case "CurrencyTh":
                    combo.DataSource = dsCurrency;
                    break;
                //case "empid":
                //    combo.DataSource = dsulogin;
                //    break;

                case "empid":
                    combo.SetClientSideEventHandler("SelectedIndexChanged", "OnSelectedIndexChanged");
                    combo.DataSource = dsulogin;
                    //combo.EnableIncrementalFiltering = false;
                    combo.EnableCallbackMode = true;
                    combo.CallbackPageSize = (!IsPostBack && !Page.IsCallback) ? 5 : 100;
                    break;
                //case "SAPMaterial":
                //combo.SetClientSideEventHandler("SelectedIndexChanged", "OnChanged");
                //combo.DataSource = dsSAPMaterial;
                //break;
                case "tcode": case "PackageType":
                    combo.DataSource = dsPrimary;
                    break;
                case "Unit":
                    combo.DataSource = dsUnit;
                    break;
            }
            combo.DataBind();
        }
        if (e.Editor is ASPxTextBox || e.Editor is ASPxComboBox)
        {
            e.Editor.Width = Unit.Pixel(200);
        }
    }

    protected void grid_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
    {
        object o = selectedDataSource;
        switch (string.Format("{0}", o))
        {
            case "Company":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("fn", e.NewValues["fn"].ToString());
                break;
            case "Loss":
                //"Insert into MasPFLoss values @PackageType,@Loss,@SubType";
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("PackageType", e.NewValues["PackageType"].ToString());
                ds.SelectParameters.Add("Loss", e.NewValues["Loss"].ToString());
                ds.SelectParameters.Add("SubType", e.NewValues["SubType"].ToString());
                break;
            case "Labor":
                //"Insert into MasLabor values @Company,@LBCode,@LBName,@LBRate@Currency,@PackSiz";
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("LBType", e.NewValues["LBType"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("LBName", e.NewValues["LBName"].ToString());
                ds.SelectParameters.Add("LBRate", e.NewValues["LBRate"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("PackSize", e.NewValues["PackSize"].ToString());
                ds.SelectParameters.Add("LBMin", e.NewValues["LBMin"].ToString());
                ds.SelectParameters.Add("LBMax", e.NewValues["LBMax"].ToString());
                ds.SelectParameters.Add("isactive",e.NewValues["isactive"].ToString());
                break;
            case "Margin":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("MarginName", e.NewValues["MarginName"].ToString());
                ds.SelectParameters.Add("MarginRate", e.NewValues["MarginRate"].ToString());
                break;
            case "Workflow":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("Userstatus", e.NewValues["Userstatus"].ToString());
                ds.SelectParameters.Add("fn", e.NewValues["fn"].ToString());
                ds.SelectParameters.Add("StatusApp", e.NewValues["StatusApp"].ToString());
                break;
            case "Standard":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("Material", e.NewValues["Material"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("PriceStd1", e.NewValues["PriceStd1"].ToString());
                ds.SelectParameters.Add("PriceStd2", e.NewValues["PriceStd2"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("From", e.NewValues["From"].ToString());
                ds.SelectParameters.Add("To", e.NewValues["To"].ToString());
                ds.SelectParameters.Add("IsActive", e.NewValues["IsActive"].ToString());
                break;
        }
    }
    protected void grid_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
    {
        ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
    }
    protected void grid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
    {
        object o = selectedDataSource;
        switch (string.Format("{0}", o))
        {
            case "Company":
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("fn", e.NewValues["fn"].ToString());
                break;
            case "Loss":
                ds.SelectParameters.Add("PackageType", e.NewValues["PackageType"].ToString());
                ds.SelectParameters.Add("Loss", e.NewValues["Loss"].ToString());
                ds.SelectParameters.Add("SubType", e.NewValues["SubType"].ToString());
                break;
            case "Labor":
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("tcode", e.NewValues["tcode"].ToString());
                ds.SelectParameters.Add("LBName", e.NewValues["LBName"].ToString());
                ds.SelectParameters.Add("LBRate", e.NewValues["LBRate"].ToString());
                ds.SelectParameters.Add("LBType", e.NewValues["LBType"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("PackSize", e.NewValues["PackSize"].ToString());
                ds.SelectParameters.Add("LBMin", e.NewValues["LBMin"].ToString());
                ds.SelectParameters.Add("LBMax", e.NewValues["LBMax"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("isactive", e.NewValues["isactive"].ToString());
                break;
            case "Margin":
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("MarginName", e.NewValues["MarginName"].ToString());
                ds.SelectParameters.Add("MarginRate", e.NewValues["MarginRate"].ToString());
                ds.SelectParameters.Add("tcode", e.NewValues["tcode"].ToString());
                break;
            case "Standard":
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("Material", e.NewValues["Material"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("PriceStd1", e.NewValues["PriceStd1"].ToString());
                ds.SelectParameters.Add("PriceStd2", e.NewValues["PriceStd2"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("From", e.NewValues["From"].ToString());
                ds.SelectParameters.Add("To", e.NewValues["To"].ToString());
                break;
            //case "Company":
            //    ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
            //    ds.SelectParameters.Add("Userstatus", e.NewValues["Userstatus"].ToString());
            //    ds.SelectParameters.Add("fn", e.NewValues["fn"].ToString());
            //    ds.SelectParameters.Add("StatusApp", e.NewValues["StatusApp"].ToString());
            //    break;
            default:
                break;
        }
    }
    protected void grid_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            var item = e.CreateItem("Refresh", "Refresh");
            item.Image.Url = @"~/Content/Images/Refresh.png";
            e.Items.Add(item);

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

    protected void grid_ContextMenuItemClick(object sender, ASPxGridViewContextMenuItemClickEventArgs e)
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

    protected void testGrid_DataBound(object sender, EventArgs e)
    {
        //ASPxGridView g = sender as ASPxGridView;
        //object o = selectedDataSource;
        ASPxGridView g = sender as ASPxGridView;
        if (g.VisibleRowCount > 5)
            g.Settings.VerticalScrollableHeight = 320;
        else
            g.Settings.VerticalScrollableHeight = g.VisibleRowCount * 23;
    }
    public class MyDropDownWindow : ITemplate
    {
        void ITemplate.InstantiateIn(Control container)
        {
            ASPxListBox lb = new ASPxListBox();
            lb.ID = "ASPxListBox1";
            container.Controls.Add(lb);

            lb.Width = Unit.Percentage(50);
            lb.SelectionMode = ListEditSelectionMode.CheckColumn;
            lb.ClientSideEvents.SelectedIndexChanged = "function(s,e) { OnSelectedIndexChanged(s,e); }";
            lb.DataBinding += lb_DataBinding;
            //for (int i = 0; i < 10; i++)
            //    lb.Items.Add(i.ToString(), i);
        }
        // MyColumnCodeBehind ComboBox binding.
        void lb_DataBinding(object sender, EventArgs e)
        {
            ASPxListBox lb = (ASPxListBox)sender;
            lb.DataSource = GetDataSource();
        }
        private List<string> GetDataSource()
        {
            return new List<string>(new string[] { "101", "102", "103", "901" });
        }
    }

    protected void grid_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
    {
        //if (e.ButtonType == ColumnCommandButtonType.Update || e.ButtonType == ColumnCommandButtonType.Cancel)
        //{
        //    e.Visible = false;
        //}
    }
}