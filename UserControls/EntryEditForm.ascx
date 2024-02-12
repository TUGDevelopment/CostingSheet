<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EntryEditForm.ascx.cs" Inherits="UserControls_EntryEditForm" %>
<script type="text/javascript">
        //function OnInit(s, e) {
        //    AdjustSize();
        //    document.getElementById("gridContainer").style.visibility = "";
        //}
        //function OnEndCallback(s, e) {
        //    AdjustSize();
        //}
        //function OnControlsInitialized(s, e) {
        //    ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
        //        AdjustSize();
        //    });
        //}
        //function AdjustSize() {
        //    var height = Math.max(0, document.documentElement.clientHeight);
        //    gridData.SetHeight(height);
        //}
        //function OnContextMenuItemClick(sender, args) {
        //    debugger;
        //    if (args.objectType == "row") {
        //        if (args.item.name == "ExportToPDF" || args.item.name == "ExportToXLS") {
        //            args.processOnServer = true;
        //            args.usePostBack = true;
        //        }
        //    }
        //}
        var curentEditingIndex;
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
                    if (args.item.name == "Revised")
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
        function completarArticulo(s) {
            var value = s.GetSelectedItem().GetColumnText(1);//s.GetText().toString();
            debugger;
            alert(value);
            grid.GetValuesOnCustomCallback("SAPMaterial|" + value, DataCallback);
        }
        function DataCallback(result) {
            var results = result.split("|");
            switch (results[0]) {
                case "SAPMaterial":
                    grid.batchEditApi.SetCellValue(curentEditingIndex, "Description", results[1]);
                    break;
            }
        }
        function OnBatchEditEndEditing(s, e) {
            debugger;
            window.setTimeout(function () {
                grid.UpdateEdit();
            }, 0);
        }
        function OnBatchEditStartEditing(s, e) {
            debugger;
            curentEditingIndex = e.visibleIndex;
        }
        function fn_AllowonlyNumeric(s, e) {
            var theEvent = e.htmlEvent || window.event;
            var key = theEvent.keyCode || theEvent.which;
            key = String.fromCharCode(key);
            var regex = /[0-9,]/;

            if (!regex.test(key)) {
                theEvent.returnValue = false;
                if (theEvent.preventDefault)
                    theEvent.preventDefault();
            }
        }
    </script>
<dx:ASPxHiddenField ID="username" runat="server" />
<dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
<dx:ASPxHiddenField ID="approv" runat="server" ClientInstanceName="approv" />
<div id="gridContainer" style="visibility: hidden"></div>
<dx:ASPxGridView runat="server" ID="gridData" KeyFieldName="ID" ClientInstanceName="gridData" Width="100%" EnableRowsCache="false"
        DataSourceID="dsgv"
        OnInit="gridData_Init" 
        OnFillContextMenuItems="gridData_FillContextMenuItems" OnContextMenuItemClick="gridData_ContextMenuItemClick"
        OnCustomDataCallback="gridData_CustomDataCallback" 
        Border-BorderWidth="0">
        <Columns>
        <dx:GridViewDataTextColumn FieldName="Company"  >
                <PropertiesTextEdit >
                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" />
            </PropertiesTextEdit>
        </dx:GridViewDataTextColumn>
        <dx:GridViewDataTextColumn FieldName="RequestNo" Caption="Request No" GroupIndex="0"/>
        <dx:GridViewDataTextColumn FieldName="Costing" Caption="Costing No" />
        <dx:GridViewDataTextColumn FieldName="RDNumber" Caption="RD Reference Number" />
        <dx:GridViewDataTextColumn FieldName="Material" Caption="Material" />
        </Columns>
        <SettingsSearchPanel ColumnNames="" Visible="false" />
		<Settings ShowGroupPanel="True" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFilterBar="Visible"/>
		<SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" />
		<SettingsPager Mode="ShowAllRecords" />
		<Styles>
			<Row Cursor="pointer" />
		</Styles>
        <SettingsContextMenu Enabled="true" />
        <ClientSideEvents Init="Demo.gridData_Init"
            RowClick="Demo.gridData_RowClick"
            EndCallback="Demo.Test" 
            ContextMenuItemClick="function(s,e) { OnContextMenuItemClick(s, e); }"/>
    </dx:ASPxGridView>
    <dx:ASPxGridViewExporter runat="server" ID="GridExporter" GridViewID="gridData" />
    <dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" ScrollBars="Vertical" ClientInstanceName="ClientFormPanel" ClientVisible="false">
        <ClientSideEvents Init="Demo.Init" />
        <PanelCollection>
            <dx:PanelContent>  
            <dx:ASPxFormLayout runat="server" ID="formLayout" CssClass="formLayout">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
            <Items>
                <dx:LayoutGroup Caption="" ColCount="3" GroupBoxDecoration="Box" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                    <Paddings PaddingTop="10px"></Paddings>
                    <GroupBoxStyle>
                        <Caption Font-Bold="true" Font-Size="10"/>
                    </GroupBoxStyle>
                    <Items>
                        <dx:LayoutItem Caption="Proposed Factory">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbCompany" runat="server" DataSourceID="dsPlant" ValueField="Code" Width="230px"
                                        ClientInstanceName="ClientCompany" TextFormatString="{0}" OnCallback="CmbCompany_Callback">
                                        <Columns>
                                            <dx:ListBoxColumn FieldName="Code" />
                                            <dx:ListBoxColumn FieldName="Title" />
                                            <dx:ListBoxColumn FieldName="Company" />
                                        </Columns>
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { OnCountryChanged(s); }"/>
                                    </dx:ASPxComboBox>
                              </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <%--<dx:LayoutItem Caption="Date">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxDateEdit runat="server" ID="deDate" ClientInstanceName="ClientDate" Width="230px">
                                        <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" />
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>--%>
                        <dx:LayoutItem Caption="Product Name">
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbProduct" runat="server" ClientInstanceName="ClientProduct" Width="230px">
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Product Code">
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbCode" runat="server" ClientInstanceName="ClientCode" Width="230px">
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Customer Name">
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbCustomer" runat="server" ClientInstanceName="ClientCustomer" Width="230px">
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                            <dx:LayoutItem Caption="Destination">
                                   <LayoutItemNestedControlCollection>
                                       <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                           <dx:ASPxTextBox ID="tbDestination" runat="server" ClientInstanceName="ClientDestination" Width="230px">
                                           </dx:ASPxTextBox>
                                       </dx:LayoutItemNestedControlContainer>
                                   </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                           <dx:LayoutItem Caption="Can Size">
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbCanSize" runat="server" ClientInstanceName="ClientCanSize" Width="230px">
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                          </dx:LayoutItem>
                          <dx:LayoutItem Caption="Net weight" ColSpan="2">
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <table>
                                           <tr>
                                               <td><dx:ASPxTextBox ID="tbNetweight" runat="server" ClientInstanceName="ClientNetweight" Width="130px">
                                                   <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}"/>
                                                   </dx:ASPxTextBox></td>
                                               <td>&nbsp;</td>
                                               <td><dx:ASPxComboBox ID="CmbNetUnit" runat="server" ClientInstanceName="ClientNetUnit" NullText="Select Unit..."  Width="96px">
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
                          <dx:LayoutItem Caption="RDNumber" >
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="tbRDNumber" ClientInstanceName="ClientRDNumber" runat="server" Width="230px">
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <%--<dx:LayoutItem Caption="Notes" Width="99%">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxMemo runat="server" ID="mNotes" Rows="2" Native="True" ClientInstanceName="ClientNotes">
                                    </dx:ASPxMemo>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>--%> 
                    </Items>
             </dx:LayoutGroup>
            <dx:LayoutGroup Caption="" GroupBoxDecoration="HeadingLine">
            <Items>
                <dx:LayoutItem>
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer>
                         <dx:ASPxMenu ID="ASPxMenu1" runat="server" CssClass="ActionMenu" SeparatorWidth="0">
                                <Items>
                                    <dx:MenuItem Text="New" Name="new" Image-Url="~/Content/Images/AddRecord.gif"/>
                                    <dx:MenuItem Text="Edit" Name="edit" Image-Url="~/Content/Images/Edit.gif"/>
                                    <dx:MenuItem Text="Delete" Name="delete" Image-Url="~/Content/Images/Remove.gif" />
                                    <dx:MenuItem Text="Export to" BeginGroup="true" Image-Url="~/Content/Images/if_sign-out_59204.png">
                                        <Items>
                                            <dx:MenuItem Name="ExportToPDF" Text="PDF" Image-Url="~/Content/Images/excel.gif"/>
                                        </Items>
                                    </dx:MenuItem>
                                </Items>
                                <Border BorderWidth="0" />
                                <ClientSideEvents ItemClick="OnItemClick" />
                            </dx:ASPxMenu>
                            <dx:ASPxGridView ID="grid" runat="server" AutoGenerateColumns="true" OnDataBinding="grid_DataBinding"
                                OnCustomCallback="grid_CustomCallback" OnCellEditorInitialize="grid_CellEditorInitialize" OnCustomDataCallback="grid_CustomDataCallback"
                                OnRowInserting="grid_RowInserting"
                                OnRowUpdating="grid_RowUpdating"
                                OnBatchUpdate="grid_BatchUpdate"
                                SettingsPager-PageSize="60" 
                                KeyFieldName="RowID" ClientInstanceName="grid" Width="100%">                                
                                <styles>
                                        <focusedrow BackColor="#f4dc7a" ForeColor="Black"></focusedrow>
                                        <FixedColumn BackColor="LightYellow"></FixedColumn>
                                        <Row Cursor="pointer" />
                                    </styles>
                                <SettingsSearchPanel ColumnNames="" Visible="false" />
		                        <Settings ShowVerticalScrollBar="True" VerticalScrollBarMode="Auto" HorizontalScrollBarMode="Auto" GridLines="Vertical" ShowStatusBar="Hidden" ShowFooter="true" 
                                        ShowGroupFooter="VisibleIfExpanded"/>
                                <SettingsBehavior AllowSort="false" AllowGroup="false" AllowFocusedRow="True" AutoExpandAllGroups="true" 
                                        EnableRowHotTrack="True" ColumnResizeMode="Control" />
		                            <SettingsPager AlwaysShowPager="true" />
                                <SettingsEditing Mode="Batch">
                                    <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="Cell"/>
                                </SettingsEditing>
                                <ClientSideEvents BatchEditEndEditing="OnBatchEditEndEditing" 
                                    BatchEditStartEditing="OnBatchEditStartEditing"/>  
                            </dx:ASPxGridView>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
            </Items>
            </dx:LayoutGroup>
            <dx:EmptyLayoutItem/>
            <dx:LayoutGroup Caption="Status" ColCount="3" GroupBoxDecoration="HeadingLine" UseDefaultPaddings="false" Paddings-PaddingTop="10">
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
            </dx:PanelContent>
    </PanelCollection>
    </dx:ASPxCallbackPanel>
    <asp:SqlDataSource ID="dsCompany" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasCompany order by Code" />
    <asp:SqlDataSource ID="dsYield" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="Select * From MasYield">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsRequester" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetReceiver" SelectCommandType="StoredProcedure">
                <SelectParameters>
            <asp:ControlParameter ControlID="username" Name="username" PropertyName="['user_name']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsReason" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasReason"/>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spsummaryFormulaall" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="username" Name="user_name" PropertyName="['user_name']"/>
        </SelectParameters>
    </asp:SqlDataSource>
   <asp:SqlDataSource ID="dsStatusApp" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasStatusApp"/>
    <asp:SqlDataSource ID="dsulogin" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select FirstName+'_'+LastName as fn,[user_name] from ulogin"/>
    <asp:SqlDataSource ID="dsRawMaterial" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetRawMaterial" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Name="Company" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>   
    <asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spmaterialreport" SelectCommandType="StoredProcedure">
<%--        <SelectParameters>
            <asp:Parameter Name="user_name" Type="String" DefaultValue=""/>
        </SelectParameters>--%>
    </asp:SqlDataSource>
   <asp:SqlDataSource ID="dsPlant" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasPlant"/>