<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CalculationForm2.ascx.cs" Inherits="UserControls_CalculationForm" %>
 
<script type="text/javascript">
    var isCustomCallback = false;
    var curentEditingIndex = 0;
    var FocusedCellColumnIndex = 0;
    function OnValidation(s, e) {
        debugger;
        var grid = ASPxClientGridView.Cast(s);
        var cellInfo1 = e.validationInfo[grid.GetColumnByField("SubContainers").index];
 
        if ((cellInfo1.value == null || cellInfo1.value == '') && (CmbIncoterm.GetValue() == 'CFR' || CmbIncoterm.GetValue() == 'CIF')) {
            cellInfo1.isValid = false;
            cellInfo1.errorText = "Invalid Case per fcl.";
        } else {
            cellInfo1.isValid = true;
        }
    }
    //ASPxClientHint.Register(".target", "hint");
    ASPxClientHint.Register('[data-visibleindex]', {
        onShowing: function (s, e) {
            var index = getElementAttr(e.targetElement);
            var key = gridData.GetRowKey(index);
            gridData.GetRowValues(index, 'ID', function (content) {
                e.contentElement.innerHTML = '<div class="hintContent">' +
                    '<iframe style="border:none;" src="popupControls/LoadContentOnDemand.aspx?Id=' + key + '" width="560" height="860"/>' +
                    '<div>' + content + '</div>' +
                    '</div>';
                ASPxClientHint.UpdatePosition(e.hintElement);
            });
            return 'loading...';
        },
        position: 'left',
        triggerAction: 'click',
        className: 'paddings'
    });
    function OnDisponsitionChanged(s) {
        //debugger;
        var _keys = s.GetValue();
        formLayout.GetItemByName("Assignee").SetVisible(false);
        if (s.GetValue().toString() == "1"||s.GetValue().toString() == "3" || s.GetValue().toString() == "4" || s.GetValue().toString() == "5") {
            gv.GetValuesOnCustomCallback('approve|' + _keys, function (r) {
                if (!r)
                    return;
                //if (r["approve"] == "reject") {
                //    ClientDisponsition.SetValue("");
                //    alert("please select items !!!")
                //}
                //else
                    if ((r["approve"] == "level1" || r["approve"] == "level2") && s.GetValue().toString() == "1") {
                    formLayout.GetItemByName("Assignee").SetVisible(true);
                    CmbAssignee.PerformCallback(r["approve"]+'|' + s.GetValue());
                }
                //updategrid(gv);
                //update 
            });
        }
    }
    function getElementAttr(element) {
        return element.dataset ? element.dataset['visibleindex'] : element.getAttribute('data-visibleindex');
    }
    function GetChangesCount(batchApi) {
        var updatedCount = batchApi.GetUpdatedRowIndices().length;
        var deletedCount = batchApi.GetDeletedRowIndices().length;
        var insertedCount = batchApi.GetInsertedRowIndices().length;

        return updatedCount + deletedCount + insertedCount;
    }
    function OnEndEdit(s, e) {
        //
        var code = s.batchEditApi.GetCellValue(visibleIndex, 'Material');
        if (code != null) {
            updategrid(s);
            
        }
    }
    function updategrid(s) {
        window.setTimeout(function () {

            //debugger;
            if (!ASPxClientEdit.ValidateGroup('group1')) {
                return alert('Field is required');
            }
            var count_value = GetChangesCount(s.batchEditApi);
            if (count_value > 0)
                s.UpdateEdit();
        }, 0);

    }
    function OnBatchEditStartEditing(s, e) {
        FocusedCellColumnIndex = e.focusedColumn.index;
        curentEditingIndex = e.visibleIndex;
        var name = e.focusedColumn.fieldName;
        e.cancel = getcheckedit(name);
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
    }
    function getindex() {
        var index = tcDemos.GetActiveTabIndex();
        if (index == 0 || index == 1) return 0;
        return index - 2;
    }
    function getcheckedit(name) {
        var index = getindex();
        //debugger;
        if (name == "Price" && index == 9) {
            var comp = grid.batchEditApi.GetCellValue(curentEditingIndex, "Component");
            if (comp == "Raw material")
            return false;
        }
        else if (name == "" && index == 10)
        {
            keyValue = gv.GetRowKey(visibleIndex);
            var value = grid.batchEditApi.GetCellValue(curentEditingIndex, "Result");
            gv.PerformCallback('delatt|' + keyValue + "|" + value);
            grid_refresh();
            //buildpComment(value, 'tbFreight');
        }
        else if (index == 10)
            return false;
        else if (name == "Result" && index == 0)
            return false;
        else if (index == 6) {
            var Array = ["Name"];
            if (Array.indexOf(name) > -1) {
                return false;
            }
        }
        else
            return true;
    }
    function onFocusedCellChanging(s, e) {
        var name = e.cellInfo.column.fieldName;
        curentEditingIndex = e.cellInfo.rowVisibleIndex
        e.cancel = getcheckedit(name);
    }
    function OnUpdateClick(s, e) {
        cp.PerformCallback("Update");
    }
    function OnCancelClick(s, e) {
        //ClientFormPanel.PerformCallback("Cancel");
        //gv.CancelEdit();  
        //gv.Refresh(); 
    }
    function OnGridBeginCallback(s, e) {
        
        if (e.command == "CANCELEDIT" || e.command == "UPDATEEDIT" || e.command == "REFRESH")
            GetBuildCalcu(false);
        if (e.command == "ADDNEWROW")
            GetBuildCalcu(false);
        if (e.command == "CUSTOMCALLBACK") {
            //ClientFormPanel.PerformCallback("Cancel");
            //s.CancelEdit()
            //s.Refresh();
            //e.processOnServer = false; 
        }
    }

    function Combo_SelectedIndexChanged(s, e) {
        var name = s.GetSelectedItem().GetColumnText(1);//s.GetText().toString();
        //gv.GetValuesOnCustomCallback("Description|" + name, DataCallback);
        //gv.SetEditValue('Description', name);
        
        grid.PerformCallback('reload|'+s.GetSelectedItem().GetColumnText(0));
    }
    function DataCallback(result) {
        var results = result.split("|");
        switch (results[0]) {
            case "Description":
                gv.batchEditApi.SetCellValue(curentEditingIndex, "Description", results[1]);
                break;
        }
    }
    function grid_OnEndCallback(s, e) {
        //debugger;
        if (!grid.cpKeyValue)
            return;
        if (grid.cpKeyValue == 'colDel' || grid.cpKeyValue == 'Submitted'){
            grid_refresh();
            //tcDemos.SetActiveTabIndex(0);
            OnUpcharge();         
        }
        delete grid.cpKeyValue;
    }
    function gv_OnEndCallback(s, e) {
        var columnInfo = s.cpColumnWidth;
        if (columnInfo) {
            dxColumnsWidth.Clear();
            dxColumnsWidth.Set(columnInfo.index, columnInfo.width);
        }
        setGridHeaderCheckboxes(s, e);
        if (!gv.cpKeyValue)
            return;
        debugger;
        var key = gv.cpKeyValue;
        var args = key.split("|");
        if (args[0] == 'TG' || args[0] == 'Margin') {

            keyValue = args[1];
            buildpComment(args[0], 'tbFreight');
            //gv.batchEditApi.SetCellValue(gv.GetFocusedRowIndex(), 'Mark','TGA');
            gv_refresh();
        }
        delete gv.cpKeyValue;
    }
    function OnUpcharge() {
        var _keys = gv.GetRowKey(gv.GetFocusedRowIndex());
        gv.GetValuesOnCustomCallback('post|' + _keys, function (r) {
            if (!r)
                return;
            gv.batchEditApi.SetCellValue(visibleIndex, 'OfferPrice', r["OfferPrice"]);
            gv.UpdateEdit();
            //updategrid(gv);
            //update 
        });
    }
    function OnEndCallback(s, e) {
        Demo.SetUnitPriceColumnVisibility();
        ASPxClientHint.Update();
        //if (s.IsEditing()) {
          
        //    var popup = s.GetPopupEditForm();
        //    popup.AdjustContentOnShow();
        //}
        if (!gridData.cpKeyValue)
            return;
        if (gridData.cpKeyValue == 0)
            alert(gridData.cpKeyValue);
        var key = gridData.cpKeyValue;
        gridData.cpKeyValue = null;
        if (key == 0)
            Demo.ChangeDemoState("MailList");
        else {
            var args = key.split("|");
            if (args[0] == "Clone") {
                Demo.ChangeDemoState("MailForm", "Clone", args[1]);
                //gv_refresh();
            } else {
                Demo.ChangeDemoState("MailForm", "EditDraft", key);
            }
        }
        delete gridData.cpKeyValue;
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
        if (CmbIncoterm.GetValue() == 'CFR' || CmbIncoterm.GetValue() == 'CIF') {
            //var value = [CmbRoute.GetText(), CmbSize.GetText(), CmbIncoterm.GetText(), CmbPaymentTerm.GetText()].join("|");
            gv.GetValuesOnCustomCallback(a + '|' + key, function (r) {
                if (!r)
                    return;
                //tbInsurance.SetText(CmbIncoterm.GetValue() == 'CIF' || 'CFR' ? r["Insurance"] : '0');
                tbInsurance.SetText(r["Insurance"]);
                tbFreight.SetText(r["Freight"]);
                OldValueFreight.Set("Freight", r["Freight"]);
            });
        } else {

            tbInsurance.SetText('');
            tbFreight.SetText('');
            OldValueFreight.Set("Freight", '');
            CmbRoute.SetSelectedIndex(-1);
            CmbSize.SetSelectedIndex(-1);
            grid.PerformCallback('FOB|0');
        }
        append_to_div("my_div", "");
        tcDemos.SetActiveTabIndex(0);
        //if (a == 'calcu')
        //    gvcal.PerformCallback('updated|0');
    }
    function append_to_div(div_name, data) {
        document.getElementById(div_name).innerText = data;
    } 
    //function OnSelectedIndexChanged(s, e) {
    //    var key = s.GetValue();
    //    var res = ['reload', key].join('|');
    //    gvcal.PerformCallback('reload');
    //    gvcal.GetValuesOnCustomCallback(res, function (r) {
    //        if (!r)
    //            return;
    //        
    //        seExchangeRate.SetText(r["ExchangeRate"]);
    //        ClientCurrency.SetText(r["Currency"]);
    //        })
    //    //Demo.SetEnable(value != "FOB");
    //    //OnValueChanged('tot',tbNumberContainer.GetText());
    //}
 
    function OnDateChanged(s, e) {
        buildpComment('ValidityDate', 'deValidityDate');
    }
    function OnExchangeRate(s, e) {
        debugger;
        var value = s.GetText();
        var OldValue = OldValueExchangeRate.Get("ExchangeRate");
        if (value < OldValue)
            buildpComment('ExchangeRate', 'seExchangeRate');
 
        if(s.GetValue()>0)
            gv.PerformCallback("Rate|0|" + s.GetValue());
    }
    function ClearOnTerm(s, e) {
        CmbPaymentTerm.SetSelectedIndex(-1);
        Clientinterest.SetText('');
    }
    function OnBillTo(s, e) {
        var key = s.GetValue();
        var custo = ClientCustomer.GetValue();
        if (custo == "") return alert("Please Select Sold To!!!")
        gv.GetValuesOnCustomCallback("zterm" + '|' + key, function (r) {
            if (!r)
                return;
            
            buildInterest(custo, r["LeadTime"]);
            CmbPaymentTerm.SetValue(r["knvv_zterm"]);
        });
    }
    function OnTermChanged(s, e) {
        
        if (s.GetValue() == null)
            s.SetSelectedIndex(-1);
        var b = ((s.GetValue() == 'Y00S' || s.GetValue() == 'D000') ? false : true);
        Clientinterest.SetEnabled(b);
        var n = s.GetSelectedItem().GetColumnText(2);
        //Clientinterest.SetText(b==true?"6":"");
        var custo = ClientCustomer.GetValue();
        buildInterest(custo, n);
        tcDemos.SetActiveTabIndex(0);
        gv.PerformCallback('recal|0');
       // buildpComment('PaymentTerm', 'CmbPaymentTerm');
    }
    function buildInterest(custo,n) {
        if (n > 0) {
            //6% * (n-5)/365
            debugger;
            //alert(custo);
            //Clientinterest.SetText((6 * ((n - 5) / 365)).toFixed(2));
            var interest = custo.length > 3 || custo =="0" ? 6 : 4;
            Clientinterest.SetText(((((interest/100) / 365)* n)*100).toFixed(2));
        } else
            Clientinterest.SetText(0);
    }
    function OnActiveTabChanged(s, e) {
        debugger;
        var Index = s.GetActiveTabIndex();
        var cd = gv.GetRowKey(gv.GetFocusedRowIndex());
        if (cd == null && Index > 1) {
           tcDemos.SetActiveTabIndex(0); 
            return alert("please select item!!!");
        }

        //var tb = grid.GetToolbarByName("toolbar");
        //if (tb != null)
        //alert(Index == 13);
        //tb.SetVisible(Index == 13);
        //var b = (Index == 0 || Index == 2);
        var keys = hGeID.Get("GeID");
        var text = ["reload", keys];
        var arge = text.join("|");
        
        if (Index == 1)
            cpUpdateFiles.PerformCallback(arge);
        //if (Index == 0) {
        //     alert("please click edit");
        //}
        tabbedGroupPageControl.SetActiveTabIndex(Index);
        //gvutilize.SetVisible(Index == 0);
        //grid.SetVisible(Index != 10);
        //gv.GetSelectedFieldValues("ID", function (values) {
        //    alert(values);
        //});
        grid.PerformCallback('symbol|' + Index);
        //gvitems.AutoFilterByColumn('Calcu', Index);
        formLayout.GetItemByName("layview").SetVisible((Index == 0 || Index == 1)?false:true);
    }
    function OnValidDateChanged() {
     //   var inputCode = ClientMaterial.GetText();
        var inputfrom = ClientValidfrom.GetValue();
        var inputto = ClientValidto.GetValue();
        if (inputto < inputfrom) {
            return alert('fromdate is greather then to date.');
        }
        gv.PerformCallback('reutilize|0');
        tcDemos.SetActiveTabIndex(0);
    //    if (inputfrom != null && inputto != null && inputCode != null) {
    //        //return alert('The Valid Date should not be empty...');
    //        gvutilize.PerformCallback('Edit');
    //        var Keys = 0;
    //        gvutilize.GetValuesOnCustomCallback(['build', Keys].join('|'), function (r) {
    //            if (!r)
    //                return;
    //            //seExchangeRate.SetText(r["Rate"]);
    //            //var code = ClientMaterial.GetText();
    //            var res = 'reload|' + inputCode + '|0';
    //            gvitems.PerformCallback(res);
    //        });      
    //    }
    }
    var FocusedCellColumnIndex = 0;
    var FocusedCellRowIndex = 0;
    //function OnInitGridBatch(s, e) {
    //    ASPxClientUtils.AttachEventToElement(s.GetMainElement(), "keydown", function (evt) {
    //        if (evt.keyCode == 40) {
    //            ASPxClientUtils.PreventEventAndBubble(evt);
    //            DownPressed(s);
    //        }
    //        //if (evt.keyCode == 13) {
    //        //    s.UpdateEdit();
    //        //}
    //    });
    //}
    //function OnBatchEditStartAlimConstat(s, e) {
    //    FocusedCellColumnIndex = e.focusedColumn.index;
    //    FocusedCellRowIndex = e.visibleIndex;
    //}
    function OnEndEditCell(s, e) {
        FocusedCellColumnIndex = 0;
        FocusedCellRowIndex = 0;
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
        
        //lvwd.PerformCallback(['edit',0].join('|'));
        var result = e.result;
        if (result == "OK") {
            var code = gv.GetEditValue('Material');
            var res = 'reload|' + code+'|0';
            grid.PerformCallback(res);
        }
    }
    function UpdateEdit(object, cellInfo) {
        //
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
        var value = ["'upload", "1"].join('|');
        gv.PerformCallback(value);
        if (e.callbackData) {
            alert(e.callbackData);
        }
    }
    function ASPxUploadControl1_OnFileUploadComplete(s, e) {
        if (e.callbackData !== "") {
            grid.Refresh();
        }
    }
    //function OnRowClick(s, e) {
    //    
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
    var keyValue = 0;
    function gv_RowDblClick(s, e) {
        var index = s.GetFocusedRowIndex();
        keyValue = s.GetRowKey(index);
        s.StartEditRow(index);
        var Keys = s.GetRowKey(e.visibleIndex);
        s.GetValuesOnCustomCallback(['build', Keys].join('|'), function (r) {
            if (!r)
                return;
            RecieveGridValues(r);
        });
    }
    function Flag_CheckedChanged(s, e) {
        gridData.PerformCallback(['Checked',s.cpLibID , s.GetChecked()].join('|'));
    }
    function RecieveGridValues(r) {
        //alert(r["ID"]);
        var res = 'reload';
        //ClientValidfrom.SetText(r["From"]);
        //ClientValidto.SetText(r["To"]);
        //gv.SetEditValue('Material',r["Material"]);
        //gv.SetEditValue('Description', r["Description"]);
        //gv.SetEditValue('MSC', r["MSC"]);
        //OnDateChanged();
        //gvutilize.PerformCallback('Build');
        //grid.PerformCallback([res, r["Material"], r["ID"]].join('|'));
        tcDemos.SetActiveTabIndex(0);
        //GetBuildCalcu(true);
        if (r["SubContainers"]!= "")
            insertSelection(7, 'Insert', r["SubContainers"]);
        //ClientMaterial.SetText(r["Material"]);
    }
    function GetBuildCalcu(b) {
        //b=true;
        //
        //tcDemos.SetVisible(b);
        //tabbedGroupPageControl.SetVisible(b);
        //var arr = "Calcu";
        //var g = arr.split(";"), s;
        //for (s = 0; s < g.length; s++) {
        //    if (g[s] != "")
        //        formLayout.GetItemByName(g[s]).SetVisible(b);
        //}
        //clear all
        //ClientUpCharge.SetText('');
        //hfUpchargeGroup.Set('Upcharge','');
        //tbUpChargePrice.SetText('');
        //tbUpCurrency.SetText('');
        //tbUpChargeUnit.SetText('');
        //tbUpChargeQuantity.SetText('');
    }
    function buildpComment(text, pcontrol) {
        
        UploadControl.ClearText();
        hfRefAtt.Set("RefAtt", "");
        var keys = hGeID.Get("GeID");
        if (keys == 0 && text == "ValidityDate") return;
        gridData.GetValuesOnCustomCallback("Comment|" + text + "|" +keys);
        pComment.SetHeaderText(text);
        //var targetArray = ["MSC", "Pacifical", "Margin"];
        var targetArray = ["Margin"];
        if (targetArray.indexOf(text) > -1) {
            cpRefAtt.PerformCallback('reload');
            cpRefAtt.SetVisible(true);
        } else
            cpRefAtt.SetVisible(false);
        //pComment.ShowAtElement(this);
        if (pcontrol != null)
        //    pComment.ShowAtElement(""); 
        //else
        pComment.ShowAtElementByID(pcontrol);
        pComment.SetSize(600, 400);
        pComment.Show();
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
        if (ClientCustomer.GetValue() == null || ClientShipTo.GetValue() == null)
            return alert('material should not be empty...');

        if (text == "OverType")
            grid.PerformCallback(["OverType", "OverPrice", evt, keyValue].join('|'));
        else
            grid.PerformCallback(["updated", text, evt, keyValue].join('|'));   
        //window.setTimeout(function () {
        //    gvitems.UpdateEdit();
        //}, 0);
    }
    function OnSelValueChanged(s, evt) {
        if (evt == 'UpCharge') {
            hfUpchargeGroup.Set('Upcharge', s.GetSelectedItem().GetColumnText(0));
            tbUpChargePrice.SetText(s.GetSelectedItem().GetColumnText(2));
            tbUpCurrency.SetText(s.GetSelectedItem().GetColumnText(3));
            tbUpChargeUnit.SetText(s.GetSelectedItem().GetColumnText(5));
            //tbUpChargeQuantity.SetText(1);
            //tbUpChargeQuantity.Focus();  
        }
    }
    function insertSelection(index, command, text) {
        //debugger;
        if (command == 'Insert') {
            //if (ClientUpCharge.GetText() != null || ClientUpCharge.GetText() != '')
            if (index == '10') {
                //var Keys = gv.GetRowKey(visibleIndex);
                grid.PerformCallback(["Attach", keyValue, text].join('|'));
            } else {
                grid.AddNewRow();
                //OnButtonClick('Upcharg');
                //grid.PerformCallback(["Insert", index, text].join('|'));
            }
        }
        if (command == 'new') {
            curentEditingIndex = 0;
            gv.AddNewRow();
        }
    }
    function OnSelectedIndexChanged(s, e) {
        debugger;  
        if (s.GetValue() == null)
            s.SetSelectedIndex(-1);
        var value = s.GetValue();
        var _b =(value == "CFR" || value == "CIF")
        Demo.SetEnable(_b);
        if (_b) {
            CmbRoute.SetIsValid(false);
            CmbSize.SetIsValid(false);
            tbFreight.SetIsValid(false);
            //gv.PerformCallback("Valid|0");
            //CmbRoute.SetErrorText('*');
        }
    }
    function OnChangedCustomer(s, e) {
        if (s.GetValue() == null) {
            s.SetSelectedIndex(-1);
            return;
        } else
            ClientMargin.SetText("3");
    }
    //function gvitems_EndCallback(s, e) {
    //            if (s.cpUpdatedMessage) {
    //        alert(s.cpUpdatedMessage);
    //    }
    //}
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
        
        //if (IsCustomExportToolbarCommand(e.item.name)) {
        if (e.item.name == "Delete") {
            var key = s.GetRowKey(s.GetFocusedRowIndex());
            s.PerformCallback('del|' + key);
        } else if (e.item.name == "New") {
            if (!ASPxClientEdit.ValidateGroup('group1')) {
                return alert('Field is required');
            }
            //if (ClientRequestNo.GetText !="###########") return "autho";
            if (CmbIncoterm.GetValue() == null)
                return alert('The Incoterm should not be empty...');
            curentEditingIndex = s.GetFocusedRowIndex();
            OnBtnShowPopupClick('Material');
        } else if (e.item.name = "Refresh") {
            gv.PerformCallback('recal|0|' + key);
            tcDemos.SetActiveTabIndex(0);
        } else if (e.item.name == "Custom") {
            //
        }
        e.processOnServer = false;
        e.usePostBack = false;
    }
    function IsCustomExportToolbarCommand(command) {
        return command == "CustomExportToXLS" || command == "CustomExportToXLSX";
    }
    function ClearForm() {
        var value = ["Clear", 0].join('|'); 
        //gvitems.PerformCallback(value);
        //gvutilize.PerformCallback(value);
        //ClientValidfrom.SetText("");
        //ClientValidto.SetText("");
        //ClientMaterial.SetText("");
    }
    function OnRowClick(s, e) {
        var row = s.GetRow(e.visibleIndex);
        //ClientUpCharge.SetValue(row.attributes["data"].nodeValue);
    }
    function OnRowDblClick(s, e) {
        //s.GetRowValues(e.visibleIndex, "Upcharge;Value;Currency;Unit;StdPackSize", OnGetRowId);
        //var row = s.GetRow(e.visibleIndex);
        //ASPxPopupControl1.Hide();
    }
    function OnGetRowId(row) {
        //ClientUpCharge.SetValue(row[0]);
        //tbUpChargePrice.SetValue(row[1]);
        //tbUpCurrency.SetValue(row[2]);
        //tbUpChargeUnit.SetValue(row[4]);
        //tbUpChargeQuantity.Focus();  
    }
    function OnButtonClick(text) {
        UpchargePopup.SetHeaderText(text);
        UpchargePopup.ShowAtElement(grid.GetMainElement());
    }
    function OnBtnShowPopupClick(text) {
        PopupControl.RefreshContentUrl();
        PopupControl.SetContentUrl('popupControls/selectedvalue.aspx?Id='+text);
        PopupControl.SetHeaderText("param:"+text);
        PopupControl.Show();
    }
    function clearDataUpcharge(s, e) {
        TextBoxUpCharge.SetText('');
        TextBoxUnit.SetText('');
        tbUpChargePrice.SetText('');
        lbUpcharge.PerformCallback('Clear');
    }
    function ClickSubmit(s, e) {
        
        grid.batchEditApi.SetCellValue(curentEditingIndex, 'UpCharge', TextBoxUpCharge.GetText());
        grid.batchEditApi.SetCellValue(curentEditingIndex, 'Quantity', TextBoxUnit.GetText());
        grid.batchEditApi.SetCellValue(curentEditingIndex, 'Currency', "USD");
        grid.batchEditApi.SetCellValue(curentEditingIndex, 'Price', tbUpChargePrice.GetText());
        grid.batchEditApi.SetCellValue(curentEditingIndex, 'Result', tbUpChargePrice.GetText());
        grid.GetValuesOnCustomCallback(['PackSize', keyValue].join('|'), function (r) {
            if (!r)
                return;
            grid.batchEditApi.SetCellValue(curentEditingIndex, 'stdPackSize', r["Unit"]);
            updategrid(grid);
        });
        UpchargePopup.Hide();
    }
    function lbUpchargeSelectedIndex(s, e) {
        
        //var items = s.GetSelectedItems();
        var keys = s.GetSelectedItem().value;
        grid.GetValuesOnCustomCallback(['Upcharg', keys, keyValue].join('|'), function (r) {
            if (!r)
                return;
            grid.batchEditApi.SetCellValue(curentEditingIndex, 'UpCharge', r["UpCharge"]);
            grid.batchEditApi.SetCellValue(curentEditingIndex, 'Quantity', r["StdPackSize"]);
            grid.batchEditApi.SetCellValue(curentEditingIndex, 'stdPackSize', r["Unit"]);
            grid.batchEditApi.SetCellValue(curentEditingIndex, 'Currency', "USD");
            grid.batchEditApi.SetCellValue(curentEditingIndex, 'Price', r["Value"]);
            grid.batchEditApi.SetCellValue(curentEditingIndex, 'Result', r["Result"]);
            updategrid(grid);
            //update 
        });
        
        UpchargePopup.Hide();
    }
    function HidePopupAndShowInfo(closedBy, returnValue) {
        PopupControl.Hide();
        //if (ClientMaterial.GetValue() == null) return alert('sold-to or ship-to should not be empty...');
        //gv.SetEditValue('Material', returnValue);
        
        if (closedBy == "Upcharg") {

        } else {
        gv.batchEditApi.SetCellValue(visibleIndex, "Material", returnValue);
        var myDate = addDays(30);
        var n = 30; //number of days to add. 
        var today = new Date(); //Today's Date
        var requiredDate = new Date(today.getFullYear(), today.getMonth(), today.getDate() + n)
        //ClientValidto.SetText(myDate);
        //var inputto = ClientValidto.GetText();
        gv.batchEditApi.SetCellValue(visibleIndex,'From', requiredDate);
        gv.batchEditApi.SetCellValue(visibleIndex,'To', requiredDate);
        gv.batchEditApi.SetCellValue(visibleIndex, 'Commission', 0);
        gv.batchEditApi.SetCellValue(visibleIndex, 'OverPrice', 0);
        gv.batchEditApi.SetCellValue(visibleIndex, 'OverType', 'USD');
        //ClientValidfrom.SetText(myDate);
        //ClientMaterial.SetText(returnValue);
        //OnDateChanged();
        //GetBuildCalcu(true);

        gv.GetValuesOnCustomCallback('MSC|' + returnValue, function (r) {
            if (!r)
                return;
            gv.batchEditApi.SetCellValue(visibleIndex,'Pacifical', r["Pacifical"]);
            gv.batchEditApi.SetCellValue(visibleIndex, 'MSC', r["MSC"]);
            gv.batchEditApi.SetCellValue(visibleIndex, 'Margin', r["Margin"]);
            gv.batchEditApi.SetCellValue(visibleIndex, 'Description', r["Description"]);
            gv.batchEditApi.SetCellValue(visibleIndex, 'Commission', r["Commission"]);
            gv.batchEditApi.SetCellValue(visibleIndex, 'OverPrice', r["OverPrice"]);
            gv.batchEditApi.SetCellValue(visibleIndex, 'OverType', r["OverType"]);

            OnSetValueToGridview('MSC', r["MSC"]);
            updategrid(gv);
        });
        }
    }
    function addDays(n) {
        var t = new Date();
        t.setDate(t.getDate() + n);
        var month = "0" + (t.getMonth() + 1);
        var date = "0" + t.getDate();
        month = month.slice(-2);
        date = date.slice(-2);
        var date = date + "/" + month + "/" + t.getFullYear();
        return date;
    }
    function OnCustomButtonClick(s, e) {
        //debugger;
        if (e.buttonID == 'customRecover') {

        }
        if (e.buttonID === "EvaluationEditBtn") {
            //editEvaluation(s.GetRowKey(e.visibleIndex), s);
            var key = s.GetRowKey(e.visibleIndex);
            s.StartEditRow(e.visibleIndex);
        }
    }
    function OnContextMenuItemClick(sender, args) {
        //debugger;
        if (args.objectType == "row") {
            if (args.item.name == "ExportToPDF" || args.item.name == "ExportToXLS") {
                args.processOnServer = true;
                args.usePostBack = true;
            }
            if (args.item.name == "calculation" || args.item.name == "Quotation") {
                var key = sender.GetRowKey(args.elementIndex);
                hGeID.Set("GeID", key);
                gridData.GetValuesOnCustomCallback("MailForm|" + args.item.name + "|" + key, Demo.OnGetRowValues);
            }
        }
    }
    function OnIsPaidClick(id, value) {
        //we do not get any data back, so the callback function is null
        //gridData.GetValuesOnCustomCallback("IsPaid|"+id + "|" + value, null);
    }
    function OnGridViewSelectionChanged(s, e) {
        var RowCount = s.GetSelectedRowCount();
        var view = Demo.DemoState.View;
        //var hiddenSelectedRowCount = gridData.GetSelectedRowCount() - GetSelectedFilteredRowCount();
        //alert(gridData.GetSelectedRowCount());
        //ClientActionMenu.GetItemByName("Approve").SetVisible(hiddenSelectedRowCount > 0);
        //ClientActionMenu.GetItemByName("Reject").SetVisible(RowCount > 0 && view == "MailList");
        //ClientActionMenu.GetItemByName("Approve").SetVisible(RowCount > 0 && view == "MailList");
    }
    function OnRemarkAtt() {
        //debugger;
        var valueatt = hfRefAtt.Get("RefAtt");
        var text = pComment.GetHeaderText();
        var value = [mRemark.GetText(),
            text, keyValue];
        hfcomment.Set("comment", value.join("|"));
        labelError.SetVisible(mRemark.GetText() == "");
        var isValid = UploadControl.GetText() != "";
        if (mRemark.GetText() == "") {
            var tbInputElement = mRemark.GetInputElement();
            //tbInputElement.style.color = "Gray";
            //tbInputElement.style.cursor = "not-allowed"; 
            //document.getElementById("myDiv").style.borderColor = "red";
            //document.getElementById("myDiv").innerText = "The Remark should not be empty...";
            return;
            //alert('The Remark should not be empty...');
        }
        if (valueatt != "") {
            gv.PerformCallback("RefAtt|" + keyValue + "|" + mRemark.GetText());
            pComment.Hide();
            mRemark.SetText('');
            cpUpdateFiles.PerformCallback(["reload", text].join("|"));
        } else if (isValid) {
            
            gv.batchEditApi.SetCellValue(visibleIndex, 'Notes', mRemark.GetText());
            UploadControl.Upload();
        }else {
            var param = ["editcomment", keyValue, value.join("|")];
            //if (Number.isInteger(text)) {
            //    param = ["comment", value.join("|")];        
            //} else
            //    param = ["editcomment", text, value.join("|")];
            gv.PerformCallback(param.join("|"));
            pComment.Hide();
            mRemark.SetText('');
            cpUpdateFiles.PerformCallback(["reload", text].join("|"));
        }

    }
    function OnFileComplete(s, e) {
        
        mRemark.SetText('');
        pComment.Hide();
        if (e.callbackData) {
            gv_refresh();
            //var fileData = e.callbackData.split('|');
            //var fileName = fileData[0],
            //    fileUrl = fileData[1],
            //    fileSize = fileData[2];
            //DXUploadedFilesContainer.AddFile(fileName, fileUrl, fileSize);
        }
    }
    function OnFocusedRowChanged(s, e) {
        ASPxClientUtils.SetCookie(s.name + '_focudedIndex', s.GetFocusedRowIndex());
        var index = s.GetFocusedRowIndex();
        var Keys = s.GetRowKey(index);
        //var deleteItem = gv.GetToolbar(0).GetItemByName('New');
        //deleteItem.SetVisible(false);  
        //var view = Demo.DemoState.View;

    }
    function OnInit(s, e) {
        if (ASPxClientUtils.GetCookie(s.name + '_focudedIndex') != null)
            s.SetFocusedRowIndex(ASPxClientUtils.GetCookie(s.name + '_focudedIndex'));
    }
    function Apply(key) {
        var rowCount = gv.GetVisibleRowsOnPage();
        for (let i = 0; i < rowCount; i++) {
            keyValue = gv.GetRowKey(i);
            var _margin = gv.batchEditApi.GetCellValue(i, 'Equivalent_Margin');
            var _apply = gv.batchEditApi.GetCellValue(i, 'Apply');
            if (_apply == "Apply") {
                gv.batchEditApi.SetCellValue(i, 'Margin', _margin);
                gv.batchEditApi.SetCellValue(i, 'Bidprice', "");
                gv.batchEditApi.SetCellValue(i, 'Apply', null);
                gv.GetValuesOnCustomCallback("_Margin" + '|' + keyValue, function (r) {
                    if (!r)
                        return;
                    var current = r["Result"];
                    //gv.batchEditApi.SetCellValue(gv.GetFocusedRowIndex(), 'Announcement_Fish_price', r["Announcement_Fish_price"]);
                    if (parseFloat(_margin) < parseFloat(current))
                        buildpComment("Margin", 'tbFreight');
                });
                updategrid(gv);
            }
        }
        //gv.UpdateEdit();
    }
    function downloadfile(key) {

        //debugger;
        //var key = gv.batchEditApi.GetCellValue(id, 'ID');
        //alert(key + ";" + id);
        //.NavigateUrlFormatString = "/Person/View/{0}"; 
        gv.GetValuesOnCustomCallback('download|' + key, function (r) {
            if (!r)
                return;
            //window.location = 'Content/UploadControl/' + r["Attached"];
            //window.open('Content/UploadControl/' + r["Attached"], '_blank');
            window.open('popupControls/DownloadFile.aspx?Id=' + r["Attached"]);
        });  
        //gv.PerformCallback('download|' + key);
    }
    function ShowDetailPopup(id) {
        var index = gv.GetFocusedRowIndex();
        keyValue = gv.GetRowKey(index);
        var cd = gv.batchEditApi.GetCellValue(index, 'Material');
        if (CmbIncoterm.GetValue() == "CFR" || CmbIncoterm.GetValue() == "CIF") {
            var name = 'SubContainers';
            if (gv.batchEditApi.GetCellValue(index, name) == "") {
                var n = prompt(name, "");
                if (n != null) {
                    if (!isNaN(n)) {
                        var current = gv.batchEditApi.GetCellValue(index, name);
                        gv.batchEditApi.SetCellValue(index, name, n);
                        updategrid(gv);
                    }
                }
                return;
            }
        }
        visibleIndex = index;
        grid.PerformCallback('reload|' + cd + '|' + id);
        tcDemos.SetActiveTabIndex(2);
        //EditPopup.ShowAtElement(gv.GetMainElement());
        //EditPopup.SetHeaderText("id:" + id);
    }
    var visibleIndex = 0;
    function onStartEdit(s, e) {
        visibleIndex = e.visibleIndex;
        //via key in args  
        //var key = e.key;
        // via visibleIndex  
        keyValue = s.GetRowKey(e.visibleIndex);  
        if (keyValue != null) {
            var targetArray = ["MSC", "Pacifical", "Margin"];
            if (targetArray.indexOf(e.focusedColumn.fieldName) > -1) {
                e.cancel = true;
                var n = prompt(e.focusedColumn.fieldName, "");
                if (n != null) {
                    if (!isNaN(n)) {
                        //debugger;
                        //var value = s.batchEditApi.GetCellValue(visibleIndex, e.focusedColumn.fieldName);
                        s.batchEditApi.SetCellValue(visibleIndex, e.focusedColumn.fieldName, n);
                        s.GetValuesOnCustomCallback("_" + e.focusedColumn.fieldName + '|' + keyValue + '|'+ n, function (r) {
                            if (!r)
                                return;
                            var current = r["Result"];
                            if (e.focusedColumn.fieldName == "Margin" && r["Announcement_Fish_price"] == "X")
                                buildpComment(e.focusedColumn.fieldName, 'tbFreight');
                            else if (parseFloat(n) < parseFloat(current) && e.focusedColumn.fieldName != "Margin")
                                buildpComment(e.focusedColumn.fieldName, 'tbFreight');
                        });
                        updategrid(gv);
                    }
                }
            }
            var EditArray = ["Commission", "OverPrice", "OverType", "SubContainers"];
            if (EditArray.indexOf(e.focusedColumn.fieldName) > -1) {
                e.cancel = true;
                var id = s.batchEditApi.GetCellValue(visibleIndex, "ID");
                EditPopup.ShowAtElement(gv.GetMainElement());
                EditPopup.SetHeaderText("id:" + id);
            }
            if (e.focusedColumn.fieldName == "Notes") {
                var Notes = s.batchEditApi.GetCellValue(visibleIndex, e.focusedColumn.fieldName);
                alert(Notes);
            }
        }
    }
    function onShown(s, e) {
        CmbCommission.SetText(gv.batchEditApi.GetCellValue(visibleIndex, "Commission"));
        textBoxOverPrice.SetText(gv.batchEditApi.GetCellValue(visibleIndex, "OverPrice"));
        CmbOverType.SetText(gv.batchEditApi.GetCellValue(visibleIndex, "OverType"));
        textBoxSubContainers.SetText(gv.batchEditApi.GetCellValue(visibleIndex, "SubContainers"));
    }
    function onAcceptClick(s, e) {
        gv.batchEditApi.SetCellValue(visibleIndex, "Commission", CmbCommission.GetText());
        gv.batchEditApi.SetCellValue(visibleIndex, "OverPrice", textBoxOverPrice.GetText());
        gv.batchEditApi.SetCellValue(visibleIndex, "OverType", CmbOverType.GetText());
        gv.batchEditApi.SetCellValue(visibleIndex, "SubContainers", textBoxSubContainers.GetText());
        EditPopup.Hide();
        updategrid(gv);  
    }
    function onCloseButtonClick(s, e) {
        if (visibleIndex <= -1)
            gv.batchEditApi.ResetChanges(visibleIndex);
    }
    var startPoint = { x: 0, y: 0 };
    var initialGridSize = { width: 0, height: 0 };
    var postpone = true;
    function GetResizeRect(grid) {
        if (!postpone) return;
        var id = grid.name + "ResizeRect";
        var element = document.getElementById(id);
        if (!element) {
            element = document.createElement("DIV");
            element.id = id;
            element.className = "ResizeRect";
            element.style.display = "none";
            document.body.appendChild(element);
        }
        return element;
    }
    function dragHelper_OnMouseDown(event) {
        var src = ASPxClientUtils.GetEventSource(event);
        startPoint.x = ASPxClientUtils.GetEventX(event);
        startPoint.y = ASPxClientUtils.GetEventY(event);
        initialGridSize.width = grid.GetWidth();
        initialGridSize.height = grid.GetHeight();
        var rectElement = GetResizeRect(grid);
        if (rectElement) {
            var gridMainElement = grid.GetMainElement();
            rectElement.style.left = ASPxClientUtils.GetAbsoluteX(gridMainElement) - 1 + "px";
            rectElement.style.top = ASPxClientUtils.GetAbsoluteY(gridMainElement) - 1 + "px";
            rectElement.style.width = initialGridSize.width + "px";
            rectElement.style.height = initialGridSize.height + "px";
            rectElement.style.display = "";
        }
        var mouseMoveHandler = function (ev) {
            ASPxClientUtils.ClearSelection();
            ResizeGrid(ASPxClientUtils.GetEventX(ev), ASPxClientUtils.GetEventY(ev), GetResizeRect(grid));
        };
        var mouseUpHandler = function (evt) {
            ResizeGrid(ASPxClientUtils.GetEventX(evt), ASPxClientUtils.GetEventY(evt));
            var rectElement = GetResizeRect(grid);
            if (rectElement)
                rectElement.style.display = "none";
            ASPxClientUtils.DetachEventFromElement(document, "mousemove", mouseMoveHandler);
            ASPxClientUtils.DetachEventFromElement(document, "mouseup", mouseUpHandler);
        };
        ASPxClientUtils.AttachEventToElement(document, "mousemove", mouseMoveHandler);
        ASPxClientUtils.AttachEventToElement(document, "mouseup", mouseUpHandler);
    }
    function ResizeGrid(x, y, rectElement) {
        var deltaX = x - startPoint.x;
        var deltaY = y - startPoint.y;
        var width = initialGridSize.width + deltaX;
        var height = initialGridSize.height + deltaY;
        if (rectElement) {
            rectElement.style.width = width + "px";
            rectElement.style.height = height + "px";
        }
        else {
            grid.SetWidth(width);
            grid.SetHeight(height);
        }
    }
    function OnTextChangedFreight(s, e) {
        debugger;
        var value = s.GetText();
        //var OldValue = OldValueFreight.Get("Freight");
        var _key = ["", 0, CmbRoute.GetValue(),
            CmbSize.GetValue(),
            CmbIncoterm.GetText(),
            CmbPaymentTerm.GetValue(),
            Clientinterest.GetText(),
            ClientCommission.Get("Commis")];
        gv.GetValuesOnCustomCallback('Freight|' + _key.join("|"), function (r) {
            if (!r)
                return;
            if (value != r["Freight"])
                buildpComment('Freight', 'tbFreight');
        });

        //var currentText = s.GetInputElement().value;  
        //
        gv.PerformCallback('recal|0');
        if (value != "")
            s.SetIsValid(true);
    }
    function OnMoreInfoClick(e, id) {
        
        UploadControl.ClearText();
        hfRefAtt.Set("RefAtt", "");
        //keyValue = id;
        pComment.SetHeaderText(id);
        pComment.Show();
    }
    //function OnDownloadClick(e, id) {
    //    //alert(id);
    //    gv.GetValuesOnCustomCallback('Attached|' + id, function (r) {
    //        if (!r)
    //            return;
    //        //window.location = 'Content/UploadControl/' + r["Attached"];
    //        //window.open('Content/UploadControl/' + r["Attached"], '_blank');
    //    });
    //}
    //var isNewClicked = false;
    //function hlNew_Click() {
    //    if (gv.batchEditApi.HasChanges()) {
    //        if (confirm("All changes will be saved automatically. Do you want to continue?"))
    //            gv.UpdateEdit();
    //        isNewClicked = true;
    //    }
    //    else {
    //        hf.Set("isNewClicked", true);
    //        cp.PerformCallback();
    //    }
    //}
    //function SetEndCallback() {
    //    
    //    if (isNewClicked == true) {
    //        isNewClicked = false;
    //        hf.Set("isNewClicked", true);
    //    }
    //    else hf.Set("isNewClicked", false);
    //    cp.PerformCallback();
    //}
    function SetButtonsVisibility(s, e) {
        
        if (!s.cpUpdatedMessage)
            return;
        //AdjustSize();
        //if (!s.cpFilterExpression)
        //    return;
        //hfKeyword.Set("Keyword", s.cpFilterExpression);
        //s.cpFilterExpression = null;
        //s.PerformCallback("X");
        //debugger;
        if (s.cpUpdatedMessage == 'Submitted') {
            gv_refresh();
        }
            //tcDemos.SetActiveTabIndex(0);
        if (s.cpUpdatedMessage) {
            //if (s.cpUpdatedMessage == "Error") 
            //alert(s.cpUpdatedMessage);
            //gvitems.PerformCallback("clear");
            delete s.cpUpdatedMessage;
            //curentEditingIndex = 0;
            //curentKeys = 0;
            //ClearForm();
        }
    }
   function gv_refresh(){
       var key = gv.GetRowKey(gv.GetFocusedRowIndex());
        gv.PerformCallback('ref|' + key);
       //gv.Refresh(); 
    }
    function grid_refresh() {
        //grid.PerformCallback('refresh|' +keyValue);
        var key = gv.GetRowKey(gv.GetFocusedRowIndex());
        grid.PerformCallback('ref|' + key);
        gv_refresh();
    }
    function DoProcessEnterKey(htmlEvent, editName) {
        if (htmlEvent.keyCode == 13) {
            ASPxClientUtils.PreventEventAndBubble(htmlEvent);
            if (editName) {
                ASPxClientControl.GetControlCollection().GetByName(editName).SetFocus();
            } else {
                btnAccept.DoClick();
            }
        }
    }
    function grid_OnCustomButtonClick(s, e) {
        if (e.buttonID == 'edcolUpload') {
            var rowVisibleIndex = e.visibleIndex;
            var rowKeyValue = s.GetRowKey(rowVisibleIndex);
            //alert('Row key value: ' + rowKeyValue);  
            s.GetRowValues(e.visibleIndex, 'Result', function (value) {
                buildpComment(value[0], 'tbFreight');
            });
        }
        if (e.buttonID == 'decolUpload') {

        }

    }
    function cbAtt_Init(s, e) {
 
    }
    function Aply_ValueChanged(s, e) {

    }
    function buildzone(_keys) {
        //debugger;
        gv.GetValuesOnCustomCallback('zone|' + _keys, function (r) {
            if (!r)
                return;
            clientzone.SetValue(r["zone"]);
        });
    }
    function OnZoneChanged(value) {

    }
    function OnGridViewEndCalback(s, e) {

    }
    function OncheckValidate() {
        if (!ASPxClientEdit.ValidateGroup('group1')) {
            return alert('Field is required');
        }
    }
    function OnGridSelectionChanged(s, e) {
        setGridHeaderCheckboxes(s, e);
    }
    function setGridHeaderCheckboxes(s, e) {
        //cbAll
        //debugger;
        var indexes = gv.cpIndexesSelected;
        //cbAll.SetChecked(s.GetSelectedRowCount() == Object.size(indexes));
        var view = Demo.DemoState.View;
        var RowCount = gv.GetSelectedRowCount();
        //cbPage
        var allEnabledRowsOnPageSelected = true;
        var indexes = gv.cpIndexesUnselected;
        var topVisibleIndex = gv.GetTopVisibleIndex();
        for (var i = topVisibleIndex; i < topVisibleIndex + gv.cpPageSize; i++) {
            if (indexes.indexOf(i) == -1)
                if (!gv.IsRowSelectedOnPage(i)) allEnabledRowsOnPageSelected = false;
        }
        ClientActionMenu.GetItemByName("Approve").SetVisible(view == "MailForm" && allEnabledRowsOnPageSelected == false);
        //ClientActionMenu.GetItemByName("Approve").SetVisible(view == "MailForm" && allEnabledRowsOnPageSelected == true);
        //cbPage.SetChecked(allEnabledRowsOnPageSelected);
    }
    function OnConfirm(_Bidprice) {
        // JS Code to handle post confirm operation
        //var _Bidprice = gv.batchEditApi.GetCellValue(gv.GetFocusedRowIndex(), 'Bidprice');
        var _key = gv.GetRowKey(gv.GetFocusedRowIndex());
        gv.PerformCallback('Change|' + _key + "|" + _Bidprice);
        //gv.batchEditApi.SetCellValue(1, 'Announcement_Fish_price', _Bidprice);
    }
</script>
<dx:ASPxHiddenField ID="DXColumnsWidth" ClientInstanceName="dxColumnsWidth" runat="server" />
<dx:ASPxHiddenField ID="hf" runat="server" ClientInstanceName="hf"/>
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
<dx:ASPxHiddenField ID="hfcomment" runat="server" ClientInstanceName="hfcomment"/>
<dx:ASPxHiddenField ID="hfRefAtt" runat="server" ClientInstanceName="hfRefAtt"/>
<%--<dx:ASPxLabel ID="ASPxLabel1" runat="server" />--%>
<dx:ASPxHint ID="ASPxHint1" runat="server"></dx:ASPxHint>
<dx:ASPxGridView runat="server" ID="gridData" ClientInstanceName="gridData"  Width="100%" DataSourceID="dsgv"
        AutoGenerateColumns="true" KeyFieldName="ID" OnCustomCallback="gridData_CustomCallback" OnCustomButtonCallback="gridData_CustomButtonCallback"
        OnCustomDataCallback="gridData_CustomDataCallback" OnFillContextMenuItems="gridData_FillContextMenuItems"
        Border-BorderWidth="0">
        <Columns>
            <dx:GridViewDataCheckColumn FieldName="FlagColor" VisibleIndex="1">
                    <DataItemTemplate>
                        <dx:ASPxCheckBox ID="cb" runat="server" Checked='<%# Convert.ToBoolean(Convert.ToInt32(Eval("Flag"))) %>' OnInit="cb_Init">
                            <ClientSideEvents CheckedChanged="Flag_CheckedChanged" />
                        </dx:ASPxCheckBox>
                    </DataItemTemplate>
                </dx:GridViewDataCheckColumn>
            <dx:GridViewCommandColumn ShowSelectCheckbox="false" ShowNewButton="true" ShowEditButton="true" VisibleIndex="0" ButtonRenderMode="Image" Width="45">
            <CustomButtons>
                <dx:GridViewCommandColumnCustomButton ID="Clone">
                    <Image ToolTip="Clone Record" Url="~/Content/Images/Copy.png"/>
                </dx:GridViewCommandColumnCustomButton>
            </CustomButtons>
            </dx:GridViewCommandColumn>
             <%--<dx:GridViewCommandColumn ShowClearFilterButton="true" Width="45px">
           <HeaderStyle CssClass="unitPriceColumn" />
                <EditCellStyle CssClass="unitPriceColumn" />
                <FilterCellStyle CssClass="unitPriceColumn" />
                <CellStyle CssClass="unitPriceColumn" />
                <FooterCellStyle CssClass="unitPriceColumn" />
                <GroupFooterCellStyle CssClass="unitPriceColumn" />
            </dx:GridViewCommandColumn>--%>
            <dx:GridViewDataColumn FieldName="ID" Width="70px" 
                HeaderStyle-HorizontalAlign="Center" 
                CellStyle-HorizontalAlign="Center">
                <%--<DataItemTemplate>
                    <input type='checkbox' <%# GetCheckBoxChecked(Eval("IsPaid"))%> onclick='OnIsPaidClick( <%# Eval("Id") %>, this.checked);' />
                </DataItemTemplate>--%>
            </dx:GridViewDataColumn>
            <dx:GridViewDataComboBoxColumn FieldName="Customer">
                <PropertiesComboBox DataSourceID="dsCustomer" TextField="Name" ValueField="Code">
                </PropertiesComboBox>
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataTextColumn FieldName="ShipToName" Caption="Ship To" />
            <dx:GridViewDataTextColumn FieldName="RequestNo"/>
            <dx:GridViewDataTextColumn FieldName="Requester"/>
            <dx:GridViewDataComboBoxColumn FieldName="StatusApp" GroupIndex="0">
                <PropertiesComboBox DataSourceID="dsStatusApp" TextField="Title" ValueField="ID" />
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataDateColumn FieldName="ValidityDate" Caption="Validity Date"> 
                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                </PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataDateColumn FieldName="CreateOn" Caption="CreateOn"> 
                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                </PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataColumn Caption="Details" Width="15%">
                <DataItemTemplate>
                    <a href="javascript:void(0);" data-visibleindex="<%# Container.VisibleIndex %>" 
                        class="<%# Eval("FileDetails").ToString() == "0" ? "hide" : string.Empty %>">More Info...</a>
                </DataItemTemplate>
<%--                <HeaderStyle CssClass="unitPriceColumn" />
                <EditCellStyle CssClass="unitPriceColumn" />
                <CellStyle CssClass="unitPriceColumn" />
                <FilterCellStyle CssClass="unitPriceColumn" />
                <FooterCellStyle CssClass="unitPriceColumn" />
                <GroupFooterCellStyle CssClass="unitPriceColumn" />--%>
            </dx:GridViewDataColumn>
            <dx:GridViewDataTextColumn FieldName="Remark" Caption="Note"/>
        </Columns>
<%--        <Templates>
            <DetailRow>
                <div class="filesContainer">
                    <dx:UploadedFilesContainer ID="FileContainer" runat="server" Width="380" Height="180" 
                NameColumnWidth="240" SizeColumnWidth="70" HeaderText="Uploaded files" /></div>
            </DetailRow>
        </Templates>
        <SettingsDetail ShowDetailRow="true" />--%>
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
        <ClientSideEvents 
        SelectionChanged="OnGridViewSelectionChanged"
        Init="Demo.gridData_Init" 
        EndCallback="OnEndCallback" ContextMenuItemClick="function(s,e) { OnContextMenuItemClick(s, e); }" 
        RowDblClick="Demo.ClientgridData_RowDblClick" />
</dx:ASPxGridView>
<dx:ASPxGridViewExporter runat="server" ID="GridExporter" GridViewID="gridData" />
<dx:ASPxPopupControl ID="PopupControl" runat="server" ClientInstanceName="PopupControl" CloseAction="OuterMouseClick" CloseOnEscape="true" 
            Width="720px" Height="500px" AllowResize="true" Modal="True"
            HeaderText="Results" AllowDragging="True" PopupAnimationType="Fade"
            EnableViewState="False">
        <ClientSideEvents PopUp="function(s, e) { ASPxClientEdit.ClearGroup('createAccountGroup'); }" CloseButtonClick="function(s, e) {gv_refresh();}" />
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
    <dx:ASPxPopupControl ID="pComment" runat="server" ClientInstanceName="pComment" AllowResize="true"
		    CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" 
        AllowDragging="True" PopupAnimationType="None" EnableViewState="False" AutoUpdatePosition="true">
        <ClientSideEvents PopUp="function(s, e) { ASPxClientEdit.ClearGroup('createAccountGroup'); }" Closing="function(s, e) { labelError.SetVisible(false);}" />
        <SizeGripImage Width="11px" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxPanel ID="ASPxPanel1" runat="server" DefaultButton="btCreate">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                        <dx:ASPxFormLayout ID="ASPxFormLayout2" runat="server" RequiredMarkDisplayMode="Auto" 
                            UseDefaultPaddings="false" AlignItemCaptionsInAllGroups="true" Width="100%">
                            <Paddings PaddingBottom="30" PaddingTop="10" />
                            <Styles>
                                <LayoutGroupBox CssClass="fullWidth fullHeight"></LayoutGroupBox>
                                <LayoutGroup Cell-CssClass="fullHeight"></LayoutGroup>
                            </Styles>
                            <Items>
                            <dx:LayoutGroup Caption="Comment" GroupBoxDecoration="None">
                            <SpanRules>
                                <dx:SpanRule BreakpointName="S" ColumnSpan="1" RowSpan="1" />
                            </SpanRules>
                            <Items>
                                <dx:LayoutItem Caption="Upload">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxUploadControl ID="UploadControl" runat="server" ClientInstanceName="UploadControl" 
                                            OnFileUploadComplete="UploadControl_FileUploadComplete"
                                            NullText="Select multiple files..." UploadMode="Advanced" ShowUploadButton="false" ShowProgressPanel="True">
                                            <AdvancedModeSettings EnableMultiSelect="True" EnableFileList="True" EnableDragAndDrop="True"  />
                                            <ValidationSettings MaxFileSize="10485760" AllowedFileExtensions=".jpg,.jpeg,.gif,.png,.xls,.xlsx,.pdf">
                                            </ValidationSettings>
                                            <ClientSideEvents FileUploadComplete="OnFileComplete" />
                                    </dx:ASPxUploadControl>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Notes">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxMemo runat="server" ID="mRemark" Rows="6" ClientInstanceName="mRemark" Width="100%">
                                    </dx:ASPxMemo>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <%--<dx:LayoutItem Caption="Topic">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxComboBox runat="server" ID="cmbTopic" Rows="6" ClientInstanceName="clientTopic"
                                            DataSourceID="dsTopic"
                                            Width="100%">
                                    </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>--%>
                            <dx:LayoutItem Caption="ref">
                                    <LayoutItemNestedControlCollection>
                                  <dx:LayoutItemNestedControlContainer>
                                  <dx:ASPxCallbackPanel ID="cpRefAtt" runat="server" Width="100%" ClientInstanceName="cpRefAtt" OnCallback="cpRefAtt_Callback" 
                                      ClientVisible="false">
                                  <PanelCollection>
                                  <dx:PanelContent runat="server">
                                        <dx:ASPxRadioButtonList runat="server" ID="cbAtt" Rows="1" TextField="Description" ValueField="ID" 
                                            ClientInstanceName="cbAtt" Width="100%">
                                            <ClientSideEvents Init="function(s, e){ cbAtt.SetSelectedItem(null);}" SelectedIndexChanged="function(s, e){ 
                                                hfRefAtt.Set('RefAtt',cbAtt.GetValue());}"/>
                                    </dx:ASPxRadioButtonList>
                                </dx:PanelContent></PanelCollection>
                                    </dx:ASPxCallbackPanel>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                            <input type="button" value="Submit" onclick="OnRemarkAtt()"/>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                        </Items>
                        </dx:LayoutGroup>
                        </Items>
                        </dx:ASPxFormLayout>
                        <dx:ASPxLabel ID="labelError" runat="server" Text="The Remark should not be empty..." ForeColor="Red"
                            ClientInstanceName="labelError" Font-Size="8pt" ClientVisible="false">
                        </dx:ASPxLabel>
                        <br />
                        <p class="note">
                            <dx:ASPxLabel ID="AllowedFileExtensionsLabel" runat="server" 
                                Text="Allowed file extensions: .jpg, .jpeg, .gif, .png, .xls, .xlsx, .pdf." Font-Size="8pt">
                            </dx:ASPxLabel>
                            <br />
                            <dx:ASPxLabel ID="MaxFileSizeLabel" runat="server" Text="Maximum file size: 10 MB." Font-Size="8pt">
                            </dx:ASPxLabel>
                        </p>
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
                    <dx:ASPxFormLayout runat="server" ID="ASPxFormLayout1" ClientInstanceName="ASPxFormLayout1" CssClass="formLayout" AlignItemCaptionsInAllGroups="true">
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
                                        ReadOnly="true" Font-Bold="true" BackColor="#cccccc">
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Valid From">
                        <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxDateEdit runat="server" ID="defrom" ClientInstanceName="ClientValidfrom" 
                                       DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                            <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                            <ClientSideEvents DateChanged="OnValidDateChanged" />
                                            </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Valid To">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxDateEdit runat="server" ID="deto" ClientInstanceName="ClientValidto"
                                                DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                                <ClientSideEvents DateChanged="OnValidDateChanged" />
                                            </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    <dx:LayoutItem Caption="Validity Date" Name="Validity_Date">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <div style="background-color:red">
                                    <dx:ASPxDateEdit runat="server" ID="deValidityDate" ClientInstanceName="deValidityDate" ReadOnly="true"
                                                DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                            <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                            <ClientSideEvents DateChanged="OnDateChanged" />
                                            </dx:ASPxDateEdit></div>
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
                            <dx:ASPxComboBox ID="CmbCustomer" runat="server" ValueField="Code" IncrementalFilteringMode="Contains" DropDownWidth="550px" 
                                ClientInstanceName="ClientCustomer" DataSourceID="dsCustomer" TextField="Name" TextFormatString="{0},{1}">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="Code" Width="20%"/>
                                    <dx:ListBoxColumn FieldName="Name"/>
                                </Columns>
                                <ClientSideEvents SelectedIndexChanged="ClearOnTerm" />
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
                            <dx:ASPxComboBox ID="CmbShipTo" runat="server" ValueField="Code" IncrementalFilteringMode="Contains" DropDownWidth="550px"
                                ClientInstanceName="ClientShipTo" DataSourceID="dsCustomer" TextField="Name" TextFormatString="{0},{1}">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="Code" Width="20%"/>
                                    <dx:ListBoxColumn FieldName="Name"/>
                                    <dx:ListBoxColumn FieldName="Zone"/>
                                </Columns>
                                <ClientSideEvents SelectedIndexChanged="function(s, e) { 
                                    //debugger;
                                    var azone = s.GetSelectedItem().GetColumnText(2);
                                    clientzone.SetValue(azone);
                                    //buildzone(s.GetValue());
                                    }" />
                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                            </dx:ASPxComboBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Bill-to">
                    <SpanRules>
                        <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                        <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                    </SpanRules>
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer>
                            <dx:ASPxComboBox ID="CmbBillTo" runat="server" ValueField="Code" IncrementalFilteringMode="Contains" DropDownWidth="550px"
                                ClientInstanceName="ClientBillTo" DataSourceID="dsSelectPayment" TextField="Name" TextFormatString="{0},{1}">
                                <ClientSideEvents SelectedIndexChanged="function(s, e) { 
                                    //debugger;
                                    var aPayment = s.GetSelectedItem().GetColumnText(2);
                                    var aLeadTime = s.GetSelectedItem().GetColumnText(3);
                                    var custo = ClientCustomer.GetValue();
                                    if(custo !=''){
                                    buildInterest(custo, aLeadTime);
                                    }
                                    CmbPaymentTerm.SetValue(aPayment);
                                    }" />
                                <Columns>
                                    <dx:ListBoxColumn FieldName="Code" Width="15%"/>
                                    <dx:ListBoxColumn FieldName="Name" Width="65%"/>
                                    <dx:ListBoxColumn FieldName="knvv_zterm" Caption="Payment" Width="10%"/>
                                    <dx:ListBoxColumn FieldName="LeadTime" Width="10%"/>
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
                                            <dx:ASPxHiddenField ID="hfOldValueExchangeRate" runat="server" ClientInstanceName="OldValueExchangeRate" />
                                            <dx:ASPxSpinEdit runat="server" ID="seExchangeRate"  
                                                ClientInstanceName="seExchangeRate" NumberType="Float" 
                                                Number="1.00" Width="80px">
                                                <ClientSideEvents ValueChanged="OnExchangeRate" KeyDown="function(s, e) { DoProcessEnterKey(e.htmlEvent, 'seExchangeRate'); }" />
                                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                            </dx:ASPxSpinEdit></td><td>&nbsp;</td><td>
                                            <dx:ASPxComboBox runat="server" ID="CmbCurrency" 
                                                ClientInstanceName="ClientCurrency" Width="80px" DataSourceID="dsCurrency" 
                                                    ValueField="value" TextField="Value">
                                                    <ClientSideEvents Init="function(s, e) { s.SetText('USD'); }"/>
                                                    <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
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
                                    <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
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
                                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
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
                                    <ClientSideEvents SelectedIndexChanged="OnSelectedIndexChanged" 
                                        ValueChanged="function(s, e) {  
                                                        OnValueChanged('tot',0);}" />
                                    <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>    
                        <dx:LayoutItem Caption="Route">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="CmbRoute" runat="server" ClientInstanceName="CmbRoute" DataSourceID="dsRoute" 
                                        TextFormatString="{0};{1}" EnableCallbackMode="true" ValueField="Code" IncrementalFilteringMode="Contains">
                                        <Columns>
                                        <dx:ListBoxColumn FieldName="Code" Width="80px"/>
                                        <dx:ListBoxColumn FieldName="Name" Width="350px"/>
                                        </Columns>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) {
                                            if (s.GetValue() == null) 
                                                s.SetSelectedIndex(-1);
                                            else
                                                s.SetIsValid(true);
                                            }"
                                          />
                                        <ValidationSettings ValidateOnLeave="False" EnableCustomValidation="True"/>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Size">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
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
                                            SelectedIndexChanged="function(s, e) {  if (s.GetValue() == null) 
                                            s.SetSelectedIndex(-1)
                                            else
                                                s.SetIsValid(true);}"/>
                                        <ValidationSettings ValidateOnLeave="False" EnableCustomValidation="True"/>
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
                                        <dx:ASPxTextBox ID="tbFreight" runat="server" ClientInstanceName="tbFreight" Width="120px">
                                            <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKey(e.htmlEvent, 'tbFreight'); }"
                                                 TextChanged="OnTextChangedFreight"/>
                                            <ValidationSettings ValidateOnLeave="False" EnableCustomValidation="True"/>
                                        </dx:ASPxTextBox>
                                        <dx:ASPxHiddenField ID="hfOldValueFreight" runat="server" ClientInstanceName="OldValueFreight" />
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Insurance">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTextBox ID="tbInsurance" runat="server" ClientInstanceName="tbInsurance" Width="120px" ReadOnly="true">
                                            <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                ValueChanged="function(s, e) { OnSetValueToGridview('Interest',s.GetText());}" />
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>      
                            <dx:LayoutItem Caption="Zone">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxComboBox ID="cmbZone" runat="server" 
                                            ClientInstanceName="clientzone" DataSourceID="dszone" ValueField="value" TextField="value" >
                                            <ClientSideEvents ValueChanged="function(s, e) {  
                                                        OnZoneChanged(s.GetValue());}" 
                                            SelectedIndexChanged="function(s, e) {  if (s.GetValue() == null) 
                                                s.SetSelectedIndex(-1)
                                            else
                                                s.SetIsValid(true);}"/>
                                            <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                        </dx:ASPxComboBox>
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
                                    <dx:ASPxMemo runat="server" ID="mNotes" ClientInstanceName="ClientNotes">
                                    </dx:ASPxMemo>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <div id="my_div"></div> 
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
                            <dx:LayoutItem Caption=""  Width="100%">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTabControl ID="tcDemos" runat="server" NameField="Id" DataSourceID="XmlDataSource1"
                                            ActiveTabIndex="0" ClientInstanceName="tcDemos">
                                            <ClientSideEvents ActiveTabChanged="function(s, e) { OnActiveTabChanged(s, e); }" 
                                                Init="function(s, e) {s.SetActiveTabIndex(0);}"/>
                                        </dx:ASPxTabControl>
                                        <asp:XmlDataSource ID="XmlDataSource1" runat="server" DataFile="~/App_Data/Platforms.xml"
                                            XPath="//Item"></asp:XmlDataSource>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <%--<dx:LayoutItem Caption=""  Width="100%">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTabControl ID="ASPxTabControl1" runat="server" NameField="Id" DataSourceID="XmlDataSource2"
                                            ActiveTabIndex="0" ClientInstanceName="ASPxTabControl1">
                                            <ClientSideEvents Init="function(s, e) {s.SetActiveTabIndex(0);}" ActiveTabChanged="function(s, e) {
                                                tabbed.SetActiveTabIndex(s.GetActiveTabIndex());}"/>
                                        </dx:ASPxTabControl>
                                        <asp:XmlDataSource ID="XmlDataSource2" runat="server" DataFile="~/App_Data/Platforms.xml"
                                            XPath="//Quot"></asp:XmlDataSource>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem> --%>
                            <dx:TabbedLayoutGroup Caption="SubTotal" ActiveTabIndex="0" ClientInstanceName="tabbedGroupPageControl" ShowGroupDecoration="false" Width="100%">
                            <Items>
                            <dx:LayoutGroup Caption="Details" ColCount="4" GroupBoxDecoration="None" UseDefaultPaddings="false">
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
                                            <ClientSideEvents FileUploadComplete="OnFileUploadComplete" FilesUploadStart="function(s,e){
                                                if (!ASPxClientEdit.ValidateGroup('group1')) {
                                                    return alert('Field is required'); 
                                                    }
                                                }" />
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
                                    <%--<dx:ASPxCallbackPanel ID="ASPxCallbackPanel1" runat="server" ClientInstanceName="cp" OnCallback="ASPxCallbackPanel1_Callback">
                                    <PanelCollection>
                                    <dx:PanelContent runat="server">
                                    </dx:PanelContent>
                                    </PanelCollection>
                                    </dx:ASPxCallbackPanel>--%>
                                    <dx:ASPxGridView ID="gv" runat="server" ClientInstanceName="gv" KeyFieldName="ID" OnBatchUpdate="gv_BatchUpdate" OnDataBound="gv_DataBound"
                                    OnCustomDataCallback="gv_CustomDataCallback" OnCustomCallback="gv_CustomCallback" OnHtmlRowPrepared="gv_HtmlRowPrepared" 
                                    OnDataBinding="gv_DataBinding" OnCellEditorInitialize="gv_CellEditorInitialize" OnCommandButtonInitialize="gv_CommandButtonInitialize" 
                                    Width="100%" EnableCallbackAnimation="false">
                                    <ClientSideEvents  Init="OnInit" CustomButtonClick="OnCustomButtonClick"
                                        BatchEditEndEditing="OnEndEdit" SelectionChanged="OnGridSelectionChanged" 
                                        ToolbarItemClick="OnToolbarItemClick" BatchEditStartEditing="onStartEdit" FocusedRowChanged="OnFocusedRowChanged" EndCallback="gv_OnEndCallback"/>
                                         <%--EndCallback="SetButtonsVisibility" EndCallback="SetEndCallback" 
                                        Init="function(s, e) { PreventEditFormPopupAnimation(s); }" BeginCallback="OnGridBeginCallback"
                                        RowDblClick="gv_RowDblClick"/>--%>
                                        <Toolbars>
                                            <dx:GridViewToolbar Name="MyToolbar">
                                                <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                                                <Items>
                                                    <%--<dx:GridViewToolbarItem Command="Custom" Name="Custom" Text="New" Image-IconID="">
                                                        <Image Url="~/Content/Images/icons8-add-file-20.png" Height="16px" Width="16px"></Image> 
                                                    </dx:GridViewToolbarItem>--%>
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
                                    <%--<dx:GridViewCommandColumn ShowSelectCheckbox="True" ShowClearFilterButton="true" Width="35px" VisibleIndex="18">
                                        <HeaderStyle CssClass="unitPriceColumn" />
                                        <EditCellStyle CssClass="unitPriceColumn" />
                                            <FilterCellStyle CssClass="unitPriceColumn" />
                                        <CellStyle CssClass="unitPriceColumn" />
                                        <FooterCellStyle CssClass="unitPriceColumn" />
                                        <GroupFooterCellStyle CssClass="unitPriceColumn" />
                                    </dx:GridViewCommandColumn>--%>
                                    <dx:GridViewDataButtonEditColumn FieldName="Material" ReadOnly="true" Width="12%" VisibleIndex="1">
                                        <PropertiesButtonEdit>
                                            <Buttons>
                                                <dx:EditButton>
                                                </dx:EditButton>
                                            </Buttons>
                                            <ClientSideEvents ButtonClick="function(s,e){
                                                if (!ASPxClientEdit.ValidateGroup('group1')) {
                                                    return alert('Field is required'); 
                                                }
                                                if(CmbIncoterm.GetValue() == null)
                                                    return alert('The Incoterm should not be empty...');
                                                    OnBtnShowPopupClick('Material');
                                                }" TextChanged ="function(s,e){var aa = gv.GetRowKey(gv.GetFocusedRowIndex());} " />
                                        </PropertiesButtonEdit>
                                    </dx:GridViewDataButtonEditColumn>
                                    <dx:GridViewDataColumn FieldName="Description" ReadOnly="true" Width="20%" VisibleIndex="2">
                                    </dx:GridViewDataColumn>
                                   <dx:GridViewDataTextColumn FieldName="Commission" VisibleIndex="3">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewBandColumn Caption="Over Price">
                                    <Columns>
                                    <dx:GridViewDataTextColumn FieldName="OverPrice" Caption="Price" VisibleIndex="4"/>
                                    <%--<dx:GridViewDataTextColumn FieldName="OverPrice" Caption="Price">
                                        <PropertiesTextEdit/>
                                            <EditItemTemplate>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <dx:ASPxTextBox ID="tbOverPrice" runat="server" Text='<%# Bind("OverPrice")%>' ClientInstanceName="tbOverPrice">
                                                            <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                    <td>&nbsp;</td>
                                                    <td>
                                                        <dx:ASPxComboBox ID="cmbOverType" runat="server" Width="35px" Value='<%# Bind("OverType") %>'
                                                            DataSourceID="dsOverType" ValueField="value" TextField="value">
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                    </dx:GridViewDataTextColumn>--%>

                                    <dx:GridViewDataColumn FieldName="OverType" Caption="Unit" VisibleIndex="5"/>
                                    </Columns>

                                    </dx:GridViewBandColumn>
                                    <dx:GridViewDataColumn FieldName="FishGroup" VisibleIndex="5" Visible="false"/>
                                    <dx:GridViewDataColumn FieldName="EUGEN" Caption="EU/GEN" VisibleIndex="5" Visible="false"/>
                                    <dx:GridViewDataTextColumn FieldName="Pacifical" VisibleIndex="6">
                                         <PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="MSC" VisibleIndex="7">
                                        <PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="Margin" Caption="%Margin" VisibleIndex="8">
                                        <PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
                                        <PropertiesTextEdit DisplayFormatString="F4" />
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="totalUpcharge" Caption="Upcharge" EditFormSettings-Visible="False" Width="60px" VisibleIndex="10">
                                        <PropertiesTextEdit DisplayFormatString="F4" />
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="SubContainers" Caption="Case per fcl." VisibleIndex="9">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="OfferPrice" EditFormSettings-Visible="False" Width="60px" VisibleIndex="10">
                                        <PropertiesTextEdit DisplayFormatString="F4" />
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewBandColumn Caption="Announcement" VisibleIndex="11">
                                        <Columns>
                                            <dx:GridViewDataTextColumn FieldName="Equivalent_Fish_price" Caption="Fish Price" EditFormSettings-Visible="False" Width="60px" >
                                                <PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
                                                <PropertiesTextEdit DisplayFormatString="F2" />
                                            </dx:GridViewDataTextColumn>
                                        </Columns>
                                    </dx:GridViewBandColumn>
                                    <dx:GridViewBandColumn Caption="Authorized" VisibleIndex="11">
                                        <Columns>
                                            <dx:GridViewDataTextColumn FieldName="Authorized_price" Caption="Fish Price" Width="60px" >
                                                <PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
                                                <PropertiesTextEdit DisplayFormatString="F2" />
                                            </dx:GridViewDataTextColumn>
                                        </Columns>
                                    </dx:GridViewBandColumn>
                                    <dx:GridViewBandColumn Caption="Bid" VisibleIndex="12">
                                        <Columns>
                                            <dx:GridViewDataTextColumn FieldName="Announcement_Fish_price" Caption="Fish Price" Width="60px">
                                                <PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                    ClientSideEvents-ValueChanged="function(s,e){  
                                                    
                                                    gv.batchEditApi.SetCellValue(gv.GetFocusedRowIndex(), 'Apply', 'Apply');
                                                    gv.batchEditApi.SetCellValue(gv.GetFocusedRowIndex(), 'Bidprice', '');
                                                    
                                                    if (confirm('change all items?'))
                                                           return OnConfirm(s.GetValue());
                                                    }" />

                                                <PropertiesTextEdit DisplayFormatString="F2" />
                                            </dx:GridViewDataTextColumn>
                                        </Columns>
                                    </dx:GridViewBandColumn>
                                    <dx:GridViewBandColumn Caption="Bid" VisibleIndex="13">
                                        <Columns>
                                            <dx:GridViewDataTextColumn FieldName="Bidprice" Caption="Unit Price">
                                                <PropertiesTextEdit ClientSideEvents-KeyPress="function(s,e){ 
                                                    fn_AllowonlyNumeric(s,e);}" 
                                                    ClientSideEvents-ValueChanged="function(s,e){
                                                    //debugger;
                                                    if(s.GetValue()==null){
                                                        s.SetValue(0);
                                                    }
                                                    
                                                    //alert(s.GetValue());
                                                    gv.batchEditApi.SetCellValue(gv.GetFocusedRowIndex(), 'Apply', 'Apply');
                                                    gv.batchEditApi.SetCellValue(gv.GetFocusedRowIndex(), 'Announcement_Fish_price', '');}" />
                                                <PropertiesTextEdit DisplayFormatString="F2" />
                                            </dx:GridViewDataTextColumn>
                                        </Columns>
                                    </dx:GridViewBandColumn>
                                    <dx:GridViewBandColumn Caption="Equivalent" VisibleIndex="14">
                                        <Columns>
                                            <dx:GridViewDataTextColumn FieldName="Equivalent_Margin" Caption="Margin" ReadOnly="true"/>
                                        </Columns>
                                    </dx:GridViewBandColumn>
                                    <dx:GridViewDataHyperLinkColumn FieldName="Apply" UnboundType="String" UnboundExpression="[ID]"  VisibleIndex="15">
                                        <PropertiesHyperLinkEdit TextField="Apply" Style-ForeColor="Red" DisplayFormatString="{0}" NavigateUrlFormatString="javascript:Apply('{0}');">
                                        </PropertiesHyperLinkEdit>
                                    </dx:GridViewDataHyperLinkColumn>
                                    <dx:GridViewDataHyperLinkColumn FieldName="Notes" EditFormSettings-Visible="False" Width="20px" VisibleIndex="16">
                                        <PropertiesHyperLinkEdit NavigateUrlFormatString="javascript:alert('{0}');" TextField="Notes" />
                                    </dx:GridViewDataHyperLinkColumn>
                                    <dx:GridViewDataHyperLinkColumn FieldName="HyperLinkColumn2" UnboundType="String" UnboundExpression="[ID]" Caption="ref" Width="20px" VisibleIndex="17">
                                        <PropertiesHyperLinkEdit TextField="Attached" DisplayFormatString="{0}" NavigateUrlFormatString="javascript:downloadfile('{0}');">
                                        </PropertiesHyperLinkEdit>
                                    </dx:GridViewDataHyperLinkColumn>
                                    <%--<dx:GridViewDataHyperLinkColumn FieldName="HyperLinkColumn2" UnboundType="String"
                                        UnboundExpression="'?id='+[ID]+'&name='+[Attached]" VisibleIndex="4">
                                        <PropertiesHyperLinkEdit TextField="Attached" DisplayFormatString="{0}" NavigateUrlFormatString="javascript:downloadfile('{0}');"></PropertiesHyperLinkEdit>
                                    </dx:GridViewDataHyperLinkColumn>
                                    <dx:GridViewDataHyperLinkColumn VisibleIndex="3" Caption="Detalis">
                                             <DataItemTemplate>
                                            <a href="javascript:void(0);" onclick="downloadfile(this, '<%# Container.KeyValue %>')">View...</a>
                                        </DataItemTemplate>
                                        <PropertiesHyperLinkEdit Target="_blank" Text="View">
                                        </PropertiesHyperLinkEdit>
                  
                                    </dx:GridViewDataHyperLinkColumn>
                                    <dx:GridViewDataHyperLinkColumn FieldName="Attached" EditFormSettings-Visible="False">
                                        <PropertiesHyperLinkEdit NavigateUrlFormatString="javascript:downloadfile('{0}');" TextField="Attached">
                                        </PropertiesHyperLinkEdit>
                                    </dx:GridViewDataHyperLinkColumn>
                                    <dx:GridViewDataHyperLinkColumn  FieldName="Attached" EditFormSettings-Visible="False" >
                                        <PropertiesHyperLinkEdit NavigateUrlFormatString="javascript:downloadfile('{0}');" TextField="Attached">
                                        </PropertiesHyperLinkEdit>
                                    </dx:GridViewDataHyperLinkColumn>
                                    <dx:GridViewCommandColumn Caption=" " Width="130px">
                                        <CustomButtons>
                                            <dx:GridViewCommandColumnCustomButton ID="EvaluationEditBtn" Text="Edit"></dx:GridViewCommandColumnCustomButton>
                                            <dx:GridViewCommandColumnCustomButton ID="EvaluationDeleteBtn" Text="Delete"></dx:GridViewCommandColumnCustomButton>
                                        </CustomButtons>
                                    </dx:GridViewCommandColumn>--%>
                                    <dx:GridViewDataHyperLinkColumn FieldName="ID"  ReadOnly="True" VisibleIndex="0" Width="45">
                                        <PropertiesHyperLinkEdit NavigateUrlFormatString="javascript:ShowDetailPopup('{0}');"
                                            Text="Edit">
                                        </PropertiesHyperLinkEdit>
                                    </dx:GridViewDataHyperLinkColumn>
                                </Columns>
                                <SettingsSearchPanel ColumnNames="" Visible="true" CustomEditorID="tbToolbarSearch"/>
		                        <%--<Settings ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="false" HorizontalScrollBarMode="Visible"/>--%>
                                <Settings  ShowFooter="true" GridLines="Both" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto"/>
		                        <SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true" AllowFocusedRow="true" />
		                        <SettingsPager PageSize="5">
                                <PageSizeItemSettings Visible="true" ShowAllItem="true"  AllItemText="All Records" />
                                </SettingsPager>
                                <SettingsEditing Mode="Batch">
                                    <BatchEditSettings ShowConfirmOnLosingChanges="false"/>
                                </SettingsEditing>
                                </dx:ASPxGridView>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem> 
                            </Items>
                            </dx:LayoutGroup >
                            <dx:LayoutGroup Caption="Attached" ColCount="4" GroupBoxDecoration="None" UseDefaultPaddings="false">
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
                                                <%--<dx:UploadedFilesContainer ID="FileContainer" runat="server" Height="80"
                                                        NameColumnWidth="240" SizeColumnWidth="70" HeaderText="Uploaded files" />--%>
                                    <dx:ASPxCallbackPanel ID="cpUpdateFiles" runat="server" Width="100%" ClientInstanceName="cpUpdateFiles"  
                                        OnCallback="cpUpdateFiles_Callback">
                                    <PanelCollection>
                                    <dx:PanelContent runat="server">
                                    <asp:GridView ID="gvFiles" runat="server" AutoGenerateColumns="false" DataKeyNames="ID" Width="100%"
                                        GridLines="None" CssClass="table table-bordered table-striped" OnRowDeleting="OnRowDeleting"
                                        EmptyDataText = "No files uploaded">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkRow" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Result" HeaderText="Topic"/>
                                            <asp:BoundField DataField="Notes" HeaderText="Notes" ItemStyle-Width="60%"/>
                                            <asp:HyperLinkField DataTextField="Name" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="~/popupControls/DownloadFile.aspx?Id={0}"
                                            HeaderText="FileName" ItemStyle-Width = "50%" />
                                            <%--<asp:TemplateField>
                                              <ItemTemplate>
                                                <a href="javascript:void(0);" onclick="downloadfile('<%# Eval("ID") %>')" id="ANotes">
                                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/icons8-attach-16.png"/></a>
                                              </ItemTemplate>
                                            </asp:TemplateField>--%>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <a href="javascript:void(0);" onclick="OnMoreInfoClick(this, '<%# Eval("Result") %>')" id="ANotes">
                                                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/cancel.gif" /></a>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Edit">
                                                <ItemTemplate>
                                                    <a href="javascript:void(0);" onclick="OnMoreInfoClick(this, '<%# Eval("Result") %>')" id="ANotes">
                                                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/Edit.gif" /></a>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    </dx:PanelContent>
                                    </PanelCollection>
                                    </dx:ASPxCallbackPanel>
                                    <%--<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
                                           SelectCommand="select * from transstdFileDetails where request=@Id and subid=0">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hGeID" Name="Id" PropertyName="['GeID']"/>
                                            </SelectParameters>
                                       </asp:SqlDataSource>  --%> 
                                    </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    </Items>
                                </dx:LayoutGroup>
                            <dx:LayoutGroup ShowCaption="False"/>
                            <dx:LayoutGroup />
                            <dx:LayoutGroup />
                            <dx:LayoutGroup />
                            <dx:LayoutGroup />
                            <dx:LayoutGroup />
                            <dx:LayoutGroup />
                            <dx:LayoutGroup />
                            <dx:LayoutGroup />
                            <dx:LayoutGroup />
                            <dx:LayoutGroup ShowCaption="False"/>
                            <dx:LayoutGroup />
                            </Items>
                    </dx:TabbedLayoutGroup>
                    <dx:LayoutItem Caption="" Name="layview" Width="100%" ClientVisible="false">
                        <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer>
                            <dx:ASPxGridView ID="gvitems" runat="server" Width="100%" ClientInstanceName="grid" 
                            OnDataBinding="gvitems_DataBinding" OnCustomCallback="gvitems_CustomCallback" KeyFieldName="RowID"
                            OnBatchUpdate="gvitems_BatchUpdate" OnCustomDataCallback="gvitems_CustomDataCallback"
                            OnCustomButtonCallback="gvitems_CustomButtonCallback">    
                            <ClientSideEvents BatchEditEndEditing="function(s,e){
                                updategrid(s);}" BatchEditStartEditing="OnBatchEditStartEditing" 
					            Init="OnInitGridBatch" FocusedCellChanging="onFocusedCellChanging" EndCallback="grid_OnEndCallback" /> 
                                <%--<Toolbars>
                                            <dx:GridViewToolbar Name="toolbar">
                                                <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                                                <Items>
                                                    <dx:GridViewToolbarItem Text="SelectAll" Name="SelectAll" Image-IconID="actions_apply_16x16"/>
                                                    <dx:GridViewToolbarItem Text="UnselectAll" Name="UnselectAll" Image-IconID="actions_close_16x16"/>
                                                </Items>
                                            </dx:GridViewToolbar>
                                        </Toolbars>--%>
                                <SettingsSearchPanel ColumnNames="" Visible="false" />
	                            <Settings  ShowFooter="true" GridLines="Both" ShowStatusBar="Visible" VerticalScrollBarMode="Auto"/>
                                <SettingsPager Mode="ShowAllRecords" AlwaysShowPager="false" PageSize="20"/>
                                <SettingsEditing Mode="Batch" BatchEditSettings-ShowConfirmOnLosingChanges="false"/>
                                <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
                                <SettingsBehavior AllowSelectSingleRowOnly="True" AllowSelectByRowClick="True" AllowFocusedRow="True" /> 
                                <Templates>
                                    <StatusBar>
                                        <div class="draggingContainer" onmousedown="dragHelper_OnMouseDown(event)">
                                            <div class="vertDragLine">
                                            </div>
                                            <div class="sizeGrip">
                                            </div>
                                        </div>
                                    </StatusBar>
                                </Templates>
                                <Styles>
                                    <Row Cursor="pointer" />
                                    <StatusBar CssClass="myStatusBar">
                                    </StatusBar>
                                </Styles>
                        </dx:ASPxGridView>
                        </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutGroup Caption="" ColCount="3" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                    <GroupBoxStyle>
                        <Caption Font-Bold="true" Font-Size="10"/>
                    </GroupBoxStyle>
                    <Items>
                        <%--<dx:LayoutItem Width="100%">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxSpreadsheet ID="spreadsheet" runat="server" Width="100%" Height="740px" ActiveTabIndex="0" 
                                        ShowConfirmOnLosingChanges="false" RibbonMode="Ribbon" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>--%>
                        <dx:LayoutItem Caption="Send Document" Width="100%" Name="Send_Document">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbDisponsition" runat="server" TextField="Title" ValueField="ID" OnCallback="CmbDisponsition_Callback"
                                    Width="230px" ClientInstanceName="ClientDisponsition"> 
                                    <ClientSideEvents  SelectedIndexChanged="function(s, e) { OnDisponsitionChanged(s); }"/>
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
                        <dx:LayoutItem Caption="Approver" Name="Assignee" Width="100%" ClientVisible="false" VerticalAlign="Middle">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ClientInstanceName="CmbAssignee" ID="CmbAssignee" runat="server" OnCallback="CmbAssignee_Callback"
                                            Width="230px" ValueField="empid" TextField="name">
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
                    </dx:SplitterContentControl>
                </ContentCollection>
            </dx:SplitterPane>
            </Panes>
            </dx:ASPxSplitter>
    </dx:PanelContent>
    </PanelCollection>
</dx:ASPxCallbackPanel>
<dx:ASPxPopupControl ID="EditPopup" ClientInstanceName="EditPopup" runat="server" 
    Modal="true" CloseAction="CloseButton" PopupHorizontalAlign="Center" PopupVerticalAlign="Middle"
    AllowResize="true" ShowFooter="true" AllowDragging="True">
    <ClientSideEvents Shown="onShown" CloseButtonClick="onCloseButtonClick" />
    <ContentCollection>
        <dx:PopupControlContentControl runat="server">
            <dx:ASPxLabel ID="ASPxLabel1" runat="server" Text="Commission" />
            <dx:ASPxComboBox ID="CmbCommission" runat="server" DataSourceID="dsCommission" TextField="value" ValueField="value" NullText="Example : xx %" 
                ClientInstanceName="CmbCommission" />
            <dx:ASPxLabel ID="ASPxLabel2" runat="server" Text="Over Price" />
            <dx:ASPxTextBox ID="textBoxOverPrice" ClientInstanceName="textBoxOverPrice" runat="server">
                <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
            </dx:ASPxTextBox>
            <dx:ASPxLabel ID="ASPxLabel3" runat="server" Text="Over Type" />
            <dx:ASPxComboBox ID="CmbOverType" runat="server" DataSourceID="dsOverType" ValueField="value" TextField="value" ClientInstanceName="CmbOverType" />
            <dx:ASPxLabel ID="ASPxLabel4" runat="server" Text="Case per fcl." />
            <dx:ASPxTextBox ID="TextBoxSubContainers" ClientInstanceName="textBoxSubContainers" runat="server">
                <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
            </dx:ASPxTextBox>
            <%--<dx:ASPxLabel ID="ASPxLabel5" runat="server" Text="Address" />
            <dx:ASPxTextBox ID="TextBoxAddress" ClientInstanceName="textBoxAddress" runat="server" />--%>
            <br />
            <dx:ASPxButton ID="btnAccept" runat="server" Text="Accept" AutoPostBack="false" ClientInstanceName="btnAccept">
                <ClientSideEvents Click="onAcceptClick" />
            </dx:ASPxButton>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl ID="UpchargePopup" ClientInstanceName="UpchargePopup" runat="server" 
    Width="700px" Height="240px" AllowResize="true" ShowFooter="true"
    Modal="true" CloseAction="CloseButton" PopupHorizontalAlign="Center" PopupVerticalAlign="Middle"
    AllowDragging="True">
    <ClientSideEvents Shown="clearDataUpcharge" />
    <ContentCollection>
        <dx:PopupControlContentControl runat="server">
            <dx:ASPxPageControl ID="citiesTabPage" runat="server" CssClass="dxtcFixed" ActiveTabIndex="0" EnableHierarchyRecreation="True"
                Border-BorderStyle="None" Width="100%">
                <TabPages>
                    <dx:TabPage Text="List">
                        <ContentCollection>
                            <dx:ContentControl ID="ContentControl1" runat="server">
                               <dx:ASPxListBox ID="lbUpcharge" runat="server" SelectionMode="Single" Width="485" Height="210px"
                                        ClientInstanceName="lbUpcharge" DataSourceID="dsUpcharge" ValueField="Id" OnCallback="lbUpcharge_Callback" 
                                        ValueType="System.String">
                                        <ClientSideEvents SelectedIndexChanged="lbUpchargeSelectedIndex" />
                                        <CaptionSettings Position="Top" />
                                        <FilteringSettings ShowSearchUI="true"/>
                                        <Columns>
                                            <dx:ListBoxColumn FieldName="UpchargeGroup" Width="100px" />
                                            <dx:ListBoxColumn FieldName="Upcharge" Width="250px" />
                                            <dx:ListBoxColumn FieldName="Value" />
                                            <dx:ListBoxColumn FieldName="Currency" />
                                            <dx:ListBoxColumn FieldName="Unit" />
                                            <dx:ListBoxColumn FieldName="StdPackSize" />
                                        </Columns>
                                    </dx:ASPxListBox>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                    <dx:TabPage Text="Other">
                        <ContentCollection>
                            <dx:ContentControl ID="ContentControl2" runat="server">
                                <dx:ASPxLabel ID="ASPxLabel5" runat="server" Text="UpCharge" />
                                <dx:ASPxTextBox ID="TextBoxUpCharge" ClientInstanceName="TextBoxUpCharge" Width="100%" runat="server"/>
                                <dx:ASPxLabel ID="ASPxLabel6" runat="server" Text="Price" />
                                <dx:ASPxTextBox runat="server" ID="tbUpChargePrice" ClientInstanceName="tbUpChargePrice" 
                                    ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}"/>
                                <dx:ASPxLabel ID="ASPxLabel7" runat="server" Text="Unit" />
                                <dx:ASPxTextBox runat="server" ID="TextBoxUnit" ClientInstanceName="TextBoxUnit" 
                                    ClientSideEvents-KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}"/>
                                <br />
                                <dx:ASPxButton ID="btSubmit" runat="server" Text="Submit" AutoPostBack="false">
                                            <ClientSideEvents Click="ClickSubmit" />
                                        </dx:ASPxButton>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                </TabPages>
            </dx:ASPxPageControl>
        </dx:PopupControlContentControl>
    </ContentCollection>
    </dx:ASPxPopupControl>
<asp:SqlDataSource ID="dsReason" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasReason"/>
<%--<asp:SqlDataSource ID="dssapMaterial" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from massapMaterial"/>--%>
<asp:SqlDataSource ID="dsPaymentTerm" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spGetPaymentTerm" SelectCommandType="StoredProcedure"/>
<asp:SqlDataSource ID="dsCustomer" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spGetCustomer" SelectCommandType="StoredProcedure">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsRoute" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select Code,Name from MasRoute union select '',''">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsContainerType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select Code,Name from MasContainerType  union select '',''">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsCommission" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select value from dbo.FNC_SPLIT('0,1,2',',')">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsinterest" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select value from dbo.FNC_SPLIT('6,4',',')">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spGetTunaStd" SelectCommandType="StoredProcedure">
    <SelectParameters>
        <asp:ControlParameter ControlID="hftype" Name="type" PropertyName="['type']"/>
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
    SelectCommand="select Code,Name from MasIncoterm  union select '',''"/>
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

<%--<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select ID, isnull(Code,Custom)Code, Name, Custom from MasCustomer union select '0','','',''">
</asp:SqlDataSource>--%>
<asp:SqlDataSource ID="dsStatusApp" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasStatusApp where levelapp in (4)"/>
<asp:SqlDataSource ID="dsOverType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT(';%;USD',';') order by value"/>
<asp:SqlDataSource ID="dsUpcharge" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from StandardUpcharge"></asp:SqlDataSource>
<%--<asp:SqlDataSource ID="dsTopic" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from stdtunaTopic where levels=1">
</asp:SqlDataSource>--%>
<asp:SqlDataSource ID="dszone" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select distinct Zone as 'value' from MasCustomer order by Zone">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsSelectPayment" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="spSelectPayment" SelectCommandType="StoredProcedure">
</asp:SqlDataSource>