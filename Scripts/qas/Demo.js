var DevAVPageName;
//var _menu;
function CreateClass(parentClass, properties) {
    var result = function () {
        if (result.preparing)
            return delete (result.preparing);
        if (result.ctor)
            result.ctor.apply(this);
    };
    result.prototype = {};
    if (parentClass) {
        parentClass.preparing = true;
        result.prototype = new parentClass;
        result.base = parentClass;
    }
    if (properties) {
        var ctorName = "constructor";
        for (var name in properties)
            if (name != ctorName)
                result.prototype[name] = properties[name];
        var ctor = properties[ctorName];
        if (ctor)
            result.ctor = ctor;
    }
    return result;
}
PageModuleBase = CreateClass(null, {

    PendingCallbacks: {},

    DoCallback: function (sender, callback) {
        if (sender.InCallback()) {
            Demo.PendingCallbacks[sender.name] = callback;
            sender.EndCallback.RemoveHandler(Demo.DoEndCallback);
            sender.EndCallback.AddHandler(Demo.DoEndCallback);
        } else {
            callback();
        }
    },

    DoEndCallback: function (s, e) {
        var pendingCallback = Demo.PendingCallbacks[s.name];
        if (pendingCallback) {
            pendingCallback();
            delete Demo.PendingCallbacks[s.name];
        }
    },

    ChangeDemoState: function (view, command, key) {
        var prev = this.DemoState;
        var current = { View: view, Command: command, Key: key };

        if (prev && current && prev.View == current.View && prev.Command == current.Command && prev.Key == current.Key)
            return;

        this.DemoState = current;
        this.OnStateChanged();
        this.ShowMenuItems();
    },

    OnStateChanged: function () { },
    ShowMenuItems: function () { },

    Adjust: function () {
        var bodySelector = LeftPanel.IsExpandable() ? "Portrait" : "Landscape";
        $('body').removeClass("Portrait").removeClass("Landscape").addClass(bodySelector);
        Demo.ChangeExpandButtonsVisibility(LeftPanel.IsExpandable(), LeftPanel.IsExpanded());
    },

    ChangeLeftPaneExpandedState: function (expand) {
        if (expand)
            LeftPanel.Expand();
        else
            LeftPanel.Collapse();
    },
    ChangeExpandButtonsVisibility: function (expandable, expand) {
        ClientExpandPaneImage.SetVisible(expandable && !expand);
        ClientCollapsePaneImage.SetVisible(expandable && expand);
    },

    // Site master
    ClientLayout_Init: function (s, e) {
		//
        ASPxClientUtils.AttachEventToElement(window, "resize", Demo.Adjust);
        if (ASPxClientUtils.touchUI) {
            ASPxClientUtils.AttachEventToElement(window, "orientationchange", function () {
                ASPxClientTouchUI.ensureOrientationChanged(Demo.Adjust);
            }, false);
        }
        Demo.Adjust();
        Demo.ShowMenuItems();
        Demo.ClientLayout_PaneResized();
    },
    ClientActionMenu_ItemClick: function (s, e) { },
    ClientLayout_PaneResized: function () { },

    ClientCollapsePaneImage_Click: function (s, e) {
        Demo.ChangeLeftPaneExpandedState(false);
        Demo.ChangeExpandButtonsVisibility(true, false);
    },
    ClientExpandPaneImage_Click: function (s, e) {
        Demo.ChangeLeftPaneExpandedState(true);
        Demo.ChangeExpandButtonsVisibility(true, true);
    },
    ClientInfoMenu_ItemClick: function (s, e) {
        if (e.item.parent && e.item.parent.name == "theme") {
            ASPxClientUtils.SetCookie("DemoCurrentTheme", e.item.name || "");
            e.processOnServer = true;
        }
    },
    //ClientInfoMenu_ItemClick: function (s, e) { },
    ClientSearchBox_KeyPress: function (s, e) {
        e = e.htmlEvent;
        if (e.keyCode === 13) {
            // prevent default browser form submission
            if (e.preventDefault)
                e.preventDefault();
            else
                e.returnValue = false;
        }
    },

    ClientSearchBox_KeyDown: function (s, e) {
        window.clearTimeout(Demo.searchBoxTimer);
        Demo.searchBoxTimer = window.setTimeout(function () {
            Demo.OnSearchTextChanged();
        }, 1200);
    },

    ClientSearchBox_TextChanged: function (s, e) {
        Demo.OnSearchTextChanged();
    },
     currentdate : function(){
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        var today = dd + '/' + mm + '/' + yyyy;
        return today;
    },
    fn_AllowonlyNumeric: function (s, e) {
        var theEvent = e.htmlEvent || window.event;
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
        var regex = /[0-9,]/;

        if (!regex.test(key)) {
            theEvent.returnValue = false;
            if (theEvent.preventDefault)
                theEvent.preventDefault();
        }
    },
    getUrlVars : function () {
        var vars = {};
        var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function(m,key,value) {
            vars[key] = value;
        });
        return vars;
    },
    OnSearchTextChanged: function () {
        window.clearTimeout(Demo.searchBoxTimer);
        var searchText = ClientSearchBox.GetText();
        if (ClientHiddenField.Get("SearchText") == searchText)
            return true;
        ClientHiddenField.Set("SearchText", searchText);
    },

    ClearSearchBox: function () {
        ClientHiddenField.Set("SearchText", "");
        ClientSearchBox.SetText("");
    },

    ShowLoadingPanel: function (element) {
        this.loadingPanelTimer = window.setTimeout(function () {
            ClientLoadingPanel.ShowInElement(element);
        }, 500);
    },
    //ClientMailTree_Init: function (s, e) {
    //    s.cpPrevSelectedNode = s.GetSelectedNode();
    //},
    //ClientMailTree_NodeClick: function (s, e) {
    //    if (s.cpPrevSelectedNode == s.GetSelectedNode())
    //        return;
    //    s.cpPrevSelectedNode = s.GetSelectedNode();
    //    //MailDemo.ClearSearchBox();
    //    //MailDemo.ShowMenuItems();
    //    //MailDemo.ChangeDemoState("MailList");
    //    //ClientMailGrid.cpResetVertPosition = true;
    //    //MailDemo.DoCallback(ClientMailGrid, function () {
    //    //    ClientMailGrid.PerformCallback("FolderChanged");
    //    //});
    //},
    HideLoadingPanel: function () {
        if (this.loadingPanelTimer > -1) {
            window.clearTimeout(this.loadingPanelTimer);
            this.loadingPanelTimer = -1;
        }
        ClientLoadingPanel.Hide();
    },
    PostponeAction: function (action, canExecute) {
        var f = function () {
            if (!canExecute())
                window.setTimeout(f, 50);
            else
                action();
        };
        f();
    }
});
MailPageModule = CreateClass(PageModuleBase, {
    constructor: function () {
        this.DemoState = { View: "MailList" };
    },
    OnStateChanged: function () {
        var state = this.DemoState;
        if (state.View == "MailList")
            this.ShowGrid();
        //if (state.View == "MailPreview")
        //    this.ShowPreview(state.Key);
        if (state.View == "MailForm")
            this.ShowForm(state.Command, state.Key);
    },
    ClientInfoMenu_ItemClick: function (s, e) {
        var command = e.item.name;
        if (command == "theme" || command == "help") return;
        var state = Demo.DemoState;
        if (command == "ExportToXLS" || command == "ExportToPDF" || command == "print") {
            ClientgridData.GetValuesOnCustomCallback("MailForm|" +command + "|" +state.Key, Demo.OnGetRowValues);
        }else
            if (e.item.parent) {
                ASPxClientUtils.SetCookie("DemoCurrentTheme", e.item.name || "");
                e.processOnServer = true;
            }
    },
    OnGetRowValues: function (values) {
        debugger;
        var state = Demo.DemoState;var url = "";
        if (values != null) {
            if (values["KeyValue"] == 0) return alert("data not found");
            var view = values["view"];
                var a = values["print"].split("|"), i;
                for (i = 0; i < a.length; i++) {
                url = "./PrintMails.aspx?usertype=0&view=" + view + "&Type=" + a[i] + "&Id=" + values["KeyValue"];
                    window.open(url, "_blank");
                }
        }
    },
    GetPopupControl: function () {
        return ASPxPopupClientControl;
    },
    GetChangesCount: function (batchApi) {
        var updatedCount = batchApi.GetUpdatedRowIndices().length;
        var deletedCount = batchApi.GetDeletedRowIndices().length;
        var insertedCount = batchApi.GetInsertedRowIndices().length;

        return updatedCount + deletedCount + insertedCount;
    },
    ClientActionMenu_ItemClick: function (s, e) {   
        debugger;
        var command = e.item.name;
        var state = Demo.DemoState;
        switch (command) {
            case "Filter":
                var popup = Demo.GetPopupControl();
                popup.Show();
                popup.BringToFront();
                break;
            case "Pending": case"All":
                hftype.Set("type", command == "Pending" ? "0" : "1");
                ClientgridData.PerformCallback(command);
                break;
            case "new":
            case "unread":
                Demo.ShowSelectedType(command, 0);
                break;
            case "reply":
                Demo.ChangeDemoState("MailForm", "Reply", state.Key);
                break;
            case "back":
                Demo.ChangeDemoState("MailList");
                break;
            case "delete":
                if (!window.confirm("Confirm Delete?"))
                    return;
                var keys = [];
                if (state.View == "MailList") {
                    keys = ClientgridData.GetSelectedKeysOnPage();
                } else if (state.View == "MailForm") {
                    keys = [state.Key];
                    Demo.ChangeDemoState("MailList");
                }
                if (keys.length > 0) {
                    Demo.DoCallback(ClientgridData, function () {
                        ClientgridData.PerformCallback("Delete|" + keys.join("|"));
                    });
                    Demo.MarkMessagesAsRead(true, keys);
                }
                break;
            case "send":
            case "save":
                if (window.ClientToEditor && !ASPxClientEdit.ValidateEditorsInContainerById("MailForm"))
                    return;
                    var value = ClientDisponsition.GetValue();
                    if (state.Key == 0)
                        if (!ClientCompany.GetValue()) {
                            ClientCompany.SetFocus();
                            return alert('The Proposed Factory should not be empty...');
                        }
                        else
                            ClientCompany.isValid = false;
                    var inputfrom = ClientValidfrom.GetValue();
                    var inputto = ClientValidto.GetValue();
                    if (inputfrom == "" || inputto == "" || inputfrom == null || inputto == null) {
                        return alert('The Valid Date should not be empty...');

                    } else
                        if (inputto < inputfrom) {
                            //Date greater than today's date 
                            return alert('fromdate is greather then to date.');
                        }
                    if ((state.Key == 0 || state.Key > 1) && value != "") {
                        var Disponsition = ClientDisponsition.GetText();
                        if (Disponsition != "") {
                            var i;
                            for (i = 0; i < 4; i++) {
                                tabbedGroupPageControl.SetActiveTabIndex(i);
                                if (!ASPxClientEdit.ValidateGroup('group1')) {
                                    return alert('Field is required');
                                }
                            }
                        }
                        if (!ClientReceiver.GetValue())
                            return alert('The Receiver should not be empty...');
                    }
                    //
                    ClientgridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + state.Key, function (values) {
                        var test = ClientDisponsition.GetValue(); 
                        //if (hfpositions.Get("Pos") == "USPN") {
                        var mytext = CmbAssignee.GetValue();
                        //} else mytext = CmbReason.GetText();
                        
                        if ((test == 3 || test == 5) && (mytext == null)) {
                            switch (test) {
                                case "3":
                                    alert('The Reason should not be empty...');
                                    break;
                                case "5": 
                                    alert('The Assignee should not be empty...');
                                    break;
                            }
                            //CmbReason.SetFocus();
                            Demo.ChangeDemoState("MailForm", "EditDraft", state.Key);
                            return;
                        }
                        if ((values["StatusApp"] != "0" && values["StatusApp"] != "-1" && values["StatusApp"] != "4" && values["StatusApp"] != "6") && (test == null || test == "0")) {
                            alert('The Disposition should not be empty...' + values["StatusApp"] + test);
                            ClientDisponsition.SetFocus();
                            Demo.ChangeDemoState("MailForm", "EditDraft", state.Key);
                            return;
                        } else {
                            var args = "SaveMail|" + state.Key;
                            Demo.ChangeDemoState("MailList");
                            Demo.DoCallback(ClientgridData, function () {
                                ClientgridData.PerformCallback(args);
                                UploadControl.Upload(); 
                            });
                        }
                    });
                break;
            case "read":
                Demo.ShowSelectedType(command, 0);
                break;
        }
    },
    ShowSelectedType: function (command, evt) {
        Demo.ChangeDemoState("MailForm", command, evt);
    },
    GettypeControl: function () {
        return popup;
    },
    ShowMenuItems: function () { 
        var view = Demo.DemoState.View;
        var state = Demo.DemoState;
        var edit = editor.Get("Name");
        var appr = approv.Get("approv");
        hfview.Set('view', view);
        var para = hfpara.Get("para").split("|");
        var b = (para[0] != "Revised" && para[0] != "Copied");
        ClientActionMenu.GetItemByName("new").SetVisible(view != "MailForm" && edit == "0");
        ClientActionMenu.GetItemByName("delete").SetVisible(view == "MailForm" && appr == "0" && state.Key != 1);
        ClientActionMenu.GetItemByName("save").SetVisible(view == "MailForm" && appr == "0");
        ClientActionMenu.GetItemByName("reply").SetVisible(view == "MailPreview");
        ClientActionMenu.GetItemByName("back").SetVisible(view != "MailList");
        ClientActionMenu.GetItemByName("Pending").SetVisible(view == "MailList" && hftype.Get("type")==1);
        ClientActionMenu.GetItemByName("All").SetVisible(view == "MailList" && hftype.Get("type") == 0);
        ClientActionMenu.GetItemByName("Filter").SetVisible(view == "MailList" && hftype.Get("type") == 1);
        var hasSelectedMails = ClientgridData.GetSelectedKeysOnPage().length > 0;
        ClientInfoMenu.GetItemByName("print").SetVisible(view == "MailForm" && state.Key != 0);

        ClientInfoMenu.GetItemByName("ExportToPDF").SetVisible(view == "MailForm" && state.Key != 0);
        ClientInfoMenu.GetItemByName("ExportToXLS").SetVisible(view == "MailForm" && state.Key != 0);
        ClientSearchBox.SetEnabled(view == "MailList");
    },
    ClientgridData_Init: function (s, e) {
        approv.Set("approv", "Vest");
        hfpara.Set('para', '0');
        hfGeID.Set("GeID", '0');
        Demo.UpdateMailGridKeyFolderHash();
    },
    ClientFormPanel_Init: function (s, e) {
        Demo.DoCallback(s, function() {
            s.PerformCallback();
        });
    },
    Init: function (s, e) {
        var key = Demo.getUrlVars()["ID"];
        if (!key)
            return;
        var form = Demo.getUrlVars()["form"];
        hfGeID.Set("GeID", key);
        Demo.ChangeDemoState("MailForm", form, key);
    },
    CostingSheet: function (s, e) {
        if (!ClientintGrid.cpKeyValue)
            return;
        var key = ClientintGrid.cpKeyValue;
        var args = key.split("|");
        for (var i = 0; i < args.length; i++) {
            (function (i) {
                console.log(i);
                var obj = usertp.Get("usertype");
                window.open('Default.aspx?viewMode=CostingEditForm&ID=' + args[i] + '&form=EditCost&UserType=' + obj, "_blank");
            })(i);
        }
        delete s.cpKeyValue; //flag = false;
    },
    OnEndCallback: function (s, e) {
        debugger;
        if (!s.cpKeyValue)
            return;
        if (s.cpKeyValue==0)
            alert(s.cpKeyValue);
        var key = s.cpKeyValue;
        if (key == 0)
            Demo.ChangeDemoState("MailList");
        else {
            var args = key.split("|");
            if (args[1] == "Edit") return alert('Can not be done');
                Demo.ChangeDemoState("MailForm", args[1], args[0]);
        }
        delete s.cpKeyValue;
    },
    MarkMessagesAsRead: function (read, keys) {
        var sendCallback = false;
    },

    ClientgridData_RowClick: function (s, e) {
        debugger;
        var src = ASPxClientUtils.GetEventSource(e.htmlEvent);
        if (src.tagName == "TD" && src.className.indexOf("dxgvCommandColumn") != -1) // selection cell
            return;
        if (!s.IsDataRow(e.visibleIndex))
            return;
        var key = s.GetRowKey(e.visibleIndex, "ID");
        s.GetRowValues(e.visibleIndex, 'RequestType', function (value) {
            Demo.ChangeDemoState("MailForm", "EditDraft", key);
        });
        hfpara.Set("para", "0");
        TabList.SetActiveTabIndex(0);
    },
    OnGetRowId: function (values)
    {
        usertp.Set("usertype", values[3]);
        var key = values[0];
        hfRequestType.Set("Type", values[1]);
        Demo.ChangeDemoState("MailForm", "EditDraft", key);
        TabList.SetActiveTabIndex(0);
    },
    OnSearchTextChanged: function () {
        var processed = MailPageModule.base.prototype.OnSearchTextChanged.call(Demo);
        if (processed) return;
        Demo.ChangeDemoState("MailList");
        Demo.DoCallback(ClientgridData, function () {
            ClientgridData.PerformCallback("Search");
        });
    },
    ClientLayout_PaneResized: function () {
        var state = Demo.DemoState;
        if (!state) return;
        if (DevAVPageName == "CustomerEditForm") {
            ClientgridData.SetHeight(0);
            ClientgridData.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
            ClientFormPanel.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
        }
    },
    ShowGrid: function () {
        ClientgridData.SetVisible(true);
    },
    ShowPreview: function (key) {
        Demo.HideLoadingPanel();
        Demo.HideGrid();
        Demo.HideForm();
        Demo.DoCallback(ClientPreviewPanel, function () {
            ClientPreviewPanel.PerformCallback(key);
        });
        Demo.MarkMessagesAsRead(true, [key]);
    },
    HideGrid: function () {
        ClientgridData.SetVisible(false);
    },
    SetEnabledForm: function (b) {
        ClientDisponsition.SetEnabled(b);
    },
    SetUnitPriceColumnVisibility: function () {
        var appr = approv.Get("approv");
        debugger;
        var v = hfStatusApp.Get("StatusApp") == "2";
        var disp = v && appr == '0' ? 'table-cell' : 'none';
        $('td.unitPriceColumn').css('display', disp);

    },
    ShowForm: function (command, key) {
        Demo.HideGrid();
        Demo.ClearForm();
        ClientFormPanel.SetVisible(command == "new" || command == "Reply" || command == "EditDraft");
        Demo.SetUnitPriceColumnVisibility();
        if (command == "new" || command == "Reply" || command == "EditDraft") {
            
            var obj = usertp.Get("usertype");
            var lTable = document.getElementById("Vat");
                lTable.style.display = obj == 0 ? 'block' : 'none';
            ClientShape.SetEnabled(obj == 0);
            formLayout.GetItemByName("PetCategory").SetCaption(obj == 0 ? "Pet Category" : "Product Category");
            formLayout.GetItemByName("PetFoodType").SetCaption(obj == 0 ? "Pet Food Type" : "Used to Pack");
            formLayout.GetItemByName("CompliedWith").SetCaption(obj == 0 ? "Complied With" : "Specification");
            formLayout.GetItemByName("layoutname").SetCaption(obj == 0 ? "Marketing Technical Request Form" : "New Request Form" );
            var arr = "Product_Type;Chunk_Type;Media_;Product_Stlye;Net_weight;Claims;NutrientProfile";
            arr = obj == 0 ? "Product_Requirement;Inner;Outer;SampleDate" : arr ;
            var g = arr.split(";"), s;
            for (s = 0; s < g.length; s++) {
                if (g[s] != "")
                    formLayout.GetItemByName(g[s]).SetVisible(false);
                    //formLayout.GetItemByName(g[s]).CssClass ='none;'
            }
        }
        if (command == "new") {
            ClientgridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;
                    Demo.SetEnabledForm(true);
                    ClientCompany.SetEnabled(true);
                    ClientReceiver.SetEnabled(true);
                    //ClientReceiver.PerformCallback("clear|0|0");
                    ClientValidfrom.SetText(null);
                    ClientValidto.SetText(null);
                    //ClientPackSize.GetInputElement().readOnly = false;
                    hfGeID.Set("GeID", 0);
                    hfpara.Set('para', '0|0');
                    ClientNewID.Set('NewID', values["NewID"]);
                    ClientDisponsition.PerformCallback(values["NewID"]);
                    gv.PerformCallback('reload|0');
					var b=true;
                    fileManager.PerformCallback(['load',b].join("|"));
                    approv.Set("approv", values["editor"]);
                    var role = hfRole.Get("Role");
                    TabList.GetTab(5).SetVisible(hfRole.Get("Role")!=0);
                };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientRequestNo });
            });
            Demo.ShowLoadingPanel(ClientFormPanel.GetMainElement());
        }
        if (command == "Reply" || command == "EditDraft") {
            ClientgridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;

                    //TabList.GetTab(5).SetVisible(hfRole.Get("Role") != 0);
                    hfRequestType.Set("Type", values["RequestType"]);
                    clienteventlog.SetValue(values["Eventlog"]);
                    ClientRequestNo.SetValue(values["RequestNo"]);
                    var b = (values["editor"] == "0");
                    Demo.SetEnabledForm(b);
                    ClientRequestType.SetText(values["Requestfor"]);
                    ClientCompany.SetValue(values["Company"]);
                    hfGeID.Set("GeID", values["ID"]);
                    ClientCompany.SetEnabled(values["ID"] == "0");
                    
                    ClientValidfrom.SetText(values["Validfrom"]);
                    ClientValidto.SetText(values["Validto"]);
                    ClientPetFoodType.SetValue(values["PetFoodType"]);
                    ClientReceiver.SetEnabled(values["StatusApp"] == 0);
                    var arge = ["load", values["Company"], values["PetCategory"],
                        values["Receiver"]].join("|");
                    ClientReceiver.SetText(values["ReceiverName"]);
                    hfReceiver.Set("Receiver", values["Receiver"]);
                    ClientPetCategory.PerformCallback(arge);
                    ClientCompliedWith.SetValue(values["CompliedWith"]);
                    ClientNutrientProfile.SetValue(values["NutrientProfile"]);
                    ClientRequestDate.SetText(values["CreateOn"]);
                    ClientNotes.SetValue(values["Notes"]);
                    ClientSize.SetText(values["Drainweight"]);
                    deSampleDate.SetText(values["SampleDate"]);
                    ClientPrimary.SetText(values["Packaging"]);
                    setprimary(values["Packaging"]);
                    ClientMaterial.SetText(values["Material"]);
                    ClientPackSize.SetText(values["PackSize"]);
                    ClientPFLid.SetValue(values["PackLid"]);
                    ClientLacquer.SetValue(values["PackLacquer"]);
                    ClientPFType.SetValue(values["PackType"]);
                    ClientDesign.SetValue(values["PackDesign"]);
                    hfSellingUnit.Set("SellingUnit", values["SellingUnit"]);
                    ClientColor.SetText(values["PackColor"]);
                    var tyuse = usertp.Get("usertype");
                    if (tyuse == '0') {

                        ClientProductType.SetValue(values["ProductType"]);
                        ClientChunkType.SetValue(values["ChunkType"]);
                        ClientMedia.SetValue(values["Media"]);
                        ClientProductStyle.SetValue(values["ProductStyle"]);
                        ClientNetweight.SetText(values["NetWeight"]);
                        ClientNetUnit.SetText(values["NetUnit"]);
                    } else {
                        tbOtherPrd.SetText(values["OtherPrd"]);
                        hfPrdRequirement.Set("PrdRequirement", values["PrdRequirement"]);
                    }
                    ClientProductNote.SetText(values["ProductNote"]);
                    ClientShape.SetText(values["PackShape"]);
                    ClientCustomerName.SetText(values["Customer"]);
                    ClientCustomerPrice.SetValue(values["Customprice"]);
                    ClientBrandName.SetText(values["Brand"]);
                    ClientDestination.SetText(values["Destination"]);
                    ClientESTVolume.SetText(values["ESTVolume"]);
                    ClientESTLaunching.SetText(values["ESTLaunching"]);
                    ClientESTFOB.SetText(values["ESTFOB"]);
                    hfIngredient.Set("Ingredient", values["Ingredient"]);
                    if (values["Concave"] != "")
                        rbConcave.SetValue(values["Concave"]);
 
                    TreeList.PerformCallback(values["Claims"]);
                    DropDownEdit.SetText(values["ClaimsText"]);

                    var c = values["Claims"].split(";"), x;
                    for (x = 0; x < c.length; x++) {
                        if (c[x] != "")
                            Clientclaims.SelectIndices(c[x]);
                    }
                    
                    ClientIngredientOther.SetText(values["IngredientOther"]);
                    ClientclaimsOther.SetText(values["ClaimsOther"]);
                    ClientCountry.SetText(values["Country"]);
                    if (values["Vet"]!="")
                        ClientVet.SelectIndices(values["Vet"]);
                    Clientetc.SetText(values["etc"]);
                    if (values["Physical"]!="")
                        ClientPhysical.SelectIndices(values["Physical"]);
                    ClientPhysicalUnit.SetText(values["PhysicalUnit"]);
                    if (values["Sample"] != "")
                        ClientSample.SelectIndices(values["Sample"]);
                    ClientSampleUnit.SetText(values["SampleUnit"]);
                    //ClientCustomer.SelectAll();
                    tbOtherRole.SetText(values["OtherRole"]);
                    hfCustomerRe.Set("CustomerRe", values["CustomerRequest"]);
                    var a = values["CustomerRequest"].split(";"), i;
                    if (values["Inner"] != "") {
                        var loopinner = values["Inner"].split(";"), n;
                        for (n = 0; n < loopinner.length; n++) {
                            ClientInner.SelectIndices(loopinner[n]);
                        }
                    }
                    tbInnerOther.SetText(values["InnerOther"]);
                    if (values["Outer"] != "") {
                        var loopOuter = values["Outer"].split(";"), o;
                        for (o = 0; o < loopOuter.length; o++) {
                            ClientOuter.SelectIndices(loopOuter[o]);
                        }
                    }
                    tbOuterOther.SetText(values["OuterOther"]);
                    //var selectedItemsCount = ClientCustomer.GetSelectedItems().length;
                    approv.Set("approv", values["editor"]);
                    //
                    hfStatusApp.Set('StatusApp', values["StatusApp"]);
                    var para = hfpara.Get("para").split("|");
                    //alert(para[0]);
                    if (para[0] == "Revised" || para[0] == "Copied") {
                        clienteventlog.SetText("");
                        ClientReceiver.SetEnabled(true);
                        if (para[0] == "Copied")
                            Demo.copied(true,values);
                        if (para[0] == "Revised") {
                            if (values["copied"] == 0)  //Status not current && inprocess
                              return alert("Can not be done");
                            else
                                Demo.copied(true, values);
                            gv.PerformCallback('reload|' + values["ID"]);
                        }
                    } else {
                        ClientDisponsition.PerformCallback(values["NewID"]);
                        ClientNewID.Set('NewID', values["NewID"]);
                        gv.PerformCallback('reload|' + values["ID"]);
                        //fileManager.PerformCallback(values["NewID"] + '|' + b);
                    }
                    //ClientPackSize.GetInputElement().readOnly = (approv.Get("approv")=="1");
                    var myString = "Reason";
                    var a = myString.split(";"), i;
                    for (i = 0; i < a.length; i++) {
                        formLayout.GetItemByName(a[i]).SetVisible(false);
                    }
                };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientRequestNo });
           });
            Demo.ShowLoadingPanel(ClientFormPanel.GetMainElement());
        }
    },
    copied: function (b, values) {
        approv.Set("approv", "0");
        var para = hfpara.Get("para").split("|");
        if (para[0] == "Copied") {
            Demo.SetEnabledForm(b);
            ClientCompany.SetEnabled(b);
            ClientRequestNo.SetText("");
            clienteventlog.SetText("");
            gv.PerformCallback('reload|0');
        }
        ClientDisponsition.PerformCallback(values["CopiedID"]);
        ClientDisponsition.SetEnabled(b)
        //fileManager.PerformCallback(values["CopiedID"] + '|' + b);
        ClientNewID.Set("NewID", values["CopiedID"]);
    },
    ClearForm: function () {
            var myString = "Vat";
            var a = myString.split(";"), i;
            for (i = 0; i < a.length; i++) {
                var lTable = document.getElementById(a[i]);
                lTable.style.display = 'block';
            }
            var arr = "Product_Type;Chunk_Type;Media_;Product_Stlye;Net_weight;Claims;NutrientProfile";
            arr = arr + ";Product_Requirement;Inner;Outer;SampleDate";
            var g = arr.split(";"), s;
            for (s = 0; s < g.length; s++) {
                if (g[s] != "")
                    formLayout.GetItemByName(g[s]).SetVisible(true);
            }
            CmbReason.SetText("");
            CmbAssignee.SetText("");
            ClientRequestNo.SetValue("");
            ClientRequestType.SetText("");
            //ClientMarketingNo.SetValue("");
            ClientCompany.SetValue("");
            ClientReceiver.SetValue("");
            ClientValidfrom.SetText(Demo.currentdate());
            ClientValidto.SetText(Demo.currentdate());
            ClientRequestDate.SetText(Demo.currentdate());
            //
            var DateTo = new Date(ClientRequestDate.GetDate());
            DateTo.setDate(DateTo.getDate() + 7);  

            deSampleDate.SetDate(DateTo);
            ClientPetFoodType.SetValue("");
            ClientPetCategory.SetValue("");
            ClientCompliedWith.SetValue("");
            var _Width =ClientCompliedWith.GetWidth();
            //CmbAssign.SetWidth(_Width);
            ClientNutrientProfile.SetValue("");
            ClientNotes.SetValue("");
            ClientSize.SetText("");
            ClientColor.SetText("");
            ClientPrimary.SetValue("");
            //checkComboBox.SetText("");
            ClientMaterial.SetValue("");
            ClientPackSize.SetText("");
            ClientPFLid.SetValue("");
            ClientLacquer.SetValue("");
            ClientPFType.SetValue("");
            ClientDesign.SetValue("");
            ClientSellingUnit.SetValue("");
            ClientProductType.SetValue("");
            ClientChunkType.SetValue("");
            ClientMedia.SetValue("");
            ClientProductStyle.SetValue("");
            ClientNetweight.SetText("");
            ClientNetUnit.SetText("");
            ClientProductNote.SetText("");
            ClientShape.SetText("");
            ClientCustomerName.SetText("");
            ClientCustomerPrice.SetValue("");
            ClientBrandName.SetText("");
            ClientDestination.SetText("");
            ClientESTVolume.SetText("");
            ClientESTLaunching.SetText("");
            ClientESTFOB.SetText("");
            //ClientIngredient.SetText("");
            //ClientIngredient.UnselectAll();
            ClientIngredientOther.SetText("");
            //Clientclaims.SetText("");
            Clientclaims.UnselectAll();
            ClientclaimsOther.SetText("");
            //ClientCustomer.UnselectAll();
            ClientVet.UnselectAll();
            Clientetc.SetText("");
            ClientPhysical.UnselectAll();
            ClientOuter.UnselectAll();
            ClientInner.UnselectAll();
            tbInnerOther.SetText("");
            tbOuterOther.SetText("");
            ClientPhysicalUnit.SetText("");
            ClientSample.UnselectAll();
            //ClientPrdRequirement.UnselectAll();
            ClientSampleUnit.SetText("");
            ClientDisponsition.SetText("");
            ClientComment.SetText("");
            ClientCountry.SetText("");
            //fileManager.SetVisible(false); 
            //UploadControl.SetVisible(false);
            var myString = "Assignee;Reason";
            var a = myString.split(";"), i, obj;
            for (i = 0; i < a.length; i++) {
                formLayout.GetItemByName(a[i]).SetVisible(false);
                }
    },
    HideForm: function () {
        //if (window.ClientAddressBookPopup)
        //    ClientAddressBookPopup.Hide();
        ClientFormPanel.SetVisible(false);
    },
    UpdateMailGridKeyFolderHash: function () {
        var hash = {};
        for (var folderName in ClientgridData.cpVisibleMailKeysHash) {
            var keys = ClientgridData.cpVisibleMailKeysHash[folderName];
            if (!keys || keys.length == 0)
                continue;
            hash[folderName] = [];
            for (var i = 0; i < keys.length; i++)
                hash[keys[i]] = folderName;
        }
        ClientgridData.cpKeyFolderHash = hash;
    }
});
UpdatePriceModule = CreateClass(PageModuleBase, {
    constructor: function () {
        this.DemoState = { View: "MailList" };
    },
    OnStateChanged: function () {
        var state = this.DemoState;
        if (state.View == "MailList")
            this.ShowGrid();
        if (state.View == "MailForm")
            this.ShowForm(state.Command, state.Key);
    },
    ClientInfoMenu_ItemClick: function (s, e) {
        //
        var command = e.item.name;
        if (command == "theme" || command == "help") return;
        var state = Demo.DemoState;
        if (command == "ExportToXLS" || command == "ExportToPDF" || command == "print") {
            ClientgridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + state.Key, Demo.OnGetRowValues);

        } else
            if (e.item.parent) {
                ASPxClientUtils.SetCookie("DemoCurrentTheme", e.item.name || "");
                e.processOnServer = true;
            }
    },
    OnGetRowValues: function (values) {
        debugger;
        var state = Demo.DemoState; var url = "";
        if (values != null) {
            if (values["KeyValue"] == 0) return alert("data not found");
            var view = values["view"];
            var a = values["print"].split("|"), i;
            for (i = 0; i < a.length; i++) {
                url = "./PrintMails.aspx?usertype=0&view=" + view + "&Type=" + a[i] + "&Id=" + values["KeyValue"];
                window.open(url, "_blank");
            }
        }
    },
    GetPopupControl: function () {
        return ASPxPopupClientControl;
    },
    GetChangesCount: function (batchApi) {
        var updatedCount = batchApi.GetUpdatedRowIndices().length;
        var deletedCount = batchApi.GetDeletedRowIndices().length;
        var insertedCount = batchApi.GetInsertedRowIndices().length;

        return updatedCount + deletedCount + insertedCount;
    },
    ClientActionMenu_ItemClick: function (s, e) {
        debugger;
        var command = e.item.name;
        var state = Demo.DemoState;
        switch (command) {
            case "Filter":
                var popup = Demo.GetPopupControl();
                popup.Show();
                popup.BringToFront();

                //filter.SetVisible(true);
                break;
            case "Pending": case "All":
                //alert(command);
                hftype.Set("type", command == "Pending" ? "0" : "1");
                ClientgridData.PerformCallback(command);
                break;
            case "new":
            case "unread":
                Demo.ShowSelectedType(command, 1);
                glAssignee.SetValue(null);
                var grid = glAssignee.GetGridView();
                grid.UnselectRows();
                grid.Refresh();
                break;
            case "reply":
                Demo.ChangeDemoState("MailForm", "Reply", state.Key);
                break;
            case "back":
                Demo.ChangeDemoState("MailList");
                break;
            case "delete":
                if (!window.confirm("Confirm Delete?"))
                    return;
                var keys = [];
                if (state.View == "MailList") {
                    keys = ClientgridData.GetSelectedKeysOnPage();
                } else if (state.View == "MailForm") {
                    keys = [state.Key];
                    Demo.ChangeDemoState("MailList");
                }
                if (keys.length > 0) {
                    Demo.DoCallback(ClientgridData, function () {
                        ClientgridData.PerformCallback("Delete|" + keys.join("|"));
                    });
                    Demo.MarkMessagesAsRead(true, keys);
                }
                break;
            case "send":
            case "save":
                if (window.ClientToEditor && !ASPxClientEdit.ValidateEditorsInContainerById("MailForm"))
                    return;
 
                    if (state.Key == 1)
                        if (!ASPxClientEdit.ValidateGroup('group2'))
                            return alert('Field is required');

                    Demo.ChangeDemoState("MailList");
                    Demo.DoCallback(ClientgridData, function () {
                        ClientgridData.PerformCallback("Edit|" + state.Key);
                    });

 
                
                //alert(state.Key);
                break;
            case "read":
                Demo.ShowSelectedType(command, 0);
                break;
        }
    },
    ShowSelectedType: function (command, evt) {
        Demo.ChangeDemoState("MailForm", command, evt);
    },
    GettypeControl: function () {
        return popup;
    },
    ShowMenuItems: function () {
        var view = Demo.DemoState.View;
        var state = Demo.DemoState;
        var edit = editor.Get("Name");
        var appr = approv.Get("approv");
 
        ClientActionMenu.GetItemByName("new").SetVisible(view != "MailForm" && edit == "0");
        ClientActionMenu.GetItemByName("delete").SetVisible(view == "MailForm" && appr == "0" && state.Key != 1);
        ClientActionMenu.GetItemByName("save").SetVisible(view == "MailForm" && appr == "0");
        ClientActionMenu.GetItemByName("reply").SetVisible(view == "MailPreview");
        ClientActionMenu.GetItemByName("back").SetVisible(view != "MailList");
        ClientActionMenu.GetItemByName("Pending").SetVisible(view == "MailList" && hftype.Get("type") == 1);
        ClientActionMenu.GetItemByName("All").SetVisible(view == "MailList" && hftype.Get("type") == 0);
        ClientActionMenu.GetItemByName("Filter").SetVisible(view == "MailList" && hftype.Get("type") == 1);
        var hasSelectedMails = ClientgridData.GetSelectedKeysOnPage().length > 0;

        ClientInfoMenu.GetItemByName("print").SetVisible(view == "MailForm" && state.Key != 0);

        ClientInfoMenu.GetItemByName("ExportToPDF").SetVisible(view == "MailForm" && state.Key != 0);
        ClientInfoMenu.GetItemByName("ExportToXLS").SetVisible(view == "MailForm" && state.Key != 0);
        ClientSearchBox.SetEnabled(view == "MailList");
    },
    ClientgridData_Init: function (s, e) {
        approv.Set("approv", "Vest");
        hfpara.Set('para', '0');
        hfGeID.Set("GeID", '0');
        UploadCustom.SetEnabled(false);
        Demo.UpdateMailGridKeyFolderHash();
    },
    ClientFormPanel_Init: function (s, e) {
        Demo.DoCallback(s, function () {
            s.PerformCallback();
        });
    },
    Init: function (s, e) {
        debugger;
        var key = Demo.getUrlVars()["ID"];
        if (!key)
            return;
        hfGeID.Set("GeID", key);
        Demo.ChangeDemoState("MailForm", "Edit", key);
    },
    CostingSheet: function (s, e) {
        if (!ClientintGrid.cpKeyValue)
            return;
        var key = ClientintGrid.cpKeyValue;
        var args = key.split("|");
        for (var i = 0; i < args.length; i++) {
            (function (i) {
                console.log(i);
                var obj = usertp.Get("usertype");
                window.open('Default.aspx?viewMode=CostingEditForm&ID=' + args[i] + '&form=EditCost&UserType=' + obj, "_blank");
            })(i);
        }
        delete s.cpKeyValue; //flag = false;
    },
    OnEndCallback: function (s, e) {
        debugger;
        if (!s.cpKeyValue)
            return;
        //popup.Show();
        //popup.PerformCallback(gv.cpKeyValue);
        if (s.cpKeyValue == 0)
            alert(s.cpKeyValue);
        var key = s.cpKeyValue;
        //s.cpKeyValue = null;
        if (key == 0)
            Demo.ChangeDemoState("MailList");
        else {
            var args = key.split("|");
            if (args[1] == "Edit") return alert('Can not be done');
            Demo.ChangeDemoState("MailForm", args[1], args[0]);
        }
        delete s.cpKeyValue;
    },
    MarkMessagesAsRead: function (read, keys) {
        var sendCallback = false;
    },

    ClientgridData_RowClick: function (s, e) {
        debugger;
        var src = ASPxClientUtils.GetEventSource(e.htmlEvent);
        if (src.tagName == "TD" && src.className.indexOf("dxgvCommandColumn") != -1) // selection cell
            return;
        if (!s.IsDataRow(e.visibleIndex))
            return;
        var key = s.GetRowKey(e.visibleIndex, "ID");
        //s.GetRowValues(e.visibleIndex, 'ID;RequestType;StatusApp;UserType', Demo.OnGetRowId);
        s.GetRowValues(e.visibleIndex, 'RequestType', function (value) {
            Demo.ChangeDemoState("MailForm", "Edit", key);
        });
        hfpara.Set("para", "0");
    },
    OnGetRowId: function (values) {
        usertp.Set("usertype", values[3]);
        var key = values[0];
        Demo.ChangeDemoState("MailForm", "Edit", key);
        //TabList.SetActiveTabIndex(0);
    },
    OnSearchTextChanged: function () {
        var processed = MailPageModule.base.prototype.OnSearchTextChanged.call(Demo);
        if (processed) return;
        Demo.ChangeDemoState("MailList");
        Demo.DoCallback(ClientgridData, function () {
            ClientgridData.PerformCallback("Search");
        });
    },
    ClientLayout_PaneResized: function () {
        var state = Demo.DemoState;
        if (!state) return;
        if (DevAVPageName == "UpdatePriceForm") {
            ClientgridData.SetHeight(0);
            ClientgridData.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
            ClientFormPanel.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
        }
    },
    ShowGrid: function () {
        ClientgridData.SetVisible(true);
        //Demo.ClientLayout_PaneResized();
    },
    ShowPreview: function (key) {
        Demo.HideLoadingPanel();
        Demo.HideGrid();
        Demo.HideForm();
        ClientFormPanel.SetContentHtml("");
        ClientFormPanel.SetVisible(true);
        Demo.DoCallback(ClientFormPanel, function () {
            ClientFormPanel.PerformCallback(key);
        });
        Demo.MarkMessagesAsRead(true, [key]);
    },
    HideGrid: function () {
        ClientgridData.SetVisible(false);
    },
    SetEnabledForm: function (b) {
        ClientCompanyEdit.SetEnabled(b);
        //ClientDisponsition.SetEnabled(b);
        UploadCustom.SetEnabled(b);
    },
    SetUnitPriceColumnVisibility: function () {
        var appr = approv.Get("approv");
        debugger;
        var v = hfStatusApp.Get("StatusApp") == "2";
        var disp = v && appr == '0' ? 'table-cell' : 'none';
        $('td.unitPriceColumn').css('display', disp);

    },
    ShowForm: function (command, key) {
        Demo.HideGrid();
        Demo.ClearForm();
        ClientFormPanel.SetVisible(command == "new" || command == "Edit");
        Demo.SetUnitPriceColumnVisibility();
        if (command == "new") {
            ClientgridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;
                    ClientintGrid.PerformCallback('AddRow|1|' + values["RequestType"]);
                    ClientCompanyEdit.SetEnabled(true);
                    ClientValidfromEdit.SetText(null);
                    ClientValidtoEdit.SetText(null);
                    ClientNewID.Set("NewID", values["NewID"]);
                    //ClientType.SetSelectedIndex(0);
                    //ClientCompanyEdit.GetInputElement().readOnly = true;
                    approv.Set("approv", values["editor"]);
                };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.tbRequestNoEdit });
            });
            Demo.ShowLoadingPanel(ClientFormPanel.GetMainElement());
        }
        if (command == "Edit") {
            ClientgridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;
                    ClientCompanyEdit.SetValue(values["Company"]);
                    var comb = values["ID"] != "0";
                    ClientCompanyEdit.SetEnabled(comb);
                    var b = (values["editor"] == "0");
                    //ClientCompanyEdit.GetInputElement().readOnly = b;
                    //ClientType.SetValue(values["UserType"]);
                    //
                    hfRequestType.Set("Type", values["RequestType"]);
                    hfGeID.Set("GeID", values["ID"]);
                    hfStatusApp.Set('StatusApp', values["StatusApp"]);
                    Demo.SetEnabledForm(b);
                    tbRequestNoEdit.SetText(values["RequestNo"]);
                    tbRemarkEdit.SetText(values["Remark"]);
                    approv.Set("approv", values["editor"]);
                    ClientValidfromEdit.SetText(values["Validfrom"]);
                    ClientValidtoEdit.SetText(values["Validto"]);
                    ClientCustomPriceEdit.SetValue(values["Customprice"]);
                    ClientCustomEdit.SetValue(values["Customer"]);
                    ClientRequestDateEdit.SetText(values["CreateOn"]);
                    UploadCustom.SetEnabled(values["StatusApp"] == 0);
                    ClientCompanyEdit.SetEnabled(values["StatusApp"] == 0);
                    //
                    ClientNewID.Set("NewID", values["NewID"]);
                    //hfDisponsition.Set('Value', '7');
                    glAssignee.SetText(values["assignee"]);
                    ClientintGrid.PerformCallback("reload|" + values["ID"]);
                    //ASPxFormLayout1.GetItemByName("Upload").SetVisible(false);
                    //ActionMenu.SetVisible(true);
                };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.tbRequestNoEdit });
            });
            Demo.ShowLoadingPanel(ClientFormPanel.GetMainElement());
        } 
    },
    ClearForm: function () {
        //upload material
        //clienteventlog.SetText("");
        tbRequestNoEdit.SetText("");
        tbRemarkEdit.SetText("");
        ClientCompanyEdit.SetValue("");
        ClientCustomPriceEdit.SetValue("");
        ClientValidfromEdit.SetText(Demo.currentdate());
        ClientValidtoEdit.SetText(Demo.currentdate());
        ClientRequestDateEdit.SetText(Demo.currentdate());
        //ClientType.SetText("");
        //var myString = "Assignee;Reason";
        //var a = myString.split(";"), i, obj;
        //for (i = 0; i < a.length; i++) {
        //    formLayout.GetItemByName(a[i]).SetVisible(false);
        //}
    },
    HideForm: function () {
        //if (window.ClientAddressBookPopup)
        //    ClientAddressBookPopup.Hide();
        ClientFormPanel.SetVisible(false);
    },
    UpdateMailGridKeyFolderHash: function () {
        var hash = {};
        for (var folderName in ClientgridData.cpVisibleMailKeysHash) {
            var keys = ClientgridData.cpVisibleMailKeysHash[folderName];
            if (!keys || keys.length == 0)
                continue;
            hash[folderName] = [];
            for (var i = 0; i < keys.length; i++)
                hash[keys[i]] = folderName;
        }
        ClientgridData.cpKeyFolderHash = hash;
    }
});
CostPageModule = CreateClass(PageModuleBase, {
    constructor: function () {
        this.DemoState = { View: "MailList" };
    },
    OnStateChanged: function () {
        var state = this.DemoState;
        if (state.View == "MailList")
            this.ShowCostGrid();
        if (state.View == "MailPreview")
            this.ShowCostPreview(state.Key);
        if (state.View == "MailForm")
            this.ShowCostForm(state.Command, state.Key);
    },
    ClientInfoMenu_ItemClick: function (s, e) {
        var command = e.item.name;
        if (command == "theme" || command == "help" || command == "print") return;
        var state = Demo.DemoState;
            if (state.Key == 0) return alert("data not found");
        if (command == "ExportToXLS" || command == "ExportToPDF") {
            if (state.View == "MailList" && command == "ExportToXLS")
                ClientCostGrid.GetSelectedFieldValues("ID", Demo.GetSelectedFieldValuesCallback);
            else {
                var url = "./PrintMails.aspx?view=1&Type=0&Id=" + state.Key;
                window.open(url, "_blank");
                ClientInfoMenu.GetItemByName('print').SetNavigateUrl(url);
            }
        }else
            if (e.item.parent) {
                ASPxClientUtils.SetCookie("DemoCurrentTheme", e.item.name || "");
                e.processOnServer = true;
            }
    },
    GetSelectedFieldValuesCallback: function (values) {
        //
        var arr1 = [];
        for (var i = 0; i < values.length; i++) {
            arr1.push(values[i]);
            var url = "./PrintMails.aspx?view=1&Type=0&Id=" + values[i];
            window.open(url, "_blank");
            ClientInfoMenu.GetItemByName('print').SetNavigateUrl(url);
        }
    },
    GetPopupControl: function () {
        return ASPxPopupClientControl;
    },
    ClientActionMenu_ItemClick: function (s, e) {
        //
        var command = e.item.name;
        var state = Demo.DemoState;
        switch (command) {
            case "Filter":
                //alert("Filter");
                var popup = Demo.GetPopupControl();
                popup.Show();
                popup.BringToFront();         
                break;
            case "Pending": case "All":
                //alert(command);
                hftype.Set("type", command == "Pending" ? "0" : "1");
                ClientCostGrid.PerformCallback(command);
                break;
            case "upload":
                Demo.ChangeDemoState("MailForm", "upload", 0);
                break;
            case "new":
                Demo.ChangeDemoState("MailForm", "New",0);
                //if (state.Command === "EditDraft")
                //var value = "New|0";
                //Demo.DoCallback(ClientCostGrid, function () {
                //    ClientCostGrid.PerformCallback(value);
                //});
                break;
            case "reply":
                Demo.ChangeDemoState("MailForm", "Reply", state.Key);
                break;
            case "back":
                Demo.ChangeDemoState("MailList");
                break;
            case "delete":
                if (!window.confirm("Confirm Delete?"))
                    return;
                var keys = [];
                if (state.View == "MailList") {
                    keys = ClientCostGrid.GetSelectedKeysOnPage();
                } else if (state.View == "MailForm") {
                    keys = [state.Key];
                    Demo.ChangeDemoState("MailList");
                }
                if (keys.length > 0) {
                    Demo.DoCallback(ClientCostGrid, function () {
                        ClientCostGrid.PerformCallback("Delete|" + keys.join("|"));
                    });
                    Demo.MarkMessagesAsRead(true, keys);
                }
                break;
            case "send":
            case "save":
                if (window.ClientCostingNo && !ASPxClientEdit.ValidateEditorsInContainerById("MailForm"))
                    return;
                if (!ASPxClientEdit.ValidateGroup('group1'))
                    return alert('Field is required');
                //var edit = editor.Get("Name");
                //Demo.update_grid();
                if (seExchangeRate.GetText()=='' || seExchangeRate.GetText() == '0')
                    return alert('The Exchange Rate should not be empty or zero...');
                var test = ClientDisponsition.GetValue();
                ClientCostGrid.GetValuesOnCustomCallback("MailForm|" + command + "|" + state.Key, function (values) {
                   
                    if ((test == 3 || test == 5 || test == 7) && (CmbReason.GetText() == "")) {
                        switch (test) {
                            case "3":
                                alert('The Reason should not be empty...');
                                break;
                            case "5": case "7":
                                alert('The Assignee should not be empty...');
                                break;
                        }
                        CmbReason.SetFocus();
                        Demo.ChangeDemoState("MailForm", "EditDraft", state.Key);
                        return;
                    }
                    if ((values["StatusApp"] != "2" && values["StatusApp"] != "-1") && (test == null || test == "0")) {
                        //alert(values["StatusApp"]);
                        alert('The Disposition should not be empty...');
                        ClientDisponsition.SetFocus();
                        Demo.ChangeDemoState("MailForm", "EditDraft", state.Key);
                        return;
                    } else {
                        //alert(state.Key);
                        debugger;
                        var args = command == "send" || command =="save" ? "SendMail" : "SaveMail";
                        if (state.Command === "EditDraft")
                            args += "|" + state.Key;
                        if (state.Command === "New")
                            args += "|" + 0;
                        Demo.ChangeDemoState("MailList");
                        Demo.DoCallback(ClientCostGrid, function () {
                            UploadControl.Upload(); 
                            //grid.UpdateEdit();
                            //grid.PerformCallback('savedata');

                            ClientCostGrid.PerformCallback(args);
                            testgrid.PerformCallback("post|" + args);
                        });
                    }
                });
                break;
            case "read":
            case "unread":
                var selectedKeys = ClientCostGrid.GetSelectedKeysOnPage();
                if (selectedKeys.length == 0)
                    return;
                ClientCostGrid.UnselectAllRowsOnPage();
                Demo.MarkMessagesAsRead(command == "read", selectedKeys);
                break;
            case "ExportToXLS":
                alert("test");
                break;
        }
    },
    update_grid: function () {
            gvCode.UpdateEdit();
            grid.UpdateEdit();
            testgrid.UpdateEdit();
    },
    OnSearchTextChanged: function () {
        var processed = MailPageModule.base.prototype.OnSearchTextChanged.call(Demo);
        if (processed) return;
        Demo.ChangeDemoState("MailList");
        Demo.DoCallback(ClientCostGrid, function () {
            ClientCostGrid.PerformCallback("Search");
        });
    },
    ClientLayout_PaneResized: function () {
        if (DevAVPageName == "CostingEditForm") {
            ClientCostGrid.SetHeight(0);
            ClientCostGrid.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
            ClientCostFormPanel.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
            //ClientCostFormPanel.GetMainElement().scrollTop = ClientCostFormPanel.GetMainElement().scrollHeight;
        }
    },
    ClientCostGrid_RowClick: function (s, e) {
        debugger;
        var src = ASPxClientUtils.GetEventSource(e.htmlEvent);
        if (src.tagName == "TD" && src.className.indexOf("dxgvCommandColumn") != -1) // selection cell
            return;
        if (!s.IsDataRow(e.visibleIndex))
            return;
        var key = s.GetRowKey(e.visibleIndex);
        //if (ClientMailTree.GetSelectedNode().name === "Drafts")
        Demo.ChangeDemoState("MailForm", "EditDraft", key);
        tcDemos.SetActiveTabIndex(0);
        //else
        //    Demo.ChangeDemoState("MailPreview", "", key);
    },
    ShowMenuItems: function () {
        var view = Demo.DemoState.View;
        var state = Demo.DemoState;
        var edit = editor.Get("Name");
        var appr = approv.Get("approv");

        ClientActionMenu.GetItemByName("new").SetVisible(view != "MailForm" && edit == "0");
        ClientActionMenu.GetItemByName("save").SetVisible(view == "MailForm" && appr == "0");
        ClientActionMenu.GetItemByName("delete").SetVisible(view == "MailForm" && edit == "0" && appr == "0" && state.Key != 0);
        ClientActionMenu.GetItemByName("reply").SetVisible(view == "MailPreview");
        ClientActionMenu.GetItemByName("back").SetVisible(view != "MailList");
        ClientActionMenu.GetItemByName("Pending").SetVisible(view == "MailList" && hftype.Get("type") == 1);
        ClientActionMenu.GetItemByName("All").SetVisible(view == "MailList" && hftype.Get("type") == 0);
        var hasSelectedMails = ClientCostGrid.GetSelectedKeysOnPage().length > 0;
        //ClientActionMenu.GetItemByName("delete").SetVisible(view == "MailList" && hasSelectedMails || view == "MailPreview");

        //var selectedNode = ClientMailTree.GetSelectedNode();
        //var showMarkAs = view == "MailList" && hasSelectedMails && selectedNode.name != "Sent Items" && selectedNode.name != "Drafts";
        //ClientActionMenu.GetItemByName("markAs").SetVisible(showMarkAs);
        //ClientInfoMenu.GetItemByName("print").SetVisible(view == "MailForm");
        ClientInfoMenu.GetItemByName("print").SetVisible(true);
        //ClientInfoMenu.GetItemByName("print").SetVisible(view == "MailList" || view == "MailForm");
        ClientSearchBox.SetEnabled(view == "MailList");
    },
    ShowCostGrid: function () {
        Demo.HideLoadingPanel();
//        Demo.HideCostPreview();
        Demo.HideCostForm();

        ClientCostGrid.SetVisible(true);
        Demo.ClientLayout_PaneResized();
    },
    HideCostGrid: function () {
        ClientCostGrid.SetVisible(false);
    },
    //ShowPreview: function (key) {
    //    MailDemo.HideLoadingPanel();
    //    MailDemo.HideMailGrid();
    //    MailDemo.HideMailForm();

    //    ClientMailPreviewPanel.SetContentHtml("");
    //    ClientMailPreviewPanel.SetVisible(true);
    //    MailDemo.DoCallback(ClientMailPreviewPanel, function () {
    //        ClientMailPreviewPanel.PerformCallback(key);
    //    });
    //    MailDemo.MarkMessagesAsRead(true, [key]);
    //},
    SetEnabledForm: function (b) {
        //action
        ClientDisponsition.SetEnabled(b);
        UploadControl.SetEnabled(b);
    },
    SetReadOnly : function (b) {
        ClientCompany.SetEnabled(b);
        ClientMarketingNo.SetEnabled(b);
        ClientReference.SetEnabled(b);
        ClientPackSize.SetEnabled(b);
        tbCanSize.SetEnabled(b);
        CmbPackaging.SetEnabled(b);
        ClientNetweight.SetEnabled(b);
        ClientNetUnit.SetEnabled(b);
    },
    //HideCostPreview: function () {
    //    ClientMailPreviewPanel.SetVisible(false);
    //},
    //Showtestgrid: function(index){
    //    formLayout.GetItemByName("_Grid").SetVisible(index == 0);
    //    formLayout.GetItemByName("_testgrid").SetVisible(index != 0);
    //},
    ClearForm: function () {
        clienteventlog.SetText("");
        CmbReason.SetValue("");
        ClientCompany.SetValue("");
        ClientCostingNo.SetValue("");
        //ClientCostingNo.SetIsValid(true);
        ClientMarketingNo.SetValue("");
        ClientPackSize.SetValue("");
        tbCanSize.SetText("");
        CmbPackaging.SetValue("");
        ClientNetweight.SetText("");
        ClientNetUnit.SetText("");
        ClientNotes.SetText("");
        ClientReference.SetText("");
        //tbProductName.SetText("");
        //ClientReference.SetText("");
        //tbMaterial.SetText("");
        //cbCompleted.SetChecked(false);
        //ClientRDNumber.Focus();
        //++++Primary Package
        ClientPackageCode.SetText("");
        tbDescription.SetText("");
        tbQuantity.SetText("");
        tbAmount.SetText("");
        tbPriceRate.SetText("");
        //tbPer.SetText("");
        CmbSellingUnit.SetText("");
        //++++Secondary Packaging
        ClientSecPackageCode.SetText("");
        tbSecDescription.SetText("");
        tbSecQuantity.SetText("");
        tbSecAmount.SetText("");
        tbSecPriceRate.SetText("");
        //tbSecPer.SetText("");
        CmbSecSellingUnit.SetText("");
        //++++Labor & Overhead
        //CmbLaborOverhead.SetText("");
        //tbLaborOverhead.SetText("");
        //tbLbOhRate.SetText("");
        //CmbLbOhCurrency.SetText("");
        //++++Margin
        ClientMargin.SetText("");
        tbMarginName.SetText("");
        tbMarginRate.SetText("");

        cmbLaborOverhead.SetText("");
        tbResultLOH.SetText("");
        cmbLOHType.SetText("");

        //tbMaterial.SetText("");
        seExchangeRate.SetText("");
        tbCustomer.SetText("");
        //tbtbMarginpct.SetText("");
        //++++Loss
        //CmbLoss.SetText("");
        //tbGrade.SetText("");
        //tbStyle.SetText("");
        //tbOHCANsize.SetText("");
        //tbpctLoss.SetText("");
        //grid.PerformCallback('clear');
        //tabbedGroupPageControl.SetActiveTabIndex(0);
        //Demo.Showtestgrid(0);
    },
    ShowCostForm: function (command, key) {
        //
        Demo.HideCostGrid();
        Demo.ClearForm();
        //    Demo.HideCostPreview();
        var appr = editor.Get("Name");
        ClientPreviewPanel.SetContentHtml("");
        if (appr == 3) {
            //ClientPreviewPanel.SetVisible(true);
            //Demo.DoCallback(ClientPreviewPanel, function () {
            //    ClientPreviewPanel.PerformCallback(key);
            //});
            Demo.ChangeDemoState("MailList");
            return alert("You are not authorized to access this page.");
        }
        else
        ClientCostFormPanel.SetVisible(true);
        //formLayout.GetItemByName("LBOh").SetVisible(false);
        //ClientRDNumber.AdjustControl();
        Demo.ClientLayout_PaneResized();
        if (command == "New") {
            ClientCostGrid.GetValuesOnCustomCallback("MailForm|" + command + "|" +key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;
                    hfid.Set('hidden_value', 0);
                    hfFolio.Set('Folio', 0);
                    hfStatusApp.Set('StatusApp', 2);
                    ClientNewID.Set('NewID', values["NewID"]);
                    testgrid.PerformCallback('reload|0');
                    gvCode.PerformCallback('reload|0');
                    grid.PerformCallback('AddRow|0');
                    ClientDisponsition.PerformCallback(values["NewID"]);
                    approv.Set("approv", values["editor"]);
                    //ClientCompany.SetEnabled(true);
                    ClientCompany.PerformCallback('AddRow|0');
                    ClientCompany.SetEnabled(true);
                    var b = true;
                    fileManager.PerformCallback(['load', b].join("|"));
                    //formLayout.GetItemByName("Offer").SetVisible(false);
                    //formLayout.GetItemByName("Containers").SetVisible(false);
                    //ClientCompany.GetInputElement().readOnly = true;
                    //alert("test");
                    //formLayout.GetItemByName("Display").SetVisible(false);
                    //formLayout.GetItemByName("Upload").SetVisible(true);
                    //ASPxMenu2.SetVisible(false);
                    //ASPxMenu1.SetEnabled(true);
                    Demo.SetReadOnly(true);
            };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientMarketingNo });
            });
            Demo.ShowLoadingPanel(ClientCostFormPanel.GetMainElement());
        }
            //EditCost
        //if (command == "EditCost") {
        //    ClientCostGrid.GetValuesOnCustomCallback("MailForm|" + command + "|" +key, function (values) {
        //        var setValuesFunc = function () {
        //            Demo.HideLoadingPanel();
        //            if (!values)
        //                return;
        //    };
        //        Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientMarketingNo });
        //    });
        //    Demo.ShowLoadingPanel(ClientCostFormPanel.GetMainElement());
        //}
        if (command == "Reply" || command == "EditDraft" || command == "EditCost") {
            ClientCostGrid.GetValuesOnCustomCallback("MailForm|" + command + "|" +key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;
                    //
                    clienteventlog.SetValue(values["Eventlog"]);
                    ClientDisponsition.PerformCallback(values["NewID"]);
                    //ClientPackSize.PerformCallback("reload|" + values["ID"]);
                    //ClientCostingNo.PerformCallback("reload|" + values["Company"]);
                    ClientCompany.PerformCallback("#TextChanged#|" + values["Company"]);
                    ClientCompany.SetValue(values["Company"]);
                    hfRequestNo.Set("ID", values["ID"]);
                    ClientCostingNo.SetText(values["RequestNo"]);
                    ClientMarketingNo.SetText(values["Marketingnumber"]);
                    ClientReference.SetText(values["RDNumber"]);
                    ClientPackSize.SetText(values["PackSize"]);
                    hfid.Set('hidden_value', values["ID"]);
                    hfFolio.Set('Folio', values["Folio"]);
                    ClientNewID.Set('NewID', values["NewID"]);
                    ClientNotes.SetValue(values["Remark"]);
                    tbCanSize.SetText(values["CanSize"]);
                    ClientNetweight.SetText(values["NetWeight"]);
                    ClientNetUnit.SetText(values["NetUnit"]);
                    seExchangeRate.SetText(values["ExchangeRate"]);
                    tbCustomer.SetText(values["Customer"]);
                    ClientValidfrom.SetText(values["From"]);
                    ClientValidto.SetText(values["To"]);

                    usertp.Set("usertype", values["UserType"]);
                    //tbMaterial.SetText(values["Material"]);
                    CmbPackaging.SetText(values["Packaging"]);
                    //if (values["Completed"]=="true")
                    //cbCompleted.SetChecked(true);
                    //tbProductName.SetText(values["Name"]);
                    //ClientRDNumber.SetText(values["RefSamples"]);
                    //tbMaterial.SetText(values["Code"]);
                    //ClientRDNumber.Focus();
                    //load
                    //formLayout.GetItemByName("Offer").SetVisible(values["StatusApp"] == 0);
                    //var NameControl = "ShipTo|SoldTo|Incoterm|PaymentTerm|Commission|TotalContainers|SubContainers";
                    //var g = NameControl.split("|"), s;
                    //for (s = 0; s < g.length; s++) {
                    //    if (g[s] != "")
                    //        //formLayout.GetItemByName(g[s]).SetVisible(values["StatusApp"] == 0);
                    //        formLayout.GetItemByName(g[s]).SetVisible(true);
                    //}                    
                    //select formula
                    //cbEnable.SetEnabled(values["StatusApp"]== 0);
                    //cbEnable.SetEnabled(true);
                    hfStatusApp.Set('StatusApp', values["StatusApp"]);
                    //if (values["StatusApp"] == 5 || values["StatusApp"] == 2 || values["StatusApp"] == -1) {
                    //    ASPxMenu1.SetEnabled(true);
                    //} else
                    //ASPxMenu1.SetEnabled(false);
                    //if(values["StatusApp"] == 4)
                    //    ASPxMenu2.SetEnabled(true);
                    //else
                    //    ASPxMenu2.SetEnabled(false);
                    var lastCompany = "Build|" + values["Company"];
                    //CmbSecPackageCode.PerformCallback(values["ID"] + '|' + values["Company"]);
                    //CmbPackageCode.PerformCallback(values["ID"] + '|' + values["Company"]);
                    //CmbMargin.PerformCallback(lastCompany);
                    //CmbLaborOverhead.PerformCallback(lastCompany);

                    //grid.GetValuesOnCustomCallback(values["RequestNo"]);
                    //ClientCostingNo.PerformCallback(lastCompany);
                    testgrid.PerformCallback('reload|' + values["Folio"]);
                    grid.PerformCallback('reload|' + values["Folio"]);
                    if (values["RequestNo"].substring(3, 4) == "1")
                        grid.PerformCallback(['changed', values["ExchangeRate"]].join("|"));
                    gvCode.PerformCallback('reload|' + values["Folio"]);
                    approv.Set("approv", values["editor"]);
                    //ClientCompany.SetEnabled(values["ID"] == "0");
                    var b = (values["editor"] == "0");
                    ClientCompany.SetEnabled(false);
                    Demo.SetEnabledForm(b);
                    Demo.SetReadOnly(values["StatusApp"] == 5 || values["StatusApp"] == 2 || values["StatusApp"] == -1);
                    //ClientCompany.GetInputElement().readOnly = b;
                    //GridView1.PerformCallback('reload|' + values["Folio"]);
                    //var test = values["StatusApp"] != "0";
                    //alert(test);
                    //formLayout.GetItemByName("Uploadfile").SetVisible(values["StatusApp"] == "0");
                    //formLayout.GetItemByName("Display").SetVisible(values["StatusApp"] != "0");
                    formLayout.GetItemByName("Reason").SetVisible(false);
                    //formLayout.GetItemByName("Upload").SetVisible(false);
                    Upload.SetEnabled(false);
                    //ASPxMenu2.SetVisible(true);
                    LoadName(1);
                    //fileManager.PerformCallback(['load', b].join("|"));
                    //grid.PerformCallback('symbol|1');
            };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientMarketingNo });
            });
            Demo.ShowLoadingPanel(ClientCostFormPanel.GetMainElement());
        }
    },
    ClientCostGrid_Init: function (s, e) {
        approv.Set("approv", "Vest");
        Upload.SetEnabled(false);
        Demo.UpdateMailGridKeyFolderHash();
    },
    Init: function (s, e) {
        //var Edit = Demo.getUrlVars()["Edit"];
        var key = Demo.getUrlVars()["ID"];
        //if (Edit == "1") {
        //    var Code = Demo.getUrlVars()["Code"];
        //    Demo.ChangeDemoState("MailForm", "EditCost",key + "|" + Code);
        //} else {
            if (!key)
                return;
        //    alert(key);
        //var UserType = Demo.getUrlVars()["UserType"];
        //if (UserType == undefined)
        //    usertp.Set("usertype", 0);
        //else
        //    usertp.Set("usertype", UserType);
            Demo.ChangeDemoState("MailForm", "EditDraft", key);
       // }
    },
    Test: function (s, e) {
        debugger;
        if (!ClientCostGrid.cpKeyValue)
            return;
        //popup.Show();
        //popup.PerformCallback(gv.cpKeyValue);
        if (ClientCostGrid.cpKeyValue == 0)
            alert(ClientCostGrid.cpKeyValue);
        var key = ClientCostGrid.cpKeyValue;
        ClientCostGrid.cpKeyValue = null;
        if (key == 0)
            Demo.ChangeDemoState("MailList");
        else
            Demo.ChangeDemoState("MailForm", "EditDraft", key);
    },
    MarkMessagesAsRead: function (read, keys) {
        var sendCallback = false;
    },
    UpdateMailGridKeyFolderHash: function () {
        var hash = {};
        for (var folderName in ClientCostGrid.cpVisibleMailKeysHash) {
            var keys = ClientCostGrid.cpVisibleMailKeysHash[folderName];
            if (!keys || keys.length == 0)
                continue;
            hash[folderName] = [];
            for (var i = 0; i < keys.length; i++)
                hash[keys[i]] = folderName;
        }
        ClientCostGrid.cpKeyFolderHash = hash;
    },
    HideCostForm: function () {
        ClientCostFormPanel.SetVisible(false);
    }
});
formulaPageModule = CreateClass(PageModuleBase, {
    constructor: function () {
        this.DemoState = { View: "MailList" };
    },
    OnStateChanged: function () {
        var state = this.DemoState;
        if (state.View == "MailList")
            this.ShowGrid();
        //if (state.View == "MailPreview")
        //    this.ShowEntryPreview(state.Key);
        if (state.View == "MailForm")
            this.ShowformulaForm(state.Command, state.Key);
    },
    OnSearchTextChanged: function () {
        var processed = MailPageModule.base.prototype.OnSearchTextChanged.call(Demo);
        if (processed) return;
        Demo.ChangeDemoState("MailList");
        Demo.DoCallback(grid, function () {
            gridData.PerformCallback("Search");
        });
    },
    ClientLayout_PaneResized: function () {
        if (DevAVPageName == "formulaForm") {
            gridData.SetHeight(0);
            gridData.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
            formulaFormPanel.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
            //ClientCostFormPanel.GetMainElement().scrollTop = ClientCostFormPanel.GetMainElement().scrollHeight;
            //treeList.SetHeight(0);
            //treeList.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
        }
    },
    //treeList_NodeClick: function (s, e) {
    //    var key = s.GetFocusedNodeKey();
    //    Demo.ChangeDemoState("MailForm", "EditDraft", key);
    //},
    gridData_RowClick: function (s, e) {
        var src = ASPxClientUtils.GetEventSource(e.htmlEvent);
        if (src.tagName == "TD" && src.className.indexOf("dxgvCommandColumn") != -1) // selection cell
            return;
        if (!s.IsDataRow(e.visibleIndex))
            return;
        var key = s.GetRowKey(e.visibleIndex);
        //if (ClientMailTree.GetSelectedNode().name === "Drafts")
        Demo.ChangeDemoState("MailForm", "EditDraft", key);
        //else
        //    Demo.ChangeDemoState("MailPreview", "", key);
    },
    gridData_Init: function (s, e) {
        approv.Set("approv", "Vest");
        Demo.UpdateMailGridKeyFolderHash();
    },
    Test: function (s, e) {
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
            Demo.ChangeDemoState("MailForm", "EditDraft", key);
        }
    },
    ShowMenuItems: function () {
        var view = Demo.DemoState.View;
        //var key = Demo.getUrlVars()["viewMode"];
        var state = Demo.DemoState;
        var edit = editor.Get("Name");
        var appr = approv.Get("approv");

        ClientActionMenu.GetItemByName("Pending").SetVisible(view == "MailList" && hftype.Get("type") == 1);
        ClientActionMenu.GetItemByName("All").SetVisible(view == "MailList" && hftype.Get("type") == 0);
        ClientActionMenu.GetItemByName("Filter").SetVisible(view == "MailList");
        ClientActionMenu.GetItemByName("back").SetVisible(view != "MailList");
        ClientActionMenu.GetItemByName("save").SetVisible(view == "MailForm" && state.key != 0 && edit=="0");
        ClientActionMenu.GetItemByName("delete").SetVisible(view == "MailForm" && state.key != 0 && edit=="0");
    },
    ClientActionMenu_ItemClick: function (s, e) {
        var command = e.item.name;
        var state = Demo.DemoState;
        switch (command) {
            case "Filter":
                var popup = Demo.GetPopupControl();
                popup.Show();
                popup.BringToFront();
                break;
        //    case "new":
        //        grid.AddNewRow();
        //        break;
            case "back":
                Demo.ChangeDemoState("MailList");
                break;
            case "save":
                Demo.ChangeDemoState("MailList");
                Demo.DoCallback(grid, function () {
                    //testGrid.PerformCallback("post");
                    grid.PerformCallback("post|" + state.Key);
                });
                break;
        //    case "delete":
        //        if (!window.confirm("Confirm Delete?"))
        //            return;
        //        var keys = [];
        //        var index = grid.GetFocusedRowIndex();
        //        grid.DeleteRow(index);
        //        break;
        }
    },
    GetPopupControl: function () {
        return ASPxPopupClientControl;
    },
    ShowformulaForm: function (command, key) {
        Demo.HideForm();
        ClientCompany.SetValue("");
        ClientCustomer.SetText("");
        ClientBrand.SetText("");
        ClientProductStyle.SetText("");
        ClientReference.SetText("");
        CmbPackaging.SetText("");
        ClientRevised.SetText("");
        formulaFormPanel.SetVisible(["read", "Reply", "EditDraft"].indexOf(command));
        if (command == "Reply" || command == "EditDraft") {
            gridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;
                    ClientRevised.SetValue(values["Costing"]);
                    ClientCompany.SetValue(values["Company"]);
                    ClientCustomer.SetText(values["Customer"]);
                    ClientBrand.SetText(values["Brand"]);
                    ClientReference.SetText(values["RDNumber"]);
                    CmbPackaging.SetText(values["Packaging"]);
                    ClientRequestNo.SetText(values["RequestNo"]);
                    editor.Set("Name", values["editor"]);
                    ClientRevised.SetText(values["Revised"]);
                    //ClientPackSize.PerformCallback("AddRow|" + values["PackSize"]);
                    ClientProductStyle.SetText(values["ProductStyle"]);
                    ClientNetweight.SetText(values["NetWeight"]);
                    ClientUnit.SetText(values["Unit"]);
                    grid.PerformCallback("reload|" + values["ID"]);
                };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientCompany });
            });
            Demo.ShowLoadingPanel(ClientFormPanel.GetMainElement());
            }
    },
    ShowGrid: function () {
        Demo.HideLoadingPanel();
        //Demo.HideCostPreview();
        //Demo.HideEntryForm();
        gridData.SetVisible(true);
        //treeList.SetVisible(true);
        Demo.ClientLayout_PaneResized();
    },
    HideForm: function () {
        //if (window.ClientAddressBookPopup)
        //    ClientAddressBookPopup.Hide();
        //treeList.SetVisible(false);
        gridData.SetVisible(false);
    },
    UpdateMailGridKeyFolderHash: function () {
        var hash = {};
        for (var folderName in gridData.cpVisibleMailKeysHash) {
            var keys = gridData.cpVisibleMailKeysHash[folderName];
            if (!keys || keys.length == 0)
                continue;
            hash[folderName] = [];
            for (var i = 0; i < keys.length; i++)
                hash[keys[i]] = folderName;
        }
        gridData.cpKeyFolderHash = hash;
    }
});
adminPageModule= CreateClass(PageModuleBase, {
    constructor: function () {
        this.DemoState = { View: "MailList" };
    },
    OnStateChanged: function () {
        var state = this.DemoState;
        if (state.View == "MailList")
            this.ShowEntryGrid();
        //if (state.View == "MailPreview")
        //    this.ShowEntryPreview(state.Key);
        if (state.View == "MailForm")
            this.ShowEntryForm(state.Command, state.Key);
    },
    HideForm: function () {
        ClientFormPanel.SetVisible(false);
    },
    ShowEntryForm: function (command, key) {
        Demo.HideGrid();
        ClientFormPanel.SetVisible(command == "upload");
    },
    HideGrid: function () {
        grid.SetVisible(false);
    },
    ShowEntryGrid: function () {
        Demo.HideLoadingPanel();
        //Demo.HideCostPreview();
        //Demo.HideEntryForm();
        grid.SetVisible(true);
        Demo.ClientLayout_PaneResized();
    },
    OnSearchTextChanged: function () {
        var processed = MailPageModule.base.prototype.OnSearchTextChanged.call(Demo);
        if (processed) return;
        Demo.ChangeDemoState("MailList");
        Demo.DoCallback(grid, function () {
            grid.PerformCallback("Search");
        });
    },
    ClientLayout_PaneResized: function () {
        if (DevAVPageName == "adminform") {
            grid.SetHeight(0);
            grid.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
        }
    },
    ShowMenuItems: function () {
        var view = Demo.DemoState.View;
        var edit = editor.Get("Name");
        //var key = Demo.getUrlVars()["viewMode"];
        ClientActionMenu.GetItemByName("back").SetVisible(view != "MailList");
        //ClientActionMenu.GetItemByName("save").SetVisible(view != "MailList");
        ClientActionMenu.GetItemByName("new").SetVisible(view != "MailForm" && edit);
        ClientActionMenu.GetItemByName("delete").SetVisible(view != "MailForm" && edit);
        ClientInfoMenu.GetItemByName("print").SetVisible(false);
    },
    uploadfile: function (name) {
    //    
        var view = Demo.DemoState.View;
    //    var myname = ["PricePolicy", "upCost"];
    //    var _file = true;
    //    Demo._menu = "";
    //    Demo.ChangeDemoState("MailList");
    //    if (myname.indexOf(name)!=-1) {
    //        Demo.ChangeDemoState("MailForm", "upload", 0);
    //        _file = false;
    //        Demo._menu = name;
    //        testGrid.PerformCallback(name);
    //    }
        if (name == 'Upload') {
            Demo.ChangeDemoState("MailForm", name, 0);
        } else
            Demo.ChangeDemoState("MailList");
        ClientFormPanel.SetVisible(view != "MailForm");
    //    grid.SetVisible(_file);
        grid.PerformCallback('reload|' + name);
    },
    ClientActionMenu_ItemClick: function (s, e) {
        var command = e.item.name;
        var state = Demo.DemoState;
        switch (command) {
            case "new":
                //if (Demo._menu == "upCost")
                //    this.uploadfile(Demo._menu);
                //    else
                grid.AddNewRow();
                break;
            case "back":
                Demo.ChangeDemoState("MailList");
                break;
            //case "upload":
                //testGrid.PerformCallback();
                //pcLogin.Show();
                //pcLogin.BringToFront();
                //break;
            case "upload":
                Demo.ChangeDemoState("MailForm", command, 0);
                //if (state.Command === "EditDraft")
                //var value = "New|0";
                //Demo.DoCallback(ClientCostGrid, function () {
                //    ClientCostGrid.PerformCallback(value);
                //});
                testGrid.PerformCallback("new");
                break;
            case "delete":
                if (!window.confirm("Confirm Delete?"))
                    return;
                var keys = [];
                var index = grid.GetFocusedRowIndex();
                grid.DeleteRow(index);
                break;
            case "save":
                //grid.GetValuesOnCustomCallback("MailForm|" + command + "|" + state.Key, function (values) {
                Demo.ChangeDemoState("MailList");
                Demo.DoCallback(grid, function () {
                    testGrid.PerformCallback("post");
                    grid.PerformCallback("post");
                });
                //});
                break;
        }
    }
});
CalculaModule = CreateClass(PageModuleBase, {
    constructor: function () {
        this.DemoState = { View: "MailList" };
    },
    OnStateChanged: function () {
        var state = this.DemoState;
        if (state.View == "MailList")
            this.ShowGrid();
        if (state.View == "MailForm")
            this.ShowForm(state.Command, state.Key);
    },
    ClientLayout_PaneResized: function () {
        var state = Demo.DemoState;
        if (!state) return;
        if (DevAVPageName == "CalculationForm") {
            gridData.SetHeight(0);
            gridData.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
            ClientFormPanel.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
            ClientPreviewPanel.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
        }
    },
    ShowForm: function (command, key) {
        //
        Demo.HideForm();
        Demo.ClearForm();
        gv.PerformCallback(["reload", key].join("|"));
        //ClientPreviewPanel.SetVisible(command == "unread" || command == "Edit");
        ClientFormPanel.SetVisible(command == "New" || command == "Reply" || command == "EditDraft");
        //ClientMailEditor.AdjustControl();
        Demo.ClientLayout_PaneResized();
        if (command == "New") {
            gridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;
                    //ClientintGrid.PerformCallback('AddRow|1|' + values["RequestType"]);
                    //ClientCompanyEdit.SetEnabled(true);
                    //ClientValidfromEdit.SetText(null);
                    //ClientValidtoEdit.SetText(null);
                    ClientNewID.Set("NewID", values["NewID"]);
                    ClientRequestNo.SetText("###########");
                    approv.Set("approv", values["editor"]);
                    hfid.Set('hidden_value', 0);
                    seExchangeRate.SetText(values["ExchangeRate"]);
                    ClientCurrency.SetText(values["Currency"]);
                    var myDate = addDays(30);
                    ClientValidto.SetText(myDate);
                    //gvcal.PerformCallback(["new", key].join("|"));
                    //gv.PerformCallback(["reload", key].join("|"));
                    ClientDisponsition.PerformCallback('0');
                };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientDisponsition });
            });
            Demo.ShowLoadingPanel(ClientPreviewPanel.GetMainElement());
        }
        if (command == "Reply" || command == "EditDraft") {
            gridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;
                    hGeID.Set("GeID", key);
                    ClientNewID.Set("NewID", values["NewID"]);
                    ClientRequestNo.SetValue(values["RequestNo"]);
                    ClientCustomer.SetValue(values["Customer"]);
                    ClientShipTo.SetValue(values["ShipTo"]);
                    ClientBillTo.SetValue(values["BillTo"]);
                    CmbPaymentTerm.SetValue(values["PaymentTerm"]);
                    CmbIncoterm.SetValue(values["Incoterm"]);
                    CmbSize.SetValue(values["Size"]);
                    CmbRoute.SetValue(values["Route"]);
                    Clientinterest.SetText(values["Interest"]);
                    tbInsurance.SetText(values["Insurance"]);
                    tbFreight.SetText(values["Freight"]);
                    ClientValidfrom.SetText(values["From"]);
                    ClientValidto.SetText(values["To"]);
                    deValidityDate.SetText(values["ValidityDate"]);
                    //ClientMargin.SetText(values["Margin"]);
                    ClientNotes.SetText(values["Remark"]);
                    seExchangeRate.SetText(values["ExchangeRate"]);
                    ClientCurrency.SetText(values["Currency"]);
                    var text = ["reload", key];
                    var arge = text.join("|");
                    gvutilize.PerformCallback(arge);
                    if (values["FreightOld"]!="")
                        append_to_div("my_div", "Freight old value : " + values["FreightOld"] + "\n"); 
                };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientCustomer });
            });
            Demo.ShowLoadingPanel(ClientFormPanel.GetMainElement());
        }
    },
    ShowGrid: function () {
        Demo.HideLoadingPanel();
        //Demo.HideCostPreview();
        //Demo.HideEntryForm();
        gridData.SetVisible(true);
        Demo.ClientLayout_PaneResized();
    },
    ClearForm: function () {
        curentEditingIndex = 0;
        ClientUpCharge.SetText("");
        //Clientinterest.SetText("");
        //ClientRequestNo.SetText("###########");
        ClientCustomer.SetSelectedIndex(-1);
        ClientShipTo.SetSelectedIndex(-1);
        ClientBillTo.SetSelectedIndex(-1);
        ClientCurrency.SetSelectedIndex(-1);
        seExchangeRate.SetText("");
        Clientinterest.SetText("");
        tbFreight.SetText("");
        CmbIncoterm.SetSelectedIndex(-1);
        CmbRoute.SetSelectedIndex(-1);
        CmbSize.SetSelectedIndex(-1);
        tbInsurance.SetText("");
        CmbPaymentTerm.SetSelectedIndex(-1);
        tcDemos.SetActiveTabIndex(0);
        GetBuildCalcu(false);
        ClientValidfrom.SetText(Demo.currentdate());
        ClientValidto.SetText(Demo.currentdate());
        deValidityDate.SetText(Demo.currentdate());
        //cancelBtn.DoClick();
    },
    SetEnable: function (b) {
        //
        CmbRoute.SetEnabled(b);
        CmbSize.SetEnabled(b);
        tbFreight.SetEnabled(b);
        //tbInsurance.SetEnabled(b);
    },
    HideForm: function () {
        //if (window.ClientAddressBookPopup)
        //    ClientAddressBookPopup.Hide();
        gridData.SetVisible(false);
    },
    UpdateMailGridKeyFolderHash: function () {
        var hash = {};
        for (var folderName in gridData.cpVisibleMailKeysHash) {
            var keys = gridData.cpVisibleMailKeysHash[folderName];
            if (!keys || keys.length == 0)
                continue;
            hash[folderName] = [];
            for (var i = 0; i < keys.length; i++)
                hash[keys[i]] = folderName;
        }
        gridData.cpKeyFolderHash = hash;
    },
    Init: function (s, e) {
        var key = Demo.getUrlVars()["ID"];
        if (!key)
            return;
        //alert(key);
        var form = Demo.getUrlVars()["form"];
        hfid.Set("hidden_value", key);
        Demo.ChangeDemoState("MailForm", "EditDraft", key);
    },
    ClientgridData_RowClick: function (s, e) {
        var src = ASPxClientUtils.GetEventSource(e.htmlEvent);
        if (src.tagName == "TD" && src.className.indexOf("dxgvCommandColumn") != -1) // selection cell
            return;
        if (!s.IsDataRow(e.visibleIndex))
            return;
        var key = s.GetRowKey(e.visibleIndex, "ID");
        Demo.ChangeDemoState("MailForm", "EditDraft", key);
    },
    ClientInfoMenu_ItemClick: function (s, e) {
        var command = e.item.name;
        if (command == "theme" || command == "help") return;
        var state = Demo.DemoState;
        if (command == "ExportToXLS" || command == "ExportToPDF" || command == "print") {
            gridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + state.Key, Demo.OnGetRowValues);
        } else
            if (e.item.parent) {
                ASPxClientUtils.SetCookie("DemoCurrentTheme", e.item.name || "");
                e.processOnServer = true;
            }
    },
    OnGetRowValues: function (values) {
        var state = Demo.DemoState; var url = "";
        if (values != null) {
            if (values["KeyValue"] == 0) return alert("data not found");
            var view = values["view"];
            var a = values["print"].split("|"), i;
            for (i = 0; i < a.length; i++) {
                url = "./PrintMails.aspx?usertype=0&view=" + view + "&Type=" + a[i] + "&Id=" + values["KeyValue"];
                window.open(url, "_blank");
            }
        }
    },
    gridData_Init: function (s, e) {
        approv.Set("approv", "Vest");
    },
    ClientActionMenu_ItemClick: function (s, e) {
        var command = e.item.name;
        var state = Demo.DemoState;
        switch (command) {
            case "Filter":
                //alert("Filter");
                var popup = Demo.GetPopupControl();
                popup.Show();
                popup.BringToFront();
                break;
            case "Pending": case "All":
                hftype.Set("type", command == "Pending" ? "0" : "1");
                gridData.PerformCallback(command);
                break;
            case "upload":
                Demo.ChangeDemoState("MailForm", "upload", 0);
                break;
            case "save":
                if (window.ClientCostingNo && !ASPxClientEdit.ValidateEditorsInContainerById("MailForm"))
                    return;
                if (!ASPxClientEdit.ValidateGroup('group1'))
                    return alert('Field is required');
				if (CmbIncoterm.GetValue() == 'CIF' || CmbIncoterm.GetValue() == 'CFR') {
                    if (tbInsurance.GetText() == '')
                        return alert('The Insurance should not be empty...');
                    if (tbFreight.GetText() == '' || tbFreight.GetText() == '0')
                        return alert('The Freight should not be empty...');
                    if (CmbRoute.GetValue() == '' || CmbRoute.GetValue() == '0')
                        return alert('The Route should not be empty...');
                    if (CmbSize.GetValue() == '' || CmbSize.GetValue() == '0')
                        return alert('The Size should not be empty...');
                }
                //var edit = editor.Get("Name");
                var _Disp = ClientDisponsition.GetValue();
                gridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + state.Key, function (values) {
                    if ((_Disp == 3 || _Disp == 5 || _Disp == 7) && (CmbReason.GetText() == "")) {
                        switch (_Disp) {
                            case "3":
                                alert('The Reason should not be empty...');
                                break;
                        }
                        CmbReason.SetFocus();
                        Demo.ChangeDemoState("MailForm", "EditDraft", state.Key);
                        return;
                    }
                    Demo.ChangeDemoState("MailList");
                    Demo.DoCallback(gridData, function () {
                        gridData.PerformCallback("post|" + state.Key);
                    });
                });
                break;
            case "new":
                Demo.ChangeDemoState("MailForm", "New", 0);
                break;
            case "reply":
                Demo.ChangeDemoState("MailForm", "Reply", state.Key);
                break;
            case "back":
                Demo.ChangeDemoState("MailList");
                break;   
            case "delete":
                if (!window.confirm("Confirm Delete?"))
                    return;
                var keys = [];
                if (state.View == "MailList") {
                    //if (state.View == "MailForm") {
                    keys = gridData.GetSelectedKeysOnPage();
                    //} else if (state.View == "MailPreview") {
                } else if (state.View == "MailForm") {
                    keys = [state.Key];
                    Demo.ChangeDemoState("MailList");
                }
                if (keys.length > 0) {
                    Demo.DoCallback(gridData, function () {
                        gridData.PerformCallback("Delete|" + keys.join("|"));
                    });
                    Demo.MarkMessagesAsRead(true, keys);
                }
                break;
        }
    },
    SetUnitPriceColumnVisibility: function () {
        var appr = approv.Get("approv");
        //var v = hfStatusApp.Get("StatusApp") == "2";
        var v = false;
        var disp = v && appr == '0' ? 'table-cell' : 'none';
        $('td.unitPriceColumn').css('display', disp);

    },
    ShowMenuItems: function () {
        //
        var view = Demo.DemoState.View;
        //var key = Demo.getUrlVars()["viewMode"];
        var state = Demo.DemoState;
        var edit = editor.Get("Name");
        var appr = approv.Get("approv");
        var type = hftype.Get("type");
        //
        ClientActionMenu.GetItemByName("Pending").SetVisible(view == "MailList" && type == 1);
        ClientActionMenu.GetItemByName("All").SetVisible(view == "MailList" && type == 0);
        //ClientActionMenu.GetItemByName("Filter").SetVisible(view == "MailList" && type == 1);
        ClientActionMenu.GetItemByName("back").SetVisible(view != "MailList");
        ClientActionMenu.GetItemByName("save").SetVisible(view == "MailForm" && edit == "0");
        ClientActionMenu.GetItemByName("new").SetVisible(view != "MailForm" && edit == "0");
        ClientActionMenu.GetItemByName("delete").SetVisible(view == "MailForm" && state.Key != 0 && edit == "0");
    }
});
CalculaModule2 = CreateClass(PageModuleBase, {
    constructor: function () {
        this.DemoState = { View: "MailList" };
    },
    OnStateChanged: function () {
        var state = this.DemoState;
        if (state.View == "MailList")
            this.ShowGrid();
        if (state.View == "MailForm")
            this.ShowForm(state.Command, state.Key);
    },
    ClientLayout_PaneResized: function () {
        var state = Demo.DemoState;
        if (!state) return;
        if (DevAVPageName == "CalculationForm2") {
            gridData.SetHeight(0);
            gridData.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
            ClientFormPanel.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
            ClientPreviewPanel.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
        }
    },
    ShowForm: function (command, key) {
        //
        Demo.HideForm();
        Demo.ClearForm();
        gv.PerformCallback(["reload", key].join("|"));
        //ClientPreviewPanel.SetVisible(command == "unread" || command == "Edit");
        ClientFormPanel.SetVisible(command == "New" || command == "Reply" || command == "EditDraft");
        //ClientMailEditor.AdjustControl();
        Demo.ClientLayout_PaneResized();
        if (command == "New") {
            gridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;
                    //ClientintGrid.PerformCallback('AddRow|1|' + values["RequestType"]);
                    //ClientCompanyEdit.SetEnabled(true);
                    //ClientValidfromEdit.SetText(null);
                    //ClientValidtoEdit.SetText(null);
                    ClientNewID.Set("NewID", values["NewID"]);
                    ClientRequestNo.SetText("###########");
                    approv.Set("approv", values["editor"]);
                    hfid.Set('hidden_value', 0);
                    seExchangeRate.SetText(values["ExchangeRate"]);
                    OldValueExchangeRate.Set("ExchangeRate", values["ExchangeRate"]);
                    ClientCurrency.SetText(values["Currency"]);
                    var myDate = addDays(30);
                    ClientValidto.SetText(myDate);
                    //gvcal.PerformCallback(["new", key].join("|"));
                    //gv.PerformCallback(["reload", key].join("|"));
                    ClientDisponsition.PerformCallback('0');
                };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientDisponsition });
            });
            Demo.ShowLoadingPanel(ClientPreviewPanel.GetMainElement());
        }
        if (command == "Reply" || command == "EditDraft") {
            gridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;
                    hGeID.Set("GeID", key);
                    ClientNewID.Set("NewID", values["NewID"]);
                    ClientRequestNo.SetValue(values["RequestNo"]);
                    ClientCustomer.SetValue(values["Customer"]);
                    ClientShipTo.SetValue(values["ShipTo"]);
                    ClientBillTo.SetValue(values["BillTo"]);
                    CmbPaymentTerm.SetValue(values["PaymentTerm"]);
                    CmbIncoterm.SetValue(values["Incoterm"]);
                    CmbSize.SetValue(values["Size"]);
                    CmbRoute.SetValue(values["Route"]);
                    Clientinterest.SetText(values["Interest"]);
                    tbInsurance.SetText(values["Insurance"]);
                    tbFreight.SetText(values["Freight"]);
                    OldValueFreight.Set("Freight",values["Freight"]);
                    ClientValidfrom.SetText(values["From"]);
                    ClientValidto.SetText(values["To"]);
                    deValidityDate.SetText(values["ValidityDate"]);
                    //ClientMargin.SetText(values["Margin"]);
                    ClientNotes.SetText(values["Remark"]);
                    seExchangeRate.SetText(values["ExchangeRate"]);
                    OldValueExchangeRate.Set("ExchangeRate",values["ExchangeRate"]);
                    ClientCurrency.SetText(values["Currency"]);
                    var text = ["reload", key];
                    var arge = text.join("|");
                    //gvutilize.PerformCallback(arge);
                    cpUpdateFiles.PerformCallback(arge);
                    ClientDisponsition.PerformCallback(values["NewID"]);
                    if (values["FreightOld"]!="")
                        append_to_div("my_div", "Freight old value : " + values["FreightOld"] + "\n"); 
                };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientCustomer });
            });
            Demo.ShowLoadingPanel(ClientFormPanel.GetMainElement());
        }
    },
    ShowGrid: function () {
        Demo.HideLoadingPanel();
        //Demo.HideCostPreview();
        //Demo.HideEntryForm();
        gridData.SetVisible(true);
        Demo.ClientLayout_PaneResized();
    },
    ClearForm: function () {
        curentEditingIndex = 0;
        ClientUpCharge.SetText("");
        //Clientinterest.SetText("");
        //ClientRequestNo.SetText("###########");
        ClientCustomer.SetSelectedIndex(-1);
        ClientShipTo.SetSelectedIndex(-1);
        ClientBillTo.SetSelectedIndex(-1);
        ClientCurrency.SetSelectedIndex(-1);
        seExchangeRate.SetText("");
        Clientinterest.SetText("");
        tbFreight.SetText("");
        CmbIncoterm.SetSelectedIndex(-1);
        CmbRoute.SetSelectedIndex(-1);
        CmbSize.SetSelectedIndex(-1);
        tbInsurance.SetText("");
        CmbPaymentTerm.SetSelectedIndex(-1);
        tcDemos.SetActiveTabIndex(0);
        GetBuildCalcu(false);
        ClientValidfrom.SetText(Demo.currentdate());
        ClientValidto.SetText(Demo.currentdate());
        deValidityDate.SetText(Demo.currentdate());
        //cancelBtn.DoClick();
    },
    SetEnable: function (b) {
        //
        CmbRoute.SetEnabled(b);
        CmbSize.SetEnabled(b);
        tbFreight.SetEnabled(b);
        //tbInsurance.SetEnabled(b);
    },
    HideForm: function () {
        //if (window.ClientAddressBookPopup)
        //    ClientAddressBookPopup.Hide();
        gridData.SetVisible(false);
    },
    UpdateMailGridKeyFolderHash: function () {
        var hash = {};
        for (var folderName in gridData.cpVisibleMailKeysHash) {
            var keys = gridData.cpVisibleMailKeysHash[folderName];
            if (!keys || keys.length == 0)
                continue;
            hash[folderName] = [];
            for (var i = 0; i < keys.length; i++)
                hash[keys[i]] = folderName;
        }
        gridData.cpKeyFolderHash = hash;
    },
    Init: function (s, e) {
        var key = Demo.getUrlVars()["ID"];
        if (!key)
            return;
        //alert(key);
        var form = Demo.getUrlVars()["form"];
        hfid.Set("hidden_value", key);
        Demo.ChangeDemoState("MailForm", "EditDraft", key);
    },
    ClientgridData_RowDblClick: function (s, e) {
        debugger;
        var src = ASPxClientUtils.GetEventSource(e.htmlEvent);
        if (src.tagName == "TD" && src.className.indexOf("dxgvCommandColumn") != -1) // selection cell
            return;
        if (!s.IsDataRow(e.visibleIndex))
            return;
        var key = s.GetRowKey(e.visibleIndex, "ID");
        Demo.ChangeDemoState("MailForm", "EditDraft", key);
    },
    ClientInfoMenu_ItemClick: function (s, e) {
        var command = e.item.name;
        if (command == "theme" || command == "help") return;
        var state = Demo.DemoState;
        if (command == "ExportToXLS" || command == "ExportToPDF" || command == "print") {
            gridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + state.Key, Demo.OnGetRowValues);
        } else
            if (e.item.parent) {
                ASPxClientUtils.SetCookie("DemoCurrentTheme", e.item.name || "");
                e.processOnServer = true;
            }
    },
    OnGetRowValues: function (values) {
        var state = Demo.DemoState; var url = "";
        if (values != null) {
            if (values["KeyValue"] == 0) return alert("data not found");
            var view = values["view"];
            var a = values["print"].split("|"), i;
            for (i = 0; i < a.length; i++) {
                url = "./PrintMails.aspx?usertype=0&view=" + view + "&Type=" + a[i] + "&Id=" + values["KeyValue"];
                window.open(url, "_blank");
            }
        }
    },
    gridData_Init: function (s, e) {
        approv.Set("approv", "Vest");
    },
    ClientActionMenu_ItemClick: function (s, e) {
        var command = e.item.name;
        var state = Demo.DemoState;
        switch (command) {
            case "Filter":
                //alert("Filter");
                var popup = Demo.GetPopupControl();
                popup.Show();
                popup.BringToFront();
                break;
            case "Pending": case "All":
                hftype.Set("type", command == "Pending" ? "0" : "1");
                gridData.PerformCallback(command);
                break;
            case "upload":
                Demo.ChangeDemoState("MailForm", "upload", 0);
                break;
            case "save":
                if (window.ClientCostingNo && !ASPxClientEdit.ValidateEditorsInContainerById("MailForm"))
                    return;
                if (!ASPxClientEdit.ValidateGroup('group1'))
                    return alert('Field is required');
                if (CmbIncoterm.GetValue() == 'CIF' || CmbIncoterm.GetValue() == 'CFR') {
                    if (tbInsurance.GetText() == '')
                        return alert('The Insurance should not be empty...');
                    if (tbFreight.GetText() == '' || tbFreight.GetText() == '0')
                        return alert('The Freight should not be empty...');
                    if (CmbRoute.GetValue() == '' || CmbRoute.GetValue() == '0')
                        return alert('The Route should not be empty...');
                    if (CmbSize.GetValue() == '' || CmbSize.GetValue() == '0')
                        return alert('The Size should not be empty...');
                }
                //var edit = editor.Get("Name");
                var _Disp = ClientDisponsition.GetValue();
                gridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + state.Key, function (values) {
                    if ((_Disp == 3 || _Disp == 5 || _Disp == 7) && (CmbReason.GetText() == "")) {
                        switch (_Disp) {
                            case "3":
                                alert('The Reason should not be empty...');
                                break;
                        }
                        CmbReason.SetFocus();
                        Demo.ChangeDemoState("MailForm", "EditDraft", state.Key);
                        return;
                    }
                    Demo.ChangeDemoState("MailList");
                    Demo.DoCallback(gridData, function () {
                        gridData.PerformCallback("post|" + state.Key);
                    });
                });
                break;
            case "new":
                Demo.ChangeDemoState("MailForm", "New", 0);
                break;
            case "reply":
                Demo.ChangeDemoState("MailForm", "Reply", state.Key);
                break;
            case "back":
                Demo.ChangeDemoState("MailList");
                break;   
            case "delete":
                if (!window.confirm("Confirm Delete?"))
                    return;
                var keys = [];
                if (state.View == "MailList") {
                    //if (state.View == "MailForm") {
                    keys = gridData.GetSelectedKeysOnPage();
                    //} else if (state.View == "MailPreview") {
                } else if (state.View == "MailForm") {
                    keys = [state.Key];
                    Demo.ChangeDemoState("MailList");
                }
                if (keys.length > 0) {
                    Demo.DoCallback(gridData, function () {
                        gridData.PerformCallback("Delete|" + keys.join("|"));
                    });
                    Demo.MarkMessagesAsRead(true, keys);
                }
                break;
        }
    },
    SetUnitPriceColumnVisibility: function () {
        var appr = approv.Get("approv");
        //var v = hfStatusApp.Get("StatusApp") == "2";
        var v = false;
        var disp = v && appr == '0' ? 'table-cell' : 'none';
        $('td.unitPriceColumn').css('display', disp);

    },
    ShowMenuItems: function () {
        //
        var view = Demo.DemoState.View;
        //var key = Demo.getUrlVars()["viewMode"];
        var state = Demo.DemoState;
        var edit = editor.Get("Name");
        var appr = approv.Get("approv");
        var type = hftype.Get("type");
        //
        ClientActionMenu.GetItemByName("Pending").SetVisible(view == "MailList" && type == 1);
        ClientActionMenu.GetItemByName("All").SetVisible(view == "MailList" && type == 0);
        //ClientActionMenu.GetItemByName("Filter").SetVisible(view == "MailList" && type == 1);
        ClientActionMenu.GetItemByName("back").SetVisible(view != "MailList");
        ClientActionMenu.GetItemByName("save").SetVisible(view == "MailForm" && edit == "0");
        ClientActionMenu.GetItemByName("new").SetVisible(view != "MailForm" && edit == "0");
        ClientActionMenu.GetItemByName("delete").SetVisible(view == "MailForm" && state.Key != 0 && edit == "0");
    }
});
QuotaPageModule = CreateClass(PageModuleBase, {
        constructor: function () {
            this.DemoState = { View: "MailList" };
        },
        OnStateChanged: function () {
            var state = this.DemoState;
            if (state.View == "MailList")
                this.ShowGrid();
        if (state.View == "MailForm")
            this.ShowForm(state.Command, state.Key);
    },
    GetSelectedFieldValuesCallback: function (values) {
        //
        var arr1 = [];
        for (var i = 0; i < values.length; i++) {
            arr1.push(values[i]);
            var url = "./PrintMails.aspx?view=2&Type=3&Id=" + values[i];
            window.open(url, "_blank");
            ClientInfoMenu.GetItemByName('print').SetNavigateUrl(url);
        }
    },
    ClientInfoMenu_ItemClick: function (s, e) {
        var command = e.item.name;
        if (command == "theme" || command == "help" || command == "print") return;
        var state = Demo.DemoState;
        if (command == "ExportToXLS" || command == "ExportToPDF") {
            if (state.View == "MailList" && command == "ExportToXLS")
                gridData.GetSelectedFieldValues("ID", Demo.GetSelectedFieldValuesCallback);
            else {
                if (state.Key == 0) return alert("data not found");
                var url = "./PrintMails.aspx?view=2&Type=3&Id=" + state.Key;
                //alert(url);
                window.open(url, "_blank");
                ClientInfoMenu.GetItemByName('print').SetNavigateUrl(url);
            }
        } else
            if (e.item.parent) {
                ASPxClientUtils.SetCookie("DemoCurrentTheme", e.item.name || "");
                e.processOnServer = true;
            }
    },
    OnSearchTextChanged: function () {
        var processed = MailPageModule.base.prototype.OnSearchTextChanged.call(Demo);
        if (processed) return;
        Demo.ChangeDemoState("MailList");
        Demo.DoCallback(gridData, function () {
            gridData.PerformCallback("Search");
        });
    },
    ClientLayout_PaneResized: function () {
        if (DevAVPageName == "QuotationForm" || DevAVPageName == "OfferPriceForm") {
            gridData.SetHeight(0);
            gridData.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
            ClientFormPanel.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
            //ClientCostFormPanel.GetMainElement().scrollTop = ClientCostFormPanel.GetMainElement().scrollHeight;
        }
    },
    gridData_RowClick: function (s, e) {
        var src = ASPxClientUtils.GetEventSource(e.htmlEvent);
        if (src.tagName == "TD" && src.className.indexOf("dxgvCommandColumn") != -1) // selection cell
            return;
        if (!s.IsDataRow(e.visibleIndex))
            return;
        var key = s.GetRowKey(e.visibleIndex);
        //
        //var a = ClientCompany.GetText();
        hfid.Set("hidden_value", key);
        //if (ClientMailTree.GetSelectedNode().name === "Drafts")
        Demo.ChangeDemoState("MailForm", "EditDraft", key);
        //else
        //    Demo.ChangeDemoState("MailPreview", "", key);
    },
    gridData_Init: function (s, e) {
        approv.Set("approv", "Vest");
        hfpara.Set('para', '0');
        hfid.Set("hidden_value", '0');
        //hftotalprice.Set("CIF", '0');
        hftype.Set("type", '0');
        Demo.UpdateMailGridKeyFolderHash();
      },
    OnEndCallback: function (s, e) {
        debugger;
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
            Demo.ChangeDemoState("MailForm", "EditDraft", key);
        }
    },
    ShowMenuItems: function () {
        var view = Demo.DemoState.View;
        //var key = Demo.getUrlVars()["viewMode"];
        //
        var state = Demo.DemoState;
        var edit = editor.Get("Name");
        var appr = approv.Get("approv");
        ClientActionMenu.GetItemByName("Pending").SetVisible(view == "MailList" && hftype.Get("type") == 1);
        ClientActionMenu.GetItemByName("All").SetVisible(view == "MailList" && hftype.Get("type") == 0);
        //ClientActionMenu.GetItemByName("Filter").SetVisible(view == "MailList" && hftype.Get("type") == 1);
        ClientActionMenu.GetItemByName("back").SetVisible(view != "MailList");
        ClientActionMenu.GetItemByName("new").SetVisible(view != "MailForm" && edit == "0");
        ClientActionMenu.GetItemByName("save").SetVisible(view == "MailForm" && appr == "0");
        ClientActionMenu.GetItemByName("delete").SetVisible(view == "MailForm" && state.key != 0 && edit == "0");
    },
    ClientActionMenu_ItemClick: function (s, e) {
        var command = e.item.name;
        var state = Demo.DemoState;
        switch (command) {
            case "back":
                Demo.ChangeDemoState("MailList");
                break;
            case "Pending": case "All":
                //alert(command);
                hftype.Set("type", command == "Pending" ? "0" : "1");
                gridData.PerformCallback(command);
                break;
            case "save":
                if (window.ClientCostingNo && !ASPxClientEdit.ValidateEditorsInContainerById("MailForm"))
                    return;
                if (!ASPxClientEdit.ValidateGroup('group1'))
                    return alert('Field is required');
                //var edit = editor.Get("Name");
                var _Disp = ClientDisponsition.GetValue();
                if (_Disp != null) {
                    var checkvalue = false;
                    if (CmbIncoterm.GetValue() != null)
                        if (CmbIncoterm.GetValue() != "0")
                        checkvalue = true;
                    if (CmbPaymentTerm.GetValue() != null && checkvalue == false)
                        if (CmbPaymentTerm.GetValue() != "0" && checkvalue == false)
                        checkvalue = true;
                    if (checkvalue == false) return alert('please select one incoterm or payment');
                }
                gridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + state.Key, function (values) {
                    if ((_Disp == 3 || _Disp == 5 || _Disp == 7) && (CmbReason.GetText() == "")) {
                        switch (_Disp) {
                            case "3":
                                alert('The Reason should not be empty...');
                                break;
                            //case "5": case "7":
                            //    alert('The Assignee should not be empty...');
                            //    break;
                        }
                        CmbReason.SetFocus();
                        Demo.ChangeDemoState("MailForm", "EditDraft", state.Key);
                        return;
                    }
                    //if ((values["StatusApp"] != "2" && values["StatusApp"] != "-1" && values["StatusApp"] != "0")) {
                    //    ClientDisponsition.SetFocus();
                    //    Demo.ChangeDemoState("MailForm", "EditDraft", state.Key);
                    //    return alert('The Disposition should not be empty...');
                    //} else {
                        Demo.ChangeDemoState("MailList");
                        Demo.DoCallback(gridData, function () {
                            gridData.PerformCallback("post|" + state.Key);
                        });
                  // }
                });
                break;
            case "new":
                Demo.ChangeDemoState("MailForm", "New", 0);
                break;
            case "delete":
                //
                if (!window.confirm("Confirm Delete?"))
                    return;
                var keys = [];
                if (state.View == "MailList") {
                    keys = gridData.GetSelectedKeysOnPage();
                } else if (state.View == "MailForm") {
                    keys = [state.Key];
                    Demo.ChangeDemoState("MailList");
                }
                if (keys.length > 0) {
                    Demo.DoCallback(gridData, function () {
                        gridData.PerformCallback("delete|" + keys.join("|"));
                    });
                    Demo.MarkMessagesAsRead(true, keys);
                }
                break;
        }
    },
    MarkMessagesAsRead: function (read, keys) {
        var sendCallback = false;
    },
    ClearForm: function () {
        curentEditingIndex = 0;
        ClientCurrency.SetSelectedIndex(-1);
        seExchangeRate.SetText("");
        Clientinterest.SetText("");
        tbFreight.SetText("");
        CmbIncoterm.SetSelectedIndex(-1);
        CmbRoute.SetSelectedIndex(-1);
        CmbSize.SetSelectedIndex(-1);
        tbInsurance.SetText("");
        ClientCustomer.SetSelectedIndex(-1);
        ClientShipTo.SetSelectedIndex(-1);

        //tbNumberContainer.SetText("");
        CmbPaymentTerm.SetSelectedIndex(-1);
        ClientCommission.SetSelectedIndex(-1);
        //tcDemos.SetActiveTabIndex(0);
        //gvcal.PerformCallback('clear|0');
        GetBuildCalcu(false);
    },
    ShowForm: function (command, key) {
        Demo.HideForm();
        Demo.ClearForm();
        //var appr = editor.Get("Name");
        //

        //ClientPreviewPanel.SetContentHtml("");
        //if (appr != 0) {
        //    Demo.ChangeDemoState("MailList");
        //    return alert("You are not authorized to access this page.");
        //}
        //else
 
        ClientFormPanel.SetVisible(["read", "Reply", "EditDraft"].indexOf(command));
        //
        if (command == "New") {
            gridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;
                    //ClientintGrid.PerformCallback('AddRow|1|' + values["RequestType"]);
                    //ClientCompanyEdit.SetEnabled(true);
                    //ClientValidfromEdit.SetText(null);
                    //ClientValidtoEdit.SetText(null);
                    //ClientNewID.Set("NewID", values["NewID"]);
                    ClientRequestNo.SetText("###########");
                    approv.Set("approv", values["editor"]);
                    hfid.Set('hidden_value', 0);
                    //gvcal.PerformCallback(["new", key].join("|"));
                    gv.PerformCallback(["reload", key].join("|"));
                    //gvimport.PerformCallback(["reload", key].join("|"));
                    ClientDisponsition.PerformCallback('0');
                };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientDisponsition });
            });
            Demo.ShowLoadingPanel(ClientPreviewPanel.GetMainElement());
        }
        if (command == "Reply" || command == "EditDraft") {
            gridData.GetValuesOnCustomCallback("MailForm|" + command + "|" + key, function (values) {
                var setValuesFunc = function () {
                    Demo.HideLoadingPanel();
                    if (!values)
                        return;
                    ClientRequestNo.SetText(values["RequestNo"]);
                    editor.Set("Name", values["editor"]);
                    var b = (values["editor"] == "0");
                    Demo.SetEnabledForm(b);
                    hfid.Set('hidden_value', values["ID"]);
                    hfStatusApp.Set('StatusApp', values["StatusApp"]);
                    approv.Set("approv", values["editor"]);
                    Clientinterest.SetText(values["Interest"]);
                    tbFreight.SetText(values["Freight"]);
                    CmbIncoterm.SetValue(values["Incoterm"]);
                    CmbRoute.SetValue(values["Route"]);
                    CmbSize.SetValue(values["Size"]);
                    tbInsurance.SetText(values["Insurance"]);
                    ClientCustomer.SetValue(values["Customer"]);
                    ClientShipTo.SetValue(values["ShipTo"]);
                    //tbNumberContainer.SetText("");
                    ClientCommission.SetValue(values["Commission"]);
                    CmbPaymentTerm.SetValue(values["PaymentTerm"]);

                    ClientDisponsition.PerformCallback(values["UniqueColumn"]);
                    var text = ["reload", key];
                    var arge = text.join("|");
                    gv.PerformCallback(arge);

                };
                Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientDisponsition });
            });
            Demo.ShowLoadingPanel(ClientFormPanel.GetMainElement());
        }
    },
    SetEnable: function (b) {
        //
        CmbRoute.SetEnabled(b);
        CmbSize.SetEnabled(b);
        //tbNumberContainer.SetEnabled(b);
        //tbContainers.SetEnabled(b);
    },
    SetEnabledForm: function (b) {
        //updatepriceform
        //ClientCompany.SetEnabled(b);
        ClientDisponsition.SetEnabled(b);
        Upload.SetEnabled(b);
    },
    ShowGrid: function () {
        Demo.HideLoadingPanel();
        //Demo.HideCostPreview();
        //Demo.HideEntryForm();
        gridData.SetVisible(true);
        Demo.ClientLayout_PaneResized();
    },
    HideForm: function () {
        //if (window.ClientAddressBookPopup)
        //    ClientAddressBookPopup.Hide();
        gridData.SetVisible(false);
    },
    UpdateMailGridKeyFolderHash: function () {
        var hash = {};
        for (var folderName in gridData.cpVisibleMailKeysHash) {
            var keys = gridData.cpVisibleMailKeysHash[folderName];
            if (!keys || keys.length == 0)
                continue;
            hash[folderName] = [];
            for (var i = 0; i < keys.length; i++)
                hash[keys[i]] = folderName;
        }
        gridData.cpKeyFolderHash = hash;
    },
    Init: function (s, e) {
        var key = Demo.getUrlVars()["ID"];
        if (!key)
            return;
        //alert(key);
        //var form = Demo.getUrlVars()["form"];
        hfid.Set("hidden_value", key);
        Demo.ChangeDemoState("MailForm", "EditDraft", key);
    },
});
UploadModule = CreateClass(PageModuleBase, {
    constructor: function () {
        this.DemoState = { View: "MailList" };
    },
    OnStateChanged: function () {
        var state = this.DemoState;
        if (state.View == "MailList")
            this.ShowGrid();
    },
    ShowMenuItems: function () {
        var role = editor.Get("Name");
        ClientActionMenu.GetItemByName("save").SetVisible(role == 0 ? true : false);
        if (role != 0) {
            return alert("You are not authorized to access this page.");
        }
    },
    ClientActionMenu_ItemClick: function (s, e) {
        var command = e.item.name;
        switch (command) {
            case "save":
                if (CmbCompany.GetText() == "") return alert( "please choose bu!!!");
                UploadControl.Upload(); 
                break;
        }
    },
    ClientLayout_PaneResized: function () {
        //
        var state = Demo.DemoState;
        if (!state) return;
        if (DevAVPageName == "UploadControl") {
            ClientFormPanel.SetHeight(ASPxClientUtils.GetDocumentClientHeight());
        }
    },
    ShowGrid: function () {
        //gridData.SetVisible(true);
        Demo.ClientLayout_PaneResized();
    }

});
ReportPageModule = CreateClass(PageModuleBase, {
    constructor: function () {
        this.DemoState = { View: "MailList" };
    },
    OnStateChanged: function () {
        var state = this.DemoState;
        if (state.View == "MailList")
            this.ShowGrid();
    },
    ShowGrid: function () {
        gridData.SetVisible(true);
        Demo.ClientLayout_PaneResized();
    },
    gridData_Init: function (s, e) {
        approv.Set("approv", "Vest");
        Demo.UpdateMailGridKeyFolderHash();
    },
    UpdateMailGridKeyFolderHash: function () {
        var hash = {};
        for (var folderName in gridData.cpVisibleMailKeysHash) {
            var keys = gridData.cpVisibleMailKeysHash[folderName];
            if (!keys || keys.length == 0)
                continue;
            hash[folderName] = [];
            for (var i = 0; i < keys.length; i++)
                hash[keys[i]] = folderName;
        }
        gridData.cpKeyFolderHash = hash;
    },
    ShowMenuItems: function () {
        //
        var role = editor.Get("Name");
        ClientActionMenu.GetItemByName("Filter").SetVisible(role == 0 ? true : false);
        ClientActionMenu.GetItemByName("ExportToXLS").SetVisible(role == 0 ? true : false);
        if (role != 0) {
            return alert("You are not authorized to access this page.");
        }
    },
    ClientActionMenu_ItemClick:function (s, e) {
        var command = e.item.name;
        switch (command) {
            case "Filter":
                var popup = Demo.GetPopupControl();
                popup.Show();
                popup.BringToFront();
                break;
            case "ExportToXLS":
                btn.DoClick();
                break;
        }
    },
    ClientLayout_PaneResized: function () {
        if (DevAVPageName == "TechnicalReport") {
            gridData.SetHeight(0);
            gridData.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
        }
    },
    OnSearchTextChanged: function () {
        var processed = MailPageModule.base.prototype.OnSearchTextChanged.call(Demo);
        if (processed) return;
        //Demo.ChangeDemoState("MailList");
        Demo.DoCallback(gridData, function () {
            gridData.PerformCallback("Search");
        });
    },
    GetPopupControl: function () {
        return ASPxPopupClientControl;
    }
});
EntryPageModule = CreateClass(PageModuleBase, {
    constructor: function () {
        this.DemoState = { View: "MailList" };
    },
    OnStateChanged: function () {
        var state = this.DemoState;
        if (state.View == "MailList")
            this.ShowEntryGrid();
        //if (state.View == "MailPreview")
        //    this.ShowEntryPreview(state.Key);
        if (state.View == "MailForm")
            this.ShowEntryForm(state.Command, state.Key);
    },
    OnSearchTextChanged: function () {
        var processed = MailPageModule.base.prototype.OnSearchTextChanged.call(Demo);
        if (processed) return;
        Demo.ChangeDemoState("MailList");
        Demo.DoCallback(grid, function () {
            grid.PerformCallback("Search");
        });
    },
    //    ClientEntryGrid_Init: function (s, e) {
    //        Demo.UpdateMailGridKeyFolderHash();
    //    },
    //    ClientFormPanel_Init: function (s, e) {
    //        Demo.DoCallback(s, function () {
    //            s.PerformCallback();
    //        });
    //    },
    //    ClientEntryGrid_RowClick: function (s, e) {
    //        var src = ASPxClientUtils.GetEventSource(e.htmlEvent);
    //        if (src.tagName == "TD" && src.className.indexOf("dxgvCommandColumn") != -1) // selection cell
    //            return;
    //        if (!s.IsDataRow(e.visibleIndex))
    //            return;
    //        var key = s.GetRowKey(e.visibleIndex);
    //        //if (ClientMailTree.GetSelectedNode().name === "Drafts")
    //        Demo.ChangeDemoState("MailForm", "EditDraft", key);
    //        //else
    //        //    Demo.ChangeDemoState("MailPreview", "", key);
    //    },
    ClientLayout_PaneResized: function () {
        if (DevAVPageName == "EditForm") {
            grid.SetHeight(0);
            grid.SetHeight(ASPxClientUtils.GetDocumentClientHeight() - TopPanel.GetHeight());
        }
    },
    ShowMenuItems: function () {
        var view = Demo.DemoState.View;
        //var key = Demo.getUrlVars()["viewMode"];
        //var edit = editor.Get("Name");
        ClientActionMenu.GetItemByName("back").SetVisible(view != "MailList");
        ClientActionMenu.GetItemByName("new").SetVisible(view != "MailForm");
        //ClientActionMenu.GetItemByName("new").SetVisible(edit);
        ClientActionMenu.GetItemByName("delete").SetVisible(view != "MailForm");
        ClientInfoMenu.GetItemByName("print").SetVisible(false);
    },
    ClientActionMenu_ItemClick: function (s, e) {
        var command = e.item.name;
        var state = Demo.DemoState;
        switch (command) {
            case "new":
                //Demo.ChangeDemoState("MailForm", "New");
                //pcLogin.Show();
                grid.AddNewRow();
                break;
            //            case "reply":
            //                Demo.ChangeDemoState("MailForm", "Reply", state.Key);
            //                break;
            //            case "back":
            //                Demo.ChangeDemoState("MailList");
            //                break;
            case "delete":
                if (!window.confirm("Confirm Delete?"))
                    return;
                var keys = [];
                //                if (state.View == "MailList") {
                //                    keys = ClientEntryGrid.GetSelectedKeysOnPage();
                //                } else if (state.View == "MailPreview") {
                //                    keys = [state.Key];
                //                    Demo.ChangeDemoState("MailList");
                //                }
                //                if (keys.length > 0) {
                //                    Demo.DoCallback(ClientEntryGrid, function () {
                //                        ClientEntryGrid.PerformCallback("Delete|" + keys.join("|"));
                //                    });
                //                    Demo.MarkMessagesAsRead(true, keys);
                //                }
                var index = grid.GetFocusedRowIndex();
                grid.DeleteRow(index);
                break;
            //            case "send":
            //            case "save":
            //                if (window.ClientCostingNo && !ASPxClientEdit.ValidateEditorsInContainerById("MailForm"))
            //                    return;
            //                var args = command == "send" ? "SendMail" : "SaveMail";
            //                if (state.Command === "EditDraft")
            //                    args += "|" + state.Key;
            //                Demo.ChangeDemoState("MailList");
            //                Demo.DoCallback(ClientEntryGrid, function () {
            //                    ClientEntryGrid.PerformCallback(args);
            //                });
            //                break;
            //            case "read":
            //            case "unread":
            //                var selectedKeys = ClientEntryGrid.GetSelectedKeysOnPage();
            //                if (selectedKeys.length == 0)
            //                    return;
            //                ClientEntryGrid.UnselectAllRowsOnPage();
            //                Demo.MarkMessagesAsRead(command == "read", selectedKeys);
            //                break;
        }
    },
    ShowEntryGrid: function () {
        Demo.HideLoadingPanel();
        //Demo.HideCostPreview();
        //Demo.HideEntryForm();
        grid.SetVisible(true);
        Demo.ClientLayout_PaneResized();
    },

    //    HideEntryGrid: function () {
    //        ClientEntryGrid.SetVisible(false);
    //    },
    ShowEntryForm: function (command, key) {
        Demo.HideEntryGrid();
        //    Demo.HideCostPreview();
        //ClientCompany.SetText("");
        //ClientCostingNo.SetValue("");
        //ClientCostingNo.SetIsValid(true);
        //ClientMarketingNo.SetValue("");
        //ClientRDNumber.SetText("");
        //ClientRDNumber.Focus();

        ////ClientintGrid.PerformCallback('clear');
        //ClientEntryFormPanel.SetVisible(true);
        //ClientRDNumber.AdjustControl();
        //Demo.ClientLayout_PaneResized();

        //if (command == "Reply" || command == "EditDraft") {
        //    ClientEntryGrid.GetValuesOnCustomCallback("MailForm|" + command + "|" + key, function (values) {
        //        var setValuesFunc = function () {
        //            Demo.HideLoadingPanel();
        //            if (!values)
        //                return;
        //            ClientCompany.SetValue(values["Company"]);
        //            ClientCostingNo.SetText(values["RequestNo"]);
        //            ClientMarketingNo.SetValue(values["Marketingnumber"]);
        //            ClientRDNumber.SetValue(values["RDNumber"]);
        //            ClientRDNumber.Focus();
        //        };
        //        Demo.PostponeAction(setValuesFunc, function () { return !!window.ClientMarketingNo });
        //    });
        //    Demo.ShowLoadingPanel(ClientEntryFormPanel.GetMainElement());
        //}
    },
    //    HideEntryForm: function () {
    //        ClientEntryFormPanel.SetVisible(false);
    //    },
    //    UpdateMailGridKeyFolderHash: function () {
    //        var hash = {};
    //        for (var folderName in ClientEntryGrid.cpVisibleMailKeysHash) {
    //            var keys = ClientEntryGrid.cpVisibleMailKeysHash[folderName];
    //            if (!keys || keys.length == 0)
    //                continue;
    //            hash[folderName] = [];
    //            for (var i = 0; i < keys.length; i++)
    //                hash[keys[i]] = folderName;
    //        }
    //        ClientEntryGrid.cpKeyFolderHash = hash;
    //        //ClientEntryGrid.SetVisible(false);
    //        //ClientEntryFormPanel.SetVisible(true);
    //        //ClientEntryGrid.GetValuesOnCustomCallback("MailForm|EditDraft|" + Folio);
    //    }
});
(function () {
    //alert(DevAVPageName);  
    //var myParam = location.search.split('viewMode=')[1];
    //DevAVPageName = myParam == "undefined" || myParam == null ? "CustomerEditForm" : myParam;
    var value = "viewMode";
    var vars = {};
    var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi,
    function(m, key, value) {
      vars[key]= value;
    });
    if (vars[value]!= undefined)
        DevAVPageName = vars[value];
    var pageModule;
    var bodyElement = $("body");
    //if (bodyElement.hasClass("mail"))
    if (DevAVPageName == "CustomerEditForm")
        pageModule = new MailPageModule();
    else if (DevAVPageName == "CostingEditForm")
        pageModule = new CostPageModule();
    else if (DevAVPageName == "formulaForm")
        pageModule = new formulaPageModule();
    else if (DevAVPageName == "EditForm")
        pageModule = new EntryPageModule();
    else if (DevAVPageName == "UpdatePriceForm")
        pageModule = new UpdatePriceModule();
    else if (DevAVPageName == "UploadControl")
        pageModule = new UploadModule();
    else if (DevAVPageName == "TechnicalReport")
        pageModule = new ReportPageModule();
    else if (DevAVPageName == "QuotationForm" || DevAVPageName == "OfferPriceForm")
        pageModule = new QuotaPageModule();
    else if (DevAVPageName == "CalculationForm")
        pageModule = new CalculaModule();
	else if (DevAVPageName == "CalculationForm2")
        pageModule = new CalculaModule2();
    else
    pageModule = new PageModuleBase();
    window.Demo = pageModule;
})();