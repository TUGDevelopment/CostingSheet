<%@ Page Language="C#" AutoEventWireup="true" CodeFile="selectedvalue.aspx.cs" Inherits="src_selectedvalue" %>
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
        function OnRowDblClick(s, e) {
            debugger;
            //var result = s.GetRowKey(e.visibleIndex);
            var Query = getParameterByName("Id");
            if (Query == 'Upcharg')
                window.parent.HidePopupAndShowInfo(Query, s.GetRowKey(e.visibleIndex));
            else {
                s.GetRowValues(e.visibleIndex, "Material", function (value) {
                    if (value) {
                        window.parent.HidePopupAndShowInfo('Client', value);
                    }
                });
            }
            //s.GetRowValues(e.visibleIndex, 'Material', OnGetRowId); 
            //window.opener.OnCloseSelector(key);
            //window.close();
        }

        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regexS = "[\\?&]" + name + "=([^&#]*)";
            var regex = new RegExp(regexS);
            var results = regex.exec(window.location.href);
            if (results == null)
                return "";
            else
                return decodeURIComponent(results[1].replace(/\+/g, " "));
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="gridContainer" style="visibility: hidden">
            <dx:ASPxGridView runat="server" ID="gv" Width="100%" KeyFieldName="Id"  
                ClientInstanceName="gv" OnDataBinding="gv_DataBinding"
				EnableViewState="false">
                <ClientSideEvents RowDblClick="OnRowDblClick"  Init="OnInit" EndCallback="OnEndCallback"/>
                <Settings VerticalScrollBarMode="Visible" VerticalScrollableHeight="500" />
                <SettingsEditing Mode="Inline"></SettingsEditing>
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
                <%--<Columns>
                    <dx:GridViewDataColumn FieldName="Material" />
                    <dx:GridViewDataColumn FieldName="Description" />
                </Columns>--%>
                <SettingsResizing ColumnResizeMode="Control" Visualization="Live" />
                <SettingsDataSecurity AllowReadUnlistedFieldsFromClientApi="True" />
                <Styles>
                    <Cell Wrap="False"/>
                    <focusedrow BackColor="#f4dc7a" ForeColor="Black"/>
                </Styles>
                <SettingsContextMenu Enabled="true"/>
                <SettingsPager PageSize="20" EnableAdaptivity="true">
                    <PageSizeItemSettings Visible="true" ShowAllItem="true" AllItemText="All Records" />
                </SettingsPager>
                <SettingsBehavior AllowSelectByRowClick="true" />
                <Settings ShowVerticalScrollBar="true" VerticalScrollableHeight="0"/>
                <SettingsSearchPanel Visible="true"  />
            </dx:ASPxGridView>
        </div>
    </form>
<asp:SqlDataSource ID="dssapMaterial" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spGetsapMaterial2" SelectCommandType="StoredProcedure"/>

<asp:SqlDataSource ID="dsRawMaterial" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spGetSelMaterial" SelectCommandType="StoredProcedure">
    <SelectParameters>
        <asp:QueryStringParameter Name="bu" QueryStringField="bu" />
    </SelectParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsUpcharge" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from StandardUpcharge"></asp:SqlDataSource>
</body>
</html>
