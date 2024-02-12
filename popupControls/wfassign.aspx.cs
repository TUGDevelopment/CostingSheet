using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_wfassign : System.Web.UI.Page
{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    private DataTable result
    {
        get { return Page.Session["result"] == null ? null : (DataTable)Page.Session["result"]; }
        set { Page.Session["result"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session.Clear();
            var Id = Request.QueryString["Id"].ToString();
            var table = Request.QueryString["table"].ToString();
            string strSQL = "select usertype,";
            strSQL += table == "TransTechnical" ? "Assignee," : "'' as Assignee,";
            strSQL += string.Format(@"company,CONVERT(nvarchar(max),UniqueColumn)'NewID' from " + table + " where Id={0}", Id);
            result = cs.builditems(strSQL);
            foreach (DataRow r in result.Rows)
            {
                Page.Session["usertype"] = string.Format("{0}", r["usertype"]);
                Page.Session["BU"] = string.Format("{0}", r["company"]);
                Page.Session["NewID"] = r["NewID"];
                Page.Session["username"] = user_name;
                Page.Session["tablename"] = table;
            }
        }
        //if (Request.QueryString["view"].ToString()== "Attach")
        //{
        //    formLayout.FindItemOrGroupByName("Assignee").Visible = false;
        //    formLayout.FindItemOrGroupByName("btnEvent").Visible = false;
        //}else
        //    formLayout.FindItemOrGroupByName("Attachment").Visible = false;
    }

    //protected void ASPxGridLookup1_DataBound(object sender, EventArgs e)
    //{
    //    ASPxGridLookup lookup = (ASPxGridLookup)(sender);
    //    foreach(DataRow r in result.Rows)
    //    {
    //        var arr = r["Assignee"].ToString().Split(',');
    //        for (int i = 0; i < arr.Length; i++)
    //        lookup.GridView.Selection.SelectRowByKey(arr[i]);
    //    }
    //}

    protected void fileManager_CustomCallback(object sender, CallbackEventArgsBase e)
    {
        ASPxFileManager fm = (ASPxFileManager)sender;
        string[] param = e.Parameter.Split('|'); //bool selected = true;
         fm.DataBind();
    }

    const string UploadDirectory = "~/Content/UploadControl/";
    protected void fileManager_FileUploading(object source, FileManagerFileUploadEventArgs e)
    {

        var GCRecord = string.Format("{0}", Page.Session["NewID"]);
        var resultFilePath = @"~/Content/UploadControl/" + GCRecord + @"/";
        if (!Directory.Exists(Server.MapPath(resultFilePath)))
        {
            Directory.CreateDirectory(Server.MapPath(resultFilePath));
        }
        string resultFileName = Path.ChangeExtension(Path.GetRandomFileName(), e.File.Name);
        var newDocPath = Server.MapPath(resultFilePath) + resultFileName;

        FileStream fs = new FileStream(newDocPath, FileMode.CreateNew);
        e.InputStream.CopyTo(fs);
        fs.Close(); //close the new file created by the FileStream
        byte[] file;
        String name = e.FileName;
        string mimeType = MimeTypes.GetContentType(name);
        using (var stream = new FileStream(newDocPath, FileMode.Open, FileAccess.Read))
        {
            using (var read = new BinaryReader(stream))
            {
                file = read.ReadBytes((int)stream.Length);
            }
        }
        using (SqlConnection con = new SqlConnection(strConn))
        {
            string query = "spinsertFileSystem";
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", name.ToString());
                cmd.Parameters.AddWithValue("@IsFolder", 0);
                cmd.Parameters.AddWithValue("@ParentID", 1);
                cmd.Parameters.AddWithValue("@Data", file);
                cmd.Parameters.AddWithValue("@LastWriteTime", DateTime.Now.ToString());
                cmd.Parameters.AddWithValue("@GCRecord", String.Format("{0}", GCRecord));
                cmd.Parameters.AddWithValue("@LastUpdateBy", String.Format("{0}", Page.Session["username"]));
                cmd.Parameters.AddWithValue("@table", Page.Session["tablename"].ToString());
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        if(Page.Session["tablename"].ToString()== "TransTechnical" && Request.QueryString["type"].ToString().Contains("Spec")) 
            buildData(name, Request.QueryString["type"].ToString());
        e.Cancel = true; //cancelling the upload, prevents duplicate uploads
        e.ErrorText = "Success";
    }

    protected void fileManager_ItemDeleting(object source, FileManagerItemDeleteEventArgs e)
    {
        {
            if (e.Item is FileManagerFolder)
            {
                //e.Cancel = true;
                //e.ErrorText = "Folder's deleting is denied";
            }
            if (e.Item is FileManagerFile)
            {
                using (SqlConnection con = new SqlConnection(strConn))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spDeleteFileSystem";
                    cmd.Parameters.AddWithValue("@ID", e.Item.Id.ToString());
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                buildData(e.Item.FullName, "Del SPEC File");
                fileManager.Settings.RootFolder = e.Item.Location.Replace(@"Folder\","");
                e.Cancel = true;
            }
        }
    }
    void buildData(string name,string t)    {
        //
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "insert into MasHistory values (@Id,@User,9,getdate(),'Upload Spec','',@table)";
            cmd.Parameters.AddWithValue("@Id", Request.QueryString["Id"].ToString());
            cmd.Parameters.AddWithValue("@User", String.Format("{0}", Page.Session["username"]));
            cmd.Parameters.AddWithValue("@table", Request.QueryString["table"].ToString());
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        string strSQL = string.Format(@"select * from TransTechnical where UniqueColumn='{0}'", Page.Session["NewID"]);
        result = cs.builditems(strSQL);
        foreach (DataRow r in result.Rows)
        {
            cs._alertmail(r, name, t, Page.Session["usertype"].ToString());
        }
    }

    protected void cpCustomerRole_Callback(object sender, CallbackEventArgsBase e)
    {

    }
}