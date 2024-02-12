<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReportQuotation.ascx.cs" Inherits="UserControls_TechnicalReport" %>
<script type="text/javascript">
        var isCustomCallback = false;
        //function OnInit(s, e) {
            //AdjustSize();
            //document.getElementById("gridContainer").style.visibility = "";
            //ClientActionMenu.GetItemByName("Filter").SetVisible(true);
            //ClientActionMenu.GetItemByName("back").SetVisible(true);
            //ClientActionMenu.GetItemByName("All").SetVisible(true);
            //ClientActionMenu.GetItemByName("save").SetVisible(true);
        //}
        function gridData_RowDblClick(s, e) {
            var key = s.GetRowKey(e.visibleIndex, "ID");
            if (key != null) {
                if (key == 0)
                    return alert("data not found");
                s.GetRowValues(e.visibleIndex, 'SubID', function (value) {
                    var url = "./Default.aspx?viewMode=QuotationForm&ID=" + value;
                    window.open(url, "_blank");
                });
            }
        }
        function OnEndCallback(s, e) {
            //AdjustSize();
            debugger;
            if (!s.cpFilterExpression)
                return;
            //hfKeyword.Set("Keyword", s.cpFilterExpression);
            //s.cpFilterExpression = null;

            //s.PerformCallback("X");
        }
        //function OnControlsInitialized(s, e) {
        //    ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
        //        AdjustSize();
        //    });
        //}
        //function AdjustSize() {
        //    var height = Math.max(0, document.documentElement.clientHeight);
        //    gridData.SetHeight(height - 80);
        //}
        function OnContextMenuItemClick(sender, args) {
            debugger;
            if (args.objectType == "row") {
                if (args.item.name == "ExportToPDF" || args.item.name == "ExportToXLS") {
                    args.processOnServer = true;
                    args.usePostBack = true;
                }
                if (args.item.name == "Charts") {
                    ASPxClientPopupControl.GetPopupControlCollection().HideAllWindows();
                    //ShowWindowByName(evt.item.name);
                    var name = "test";
                    ShowWindowByName(name, "ManhattanBar");
                }
            }
        }
        function ShowWindowByName(key,name) {
            var popupControl = GetPopupControl();
            var window = popupControl.GetWindowByName(name);
            //debugger;
            var url = 'popupControls/selFilter.aspx';
            popupControl.SetFooterText(url);
            popupControl.SetContentUrl(url);
            popupControl.ShowWindowAtElementByID(window, "tdMenu");    
        }
        function GetPopupControl() {
            return ASPxPopupClientControl;
        }
    function HidePopupAndShowInfo(closedBy, returnValue) {
        ASPxPopupClientControl.Hide();
        if (closedBy == "Client")
        gridData.PerformCallback("filter|"+returnValue);
        }
        //document.onkeydown = function (e) {
        //    debugger;
        //    alert("test");
            //if (ASPxClientUtils.GetKeyCode(e) == 27)
            //    gridData.CloseFilterControl();
        //};
</script>
<dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
<dx:ASPxHiddenField ID="approv" runat="server" ClientInstanceName="approv" />
<dx:ASPxHiddenField ID="username" runat="server" ClientInstanceName="username" />
<dx:ASPxHiddenField ID="hfKeyword" runat="server" ClientInstanceName="hfKeyword" />
<dx:ASPxHiddenField ID="usertp" runat="server" ClientInstanceName="usertp" />
<dx:ASPxLabel ID="ASPxLabel1" runat="server" />
<div id="gridContainer" style="visibility: hidden"></div>
<dx:ASPxGridView runat="server" ID="gridData" ClientInstanceName="gridData" EnableRowsCache="false" Width="100%"
        AutoGenerateColumns="true" KeyFieldName="ID" OnCellEditorInitialize="gridData_CellEditorInitialize" 
        OnFillContextMenuItems="gridData_FillContextMenuItems" OnContextMenuItemClick="gridData_ContextMenuItemClick"
        OnDataBound="gridData_DataBound" OnDataBinding="gridData_DataBinding"
        OnCustomCallback="gridData_CustomCallback"
        OnCustomColumnDisplayText="gridData_CustomColumnDisplayText"
        OnBeforeGetCallbackResult="BeforeGetCallbackResult" OnPreRender="PreRender" 
        Border-BorderWidth="0">
<%--    <Columns>
        <dx:GridViewDataColumn FieldName="RequestNo" Caption="Request No" VisibleIndex="0" Width="8%"/>
        <dx:GridViewDataComboBoxColumn FieldName="StatusApp" GroupIndex="1">
            <PropertiesComboBox DataSourceID="dsStatusApp" TextField="Title" ValueField="ID" />
        </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataDateColumn FieldName="CreateOn" >
            <PropertiesDateEdit DisplayFormatString="dd-MM-yyyy"/>
        </dx:GridViewDataDateColumn> 
        <dx:GridViewDataColumn FieldName="Customer"/>
        <dx:GridViewDataColumn FieldName="Destination"/>
        <dx:GridViewDataTokenBoxColumn FieldName="CD">
            <PropertiesTokenBox DataSourceID="dsuser" ValueField="user_name" TextField="fn"/>
        </dx:GridViewDataTokenBoxColumn>
        <dx:GridViewDataTokenBoxColumn FieldName="PMC">
            <PropertiesTokenBox DataSourceID="dsuser" ValueField="user_name" TextField="fn"/>
        </dx:GridViewDataTokenBoxColumn>
        <dx:GridViewDataColumn FieldName="formula"/>
        <dx:GridViewDataColumn FieldName="Company"/>
        <dx:GridViewDataColumn FieldName="Notes"/>
    </Columns>--%>
    <SettingsSearchPanel ColumnNames="" Visible="false" />
	<Settings ShowGroupPanel="false" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="true" />
	<SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true"/>
		<SettingsPager PageSize="50">
    <PageSizeItemSettings Visible="true" ShowAllItem="true"  AllItemText="All Records" />
    </SettingsPager>
	<Styles>
		<Row Cursor="pointer" />
	</Styles>
    <SettingsContextMenu Enabled="true" />
    <TotalSummary>
        <dx:ASPxSummaryItem FieldName="RequestNo" SummaryType="Count" />
    </TotalSummary>

    <ClientSideEvents 
        Init="Demo.gridData_Init" ContextMenuItemClick="function(s,e) { OnContextMenuItemClick(s, e); }" 
        RowDblClick="gridData_RowDblClick" />
</dx:ASPxGridView>
<dx:ASPxGridViewExporter runat="server" ID="GridExporter" GridViewID="gridData" />
<dx:ASPxButton ID="btn" runat="server" ClientInstanceName="btn" ClientVisible="false" OnClick="btn_Click" />
<dx:ASPxPopupControl ClientInstanceName="ASPxPopupClientControl" SkinID="None" HeaderText="Search" ShowCloseButton="true" Modal="True"
        ShowFooter="True" PopupAction="LeftMouseClick" CloseAction="OuterMouseClick" PopupHorizontalAlign="OutsideRight"
        PopupVerticalAlign="TopSides" EnableViewState="False" AllowResize="true"
        ID="ASPxPopupControl1" runat="server" PopupHorizontalOffset="34" PopupVerticalOffset="2" 
        Width="800px" Height="500px"
        CloseAnimationType="None"
        PopupElementID="imgButton"
        EnableHierarchyRecreation="True">
        <Border BorderColor="#7EACB1" BorderStyle="Solid" BorderWidth="1px" />
    <ContentCollection>
        <dx:PopupControlContentControl runat="server" SupportsDisabledAttribute="True">
        <dx:ASPxFilterControl ID="filter" runat="server" ClientVisible="false" 
            ClientInstanceName="filter">
            <Columns>
                <dx:FilterControlDateColumn ColumnType="DateTime" PropertyName="CreateOn" DisplayName="CreateOn">
                    <PropertiesDateEdit DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy" />
                </dx:FilterControlDateColumn>
                <dx:FilterControlTextColumn ColumnType="String" PropertyName="Customer" DisplayName="Customer" />
                <dx:FilterControlTextColumn ColumnType="String" PropertyName="CostNo" DisplayName="CostNo" />
		        <dx:FilterControlTextColumn ColumnType="String" PropertyName="Material" DisplayName="Material" />
		        <dx:FilterControlTextColumn ColumnType="String" PropertyName="RequestNo" DisplayName="Quotation" />
		        <dx:FilterControlTextColumn ColumnType="String" PropertyName="CreateBy" DisplayName="Requester" />
            </Columns>
            <ClientSideEvents Applied="function(s, e) { gridData.ApplyFilter(e.filterExpression);
                gridData.PerformCallback('filter');}" />
        </dx:ASPxFilterControl>
<%--        <dx:ASPxFilterControl ID="filter" runat="server" Width="100%" ClientInstanceName="filter" ViewMode="VisualAndText">
        </dx:ASPxFilterControl>--%>
        <div style="text-align: right; display:none">
            <dx:ASPxButton runat="server" ID="btnApply" Text="Apply" AutoPostBack="false" UseSubmitBehavior="false" Width="80px" Style="margin: 12px 1em auto auto;">
                <ClientSideEvents Click="function() { filter.Apply(); 
                    var popup = GetPopupControl();
                    popup.Hide();}" />
            </dx:ASPxButton>
        </div>
        </dx:PopupControlContentControl>
    </ContentCollection>
</dx:ASPxPopupControl>
<dx:ASPxCallbackPanel ID="PreviewPanel" runat="server" RenderMode="Div" Height="100%" CssClass="PreviewPanel" ClientInstanceName="ClientPreviewPanel" 
    ClientVisible="false"/>
<%--<asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spSummaryReport" SelectCommandType="StoredProcedure">
    <SelectParameters>
        <asp:Parameter Name="UserNo" Type="String"/>
        <asp:Parameter Name="Keyword" Type="String"/>
    </SelectParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsResult" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spSummaryCosting" SelectCommandType="StoredProcedure">
    <SelectParameters>
        <asp:Parameter Name="UserNo" Type="String"/>
        <asp:Parameter Name="Keyword" Type="String"/>
    </SelectParameters>
</asp:SqlDataSource>--%>
<asp:SqlDataSource ID="dsuser" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select upper(user_name)'user_name',firstname +' '+lastname as fn from ulogin">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsStatusApp" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from MasStatusApp where levelapp in (0,2)"/>
<asp:SqlDataSource ID="CategoriesDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select 0 Category,'Draft' as CategoryName"/>
<%--    <asp:SqlDataSource ID="dsCustomer" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select Value from dbo.FNC_SPLIT('6,4',',')">
    </asp:SqlDataSource>--%>
<asp:SqlDataSource ID="dsCustomer" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select isnull(Code,Custom)Code, Name, Custom from MasCustomer union select '0','',''">
</asp:SqlDataSource>