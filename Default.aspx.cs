using DevExpress.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

public partial class _Default : MasterDetailPage{
    MasterUserControl masterUC;
    public override MasterUserControl MasterUC { get { return masterUC; } }
    protected BasePage BasePage { get { return Page as BasePage; } }
    public override string PageName { get { return "TechnicalReport"; } }
    //string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    string url = "~/Default.aspx?viewMode=CostingEditForm";
    MyDataModule cs = new MyDataModule();
    
    ServiceCS myclass = new ServiceCS();
    protected void Page_Load(object sender, EventArgs e)
    {
        //var CurUserName = HttpContext.Current.User.Identity;
        //string uName = "";
        //uName =  MyDataModule.uEmail(HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @""));
        //string a = MyToDataTable.Increment("X");
        //string uName = MyDataModule.GetUserEmail(User.Identity.Name);
        if (!IsPostBack){
            //myclass.GetReadExcelupdateJob("dd");
            //if (Request.QueryString["viewMode"] == null)
            //    Response.Redirect(Page.ResolveClientUrl(url));
            //Session["viewMode"] = "adminform";//Request.QueryString["viewMode"] ;// cs.user();
            //SqlParameter[] param = {new SqlParameter("@username", user_name),
            //new SqlParameter("@viewMode", this.Session["viewMode"])};
            //var table = cs.GetRelatedResources("spGetMenuAuth2", param);
            //CreateTreeViewNodesRecursive(table, treeView.Nodes, "");
            //treeView.ExpandToDepth(0);
            //LoadUserControls();
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AddHeader("Expires", "0");
        }
        //DefineImages(treeView.Nodes);
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        LoadUserControls();
    }
    protected void Page_PreInit(object sender, EventArgs e)
    {
        Utils.ApplyTheme(this);
    }
    protected void LoadUserControls()
    {
        try
        {
            var viewMode = HttpContext.Current.Request.QueryString["viewMode"];
            if (viewMode == null)
                Response.Redirect(Page.ResolveClientUrl(url));
            if (string.IsNullOrEmpty(viewMode))
            //viewMode = viewMode == null ? "CostingEditForm" : "CustomerEditForm";
            //viewMode = PageName;
            MasterContainer.Controls.Clear();
            //var pageNameScript = string.Format("<script type='text/javascript'>DevAVPageName = '{0}';</script>", viewMode);
            //Page.Header.Controls.AddAt(0, new LiteralControl(pageNameScript));
            //this.masterUC = LoadControl(string.Format("~/UserControls/{0}.ascx", viewMode)) as MasterUserControl;
            //MasterContainer.Controls.Add(MasterUC);
            Control myControl = LoadControl(string.Format("~/UserControls/{0}.ascx", viewMode));
            myControl.ID = "ucx_mapa";
            cp.Controls.Clear();
            cp.Controls.Add(myControl);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    private void CreateTreeViewNodesRecursive(DataTable table, TreeViewNodeCollection nodesCollection, string parentID)
    {
        for (int i = 0; i < table.Rows.Count; i++)
        {
            if (table.Rows[i]["ParentID"].ToString() == parentID)
            {
                TreeViewNode node = new TreeViewNode(table.Rows[i]["Text"].ToString(), table.Rows[i]["ID"].ToString()
                   // , @"~/Content/Images/" + table.Rows[i]["ImageUrl"].ToString()
                    ,"", string.Format(@"~/Default.aspx?viewMode={0}",table.Rows[i]["Url"].ToString())
                    );
                nodesCollection.Add(node);
                CreateTreeViewNodesRecursive(table, node.Nodes, node.Name);
            }
        }
    }
    protected void cp_Callback(object sender, CallbackEventArgsBase e)
    {
        LoadUserControls();
    }
 
    protected void treeView_NodeDataBound(object sender, TreeViewNodeEventArgs e)
    {
        TreeViewNode node = e.Node as TreeViewNode;
        XmlNode dataNode = ((e.Node.DataItem as IHierarchyData).Item as XmlNode);
        if (dataNode.Name == "class")
            node.TextTemplate = new TextColorItemTemplate();
        else
            node.TextTemplate = new TextItemTemplate();
    }
}

