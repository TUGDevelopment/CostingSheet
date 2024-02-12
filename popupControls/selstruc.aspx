<%@ Page Language="C#" AutoEventWireup="true" CodeFile="selstruc.aspx.cs" Inherits="selcosting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body, html {
            padding: 0;
            margin: 0;
        }
    </style>
    <script type="text/javascript">
        function getUrlVars() {
            var vars = {};
            var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
                vars[key] = value;
            });
            return vars;
        }
        function SynchronizeListBoxValues(dropDown, args, text) {
            //debugger;
            var texts = dropDown.GetText().split(";");
            var values = GetValuesByTexts(texts, text, checkListBox);
            checkListBox.SelectValues(values);
            UpdateText(text, checkListBox, dropDownEdit); // for remove non-existing texts
            if (values != "")
                dropDown.isValid = false;
        }
        function OnListBoxSelectionChanged(s, e, text) {
            UpdateText(text, checkListBox, dropDownEdit);
            //Session["FirstName"]
        }
        function UpdateText(text, ListBox, dropDown) {
            var selectedItems = null;
            selectedItems = ListBox.GetSelectedItems();
            dropDown.SetText(GetSelectedItemsText(selectedItems));
        }
        function GetSelectedItemsText(items) {
            var texts = [];
            for (var i = 0; i < items.length; i++)
                //if (items[i].index != 0)
                texts.push(items[i].text);
            return texts.join(";");
        }
        function GetValuesByTexts(texts, text, ListBox) {
            var actualValues = [];
            var item;
            for (var i = 0; i < texts.length; i++) {
                item = ListBox.FindItemByText(texts[i]);
                if (item != null)
                    actualValues.push(item.value);
            }
            return actualValues;
        }
        function OnRowDblClick(s, e) {
            var result = s.GetRowKey(e.visibleIndex);
            window.parent.HidePopupAndShowInfo('Client', result);
        }
        function OnInit(s, e) {
            AdjustSize();
            document.getElementById("gridContainer").style.visibility = "";
            gv.AddNewRow();
        }
        function OnEndCallback(s, e) {
            AdjustSize();
            if (s.cpShowAlert) {
                var key = getUrlVars()["view"];
                window.parent.HidePopupAndShowInfo(key, "");
            }
        }
        function OnControlsInitialized(s, e) {
            ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
                AdjustSize();
            });
        }

        function AdjustSize() {
            var height = Math.max(0, document.documentElement.clientHeight);
            gv.SetHeight(height);
        }
        function OnAllCheckedChanged(s, e) {
            if (s.GetChecked())
                gv.SelectRows();
            else
                gv.UnselectRows();
        }
        function test() {
             gv.GetValuesOnCustomCallback("CostingSheet|0" , function (r) {
                if (!r)
                    return;
                 var result = r["Code"];
                 var key = getUrlVars()["view"];
                 window.parent.HidePopupAndShowInfo(key, result);
            })
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="gridContainer" style="visibility: hidden">
          <dx:ASPxGridView ID="gv" runat="server" ClientInstanceName="gv" KeyFieldName="ID" Width="100%" OnDataBinding="gv_DataBinding" 
                    OnDataBound="gv_DataBound" OnHtmlRowPrepared="gv_HtmlRowPrepared" OnCustomDataCallback="gv_CustomDataCallback"
                    OnRowUpdating="gv_RowUpdating" OnRowInserting="gv_RowInserting" OnCellEditorInitialize="gv_CellEditorInitialize" 
                    OnCustomCallback="gv_CustomCallback" AutoGenerateColumns="true">
                    <SettingsBehavior ProcessSelectionChangedOnServer="true"></SettingsBehavior>
                    <Settings ShowVerticalScrollBar="true" VerticalScrollableHeight="0" ShowFooter="false" ShowStatusBar="Visible"/>
                    <SettingsSearchPanel CustomEditorID="tbToolbarSearch" />
                    <Toolbars>
                        <dx:GridViewToolbar>
                            <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                            <Items>
                                <dx:GridViewToolbarItem Alignment="Right">
                                    <Template>
                                        <dx:ASPxButtonEdit ID="tbToolbarSearch" runat="server" NullText="Search..." Height="100%">
                                            <Buttons>
                                                <dx:SpinButtonExtended Image-IconID="find_find_16x16gray" />
                                            </Buttons>
                                        </dx:ASPxButtonEdit>
                                    </Template>
                                </dx:GridViewToolbarItem>
                            </Items>
                        </dx:GridViewToolbar>
                    </Toolbars>
                    <%--<Templates>
                        <GroupRowContent>
                           <table>
                               <tr><td>
                                    <dx:ASPxCheckBox ID="checkBox" runat="server" />
                               </td><td>
                                    <dx:ASPxLabel ID="CaptionText" runat="server" Text='<%# GetCaptionText(Container) %>' />
                               </td></tr>
                           </table>
                        </GroupRowContent>
                        <StatusBar>
                        <div style="text-align: right">
                            <dx:ASPxButton ID="btnSave" runat="server" RenderMode="Outline" Text="Submit" AutoPostBack="false">
                            <ClientSideEvents Click="test" />
                            </dx:ASPxButton>
                            <dx:ASPxButton ID="btnCancel" runat="server" RenderMode="Outline" Text="Cancel" AutoPostBack="false">
                            <ClientSideEvents Click="function(s,e){ window.parent.HidePopupAndShowInfo('', '');}" />
                            </dx:ASPxButton>
                        </div>
                    </StatusBar>
                    </Templates>--%>
                    <GroupSummary>
                        <dx:ASPxSummaryItem FieldName="RequestNo" SummaryType="Count" />
                    </GroupSummary>
                    <ClientSideEvents Init="OnInit" EndCallback="OnEndCallback"/>
                    <SettingsBehavior AllowSelectByRowClick="true" AutoExpandAllGroups="true" />
                    <Styles>
                        <StatusBar CssClass="StatusBarWithButtons">
                        </StatusBar>
                    </Styles>
                </dx:ASPxGridView>
        </div>
    </form>
<%--    <asp:SqlDataSource ID="dsGetcosting" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetCostQuota" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="user_name" SessionField="user_name" />
            <asp:SessionParameter Name="Id" SessionField="value" />
        </SelectParameters>
    </asp:SqlDataSource>--%>
    <asp:SqlDataSource ID="dsProductGroup" runat="server" ConnectionString="<%$ ConnectionStrings:LabConnectionString %>"
        SelectCommand="select ProductGroup,Name from tblProductGroup where GroupType='F'">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="ds" runat="server" ConnectionString="<%$ ConnectionStrings:LabConnectionString %>"/>
</body>
</html>