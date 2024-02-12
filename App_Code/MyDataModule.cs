using System;
using System.Web;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Net.Mail;
using System.IO;
using System.Collections;
using System.ComponentModel;
using ClosedXML.Excel;
using System.Net;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Reflection;
//using System.Security.Claims;
//using System.Security.Principal;
using System.DirectoryServices;

//using Twilio;
//using Twilio.Rest.Api.V2010.Account;
//using Twilio.Types;
/// <summary>
/// Summary description for MyDataModule
/// </summary>
public class MyDataModule
{
    HttpContext Context = HttpContext.Current;
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    public string CurUserName = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    //public string EMPID;
    //public string EMPName;
    //public string EMPSName;
    //public string EMPGender;
    //public string EMPLevel;
    //public string EMPCompany;
    public MyDataModule()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public DataTable GetCusFormula(string id)
    {
        SqlParameter[] param = {new SqlParameter("@Id",id.ToString()),
                        new SqlParameter("@formula","1"),
                        new SqlParameter("@type","")};
        return GetRelatedResources("spGetCusFormula", param);

    }
    public static string GetUserEmail(string UserId)
    {

        var searcher = new DirectorySearcher("LDAP://" + UserId.Split('\\').First().ToLower())
        {
            Filter = "(&(ObjectClass=person)(sAMAccountName=" + UserId.Split('\\').Last().ToLower() + "))"
        };

        var result = searcher.FindOne();
        if (result == null)
            return string.Empty;

        return result.Properties["mail"][0].ToString();

    }
    public static string uEmail(string uid)
    {
        DirectorySearcher dirSearcher = new DirectorySearcher();
        DirectoryEntry entry = new DirectoryEntry(dirSearcher.SearchRoot.Path);
        dirSearcher.Filter = "(&(objectClass=user)(objectcategory=person)(mail=" + uid + "*))";

        SearchResult srEmail = dirSearcher.FindOne();

        string propName = "mail";
        ResultPropertyValueCollection valColl = srEmail.Properties[propName];
        try
        {
            return valColl[0].ToString();
        }
        catch
        {
            return "";
        }
    }

    //public static IIdentity GetIdentity()
    //{
    //    if (HttpContext.Current != null && HttpContext.Current.User != null)
    //    {
    //        return HttpContext.Current.User.Identity;
    //    }
    //    return ClaimsPrincipal.Current != null ? ClaimsPrincipal.Current.Identity : null;
    //}

    public DataTable GettableLoss(string Key, DateTime ValidTo, string utype)
    {
        SqlParameter[] p = { new SqlParameter("@PackageType", Key.ToString()),
        new SqlParameter("@To", ValidTo),
        new SqlParameter("@u",string.Format("{0}", utype))};
        return GetRelatedResources("spGetLoss", p);
    }
    public string GetExcelColumnName(int columnNumber)
    {
        string columnName = "";

        while (columnNumber > 0)
        {
            int modulo = (columnNumber - 1) % 26;
            columnName = Convert.ToChar('A' + modulo) + columnName;
            columnNumber = (columnNumber - modulo) / 26;
        }

        return columnName;
    }
    public List<T> ConvertDataTable<T>(DataTable dt)
    {
        List<T> data = new List<T>();
        foreach (DataRow row in dt.Rows)
        {
            T item = GetItem<T>(row);
            data.Add(item);
        }
        return data;
    }
    public T GetItem<T>(DataRow dr)
    {
        Type temp = typeof(T);
        T obj = Activator.CreateInstance<T>();

        foreach (DataColumn column in dr.Table.Columns)
        {
            foreach (PropertyInfo pro in temp.GetProperties())
            {
                //if (pro.PropertyType.Name == "String")
                //{ }
                if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                
            }
        }
        return obj;
    }
    public byte[] ReadToEnd(Stream stream)
    {
        long originalPosition = 0;

        if (stream.CanSeek)
        {
            originalPosition = stream.Position;
            stream.Position = 0;
        }

        try
        {
            byte[] readBuffer = new byte[4096];

            int totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
            {
                totalBytesRead += bytesRead;

                if (totalBytesRead == readBuffer.Length)
                {
                    int nextByte = stream.ReadByte();
                    if (nextByte != -1)
                    {
                        byte[] temp = new byte[readBuffer.Length * 2];
                        Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                        Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                        readBuffer = temp;
                        totalBytesRead++;
                    }
                }
            }

            byte[] buffer = readBuffer;
            if (readBuffer.Length != totalBytesRead)
            {
                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
            }
            return buffer;
        }
        finally
        {
            if (stream.CanSeek)
            {
                stream.Position = originalPosition;
            }
        }
    }
    public DataTable _createData()
    {
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[] { new DataColumn("RequestNo"),
            new DataColumn("Formula",typeof(string)),
            new DataColumn("RM",typeof(float)),
            new DataColumn("Ing"),
            new DataColumn("Margin"),
            new DataColumn("PrimaryPkg"),
            new DataColumn("SecondaryPkg"),
            new DataColumn("UpCharge"),
            new DataColumn("RmIng"),
            new DataColumn("Loss"),
            new DataColumn("FOB"),
            new DataColumn("PerMargin"),
            new DataColumn("Rate"),
            new DataColumn("LOH",typeof(float)),
            new DataColumn("PackSize",typeof(float)),
            new DataColumn("PackSizeh",typeof(float))});
        return dt;
    }

    public void _alertmail(DataRow dr, string filename, string t, string usertype)
    {
        //DataTable dt = new DataTable();
        //SqlParameter[] param = {new SqlParameter("@Id",dr["ID"].ToString()),
        //                new SqlParameter("@User",CurUserName),
        //                new SqlParameter("@StatusApp","2"),
        //                new SqlParameter("@table",string.Format("{0}", "TransTechnical")),
        //                new SqlParameter("@AppStatus","2")};
        //dt = GetRelatedResources("spsender", param);
        //foreach (DataRow row in dt.Rows)
        //{
        string strArrStr = string.Concat(dr["Requester"], ",", dr["Assignee"]);
        List<string> list = new List<string>();
        List<string> listsender = new List<string>();
        string[] split = strArrStr.ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in split)
        {
            if (s.Trim() != "")
            {
                list.Add(GetData(s, "email"));
                listsender.Add(GetData(s, "fn"));
            }
        }
        string sender = String.Join(",", list.ToArray());
        //list = new List<string>();
        //split = row["MailCc"].ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //foreach (string s in split)
        //{
        //    if (s.Trim() != "")
        //        list.Add(GetData(s, "email"));
        //}
        string form = (dr["RequestNo"].ToString().Substring(3, 1) == "1") ? "Edit" : "EditDraft";
        //string MailCc = String.Join(",", list.ToArray());
        string _text = string.Format("Dear {0} ", String.Join(",", listsender.ToArray()));
        //string name = string.Format(_text, cs.GetSubject(Page.Session["usertype"].ToString()), dr["RequestNo"].ToString(), dr["Customer"]);
        string subject = string.Format(@"{0}:{1}:Request No.: " + dr["RequestNo"].ToString() + " V.{2} ,"
        + dr["Customer"] + "/", GetSubject(usertype.ToString()), t, string.Format("{0:00}", dr["Revised"]));
        string _link = _text;
        _link += "<br/> Request No.: " + dr["RequestNo"].ToString();
        _link += "<br/> Customer Name : " + dr["Customer"];
        _link += "<br/> Request By : " + GetData(dr["Requester"].ToString(), "fn");
        _link += "<br/> Sender : " + GetData(CurUserName.ToString(), "fn");
        _link += "<br/> File Name : " + filename;
        _link += "<br/> The document link --------><a href=" + GetSettingValue();
        _link += @"/Default.aspx?viewMode=CustomerEditForm&ID=" + dr["ID"].ToString() + "&form=" + form + "&UserType=" + usertype;
        _link += " style='color: rgb(0,255,0)'><font color='#0000FF'>click</font></a>";
        //sendemail(sender, GetData(CurUserName, "email"), _link, subject);
        //}
    }
    public DataTable GetMedia(string Code, string group,
     DateTime dtFrom, DateTime dtTo)
    {
        DataTable dt = new DataTable();
        if (dtFrom == null || dtTo == null) return dt;
        SqlParameter[] param = {
            new SqlParameter("@group", group.ToString()),
            new SqlParameter("@from", dtFrom),
            new SqlParameter("@to", dtTo),
            new SqlParameter("@Code", Code.ToString())};
        dt = GetRelatedResources("spTunaStdMedia2", param);
        return (DataTable)dt;
    }
    public string GetAvgFishPrice(string _separa,
     string MonthName, object FishGroup, object FishCert, object SHD,
     DateTime dtFrom, DateTime dtTo, string SubID)
    {
        if (string.IsNullOrEmpty(_separa)) return "";
        decimal result = 0;
        //if (!string.IsNullOrEmpty(SubID))
        //{
        //    string Id = String.Format("{0}", hGeID["GeID"]);
        //    _dtcomponent = cs.builditems(@"select * from TransStdComponent WHERE component='RM' and RequestNo = "+ 
        //        Id +" and SubID = "+ SubID );
        //    foreach (DataRow _r in _dtcomponent.Rows)
        //    {
        //        string[] array = { _r["Result"].ToString(), _r["Name"].ToString(), "USD", "KG" };
        //        return string.Join("|", array).ToString();
        //    }
        //}
        if (dtFrom == null || dtTo == null) return string.Format("{0}", 0);
        SqlParameter[] param = { new SqlParameter("@from", dtFrom),
            new SqlParameter("@to", dtTo),
            new SqlParameter("@cols", MonthName),
            new SqlParameter("@FishGroup",string.Format("{0}",FishGroup)),
            new SqlParameter("@FishCert",string.Format("{0}",FishCert)),
            new SqlParameter("@SHD",string.Format("{0}",SHD)),
            new SqlParameter("@Code",string.Format("{0}",_separa))};
        var dt = GetRelatedResources("spTunaStdFishPrice", param);
        if (dt.Rows.Count > 0)
        {
            result = Convert.ToDecimal(dt.Compute("Max(Result)", "").ToString());
            if (dt.Rows[0]["Unit"].ToString() == "KG")
                result = result * 1000;
        }
        if (dt.Rows.Count == 0) return "";
        string[] arr = { result.ToString(), dt.Rows[0]["Name"].ToString(), dt.Rows[0]["Currency"].ToString(),
         dt.Rows[0]["Unit"].ToString()};
        return string.Join("|", arr).ToString();
    }
    public DataTable GetMergeDataSource(int formula, string ty, string Id)
    {
        DataTable dt = new DataTable();// string Folio = "";
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spcalcuformula";
            cmd.Parameters.AddWithValue("@Id", Id.ToString());
            cmd.Parameters.AddWithValue("@formula", formula.ToString());
            cmd.Parameters.AddWithValue("@User", CurUserName.ToString());
            cmd.Parameters.AddWithValue("@type", ty.ToString());
            cmd.Connection = con;
            con.Open();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
        }
        return dt;
        //    string query = @"SELECT * FROM Employees";
        //    DataTable employees = new DataTable();
        //    using (var conn = new SqlConnection(strConn))
        //    using (var dataAdapter = new SqlDataAdapter(query, conn))
        //    {
        //        conn.Open();
        //        dataAdapter.Fill(employees);
        //        conn.Close();
        //    }
        //    return employees;

    }
    public void _del(string Id)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spDeleteFileSystem";
            cmd.Parameters.AddWithValue("@ID", Id.ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    public DataTable testgrid_reload(string Key)
    {
        DataTable _dataTable = new DataTable();
        if (Key == "0")
            _dataTable = new DataTable();
        SqlParameter[] p = { new SqlParameter("@param", Key.ToString()) };
        _dataTable = GetRelatedResources("spGetCostingItem", p);
        _dataTable.PrimaryKey = new DataColumn[] { _dataTable.Columns["ID"] };
        return _dataTable;
    }
    public DataTable ConvertToDatatable(string data)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spGetFormula";
            cmd.Parameters.AddWithValue("@data", data);
            cmd.Parameters.AddWithValue("@user", CurUserName.ToString());
            cmd.Connection = con;
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            oAdapter.Fill(dt);
            con.Close();
            return dt;
        }
    }
    public string GetMinPrice(string _f, string keys)
    {
        //var comparer = new CustomComparer();
        //DataTable dtUnion = tcustomer.AsEnumerable()
        //      .Union(_dataTable.AsEnumerable(), comparer).CopyToDataTable<DataRow>();
        double r = 0;
        DataTable _dt = builditems(@"select * from TransCostingHeader where ID=" + keys);
        foreach (DataRow rw in _dt.Rows) {
            var MyTable = new DataTable();
            double[] doarray = new double[7];
            double PerMargin = 0;
            DataTable tcustomer = ConvertToDatatable(keys);
            DataRow[] selected = tcustomer.Select("Formula = '" + _f + "'");
            var values = new[] { "ingredient", "solid portion", "solution" };
            foreach (DataRow row in selected)
            {
                string n = row["Component"].ToString().ToLower();
                if (n.ToString() == "raw material") {
                    doarray[0] += Convert.ToDouble(row["PriceOfCarton"]);
                    if (row["LBRate"].ToString() != "")
                    {
                        string sumLB = tcustomer.Compute("sum(GUnit)", "LBRate<>'' and Formula ='" + _f.ToString() + "'").ToString();
                        doarray[6] += (Convert.ToDouble(row["LBRate"]) * Convert.ToDouble(row["GUnit"])) / Convert.ToDouble(sumLB);
                    }
                } else //if (n.ToString() == "" || values.Any(n.Contains)) 
                {
                    double ingredient = Convert.ToDouble(row["PriceOfCarton"]);
                    doarray[1] += Convert.ToDouble(row["PriceOfCarton"]);
                    //doarray[2] += Convert.ToDouble(row["PriceOfCartonusd"]);

                }
            }
            DataTable _dataTable = testgrid_reload(keys);
            DataRow[] _sel = _dataTable.Select("Formula = '" + _f + "'");
            foreach (DataRow row in _sel)
            {
                switch (row["SubType"].ToString().ToLower())
                {
                    case "primary packaging":
                        doarray[3] += Convert.ToDouble(row["Amount"]);
                        break;
                    case "secondary packaging":
                        doarray[4] += Convert.ToDouble(row["Amount"]);
                        break;
                    case "upcharge":
                        doarray[5] += Convert.ToDouble(row["Amount"].ToString() == "" ? "0" : row["Amount"]);
                        break;
                    case "margin":
                        PerMargin = Convert.ToDouble(row["Per"]);
                        break;
                }
            }
            string[] SubType = Regex.Split("Raw Material & Ingredient;Primary Packaging;Secondary Packaging", ";");
            double[] LossArr = new double[3];
            double _total = 0;
            for (int i = 0; i <= SubType.Length - 1; i++)
            {
                double _loss = (Convert.ToDouble(GetLoss(SubType[i], rw["Packaging"].ToString(), Convert.ToDateTime(rw["To"]))) / 100);
                switch (SubType[i])
                {
                    case "Raw Material & Ingredient":
                        _total = doarray[0] + doarray[1];
                        LossArr[0] = _total * _loss;
                        break;
                    case "Primary Packaging":
                        LossArr[1] = doarray[3] * _loss;
                        break;
                    case "Secondary Packaging":
                        LossArr[2] = doarray[4] * _loss;
                        break;
                }

            }

            double totalCount = 0;
            double sum = LossArr.Sum();
            totalCount = doarray.Sum() + sum;
            //object sumObject = string.Format(@"{0}", rw["PerMargin"].ToString() == "" ? "0" : rw["PerMargin"].ToString());

            double _MinPrice = (totalCount / ((100 - PerMargin) / 100));
            double _Margin = _MinPrice * (PerMargin / 100);
            List<double> list1 = new List<double>() {
            sum,
            doarray.Sum(),
            _Margin};
            r = list1.Sum();
            if (r > 0)
                if (rw["ExchangeRate"].ToString() != "") {
                    //        //if (CmbCurrency.Value.ToString() != "USD")
                    //        //    rw["Rate"] = Convert.ToDecimal(seExchangeRate.Value);
                    //        //else
                    r = r / Convert.ToDouble(rw["ExchangeRate"]);
                }
        }
        return r.ToString("F",
                  CultureInfo.InvariantCulture);
    }
    public DataTable GetFillWeight(string _separa)
    {
        DataTable dt = new DataTable();
        if (!string.IsNullOrEmpty(_separa))
        {
            SqlParameter[] param = { new SqlParameter("@Code", string.Format("{0}", _separa)) };
            dt = GetRelatedResources("spGetFillWeight2", param);
        }
        return dt;
    }
    public DateTime GetValidTo(string Id)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select [To] from TransCostingHeader where Id=@id";
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.Connection = con;
            con.Open();
            var getValue = cmd.ExecuteScalar();
            con.Close();
            return (DateTime)getValue;
        }
    }
    public string GetLoss(string PackType, string _Packaging, DateTime Validto)
    {
        string str = "";
        str = (string)ReadItems(@"select Loss from Maspfloss where SubType='" +
            PackType + "' and ('" + Validto + "' between Validfrom and Validto ) and PackageType in ('" + _Packaging.ToString() + "')");
        return str == "" ? "0" : str;
    }
    public int GetMonthsBetween(DateTime from, DateTime to)
    {
        //if (from > to) return GetMonthsBetween(to, from);

        //var monthDiff = Math.Abs((to.Year * 12 + (to.Month - 1)) - (from.Year * 12 + (from.Month - 1)));

        //if (from.AddMonths(monthDiff) > to || to.Day < from.Day)
        //{
        //    return monthDiff - 1;
        //}
        //else
        //{
        //    return monthDiff;
        //}
        var fromdate = from.Date.ToString("yyyyMMdd");
        var toate = to.Date.ToString("yyyyMMdd");
        string strSQL = string.Format(@"SELECT DATEDIFF(month, '{0}', '{1}') AS DateDiff", fromdate, toate);
        string monthDiff = ReadItems(strSQL);
        return Convert.ToInt32(monthDiff);
    }
    string test()
    {
        return "X";
    }
    public IEnumerable<int> Integers()
    {
        string x = test();
        Context.Response.Write(x);
        yield return 1;
        yield return 2;
        yield return 4;
        yield return 8;
        yield return 16;
        yield return 16777216;
    }

    public string ValueToPass
    {
        get
        {
            if (Context.Items["ValueToPass"] == null)
                Context.Items["ValueToPass"] = string.Empty;
            return (string)Context.Items["ValueToPass"];
        }
        set
        {
            Context.Items["ValueToPass"] = value;
        }
    }
    //public DataTable _dt {
    //    get { return Context.Session["setableKey"] == null ? null : (DataTable)Context.Session["setableKey"]; }
    //    set { Context.Session["setableKey"] = value; }
    //}
    //public DataTable _dataTable {
    //    get { return Context.Session["sessionKey"] == null ? null : (DataTable)Context.Session["sessionKey"]; }
    //    set { Context.Session["sessionKey"] = value;
    //    }
    //}
    //public DataTable tcustomer {
    //    get { return Context.Session["seGetMyData"] == null ? null : (DataTable)Context.Session["seGetMyData"];
    //    }
    //    set { Context.Session["seGetMyData"] = value; }
    //}
    public bool IsNumeric(string s)
    {
        foreach (char c in s)
        {
            if (!char.IsDigit(c) && c != '.')
            {
                return false;
            }
        }

        return true;
    }
    public string user()
    {
        string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
        string strSQL = @"select count(userlevel) as 'userlevel' from ulogin where [user_name]='" +
            user_name + "' and userlevel in (0,1,2)";
        return string.Format("{0}", this.ReadItems(strSQL));
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
    public void smpcgmail(string MailTo, string MailSubject)
    {
        try
        {
            //string AppLocation = "";  
            //AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);  
            //AppLocation = AppLocation.Replace("file:\\", "");  
            //string file = AppLocation + "\\ExcelFiles\\DataFile.xlsx";  
            string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/DataFile.xlsx");
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.office365.com");
            mail.From = new MailAddress("Costing.WebBase@thaiunion.com");
            mail.To.Add(MailTo); // Sending MailTo  
            List<string> li = new List<string>();
            li.Add("voravut.somboornpong@thaiunion.com");
            //li.Add("saihacksoft@gmail.com");  
            //li.Add("saihacksoft@gmail.com");  
            //li.Add("saihacksoft@gmail.com");  
            //li.Add("saihacksoft@gmail.com");  
            mail.CC.Add(string.Join<string>(",", li)); // Sending CC  
            mail.Bcc.Add(string.Join<string>(",", li)); // Sending Bcc   
            mail.Subject = MailSubject; // Mail Subject  
            mail.Body = "Sales Report *This is an automatically generated email, please do not reply*";
            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment(file); //Attaching File to Mail  
            mail.Attachments.Add(attachment);
            SmtpServer.Port = 587; //PORT  
            SmtpServer.EnableSsl = true;
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new NetworkCredential("Costing.WebBase@thaiunion.com", "AAaa123*");
            SmtpServer.Send(mail);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public void GetDataReport()
    {
        DataSet ds = null;
        string strConnString = ConfigurationManager.ConnectionStrings["ncpDbConnectionString"].ConnectionString;
        using (SqlConnection con = new SqlConnection(strConnString))
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
    protected void RaiseException(object sender, EventArgs e)
    {
        try
        {
            int i = int.Parse("Mudassar");
        }
        catch (Exception ex)
        {
            this.LogError(ex);
        }
    }

    private void LogError(Exception ex)
    {
        string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
        message += Environment.NewLine;
        message += "-----------------------------------------------------------";
        message += Environment.NewLine;
        message += string.Format("Message: {0}", ex.Message);
        message += Environment.NewLine;
        message += string.Format("StackTrace: {0}", ex.StackTrace);
        message += Environment.NewLine;
        message += string.Format("Source: {0}", ex.Source);
        message += Environment.NewLine;
        message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
        message += Environment.NewLine;
        message += "-----------------------------------------------------------";
        message += Environment.NewLine;
        string path = HttpContext.Current.Server.MapPath("~/ErrorLog/ErrorLog.txt");
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(message);
            writer.Close();
        }
    }
    public string GetUserAD()
    {
        return CurUserName;
    }
    public string GetSettingValue()
    {
        return String.Format(ConfigurationManager.AppSettings["SiteUrl"]);
    }
    public string GetNewID()
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
            con.Dispose();
            return (string)getValue;
        }
    }
    public int FindMaxValue<T>(List<T> list, Converter<T, int> projection)
    {
        if (list == null) return 0;
        if (list.Count == 0)
        {
            return list.Count;
        }
        int maxValue = int.MinValue;
        foreach (T item in list)
        {
            int value = projection(item);
            if (value > maxValue)
            {
                maxValue = value;
            }
        }
        return maxValue;
    }
    public DataTable GetRelatedResources(string StoredProcedure, object[] Parameters)
    {
        var Results = new DataTable();
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
        return Results;
    }
    public void GetExecuteNonQuery(string StoredProcedure, object[] Parameters)
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
    public void DelExecuteNonQuery(string StoredProcedure, object[] Parameters)
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
                    cmd.CommandType = CommandType.Text;
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
    public string Authorize(string RoleList, string CurUserName)
    {
        string MemberResult = "No";
        if (RoleList.IndexOf(CurUserName.ToUpper()) > 0)
        {
            MemberResult = "Yes";
            goto ex;
        }
        string[] ArrRoles = RoleList.Split(',');
        foreach (string tmpRole in ArrRoles)
        {
            if (tmpRole.Contains(CurUserName))
            //if (Roles.IsUserInRole(CurUserName, tmpRole))
            {
                MemberResult = "Yes";
                goto ex;
            }

        }

    ex:
        return MemberResult;
    }
    public string IsMemberOfRole(string role)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Validate_User";
            cmd.Parameters.AddWithValue("@UserId", CurUserName);
            cmd.Parameters.AddWithValue("@role", role);
            cmd.Connection = con;
            con.Open();
            var getValue = cmd.ExecuteScalar();
            con.Close();
            return (string)getValue;
        }
    }
    public string Role()
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select convert(nvarchar(max),idx)idx from dbo.FindULevel(@UserId)";
            cmd.Parameters.AddWithValue("@UserId", CurUserName);
            cmd.Connection = con;
            con.Open();
            var getValue = cmd.ExecuteScalar();
            con.Close();
            return (string)getValue;
        }
    }
    public string CheckRole()
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spGetrole";
            cmd.Parameters.AddWithValue("@user", CurUserName);
            cmd.Connection = con;
            con.Open();
            var getValue = cmd.ExecuteScalar();
            con.Close();
            return (string)getValue;
        }
    }
    //public DataTable GetData(SqlCommand cmd)
    //{
    //    DataTable dt = new DataTable();
    //    String strConnString = System.Configuration.ConfigurationManager.
    //         ConnectionStrings["ncpDbConnectionString"].ConnectionString;
    //    SqlConnection con = new SqlConnection(strConnString);
    //    SqlDataAdapter sda = new SqlDataAdapter();
    //    cmd.CommandType = CommandType.Text;
    //    cmd.Connection = con;
    //    try
    //    {
    //        con.Open();
    //        sda.SelectCommand = cmd;
    //        sda.Fill(dt);
    //        return dt;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally
    //    {
    //        con.Close();
    //        sda.Dispose();
    //        con.Dispose();
    //    }
    //}

    public void emlSend(string MailTo, string MailCc, string Body, string Subject, string lstValue)
    {
        insertsendmail(MailTo, MailCc, Body, string.Format("{0}_Att", Subject));
        byte[] bytes;
        string fileName;
        MailTo = "voravut.somboornpong@thaiunion.com"; MailCc = "";
        MailMessage email = new MailMessage();
        SmtpClient SmtpServer = new SmtpClient("attgmail");

        email.From = new MailAddress("Costing.WebBase@thaiunion.com");
        if (string.IsNullOrEmpty(MailTo)) return;
        string[] words = MailTo.Split(',');
        foreach (string word in words)
        {
            if (!string.IsNullOrEmpty(word))
                email.To.Add(new MailAddress(word));
        }
        string[] c = MailCc.Split(',');
        foreach (string s in c)
            if (!string.IsNullOrEmpty(s))
                email.CC.Add(new MailAddress(s));
        email.Subject = string.Format("{0}", Subject);
        email.Body = Body;
        //Use value in Select statement
        DataTable dt = builditems(@"SELECT Name, Data from FileSystem where GCRecord = '" + lstValue + "' and IsFolder=0");
        foreach (DataRow dr in dt.Rows)
        {
            bytes = (byte[])dr["Data"];
            fileName = dr["Name"].ToString();
            //This works if files are stored in folder instead of the below code
            //Attachment resume = new Attachment(Server.MapPath(VirtualPathUtility.ToAbsolute("~/images/cv/" + fileName)));

            //Attach Data as Email Attachment
            MemoryStream pdf = new MemoryStream(bytes);
            Attachment data = new Attachment(pdf, fileName);
            email.Attachments.Add(data);
        }
        //send the message
        email.IsBodyHtml = true;
        SmtpClient smtp = new SmtpClient();
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new System.Net.NetworkCredential("Costing.WebBase@thaiunion.com", "AAaa123*");
        //client.Credentials = new System.Net.NetworkCredential("wshuttleadm@thaiunion.com", "AAaa123*");
        smtp.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
        smtp.Host = "smtp.office365.com";
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.EnableSsl = true;
        smtp.Send(email);
    }
    public void sendemail(string MailTo, string MailCc, string _Body, string _Subject)
    {
        insertsendmail(MailTo, MailCc, _Body, _Subject);
        //MailMessage msg = new MailMessage();
        //SmtpClient smtp = new SmtpClient();
        //if (string.IsNullOrEmpty(MailTo)) return;
        //string[] words = MailTo.Split(',');
        //foreach (string word in words)
        //{
        //    if (!string.IsNullOrEmpty(word))
        //        msg.To.Add(new MailAddress(word));
        //}
        //List<string> myList = new List<string>();
        //string[] c = MailCc.Split(',');
        //foreach (string s in c)
        //    if (!string.IsNullOrEmpty(s))
        //    {
        //        msg.CC.Add(new MailAddress(s));
        //        myList.Add(s);
        //    }
        ////if(CurUserName!= "CP4790028")
        ////string MyCurMail = GetData(CurUserName, "email");
        ////if (!myList.Contains(MyCurMail))
        ////{
        ////    msg.CC.Add(new MailAddress(MyCurMail));
        ////}
        //msg.From = new MailAddress("Costing.WebBase@thaiunion.com");
        //msg.Subject = string.Format("{0}", _Subject);
        //msg.Body = _Body;

        //msg.IsBodyHtml = true;
        //msg.Priority = MailPriority.High;
        //smtp.Host = "192.168.1.39";
        //smtp.Port = 25;
        //smtp.Send(msg);
        //smtp.Dispose();
    }

    //public void sendemail(string MailTo, string MailCc, string _Body, string _Subject)
    //{
    //    insertsendmail(MailTo, MailCc, _Body, _Subject);
    //    MailTo = "voravut.somboornpong@thaiunion.com"; MailCc = "";
    //    MailMessage msg = new MailMessage();
    //    if (string.IsNullOrEmpty(MailTo)) return;
    //    string[] words = MailTo.Split(',');
    //    foreach (string word in words)
    //    {
    //        if (!string.IsNullOrEmpty(word))
    //            msg.To.Add(new MailAddress(word));
    //    }
    //    List<string> myList = new List<string>();
    //    string[] c = MailCc.Split(',');
    //    foreach (string s in c)
    //        if (!string.IsNullOrEmpty(s)) { 
    //            msg.CC.Add(new MailAddress(s));
    //            myList.Add(s);
    //    }
    //    //if(CurUserName!= "CP4790028")
    //    string MyCurMail = GetData(CurUserName, "email");
    //    if (!myList.Contains(MyCurMail)){
    //        msg.CC.Add(new MailAddress(MyCurMail));
    //    }
    //    msg.From = new MailAddress("Costing.WebBase@thaiunion.com");
    //    msg.Subject = string.Format("{0}", _Subject);
    //    msg.Body = _Body;// "Material  " + _Material.ToString() + " Created sap Complate";
    //    //msg.Attachments.Add(new Attachment(@"C:\\inetpub\wwwroot\WebService_jQuery_Ajax\FileTest\textfile.log"));
    //    msg.IsBodyHtml = true;
    //    SmtpClient client = new SmtpClient();
    //    client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
    //    client.Host = "smtp.office365.com";
    //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
    //    client.EnableSsl = true;  
    //    //client.Credentials = new System.Net.NetworkCredential("Costing.WebBase@thaiunion.com", "AAaa123*");
    //    NetworkCredential loginInfo = new NetworkCredential("Costing.WebBase@thaiunion.com", "AAaa123*"); // password for connection smtp if u dont have have then pass blank
    //    client.UseDefaultCredentials = false;
    //    SmtpClient smtp = new SmtpClient();
    //    smtp.UseDefaultCredentials = true;
    //    client.Credentials = loginInfo;
    //    //client.Credentials = new System.Net.NetworkCredential("wshuttleadm@thaiunion.com", "AAaa123*");
    //    client.Send(msg);
    //}
    public void sendemail_win(string MailTo,
     string MailCc,
     string _Body,
     string _Subject, string _Attachments)
    {
        MailTo = "voravut.somboornpong@thaiunion.com"; MailCc = "";
        insertsendmail(MailTo, MailCc, _Body, _Subject);
        MailMessage msg = new MailMessage();
        if (string.IsNullOrEmpty(MailTo)) return;
        string[] words = MailTo.Split(',');
        foreach (string word in words)
        {
            if (!string.IsNullOrEmpty(word))
                msg.To.Add(new MailAddress(word));
        }
        string[] c = MailCc.Split(',');
        foreach (string s in c)
            if (!string.IsNullOrEmpty(s))
                msg.CC.Add(new MailAddress(s));
        msg.From = new MailAddress("wshuttleadm@thaiunion.com");
        msg.Subject = _Subject;
        msg.Body = _Body;// "Material  " + _Material.ToString() + " Created sap Complate";
        if (!string.IsNullOrEmpty(_Attachments))
        {
            //@"C:\\inetpub\wwwroot\WebService_jQuery_Ajax\FileTest\textfile.log"
            msg.Attachments.Add(new Attachment(_Attachments));
        }
        msg.IsBodyHtml = true;
        SmtpClient client = new SmtpClient();
        client.UseDefaultCredentials = false;
        client.Credentials = new System.Net.NetworkCredential("wshuttleadm@thaiunion.com", "WSP@ss2018");
        client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
        client.Host = "smtp.office365.com";
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.EnableSsl = true;
        //client.Send(msg);
    }
    public string insertsendmail(string MailTo, string MailCc, string _Body, string _Subject)
    {
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            //SELECT FirstName + '.' + LastName + '@thaiunion.com' AS Email"
            cmd.CommandText = "Insert into MailData values(@Sender,@To,@Cc,'',@Subject,@Body,getdate(),1,getdate(),'TEXT',1,0)";
            cmd.Parameters.AddWithValue("@Sender", String.Format("{0}", 10));
            cmd.Parameters.AddWithValue("@To", MailTo.ToString());
            cmd.Parameters.AddWithValue("@Cc", MailCc.ToString());
            cmd.Parameters.AddWithValue("@Subject", _Subject.ToString());
            cmd.Parameters.AddWithValue("@Body", _Body.ToString());
            cmd.Connection = con;
            con.Open();
            var getValue = cmd.ExecuteScalar();
            con.Close();
            return ((string)getValue == null) ? string.Empty : getValue.ToString();
        }
    }
    //public string getuadmin()
    //{
    //    using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        SqlCommand cmd = new SqlCommand();
    //        cmd.CommandType = CommandType.Text;
    //        //SELECT FirstName + '.' + LastName + '@thaiunion.com' AS Email"
    //        cmd.CommandText = "select userlevel from ulogin where [user_name]=@User and IsResign=0";
    //        cmd.Parameters.AddWithValue("@User", CurUserName.ToString());
    //        cmd.Connection = con;
    //        con.Open();
    //        var getValue = cmd.ExecuteScalar();
    //        con.Close();
    //        con.Dispose();
    //        return ((string)getValue == null) ? string.Empty : getValue.ToString();
    //    }
    //}
    //public string roleadmin()
    //{
    //    using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        SqlCommand cmd = new SqlCommand();
    //        cmd.CommandType = CommandType.Text;
    //        //SELECT FirstName + '.' + LastName + '@thaiunion.com' AS Email"
    //        cmd.CommandText = "select userlevel from ulogin where [user_name]=@User and IsResign=0";
    //        cmd.Parameters.AddWithValue("@User", CurUserName.ToString());
    //        cmd.Connection = con;
    //        con.Open();
    //        var getValue = cmd.ExecuteScalar();
    //        con.Close();
    //        con.Dispose();
    //        return ((string)getValue == null) ? string.Empty : getValue.ToString();
    //    }
    //}
    public string GetSubject(string type)
    {
        string strSQL = @"select top 1 * from Masusertype 
            where dbo.fnc_checktype([ID],'" + string.Format("{0}", type) + "')>0";
        if (strSQL == "") return strSQL;
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlDataAdapter da = new SqlDataAdapter(strSQL, strConn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                return dr["Subject"].ToString();
            }
            return "";
        }
    }
    public string Getuser_name(string user_name, string type)
    {
        string strSQL = @"select user_name from ulogin 
            where [FirstName]+ ' ' + LastName='" + string.Format("{0}", user_name) + "' and IsResign=0";
        if (strSQL == "") return strSQL;
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlDataAdapter da = new SqlDataAdapter(strSQL, strConn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                switch (type.ToLower())
                {
                    case "user_name":
                        return dr["user_name"].ToString();
                }
            }
            return "";
        }
    }
    public DataTable GetSqlCommand(SqlCommand cmd)
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
    public string GetData(string user_name, string type)
    {
        string strSQL = @"select *,replace(Bu,';','|')nBu from ulogin 
            where [user_name]='" + string.Format("{0}", user_name) + "' and IsResign=0";
        if (strSQL == "") return strSQL;
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlDataAdapter da = new SqlDataAdapter(strSQL, strConn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                switch (type.ToLower())
                {
                    case "fn":
                        return dr["FirstName"] + " " + dr["LastName"];
                    case "email":
                        return dr["email"].ToString();
                    case "uselevel": case "X": case "userlevel":
                        return dr["userlevel"].ToString();
                    case "obu":
                        return dr["Bu"].ToString();
                    case "bu":
                        return dr["nBu"].ToString();
                    case "position":
                        return dr["Position"].ToString();
                    case "usertype":
                        return dr["usertype"].ToString();
                }
            } return "";
        }
    }
    //public string GetEmail(string user_name,string type)
    //{
    //    using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        SqlCommand cmd = new SqlCommand();
    //        cmd.CommandType = CommandType.Text;
    //        //SELECT FirstName + '.' + LastName + '@thaiunion.com' AS Email"
    //        cmd.CommandText = "select email from ulogin where [user_name]=@User and IsResign=0";
    //        cmd.Parameters.AddWithValue("@User", user_name.ToString());
    //        cmd.Connection = con;
    //        con.Open();
    //        var getValue = cmd.ExecuteScalar();
    //        con.Close();
    //        return (string)getValue;
    //    }
    //}
    //public string Getfullname(string user_name)
    //{
    //    using (SqlConnection con = new SqlConnection(strConn))
    //    {
    //        SqlCommand cmd = new SqlCommand();
    //        cmd.CommandType = CommandType.Text;
    //        //SELECT FirstName + '.' + LastName + '@thaiunion.com' AS Email"
    //        cmd.CommandText = "select FirstName+' '+LastName as 'fullname' from ulogin where [user_name]=@User and IsResign=0";
    //        cmd.Parameters.AddWithValue("@User", user_name.ToString());
    //        cmd.Connection = con;
    //        con.Open();
    //        var getValue = cmd.ExecuteScalar();
    //        con.Close();
    //        return (string)getValue;
    //    }
    //}

    public DataTable reload(DataRow _drow)
    {
        ServiceCS mycs = new ServiceCS();
        DataTable dt = new DataTable();
        SqlParameter[] param = {new SqlParameter("@Id",_drow["ID"].ToString()),
                new SqlParameter("@username",CurUserName.ToString())};
        dt = GetRelatedResources("spGetFormulaItems", param);
        var result = _createData().Clone();
        result.Clear();
        double[] doarray = new double[7];
        DataRow _r = result.NewRow();
        _r["RequestNo"] = _drow["RequestNo"].ToString();
        _r["Formula"] = _drow["Formula"].ToString();

        var values = new[] { "ingredient", "solid portion", "solution" };
        foreach (DataRow row in dt.Rows)
        {
            _r["PackSize"] = row["PackSize"].ToString();
            _r["PackSizeh"] = row["PackSizeh"].ToString();
            if (row["Component"].ToString().ToLower().Contains("raw material"))
                doarray[0] += Convert.ToDouble(row["PriceOfCarton"]);
            else if (row["Component"].ToString().ToLower().Contains("primary packaging"))
            {
                doarray[3] += Convert.ToDouble(row["PriceOfCarton"]);
            }
            else if (row["Component"].ToString().ToLower().Contains("secondary packaging"))
            {
                doarray[4] += Convert.ToDouble(row["PriceOfCarton"]);
            }
            else if (row["Component"].ToString().ToLower().Contains("upcharge"))
            {

                doarray[5] += Convert.ToDouble(row["PriceOfCarton"]);
            }
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
        DataTable dtFormula = GetMergeDataSource(Convert.ToInt32(_drow["Formula"]),
            "3", _drow["RequestNo"].ToString());
        //DataRow _rw = dtFormula.Select("").FirstOrDefault();
        double _sumLBRate = 0;
        foreach (DataRow row in dtFormula.Rows)
        {
            if (row["SellingUnit"].ToString() == "%")
            {
                object totalLBRate = dtFormula.Compute("Sum(LBRate)", "title in ('LOH','Labor & Overhead')").ToString();
                _sumLBRate += Convert.ToDouble(totalLBRate) * (Convert.ToDouble(row["LBRate"]) / 100);
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
    public string GetSinglePrice(DataRow row,DataRow _dr)
    {
        decimal r = 0;
        DataTable MyTable = reload(row);
        if (MyTable != null)
        {
            string[] SubType = Regex.Split("Raw Material & Ingredient;Primary Packaging", ";");
            string[] valueType = Regex.Split("RmIng;PrimaryPkg", ";");
            string[] arr = Regex.Split("RM;Ing;RmIng;UpCharge;LOH;PrimaryPkg", ";");
            foreach (DataRow rw in MyTable.Rows)
            {
                for (int i = 0; i <= arr.Length - 1; i++)
                    rw[arr[i]] = (Convert.ToDecimal(rw[arr[i]]) / Convert.ToDecimal(rw["PackSizeh"])) * Convert.ToDecimal(rw["PackSize"]);
                decimal _rm, _totalrm;
                var _Packaging = row["Packaging"].ToString();
                var tableloss = GettableLoss(_Packaging, Convert.ToDateTime(_dr["To"]), string.Format("{0}", row["UserType"]));
                for (int i = 0; i <= valueType.Length - 1; i++)
                {
                    _rm = Convert.ToDecimal(rw[valueType[i]]);
                    var _drloss = tableloss.Select("SubType='" + SubType[i] + "'").FirstOrDefault();
                    decimal _loss = 0;
                    decimal.TryParse(_drloss["Loss"].ToString(), out _loss);
                    _totalrm = _rm * (Convert.ToDecimal(_loss) / 100);
                    rw["Loss"] = Convert.ToDecimal(rw["Loss"]) + _totalrm;
                    //rw["FOB"] = Convert.ToDecimal(rw["FOB"]) + _totalrm + Convert.ToDecimal(rw[valueType[i]]);

                }
                //decimal totalCount = 0;
                //totalCount = Convert.ToDecimal(rw["FOB"]) + Convert.ToDecimal(rw["LOH"]) + Convert.ToDecimal(rw["UpCharge"]);
                //object sumObject = string.Format(@"{0}", rw["PerMargin"].ToString() == "" ? "0" : rw["PerMargin"].ToString());

                //decimal _MinPrice = (totalCount / ((100 - Convert.ToDecimal(sumObject)) / 100));
                //rw["Margin"] = _MinPrice * (Convert.ToDecimal(sumObject) / 100);

                List<decimal> list1 = new List<decimal>() { Convert.ToDecimal(rw["Ing"]),
                    Convert.ToDecimal(rw["RM"]),
                    Convert.ToDecimal(rw["PrimaryPkg"]),
                    Convert.ToDecimal(rw["LOH"]),
                    Convert.ToDecimal(rw["UpCharge"]),
                    Convert.ToDecimal(rw["Loss"])};
                r = list1.Sum();
                //if (r > 0)
                //{
                //    if (rw["Rate"].ToString() != "")
                //        r = r / Convert.ToDecimal(rw["Rate"]);
                //}
            }
        }

        return r.ToString("F",
                  CultureInfo.InvariantCulture);
    }
    public DataTable GetProductFormula(string Id)
    {
        SqlParameter[] param = { new SqlParameter("@Id", Id.ToString()) };
        return GetRelatedResources("spGetProductFormula", param);
    }
    public string ReadItems(string strQuery)
    {
        string result = "";
        // (ByVal FieldName As String, ByVal TableName As String, ByVal Cur As String, ByVal Value As String) As String
        DataTable dt = new DataTable();
        SqlConnection con = new SqlConnection(strConn);
        SqlDataAdapter sda = new SqlDataAdapter();
        SqlCommand cmd = new SqlCommand(strQuery);
        cmd.CommandType = CommandType.Text;
        cmd.Connection = con;
        con.Open();
        sda.SelectCommand = cmd;
        sda.Fill(dt);
        con.Close();
        con.Dispose();
        StringBuilder sb = new StringBuilder();
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
            {
                sb.Append(row[0] + ",");
            }
            if (result.Length < 2)
            {
                result = sb.ToString();
                result = result.Substring(0, (result.Length - 1));
            }
        }
        return result;
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
            oConn.Dispose();
            return dt;
        }
    }
    public DataTable GetElement(string Element)
    { //spGetElement
        var Results = new DataTable();
        SqlParameter[] param = { new SqlParameter("@tablename", (string)Element) };
        Results = GetRelatedResources("spGetElement", param);
        return Results;
    }
    public bool WriteJason(DataTable dt, string path)
    {
        try
        {

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = null;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, string>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName.Trim().ToString(), Convert.ToString(dr[col]));
                }
                rows.Add(row);
            }
            string jsonstring = serializer.Serialize(rows);

            using (var file = new StreamWriter(path, false))
            {
                file.Write(jsonstring);
                file.Close();
                file.Dispose();
            }
            return true;
        }
        catch { return false; }
    }
    public string newvalue()
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
            return (string)getValue;
        }
    }
    public string schars(object text)
    {
        StringBuilder sb = new StringBuilder();
        string[] split = text.ToString().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in split)
        {
            if (s.Trim() != "")
                sb.Append(GetData(s,"fn") + ",");
        }
        string chars = "";
        if (sb.Length > 0)
            chars = sb.ToString().Substring(0, sb.Length - 1);
        return chars;
    }
    //public double ConvertToDouble(string s)
    //{
    //    char systemSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
    //    double result = 0;
    //    try
    //    {
    //        if (s != null)
    //            if (!s.Contains(","))
    //                result = double.Parse(s, CultureInfo.InvariantCulture);
    //            else
    //                result = Convert.ToDouble(s.Replace(".", systemSeparator.ToString()).Replace(",", systemSeparator.ToString()));
    //    }
    //    catch (Exception e)
    //    {
    //        try
    //        {
    //            result = Convert.ToDouble(s);
    //        }
    //        catch
    //        {
    //            try
    //            {
    //                result = Convert.ToDouble(s.Replace(",", ";").Replace(".", ",").Replace(";", "."));
    //            }
    //            catch
    //            {
    //                throw new Exception("Wrong string-to-double format");
    //            }
    //        }
    //    }
    //    return result;
    //}
    public bool HasColumn(DataTable data, string columnName)
    {
        if (data == null || string.IsNullOrEmpty(columnName))
        {
            return false;
        }

        foreach (DataColumn column in data.Columns)
            if (columnName.Equals(column.ColumnName, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }
}
