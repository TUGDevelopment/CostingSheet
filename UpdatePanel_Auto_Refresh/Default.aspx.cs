using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DevExpress.Web.Data;
using DevExpress.Web;
using System.Web.UI;

public partial class _Default : System.Web.UI.Page {
    protected List<GridDataItem> GridData {
        get {
            var key = "34FAA431-CF79-4869-9488-93F6AAE81263";
            if(!IsPostBack || Session[key] == null)
                Session[key] = Enumerable.Range(0, 100).Select(i => new GridDataItem {
                    ID = i,
                    C1 = i % 2,
                    C2 = i * 0.5 % 3,
                    C3 = "C3 " + i,
                    C4 = i % 2 == 0,
                    C5 = new DateTime(2013 + i, 12, 16)
                }).ToList();
            return (List<GridDataItem>)Session[key];
        }
    }
    int vi = 0;
    protected void Page_Load(object sender, EventArgs e) {
        grvAssignPeriods.DataSource = GridData;
        grvAssignPeriods.DataBind();

       
        //for (int i = 1; i <= 3; i++) {
        //    var suffix = string.Empty;
        //    if (i.ToString().Length < 2) {
        //        suffix = i.ToString().PadLeft(2, '0');
        //    }
        //    else {
        //        suffix = i.ToString();
        //    }

        //    var startdate = string.Empty;
        //    var stdate = string.Empty;
        //    if (startdate != null) {
        //        //stdate = Convert.ToDateTime(startdate.START_DATE).ToString("MM/dd/yy");
        //    }

        //    var chkbx = new GridViewDataCheckColumn { FieldName = "Unbound_"+vi.ToString(), UnboundType = DevExpress.Data.UnboundColumnType.Integer };
        //    chkbx.MinWidth = 75;
        //    chkbx.Settings.AllowSort = DevExpress.Utils.DefaultBoolean.False;
        //    vi = vi++;
        //    chkbx.VisibleIndex = vi;
        //    chkbx.DataItemTemplate = new CheckBoxTemplate(suffix, "WK" + suffix + stdate);
        //    grvAssignPeriods.Columns.Add(chkbx);

        //}
    }
    protected void Grid_RowInserting(object sender, ASPxDataInsertingEventArgs e) {
        InsertNewItem(e.NewValues);
        CancelEditing(e);
    }
    protected void Grid_RowUpdating(object sender, ASPxDataUpdatingEventArgs e) {
        UpdateItem(e.Keys, e.NewValues);
        CancelEditing(e);
    }
    protected void Grid_RowDeleting(object sender, ASPxDataDeletingEventArgs e) {
        DeleteItem(e.Keys, e.Values);
        CancelEditing(e);
    }
    protected void Grid_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e) {
        if(!BatchUpdateCheckBox.Checked)
            return;

        foreach(var args in e.InsertValues)
            InsertNewItem(args.NewValues);
        foreach(var args in e.UpdateValues)
            UpdateItem(args.Keys, args.NewValues);
        foreach(var args in e.DeleteValues)
            DeleteItem(args.Keys, args.Values);

        e.Handled = true;
    }
    protected GridDataItem InsertNewItem(OrderedDictionary newValues) {
        var item = new GridDataItem() { ID = GridData.Count };
        LoadNewValues(item, newValues);
        GridData.Add(item);
        return item;
    }
    protected GridDataItem UpdateItem(OrderedDictionary keys, OrderedDictionary newValues) {
        var id = Convert.ToInt32(keys["ID"]);
        var item = GridData.First(i => i.ID == id);
        LoadNewValues(item, newValues);
        return item;
    }
    protected GridDataItem DeleteItem(OrderedDictionary keys, OrderedDictionary values) {
        var id = Convert.ToInt32(keys["ID"]);
        var item = GridData.First(i => i.ID == id);
        GridData.Remove(item);
        return item;
    }
    protected void LoadNewValues(GridDataItem item, OrderedDictionary values) {
        item.C1 = Convert.ToInt32(values["C1"]);
        item.C2 = Convert.ToDouble(values["C2"]);
        item.C3 = Convert.ToString(values["C3"]);
        item.C4 = Convert.ToBoolean(values["C4"]);
        item.C5 = Convert.ToDateTime(values["C5"]);
    }
    protected void CancelEditing(CancelEventArgs e) {
        e.Cancel = true;
        grvAssignPeriods.CancelEdit();
    }
    public class GridDataItem {
        public int ID { get; set; }
        public int C1 { get; set; }
        public double C2 { get; set; }
        public string C3 { get; set; }
        public bool C4 { get; set; }
        public DateTime C5 { get; set; }
    }




    class CheckBoxTemplate: ITemplate {
        private string value;
        private string suffix;
        public CheckBoxTemplate(string suff, string val) {
            value = val;
            suffix = suff;
        }

        public void InstantiateIn(Control container) {
            GridViewDataItemTemplateContainer dataItemContainer = container as GridViewDataItemTemplateContainer;
            int overriden = 1;// Convert.ToInt32(DataBinder.Eval(dataItemContainer.DataItem, OOH_EDI.OVERRIDDEN));
            int week = 1;// Convert.ToInt32(DataBinder.Eval(dataItemContainer.DataItem, OOH_EDI.WEEK_PREFIX + suffix));

            ASPxCheckBox chk = new ASPxCheckBox();
            chk.ClientInstanceName = string.Format("cb_{0}_{1}", dataItemContainer.Column.FieldName, dataItemContainer.VisibleIndex);
                chk.ID = String.Format(value);
            if (overriden == 1) {
                chk.Visible = false;
                dataItemContainer.Visible = false;
                chk.Enabled = false;
            }
            else {
                chk.Visible = true;
                dataItemContainer.Visible = true;
                chk.Enabled = true;
            }
            chk.Checked = Convert.ToBoolean(week);
            dataItemContainer.Controls.Add(chk);
        }
    }



protected void grvAssignPeriods_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e) {
    if (e.DataColumn.FieldName == "C4") {
        e.Cell.Attributes.Add("id", string.Format("checkCell_{0}_{1}", e.DataColumn.FieldName, e.VisibleIndex));
    }
}
}