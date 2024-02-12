using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_OfferPriceForm : System.Web.UI.UserControl
{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
            SetInitialRow();
    }
    public void SetInitialRow()
    {
        hfid["hidden_value"] = string.Empty;
        //hfPayment["value"] = string.Empty;
        hftype["type"] = string.Format("{0}", 0);
        //user_name.Replace("fo5910155", "MO581192");
        username["user_name"] = user_name;

    }

    protected void CmbCompany_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
    {

    }

    protected void CmbCustomer_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
    {

    }

    protected void CmbShipTo_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
    {

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
            SqlParameter[] param = {new SqlParameter("@param",id.ToString()),
                        //new SqlParameter("@Id",dr["CostingNo"].ToString())
                };
            DataTable dt = cs.GetRelatedResources("spSelQuotaHeader", param);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                //result["UniqueColumn"] = dr["UniqueColumn"].ToString();
                result["StatusApp"] = dr["StatusApp"].ToString();
                result["ID"] = dr["ID"].ToString();
                e.Result = result;
            }
        }
    }
}