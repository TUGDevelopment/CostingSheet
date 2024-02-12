using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DevExpress.Web;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using System.IO;
using DevExpress.Web.ASPxGauges.Printing;
using DevExpress.Web.ASPxTreeList;
using DevExpress.Web.ASPxTreeList.Internal;
using System.Collections.Generic;

public partial class UserControls_test : MasterUserControl
{
    public override void Update()
    {
        //gridData.DataSource = dsgv;
        //DataTable dt = ((DataView)dsgv.Select(DataSourceSelectArguments.Empty)).Table;
        ASPxGridView1.DataBind();
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Session["Data"] == null)
        {
            List<Record> list = new List<Record>();
            for (int i = 0; i < 20; i++)
                list.Add(new Record(i, i, i));
            Session["Data"] = list;
        }

        List<DetailRecord> dList = new List<DetailRecord>();
        for (int i = 0; i < 20; i++)
            dList.Add(new DetailRecord(i));
        ASPxGridView1.DataSource = Session["Data"];
        if (ASPxGridView1.Columns["Data"] == null)
        {

            GridViewDataComboBoxColumn column = new GridViewDataComboBoxColumn();
            column.FieldName = "Data";
            column.PropertiesComboBox.DataSource = dsYield;
            column.PropertiesComboBox.ValueField = "Id";
            column.PropertiesComboBox.ValueType = typeof(int);
            column.PropertiesComboBox.TextField = "Material";
            column.PropertiesComboBox.EnableCallbackMode = true;
            column.PropertiesComboBox.CallbackPageSize = 10;
            ASPxGridView1.Columns.Add(column);
        }
        else
        {
            GridViewDataComboBoxColumn column = ASPxGridView1.Columns["Data"] as GridViewDataComboBoxColumn;
            column.PropertiesComboBox.DataSource = dsYield;
            column.PropertiesComboBox.EnableCallbackMode = true;
            column.PropertiesComboBox.CallbackPageSize = 10;
        }
        ASPxGridView1.DataBind();
    }
    protected void ASPxGridView1_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
    {
        ASPxComboBox comboBox = e.Editor as ASPxComboBox;
        //List<DetailRecord> dList = new List<DetailRecord>();
        //for (int i = e.VisibleIndex; i < e.VisibleIndex + 3; i++)
        //    dList.Add(new DetailRecord(i));
        comboBox.EnableCallbackMode = true;
        comboBox.CallbackPageSize = 10;
        comboBox.DataSource = dsYield;
        comboBox.DataBindItems();
    }
    //protected void ASPxGridView1_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
    //{
    //    //List<Record> data = Session["Data"] as List<Record>;
    //    //data[Convert.ToInt32(e.Keys[0])].Data = Convert.ToInt32(e.NewValues["Data"]);
    //    //e.Cancel = true;
    //    //(sender as ASPxGridView).CancelEdit();
    //}
}

public class Record
{
    int id;

    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    int data;

    public int Data
    {
        get { return data; }
        set { data = value; }
    }
    int textdata;
    public int Textdata
    {
        get { return textdata; }
        set { textdata = value; }
    }
    public Record(int id, int data, int textdata)
    {
        this.id = id;
        this.data = data;
        this.textdata = textdata;
    }
}
public class DetailRecord
{
    int id;

    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    public string Data
    {
        get { return "row " + Id.ToString(); }
    }
    public string Textdata
    {
        get { return "X"; }
    }
    public DetailRecord(int id)
    {
        this.id = id;
    }
}