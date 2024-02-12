<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CalculationForm.ascx.cs" Inherits="UserControls_CalculationForm" %>
<script type="text/javascript">
    var isCustomCallback = false;
    var curentEditingIndex = 0;
    function OnGridBeginCallback(s, e) {
        if (e.command == "CANCELEDIT" || e.command =="UPDATEEDIT")
            GetBuildCalcu(false);
        if (e.command == "ADDNEWROW")
            GetBuildCalcu(false);
    }
    function SetButtonsVisibility(s, e) {
        //AdjustSize();
        debugger;
        //if (!s.cpFilterExpression)
        //    return;
        //hfKeyword.Set("Keyword", s.cpFilterExpression);
        //s.cpFilterExpression = null;
        //s.PerformCallback("X");
        if (s.cpUpdatedMessage) {
            //if (s.cpUpdatedMessage == "Error") 
            //alert(s.cpUpdatedMessage);
            gvitems.PerformCallback("clear");
            delete s.cpUpdatedMessage;
            curentEditingIndex = 0;
            curentKeys = 0;
            ClearForm();
        }
    }
    function OnBatchEditStartEditing(s, e) {
        curentEditingIndex = e.visibleIndex;
    }
    function Combo_SelectedIndexChanged(s, e) {
        debugger;
        var name = s.GetSelectedItem().GetColumnText(1);//s.GetText().toString();
        //gv.GetValuesOnCustomCallback("Description|" + name, DataCallback);
        gv.SetEditValue('Description', name);
        gvit.PerformCallback('reload|'+s.GetSelectedItem().GetColumnText(0));
    }
    function DataCallback(result) {

        var results = result.split("|");
        switch (results[0]) {
            case "Description":
                gv.batchEditApi.SetCellValue(curentEditingIndex, "Description", results[1]);
                break;
        }
    }
    function OnEndCallback(s, e) {
        Demo.SetUnitPriceColumnVisibility();
        //if (s.IsEditing()) {
        //    debugger;
        //    var popup = s.GetPopupEditForm();
        //    popup.AdjustContentOnShow();
        //}
    }
    function PreventEditFormPopupAnimation(s) {
        var popup = s.GetPopupEditForm();
        if (popup) {
            popup.popupAnimationType = "none";
        }
    }
    function OnValueChanged(a, text) {
        //if (a == 'sub') {
        //    var arge = ["updated", text, ""].join("|");
        //    gvcal.PerformCallback(arge);
        //}
        var value = ["", text, CmbRoute.GetValue(),
            CmbSize.GetValue(),
            CmbIncoterm.GetText(),
            CmbPaymentTerm.GetValue(),
            Clientinterest.GetText(),
            ClientCommission.Get("Commis")];
        var key = value.join("|");
        if (CmbIncoterm.GetValue() != 'FOB') {
            var value = [CmbRoute.GetText(), CmbSize.GetText(), CmbIncoterm.GetText(), CmbPaymentTerm.GetText()].join("|");
            gvitems.GetValuesOnCustomCallback(a + '|' + key, function (r) {
                if (!r)
                    return;
                tbInsurance.SetText(CmbIncoterm.GetValue() == 'CIF' || 'CFR' ? r["Insurance"] : '0');
                tbFreight.SetText(r["Freight"]);
            });
        } else if (CmbIncoterm.GetValue() == 'FOB') {
            tbInsurance.SetText('');
            tbFreight.SetText('');
            CmbRoute.SetSelectedIndex(-1);
            CmbSize.SetSelectedIndex(-1);
            gvitems.PerformCallback('FOB|0');
        }
        //if (a == 'calcu')
        //    gvcal.PerformCallback('updated|0');
    }
    //function OnSelectedIndexChanged(s, e) {
    //    var key = s.GetValue();
    //    var res = ['reload', key].join('|');
    //    gvcal.PerformCallback('reload');
    //    gvcal.GetValuesOnCustomCallback(res, function (r) {
    //        if (!r)
    //            return;
    //        debugger;
    //        seExchangeRate.SetText(r["ExchangeRate"]);
    //        ClientCurrency.SetText(r["Currency"]);
    //        })
    //    //Demo.SetEnable(value != "FOB");
    //    //OnValueChanged('tot',tbNumberContainer.GetText());
    //}
    function OnExchangeRate(s, e) {
        if(s.GetValue()>0)
        gvitems.PerformCallback("Rate|" + s.GetValue());
    }
    function OnTermChanged(s, e) {
        if (s.GetValue() == null)
            s.SetSelectedIndex(-1);
        var b = ((s.GetValue() == 'Y00S' || s.GetValue() == 'D000') ? false : true);
        Clientinterest.SetEnabled(b);
        var n = s.GetSelectedItem().GetColumnText(2);
        //Clientinterest.SetText(b==true?"6":"");
        if (n > 0) {
            //6% * (n-5)/365
            //Clientinterest.SetText((6 * ((n - 5) / 365)).toFixed(2));
            Clientinterest.SetText(((((6/100) / 365)* n)*100).toFixed(2));
        } else
            Clientinterest.SetText(0);
    }
    function OnActiveTabChanged(s, e) {
       var Index = s.GetActiveTabIndex();
        //var b = (Index == 0 || Index == 2);
        tabbedGroupPageControl.SetActiveTabIndex(Index);
        gvutilize.SetVisible(Index == 0);
        gvitems.SetVisible(Index != 0);
        gvitems.PerformCallback('symbol|' + Index);
    }
    function OnDateChanged() {
        debugger;
        var inputCode = ClientMaterial.GetText();
        var inputfrom = ClientValidfrom.GetValue();
        var inputto = ClientValidto.GetValue();
        if (inputfrom != null && inputto != null && inputCode != null) {
            //return alert('The Valid Date should not be empty...');
            gvutilize.PerformCallback('reload');
            var Keys = 0;
            gvutilize.GetValuesOnCustomCallback(['build', Keys].join('|'), function (r) {
                if (!r)
                    return;
                //seExchangeRate.SetText(r["Rate"]);
                //var code = ClientMaterial.GetText();
                var res = 'reload|' + inputCode + '|0';
                gvitems.PerformCallback(res);
            });      
        }
    }
    var FocusedCellColumnIndex = 0;
    var FocusedCellRowIndex = 0;
    function OnInitGridBatch(s, e) {
        ASPxClientUtils.AttachEventToElement(s.GetMainElement(), "keydown", function (evt) {
            if (evt.keyCode == 40) {
                ASPxClientUtils.PreventEventAndBubble(evt);
                DownPressed(s);
            }
            //if (evt.keyCode == 13) {
            //    s.UpdateEdit();
            //}
        });
    }
    function OnBatchEditStartAlimConstat(s, e) {
        FocusedCellColumnIndex = e.focusedColumn.index;
        FocusedCellRowIndex = e.visibleIndex;
    }
    function OnEndEditCell(s, e) {
        FocusedCellColumnIndex = 0;
        FocusedCellRowIndex = 0;
        debugger;
        var cellInfo = s.batchEditApi.GetEditCellInfo();
        setTimeout(function () {
            if (s.batchEditApi.HasChanges(cellInfo.rowVisibleIndex, cellInfo.column.index))
                UpdateEdit(createObject(s, s.GetRowKey(e.visibleIndex), e.rowValues), cellInfo);
        }, 0);
        window.setTimeout(function () {
            s.UpdateEdit();
        }, 0);
    }
    function OnEndUpdateCallback(s, e) {
        debugger;
        //lvwd.PerformCallback(['edit',0].join('|'));
        var result = e.result;
        if (result == "OK") {
            var code = ClientMaterial.GetText();
            var res = 'reload|' + code+'|0';
            gvitems.PerformCallback(res);
        }
    }
    function UpdateEdit(object, cellInfo) {
        //debugger;
        //lvwd.PerformCallback('edit|'+[object].join('|'));
        callback.cpCellInfo = cellInfo;
        callback.PerformCallback(JSON.stringify(object));
    }
    function createObject(g, key, values) {
        var object = {};
        object["ID"] = key;
        Object.keys(values).map(function (k) {
            object[g.GetColumn(k).fieldName] = values[k].value;
        });
        return object;
    }
    function DownPressed(s) {
        var lastRecordIndex = s.GetTopVisibleIndex() + s.GetVisibleRowsOnPage() - 1;
        if (FocusedCellRowIndex < lastRecordIndex)
            s.batchEditApi.StartEdit(FocusedCellRowIndex + 1, FocusedCellColumnIndex);
        else
            s.batchEditApi.EndEdit();
    }
    function OnFileUploadComplete(s, e) {
            var value = ["'upload","1"].join('|');
            gv.PerformCallback(value);
    }
    //function OnRowClick(s, e) {
    //    debugger;
    //    //var index = s.GetFocusedRowIndex();
    //    //alert(curentEditingIndex);
    //    //s.StartEditRow(e.visibleIndex);
    //    var key = s.GetRowKey(e.visibleIndex);
    //    s.GetValuesOnCustomCallback(['build' ,key].join('|'), function (r) {
    //        if (!r)
    //            return;
    //        RecieveGridValues(r);
    //        })
    //}
    function OnCountryChanged(s, e) {

    }
    function gv_RowDblClick(s, e) {
        debugger;
        var index = s.GetFocusedRowIndex();
        curentEditingIndex = e.visibleIndex;
        curentEditingIndex += 1;
        //alert(curentEditingIndex);
        s.StartEditRow(index);
        var Keys = s.GetRowKey(e.visibleIndex);
        s.GetValuesOnCustomCallback(['build', Keys].join('|'), function (r) {
            if (!r)
                return;
            RecieveGridValues(r);
        });
        //var value = 'PaymentTerm;Interest;Customer;ShipTo;Commission;Size;Freight;Insurance;Remark;Route;Incoterm;RequestNo;Currency;ExchangeRate';
        //var value = 'Freight;Insurance;Remark;Route;Incoterm;RequestNo;Currency;ExchangeRate';
        //value= 'PaymentTerm;Interest;Customer;ShipTo;Commission;Size;'.concat(value);
        //s.GetRowValues(index, value, RecieveGridValues);
    }
    function RecieveGridValues(r) {
        //alert(r["ID"]);
        var res = 'reload';
        ClientValidfrom.SetText(r["From"]);
        ClientValidto.SetText(r["To"]);
        ClientMaterial.SetText(r["Material"]);
        //OnDateChanged();
        gvutilize.PerformCallback('reload');
        gvitems.PerformCallback([res, r["Material"], r["ID"]].join('|'));
        tcDemos.SetActiveTabIndex(0);
        GetBuildCalcu(true);
        debugger;
        if (r["SubContainers"]!= "")
            insertSelection(7, 'Insert', r["SubContainers"]);
        //ClientMaterial.SetText(r["Material"]);
    }
    function GetBuildCalcu(b) {
        debugger;
        tcDemos.SetVisible(b);
        tabbedGroupPageControl.SetVisible(b);
        var arr = "utilize;SubTotal";
        var g = arr.split(";"), s;
        for (s = 0; s < g.length; s++) {
            if (g[s] != "")
                formLayout.GetItemByName(g[s]).SetVisible(b);
        }
    }
    function OnSetValueToGridview(text, evt) {
        //if (tcDemos.GetActiveTabIndex() != 8)
        //    tcDemos.SetActiveTabIndex(8);
            //for (var indexa = gvitems.GetTopVisibleIndex(); indexa < gvitems.GetVisibleRowsOnPage(); indexa++) {
            //    var invalue = gvitems.batchEditApi.GetCellValue(indexa, "Name");
            //    if (invalue == text)
            //        switch (text) {
            //            case "OverPrice":
            //                gvitems.batchEditApi.SetCellValue(indexa, "Price", evt, null, true);
            //                break;
            //            default:
            //                gvitems.batchEditApi.SetCellValue(indexa, "Quantity", evt, null, true);
            //                break;
            //        }
            //}
        if (ClientCustomer.GetValue() == null || ClientShipTo.GetValue() == null) return alert('material should not be empty...');

        gvitems.PerformCallback(["updated",text,evt].join('|'));   
        //window.setTimeout(function () {
        //    gvitems.UpdateEdit();
        //}, 0);
    }
    function OnSelValueChanged(s, evt) {
        if (evt == 'UpCharge') {
            hfUpchargeGroup.Set('Upcharge', s.GetSelectedItem().GetColumnText(0));
            tbUpChargePrice.SetText(s.GetSelectedItem().GetColumnText(2));
            tbUpCurrency.SetText(s.GetSelectedItem().GetColumnText(3));
            tbUpChargeUnit.SetText(s.GetSelectedItem().GetColumnText(4));
            tbUpChargeQuantity.Focus();  
        }
    }
    function insertSelection(index,command,text) {
        debugger;
        if (command == 'Insert') {
            //if (ClientUpCharge.GetText() != null || ClientUpCharge.GetText() != '')
                gvitems.PerformCallback("Insert|" + index +"|"+text);
        }
        if (command == 'new') {
            curentEditingIndex = 0;
            gv.AddNewRow();
        }
    }
    function OnSelectedIndexChanged(s, e) {
        if (s.GetValue() == null)
            s.SetSelectedIndex(-1);
        var value = s.GetValue();
        Demo.SetEnable(value != "FOB");
    }
    function OnChangedCustomer(s, e) {
        debugger;
        if (s.GetValue() == null) {
            s.SetSelectedIndex(-1);
            return;
        } else
            ClientMargin.SetText("3");
    }
    function gvitems_EndCallback(s, e) {
                if (s.cpUpdatedMessage) {
            alert(s.cpUpdatedMessage);
        }
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
    function OnToolbarItemClick(s, e) {
        debugger;
        var key = s.GetRowKey(s.GetFocusedRowIndex());  
        s.PerformCallback('del|' + key);
        e.processOnServer=false;  
        e.usePostBack = false;  
    }
    function ClearForm() {
        var value = ["Clear", 0].join('|'); 
        gvitems.PerformCallback(value);
        gvutilize.PerformCallback(value);
        ClientValidfrom.SetText("");
        ClientValidto.SetText("");
        ClientMaterial.SetText("");
    }
    function OnRowClick(s, e) {
        var row = s.GetRow(e.visibleIndex);
        //ClientUpCharge.SetValue(row.attributes["data"].nodeValue);
    }
    function OnRowDblClick(s, e) {
        s.GetRowValues(e.visibleIndex, "Upcharge;Value;Currency;Unit", OnGetRowId);
        //var row = s.GetRow(e.visibleIndex);
        ASPxPopupControl1.Hide();
    }
    function OnGetRowId(row) {
	debugger;
        ClientUpCharge.SetValue(row[0]);
        tbUpChargePrice.SetValue(row[1]);
        tbUpCurrency.SetValue(row[2]);
        tbUpChargeUnit.SetValue(row[3]);
        tbUpChargeQuantity.Focus();  
    }
    function OnButtonClick(s, e) {
        ASPxPopupControl1.Show();
    }
    function OnBtnShowPopupClick() {
        PopupControl.RefreshContentUrl();
        PopupControl.SetContentUrl('popupControls/selectedvalue.aspx?ID=1');
        PopupControl.SetHeaderText("param");
        PopupControl.Show();
    }
    function HidePopupAndShowInfo(closedBy, returnValue) {
        debugger;
        PopupControl.Hide();
        //if (ClientMaterial.GetValue() == null) return alert('sold-to or ship-to should not be empty...');
        gv.SetEditValue('Material', returnValue);

        var myDate = new Date();
        gv.SetEditValue('From', myDate);
        gv.SetEditValue('To', myDate);
        ClientValidto.SetText(Demo.currentdate());
        ClientValidfrom.SetText(Demo.currentdate());
        ClientMaterial.SetText(returnValue);
        OnDateChanged();
        GetBuildCalcu(true);
        gv.GetValuesOnCustomCallback('MSC|' + returnValue, function (r) {
            if (!r)
                return;
            gv.SetEditValue('MSC', r["value"]);
            OnSetValueToGridview('MSC', r["value"]);
        });
    }
    function OnCustomButtonClick(s, e) {
        debugger;
        if (e.buttonID == 'customRecover') {

        }
    }
    </script>
<dx:ASPxHiddenField ID="hfgetvalue" runat="server" ClientInstanceName="ClientNewID" />
<dx:ASPxHiddenField ID="hfStatusApp" runat="server" ClientInstanceName="hfStatusApp" />
<dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
<dx:ASPxHiddenField ID="approv" runat="server" ClientInstanceName="approv" />
<dx:ASPxHiddenField ID="hfuser" runat="server" ClientInstanceName="hfuser" />
<dx:ASPxHiddenField ID="hfKeyword" runat="server" ClientInstanceName="hfKeyword" />
<dx:ASPxHiddenField ID="hftype" runat="server" ClientInstanceName="hftype" />
<dx:ASPxHiddenField ID="hfid" runat="server" ClientInstanceName="hfid" />
<dx:ASPxHiddenField ID="hGeID" runat="server" ClientInstanceName="hGeID"/>
<dx:ASPxHiddenField ID="hCommission" runat="server" ClientInstanceName="ClientCommission" />
<dx:ASPxHiddenField ID="hfUpchargeGroup" runat="server" ClientInstanceName="hfUpchargeGroup" />
<dx:ASPxLabel ID="ASPxLabel1" runat="server" />
<div id="gridContainer" style="visibility: hidden"></div>
<dx:ASPxGridView runat="server" ID="gridData" ClientInstanceName="gridData"  Width="100%" DataSourceID="dsgv"
        AutoGenerateColumns="true" KeyFieldName="ID" OnCustomCallback="gridData_CustomCallback"
        OnCustomDataCallback="gridData_CustomDataCallback"
        Border-BorderWidth="0">
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
    <ClientSideEvents 
        Init="Demo.gridData_Init" 
        EndCallback="OnEndCallback" ContextMenuItemClick="function(s,e) { OnContextMenuItemClick(s, e); }" 
        RowClick="Demo.ClientgridData_RowClick"/>
</dx:ASPxGridView>
<dx:ASPxGridViewExporter runat="server" ID="GridExporter" GridViewID="gridData" />
    <dx:ASPxPopupControl ID="PopupControl" runat="server" ClientInstanceName="PopupControl" CloseAction="OuterMouseClick" CloseOnEscape="true" 
            Width="620px" Height="500px" AllowResize="true"
            HeaderText="Results" AllowDragging="True" PopupAnimationType="Fade"
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
    <dx:ASPxCallbackPanel ID="PreviewPanel" runat="server" RenderMode="Div" Height="100%" CssClass="PreviewPanel" ClientInstanceName="ClientPreviewPanel" 
        ClientVisible="false" />
        <dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" ClientVisible="false" ScrollBars="Vertical" ClientInstanceName="ClientFormPanel">
        <ClientSideEvents Init="Demo.Init" />
        <PanelCollection>
            <dx:PanelContent>
            <dx:ASPxSplitter ID="ASPxSplitter1" runat="server" ClientInstanceName="sampleSplitter" ShowCollapseBackwardButton="True" ShowCollapseForwardButton="True" Orientation="Vertical" >
                <Styles>
                    <Pane>
                        <Paddings Padding="0px" />
                    </Pane>
                </Styles>
                <Panes>
                <dx:SplitterPane Name="gridContainer" Size="60%" MinSize="100" AutoHeight="True" PaneStyle-Border-BorderWidth="0px">
                <ContentCollection>
                    <dx:SplitterContentControl runat="server">
                    <dx:ASPxFormLayout runat="server" ID="ASPxFormLayout1" ClientInstanceName="formLayout" CssClass="formLayout" AlignItemCaptionsInAllGroups="true">
                        <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
                        <Items> 
                            <dx:LayoutGroup Caption="TunaStandard" GroupBoxDecoration="None" UseDefaultPaddings="false" ColCount="4">
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
                    <dx:LayoutItem Caption="Sold-to">
                    <SpanRules>
                        <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                        <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                    </SpanRules>
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer>
                            <%--<dx:ASPxButtonEdit ID="CmbCustomer" ClientInstanceName="ClientCustomer" Width="100%" runat="server">
                            <Buttons>
                                <dx:EditButton>
                                </dx:EditButton>
                            </Buttons>
                                <ClientSideEvents ButtonClick="OnButtonClick" />
                        </dx:ASPxButtonEdit>--%>
                            <dx:ASPxComboBox ID="CmbCustomer" runat="server" ValueField="Code" IncrementalFilteringMode="Contains"  
                                ClientInstanceName="ClientCustomer" DataSourceID="dsCustomer" TextField="Name" TextFormatString="{0},{1}">
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
                                ClientInstanceName="ClientShipTo" DataSourceID="dsCustomer" TextField="Name" TextFormatString="{0},{1}">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="Code"/>
                                    <dx:ListBoxColumn FieldName="Name"/>
                                </Columns>
                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                            </dx:ASPxComboBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Currency">
                        <SpanRules>
                            <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                            <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                        </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <table>
                                        <tr>
                                            <td>
                                            <dx:ASPxSpinEdit runat="server" ID="seExchangeRate" 
                                                ClientInstanceName="seExchangeRate" NumberType="Float" 
                                                Number="1.00" Width="80px">
                                                <ClientSideEvents ValueChanged="OnExchangeRate" />
                                            </dx:ASPxSpinEdit></td><td>&nbsp;</td><td>
                                            <dx:ASPxComboBox runat="server" ID="CmbCurrency" 
                                                ClientInstanceName="ClientCurrency" Width="80px" DataSourceID="dsCurrency" 
                                                    ValueField="value" TextField="Value">
                                                    <ClientSideEvents Init="function(s, e) { s.SetText('USD'); }"/>
                                                </dx:ASPxComboBox>
                                            </td>
                                        </tr>
                                    </table>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Payment Term" Name="PaymentTerm">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbPaymentTerm" runat="server" ClientInstanceName="CmbPaymentTerm"
                                    EnableCallbackMode="true" CallbackPageSize="200" ValueField="Code" OnCallback="CmbPaymentTerm_Callback" 
                                    DataSourceID="dsPaymentTerm" TextFormatString="{0};{1}">   
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
                        <dx:LayoutItem Caption="Actual interest">
                        <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                     <table>
                                        <tr>
                                        <td>
                                            <dx:ASPxTextBox runat="server" ID="tbinterest" ClientInstanceName="Clientinterest" Width="120px" ReadOnly="true">
                                            </dx:ASPxTextBox></td>
                                        <td>&nbsp;</td>
                                        <td>%</td>
                                    </tr>
                                    </table>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Incoterm" Name="Incoterm">
                        <SpanRules>
                            <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                            <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                        </SpanRules>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbIncoterm" runat="server" ClientInstanceName="CmbIncoterm" DataSourceID="dsIncoterm"
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
                        <dx:LayoutItem Caption="Route">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="CmbRoute" runat="server" ClientInstanceName="CmbRoute" DataSourceID="dsRoute" 
                                        TextFormatString="{0};{1}" EnableCallbackMode="true" ValueField="Code" 
                                        CallbackPageSize="10" Width="300px">
                                        <Columns>
                                        <dx:ListBoxColumn FieldName="Code"/>
                                        <dx:ListBoxColumn FieldName="Name"/>
                                        </Columns>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) {  if (s.GetValue() == null) s.SetSelectedIndex(-1);}" />
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Size">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="CmbSize" runat="server" ClientInstanceName="CmbSize" DataSourceID="dsContainerType" 
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
                        
                        <dx:LayoutItem Caption="Freight">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTextBox ID="tbFreight" runat="server" ClientInstanceName="tbFreight">
                                        <%--<ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                ValueChanged="function(s, e) { OnValueChanged('sub',tbNumberContainer.GetText());}" />--%>
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Insurance">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTextBox ID="tbInsurance" runat="server" ClientInstanceName="tbInsurance">
                                            <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                ValueChanged="function(s, e) { OnSetValueToGridview('Interest',s.GetText());}" />
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
                                    <dx:LayoutItemNestedControlContainer><dx:ASPxMemo runat="server" ID="mNotes" Rows="4" ClientInstanceName="ClientNotes">
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
                            <dx:ASPxFormLayout runat="server" ID="formLayout" ClientInstanceName="formLayout" CssClass="formLayout" AlignItemCaptionsInAllGroups="true">
                            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
                            <Items> 
                    <dx:LayoutGroup Caption="" ColCount="4" GroupBoxDecoration="None" UseDefaultPaddings="false">
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
                          <dx:LayoutItem Caption="Selected" Name="Upload" Width="100%">
                                <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxUploadControl ID="Upload" runat="server" OnFileUploadComplete="Upload_FileUploadComplete" Width="200" ClientInstanceName="Upload" 
                                    NullText="Click here to browse files..." UploadMode="Advanced" FileUploadMode="OnPageLoad" AutoStartUpload="True">
                                    <ValidationSettings AllowedFileExtensions=".xls,.xlsx">
                                    </ValidationSettings>
                                    <ClientSideEvents FileUploadComplete="OnFileUploadComplete" />
                                </dx:ASPxUploadControl>
                                </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="" Width="100%" ColSpan="4">
                        <SpanRules>
                            <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                            <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                        </SpanRules>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                            <dx:ASPxGridView ID="gv" runat="server" ClientInstanceName="gv" OnRowUpdating="gv_RowUpdating" KeyFieldName="ID"  
                            OnCustomDataCallback="gv_CustomDataCallback" OnCustomCallback="gv_CustomCallback" OnRowDeleting="gv_RowDeleting"
                            OnDataBinding="gv_DataBinding" OnHtmlRowPrepared="gv_HtmlRowPrepared"
                            onHtmlEditFormCreated="gv_HtmlEditFormCreated"
                            Width="100%" OnRowInserting="gv_RowInserting">
                            <ClientSideEvents BatchEditStartEditing="OnBatchEditStartEditing" EndCallback="SetButtonsVisibility" 
                                Init="function(s, e) { PreventEditFormPopupAnimation(s); }" BeginCallback="OnGridBeginCallback"
                                RowDblClick="gv_RowDblClick"/>
                                <Toolbars>
                                    <dx:GridViewToolbar>
                                        <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                                        <Items>
                                            <dx:GridViewToolbarItem Command="New" />
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
                            <Columns>
                            <dx:GridViewDataButtonEditColumn FieldName="Material">
                                <PropertiesButtonEdit>
                                    <Buttons>
                                        <dx:EditButton>
                                        </dx:EditButton>
                                    </Buttons>
                                    <ClientSideEvents ButtonClick="OnBtnShowPopupClick" />
                                </PropertiesButtonEdit>
                            </dx:GridViewDataButtonEditColumn>
                            <dx:GridViewDataColumn FieldName="Description"/>
                            <dx:GridViewDataDateColumn FieldName="From">
                                <PropertiesDateEdit DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                    <ClientSideEvents DateChanged="function(s, e){ ClientValidfrom.SetValue(s.GetValue()); }" />
                                </PropertiesDateEdit>
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataDateColumn FieldName="To">
                                <PropertiesDateEdit DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                    <ClientSideEvents DateChanged="function(s, e){ ClientValidto.SetValue(s.GetValue()); 
                                        OnDateChanged();}" />
                                </PropertiesDateEdit>
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataComboBoxColumn FieldName="Commission">
                                <PropertiesComboBox DataSourceID="dsCommission" TextField="value" ValueField="value" NullText="Example : xx %">
                                    <ClientSideEvents ValueChanged="function(s, e){ ClientCommission.Set('Value Commission',s.GetValue());
                                        OnSetValueToGridview('Value Commission',s.GetValue());}" />
                                </PropertiesComboBox>
                            </dx:GridViewDataComboBoxColumn>
                            <dx:GridViewDataTextColumn FieldName="OverPrice">
                                <PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                    ClientSideEvents-ValueChanged="function(s, e) { OnSetValueToGridview('OverPrice',s.GetText());}" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Pacifical"><PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                    ClientSideEvents-ValueChanged="function(s, e) { OnSetValueToGridview('Pacifical',s.GetText());}" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="MSC"><PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                    ClientSideEvents-ValueChanged="function(s, e) { OnSetValueToGridview('MSC',s.GetText());}" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Margin" Caption="%Margin"><PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                    ClientSideEvents-ValueChanged="function(s, e) { OnSetValueToGridview('Margin',s.GetText());}" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="SubContainers" Caption="SubContainers">
                                <PropertiesTextEdit>
                                    <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                        ValueChanged="function(s, e) { 
                                        insertSelection(7,'Insert',s.GetText());}" />
                                </PropertiesTextEdit>
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataColumn FieldName="MinPrice" EditFormSettings-Visible="False"/>
                            <dx:GridViewDataColumn FieldName="OfficePrice" EditFormSettings-Visible="False"/>
                        </Columns>
                        <EditFormLayoutProperties ColCount="3">
                        <Items>
                            <dx:GridViewColumnLayoutItem ColumnName="Material"/>
                            <dx:GridViewColumnLayoutItem ColumnName="From"/>
                            <dx:GridViewColumnLayoutItem ColumnName="To"/>
                            <dx:GridViewColumnLayoutItem ColumnName="Commission" />
                            <dx:GridViewColumnLayoutItem ColumnName="OverPrice" />
                            <dx:GridViewColumnLayoutItem ColumnName="Pacifical" Caption="%Pacifical" />
                            <dx:GridViewColumnLayoutItem ColumnName="MSC" Caption="%MSC" />
                            <dx:GridViewColumnLayoutItem ColumnName="Margin" Caption="%Margin" />
                            <dx:GridViewColumnLayoutItem ColumnName="SubContainers" Caption="Case per FCL" />
                            <dx:EditModeCommandLayoutItem ColSpan="3" HorizontalAlign="Right" />
                            </Items>
                        </EditFormLayoutProperties>
                        <SettingsSearchPanel ColumnNames="" Visible="true" CustomEditorID="tbToolbarSearch"/>
		                <Settings ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="false" />
		                <SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true" AllowFocusedRow="true" />
		                <SettingsPager PageSize="5">
                        <PageSizeItemSettings Visible="true" ShowAllItem="true"  AllItemText="All Records" />
                        </SettingsPager>
                        <SettingsExport EnableClientSideExportAPI="true" ExcelExportMode="DataAware" />
                        <SettingsEditing Mode="EditFormAndDisplayRow" />
		                <Styles>
			                <Row Cursor="pointer" />
		                </Styles>
                        </dx:ASPxGridView>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem> 
                    <dx:LayoutItem Caption="Material" Width="100%" ClientVisible="false">
                        <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbMaterial" ClientInstanceName="ClientMaterial">
                                            </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    <dx:LayoutItem Caption="From" ClientVisible="false">
                        <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxDateEdit runat="server" ID="defrom" ClientInstanceName="ClientValidfrom" 
                                       DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                            </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="To" ClientVisible="false">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxDateEdit runat="server" ID="deto" ClientInstanceName="ClientValidto"
                                                DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                            </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Case per FCL" ClientVisible="false">
                                <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="tbNumberContainer" runat="server" ClientInstanceName="tbNumberContainer" Width="300px">
                                        <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                            ValueChanged="function(s, e) { OnValueChanged('calcu',s.GetText());}" />
                                        </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                    </Items>
                    </dx:LayoutGroup >
                            <dx:LayoutItem Caption="">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTabControl ID="tcDemos" runat="server" NameField="Id" DataSourceID="XmlDataSource1" ActiveTabIndex="0" ClientInstanceName="tcDemos" ClientVisible="false">
                                           <ClientSideEvents ActiveTabChanged="function(s, e) { OnActiveTabChanged(s, e); }" Init="function(s, e) {s.SetActiveTabIndex(0);}"/>
                                        </dx:ASPxTabControl>
                                        <asp:XmlDataSource ID="XmlDataSource1" runat="server" DataFile="~/App_Data/Platforms.xml"
                                            XPath="//Item"></asp:XmlDataSource>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem> 
                           <dx:TabbedLayoutGroup Caption="SubTotal" ActiveTabIndex="0" ClientInstanceName="tabbedGroupPageControl" Name="SubTotal" ShowGroupDecoration="false" Width="100%" ClientVisible="false">
                            <Items>    
                            <dx:LayoutGroup Caption="Utilize" ColCount="3">
                                <Items>                                
                                </Items>
                            </dx:LayoutGroup>
                            <dx:LayoutGroup Caption=""/>
                            <dx:LayoutGroup Caption=""/>
                            <dx:LayoutGroup Caption=""/>
                            <dx:LayoutGroup Caption=""/>
                            <dx:LayoutGroup Caption=""/>
                            <dx:LayoutGroup Caption="Upcharge" ColCount="2">
                                <Items>
                                    <dx:LayoutItem Caption="UpCharge">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer>
                                                <%--<dx:ASPxComboBox runat="server" ID="CmbUpCharge" ClientInstanceName="CmbUpCharge" DataSourceID="dsUpCharge"
                                                    TextFormatString="{1}" ValueField="Id" >
                                                    <Columns>
                                                        <dx:ListBoxColumn FieldName="UpchargeGroup" />
                                                        <dx:ListBoxColumn FieldName="Upcharge" />
                                                        <dx:ListBoxColumn FieldName="Value" />
                                                        <dx:ListBoxColumn FieldName="Currency" />
                                                        <dx:ListBoxColumn FieldName="Unit" />
                                                    </Columns>
                                                    <ClientSideEvents ValueChanged="function(s, e) { OnSelValueChanged(s,'UpCharge'); }" />
                                                </dx:ASPxComboBox>--%>
                                                <dx:ASPxPopupControl ID="ASPxPopupControl1" runat="server" ClientInstanceName="ASPxPopupControl1"
		                                            PopupElementID="CmbUpCharge" PopupHorizontalAlign="LeftSides" PopupVerticalAlign="Below"
		                                            ShowCloseButton="False" ShowHeader="False" Height="0px" Width="0px">
		                                            <ContentCollection>
			                                            <dx:PopupControlContentControl runat="server">
				                                            <dx:ASPxGridView runat="server" KeyFieldName="Id" AutoGenerateColumns="False" DataSourceID="dsUpCharge" Width="425px" 
                                                                ID="ASPxGridView1">
					                                            <ClientSideEvents RowDblClick="OnRowDblClick" />
					                                            <Columns>
						                                            <dx:GridViewDataTextColumn FieldName="UpchargeGroup" ReadOnly="True" VisibleIndex="0">
							                                            <EditFormSettings Visible="False"></EditFormSettings>
						                                            </dx:GridViewDataTextColumn>
						                                            <dx:GridViewDataTextColumn FieldName="Upcharge" Width="220px" VisibleIndex="1"></dx:GridViewDataTextColumn>
                                                                    <dx:GridViewDataTextColumn FieldName="Value" Width="80px" VisibleIndex="2"></dx:GridViewDataTextColumn>
                                                                    <dx:GridViewDataTextColumn FieldName="Currency" VisibleIndex="3"></dx:GridViewDataTextColumn>
                                                                    <dx:GridViewDataTextColumn FieldName="Unit" VisibleIndex="4"></dx:GridViewDataTextColumn>
					                                            </Columns>
					                                            <SettingsBehavior AllowFocusedRow="True"></SettingsBehavior>
					                                            <SettingsPager PageSize="20"></SettingsPager>
					                                            <Settings ShowVerticalScrollBar="True" VerticalScrollableHeight="150" />
                                                                <SettingsDataSecurity AllowReadUnlistedFieldsFromClientApi="True" />
				                                            </dx:ASPxGridView>
			                                            </dx:PopupControlContentControl>
		                                            </ContentCollection>
		                                            <ContentStyle>
			                                            <Paddings Padding="0px"></Paddings>
		                                            </ContentStyle>
		                                            <Border BorderStyle="None"></Border>
	                                            </dx:ASPxPopupControl>
                                                <%--<dx:ASPxTextBox runat="server" ID="CmbUpCharge" ClientInstanceName="ClientUpCharge" Width="100%"/>--%>
                                                <dx:ASPxButtonEdit ID="CmbUpCharge" ClientInstanceName="ClientUpCharge" Width="100%" runat="server">
                                                    <Buttons>
                                                        <dx:EditButton>
                                                        </dx:EditButton>
                                                    </Buttons>
                                                        <ClientSideEvents ButtonClick="OnButtonClick" />
                                                </dx:ASPxButtonEdit>
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
                                    <dx:LayoutItem Caption="Quantity Per Case">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer>
                                                <dx:ASPxTextBox runat="server" ID="tbUpChargeQuantity" ClientInstanceName="tbUpChargeQuantity">
                                                </dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="Currency/Unit" ClientVisible="false">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer>
                                                <table>
                                                    <tr>
                                                        <td><dx:ASPxTextBox ID="tbUpCurrency" runat="server"  ClientInstanceName="tbUpCurrency" Width="40px" ReadOnly="true"/></td><td>&nbsp;</td>
                                                        <td><dx:ASPxTextBox runat="server" ID="tbUpChargeUnit" ClientInstanceName="tbUpChargeUnit" Width="35px" ReadOnly="true"/></td><td>&nbsp;</td>
                                                        <td><%--<dx:ASPxButton runat="server" RenderMode="Link" AutoPostBack="false" Text = "New Item" ImagePosition = "Right">
                                                            <Image IconID="actions_additem_16x16gray"></Image>
                                                            <ClientSideEvents Click="function(s,e){ insertSelection(6,'Insert',s); }" />
                                                        </dx:ASPxButton>--%></td>
                                                    </tr>
                                                </table>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                </Items>
                            </dx:LayoutGroup>
                            <dx:LayoutGroup Caption="Route"/>
                            <dx:LayoutGroup Caption=""/>
                    </Items>
                    </dx:TabbedLayoutGroup>
                    <dx:LayoutItem Caption="" Width="100%" Name="utilize" ClientVisible="false">
                            <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                 <dx:ASPxGridView ID="gvutilize" runat="server" Width="35%" ClientInstanceName="gvutilize" KeyFieldName="Id" OnRowValidating="gvutilize_RowValidating"
                                            OnCustomDataCallback="gvutilize_CustomDataCallback"
                                            OnCustomCallback="gvutilize_CustomCallback" OnDataBinding="gvutilize_DataBinding" OnRowUpdating="gvutilize_RowUpdating">
                                            <ClientSideEvents Init="OnInitGridBatch" BatchEditStartEditing="OnBatchEditStartAlimConstat" BatchEditEndEditing="OnEndEditCell" />
                                            <Columns>
                                                <dx:GridViewDataColumn FieldName="Name" ReadOnly="true" EditFormSettings-Visible="False" />
                                                <dx:GridViewDataColumn FieldName="Result" Caption="%" />
                                                <dx:GridViewDataTextColumn FieldName="StatusApp" ReadOnly="True" Visible="false">
                                                    <EditFormSettings Visible="False" />
                                                </dx:GridViewDataTextColumn>
                                            </Columns>
                                            <SettingsEditing Mode="Batch" />
                                            <styles>
                                            <focusedrow BackColor="#f4dc7a" ForeColor="Black"></focusedrow>
                                            <FixedColumn BackColor="LightYellow"></FixedColumn>
                                        </styles>
                                        <SettingsSearchPanel ColumnNames="" Visible="false" />
	                                    <Settings  ShowFooter="true" GridLines="Both" ShowStatusBar="Hidden"/>
                                        <SettingsPager AlwaysShowPager="false" PageSize="20"/>
		                                <Styles>
			                                <Row Cursor="pointer" />
		                                </Styles>
                                        <TotalSummary>
                                            <dx:ASPxSummaryItem FieldName="Result" SummaryType="Sum" />
                                        </TotalSummary>
                                        </dx:ASPxGridView>
                                        <dx:ASPxCallback ID="ASPxCallback" runat="server" ClientInstanceName="callback" OnCallback="ASPxCallback_Callback" 
                                            ClientSideEvents-CallbackComplete="OnEndUpdateCallback"/>

                                  <dx:ASPxGridView ID="gvitems" runat="server" Width="100%" ClientInstanceName="gvitems" ClientVisible="false" 
                                    OnDataBound="gvitems_DataBound" OnBatchUpdate="gvitems_BatchUpdate"  
                                    OnCustomDataCallback="gvitems_CustomDataCallback" KeyFieldName="RowID" OnCustomButtonCallback="gvitems_CustomButtonCallback"   
                                    OnCustomCallback="gvitems_CustomCallback" OnDataBinding="gvitems_DataBinding">
                                    <%--<Columns>
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
                                                    <ClientSideEvents Click="function(s,e){ insertSelection(s,'Insert'); }" />
                                                </dx:ASPxButton>
                                            </HeaderTemplate>
                                        </dx:GridViewCommandColumn>
                                 
                                        <dx:GridViewDataColumn FieldName="Component" />
                                        <dx:GridViewDataColumn FieldName="Name" Width="50%" />
                                        <dx:GridViewDataColumn FieldName="Currency" />
                                        <dx:GridViewDataColumn FieldName="Quantity" />
                                        <dx:GridViewDataColumn FieldName="Price" />
                                        <dx:GridViewDataColumn FieldName="Result" />
                                        <dx:GridViewDataColumn FieldName="Calcu" HeaderStyle-CssClass="unitPriceColumn" CellStyle-CssClass="unitPriceColumn" Width="0px"/>
                                        <dx:GridViewDataColumn FieldName="Unit" />
                                        <dx:GridViewDataColumn FieldName="BaseUnit" />
                                    </Columns>--%>
                                    <SettingsSearchPanel ColumnNames="" Visible="false" />
	                                    <Settings  ShowFooter="true" GridLines="Both" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto"/>
                                        <SettingsPager Mode="ShowAllRecords" AlwaysShowPager="false" PageSize="20"/>
		                                <Styles>
			                                <Row Cursor="pointer" />
		                                </Styles>
                                    <SettingsEditing Mode="Batch"/>
                                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>  
                                </dx:ASPxGridView>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                <dx:LayoutGroup Caption="" ColCount="3" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
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
                        <dx:LayoutItem Caption="Comment" Width="100%">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxMemo runat="server" ID="mComment" Rows="4" ClientInstanceName="ClientComment">
                                    </dx:ASPxMemo>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem> 
                        <dx:LayoutItem Caption="EventLog" Width="100%" Name="EventLog" CaptionStyle-Font-Bold="true" ClientVisible="false">
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
                                    </dx:SplitterContentControl>
                                </ContentCollection>
                            </dx:SplitterPane>
                        </Panes>
            </dx:ASPxSplitter>
    </dx:PanelContent>
    </PanelCollection>
</dx:ASPxCallbackPanel>
<asp:SqlDataSource ID="dsReason" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasReason"/>
<asp:SqlDataSource ID="dssapMaterial" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from massapMaterial"/>
<asp:SqlDataSource ID="dsPaymentTerm" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select Code,Value,LeadTime from MasPaymentTerm union select '0','None',''"/>
<asp:SqlDataSource ID="dsCustomer" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select case when Code is null Or Code ='' then Custom else Code end as 'Code',Name from MasCustomer where  Custom=1 union select '0','None'">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsRoute" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select Code,Name from MasRoute union select '0','None'">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsContainerType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select Code,Name from MasContainerType  union select '0','None'">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsCommission" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select value from dbo.FNC_SPLIT('0,1,2',',')">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsinterest" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select value from dbo.FNC_SPLIT('6,4',',')">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from TransTunaStd where createby=@user">
    <SelectParameters>
        <asp:ControlParameter ControlID="hfuser" Name="user" PropertyName="['user_name']"/>
    </SelectParameters>
</asp:SqlDataSource>
<%--<asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spGetTunaCalculation" SelectCommandType="StoredProcedure">
    <SelectParameters>
        <asp:Parameter Name="Param" />
        <asp:Parameter Name="Code" />
    </SelectParameters>
</asp:SqlDataSource>--%>
   <asp:SqlDataSource ID="dsPlant" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasPlant where Company in (select distinct value from dbo.FNC_SPLIT(@Bu,'|'))">
       <SelectParameters>
            <asp:SessionParameter Name="BU" SessionField="BU" Type="String" />
       </SelectParameters>
   </asp:SqlDataSource>
<asp:SqlDataSource ID="dsIncoterm" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select Code,Name from MasIncoterm  union select '0','None'"/>
<asp:SqlDataSource ID="dsCurrency" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select value from dbo.FNC_SPLIT('THB;USD;JPY;EUR;GBP',';') order by value">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsCompany" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spGetCompany" SelectCommandType="StoredProcedure">
    <SelectParameters>
        <asp:Parameter Name="ID" Type="String" />
	    <asp:Parameter Name="BU" Type="String" />
    </SelectParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsUpcharge" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from StandardUpcharge">
</asp:SqlDataSource>
<%--<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select ID, isnull(Code,Custom)Code, Name, Custom from MasCustomer union select '0','','',''">
</asp:SqlDataSource>--%>