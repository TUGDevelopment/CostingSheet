<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript">
        //function SetCheckBoxChecked(fieldName, index, checked) {
        //    var checkBox = ASPxClientControl.GetControlCollection().GetByName('cb_' + fieldName + "_" + index);
        //    checkBox.SetChecked(checked);
        //}

        function SetCheckBoxVisible(fieldName, index, visibility) {
            var id = 'checkCell_' + fieldName + "_" + index;
            if (document.getElementById(id) != null)
                document.getElementById(id).getElementsByTagName("span")[0].style.visibility = visibility;
        }

        function onClick(s, e) {
            SetCheckBoxVisible("C4", 3, "hidden")
        }
        function onClick1(s, e) {
            SetCheckBoxVisible("C4", 3, "visible")
        }

    </script>
</head>
<body>
    <form id="frmMain" runat="server">
        <dx:ASPxCheckBox ID="BatchUpdateCheckBox" runat="server" Text="Handle BatchUpdate event"
            AutoPostBack="true" />
        <dx:ASPxGridView ID="grvAssignPeriods" runat="server" KeyFieldName="ID" OnBatchUpdate="Grid_BatchUpdate"
            OnRowInserting="Grid_RowInserting" OnRowUpdating="Grid_RowUpdating" OnRowDeleting="Grid_RowDeleting"
            OnHtmlDataCellPrepared="grvAssignPeriods_HtmlDataCellPrepared">
            <Columns>
                <dx:GridViewCommandColumn ShowNewButtonInHeader="true" ShowDeleteButton="true" />
                <dx:GridViewDataColumn FieldName="C1" />
                <dx:GridViewDataSpinEditColumn FieldName="C2" />
                <dx:GridViewDataTextColumn FieldName="C3" />
                <dx:GridViewDataCheckColumn FieldName="C4" />
                <dx:GridViewDataDateColumn FieldName="C5" />
            </Columns>
            <SettingsEditing Mode="Batch" />
        </dx:ASPxGridView>
        <dx:ASPxButton ID="ASPxButton1" runat="server" Text="Hide" AutoPostBack="false">
            <ClientSideEvents Click="onClick" />
        </dx:ASPxButton>
        <dx:ASPxButton ID="ASPxButton2" runat="server" Text="Show" AutoPostBack="false">
            <ClientSideEvents Click="onClick1" />
        </dx:ASPxButton>
    </form>
</body>
</html>
