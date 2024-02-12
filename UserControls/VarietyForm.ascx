<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VarietyForm.ascx.cs" Inherits="UserControls_VarietyForm" %>
<dx:ASPxGridView runat="server" ID="gridData" KeyFieldName="ID" ClientInstanceName="gridData" Width="100%" ClientVisible="false" />
<dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" ScrollBars="Vertical" ClientInstanceName="ClientCostFormPanel">
        <PanelCollection>
            <dx:PanelContent>  
            <dx:ASPxFormLayout runat="server" ID="formLayout" ClientInstanceName="formLayout" CssClass="formLayout" AlignItemCaptionsInAllGroups="true">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
            <Items>
                   <dx:LayoutGroup Caption="Variety" ColCount="4" GroupBoxDecoration="Box" UseDefaultPaddings="false" Paddings-PaddingTop="10">
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
                        <dx:LayoutItem Caption="Company Code" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbCompany" runat="server" DataSourceID="dsCompany" ValueField="Code" 
                                        ClientInstanceName="ClientCompany" TextFormatString="{0}"  >
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
                        <dx:LayoutItem Caption="Request No" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxButtonEdit ID="CmbCostingNo" ClientInstanceName="ClientCostingNo" ReadOnly="true"  
                                    Width="100%" runat="server">
                                        <Buttons>
                                            <dx:EditButton>
                                            </dx:EditButton>
                                        </Buttons>
                                            <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('RequestNo'); }" />
                                    </dx:ASPxButtonEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Costing Sheet No" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbMarketingNo" ClientInstanceName="ClientMarketingNo"  ReadOnly="true"/>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Exchange Rate" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxSpinEdit runat="server" ID="seExchangeRate" ClientInstanceName="seExchangeRate" NumberType="Float" 
                                        Number="0.00">
                          
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                    </dx:ASPxSpinEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Reference" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbReference" ClientInstanceName="ClientReference" MaxLength="250" >
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
			            </dx:LayoutItem>

                        <dx:LayoutItem Caption="From" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxDateEdit runat="server" ID="defrom" ClientInstanceName="ClientValidfrom" DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                  
                                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                            </dx:ASPxDateEdit> 
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>      
                        <dx:LayoutItem Caption="To" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                 <dx:ASPxDateEdit runat="server" ID="deto" ClientInstanceName="ClientValidto" DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                     
                                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                            </dx:ASPxDateEdit> 
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
			            <dx:LayoutItem Caption="Customer" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxTextBox runat="server" ID="tbCustomer" ClientInstanceName="tbCustomer" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                                                                     
                        <dx:LayoutItem Caption="Remark" Width="93.6%" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxMemo runat="server" ID="mNotes" Rows="4" ClientInstanceName="ClientNotes">
                                    </dx:ASPxMemo>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem> 
                    </Items>
                </dx:LayoutGroup>
               <dx:LayoutItem Caption="">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer>
                            <dx:ASPxTabControl ID="tcDemos" runat="server" NameField="Id" DataSourceID="XmlDataSource1" ActiveTabIndex="0" ClientInstanceName="tcDemos">
                               <ClientSideEvents ActiveTabChanged="function(s, e) { OnActiveTabChanged(s, e); }" Init="function(s, e) { s.SetActiveTabIndex(0); testgrid.SetVisible(false); }" />
                            </dx:ASPxTabControl>
                            <asp:XmlDataSource ID="XmlDataSource1" runat="server" DataFile="~/App_Data/Platforms.xml"
                                XPath="//product"></asp:XmlDataSource>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem> 
		        <dx:TabbedLayoutGroup Caption="SubTotal" ActiveTabIndex="0" ClientInstanceName="tabbedGroupPageControl" ShowGroupDecoration="false" Width="100%">   
                    <Items>
                        <dx:LayoutGroup Caption="" ColCount="3">
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
                    <dx:LayoutItem Caption="Upload file" Name="Upload" Width="100%" ClientVisible="false">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxUploadControl ID="Upload" runat="server"  Width="200" ClientInstanceName="Upload" 
                                    NullText="Click here to browse files..." UploadMode="Advanced" FileUploadMode="OnPageLoad" AutoStartUpload="True">
                                    <ValidationSettings AllowedFileExtensions=".xls,.xlsx">
                                    </ValidationSettings>
                           
                                </dx:ASPxUploadControl>
                                </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
		                <dx:LayoutItem Caption="Pack Size" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
				                <table>
				                <tr>
				                <td>
				                <dx:ASPxTextBox runat="server" ID="tbPackSize" ClientInstanceName="ClientPackSize" MaxLength="250" Width="100px">
                                    <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" TextChanged="function(s, e) { OnValueChanged(s,'pkg'); }" /> 
                                    </dx:ASPxTextBox>
				                    </td><td>&nbsp;</td>
                                        <td>
                                            <dx:ASPxComboBox runat="server" ID="CmbPackaging" ValueField="ID" TextField="Name" 
                                        IncrementalFilteringMode="StartsWith"
                                        ClientInstanceName="CmbPackaging" Width="96px">
                                        <ClientSideEvents ValueChanged="function(s, e) {  
                                                        //grid.PerformCallback('Labor|'+s.GetText());
                                                        //CmbMargin.PerformCallback('reload|'+s.GetText());
                                            }" />
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                    </dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                </table>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Can Size" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbCanSize" ClientInstanceName="tbCanSize">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem> 
                        <dx:LayoutItem Caption="Net weight" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <table>
                                           <tr>
                                               <td><dx:ASPxTextBox ID="tbNetweight" runat="server" ClientInstanceName="ClientNetweight" Width="100px">
                                                   <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
        
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
                                                   <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                                   </dx:ASPxComboBox></td>
                                           </tr>
                                       </table>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>      
                        </Items>
                        </dx:LayoutGroup>
		            <dx:LayoutGroup Caption="Primary Package" ColCount="3">
                    <Items>
                    <dx:LayoutItem Caption="Package Code">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxButtonEdit ID="CmbPackageCode" ClientInstanceName="ClientPackageCode"
                                    Width="100%" runat="server">
                                        <Buttons>
                                            <dx:EditButton>
                                            </dx:EditButton>
                                        </Buttons>
                                            <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('PackageCode'); }" />
                                    </dx:ASPxButtonEdit>
                                    <%--<dx:ASPxComboBox ID="CmbPackageCode" runat="server" ValueField="Material" DataSourceID="dsPackage"
                                     DropDownWidth="600px" EnableCallbackMode="true" TextFormatString="{0} ({1})" IncrementalFilteringMode="Contains" DropDownStyle="DropDown"
                                     CallbackPageSize="10"  ClientInstanceName="CmbPackageCode">
                                      <Columns>
                                            <dx:ListBoxColumn FieldName="Material"/>
                                            <dx:ListBoxColumn FieldName="Description" Width="230px"/>
                                            <dx:ListBoxColumn FieldName="Price"/>
                                            <dx:ListBoxColumn FieldName="Currency"/>
                                            <dx:ListBoxColumn FieldName="Unit"/>
                                        </Columns>
                                        <ClientSideEvents ValueChanged="function(s, e) { OnValueChanged(s,'prima'); }" />
                                    </dx:ASPxComboBox>--%>
                                </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Description">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbDescription" ClientInstanceName="tbDescription">
                                    </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Quantity">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <%--<dx:ASPxTextBox runat="server" ID="tbQuantity" ClientInstanceName="tbQuantity">
                                        <ClientSideEvents KeyPress="function(s,e) { ProcessKeyPress(s, e);}"
                                            TextChanged="function(s, e) { OnTextChangedHandler(s,'prima');}" />
                                    </dx:ASPxTextBox>--%>
                                    <dx:ASPxSpinEdit ID="tbQuantity" runat="server" Width="100" ClientInstanceName="tbQuantity"> 
                                                <ValidationSettings ErrorDisplayMode="Text" ErrorText="*" SetFocusOnError="true">
                                                    <%--<RequiredField IsRequired="true" />--%>
                                                </ValidationSettings>
                                                <SpinButtons ShowIncrementButtons="false" />
                                                <ClientSideEvents ValueChanged="function(s, e) { OnTextChangedHandler(s,'prima');}" />
                                            </dx:ASPxSpinEdit>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Amount">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbAmount" ClientInstanceName="tbAmount" DisplayFormatString="##,###.##" ReadOnly="true">
                                    </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Price/Rate">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbPriceRate" ClientInstanceName="tbPriceRate" DisplayFormatString="##,###.##">
                                    </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <%--<dx:LayoutItem Caption="Per">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbPer" ClientInstanceName="tbPer">
                                    </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>--%>
                        <dx:LayoutItem Caption="Selling Unit">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <%--<dx:ASPxTextBox runat="server" ID="tbSellingUnit" ClientInstanceName="tbSellingUnit">
                                    </dx:ASPxTextBox>--%>
                                    <dx:ASPxComboBox ID="CmbSellingUnit" runat="server"  ClientInstanceName="CmbSellingUnit" Width="170px"
                                    TextFormatString="{0}">
                                    <Items>
                                        <dx:ListEditItem Text="THB" />
                                        <dx:ListEditItem Text="USD" />
                                    </Items>
                                </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:LayoutGroup>
                <dx:LayoutGroup Caption="Secondary Packaging" GroupBoxDecoration="None" ColCount="3">
                    <Items>
                    <dx:LayoutItem Caption="Package Code">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxButtonEdit ID="CmbSecPackageCode" ClientInstanceName="ClientSecPackageCode"
                                    Width="100%" runat="server">
                                        <Buttons>
                                            <dx:EditButton>
                                            </dx:EditButton>
                                        </Buttons>
                                            <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('SecPackageCode'); }" />
                                    </dx:ASPxButtonEdit>
                                    <%--<dx:ASPxComboBox ID="CmbSecPackageCode" runat="server" ValueField="Material" DataSourceID="dsPackage"     
                                      EnableCallbackMode="true" TextFormatString="{0} ({1})"  IncrementalFilteringMode="Contains" DropDownStyle="DropDown"
                                      CallbackPageSize="10"  ClientInstanceName="CmbSecPackageCode" DropDownWidth="600px" >
                                      <Columns>
                                            <dx:ListBoxColumn FieldName="Material"/>
                                            <dx:ListBoxColumn FieldName="Description" Width="230px"/>
                                            <dx:ListBoxColumn FieldName="Price"/>
                                            <dx:ListBoxColumn FieldName="Currency"/>
                                            <dx:ListBoxColumn FieldName="Unit"/>
                                        </Columns>
                                        <ClientSideEvents ValueChanged="function(s, e) { OnValueChanged(s,'secp'); }" />
                                    </dx:ASPxComboBox>--%>
                                </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Description">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbSecDescription" ClientInstanceName="tbSecDescription">
                                    </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Quantity">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbSecQuantity" ClientInstanceName="tbSecQuantity">
                                        <ClientSideEvents KeyPress="function(s,e) { ProcessKeyPress(s, e);}"
                                            TextChanged="function(s, e) { OnTextChangedHandler(s,'secp');}" />
                                    </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Amount">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbSecAmount" ClientInstanceName="tbSecAmount" ReadOnly="true">
                                    </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Price/Rate">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbSecPriceRate" ClientInstanceName="tbSecPriceRate">
                                    </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <%--<dx:LayoutItem Caption="Per">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbSecPer" ClientInstanceName="tbSecPer">
                                    </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>--%>
                        <dx:LayoutItem Caption="Selling Unit">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <%--<dx:ASPxTextBox runat="server" ID="tbSecSellingUnit" ClientInstanceName="tbSecSellingUnit">
                                    </dx:ASPxTextBox>--%>
                                    <dx:ASPxComboBox ID="CmbSecSellingUnit" runat="server"  ClientInstanceName="CmbSecSellingUnit" Width="170px"
                                    TextFormatString="{0}">
                                    <Items>
                                        <dx:ListEditItem Text="THB" />
                                        <dx:ListEditItem Text="USD" />
                                    </Items>
                                </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:LayoutGroup>            
	    <dx:LayoutGroup Caption="Margin" ColCount="2">
                <Items>
                    <dx:LayoutItem Caption="Margin">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <%--<dx:ASPxComboBox ID="CmbMargin" runat="server" ValueField="MarginCode" DataSourceID="dsMargin" OnCallback="CmbMargin_Callback"
                                     TextFormatString="{0}" ClientInstanceName="CmbMargin" DropDownWidth="400px">
                                      <Columns>
                                            <dx:ListBoxColumn FieldName="MarginCode"/>
                                            <dx:ListBoxColumn FieldName="MarginName" Width="250px"/>
                                            <dx:ListBoxColumn FieldName="MarginRate"/>
                                            <dx:ListBoxColumn FieldName="PercentMargin" Width="30px" Caption="%"/>
                                        </Columns>
                                        <ClientSideEvents ValueChanged="function(s, e) { OnValueChanged(s,'margin'); }" />
                                    </dx:ASPxComboBox>--%>
                                    <dx:ASPxButtonEdit ID="CmbMargin" ClientInstanceName="ClientMargin"
                                    Width="100%" runat="server">
                                        <Buttons>
                                            <dx:EditButton>
                                            </dx:EditButton>
                                        </Buttons>
                                            <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Margin'); }" />
                                    </dx:ASPxButtonEdit>
                                </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="MarginName">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbMarginName" ClientInstanceName="tbMarginName">
                                    </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="%">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbMarginRate" ClientInstanceName="tbMarginRate">
                                    </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <%--<dx:LayoutItem Caption="%">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbtbMarginpct" ClientInstanceName="tbtbMarginpct">
                                    </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>--%>
                </Items>
            </dx:LayoutGroup>
           <dx:LayoutGroup Caption="Upcharge" ColCount="3">
                <Items>
                    <dx:LayoutItem Caption="UpCharge">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxTextBox runat="server" ID="tbUpCharge" ClientInstanceName="tbUpCharge">
                                </dx:ASPxTextBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Price">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxTextBox runat="server" ID="tbUpChargePrice" ClientInstanceName="tbUpChargePrice">
                                </dx:ASPxTextBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Quantity">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxTextBox runat="server" ID="tbUpChargeQuantity" ClientInstanceName="tbUpChargeQuantity">
                                </dx:ASPxTextBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Currency">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbUpChargeCurrency" runat="server"  ClientInstanceName="CmbUpChargeCurrency" Width="170px"
                                    TextFormatString="{0}">
                                    <Items>
                                        <dx:ListEditItem Text="THB" />
                                        <dx:ListEditItem Text="USD" />
                                    </Items>
                                </dx:ASPxComboBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                </Items>
            </dx:LayoutGroup>
	<dx:LayoutGroup Caption="Labor&Overhead" ColCount="1" Width="80%">
                    <Items>
                    <dx:LayoutItem Caption="Labor&Overhead">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox runat="server" ID="cmbLaborOverhead" ClientInstanceName="cmbLaborOverhead"
                                    DataSourceID="dsLOH" ValueField="value" TextField="value"/>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                      </dx:LayoutItem>
                      <dx:LayoutItem Caption="Unit/Base">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxTextBox runat="server" ID="tbResultLOH" ClientInstanceName="tbResultLOH"/>
                                            </td>
                                            <td>&nbsp;</td>
                                            <td>
                                                <dx:ASPxComboBox ID="cmbLOHType" runat="server" Width="35px" ClientInstanceName="cmbLOHType"
                                                    DataSourceID="dsLOHBase" ValueField="value" TextField="value"/>
                                            </td>
                                        </tr>
                                    </table>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                        </Items>
                    </dx:LayoutGroup>
                    <dx:LayoutGroup Caption="Product Name" ColCount="1" Width="80%">
                    <Items>
                        <dx:LayoutGroup Caption="" GroupBoxDecoration="None">
                            <Items>
                        <dx:LayoutItem Caption="">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxGridView ID="gvCode" runat="server" ClientInstanceName="gvCode"                                    
                                    KeyFieldName="ID">
 
                                    <Columns>
                                        <%--<dx:GridViewCommandColumn ShowClearFilterButton="true" Width="42px" ButtonRenderMode="Image" ShowInCustomizationForm="True"
                                                FixedStyle="Left" ShowNewButtonInHeader="true" ShowEditButton="true" ShowDeleteButton="true">   
                                                 <CustomButtons>
                                                    <dx:GridViewCommandColumnCustomButton ID="colDel">
                                                        <Image ToolTip="Remove" Url="~/Content/Images/Cancel.gif"/>
                                                    </dx:GridViewCommandColumnCustomButton>
                                                </CustomButtons>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <HeaderTemplate>
                                                    <dx:ASPxButton runat="server" RenderMode="Link" AutoPostBack="false">
                                                        <Image ToolTip="Insert" Url="~/Content/Images/icons8-plus-math-filled-16.png" />
                                                        <ClientSideEvents Click="function(s,e){ gvCode.AddNewRow(); }" />
                                                    </dx:ASPxButton>
                                                </HeaderTemplate>
                                            </dx:GridViewCommandColumn>
       --%>
                                        <dx:GridViewDataColumn FieldName="Formula" ReadOnly="true" Width="45px"/>
                                        <dx:GridViewDataTextColumn FieldName="Code" />
                                        <dx:GridViewDataTextColumn FieldName="Name"/>
                                        <dx:GridViewDataTextColumn FieldName="RefSamples" />
                                    </Columns>
                                    <SettingsSearchPanel ColumnNames="" Visible="false" />
		                            <Settings ShowGroupPanel="false" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="false" ShowStatusBar="Hidden"/>
		                            <SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true"/>
		                            <SettingsPager Mode="ShowAllRecords"/>
		                            <Styles>
			                            <Row Cursor="pointer" />
		                            </Styles>
                                    <SettingsContextMenu Enabled="true" />
                                    <EditFormLayoutProperties>
                                        <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="0" />
                                    </EditFormLayoutProperties>
                                    <SettingsEditing Mode="Batch">
                                       <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="Cell"/>
                                   </SettingsEditing>    
                                </dx:ASPxGridView>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                                
                            </Items>
                        </dx:LayoutGroup>
                    </Items>
                </dx:LayoutGroup>
                <dx:LayoutGroup Caption="Attached file">
                       <Items>
                        <dx:LayoutItem Caption="Selected" Width="100%">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxUploadControl ID="UploadControl" runat="server" ClientInstanceName="UploadControl" Width="200" 
                                            NullText="Select multiple files..." UploadMode="Advanced" ShowUploadButton="false" ShowProgressPanel="True"
                                              >
                                            <AdvancedModeSettings EnableMultiSelect="True" EnableFileList="True" EnableDragAndDrop="True"  />
                                            <ValidationSettings MaxFileSize="10485760" AllowedFileExtensions=".jpg,.jpeg,.gif,.png,.xls,.xlsx,.pdf">
                                            </ValidationSettings>
                                           
                                    </dx:ASPxUploadControl>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                          <dx:LayoutItem Caption="">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxFileManager ID="fileManager" runat="server" Height="240px" 
                                        ClientInstanceName="fileManager">
                        
                                        <Settings ThumbnailFolder="~/Content/FileManager" InitialFolder="Salvador Dali\1936 - 1945" 
                                            AllowedFileExtensions=".rtf, .pdf, .doc, .docx, .odt, .txt, .xls, .xlsx, .xlsb, .ods, .ppt, .pptx, .odp, .jpe, .jpeg, .jpg, .gif, .png , .msg"/>
                                        <SettingsFileList>
                                            <ThumbnailsViewSettings ThumbnailWidth="100" ThumbnailHeight="100" />
                                        </SettingsFileList>
                                        <SettingsToolbar ShowPath="false" ShowRefreshButton="false" />
                                        <SettingsDataSource KeyFieldName="ID" ParentKeyFieldName="ParentID" NameFieldName="Name" IsFolderFieldName="IsFolder" FileBinaryContentFieldName="Data" LastWriteTimeFieldName="LastWriteTime" />
                                        <SettingsEditing AllowCreate="false" AllowDelete="true" AllowMove="false" AllowRename="false" AllowDownload="true" />
                                        <SettingsBreadcrumbs Visible="true" ShowParentFolderButton="true" Position="Top" />
                                        <SettingsUpload UseAdvancedUploadMode="true" Enabled="false">
                                            <AdvancedModeSettings EnableMultiSelect="true"  />
                                            <ValidationSettings 
                                                MaxFileSize="10000000" 
                                                MaxFileSizeErrorText="The file you are trying to upload is larger than what is allowed (10 MB).">
                                            </ValidationSettings>
                                        </SettingsUpload>
					                <Settings EnableMultiSelect="true" />
                                    </dx:ASPxFileManager>
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
	    <%--<dx:LayoutItem Caption="" CaptionSettings-Location="Top" Name="testMenu" ClientVisible="false">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxMenu ID="ASPxMenu1" runat="server" CssClass="ActionMenu" ClientInstanceName="ASPxMenu1" SeparatorWidth="0" BackColor="White">
                                <Items>
                                    <dx:MenuItem Text="Add Row" Name="new"  Image-Url="~/Content/Images/icons8-plus-16.png"/>
                                    
                                    <dx:MenuItem Text="Clear" Name="clear" Image-Url="~/Content/Images/Delete.gif" />
                                    <dx:MenuItem Text="Export to" BeginGroup="true" Image-Url="~/Content/Images/if_sign-out_59204.png">
                                        <Items>
                                            <dx:MenuItem Name="ExportToPDF" Text="PDF" Image-Url="~/Content/Images/excel.gif"/>
                                        </Items>
                                    </dx:MenuItem>
                                </Items>
                                <ClientSideEvents ItemClick="ConfirmAndExecute" />
                                <Border BorderWidth="0" />
                                </dx:ASPxMenu>
                                
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>--%>
                    <dx:LayoutGroup Caption="" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                    <Paddings PaddingLeft="0" />
                    <GridSettings StretchLastItem="true" WrapCaptionAtWidth="660">
                       <Breakpoints>
                            <dx:LayoutBreakpoint MaxWidth="500" ColumnCount="1" Name="S" />
                            <dx:LayoutBreakpoint MaxWidth="800" ColumnCount="2" Name="M" />
                       </Breakpoints>
                       </GridSettings>
                    <GroupBoxStyle>
                        <Caption Font-Bold="true" Font-Size="10"/>
                    </GroupBoxStyle>
                    <Items>  
                        <dx:LayoutItem Caption="" Width="100%">
                          <SpanRules>
                            <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                            <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                        </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxGridView ID="grid" ClientInstanceName="grid" runat="server" Width="100%" 
                                    SettingsPager-PageSize="60" AutoGenerateColumns="false" KeyFieldName="RowID"> 
                                   
                                    <Columns>
					                <dx:GridViewCommandColumn  ShowClearFilterButton="true" Width="42px" ButtonRenderMode="Image"
                                            FixedStyle="Left" ShowNewButtonInHeader="true" ShowEditButton="true" ShowDeleteButton="true">
                                            <CustomButtons>
                                                <dx:GridViewCommandColumnCustomButton ID="EditCost">
                                                    <Image ToolTip="Remove" Url="~/Content/Images/Cancel.gif"/>
                                                </dx:GridViewCommandColumnCustomButton>
                                            </CustomButtons>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <HeaderTemplate>
                                                <dx:ASPxButton runat="server" RenderMode="Link" AutoPostBack="false">
                                                    <Image ToolTip="Insert" Url="~/Content/Images/icons8-plus-math-filled-16.png" />
                               
                                                </dx:ASPxButton>
                                            </HeaderTemplate>
                                        </dx:GridViewCommandColumn>
                                        <%--<dx:GridViewCommandColumn ShowSelectCheckbox="True" ShowClearFilterButton="true" 
					                    FixedStyle="Left" SelectAllCheckboxMode="Page" Visible="false"/>
                                        <dx:GridViewCommandColumn ShowDeleteButton="true" FixedStyle="Left" Width="75px"/>
                                        <dx:GridViewDataTextColumn Caption="#" FixedStyle="Left">
                                            <DataItemTemplate>
                                                <dx:ASPxCheckBox ID="cbCheck" runat="server" AutoPostBack="false" OnLoad="cbCheck_Load" />
                                            </DataItemTemplate>
                                            <HeaderTemplate>
                                                <dx:ASPxCheckBox ID="SelectAllCheckBox" runat="server" ToolTip="Select/Unselect all rows on the page" AutoPostBack="false"
                                                    ClientSideEvents-CheckedChanged="function(s, e) { grid.PerformCallback('cbCheck|'+s.GetChecked()); }" />
                                            </HeaderTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataColumn FieldName="RowID" FixedStyle="Left"  Width="0px"/>--%>
                                        <dx:GridViewDataComboBoxColumn FieldName="Component" FixedStyle="Left" VisibleIndex="1"/>
                                        <dx:GridViewDataComboBoxColumn FieldName="SubType" FixedStyle="Left"/>
                                        <dx:GridViewDataColumn FieldName="aValidate">
                                            <HeaderStyle CssClass="hide" />
                                            <EditCellStyle CssClass="hide" />
                                            <CellStyle CssClass="hide" />
                                            <FilterCellStyle CssClass="hide" />
                                            <FooterCellStyle CssClass="hide" />
                                            <GroupFooterCellStyle CssClass="hide" />
                                        </dx:GridViewDataColumn>
                                        <dx:GridViewDataColumn FieldName="Description" Width="180" FixedStyle="Left"/>
                                        <dx:GridViewDataButtonEditColumn FieldName="SAPMaterial" FixedStyle="Left" UnboundType="String">
                                            <PropertiesButtonEdit>
                                                <Buttons>
                                                    <dx:EditButton />
                                                </Buttons>
                                                <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('SAPMaterial'); }" />
                                            </PropertiesButtonEdit>
                                        </dx:GridViewDataButtonEditColumn>
                                        <%--<dx:GridViewDataComboBoxColumn FieldName="SAPMaterial" FixedStyle="Left" UnboundType="String">
                                            <PropertiesComboBox ValueField="RawMaterial" EnableCallbackMode="true" TextFormatString="{0}" 
                                                CallbackPageSize="10" DataSourceID="dsRawMaterial">
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { Combo_SelectedIndexChanged(s); }"/>
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="RawMaterial" />
                                                    <dx:ListBoxColumn FieldName="Description" />
                                                </Columns>
                                                <ClientSideEvents EndCallback="Combo_EndCallback" />
                                            </PropertiesComboBox>
                                            <Settings AutoFilterCondition="Contains" FilterMode="DisplayText" />
                                        </dx:GridViewDataComboBoxColumn>--%>
                                        <dx:GridViewDataTextColumn FieldName="GUnit" FixedStyle="Left"/>
                                        <dx:GridViewDataTextColumn FieldName="Yield" FixedStyle="Left"/>
                                        <dx:GridViewDataButtonEditColumn FieldName="RawMaterial">
                                            <PropertiesButtonEdit>
                                                <Buttons>
                                                    <dx:EditButton />
                                                </Buttons>
                                                <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('RawMaterial'); }" />
                                            </PropertiesButtonEdit>
                                        </dx:GridViewDataButtonEditColumn>
                                        <%--<dx:GridViewDataComboBoxColumn FieldName="RawMaterial">
                                            <PropertiesComboBox EnableCallbackMode="true" CallbackPageSize="20" DataSourceID="dsMaterial" TextFormatString="{0}"  ValueField="Material" DropDownStyle="DropDown">
                                                <Columns>                                            
                                                    <dx:ListBoxColumn FieldName="Material" />
                                                    <dx:ListBoxColumn FieldName="Name" />
                                                    <dx:ListBoxColumn FieldName="Yield" />
                                                </Columns>
                                                <ClientSideEvents SelectedIndexChanged="OnSelectedIndexChanged" />
                                            </PropertiesComboBox>
                                            <Settings AutoFilterCondition="Contains" FilterMode="DisplayText" />
                                        </dx:GridViewDataComboBoxColumn>--%>
                                        <dx:GridViewDataTextColumn FieldName="Name" />
                                        <dx:GridViewDataTextColumn FieldName="PriceOfUnit"/>       
                                        <dx:GridViewDataComboBoxColumn FieldName="Currency">
                                            <PropertiesComboBox TextFormatString="{0}" DataSourceID="dsCurrency" ValueField="Value">
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="Value" />
                                                </Columns>
                             
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataComboBoxColumn FieldName="Unit">
                                            <PropertiesComboBox TextFormatString="{0}" DataSourceID="dsUnit" ValueField="Value">
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="Value" />
                                                </Columns>
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataColumn FieldName="ExchangeRate" />
                                        <dx:GridViewDataColumn FieldName="BaseUnit" />
                                        <dx:GridViewDataColumn FieldName="PriceOfCarton"  />
                                        <dx:GridViewDataColumn FieldName="Remark"  />
					                    <dx:GridViewBandColumn Caption="LBOh">
                                            <Columns>
                                                <dx:GridViewDataButtonEditColumn FieldName="LBOh">
                                                    <PropertiesButtonEdit>
                                                        <Buttons>
                                                            <dx:EditButton />
                                                        </Buttons>
                                                        <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('LBOh'); }" />
                                                    </PropertiesButtonEdit>
                                                </dx:GridViewDataButtonEditColumn>
                                        <%--<dx:GridViewDataComboBoxColumn FieldName="LBOh" Caption="Code">
                                            <PropertiesComboBox ValueField="LBCode" DataSourceID="dsLaborOverhead"
                                                EnableCallbackMode="true" CallbackPageSize="10" TextFormatString="{0}" DropDownStyle="DropDown">
                                                <ClientSideEvents SelectedIndexChanged="OnChanged" />
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="LBCode"/>
                                                    <dx:ListBoxColumn FieldName="LBName" Width="250px"/>
                                                    <dx:ListBoxColumn FieldName="PackSize"/>
                                                    <dx:ListBoxColumn FieldName="LBType"/>
                                                    <dx:ListBoxColumn FieldName="LBRate"/>
                                                    <dx:ListBoxColumn FieldName="Currency"/>
                                                </Columns>
                                            </PropertiesComboBox>
                                            <Settings AutoFilterCondition="Contains" FilterMode="DisplayText" />
                                        </dx:GridViewDataComboBoxColumn>--%>
                                        <dx:GridViewDataTextColumn FieldName="LBRate" Caption="Rate"/>
                                            </Columns>
                                        </dx:GridViewBandColumn>
                                        <dx:GridViewDataColumn FieldName="Formula">
                                            <HeaderStyle CssClass="hide" />
                                            <EditCellStyle CssClass="hide" />
                                            <CellStyle CssClass="hide" />
                                            <FilterCellStyle CssClass="hide" />
                                            <FooterCellStyle CssClass="hide" />
                                            <GroupFooterCellStyle CssClass="hide" />
                                        </dx:GridViewDataColumn>
                                        <dx:GridViewDataColumn FieldName="IsActive">
                                            <HeaderStyle CssClass="hide" />
                                            <EditCellStyle CssClass="hide" />
                                            <CellStyle CssClass="hide" />
                                            <FilterCellStyle CssClass="hide" />
                                            <FooterCellStyle CssClass="hide" />
                                            <GroupFooterCellStyle CssClass="hide" />
                                        </dx:GridViewDataColumn>
					                <dx:GridViewDataColumn FieldName="Mark" Width="10px" ReadOnly="true"/>
                                    </Columns>
                               
                                    <SettingsSearchPanel ColumnNames="" Visible="false" />
		                            <Settings ShowGroupPanel="false" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="true" ShowStatusBar="Hidden"/>
                                    <SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true" AllowFocusedRow="true" />
					                <SettingsPager PageSize="50">
						            <PageSizeItemSettings Visible="true" ShowAllItem="true"  AllItemText="All Records" />
					                </SettingsPager>
		                            <Styles>
			                            <Row Cursor="pointer" />
		                            </Styles>
                                    <SettingsContextMenu Enabled="true" />
				                    <styles>
                                        <focusedrow BackColor="#f4dc7a" ForeColor="Black"></focusedrow>
                                        <FixedColumn BackColor="LightYellow"></FixedColumn>
                                    </styles>
                                    <SettingsSearchPanel ColumnNames="" Visible="false" />
                                    <TotalSummary>
                                        <dx:ASPxSummaryItem FieldName="GUnit" SummaryType="Sum" />
                                        <dx:ASPxSummaryItem FieldName="PriceOfCarton" SummaryType="Custom" />
                                        <dx:ASPxSummaryItem FieldName="LBRate" SummaryType="Custom" />
                                    </TotalSummary>
                                    <SettingsEditing Mode="Batch">
                                       <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="Cell"/>
                                   </SettingsEditing>
                                </dx:ASPxGridView>

				                <dx:ASPxGridView ID="testgrid" ClientInstanceName="testgrid" runat="server" Width="100%" KeyFieldName="ID" >
       

                                    <Toolbars>
                                    <dx:GridViewToolbar Name="toolbar">
                                        <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                                        <Items>
                                            <dx:GridViewToolbarItem Command="New" Name="New" />
                                            <dx:GridViewToolbarItem Command="Edit" />
                                            <dx:GridViewToolbarItem Text="Remove" Name="Remove" Image-Url="~/Content/Images/Cancel.gif" />
                                            <dx:GridViewToolbarItem Text="Undo" ItemStyle-Width="100px" Name="Undo" Image-IconID="actions_resetchanges_16x16devav" />
                                        </Items>
                                    </dx:GridViewToolbar>
                                </Toolbars>
                                    <Columns>
                                        <%--<dx:GridViewCommandColumn Name="myCommandColumn" ShowClearFilterButton="true" Width="42px" ButtonRenderMode="Image"
                                                FixedStyle="Left" ShowNewButtonInHeader="true" ShowEditButton="true" ShowDeleteButton="true">   
                                                 <CustomButtons>
                                                    <dx:GridViewCommandColumnCustomButton ID="remove" >
                                                        <Image ToolTip="Remove" Url="~/Content/Images/Cancel.gif"/>
                                                        
                                                    </dx:GridViewCommandColumnCustomButton>
                                                </CustomButtons>
                                            
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <HeaderTemplate>
                                                    <dx:ASPxButton runat="server" RenderMode="Link" AutoPostBack="false">
                                                        <Image ToolTip="Insert" Url="~/Content/Images/icons8-plus-math-filled-16.png" />
                                                        <ClientSideEvents Click="function(s,e){ insertSelection('new'); }" />
                                                    </dx:ASPxButton>
                                                </HeaderTemplate>
                                            </dx:GridViewCommandColumn>--%>
                                        <dx:GridViewDataColumn FieldName="Component" Width="130" FixedStyle="Left"/>
                                        <dx:GridViewDataColumn FieldName="Code" Caption="SAPMaterial" FixedStyle="Left"/>
                                        <dx:GridViewDataColumn FieldName="Name" Width="180" FixedStyle="Left"/>
                                        <dx:GridViewDataColumn FieldName="Quantity" />
                                        <dx:GridViewDataColumn FieldName="PriceUnit" />
                                        <dx:GridViewDataColumn FieldName="Amount" />
                                        <dx:GridViewDataTextColumn  FieldName="Per">
                                            <PropertiesTextEdit>
                                                <ClientSideEvents ValueChanged="function(s, e) { ChangeBatchEditorValue(s, e); }" />
                                            </PropertiesTextEdit>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataColumn FieldName="SellingUnit" />
                                        <dx:GridViewDataColumn FieldName="Loss" />
                                        <dx:GridViewDataColumn FieldName="Formula">
                                            <HeaderStyle CssClass="hide" />
                                            <EditCellStyle CssClass="hide" />
                                            <CellStyle CssClass="hide" />
                                            <FilterCellStyle CssClass="hide" />
                                            <FooterCellStyle CssClass="hide" />
                                            <GroupFooterCellStyle CssClass="hide" />
                                        </dx:GridViewDataColumn>
					                <dx:GridViewDataColumn FieldName="Mark" Width="20px" />
                                    </Columns>
                                    <SettingsEditing Mode="Batch">
                                       <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="Cell"/>
                                   </SettingsEditing>
                                    <SettingsSearchPanel ColumnNames="" Visible="false" />
		                            <Settings ShowGroupPanel="false" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" 
					                ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="true" ShowStatusBar="Hidden"/>
		                            <SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true"/>
					                <SettingsPager AlwaysShowPager="true"/>
		                            <Styles>
			                            <Row Cursor="pointer" />
		                            </Styles>
                                    <SettingsContextMenu Enabled="true" />
                                    <TotalSummary>
                                        <dx:ASPxSummaryItem FieldName="Loss" SummaryType="Custom" ShowInColumn="Loss" />
                                    </TotalSummary>
                                </dx:ASPxGridView>

                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                       </Items>
                </dx:LayoutGroup>
             <dx:LayoutGroup Caption="Status" ColCount="3" GroupBoxDecoration="Box" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                    <GroupBoxStyle>
                        <Caption Font-Bold="true" Font-Size="10"/>
                    </GroupBoxStyle>
                    <Items>
                        <dx:LayoutItem Caption="Send Document" Width="100%">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                              <dx:ASPxComboBox ID="CmbDisponsition" runat="server" TextField="Title" ValueField="ID"  
                                    Width="230px" ClientInstanceName="ClientDisponsition">
 
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
                        <dx:LayoutItem Caption="Comment" Width="93.2%">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxMemo runat="server" ID="mComment" Rows="4" ClientInstanceName="ClientComment">
                                    </dx:ASPxMemo>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                       <dx:LayoutItem Caption="EventLog" Width="100%" Name="EventLog" CaptionStyle-Font-Bold="true">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <%--<dx:ASPxHiddenField ID="eventlog" runat="server" ClientInstanceName="eventlog" />
                                        <asp:Literal ID="litText" runat="server"></asp:Literal>--%>
                                        <div id="divScroll" style="overflow: scroll; height: 230px; width: auto;">
                                        <dx:aspxlabel id="lbEventLog" runat="server" clientinstancename="clienteventlog" text="ASPxLabel"/></div>
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
        SelectCommand="spGetCompany" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Name="ID" Type="String" />
	    <asp:Parameter Name="BU" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsselectcosting" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spselectcosting" SelectCommandType="StoredProcedure">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsulogin" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select FirstName+' '+LastName as fn,[user_name] from ulogin"/>
 

    <asp:SqlDataSource ID="dsCurrency" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('THB;USD;',';')">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsUnit" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('KG;G;Ton',';')">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsMaterial" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetMaterial" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Name="Company" Type="String" />
            <asp:Parameter Name="RawMaterial" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsReason" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasReason"/>
    <asp:SqlDataSource ID="ods" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetCostingSheet" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Name="Id" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
 
   <asp:SqlDataSource ID="dsnamelist" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="spGetFormulaHeader" SelectCommandType="StoredProcedure">
       <SelectParameters>
           <asp:Parameter Name="Id" />
       </SelectParameters>
   </asp:SqlDataSource>
   <asp:SqlDataSource ID="dsStatusApp" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasStatusApp where levelapp in (1,2)"/>
 
    <asp:SqlDataSource ID="dsLOHBase" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('%;THB',';') order by value"/>
    <asp:SqlDataSource ID="dsLOH" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('Labelling & WH DL;LOH Factor Adjustment',';') order by value"/>