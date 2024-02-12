<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomerEditForm.ascx.cs" Inherits="UserControls_WebUserControl" %>
<script type="text/javascript">

    var lastCountry = null;
    var pagesymboll = null;
    var curentEditingIndex;
    var textSeparator = ";";
    var windowOverName = '';
    function onFileUploadStart(s, e) {
        //UploadedFilesTokenBox.SetVisible(true);
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
    function OnTabbedGroupPageControlInit(s, e) {
        s.SetActiveTabIndex(0);
        //s.SetActiveTabIndex(TabList.GetActiveTabIndex());
	}
    function OnBatchEditStartEditing(s, e) {
        curentEditingIndex = e.visibleIndex;
        //var currentCountry = ClientintGrid.batchEditApi.GetCellValue(curentEditingIndex, "SAPMaterial");
        //if (currentCountry != lastCombo && e.focusedColumn.fieldName == "RawMaterial" && currentCountry != null) {
        //    lastCombo = currentCountry;
        //    RefreshData(currentCountry);
        //}
    }

    function SynchronizeListBoxValues(dropDown, args, text) {
        debugger;
        var texts = dropDown.GetText().split(textSeparator);
        var values = GetValuesByTexts(texts, text);
        //if (text == '0')
        //    checkListBox.SelectValues(values);
        if (text == '1')
            cbRequestType.SelectValues(values);
        UpdateText(text); // for remove non-existing texts
        if (values != "")
            dropDown.isValid = false;
    }
       
    function OnListBoxSelectionChanged(listBox, args, text) {
        UpdateText(text);
    }
    function UpdateText(text) {
        var selectedItems = null;
        switch (text) {
            //case '0':
            // selectedItems = checkListBox.GetSelectedItems();
            // checkComboBox.SetText(GetSelectedItemsText(selectedItems));
            // break;
            case '1':
                selectedItems = cbRequestType.GetSelectedItems();
                ClientRequestType.SetText(GetSelectedItemsText(selectedItems));
                break;
        }
    }
    function GetSelectedItemsText(items) {
        var texts = [];
        for (var i = 0; i < items.length; i++)
            //if (items[i].index != 0)
            texts.push(items[i].text);
        return texts.join(textSeparator);
    }
    function GetValuesByTexts(texts, text) {
        var actualValues = [];
        var item;
        for (var i = 0; i < texts.length; i++) {
            //if(text == '0')
            //    item = checkListBox.FindItemByText(texts[i]);
            if (text == '1')
                item = cbRequestType.FindItemByText(texts[i]);
            if (item != null)
                actualValues.push(item.value);
        }
        return actualValues;
    }

    function rb_SelectedIndexChanged(s, e) {
        var popup = Demo.GettypeControl();
        popup.Hide();
        var keys = s.GetValue();
        usertp.Set("usertype", keys);
        ClientCompany.PerformCallback('reload');
        Demo.ChangeDemoState("MailForm", "read", 0);
    }
    function combo_SelectedIndexChanged(s) {
        debugger;
        var KeyText = s.GetSelectedItem().GetColumnText(2);
        var arg = ["Add", s.GetValue(), "0"].join('|');
            ClientPetCategory.PerformCallback(arg);
            //CmbAssign.GetGridView().PerformCallback("Add|" + s.GetValue());
            //CmbAssign.GetGridView().Refresh();
    }
    function CateSelectedIndexChanged(s, e) {
	    var obj = s.GetSelectedItem().GetColumnText(1);//UserType
        //ClientCompliedWith.SetEnabled(obj == 0 || obj == 1);
	    var arg = ["Add",ClientCompany.GetValue(), s.GetValue(),-1].join('|');
        //ClientReceiver.PerformCallback(arg);
        //usertp.Set("usertype", obj);
        var value = 'reload|' + s.GetValue();
        var rusult = 'build|' + s.GetValue();
        cpCustomerRole.PerformCallback(rusult);
        cpUpdateRole.PerformCallback(rusult);
        cpPrdRequirement.PerformCallback(rusult);
        ClientSellingUnit.PerformCallback("reload|-1");
    }
    function RecSelectedIndexChanged(s, e) {
        debugger;
        var b = false;
        var myType = ClientRequestType.GetText();
        if (myType == "") {
            s.SetSelectedIndex(-1);
            return alert('Please select request type !!!');
        }
        var a = myType.split(";"), i;
        for (i = 0; i < a.length; i++) {
            if (a[i] == "Costing")
                b = true;
        }
        if (s.GetText() == "None" && b == false) {
            s.SetSelectedIndex(-1);
            return alert('Please select R&D !!!');
        }
    }
    function OnCountryChanged(ClientPrimary) {
        var KeyText = ClientPrimary.GetText().toString();
        ClientColor.SetText("");
        if (ClientPFType.InCallback())
            lastCountry = ClientPrimary.GetText().toString();
        else {
            setprimary(KeyText);
        }Cmb
    }
    function setprimary(KeyText) {
        build_data(KeyText);
    }
    function OnEndCallback(s, e) {
        if (lastCountry) {
            build_data(lastCountry);
            lastCountry = null;
        }
    }
    function build_data(KeyText) {
        //ClientPFType.PerformCallback(KeyText);
        //ClientDesign.PerformCallback(KeyText);
        //ClientPFLid.PerformCallback(KeyText);
        //ClientLacquer.PerformCallback(KeyText);
        //ClientColor.PerformCallback(KeyText);
        //ClientMaterial.PerformCallback(KeyText);
        //ClientShape.PerformCallback(KeyText);
    }
    function OnFileUploadComplete(s, e) {
        //    ClientintGrid.PerformCallback();
        //ClientintGrid.PerformCallback('upload|1');
        debugger;
        if (e.callbackData) {
            //cmbformu.PerformCallback();
            gvformu.PerformCallback("reload|" + usertp.Get("usertype"));
        }
    }
    function UpCode_FileUploadComplete(s, e) {
        //debugger;
        //if (update_name == 'gv_Upload')
        gv.PerformCallback('upload|1');
        //if (update_name == 'grid_Upload')
        //    grid.PerformCallback('upload|1');
    }
    function MoveToPage(symbol) {
        debugger;
        //alert(symbol);
        pagesymboll = symbol
        gvformu.PerformCallback('symbol|' + symbol);
        //GridView1.PerformCallback('symbol|' + symbol);
    }
        //function OnRowClick(s, e) {       
        //    var key = s.GetRowKey(e.visibleIndex, "Code");
        //    debugger;
        //    alert('Last Key = ' + key);
        //    window.open('Default.aspx?viewMode=CostingEditForm&Code=' + key + '&Edit=1', 'blank');
        //}
        //function OnRowClick(s, e) {
        //    alert(e.visibleIndex);
        //    s.GetRowValues(e.visibleIndex, 'Code', OnGetRowValues);
        //}
    function OnGetRowValues(values) {
        //alert(values);
        //debugger;
        if (values == "")
            return;
        gvformu.PerformCallback("EditCost|" + values);
        //var ID = tbRequestNoEdit.GetText().toString()
        //window.open('Default.aspx?viewMode=CostingEditForm&ID=' + ID + '&Code=' + values + '&Edit=1', 'blank');
    }

    function OnIDChanged(s) {
            var Company = ClientCompany.GetValue();
            var Cate = ClientPetCategory.GetValue();
        if (s.GetValue().toString() == "3" || s.GetValue().toString() == "5" || s.GetValue().toString() == "7") {
            formLayout.GetItemByName("Reason").SetVisible(false);
            var name = s.GetText().toString() == "Reject" ? "Assignee;Reason" : "Assignee";
            var args = name.split(";")
            var identi = true;// s.GetValue().toString() == "3" || s.GetValue().toString() == "5";
            for (i = 0; i < args.length; i++) {
                obj = formLayout.GetItemByName(args[i]);
                if (s.GetText().toString() == "Reject" && args[i] == "Assignee")
                    obj.SetCaption("To");
                else
                    obj.SetCaption(args[i]);
                formLayout.GetItemByName(args[i]).SetVisible(identi);
            }
            var NewID = ClientNewID.Get("NewID");
            //hfDisponsition.Set('Value', s.GetValue().toString());
            CmbAssignee.SetValue(null);
            var assi = ["load", Company, Cate].join('|');
            CmbAssignee.GetGridView().PerformCallback(assi);
            //var grid = CmbAssignee.GetGridView();
            //grid.UnselectRows();
            //grid.Refresh();
            CmbReason.PerformCallback(s.GetValue());
            } else if (s.GetValue().toString() == "1") {
            formLayout.GetItemByName("Assignee").SetVisible(true);
            var arge = ["Assignee",Company, Cate].join("|");
            CmbAssignee.GetGridView().PerformCallback(arge);
            } else {
                debugger;
                var myString = "Assignee;Reason";
                var a = myString.split(";"), i, obj;
                for (i = 0; i < a.length; i++) {
                    formLayout.GetItemByName(a[i]).SetVisible(false);
                }
            }
        }
    function OnGetRowTest(values) {
        
        var r = confirm("Are you copy sure ?");
        if (r == true) {
            var args = "0"; var keys = "";
            for (i = 0; i < values.length; i++) {
                keys += values[i] + "|";
            }
            hfpara.Set('para', 'copied|' + keys);
            //var para = hfpara.Get("copied");
            //alert(para);
            //    var args = "0";
            //    if (values[1] == "1" || values[2] != "4")
            //    {
            //        alert("Can not be copied");
            //        return;
            //    }
            //    if (values[2] == "4" && values[1]=="0") {
            //        var r = confirm("Are you copy formula sure ?");
            //        if (r == true) {
            //            args = "2";
            //        }
            //    } 
            ClientgridData.PerformCallback("Test|" + values[0] + "|" + args);
        }
    }
    function OnClosing(s, e) {
        debugger;
        var val=rbUserType.GetValue(); 
        if (e.cancel == false && val== null) {
            //var r = confirm('Do you want to close this popup?');
            //if (r == true) {
            Demo.ChangeDemoState("MailList");//}
        }
    }
    function OnCustomButtonClick(s, e) {
        s.GetRowValues(e.visibleIndex, 'ID;RequestType;StatusApp', OnGetRowTest);
        //var keyValue = ClientgridData.GetRowKey(e.visibleIndex);
        //if (r == true)
        //{ e.processOnServer = true; }
        //else
        //{ e.processOnServer = false; }
        //OnDetailsClick(keyValue)
    }

        //function OnDetailsClick(keyValue) {
        //    popup.Show();
        //    popup.PerformCallback(keyValue);
        //}
        //function CheckedChanged(s, e) {
        //    if (s.GetChecked()) {
        //    }
    //}
    function OnValidation(s, e) {
        debugger;
        if (e.value == null) {
            e.isValid = true;
            e.errorText = "Field is required.";
            return;
        }
        //else
        //    e.isValid = true;
    }
    function OnSelectedIndexChanged(s, e) {
        debugger;
        var value = null;
        value = s.GetSelectedItem().GetColumnText(1);//s.GetText().toString();
        gvformu.batchEditApi.SetCellValue(curentEditingIndex, "CostingSheet", value);
        //ClientintGrid.batchEditApi.SetCellValue(curentEditingIndex, "Code", s.GetSelectedItem().GetColumnText(0));
        //ClientintGrid.SetEditValue('CostingSheet', value);
        //alert(lastCountry);
        //ClientintGrid.GetValuesOnCustomCallback("Costing", DataCallback);
    }
    function DataCallback(result) {
        //alert(result);
        debugger;
        //var results = result.split("|");
        switch (result) {
            case "Costing":
                gvformu.batchEditApi.SetCellValue(curentEditingIndex, "CostingSheet", result);
                break;
          }
    }
    function OnCellMouseOver(cellElement, evt) {
        //alert(cellElement);
        ASPxClientPopupControl.GetPopupControlCollection().HideAllWindows();
        //ShowWindowByName(evt.item.name);
        var name = cellElement;
        ShowWindowByName(name, "ManhattanBar");
    }
    function ShowWindowByName(key,name) {
        var popupControl = GetPopupControl();
        var window = popupControl.GetWindowByName(name);
        debugger;
        var customerID = key;
        popupControl.SetFooterText('Id:' + key);
        popupControl.SetContentUrl('popupControls/MSCharts.aspx?id=' + customerID);
        popupControl.ShowWindowAtElementByID(window, "tdMenu");
    }
    function OnCellMouseOut(cellElement) {
        ASPxClientPopupControl.GetPopupControlCollection().HideAllWindows();
    }
    function GetPopupControl() {
        return ASPxPopupClientControl;
    }
    function OnCellClick(s, row_index, col_index, KeyValue, Data, event) {
        debugger;
        var rowVisibleIndex = row_index;
        //var fName = s.GetColumn(col_index).fieldName;
        var url = "./popupControls/MSCharts.aspx?id=" + KeyValue;
        window.open(url, "_blank");;
        event.cancelBubble = true;
        //if (fName == 'Info')
        //    event.cancel = true;
        //    menu.ShowAtPos(ASPxClientUtils.GetEventX(event), ASPxClientUtils.GetEventY(event));
    }
    function OnContextMenuItemClick(sender, args) {
        debugger;
        if (args.objectType == "row") {
            if (args.item.name == "ExportToPDF" || args.item.name == "ExportToXLS") {
                args.processOnServer = true;
                args.usePostBack = true;
            } else if (args.item.name == "Attach" || args.item.name == "Attach Spec" || args.item.name == "Upload Formula") {
                //alert(args.item.name);
                var ID = sender.GetRowKey(args.elementIndex);
                var a = ID.split("|")
                var assignurl = 'popupControls/wfassign.aspx?table=TransTechnical&view=' + args.item.name + '&id=' + a[0] + '&type=' + args.item.name;
                pcassign.SetFooterText('Id:' + a[0]);
                pcassign.SetContentUrl(assignurl);
                pcassign.SetHeaderText(assignurl);
                pcassign.Show();
                pcassign.BringToFront();
            } else if (args.item.name == "Product") {
                var ID = sender.GetRowKey(args.elementIndex);
                var a = ID.split("|")
                var url = 'popupControls/product_list.aspx?table=TransTechnical&view=' + args.item.name + '&id=' + a[0];
                url += "&usertype=" + usertp.Get("usertype");
                pcassign.SetContentUrl(url);
                pcassign.SetHeaderText(url);
                pcassign.Show();
                pcassign.BringToFront();
            } else if (args.item.name == "Assign") {
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
            } else {
                var g = ClientgridData;
                g.GetRowValues(args.elementIndex, 'RequestType', function (value) {
                    if (value) {
                        if (value == "1")
                            return alert("Can not " + args.item.name);
                        else {
                        args.processOnServer = false;
                        var key = sender.GetRowKey(args.elementIndex);
                        var a = key.split("|")
                        //if (a[2] == "0" && args.item.name == "Revised")
                        //        return alert("Can not be done");
                            hfpara.Set('para', args.item.name + '|' + key);
                            g.PerformCallback(args.item.name +"|" + a[0]);
                            Demo.ChangeDemoState("MailForm", "EditDraft", a[0]);
                        }
                    }
                });
            }
        }
    }
    function fn_AllowonlyNumeric(s, e) {
        var theEvent = e.htmlEvent || window.event;
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
        var regex = /[0-9,.]/;

        if (!regex.test(key)) {
            theEvent.returnValue = false;
            if (theEvent.preventDefault)
                theEvent.preventDefault();
        }
    }
    function OnAllCheckedChanged(s, e) {
            if (s.GetChecked())
                gvformu.SelectRows();
            else
                gvformu.UnselectRows();
    }
    //function HidePopupAndShowInfo(closedBy, returnValue) {
    //    CloseHidePopup();
    //    //alert('Closed By: ' + closedBy + '\nReturn Value: ' + returnValue);
    //    var a = returnValue.split("|");
    //    glAssignee.SetValue(a[1]);
    //    ClientgridData.PerformCallback('Assignee|' + returnValue);
    //}
    function CloseHidePopup() {
        pcassign.Hide();
    }
    function _BatchEditEndEditing(s, e) {
        debugger;
        window.setTimeout(function () {
            var count_value = Demo.GetChangesCount(s.batchEditApi);
            if (count_value > 0)
                s.UpdateEdit();
        }, 0);
    }

    function _CustomButtonClick(s, e) {
        debugger;
        s.GetRowValues(e.visibleIndex, 'Id',
            function (value) {
                s.PerformCallback(['New', value].join('|'));
            });
    }

    function OnFileUploaded(s, e) {
        console.log("e.fileName=" + e.fileName);
        console.log("e.folder=" + e.folder);
    }
    function ChangeDateTo(s, e) {
        var DateTo = new Date(s.GetDate());
        DateTo.setDate(DateTo.getDate() + 7);  
        deSampleDate.SetDate(DateTo);
    }  
        var postponedCallbackRequired = false;
    function OnSelectedFileChanged(s, e) {
            if (e.file) {

            }
    }
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
    function OnButtonClick(evt) {
        debugger;
        var deto = ClientValidto.GetValue();
        var defrom = ClientValidfrom.GetValue();
        var url = 'popupControls/popupsel.aspx?view=' + evt;
        if (evt == "Primary")
            url += "&usertype=" + usertp.Get("usertype");
        else if (evt == "Packaging") 
            url = 'popupControls/selmaster.aspx?view=' + evt + "&Id=" + hfGeID.Get("GeID") + "&Company=" + ClientCompany.GetValue() + "&Type=" + Clientusertp.Get("ut")+"&from=" + formatDate(defrom, 'yyyyMMdd') + "&to=" + formatDate(deto, 'yyyyMMdd');
        else if (evt == "DL" || evt == "BCDL")
            url = 'popupControls/selmaster.aspx?view=' + evt + "&SubID=" + ClientPage.GetValue() + "&Company=" + ClientCompany.GetValue();
        else if (evt == "Semi" || evt == "SelMaterial")
            url = 'popupControls/selmaster.aspx?view=' + evt + "&ID=" + hfGeID.Get("GeID") + "&Company=" + ClientCompany.GetValue();
        else if (evt == "Receiver")
            url += "&Plant=" + ClientCompany.GetValue() + "&Category=" + ClientPetCategory.GetValue();
        else if (evt == "CostingSheet")
            url = 'popupControls/selmaster.aspx?view=' + evt + "&Company=" + ClientCompany.GetValue() + "&type=1";
        else if (evt == "PackingStyle")
            url = 'popupControls/selmaster.aspx?view=' + evt + "&TypeofPrimary=" + ClientPrimary.GetValue();
        else 
            url += "&usertype=" + usertp.Get("usertype") + "&Primary=" + ClientPrimary.GetText();
        var popupControl = GetPopupControl();
        popupControl.RefreshContentUrl();
        popupControl.SetContentUrl(url);
        //popupControl.SetHeaderText("param");
        popupControl.SetFooterText(url);
        popupControl.Show();
        //popupControl.SetPopupElementID();
    }
    function onFocusedCellChanging(s, e) {
        //debugger;
        var Result = s.batchEditApi.GetCellValue(s.GetFocusedRowIndex(), 'Result');
        if (e.cellInfo.column.fieldName == "CostingSheet")
            e.cancel = Result != "0" || Result != "" ? true : false;
    }
    function HidePopupAndShowInfo(closedBy, returnValue) {
        //debugger;
        GetPopupControl().Hide();
        if (closedBy=="Receiver")
            ClientReceiver.SetValue(returnValue);
        if (closedBy == "Primary") {
            ClientPrimary.SetValue(returnValue);
            resetpkg();
        }
        if (closedBy =="Color")
            ClientColor.SetValue(returnValue);
        if (closedBy =="Material")
            ClientMaterial.SetValue(returnValue);
        if (closedBy =="PFLid")
            ClientPFLid.SetValue(returnValue);
        if (closedBy == "Lacquer")
            ClientLacquer.SetValue(returnValue);
        if (closedBy == "PFType")
            ClientPFType.SetValue(returnValue);
        if (closedBy == "Design")
            ClientDesign.SetValue(returnValue);
        if (closedBy == "Shape")
            ClientShape.SetValue(returnValue);
        //gv.SetEditValue('RequestNo', returnValue);
        if (closedBy == "BCDL" || closedBy == "Semi" ||closedBy == "Packaging") 
            gvformu.PerformCallback("Insert|" + returnValue +"|"+ closedBy);
        if (closedBy == "PackingStyle")
            PackingStyleComboBox.SetValue(returnValue);
        if (closedBy == "SelMaterial") {
            gvformu.GetValuesOnCustomCallback("SelMaterial|" + returnValue + "|" + gvformu_visibleIndex, function (r) {
                if (!r)
                    return;
                gvformu.batchEditApi.SetCellValue(gvformu_visibleIndex, "Material", r["Material"]);
                gvformu.batchEditApi.SetCellValue(gvformu_visibleIndex, "Name", r["Name"]);
                gvformu.batchEditApi.SetCellValue(gvformu_visibleIndex, "PriceOfUnit", r["Price"]);
                gvformu.batchEditApi.SetCellValue(gvformu_visibleIndex, "AdjustPrice", r["Price"]);
            })
        }   
    }
    function updategridview(s) {
        window.setTimeout(function () {
            var count_value = Demo.GetChangesCount(s.batchEditApi);
            if (count_value > 0)
                s.UpdateEdit();
        }, 0);
    }
    function resetpkg() {
        ClientPFType.SetText('-');
        ClientDesign.SetText('-');
        ClientPFLid.SetText('-');
        ClientLacquer.SetText('-');
        ClientColor.SetText('-');
        ClientMaterial.SetText('-');
        ClientShape.SetText('-');
    }
    var update_name;
    function OnToolbarItemClick(s, e) {
        if (e.item.name == "Remove") {
            if (confirm('Confirm: delete all items?')) {
                gv.PerformCallback("Remove|0");
            } else
                gv.PerformCallback("Delete|0");
        }
        if (e.item.name == "Upload") {
            //pcuploadformula.SetContentUrl(assignurl);
            //pcuploadformula.SetHeaderText(assignurl);
            pcuploadformula.Show();
            pcuploadformula.BringToFront();
        }
    }
      function ClearSelection() {
        TreeList.SetFocusedNodeKey("");
        UpdateControls(null, "");
    }
    function UpdateSelection() {
        var employeeName = "";
        var focusedNodeKey = TreeList.GetFocusedNodeKey();
        if (focusedNodeKey != "")
            employeeName = TreeList.cpEmployeeNames[focusedNodeKey];
        UpdateControls(focusedNodeKey, employeeName);
    }
    function UpdateControls(key, text) {
        DropDownEdit.SetText(text);
        DropDownEdit.SetKeyValue(key);
        DropDownEdit.HideDropDown();
        UpdateButtons();
    }
    function UpdateButtons() {
        clearButton.SetEnabled(DropDownEdit.GetText() != "");
        selectButton.SetEnabled(TreeList.GetFocusedNodeKey() != "");
    }
    function OnDropDown() {
        TreeList.SetFocusedNodeKey(DropDownEdit.GetKeyValue());
        TreeList.MakeNodeVisible(TreeList.GetFocusedNodeKey());
    }
    function ButtonClickHandler(s, e) {
        if (e.buttonIndex == 0) {
            DropDownEdit.SetKeyValue(null);
            SynchronizeFocusedNode();
        }
    }
    function DropDownHandler(s, e) {
        SynchronizeFocusedNode();
    }
    function SynchronizeFocusedNode() {

    }
    function treeList_SelectionChanged(s, e) {
        //ListCheckedNodes(s)
        window.setTimeout(function () { s.PerformCustomDataCallback(''); }, 0) 
    }
    function treeList_CustomDataCallback(s, e) {
        DropDownEdit.SetText(e.result);
    }
    function Inner_SelectedIndexChanged(s, e) {
            debugger;
            //hfInner.Set('Inner', s.GetSelectedValues().join(';'));
    }
    function Outer_SelectedIndexChanged(s, e) {
        debugger;
        var values = s.GetSelectedValues();
        var items = s.GetSelectedItems();
        var texts = [];
        for (var i = 0; i < items.length; i++) {
            texts.push(items[i].text);
        }
        alert('LEAD|' + values + '|' + texts);
        //hfOuter.Set('Outer', s.GetSelectedValues().join(';'));
    }  
    //function OnBtnShowPopupClick(text) {
    //    PopupControl.RefreshContentUrl();
    //    PopupControl.SetContentUrl('popupControls/selectedvalue.aspx?Id=' + text + '&bu=' + ClientCompany.GetValue());
    //    PopupControl.SetHeaderText("param:" + text);
    //    PopupControl.Show();
    //}
    //function HidePopupAndShowInfo(closedBy, returnValue) {

    //    PopupControl.Hide();
    //    gvformu.batchEditApi.SetCellValue(gvformu_visibleIndex, "Material", returnValue);
    //    updategrid(gvformu);
    //}
    var gvformu_visibleIndex = 0;
    function gvformu_onStartEdit(s, e) {
        gvformu_visibleIndex = e.visibleIndex;
        keyValue = s.GetRowKey(e.visibleIndex);
        if (keyValue != null) {
            var targetArray = ["PriceOfUnit", "Diff", "PriceOfCarton"];
            if (targetArray.indexOf(e.focusedColumn.fieldName) > -1) {
                e.cancel = true;
            }
        }
    }
    function gvformu_OnEndEdit(s, e) {
        debugger;
        var code = s.batchEditApi.GetCellValue(gvformu_visibleIndex, 'Component');
        if (code != null) {
            //alert(ClientPage.GetValue());
            updategrid(s);
        }
    }
    function updategrid(s) {
        window.setTimeout(function () {
            var count_value = Demo.GetChangesCount(s.batchEditApi);
            if (count_value > 0)
                s.UpdateEdit();
        }, 0);

    }
    function ShowRowsCount() {
        labelInfo.SetText('rows count: ' + gvformu.GetSelectedRowCount());
    }
    function uploadStarted(s, e) {
        debugger;
        //var fileNames = e.fileNames.split(", ");
        var fileNames = e.fileNames;
        var existsFiles = fileManager.GetItems();
        var found = [];
        for (var i = 0; i < existsFiles.length; i++) {
            var ef = existsFiles[i];
            for (var j = 0; j < fileNames.length; j++) {
                if (ef.name.toLowerCase() == fileNames[j].toLowerCase())
                    found.push(ef.name);
            }
        }
        if (found.length > 0) {
            if (confirm(found.length + " file(s) with the same name exist(s). Are you sure you want to overwrite?")) {
                hffileManager.Set("overwrite", true);
                return
            }
            e.cancel = true;
        }
        hffileManager.Set("overwrite", false);
    }
    function lbLaborSelectedIndex(s, e) {

    }
    function OnDLButtonClick(s, e) {
        debugger;
        if (e.buttonID == 'customRecover') {

        }
        if (e.buttonID == "EditCost") {
            var key = s.GetRowKey(e.visibleIndex, "ID");
            s.PerformCallback('del|' + key);
        }
    }
    function OnDLToolbarItemClick(s, e) {
        debugger;
        if (e.item.name == "BCDL" || e.item.name == "Semi" || e.item.name == "Packaging") {
            OnButtonClick(e.item.name); 
        }
    }
    function OnToolbarItemClick_ordersGridView(s, e) {
        if (e.item.name == "AddNew") {
            employeeEditPopup.Show();
        }
    }
    function employeeCancelButton_Click(s, e) {
        employeeEditPopup.Hide();
    }
    function gridEditButton_Click(e) {
        ASPxPopupControl2.Show();
    }
    function employeesGrid_FocusedRowChanged(s, e) {
        debugger;
        var kk = s.GetRowKey(e.visibleIndex);
        updateDetailInfo(kk);
    }
    function updateDetailInfo(kk) {
        gv_detail.PerformCallback(['reload', kk].join('|'));
    }
    function employeeSaveButton_Click(s, e) {
        employeeEditPopup.Hide();
        Demo.DoCallback(ordersGridView, function () {
            ordersGridView.PerformCallback('save');
        });
    }
    function clearDataLabor(s, e) {
        //TextBoxUpCharge.SetText('');
        //TextBoxUnit.SetText('');
        //tbUpChargePrice.SetText('');
    }
    var keyValue;
    function OnMoreInfoClick(element, key) {
            callbackPanel.SetContentHtml("");
            popup.ShowAtElement(element);
            keyValue = key;
        }
        function popup_Shown(s, e) {
            callbackPanel.PerformCallback(keyValue);
        }
</script>
 <dx:ASPxHiddenField ID="hfStatusApp" runat="server" ClientInstanceName="hfStatusApp" />
 <dx:ASPxHiddenField ID="hfgetvalue" runat="server" ClientInstanceName="ClientNewID" />
 <dx:ASPxHiddenField ID="hfGeID" runat="server" ClientInstanceName="hfGeID" />
 <dx:ASPxHiddenField ID="hfuser" runat="server" ClientInstanceName="username"/>
 <dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
 <dx:ASPxHiddenField ID="approv" runat="server" ClientInstanceName="approv" />
 <dx:ASPxHiddenField ID="hfpara" runat="server" ClientInstanceName="hfpara" />
 <dx:ASPxHiddenField ID="hftype" runat="server" ClientInstanceName="hftype" />
 <dx:ASPxHiddenField ID="usertp" runat="server" ClientInstanceName="usertp" />
 <dx:ASPxHiddenField ID="Clientusertp" runat="server" ClientInstanceName="Clientusertp" />
 <dx:ASPxHiddenField ID="hfBU" runat="server" ClientInstanceName="ClientBU" />
 <dx:ASPxHiddenField ID="hfview" runat="server" ClientInstanceName="hfview"/>
 <dx:ASPxHiddenField ID="hfRole" runat="server" ClientInstanceName="hfRole"/>
 <dx:ASPxHiddenField ID="hfKeyword" runat="server" ClientInstanceName="hfKeyword"/>
 <dx:ASPxHiddenField ID="hftablename" runat="server" ClientInstanceName="hftablename" />
 <dx:ASPxHiddenField ID="hfRequestType" runat="server" ClientInstanceName="hfRequestType" />
 <dx:ASPxHiddenField ID="hfCustomerRe" runat="server" ClientInstanceName="hfCustomerRe" />
 <dx:ASPxHiddenField ID="hfPrdRequirement" runat="server" ClientInstanceName="hfPrdRequirement" />
 <dx:ASPxHiddenField ID="hfIngredient" runat="server" ClientInstanceName="hfIngredient" />
 <dx:ASPxHiddenField ID="hfReceiver" runat="server" ClientInstanceName="hfReceiver" />
 <dx:ASPxHiddenField ID="hfSellingUnit" runat="server" ClientInstanceName="hfSellingUnit" />
 <dx:ASPxHiddenField ID="hffileManager" ClientInstanceName="hffileManager" runat="server"/>
 <%--<dx:ASPxHiddenField ID="hfInner" runat="server" ClientInstanceName="hfInner" />
 <dx:ASPxHiddenField ID="hfOuter" runat="server" ClientInstanceName="hfOuter" />--%>
 <dx:ASPxGridView runat="server" ID="gridData" KeyFieldName="ID;RequestType;StatusApp;UserType" DataSourceID="dsgv" ClientInstanceName="ClientgridData" Width="100%"  
        AutoGenerateColumns="true" OnCustomCallback="gridData_CustomCallback" OnCustomButtonCallback="gridData_CustomButtonCallback"
        OnCustomDataCallback="gridData_CustomDataCallback"
	    OnBeforeGetCallbackResult="BeforeGetCallbackResult" OnPreRender="PreRender" 
        OnContextMenuItemClick="gridData_ContextMenuItemClick" OnFillContextMenuItems="gridData_FillContextMenuItems"
        OnCustomButtonInitialize="gridData_CustomButtonInitialize"
        Border-BorderWidth="0">
        <Paddings PaddingTop="0px" Padding="1px" PaddingBottom="0px" PaddingLeft="0px" />
        <Columns>
        <%--<dx:GridViewCommandColumn ShowNewButton="true" ShowEditButton="true" VisibleIndex="0" ButtonRenderMode="Image" Width="45" Visible="false">
            <CustomButtons>
                <dx:GridViewCommandColumnCustomButton ID="Clone">
                    <Image ToolTip="Clone Record" Url="~/Content/Images/Copy.png"/>
                </dx:GridViewCommandColumnCustomButton>
                <dx:GridViewCommandColumnCustomButton ID="btnDetails" Text="Details">
                    <Image ToolTip="Clone Record" Url="~/Content/Images/Copy.png"/>
                </dx:GridViewCommandColumnCustomButton>
            </CustomButtons>
        </dx:GridViewCommandColumn>--%>
	<dx:GridViewDataColumn FieldName="UniqueColumn" Visible="false"/>
        <dx:GridViewDataColumn FieldName="RequestNo" Caption="Request No" VisibleIndex="1" Width="8%"/>
        <dx:GridViewDataTextColumn FieldName="Revised" Caption="Revised" VisibleIndex="2"/>
        <dx:GridViewDataTextColumn FieldName="Company" VisibleIndex="3" Caption="Proposed Factory">
            <PropertiesTextEdit >
            <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" />
        </PropertiesTextEdit>
        </dx:GridViewDataTextColumn>
        
        <dx:GridViewDataColumn FieldName="StatusAppTitle" Caption="StatusApp" GroupIndex="0"/>
<%--    <dx:GridViewDataTextColumn FieldName="Plant"  Width="45"/>   
        <dx:GridViewDataComboBoxColumn FieldName="StatusApp" GroupIndex="0">
            <PropertiesComboBox DataSourceID="dsStatusApp" TextField="Title" ValueField="ID" />
        </dx:GridViewDataComboBoxColumn>--%>
        <dx:GridViewDataTextColumn FieldName="Customer" Caption="Customer" />
        <dx:GridViewDataTextColumn FieldName="Brand" />
        <dx:GridViewDataTextColumn FieldName="RequestType"/>
        <dx:GridViewDataComboBoxColumn FieldName="UserType" Caption="Type">
            <PropertiesComboBox DataSourceID="dstype" TextField="Name" ValueField="ID"/>
        </dx:GridViewDataComboBoxColumn> 
 
        <dx:GridViewDataTextColumn FieldName="PackSize" Caption="PackSize" />
        <dx:GridViewDataComboBoxColumn FieldName="PetCategory" Caption="Product Category">
            <PropertiesComboBox DataSourceID="dsPetCategory" TextField="Name" ValueField="ID" /> 
        </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataTextColumn FieldName="RequesterName"/>
        <dx:GridViewDataDateColumn FieldName="CreateOn" Caption="CreateOn"> 
            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
            </PropertiesDateEdit>
        </dx:GridViewDataDateColumn>
                <dx:GridViewDataTokenBoxColumn FieldName="Assignee">
            <PropertiesTokenBox DataSourceID="dsuser" ValueField="user_name" TextField="fn"/>
	    </dx:GridViewDataTokenBoxColumn>
        <dx:GridViewDataTextColumn FieldName="ReceiverName"/>
        <%--<dx:GridViewDataColumn Caption="More Info">
            <DataItemTemplate>
                <dx:ASPxImage ID="InformationImg" runat="server" ImageUrl="~/Content/images/MathIcons/3DPlot.png" ClientInstanceName="InformationImg" Cursor="help" />
            </DataItemTemplate>
          </dx:GridViewDataColumn>--%>
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
    <dx:ASPxPopupControl ID="popup" ClientInstanceName="popup" runat="server" AllowDragging="true"
        PopupHorizontalAlign="OutsideRight" HeaderText="Details">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxCallbackPanel ID="callbackPanel" ClientInstanceName="callbackPanel" runat="server"
                    Width="320px" Height="100px" OnCallback="callbackPanel_Callback" RenderMode="Table">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                            <table class="InfoTable">
                                <tr>
                                    <td>
                                        <dx:ASPxBinaryImage ID="edBinaryImage" runat="server" AlternateText="Loading..." ImageAlign="Left" CssClass="Image" />
                                    </td>
                                    <td>
                                        <asp:Literal ID="litText" runat="server" Text="" />
                                    </td>
                                </tr>
                            </table>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchAtWindowInnerWidth="800" />
        <ClientSideEvents Shown="popup_Shown" />
    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl ID="LaborPopup" ClientInstanceName="LaborPopup" runat="server" 
    Width="700px" Height="240px" AllowResize="true" ShowFooter="true"
    Modal="true" CloseAction="CloseButton" PopupHorizontalAlign="Center" PopupVerticalAlign="Middle"
    AllowDragging="True">
    <ClientSideEvents Shown="clearDataLabor" />
    <ContentCollection>
        <dx:PopupControlContentControl runat="server">
            <dx:ASPxPageControl ID="citiesTabPage" runat="server" CssClass="dxtcFixed" ActiveTabIndex="0" EnableHierarchyRecreation="True"
                Border-BorderStyle="None" Width="100%">
                <TabPages>
                    <dx:TabPage Text="List">
                        <ContentCollection>
                            <dx:ContentControl ID="ContentControl1" runat="server">
                               <dx:ASPxListBox ID="lbLabor" runat="server" SelectionMode="Single" Width="485" Height="210px"
                                        ClientInstanceName="lbLabor" DataSourceID="dsLabor" ValueField="Id" 
                                        ValueType="System.String">
                                        <ClientSideEvents SelectedIndexChanged="lbLaborSelectedIndex" />
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
                                <%--<dx:ASPxLabel ID="ASPxLabel5" runat="server" Text="UpCharge" />
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
                                        </dx:ASPxButton>--%>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                </TabPages>
            </dx:ASPxPageControl>
        </dx:PopupControlContentControl>
    </ContentCollection>
    </dx:ASPxPopupControl>
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
            Width="880px" Height="380px"
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
        <dx:ASPxPopupControl ClientInstanceName="pcuploadformula" ShowCloseButton ="true" AllowDragging="True"
            ShowFooter="True" PopupAction="None" CloseAction="OuterMouseClick" CloseOnEscape="true" PopupHorizontalAlign="OutsideRight"
            PopupVerticalAlign="Below" AllowResize="true" 
            ID="pcuploadformula" runat="server" PopupHorizontalOffset="34" PopupVerticalOffset="2" 
            Width="680px" Height="380px"
            EnableHierarchyRecreation="True">
            <Border BorderColor="#7EACB1" BorderStyle="Solid" BorderWidth="1px" />
            <ContentCollection>
                <dx:PopupControlContentControl runat="server" SupportsDisabledAttribute="True">
                    <%--<dx:ASPxUploadControl ID="Upformula" runat="server" Width="200" ClientInstanceName="Upformula" 
                                    NullText="Click here to browse files..." UploadMode="Advanced" FileUploadMode="OnPageLoad" AutoStartUpload="True">
                                    <ValidationSettings AllowedFileExtensions=".xls,.xlsx">
                                    </ValidationSettings>
                                    <ClientSideEvents FileUploadComplete="UpCode_FileUploadComplete" />
                                </dx:ASPxUploadControl>--%>
                    <dx:ASPxGridView ID="gvformula" runat="server" />
                </dx:PopupControlContentControl>
            </ContentCollection>
    </dx:ASPxPopupControl>  
    <dx:ASPxPopupControl ID="PopupControl" runat="server" ClientInstanceName="PopupControl" CloseAction="OuterMouseClick" CloseOnEscape="true" 
            Width="720px" Height="500px" AllowResize="true" Modal="True"
            HeaderText="Results" AllowDragging="True" PopupAnimationType="Fade"
            EnableViewState="False">
        <ClientSideEvents PopUp="function(s, e) { ASPxClientEdit.ClearGroup('createAccountGroup'); }" />
        <SizeGripImage Width="11px" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxPanel ID="ASPxPanel1" runat="server" DefaultButton="btCreate">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl ID="ASPxPopupControl2" ClientInstanceName="ASPxPopupControl2" runat="server" PopupHorizontalAlign="WindowCenter" ShowCloseButton="false" CloseOnEscape="true"
    PopupVerticalAlign="WindowCenter" CloseAction="None" Modal="true" PopupAnimationType="Fade" CssClass="emplEditFormPopup">
    <ContentCollection>
        <dx:PopupControlContentControl runat="server">
        
        <dx:ASPxFormLayout ID="ASPxFormLayout1" runat="server" AlignItemCaptionsInAllGroups="True">
                <Styles>
                    <LayoutGroup CssClass="group"></LayoutGroup>
                </Styles>
                <Items>
                    <dx:LayoutGroup ColCount="2" GroupBoxDecoration="None">
                        <Items>
                            <dx:LayoutItem Caption="Description">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" ItemStyle-Width="30" />
                <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-Width="150" />
                <asp:BoundField DataField="Country" HeaderText="Country" ItemStyle-Width="150" />
            </Columns>
            </asp:GridView>
            <br />
            <asp:TextBox ID="txtCopied" runat="server" TextMode="MultiLine" AutoPostBack="true"
            OnTextChanged="PasteToGridView" Height="200" Width="400" /> 
                                        
                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                
                                        </dx:LayoutItem>
                            
                        </Items>
                  </dx:LayoutGroup>
                    </Items>                
                    </dx:ASPxFormLayout>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl ID="EmployeeEditPopup" ClientInstanceName="employeeEditPopup" runat="server" PopupHorizontalAlign="WindowCenter" ShowCloseButton="false" CloseOnEscape="true"
    PopupVerticalAlign="WindowCenter" CloseAction="None" Modal="true" PopupAnimationType="Fade" CssClass="emplEditFormPopup">
    <ContentCollection>
        <dx:PopupControlContentControl runat="server">
            <dx:ASPxFormLayout ID="EmployeeEditFormLayout" runat="server" AlignItemCaptionsInAllGroups="True">
                <Styles>
                    <LayoutGroup CssClass="group"></LayoutGroup>
                </Styles>
                <Items>
                    <dx:LayoutGroup ColCount="2" GroupBoxDecoration="None">
                        <Items>
                            <dx:LayoutItem Caption="Packing Description">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxMemo ID="PackingTextBox" ClientInstanceName="PackingTextBox" runat="server" Width="100%" Native="True" MaxLength="30">
                                        </dx:ASPxMemo>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="OD">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="ODTextBox" runat="server" Width="250px" MaxLength="30">
                                            <ValidationSettings ErrorDisplayMode="ImageWithTooltip">
                                            </ValidationSettings>
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="PackingStyle">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxButtonEdit ID="PackingStyleComboBox" ClientInstanceName="PackingStyleComboBox" runat="server" Width="250px">
                                            <Buttons>
                                            <dx:EditButton>
                                            </dx:EditButton>
                                        </Buttons>
                                        <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('PackingStyle'); }" />
                                        </dx:ASPxButtonEdit>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <%--<dx:LayoutItem Caption="Title">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="TitleTextBox" runat="server" Width="250px" MaxLength="30">
                                            <ValidationSettings ErrorDisplayMode="ImageWithTooltip">
                                                <RequiredField IsRequired="True" ErrorText="Title is required" />
                                            </ValidationSettings>
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>--%>
                        </Items>
                    </dx:LayoutGroup>
                    <dx:LayoutGroup GroupBoxDecoration="None" ShowCaption="False" CssClass="addressGroup">
                        <Items>
                            
                            <%--<dx:LayoutItem Caption=" " RequiredMarkDisplayMode="Hidden">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <table class="fullWidthTable">
                                            <tr>
                                                <td>
                                                    <dx:ASPxTextBox ID="CityTextBox" runat="server" NullText="City" NullTextDisplayMode="UnfocusedAndFocused" Width="300px" MaxLength="30">
                                                        <ValidationSettings ErrorDisplayMode="ImageWithTooltip">
                                                            <RequiredField IsRequired="True" ErrorText="City is required" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </td>
                                                <td>
                                                    <dx:ASPxComboBox ID="StateComboBox" runat="server" NullText="State" NullTextDisplayMode="UnfocusedAndFocused" Width="100px" IncrementalFilteringMode="StartsWith">
                                                        <ValidationSettings ErrorDisplayMode="ImageWithTooltip">
                                                            <RequiredField IsRequired="True" ErrorText="State is required" />
                                                        </ValidationSettings>
                                                    </dx:ASPxComboBox>
                                                </td>
                                                <td class="fullWidthCell">
                                                    <dx:ASPxTextBox ID="ZipCodeTextBox" runat="server" NullText="Zipcode" Width="100%">
                                                        <MaskSettings Mask="00000" ErrorText="Invalid zipcode, format: #####" />
                                                        <ValidationSettings ErrorDisplayMode="ImageWithTooltip" />
                                                    </dx:ASPxTextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>--%>
                        </Items>
                    </dx:LayoutGroup>
                    <%--<dx:LayoutGroup ColCount="2" GroupBoxDecoration="None" ShowCaption="False">
                        <Items>
                            <dx:LayoutItem Caption="Home">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="HomeNumberTextBox" runat="server" Width="250px">
                                            <MaskSettings Mask="(999) 000-0000" ErrorText="Invalid number, format example: (818) 844-3383"  />
                                            <ValidationSettings ErrorDisplayMode="ImageWithTooltip">
                                                <RequiredField IsRequired="True" ErrorText="Home number is required" />
                                            </ValidationSettings>
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Email">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="EmailTextBox" runat="server" Width="250px" MaxLength="30">
                                            <ValidationSettings ErrorDisplayMode="ImageWithTooltip">
                                                <RequiredField IsRequired="True" ErrorText="Email is required" />
                                                <RegularExpression ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.[a-zA-Z]+([-.][a-zA-Z]+)*" ErrorText="Invalid email, format example: info@devexpress.com" />
                                            </ValidationSettings>
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Mobile">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="MobileNumberTextBox" runat="server" Width="250px">
                                            <MaskSettings Mask="(999) 000-0000" ErrorText="Invalid number, format example: (818) 844-3383"  />
                                            <ValidationSettings ErrorDisplayMode="ImageWithTooltip">
                                                <RequiredField IsRequired="True" ErrorText="Mobile number is required" />
                                            </ValidationSettings>
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Skype">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="SkypeTextBox" runat="server" Width="250px" MaxLength="30">
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                        </Items>
                    </dx:LayoutGroup>
                    <dx:LayoutGroup ColCount="2" GroupBoxDecoration="None" ShowCaption="False">
                        <Items>
                            <dx:LayoutItem Caption="Hire Date">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxDateEdit ID="HireDateEdit" runat="server" Width="250px">
                                            <ValidationSettings ErrorDisplayMode="ImageWithTooltip">
                                                <RequiredField IsRequired="True" ErrorText="Hire date is required" />
                                            </ValidationSettings>
                                        </dx:ASPxDateEdit>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Dept.">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxComboBox ID="DeptComboBox" runat="server" Width="250px">
                                            <ValidationSettings ErrorDisplayMode="ImageWithTooltip">
                                                <RequiredField IsRequired="True" ErrorText="Department is required" />
                                            </ValidationSettings>
                                        </dx:ASPxComboBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                        </Items>
                    </dx:LayoutGroup>--%>
                </Items>
            </dx:ASPxFormLayout>
            <div class="buttonsContainer">
                <dx:ASPxButton ID="EmployeeSaveButton" runat="server" AutoPostBack="false" Text="Save" Width="100px">
                    <ClientSideEvents Click="employeeSaveButton_Click" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="EmployeeCancelButton" runat="server" AutoPostBack="False" UseSubmitBehavior="False" Text="Cancel" Width="100px">
                    <ClientSideEvents Click="employeeCancelButton_Click" />
                </dx:ASPxButton>
            </div>
            <div style="clear: both">
            </div>
        </dx:PopupControlContentControl>
    </ContentCollection>
</dx:ASPxPopupControl>

    <dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" ClientVisible="false" ScrollBars="Vertical" ClientInstanceName="ClientFormPanel">
        <ClientSideEvents Init="Demo.Init"/>
        <PanelCollection>
            <dx:PanelContent>  
            <dx:ASPxFormLayout runat="server" ID="formLayout" ClientInstanceName="formLayout" CssClass="formLayout" AlignItemCaptionsInAllGroups="true">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
            <Items>
                <dx:LayoutGroup Caption="Marketing Technical Request Form" Name="layoutname" ColCount="4" GroupBoxDecoration="HeadingLine" UseDefaultPaddings="false" Paddings-PaddingTop="10">
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
                                    <dx:ASPxTextBox runat="server" ID="tbRequestNo" ClientInstanceName="ClientRequestNo" 
                                        ReadOnly="true">
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Proposed Factory" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="cmbCompany" runat="server" ValueField="Code" TextField="Title" DataSourceID="dsPlant" EnableClientSideAPI="true" 
                                        TextFormatString="{0}; {1}" ClientInstanceName="ClientCompany">
                                        <Columns>
                                            <dx:ListBoxColumn FieldName="Code" />
                                            <dx:ListBoxColumn FieldName="Title" />
                                            <dx:ListBoxColumn FieldName="Company"/>
                                        </Columns>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { combo_SelectedIndexChanged(s); }" />
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>                          
                        <dx:LayoutItem Caption="Valid From" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxDateEdit runat="server" ID="defrom" ClientInstanceName="ClientValidfrom" DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
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
                                 <dx:ASPxDateEdit runat="server" ID="deto" ClientInstanceName="ClientValidto" DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                            </dx:ASPxDateEdit> 
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Request Type" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxDropDownEdit ClientInstanceName="ClientRequestType" ID="cbRequestType" runat="server" AnimationType="Auto">
                                        <DropDownWindowStyle BackColor="#EDEDED" />
                                        <DropDownWindowTemplate>
                                            <dx:ASPxListBox Width="100%" Height="200px" ID="listBox" ClientInstanceName="cbRequestType" SelectionMode="CheckColumn" DataSourceID="dsRequestType" 
                                                OnCallback="listBox_Callback" 
                                                TextField="Name" ValueField="ID" 
                                                runat="server">
                                                <Border BorderStyle="None" />
                                                <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />                                      
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { OnListBoxSelectionChanged(s, e, '1');}" />
                                            </dx:ASPxListBox>
                                        </DropDownWindowTemplate>
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                        <ClientSideEvents TextChanged="function(s, e) { SynchronizeListBoxValues(s, e, '1');}" DropDown="function(s, e) { SynchronizeListBoxValues(s, e, '1');}" />
                                </dx:ASPxDropDownEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Pet Category" Name="PetCategory" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ClientInstanceName="ClientPetCategory" ID="CmbPetCategory" runat="server" DropDownWidth="340px" 
                                        TextFormatString="{0}" OnCallback="CmbPetCategory_Callback" EnableClientSideAPI="true" ValueField="ID" TextField="Name">
                                        <Columns>
                                            <dx:ListBoxColumn FieldName="Name" Width="90%" />
                                            <dx:ListBoxColumn FieldName="usertype" ClientVisible="false"/>
                                        </Columns>
                                        <ClientSideEvents Validation="OnValidation" SelectedIndexChanged="CateSelectedIndexChanged" />
                                        <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" ValidationGroup="group1"/>                                            
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Pet Food Type" Name="PetFoodType" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ClientInstanceName="ClientPetFoodType" ID="CmbPetFoodType" runat="server" OnCallback="CmbPetFoodType_Callback" 
                                        DataSourceID="dsPetFoodType" ValueField="ID" TextField="Name" >
                                        <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" ValidationGroup="group1"/>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Complied With" Name="CompliedWith" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ClientInstanceName="ClientCompliedWith" ID="CmbCompliedWith" runat="server" DataSourceID="dsCompliedWith" OnCallback="CmbCompliedWith_Callback"  
                                         ValueField="ID" TextField="Name">
                                        <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" ValidationGroup="group1"/>
                                    </dx:ASPxComboBox>
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
                                    <dx:ASPxDateEdit runat="server" ID="deRequestDate" ClientInstanceName="ClientRequestDate" 
                                        DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                        <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" ValidationGroup="group1" />
                                                                                <ClientSideEvents DateChanged="ChangeDateTo" />
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Nutrient Profile" Name="NutrientProfile" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ClientInstanceName="ClientNutrientProfile" ID="CmbNutrientProfile" runat="server"   
                                         DataSourceID="dsNutrientProfile"
                                        ValueField="ID" TextField="Name">                                    
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Sample Required Date" Name="SampleDate" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxDateEdit runat="server" ID="deSampleDate" ClientInstanceName="deSampleDate"
                                        DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                        <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" ValidationGroup="group1" />
                                    </dx:ASPxDateEdit>
                                <div id="divprofile" runat="server" style="display:none" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="R&D" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <%--<dx:ASPxComboBox ID="CmbReceiver" ClientInstanceName="ClientReceiver" runat="server" 
                                       OnCallback="CmbReceiver_Callback" ValueField="user_name" TextField="fn">
                                        <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" ValidationGroup="group1"/>
                                        <ClientSideEvents SelectedIndexChanged="RecSelectedIndexChanged" />
                                    </dx:ASPxComboBox>--%>
                                    <dx:ASPxButtonEdit ID="CmbReceiver" ClientInstanceName="ClientReceiver" Width="100%" ReadOnly="true" runat="server">
                                        <Buttons>
                                            <dx:EditButton>
                                            </dx:EditButton>
                                        </Buttons>
                                        <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Receiver'); }" />
                                    </dx:ASPxButtonEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Notes" ColSpan="4" Width="100%" VerticalAlign="Middle">
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
                <dx:LayoutItem Caption="">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer>
                            <dx:ASPxTabControl ID="TabList" runat="server" NameField="Id" DataSourceID="XmlDataSource1" ActiveTabIndex="0" ClientInstanceName="TabList">
                               <ClientSideEvents ActiveTabChanged="OnActiveTabChanged" Init="OnTabbedGroupPageControlInit"/>
                            </dx:ASPxTabControl>
                            <asp:XmlDataSource ID="XmlDataSource1" runat="server" 
                                ></asp:XmlDataSource>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>   
                
                   <dx:TabbedLayoutGroup Caption="" ActiveTabIndex="0" ClientInstanceName="tabbedGroupPageControl" ShowGroupDecoration="false" Width="100%">   
                       <Items>
                       <dx:LayoutGroup Caption="Customer Details" ColCount="3" >
                       <GridSettings StretchLastItem="true" WrapCaptionAtWidth="660">
                       <Breakpoints>
                            <dx:LayoutBreakpoint MaxWidth="500" ColumnCount="1" Name="S" />
                            <dx:LayoutBreakpoint MaxWidth="800" ColumnCount="2" Name="M" />
                       </Breakpoints>
                       </GridSettings>
                       <Items>
                          <dx:LayoutItem Caption="Customer Price" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxComboBox ID="CmbCustomerPrice" runat="server" DataSourceID="dsCustomerPrice" TextField="Name" 
                                           ValueField="ID" OnCallback="CmbCustomerPrice_Callback" ClientInstanceName="ClientCustomerPrice">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxComboBox>
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

                          <dx:LayoutItem Caption="Customer Name" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbCustomerName" runat="server" ClientInstanceName="ClientCustomerName">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>     
                           <dx:LayoutItem Caption="Brand Name" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbBrandName" runat="server" ClientInstanceName="ClientBrandName">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxTextBox>
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
                                       <dx:ASPxTextBox ID="tbDestination" runat="server" ClientInstanceName="ClientDestination">
                                           <%--<ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>--%>
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>    
                        <dx:LayoutItem Caption="Country Regulation" VerticalAlign="Middle">
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
                      <dx:LayoutItem Caption="EST.Volume(FCL/YR)" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbESTVolume" runat="server" ClientInstanceName="ClientESTVolume">
                                           <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                           <dx:LayoutItem Caption="EST.Launching  Date" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbESTLaunching" runat="server" ClientInstanceName="ClientESTLaunching">
                                           <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>                 
                           <dx:LayoutItem Caption="EST.FOB Target Price ($/CASE)" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbESTFOB" runat="server" ClientInstanceName="ClientESTFOB">
                                           <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                           
                           <dx:LayoutItem Width="100%" Caption="Customer Request" CaptionStyle-Font-Bold="true" VerticalAlign="Middle">
			                <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                  <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                  <table>
                                    <tr>
                                      <td colspan="5">
                                   <dx:ASPxCallbackPanel ID="cpCustomerRole" runat="server" Width="100%" ClientInstanceName="cpCustomerRole" OnCallback="cpCustomerRole_Callback" >
                                  <PanelCollection>
                                  <dx:PanelContent runat="server">
                                       <dx:ASPxCheckBoxList ID="cbCustomer" runat="server" DataSourceID="dsCustomerRequest" ValueField="ID" TextField="Name" RepeatDirection="Horizontal"
                                           ClientInstanceName="ClientCustomer" RepeatColumns="10" RepeatLayout="Table" Border-BorderWidth="0">
                                            <CaptionSettings Position="Top" />
					                        <ClientSideEvents SelectedIndexChanged="function(s, e){ 
                                                hfCustomerRe.Set('CustomerRe',s.GetSelectedValues().join(';'));}" />
                                        </dx:ASPxCheckBoxList>
                                      </dx:PanelContent></PanelCollection>
                                    </dx:ASPxCallbackPanel>
                                        </td><td>
                                      <dx:ASPxTextBox ID="tbOtherRole" runat="server" ClientInstanceName="tbOtherRole" /></td>
                                        </tr>
                                        <tr id="Vat" style="display:none">
                                            <td colspan="4">
                                            <dx:ASPxCheckBoxList ID="cbVet" runat="server" ClientInstanceName="ClientVet" 
                                            RepeatColumns="4" RepeatLayout="Table" Border-BorderWidth="0">
                                            <CaptionSettings Position="Top" />
                                            <Items>
                                                <dx:ListEditItem Text="Vet.Certificate" Value="0" />
                                            </Items>
                                            </dx:ASPxCheckBoxList></td>
                                         <td>etc.</td>
                                         <td><dx:ASPxTextBox ID="tbetc" runat="server" ClientInstanceName="Clientetc"/></td>
                                        </tr>
                                       </table>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>    
			                <dx:LayoutItem Caption="" VerticalAlign="Middle">
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <table><tr id="Physical">
                                        <td>
                                        <dx:ASPxCheckBoxList ID="cbPhysical" runat="server" ClientInstanceName="ClientPhysical" 
                                            RepeatColumns="4" RepeatLayout="Table" Border-BorderWidth="0">
                                            <CaptionSettings Position="Top" />
                                            <Items>
                                                <dx:ListEditItem Text="Physical Sample Needed" Value="0" />
                                            </Items>
                                        </dx:ASPxCheckBoxList></td>
                                        <td> <dx:ASPxTextBox ID="tbPhysicalUnit" Width="60px" runat="server" ClientInstanceName="ClientPhysicalUnit">
                                             </dx:ASPxTextBox></td>
                                        <td>&nbsp;Unit</td>
                                        <td>
                                        <dx:ASPxCheckBoxList ID="cbSample" runat="server" ClientInstanceName="ClientSample" 
                                            RepeatColumns="4" RepeatLayout="Table" Border-BorderWidth="0">
                                            <CaptionSettings Position="Top" />
                                            <Items>
                                                <dx:ListEditItem Text="Sample for Customer" Value="0" />
                                            </Items>
                                        </dx:ASPxCheckBoxList></td>
                                        <td> <dx:ASPxTextBox ID="tbSampleUnit" ClientInstanceName="ClientSampleUnit" Width="60px" runat="server">
                                             </dx:ASPxTextBox></td>
                                        <td>&nbsp;Unit</td>
                                        </tr>
                                        </table>
                                       </dx:LayoutItemNestedControlContainer>
                                   </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                       </Items>
                       </dx:LayoutGroup>
                       <dx:LayoutGroup Caption="Product Detail" ColCount="3">
                        <GridSettings StretchLastItem="true" WrapCaptionAtWidth="660">
                            <Breakpoints>
                                <dx:LayoutBreakpoint MaxWidth="500" ColumnCount="1" Name="S" />
                                <dx:LayoutBreakpoint MaxWidth="800" ColumnCount="2" Name="M" />
                            </Breakpoints>
                        </GridSettings>
                       <Items>     
                          <dx:LayoutItem Caption="Product Type" Name="Product_Type" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxComboBox ID="CmbProductType" runat="server" DataSourceID="dsProductType" ValueField="ID" TextField="Name" 
                                           IncrementalFilteringMode="Contains" ClientInstanceName="ClientProductType">
                                           <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxComboBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>     
                           
                           <dx:LayoutItem Caption="Chunk Type" Name="Chunk_Type" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <table>
                                           <tr>
                                               <td>
                                               <dx:ASPxComboBox ID="CmbChunkType" runat="server" DataSourceID="dsChunkType" ValueField="ID" TextField="Name"  
                                                  ClientInstanceName="ClientChunkType">
                                                <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                               </dx:ASPxComboBox>
                                               </td>
                                               <td>&nbsp;etc.</td>
                                               <td>
                                                <dx:ASPxTextBox ID="tbChunkType" runat="server" ClientInstanceName="ClientChunkTypeNote"/></td>
                                           </tr>
                                       </table>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem> 
                           
                           <dx:LayoutItem Caption="Media" Name="Media_" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxComboBox ID="CmbMedia" runat="server" DataSourceID="dsMedia" ValueField="ID" TextField="Name"  
                                           ClientInstanceName="ClientMedia">
                                           <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxComboBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                        <dx:LayoutItem Caption="Product Style" Name="Product_Stlye" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                    <table>
                                           <tr>
                                               <td>
                                       <dx:ASPxComboBox ID="CmbProductStyle" runat="server" DataSourceID="dsProductStyle" ValueField="ID" TextField="Name"
                                          ClientInstanceName="ClientProductStyle">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                        </dx:ASPxComboBox></td>
                                               <td>&nbsp;etc.</td>
                                               <td>
                                        <dx:ASPxTextBox ID="tbProductNote" runat="server" ClientInstanceName="ClientProductNote"/></td>
                                           </tr>
                                       </table>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>  
                           <dx:LayoutItem Caption="Net weight" ColSpan="2" Name="Net_weight" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <table>
                                           <tr>
                                               <td><dx:ASPxTextBox ID="tbNetweight" runat="server" ClientInstanceName="ClientNetweight" Width="100px">
                                                   <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                                   <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
                                                   </dx:ASPxTextBox></td>
                                               <td>&nbsp;</td>
                                               <td><dx:ASPxComboBox ID="CmbNetUnit" runat="server" ClientInstanceName="ClientNetUnit" NullText="Select Unit..."  Width="100px">
                                                    <ClearButton DisplayMode="OnHover"></ClearButton>
                                                   <Items>
                                                      <dx:ListEditItem Text="Grams" Value="1" Selected="true"/>
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
                           <dx:LayoutItem Width="100%" ColumnSpan="3" Caption="Product Requirement" Name="Product_Requirement" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                  <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True"> 
                                    <table>
                                        <tr><td>
					                <dx:ASPxCallbackPanel ID="cpPrdRequirement" runat="server" Width="100%" ClientInstanceName="cpPrdRequirement" 
                                            OnCallback="cpPrdRequirement_Callback" >
                                          <PanelCollection>
                                          <dx:PanelContent runat="server">
                                       <dx:ASPxCheckBoxList ID="cbPrdRequirement" runat="server" DataSourceID="dsPrdRequirement" ValueField="Id" TextField="Name"
                                           ClientInstanceName="ClientPrdRequirement" RepeatColumns="10" RepeatLayout="Table" Border-BorderWidth="0">
                                            <CaptionSettings Position="Top" />
                                            <ClientSideEvents SelectedIndexChanged="function(s,e) { hfPrdRequirement.Set('PrdRequirement',s.GetSelectedValues().join(';'));}" />
                                        </dx:ASPxCheckBoxList>					
                                        </dx:PanelContent></PanelCollection>
                                            </dx:ASPxCallbackPanel></td><td>
                                        <dx:ASPxTextBox ID="tbOtherPrd" runat="server" ClientInstanceName="tbOtherPrd" /></td></tr></table>
                                          </dx:LayoutItemNestedControlContainer>
                                   </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
			                <dx:EmptyLayoutItem Width="100%" />
                            <dx:LayoutItem Caption="Selected" Name="Upload" Width="100%" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxUploadControl ID="UpCode" runat="server" Width="200" ClientInstanceName="UpCode" OnFileUploadComplete="UpCode_FileUploadComplete"
                                    NullText="Click here to browse files..." UploadMode="Advanced" FileUploadMode="OnPageLoad" AutoStartUpload="True">
                                    <ValidationSettings AllowedFileExtensions=".xls,.xlsx">
                                    </ValidationSettings>
                                    <ClientSideEvents FileUploadComplete="UpCode_FileUploadComplete" />
                                </dx:ASPxUploadControl>
                                </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="" Name="Product_List" Width="100%" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxGridView ID="gv" runat="server" ClientInstanceName="gv" OnCustomCallback="gv_CustomCallback" OnBatchUpdate="gv_BatchUpdate"
                                    OnDataBinding="gv_DataBinding" OnCustomButtonCallback="gv_CustomButtonCallback" 
                                    OnCustomButtonInitialize="gv_CustomButtonInitialize" KeyFieldName="Id">
                                    <Toolbars>
                                        <dx:GridViewToolbar Name="MyToolbar">
                                            <Items>
                                                <dx:GridViewToolbarItem Command="New" Text="New"/>
                                                <dx:GridViewToolbarItem Text="Remove" Name="Remove" Image-Url="~/Content/Images/Cancel.gif" />
                                                <dx:GridViewToolbarItem Text="Upload" Name="Upload" Image-Url="~/Content/Images/excel.gif" />
                                            </Items>
                                        </dx:GridViewToolbar> 
                                    </Toolbars>
                                    <Columns>
                                        <dx:GridViewCommandColumn ShowClearFilterButton="true" Width="75px" ButtonRenderMode="Image"
                                            FixedStyle="Left" ShowNewButtonInHeader="true" ShowEditButton="true" ShowDeleteButton="true">
                                            <CustomButtons>
                                                <dx:GridViewCommandColumnCustomButton ID="Edit">
                                                    <Image ToolTip="Remove" Url="~/Content/Images/Cancel.gif"/>
                                                </dx:GridViewCommandColumnCustomButton>
                                            </CustomButtons>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <HeaderTemplate>
                                                <dx:ASPxButton runat="server" RenderMode="Link" AutoPostBack="false">
                                                    <Image ToolTip="Insert" Url="~/Content/Images/icons8-plus-math-filled-16.png" />
                                                    <ClientSideEvents Click="function(s,e){ gv.AddNewRow(); }" />
                                                </dx:ASPxButton>
                                            </HeaderTemplate>
                                        </dx:GridViewCommandColumn>
                                        <dx:GridViewCommandColumn ShowSelectCheckbox="True" ShowClearFilterButton="true" SelectAllCheckboxMode="Page" Width="45px"/>
                                        <dx:GridViewDataColumn FieldName="Id" ReadOnly="true" Width="0" EditFormSettings-Visible="False" />
                                        <dx:GridViewDataTextColumn FieldName="Name" Caption="ProductName" Width="35%"/>
                                        <dx:GridViewDataTextColumn FieldName="Primary"/>
                                        <dx:GridViewDataTextColumn FieldName="PrimarySize"/>
                                        <dx:GridViewDataTextColumn FieldName="NetWeight" Caption="N/W (g)" />
                                        <dx:GridViewDataTextColumn FieldName="PackSize"/>
                                        <dx:GridViewDataTextColumn FieldName="DW" Caption="D/W (g)" />
                                        <dx:GridViewDataComboBoxColumn FieldName="DWType">
                                            <PropertiesComboBox DataSourceID="dsDWeight" ValueField="ID" TextField="Name" />
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataTextColumn FieldName="FixedFillWeight" Caption="Fixed F/W (g)" />
                                        <dx:GridViewDataTextColumn FieldName="PW" Caption="P/W (g)" />
                                        <dx:GridViewDataTextColumn FieldName="TargetPrice" />
                                        <dx:GridViewDataTextColumn FieldName="SaltContent" />
                                        <dx:GridViewDataTextColumn FieldName="Efficiency" Width="0px" CellStyle-CssClass="unitPriceColumn" HeaderStyle-CssClass="unitPriceColumn" />
                                        <dx:GridViewDataTextColumn FieldName="Yield" Width="0px" CellStyle-CssClass="unitPriceColumn" HeaderStyle-CssClass="unitPriceColumn"/>
                                        <%--<dx:GridViewDataComboBoxColumn FieldName="Country" Caption="Zone/Country" Visible="false">
                                            <PropertiesComboBox DataSourceID="dsCountry" ValueField="Id" TextField="Name">
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="Code" />
                                                    <dx:ListBoxColumn FieldName="Name" />
                                                </Columns>
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataTextColumn FieldName="Destination" Caption="Country" Visible="false"/>--%>
                                        <dx:GridViewDataColumn FieldName="Mark" Width="0px" CellStyle-CssClass="unitPriceColumn" HeaderStyle-CssClass="unitPriceColumn"/>
                                    </Columns>
                                    <SettingsSearchPanel ColumnNames="" Visible="false" />
		                            <Settings ShowGroupPanel="false" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="false" ShowStatusBar="Hidden"/>
		                            <SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true"/>
		                            <SettingsPager Mode="ShowAllRecords"/>
		                            <Styles>
			                            <Row Cursor="pointer" />
		                            </Styles>
                                    <SettingsContextMenu Enabled="false" />
                                    <EditFormLayoutProperties>
                                        <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="600" />
                                    </EditFormLayoutProperties>
                                    <SettingsEditing Mode="Batch">
                                       <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="Cell"/>
                                   </SettingsEditing>  
                                    <ClientSideEvents BatchEditEndEditing="_BatchEditEndEditing" BatchEditStartEditing="function(s, e) {
                                            curentEditingIndex = e.visibleIndex; }" ToolbarItemClick="OnToolbarItemClick"/>
                                </dx:ASPxGridView>
                                </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                       </Items>
                       </dx:LayoutGroup>
                       <dx:LayoutGroup Caption="Packaging" ColCount="4" Width="100%"> 
                           <GridSettings StretchLastItem="true" WrapCaptionAtWidth="660">
                                <Breakpoints>
                                    <dx:LayoutBreakpoint MaxWidth="500" ColumnCount="1" Name="S" />
                                    <dx:LayoutBreakpoint MaxWidth="800" ColumnCount="2" Name="M" />
                                </Breakpoints>
                            </GridSettings>
                       <Items>  
                        <dx:LayoutItem Caption="Primary Type" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxButtonEdit ID="CmbPrimary" runat="server" ReadOnly="true" ClientInstanceName="ClientPrimary">
                                           <Buttons>
                                                <dx:EditButton>
                                                </dx:EditButton>
                                           </Buttons>
                                           <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Primary'); }" />
                                       </dx:ASPxButtonEdit>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>     
                           <dx:LayoutItem Caption="Color(cup/can)" Name="Colortext" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxButtonEdit ID="CmbColor" runat="server" ClientInstanceName="ClientColor">
                                           <Buttons>
                                                <dx:EditButton>
                                                </dx:EditButton>
                                           </Buttons>
                                           <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Color'); }" />
                                       </dx:ASPxButtonEdit>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem> 
                           <dx:LayoutItem Caption="Material(Can,Pouch)" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <%--<dx:ASPxComboBox ClientInstanceName="ClientMaterial" ID="CmbMaterial" runat="server" OnCallback="CmbMaterial_Callback" DataSourceID="dsPackagType" 
                                       ValueField="ID" TextField="Name">
                                       <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxComboBox>--%>
                                       <dx:ASPxButtonEdit ID="CmbMaterial" runat="server" ClientInstanceName="ClientMaterial">
                                           <Buttons>
                                                <dx:EditButton>
                                                </dx:EditButton>
                                           </Buttons>
                                           <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Material'); }" />
                                       </dx:ASPxButtonEdit>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                          <dx:LayoutItem Caption="Pack Size" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                    <dx:ASPxTextBox ID="tbPackSize" runat="server" ClientInstanceName="ClientPackSize" NullText="Example : 1,2,3">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                        <ClientSideEvents KeyPress="function(s,e){ fn_AllowonlyNumeric(s,e);}" />
                                    </dx:ASPxTextBox>
                                    
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>   
                           <dx:LayoutItem Caption="Lid(Can,Cup)" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxButtonEdit ID="CmbPFLid" runat="server" ClientInstanceName="ClientPFLid">
                                           <Buttons>
                                                <dx:EditButton>
                                                </dx:EditButton>
                                           </Buttons>
                                           <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('PFLid'); }" />
                                       </dx:ASPxButtonEdit>
                                       <%--<dx:ASPxComboBox ID="CmbPFLid" runat="server" TextField="Name" ValueField="ID" OnCallback="CmbPFLid_Callback" DataSourceID="dsPFLid" 
                                         ClientInstanceName="ClientPFLid">
                                        <ClientSideEvents Validation="function(s,e) { e.IsValid = s.GetValue() == ''}"/>
                                        <ValidationSettings Display="Dynamic" SetFocusOnError="true" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxComboBox>--%>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem> 
                           
                           <dx:LayoutItem Caption="Shape(Cup)" Name="Shape" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <%--<dx:ASPxComboBox ClientInstanceName="ClientShape" ID="CmbShape" runat="server" OnCallback="CmbShape_Callback" DataSourceID="dsShape" 
                                           ValueField="ID" TextField="Name">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxComboBox>--%>
                                       <dx:ASPxButtonEdit ID="CmbShape" runat="server" ClientInstanceName="ClientShape">
                                           <Buttons>
                                                <dx:EditButton>
                                                </dx:EditButton>
                                           </Buttons>
                                           <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Shape'); }" />
                                       </dx:ASPxButtonEdit>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>  
                            <dx:LayoutItem Caption="Lacquer(Can)" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxButtonEdit ID="CmbLacquer" runat="server" ClientInstanceName="ClientLacquer">
                                           <Buttons>
                                                <dx:EditButton>
                                                </dx:EditButton>
                                           </Buttons>
                                           <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Lacquer'); }" />
                                       </dx:ASPxButtonEdit>
                                       <%--<dx:ASPxComboBox ClientInstanceName="ClientLacquer" ID="CmbLacquer" runat="server" OnCallback="CmbLacquer_Callback" DataSourceID="dsLacquer" 
                                           ValueField="ID" TextField="Name">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxComboBox>--%>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                          <dx:LayoutItem Caption="Type" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxButtonEdit ID="CmbPFType" runat="server" ClientInstanceName="ClientPFType">
                                           <Buttons>
                                                <dx:EditButton>
                                                </dx:EditButton>
                                           </Buttons>
                                           <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('PFType'); }" />
                                       </dx:ASPxButtonEdit>
                                       <%--<dx:ASPxComboBox ID="CmbPFType" runat="server" TextField="Name" ValueField="ID" OnCallback="CmbPFType_Callback" DataSourceID="dsPFType" 
                                           ClientInstanceName="ClientPFType" 
                                           IncrementalFilteringMode="Contains">
                                           <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxComboBox>--%>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>     
                           
                           <dx:LayoutItem Caption="Design(Can,Cup)" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxButtonEdit ID="CmbDesign" runat="server" ClientInstanceName="ClientDesign">
                                           <Buttons>
                                                <dx:EditButton>
                                                </dx:EditButton>
                                           </Buttons>
                                           <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Design'); }" />
                                       </dx:ASPxButtonEdit>
                                       <%--<dx:ASPxComboBox ID="CmbDesign" TextField="Name" ValueField="ID" runat="server" OnCallback="CmbDesign_Callback" DataSourceID="dsDesign" 
                                         ClientInstanceName="ClientDesign">
                                        <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxComboBox>--%>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem> 
                           
                           <dx:LayoutItem Caption="Selling Unit" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxComboBox ID="CmbSellingUnit" DataSourceID="dsSellingUnit" ValueField="ID" TextField="Name" runat="server"  
                                         ClientInstanceName="ClientSellingUnit" OnCallback="CmbSellingUnit_Callback">
                                       <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxComboBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
			                <dx:LayoutItem Caption="Primary Size(By RD)" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                   <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                       <dx:ASPxTextBox ID="tbSize" runat="server" ClientInstanceName="ClientSize" NullText="Example : 211x106 for 80g / 300x200 for 156g">
                                       <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                       </dx:ASPxTextBox>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
			                
                       </Items>
                       </dx:LayoutGroup>
                       <dx:LayoutGroup Caption="Ingredient restrictions & claims">
                        <GridSettings StretchLastItem="true" WrapCaptionAtWidth="660">
                        <Breakpoints>
                            <dx:LayoutBreakpoint MaxWidth="500" ColumnCount="1" Name="S" />
                            <dx:LayoutBreakpoint MaxWidth="800" ColumnCount="2" Name="M" />
                        </Breakpoints>
                        </GridSettings>
                       <Items>
                           <dx:LayoutItem Caption="Ing.restrictions" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxCallbackPanel ID="cpUpdateRole" runat="server" Width="100%" ClientInstanceName="cpUpdateRole"  OnCallback="cpUpdateRole_Callback">
                                <PanelCollection>
                                <dx:PanelContent runat="server">
                                <table>
                                    <tr><td colspan="8">
                                <dx:ASPxCheckBoxList ID="cbIngredient" runat="server" DataSourceID="dsIngredient" ValueField="ID" TextField="Name" 
                                           ClientInstanceName="ClientIngredient" RepeatColumns="6" RepeatLayout="Table" Border-BorderWidth="0">
                                            <CaptionSettings Position="Top" />
					                        <ClientSideEvents SelectedIndexChanged="function(s,e) { hfIngredient.Set('Ingredient',s.GetSelectedValues().join(';'));}" />
                                        </dx:ASPxCheckBoxList></td>
                                        <td><dx:ASPxTextBox ID="tbIngredientOther" ClientInstanceName="ClientIngredientOther" Width="230px" runat="server"/></td>
                                    </tr>
                                        
                                  <%--  <dx:ASPxGridLookup ID="CmbIngredient" runat="server" SelectionMode="Multiple" DataSourceID="dsIngredient" ClientInstanceName="ClientIngredient"
                                    OnInit="CmbIngredient_Init"
                                    KeyFieldName="value" TextFormatString="{0}" MultiTextSeparator=", ">
                                    <Columns>
                                        <dx:GridViewCommandColumn ShowSelectCheckbox="True" />
                                        <dx:GridViewDataColumn FieldName="value" />
                                    </Columns>
                                    <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                </dx:ASPxGridLookup>--%>
                                </table>
                                </dx:PanelContent></PanelCollection>
                                </dx:ASPxCallbackPanel>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <%--<dx:LayoutItem Caption="Other" Width="100%">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                       <dx:ASPxTextBox ID="tbIngredientOther" ClientInstanceName="ClientIngredientOther" Width="230px" runat="server"/>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem> --%>
                        <dx:LayoutItem Caption="Claims" Name="Claims" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <%--<dx:ASPxGridLookup ID="Cmbclaims" runat="server" SelectionMode="Multiple" DataSourceID="dsclaims" ClientInstanceName="Clientclaims"
                                    OnInit="Cmbclaims_Init"
                                    KeyFieldName="value" TextFormatString="{0}" MultiTextSeparator=", ">
                                    <Columns>
                                        <dx:GridViewCommandColumn ShowSelectCheckbox="True" />
                                        <dx:GridViewDataColumn FieldName="value" />
                                    </Columns>
                                    <ValidationSettings Display="Dynamic" RequiredField-IsRequired="true" ValidationGroup="group1"/>
                                </dx:ASPxGridLookup>--%>
                                <table>
                                    <tr><td colspan="3">
                                    <dx:ASPxCheckBoxList ID="cbclaims" runat="server" DataSourceID="dsclaims" ValueField="ID" TextField="Name"
                                           ClientInstanceName="Clientclaims" RepeatColumns="5" RepeatLayout="Table" Border-BorderWidth="0">
                                            <CaptionSettings Position="Top" />
                                        </dx:ASPxCheckBoxList></td>
                                        <td></td>
                                    </tr>
                                </table>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
			            <dx:LayoutItem Caption="" VerticalAlign="Middle" Name="listclaim">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <table>
                                    <tr>
                                        <td>
                                <dx:ASPxDropDownEdit ID="ddelistclaim" runat="server" ClientInstanceName="DropDownEdit"
                                Width="485px" AllowUserInput="False" AnimationType="None">
                                <DropDownWindowStyle>
                                </DropDownWindowStyle>
                                <ClientSideEvents Init="UpdateSelection" DropDown="OnDropDown" />
                                <DropDownWindowTemplate>
                                    <div>
                                    <dx:ASPxTreeList ID="treeList" ClientInstanceName="TreeList" runat="server" AutoGenerateColumns="False" DataSourceID="dslistclaim"                   
					                        OnDataBound="treeList_DataBound" OnCustomDataCallback="treeList_CustomDataCallback" OnCustomCallback="treeList_CustomCallback" 
                                            Width="100%" KeyFieldName="ID" ParentFieldName="ParentID">         
                                            <Columns>
                                                <dx:TreeListDataColumn FieldName="Name" Caption="Name" VisibleIndex="0" />
                                            </Columns>
                                            <SettingsBehavior ExpandCollapseAction="NodeDblClick"   />
                                            <SettingsSelection Enabled="True" AllowSelectAll="false"   />
                                            <Settings ShowColumnHeaders="false" VerticalScrollBarMode="Auto" ScrollableHeight="150"/>
                                            <ClientSideEvents SelectionChanged="treeList_SelectionChanged" CustomDataCallback="treeList_CustomDataCallback"  />
                                        </dx:ASPxTreeList>
                                        </div>
                                            <table style="background-color: White; width: 100%;display:none;">
                                                <tr>
                                                    <td style="padding: 10px;">
                                                        <dx:ASPxButton ID="clearButton" ClientEnabled="false" ClientInstanceName="clearButton"
                                                            runat="server" AutoPostBack="false" Text="Clear">
                                                            <ClientSideEvents Click="ClearSelection" />
                                                        </dx:ASPxButton>
                                                    </td>
                                                    <td style="text-align: right; padding: 10px;">
                                                        <dx:ASPxButton ID="selectButton" ClientEnabled="false" ClientInstanceName="selectButton"
                                                            runat="server" AutoPostBack="false" Text="Select">
                                                            <ClientSideEvents Click="UpdateSelection" />
                                                        </dx:ASPxButton>
                                                        <dx:ASPxButton ID="closeButton" runat="server" AutoPostBack="false" Text="Close">
                                                            <ClientSideEvents Click="function(s,e) { DropDownEdit.HideDropDown(); }" />
                                                        </dx:ASPxButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </DropDownWindowTemplate>
                                        <ClientSideEvents DropDown="DropDownHandler" ButtonClick="ButtonClickHandler" />
                                    </dx:ASPxDropDownEdit></td>
                                    <td>&nbsp;Concave&nbsp;</td>
                                    <td><dx:ASPxRadioButtonList ID="rbConcave" runat="server" DataSourceID="dsConcave" ClientInstanceName="rbConcave"
                                            ValueField="idx" TextField="value" RepeatColumns="4" RepeatLayout="Flow" Border-BorderWidth="0">
                                            <CaptionSettings Position="Top" />
                                        </dx:ASPxRadioButtonList></td>
                                    </tr>
                                </table>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
 
                        <dx:LayoutItem Caption="Other">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="tbclaimsOther" ClientInstanceName="ClientclaimsOther" Width="230px" runat="server">
                                            </dx:ASPxTextBox>
                                       <%--<dx:ASPxTextBox ID="tbclaimsOther" ClientInstanceName="ClientclaimsOther" Width="230px" runat="server"/> --%>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                        <%--<dx:LayoutItem Caption="Other">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                       <dx:ASPxTextBox ID="tbclaimsOther" ClientInstanceName="ClientclaimsOther" Width="230px" runat="server"/>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem> --%>
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
                        <dx:LayoutItem Caption="Selected" Width="100%" VerticalAlign="Middle" ClientVisible="false">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxUploadControl ID="UploadControl" runat="server" ClientInstanceName="UploadControl" Width="200"
                                            NullText="Select multiple files..." UploadMode="Advanced" ShowUploadButton="false" ShowProgressPanel="True"
                                            OnFileUploadComplete="UploadControl_FileUploadComplete">
                                            <AdvancedModeSettings EnableMultiSelect="True" EnableFileList="True" EnableDragAndDrop="True"  />
                                            <ValidationSettings MaxFileSize="10485760" AllowedFileExtensions=".jpg,.jpeg,.gif,.png,.xls,.xlsx,.pdf">
                                            </ValidationSettings>
                                        <ClientSideEvents FilesUploadStart="onFileUploadStart" />
                                    </dx:ASPxUploadControl>
                                    <%--<br />
                                    <asp:FileUpload ID="FileUploadControl" runat="server" AllowMultiple="true" />
                                            <dx:ASPxTokenBox runat="server" Width="100%" ID="UploadedFilesTokenBox" ClientInstanceName="UploadedFilesTokenBox"
                                                NullText="Select the documents to submit" AllowCustomTokens="false" ClientVisible="false">
                                                <ValidationSettings EnableCustomValidation="true" />
                                            </dx:ASPxTokenBox>--%>
                                   </dx:LayoutItemNestedControlContainer>
                               </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
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
                       <dx:LayoutGroup Caption="Details" ColCount="4">
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
                            <dx:LayoutItem Width="100%" Caption="" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                  <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True"> 
                                      <dx:ASPxGridView runat="server" ID="gvformu" ClientInstanceName="gvformu" KeyFieldName="ID" Width="100%"
                                          OnDataBound="gvformu_DataBound" OnCustomDataCallback="gvformu_CustomDataCallback"
                                          OnDataBinding="gvformu_DataBinding" OnCustomCallback="gvformu_CustomCallback" OnBatchUpdate="gvformu_BatchUpdate">
                                          <ClientSideEvents BatchEditEndEditing="gvformu_OnEndEdit" BatchEditStartEditing="gvformu_onStartEdit"
                                              ToolbarItemClick="OnDLToolbarItemClick" 
                                              CustomButtonClick="OnDLButtonClick"  />
                                          <Toolbars>
                                            <dx:GridViewToolbar Name="MyToolbar">
                                                <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                                                <Items>
                                                    <dx:GridViewToolbarItem Command="Custom" Name="Semi" Text="Semi" Image-IconID="actions_add_16x16office2013" />
                                                    <dx:GridViewToolbarItem Command="Custom" Name="BCDL" Text="DL" Image-IconID="actions_add_16x16office2013" />
                                                    <dx:GridViewToolbarItem Command="Custom" Name="Packaging" Text="Packaging" Image-IconID="actions_add_16x16office2013" />          
                                                    <dx:GridViewToolbarItem Command="Refresh" BeginGroup="true" AdaptivePriority="2" />
                                                </Items>
                                                
                                            </dx:GridViewToolbar>
                                          
                                          </Toolbars>
                                          <Columns>
                                          <dx:GridViewCommandColumn  ShowClearFilterButton="true" Width="62px" ButtonRenderMode="Image"
                                            FixedStyle="Left" ShowNewButtonInHeader="true" ShowEditButton="true" ShowDeleteButton="true">
                                            <CustomButtons>
                                                <dx:GridViewCommandColumnCustomButton ID="EditCost">
                                                    <Image ToolTip="Remove" Url="~/Content/Images/Cancel.gif"/>
                                                </dx:GridViewCommandColumnCustomButton>
                                            </CustomButtons>
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </dx:GridViewCommandColumn>
                                              <dx:GridViewDataComboBoxColumn FieldName="Component" FixedStyle="Left" VisibleIndex="1" GroupIndex="0">
                                                  <PropertiesComboBox DataSourceID="dsComponentMat" ValueField="idx" TextField="value" />
                                              </dx:GridViewDataComboBoxColumn>
                                              <dx:GridViewDataButtonEditColumn FieldName="Material" ReadOnly="true" Width="18%">
                                                    <PropertiesButtonEdit>
                                                        <Buttons>
                                                            <dx:EditButton>
                                                            </dx:EditButton>
                                                        </Buttons>
                                                        <ClientSideEvents ButtonClick="function(s,e){OnButtonClick('SelMaterial');}" />
                                                    </PropertiesButtonEdit>
                                                </dx:GridViewDataButtonEditColumn>
                                              <dx:GridViewDataColumn FieldName="Name" Caption="Description"/>
                                              <dx:GridViewDataColumn FieldName="Result" Caption="GUnit" />
                                              <dx:GridViewDataColumn FieldName="Yield"/>
                                              <dx:GridViewDataTextColumn FieldName="PriceOfUnit" PropertiesTextEdit-DisplayFormatString="F2">
                                              </dx:GridViewDataTextColumn>
                                              <dx:GridViewDataTextColumn FieldName="AdjustPrice">
                                                  <PropertiesTextEdit DisplayFormatString="F2" />
                                              </dx:GridViewDataTextColumn>
                                              <dx:GridViewDataComboBoxColumn FieldName="Unit">
                                                    <PropertiesComboBox TextFormatString="{0}" DataSourceID="dsUnit" ValueField="Code">
                                                        <Columns>
                                                            <dx:ListBoxColumn FieldName="Code" />
                                                        </Columns>
                                                    </PropertiesComboBox>
                                                </dx:GridViewDataComboBoxColumn>
                                              <dx:GridViewDataColumn FieldName="Diff"/>
                                              <dx:GridViewDataColumn FieldName="PriceOfCarton" Caption="Total(THB)"/>
                                              <%--<dx:GridViewBandColumn Caption="LBOh">
                                                <Columns>
                                                <dx:GridViewDataButtonEditColumn FieldName="LBOh" Caption="LBCode">
                                                    <PropertiesButtonEdit>
                                                        <Buttons>
                                                            <dx:EditButton />
                                                        </Buttons>
                                                        <ClientSideEvents ButtonClick="function(s, e) { OnButtonClick('Semi'); }" />
                                                    </PropertiesButtonEdit>
                                                </dx:GridViewDataButtonEditColumn>
                                                <dx:GridViewDataColumn FieldName="LBRate"/>
                                                </Columns>
                                              </dx:GridViewBandColumn>--%>
                                              
                                              <dx:GridViewDataColumn FieldName="Remark"/>
                                              <dx:GridViewDataColumn FieldName="Formula" Width="0">
                           <%--                     <HeaderStyle CssClass="hide" />
                                                <EditCellStyle CssClass="hide" />
                                                <CellStyle CssClass="hide" />
                                                <FilterCellStyle CssClass="hide" />
                                                <FooterCellStyle CssClass="hide" />
                                                <GroupFooterCellStyle CssClass="hide" />--%>
                                            </dx:GridViewDataColumn>
                                          </Columns>
                                        <Templates>
                                            <PagerBar>
                                                <table>
                                                    <tr>
                                                        <%--<td>
                                                            <dx:aspxgridviewtemplatereplacement runat="server" replacementtype="pager" />
                                                        </td>--%>
                                                        <td>
                                                            <dx:ASPxLabel ID="labelInfo" runat="server" Text="" ClientInstanceName="labelInfo">
                                                                <ClientSideEvents Init="function(s, e) { ShowRowsCount(); }" />
                                                            </dx:ASPxLabel>
                                                        </td>
                                                         <td>
                                                            <dx:ASPxComboBox ID="cbPage" runat="server" ToolTip="Current Page" Width="250px" ClientInstanceName="ClientPage" OnCallback="cbPage_Callback" 
                                                                OnInit="cbPage_Init">
                                                                <ClientSideEvents SelectedIndexChanged="function (s, e) { MoveToPage(s.GetValue()); }"
                                                                    Init="function (s, e) { s.SetValue(gvformu.cpPageIndex); }" />
                                                            </dx:ASPxComboBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </PagerBar>
                                        </Templates>
            
<%--                                        <GroupSummary>
                                            <dx:ASPxSummaryItem FieldName="PriceOfUnit" ShowInGroupFooterColumn="PriceOfUnit" SummaryType="Average" Tag="GroupPriceOfUnit" />
                                            <dx:ASPxSummaryItem FieldName="Result" ShowInGroupFooterColumn="Result" SummaryType="Count" />
                                        </GroupSummary>--%>
                          
                                        <SettingsBehavior AutoExpandAllGroups="true" />
                                        <SettingsPager PageSize="50">
						                <PageSizeItemSettings Visible="true" ShowAllItem="true"  AllItemText="All Records" />
					                    </SettingsPager>
                                        <SettingsSearchPanel ColumnNames="" Visible="false" />
		                                <Settings ShowGroupPanel="false" VerticalScrollBarMode="Auto" VerticalScrollableHeight="240" ShowGroupedColumns="True" 
                                            GridLines="Vertical" ShowFooter="true" ShowStatusBar="Hidden"/>
		                                <SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true"/>
		                    
		                                <Styles>
			                                <Row Cursor="pointer" />
		                                </Styles>
                                        <SettingsContextMenu Enabled="false" />

                                        <EditFormLayoutProperties>
                                            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="600" />
                                        </EditFormLayoutProperties>
                                        <SettingsEditing Mode="Batch">
                                           <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="Cell"/>
                                       </SettingsEditing>
                                      </dx:ASPxGridView>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <%--<dx:LayoutItem Caption="Product Code">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="cmbformu" runat="server" ValueField="Id" TextField="Name" OnCallback="cmbformu_Callback" 
                                        ClientInstanceName="cmbformu" Width="300px">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { gvformu.PerformCallback('symbol|' + s.GetValue()); }" />
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>--%>
                          </Items>
                      </dx:LayoutGroup>   
                        <dx:LayoutGroup Caption="Destination" ColCount="3" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
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
                        <dx:LayoutItem Width="100%" RowSpan="2" Caption="Zone/Country" VerticalAlign="Middle">
			                <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                  <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                      <dx:ASPxGridView ID="gvzone" runat="server" ClientInstanceName="gvzone" KeyFieldName="Id" OnDataBinding="gvzone_DataBinding"
                                          OnCustomCallback="gvzone_CustomCallback" OnBatchUpdate="gvzone_BatchUpdate" OnCustomButtonCallback="gvzone_CustomButtonCallback">
                                          <Columns>
                                              <dx:GridViewCommandColumn ShowClearFilterButton="true" Width="45px" ButtonRenderMode="Image"
                                                    FixedStyle="Left" ShowNewButtonInHeader="true" ShowEditButton="true" ShowDeleteButton="true">
                                                    <CustomButtons>
                                                        <dx:GridViewCommandColumnCustomButton ID="Remove">
                                                            <Image ToolTip="Remove" Url="~/Content/Images/Cancel.gif"/>
                                                        </dx:GridViewCommandColumnCustomButton>
                                                    </CustomButtons>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <HeaderTemplate>
                                                        <dx:ASPxButton runat="server" RenderMode="Link" AutoPostBack="false">
                                                            <Image ToolTip="Insert" Url="~/Content/Images/icons8-plus-math-filled-16.png" />
                                                            <ClientSideEvents Click="function(s,e){ gvzone.AddNewRow(); }" />
                                                        </dx:ASPxButton>
                                                    </HeaderTemplate>
                                                </dx:GridViewCommandColumn>
                                              
                                              <dx:GridViewDataComboBoxColumn FieldName="Country" Caption="Zone/Country">
                                                <PropertiesComboBox DataSourceID="dsCountry" ValueField="Code" TextField="Name" 
                                                    IncrementalFilteringMode="Contains" EnableSynchronization="False">
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="Code" />
                                                    <dx:ListBoxColumn FieldName="Name" />
                                                    </Columns>
                                                </PropertiesComboBox>
                                            </dx:GridViewDataComboBoxColumn>
                                            <dx:GridViewDataTextColumn FieldName="Zone" Caption="Country"/>
                                          </Columns>
                                           <SettingsSearchPanel ColumnNames="" Visible="false" />
		                                    <Settings ShowGroupPanel="false" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="false" ShowStatusBar="Hidden"/>
		                                    <SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true"/>
		                                    <SettingsPager Mode="ShowAllRecords"/>
		                                    <Styles>
			                                    <Row Cursor="pointer" />
		                                    </Styles>
                                            <SettingsContextMenu Enabled="false" />
                                            <EditFormLayoutProperties>
                                                <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="600" />
                                            </EditFormLayoutProperties>
                                            <SettingsEditing Mode="Batch">
                                               <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="Cell"/>
                                           </SettingsEditing>  
                                           <ClientSideEvents BatchEditEndEditing="_BatchEditEndEditing" BatchEditStartEditing="function(s, e) {
                                            curentEditingIndex = e.visibleIndex; }" />
                                      </dx:ASPxGridView>
                                  </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                           </dx:LayoutItem>
                    </Items>
                    </dx:LayoutGroup>
                    <dx:LayoutGroup Caption="PackingStyle" ColCount="3" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
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
                    <%--<dx:LayoutItem Caption="Inner" Name="Inner" Width="100%" VerticalAlign="Middle">
                                    <SpanRules>
                                        <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                        <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                                    </SpanRules>
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <table>
                                            <tr><td colspan="3">
                                            <dx:ASPxCheckBoxList ID="cbInner" runat="server" Border-BorderWidth="0" RepeatDirection="Horizontal" DataSourceID="dsSecPackage"
                                                ValueType="System.Int32" ValueField="ID" TextField="Name" ClientInstanceName="ClientInner">
                                                <ClientSideEvents SelectedIndexChanged ="Inner_SelectedIndexChanged"  />
                                                </dx:ASPxCheckBoxList>
                                                </td>
                                                <td><dx:ASPxTextBox ID="tbInnerOther" ClientInstanceName="tbInnerOther" Width="230px" runat="server">
                                                    </dx:ASPxTextBox></td>
                                            </tr>
                                           
                                        </table>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Outer" Name="Outer" Width="100%" VerticalAlign="Middle">
                                <SpanRules>
                                    <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                    <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                                </SpanRules>
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <table>
                                            <tr><td colspan="3">
                                            <dx:ASPxCheckBoxList ID="cbOuter" runat="server" Border-BorderWidth="0" RepeatDirection="Horizontal" DataSourceID="dsOuter"
                                                ValueType="System.Int32" ValueField="ID" TextField="Name" ClientInstanceName="ClientOuter">
                                                <ClientSideEvents SelectedIndexChanged ="Outer_SelectedIndexChanged" />
                                                </dx:ASPxCheckBoxList>
                                                </td>
						                        <td><dx:ASPxTextBox ID="tbOuterOther" ClientInstanceName="tbOuterOther" Width="230px" runat="server">
                                                    </dx:ASPxTextBox></td>
                                            </tr>
                                        </table>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Selected" Name="Upload" Width="100%">
                                        <SpanRules>
                                        <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                        <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                                    </SpanRules>
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxUploadControl ID="ASPxUploadControl1" runat="server" OnFileUploadComplete="Upload_FileUploadComplete" Width="200" ClientInstanceName="Upload"  
                                            NullText="Click here to browse files..." UploadMode="Advanced" FileUploadMode="OnPageLoad" AutoStartUpload="True">
                                            <ValidationSettings AllowedFileExtensions=".xls,.xlsx">
                                            </ValidationSettings>
                                            <ClientSideEvents FileUploadComplete="OnFileUploadComplete" />
                                        </dx:ASPxUploadControl>
                                        </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>--%>

                        <dx:LayoutItem Width="100%" RowSpan="2" Caption="" VerticalAlign="Middle"  CaptionSettings-Location="Left">
			                <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                               <LayoutItemNestedControlCollection>
                                  <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                      <dx:ASPxGridView ID="ordersGridView" ClientInstanceName="ordersGridView" runat="server" 
                                           OnDataBinding="ordersGridView_DataBinding" OnCustomCallback="ordersGridView_CustomCallback"
                                           KeyFieldName="Id" Width="100%">
                                          <ClientSideEvents ToolbarItemClick="OnToolbarItemClick_ordersGridView" RowFocusing="employeesGrid_FocusedRowChanged" />
                                          <Toolbars>
                                            <dx:GridViewToolbar Name="MyToolbarorders">
                                                <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                                                <Items>
                                                    <dx:GridViewToolbarItem Command="Custom" Name="AddNew" Text="AddNew" Image-IconID="actions_add_16x16office2013" />
                                                    <%--<dx:GridViewToolbarItem Command="Custom" Name="BCDL" Text="DL" Image-IconID="actions_add_16x16office2013" />
                                                    <dx:GridViewToolbarItem Command="Custom" Name="Packaging" Text="Packaging" Image-IconID="actions_add_16x16office2013" />         
                                                    <dx:GridViewToolbarItem Command="Refresh" BeginGroup="true" AdaptivePriority="2" />--%> 
                                                </Items>
                                            </dx:GridViewToolbar>
                                          </Toolbars>
                                           <Columns>
                                               <%--<dx:GridViewCommandColumn ShowClearFilterButton="true" Width="45px" ButtonRenderMode="Image"
                                                    FixedStyle="Left" ShowNewButtonInHeader="true" ShowEditButton="true" ShowDeleteButton="true">
                                                    <CustomButtons>
                                                        <dx:GridViewCommandColumnCustomButton ID="ordersGridView_Delete">
                                                            <Image ToolTip="Delete" Url="~/Content/Images/Cancel.gif"/>
                                                        </dx:GridViewCommandColumnCustomButton>
                                                    </CustomButtons>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <HeaderTemplate>
                                                        <dx:ASPxButton runat="server" RenderMode="Link" AutoPostBack="false">
                                                            <Image ToolTip="Insert" Url="~/Content/Images/icons8-plus-math-filled-16.png" />
                                                            <ClientSideEvents Click="function(s,e){ ordersGridView.AddNewRow(); }" />
                                                        </dx:ASPxButton>
                                                    </HeaderTemplate>
                                                </dx:GridViewCommandColumn>--%>
                                               <dx:GridViewDataTextColumn FieldName="Id" ReadOnly="True" VisibleIndex="0">
                                                    <EditFormSettings Visible="False" />
                                               </dx:GridViewDataTextColumn>
                                               <dx:GridViewDataColumn FieldName="Option" />
                                               <dx:GridViewDataColumn FieldName="OD" />
                                               <dx:GridViewDataColumn FieldName="StylePackage" />
                                               <dx:GridViewDataColumn Caption="Details" Width="10%">
                                                    <DataItemTemplate>
                                                        <div id='<%# Eval("ID") %>' class="gridEditButton" onclick="GridEditButton_Click(event)" title="Edit Detail"></div>
                                                    </DataItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                </dx:GridViewDataColumn>
                                           </Columns>    
                                    <Settings VerticalScrollBarMode="Auto" VerticalScrollableHeight="190" GridLines="None" />
                                    <SettingsBehavior AllowFocusedRow="true" />
                                    <SettingsPager Mode="ShowAllRecords"></SettingsPager>
                                    <Styles>
                                        <Table CssClass="dataTable"></Table>
                                        <Header CssClass="header"></Header>
                                        <CommandColumn CssClass="commandColumn"></CommandColumn>
                                        <CommandColumnItem CssClass="commandColumnBtn"></CommandColumnItem>
                                        <FocusedRow CssClass="focusRow"></FocusedRow>
                                    </Styles>

                                    <SettingsEditing Mode="Inline">
                                    <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="Cell"/>
                                    </SettingsEditing>
                                   </dx:ASPxGridView>
                                  <div class="employeesDetailsMainContainer">
                                    <div class="employeesDetailsContainer">
                                        <table>
                                            <tr>
                                                <td>
                                                    <dx:ASPxHeadline ID="DetailsHeaderHeadLine" CssClass="employeesDetailsHeadline" runat="server" HeaderText="Detail" ContentText="Position">
                                                        <HeaderStyle CssClass="header" />
                                                        <ContentStyle CssClass="content"></ContentStyle>
                                                        <RightPanelStyle CssClass="rightPanel"></RightPanelStyle>
                                                    </dx:ASPxHeadline>
                                                </td>
                                                <td class="employeeEditButtonCell">
                                                    <dx:ASPxImage ID="EditImage" runat="server" ImageUrl="~/Content/Images/Buttons/EditCustomerButton_Gray.png">
                                                    </dx:ASPxImage>
                                                </td>
                                            </tr>
                                        </table>
                                        <table class="employeesDetailsInfo">
                                            <tr>
                                               <td>
                                                    Primary size : <span runat="server" id="DetailsAddressText"></span>
                                                    pack size : <span runat="server" id="DetailsPhoneText"></span>
                                                    PKG type :  <span runat="server" id="DetailsEmailText"></span>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div class="employeesPagesContainer">
                                    <br />
                                    <dx:ASPxGridView ID="gv_detail" ClientInstanceName="gv_detail" runat="server" OnBeforePerformDataSelect="detail_BeforePerformDataSelect" 
                                        OnCustomCallback="detail_CustomCallback" KeyFieldName="Id" 
                                        OnBatchUpdate="detail_BatchUpdate" Width="100%" OnDataBinding="detail_DataBinding" >
                                    <Columns>
                                        <dx:GridViewCommandColumn ShowClearFilterButton="true" Width="75px" ButtonRenderMode="Image"
                                            FixedStyle="Left" ShowNewButtonInHeader="true" ShowEditButton="true" ShowDeleteButton="true">
                                            <CustomButtons>
                                                <dx:GridViewCommandColumnCustomButton ID="detail_Edit">
                                                    <Image ToolTip="Remove" Url="~/Content/Images/Cancel.gif"/>
                                                </dx:GridViewCommandColumnCustomButton>
                                            </CustomButtons>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <HeaderTemplate>
                                                <dx:ASPxButton runat="server" RenderMode="Link" AutoPostBack="false">
                                                    <Image ToolTip="Insert" Url="~/Content/Images/icons8-plus-math-filled-16.png" />
                                                    <ClientSideEvents Click="function(s,e){ gv_detail.AddNewRow(); }" />
                                                </dx:ASPxButton>
                                            </HeaderTemplate>
                                        </dx:GridViewCommandColumn>
                                        <dx:GridViewDataComboBoxColumn FieldName="PackingStyle">
                                            <PropertiesComboBox DataSourceID="dsPackagingType" ValueField="Id" TextField="Description">
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="Id" />
                                                    <dx:ListBoxColumn FieldName="Description" />
                                                </Columns>
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataTextColumn FieldName="spec" />
                                        <dx:GridViewDataTextColumn FieldName="coating" />
                                        <dx:GridViewDataTextColumn FieldName="totalcolor" />
                                        <dx:GridViewDataMemoColumn FieldName="note" />
                                        
                                    </Columns>
                                    <SettingsSearchPanel ColumnNames="" Visible="false" />
		                            <Settings ShowGroupPanel="false" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="false" ShowStatusBar="Hidden"/>
		                            <SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true"/>
		                            <SettingsPager Mode="ShowAllRecords"/>
		                            <Styles>
			                            <Row Cursor="pointer" />
		                            </Styles>
                                    <SettingsContextMenu Enabled="false" />
                                    <EditFormLayoutProperties>
                                        <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="600" />
                                    </EditFormLayoutProperties>
                                    <SettingsEditing Mode="Batch">
                                        <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="Cell"/>
                                    </SettingsEditing> 
                                </dx:ASPxGridView>
                            </div>
                         </div>
                         </dx:LayoutItemNestedControlContainer>
                                      </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>    
                    </dx:LayoutGroup>
                    </Items>
                    </dx:TabbedLayoutGroup>
                    <dx:LayoutGroup Caption="Action" ColCount="3" GroupBoxDecoration="HeadingLine" UseDefaultPaddings="false" Paddings-PaddingTop="10">
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
                        <dx:LayoutItem Caption="Send Document" Width="100%" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                              <dx:ASPxComboBox ID="CmbDisponsition" runat="server" TextField="Title" ValueField="Id" OnCallback="CmbDisponsition_Callback" Width="240px"
                                     ClientInstanceName="ClientDisponsition">
                                  <ClientSideEvents  SelectedIndexChanged="function(s, e) { OnIDChanged(s); }"/>
                              </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Reason for rejection" Name="Reason" ClientVisible="false" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ClientInstanceName="CmbReason" ID="CmbReason" runat="server" OnCallback="CmbReason_Callback"
                                          ValueField="ID" TextField="Description">
                                        <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic" />
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
                        <dx:LayoutItem Caption="Comment" Width="100%" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxMemo runat="server" ID="mComment" Rows="4" Native="True" ClientInstanceName="ClientComment" >
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
                                        <%--<dx:ASPxHiddenField ID="eventlog" runat="server" ClientInstanceName="eventlog" />
                                        <asp:Literal ID="litText" runat="server"></asp:Literal>--%>
					                    <div id="divScroll" style="overflow: scroll; height: auto; width: auto; display: none"></div>
                                        <dx:aspxlabel id="lbEventLog" runat="server" clientinstancename="clienteventlog" text="ASPxLabel"></dx:aspxlabel>
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
        <asp:SqlDataSource ID="dsingredient" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="spGetIngredient" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="FormPanel$formLayout$CmbPetCategory" Name="ID" PropertyName="Value" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="dsReason" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasReason"/>
    <asp:SqlDataSource ID="dsProductType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasProductType"/>
    <asp:SqlDataSource ID="dsChunkType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasChunkType" />
    <asp:SqlDataSource ID="dsProductStyle" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasProductStyle" />
    <asp:SqlDataSource ID="dsCustomerRequest" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spCustomerRequest" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="FormPanel$formLayout$CmbPetCategory" Name="Category" PropertyName="Value" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="dsuser" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetUser" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsRequestType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasRequestType where dbo.fnc_checktype(usertype,@usertype)>0 and IsActive=0 order by Name">
        <SelectParameters>
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsPetFoodType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasPetFoodType where dbo.fnc_checktype(usertype,@usertype)>0" >
        <SelectParameters>
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsCompany" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasCompany where Code in (select distinct value from dbo.FNC_SPLIT(@Bu,'|')) order by Code">
    <SelectParameters>
            <asp:ControlParameter ControlID="hfBU" Name="BU" PropertyName="['BU']" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsPetCategory" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetCategory" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="dsclaims" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
         SelectCommand="select * from Masclaims where dbo.fnc_checktype(ClaimType,0)>0 and id in (0,1,2)" />
    <asp:SqlDataSource ID="dslistclaim" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
         SelectCommand="select * from Masclaims where dbo.fnc_checktype(ClaimType,'1;2;3;4;5')>0" />
    <asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetselectall" SelectCommandType="StoredProcedure">
        <SelectParameters>
		<asp:ControlParameter ControlID="hfuser" Name="user_name" PropertyName="['user_name']"/>
	        <asp:ControlParameter ControlID="hftype" Name="type" PropertyName="['type']"/>
		<asp:ControlParameter ControlID="hfKeyword" Name="Keyword" PropertyName="['Keyword']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="ArtsDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetFileSystem" SelectCommandType="StoredProcedure" CancelSelectOnNullParameter="False">
        <SelectParameters>
           <asp:ControlParameter ControlID="hfgetvalue" Name="GCRecord" PropertyName="['NewID']"/>
           <asp:ControlParameter ControlID="hfuser" Name="username" PropertyName="['user_name']"/>
           <asp:ControlParameter ControlID="hftablename" Name="tablename" PropertyName="['tablename']"/>
        </SelectParameters>
    </asp:SqlDataSource>
   <asp:SqlDataSource ID="dsCustomerPrice" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasCustomerPrice where dbo.fnc_checktype(usertype,@usertype)>0">
        <SelectParameters>
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
   </asp:SqlDataSource>
   <asp:SqlDataSource ID="dsPlant" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetPlant" SelectCommandType="StoredProcedure">
       <SelectParameters>
            <asp:ControlParameter ControlID="hfBU" Name="BU" PropertyName="['BU']" />
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
       </SelectParameters>
   </asp:SqlDataSource>
   <asp:SqlDataSource ID="dsStatusApp" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasStatusApp where levelapp in (0,2)"/>
   <asp:SqlDataSource ID="dsComponent" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetComponent" SelectCommandType="StoredProcedure" OnSelecting="dsComponent_Selecting">
       <SelectParameters>
           <asp:ControlParameter ControlID="FormPanel$formLayout$cmbCompany" Name="Plant" PropertyName="Value" />
           <asp:ControlParameter ControlID="FormPanel$formLayout$CmbPetCategory" Name="Category" PropertyName="Value" />
           <asp:ControlParameter ControlID="FormPanel$formLayout$CmbDisponsition" Name="action" PropertyName="Value"/>
           <asp:ControlParameter ControlID="hfgetvalue" Name="Id" PropertyName="['NewID']"/>
           <asp:ControlParameter ControlID="hfuser" Name="user" PropertyName="['user_name']"/>
       </SelectParameters>
   </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsCompliedWith" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="spGetCompliedWith" SelectCommandType="StoredProcedure">
        <SelectParameters>
             <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsDWeight" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="select * from MasDWeight where dbo.fnc_checktype(usertype,@usertype)>0">
        <SelectParameters>
             <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsPrdRequirement" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="spGetPrdRequirement" SelectCommandType="StoredProcedure">
        <SelectParameters>
		<asp:ControlParameter ControlID="FormPanel$formLayout$CmbPetCategory" Name="ID" PropertyName="Value" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsNutrientProfile" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="select * from MasNutrientProfile" />
    <asp:SqlDataSource ID="dsSecPackage" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="select * from MasSecPackage where tcode='Inner'" />
    <asp:SqlDataSource ID="dsOuter" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="select * from MasSecPackage where tcode='Outer'" />
    <asp:SqlDataSource ID="dstype" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="	select * from masusertype">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsMedia" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="select * from MasMedia" />
    <asp:SqlDataSource ID="dsSellingUnit" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="spGetSellingUnit" SelectCommandType="StoredProcedure">
        <SelectParameters>
             <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server"
            ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>" />
    <asp:SqlDataSource ID="dsConcave" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="select * from dbo.FNC_SPLIT('yes,no',',')" />

    <asp:SqlDataSource ID="dsComponentMat" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="select * from dbo.FNC_SPLIT('Raw Material,Ingredient,Primary Packaging,Secondary Packaging,DL,Co  product,By  product',',')" />
    <asp:SqlDataSource ID="dsUnit" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select Code from MasUnit">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsLabor" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="spGetLabor" SelectCommandType="StoredProcedure">
        <SelectParameters>
             <asp:ControlParameter ControlID="hfgetvalue" Name="NewID" PropertyName="['NewID']" />
             <asp:ControlParameter ControlID="FormPanel$formLayout$cmbCompany" Name="Plant" PropertyName="Value" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsCountry" runat="server" ConnectionString="<%$ ConnectionStrings:LabConnectionString %>"
       SelectCommand="select Id,Code,Description as Name from [transGrade] where ProductGroup='PF-fish base' and ProductType=case when  dbo.fnc_checktype(@usertype,'0')>0 then 'PF' else 'HF' end and IsActive=1">
        <SelectParameters>
             <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsPackagingType" runat="server" ConnectionString="<%$ ConnectionStrings:PKGConnectionString %>"
       SelectCommand="select Id,Description from MaterialClass" />
    <%--<asp:SqlDataSource ID="dsPackingStyle" runat="server" ConnectionString="<%$ ConnectionStrings:PKGConnectionString %>"
       SelectCommand="select * from MasPackingStyle where TypeofPrimary=@TypeofPrimary">
        <SelectParameters>
            <asp:ControlParameter ControlID="FormPanel$formLayout$CmbPrimary" Name="TypeofPrimary" />
        </SelectParameters>
    </asp:SqlDataSource>--%>

    <asp:SqlDataSource ID="dsCustomer" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select case when Code is null Or Code ='' then Custom else Code end as 'Code',Name from MasCustomer union select '0','None'">
    </asp:SqlDataSource>