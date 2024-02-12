using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using DevExpress.Web.Internal;
using DevExpress.Web;

/// <summary>
/// Summary description for Utils
/// </summary>
public static class Utils
{
    public const string ThomasEmail = "thomas.hardy@example.com";
    static bool? _isSiteMode;
    static HttpContext Context { get { return HttpContext.Current; } }
    static List<NavigationItem> _navigationItems;
    static Thread backgroundThread;
    static MyDataModule myData = new MyDataModule();
    public static bool IsIE7 { get { return RenderUtils.Browser.IsIE && RenderUtils.Browser.Version < 8; } }
    public static bool IsSiteMode
    {
        get
        {
            if (!_isSiteMode.HasValue)
                _isSiteMode = ConfigurationManager.AppSettings["SiteMode"].Equals("true", StringComparison.InvariantCultureIgnoreCase);
            return _isSiteMode.Value;
        }
    }
    public static string GetReadOnlyMessageText
    {
        get
        {
            return "test";
        }
    }
    public static bool IsDarkTheme
    {
        get
        {
            var theme = CurrentTheme;
            return theme == "Office2010Black" || theme == "PlasticBlue" || theme == "RedWine" || theme == "BlackGlass";
        }
    }

    public static string CurrentPageName
    {
        get
        {
            var key = "CE1167E3-A068-4E7C-8BFD-4A7D308BEF43";
            if (Context.Items[key] == null)
                Context.Items[key] = GetCurrentPageName();
            return Context.Items[key].ToString();
        }
    }
        public static string FormatSize(object value) {
            double amount = Convert.ToDouble(value);
            string unit = "KB";
            if(amount != 0) {
                if(amount <= 1024)
                    amount = 1;
                else
                    amount /= 1024;

                if(amount > 1024) {
                    amount /= 1024;
                    unit = "MB";
                }
                if(amount > 1024) {
                    amount /= 1024;
                    unit = "GB";
                }
            }
            return String.Format("{0:#,0} {1}", Math.Round(amount, MidpointRounding.AwayFromZero), unit);
        }
    static string GetCurrentPageName(){
        //var result = Context.Request.QueryString["viewMode"] != null ? Context.Request.QueryString["viewMode"] : "";
        var fileName = Path.GetFileName(Context.Request.Path);
        var result = fileName.Substring(0, fileName.Length - 5);
        if (result.ToLower() == "default")
            result = "form";
        else if (result.ToLower() == "setup")
            result = "setup";
        //else if (result.ToLower() == "adminform")
        //    result = "admin";
        //else if (result.ToLower() == "upload")
        //    result = "upload";
        else if (result.ToLower().Contains("print"))
            result = "print";
        return result.ToLower();
    }
    // Search
    public static string HighlightSearchText(string source, string searchText)
    {
        //if (string.IsNullOrWhiteSpace(searchText))
        //    return source;
        //var regex = GetSearchExpression(searchText);
        //if (regex.IsMatch(source))
        //    return string.Format("<span>{0}</span>", regex.Replace(source, "<span class='hgl'>$0</span>"));
        return source;
    }
    public static List<NavigationItem> NavigationItems
    {
        get
        {
            if (_navigationItems == null)
            {
                _navigationItems = new List<NavigationItem>();
                PopuplateNavigationItems(_navigationItems);
            }
            return _navigationItems;
        }
    }
    static void PopuplateNavigationItems(List<NavigationItem> list)
    {
        var path = Utils.Context.Server.MapPath("~/App_Data/Navigation.xml");
        list.AddRange(XDocument.Load(path).Descendants("Item").Select(n => new NavigationItem()
        {
            Text = n.Attribute("Text").Value,
            NavigationUrl = n.Attribute("NavigateUrl").Value,
            SpriteClassName = n.Attribute("SpriteClassName").Value
        }));
    }
    public static void ApplyTheme(Page page)
    {
        var themeName = CurrentTheme;
        if (string.IsNullOrEmpty(themeName))
            themeName = "DevEx";
        page.Theme = themeName;
    }
    public static string GetSearchText(Page page)
    {
        var key = "D672659E-FF11-40FF-A63B-FAFB0BFE760B";
        if (Context.Items[key] == null)
        {
            string value = null;
            if (!TryGetClientStateValue<string>(page, "SearchText", out value))
                value = string.Empty;
            Context.Items[key] = value;
        }
        return Context.Items[key].ToString();
    }
    public static bool TryGetClientStateValue<T>(Page page, string key, out T result)
    {
        var hiddenField = page.Master.Master.FindControl("HiddenField") as ASPxHiddenField;
        if (hiddenField == null || !hiddenField.Contains(key))
        {
            result = default(T);
            return false;
        }
        result = (T)hiddenField[key];
        return true;
    }
    public static string CurrentTheme
    {
        get
        {
            var themeCookie = Context.Request.Cookies["DemoCurrentTheme"];
            return themeCookie == null ? "DevEx" : HttpUtility.UrlDecode(themeCookie.Value);
        }
    }
    public static string FindFile(this string fileName)
    {
        return GetFileSearchPaths(fileName).FirstOrDefault(x => File.Exists(x));
    }
    public static IEnumerable<string> GetFileSearchPaths(string fileName)
    {
        yield return fileName;
        yield return Path.Combine(
            Directory.GetParent(Path.GetDirectoryName(fileName)).FullName,
            Path.GetFileName(fileName)
        );
    }
    // or as an extension
    public static bool FileExists(string fileName)
    {
        return GetFileSearchPaths(fileName).Any(File.Exists);
    }
}

public class NavigationItem
{
    public string Text { get; set; }
    public string NavigationUrl { get; set; }
    public string SpriteClassName { get; set; }
}

//public partial class Supplier
//{
//    public Supplier()
//    {
//        this.Products = new List<Product>();
//    }
//    public int SupplierID { get; set; }
//    public string CompanyName { get; set; }
//    public string ContactName { get; set; }
//    public string ContactTitle { get; set; }
//    public string Address { get; set; }
//    public string City { get; set; }
//    public string Region { get; set; }
//    public string PostalCode { get; set; }
//    public string Country { get; set; }
//    public string Phone { get; set; }
//    public string Fax { get; set; }
//    public string HomePage { get; set; }
//    public virtual ICollection<Product> Products { get; set; }
//}
//public partial class Category
//{
//    public Category()
//    {
//        this.Products = new List<Product>();
//    }

//    public int CategoryID { get; set; }
//    public string CategoryName { get; set; }
//    public string Description { get; set; }
//    public byte[] Picture { get; set; }
//    public virtual ICollection<Product> Products { get; set; }
//}
public partial class Product
{
    public Product()
    {
    }
    public string StatusApp { get; set; }
}
