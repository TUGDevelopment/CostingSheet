<%@ Control Language="C#" AutoEventWireup="true" CodeFile="QuotationForm.ascx.cs" Inherits="UserControls_QuotationForm" %>
<script type="text/javascript">
    var pagesymboll = null;
    var timerHandle = -1;
    var curentEditingIndex = 0;
    var curentKeys = 0;
    function OnExchangeRate(s, e) {
        //debugger;
        var Curr = ClientCurrency.GetValue();
        var Rate = seExchangeRate.GetValue();
        if (Curr != null && Rate !=null) {
            var arge = ["buildExch",curentKeys, Curr, Rate].join("|");
            build_Data(arge);
            OnValueChanged('sub',0);
        }
    }
    function gv_RowDblClick(s, e) {
        //debugger;
        var index = s.GetFocusedRowIndex();
        //curentEditingIndex = e.visibleIndex;
        //curentEditingIndex+=1;
        //alert(curentEditingIndex);
        s.StartEditRow(index);
        var Keys = s.GetRowKey(e.visibleIndex);
        gv.GetValuesOnCustomCallback(['reload', Keys].join('|'), function (r) {
            if (!r)
                return;
            RecieveGridValues(r);
        });
        //var value = 'PaymentTerm;Interest;Customer;ShipTo;Commission;Size;Freight;Insurance;Remark;Route;Incoterm;RequestNo;Currency;ExchangeRate';
        //var value = 'Freight;Insurance;Remark;Route;Incoterm;RequestNo;Currency;ExchangeRate';
        //value= 'PaymentTerm;Interest;Customer;ShipTo;Commission;Size;'.concat(value);
        //s.GetRowValues(index, value, RecieveGridValues);
    }
    function OnIDChanged(s) {
        var identi = s.GetValue().toString() == "3";
        //var group = sampleSplitter.("formLayout");
        //var p = sampleSplitter.GetPaneByName('gridContainer');
        formLayout.GetItemByName("Reason").SetVisible(identi);
    }
    //function Oninterest(s, e) {
    //    if (s.GetValue() < 4) {
    //        alert('> 4');
    //        s.SetText('');
    //    }
    //} 
    function ChangeBatchEditorValue(s, e) {
        //alert("value changed");
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
    function OnValueChanged(a,text) {
        //if (ClientCostingNo.GetText() == "") return;
        //var subcon = tbContainers.GetText == '' ? 0 : tbContainers.GetText();
        //var symbol= pagesymboll == null  ? 1 : pagesymboll
        if (a == 'sub') {
            //testgv.PerformCallback('updated|' + symbol + '|' + text);
            var arge = ["updated", text, ""].join("|");
            gv.PerformCallback(arge);
            //Demo.LoadName(symbol);
        }
        //var value = ["", text, CmbRoute.GetValue(),
        //    CmbSize.GetValue(),
        //    CmbIncoterm.GetText(),
        //    CmbPaymentTerm.GetValue(),
        //    Clientinterest.GetText()];
        //var key = value.join("|");
        if (CmbIncoterm.GetValue() != 'FOB') {
            //var value = [CmbRoute.GetText() ,CmbSize.GetText() ,CmbIncoterm.GetText(),CmbPaymentTerm.GetText()].join("|");
            gv.GetValuesOnCustomCallback(a + '|0', function (r) {
                if (!r)
                    return;
                tbInsurance.SetText(CmbIncoterm.GetValue() == 'CIF' ? r["Insurance"] : '0');
                tbFreight.SetText(r["Freight"]);
            });
        }
        //if (a == 'calcu')
        //    gvcal.PerformCallback('updated|0');
    }
    var focusedColumn;
    var prevFocusedColumn;
    function ChangeValues(s, e) {
        window.setTimeout(function () {
        //var indices = s.batchEditApi.GetRowVisibleIndices();
            //alert(indices);
            var invalue = s.batchEditApi.GetCellValue(curentEditingIndex, prevFocusedColumn);
            if (prevFocusedColumn == 1) {
                    for (var i = 0; i < s.columns.length; i++) {
                        if (parseInt("0" + s.columns[i].fieldName, 10) > 0)
                            s.batchEditApi.SetCellValue(curentEditingIndex, s.columns[i].fieldName, invalue);
                    }
            }
            s.UpdateEdit();
            //var arge = ["updated", 0, ""].join("|");
            //s.PerformCallback(arge);(arge);
        }, 0);
    }
    function OnFileUploadComplete(s, e) {
        gv.PerformCallback('upload|1');
        OnValueChanged('sub', 0);
        //debugger;
        //var symbol= pagesymboll == null  ? 1 : pagesymboll
        //Demo.LoadName(symbol);
    }
    function OnSelectedIndexChanged(s, e) {
        if (s.GetValue() == null)
            s.SetSelectedIndex(-1);
        var value = s.GetValue();
        Demo.SetEnable(value != "FOB");
        if (value != "FOB") {
            CmbRoute.SetIsValid(false);
            CmbSize.SetIsValid(false);
            tbFreight.SetIsValid(false);
            //CmbRoute.SetErrorText('*');
        }
        //OnValueChanged('tot',tbNumberContainer.GetText());
    }
    //function OnChanged(s, e) {
    //    //if (confirm("copy going to shipto")) {
    //    //}
    //    var a = s.GetValue().toString();
    //    gv.UpdateEdit();
    //    if (CmbPaymentTerm.GetValue() == null) {
    //        gv.PerformCallback('load|' + a);
    //        //var v = hfPayment.Get("value");
    //        //CmbPaymentTerm.SetValue(v);
    //    }
    //}
    function OnTermChanged(s, e) {
        if (s.GetValue() == null)
            s.SetSelectedIndex(-1);
        var b =((s.GetValue() == 'Y00S' || s.GetValue() == 'D000') ? false : true);
        Clientinterest.SetEnabled(b);
        var n = s.GetSelectedItem().GetColumnText(2); //Clientinterest.SetText(b==true?"6":"");
        if (n > 0) { //6% * (n-5)/365
            //Clientinterest.SetText(((6/100) * ((n - 5) / 365)).toFixed(2));
            Clientinterest.SetText(((((6/100) / 365)* n)*100).toFixed(6));
        } else
            Clientinterest.SetText(0);
        OnValueChanged('sub',0);
    }
    function SetButtonsVisibility(s) {
        if (s.cpKeyValue != undefined && s.cpKeyValue != "") {
            alert(s.cpKeyValue);
            delete s.cpKeyValue;
        }
        if (s.cpUpdatedMessage) {
            delete s.cpUpdatedMessage;
            //curentEditingIndex = 0;
            curentKeys = 0;
            Demo.ClearForm();
        }
        if (s.cpClone) {
            debugger;
            gv.GetValuesOnCustomCallback(s.cpClone, function (r) {
                if (!r)
                    return;
                RecieveGridValues(r);
            });
            delete s.cpClone;
        }
    }
    function OnChangedCosting(s, e) {
        //debugger;
        //Demo.ClearForm();
        ClientCurrency.SetText('USD');
        seExchangeRate.SetText(s.GetSelectedItem().GetColumnText(3));
        curentKeys = s.GetValue();
        var arge = ["reload",curentKeys].join("|");
        //build_Data(arge);
        //CmbCustomer.PerformCallback(arge);
        //ClientShipTo.PerformCallback(arge);
    }
    function build_Data(arge) {
        gv.PerformCallback(arge);
    }
    function RecieveGridValues(values) {
        //debugger;
        CmbPaymentTerm.SetValue(values["PaymentTerm"]);
        Clientinterest.SetText(values["Interest"]);
        //CmbCustomer.SetValue(values["Customer"]);
        //ClientShipTo.SetValue(values["ShipTo"]);
        //ClientCommission.SetValue(values["Commission"]);
        CmbSize.SetValue(values["Size"]);
        //tbNumberContainer.SetText(values[6]);
        tbFreight.SetText(values["Freight"]);
        tbInsurance.SetText(values["Insurance"]);
        //ClientRemark.SetText(values["Remark"]);
        CmbRoute.SetValue(values["Route"]);
        CmbIncoterm.SetValue(values["Incoterm"]);
        curentKeys = values["RequestNo"];
        ClientCurrency.SetText(values["Currency"]);
        seExchangeRate.SetText(values["ExchangeRate"]);
        //OnValueChanged('sub', values[6]);
        var result = ["edit", values["ID"]].join("|");
        //gvcal.PerformCallback(result);
        //GetBuildCalcu(true);
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
    function gv_OnContextMenuItemClick(sender, args) {
        debugger;
        if (args.objectType == "row") {
            if (args.item.name == "PDF" || args.item.name == "XLS" || args.item.name == "ExportToXLS") {
                //args.processOnServer = true;
                //args.usePostBack = true;
                 btexport.DoClick();
            } 
            if (args.item.name == "Clear")
                 gv.PerformCallback("Clear|0");
        }
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
    function OnActiveTabChanged(s, e) {
        //debugger;
        //var selectedIndex = s.GetActiveTab().name;
        var Index = s.GetActiveTabIndex();
        //var b = (Index == 0 || Index == 2);
        tabbedGroupPageControl.SetActiveTabIndex(Index);
        //gvcal.SetVisible(Index == 2);
        //testgv.SetVisible(Index == 0);
        //formLayout.GetItemByName("item_det").SetVisible(Index == 0);
        //formLayout.GetItemByName("item_cal").SetVisible(Index == 0);
        
    }
    //function OnChangeChanged(text,s) {
    //    var symbol = 1;
    //    if (pagesymboll != null)
    //        symbol = pagesymboll;
    //    var value = [text,symbol,s.GetText()];
    //    var key = value.join("|");
    //    cb.PerformCallback(key);
    //}
    function OnEndCallback(s, e) {
        if (s.cp_Arg) {
            alert('XX_' + s.cp_Arg);
            //delete (s.cp_Arg);
        }
    }
    function onValidation(s, e) {
        //debugger;
        e.isValid = e.value == null ? false : true;
        //e.errorText = "validation error!";
    }
 
    function insertSelection(command) {
        //alert(command);
        //curentEditingIndex = 0;
        //gv.PerformCallback([command, 0].join("|"));
        //gv.AddNewRow();
        OnBtnShowPopupClick(command);
    }
    function OnChangedCustomer(s, e) {
        debugger;
        if (s.GetValue() == null) {
            s.SetSelectedIndex(-1);
            return;
        }
        //CmbPaymentTerm.PerformCallback(s.GetText());
        //CmbPaymentTerm.SetValue(s.GetSelectedItem().GetColumnText(2));
    }
    function OnBatchStartEdit(s, e) {
        var keyIndex = e.visibleIndex;
        var key = s.GetRowKey(keyIndex);
        //alert('DD_' + key);
    }
    function getId(s, e) {
        curentEditingIndex = e.visibleIndex;
        prevFocusedColumn = focusedColumn;
        focusedColumn = e.focusedColumn.fieldName; 
        var currentRow = s.batchEditApi.GetCellValue(curentEditingIndex, "Name");
        if (currentRow == "MinPrice" || currentRow == "OfferPrice") {
            e.cancel = true;
        }
        //var value = s.batchEditApi.GetCellValue(e.visibleIndex, focusedColumn);
        //alert(value + key);
    }
    function onBeginCallback(s, e) {
        if (e.command == "CANCELEDIT" || e.command == "UPDATEEDIT")
            GetBuildCalcu(false);
        if (e.command == "ADDNEWROW" || e.command == "STARTEDIT")
            GetBuildCalcu(true);
    }
    function GetBuildCalcu(b) {
        debugger;
        //var arr = "Currency;PaymentTerm;Actual_interest;Commission;Incoterm;Route;Size;Freight;Insurance;gvcal";
        //var arr = "Currency";
        //var g = arr.split(";"), s;
        //for (s = 0; s < g.length; s++) {
        //    if (g[s] != "")
        //        formLayout.GetItemByName(g[s]).SetVisible(b);
        //}
    }
    //function OnButtonClick(s, e) {
    //}
    function OnBtnShowPopupClick(view) {
        var url = 'popupControls/selcosting.aspx?view='+view;
        PopupControl.RefreshContentUrl();
        PopupControl.SetContentUrl(url);
        PopupControl.SetHeaderText(url);
        PopupControl.Show();
    }
    function HidePopupAndShowInfo(closedBy, returnValue) {
        PopupControl.Hide();
        if (closedBy == 'new') {
            var arge = [closedBy,"0", returnValue].join("|");
            gv.PerformCallback(arge);
        }
        if (closedBy == 'edit') {

        }
        if (closedBy == 'cost') {
            debugger;
            gv.GetValuesOnCustomCallback('cost|' + returnValue, function (r) {
                if (!r)
                    return;
                gv.batchEditApi.SetCellValue(curentEditingIndex, 'Code', r["Code"]);
                gv.batchEditApi.SetCellValue(curentEditingIndex, 'Name', r["Name"]);
                updategrid(gv);
            });
        }
        //gv.SetEditValue('RequestNo', returnValue);
    }
    function OnBatchEditStartEditing(s, e) {
        curentEditingIndex = e.visibleIndex;
        //var currentCountry = ClientintGrid.batchEditApi.GetCellValue(curentEditingIndex, "SAPMaterial");
        //if (currentCountry != lastCombo && e.focusedColumn.fieldName == "RawMaterial" && currentCountry != null) {
        //    lastCombo = currentCountry;
        //    RefreshData(currentCountry);
        //}
    }
    function OnBatchEditEndEditing(s, e) {
        updategrid(s);
    }
    function updategrid(s) {
        window.setTimeout(function () {
            s.UpdateEdit();
        }, 0);
    }
    function OnInitGridBatch(s, e) {
        ASPxClientUtils.AttachEventToElement(s.GetMainElement(), "keydown", function (evt) {
            if (evt.keyCode === ASPxClientUtils.StringToShortcutCode("Enter") || evt.keyCode === ASPxClientUtils.StringToShortcutCode("DOWN")) {
                DownPressed(s);
                ASPxClientUtils.PreventEventAndBubble(evt);
            }
        });
    }
    function DownPressed(s) {
        var lastRecordIndex = s.GetTopVisibleIndex() + s.GetVisibleRowsOnPage() - 1;
        if (curentEditingIndex < lastRecordIndex)
            s.batchEditApi.StartEdit(curentEditingIndex + 1, FocusedCellColumnIndex);
        else
            s.batchEditApi.EndEdit();
        //s.UpdateEdit();
    }
    function OnGridSelectionChanged(s, e) {
        setGridHeaderCheckboxes(s, e);
    }
    function setGridHeaderCheckboxes(s, e) {
        //cbAll
        debugger;
        var indexes = gridData.cpIndexesSelected;
        //cbAll.SetChecked(s.GetSelectedRowCount() == Object.size(indexes));
        var view = Demo.DemoState.View;
        var RowCount = gridData.GetSelectedRowCount();
        //cbPage
        var allEnabledRowsOnPageSelected = true;
        var indexes = gridData.cpIndexesUnselected;
        var topVisibleIndex = gridData.GetTopVisibleIndex();
        for (var i = topVisibleIndex; i < topVisibleIndex + gridData.cpPageSize; i++) {
            if (indexes.indexOf(i) == -1)
                if (!gridData.IsRowSelectedOnPage(i)) allEnabledRowsOnPageSelected = false;
        }
        ClientActionMenu.GetItemByName("Approve").SetVisible(view == "MailList" && RowCount > 0);
        ClientActionMenu.GetItemByName("Reject").SetVisible(view == "MailList" && RowCount > 0);
        //cbPage.SetChecked(allEnabledRowsOnPageSelected);
    }
</script>
<dx:ASPxHiddenField ID="hfid" runat="server" ClientInstanceName="hfid" />
<dx:ASPxHiddenField ID="hfStatusApp" runat="server" ClientInstanceName="hfStatusApp" />
<dx:ASPxHiddenField ID="username" runat="server" ClientInstanceName="username"/>
<dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
<dx:ASPxHiddenField ID="approv" runat="server" ClientInstanceName="approv" />
<dx:ASPxHiddenField ID="hftype" runat="server" ClientInstanceName="hftype" />
<dx:ASPxHiddenField ID="hfpara" runat="server" ClientInstanceName="hfpara" />
<dx:ASPxHiddenField ID="usertp" runat="server" ClientInstanceName="usertp" />
<%--<dx:ASPxTreeList ID="treeList" runat="server" AutoGenerateColumns="False" DataSourceID="DepartmentsDataSource"
        Width="100%" KeyFieldName="ID" ParentFieldName="ParentID" OnHtmlRowPrepared="treeList_HtmlRowPrepared"
        OnHtmlDataCellPrepared="treeList_HtmlDataCellPrepared">
    <Columns>
        <dx:TreeListDataColumn FieldName="DepartmentName" Caption="Department" VisibleIndex="0" />
        <dx:TreeListDataColumn FieldName="Location" VisibleIndex="1" />
        <dx:TreeListDataColumn FieldName="Budget" VisibleIndex="2" DisplayFormat="{0:C}" Name="budget" />
        <dx:TreeListDataColumn FieldName="Phone1" VisibleIndex="3" Caption="Phone" />
    </Columns>
    <Settings GridLines="Both" />
    <SettingsBehavior AutoExpandAllNodes="True" ExpandCollapseAction="NodeDblClick" />
    <Styles>
        <Cell>
            <Border BorderColor="White" />
        </Cell>
    </Styles>
</dx:ASPxTreeList>--%>
<dx:ASPxGridView runat="server" ID="gridData" KeyFieldName="ID" DataSourceID="dsgv" ClientInstanceName="gridData" Width="100%"  
        AutoGenerateColumns="true" OnCustomDataCallback="gridData_CustomDataCallback" OnCustomCallback="gridData_CustomCallback" 
        OnContextMenuItemClick="gridData_ContextMenuItemClick" OnCommandButtonInitialize="gridData_CommandButtonInitialize" Border-BorderWidth="0">
        <Columns>
            <dx:GridViewCommandColumn ShowNewButton="true" ShowEditButton="true" VisibleIndex="0" ButtonRenderMode="Image" Width="45px">
            <CustomButtons>
                <dx:GridViewCommandColumnCustomButton ID="Clone">
                    <Image ToolTip="Clone Record" Url="~/Content/Images/Copy.png"/>
                </dx:GridViewCommandColumnCustomButton>
            </CustomButtons>
            </dx:GridViewCommandColumn>
            <%--<dx:GridViewDataTextColumn FieldName="Company" />
            <dx:GridViewDataTextColumn FieldName="MarketingNumber" Caption="Costing No" />  
            <dx:GridViewDataTextColumn FieldName="CanSize" Caption="CanSize" />--%> 
            <dx:GridViewCommandColumn ShowSelectCheckbox="True" ShowClearFilterButton="true" SelectAllCheckboxMode="None" Width="45" VisibleIndex="0"/>
        
            <dx:GridViewDataTextColumn FieldName="ID" Width="30px"/>
            <dx:GridViewDataTextColumn FieldName="Customer" Width="25%"/>
            <dx:GridViewDataTextColumn FieldName="ShipTo" Width="25%"/> 
            <dx:GridViewDataTextColumn FieldName="RequestNo"/>
            <dx:GridViewDataTextColumn FieldName="Requester"/>
            <dx:GridViewDataTextColumn FieldName="PaymentTerm"/>
            <dx:GridViewDataTextColumn FieldName="Incoterm"/>
            <dx:GridViewDataComboBoxColumn FieldName="StatusApp" GroupIndex="0">
                <PropertiesComboBox DataSourceID="dsStatusApp" TextField="Title" ValueField="ID" />
            </dx:GridViewDataComboBoxColumn>
            <%--<dx:GridViewDataTextColumn FieldName="Revised" Width="70px"/>--%>
            <dx:GridViewDataDateColumn FieldName="CreateOn" Caption="CreateOn"> 
                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                </PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
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
        <ClientSideEvents 
            ContextMenuItemClick="function(s,e) { OnContextMenuItemClick(s, e); }"
            RowClick="Demo.gridData_RowClick" EndCallback="Demo.OnEndCallback"
            CustomButtonClick="OnCustomButtonClick" SelectionChanged="OnGridSelectionChanged"
            Init="Demo.gridData_Init"/>
    </dx:ASPxGridView>
    <dx:ASPxGridViewExporter runat="server" ID="GridExporter" GridViewID="gridData" />
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
    <dx:ASPxCallback ID="cb" ClientInstanceName="cb" runat="server" OnCallback="cb_Callback">
        <ClientSideEvents CallbackComplete="function (s, e) { var test =e.result; }" />
    </dx:ASPxCallback>
    <dx:ASPxCallbackPanel ID="PreviewPanel" runat="server" RenderMode="Div" Height="100%" CssClass="PreviewPanel" ClientInstanceName="ClientPreviewPanel" ClientVisible="false"
        OnCallback="PreviewPanel_Callback" />
    <dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" ClientVisible="false" ScrollBars="Vertical" ClientInstanceName="ClientFormPanel">
        <ClientSideEvents Init="Demo.Init" />
        <PanelCollection>
            <dx:PanelContent>
            <dx:ASPxFormLayout runat="server" ID="formLayout" ClientInstanceName="formLayout" CssClass="formLayout" AlignItemCaptionsInAllGroups="true">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
            <Items>
                <dx:LayoutGroup Caption="Calculate" ColCount="4" GroupBoxDecoration="None" UseDefaultPaddings="false">
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
                                    <dx:ASPxTextBox runat="server" ID="tbRequestNo" ClientInstanceName="ClientRequestNo" BackColor="#cccccc"
                                        ReadOnly="true" Font-Bold="true">
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    <dx:LayoutItem Caption="Sold-to">
                    <SpanRules>
                        <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                        <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                    </SpanRules>
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer>
                        <dx:ASPxComboBox ID="CmbCustomer" runat="server" ValueField="Code" IncrementalFilteringMode="Contains"  
                                ClientInstanceName="ClientCustomer" DataSourceID="dsCustomer" TextField="Name" TextFormatString="{0};{1}">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="Code"/>
                                    <dx:ListBoxColumn FieldName="Name"/>
                                </Columns>
                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                            </dx:ASPxComboBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Ship-to">
                    <SpanRules>
                        <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                        <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                    </SpanRules>
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer>
                            <dx:ASPxComboBox ID="CmbShipTo" runat="server" ValueField="Code" IncrementalFilteringMode="Contains" 
                                ClientInstanceName="ClientShipTo" DataSourceID="dsCustomer" TextField="Name" TextFormatString="{0};{1}">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="Code"/>
                                    <dx:ListBoxColumn FieldName="Name"/>
                                </Columns>
<%--                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>--%>
                            </dx:ASPxComboBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Currency" Name="Currency">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <table>
                                    <tr>
                                        <td>
                                        <dx:ASPxComboBox runat="server" ID="CmbCurrency" 
                                            ClientInstanceName="ClientCurrency" Width="80px" DataSourceID="dsCurrency" 
                                                ValueField="value" TextField="Value">
                                                <ClientSideEvents ValueChanged="OnExchangeRate" />
                                            </dx:ASPxComboBox>
                                        </td><td>&nbsp;=&nbsp;</td><td>
                                        <dx:ASPxSpinEdit runat="server" ID="seExchangeRate" 
                                            ClientInstanceName="seExchangeRate" NumberType="Float" 
                                            Number="0.00" Width="80px">
                                            <ClientSideEvents ValueChanged="OnExchangeRate" />
                                        </dx:ASPxSpinEdit>
                                        </td><td>&nbsp;THB</td>
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
                                        NullText="Example : xx %" Width="50px">
                                            <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                            ValueChanged="function(s, e) { OnValueChanged('sub',0);}" />
                                            <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                        </dx:ASPxComboBox></td>
                                        <td>&nbsp;</td>
                                        <td>%&nbsp;</td>
                                        <td>Brand&nbsp;</td>
                                        <td><dx:ASPxComboBox runat="server" ID="CmbBrand" 
                                            ClientInstanceName="ClientBrand" Width="80px" DataSourceID="dsBrand" 
                                                ValueField="Code" TextField="Name">
                                            <Columns>
                                                <dx:ListBoxColumn FieldName="Code" />
                                                <dx:ListBoxColumn FieldName="Name" />
                                            </Columns>
                                            </dx:ASPxComboBox></td>
                                    </tr>
                                    </table>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Payment Term" Name="PaymentTerm">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbPaymentTerm" runat="server" ClientInstanceName="CmbPaymentTerm" NullText="Select term of payment"
                                    EnableCallbackMode="true" CallbackPageSize="500" ValueField="Code" TextFormatString="{0};{1}" 
                                    DataSourceID="dsPaymentTerm">   
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="Code"/>
                                        <dx:ListBoxColumn FieldName="Value" Width="240px"/>
                                        <dx:ListBoxColumn FieldName="LeadTime" Caption="No. of Days" Width="80px"/>
                                    </Columns>
                                    <ClientSideEvents SelectedIndexChanged="OnTermChanged" />
                                </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Actual interest" Name="Actual_interest">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                     <table>
                                        <tr>
                                        <td>
                                            <dx:ASPxTextBox runat="server" ID="tbinterest" ClientInstanceName="Clientinterest" 
                                                Width="120px" ReadOnly="true"/> </td>
                                        <td>&nbsp;</td>
                                        <td></td>
                                    </tr>
                                    </table>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Incoterm" Name="Incoterm">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbIncoterm" runat="server" ClientInstanceName="CmbIncoterm" DataSourceID="dsIncoterm" NullText=""
                                    ValueField="Code" TextFormatString="{0};{1}">
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="Code"/>
                                        <dx:ListBoxColumn FieldName="Name"/>
                                    </Columns>
                                    <ClientSideEvents SelectedIndexChanged="OnSelectedIndexChanged" ValueChanged="function(s, e) {  
                                                        OnValueChanged('tot',0);}" />
                                </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>                       
                        <dx:LayoutItem Caption="Route" Name="Route">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="CmbRoute" runat="server" ClientInstanceName="CmbRoute" DataSourceID="dsRoute" NullText=""
                                        TextFormatString="{0};{1}" EnableCallbackMode="true" IncrementalFilteringMode="Contains" EnableSynchronization="False"
                                        ValueField="Code" 
                                        CallbackPageSize="1000">
                                        <Columns>
                                        <dx:ListBoxColumn FieldName="Code"/>
                                        <dx:ListBoxColumn FieldName="Name"/>
                                        </Columns>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) {  if (s.GetValue() == null) s.SetSelectedIndex(-1);}" />
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Size" Name="Size">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="CmbSize" runat="server" ClientInstanceName="CmbSize" DataSourceID="dsContainerType" NullText=""
                                        TextFormatString="{0};{1}" ValueField="Code">
                                        <Columns>
                                        <dx:ListBoxColumn FieldName="Code"/>
                                        <dx:ListBoxColumn FieldName="Name"/>
                                        </Columns>
                                        <ClientSideEvents ValueChanged="function(s, e) {  
                                                        OnValueChanged('tot',0);}" 
                                            SelectedIndexChanged="function(s, e) {  if (s.GetValue() == null) s.SetSelectedIndex(-1);}"/>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Freight" Name="Freight">
                           <SpanRules>
                                    <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                    <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                                </SpanRules>
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <table>
                                        <tr>
                                        <td>
                                            <dx:ASPxTextBox ID="tbFreight" runat="server" ClientInstanceName="tbFreight">
                                        <%--<ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                ValueChanged="function(s, e) { OnValueChanged('sub',tbNumberContainer.GetText());}" />--%>
                                        </dx:ASPxTextBox></td>
                                        <td>&nbsp;(USD)</td>
                                        <td></td>
                                    </tr>
                                    </table>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Insurance" Name="Insurance">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <table>
                                        <tr>
                                        <td>
                                            <dx:ASPxTextBox ID="tbInsurance" runat="server" ClientInstanceName="tbInsurance" ReadOnly="true">
                                            <%--<ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                ValueChanged="function(s, e) { OnValueChanged('sub',tbNumberContainer.GetText());}" />--%>
                                        </dx:ASPxTextBox></td>
                                        <td>&nbsp;(USD)</td>
                                        <td></td>
                                    </tr>
                                    </table>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem> 
                        <dx:LayoutItem Caption="Notes" Width="100%">
                        <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer><dx:ASPxMemo runat="server" ID="mNotes" Rows="4" ClientInstanceName="ClientNotes">
                                    </dx:ASPxMemo>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                        <dx:LayoutItem Caption="Upload Adjust Cost" Name="Upload" Width="100%">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxUploadControl ID="Upload" runat="server" OnFileUploadComplete="Upload_FileUploadComplete" Width="200px" 
                                    ClientInstanceName="Upload" 
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
                                        <dx:ASPxGridView runat="server" ID="gv" Width="100%" ClientInstanceName="gv" OnBatchUpdate="gv_BatchUpdate"
                                            OnCustomCallback="gv_CustomCallback" OnDataBinding="gv_DataBinding"
                                            OnHtmlDataCellPrepared="gv_HtmlDataCellPrepared" OnDataBound="gv_DataBound"
                                            OnCustomDataCallback="gv_CustomDataCallback" OnFillContextMenuItems="gv_FillContextMenuItems" 
                                            OnCustomButtonCallback="gv_CustomButtonCallback" KeyFieldName="ID">
                                            <Columns>
                                             <dx:GridViewCommandColumn ShowClearFilterButton="true" Width="45px" ButtonRenderMode="Image" ShowInCustomizationForm="True"
                                                FixedStyle="Left" ShowNewButtonInHeader="true" ShowEditButton="true" ShowDeleteButton="true">   
                                                 <CustomButtons>
                                                    <dx:GridViewCommandColumnCustomButton ID="remove">
                                                        <Image ToolTip="Remove" Url="~/Content/Images/Cancel.gif"/>
                                                    </dx:GridViewCommandColumnCustomButton>
                                                     <%--<dx:GridViewCommandColumnCustomButton ID="Clone">
                                                        <Image ToolTip="Clone Record" Url="~/Content/Images/copy.gif"/>
                                                    </dx:GridViewCommandColumnCustomButton>--%>
                                                </CustomButtons>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <HeaderTemplate>
                                                    <dx:ASPxButton runat="server" RenderMode="Link" AutoPostBack="false">
                                                        <Image ToolTip="Insert" Url="~/Content/Images/icons8-plus-math-filled-16.png" />
                                                        <ClientSideEvents Click="function(s,e){ OnBtnShowPopupClick('new'); }" />
                                                    </dx:ASPxButton>
                                                </HeaderTemplate>
                                            </dx:GridViewCommandColumn>
                                            <%--<dx:GridViewDataButtonEditColumn FieldName="CostNo" Caption="CostingNo">
                                                <PropertiesButtonEdit>
                                                    <Buttons>
                                                        <dx:EditButton />
                                                    </Buttons>
                                                    <ClientSideEvents ButtonClick="function(s,e){ OnBtnShowPopupClick('edit'); }" />
                                                </PropertiesButtonEdit>
                                            </dx:GridViewDataButtonEditColumn>--%>
                                            <dx:GridViewDataColumn FieldName="ID" VisibleIndex="1"/>
                                            <dx:GridViewDataColumn FieldName="CostNo" Caption="CostingNo" ReadOnly="true"/>
                                            <%--<dx:GridViewDataColumn FieldName="Code"/>--%>
                                            <dx:GridViewDataButtonEditColumn FieldName="Code" ReadOnly="true" Width="12%">
                                                <PropertiesButtonEdit>
                                                    <Buttons>
                                                        <dx:EditButton>
                                                        </dx:EditButton>
                                                    </Buttons>
                                                    <ClientSideEvents ButtonClick="function(s,e){
                                                        debugger;
                                                        var CostNo = gv.batchEditApi.GetCellValue(curentEditingIndex, 'CostNo');
                                                            OnBtnShowPopupClick('cost&Costing=' + CostNo);
                                                        }" />
                                                </PropertiesButtonEdit>
                                            </dx:GridViewDataButtonEditColumn>    
                                            <dx:GridViewDataColumn FieldName="Name"/>
                                            
                                            <dx:GridViewDataColumn FieldName="Overprice"/>
                                            <dx:GridViewDataColumn FieldName="Extracost"/>      
                                            <dx:GridViewDataColumn FieldName="SubContainers"/> 
                                                
                                            <dx:GridViewDataColumn FieldName="ExchangeRate"/>
                                            <dx:GridViewDataColumn FieldName="MinPriceExch" Caption="MinPrice" EditFormSettings-Visible="False"/>
                                            <dx:GridViewDataColumn FieldName="OfferPriceExch" Caption="OfferPrice by System" EditFormSettings-Visible="False"/>
                                            
                                            <dx:GridViewDataColumn FieldName="OfferPrice" Visible="false" />
                                            <dx:GridViewDataColumn FieldName="OfferPrice_Adjust"/>
                                            <dx:GridViewDataTextColumn FieldName="StatusApp" Caption="below cost" />
                                            <dx:GridViewDataTextColumn FieldName="Diff" EditFormSettings-Visible="False" >
                                                <PropertiesTextEdit DisplayFormatString="f2" />
                                            </dx:GridViewDataTextColumn>
                                            <dx:GridViewDataTextColumn FieldName="PercentDiff" Caption="% Diff" EditFormSettings-Visible="False">
                                                <PropertiesTextEdit DisplayFormatString="f2" />
                                            </dx:GridViewDataTextColumn>
                                            <dx:GridViewDataColumn FieldName="Mark" Caption="Active" EditFormSettings-Visible="False" >
                                                <%--<HeaderStyle CssClass="hide" />
                                                <EditCellStyle CssClass="hide" />
                                                <CellStyle CssClass="hide" />
                                                <FilterCellStyle CssClass="hide" />
                                                <FooterCellStyle CssClass="hide" />
                                                <GroupFooterCellStyle CssClass="hide" />--%>
                                            </dx:GridViewDataColumn>
                                            </Columns>
                                            <Styles>
                                                <StatusBar CssClass="StatusBarWithButtons">
                                                </StatusBar>
                                            </Styles>
                                            <ClientSideEvents RowDblClick="gv_RowDblClick" ContextMenuItemClick="gv_OnContextMenuItemClick" 
                                                BatchEditEndEditing="OnBatchEditEndEditing" BatchEditStartEditing="OnBatchEditStartEditing"
                                                Init="OnInitGridBatch"/>
                                            <SettingsSearchPanel ColumnNames="" Visible="false" CustomEditorID="tbToolbarSearch"/>
		                                    <Settings ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="false" ShowStatusBar="Hidden"/>
		                                    <SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true" AllowFocusedRow="true" />
		                                    <SettingsPager PageSize="50">
                                            <PageSizeItemSettings Visible="true" ShowAllItem="true"  AllItemText="All Records" />
                                            </SettingsPager>
                                            <SettingsExport EnableClientSideExportAPI="true" ExcelExportMode="DataAware" />
                                            <SettingsContextMenu Enabled="true"/>
                                            <SettingsEditing Mode="Batch">
                                                 <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="Cell"/>
                                            </SettingsEditing>
                                        </dx:ASPxGridView>
                                        <dx:ASPxButton ID="btexport" runat="server" ClientInstanceName="btexport" 
                                            OnClick="btexport_Click" ClientVisible="false"/>
                                    <%--<dx:ASPxButton ID="btn" runat="server" ClientInstanceName="btn" ClientVisible="false" OnClick="btn_Click" />--%>
                                    </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>                  
                            </Items>
                                    </dx:LayoutGroup>
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
                                                    <dx:ASPxComboBox ClientInstanceName="ClientReason" ID="CmbReason" runat="server" DataSourceID="dsReason" 
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
                                        <dx:LayoutItem Caption="EventLog" Width="100%" Name="EventLog" CaptionStyle-Font-Bold="true">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer>
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
<asp:SqlDataSource ID="dsIncoterm" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select Code,Name from MasIncoterm  union select '0',''"/>
<asp:SqlDataSource ID="dsPaymentTerm" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select Code,Value,LeadTime from MasPaymentTerm union select '0','',0"/>
<asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spGetQuotaHeader" SelectCommandType="StoredProcedure">
    <SelectParameters>
        <asp:ControlParameter ControlID="username" Name="user_name" PropertyName="['user_name']"/>
        <asp:ControlParameter ControlID="hftype" Name="type" PropertyName="['type']"/>
    </SelectParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsStatusApp" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasStatusApp where levelapp in (3,2)"/>
<asp:SqlDataSource ID="dsReason" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasReason"/>
<asp:SqlDataSource ID="dsPrimary" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasPrimary where PF='1'"/>
<asp:SqlDataSource ID="dsCustomer" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select case when Code is null Or Code ='' then Custom else Code end as 'Code',Name from MasCustomer union select '0','None'">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsRoute" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select Code,Name from MasRoute union select '0',''">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsContainerType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select Code,Name from MasContainerType  union select '0',''">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsCommission" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select value from dbo.FNC_SPLIT(',0,1,2',',')">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsinterest" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select value from dbo.FNC_SPLIT('0,6,4',',')">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsuser" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="SELECT au_id,[user_name],firstname +' '+ lastname as fn,Position,Email from ulogin "/>
<%--<asp:SqlDataSource ID="dsGetcosting" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spGetCostQuota" SelectCommandType="StoredProcedure">
    <SelectParameters>
        <asp:ControlParameter ControlID="username" Name="user_name" PropertyName="['user_name']"/>
        <asp:ControlParameter ControlID="hfid" Name="Id" PropertyName="['hidden_value']"/>
    </SelectParameters>
</asp:SqlDataSource>--%>
<asp:SqlDataSource ID="dsCurrency" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select value from dbo.FNC_SPLIT('THB;USD;JPY;EUR;GBP',';') order by value">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsBrand" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select Code,Name from MasBrand">
</asp:SqlDataSource>
