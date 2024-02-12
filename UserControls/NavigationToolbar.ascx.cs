using System;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Web;
using System.Data.SqlClient;

public partial class UserControls_NavigationToolbar : System.Web.UI.UserControl {
    protected void NavigationMenu_Init(object sender, EventArgs e)
    {
        //try
        //{
        var menu = (ASPxMenu)sender;
        if (Utils.CurrentPageName.ToLower() == null)
            return;
        //var rootItem =  Utils.NavigationItems.First(i => i.Text.ToLower() == Utils.CurrentPageName.ToLower());
        var rootMenuItem = new MenuItem(); 
        menu.Items.Add(rootMenuItem);
        //rootMenuItem.Text = rootItem.Text;
        //rootMenuItem.Image.SpriteProperties.CssClass = rootItem.SpriteClassName;
        rootMenuItem.PopOutImage.SpriteProperties.CssClass = "Sprite_Arrow";

        //foreach (var item in Utils.NavigationItems)
        //{
        //    var menuItem = new MenuItem();
        //    rootMenuItem.Items.Add(menuItem);
        //    menuItem.Text = item.Text;
        //    menuItem.NavigateUrl = item.NavigationUrl;
        //    menuItem.Selected = item == rootItem;
        //}
        //menu.ShowPopOutImages = DefaultBoolean.True;
        //}
        //catch (Exception)
        //{
        //    // Print the error message, cleanup, whatever
        //}
    }
}
