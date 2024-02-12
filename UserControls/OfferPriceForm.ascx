<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OfferPriceForm.ascx.cs" Inherits="UserControls_OfferPriceForm" %>
    <script type="text/javascript">
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
        function OnCountryChanged(s) {

        }
        function OnCustomer(s, e) {

        }
        function OnValueChanged(a, text) {
        }
        function Oninterest(s, e) {

        }
        function OnSelectedIndexChanged(s, e) {

        }
    function OnGetRowTest(values) {
        var r = confirm("Are you copy sure ?");
        if (r == true) {
            var para = hfpara.Get("Copied");
            //alert(para);
            gridData.PerformCallback("Copied|" + values);
        }
    }
    function OnCustomButtonClick(s, e) {
        //debugger;
        var keyValue = gridData.GetRowKey(e.visibleIndex);
        OnGetRowTest(keyValue);
        }
    function OnContextMenuItemClick(sender, args) {
        //debugger;
        if (args.objectType == "row") {
            args.processOnServer = false;
            var key = sender.GetRowKey(args.elementIndex);
            if (args.item.name == "Copied") {
                OnGetRowTest(key);
            }
        }
    }
   </script>
<dx:ASPxHiddenField ID="hfpara" runat="server" ClientInstanceName="hfpara" />
<dx:ASPxHiddenField ID="hfid" runat="server" ClientInstanceName="hfid" />
<dx:ASPxHiddenField ID="hftype" runat="server" ClientInstanceName="hftype" />
<dx:ASPxHiddenField ID="username" runat="server" ClientInstanceName="username" />
<dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
<dx:ASPxHiddenField ID="approv" runat="server" ClientInstanceName="approv" />
<dx:ASPxGridView runat="server" ID="gridData" KeyFieldName="ID" DataSourceID="dsgv" ClientInstanceName="gridData" Width="100%" EnableRowsCache="false"
    Border-BorderWidth="0">
    <ClientSideEvents RowClick="Demo.gridData_RowClick" />
    <Columns>
        <dx:GridViewDataTextColumn FieldName="ID" />
        <dx:GridViewDataTextColumn FieldName="RequestNo" Caption="Request No"/>
        <dx:GridViewDataComboBoxColumn FieldName="StatusApp" GroupIndex="0">
            <PropertiesComboBox DataSourceID="dsStatusApp" TextField="Title" ValueField="ID"/>
        </dx:GridViewDataComboBoxColumn>
    </Columns>
    <SettingsSearchPanel ColumnNames="" Visible="false" />
		<Settings ShowGroupPanel="True" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="true" />
		<SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true"/>
		    <SettingsPager PageSize="50">
        <PageSizeItemSettings Visible="true" ShowAllItem="true"  AllItemText="All Records" />
        </SettingsPager>
		<Styles>
			<Row Cursor="pointer" />
		</Styles>
        <SettingsContextMenu Enabled="true" />
        <ClientSideEvents 
            ContextMenuItemClick="function(s,e) { OnContextMenuItemClick(s, e); }"
            RowClick="Demo.gridData_RowClick"
            CustomButtonClick="OnCustomButtonClick"
            EndCallback="Demo.Test"
            Init="Demo.gridData_Init"/>
        <TotalSummary>
            <dx:ASPxSummaryItem FieldName="RequestNo" SummaryType="Count" />
        </TotalSummary>
</dx:ASPxGridView>
<dx:ASPxGridViewExporter runat="server" ID="GridExporter" GridViewID="gridData" />
<%--    <dx:ASPxCallbackPanel ID="PreviewPanel" runat="server" RenderMode="Div" Height="100%" CssClass="PreviewPanel" ClientInstanceName="ClientPreviewPanel" 
        ClientVisible="false"/>
    <dx:ASPxCallbackPanel ID="PreviewPanel" runat="server" RenderMode="Div" Height="100%" CssClass="PreviewPanel" ClientInstanceName="ClientPreviewPanel" 
        ClientVisible="false" OnCallback="PreviewPanel_Callback" />--%>
    <dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" ClientVisible="false" ScrollBars="Vertical" ClientInstanceName="ClientFormPanel">
        <ClientSideEvents Init="Demo.Init" />
        <PanelCollection>
            <dx:PanelContent>
            <dx:ASPxFormLayout runat="server" ID="formLayout" ClientInstanceName="formLayout" CssClass="formLayout" >
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
            <Items>
                <dx:LayoutGroup Caption="" ColCount="3" GroupBoxDecoration="HeadingLine" UseDefaultPaddings="false">
                    <Paddings PaddingTop="10px"></Paddings>
                    <GroupBoxStyle>
                        <Caption Font-Bold="true" Font-Size="10"/>
                    </GroupBoxStyle>
                    <Items>
                    <dx:LayoutItem Caption="Company Code">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                            <dx:ASPxComboBox ID="CmbCompany" runat="server" DataSourceID="dsCompany" ValueField="Code" Width="200px"
                                    ClientInstanceName="ClientCompany" TextFormatString="{0}" OnCallback="CmbCompany_Callback">
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="Code"/>
                                        <dx:ListBoxColumn FieldName="Name"/>
                                    </Columns>
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { OnCountryChanged(s); }"/>
                                    <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Costing Sheet No">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox runat="server" ID="CMbCostingNo" ClientInstanceName="ClientCostingNo" Width="200px" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Customer">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbCustomer" runat="server" ClientInstanceName="ClientCustomer" ValueField="Code" OnCallback="CmbCustomer_Callback"
                                    EnableCallbackMode="true" CallbackPageSize="10" DataSourceID="dsCustomer" TextFormatString="{0};{1}" Width="200px">   
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="Code"/>
                                        <dx:ListBoxColumn FieldName="Name" Width="240px"/>
                                        <dx:ListBoxColumn FieldName="TermOfPayment" Width="240px"/>
                                    </Columns>
                                    <ClientSideEvents SelectedIndexChanged="OnCustomer" />
                                </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Ship-To">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbShipTo" runat="server" ClientInstanceName="ClientShipTo" DataSourceID="dsCustomer" ValueField="Code" OnCallback="CmbShipTo_Callback" 
                                    EnableCallbackMode="true" CallbackPageSize="10" TextFormatString="{0};{1}" Width="200px">   
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="Code"/>
                                        <dx:ListBoxColumn FieldName="Name" Width="240px"/>
                                    </Columns>
                                </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Payment Term" Name="PaymentTerm">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbPaymentTerm" runat="server" ClientInstanceName="ClientPaymentTerm" 
                                    EnableCallbackMode="true" CallbackPageSize="10" ValueField="Code"
                                    DataSourceID="dsPaymentTerm" TextFormatString="{0};{1}" Width="200px">   
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="Code"/>
                                        <dx:ListBoxColumn FieldName="Value" Width="240px"/>
                                    </Columns>
                                </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Actual interest">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                     <table>
                                           <tr>
                                               <td>
                                                   <dx:ASPxTextBox runat="server" ID="tbinterest" ClientInstanceName="Clientinterest" Width="50px">
                                                       <ClientSideEvents TextChanged="function(s, e) { Oninterest(s,e);}" />
                                                   </dx:ASPxTextBox></td>
                                               <td>&nbsp;</td>
                                               <td>%</td>
                                    </tr>
                                    </table>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Commission" Name="Commission">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <table>
                                    <tr>
                                        <td>
                                        <dx:ASPxComboBox runat="server" ID="CmbCommission" ClientInstanceName="ClientCommission" TextFormatString="{0}" 
                                        DataSourceID="dsCommission" TextField="value" ValueField="value"        
                                        NullText="Example : xx %" Width="120px">
                                        <ClientSideEvents ValueChanged="function(s, e) { OnValueChanged('sub',tbNumberContainer.GetText());}" />
                                    </dx:ASPxComboBox></td>
                                        <td>&nbsp;</td>
                                        <td>%</td>
                                    </tr>
                                    </table>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem> 
                        <dx:LayoutItem Caption="Incoterm" Name="Incoterm">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbIncoterm" runat="server" ClientInstanceName="CmbIncoterm" DataSourceID="dsIncoterm"
                                    ValueField="Code" TextFormatString="{0};{1}" Width="200px">
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="Code"/>
                                        <dx:ListBoxColumn FieldName="Name"/>
                                    </Columns>
                                    <ClientSideEvents SelectedIndexChanged="OnSelectedIndexChanged" />
                                </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>                       
                        <dx:LayoutItem Caption="Route">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="CmbRoute" runat="server" ClientInstanceName="ClientRoute" DataSourceID="dsRoute" 
                                        TextFormatString="{0};{1}" EnableCallbackMode="true" ValueField="Code" 
                                        CallbackPageSize="10" Width="200px">
                                        <Columns>
                                        <dx:ListBoxColumn FieldName="Code"/>
                                        <dx:ListBoxColumn FieldName="Name"/>
                                        </Columns>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Size">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="CmbSize" runat="server" ClientInstanceName="ClientSize" DataSourceID="dsContainerType" 
                                        TextFormatString="{0};{1}" ValueField="Code" Width="200px">
                                        <Columns>
                                        <dx:ListBoxColumn FieldName="Code"/>
                                        <dx:ListBoxColumn FieldName="Name"/>
                                        </Columns>
                                        <ClientSideEvents ValueChanged="function(s, e) {  
                                                        OnValueChanged('tot',tbNumberContainer.GetText());}"/>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Containers Qty" Name="TotalContainers">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxTextBox ID="tbNumberContainer" runat="server" ClientInstanceName="tbNumberContainer" Width="200px">
                                    <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                        ValueChanged="function(s, e) { OnValueChanged('calcu',s.GetText());}" />
                                </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Freight">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTextBox ID="tbFreight" runat="server" ClientInstanceName="tbFreight"
                                         Width="100px">
                                        <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                ValueChanged="function(s, e) { OnValueChanged('sub',tbNumberContainer.GetText());}" />
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="OverPrice">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTextBox ID="tbOverprice" runat="server" ClientInstanceName="ClientOverprice"
                                         Width="200px">
                                            <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                ValueChanged="function(s, e) { OnValueChanged('sub',tbNumberContainer.GetText());}" />
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Extra cost">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTextBox ID="tbExtracost" runat="server" ClientInstanceName="ClientExtracost" Width="200px">
                                            <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                ValueChanged="function(s, e) { OnValueChanged('sub',tbNumberContainer.GetText());}" />
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Insurance">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTextBox ID="tbInsurance" runat="server" ClientInstanceName="tbInsurance"
                                            BackColor="#F2F2F2" ForeColor="Black" ReadOnly="true" Width="100px">
                                            <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                ValueChanged="function(s, e) { OnValueChanged('sub',tbNumberContainer.GetText());}" />
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Remark" Width="100%">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxMemo runat="server" ID="mRemark" Rows="3" Width="60%" ClientInstanceName="ClientRemark"/>
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
<asp:SqlDataSource ID="dsStatusApp" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasStatusApp where levelapp in (1,2)"/>
<asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spGetQuotaHeader" SelectCommandType="StoredProcedure">
    <SelectParameters>
        <asp:ControlParameter ControlID="username" Name="user_name" PropertyName="['user_name']"/>
        <asp:ControlParameter ControlID="hftype" Name="type" PropertyName="['type']"/>
    </SelectParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsCompany" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spGetCompany" SelectCommandType="StoredProcedure">
    <SelectParameters>
        <asp:Parameter Name="ID" Type="String" />
        <asp:Parameter Name="BU" Type="String" />
    </SelectParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsCustomer" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spGetCustomer" SelectCommandType="StoredProcedure">
    <SelectParameters>
        <asp:Parameter Name="Company" />
        <asp:Parameter Name="Param" />
    </SelectParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsIncoterm" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from MasIncoterm"/>
<asp:SqlDataSource ID="dsPaymentTerm" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from MasPaymentTerm"/>
<asp:SqlDataSource ID="dsCommission" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select value from dbo.FNC_SPLIT('0,1,2',',')">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsinterest" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select value from dbo.FNC_SPLIT('0,6,4',',')">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsRoute" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from MasRoute">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsContainerType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from MasContainerType">
</asp:SqlDataSource>