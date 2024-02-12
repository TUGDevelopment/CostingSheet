using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class popupControls_LoadContentOnDemend : System.Web.UI.Page
{
    MyDataModule cs = new MyDataModule();
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Clear();
        //Response.Write("<br/> " + HttpContext.Current.Request.Url.AbsoluteUri);
        if (!IsPostBack)
        {
            //CreateGridView();
        }
    }
    private void CreateGridView()
    {
        GridView grid = new GridView();
        grid.AutoGenerateColumns = false;
        DataTable dt = GetData();
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            BoundField boundfield = new BoundField();
            boundfield.DataField = dt.Columns[i].ColumnName.ToString();
            boundfield.HeaderText = dt.Columns[i].ColumnName.ToString();
            grid.Columns.Add(boundfield);
        }
        TemplateField tfield = new TemplateField();
        tfield.HeaderText = "DownloadFile";

        grid.Columns.Add(tfield);
        //grid.RowDataBound += OnRowDataBound;
        grid.DataSource = dt;
        grid.DataBind();
        grid.HeaderStyle.CssClass = "header";
        grid.RowStyle.CssClass = "rowstyle";
        Panel1.Controls.Add(grid);
    }

    //protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {
    //        LinkButton lnkView = new LinkButton();
    //        lnkView.ID = "lnkView";
    //        lnkView.Text = "View";
    //        lnkView.Click += ViewDetails;
    //        lnkView.CommandArgument = (e.Row.DataItem as DataRowView).Row["ID"].ToString();
    //        e.Row.Cells[2].Controls.Add(lnkView);
    //    }
    //}
    public DataTable GetData()
    {
        SqlParameter[] param = {
                        new SqlParameter("@Id", Request.QueryString["Id"].ToString())};
        DataTable dt = cs.GetRelatedResources("spGetstdAttached", param);
        dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
        return dt;
    }
    const string UploadDirectory = "~/Content/UploadControl/";
    protected void ViewDetails(object sender, EventArgs e)
    {
        string Id = (sender as LinkButton).CommandArgument;  
        DataTable dt = cs.builditems("SELECT * FROM transstdFileDetails where ID=" + Id);
        foreach (DataRow dr in dt.Rows)
        {
            //string contentType = MimeTypes.GetContentType(dr["Name"].ToString());
            //Response.Clear();
            //Response.Buffer = true;
            //Response.Charset = "";
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.ContentType = contentType;
            //Response.AppendHeader("Content-Disposition", "attachment; filename=" + dr["Name"]);
            //Response.BinaryWrite((byte[])dr["Attached"]);
            //Response.Flush();
            //Response.End();
            char[] charsToTrim = { '*', ' ', '\'', ',' };
            string result = dr["Name"].ToString().Trim(charsToTrim);
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = MimeTypes.GetContentType(dr["Name"].ToString()); ;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + result.Replace(",", "_"));
            Response.BinaryWrite((byte[])dr["Attached"]);
            Response.Flush();
            Response.End();
        }

    }
}