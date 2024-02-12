using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using DevExpress.Web;
using DevExpress.Web.ASPxSpreadsheet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class popupControls_product_list : System.Web.UI.Page
{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    private DataTable Tablelist
    {
        get { return Page.Session["sessionlist"] == null ? null : (DataTable)Page.Session["sessionlist"]; }
        set { Page.Session["sessionlist"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session.Clear();
            var Id = Request.QueryString["Id"].ToString();
            var table = Request.QueryString["table"].ToString();
            string strSQL = "select StatusApp,Id,usertype,";
            strSQL += table == "TransTechnical" ? "Assignee," : "'' as Assignee,";
            strSQL += string.Format(@"company,CONVERT(nvarchar(max),UniqueColumn)'NewID' from " + table + " where Id={0}", Id);
            var result = cs.builditems(strSQL);
            foreach (DataRow r in result.Rows)
            {
                Page.Session["StatusApp"]= string.Format("{0}", r["StatusApp"]);
                Page.Session["usertype"] = string.Format("{0}", r["usertype"]);
                Page.Session["BU"] = string.Format("{0}", r["company"]);
                Page.Session["NewID"] = r["Id"];
                Page.Session["username"] = user_name;
                Page.Session["tablename"] = table;
            }
        namelist(Id);
        gv.DataBind();
        }
    }
    string FilePath
    {
        get { return Page.Session["sessionFile"] == null ? String.Empty : Page.Session["sessionFile"].ToString(); }
        set { Page.Session["sessionFile"] = value; }
    }
    void namelist(string Folio)
    {
        dsnamelist.SelectParameters.Clear();
        dsnamelist.SelectParameters.Add("Id", string.Format("{0}", Folio.ToString()));
        dsnamelist.DataBind();
        Tablelist = ((DataView)dsnamelist.Select(DataSourceSelectArguments.Empty)).Table;
        Tablelist.PrimaryKey = new DataColumn[] { Tablelist.Columns["Id"] };
    }
    void insertname()
    {
        string keys = Request.QueryString["Id"].ToString();
        if (Tablelist == null) return;
        if (Tablelist.Rows.Count == 0) return;
        foreach (DataRow c in Tablelist.Rows)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spinsertProductLst";
                cmd.Parameters.AddWithValue("@name", c["Name"].ToString());
                cmd.Parameters.AddWithValue("@NetWeight", string.Format("{0}", c["NetWeight"]));
                cmd.Parameters.AddWithValue("@SaltContent", string.Format("{0}", c["SaltContent"]));
                cmd.Parameters.AddWithValue("@DWType", c["DWType"].ToString());
                cmd.Parameters.AddWithValue("@Mark", c["Mark"].ToString());
                cmd.Parameters.AddWithValue("@FixedFillWeight", string.Format("{0}", c["FixedFillWeight"]));
                cmd.Parameters.AddWithValue("@PW", c["PW"].ToString());
                cmd.Parameters.AddWithValue("@TargetPrice", c["TargetPrice"].ToString());
                cmd.Parameters.AddWithValue("@DW", c["DW"].ToString());
                cmd.Parameters.AddWithValue("@Efficiency", c["Efficiency"].ToString());
                cmd.Parameters.AddWithValue("@Yield", c["Yield"].ToString());
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", c["ID"]));
                cmd.Parameters.AddWithValue("@RequestNo", keys.ToString());
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
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
        string dirVirtualPath = @"\\192.168.1.193\XlsTables";
        //string dirVirtualPath = @"C:\\temp";
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
            int NextRowID = 0; int i = 0;
            NextRowID = Tablelist.Rows.Count;
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
                    double num;
                    string value = dr["Column2"].ToString().Trim();
                    if (double.TryParse(dr["Column1"].ToString(), out num))
                        if (!string.IsNullOrEmpty(value))
                        {
                            NextRowID++;
                            object[] array = new object[11];
                            array[0] = num;
                            array[1] = NextRowID;
                            array[2] = dr["Column2"];
                            array[3] = dr["Column3"];
                            array[4] = string.Format("{0}", dr["Column4"]);
                            array[5] = string.Format("{0}", dr["Column5"]);
                            array[6] = string.Format("{0}", dr["Column6"]);
                            array[7] = 0;
                            array[8] = 0;
                            array[9] = 0;
                            array[10] = 0;
                            if (!string.IsNullOrEmpty(string.Format("{0}", array[1])))
                                Tablelist.Rows.Add(array);
                        }
                }
            }
        }
    }

    protected void gv_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        string[] param = e.Parameters.Split('|'); //bool selected = true;
        switch (param[0])
        {
            case "reload":
                this.Session.Remove("sessionlist");
                Tablelist = new DataTable();
                namelist(param[1]);
                break;
            case "SaveMail":
                insertname();
                break;
        }
        g.DataBind();
    }

    protected void gv_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
    {
        var values = new[] { "Id", "RowID", "RequestNo" };
        foreach (var args in e.InsertValues)
        {
            DataRow row = Tablelist.NewRow();
            int NextRowID = Tablelist.Rows.Count;
            NextRowID++;
            row["Id"] = NextRowID;
            row["RowID"] = row["Id"];
            foreach (DataColumn column in Tablelist.Columns)
            {
                if (!values.Any(column.ColumnName.Contains))
                {
                    row[column.ColumnName] = args.NewValues[column.ColumnName];
                }
            }
            Tablelist.Rows.Add(row);
        }
        foreach (var args in e.UpdateValues)
        {
            DataRow dr = Tablelist.Rows.Find(args.Keys["Id"]);
            foreach (DataColumn column in Tablelist.Columns)
            {
                if (!values.Any(column.ColumnName.Contains))
                {
                    dr[column.ColumnName] = args.NewValues[column.ColumnName];
                }
            }
        }
        e.Handled = true;
    }

    protected void gv_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.KeyFieldName = "Id";
        g.DataSource = Tablelist;
        g.ForceDataRowType(typeof(DataRow));
    }

    protected void gv_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        if (e.ButtonID != "Edit") return;
        object keyValue = g.GetRowValues(e.VisibleIndex, g.KeyFieldName);
        DataRow found = Tablelist.Rows.Find(keyValue);
        //t.Rows.Remove(found);
        found["Mark"] = found["Mark"].ToString() == "D" ? "" : "D";
        Tablelist.AcceptChanges();
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

    protected void gv_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (g.VisibleRowCount > 5)
            g.Settings.VerticalScrollableHeight = 320;
        else
            g.Settings.VerticalScrollableHeight = g.VisibleRowCount * 23;
        if (!Page.Session["username"].ToString().Contains("0"))
        {
            string str = Page.Session["username"].ToString();
            if (str.Contains('1'))
            {
                g.Columns[1].Width = Unit.Pixel(0);
            }
            else
            {
                g.Columns[1].Width = Unit.Pixel(0);
                g.Columns[5].Caption = "Unit";
                g.Columns[5].VisibleIndex = 3;
                g.Columns[6].Caption = "F/W (g)";
                g.Columns[7].Width = Unit.Pixel(0);
                g.Columns[9].Caption = "Volume";
                if (Page.Session["StatusApp"].ToString() == "6" || Page.Session["StatusApp"].ToString() == "2")
                {
                    g.Columns[10].Width = Unit.Percentage(10);
                    g.Columns[11].Width = Unit.Percentage(10);
                }
            }
        }
    }
}