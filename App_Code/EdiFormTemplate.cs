using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using DevExpress.Web;
/// <summary>
/// Summary description for EdiFormTemplate
/// </summary>
public class EdiFormTemplate : ITemplate
{
    private ASPxGridView _gridView;

    public ASPxGridView Grid
    {
        get
        {
            return _gridView;
        }

        set
        {
            _gridView = value;
        }
    }



    public EdiFormTemplate()
    {

    }

    public void InstantiateIn(Control container)
    {
        int index = (container as GridViewEditFormTemplateContainer).VisibleIndex;

        ASPxPageControl pc = new ASPxPageControl();
        pc.ID = "ASPxPageControl1";
        pc.TabPages.Add("Material");
        pc.TabPages.Add("Description");

        ASPxLabel lab1 = new ASPxLabel();
        lab1.Text = "Categories:";
        pc.TabPages[0].Controls.Add(lab1);

        ASPxTextBox catTxt = new ASPxTextBox();
        catTxt.ID = "ASPxTextBox1";
        if (!_gridView.IsNewRowEditing)
        {
            catTxt.Text = _gridView.GetRowValues(index, "Material").ToString();
        }
        pc.TabPages[0].Controls.Add(catTxt);

        ASPxLabel lab2 = new ASPxLabel();
        lab2.Text = "Description:";
        pc.TabPages[1].Controls.Add(lab2);

        ASPxTextBox descTxt = new ASPxTextBox();
        descTxt.ID = "ASPxTextBox2";
        if (!_gridView.IsNewRowEditing)
        {
            descTxt.Text = _gridView.GetRowValues(index, "Material").ToString();
        }
        pc.TabPages[1].Controls.Add(descTxt);


        container.Controls.Add(pc);

        ASPxGridViewTemplateReplacement upd = new ASPxGridViewTemplateReplacement();
        upd.ReplacementType = GridViewTemplateReplacementType.EditFormUpdateButton;
        upd.ID = "Update";
        container.Controls.Add(upd);

        ASPxGridViewTemplateReplacement can = new ASPxGridViewTemplateReplacement();
        can.ReplacementType = GridViewTemplateReplacementType.EditFormCancelButton;
        can.ID = "Cancel";
        container.Controls.Add(can);


    }
}
