using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using DevExpress.Web;

public partial class Site : System.Web.UI.MasterPage
{
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    List<MenuInfo> menuItems = new List<MenuInfo>();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) {
            //this.Session["username"] = user_name;
            //test(navBar, nwd);
            }
    }

    private void BuildNavBarItems(ASPxNavBar navbar, List<MenuInfo> menuitems, string menutitle)
    {
        // Set the Title
        //this.ASPxRoundPanel2.HeaderText = menutitle;
        string navUrl = string.Empty;
        // I use a Dictionary object to keep track of the Menu Groups
        Dictionary<string, NavBarGroup> navGroups =
            new Dictionary<string, NavBarGroup>();
        // Clear the Items to start fresh.
        navbar.Groups.Clear();
        foreach (MenuInfo m in menuitems)
        {
            navUrl = m.MenuUrl;

            // If the Key Exists in the Dictionary add the new Item to the items collection of that NavGroup
            if (navGroups.ContainsKey(m.MenuGroup))
                navGroups[m.MenuGroup].Items.Add(new NavBarItem(m.MenuText, "", "", navUrl));
            else
            {
                // Otherwise we add a new navGroup to the Navbar collection, add the navbarItem to it and store it in the dictionary
                NavBarGroup navgroup = navbar.Groups.Add(m.MenuGroup, m.MenuGroup);
                navgroup.Items.Add(new NavBarItem(m.MenuText, "", "", navUrl));
                navGroups.Add(m.MenuGroup, navgroup);
            }
        }
    }
    //protected void BuildNavBar(ASPxNavBar navbar, SqlDataSource dataSource)
    //{
    //    // Get DataView
    //    DataSourceSelectArguments arg = new DataSourceSelectArguments();
    //    DataView dataView = dataSource.Select(arg) as DataView;
    //    dataView.Sort = "level";
    //    for (int i = 0; i < dataView.Count; i++)
    //    {
    //        DataRow row = dataView[i].Row;
    //        string parentID = row["level"].ToString();
    //        NavBarItem item = CreateItem(row);
    //        NavBarGroup g = navbar.Groups.FindByName(parentID);
    //        if (g == null)
    //        {
    //            g = new NavBarGroup() { Name = row["level"].ToString(), Text = "Category" + row["level"].ToString() };
    //            navbar.Groups.Add(g);
    //        }
    //        g.Items.Add(item);
    //    }
    //}
    private void test(ASPxNavBar navbar, SqlDataSource dataSource)
    {
        DataSourceSelectArguments arg = new DataSourceSelectArguments();
        DataView dataView = dataSource.Select(arg) as DataView;
        dataView.Sort = "level";
        DataTable table = dataView.Table;
        DataRow[] mytable = table.Select("level='1'");
        int i = 1;
        foreach (DataRow row in mytable)
        {
            DataRow[] arr = table.Select("ParentID='" + row["ID"].ToString() + "'");
            foreach (DataRow r in arr) {
                string NavigateUrl = "";
                switch (row["Text"].ToString())
                {
                    case "Form":case "Report":
                        NavigateUrl = string.Format("~/Default.aspx?viewMode={0}", r["URL"]);
                        break;
                    case "Author": case "Costing":case "Price":
                        NavigateUrl = string.Format(@"~/admin.aspx?viewMode=adminform&role={0}", r["URL"]); 
                        break;
                    default:
                        NavigateUrl = string.Format(@"~/setup.aspx?viewMode=EditForm&role={0}", r["Text"]);
                        break;//setup.aspx?viewMode=EditForm
                }
                if (r["Text"].ToString() == "Upload Costing")
                    NavigateUrl = "~/Upload.aspx?viewMode=UploadControl";
                menuItems.Add(new MenuInfo() { MenuId = i++, MenuName = r["Text"].ToString(), MenuText = r["Text"].ToString()
                    , MenuUrl = NavigateUrl, MenuGroup = row["Text"].ToString()
                });
            }
        }
        //BuildNavBarItems(this.navBar, menuItems, "Main Menu Title");
    }
    //private NavBarItem CreateItem(DataRow row)
    //{
    //    NavBarItem ret = new NavBarItem();
    //    ret.Name = row["Text"].ToString();
    //    ret.Text = row["Text"].ToString();
    //    //ret.NavigateUrl = row["Url"].ToString();
    //    return ret;
    //}
    private class MenuInfo
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuText { get; set; }
        public string MenuUrl { get; set; }
        public string MenuGroup { get; set; }
    }
}
