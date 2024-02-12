<%@ Page Language="C#" AutoEventWireup="true" CodeFile="selmaster.aspx.cs" Inherits="popupControls_selmaster" %>

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
        window.onload = function () {
            //alert("Got Called!!");
            //.PerformCallback('reload');
            //var UserType = Demo.getUrlVars()["UserType"];
        }
        function getUrlVars() {
            var vars = {};
            var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
                vars[key] = value;
            });
            return vars;
        }
        function OnItemDoubleClick(s, e) {
            debugger;
            var view = getUrlVars()["view"];
            //var result = s.GetSelectedItem().text;
            //var result = s.GetRowKey(e.visibleIndex);
            var param = view == "RawMaterial"?"Material":"ID";
            s.GetRowValues(e.visibleIndex, param, function (value) {
                if (value) {
                    window.parent.HidePopupAndShowInfo(view, value);
                }
            });
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
                    <dx:ASPxGridView ID="gv" runat="server" ClientInstanceName="gv" KeyFieldName="ID" Width="100%"
                    OnCustomCallback="gv_CustomCallback" OnDataBinding="gv_DataBinding">
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
                    <ClientSideEvents Init="OnInit" EndCallback="OnEndCallback" RowDblClick="OnItemDoubleClick"/>
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
                    <SettingsSearchPanel Visible="true"  />
                    <Settings VerticalScrollBarMode="Visible" VerticalScrollableHeight="500" />
                    <SettingsEditing Mode="Inline"></SettingsEditing>
                </dx:ASPxGridView>
        </div>
    </form>
</body>
</html>
