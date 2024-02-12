using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Web;
using DevExpress.Web;
using System.Data;

// TODO rename file
public abstract class BasePage : System.Web.UI.Page
{
    public abstract string PageName { get; }
    // public abstract FilterBag FilterBag { get; }

    //public abstract IEnumerable<FilterControlColumn> GetFilterColumns();
    //public abstract void OnFilterChanged();

    //public virtual bool ContentHasScroll { get { return false; } }
    //public virtual bool ContentHasBorder { get { return true; } }
    //public virtual bool HasSearchPanel { get { return true; } }

    //public virtual void SaveEditFormChanges(string args) { }
    //public virtual void DeleteEntry(string args) { }
    //public virtual void PrepareToolbarItem(DevExpress.Web.MenuItem menuItem) { }

    //public bool CanHighlightField(string fieldName)
    //{
    //    return FilterBag.SearchFieldNames.Contains(fieldName);
    //}
    //public string HighlightSearchText(string text)
    //{
    //    return !string.IsNullOrWhiteSpace(text) ? DemoUtils.HighlightSearchText(text, FilterBag.SearchText) : text;
    //}
}

public abstract class MasterDetailPage : BasePage
{
    public abstract MasterUserControl MasterUC { get; }
    //public abstract DetailUserControl DetailUC { get; }

    public long SelectedItemID { get { return MasterUC.SelectedItemID; } }
    public void Update()
    {
        MasterUC.Update(); // should force update detail
    }
    //public virtual void UpdateDetail()
    //{
    //    if (DetailUC != null)
    //        DetailUC.Update();
    //}
}
public abstract class MasterUserControl : ViewUserControl
{
    //public new MasterDetailPage OwnerPage { get { return Page as MasterDetailPage; } }
    //protected virtual void UpdateDetail()
    //{
    //    OwnerPage.UpdateDetail();
    //}
}
//public abstract class DetailUserControl : ViewUserControl
//{
//    public new MasterDetailPage OwnerPage { get { return Page as MasterDetailPage; } }
//    public override long SelectedItemID { get { return OwnerPage.SelectedItemID; } set { } }
//}
public class TextItemTemplate : ITemplate
{
    public void InstantiateIn(Control container)
    {
        TreeViewNodeTemplateContainer nodeContainer = container as TreeViewNodeTemplateContainer;
        ASPxLabel lb = new ASPxLabel();
        lb.ID = lb.ClientID;
        lb.Text = nodeContainer.Node.Text;

        container.Controls.Add(lb);
    }
}

public class TextColorItemTemplate : ITemplate
{
    public void InstantiateIn(Control container)
    {
        TreeViewNodeTemplateContainer nodeContainer = container as TreeViewNodeTemplateContainer;
        ASPxLabel lb = new ASPxLabel();
        lb.ID = lb.ClientID;

        lb.Text = nodeContainer.Node.Text;
        lb.ForeColor = System.Drawing.Color.Blue;

        container.Controls.Add(lb);
    }
}
public abstract class ViewUserControl : UserControl
{
    public abstract void Update();

    public BasePage OwnerPage { get { return Page as BasePage; } }
    public virtual long SelectedItemID { get; set; }
    //protected FilterBag Filter { get { return OwnerPage.FilterBag; } }
}
//public abstract class EditFormUserControl : UserControl
//{
//    public abstract long SaveChanges(string args);
//}

//public abstract class ChartUserControl : UserControl
//{
//    public abstract WebChartControl Chart { get; }

//    public virtual long SelectedItemID { get; set; }
//    protected void ChangeChartWidth(int width)
//    {
//        Chart.Width = Unit.Pixel(width < 500 ? width : 500);
//        Chart.Series[0].LabelsVisibility = width < 375 ? DefaultBoolean.False : DefaultBoolean.True;
//    }
//    protected void OnCustomDrawSeriesPoint(CustomDrawSeriesPointEventArgs e)
//    {
//        e.LegendText = e.SeriesPoint.Argument.ToString();
//        var legendImage = new Bitmap(11, 11);
//        using (var graphics = Graphics.FromImage(legendImage))
//        {
//            graphics.FillRectangle(new LinearGradientBrush(new Rectangle(new Point(), legendImage.Size),
//                e.LegendDrawOptions.Color, e.LegendDrawOptions.ActualColor2, LinearGradientMode.BackwardDiagonal),
//                new Rectangle(new Point(), legendImage.Size));
//        }
//        e.LegendMarkerImage = legendImage;
//    }
//}
