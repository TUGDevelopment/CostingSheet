using ClosedXML.Excel;
using DevExpress.CodeParser;
using DevExpress.Data;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using DevExpress.Web;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxSpreadsheet;
using Newtonsoft.Json;
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
public partial class UserControls_CalculationForm : MasterUserControl
{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    HttpContext Context = HttpContext.Current;
    string FilePath    {
        get { return this.Session["sessionFile"] == null ? String.Empty : this.Session["sessionFile"].ToString(); }
        set { this.Session["sessionFile"] = value; }
    }
    //string separa
    //{
    //    get { return this.Session["separa"] == null ? String.Empty : this.Session["separa"].ToString(); }
    //    set { this.Session["separa"] = value; }
    //}
    public DataTable _UpChargedt
    {
        get { return Page.Session["UpChargedt"] == null ? null : (DataTable)Page.Session["UpChargedt"]; }
        set { Page.Session["UpChargedt"] = value; }
    }
    public DataTable _utilizedt
    {
        get { return Page.Session["utilizedt"] == null ? null : (DataTable)Page.Session["utilizedt"]; }
        set { Page.Session["utilizedt"] = value; }
    }
    public DataTable tcustomer
    {
        get { return Page.Session["tcustomer"] == null ? null : (DataTable)Page.Session["tcustomer"]; }
        set { Page.Session["tcustomer"] = value; }
    }
    public DataTable _dt
    {
        get { return Page.Session["setableKey"] == null ? null : (DataTable)Page.Session["setableKey"]; }
        set { Page.Session["setableKey"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            SetInitialRow();
    }
    public void SetInitialRow()
    {
        Session.Clear();
        hfuser["user_name"] = user_name;
        hfgetvalue["NewID"] = string.Format("{0}", cs.GetNewID());
        editor["Name"] = cs.IsMemberOfRole(string.Format("{0}", 0));
        hftype["type"] = string.Format("{0}", 0);
        Page.Session["BU"] = string.Format("{0}", cs.GetData(user_name, "bu"));
        hGeID["GeID"] = string.Format("{0}", 0);
        hCommission["Commis"] = "";
        hfUpchargeGroup["Upcharge"] = "";
    }
    string GetDescrip(string Code){
        return cs.ReadItems(@"select top 1 b.Description from massapMaterial b where b.Material='"+ 
            Code.ToString() + "'");
    }
    protected void gv_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        var values = new[] { "Upcharge" };
        string[] valueType = Regex.Split("Material;From;To;Commission;OverPrice;Pacifical;MSC;Margin;SubContainers", ";");
        DataRow dr = tcustomer.Rows.Find(e.Keys[0]);
        //foreach (DataColumn column in tcustomer.Columns)
        //{
        //    if (values.Any(column.ColumnName.Contains))
        for (int i = 0; i <= valueType.Length - 1; i++)
        {
            dr[valueType[i]] = e.NewValues[valueType[i]];
            if (valueType[i] == "Material")
                dr["Description"] = GetDescrip(e.NewValues[valueType[i]].ToString());
        }
        //}
        DataRow rw = _dt.Select("Name='FOB price(18 - digits)'").FirstOrDefault();
        if (rw != null)
        {
            dr["MinPrice"] = rw["Result"].ToString();
            dr["OfficePrice"] = _dt.Compute("Max(Result)", "Name='Offer Price'").ToString();
        }
        var valuesupcha = new[] { "Secondary Packaging", "FOB price(18 - digits)" };
        DataRow[] result = _dt.Select(string.Format("Calcu='{0}'", 6));
        foreach (DataRow _rw in result)
        {
            if (!valuesupcha.Any(_rw["Name"].ToString().Contains))
            {
                DataRow rwupcharge = _UpChargedt.Select(string.Format("UpCharge in ('{0}') and SubID ='{1}' and RequestNo ='{2}'", 
                    _rw["Name"], e.Keys[0], hGeID["GeID"])).FirstOrDefault();
                if (rwupcharge != null)
                {
                    rwupcharge["UpchargeGroup"] = _rw["Component"];
                    rwupcharge["UpCharge"] = _rw["Name"];
                    rwupcharge["Quantity"] = _rw["Quantity"];
                }
                else
                {
                    DataRow _rwupcharge = _UpChargedt.NewRow();
                    int NextRowID = Convert.ToInt32(_UpChargedt.AsEnumerable()
                        .Max(row => row["ID"]));
                    NextRowID++;
                    _UpChargedt.Rows.Add(NextRowID,
                        _rw["Component"],
                        _rw["Name"],
                        _rw["Price"],
                        _rw["Quantity"],
                        _rw["Currency"],
                        _rw["Result"],
                        e.Keys[0],
                        hGeID["GeID"]);
                }
            }
        }

        //utilizedt
        List<string> list = new List<string>();
        foreach (DataRow rwuti in _utilizedt.Rows)
        {
            list.Add(rwuti["Result"].ToString());
        }
        if (list.Count > 0)
            dr["Utilize"] = String.Join("|", list.ToArray());

        g.CancelEdit();
        e.Cancel = true;

        g.JSProperties["cpUpdatedMessage"] = "successfully";
    }
    DataRow updated(DataRow dr, int ID, string X)
    {

        return dr;
    }
        protected void gv_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        int NextRowID = Convert.ToInt32(tcustomer.AsEnumerable()
            .Max(row => row["Id"]));
        DataRow rw = tcustomer.NewRow();
        NextRowID++;
        rw["ID"] = NextRowID;
        var values = new[] { "ID", "RowID", "RequestNo" };
        foreach (DataColumn column in tcustomer.Columns)
        {
            if (!values.Any(column.ColumnName.Contains))
            {
                rw[column.ColumnName] = e.NewValues[column.ColumnName];
            }
        }
        tcustomer.Rows.Add(rw);
        e.Cancel = true;
        g.CancelEdit();
    }
    DataTable exchangerate()
    {
       SqlParameter[] param = { new SqlParameter("@from", DateTime.Now),
            new SqlParameter("@to", DateTime.Now)};
        var dt = cs.GetRelatedResources("spTunaStdExchangeRat", param);
        return dt;
    }
    protected void gv_CustomDataCallback(object sender, DevExpress.Web.ASPxGridViewCustomDataCallbackEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var result = new Dictionary<string, string>();
        var args = e.Parameters.Split('|');
        if (args[0] == "reload")
        {
            tcustomer = new DataTable();
            tcustomer = buildLoadgv;
        }
        if (args[0] == "MSC")
        {
            DataTable table = GetCertificate_fee(args[1]);
            foreach(DataRow rw in table.Rows)
            {
                result["value"] = string.Format(@"{0}", rw["free"]);
                e.Result = result;
            }
        }
            if (args[0] == "build")
        {
            DataRow dr = tcustomer.Rows.Find(args[1]);
            if(dr!=null)
            {
                result["Material"] =string.Format(@"{0}", dr["Material"]);
                result["From"] =  Convert.ToDateTime(dr["From"]).ToString("dd-MM-yyyy");
                result["To"] = Convert.ToDateTime(dr["To"]).ToString("dd-MM-yyyy");
                result["ID"] = string.Format("{0}", args[1]);
                result["SubContainers"] = dr["SubContainers"].ToString();
                //foreach (DataRow _rwx in exchangerate().Rows){
                //    result["Rate"] = _rwx["To"].ToString();
                //    result["Currency"] = _rwx["Currency"].ToString();
                //}
            }
            e.Result = result;
        }
        //else
        //e.Result = string.Format("{0}", e.Parameters);
    }
    //
    DataTable GetCertificate_fee(string separa)
    {
        DataTable dt = new DataTable();
        if (!string.IsNullOrEmpty(separa))
        {
            SqlParameter[] param = { new SqlParameter("@Code", string.Format("{0}", separa)) };
            dt = cs.GetRelatedResources("spGetCertificate_fee", param);
        }
        return dt;
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
        if (args[0] == "del")
        {

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
        if (args[0] == "reload")
        {
            tcustomer = new DataTable();
            tcustomer = buildgv(args[1]);
            tcustomer.PrimaryKey = new DataColumn[] { tcustomer.Columns["ID"] };
            //UpCharge
            BuildUpCharge(args[1]);
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
    void BuildUpCharge(string Keys)
    {
        SqlParameter[] param = { new SqlParameter("@ID", Keys.ToString()) };
        _UpChargedt= cs.GetRelatedResources("spGetstdUpCharge", param);
        _UpChargedt.PrimaryKey = new DataColumn[] { _UpChargedt.Columns["ID"] };
    }
    DataRow BuildCustom(DataRow dr, int ID, string X)
    {
        var arr = new[] { "MinPrice", "OverPrice", "Extracost", "SubContainers", "OfferPrice" };
        foreach (DataColumn column in tcustomer.Columns)
        {
            var n = column.ColumnName;
            //if (n.Contains("Material"))
            //    dr[n] = CmbMaterial.Text;
            //else if (n.Contains("ID"))
            //    dr[n] = ID;
            //else if (n == "Customer")
            //    dr[n] = string.Format(@"{0}", CmbCustomer.Value);
            //else if (n == "ShipTo")
            //    dr[n] = string.Format(@"{0}", CmbShipTo.Value);
            if (n.Contains("PaymentTerm"))
                dr[n] = CmbPaymentTerm.Value;
            //else if (n.Contains("Interest"))
            //    dr[n] = tbinterest.Value;
            //else if (n.Contains("Commission"))
            //    dr[n] = CmbCommission.Text;
            //else if (n.Contains("Incoterm"))
            //    dr[n] = CmbIncoterm.Value;
            //else if (n.Contains("Route"))
            //    dr[n] = CmbRoute.Value;
            //else if (n.Contains("Size"))
            //    dr[n] = CmbSize.Value;
            //else if (n.Contains("Quantity"))
            //    dr[n] = tbNumberContainer.Text;
            //else if (n.Contains("Freight"))
            //    dr[n] = tbFreight.Text;
            //else if (n.Contains("Currency"))
            //    dr[n] = CmbCurrency.Text;
            //else if (n.Contains("ExchangeRate"))
            //    dr[n] = seExchangeRate.Text;
            //else if (n.Contains("Insurance"))
            //    dr[n] = tbInsurance.Text;
            //else if (n.Contains("Remark"))
            //    dr[n] = mRemark.Text;
            else
                dr[n] = "";
        }
        return dr;
    }
    protected void gv_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.KeyFieldName = "ID";
        g.DataSource = tcustomer;
        g.ForceDataRowType(typeof(DataRow));
    }
    DataTable buildgv(string Id)
    {
            SqlParameter[] param = { new SqlParameter("@ID", Id.ToString()) };
        return cs.GetRelatedResources("spGetTunaStdItems", param);
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

    protected void gvutilize_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        if (args[0] == "reload")
        {
            this.Page.Session.Remove("utilizedt");
            _utilizedt = Getutilize();
        }
        g.DataBind();
    }
    DataTable Getutilize()
    {
        DateTime from = Convert.ToDateTime(defrom.Value).Date;
        //DateTime to = Convert.ToDateTime(defrom.Value);
        int monthDiff = cs.GetMonthsBetween(from, Convert.ToDateTime(deto.Value).Date);
        int monthNumber = Convert.ToInt32(from.ToString("MM", CultureInfo.InvariantCulture)); 
        int i = 0;
        monthDiff++;
        using (DataTable table = new DataTable())
        {
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Result", typeof(double));
            table.Columns.Add("StatusApp", typeof(string));
            table.Columns.Add("Cost", typeof(string));
            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = table.Columns["Id"];
            table.PrimaryKey = keyColumns;
            int initial = 100;
            int total = GetTotal(initial, monthDiff);
            while (i < monthDiff)
            {
                i++;
                string monthName = new DateTimeFormatInfo().GetAbbreviatedMonthName(monthNumber);
                
                table.Rows.Add(i, monthName, total<=initial?total:initial, "");
                monthNumber++;
                if (monthNumber > 12)
                    monthNumber = monthNumber - 12;
                initial = initial - total;
            }
     
        return table;
        }
    }
    protected void gvutilize_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.KeyFieldName = "Id";
        g.DataSource = _utilizedt;
        g.ForceDataRowType(typeof(DataRow));
    }
    public void UpdateData(GridDataItem model)
    {
        DataRow dr = _utilizedt.Rows.Find(model.Id);
        var values = new[] { "Result" };
        foreach (DataColumn column in _utilizedt.Columns)
        {
            if (values.Any(column.ColumnName.Contains))
                dr[column.ColumnName] = model.Result;
        }
    }
    protected void ASPxCallback_Callback(object source, CallbackEventArgs e)
    {
        try
        {
            UpdateData(JsonConvert.DeserializeObject<GridDataItem>(e.Parameter));
            e.Result = "OK";
        }
        catch (Exception ex)
        {
            e.Result = ex.Message;
        }
    }
    int GetTotal(int rowTotal,int rowPerPage)
    {
        float pageTotal = 0;
        int pTotal = 0;
        if (rowTotal % rowPerPage == 0)
        {
            pTotal = rowTotal / rowPerPage;
        } else {
            pageTotal = rowTotal / rowPerPage;
            pTotal = (int)Math.Floor(pageTotal) + 1;
        }
        return pTotal;
    }
    protected void gvutilize_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        var values = new[] { "Result" };
        bool mark = false;
        int intpercent = 100;
        int c = _utilizedt.Rows.Count;
        foreach (DataRow dr in _utilizedt.Rows)
        {
            //DataRow dr = _utilizedt.Rows.Find(e.Keys[0]);
            foreach (DataColumn column in _utilizedt.Columns)
            {
                if (column.ColumnName == "Result")
                {
                    if (dr["Id"].ToString() == e.Keys[0].ToString())
                    {
                        mark = true;
                        if (values.Any(column.ColumnName.Contains))
                        {
                            dr[column.ColumnName] = e.NewValues[column.ColumnName].ToString();
                            c = c - 1;
                            intpercent = intpercent - Convert.ToInt32(dr[column.ColumnName]);
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
                            intpercent = intpercent - Convert.ToInt32(dr[column.ColumnName]);
                        }
                    }
                }
            }
        }
        g.CancelEdit();
        e.Cancel = true;
    }

    protected void gvitems_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        if (args[0] == "clear")
        {
            this.Page.Session.Remove("setableKey");
            _dt = new DataTable();
        }
            if (args[0] == "Insert")
        {
            insertsGrid(args[1],args[2]);
        }
        if (args[0] == "reload")
        {
            this.Page.Session.Remove("setableKey");
            _dt = GetCalcu(args[1], args[2]);
        }
        if (args[0] == "updated"){
            var value = args[1];
            var rowsToUpdate = _dt.AsEnumerable().Where(r => r.Field<string>("Name") == value).FirstOrDefault();
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
        g.DataBind();
    }
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
    DataTable GetMedia(string Code,string group)
    {
        DataTable dt = new DataTable();
        if (defrom.Value==null || deto.Value == null) return dt;
        SqlParameter[] param = {
            new SqlParameter("@group", group.ToString()),
            new SqlParameter("@from", defrom.Value),
            new SqlParameter("@to", deto.Value),
            new SqlParameter("@Code", Code.ToString())};
        dt = cs.GetRelatedResources("spTunaStdMedia", param);
        return (DataTable)dt;
    }
    string GetAvgFishPrice(string separa,DataRow rw, object FishGroup,object FishCert,object SHD)
    {
        if (string.IsNullOrEmpty(separa)) return "";
                   decimal result = 0;
            if (defrom.Value==null || deto.Value == null) return string.Format("{0}", 0);
            SqlParameter[] param = { new SqlParameter("@from", defrom.Value),
            new SqlParameter("@to", deto.Value),
            new SqlParameter("@cols", rw["Name"]),
            new SqlParameter("@FishGroup",string.Format("{0}",FishGroup)),
            new SqlParameter("@FishCert",string.Format("{0}",FishCert)),
            new SqlParameter("@SHD",string.Format("{0}",SHD)),
            new SqlParameter("@Code",string.Format("{0}",separa))};
            var dt = cs.GetRelatedResources("spTunaStdFishPrice", param);
                if (dt.Rows.Count > 0){
                result = Convert.ToDecimal(dt.Compute("Max(Result)", "").ToString());
                if (dt.Rows[0]["Unit"].ToString() == "KG")
                    result = result * 1000;
                }
        if (dt.Rows.Count == 0) return ""; 
            string[] arr = { result.ToString(), dt.Rows[0]["Name"].ToString(), dt.Rows[0]["Currency"].ToString(),
         dt.Rows[0]["Unit"].ToString()};
        return string.Join("|", arr).ToString();
    }
    DataTable GetFillWeight(string separa)
    {
        DataTable dt = new DataTable();
        if (!string.IsNullOrEmpty(separa)) {
            SqlParameter[] param = { new SqlParameter("@Code",string.Format("{0}",separa))};
            dt = cs.GetRelatedResources("spGetFillWeight", param);
        }
        return dt;
    }
    decimal GetResultdt(DataTable dt,string strSQL)
    {
        decimal result = 0;
        DataRow _row = dt.Select(strSQL).FirstOrDefault();
        if (_row != null){
            result = Convert.ToDecimal(_row["Result"].ToString());
        }
        return result;
    }
    public static double RoundUp(double input, int places)
    {
        double multiplier = Math.Pow(10, Convert.ToDouble(places));
        return Math.Ceiling(input * multiplier) / multiplier;
    }
    DataTable GetCalcu(string Code,string SubId)
    {
        int Index = gv.EditingRowVisibleIndex;
        object keyValue = gv.GetRowValues(Index, "Material");
        if (keyValue != null)
            Code = string.Format("{0}", keyValue);
        int Id = Convert.ToInt32(hGeID["GeID"]);
        SqlParameter[] param = { new SqlParameter("@Param", Id.ToString()),
        new SqlParameter("@Code", Code.ToString())};
        var dt = cs.GetRelatedResources("spGetTunaCalculation", param);
        dt.PrimaryKey = new DataColumn[] { dt.Columns["RowID"] };

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
                object totalfillweight=0;
                decimal _Yield = 0, _FillWeight = 0, _sumFillWeight = 0;
                string stdPackSize = dt.Compute("Max(Result)", "OrderIndex = 9").ToString();
                DataTable _dtFillWeight = GetFillWeight(Code);
            if (_dtFillWeight.Rows.Count == 0) return null;
            if (_utilizedt != null)
            {
                decimal totlpertuni = 0, _sumraw = 0;
                string stdprice = "", _Currency = "";
                foreach (DataRow _rw in _dtFillWeight.Rows)
                {
                    decimal _culcu = 0;
                    string _sumuti = _utilizedt.Compute("Sum(Result)", "").ToString();
                    _FillWeight = Convert.ToDecimal(_rw["Result"].ToString());
                    _Yield = Convert.ToDecimal(_rw["Yield"].ToString()) / 100;
                    totlpertuni = 0;
                    foreach (DataRow _r in _utilizedt.Rows)
                    {
                        var args = GetAvgFishPrice(Code.ToString(), _r, _rw["FishGroup"], _rw["FishCert"], _rw["SHD"]).Split('|');
                        if (args[0].ToString() == "") goto JumpEx;
                        if (args.Length > 1)
                        {
                            stdprice = args[0].ToString();
                            _r["Cost"] = args[0].ToString();
                        }
                        if (!string.IsNullOrEmpty(_r["Result"].ToString()) && _sumuti != "0")
                        {
                            decimal _value = Convert.ToDecimal(stdprice) * (Convert.ToDecimal(_r["Result"].ToString()) / Convert.ToDecimal(_sumuti));
                            stdprice = _value.ToString("F");
                            totlpertuni += _value;
                            //totlpertuni += Convert.ToDecimal(_r["Result"].ToString());
                        }
                        _culcu += Convert.ToDecimal(_FillWeight / 1000 / _Yield * Convert.ToDecimal(stdprice) / 1000 * Convert.ToDecimal(stdPackSize));
                        _Currency = args[2].ToString();

              
                    }
          _sumraw += _culcu;
                    _sumFillWeight += _FillWeight;                    
                    table.Rows.Add(i++, "RM", string.Format("{0} {1}", _rw["Name"], _rw["FishCert"]), _rw["Result"], _culcu.ToString("F"), 1,
                        _Yield, totlpertuni.ToString("F"), _Currency + "/Case");
                    table.Rows.Add(i++, "Raw material", string.Format("{0} {1}", _rw["Name"], _rw["FishCert"]), _Currency + "/Case", _culcu.ToString("F"), 8,
                        _FillWeight.ToString("F"), totlpertuni.ToString("F"), "");
                }
                string name = string.Format(@"Cost Per Case ({0})", stdPackSize);
                totalfillweight = _dtFillWeight.Compute("Sum(Result)", "");//calculator raw material fillweight
                //totalfillweight = Convert.ToDecimal(totalfillweight) / Convert.ToDecimal(_dtFillWeight.Rows.Count);
                table.Rows.Add(i++, "", name, "", _sumraw.ToString("F"), 1, "", "", _Currency + "/Case");
                //table.Rows.Add(i++, "", "Raw material", _Currency + "/Case", _sumraw.ToString("F"), 8, _sumFillWeight.ToString("F"), totlpertuni.ToString("F"), "");
                }
                //Media(ing)
                decimal _summedia = 0;
                foreach (DataRow _rw in GetMedia(Code.ToString(), "Media").Rows)
                {
                    decimal _oilloss = Getloss(_rw["GroupDescription"].ToString());
                    decimal _sumoil = Convert.ToDecimal(_rw["Price"].ToString()) / 1000 * Convert.ToDecimal(_rw["MediaWeight"].ToString()) * (1 + _oilloss / 100);
                    table.Rows.Add(i++,  string.Format("{0}", _rw["GroupDescription"]), _rw["CodeName"], _oilloss.ToString(), _sumoil.ToString("F"), 2, 
                        _rw["MediaWeight"].ToString(), _rw["Price"].ToString(),
                        _rw["Currency"]+"/"+ _rw["Unitofmeasurement"], _rw["Currency"] +"/Can");
                    _summedia += _sumoil;
                }
                decimal _media = Convert.ToDecimal(_summedia * Convert.ToDecimal(stdPackSize) / Convert.ToDecimal(seExchangeRate.Value));
                table.Rows.Add(i++, "Media", string.Format("Media Cost Per Case ({0})", stdPackSize), "", _media.ToString("F"), 2, "", "", "","USD/Case");
                table.Rows.Add(i++, "", "Media", "USD/Case", _media.ToString("F"), 8, "", "", "");
                decimal _primary = 0;
                foreach (DataRow _rw in GetMedia(Code.ToString(), "PKG").Rows)
                {
                    DataRow _result = dt.Select("OrderIndex = '6'").FirstOrDefault();
                    //decimal _primary = Convert.ToDecimal(_rw["Price"]) * (1 + (Convert.ToDecimal(_result["Result"])/100)) * (Convert.ToDecimal(stdPackSize) / Convert.ToDecimal(seExchangeRate.Value));
                    decimal _priloss = Getloss(_rw["GroupDescription"].ToString());
                    decimal _sumpri = Convert.ToDecimal(_rw["Price"]) * (1 + _priloss / 100);
                    table.Rows.Add(i++, _rw["GroupDescription"], _rw["Description"], _rw["Currency"]+"/" + _rw["Unit"], _sumpri.ToString("F2"), 3, _priloss.ToString(), _rw["Price"], 0,
                        _rw["Currency"] +"/"+ _rw["Unitofmeasurement"]);
                    _primary += _sumpri;
                }
                decimal _Packaging = Convert.ToDecimal(_primary * Convert.ToDecimal(stdPackSize) / Convert.ToDecimal(seExchangeRate.Value));
                table.Rows.Add(i++, "Packaging", string.Format("Primary Packaging per case ({0})", stdPackSize), "",
                    String.Format("{0:0.00}",_Packaging.ToString("F2")), 3, "", "", "", "USD/Case");
                table.Rows.Add(i++, "", "Primary Packaging", "USD/Case", _Packaging.ToString("F"), 8, "", "", "");
                decimal _sumLOHCost = 0;
                foreach (DataRow _rw in GetMedia(Code.ToString(), "Labor").Rows)
                {
                    if (_rw["Title"].ToString() == "Laborcost")
                    {
                    decimal _Labor = Convert.ToDecimal(_rw["Cost"]) * Convert.ToDecimal(totalfillweight) * Convert.ToDecimal(stdPackSize);
                    table.Rows.Add(i++, "DL", string.Format("{0}  by grade", _rw["Title"]), _rw["Currency"] + "/" + _rw["Unit"], _Labor, 4, totalfillweight, Convert.ToDecimal(_rw["Cost"]), 0,
                        _rw["Currency"] + "/Case");
                    _sumLOHCost += _Labor;
                    }
                    else
                    {
                        //DataRow _rwx = GetMedia(Code.ToString(), "Style").Select("").FirstOrDefault();
                        decimal _LOHCost = Convert.ToDecimal(_rw["Cost"]) / Convert.ToDecimal(_rw["StdPackSize"]) * Convert.ToDecimal(stdPackSize);
                        table.Rows.Add(i++, _rw["Title"], string.Format("{0} {2} {3} pack size {1}", _rw["Title"], stdPackSize,_rw["PackagingType"],_rw["Size"]), 
                            _rw["Currency"]+"/"+ _rw["Unit"], _LOHCost.ToString(), 4, "", _rw["Cost"], "", _rw["Currency"] + "/Case");
                        _sumLOHCost += _LOHCost;
                    }
                }
                decimal _LOH = Convert.ToDecimal(_sumLOHCost / Convert.ToDecimal(seExchangeRate.Value));
                table.Rows.Add(i++, "LOH", string.Format("LOH per pack {0}", stdPackSize), "", _LOH.ToString("F"), 4, "", "", "", "USD/Case");
                table.Rows.Add(i++, "", string.Format("LOH per pack {0}", stdPackSize), "USD/Case", _LOH.ToString("F"), 8, "", "", "");
                object sumObject =0;
                //sumObject = table.Compute("Sum(Result)", "Component in ('LOH','Media','Packaging')");
                //table.Rows.Add(i++, "Total Price", string.Format("16 - digit price per pack {0}", stdPackSize), "USD/Case", sumObject, 8, 0, 0, 0);

                //Packing Style
                decimal _totalPrice = 0; decimal _totalStyle = 0;decimal _PackSize = 0; decimal _Margin = 0; decimal _sumprice = 0;
            foreach (DataRow _rw in GetMedia(Code.ToString(), "Style").Rows)
            {
                //table.Rows.Add(i++, "","Pack size","", _rw["PackSize"], 5, stdPackSize, 0);
                //table.Rows.Add(i++, "","Packing LOH cost per case", "", _rw["LaborCost"],5, stdPackSize, 0);
                //table.Rows.Add(i++, "","Secondary Packaging cost per case", "", _rw["SecPKGCost"], 5, stdPackSize, 0);
                decimal LaborCost;
                decimal.TryParse(_rw["LaborCost"].ToString(), out LaborCost);
                decimal _sumStyle = LaborCost ;
                _totalStyle = _sumStyle / Convert.ToDecimal(seExchangeRate.Value);
                table.Rows.Add(i++, Code.Substring(16, 2), _rw["GroupStyle"], _rw["PackSize"], _sumStyle, 5, LaborCost, 0, _rw["Currency"] + "/" + _rw["Unit"]);
                _PackSize = Convert.ToDecimal(_rw["PackSize"]);
                //table.Rows.Add(i++, "Cansize", _rw["Size"],"",0, 5, 0, 0, 0);
                //table.Rows.Add(i++,"Packing LOH cost per case", _rw["LaborCost"],"",0, 5, 0, 0, _rw["PackSize"]);
                //table.Rows.Add(i++,"Secondary Packaging cost per case", _rw["PackSize"],"",0, 5, 0, 0, _rw["PackSize"]);
                table.Rows.Add(i++, "Packing Style", string.Format("Pack size {0}", _rw["PackSize"]), "", _totalStyle.ToString("F"), 5, "", "", "USD/" + _rw["Unit"]);
                table.Rows.Add(i++, "", "Packing Style", "USD/" + _rw["Unit"], _totalStyle.ToString("F"), 8, "", "", "");

                decimal _sumSecPKGCost = 0;
                foreach (DataRow _xrw in GetSecPKGCost(Code.ToString()).Rows)
                {
                    //string _UnitUpcharge = _xrw["Currency"].ToString() + "/" + _xrw["Unit"].ToString();
                    string _UnitSecPKGCost = "THB/Case";
                    decimal _SecAmount = Convert.ToDecimal(Convert.ToDecimal(_xrw["Amount"]) / Convert.ToDecimal(seExchangeRate.Value));
                    table.Rows.Add(i++, "", "Secondary Packaging per case", _UnitSecPKGCost, _SecAmount.ToString("F"), 5, "", Convert.ToDecimal(_xrw["Amount"]).ToString("F"),
                        "USD/Case");
                    //"USD/" + _xrw["Unit"].ToString());
                    _sumSecPKGCost += _SecAmount;
                }
                decimal _totalSecPKGCost = Convert.ToDecimal(_sumSecPKGCost);
                table.Rows.Add(i++, "", "Secondary Packaging", "USD/Case", _totalSecPKGCost.ToString("F"), 8, "", "", "");
                //sumObject = table.Compute("Sum(Result)", "Component='Total Price'");
                //upcharge
                decimal _totupcharge = 0;
                DataRow[] result = _UpChargedt.Select(string.Format("SubID='{0}'", SubId));
                foreach (DataRow _row in result)
                {
                    table.Rows.Add(i++, _row["UpchargeGroup"],
                        _row["Upcharge"],
                        _row["Currency"],
                        _row["Result"],
                        6,
                        _row["Quantity"],
                        _row["Price"],
                        0);
                    _totupcharge += Convert.ToDecimal(_row["Result"].ToString());
                }
                table.Rows.Add(i++, "", "Upcharge", "USD/Case", _totupcharge.ToString("F"), 8, "", "", "");

                sumObject = table.Compute("Sum(Result)", "Component in ('LOH','Media','Packaging')");
                _sumprice = Convert.ToDecimal(sumObject);
                if (sumObject != System.DBNull.Value)
                {
                    //foreach (DataRow _rwx in GetMedia(Code.ToString(), "Margin").Rows)
                    //{
                    //    _Margin = Convert.ToDecimal(_rwx["Margin"]);
                    //    decimal _totalmargin = _sumprice * (Convert.ToDecimal(_rwx["Margin"]) / 100);
                    //    table.Rows.Add(800, "", "Margin", "USD/Case", _totalmargin.ToString("F"), 8, _Margin, 0, 0);
                    //}
                    table.Rows.Add(800, "", "Margin", "USD/Case", 0, 8, _Margin, 0, 0);
                }
            }


            //table.Rows.Add(i++, "", "Upcharge1(Packing style per can)", "", 0, 8, 0, 0);
            //table.Rows.Add(i++, "", "Upcharge2(Packing style per case)", "", 0, 8, 0, 0);

            int summary = 802;
            //sumObject = table.Compute("Sum(Result)", "Name in ('Margin', 'Secondary Packaging')");
            if (sumObject != System.DBNull.Value){
                _totalPrice = ((_sumprice / Convert.ToDecimal(stdPackSize) * Convert.ToDecimal(_PackSize)) + _totalStyle) * (1 + Convert.ToDecimal(_Margin) / 100);
                table.Rows.Add(summary++, "", "FOB price(18 - digits)", "USD/Case", Convert.ToDecimal(_totalPrice).ToString("F"), 8, 0, 0, 0);
                }
            
            //table.Rows.Add(i++, "", "Margin", "", 0, 10, 0, 0, 0);
            //table.Rows.Add(i++, "", "FOB price", "", 0, 11, 0, 0, 0);
            string _currency = "USD/pack size";
                table.Rows.Add(summary++, "", "Value Commission", _currency, 0, 8, 0, 0, 0);
                table.Rows.Add(summary++, "", "OverPrice", _currency, 0, 8, 0, 0, 0);
                table.Rows.Add(summary++, "", "FOB Price", _currency, 0, 8, 0, 0, 0);
            
                //table.Rows.Add(i++, "", "Payment Term", "", 0, 16, 0, 0, 0);
                //table.Rows.Add(i++, "", "FOB Price 2", "", 0, 17, 0, 0, 0);

                table.Rows.Add(summary++, "", "Route", "", 0, 8, 0, 0, 0);
                table.Rows.Add(summary++, "", "Insurance", "", 0, 8, 0, 0, 0);    
                table.Rows.Add(summary++, "", "Interest", "", 0, 8, 0, 0, 0);
                table.Rows.Add(summary++, "", "CIF Price", _currency, 0, 8, 0, 0, 0);

                table.Rows.Add(summary++, "", "Pacifical", "", 0, 8, 0, 0, 0);

                table.Rows.Add(summary++, "", "MSC", "", 0, 8, 0, 0, 0);
                table.Rows.Add(summary++, "", "Offer Price", _currency, 0, 8, 0, 0, 0);
                

            DataRow dr = tcustomer.Rows.Find(SubId);
            if (dr!=null)
            {
                string[] valueType = Regex.Split("Commission;OverPrice;Pacifical;MSC;Margin;Upcharge", ";");
                for (int ii = 0; ii <= valueType.Length - 1; ii++)
                {
                    string value = valueType[ii].ToString();
                    var rowsToUpdate = table.AsEnumerable().Where(r => r.Field<string>("Name") == value);
                    //DataRow []res = table.Select(string.Format("Name='{0}'", valueType[ii]));
                    foreach (var _row in rowsToUpdate)
                    {
                        switch (value)
                        {
                            case "Route":
                                double _SubContainers = 0, sumFreight = 0; 
                                double Freight = 0;
                                if (double.TryParse(dr["SubContainers"].ToString(), out _SubContainers))
                                {
                                    double.TryParse(tbFreight.Text, out Freight);
                                    sumFreight=(Convert.ToDouble(Freight) / Convert.ToDouble(_SubContainers));
                                    //table.Rows.Add(i++, "Route", CmbRoute.Text, "USD/Case",
                                    //    sumFreight.ToString("F2"), 7, _SubContainers, Freight, 0);
                                    //_row.SetField("Result", sumFreight.ToString("F"));
                                }
                                    break;
                            case "Margin":
                                if (decimal.TryParse(dr["Margin"].ToString(), out _Margin))
                                {
                                    //decimal _totalmargin = _sumprice * (_Margin / 100);
                                    //table.Rows.Add(800, "", "Margin", "USD/Case", _totalmargin.ToString("F"), 8, _Margin, 0, 0);
                                    //_row.SetField("Result" , _totalmargin.ToString("F"));
                                    _row.SetField("Quantity", _Margin.ToString("F"));
                                }
                                break;
                            case "OverPrice":
                            case "MSC":
                            case "Pacifical":
                                _row.SetField("Quantity", dr[value].ToString());
                                break;
                            case "UpCharge":
                                DataRow[] dtUpCharge = _UpChargedt.Select(string.Format("SubID='{0}'", SubId));
                                foreach (DataRow _rw in dtUpCharge)
                                {
                                    table.Rows.Add(_rw["ID"], 
                                        _rw["UpchargeGroup"],
                                        _rw["Upcharge"],
                                        _rw["Currency"],
                                        _rw["Result"],
                                        6,
                                        _rw["Quantity"],
                                        _rw["Price"],
                                        0);
                                }
                                break;
                        }
                        _row.AcceptChanges();
                    }
                }
            }
            //table.Rows.Add(i++, "", "Discount", "", 0, 24, 0, 0, 0);
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
            new SqlParameter("@Material", value),
        new SqlParameter("@Customer", CmbCustomer.Value),
        new SqlParameter("@ShipTo", CmbShipTo.Value)};
        var Results = cs.GetRelatedResources("spStdSecPKGCost", param);
   
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
    double GetSumutilize()
    {
        object sumObject;
        if (_utilizedt == null) return 0;
        sumObject = _utilizedt.Compute("Sum(Result)", string.Empty);
        return (double)sumObject;
    }
    protected void gvitems_DataBinding(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        g.KeyFieldName = "RowID";
        g.DataSource = ReCreateColumns;
        g.ForceDataRowType(typeof(DataRow));

    }
    DataTable GetCurrentTable()
    {
        DataTable dt = new DataTable();
        dt.Clear();
        dt.Columns.Add("Name");
        dt.Columns.Add("Marks");
        dt.Rows.Add(new object[] { "RowID;Calcu", 0 });
        dt.Rows.Add(new object[] { "RowID;Component;Name;Currency;Quantity;Price;Result;Unit;Calcu", 1 });
        dt.Rows.Add(new object[] { "RowID;Component;Name;Currency;Quantity;Price;Unit;Result;BaseUnit;Calcu", 2 });
        dt.Rows.Add(new object[] { "RowID;Component;Name;Quantity;Price;Currency;Result;BaseUnit;Calcu", 3 });
        dt.Rows.Add(new object[] { "RowID;Component;Name;Quantity;Price;Currency;Result;BaseUnit;Calcu", 4 });
        dt.Rows.Add(new object[] { "RowID;Component;Name;Currency;Price;Result;Unit;Calcu", 5 });
        dt.Rows.Add(new object[] { "RowID;Component;Name;Price;Result;Currency;Calcu", 6 });
        dt.Rows.Add(new object[] { "RowID;Component;Name;Quantity;Price;Result;BaseUnit;Calcu", 7 });
        dt.Rows.Add(new object[] { "RowID;Component;Name;Quantity;Price;Result;Currency;Calcu", 8 });
        return dt;
    }
    private DataTable ReCreateColumns
    {
        get
        {
            ASPxGridView g = gvitems;
            var index = tcDemos.ActiveTabIndex;
            g.Columns.Clear();
            DataTable table = GetCurrentTable();
            DataRow[] result = table.Select(string.Format("Marks='{0}'", index));
            if (_dt == null) return null;
            foreach (DataRow data in result)
            {
                string[] args = data["Name"].ToString().Split(';');
                for (int x = 0; x < args.Length; x++)
                {
                    GridViewDataTextColumn column = new GridViewDataTextColumn();
                    column.FieldName = args[x];
                    if (args[x] == "Result")
                        column.PropertiesTextEdit.DisplayFormatString = "{0:0.00}";
                    // set additional column properties
                    if (index == 1)
                    {
                        if (args[x] == "Currency")
                            column.Caption = "FillWeight";
                        else if (args[x] == "Quantity")
                            column.Caption = "Yield";
                        else if (args[x] == "Price")
                            column.Caption = "Cost";
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
                            column.Caption = "Cost";
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
                            column.Caption = "Cost";
                        else
                            column.Caption = args[x];
                    }
                    else if (index == 4)
                    {
                        if (args[x] == "Currency")
                            column.Caption = "Unit";
                        else if (args[x] == "Price")
                            column.Caption = "Cost";
                        else
                            column.Caption = args[x];
                    }
                    else if (index == 5)
                    {
                        if (args[x] == "Currency")
                            column.Caption = "Pack size/Unit";
                        else if (args[x] == "Price")
                            column.Caption = "Cost";
                        else
                            column.Caption = args[x];
                    }
                    else if (index == 6)
                    {
                        if (args[x] == "BaseUnit")
                            column.Caption = "Currency";
                        else
                            column.Caption = args[x];
                    }
                    else if (index == 7)
                    {
                        if (args[x] == "Quantity")
                            column.Caption = "cases per fcl";
                        else if (args[x] == "Price")
                            column.Caption = "Cost";
                        else if (args[x] == "BaseUnit")
                            column.Caption = "Currency";
                        else
                            column.Caption = args[x];
                    }
                    else
                        column.Caption = args[x];
                    if (args[x] == "Calcu")
                        column.Width = Unit.Pixel(0);
                    g.Columns.Add(column);
                }
            }
            if (_dt.Rows.Count > 0)
            {
                gvitems = g;
                gvitems.KeyFieldName = _dt.Columns[0].ColumnName;
                gvitems.Columns[0].Visible = false;
            }
            return _dt;
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
            c.HeaderTemplate = new CustomDataColumnTemplate();
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
    private DataTable GetTableFromExcel(DataTable table)
    {
        var dt = new DataTable();
        dt = tcustomer.Clone();
        int NextRowID = dt.Rows.Count;
        for (int i = 0; i < table.Rows.Count; i++)
        {
            if (i > 0)
            {
                DataRow row = dt.NewRow();
                NextRowID++;
                row["ID"] = NextRowID;
                row["From"] = DateTime.Now.ToString("dd-MM-yyyy");
                row["To"] = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");
                row["Material"] = table.Rows[i]["Column2"].ToString();
                row["Description"] = GetDescrip(row["Material"].ToString());
                //List<string> list = new List<string>();
                //foreach (DataColumn column in table.Columns)
                //{
                //    if (column.ToString() == "Column3")
                //        if (!string.IsNullOrEmpty(table.Rows[i][column].ToString()))
                //            list.Add(table.Rows[i][column].ToString());
                //}
                //if (list.Count > 0)
                //    row["Upcharge"] = String.Join("|", list.ToArray());
                dt.Rows.Add(row);
            }
        }
            return dt;
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

        string fileName = e.UploadedFile.FileName;
        //string fileFullPath = Path.Combine(dirPhysicalPath, fileName);
        FilePath = Path.Combine(dirPhysicalPath, fileName);
        //FilePath = string.Format(Server.MapPath("~/XlsTables/{0}") , e.UploadedFile.FileName);
        e.UploadedFile.SaveAs(FilePath);
        if (!string.IsNullOrEmpty(FilePath))
        {
            Workbook book = new Workbook();
            book.InvalidFormatException += book_InvalidFormatException;
            book.LoadDocument(FilePath);
            Worksheet sheet = book.Worksheets.ActiveWorksheet;
            CellRange range = sheet.GetUsedRange();
            DataTable table = sheet.CreateDataTable(range, false);
            DataTableExporter exporter = sheet.CreateDataTableExporter(range, table, false);
            exporter.CellValueConversionError += exporter_CellValueConversionError;
            exporter.Export();
            //this.Session.Remove("seGetMyData");
            //tcustomer = new DataTable();
            tcustomer = GetTableFromExcel(table);
            tcustomer.PrimaryKey = new DataColumn[] { tcustomer.Columns["ID"] };
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
        }
    }

    protected void gridData_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        var args = e.Parameters.Split('|');
        long id;
        if (!long.TryParse(args[2], out id))
            return;
        var result = new Dictionary<string, string>();
        DataTable dt = new DataTable();
        if (args[1] == "New")
        {
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
        if (args[1] == "EditDraft")
        {
            SqlParameter[] param = {new SqlParameter("@RequestNo",args[2].ToString()),
                    new SqlParameter("@value","Customer".ToString())};
            dt = cs.GetRelatedResources("spGetTunaCustomer", param);
            foreach(DataRow rw in dt.Rows) {
                result["NewID"] = string.Format("{0}",rw["UniqueColumn"]);
                result["RequestNo"] = string.Format("{0}",rw["RequestNo"]);
                result["Customer"] = string.Format("{0}", rw["Customer"]);
                result["ShipTo"] = string.Format("{0}", rw["ShipTo"]);
                result["Incoterm"] = string.Format("{0}", rw["Incoterm"]);
                result["Currency"] = string.Format("{0}", rw["Currency"]);
                result["PaymentTerm"] = string.Format("{0}", rw["PaymentTerm"]);
                result["ExchangeRate"] = string.Format("{0}", rw["ExchangeRate"]);
                result["Insurance"] = string.Format("{0}", rw["Insurance"]);
                result["Interest"] = string.Format("{0}", rw["Interest"]);
                result["Size"] = string.Format("{0}", rw["Size"]);
                result["Freight"] = string.Format("{0}", rw["Freight"]);
                result["Route"] = string.Format("{0}", rw["Route"]);
                result["Remark"] = string.Format("{0}", rw["Remark"]);
                e.Result = result;
            }
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
                ws.Cells["F" + i].Value = string.Format("{0}", dt.Rows[0]["CostNo"]);
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

    protected void gvutilize_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
    {

    }

    protected void gvitems_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        e.Cancel = true;
        g.CancelEdit();
    }
    void insertsGrid(string p, string text)
    {
        DataRow rw = _dt.NewRow();
        int iMax = 600;
        foreach (DataRow oneNewrow in _dt.Select("Calcu = "+p.ToString())) {
            if(Convert.ToInt32(oneNewrow["RowID"]) != 699)
            if (iMax < Convert.ToInt32(oneNewrow["RowID"]))
                iMax = Convert.ToInt32(oneNewrow["RowID"]);
        }
        iMax++;
        switch (p.ToString())
        {
            case "6":
                //if (_UpChargedt == null){
                //    _UpChargedt = new DataTable(_dt.TableName);
                //    _UpChargedt = _dt.Clone();
                //}
                double UpChargePrice, ChargeQuantity;
                if (Double.TryParse(tbUpChargeQuantity.Text, out ChargeQuantity))
                {
                    if (Double.TryParse(tbUpChargePrice.Text, out UpChargePrice))
                        //if (tbUpChargeUnit.Text.ToLower() == "can")
                        //{
                        //    DataRow _result = _dt.Select("Component = 'Packing Style'").FirstOrDefault();
                            UpChargePrice = UpChargePrice * ChargeQuantity;
                        //}
                    _dt.Rows.Add(iMax++, hfUpchargeGroup["Upcharge"], CmbUpCharge.Text, "USD/Case", UpChargePrice.ToString("F2"), 6, tbUpChargeQuantity.Text,
                        tbUpChargePrice.Text, 0);
                }
                break;
            case "7":
                int NextRowID = Convert.ToInt32(_dt.AsEnumerable()
                    .Max(row => row["RowID"]));
                NextRowID++;
                double NumberContainer;
                if (Double.TryParse(text, out NumberContainer))
                {
                    double interest = 0, Freight = 0, Insurance = 0;
                    double sumFreight = 0;
                    object minprice = _dt.Compute("Sum(Result)", "Name in ('FOB price(18 - digits)','Value Commission')");
                    if (!string.IsNullOrEmpty(tbFreight.Text) || !string.IsNullOrEmpty(text))
                        double.TryParse(tbFreight.Text, out Freight);
                    double.TryParse(tbinterest.Text, out interest);
                    sumFreight = (Convert.ToDouble(Freight) / Convert.ToDouble(text));
                    double A = 0, B = 0, interrest = 0;
                    object _commission = _dt.Compute("Sum(Result)", "Name in ('Value Commission')"); ;

                    interrest = (Convert.ToDouble(minprice) + Convert.ToDouble(_commission) + sumFreight) * (Convert.ToDouble(interest) / 100);
                    A = Convert.ToDouble(minprice) + sumFreight + interrest;
                    if (double.TryParse(tbInsurance.Text, out Insurance))
                        B = Convert.ToDouble(string.Format("{0:#,##0.####}", A * (Insurance == 0 ? 0 : Convert.ToDouble(1.0011))));
                    //B = Convert.ToDouble(string.Format("{0:#,##0.####}", A * (_Insurance==0?0:Convert.ToDouble(0.5/100))));
                    //OfferPrice = minprice + totalCount + interrest + sumFreight + Convert.ToDouble(B);
                    DataRow _result = _dt.Select("Component = 'Packing Style'").FirstOrDefault();
                    if (_result != null) {
                        _result["Quantity"] = NumberContainer;
                        _result["Result"] = sumFreight.ToString("F2");
                            }
                    var rowsToUpdate = _dt.AsEnumerable().Where(r => r.Field<string>("Component") == "Route").FirstOrDefault();
                    if (rowsToUpdate == null) { 
                        _dt.Rows.Add(NextRowID++, "Route", CmbRoute.Text, "", sumFreight.ToString("F2"), 7, NumberContainer, Freight, 0, "USD/Case");
                    } else {
                        rowsToUpdate.SetField("Quantity", NumberContainer.ToString("F"));
                    }
                    DataRow[] _dr = _dt.Select("Name in ('Route','Insurance')");
                    foreach (DataRow _r in _dr)
                    {
                        switch (_r["Name"].ToString())
                        {
                            case "Route":
                                _r["Result"] = sumFreight.ToString("F2");
                                break;
                        }
                    }
                }
                break;
        }

    }

    protected void gvutilize_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var result = new Dictionary<string, string>();
        var args = e.Parameters.Split('|');
        if (args[0] == "build")
        {
            foreach (DataRow _rwx in exchangerate().Rows)
            {
                result["Rate"] = _rwx["Rate"].ToString();
                result["Currency"] = _rwx["Currency"].ToString();
                e.Result = result;
            }
        }
        if (args[0] == "tot" || args[0] == "sub")
        {   //update subContainers
            //if ((tsubContainers.Rows.Count > 0) && (args[0] == "tot"))
            //    foreach (DataRow row in tsubContainers.Rows)
            //        row["subContainers"] = string.Format("{0}", args[2].ToString());
            
            SqlParameter[] param = {new SqlParameter("@Route",args[3].ToString()),
                new SqlParameter("@Container",args[4].ToString())};
            DataTable dt = cs.GetRelatedResources("spGetselectFreight", param);
            //int cou = dt.Rows.Count;
            //if (cou > 0)
            //{
            foreach (DataRow dr in dt.Rows)
            {
                //double minprice = Convert.ToDouble(tbMinPrice.Text);
                double _Insurance = 1;
                if (args[5] != null)
                {
                    if (args[5].ToString().Substring(1, 1) == "I")
                        //double.TryParse(tbInsurance.Text, out Insurance);
                        _Insurance = 1.005;
                }
                result["Insurance"] = "1.005";
                result["Freight"] = string.Format("{0}", Convert.ToDouble(dr["MKTCost"]) * _Insurance);
                e.Result = result;
            }
        }
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

    protected void gvitems_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var result = new Dictionary<string, string>();
        var args = e.Parameters.Split('|');
        if (args[0] == "tot" || args[0] == "sub"){
            SqlParameter[] param = {new SqlParameter("@Route",args[3].ToString()),
                new SqlParameter("@Container",args[4].ToString())};
            DataTable dt = cs.GetRelatedResources("spGetselectFreight", param);
            //int cou = dt.Rows.Count;
            //if (cou > 0)
            //{
            foreach (DataRow dr in dt.Rows)
            {
                //double minprice = Convert.ToDouble(tbMinPrice.Text);
                double _Insurance = 1;
                if (args[5] != "")
                {
                    if (args[5].ToString().Substring(1, 1) == "I")
                        //double.TryParse(tbInsurance.Text, out Insurance);
                        _Insurance = 1.005;
                }
                result["Insurance"] = "1.005";
                result["Freight"] = string.Format("{0}", Convert.ToDouble(dr["MKTCost"]) * _Insurance);
                e.Result = result;
            }
        }
    }
    void post(string ID)
    {
        foreach (DataRow rw in tcustomer.Rows)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                string query = "spInsertTunaStdItems";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", rw["ID"].ToString());
                    cmd.Parameters.AddWithValue("@Material", rw["Material"].ToString());
                    cmd.Parameters.AddWithValue("@Utilize", rw["Utilize"].ToString());
                    cmd.Parameters.AddWithValue("@From", rw["From"]);
                    cmd.Parameters.AddWithValue("@To", rw["To"]);
                    cmd.Parameters.AddWithValue("@subContainers", rw["subContainers"]);
                    cmd.Parameters.AddWithValue("@RawMaterial", rw["RawMaterial"]);
                    cmd.Parameters.AddWithValue("@Media", rw["Media"]);
                    cmd.Parameters.AddWithValue("@Packaging", rw["Packaging"]);
                    cmd.Parameters.AddWithValue("@LOHCost", rw["LOHCost"]);
                    cmd.Parameters.AddWithValue("@PackingStyle", rw["PackingStyle"]);
                    cmd.Parameters.AddWithValue("@Upcharge", rw["Upcharge"]);
                    cmd.Parameters.AddWithValue("@RequestNo", ID.ToString());
                    cmd.Parameters.AddWithValue("@Commission", rw["Commission"]);
                    cmd.Parameters.AddWithValue("@OverPrice", rw["OverPrice"]);
                    cmd.Parameters.AddWithValue("@Pacifical", rw["Pacifical"]);
                    cmd.Parameters.AddWithValue("@MSC", rw["MSC"]);
                    cmd.Parameters.AddWithValue("@Margin", rw["Margin"]);
                    cmd.Parameters.AddWithValue("@MinPrice", rw["MinPrice"]);
                    cmd.Parameters.AddWithValue("@OfficePrice", rw["OfficePrice"]);
                    cmd.Parameters.AddWithValue("@Mark", rw["Mark"]);
                    con.Open();
                    //cmd.ExecuteNonQuery();
                    var getValue = cmd.ExecuteScalar();
                    con.Close();
                    foreach (DataRow row in _UpChargedt.Rows)
                    {
                        SqlParameter[] param = new SqlParameter[] {new SqlParameter("@ID",string.Format("{0}", row["ID"])),
                        new SqlParameter("@UpchargeGroup", string.Format("{0}", row["UpchargeGroup"])),
                        new SqlParameter("@Upcharge", string.Format("{0}", row["Upcharge"])),
                        new SqlParameter("@Price", string.Format("{0}", row["Price"])),
                        new SqlParameter("@Quantity", string.Format("{0}", row["Quantity"])),
                        new SqlParameter("@Currency", string.Format("{0}", row["Currency"])),
                        new SqlParameter("@Result", string.Format("{0}", row["Result"])),
                        new SqlParameter("@Mark", string.Format("{0}", rw["Mark"])),//Header
                        new SqlParameter("@SubID", string.Format("{0}", getValue.ToString())),
                        new SqlParameter("@RequestNo", string.Format("{0}", ID))};
                        cs.GetExecuteNonQuery("spinsetstdUpCharge", param);
                    }
                }
            }
        }
    }
    protected void gvitems_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        if (e.ButtonID != "colDel") return;
        object keyValue = g.GetRowValues(e.VisibleIndex, g.KeyFieldName);
        //DataRow found = _UpChargedt.Rows.Find(keyValue);
        //found["UpchargeGroup"] = found["UpchargeGroup"].ToString() == "D" ? "" : "D";
        //_UpChargedt.AcceptChanges();

        DataRow rw = _dt.Rows.Find(keyValue);
        rw["Component"] = rw["Component"].ToString() == "D" ? "" : "D";
        _dt.AcceptChanges();

        g.DataBind();
    }
    protected void gridData_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        if (args[0] == "post")
        {
            string value = "";
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spInsertTunaStd";
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", args[1]));
                cmd.Parameters.AddWithValue("@User", user_name.ToString());
                cmd.Parameters.AddWithValue("@Incoterm", string.Format("{0}", CmbIncoterm.Value));
                cmd.Parameters.AddWithValue("@Route", string.Format("{0}", CmbRoute.Value));
                cmd.Parameters.AddWithValue("@Size", string.Format("{0}", CmbSize.Value));
                cmd.Parameters.AddWithValue("@Quantity", tbNumberContainer.Text);
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
                cmd.Parameters.AddWithValue("@RequestNo", "");
                cmd.Parameters.AddWithValue("@NewID", hfgetvalue["NewID"]);
                cmd.Connection = con;
                con.Open();
                var getValue = cmd.ExecuteScalar();
                con.Close();
                value = getValue.ToString();
                post(value);
                //gridData.JSProperties["cpKeyValue"] = (getValue == null) ? string.Empty : getValue.ToString();
            }
        }
        if (args[0] == "Delete")
        {
            Delete(args[1]);
        }
        Update();
    }
    public override void Update()
    {
        //gridData.DataSource = dsgv;
        gridData.DataBind();
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
        foreach (var args in e.UpdateValues)
        {
         
        }
        e.Handled = true;
    }

    protected void gvitems_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        var index = tcDemos.ActiveTabIndex;
        if (index == 6 || index == 7)
        {
            GridViewCommandColumn col = new GridViewCommandColumn();
            col.Name = "CommandColumn";
            col.ShowEditButton = true;
            col.ShowNewButtonInHeader = true;
            //col.VisibleIndex = 0;
            col.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            col.HeaderTemplate = new CustomDataColumnTemplate();
            col.ButtonRenderMode = GridCommandButtonRenderMode.Image;
            GridViewCommandColumnCustomButton but = new GridViewCommandColumnCustomButton();
            but.ID = "colDel";
            but.Image.ToolTip = "Remove";
            but.Image.Url = "~/Content/Images/Cancel.gif";
            col.Width = Unit.Pixel(45);
            col.CustomButtons.Add(but);
            g.Columns.Add(col);
        }
        g.FilterExpression = "[Calcu] = '" + ActivePageSymbol + "'";
        if (_dt != null)
        {
            //g.AutoFilterByColumn(g.Columns["Calcu"], string.Format("{0}", ActivePageSymbol));
            //((GridViewDataColumn)g.Columns[1]).SortAscending();
            if (_dt != null)
                if (_dt.Rows.Count > 0)
                {
                    //for (int i = 1; i < 10; i++) { 
                    //    g.Columns[i].VisibleIndex = i;
                    //    g.Columns[i].Visible=true;
                    //}

                    //if (index == 1)
                    //{
                    //    g.Columns[3].Caption = "FillWeight";
                    //    g.Columns[4].Caption = "Yield";
                    //    g.Columns[5].Caption = "Cost";
                    //    g.Columns[8].Caption = "Currency";
                    //    g.Columns[9].Visible = false;
                    //    //g.Columns[8].VisibleIndex = 3;
                    //}
                    //if (index == 2)
                    //{
                    //    g.Columns[3].Caption = "Loss";
                    //    g.Columns[4].Caption = "FillWeight";
                    //    g.Columns[5].Caption = "Cost";
                    //    g.Columns[8].Caption = "Unit";
                    //    g.Columns[8].VisibleIndex = 6;
                    //}
                    //if (index == 3)
                    //{
                    //    g.Columns[4].Caption = "Loss";
                    //    g.Columns[3].Caption = "Unit";
                    //    g.Columns[3].VisibleIndex = 5;
                    //    g.Columns[8].Visible = false;
                    //}
                    //if (index == 4)
                    //{
                    //    g.Columns[4].Caption = "FillWeight";
                    //    g.Columns[3].Caption = "Unit";
                    //    g.Columns[3].VisibleIndex = 5;
                    //    g.Columns[8].Visible = false;
                    //    //g.Columns[9].Width = Unit.Pixel(0);
                    //}
                    //if (index == 5)
                    //{
                    //    g.Columns[3].Caption = "Pack size";
                    //    g.Columns[4].Caption = "Packing LOH";
                    //    g.Columns[5].Caption = "Cost";
                    //    //g.Columns[5].Width = Unit.Percentage(20);
                    //    g.Columns[8].Caption = "Unit";
                    //    //g.Columns[8].VisibleIndex = 8;
                    //    g.Columns[6].VisibleIndex = 6;
                    //    //g.Columns[3].Width = Unit.Pixel(0);
                    //    g.Columns[4].Visible = false;
                    //    g.Columns[9].Visible = false;
                    //}
                    //if (index == 6)
                    //{
                    //    //((GridViewDataColumn)g.Columns[1]).SortAscending();
                    //    g.Columns[3].Visible = false;
                    //    g.Columns[4].Visible = false;
                    //    g.Columns[5].Visible = false;
                    //    g.Columns[8].Visible = false;
                    //    g.Columns[9].Caption = "Currency";

                    //}
                    //if (index == 7)
                    //{
                    //    g.Columns[3].Width = Unit.Pixel(0);
                    //    g.Columns[4].Caption = "cases per fcl";
                    //    g.Columns[5].Caption = "Cost";
                    //    g.Columns[8].Visible = false;
                    //    g.Columns[9].Caption = "Currency";
                    //}
                    if (index == 8)
                    {
                    //g.Columns[3].VisibleIndex = 6;
                    //g.Columns[8].Visible = false;
                    //g.Columns[9].Visible = false;
                        var values = new[] { "Secondary Packaging","FOB Price","CIF Price", "MSC","Interest","Interest","Offer Price","Margin","FOB price(18 - digits)",
                        "Value Commission", "OverPrice", "Pacifical"};
                        for (int i = 0; i <= values.Length - 1; i++)
                        {
                            object sumObject;
                            DataRow _xdr = _dt.Select(string.Format("Name='{0}'", values[i])).FirstOrDefault();
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
                                        sumObject = _dt.Compute(@"Sum(Result)", "Name in ('Secondary Packaging','Media'," +
                                            "'Primary Packaging','Packing Style','Raw material') or Component in ('LOH')");
                                        if (sumObject != System.DBNull.Value)
                                        {
                                            var rowsToUpdate = _dt.AsEnumerable().Where(r => r.Field<string>("Name") == "Margin").FirstOrDefault();
                                            decimal PerMargin = 0, _sumprice = Convert.ToDecimal(sumObject);
                                            decimal.TryParse(rowsToUpdate["Quantity"].ToString(), out PerMargin);
                                            decimal _totalmargin = (_sumprice / ((100 - PerMargin) / 100));
                                            decimal _Margin = _totalmargin * (PerMargin / 100);
                                            //decimal _totalmargin = _sumprice * (Convert.ToDecimal(PerMargin) / 100);
                                            rowsToUpdate["Result"] = _Margin.ToString("F2");
                                            _xdr["Result"] = Convert.ToDecimal(_sumprice + _Margin).ToString("F2");
                                        }
                                        break;
                                    case "FOB Price":
                                        object totalsumObject = _dt.Compute("Sum(Result)", "Name in ('Value Commission','FOB price(18 - digits)','OverPrice')");
                                        _xdr["Result"] = Convert.ToDecimal(totalsumObject).ToString("F2");
                                        break;
                                    case "Interest":
                                        decimal _Interest = 0;
                                        if (decimal.TryParse(tbinterest.Text, out _Interest))
                                        {
                                            object _sumObject = _dt.Compute("Sum(Result)", "Name in ('FOB price(18 - digits)')");
                                            decimal _totalprice = Convert.ToDecimal(_sumObject) / (1 - (_Interest / 100)) - Convert.ToDecimal(_sumObject);
                                            _xdr["Result"] = Convert.ToDouble(_totalprice).ToString("F2");
                                        }
                                        break;
                                    case "CIF Price":
                                        object _cifsumObject = _dt.Compute("Sum(Result)", "Name in ('FOB Price','Route','Insurance','Interest')");
                                        _xdr["Result"] = Convert.ToDecimal(_cifsumObject).ToString("F2");
                                        break;
                                    case "Offer Price":

                                        object _OffersumObject = _dt.Compute("Sum(Result)", "Name in ('CIF Price','MSC','Pacifical')");
                                        //"Insurance"
                                        double _sumfobprice = Convert.ToDouble(_OffersumObject);
                                        _xdr["Result"] = Convert.ToDecimal(_sumfobprice).ToString("F2");
                                        if (CmbIncoterm.Value!=null)
                                            if (CmbIncoterm.Value.ToString() != "FOB") { 
                                        var _rwInsurance = _dt.AsEnumerable().Where(r => r.Field<string>("Name") == "Insurance").FirstOrDefault();
                                        //object sumFreight = _dt.Compute("Sum(Result)", "Name in ('Route')");
                                        //object _sumfobprice = _dt.Compute("Sum(Result)", "Name in ('FOB price(18 - digits)')");
                                        //double _Insurance = ((Convert.ToDouble(_sumfobprice) * 1.1) + Convert.ToDouble(sumFreight)) * (1.005 / 100);
                                        double _Insurance = (_sumfobprice * 0.0011);
                                        _rwInsurance["Result"] = Convert.ToDouble(_Insurance).ToString("F2");
                                        _xdr["Result"] = String.Format("{0:0.00}",Convert.ToDecimal(_sumfobprice + _Insurance));
                                            }
                                            else{
                                                var rxInsu = _dt.AsEnumerable().Where(r => r.Field<string>("Name") == "Insurance").FirstOrDefault();
                                                rxInsu["Result"] = "0";
                                            }
                                        break;
                                    case "Value Commission":

                                        DataRow drw = _dt.Select("Name='FOB price(18 - digits)'").FirstOrDefault();
                                        if (drw != null)
                                        {
                                            decimal _commition = Convert.ToDecimal(_xdr["Quantity"]) / 100;
                                            if (_commition > 0)
                                            {
                                                decimal _valuecomm = Convert.ToDecimal(drw["Result"]) / (1 - _commition) - Convert.ToDecimal(drw["Result"]);
                                                _xdr["Result"] = _valuecomm.ToString("F2");
                                            }
                                            else
                                                _xdr["Result"] = "0";
                                        }
                                        break;

                                    case "OverPrice":
                                        sumObject = _dt.Compute("Sum(Result)", "Name in ('Value Commission','FOB price(18 - digits)')");
                                        decimal _OverPrice = Convert.ToDecimal(_xdr["Price"]);
                                        if (_OverPrice > 0)
                                        {
                                            _xdr["Result"] = _OverPrice.ToString("F2");

                                        }
                                        else
                                            _xdr["Result"] = "0";
                                        break;
                                    case "Pacifical":
                                    case "MSC":
                                        sumObject = _dt.Compute("Sum(Result)", "Name in ('CIF Price')");
                                        decimal Quantity = 0;
                                        decimal.TryParse(_xdr["Quantity"].ToString(), out Quantity);
                                        decimal _sumPrice = Quantity / 100;
                                        if (_sumPrice > 0)
                                        {
                                            decimal _totalprice = Convert.ToDecimal(sumObject) / (1 - _sumPrice) - Convert.ToDecimal(sumObject);
                                            _xdr["Result"] = _totalprice.ToString("F2");

                                        }
                                        else
                                            _xdr["Result"] = "0";
                                        break;
                                }
                            }
                        }
                    }
                }
        }
    }
    void GetUpdateItems()
    {
        if (_dt == null) return;
        var values = new[] { "Value Commission", "OverPrice", "Pacifical", "MSC", "Margin", "Interest" };
        for (int i = 0; i <= values.Length - 1; i++)
            {
                //object sumObject;
                DataRow _dr = _dt.Select(string.Format("Name='{0}'", values[i])).FirstOrDefault();
                //if (sumObject != System.DBNull.Value)

                if (_dr != null)
                {

            }
            //Value Commission
            //insertsGrid(args[1]);
        }
    }
    protected void gvitems_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        if (_dt != null)
        {
            DataRow dr = _dt.Rows.Find(e.Keys["RowID"]);
            foreach (DataColumn column in _dt.Columns)
            {
                if (column.ColumnName.ToString() != "RowID")
                {
                    dr[column.ColumnName] = e.NewValues[column.ColumnName];
                }
            }
            //dr["Data"] = e.NewValues["Data"];
        }
        g.CancelEdit();
        e.Cancel = true;
    }
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
        public void InstantiateIn(Control container)
        {
            GridViewHeaderTemplateContainer c = (GridViewHeaderTemplateContainer)container;
            ASPxButton btn = new ASPxButton();
            btn.RenderMode = ButtonRenderMode.Link;
            btn.AutoPostBack = false;
            btn.ID = "button" + c.ItemIndex;
          
            c.Controls.Add(btn);
            //btn.Text = "Button";
            btn.Image.Url= "~/Content/Images/icons8-plus-math-filled-16.png";
            //btn.Click += ASPxButton1_Click;
            btn.ClientSideEvents.Click = "function(s,e){ insertSelection(6,'Insert',s); }";
        }
        protected void ASPxButton1_Click(object sender, EventArgs e)
        {

        }
    }

    protected void gv_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
    {
        if (e.DataColumn.FieldName == "Mark" )
        {
            var g = sender as ASPxGridView;
            DataRow row = g.GetDataRow(e.VisibleIndex);
            int index = e.VisibleIndex;
            //e.Cell.ForeColor = Color.Black;
            if (g.VisibleRowCount != 0 && row != null)
            {
                if (g.GetRowValues(index, "Mark").ToString() == "D")
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
        if (tcustomer != null)
        {
            DataRow dr = tcustomer.Rows.Find(e.KeyValue);
            if (dr != null)
            {
                if (dr["Mark"].ToString() == "D")
                    e.Row.BackColor = Color.Orange;
                else
                    e.Row.BorderColor = Color.Black;
            }
        }
    }

    protected void gv_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
    {
        ASPxGridView g = (ASPxGridView)sender;
        DataRow rw = tcustomer.Rows.Find(e.Keys[0]);
        rw["Mark"] = rw["Mark"].ToString() == "D" ? "" : "D";
        tcustomer.AcceptChanges();
        e.Cancel = true;
    }
}
public class GridDataItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Result { get; set; }
    public string StatusApp { get; set; }
}
