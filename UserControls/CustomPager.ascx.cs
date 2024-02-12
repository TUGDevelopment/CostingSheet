using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_CustomPager : System.Web.UI.UserControl
{
    protected void cbPage_Init(object sender, EventArgs e)
    {
        ASPxComboBox cb = sender as ASPxComboBox;
        ASPxGridView grid = GetParentGrid(cb);
        for (int i = 1; i <= grid.PageCount; i++)
            cb.Items.Add(i.ToString(), i);
    }
    protected void cbRecords_Init(object sender, EventArgs e)
    {
        Int32[] values = { 10, 25, 50, 100 };
        ASPxComboBox cb = sender as ASPxComboBox;
        for (int i = 0; i < values.Length; i++)
        {
            cb.Items.Add(values[i].ToString(), values[i]);
        }
    }
    protected ASPxGridView GetParentGrid(ASPxComboBox cb)
    {
        GridViewPagerBarTemplateContainer container = (GridViewPagerBarTemplateContainer)cb.NamingContainer.NamingContainer;
        return container.Grid;
    }
    public void InstantiateIn(Control container)
    {
    }
}