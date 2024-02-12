<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RequestDesForm.ascx.cs" Inherits="UserControls_UploadcostForm" %>
<script type="text/javascript">
    var lastCountry = null;
    var pagesymboll = null;
    var curentEditingIndex;
    var textSeparator = ";";
    var windowOverName = '';
    function MoveToPage(symbol) {
        debugger;
        //alert(symbol);
        pagesymboll = symbol
        ClientintGrid.PerformCallback('symbol|' + symbol);
        //GridView1.PerformCallback('symbol|' + symbol);
    }
    function OnCustomButtonClick(s, e) {
        s.GetRowValues(e.visibleIndex, 'ID', OnGetRowTest);
    }
    function OnGetRowTest(values) {
        debugger;
            ClientgridData.PerformCallback("Test|" + values[0] + "|" + args);
    }
    function OnContextMenuItemClick(sender, args) {
        debugger;
        if (args.objectType == "row") {
            if (args.item.name == "ExportToPDF" || args.item.name == "ExportToXLS") {
                args.processOnServer = true;
                args.usePostBack = true;
            }else if (args.item.name == "Assign") {
                //alert(args.item.name);
                var ID = sender.GetRowKey(args.elementIndex);
                var a = ID.split("|")
                var url = 'popupControls/reassign.aspx?table=TransTechnical&view=' + args.item.name + '&id=' + a[0];
                //pcassign.SetFooterText('Id:' + a[0]);
                pcassign.SetContentUrl(url);
                //pcassign.ShowWindowAtElementByID(window, "tdMenu");
                pcassign.SetHeaderText(url);
                pcassign.Show();
                pcassign.BringToFront();
            }
        }
    }
    function GetPopupControl() {
        return ASPxPopupClientControl;
    }
    function OnButtonClick(evt) {
        var url = 'popupControls/popupsel.aspx?view=' + evt;
        if (evt == "CostingSheet")
            url = "popupControls/selmaster.aspx?view=RequestForm&Company=" + ClientCompanyEdit.GetValue() + "&type=1";
           var popupControl = GetPopupControl();
        popupControl.RefreshContentUrl();
        popupControl.SetContentUrl(url);
 
        popupControl.SetFooterText(url);
        popupControl.Show();
    }
    function HidePopupAndShowInfo(closedBy, returnValue) {
        debugger;
        GetPopupControl().Hide();
        if (closedBy == "CostingSheet" || closedBy =="RequestForm") {
            var g = ClientintGrid.batchEditApi;
            //g.SetCellValue(curentEditingIndex, "CostingSheet", returnValue);
            ClientintGrid.GetValuesOnCustomCallback("CostingSheet|" + returnValue + "|" + curentEditingIndex, function (r) {
                if (!r)
                    return;
                g.SetCellValue(curentEditingIndex, "Material", r["Code"]);
                //g.SetCellValue(curentEditingIndex, "CostingSheet", r["CostNo"]);
                //updategridview(ClientintGrid);
            })
        }
    }
    function OnBatchEditEndEditing(s, e) {
        window.setTimeout(function () {
            debugger;
            var count_value = Demo.GetChangesCount(s.batchEditApi);
            if (count_value > 0)
                s.UpdateEdit();
        }, 0);
    }
    function OnBatchEditStartEditing(s, e) {
        curentEditingIndex = e.visibleIndex;
        //var currentCountry = ClientintGrid.batchEditApi.GetCellValue(curentEditingIndex, "SAPMaterial");
        //if (currentCountry != lastCombo && e.focusedColumn.fieldName == "RawMaterial" && currentCountry != null) {
        //    lastCombo = currentCountry;
        //    RefreshData(currentCountry);
        //}
    }
    function OnBatchEditRowInserting(s, e) {
        //alert("Insert");
    }
    function OnFileUploadComplete(s, e) {
        //    ClientintGrid.PerformCallback();
        ClientintGrid.PerformCallback('upload|1');
    }
    function OnSelectedIndexChangedEdit(s, e) {
        UploadCustom.SetEnabled(true);
        glAssignee.GetGridView().PerformCallback("Add|" + s.GetValue());

    }
    function OnAllCheckedChanged(s, e) {
            if (s.GetChecked())
                ClientintGrid.SelectRows();
            else
                ClientintGrid.UnselectRows();
    }
</script>

<dx:ASPxHiddenField ID="hfBU" runat="server" ClientInstanceName="ClientBU" />
<dx:ASPxHiddenField ID="hfKeyword" runat="server" ClientInstanceName="hfKeyword"/>
<dx:ASPxHiddenField ID="hftype" runat="server" ClientInstanceName="hftype" />
<dx:ASPxHiddenField ID="hfStatusApp" runat="server" ClientInstanceName="hfStatusApp" />
<dx:ASPxHiddenField ID="hfgetvalue" runat="server" ClientInstanceName="ClientNewID" />
<dx:ASPxHiddenField ID="hfGeID" runat="server" ClientInstanceName="hfGeID" />
<dx:ASPxHiddenField ID="hfuser" runat="server" ClientInstanceName="username"/>
<dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
<dx:ASPxHiddenField ID="approv" runat="server" ClientInstanceName="approv" />
<dx:ASPxHiddenField ID="hftablename" runat="server" ClientInstanceName="hftablename" />
<dx:ASPxHiddenField ID="hfRequestType" runat="server" ClientInstanceName="hfRequestType" />
<dx:ASPxHiddenField ID="usertp" runat="server" ClientInstanceName="usertp" />
<dx:ASPxHiddenField ID="hfpara" runat="server" ClientInstanceName="hfpara" />
<dx:ASPxHiddenField ID="hfRole" runat="server" ClientInstanceName="hfRole"/>
<dx:ASPxGridView runat="server" ID="gridData" KeyFieldName="ID" DataSourceID="dsgv" ClientInstanceName="ClientgridData" Width="100%"  
        AutoGenerateColumns="true" OnCustomCallback="gridData_CustomCallback" OnFillContextMenuItems="gridData_FillContextMenuItems"
        OnCustomDataCallback="gridData_CustomDataCallback"
	    OnBeforeGetCallbackResult="BeforeGetCallbackResult" OnPreRender="PreRender" 
        OnContextMenuItemClick="gridData_ContextMenuItemClick" 
        OnCustomButtonInitialize="gridData_CustomButtonInitialize" 
        Border-BorderWidth="0">
        <Paddings PaddingTop="0px" Padding="1px" PaddingBottom="0px" PaddingLeft="0px" />
        <Columns>
        <dx:GridViewDataColumn FieldName="RequestNo" Caption="Request No" VisibleIndex="1" Width="8%"/>
        <dx:GridViewDataTokenBoxColumn FieldName="Destination" VisibleIndex="2">
            <PropertiesTokenBox DataSourceID="dsCountry" TextField="Name" ValueField="ID" />
        </dx:GridViewDataTokenBoxColumn>
        <dx:GridViewDataTextColumn FieldName="Company" VisibleIndex="3">
            <PropertiesTextEdit >
            <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" />
        </PropertiesTextEdit>
        </dx:GridViewDataTextColumn>
        <dx:GridViewDataColumn FieldName="StatusApp" Caption="StatusApp" GroupIndex="0"/>
        <%--
        <dx:GridViewDataTextColumn FieldName="Customer" Caption="Customer" />
        <dx:GridViewDataTextColumn FieldName="RequestType"/>--%>
        <dx:GridViewDataTextColumn FieldName="CreateBy" Caption="Requester" />
        <dx:GridViewDataDateColumn FieldName="CreateOn" Caption="CreateOn"> 
            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
            </PropertiesDateEdit>
        </dx:GridViewDataDateColumn>
        <dx:GridViewDataTokenBoxColumn FieldName="Assignee">
            <PropertiesTokenBox DataSourceID="dsuser" ValueField="user_name" TextField="fn"/>
	    </dx:GridViewDataTokenBoxColumn>
        </Columns>
        <SettingsDataSecurity AllowReadUnlistedFieldsFromClientApi="True" />
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
            ContextMenuItemClick="function(s,e) { OnContextMenuItemClick(s, e); }"
            CustomButtonClick="OnCustomButtonClick"  
            Init="Demo.ClientgridData_Init" 
            RowClick="Demo.ClientgridData_RowClick"/>
    </dx:ASPxGridView>
    <dx:ASPxGridViewExporter runat="server" ID="GridExporter" GridViewID="gridData" />
    <dx:ASPxPopupControl ID="pcassign" runat="server" Width="880px" Height="380px" CloseAction="OuterMouseClick" CloseOnEscape="true"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcassign" AllowResize="true"
        HeaderText="assign" AllowDragging="True" PopupAnimationType="Fade"
        PopupHorizontalOffset="40" PopupVerticalOffset="40" AutoUpdatePosition="true">
        <ClientSideEvents PopUp="function(s, e) { ASPxClientEdit.ClearGroup('createAccountGroup'); }" />
        <SizeGripImage Width="11px" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxPanel ID="Panel2" runat="server" DefaultButton="btCreate">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                     </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl ClientInstanceName="ASPxPopupClientControl" ShowCloseButton ="true" AllowDragging="True"
            ShowFooter="True" PopupAction="None" CloseAction="OuterMouseClick" CloseOnEscape="true" PopupHorizontalAlign="OutsideRight"
            PopupVerticalAlign="Below" AllowResize="true" 
            ID="ASPxPopupControl1" runat="server" PopupHorizontalOffset="34" PopupVerticalOffset="2" 
            Width="680px" Height="380px"
            EnableHierarchyRecreation="True">
            <Border BorderColor="#7EACB1" BorderStyle="Solid" BorderWidth="1px" />
            <ContentCollection>
                <dx:PopupControlContentControl runat="server" SupportsDisabledAttribute="True">
                <dx:ASPxFilterControl ID="filter" runat="server" 
                    ClientInstanceName="filter">
                    <Columns>
                        <dx:FilterControlDateColumn ColumnType="DateTime" PropertyName="CreateOn" DisplayName="CreateOn">
                            <PropertiesDateEdit DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy" />
                        </dx:FilterControlDateColumn>
                        <dx:FilterControlTextColumn ColumnType="String" PropertyName="Customer" DisplayName="Customer" />
                        <dx:FilterControlTextColumn ColumnType="String" PropertyName="RequestNo" DisplayName="RequestNo" />
                    </Columns>
                    <ClientSideEvents Applied="function(s, e) { ClientgridData.ApplyFilter(e.filterExpression);
                        ClientgridData.PerformCallback('filter');}" />
                </dx:ASPxFilterControl>
                <div style="text-align: right">
                    <dx:ASPxButton runat="server" ID="btnApply" Text="Apply" AutoPostBack="false" UseSubmitBehavior="false" Width="80px" Style="margin: 12px 1em auto auto;">
                        <ClientSideEvents Click="function() { filter.Apply(); 
                            var popup = GetPopupControl();
                            popup.Hide();}" />
                    </dx:ASPxButton>
                </div>
                </dx:PopupControlContentControl>
            </ContentCollection>
        </dx:ASPxPopupControl>   
        <dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" Height="100%" ClientInstanceName="ClientFormPanel" ClientVisible="false">
	<ClientSideEvents Init="Demo.Init" />
        <PanelCollection>
            <dx:PanelContent>  
            <dx:ASPxFormLayout runat="server" ID="formLayout" ClientInstanceName="formLayout" CssClass="formLayout" AlignItemCaptionsInAllGroups="true">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
            <Items>
                <dx:LayoutGroup Caption="Marketing to update costing sheet" ColCount="4" GroupBoxDecoration="None"
                    UseDefaultPaddings="false" Paddings-PaddingTop="10">
                    <GroupBoxStyle>
                        <Caption Font-Bold="true" Font-Size="10"/>
                    </GroupBoxStyle>
                    <GridSettings StretchLastItem="true" WrapCaptionAtWidth="660">
                        <Breakpoints>
                            <dx:LayoutBreakpoint MaxWidth="500" ColumnCount="1" Name="S" />
                            <dx:LayoutBreakpoint MaxWidth="800" ColumnCount="2" Name="M" />
                        </Breakpoints>
                    </GridSettings>
                    <Items>

                        <dx:LayoutItem Caption="Request No" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbRequestNoEdit" ClientInstanceName="tbRequestNoEdit" ReadOnly="true">
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Factory" VerticalAlign="Middle" Name="ProductCategory">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbCompanyEdit" runat="server" DataSourceID="dsPlant" ValueField="Code"
                                        ClientInstanceName="ClientCompanyEdit" TextFormatString="{0}">
                                        <Columns>
                                            <dx:ListBoxColumn FieldName="Code" />
                                            <dx:ListBoxColumn FieldName="Title" />
                                            <dx:ListBoxColumn FieldName="Company"/>
                                            <%--<dx:ListBoxColumn FieldName="usertype" />--%>
                                        </Columns>
                                    <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group2"/>
                                    <ClientSideEvents SelectedIndexChanged="OnSelectedIndexChangedEdit" />
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>                    
                       
                        <dx:LayoutItem Caption="Valid from" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxDateEdit runat="server" ID="deValidfromEdit" ClientInstanceName="ClientValidfromEdit"
                                                    DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                                    <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group2"/>
                                                </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Valid To" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxDateEdit runat="server" ID="deValidtoEdit" ClientInstanceName="ClientValidtoEdit"
                                            DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                            <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group2"/>
                                        </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Assignee" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxGridLookup ID="glAssignee" runat="server" ClientInstanceName="glAssignee" DataSourceID="dsAssigneeEdit" SelectionMode="Multiple"
                                        KeyFieldName="user_name" TextFormatString="{0}" MultiTextSeparator=", " OnDataBound="glAssignee_DataBound">
                                        <Columns>
                                        <dx:GridViewCommandColumn ShowSelectCheckbox="True" />
                                        <dx:GridViewDataColumn FieldName="fn" />
                                    </Columns>
                                    <GridViewProperties>
                                        <Settings ShowFilterRow="true" ShowStatusBar="Visible"/>
                                        <SettingsPager PageSize="7" EnableAdaptivity="true" />
                                    </GridViewProperties>
                                </dx:ASPxGridLookup>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>                        
                        <dx:LayoutItem Caption="Request Date" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxDateEdit runat="server" ID="deRequestDateEdit" ClientInstanceName="ClientRequestDateEdit" 
                                         DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                        <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" ValidationGroup="group2"/>
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Destination" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxComboBox ID="CmbCustomPriceEdit" runat="server" DataSourceID="dsCountry" TextField="Name" ValueField="ID" 
                                          ClientInstanceName="ClientCustomPriceEdit">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group2"/>
                                       </dx:ASPxComboBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>   

                          <%--<dx:LayoutItem Caption="Customer" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbCustomerEdit" runat="server" ClientInstanceName="ClientCustomEdit">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>  --%>
                        <dx:LayoutItem Caption="Notes" ColSpan="4" Width="100%" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxMemo runat="server" ID="tbRemarkEdit" Rows="4" ClientInstanceName="tbRemarkEdit">
                                    </dx:ASPxMemo>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>  
                        <dx:LayoutItem Caption="Selected" Name="Upload" Width="100%">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxUploadControl ID="Upload" runat="server" OnFileUploadComplete="Upload_FileUploadComplete" Width="200" ClientInstanceName="UploadCustom" 
                                    NullText="Click here to browse files..." UploadMode="Advanced" FileUploadMode="OnPageLoad" AutoStartUpload="True">
                                    <ValidationSettings AllowedFileExtensions=".xls,.xlsx">
                                    </ValidationSettings>
                                    <ClientSideEvents FileUploadComplete="OnFileUploadComplete" />
                                </dx:ASPxUploadControl>
                                </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="" Width="100%">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxGridView ID="Grid" ClientInstanceName="ClientintGrid" runat="server" Width="100%"   
                                    OnDataBinding="Grid_DataBinding" AutoGenerateColumns="false" KeyFieldName="ID"  
                                    OnCustomCallback="Grid_CustomCallback" 
                                    OnCustomDataCallback="Grid_CustomDataCallback"
                                    OnBatchUpdate="Grid_BatchUpdate">     
                                    <Toolbars>
                                        <dx:GridViewToolbar Name="MyToolbar">
                                            <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                                            <Items>
                                                <dx:GridViewToolbarItem Command="New"/>
                                                <dx:GridViewToolbarItem Command="Delete" />
                                                <dx:GridViewToolbarItem Command="Refresh" BeginGroup="true" AdaptivePriority="2" />
                                                <dx:GridViewToolbarItem Text="Export to" Image-IconID="actions_download_16x16office2013" BeginGroup="true" AdaptivePriority="1">
                                                    <Items>
                                                        <dx:GridViewToolbarItem Command="ExportToPdf" />
                                                        <dx:GridViewToolbarItem Command="ExportToDocx" />
                                                        <dx:GridViewToolbarItem Command="ExportToRtf" />
                                                        <dx:GridViewToolbarItem Command="ExportToCsv" />
                                                        <dx:GridViewToolbarItem Command="ExportToXls" Text="Export to XLS(DataAware)" />
                                                        <dx:GridViewToolbarItem Name="CustomExportToXLS" Text="Export to XLS(WYSIWYG)" Image-IconID="export_exporttoxls_16x16office2013" />
                                                        <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Export to XLSX(DataAware)" />
                                                        <dx:GridViewToolbarItem Name="CustomExportToXLSX" Text="Export to XLSX(WYSIWYG)" Image-IconID="export_exporttoxlsx_16x16office2013" />
                                                    </Items>
                                                </dx:GridViewToolbarItem>
                                            </Items>
                                        </dx:GridViewToolbar>
                                    </Toolbars>                            
                                    <Columns>
                                        <dx:GridViewBandColumn Caption="Costing Sheet">
                                            <Columns>
                                                <dx:GridViewDataButtonEditColumn FieldName="Material">
                                                    <PropertiesButtonEdit>
                                                        <Buttons>
                                                    <dx:EditButton />
                                                    </Buttons>
                                                    <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('CostingSheet'); }" />
                                                    </PropertiesButtonEdit>
                                                </dx:GridViewDataButtonEditColumn>
                                                
                                                <%--<dx:GridViewDataComboBoxColumn FieldName="Result">
                                                    <PropertiesComboBox DataSourceID="dsAction" ValueField="idx" TextField="value" />
                                                </dx:GridViewDataComboBoxColumn>--%>
                                            </Columns>
                                        </dx:GridViewBandColumn>
                                        <%--<dx:GridViewDataColumn FieldName="SiteId" Width="15%"/>
					                    <dx:GridViewDataColumn FieldName="Series" Caption="Status"/>--%>
                                        <dx:GridViewDataColumn FieldName="Mark" />
                                    </Columns>
                                    <SettingsSearchPanel ColumnNames="" Visible="false" />
		                            <Settings ShowGroupPanel="false" VerticalScrollableHeight="300" VerticalScrollBarMode="Visible"
					                ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="false" ShowStatusBar="Hidden"/>
		                            <SettingsPager PageSize="500">
                                        <PageSizeItemSettings Visible="true" ShowAllItem="true"  AllItemText="All Records" />
                                    </SettingsPager>
		                            <Styles>
			                            <Row Cursor="pointer" />
		                            </Styles>
				                    <SettingsContextMenu Enabled="true"/>
                                    <SettingsEditing Mode="Batch" />                                   
                                    <ClientSideEvents 
                                        ContextMenuItemClick="function(sender, args) { 
                                        if (args.objectType == 'row') {
                                            if (args.item.name == 'PDF' || args.item.name == 'XLS' || args.item.name == 'ExportToXLS') {
                                                btn.DoClick();
                                            }
                                        }}" BatchEditEndEditing="OnBatchEditEndEditing" BatchEditStartEditing="OnBatchEditStartEditing"/>
                                </dx:ASPxGridView>
                                <dx:ASPxButton ID="btn" runat="server" Text="Export" ClientInstanceName="btn" 
                                    RenderMode="Link" OnClick="btn_Click" ClientVisible="false" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:LayoutGroup>
                <dx:LayoutGroup Caption="Action" ColCount="3" GroupBoxDecoration="Box" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                    <GroupBoxStyle>
                        <Caption Font-Bold="true" Font-Size="10"/>
                    </GroupBoxStyle>
                    <GridSettings StretchLastItem="true" WrapCaptionAtWidth="660">
                        <Breakpoints>
                            <dx:LayoutBreakpoint MaxWidth="500" ColumnCount="1" Name="S" />
                            <dx:LayoutBreakpoint MaxWidth="800" ColumnCount="2" Name="M" />
                        </Breakpoints>
                    </GridSettings>
                    <Items>
                        <dx:LayoutItem Caption="Disponsition">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                              <dx:ASPxComboBox ID="CmbDisponsition" runat="server" TextField="Title" ValueField="Id" OnCallback="CmbDisponsition_Callback"
                                           ClientInstanceName="ClientDisponsition" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Comment" Width="100%">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxMemo runat="server" ID="mComment" Rows="4" ClientInstanceName="mComment">
                                    </dx:ASPxMemo>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem> 
                        <dx:LayoutItem Caption="EventLog" Width="100%" CaptionStyle-Font-Bold="true" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
					                    <div id="divScroll" style="overflow: scroll; height: 230px; width: auto;">
                                        <dx:aspxlabel id="lbEventLog" runat="server" clientinstancename="clienteventlog" text="ASPxLabel"></dx:aspxlabel></div>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem> 
                    </Items>
                    </dx:LayoutGroup>
            </Items>
    </dx:ASPxFormLayout>
            </dx:PanelContent>
    </PanelCollection>
    </dx:ASPxCallbackPanel>
    <asp:SqlDataSource ID="dsresult" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetRequestDesForm" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Name="ID" Type="String" />
            <asp:Parameter Name="user" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>  
    <asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetRequestDesall" SelectCommandType="StoredProcedure">
        <SelectParameters>
		<asp:ControlParameter ControlID="hfuser" Name="user_name" PropertyName="['user_name']"/>
	        <asp:ControlParameter ControlID="hftype" Name="type" PropertyName="['type']"/>
		<asp:ControlParameter ControlID="hfKeyword" Name="Keyword" PropertyName="['Keyword']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsAssigneeEdit" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spAssigneeEdit" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
            <asp:ControlParameter ControlID="hfgetvalue" Name="ID" PropertyName="['NewID']"/>
            <asp:ControlParameter ControlID="FormPanel$formLayout$CmbCompanyEdit" Name="BU" PropertyName="Value" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsCompany" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasCompany where Code in (select distinct value from dbo.FNC_SPLIT(@Bu,'|')) order by Code">
    <SelectParameters>
            <asp:ControlParameter ControlID="hfBU" Name="BU" PropertyName="['BU']" />
        </SelectParameters>
    </asp:SqlDataSource>
   <asp:SqlDataSource ID="dsPlant" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetPlant" SelectCommandType="StoredProcedure">
       <SelectParameters>
            <asp:ControlParameter ControlID="hfBU" Name="BU" PropertyName="['BU']" />
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
       </SelectParameters>
   </asp:SqlDataSource>
<%--   <asp:SqlDataSource ID="dsCustomerPrice" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasCustomerPrice where dbo.fnc_checktype(usertype,@usertype)>0">
        <SelectParameters>
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
   </asp:SqlDataSource>--%>
    <asp:SqlDataSource ID="dsuser" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetUser" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from masusertype where Id in (select value from dbo.FNC_SPLIT(@usertype,';'))">
        <SelectParameters>
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsCountry" runat="server" ConnectionString="<%$ ConnectionStrings:LabConnectionString %>"
       SelectCommand="select Id,Code, Description as Name from [transGrade] where ProductType='PF'">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsAction" runat="server" ConnectionString="<%$ ConnectionStrings:LabConnectionString %>"
       SelectCommand="select * from dbo.FNC_SPLIT(';Ok use current code;New code, same formulation;Request New TRF',';')">
    </asp:SqlDataSource>
 