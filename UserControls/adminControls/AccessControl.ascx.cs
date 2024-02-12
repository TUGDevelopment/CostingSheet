using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_adminControls_AccessControl : MasterUserControl
{

    string user_name = "fo5910155";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            FileManager.SettingsPermissions.Role = "Administrator";
    }
    public override void Update()
    { }
    protected void ArtsDataSource_Inserting(object sender, SqlDataSourceCommandEventArgs e)
    {
        e.Command.Parameters["@GCRecord"].Value = "3C985E3B-F003-4596-B1BC-0CAB4050324F";
        e.Command.Parameters["@LastUpdateBy"].Value = user_name.ToString();
    }

    protected void ArtsDataSource_Updating(object sender, SqlDataSourceCommandEventArgs e)
    {
        e.Command.Parameters["@LastUpdateBy"].Value = user_name.ToString();
    }
}