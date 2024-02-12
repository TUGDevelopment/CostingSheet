<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CostingEditForm2.ascx.cs" Inherits="UserControls_CostingEditForm" %>
   <script type="text/javascript">
       var pagesymboll = null;
       var lastCountry = null;
       var curentEditingIndex;
       var FocusedCellColumnIndex = 0;
       var lastCombo = null;
       var isCustomCascadingCallback = false;
       var focusedfieldName = false;
       var timerHandle = -1;
       var monthNames = [
           "January", "February", "March", "April", "May", "June", "July",
           "August", "September", "October", "November", "December"
       ];
       var dayOfWeekNames = [
           "Sunday", "Monday", "Tuesday",
           "Wednesday", "Thursday", "Friday", "Saturday"
       ];
       function formatDate(date, patternStr) {
           if (!patternStr) {
               patternStr = 'M/d/yyyy';
           }
           var day = date.getDate(),
               month = date.getMonth(),
               year = date.getFullYear(),
               hour = date.getHours(),
               minute = date.getMinutes(),
               second = date.getSeconds(),
               miliseconds = date.getMilliseconds(),
               h = hour % 12,
               hh = twoDigitPad(h),
               HH = twoDigitPad(hour),
               mm = twoDigitPad(minute),
               ss = twoDigitPad(second),
               aaa = hour < 12 ? 'AM' : 'PM',
               EEEE = dayOfWeekNames[date.getDay()],
               EEE = EEEE.substr(0, 3),
               dd = twoDigitPad(day),
               M = month + 1,
               MM = twoDigitPad(M),
               MMMM = monthNames[month],
               MMM = MMMM.substr(0, 3),
               yyyy = year + "",
               yy = yyyy.substr(2, 2)
               ;
           // checks to see if month name will be used
           patternStr = patternStr
               .replace('hh', hh).replace('h', h)
               .replace('HH', HH).replace('H', hour)
               .replace('mm', mm).replace('m', minute)
               .replace('ss', ss).replace('s', second)
               .replace('S', miliseconds)
               .replace('dd', dd).replace('d', day)

               .replace('EEEE', EEEE).replace('EEE', EEE)
               .replace('yyyy', yyyy)
               .replace('yy', yy)
               .replace('aaa', aaa);
           if (patternStr.indexOf('MMM') > -1) {
               patternStr = patternStr
                   .replace('MMMM', MMMM)
                   .replace('MMM', MMM);
           }
           else {
               patternStr = patternStr
                   .replace('MM', MM)
                   .replace('M', M);
           }
           return patternStr;
       }
       function twoDigitPad(num) {
           return num < 10 ? "0" + num : num;
       }
       function GetChangesCount(batchApi) {
           var updatedCount = batchApi.GetUpdatedRowIndices().length;
           var deletedCount = batchApi.GetDeletedRowIndices().length;
           var insertedCount = batchApi.GetInsertedRowIndices().length;

           return updatedCount + deletedCount + insertedCount;
       }
       function Combo_SelectedIndexChanged(s, e) {
           lastCombo = s.GetValue();
           isCustomCascadingCallback = true;
           RefreshData(lastCombo);
           debugger;
           var textname = s.GetSelectedItem().GetColumnText(1);//s.GetText().toString();
           grid.batchEditApi.SetCellValue(curentEditingIndex, "Description", textname);
       }
       function OnSelectedIndexChanged(s, e) {
           var name = s.GetSelectedItem().GetColumnText(1);//s.GetText().toString();
           var yield_value = s.GetSelectedItem().GetColumnText(2);
           //grid.GetEditor("Yield").SetText(s.GetSelectedItem().GetColumnText(2));
           //grid.SetEditValue('Name', name);
           //grid.SetEditValue("Name", name);
           //alert(name);
           //var newValueOfComboBox = name + '|' + s.GetSelectedItem().GetColumnText(2);//s.GetValue();
           //grid.GetValuesOnCustomCallback("RawMaterial|"+newValueOfComboBox, DataCallback);
       }
       function OnChanged(s, e) {
           if (s.GetItemCount() > 0) {
               var LBRate = s.GetSelectedItem().GetColumnText(4);//s.GetText().toString();
               grid.batchEditApi.SetCellValue(curentEditingIndex, "LBRate", LBRate);
               //grid.GetValuesOnCustomCallback("LBRate|" + LBRate, DataCallback);
               //alert(lastCountry);
           }
       }
       function DataCallback(result) {
           //alert(result);
           var results = result.split("|");
           switch (results[0]) {
               case "LBRate":
                   grid.batchEditApi.SetCellValue(curentEditingIndex, "LBRate", results[1]);
                   break;
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
           //if (isCustomCascadingCallback) {
           //    if (s.GetItemCount() > 0)
           //        grid.batchEditApi.SetCellValue(curentEditingIndex, "RawMaterial", s.GetItem(0).value);
           //    isCustomCascadingCallback = false;
           //}
           if (isCustomCascadingCallback) {
               isCustomCascadingCallback = false;
               s.SetSelectedIndex(0);
           }
       }
       function RefreshData(countryValue) {
           //hf.Set("CurrentCountry", countryValue);
           //RawMaterial.PerformCallback();
           //grid.GetEditor("RawMaterial").PerformCallback(countryValue);
       }
       function OnCountryChanged(ClientCompany) {
           debugger;
           //var strcom = ClientCompany.GetText().toString().substring(0,3);
           var strcom = ClientCompany.GetText().toString();
           if (ClientCostingNo.InCallback())
               lastCountry = strcom.toString();
           else {
               var param = "Build|" + strcom.toString();
               //ClientCostingNo.PerformCallback(param);
               //CmbPackageCode.PerformCallback(param);
               //CmbSecPackageCode.PerformCallback(param);
               //CmbMargin.PerformCallback(param);
               //CmbLaborOverhead.PerformCallback(param);
               //CmbLoss.PerformCallback("Build|" + strcom.toString());
           }
       }
       function OnEndCallback(s, e) {
           if (lastCountry) {
               var value = "Build|" + lastCountry;
               //ClientCostingNo.PerformCallback(value);
               //CmbPackageCode.PerformCallback(value);
               //CmbSecPackageCode.PerformCallback(value);
               //CmbMargin.PerformCallback(value);
               //CmbLaborOverhead.PerformCallback(value);
               lastCountry = null;
           }
           //if (s.cpFlag == true) {
           //    s.SetText(null);
           //    debugger;
           //    delete (s.cpFlag);
           //}
       }
       function OnTextChanged(s, e) {
           debugger;
           hfid.Set('hidden_value', s.GetValue());
           ClientValidfrom.SetText(s.GetSelectedItem().GetColumnText(2));
           ClientValidto.SetText(s.GetSelectedItem().GetColumnText(3));
           CmbPackaging.SetText(s.GetSelectedItem().GetColumnText(5));
           tbCustomer.SetText(s.GetSelectedItem().GetColumnText(8));
           usertp.Set("usertype", (s.GetSelectedItem().GetColumnText(9)));
           if (usertp.Get("usertype") == '1') {
               seExchangeRate.SetNumber('1');
               tbCanSize.SetText('0');
               ClientReference.SetText('0');
               ClientNetweight.SetText('0');
               ClientNetUnit.GetValue(1);
           }
           var compa = ClientCompany.GetValue();
           CmbPackageCode.PerformCallback(s.GetValue() + '|' + compa);
           CmbSecPackageCode.PerformCallback(s.GetValue() + '|' + compa);
           var value = s.GetSelectedItem().GetColumnText(6);//s.GetText().toString().substring(3, 4);
           //alert(value);
           if (value == '2')
               confirm_value(s.GetValue());
           //if (confirm("Are you sure?")) {
           //    confirm_value(s.GetValue());
           //}
           Upload.SetEnabled(true);
           var array = s.GetSelectedItem().GetColumnText(4).split(",");
           if (array.length > 1) {
               //alert(array);
               treeList.SetVisible(true);
               treeList.PerformCallback("reload|" + s.GetValue() + "|" + array);
               hflit.Set('hflit', s.GetValue());
               popup.Show();
               popup.BringToFront();
           } else
               ClientPackSize.SetText(array[0]);

           //ClientPackSize.PerformCallback("AddRow|" + s.GetValue());
           //CmbPackaging.PerformCallback("AddRow|" + s.GetValue());
           //var value = s.GetValue() + "|" + s.GetSelectedItem().GetColumnText(0) + "|" + s.GetSelectedItem().GetColumnText(1);
           //Grid.GetValuesOnCustomCallback(s.GetText().toString());
           var Net = s.GetSelectedItem().GetColumnText(7).split("|");
           ClientNetweight.SetText(Net[0]);
           ClientNetUnit.SetText(Net[1]);
           var keys = lastCountry == '' ? s.GetValue() : lastCountry;
           //CmbMargin.PerformCallback("reload|" + keys);
           //CmbMargin.PerformCallback("reload|" + lastCountry + "|" + s.GetValue());
       }
       function confirm_value(keyValue) {
           // JS Code to handle post confirm operation
           GridView1.PerformCallback('copy|' + keyValue);
           OnDetailsClick(keyValue)
       }
       function OnDetailsClick(keyValue) {
           //alert(keyValue);
           debugger;
           hflit.Set('hflit', keyValue);
           popup.Show();
           popup.BringToFront();
           //popup.PerformCallback(keyValue);
           //TreeList.PerformCallback(keyValue);
       }
       function clearData() {
           //tbProductName.SetText("");
           //ClientRDNumber.SetText("");
           //tbMaterial.SetText("");
       }
       function OnFileUploadComplete(s, e) {
           clearData();
           var value = ["'upload", "1"].join('|');
           grid.PerformCallback(value);
           testgrid.PerformCallback(value);
           gvCode.PerformCallback(value);
           if (e.callbackData) {
               //               debugger;
               var fileData = e.callbackData.split('|');
               tbCanSize.SetText(fileData[0]);
               ClientReference.SetText(fileData[1]);
               seExchangeRate.SetNumber(fileData[2]);
               //tbProductName.SetText(fileData[3]);
               //ClientRDNumber.SetText(fileData[4]);
               //tbMaterial.SetText(fileData[5]);
           }
       }
       function OnTextChangedHandler(s, evt) {
           //alert('TextChanged. Text = ' + s.GetText());
           if (evt == 'prima') {
               tbAmount.SetText(tbPriceRate.GetText() * s.GetText());
           }
           if (evt == 'secp') {
               tbSecAmount.SetText(tbSecPriceRate.GetText() * s.GetText());
           }
       }
       function ProcessKeyPress(s, evt) {
           var charCode = (evt.htmlEvent.which) ? evt.htmlEvent.which : event.keyCode
           if (charCode > 31 && (charCode < 48 || charCode > 57))
               _aspxPreventEvent(evt.htmlEvent);

       }
       function OnValueChanged(s, evt) {
           if (evt == 'margin') {
               tbMarginName.SetText(s.GetSelectedItem().GetColumnText(1));
               tbMarginRate.SetText(s.GetSelectedItem().GetColumnText(2));
           }
           if (evt == 'prima') {
               tbDescription.SetText(s.GetSelectedItem().GetColumnText(1));
               tbPriceRate.SetText(s.GetSelectedItem().GetColumnText(2));
               CmbSellingUnit.SetText(s.GetSelectedItem().GetColumnText(3));
               //tbAmount.GetInputElement().style.backgroundColor = "Silver";
               tbQuantity.Focus();
           }
           if (evt == 'secp') {
               tbSecDescription.SetText(s.GetSelectedItem().GetColumnText(1));
               tbSecPriceRate.SetText(s.GetSelectedItem().GetColumnText(2));
               CmbSecSellingUnit.SetText(s.GetSelectedItem().GetColumnText(3));
               tbSecQuantity.Focus();
           }
           if (evt == 'labor') {
               tbLaborOverhead.SetText(s.GetSelectedItem().GetColumnText(1));
               tbLbOhPackSize.SetText(s.GetSelectedItem().GetColumnText(2));
               tbLbOhType.SetText(s.GetSelectedItem().GetColumnText(3));
               tbLbOhRate.SetText(s.GetSelectedItem().GetColumnText(4));
               CmbLbOhCurrency.SetText(s.GetSelectedItem().GetColumnText(5));
           }
           if (evy = "pkg") {
               grid.PerformCallback('Labor|' + s.GetText());
               //Upload.SetEnabled(true);
               //var collection = s.GetTokenCollection();
               //var last = collection[collection.length - 1];
               //var tokens = s.GetTokenCollection();
               //for (var i = 0; i < tokens.length; i++) {
               //    s.RemoveItem(i);                   
               //    s.SetText(tokens[i]);
               //}
           }
       }
       function gvCode_OnBatchEditEndEditing(s, e) {
           window.setTimeout(function () {
               var count_value = GetChangesCount(s.batchEditApi);
               if (count_value > 0)
                   s.UpdateEdit();
           }, 0);
       }
       function onFocusedCellChanging(s, e) {

           //var sapmat =s.batchEditApi.GetCellValue(s.GetFocusedRowIndex(), 'SAPMaterial');
           //if (e.cellInfo.column.fieldName == "PriceOfUnit" && s.batchEditApi.GetCellValue(s.GetFocusedRowIndex(), 'SAPMaterial') != null)
           //   e.cancel = true;
           if (e.cellInfo.column.fieldName == "ExchangeRate")
               e.cancel = true;
       }
       function OnBatchEditStartEditing(s, e) {
           FocusedCellColumnIndex = e.focusedColumn.index;
           curentEditingIndex = e.visibleIndex;
           debugger;
           var currentCountry = grid.batchEditApi.GetCellValue(curentEditingIndex, "SAPMaterial");
           if (e.focusedColumn.fieldName == "PriceOfUnit" && (currentCountry != null && currentCountry != "")) {
               e.cancel = true;
           }
           var targetArray = ["GUnit", "BaseUnit", "Yield", "Currency", "PriceOfUnit", "Unit","LBRate","Component", "SubType"];

           if (targetArray.indexOf(e.focusedColumn.fieldName) > -1)
               focusedfieldName = true;
           if (currentCountry != lastCombo && e.focusedColumn.fieldName == "RawMaterial" && currentCountry != null) {
               lastCombo = currentCountry;
               RefreshData(currentCountry);
           }
       }
       function testgrid_OnBatchEditEndEditing(s, e) {
           debugger;
           var index = curentEditingIndex;
           var param = 0;
           s.GetValuesOnCustomCallback("GetBatch|" + index + "|" + param, function (r) {
               if (!r)
                   return;
               s.batchEditApi.SetCellValue(curentEditingIndex, "Loss", r["Loss"]);
               s.batchEditApi.SetCellValue(curentEditingIndex, "Amount", r["Amount"]);
           })
           window.setTimeout(function () {
               var count_value = GetChangesCount(s.batchEditApi);
               if (count_value > 0)
                   s.UpdateEdit();
           }, 0);
       }
       function RecieveGridValues(r) {
           var g = testgrid;
           g.batchEditApi.SetCellValue(curentEditingIndex, "BatchSap", r["BatchSap"]);
       }
       function OnBatchEditEndEditing(s, e) {
           if (focusedfieldName)
               calculator(s);
           focusedfieldName = false;
       }
       //function OnDeleteClick(s, e) {
       //    debugger;
       //    if (e.buttonID == 'remove') {
       //        alert('Row index: ' + e.visibleIndex);
       //        var index = s.GetFocusedRowIndex();
       //        s.PerformCallback('Delete|' + index);
       //    }
       //}
       //function OnItemClick(s, e) {
       //        var command = e.item.name;
       //        switch (command) {
       //            case "new":
       //                grid.AddNewRow();
       //                break;
       //            case "delete":
       //                var index = grid.GetFocusedRowIndex();
       //                grid.DeleteRow(index);
       //                break;
       //        }
       //}
       function insertSelection(command) {
           debugger;
           if (pagesymboll == null || pagesymboll == 1)
               if (confirm("copy all items?")) {
                   return OnConfirm();
               }
           testgrid.PerformCallback("AddRow|" + tcDemos.GetActiveTabIndex());
       }
       //function ConfirmAndExecute(s, e) {
       //    var command = e.item.name;
       //    switch (command) {
       //        case "new":
       //            if (pagesymboll == null || pagesymboll == 1)
       //                if (confirm("copy going to other formula.")) {
       //                    OnConfirm();
       //                }
       //                else
       //                    //alert("you missed !")
       //                    testgrid.PerformCallback('AddRow');
       //            else
       //                testgrid.PerformCallback('AddRow');
       //            break;
       //        case "clear":
       //            OnDeleteClick();
       //            break;
       //    }
       //}
       function OnConfirm() {
           // JS Code to handle post confirm operation
           testgrid.PerformCallback('copy|' + tcDemos.GetActiveTabIndex());
       }
       function MoveToPage(symbol) {
           //alert(symbol);
           debugger;
           // var count_value  = GetChangesCount(grid.batchEditApi);
           // if (count_value > 0)
           //        grid.UpdateEdit();
           //if (GetChangesCount(testgrid.batchEditApi) > 0)
           //   testgrid.UpdateEdit();
           pagesymboll = symbol;
           LoadName(symbol);
           //debugger;
           //grid.PerformCallback('symbol|' + symbol);
           //testgrid.PerformCallback('symbol|' + symbol);

           //Demo.update_grid();
           //GridView1.PerformCallback('symbol|' + symbol);
       }
       function LoadName(symbol) {
           clearData();
           var value = "symbol|" + symbol;
           grid.PerformCallback(value);
           testgrid.PerformCallback(value);
       }
       function ChangeBatchEditorValue(s, e) {
           //alert("value changed");
       }
       function OnIDChanged(s) {
           debugger;
           var identi = s.GetValue().toString() == "3";
           formLayout.GetItemByName("Reason").SetVisible(identi);
       }
       function UpdateSelection(s, e) {
           //    var key = s.GetRowKey(e.visibleIndex);
           //    debugger;
           //    var focusedNodeKey = TreeList.GetFocusedNodeKey();
           //    if (focusedNodeKey != "")
           //        employeeName = TreeList.cpEmployeeNames[focusedNodeKey];
           s.GetRowValues(e.visibleIndex, 'ID;MarketingNumber;RequestNo', OnGetRowValues);
       }
       function OnGetRowValues(values) {
           debugger;
           var key = values[2];
           hflit.Set("hflit", values[0]);
           //alert(key);
           popup.Hide();
           ClientCostGrid.PerformCallback('Test|' + values[1] + '|' + key);
       }
       function OnContextMenuItemClick(sender, args) {
           debugger;
           if (args.objectType == "row") {
               if (args.item.name == "ExportToPDF" || args.item.name == "ExportToXLS") {
                   args.processOnServer = true;
                   args.usePostBack = true;
               }
               else if (args.item.name == "Assign" || args.item.name == "Attach") {
                   var ID = sender.GetRowKey(args.elementIndex);
                   var a = ID.split("|")
                   pcassign.SetFooterText('Id:' + a[0]);
                   pcassign.SetContentUrl('popupControls/wfassign.aspx?table=TransCostingHeader&view=' + args.item.name + '&id=' + a[0]);
                   pcassign.Show();
                   pcassign.BringToFront();
 
               }
               else {
                   args.processOnServer = false;
                   var key = sender.GetRowKey(args.elementIndex);
                   var a = key.split("|")
                   if (args.item.name == "Copied" || args.item.name == "Revised")
                       ClientCostGrid.PerformCallback([args.item.name, a[0]].join("|"));
               }
           }
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
       function treeList_DblClick(s, e) {
           var employeeName = "";
           var focusedNodeKey = s.GetFocusedNodeKey();
           if (focusedNodeKey != "")
               employeeName = s.cpEmployeeNames[focusedNodeKey];
           UpdateControls(focusedNodeKey, employeeName);
       }
       function UpdateControls(key, text) {
           if (text != "" && text != undefined) {
               ClientPackSize.SetText(text);
               popup.Hide();
           }
       }
       function OnBatchEditRowInserting(s, e) {
           //alert("Insert");
       }
       function OnActiveTabChanged(s, e) {
           //debugger;
           //var selectedIndex = s.GetActiveTab().name;
           var selectedIndex = s.GetActiveTabIndex();
           //if(selectedIndex.length==1)
           var b = selectedIndex != 0 && selectedIndex != 6 && selectedIndex != 7;
           //tabbedGroupPageControl.SetActiveTabIndex(selectedIndex == 5 ? 4 : selectedIndex);
           tabbedGroupPageControl.SetActiveTabIndex(selectedIndex);
           grid.SetVisible(selectedIndex == 0);
           testgrid.SetVisible(b);
           testgrid.PerformCallback('TabChanged|' + selectedIndex);
           if (selectedIndex == 7)
               fileManager.PerformCallback(['load', 0].join("|"));
           //formLayout.GetItemByName("testMenu").SetVisible(b);
       }
       function OnTextChangedData(text, val) {
           var symboll = 1;
           if (pagesymboll != null)
               symboll = pagesymboll;
           //cb.PerformCallback(text+ symboll +'|'+val);
       }
       function OnExchangeRate(s, e) {
           grid.PerformCallback('changed|' + s.GetText());
       }
       function OnFileUploaded(s, e) {
           console.log("e.fileName=" + e.fileName);
           console.log("e.folder=" + e.folder);
       }
       function rate_SelectedIndexChaged(s, e) {

           if (s.GetValue() == 'USD') {
               grid.batchEditApi.SetCellValue(curentEditingIndex, 'ExchangeRate', seExchangeRate.GetText());
           } else
               grid.batchEditApi.SetCellValue(curentEditingIndex, 'ExchangeRate', 1);
       }
       function getNum(str) {
           return /[-+]?[0-9]*\.?[0-9]+/.test(str) ? parseFloat(str) : 0;
       }
       function calculator(s) {
           var PriceOfCarton = 0; var BaseUnit = 0;
           var ExchangeRate = s.batchEditApi.GetCellValue(curentEditingIndex, "ExchangeRate");
           var PriceOfUnit = s.batchEditApi.GetCellValue(curentEditingIndex, "PriceOfUnit");
           var Unit = s.batchEditApi.GetCellValue(curentEditingIndex, "Unit");
           var GUnit = s.batchEditApi.GetCellValue(curentEditingIndex, "GUnit");
           var Yield = s.batchEditApi.GetCellValue(curentEditingIndex, "Yield");
           if (getNum(GUnit) == 0 || getNum(Yield) == 0 || getNum(PriceOfUnit) == 0) return;
           if (ExchangeRate != 0 && ExchangeRate != 0) {
               BaseUnit = parseFloat(ExchangeRate) * parseFloat(PriceOfUnit);
               if (Unit == "Ton" || Unit == "MT")
                   BaseUnit = parseFloat(BaseUnit) / 1000;
           }

           s.batchEditApi.SetCellValue(curentEditingIndex, "BaseUnit", BaseUnit);

           var total = parseFloat(GUnit) / 1000;
           if (total != null || Yield != null || BaseUnit != null) {
               PriceOfCarton = (parseFloat(total) * parseFloat(BaseUnit) * parseFloat(ClientPackSize.GetText()) / (parseFloat(Yield) / 100));
           }
           s.batchEditApi.SetCellValue(curentEditingIndex, "PriceOfCarton", PriceOfCarton);
           window.setTimeout(function () {
               var count_value = GetChangesCount(s.batchEditApi);
               if (count_value > 0)
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
       function getId(s, e) {

           var focusedColumn = e.focusedColumn.fieldName;
           if (focusedColumn != "Per") {
               e.cancel = true;
           }
       }
       function OnButtonClick(evt) {
           debugger;
           var url;
	   var deto = ClientValidto.GetValue();
           var defrom = ClientValidfrom.GetValue();
           url = 'popupControls/selmaster.aspx?view=' + evt;
           if (evt == "SAPMaterial"){
               url += "&ID=" + hfRequestNo.Get("ID") + "&Company=" + ClientCompany.GetValue();
	       url += "&from=" + formatDate(defrom, 'yyyyMMdd') + "&to=" + formatDate(deto, 'yyyyMMdd');
	      }
           else if (evt == "RawMaterial") {
               var SAPMaterial = grid.batchEditApi.GetCellValue(curentEditingIndex, "SAPMaterial");
               url += "&RawMaterial=" + SAPMaterial + "&Company=" + ClientCompany.GetValue();
           }
           else if (evt == "LBOh") {
               url += "&Company=" + ClientCompany.GetValue() + "&NetWeight=" + ClientNetweight.GetText() + "&Packaging=" + CmbPackaging.GetText();
               url += "&UserType=" + usertp.Get("usertype") + "&PackSize=" + ClientPackSize.GetText();
           }
           else if (evt == "RequestNo")
               url += "&Company=" + ClientCompany.GetValue() + "&User=" + username.Get("user_name");
           else if (evt == "Margin") {
               url += "&Company=" + ClientCompany.GetValue() + "&NetWeight=" + ClientNetweight.GetText() + "&Packaging=" + CmbPackaging.GetText();
               url += "&UserType=" + usertp.Get("usertype") + "&ID=" + hfRequestNo.Get("ID");
           }
           else if (evt == "PackageCode" || evt == "SecPackageCode") {
               var type = evt == "PackageCode" ? "1" : "0"
               url += "&Company=" + ClientCompany.GetValue() + "&Id=" + hfid.Get("hidden_value") + "&Type=" + type;
	       url += "&from=" + formatDate(defrom, 'yyyyMMdd') + "&to=" + formatDate(deto, 'yyyyMMdd');
           }
           else
               url += "&usertype=" + hfRequestNo.Get("ID") + "&Primary=" + ClientPrimary.GetText();
           pcassign.RefreshContentUrl();
           pcassign.SetContentUrl(url);
           pcassign.SetHeaderText(url);
           //pcassign.SetFooterText(url);
           pcassign.Show();
           //popupControl.SetPopupElementID();
       }
       function HidePopupAndShowInfo(closedBy, returnValue) {
           debugger;
           pcassign.Hide();
           if (closedBy == "SAPMaterial") {
               grid.batchEditApi.SetCellValue(curentEditingIndex, "SAPMaterial", returnValue);
               grid.GetValuesOnCustomCallback("SAPMaterial|" + returnValue + "|" + curentEditingIndex, function (r) {
                   if (!r)
                       return;
                   var g = grid.batchEditApi;
                   g.SetCellValue(curentEditingIndex, "PriceOfUnit", r["PriceOfUnit"]);
                   g.SetCellValue(curentEditingIndex, "Currency", r["Currency"]);
                   g.SetCellValue(curentEditingIndex, "Unit", r["Unit"]);
                   g.SetCellValue(curentEditingIndex, "ExchangeRate", r["ExchangeRate"]);
                   g.SetCellValue(curentEditingIndex, "BaseUnit", r["BaseUnit"]);
                   g.SetCellValue(curentEditingIndex, "Description", r["Description"]);
		   calculator(grid);
               })
           }
           else if (closedBy == "LBOh") {
               grid.GetValuesOnCustomCallback("LBOh|" + returnValue + "|" + curentEditingIndex, function (r) {
                   if (!r)
                       return;
                   grid.batchEditApi.SetCellValue(curentEditingIndex, "LBOh", r["LBCode"]);
                   grid.batchEditApi.SetCellValue(curentEditingIndex, "LBRate", r["Rate"]);
		   calculator(grid);
               })
           }else if (closedBy == "RawMaterial") {
               grid.batchEditApi.SetCellValue(curentEditingIndex, "RawMaterial", returnValue);
               var SAPMaterial = grid.batchEditApi.GetCellValue(curentEditingIndex, "SAPMaterial");
               if (!SAPMaterial) {
                   grid.GetValuesOnCustomCallback("RawMaterial|" + returnValue + "|" + SAPMaterial, function (r) {
                       if (!r)
                           return;
                       grid.batchEditApi.SetCellValue(curentEditingIndex, "Name", r["Name"]);
                       grid.batchEditApi.SetCellValue(curentEditingIndex, "Yield", r["Yield"]);
		       calculator(grid);
                   })
               }
           }else
           ClientCostGrid.GetValuesOnCustomCallback(["Get",closedBy, returnValue].join('|'), OnGetValuesOnCustomCallbackComplete);
       }
       function OnGetValuesOnCustomCallbackComplete(values) {
           if (!values)
               return;
           if (values["view"] == "RequestNo") {
               RequestOnTextChanged(values);
           }
           if (values["view"] == "Margin") {
               tbMarginRate.SetText(values["Rate"]);
               tbMarginName.SetText(values["Name"]);
               ClientMargin.SetText(values["ID"]);
           }
           if (values["view"] == "PackageCode") {
               tbDescription.SetValue(values["Name"]);
               ClientPackageCode.SetText(values["ID"]);
               tbPriceRate.SetText(values["Rate"]);
               CmbSellingUnit.SetText(values["Unit"]);
               tbQuantity.Focus();
           }
           if (values["view"] == "SecPackageCode") {
               tbSecDescription.SetValue(values["Name"]);
               ClientSecPackageCode.SetText(values["ID"]);
               tbSecPriceRate.SetValue(values["Rate"]);
               CmbSecSellingUnit.SetValue(values["Unit"]);
               tbSecQuantity.Focus();
           }
       }
       function OnCellCheckedChanged(s, e) {
           gvCode.batchEditApi.EndEdit();
       }
       function OnInitHeader(s, e) {
           setTimeout(function () { CheckSelectedCellsOnPage("usualCheck"); }, 0);
       }
       function OnHeaderCheckBoxCheckedChanged(s, e) {
           var visibleIndices = gvCode.batchEditApi.GetRowVisibleIndices();
           var totalRowsCountOnPage = visibleIndices.length;
           for (var i = 0; i < totalRowsCountOnPage ; i++) {
               gvCode.batchEditApi.SetCellValue(visibleIndices[i], "IsActive", s.GetChecked())
           }
       }
       function CheckSelectedCellsOnPage(checkType) {
           var currentlySelectedRowsCount = 0;
           var visibleIndices = gvCode.batchEditApi.GetRowVisibleIndices();
           var totalRowsCountOnPage = visibleIndices.length;
           for (var i = 0; i < totalRowsCountOnPage ; i++) {
               if (gvCode.batchEditApi.GetCellValue(visibleIndices[i], "IsActive"))
                   currentlySelectedRowsCount++;
           }
           if (checkType == "insertCheck")
               totalRowsCountOnPage++;
           else if (checkType == "deleteCheck") {
               totalRowsCountOnPage--;
               if (DeletedValue)
                   currentlySelectedRowsCount--;
           }
           if (currentlySelectedRowsCount <= 0)
               HeaderCheckBox.SetCheckState("Unchecked");
           else if (currentlySelectedRowsCount >= totalRowsCountOnPage)
               HeaderCheckBox.SetCheckState("Checked");
           else
               HeaderCheckBox.SetCheckState("Indeterminate");
       }
       function RequestOnTextChanged(r) {
           ClientCostingNo.SetValue(r["RequestNo"]);
           hfRequestNo.Set("ID", r["ID"]);
           hfid.Set('hidden_value', r["ID"]);
           ClientValidfrom.SetText(r["form"]);
           ClientValidto.SetText(r["to"]);
           CmbPackaging.SetText(r["Packaging"]);
           tbCustomer.SetText(r["Customer"]);
           usertp.Set("usertype", r["UserType"]);
           if (usertp.Get("usertype") == '1') {
               seExchangeRate.SetNumber('1');
               tbCanSize.SetText('0');
               ClientReference.SetText('0');
               ClientNetweight.SetText('0');
               ClientNetUnit.GetValue(1);
           }
           var compa = ClientCompany.GetValue();
           //CmbPackageCode.PerformCallback(r["ID"] + '|' + compa);
           //CmbSecPackageCode.PerformCallback(r["ID"] + '|' + compa);
           //var value = s.GetSelectedItem().GetColumnText(6);//s.GetText().toString().substring(3, 4);
           //alert(value);
           if (r["RequestType"] == '2')
               confirm_value(r["ID"]);
           //if (confirm("Are you sure?")) {
           //    confirm_value(s.GetValue());
           //}
           Upload.SetEnabled(true);
           var array = r["PackSize"].split(",");
           if (array.length > 1) {
               //alert(array);
               treeList.SetVisible(true);
               treeList.PerformCallback("reload|" + r["ID"] + "|" + array);
               hflit.Set('hflit', r["ID"]);
               popup.Show();
               popup.BringToFront();
           } else
               ClientPackSize.SetText(array[0]);
           //ClientPackSize.PerformCallback("AddRow|" + s.GetValue());
           //CmbPackaging.PerformCallback("AddRow|" + s.GetValue());
           //var value = s.GetValue() + "|" + s.GetSelectedItem().GetColumnText(0) + "|" + s.GetSelectedItem().GetColumnText(1);
           //Grid.GetValuesOnCustomCallback(s.GetText().toString());
           var Net = r["NetWeight"].split("|");
           ClientNetweight.SetText(Net[0]);
           ClientNetUnit.SetText(Net[1]);
           var keys = lastCountry == '' ? r["ID"] : lastCountry;
           //CmbMargin.PerformCallback("reload|" + keys);
           //CmbMargin.PerformCallback("reload|" + lastCountry + "|" + s.GetValue());
       }
	function OnCustomButtonClick(s, e) {
           //e.processOnServer = true;
           //if (pagesymboll == null || pagesymboll == 1)
           if (confirm('Confirm: delete all items?')) {
               s.PerformCallback("remove|" + tcDemos.GetActiveTabIndex());
           }else
               s.PerformCallback("delete|" + tcDemos.GetActiveTabIndex());
       }
       function OnAllCheckedChanged(s, e) {
           if (s.GetChecked())
               testgrid.SelectRows();
           else
               testgrid.UnselectRows();
       }
       function buildActiontiilbar() {
           var tb = testgrid.GetToolbarByName("toolbar")
           var Delete = tb.GetItemByName("Remove")
           Delete.SetVisible = false;
       }
       function onAcceptClick(s, e) {
           EditPopup.Hide();
           grid.PerformCallback("Request|0");
           //alert('success')
       }
       function BuildEditPopup() {
           EditPopup.Show();
           EditPopup.BringToFront();
           ClientCountry.PerformCallback();
       }
       function OnToolbarItemClick(s, e) {
       	   
           if(e.item.name=="New")
               insertSelection('new');
           if (e.item.name == "Remove") {
               //if (pagesymboll == null || pagesymboll == 1)
                   if (confirm('Confirm: delete all items?')) {
                       testgrid.PerformCallback("Remove|0");
                   } else
                       testgrid.PerformCallback("Delete|0");
           }
           //e.processOnServer = true;
           //e.usePostBack = false;
       }
      function OnDateChanged(s, e) {
           grid.PerformCallback("ChangeValidTo|0");
           testgrid.PerformCallback("ChangeValidTo|0");
       }
       function onCloseButtonClick(s, e) {
           if (curentEditingIndex <= -1)
               gvCode.batchEditApi.ResetChanges(curentEditingIndex);
       }
    </script>
<dx:ASPxHiddenField ID="hftablename" runat="server" ClientInstanceName="hftablename" />
<dx:ASPxHiddenField ID="hfid" runat="server" ClientInstanceName="hfid" />
<dx:ASPxHiddenField ID="hf" runat="server" ClientInstanceName="hf"/>
<dx:ASPxHiddenField ID="hfgetvalue" runat="server" ClientInstanceName="ClientNewID" />
<dx:ASPxHiddenField ID="hfStatusApp" runat="server" ClientInstanceName="hfStatusApp" />
<dx:ASPxHiddenField ID="hfFolio" runat="server" ClientInstanceName="hfFolio"/>
<dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
<dx:ASPxHiddenField ID="approv" runat="server" ClientInstanceName="approv" />
<dx:ASPxHiddenField ID="hftype" runat="server" ClientInstanceName="hftype" />
<dx:ASPxHiddenField ID="hfuser" runat="server" ClientInstanceName="username"/>
<dx:ASPxHiddenField ID="usertp" runat="server" ClientInstanceName="usertp" />
<dx:ASPxHiddenField ID="hfRequestNo" runat="server" ClientInstanceName="hfRequestNo"/>
<dx:ASPxHiddenField ID="hfuserlevel" runat="server" ClientInstanceName="hfuserlevel"/>
<dx:ASPxGridView runat="server" ID="gridData" KeyFieldName="ID" DataSourceID="dsgv" ClientInstanceName="ClientCostGrid" Width="100%" 
        AutoGenerateColumns="true" OnCustomCallback="gridData_CustomCallback" OnCustomButtonCallback="gridData_CustomButtonCallback"
        OnCustomDataCallback="gridData_CustomDataCallback" OnCustomGroupDisplayText="gridData_CustomGroupDisplayText"
        OnDataBound="gridData_DataBound"
	OnBeforeGetCallbackResult="BeforeGetCallbackResult" OnPreRender="PreRender"
        OnFillContextMenuItems="gridData_FillContextMenuItems" OnContextMenuItemClick="gridData_ContextMenuItemClick"
        Border-BorderWidth="0">
    <Columns>
        <dx:GridViewCommandColumn ShowNewButton="true" ShowEditButton="true" VisibleIndex="0" ButtonRenderMode="Image" Width="45" Visible="false">
            <CustomButtons>
                <dx:GridViewCommandColumnCustomButton ID="Clone">
                    <Image ToolTip="Clone Record" Url="~/Content/Images/Copy.png"/>
                </dx:GridViewCommandColumnCustomButton>
            </CustomButtons>
        </dx:GridViewCommandColumn>
	<dx:GridViewCommandColumn ShowSelectCheckbox="True" ShowClearFilterButton="true" SelectAllCheckboxMode="Page" Width="45" VisibleIndex="0" Visible="false"/>
        <dx:GridViewDataTextColumn FieldName="Company" />
        <dx:GridViewDataTextColumn FieldName="RequestNo" Caption="Request No" GroupIndex="0"/>
        <dx:GridViewDataTextColumn FieldName="Revised" Caption="Revised" Width="70px"/>
        <dx:GridViewDataComboBoxColumn FieldName="StatusApp">
            <PropertiesComboBox DataSourceID="dsStatusApp" TextField="Title" ValueField="ID" />
        </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataTextColumn FieldName="MarketingNumber" Caption="Costing No" />
        <dx:GridViewDataTextColumn FieldName="RDNumber" Caption="RD Number" />
        <dx:GridViewDataTextColumn FieldName="PackSize" Caption="Size" Width="40px"/>
        <dx:GridViewDataTextColumn FieldName="CanSize" Caption="CanSize" />
        <dx:GridViewDataTextColumn FieldName="Customer"/>
        <dx:GridViewDataTextColumn FieldName="Destination"/>
        <dx:GridViewDataComboBoxColumn FieldName="Requester">
            <PropertiesComboBox DataSourceID="dsulogin" TextField="fn" ValueField="user_name" />
        </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataTextColumn FieldName="NetWeight" Caption="Net weight" />
        <dx:GridViewDataDateColumn FieldName="CreateOn" Caption="CreateOn"> 
            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
            </PropertiesDateEdit>
        </dx:GridViewDataDateColumn>
	<dx:GridViewDataColumn FieldName="UniqueColumn" Visible="false"/>
        </Columns>
	<SettingsDataSecurity AllowReadUnlistedFieldsFromClientApi="True" />
        <SettingsSearchPanel ColumnNames="" Visible="false" />
		<Settings ShowGroupPanel="false" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="false" />
		<SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true"/>
		    <SettingsPager PageSize="50">
        <PageSizeItemSettings Visible="true" ShowAllItem="true"  AllItemText="All Records" />
        </SettingsPager>
		<Styles>
			<Row Cursor="pointer" />
		</Styles>
	        <TotalSummary>
            <dx:ASPxSummaryItem FieldName="RequestNo" SummaryType="Count" />
        </TotalSummary>
        <SettingsContextMenu Enabled="true" />
        <ClientSideEvents RowClick="Demo.ClientCostGrid_RowClick"
            ContextMenuItemClick="function(s,e) { OnContextMenuItemClick(s, e); }"
            Init="Demo.ClientCostGrid_Init" />
    </dx:ASPxGridView>
    <dx:ASPxGridViewExporter runat="server" ID="GridExporter" GridViewID="gridData" />
    <dx:ASPxPopupControl ID="pcassign" runat="server" ShowCloseButton ="true" AllowDragging="True" AllowResize="true"
            ShowFooter="True" PopupAction="None" CloseAction="OuterMouseClick" CloseOnEscape="true" PopupHorizontalAlign="OutsideRight"
            PopupVerticalAlign="Below" Width="680px" Height="380px"
            PopupHorizontalOffset="40" PopupVerticalOffset="40">
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
    <dx:ASPxPopupControl ID="EditPopup" ClientInstanceName="EditPopup" runat="server" 
    Modal="true" CloseAction="CloseButton" PopupHorizontalAlign="Center" PopupVerticalAlign="Middle"
    AllowResize="true" ShowFooter="true" AllowDragging="True">
    <ClientSideEvents CloseButtonClick="onCloseButtonClick" />
    <ContentCollection>
        <dx:PopupControlContentControl runat="server">
            <dx:ASPxLabel ID="ASPxLabel1" runat="server" Text="Zone/Country" />
            <dx:ASPxComboBox ID="CmbCountry" runat="server" OnCallback="CmbCountry_Callback"  ValueField="Code" TextField="Name"
                ClientInstanceName="ClientCountry">
                <Columns>
                    <dx:ListBoxColumn FieldName="Code" />
                    <dx:ListBoxColumn FieldName="Name" />
                    <dx:ListBoxColumn FieldName="Zone" />
                </Columns>
                <ClientSideEvents SelectedIndexChanged="function(s, e) { 
                                                       var value =s.GetSelectedItem().GetColumnText(2);
                                                       textBoxCountry.SetText(value); }" />
            </dx:ASPxComboBox>
            <dx:ASPxLabel ID="ASPxLabel2" runat="server" Text="Country" />
            <dx:ASPxTextBox ID="textBoxCountry" ClientInstanceName="textBoxCountry" runat="server">
            </dx:ASPxTextBox>
            <%--<dx:ASPxLabel ID="ASPxLabel3" runat="server" Text="Over Type" />
            <dx:ASPxComboBox ID="CmbOverType" runat="server" DataSourceID="dsOverType" ValueField="value" TextField="value" ClientInstanceName="CmbOverType" />
            <dx:ASPxLabel ID="ASPxLabel4" runat="server" Text="Case per fcl." />
            <dx:ASPxTextBox ID="TextBoxSubContainers" ClientInstanceName="textBoxSubContainers" runat="server">
                <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
            </dx:ASPxTextBox>
            <dx:ASPxLabel ID="ASPxLabel5" runat="server" Text="Address" />
            <dx:ASPxTextBox ID="TextBoxAddress" ClientInstanceName="textBoxAddress" runat="server" />--%>
            <br />
            <dx:ASPxButton ID="btnAccept" runat="server" Text="Accept" AutoPostBack="false" ClientInstanceName="btnAccept">
                <ClientSideEvents Click="onAcceptClick" />
            </dx:ASPxButton>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl ID="popup" ClientInstanceName="popup" runat="server" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
        AllowDragging="True" PopupAnimationType="None" EnableViewState="False"
        HeaderText="Show Popup Window" Width="350px">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <table>
                    <tr>
                        <td>
                            <dx:ASPxBinaryImage ID="edBinaryImage" runat="server" AlternateText="Loading..."
                                ImageAlign="Left" CssClass="Image">
                            </dx:ASPxBinaryImage>
                            <dx:ASPxHiddenField ID="hflit" runat="server" ClientInstanceName="hflit"/>
                            <dx:ASPxGridView ID="GridView1" DataSourceID="ods" ClientInstanceName="GridView1" runat="server"
                                OnCustomCallback="GridView1_CustomCallback" ClientVisible="false"
                                KeyFieldName="ID;MarketingNumber;RequestNo">
                                <Columns>
                                    <%--<dx:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0" SelectAllCheckboxMode="Page" />--%>
                                    <dx:GridViewDataColumn FieldName="MarketingNumber" Caption="CostingNo" VisibleIndex="1" />
                                    <dx:GridViewDataColumn FieldName="RDNumber"/>
                                    <dx:GridViewDataColumn FieldName="CanSize"/>
                                    <%--<dx:GridViewDataColumn FieldName="RequestNo"/>--%>
                                </Columns>
                                <SettingsBehavior AllowFocusedRow="True" />
                                <ClientSideEvents RowDblClick="UpdateSelection" /> 
                            </dx:ASPxGridView>
			                    <dx:ASPxTreeList ID="treeList" ClientInstanceName="treeList" runat="server" OnDataBinding="treeList_DataBinding" ClientVisible="false"
                                OnCustomCallback="treeList_CustomCallback" OnCustomDataCallback="treeList_CustomDataCallback" Width="100%" 
				                OnCustomJSProperties="treeList_CustomJSProperties"
                                KeyFieldName="ID" ParentFieldName="ParentID">
                                <ClientSideEvents NodeDblClick="treeList_DblClick" />
                                    <Settings ShowTreeLines="False" SuppressOuterGridLines="true" />
                                    <SettingsBehavior AllowFocusedNode="True" ExpandCollapseAction="NodeDblClick" />
                                </dx:ASPxTreeList>
                            <%--<dx:ASPxDropDownEdit ID="DropDownEdit" runat="server" ClientInstanceName="DropDownEdit" ClientVisible="false"
                                Width="170px" AllowUserInput="False" AnimationType="None">
                                <DropDownWindowTemplate>
                                    <div>
                                      <dx:ASPxTreeList ID="TreeList" ClientInstanceName="TreeList" runat="server" OnCustomDataCallback="TreeList_CustomDataCallback" 
                                            OnDataBinding="TreeList_DataBinding"
                                            OnCustomCallback="TreeList_CustomCallback"
                                            Width="500px" OnCustomJSProperties="TreeList_CustomJSProperties"
                                            KeyFieldName="ID" ParentFieldName="ParentID">
                                            <Settings VerticalScrollBarMode="Auto" ScrollableHeight="150" />
                                            <ClientSideEvents FocusedNodeChanged="function(s,e){ selectButton.SetEnabled(true); }" />
                                            <BorderBottom BorderStyle="Solid" />
                                            <SettingsBehavior AllowFocusedNode="true" AutoExpandAllNodes="true" FocusNodeOnLoad="false" />
                                            <SettingsPager Mode="ShowAllNodes">
                                            </SettingsPager>
                                            <Styles>
                                                <Node Cursor="pointer">
                                                </Node>
                                                <Indent Cursor="default">
                                                </Indent>
                                            </Styles>
                                          <Columns>
                                              <dx:TreeListTextColumn FieldName="MarketingNumber" VisibleIndex="0"/>
                                              <dx:TreeListTextColumn FieldName="Material" VisibleIndex="1"/>
                                              <dx:TreeListTextColumn FieldName="PackSize" VisibleIndex="2"/>
                                          </Columns>
                                          </dx:ASPxTreeList>  
                                    </div>
                                    <table style="background-color: White; width: 100%;">
                                        <tr>
                                            <td style="padding: 10px;">
                                                <dx:ASPxButton ID="clearButton" ClientEnabled="false" ClientInstanceName="clearButton"
                                                    runat="server" AutoPostBack="false" Text="Clear">
                                                </dx:ASPxButton>
                                            </td>
                                            <td style="text-align: right; padding: 10px;">
                                                <dx:ASPxButton ID="selectButton" ClientEnabled="false" ClientInstanceName="selectButton"
                                                    runat="server" AutoPostBack="false" Text="Select">
                                                    <ClientSideEvents Click="UpdateSelection" />
                                                </dx:ASPxButton>
                                                <dx:ASPxButton ID="closeButton" runat="server" AutoPostBack="false" Text="Close">
                                                    <ClientSideEvents Click="function(s,e) { popup.Hide(); }" />
                                                </dx:ASPxButton>
                                            </td>
                                        </tr>
                                    </table>
                                </DropDownWindowTemplate>
                            </dx:ASPxDropDownEdit>--%>
                        </td>
                    </tr>
                </table>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
 <%--<dx:ASPxCallback ID="cb" ClientInstanceName="cb" runat="server" OnCallback="cb_Callback">
        <ClientSideEvents CallbackComplete="function (s, e) { var test =e.result; }" />
    </dx:ASPxCallback>
    <dx:ASPxRoundPanel ID="rp" ShowHeader="false" runat="server" Width="400px">
        <PanelCollection>
            <dx:PanelContent runat="server">
     --%>
    <dx:ASPxCallbackPanel ID="PreviewPanel" runat="server" RenderMode="Div" Height="100%" CssClass="PreviewPanel" ClientInstanceName="ClientPreviewPanel" ClientVisible="false"
        OnCallback="PreviewPanel_Callback" />
    <dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" ClientVisible="false" ScrollBars="Vertical" ClientInstanceName="ClientCostFormPanel">
        <ClientSideEvents Init="Demo.Init" />
        <PanelCollection>
            <dx:PanelContent>  
            <dx:ASPxFormLayout runat="server" ID="formLayout" ClientInstanceName="formLayout" CssClass="formLayout" AlignItemCaptionsInAllGroups="true">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
            <Items>
		<dx:LayoutGroup Caption="Costing Sheet" ColCount="4" GroupBoxDecoration="HeadingLine" UseDefaultPaddings="false" Paddings-PaddingTop="10">
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
                                <%--<dx:ASPxComboBox ID="CmbCostingNo" runat="server"  DropDownWidth="650px" ClientInstanceName="ClientCostingNo"
                                    DropDownStyle="DropDownList" ValueField="ID" OnCallback="CmbCostingNo_Callback"
                                    ValueType="System.String" TextFormatString="{0}" EnableCallbackMode="true" IncrementalFilteringMode="Contains"
                                    CallbackPageSize="30">
                                    <ClientSideEvents EndCallback="OnEndCallback" TextChanged="OnTextChanged"/>
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="RequestNo" Width="130px" />      
                                        <dx:ListBoxColumn FieldName="Revised" Width="30px" />
                                        <dx:ListBoxColumn FieldName="RequestDate" Caption="Form"  Width="80px" />
                                        <dx:ListBoxColumn FieldName="RequireDate" Caption="To" Width="80px" />
                                        <dx:ListBoxColumn FieldName="PackSize" Width="100px" />
                                        <dx:ListBoxColumn FieldName="Packaging" Caption="Primary Package" Width="80px" />
                                        <dx:ListBoxColumn FieldName="RequestType" Width="0px" />
                                        <dx:ListBoxColumn FieldName="NetWeight" Width="50px" />
					                    <dx:ListBoxColumn FieldName="Customer" Width="90px" />
                                        <dx:ListBoxColumn FieldName="UserType" Width="90px" />
                                        <dx:ListBoxColumn FieldName="Product" Width="90px" />
                                    </Columns>
                                </dx:ASPxComboBox>
                                <dx:ASPxGridLookup ID="CmbCostingNo" runat="server" KeyFieldName="ID" TextFormatString="{0}" 
                                    ClientInstanceName="ClientCostingNo" Width="200" OnInit="CmbCostingNo_Init" DataSourceID="dsCostingNo">
                                    <ClientSideEvents ValueChanged="OnTextChanged" />
                                    <Columns>                                     
                                        <dx:GridViewDataTextColumn FieldName="RequestNo" Width="100px" />
                                        <dx:GridViewDataTextColumn FieldName="MarketingNumber" />
                                        <dx:GridViewDataTextColumn FieldName="RequireDate" Width="100px" /> 
                                        <dx:GridViewDataTextColumn FieldName="PackSize" />
                                    </Columns>
                                    <GridViewProperties>
                                        <SettingsBehavior AllowFocusedRow="True"></SettingsBehavior>
                                    </GridViewProperties>
                                </dx:ASPxGridLookup>--%>
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
                                        <ClientSideEvents  ValueChanged="OnExchangeRate" />
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
                                            <dx:ASPxComboBox runat="server" ID="CmbPackaging" ValueField="ID" TextField="Name" DataSourceID="dsPrimary"
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
				            <%--<dx:ASPxTokenBox ID="tkbPackSize" runat="server" AllowMouseWheel="True" ClientInstanceName="ClientPackSize" 
                                    OnCallback="tkbPackSize_Callback" TextField="Value" ValueField="Value"  
                                    EnableCallbackMode="True" Tokens="" >
                                    <ValidationSettings>
				                        <RegularExpression ValidationExpression="\d+" />
			                        </ValidationSettings>
                                    <ClientSideEvents TokensChanged="function(s, e) { OnValueChanged(s,'pkg'); }"
                                         KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}"/>
                                </dx:ASPxTokenBox>
                              <dx:ASPxComboBox ID="CmbPackSize" runat="server" TextField="Value" ValueField="Value"  
                                  OnCallback="CmbPackSize_Callback" ClientInstanceName="ClientPackSize">
                                  <ClientSideEvents  ValueChanged="function(s, e) {
                                                    Upload.SetEnabled(true);}" />
                                  <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                              </dx:ASPxComboBox>--%>
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
				    <ClientSideEvents DateChanged="OnDateChanged" />
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
				 <ClientSideEvents DateChanged="OnDateChanged" />
                                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                            </dx:ASPxDateEdit> 
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
                         <%--<dx:LayoutItem Caption="Packaging">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox runat="server" ID="CmbPackaging" ValueField="ID" TextField="Name" DataSourceID="dsPrimary"
                                        IncrementalFilteringMode="StartsWith"
                                        ClientInstanceName="CmbPackaging" >
                                        <ClientSideEvents ValueChanged="function(s, e) {  
                                                        grid.PerformCallback('Labor|'+s.GetText());
							CmbMargin.PerformCallback('reload|'+s.GetText());}" />
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>--%>
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
                        <dx:LayoutItem Caption="Net weight" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <table>
                                           <tr>
                                               <td><dx:ASPxTextBox ID="tbNetweight" runat="server" ClientInstanceName="ClientNetweight" Width="113px">
                                                   <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                                   <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" 
                                                        ValueChanged="function(s, e) {  
                                                        grid.PerformCallback('Labor|'+s.GetText());}"/>
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
                        <Items>
                    <dx:LayoutItem Caption="Upload file" Name="Upload" Width="100%">
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
		                <%--<dx:LayoutItem Caption="Material">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxTextBox runat="server" ID="tbMaterial" ClientInstanceName="tbMaterial">
                                    <ClientSideEvents ValueChanged="function(s, e) {OnTextChangedData('Code|',s.GetText());}" />
                                </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
		        <dx:LayoutItem Caption="Product Name">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxTextBox runat="server" ID="tbProductName" ClientInstanceName="tbProductName">
                                    <ClientSideEvents ValueChanged="function(s, e) {OnTextChangedData('Name|',s.GetText());}" />
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="RD Number">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox runat="server" ID="tbRDNumber" ClientInstanceName="ClientRDNumber" MaxLength="250" Width="230px">
				    <ClientSideEvents ValueChanged="function(s, e) {OnTextChangedData('RefSamples|',s.GetText());}" />
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                     </dx:LayoutItem>--%>
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
                                    <dx:ASPxSpinEdit ID="tbQuantity" runat="server" ClientInstanceName="tbQuantity"> 
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
		            <dx:LayoutGroup Caption="Product Name" ColCount="4" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
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
                        <dx:LayoutItem Caption="">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxGridView ID="gvCode" runat="server" ClientInstanceName="gvCode" OnDataBinding="gvCode_DataBinding"
                                    OnCustomCallback="gvCode_CustomCallback" OnBatchUpdate="gvCode_BatchUpdate" OnDataBound="gvCode_DataBound"                                    
                                    KeyFieldName="ID">
                                    <ClientSideEvents BatchEditEndEditing="gvCode_OnBatchEditEndEditing" BatchEditStartEditing="function(s, e) {
                                            curentEditingIndex = e.visibleIndex;
                                            clearTimeout(timerHandle); }"/>
                                    <Columns>
                                        <dx:GridViewCommandColumn ShowClearFilterButton="true" Width="52px" ButtonRenderMode="Image" ShowInCustomizationForm="True"
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
					
					<dx:GridViewDataTextColumn FieldName="Code" />
                                        <dx:GridViewDataTextColumn FieldName="Name"/>
                                        <dx:GridViewDataColumn FieldName="Formula" Width="145px"/>
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
                <dx:LayoutGroup Caption="Attached file" ColCount="4" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
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
                        <dx:LayoutItem Caption="">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="2" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
				                    <dx:ASPxCallbackPanel ID="cpUpdateFiles" runat="server" Width="100%" ClientInstanceName="cpUpdateFiles" OnCallback="cpUpdateFiles_Callback">
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
				    <%--
					<dx:ASPxFileManager ID="fileManager" runat="server" DataSourceID="ArtsDataSource" 
					OnFileDownloading="fileManager_FileDownloading"
					OnCustomCallback="fileManager_CustomCallback" Height="240px" 
                                        ClientInstanceName="fileManager">
                                        <ClientSideEvents FileUploaded="OnFileUploaded"/>
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
					<Settings EnableMultiSelect="false" />
					</dx:ASPxFileManager>--%>
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
                                <dx:ASPxGridView ID="grid" ClientInstanceName="grid" runat="server" Width="100%" OnCustomCallback="grid_CustomCallback"
                                    SettingsPager-PageSize="60" OnDataBinding="grid_DataBinding"  AutoGenerateColumns="false" KeyFieldName="RowID" 
                                    OnDataBound="grid_DataBound" OnCellEditorInitialize="grid_CellEditorInitialize" OnCustomDataCallback="grid_CustomDataCallback"
                                    OnCustomSummaryCalculate="grid_CustomSummaryCalculate" OnHtmlDataCellPrepared="grid_HtmlDataCellPrepared"
                                    OnCommandButtonInitialize="grid_CommandButtonInitialize" OnBatchUpdate="grid_BatchUpdate"
				                    OnCustomButtonCallback="grid_CustomButtonCallback">
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
                                                    <ClientSideEvents Click="function(s,e){ grid.AddNewRow(); }" />
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
                                                <ClientSideEvents SelectedIndexChanged="rate_SelectedIndexChaged" />
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
                                    <ClientSideEvents BatchEditEndEditing="OnBatchEditEndEditing" BatchEditStartEditing="OnBatchEditStartEditing" 
					                    Init="OnInitGridBatch" FocusedCellChanging="onFocusedCellChanging"/>                                
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

				 <dx:ASPxGridView ID="testgrid" ClientInstanceName="testgrid" runat="server" Width="100%" KeyFieldName="ID"   
                                    OnDataBinding="testgrid_DataBinding" OnDataBound="testgrid_DataBound"
                                    OnCustomDataCallback="testgrid_CustomDataCallback"
                                    OnCustomCallback="testgrid_CustomCallback" OnBatchUpdate="testgrid_BatchUpdate">
                                    <ClientSideEvents BatchEditEndEditing="testgrid_OnBatchEditEndEditing" BatchEditStartEditing="getId" 
					CustomButtonClick="OnCustomButtonClick" ToolbarItemClick="OnToolbarItemClick"/>
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
					                <SettingsPager AlwaysShowPager="true" PageSize="50"/>
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
        SelectCommand="spselectcosting" SelectCommandType="StoredProcedure">
        <SelectParameters>
	    <asp:ControlParameter ControlID="hftype" Name="type" PropertyName="['type']"/>
            <asp:ControlParameter ControlID="hfuser" Name="user_name" PropertyName="['user_name']"/>
        </SelectParameters>
    </asp:SqlDataSource>
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
    <asp:SqlDataSource ID="dsLaborOverhead" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetLaborOverhead" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="FormPanel$formLayout$CmbCompany" Name="Company" PropertyName="Value" />
            <asp:ControlParameter ControlID="FormPanel$formLayout$CmbPackaging" Name="Packaging" PropertyName="Text" />
            <asp:ControlParameter ControlID="FormPanel$formLayout$tbNetweight" Name="NetWeight" PropertyName="Text" />
            <asp:ControlParameter ControlID="FormPanel$formLayout$tbPackSize" Name="PackSize" PropertyName="Text" />
	        <asp:ControlParameter ControlID="usertp" Name="UserType" PropertyName="['usertype']"/>
        </SelectParameters>
    </asp:SqlDataSource>
<%--    <asp:SqlDataSource ID="dsLabor" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetLabor" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Name="Company" Type="String" />
            <asp:Parameter Name="CostingNo" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsMargin" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetMargin" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Name="Company" Type="String" />
            <asp:Parameter Name="CostingNo" Type="String" />
	        <asp:Parameter Name="NetWeight" Type="String" />
	        <asp:Parameter Name="Packaging" Type="String" />
		    <asp:Parameter Name="UserType" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>--%>
<%--<asp:SqlDataSource ID="dsPackage" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetPackage" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Name="Id" Type="String" />
            <asp:Parameter Name="Company" Type="String" />
            <asp:Parameter Name="type" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsSecPackage" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select ID,SAPMaterial,Name,Price,Currency,Unit from MasPrice where Company=@Company and SAPMaterial like '5%' and substring(SAPMaterial,2,1) in ('F')">
        <SelectParameters>
            <asp:Parameter Name="Company" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsPackage" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select ID,SAPMaterial,Name,Price,Currency,Unit from MasPrice where Company=@Company and SAPMaterial like '5%' and substring(SAPMaterial,2,1) in ('1')">
        <SelectParameters>
            <asp:Parameter Name="Company" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsYield" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="Select * From MasYield Where Company=@Company and substring(Material,1,1) not in ('5')">
        <SelectParameters>
            <asp:Parameter Name="Company" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>--%>
    <asp:SqlDataSource ID="dsCurrency" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('THB;USD;',';')">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsUnit" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('KG;G;Ton',';')">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"/>
<%--    <asp:SqlDataSource ID="dsRawMaterial" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetRawMaterial" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="FormPanel$formLayout$CmbCompany" Name="Company" PropertyName="Value" />
	    <asp:ControlParameter ControlID="hfRequestNo" Name="RequestNo" PropertyName="['ID']" />
        </SelectParameters>
    </asp:SqlDataSource>--%>
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
    <asp:SqlDataSource ID="dsPrimary" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetPrimary" SelectCommandType="StoredProcedure">
        <SelectParameters>
             <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
    </asp:SqlDataSource>
   <asp:SqlDataSource ID="dsnamelist" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="spGetFormulaHeader2" SelectCommandType="StoredProcedure">
       <SelectParameters>
           <asp:Parameter Name="Id" />
       </SelectParameters>
   </asp:SqlDataSource>
   <asp:SqlDataSource ID="dsStatusApp" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasStatusApp where levelapp in (1,2)"/>
<%--
    <asp:SqlDataSource ID="ArtsDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetFileSystem" SelectCommandType="StoredProcedure" CancelSelectOnNullParameter="False"
        DeleteCommand="DELETE FROM FileSystem2 WHERE ID = @Id">
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <SelectParameters>
           <asp:ControlParameter ControlID="hfgetvalue" Name="GCRecord" PropertyName="['NewID']"/>
           <asp:ControlParameter ControlID="hfuser" Name="username" PropertyName="['user_name']"/>
	   <asp:ControlParameter ControlID="hftablename" Name="tablename" PropertyName="['tablename']"/>
        </SelectParameters>
    </asp:SqlDataSource>--%>
    <asp:SqlDataSource ID="dsLOHBase" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('%;THB',';') order by value"/>
    <asp:SqlDataSource ID="dsLOH" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('Labelling & WH DL;LOH Factor Adjustment',';') order by value"/>