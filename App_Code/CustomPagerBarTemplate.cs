using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for CustomPagerBarTemplate
/// </summary>
public class MyPagerBarTemplate : ITemplate
{
    string activePageSymbol;int page;

    public MyPagerBarTemplate(string activePageSymbol,int page)
    {
        this.activePageSymbol = activePageSymbol;
        this.page = page;
    }
    public string ActivePageSymbol { get { return activePageSymbol; } }
    public void InstantiateIn(Control container)
    {
        List<string> alphabet = new List<string>(page);
        int startCharIndex = 1;
        int endChartIndex = Convert.ToInt32(page);
        for (int i = startCharIndex; i <= endChartIndex; i++)
            alphabet.Add(i.ToString());

        WebControl div = new WebControl(HtmlTextWriterTag.Div);
        container.Controls.Add(div);
        div.Attributes["class"] = "container";


        foreach (string item in alphabet)
        {
            HtmlAnchor anchor = new HtmlAnchor();
            div.Controls.Add(anchor);

            anchor.HRef = string.Format("javascript:MoveToPage('{0}')", item);
            anchor.InnerText = item;
            anchor.Attributes["class"] = "anchor";
            if (item == ActivePageSymbol)
                anchor.Attributes["class"] += " active";
        }
    }
}