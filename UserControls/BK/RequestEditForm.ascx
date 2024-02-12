<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RequestEditForm.ascx.cs" Inherits="UserControls_RequestEditForm" %>
<script type="text/javascript">
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
    function OnTabbedGroupPageControlInit(s, e) {
        s.SetActiveTabIndex(0);
        //s.SetActiveTabIndex(TabList.GetActiveTabIndex());
    }

    function CateSelectedIndexChanged(s, e) {
    }
    function OnIDChanged(s) {
        var identi = s.GetValue().toString() == "3";
        formLayout.GetItemByName("Assignee").SetVisible(s.GetValue().toString() == "5");
        formLayout.GetItemByName("Reason").SetVisible(identi);
        CmbAssignee.GetGridView().PerformCallback(['load', s.GetValue()].join('|'));
        CmbReason.PerformCallback(s.GetValue());
    }
    function OnValidation(s, e) {
        //debugger;
        if (e.value == null) {
            e.isValid = true;
            e.errorText = "Field is required.";
            return;
        }
        //else
        //    e.isValid = true;
    }

    function OnValueChanged(evt) {
        var result = [ClientProductGroup.GetValue(), ClientContainerLid.GetValue(),
            ClientGrade.GetValue(), ClientZone.GetValue(), "\xa0",ClientRawMaterial.GetValue(),ClientStyle.GetValue(), ClientTopping.GetValue(), ClientSizeMedia.GetValue()].join("");
        ClientProductCode.SetText(result);
        
        ClientResult.SetText(["2", ClientProductGroup.GetValue(), ClientRawMaterial.GetValue(), ClientStyle.GetValue(), ClientTopping.GetValue(),
            ClientSizeMedia.GetValue(), ClientContainerLid.GetValue(), ClientGrade.GetValue(), ClientZone.GetValue(), ClientPackingStyle.GetValue()].join(""));
        //if (evt == "ProductGroup") {
            debugger;
        //    setResult(ClientProductGroup.GetValue());
        //}
        if ([ClientResult.GetText(), ""].join("").length > 15) {
            ClientgridData.GetValuesOnCustomCallback(["GetResult", evt, 0].join("|"), function (r) {
                if (!r)
                    return;
                ClientHierarchy.SetText(r["Name"]);
                //    ClientResult.SetText(r["result"]);
                //    ClientProductCode.SetText(r["Code"]);
            });
        }
    }
    function OnButtonClick(s, e) {
        var n = prompt('Input result', '');
        if (n != null) {
            ClientResult.SetText(n);
        }
    }
    function OnBtnShowPopupClick(view) {
        var url = 'popupControls/selcosting.aspx?view=' + view;
        PopupControl.RefreshContentUrl();
        PopupControl.SetContentUrl(url);
        PopupControl.SetHeaderText(url);
        PopupControl.Show();
    }
    function OnBtnBuildClick(view) {
        var url = 'popupControls/selstruc.aspx?view=' + view;
        PopupControl.RefreshContentUrl();
        PopupControl.SetContentUrl(url);
        PopupControl.SetHeaderText(url);
        PopupControl.Show();
    }
    function HidePopupAndShowInfo(closedBy, returnValue) {
        PopupControl.Hide();
        var param = [ClientProductGroup.GetValue(), "0"].join('|');
        if (closedBy == "CanSize") {
            ClientSizeMedia.PerformCallback(param);
        }
        if (closedBy == 'renew') {
            var arge = [closedBy, returnValue, 0].join("|");
            
            //gv.PerformCallback(arge);
            ClientgridData.GetValuesOnCustomCallback(arge, function (r) {
                if (!r)
                    return;
		debugger;
                ClientTechnicalNo.SetValue(r["Code"]);
                ClientMarketingNo.SetValue(r["Name"]);
		hfCostID.Set("CostID", r["CostID"]);
                hfTrfID.Set("TrfID", r["TRFID"]);
		ClientRefSamples.SetText(r["RefSamples"]);
            });
        }
        //gv.SetEditValue('RequestNo', returnValue);
    }

    function OnEndCallback(s) {
        if (lastCountry) {
            s.PerformCallback(lastCountry);
            lastCountry = null;
        }
    }
    function OnSelectedIndexChanged(s, e) {
        var rusult = 'build|' + s.GetValue();
        cpUpdateRole.PerformCallback(rusult);
    }
    var lastCountry = null;
    function OnCountryChanged(cmbCountry) {
        if (ClientTopping.InCallback())
            lastCountry = cmbCountry.GetValue().toString();
        else {
            var param = [cmbCountry.GetValue().toString(), "0"].join('|');
            ClientTopping.PerformCallback(param);
            ClientRawMaterial.PerformCallback(param);
            ClientStyle.PerformCallback(param);

            //gLookUp.GetGridView().PerformCallback(param);
            ClientSizeMedia.PerformCallback(param);
            ClientContainerLid.PerformCallback(param);
            ClientGrade.PerformCallback(param);
            ClientZone.PerformCallback(param);
        }
    }
    function setResult(doc) {
        ClientgridData.GetValuesOnCustomCallback(["doc" , doc,0].join("|"), function (r) {
            if (!r)
                return;
            //Demo.setproduct(r);
        })
    }
    function OnGridSelectionChanged(s, e) {
        setGridHeaderCheckboxes(s, e);
    }
    function setGridHeaderCheckboxes(s, e) {
        //cbAll
        debugger;
        var indexes = ClientgridData.cpIndexesSelected;
        //cbAll.SetChecked(s.GetSelectedRowCount() == Object.size(indexes));
        var view = Demo.DemoState.View;
        var RowCount = ClientgridData.GetSelectedRowCount();
        //cbPage
        var allEnabledRowsOnPageSelected = true;
        var indexes = ClientgridData.cpIndexesUnselected;
        var topVisibleIndex = ClientgridData.GetTopVisibleIndex();
        for (var i = topVisibleIndex; i < topVisibleIndex + ClientgridData.cpPageSize; i++) {
            if (indexes.indexOf(i) == -1)
                if (!ClientgridData.IsRowSelectedOnPage(i)) allEnabledRowsOnPageSelected = false;
        }
        ClientActionMenu.GetItemByName("Approve").SetVisible(view == "MailList" && RowCount > 0);
        //ClientActionMenu.GetItemByName("Reject").SetVisible(view == "MailList" && RowCount > 0);
        //cbPage.SetChecked(allEnabledRowsOnPageSelected);
    }
</script>
<dx:ASPxHiddenField ID="hfTrfID" runat="server" ClientInstanceName="hfTrfID" />
<dx:ASPxHiddenField ID="hfCostID" runat="server" ClientInstanceName="hfCostID" />
<dx:ASPxHiddenField ID="hfGeID" runat="server" ClientInstanceName="hfGeID" />
<dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
<dx:ASPxHiddenField ID="username" runat="server" ClientInstanceName="username"/>
<dx:ASPxHiddenField ID="hftype" runat="server" ClientInstanceName="hftype" />
<dx:ASPxHiddenField ID="approv" runat="server" ClientInstanceName="approv" />
<dx:ASPxGridView runat="server" ID="gridData" KeyFieldName="ID" ClientInstanceName="ClientgridData" Width="100%" DataSourceID="dsgv" 
        AutoGenerateColumns="true" OnCustomDataCallback="gridData_CustomDataCallback" OnCustomCallback="gridData_CustomCallback" 
        OnFillContextMenuItems="gridData_FillContextMenuItems"
        OnContextMenuItemClick="gridData_ContextMenuItemClick" OnCommandButtonInitialize="gridData_CommandButtonInitialize" Border-BorderWidth="0">
        <Columns>
            <%--<dx:GridViewCommandColumn ShowNewButton="true" ShowEditButton="true" VisibleIndex="0" ButtonRenderMode="Image" Width="45px">
            <CustomButtons>
                <dx:GridViewCommandColumnCustomButton ID="Clone">
                    <Image ToolTip="Clone Record" Url="~/Content/Images/Copy.png"/>
                </dx:GridViewCommandColumnCustomButton>
            </CustomButtons>
            </dx:GridViewCommandColumn>--%>
            <dx:GridViewCommandColumn ShowSelectCheckbox="True" ShowClearFilterButton="true" SelectAllCheckboxMode="None" Width="45" VisibleIndex="0"/>
            <dx:GridViewDataTextColumn FieldName="ID" Width="30px"/>
            <dx:GridViewDataTextColumn FieldName="DocumentNo" Caption="RequestNo"/>
            <dx:GridViewDataTextColumn FieldName="RequestName" Caption="TRF"/>
            <dx:GridViewDataTextColumn FieldName="CostName" Caption="CostingNo."/> 
            <dx:GridViewDataTextColumn FieldName="RefSamples"/> 
	    <dx:GridViewDataComboBoxColumn FieldName="Destination">
                <PropertiesComboBox DataSourceID="dsCountry" ValueField="Code" TextField="Name" />
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataComboBoxColumn FieldName="StatusApp" GroupIndex="0">
                <PropertiesComboBox DataSourceID="dsStatusApp" TextField="Title" ValueField="ID" />
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataTextColumn FieldName="Result" Caption="Material"/>
            <dx:GridViewDataComboBoxColumn FieldName="Requester">
                <PropertiesComboBox DataSourceID="dsulogin" ValueField="user_name" TextField="fn" />
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataDateColumn FieldName="CreateOn" Caption="CreateOn"> 
                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                </PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
        </Columns>
        <SettingsContextMenu Enabled="true" />
        <SettingsSearchPanel ColumnNames="" Visible="false" />
		<Settings ShowGroupPanel="false" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="true" />
		<SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true"/>
		    <SettingsPager PageSize="50">
        <PageSizeItemSettings Visible="true" ShowAllItem="true"  AllItemText="All Records" />
        </SettingsPager>
		<Styles>
			<Row Cursor="pointer" />
		</Styles>
        <ClientSideEvents 
            RowClick="Demo.gridData_RowClick" EndCallback="Demo.OnEndCallback" SelectionChanged="OnGridSelectionChanged"
            Init="Demo.gridData_Init"/>
        </dx:ASPxGridView>
        <dx:ASPxPopupControl ID="PopupControl" runat="server" ClientInstanceName="PopupControl" AllowDragging="True" AllowResize="true" 
            ShowFooter="True" PopupAction="None" CloseAction="OuterMouseClick" CloseOnEscape="true" PopupHorizontalAlign="OutsideRight"
            PopupVerticalAlign="Below"
            Width="620px" Height="500px"
            HeaderText="Results" PopupAnimationType="Fade"
            EnableViewState="False">
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
    <dx:ASPxGridViewExporter runat="server" ID="GridExporter" GridViewID="gridData" />
    <dx:ASPxCallbackPanel ID="PreviewPanel" runat="server" RenderMode="Div" Height="100%" CssClass="PreviewPanel" ClientInstanceName="ClientPreviewPanel">

    </dx:ASPxCallbackPanel>
    <dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" ClientVisible="false" ScrollBars="Vertical" ClientInstanceName="ClientFormPanel">
        <ClientSideEvents Init="Demo.Init" />
        <PanelCollection>
            <dx:PanelContent>
            <dx:ASPxFormLayout runat="server" ID="formLayout" ClientInstanceName="formLayout" CssClass="formLayout" AlignItemCaptionsInAllGroups="true">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
            <Items>
                <dx:LayoutGroup ColCount="4" GroupBoxDecoration="None" UseDefaultPaddings="false">
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
                          <dx:LayoutItem Caption="Request No">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbRequestNo" ClientInstanceName="ClientRequestNo"
                                        ReadOnly="true" Font-Bold="true">
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Technical No" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:aspxbuttonedit runat="server" ID="CmbTechnicalNo" ClientInstanceName="ClientTechnicalNo">
                                    <buttons>
                                            <dx:editbutton Text="link">
                                            </dx:editbutton>
                                        </buttons>
                                </dx:aspxbuttonedit>
                                    
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
                                    <dx:aspxbuttonedit id="CmbMarketingNo" clientinstancename="ClientMarketingNo" readonly="true"   
                                    width="100%" runat="server">
                                        <buttons>
                                            <dx:editbutton>
                                            </dx:editbutton>
                                        </buttons>
                                            
                                            <clientsideevents buttonclick="function(s, e) { OnBtnShowPopupClick('renew'); }" />
                                    </dx:aspxbuttonedit>
                                    
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="RefSamples" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbRefSamples" runat="server" ClientInstanceName="ClientRefSamples">
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
			<dx:LayoutItem Caption="R&D PIC" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="CmbReceiver" ClientInstanceName="ClientReceiver" DataSourceID="dsulogin"
                                        ValueField="user_name" TextField="fn" Width="100%" ReadOnly="true" runat="server">
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Customer" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbCustomer" runat="server" ClientInstanceName="ClientCustomer" ReadOnly="true">
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                        </dx:LayoutItem> 
                        <dx:LayoutItem Caption="Zone/Country" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxComboBox ID="CmbDestination" DropDownWidth="60%" runat="server" ClientInstanceName="ClientDestination" DataSourceID="dsCountry" ValueField="Code" TextField="Name">
                                           <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                           <Columns>
                                                    <dx:ListBoxColumn FieldName="Code" Width="120px" />
                                                    <dx:ListBoxColumn FieldName="Name" Width="100%" />
                                                </Columns>
                                       </dx:ASPxComboBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>  
                        <dx:LayoutItem Caption="Country" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbCountry" runat="server" ClientInstanceName="ClientCountry">
                                           <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxTextBox>
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
                                    <dx:ASPxDateEdit runat="server" ID="defrom" ClientInstanceName="ClientRequestDate" 
                                        DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                        <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" ValidationGroup="group1" />
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Nutrition" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxComboBox ID="CmbNutrition" DropDownWidth="330px" runat="server" ClientInstanceName="ClientNutrition" DataSourceID="dsNutrition" ValueField="Code" TextField="Name">
                                           <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                           <Columns>
                                                    <dx:ListBoxColumn FieldName="Code" Width="20%" />
                                                    <dx:ListBoxColumn FieldName="Name" />
                                                </Columns>
                                            
                                       </dx:ASPxComboBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>  
                        <dx:LayoutItem Caption="Pet Type" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxComboBox ID="CmbPetType" DropDownWidth="40%" runat="server" ClientInstanceName="ClientPetType" DataSourceID="dsPetType" ValueField="Code" TextField="Name">
                                           <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                           <Columns>
                                                    <dx:ListBoxColumn FieldName="Code" Width="20%" />
                                                    <dx:ListBoxColumn FieldName="Name" />
                                                </Columns>
                                       </dx:ASPxComboBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>  
                        <dx:LayoutItem Caption="Division" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxComboBox ID="CmbDivision" runat="server" ClientInstanceName="ClientDivision" DataSourceID="dsDivision" ValueField="PH2" TextField="PH2Des">
                                           <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                           <Columns>
                                                    <dx:ListBoxColumn FieldName="PH2" Width="20%" />
                                                    <dx:ListBoxColumn FieldName="PH2Des" />
                                                    <dx:ListBoxColumn FieldName="PDGROUP" />
                                                    <dx:ListBoxColumn FieldName="PRODUCT_LINE" />
                                                    <dx:ListBoxColumn FieldName="DIVISION" />
                                                </Columns>
                                       </dx:ASPxComboBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>  
                            
                        <dx:LayoutItem Caption="Remark" Width="100%" VerticalAlign="Middle">
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
                        
                        <dx:LayoutItem RowSpan="3" Caption="Code Product" Width="100%" VerticalAlign="Middle">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxButtonEdit ID="beCodeProduct" ClientInstanceName="ClientCodeProduct" ReadOnly="true"  
                                        Width="100%" runat="server">
                                            <Buttons>
                                                <dx:EditButton Image-IconID="edit_copy_16x16gray"/>
                                            </Buttons>
                                            </dx:ASPxButtonEdit>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>      
                        <dx:LayoutItem RowSpan="3" Caption="Material" Width="100%" VerticalAlign="Middle">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxButtonEdit ID="beresult" ClientInstanceName="ClientResult" ReadOnly="true" Width="100%" runat="server">
                                            <Buttons>
                                                <dx:EditButton Image-IconID="edit_copy_16x16gray"/>
                                            </Buttons>
                                            <ClientSideEvents ButtonClick="OnButtonClick" />
                                           <%--<ClientSideEvents ButtonClick="function(s, e) { 
                                                //setResult(ClientResult.GetText());
                                                    var n = prompt('Input result', '');
                                                    if (n != null) {
                                                        ClientResult.SetText(n);}" />     --%>
                                        </dx:ASPxButtonEdit>
                                        <%--<dx:ASPxTextBox id="tbresult" runat="server" clientinstancename="Clientresult" Font-Bold="true"
                                            Font-Size="Larger"/>--%>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem> 
                            <dx:LayoutItem RowSpan="3" Caption="Product Code" Width="100%" VerticalAlign="Middle">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxButtonEdit ID="CmbProductCode" ClientInstanceName="ClientProductCode" ReadOnly="true"  
                                        Width="100%" runat="server">
                                            <Buttons>
                                                <dx:EditButton Image-IconID="edit_copy_16x16gray"/>
                                            </Buttons>
                                            </dx:ASPxButtonEdit>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem RowSpan="3" Caption="Product Hierarchy (Sales Org.)" Width="100%" VerticalAlign="Middle">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxButtonEdit ID="CmbHierarchy" ClientInstanceName="ClientHierarchy" ReadOnly="true"  
                                        Width="100%" runat="server">
                                            <Buttons>
                                                <dx:EditButton Image-IconID="edit_copy_16x16gray"/>
                                            </Buttons>
                                            </dx:ASPxButtonEdit>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>   
                        </Items>
                    </dx:LayoutGroup>
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
                    <dx:LayoutGroup Caption="Material Structure" ColCount="1" GroupBoxDecoration="None" UseDefaultPaddings="false">
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
                        <dx:LayoutItem Caption="Product Group" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxComboBox ID="cmbProductGroup" DataSourceID="dsProductGroup" runat="server" EnableCallbackMode="True"
                                                   TextField="Name" ValueField="ProductGroup" IncrementalFilteringMode="Contains" TextFormatString="{0}; {1}"
                                                   EnableClientSideAPI="true" OnCallback="cmbProductGroup_Callback" 
                                                   ClientInstanceName="ClientProductGroup">
                                                   <Columns>
                                                       <dx:ListBoxColumn FieldName="ProductGroup" />
                                                       <dx:ListBoxColumn FieldName="Name" />
                                                       <%--<dx:ListBoxColumn FieldName="Hierarchy" />--%>
                                                   </Columns>
                                                   <ClientSideEvents SelectedIndexChanged="function(s, e) { 
                                                       //debugger;
                                                       //var grid = gLookUp.GetGridView();
                                                       //grid.PerformCallback('ValueChanged');
                                                       var value =s.GetSelectedItem().GetColumnText(2);
                                                       //tbProductGroup.SetText(value);
                                                       //tbProductGroup.GetText(s.SelectedItem.GetValue('Hierarchy'));
                                                       OnCountryChanged(s); }" ValueChanged="function(s, e) { OnValueChanged('ProductGroup');
                                                       }" />
                                                   <ClearButton DisplayMode="OnHover"></ClearButton>
                                               </dx:ASPxComboBox>
                                       <table style="display:none;">
                                           <tr>
                                               <td>
                                               </td>
                                               <td>&nbsp;<%--<dx:ASPxLabel ID="tbProductGroup" runat="server" ClientInstanceName="tbProductGroup" />--%></td>        
                                               <td><dx:ASPxButton ID="ASPxButton8" runat="server" AutoPostBack="False" RenderMode="Link" 
                                                   Image-IconID="actions_additem_16x16gray">
                                                   <ClientSideEvents Click="function(s, e) {OnBtnBuildClick('ProductGroup');}" />
                                                </dx:ASPxButton></td>
                                           </tr>
                                       </table>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem> 
                           <dx:LayoutItem Caption="Species+catch method+cert">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="cmbRawMaterial" ClientInstanceName="ClientRawMaterial" OnCallback="cmbRawMaterial_Callback"
                                                    ValueField="Code" runat="server" IncrementalFilteringMode="Contains" EnableSynchronization="False">
                                                    <Columns>
                                                          <dx:ListBoxColumn FieldName="Code" Width="20%" />
                                                          <dx:ListBoxColumn FieldName="Description" />
                                                    </Columns>
                                                    <ClearButton DisplayMode="OnHover"></ClearButton>
                                                    <ClientSideEvents ValueChanged="function(s, e) {OnValueChanged('RawMaterial');}" EndCallback="function(s, e) { OnEndCallback(s); }"/>
                                                </dx:ASPxComboBox>
                                        <table style="display:none;">
                                           <tr>
                                               <td>
                                                </td>
                                               <td>&nbsp;</td>
                                               
                                               <td><dx:ASPxButton ID="ASPxButton1" runat="server" AutoPostBack="False" RenderMode="Link" 
                                                   Image-IconID="actions_additem_16x16gray">
                                                   <ClientSideEvents Click="function(s, e) {OnBtnBuildClick('RawMaterial');}" />
                                                </dx:ASPxButton><%--<dx:ASPxTextBox ID="tbRawMaterial" runat="server" ClientInstanceName="tbRawMaterial" Width="113px">
                                                   <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                                   </dx:ASPxTextBox>--%></td>
                                           </tr>
                                       </table>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                           <dx:LayoutItem Caption="Style">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="cmbStyle" ClientInstanceName="ClientStyle"
                                                    OnCallback="cmbPackingStyle_Callback" EnableCallbackMode="True" IncrementalFilteringMode="Contains" EnableSynchronization="False"
                                                    ValueField="Code" runat="server">
                                                    <Columns>
                                                          <dx:ListBoxColumn FieldName="Code" Width="20%"/>
                                                          <dx:ListBoxColumn FieldName="Description" />
                                                    </Columns>
                                                    <ClearButton DisplayMode="OnHover"></ClearButton>
                                                    <ClientSideEvents ValueChanged="function(s, e) { OnValueChanged('Style');}" EndCallback="function(s, e) { OnEndCallback(s); }"/>
                                                </dx:ASPxComboBox>
                                    <table style="display:none;">
                                           <tr>
                                               <td>
                                                </td>
                                               <td>&nbsp;</td>
                                               <td><dx:ASPxButton ID="ASPxButton2" runat="server" AutoPostBack="False" 
                                                   RenderMode="Link" Image-IconID="actions_additem_16x16gray">
                                                   <ClientSideEvents Click="function(s,e) { OnBtnBuildClick('Style');}" />
                                                   </dx:ASPxButton>
                                               </td>
                                           </tr>
                                       </table>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                           <dx:LayoutItem Caption="Topping & Special Specification">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    
                                    <dx:ASPxComboBox ID="cmbTopping"  ClientInstanceName="ClientTopping" OnCallback="cmbTopping_Callback" 
                                                    ValueField="Code" runat="server" IncrementalFilteringMode="Contains" EnableSynchronization="False">
                                                    <Columns>
                                                          <dx:ListBoxColumn FieldName="Code" Width="20%"/>
                                                          <dx:ListBoxColumn FieldName="Description" />
                                                    </Columns>
                                                    <ClearButton DisplayMode="OnHover"></ClearButton>
                                                    <ClientSideEvents ValueChanged="function(s, e) {OnValueChanged('Topping');}" EndCallback="function(s, e) { OnEndCallback(s); }"/>
                                                </dx:ASPxComboBox>
                                    <table style="display:none;">
                                           <tr>
                                               <td>
                                                </td>
                                               <td>&nbsp;</td>
                                               <td><dx:ASPxButton ID="ASPxButton3" runat="server" AutoPostBack="False" RenderMode="Link" 
                                                   Image-IconID="actions_additem_16x16gray">
                                                   <ClientSideEvents Click="function(s, e) {OnBtnBuildClick('MediaType');}" />
                                                   </dx:ASPxButton></td>
                                           </tr>
                                       </table>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                           <dx:LayoutItem Caption="Size & Media & Net weight" Width="100%" VerticalAlign="Middle">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    
                                    <%--<dx:ASPxGridLookup ID="GridLookup" runat="server" KeyFieldName="ID" SelectionMode="Single" IncrementalFilteringMode="StartsWith"
                                        TextFormatString="{0} {1}" Width="200px" ClientInstanceName="gLookUp" OnInit="GridLookup_Init">
                                        <ClearButton DisplayMode="OnHover" />
                                        <Columns>
                                            <dx:GridViewDataTextColumn FieldName="Code" Width="100px" />
                                            <dx:GridViewDataTextColumn FieldName="CanSize" Width="100px" Caption="Container Type" />
                                            <dx:GridViewDataTextColumn FieldName="Type" Caption="Container Size" />
                                            <dx:GridViewDataTextColumn FieldName="Media" />
                                            <dx:GridViewDataTextColumn FieldName="NW" />
                                            <dx:GridViewDataTextColumn FieldName="NutritionType" />
                                        </Columns>
                                         
                                        <GridViewProperties>
                                            <SettingsBehavior AllowDragDrop="False" EnableRowHotTrack="True" />
                                            <SettingsPager NumericButtonCount="3" EnableAdaptivity="true" />
                                            <Settings ShowFilterRow="true" />
                                        </GridViewProperties>
                                    </dx:ASPxGridLookup>--%>
                                    <dx:ASPxComboBox ID="cmbSizeMedia" OnCallback="cmbSizeMedia_Callback" ClientInstanceName="ClientSizeMedia" 
                                                    ValueField="Code" runat="server" IncrementalFilteringMode="Contains" EnableSynchronization="False">
                                                    <Columns>
                                                        <dx:ListBoxColumn FieldName="Code" Width="20%"/>
                                                        <dx:ListBoxColumn FieldName="CanSize" Caption="Container Type" />
                                                        <dx:ListBoxColumn FieldName="Type" Caption="Container Size" />
                                                        <dx:ListBoxColumn FieldName="Media" />
                                                        <dx:ListBoxColumn FieldName="NW" />
                                                        <dx:ListBoxColumn FieldName="NutritionType" />
                                                    </Columns>
                                                    <ClearButton DisplayMode="OnHover"></ClearButton>
                                                    <ClientSideEvents ValueChanged="function(s, e) {OnValueChanged('SizeMedia');}" EndCallback="function(s, e) { OnEndCallback(s); }"/>
                                                </dx:ASPxComboBox>
                                    <table style="display:none;">
                                           <tr>
                                               <td>
                                                </td>
                                               <td>&nbsp;</td>
                                               <td><dx:ASPxButton ID="ASPxButton4" runat="server" AutoPostBack="False" 
                                                   RenderMode="Link" Image-IconID="actions_additem_16x16gray">
                                                   <ClientSideEvents Click="function(s, e) {OnBtnBuildClick('CanSize');}" />
                                                   </dx:ASPxButton></td>
                                           </tr>
                                       </table>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                        <dx:LayoutItem Caption="Container & Lid Type" VerticalAlign="Middle" Width="100%">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxComboBox ID="CmbContainerLid" runat="server" ClientInstanceName="ClientContainerLid" ValueField="Code"  
                                                   OnCallback="CmbContainerLid_Callback" NullText="Select ..." IncrementalFilteringMode="Contains" EnableSynchronization="False">
                                                   <Columns>
                                                       <dx:ListBoxColumn FieldName="Code" Width="20%"/>
                                                       <dx:ListBoxColumn FieldName="ContainerType" />
                                                       <dx:ListBoxColumn FieldName="LidType" />
                                                   </Columns>
                                                    <ClearButton DisplayMode="OnHover"></ClearButton>
                                                   <%--<ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>--%>
                                                   <ClientSideEvents ValueChanged="function(s, e) {OnValueChanged('ContainerLid');}" EndCallback="function(s, e) { OnEndCallback(s); }"/>
                                                   </dx:ASPxComboBox>
                                       <table style="display:none;">
                                           <tr>
                                               <td></td>
                                               <td>&nbsp;</td>
                                               <td><dx:ASPxButton ID="ASPxButton5" runat="server" AutoPostBack="False" RenderMode="Link" 
                                                   Image-IconID="actions_additem_16x16gray">
                                                   <ClientSideEvents Click="function(s, e) {OnBtnBuildClick('ContainerLid');}" />
                                                   </dx:ASPxButton></td>
                                           </tr>
                                       </table>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem> 
                           <dx:LayoutItem Caption="Zone or Country Group">
                               <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="cmbGrade" ValueField="Code" OnCallback="cmbGrade_Callback"
                                                    ClientInstanceName="ClientGrade" runat="server" IncrementalFilteringMode="Contains" EnableSynchronization="False">
                                                    <Columns>
                                                          <dx:ListBoxColumn FieldName="Code" Width="120px"/>
                                                          <dx:ListBoxColumn FieldName="Description" />
                                                    </Columns>
                                                    <ClearButton DisplayMode="OnHover"></ClearButton>
                                                    <ClientSideEvents ValueChanged="function(s, e) { OnValueChanged('Grade');}" EndCallback="function(s, e) { OnEndCallback(s); }"/>
                                                </dx:ASPxComboBox>
                                    <table style="width:100%;display:none;">
                                           <tr>
                                               <td>
                                                </td>
                                               <td>&nbsp;</td>
                                               <td><dx:ASPxButton ID="ASPxButton6" runat="server" AutoPostBack="False" RenderMode="Link" 
                                                   Image-IconID="actions_additem_16x16gray">
                                                   <ClientSideEvents Click="function(s, e) {OnBtnBuildClick('Grade');}" />
                                                   </dx:ASPxButton></td>
                                           </tr>
                                       </table>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                           <dx:LayoutItem Caption="Customer & Brand">
                               <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="cmbZone" ValueField="Code" OnCallback="cmbZone_Callback"
                                                    ClientInstanceName="ClientZone" runat="server" IncrementalFilteringMode="Contains" EnableSynchronization="False">
                                                    <Columns>
                                                          <dx:ListBoxColumn FieldName="Code" Width="20%"/>
                                                          <dx:ListBoxColumn FieldName="Description" />
                                                    </Columns>
                                                    <ClearButton DisplayMode="OnHover"></ClearButton>
                                                    <ClientSideEvents ValueChanged="function(s, e) {OnValueChanged('Zone');}" EndCallback="function(s, e) { OnEndCallback(s); }"/>
                                                </dx:ASPxComboBox>
                                    <table style="display:none;">
                                           <tr>
                                               <td>
                                                </td>
                                               <td>&nbsp;<dx:ASPxLabel ID="bZone" runat="server" ClientInstanceName="bBrand" /></td>
                                               <td><%--<dx:ASPxTextBox ID="ASPxTextBox2" runat="server" ClientInstanceName="tbPackingStyle" />--%>
                                                   <dx:ASPxButton ID="ASPxButton7" runat="server" AutoPostBack="False" RenderMode="Link" Image-IconID="actions_additem_16x16gray">
                                                   <ClientSideEvents Click="function(s, e) {OnBtnBuildClick('Brand');}" />
                                                   </dx:ASPxButton></td>
                                           </tr>
                                       </table>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                          <dx:LayoutItem Caption="Packing Styles">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTextBox ID="tbPackingStyle" runat="server" ClientInstanceName="ClientPackingStyle">
                                            <ClientSideEvents TextChanged="function(s, e) {OnValueChanged('PackingStyles');}" />
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                        </Items>
                    </dx:LayoutGroup>

                    <dx:LayoutGroup Caption="Attached file" ColCount="4" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
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
                    <dx:LayoutGroup Caption="Status" ColCount="3" GroupBoxDecoration="Box" UseDefaultPaddings="false" Paddings-PaddingTop="10">
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
                        <dx:LayoutItem Caption="Send Document" Width="100%">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                              <dx:ASPxComboBox ID="CmbDisponsition" runat="server" TextField="Title" ValueField="ID" OnCallback="CmbDisponsition_Callback"
                                    Width="330px" ClientInstanceName="ClientDisponsition">
                                  <ClientSideEvents  SelectedIndexChanged="function(s, e) { OnIDChanged(s); }" />
                              </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
			<dx:LayoutItem Caption="Assignee" Name="Assignee" Width="100%" ClientVisible="false" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxGridLookup ID="CmbAssignee" runat="server" ClientInstanceName="CmbAssignee" DataSourceID="dsComponent" OnInit="CmbAssignee_Init"                                       
                                        Width="350px" KeyFieldName="user_name" TextFormatString="{0}" MultiTextSeparator=", ">
                                        <Columns>
                                        <dx:GridViewCommandColumn ShowSelectCheckbox="True" />
                                        <dx:GridViewDataColumn FieldName="Description" />
                                    </Columns>
                                    <GridViewProperties>
                                        <Settings ShowFilterRow="true" ShowStatusBar="Visible" />
                                    </GridViewProperties>
                                    <ClientSideEvents Validation="function(s,e) { e.IsValid = s.GetValue() == ''}" />
                                </dx:ASPxGridLookup>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Reason for rejection" Name="Reason" ClientVisible="false">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ClientInstanceName="CmbReason" ID="CmbReason" runat="server" OnCallback="CmbReason_Callback" 
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
    <asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetRequestForm" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="username" Name="user_name" PropertyName="['username']"/>
            <asp:ControlParameter ControlID="hftype" Name="type" PropertyName="['type']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsProductGroup" runat="server" ConnectionString="<%$ ConnectionStrings:LabConnectionString %>"
        SelectCommand="select ID,ProductGroup,concat('(',ProductGroup,'): ',Name) as 'Name' from tblProductGroup where GroupType='F' and IsActive='1'">
<%--        <SelectParameters>
            <asp:ControlParameter ControlID="FormPanel$formLayout$CmbNutrition" Name="Nutrition" PropertyName="Text"  />
        </SelectParameters>--%>
    </asp:SqlDataSource>
 
    <asp:SqlDataSource ID="dsReason" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasReason"/>
    <asp:SqlDataSource ID="dsulogin" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="select user_name,concat(firstname ,' ',lastname) as fn from ulogin" />
    <asp:SqlDataSource ID="dsCountry" runat="server" ConnectionString="<%$ ConnectionStrings:LabConnectionString %>"
       SelectCommand="select distinct Code, Description as Name from [transGrade] where ProductType='PF'">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsPetType" runat="server" ConnectionString="<%$ ConnectionStrings:LabConnectionString %>"
       SelectCommand="select Code,Name from HierPetType">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsNutrition" runat="server" ConnectionString="<%$ ConnectionStrings:LabConnectionString %>"
       SelectCommand="select Code,Name from HierNutrition">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsDivision" runat="server" ConnectionString="<%$ ConnectionStrings:LabConnectionString %>"
       SelectCommand="select * from HierDivision">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsStatusApp" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasStatusApp where levelapp in (5)"/> 
    <asp:SqlDataSource ID="dsComponent" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetCompoEditForm" SelectCommandType="StoredProcedure">
       <SelectParameters>
           <asp:ControlParameter ControlID="FormPanel$formLayout$CmbDisponsition" Name="action" PropertyName="Value"/>
           <asp:ControlParameter ControlID="hfGeID" Name="Id" PropertyName="['GeID']"/>
           <asp:ControlParameter ControlID="username" Name="user" PropertyName="['username']"/>
       </SelectParameters>
   </asp:SqlDataSource>
