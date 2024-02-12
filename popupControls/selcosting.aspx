<%@ Page Language="C#" AutoEventWireup="true" CodeFile="selcosting.aspx.cs" Inherits="selcosting" %>

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
        function OnRowDblClick(s, e) {
            debugger;
            var result = s.GetRowKey(e.visibleIndex);
            window.parent.HidePopupAndShowInfo('Client', result);
        }
        function OnInit(s, e) {
            AdjustSize();
            document.getElementById("gridContainer").style.visibility = "";
        }
        function OnEndCallback(s, e) {
            AdjustSize();
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
                    <Templates>
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
                    </Templates>
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
    <asp:SqlDataSource ID="dsGetcosting" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetCostQuota" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="user_name" SessionField="user_name" />
            <asp:SessionParameter Name="Id" SessionField="value" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsGetCostingMat" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetCostingMat" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <%--<asp:SessionParameter Name="Costing" SessionField="Costing" />--%>
            <asp:QueryStringParameter Name="Costing" QueryStringField="Costing" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="ds" runat="server" ConnectionString="<%$ ConnectionStrings:LabConnectionString %>"/>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"/>
</body>
</html>