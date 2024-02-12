<%@ Control Language="C#" AutoEventWireup="true" CodeFile="formulaForm.ascx.cs" Inherits="UserControls_test" %>
   <script type="text/javascript">
       var curentEditingIndex;
       var lastCombo = null;
       var isCustomCascadingCallback = false;
        function OnIDChanged(s) {
            debugger;
            var identi = s.GetValue().toString() == "3";
            formLayout.GetItemByName("Reason").SetVisible(identi);
        }
        function OnContextMenuItemClick(sender, args) {
            debugger;
            if (args.objectType == "row") {
                if (args.item.name == "ExportToPDF" || args.item.name == "ExportToXLS") {
                    args.processOnServer = true;
                    args.usePostBack = true;
                } else {
                    args.processOnServer = false;
                    var key = sender.GetRowKey(args.elementIndex);
                    var a = key.split("|")
                    if(args.item.name == "Revised")
                        gridData.PerformCallback(args.item.name + "|" + a[0]);
                }
            }
        }
        function OnItemClick(s, e) {
            debugger;
               var command = e.item.name;
               switch (command) {
                   case "new":
                       grid.AddNewRow();
                       break;
                   case "delete":
                       var index = grid.GetFocusedRowIndex();
                       grid.DeleteRow(index);
                       break;
               }
               return false;
        }
        function OnCountryChanged(ClientCompany) {
            //ClientCompany.PerformCallback("reload|" + ClientCompany);
        }
        function completarArticulo(s, e) {
            //alert("test");
        }
        function Combo_SelectedIndexChanged(s, e) {          
            lastCombo = s.GetValue();
            //alert(lastCombo);
            isCustomCascadingCallback = true;
            RefreshData(lastCombo);
            debugger;
            var textname = s.GetSelectedItem().GetColumnText(1);//s.GetText().toString();
            grid.GetValuesOnCustomCallback("Description|" + textname, DataCallback);
            //ClientintGrid.SetEditValue('Description', textname);
        }
        function OnSelectedIndexChanged(s, e) {
            //debugger;
            var name = s.GetSelectedItem().GetColumnText(1);//s.GetText().toString();

            var newValueOfComboBox = name + '|' + s.GetSelectedItem().GetColumnText(2);//s.GetValue();
            grid.GetValuesOnCustomCallback("RawMaterial|" + newValueOfComboBox, DataCallback);
        }
        function DataCallback(result) {
            //alert(result);
            var results = result.split("|");
            switch (results[0]) {
                case "Description":
                    grid.batchEditApi.SetCellValue(curentEditingIndex, "Description", results[1]);
                    break;
                case "RawMaterial":
                    grid.batchEditApi.SetCellValue(curentEditingIndex, "Name", results[1]);
                    grid.batchEditApi.SetCellValue(curentEditingIndex, "Yield", results[2]);
                    break;
            }
        }
        function Combo_EndCallback(s, e) {
            if (isCustomCascadingCallback) {
                if (s.GetItemCount() > 0)
                    grid.batchEditApi.SetCellValue(curentEditingIndex, "RawMaterial", s.GetItem(0).value);
                isCustomCascadingCallback = false;
            }
        }
        function RefreshData(countryValue) {
            hf.Set("CurrentCountry", countryValue);
            RawMaterial.PerformCallback();
        }
        function OnBatchEditStartEditing(s, e) {
            debugger;
            curentEditingIndex = e.visibleIndex;
            var currentCountry = s.batchEditApi.GetCellValue(curentEditingIndex, "SAPMaterial");
            if (currentCountry != lastCombo && e.focusedColumn.fieldName == "RawMaterial" && currentCountry != null) {
                lastCombo = currentCountry;
                RefreshData(currentCountry);
            }
        }
        function OnBatchEditEndEditing(s, e) {
//            debugger;
            window.setTimeout(function () {
                s.UpdateEdit();
            }, 0);
       }
       function OnButtonClick(evt, s) {
           debugger;
           var url = 'popupControls/selmaster.aspx?view=' + evt;
           if (evt == "Matformu") {

           }
           else if (evt == "Receiver")
               url += "&Plant=" + ClientCompany.GetValue() + "&Category=0";
           else if (evt == "RequestNo")
               url += "&Company=" + ClientCompany.GetValue() + "&User=" + username.Get("user_name");
           pcassign.RefreshContentUrl();
           pcassign.SetContentUrl(url);
           pcassign.SetHeaderText(url);
           pcassign.Show();
       }
       function HidePopupAndShowInfo(closedBy, returnValue) {
           debugger;
           pcassign.Hide();
           if (closedBy == "Matformu") {
               grid.batchEditApi.SetCellValue(curentEditingIndex, "Matformu", returnValue);
               grid.GetValuesOnCustomCallback("Matformu|" + returnValue + "|" + curentEditingIndex, function (r) {
                   if (!r)
                       return;
                   var g = grid.batchEditApi;
                   g.SetCellValue(curentEditingIndex, "Material", r["Code"]);
                   g.SetCellValue(curentEditingIndex, "Description", r["Name"]);
                   //g.SetCellValue(curentEditingIndex, "Unit", r["Unit"]);
                   //g.SetCellValue(curentEditingIndex, "ExchangeRate", r["ExchangeRate"]);
                   //g.SetCellValue(curentEditingIndex, "BaseUnit", r["BaseUnit"]);
                   //g.SetCellValue(curentEditingIndex, "Description", r["Description"]);
                   //calculator(grid);
               })
           }
           else if (closedBy == "Matdetail") {
               detail.batchEditApi.SetCellValue(curentEditingIndex, "Matdetail", returnValue);
               detail.GetValuesOnCustomCallback("Matdetail|" + returnValue + "|" + curentEditingIndex, function (r) {
                   if (!r)
                       return;
                   var g = detail.batchEditApi;
                   g.SetCellValue(curentEditingIndex, "Material", r["Code"]);
                   g.SetCellValue(curentEditingIndex, "Description", r["Name"]);
                   //g.SetCellValue(curentEditingIndex, "Unit", r["Unit"]);
                   //g.SetCellValue(curentEditingIndex, "ExchangeRate", r["ExchangeRate"]);
                   //g.SetCellValue(curentEditingIndex, "BaseUnit", r["BaseUnit"]);
                   //g.SetCellValue(curentEditingIndex, "Description", r["Description"]);
                   //calculator(grid);
               })
           }
           else
               gridData.GetValuesOnCustomCallback(["Get", closedBy, returnValue].join('|'), OnGetValuesOnCustomCallbackComplete);
       }
       function RequestOnTextChanged(r) {
           CmbTechnical.SetValue(r["RequestNo"]);
           hfRequestNo.Set("ID", r["ID"]);
           //CmbPackaging.SetText(r["Packaging"]);
           ClientCustomer.SetText(r["Customer"]);
           ClientNetweight.SetText(r["NetWeight"]);
           ClientUnit.SetText(r["WeightUnit"]);
       }
       function OnGetValuesOnCustomCallbackComplete(values) {
           if (!values)
               return;
           if (values["view"] == "RequestNo") {
               RequestOnTextChanged(values);
           }
       }
       function OnTabbedGroupPageControlInit(s, e) {
           s.SetActiveTabIndex(0);
           //s.SetActiveTabIndex(TabList.GetActiveTabIndex());
       }
       function OnActiveTabChanged(s, e) {
           var selectedIndex = s.GetActiveTabIndex();
           UpdateTabListDecoration(s);
           tabbedGroupPageControl.SetActiveTabIndex(selectedIndex);
           //if (selectedIndex == 4)
           //fileManager.PerformCallback(['load', 0].join("|"));
           //    cpUpdateFiles.PerformCallback(['reload', 0].join("|"));
       }
       function UpdateTabListDecoration(s) {
           //debugger;
           var selectedIndex = s.GetActiveTabIndex();
       }
       function OnContextMenuItemClick(s, e) {
           if (e.objectType == "row") {
               //alert(e.item.name);
           }
       }
       function OnCustomButtonClick(s, e) {
           s.GetRowValues(e.visibleIndex, 'Id', OnGetRowTest);
       }
       function OnGetRowTest(values) {
           //alert(values);
           grid.PerformCallback(['New', values].join('|'));
       }
       function fn_AllowonlyNumeric(s, e) {
           var theEvent = e.htmlEvent || window.event;
           var key = theEvent.keyCode || theEvent.which;
           key = String.fromCharCode(key);
           var regex = /[0-9,.,-]/;

           if (!regex.test(key)) {
               theEvent.returnValue = false;
               if (theEvent.preventDefault)
                   theEvent.preventDefault();
           }
       }
       function OnNetweight(s, e) {
           grid.PerformCallback('changed|' + s.GetText());
       }
       function OnToolbarItemClick(s, e) {
           debugger;
           var key = s.GetRowKey(s.GetFocusedRowIndex());
           if (e.item.name == "Remove") {
               if (confirm('Confirm: delete items?' + key)) {
                   grid.PerformCallback("Remove|" + key);
               } else
                   grid.PerformCallback("Delete|" + key);
           }
       }
   </script>

<dx:ASPxHiddenField ID="usertp" runat="server" ClientInstanceName="usertp" />
<dx:ASPxHiddenField ID="hfgetvalue" runat="server" ClientInstanceName="ClientNewID" />
<dx:ASPxHiddenField ID="hfGeID" runat="server" ClientInstanceName="hfGeID" />
<dx:ASPxHiddenField ID="hfid" runat="server" ClientInstanceName="hfid" />
<dx:ASPxHiddenField ID="hf" runat="server" ClientInstanceName="hf"/>
<dx:ASPxHiddenField ID="hfStatusApp" runat="server" ClientInstanceName="hfStatusApp" />
<dx:ASPxHiddenField ID="username" runat="server" ClientInstanceName="username"/>
<dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
<dx:ASPxHiddenField ID="approv" runat="server" ClientInstanceName="approv" />
<dx:ASPxHiddenField ID="hftype" runat="server" ClientInstanceName="hftype" />
<dx:ASPxHiddenField ID="hfBu" runat="server" ClientInstanceName="hfBu"/>
<dx:ASPxHiddenField ID="hfRequestNo" runat="server" ClientInstanceName="hfRequestNo"/>
<dx:ASPxGridView runat="server" ID="gridData" KeyFieldName="ID" DataSourceID="dsgv" ClientInstanceName="gridData" Width="100%" EnableRowsCache="false"
        AutoGenerateColumns="true" OnCustomCallback="gridData_CustomCallback" OnCustomButtonCallback="gridData_CustomButtonCallback"
        OnCustomDataCallback="gridData_CustomDataCallback" OnCustomGroupDisplayText="gridData_CustomGroupDisplayText"
        OnDataBound="gridData_DataBound"
        OnFillContextMenuItems="gridData_FillContextMenuItems" OnContextMenuItemClick="gridData_ContextMenuItemClick"
        Border-BorderWidth="0">
    <Columns>
        <%--<dx:GridViewCommandColumn ShowNewButton="true" ShowEditButton="true" VisibleIndex="0" ButtonRenderMode="Image" Width="45">
            <CustomButtons>
                <dx:GridViewCommandColumnCustomButton ID="Clone">
                    <Image ToolTip="Clone Record" Url="~/Content/Images/Copy.png"/>
                </dx:GridViewCommandColumnCustomButton>
            </CustomButtons>
        </dx:GridViewCommandColumn>
        <dx:GridViewDataTextColumn FieldName="Company" />
        <dx:GridViewDataTextColumn FieldName="MarketingNumber" Caption="Costing No" />      
        <dx:GridViewDataTextColumn FieldName="CanSize" Caption="CanSize" />
        <dx:GridViewDataTextColumn FieldName="Code"/>
        <dx:GridViewDataTextColumn FieldName="Name" Caption="Product Name"/>
        <dx:GridViewDataComboBoxColumn FieldName="StatusApp">
            <PropertiesComboBox DataSourceID="dsStatusApp" TextField="Title" ValueField="ID" />
        </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataTextColumn FieldName="Destination"/>
        <dx:GridViewDataComboBoxColumn FieldName="Requester">
            <PropertiesComboBox DataSourceID="dsulogin" TextField="fn" ValueField="user_name" />
        </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataDateColumn FieldName="CreateOn" Caption="CreateOn"> 
            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
            </PropertiesDateEdit>
        </dx:GridViewDataDateColumn>--%>
            <dx:GridViewDataColumn FieldName="ID" />
            <dx:GridViewDataColumn FieldName="RequestNo" GroupIndex="0"/>
            <dx:GridViewDataColumn FieldName="Code"/>  
            <dx:GridViewDataColumn FieldName="RefSamples"/>  
            <dx:GridViewDataColumn FieldName="NetWeight"/>
            <dx:GridViewDataColumn FieldName="Packaging" Width="70px"/>
            <dx:GridViewDataColumn FieldName="CostNo" Caption="TRF"/>
            <dx:GridViewDataColumn FieldName="Customer"/>
            <dx:GridViewDataColumn FieldName="Revised" Width="70px"/>
        </Columns>
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
        <ClientSideEvents ContextMenuItemClick="function(s,e) { OnContextMenuItemClick(s, e); }"
            RowClick="Demo.gridData_RowClick"
            EndCallback="Demo.Test"
            Init="Demo.gridData_Init"/>
    </dx:ASPxGridView>
    <dx:ASPxGridViewExporter runat="server" ID="GridExporter" GridViewID="gridData" />
    <dx:ASPxPopupControl ID="pcassign" runat="server" ShowCloseButton ="true" AllowDragging="True" AllowResize="true"
            ShowFooter="True" PopupAction="None" CloseAction="OuterMouseClick" CloseOnEscape="true" PopupHorizontalAlign="OutsideRight"
            PopupVerticalAlign="Below" Width="680px" Height="380px"
            PopupHorizontalOffset="40" PopupVerticalOffset="40">
        <ClientSideEvents PopUp="function(s, e) { ASPxClientEdit.ClearGroup('createAccountGroup'); }"  />
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
    <dx:ASPxPopupControl ClientInstanceName="ASPxPopupClientControl" SkinID="None" HeaderText="MSCharts" ShowCloseButton="true"
            ShowFooter="True" PopupAction="LeftMouseClick" CloseAction="OuterMouseClick" PopupHorizontalAlign="OutsideRight"
            PopupVerticalAlign="TopSides" EnableViewState="False"
            ID="ASPxPopupControl1" runat="server" PopupHorizontalOffset="34" PopupVerticalOffset="2" 
            Width="680px" Height="380px"
            CloseAnimationType="None"
            PopupElementID="imgButton"
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
                </Columns>
<%--                <ClientSideEvents Applied="function(s, e) { gridData.ApplyFilter(e.filterExpression);
                    gridData.PerformCallback('filter');}" />--%>
            </dx:ASPxFilterControl>
    <%--        <dx:ASPxFilterControl ID="filter" runat="server" Width="100%" ClientInstanceName="filter" ViewMode="VisualAndText">
            </dx:ASPxFilterControl>--%>
            <div style="text-align: right">
                <dx:ASPxButton runat="server" ID="btnApply" Text="Apply" AutoPostBack="false" UseSubmitBehavior="false" Width="80px" 
                    Style="margin: 12px 1em auto auto;">
                    <%--<ClientSideEvents Click="function() { filter.Apply(); 
                        var popup = GetPopupControl();
                        popup.Hide();}" />--%>
                </dx:ASPxButton>
            </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
    <%--<dx:ASPxCallbackPanel ID="PreviewPanel" runat="server" RenderMode="Div" Height="100%" CssClass="PreviewPanel" ClientInstanceName="ClientPreviewPanel"/>--%>
    <dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" ScrollBars="Vertical" ClientInstanceName="formulaFormPanel" ClientVisible="false">
        <PanelCollection>
            <dx:PanelContent>  
            <dx:ASPxSplitter ID="ASPxSplitter1" runat="server" ClientInstanceName="sampleSplitter" ShowCollapseBackwardButton="True" 
                ShowCollapseForwardButton="True" Orientation="Vertical" >
                <Styles>
                    <Pane>
                        <Paddings Padding="0px" />
                    </Pane>
                </Styles>
                <Panes>
                <dx:SplitterPane Name="gridContainer" Size="60%" MinSize="100" AutoHeight="True" PaneStyle-Border-BorderWidth="0px">
                <ContentCollection>
                    <dx:SplitterContentControl runat="server">

            <dx:ASPxFormLayout runat="server" ID="formLayout" CssClass="formLayout" AlignItemCaptionsInAllGroups="true">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
            <Items>
                <dx:LayoutGroup Caption="" ShowCaption="False" ColCount="4" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                    <Paddings PaddingTop="10px"></Paddings>
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
                        <dx:LayoutItem Caption="Company Code">
                                                        <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbCompany" runat="server" DataSourceID="dsCompany" ValueField="Code" Width="200px"
                                        ClientInstanceName="ClientCompany" TextFormatString="{0}" OnCallback="CmbCompany_Callback">
                                        <Columns>
                                            <dx:ListBoxColumn FieldName="Code"/>
                                            <dx:ListBoxColumn FieldName="Title"/>
                                        </Columns>
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { OnCountryChanged(s); }"/>
                                    </dx:ASPxComboBox>
                              </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="RequestNo">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbRequestNo" ClientInstanceName="ClientRequestNo" Width="200px" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="TRF no.">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <%--<dx:ASPxTextBox ID="tbBrand" runat="server" ClientInstanceName="ClientBrand" Width="200px">
                                           <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" />
                                       </dx:ASPxTextBox>--%>
                                       <dx:ASPxButtonEdit ID="CmbTechnical" runat="server" ReadOnly="true" Width="200px" ClientInstanceName="CmbTechnical">
                                           <Buttons>
                                                <dx:EditButton>
                                                </dx:EditButton>
                                           </Buttons>
                                           <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('RequestNo',s); }" />
                                       </dx:ASPxButtonEdit>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                        <dx:LayoutItem Caption="Customer">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbCustomer" runat="server" ClientInstanceName="ClientCustomer">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" />
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                        <dx:LayoutItem Caption="Destination">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbDestination" runat="server" ClientInstanceName="ClientDestination" Width="200px">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" />
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                        <dx:LayoutItem Caption="RD PIC">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxButtonEdit ID="CmbReceiver" ClientInstanceName="ClientReceiver" Width="200px" ReadOnly="true" runat="server">
                                        <Buttons>
                                            <dx:EditButton>
                                            </dx:EditButton>
                                        </Buttons>
                                            <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Receiver',s); }" />
                                    </dx:ASPxButtonEdit>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                        <dx:LayoutItem Caption="Product Name">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbProduct" runat="server" ClientInstanceName="ClientProduct" Width="200px">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" />
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                        <%--<dx:LayoutItem Caption="RD Ref. No.">
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="ASPxTextBox4" runat="server" ClientInstanceName="ClientCustomer" Width="200px">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" />
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                            <dx:LayoutItem Caption="Date">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxDateEdit runat="server" ID="deDate" ClientInstanceName="ClientDate" Width="200px">
                                        <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" />
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>--%>
                        <dx:LayoutItem Caption="Date" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                 <dx:ASPxDateEdit runat="server" ID="deto" ClientInstanceName="ClientValidto" DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                            </dx:ASPxDateEdit> 
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Scheduled Process">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbScheduledProcess" runat="server" ClientInstanceName="ClientScheduledProcess" Width="200px">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" />
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                           <dx:LayoutItem Caption="Product Style">
                               <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                              <dx:ASPxTextBox ID="tbProductStyle" runat="server" TextField="Value" ValueField="Value" Width="200px" 
                                  ClientInstanceName="ClientProductStyle">
                              </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="NW (g.)">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <table>
                                           <tr>
                                               <td><dx:ASPxTextBox ID="tbNetweight" runat="server" ClientInstanceName="ClientNetweight" Width="100px">
                                                   <ClientSideEvents  ValueChanged="OnNetweight" />
                                                   <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" />
                                                   </dx:ASPxTextBox></td>
                                               <td>&nbsp;</td>
                                               <td><dx:ASPxComboBox ID="CmbUnit" runat="server" ClientInstanceName="ClientUnit" 
                                                   NullText="Select Unit..."  Width="96px">
                                                    <ClearButton DisplayMode="OnHover"></ClearButton>
                                                   <Items>
                                                      <dx:ListEditItem Text="Grams" Value="1" />
                                                      <dx:ListEditItem Text="Ounces" Value="2" />
                                                      <dx:ListEditItem Text="Lbs" Value="3" />
                                                      <dx:ListEditItem Text="KG" Value="4" />
                                                   </Items>
                                                   </dx:ASPxComboBox></td>
                                           </tr>
                                       </table>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>   
                           <dx:LayoutItem Caption="FW (g.)">
                               <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                              <dx:ASPxTextBox ID="tbFW" runat="server" TextField="Value" ValueField="Value" 
                                  ClientInstanceName="ClientFW">
                                  <ClientSideEvents ValueChanged="function(s, e){
                                      grid.PerformCallback('changed|' + s.GetText());
                                      }" />
                                  <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" />
                              </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="RD Ref. No." >
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="tbReference" ClientInstanceName="ClientReference" runat="server"  Width="200px">
                                        <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" />
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Packaging" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxButtonEdit ID="CmbPackaging" runat="server" ReadOnly="true" Width="200px" ClientInstanceName="CmbPackaging">
                                           <Buttons>
                                                <dx:EditButton>
                                                </dx:EditButton>
                                           </Buttons>
                                           <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Primary',s); }" />
                                       </dx:ASPxButtonEdit>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>     
                        <dx:LayoutItem Caption="Sample Quantity">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbRevised" ClientInstanceName="ClientRevised" 
                                         Width="200px">
                                        <ClientSideEvents ValueChanged="function(s, e){
                                      grid.PerformCallback('changed|' + s.GetText());
                                      }" />
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" />
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Notes" Width="100%">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxMemo runat="server" ID="mNotes" Rows="4" Native="True" ClientInstanceName="ClientNotes">
                                    </dx:ASPxMemo>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem> 
                    </Items>
             </dx:LayoutGroup>
            
            </Items>
            </dx:ASPxFormLayout>
                        </dx:SplitterContentControl>
                    </ContentCollection>
                </dx:SplitterPane>
                <dx:SplitterPane Name="editorsContainer" MinSize="100" ShowCollapseForwardButton="True" AutoHeight="true" PaneStyle-Border-BorderWidth="0px">
                    <ContentCollection>
                    <dx:SplitterContentControl runat="server">
<dx:ASPxFormLayout runat="server" ID="ASPxFormLayout1" ClientInstanceName="formLayout" CssClass="formLayout" AlignItemCaptionsInAllGroups="true">
                            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
                            <Items> 
                        <dx:LayoutItem Caption="">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer>
                            <dx:ASPxTabControl ID="TabList" runat="server" NameField="Id" DataSourceID="XmlDataSource1" ActiveTabIndex="0" ClientInstanceName="TabList">
                               <ClientSideEvents ActiveTabChanged="OnActiveTabChanged" Init="OnTabbedGroupPageControlInit"/>
                            </dx:ASPxTabControl>
                            <asp:XmlDataSource ID="XmlDataSource1" runat="server" DataFile="~/App_Data/Platforms.xml"
                                XPath="//Request"></asp:XmlDataSource>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>      
                   <dx:TabbedLayoutGroup Caption="" ActiveTabIndex="0" ClientInstanceName="tabbedGroupPageControl" ShowGroupDecoration="false" Width="100%">   
                       <Items>
                <dx:LayoutGroup Caption="" GroupBoxDecoration="None" ColCount="3" >
                    <Paddings PaddingTop="10px"></Paddings>
                           <GridSettings StretchLastItem="true" WrapCaptionAtWidth="660">
                           <Breakpoints>
                                <dx:LayoutBreakpoint MaxWidth="500" ColumnCount="1" Name="S" />
                                <dx:LayoutBreakpoint MaxWidth="800" ColumnCount="2" Name="M" />
                           </Breakpoints>
                           </GridSettings>
                <Items>
                <dx:LayoutItem Caption="" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer>
                         <%--<dx:ASPxMenu ID="ASPxMenu1" runat="server" CssClass="ActionMenu" SeparatorWidth="0">
                                <Items>
                                    <dx:MenuItem Text="New" Name="new" Image-Url="~/Content/Images/AddRecord.gif"/>
                                    <dx:MenuItem Text="Delete" Name="delete" Image-Url="~/Content/Images/Remove.gif" />
                                    <dx:MenuItem Text="Export to" BeginGroup="true" Image-Url="~/Content/Images/if_sign-out_59204.png">
                                        <Items>
                                            <dx:MenuItem Name="ExportToXLS" Text="XLS" Image-Url="~/Content/Images/excel.gif"/>
                                        </Items>
                                    </dx:MenuItem>
                                </Items>
                                <Border BorderWidth="0" />
                                <ClientSideEvents ItemClick="OnItemClick" />
                            </dx:ASPxMenu>--%>
                            <dx:ASPxGridView ID="grid" runat="server" AutoGenerateColumns="true" OnDataBinding="grid_DataBinding"
                                OnCustomCallback="grid_CustomCallback" OnBatchUpdate="grid_BatchUpdate" OnCustomDataCallback="grid_CustomDataCallback"
                                OnCustomSummaryCalculate="grid_CustomSummaryCalculate" OnDetailRowGetButtonVisibility="grid_DetailRowGetButtonVisibility"
                                SettingsPager-PageSize="60" EnableRowsCache="false" OnCellEditorInitialize="grid_CellEditorInitialize"
                                KeyFieldName="Id" ClientInstanceName="grid" Width="100%">    
                                <Toolbars>
                                    <dx:GridViewToolbar Name="toolbar">
                                        <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                                        <Items>
                                            <dx:GridViewToolbarItem Command="New" Name="New" />
                                            <dx:GridViewToolbarItem Command="Edit" />
                                            <dx:GridViewToolbarItem Text="Remove" Name="Remove" Image-Url="~/Content/Images/Cancel.gif" />
                                            <dx:GridViewToolbarItem Text="Undo" ItemStyle-Width="100px" Name="Undo" Image-IconID="actions_resetchanges_16x16devav" />
                                            <%--<dx:GridViewToolbarItem Text="|" />--%> 
                                            <dx:GridViewToolbarItem Text="Request" Name="Request" Image-IconID="actions_apply_16x16" ClientVisible="false"/>
                                            <dx:GridViewToolbarItem Text="Reject" Name="Reject" Image-IconID="actions_close_16x16" ClientVisible="false"/>
                                        </Items>
                                    </dx:GridViewToolbar>
                                </Toolbars>
                                <styles>
                                    <focusedrow BackColor="#f4dc7a" ForeColor="Black"></focusedrow>
                                    <FixedColumn BackColor="LightYellow"></FixedColumn>
                                    <Row Cursor="pointer" />
                                </styles>
                                <ClientSideEvents BatchEditEndEditing="OnBatchEditEndEditing" BatchEditStartEditing="OnBatchEditStartEditing" CustomButtonClick="OnCustomButtonClick" 
                                    ContextMenuItemClick="function(s,e) { OnContextMenuItemClick(s, e); }" ToolbarItemClick="OnToolbarItemClick"/>
                                <Columns>
                                    <dx:GridViewCommandColumn ShowNewButton="true" ShowEditButton="true" VisibleIndex="0" ButtonRenderMode="Image" Width="45">
                                        <CustomButtons>
                                                <dx:GridViewCommandColumnCustomButton ID="Insert">
                                                    <Image ToolTip="Add" Url="~/Content/Images/AddRecord.gif"/>
                                                </dx:GridViewCommandColumnCustomButton>
                                            </CustomButtons>
                                        </dx:GridViewCommandColumn>
                                    <dx:GridViewDataColumn FieldName="Id" ReadOnly="true" Width="0px"/>
                                    <dx:GridViewDataComboBoxColumn FieldName="Component" GroupIndex="1">
                                        <PropertiesComboBox DataSourceID="dsComponent" TextField="Name" ValueField="Name" />
                                    </dx:GridViewDataComboBoxColumn>
                                    <%--<dx:GridViewDataComboBoxColumn FieldName="SubType">
                                    </dx:GridViewDataComboBoxColumn>--%>
                                    <dx:GridViewDataColumn FieldName="Description" Width="130px"/>
                                    <dx:GridViewDataColumn FieldName="Note" Width="130px"/>
                                    <dx:GridViewDataColumn FieldName="IDNumber" Width="130px"/>
                                    <%-- <dx:GridViewDataComboBoxColumn FieldName="SAPMaterial" UnboundType="String">
                                       <PropertiesComboBox ValueField="RawMaterial" DataSourceID="dsSAPMaterial" EnableCallbackMode="true" TextFormatString="{0}" 
                                            DropDownWidth="230px" CallbackPageSize="10" DropDownStyle="DropDown">
                                            <ClientSideEvents SelectedIndexChanged="Combo_SelectedIndexChanged"/>
                                            <Columns>
                                                <dx:ListBoxColumn FieldName="RawMaterial" />
                                                <dx:ListBoxColumn FieldName="Description" />
                                            </Columns>
                                        </PropertiesComboBox>
                                        <Settings AutoFilterCondition="Contains" FilterMode="DisplayText" />
                                    </dx:GridViewDataComboBoxColumn>--%>
                                    <dx:GridViewDataButtonEditColumn FieldName="Material" UnboundType="String">
                                            <PropertiesButtonEdit>
                                                <Buttons>
                                                    <dx:EditButton />
                                                </Buttons>
                                                <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Matformu',s); }" />
                                            </PropertiesButtonEdit>
                                        </dx:GridViewDataButtonEditColumn>
                                    <dx:GridViewDataTextColumn FieldName="Yield" Caption="%Yield" Width="30px"/>
                                        <%--<dx:GridViewDataComboBoxColumn FieldName="RawMaterial" Width="200px">
                                            <PropertiesComboBox EnableCallbackMode="true" CallbackPageSize="10" 
                                               OnItemsRequestedByFilterCondition="OnItemsRequestedByFilterCondition" TextFormatString="{0}"  
                                                ValueField="Material" DropDownStyle="DropDown">
                                                <Columns>                                            
                                                    <dx:ListBoxColumn FieldName="Material" />
                                                    <dx:ListBoxColumn FieldName="Name" />
                                                    <dx:ListBoxColumn FieldName="Yield" />
                                                </Columns>
                                                <ClientSideEvents SelectedIndexChanged="OnSelectedIndexChanged" />
                                            </PropertiesComboBox>
                                            <Settings AutoFilterCondition="Contains" FilterMode="DisplayText" />
                                        </dx:GridViewDataComboBoxColumn>--%>
                                        <dx:GridViewDataTextColumn FieldName="Result" Caption="g./container" Width="80px"/>
                                        <%--<dx:GridViewBandColumn Caption="(gm./can)">
                                            <Columns>
                                        <dx:GridViewDataTextColumn FieldName="Result"/>
                                        </Columns>
                                    </dx:GridViewBandColumn>--%>
                                    <dx:GridViewDataTextColumn FieldName="NW" Caption= "% by NW" Width="80px">
                                        <PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
                                        <PropertiesTextEdit DisplayFormatString="F3" />
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Portion" Caption= "% Portion" Width="80px"/>
                                    <dx:GridViewDataTextColumn FieldName="Batch" Caption= "Batch(g)"/>
                                    <dx:GridViewDataTextColumn FieldName="ByPercent" Caption="Quantity" />
                                </Columns>
                                <TotalSummary>
                                    <dx:ASPxSummaryItem FieldName="Result" SummaryType="Sum" />
                                    
                                    <dx:ASPxSummaryItem FieldName="NW" SummaryType="Sum" ShowInColumn="Component" />
                                    <dx:ASPxSummaryItem FieldName="Portion" SummaryType="Sum" />
                                    <dx:ASPxSummaryItem FieldName="Batch" SummaryType="Sum" />
                                </TotalSummary>
                                <GroupSummary>
                                        <dx:ASPxSummaryItem FieldName="Batch" ShowInGroupFooterColumn="Batch" SummaryType="Sum" />
                                        <dx:ASPxSummaryItem FieldName="NW" ShowInGroupFooterColumn="NW" SummaryType="Sum" ShowInColumn="Component" />
                                        <%--<dx:ASPxSummaryItem FieldName="NW" ShowInColumn="NW" SummaryType="Count" ShowInGroupFooterColumn="Component" DisplayFormat="n0" />--%>
                                    </GroupSummary>
                                <SettingsDetail ShowDetailRow="True" />
                                <Templates>
                                    <DetailRow>
                                        <dx:ASPxGridView ID="detail" runat="server" AutoGenerateColumns="False" ClientInstanceName="detail"  OnCustomButtonCallback="detail_CustomButtonCallback"
                                            OnBatchUpdate="detail_BatchUpdate" OnCustomDataCallback="detail_CustomDataCallback" OnCustomCallback="detail_CustomCallback"
                                            KeyFieldName="Id" Width="100%" OnBeforePerformDataSelect="detail_BeforePerformDataSelect">
                                            <ClientSideEvents BatchEditEndEditing="OnBatchEditEndEditing" BatchEditStartEditing="OnBatchEditStartEditing"/>
						                    <Columns>
                                                <dx:GridViewCommandColumn ShowNewButton="true" ShowEditButton="true" VisibleIndex="0" ButtonRenderMode="Image" Width="45">
                                                    <CustomButtons>
                                                        <dx:GridViewCommandColumnCustomButton ID="Remove">
                                                            <Image ToolTip="Remove" Url="~/Content/Images/Cancel.gif"/>
                                                        </dx:GridViewCommandColumnCustomButton>
                                                    </CustomButtons>
                                                </dx:GridViewCommandColumn>
							                    <dx:GridViewDataTextColumn FieldName="Id" ReadOnly="True" VisibleIndex="1" Visible="false" />
                                               <dx:GridViewDataComboBoxColumn FieldName="Component" Width="0px">
                                                    <PropertiesComboBox DataSourceID="dsComponent" TextField="Name" ValueField="Name" />
                                                </dx:GridViewDataComboBoxColumn>
                                                <%--<dx:GridViewDataComboBoxColumn FieldName="SubType" Width="0px">
                                                </dx:GridViewDataComboBoxColumn>--%>
                                                <dx:GridViewDataColumn FieldName="Description" Width="130px"/>
                                                <dx:GridViewDataColumn FieldName="Note" Width="130px"/>
                                                <dx:GridViewDataColumn FieldName="IDNumber" Width="130px"/>
                                                <dx:GridViewDataTextColumn FieldName="ParentID" VisibleIndex="1" Width="0px"/>
                                                <%--<dx:GridViewDataComboBoxColumn FieldName="Material" VisibleIndex="2">
                                                    <PropertiesComboBox DataSourceID="dsMaterial" TextField="Name" ValueField="ID">
                                                    <Columns>
                                                        <dx:ListBoxColumn FieldName="ID" />
                                                        <dx:ListBoxColumn FieldName="Name" />
                                                    </Columns>
                                                    </PropertiesComboBox>
                                                </dx:GridViewDataComboBoxColumn>--%>
                                                <dx:GridViewDataButtonEditColumn FieldName="Material" UnboundType="String">
                                                    <PropertiesButtonEdit>
                                                        <Buttons>
                                                            <dx:EditButton />
                                                        </Buttons>
                                                        <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Matdetail',s); }" />
                                                    </PropertiesButtonEdit>
                                                </dx:GridViewDataButtonEditColumn>
							                    <dx:GridViewDataTextColumn FieldName="Yield" Caption="%Yield"/>
                                                <dx:GridViewDataTextColumn FieldName="Result" Caption="g./container"/>
                                                <dx:GridViewDataTextColumn FieldName="NW" Caption= "% by NW">
                                                    <PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
                                                    <PropertiesTextEdit DisplayFormatString="F3" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Portion" Caption= "% Portion"/>
                                                <dx:GridViewDataTextColumn FieldName="Batch" Caption= "Batch(g)"/>
                                                <dx:GridViewDataTextColumn FieldName="ByPercent" Caption="Quantity"/>
						                    </Columns>
                                            <SettingsEditing Mode="Batch"/>
                                            <Settings VerticalScrollBarMode="Auto" HorizontalScrollBarMode="Auto" GridLines="Vertical" ShowGroupPanel="True"
                                                ShowStatusBar="Hidden" ShowFooter="true" 
                                                ShowGroupFooter="VisibleIfExpanded" />  
					                    </dx:ASPxGridView>
                                    </DetailRow>
                                </Templates>

                                <SettingsSearchPanel ColumnNames="" Visible="false" />
		                        <Settings VerticalScrollBarMode="Auto" HorizontalScrollBarMode="Auto" GridLines="Vertical" ShowGroupPanel="True"
                                        ShowStatusBar="Hidden" ShowFooter="true" 
                                        ShowGroupFooter="VisibleIfExpanded"/>
                                <SettingsBehavior AllowSort="false" AllowGroup="false" AllowFocusedRow="True" AutoExpandAllGroups="true" 
                                        EnableRowHotTrack="True" ColumnResizeMode="Control" />
		                            <SettingsPager AlwaysShowPager="true" />
                                <SettingsEditing Mode="Batch">
                                    <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="Cell"/>
                                </SettingsEditing>
                            </dx:ASPxGridView>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
            </Items>
            </dx:LayoutGroup>
            <dx:LayoutGroup Caption="Attached file" ColCount="3" GroupBoxDecoration="None" UseDefaultPaddings="false">
                <Paddings PaddingTop="10px"></Paddings>
                <GridSettings StretchLastItem="true" WrapCaptionAtWidth="660">
                <Breakpoints>
                    <dx:LayoutBreakpoint MaxWidth="500" ColumnCount="1" Name="S" />
                    <dx:LayoutBreakpoint MaxWidth="800" ColumnCount="2" Name="M" />
                </Breakpoints>
                </GridSettings>
                <Items>
                    <dx:LayoutItem Caption="" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxCallbackPanel ID="cpUpdateFiles" runat="server" Width="100%" ClientInstanceName="cpUpdateFiles" 
                                    OnCallback="cpUpdateFiles_Callback">
                                    <PanelCollection>
                                    <dx:PanelContent runat="server">
                                    <%--<asp:GridView ID="gvFiles" runat="server" AutoGenerateColumns="false" Class="table table-striped table-bordered table-hover">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkRow" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Text" HeaderText="File Name"/>    
                                             <asp:TemplateField>    
                                                <ItemTemplate>    
                                                    <asp:LinkButton ID="DownloadLink" runat="server" Text="Download" OnClick="DownloadData" CommandArgument='<%# Eval("Value") %>'>
                                                        <img src="~/Content/Image/icons8-attach-16.png" /></asp:LinkButton>    
                                                </ItemTemplate>    
                                            </asp:TemplateField>    
                                            <asp:TemplateField>    
                                                <ItemTemplate>    
                                                    <asp:LinkButton ID="DeleteLink" runat="server" Text="Delete" OnClick="DeleteData" CommandArgument=' <%# Eval("value") %>'>
                                                        <img src="~/Content/Image/Cancel.gif" /></asp:LinkButton>    
                                                </ItemTemplate>    
                                            </asp:TemplateField> 
                                        </Columns>
                                    </asp:GridView>--%>
                                    <dx:ASPxFileManager ID="fileManager" runat="server" CustomFileSystemProviderTypeName="FileStreamProvider" Height="260px" 
                                        ClientInstanceName="fileManager">
                                        <SettingsDataSource KeyFieldName="ID" ParentKeyFieldName="ParentID" NameFieldName="Name" IsFolderFieldName="IsFolder" FileBinaryContentFieldName="Data" />
                                        <SettingsEditing AllowCreate="True" AllowDelete="True" AllowMove="True" AllowRename="True" AllowDownload="true"/>
                                        <Settings RootFolder="Available Files"/>
                                        <SettingsUpload UseAdvancedUploadMode="true" Enabled="true">
                                            <AdvancedModeSettings EnableMultiSelect="true"  />
                                            <ValidationSettings 
                                                MaxFileSize="10000000" 
                                                MaxFileSizeErrorText="The file you are trying to upload is larger than what is allowed (10 MB).">
                                            </ValidationSettings>
                                        </SettingsUpload>
					                    <Settings EnableMultiSelect="true" />
                                    </dx:ASPxFileManager>
                                    <%--<dx:ASPxFileManager ID="fileManager" runat="server" Height="240px" CustomFileSystemProviderTypeName="FileStreamProvider" OnFolderCreating="OnFolderCreating" 
                                        OnFileUploading="OnFileUploading" OnItemDeleting="OnItemDeleting" 
                                        ClientInstanceName="fileManager">
                                        <ClientSideEvents FileUploaded="OnFileUploaded" FilesUploading="uploadStarted" SelectedFileOpened="function(s, e) {e.file.Download();e.processOnServer = false;}" />
                                        <Settings ThumbnailFolder="~\Thumb\" RootFolder="a1" 
                                            AllowedFileExtensions=".rtf, .pdf, .doc, .docx, .odt, .txt, .xls, .xlsx, .xlsb, .ods, .ppt, .pptx, .odp, .jpe, .jpeg, .jpg, .gif, .png , .msg"/>
                                        <SettingsFileList View="Thumbnails">
                                            <ThumbnailsViewSettings ThumbnailWidth="100" ThumbnailHeight="100" />
                                        </SettingsFileList>
                                        <SettingsToolbar ShowPath="false" ShowRefreshButton="false" />
                                        <SettingsDataSource KeyFieldName="ID" ParentKeyFieldName="ParentID" NameFieldName="Name" IsFolderFieldName="IsFolder" 
                                            FileBinaryContentFieldName="Data" LastWriteTimeFieldName="LastWriteTime" />
                                        <SettingsEditing AllowCreate="false" AllowDelete="true" AllowMove="false" AllowRename="false" AllowDownload="true" />
                                        <SettingsBreadcrumbs Visible="true" ShowParentFolderButton="true" Position="Top" />
                                        <SettingsUpload UseAdvancedUploadMode="true" Enabled="true">
                                            <AdvancedModeSettings EnableMultiSelect="true"  />
                                            <ValidationSettings 
                                                MaxFileSize="10000000" 
                                                MaxFileSizeErrorText="The file you are trying to upload is larger than what is allowed (10 MB).">
                                            </ValidationSettings>
                                        </SettingsUpload>
					                <Settings EnableMultiSelect="false" />
                                    <ClientSideEvents SelectedFileChanged="OnSelectedFileChanged" />
                                    </dx:ASPxFileManager>--%>
                                    <%--<dx:ASPxPopupControl AllowResize="true" ID="PopupWithDocument" ClientInstanceName="PopupWithDocument" runat="server" 
                                        OnWindowCallback="PopupWithDocument_WindowCallback" PopupHorizontalAlign="WindowCenter" AllowDragging="true" CloseAction="CloseButton" >
                                       <ClientSideEvents  EndCallback="OnEndCallback" />
                                         <ContentCollection>
                                            <dx:PopupControlContentControl runat="server">
                                                <dx:ASPxSpreadsheet ID="ASPxSpreadsheet1" runat="server" Visible="false"></dx:ASPxSpreadsheet>
                                                <dx:ASPxRichEdit ID="ASPxRichEdit1" runat="server" Visible="false"></dx:ASPxRichEdit>
                                            </dx:PopupControlContentControl>
                                        </ContentCollection>
                                    </dx:ASPxPopupControl>--%>
                                    </dx:PanelContent>
                                    </PanelCollection>
				                    </dx:ASPxCallbackPanel>
                                   <br />
                                    <p class="note">
                                        <dx:ASPxLabel ID="AllowedFileExtensionsLabel" runat="server" Text="Allowed file extensions: .jpg, .jpeg, .gif, .png, .xls, .xlsx, .pdf." Font-Size="8pt">
                                        </dx:ASPxLabel>
                                        <br />
                                        <dx:ASPxLabel ID="MaxFileSizeLabel" runat="server" Text="Maximum file size: 10 MB." Font-Size="8pt">
                                        </dx:ASPxLabel>
                                    </p>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                </Items>
                </dx:LayoutGroup>
            </Items>
            </dx:TabbedLayoutGroup>
            <dx:LayoutGroup Caption="Status" ColCount="4" GroupBoxDecoration="HeadingLine" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                <GroupBoxStyle>
                    <Caption Font-Bold="true" Font-Size="10"/>
                </GroupBoxStyle>
                <Items>
                    <dx:LayoutItem Caption="Send Document" Width="100%">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                            <dx:ASPxComboBox ID="CmbDisponsition" runat="server" TextField="Title" ValueField="ID" OnCallback="CmbDisponsition_Callback"
                                Width="230px" ClientInstanceName="ClientDisponsition">
                                <ClientSideEvents  SelectedIndexChanged="function(s, e) { OnIDChanged(s); }" />
                            </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Reason for rejection" Name="Reason" ClientVisible="false">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ClientInstanceName="CmbReason" ID="CmbReason" runat="server" DataSourceID="dsReason" 
                                        Width="230px" ValueField="ID" TextField="Description">
                                    <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" />
                                </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Comment" Width="100%">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxMemo runat="server" ID="mComment" Rows="4" ClientInstanceName="ClientComment">
                                </dx:ASPxMemo>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem> 
                </Items>
            </dx:LayoutGroup>
                                </Items>
    </dx:ASPxFormLayout>
                        </dx:SplitterContentControl>
                    </ContentCollection>
                </dx:SplitterPane>
                </Panes>
                </dx:ASPxSplitter>
            </dx:PanelContent>

    </PanelCollection>
    </dx:ASPxCallbackPanel>
    <asp:SqlDataSource ID="dsCompany" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetPlant" SelectCommandType="StoredProcedure">
       <SelectParameters>
            <asp:ControlParameter ControlID="hfBU" Name="BU" PropertyName="['BU']" />
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
       </SelectParameters>

    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsRequester" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetReceiver" SelectCommandType="StoredProcedure">
                <SelectParameters>
            <asp:ControlParameter ControlID="username" Name="username" PropertyName="['user_name']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsReason" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasReason"/>
<%--    <asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spsummaryFormulaall" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="username" Name="user_name" PropertyName="['user_name']"/>
        </SelectParameters>
    </asp:SqlDataSource>--%>
    <asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from TransCusFormulaHeader">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsPrimary" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasPrimary"/>
    <asp:SqlDataSource ID="dsulogin" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select FirstName+'_'+LastName as fn,[user_name] from ulogin"/>
<%--    <asp:SqlDataSource ID="dsSAPMaterial" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetSAPMaterial" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Name="Company" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>--%>
<%--    <asp:SqlDataSource ID="dsMaterial" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetMaterial" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Name="Company" Type="String" />
            <asp:Parameter Name="RawMaterial" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>--%>
    <asp:SqlDataSource ID="dsMaterial" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasMatformu where Company=@Company">
        <SelectParameters>
                    <asp:Parameter Name="Company" Type="String" />
            </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsComponent" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="select * from MasCusProductStyle" />