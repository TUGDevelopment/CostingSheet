using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Download : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e) {
        ExportToResponse(Request["id"], "test", "txt", true);
    }

    public void ExportToResponse(string text, string fileName, string fileType, bool inline) {
        Response.Clear();
        Response.ContentType = "application/" + fileType;
        Response.AddHeader("Content-Disposition", string.Format("{0}; filename={1}.{2}", inline ? "Inline" : "Attachment", fileName, fileType));
        Response.AddHeader("Content-Length", text.Length.ToString());
        //Response.ContentEncoding = System.Text.Encoding.Default;
        Response.Write(text);
        Response.Flush();
        Response.Close();
        Response.End();
    }


}