<%@ Control Language="C#" AutoEventWireup="true" CodeFile="adminform.ascx.cs" Inherits="UserControls_adminControls_adminform" %>
<script type="text/javascript">
    function OnInit(s, e) {
        debugger;
        var edit = editor.Get("Name");
        grid.SetVisible(edit);
            AdjustSize();
            document.getElementById("gridContainer").style.visibility = "";
        }
        function OnEndCallback(s, e) {
            AdjustSize();
        }
        function OnControlsInitialized(s, e) {
            ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
                AdjustSize();
            });
        }
        function AdjustSize() {
            var height = Math.max(0, document.documentElement.clientHeight);
            testGrid.SetHeight(height - 420);
        }
        var lastCountry = null;
    function OnSelectedIndexChanged(s, e) {
           debugger;
            lastCountry = s.GetSelectedItem().GetColumnText(1);//s.GetText().toString();
            //grid.SetEditValue('Description', lastCountry);
            grid.SetEditValue('FirstName', lastCountry);
            grid.SetEditValue('LastName', s.GetSelectedItem().GetColumnText(2));
            //alert(lastCountry);
        }
        function OnChanged(s, e) {
            lastCountry = s.GetSelectedItem().GetColumnText(1);//s.GetText().toString();
            grid.SetEditValue('Name', lastCountry);
            //alert(lastCountry);
        }
        function OnContextMenuItemClick(sender, args) {
            if (args.objectType == "row") {
                if (args.item.name == "ExportToPDF" || args.item.name == "ExportToXLS") {
                    args.processOnServer = true;
                    args.usePostBack = true;
                }
            }
        }
        function OnFileUploadComplete(s, e) {
            //alert("test");
            testGrid.PerformCallback('upload|1');
        }
        function OnFilesUploadStart(s, e) {
            debugger;
            uploader.Set('uploader', Demo._menu);
        }
        function SynchronizeListBoxValues(dropDown, args, text) {
            //debugger;
            var texts = dropDown.GetText().split(textSeparator);
            var values = GetValuesByTexts(texts, text);
                checkListBox.SelectValues(values);
                UpdateText(text); // for remove non-existing texts
            if (values != "")
                dropDown.isValid = false;
        }
        var textSeparator = ";";
        function OnListBoxSelectionChanged(s, e, text) {
            //alert("test :" + text);
            UpdateText(text);
        }
        function UpdateText(text) {
            var selectedItems = null;
                 selectedItems = checkListBox.GetSelectedItems();
                 dropDownEdit.SetText(GetSelectedItemsText(selectedItems));
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
                    item = checkListBox.FindItemByText(texts[i]);
                if (item != null)
                    actualValues.push(item.value);
            }
            return actualValues;
        }
        //function OnSelectedIndexChanged(s, e) {
        //    debugger;
        //    var selectedIndices = s.GetSelectedIndices();
        //    var text = '';
        //    for (i = 0; i < selectedIndices.length; i++) {
        //        //text += selectedIndices[i].toString();
        //        text += s.itemsValue[i].toString();
        //        if (i < selectedIndices.length - 1)
        //            text += ',';
        //    }
        //    dropDownEdit.SetText(text);
        //}
    </script>
<dx:ASPxHiddenField ID="hBu" runat="server" ClientInstanceName="hBu"/>
<dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
<dx:ASPxHiddenField ID="approv" runat="server" ClientInstanceName="approv" /> 
<dx:ASPxHiddenField ID="uploader" runat="server" ClientInstanceName="uploader" /> 
<dx:ASPxHiddenField ID="hfgetvalue" runat="server" ClientInstanceName="hfgetvalue" />
<dx:ASPxHiddenField ID="usertp" runat="server" ClientInstanceName="usertp" />
<div id="gridContainer" style="visibility: hidden">
<dx:ASPxGridView runat="server" ID="grid" ClientInstanceName="grid" OnCustomCallback="grid_CustomCallback" 
    OnFillContextMenuItems="grid_FillContextMenuItems" OnContextMenuItemClick="grid_ContextMenuItemClick"
    OnCellEditorInitialize="grid_CellEditorInitialize" KeyFieldName="ID" EnableViewState="false"
    OnCommandButtonInitialize="grid_CommandButtonInitialize"
    OnDataBinding="grid_DataBinding" Width="100%"
    OnDataBound="grid_DataBound" 
    OnRowUpdating="grid_RowUpdating"
    OnRowDeleting="grid_RowDeleting"
    OnRowInserting="grid_RowInserting"    
    Border-BorderWidth="0">
    <ClientSideEvents Init="OnInit" EndCallback="OnEndCallback" ContextMenuItemClick="function(s,e) { OnContextMenuItemClick(s, e); }"/>
    <SettingsSearchPanel ColumnNames="" Visible="false" />
	<Settings VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="true" />
	<SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true" AllowFocusedRow="true"/>
	<SettingsPager PageSize="50">
    <PageSizeItemSettings Visible="true" ShowAllItem="true" AllItemText="All Records" />
    </SettingsPager>
	<Styles>
		<Row Cursor="pointer" />
	</Styles>
    <SettingsContextMenu Enabled="true" />
    <SettingsEditing EditFormColumnCount="1" Mode="PopupEditForm" />
    <SettingsCommandButton EditButton-Image-Url="~/Content/images/icons8-edit-16.png" EditButton-RenderMode="Image"
        UpdateButton-RenderMode="Button"  CancelButton-RenderMode="Button"  />
    <SettingsPopup>
            <EditForm Modal="true" AllowResize="true" />
    </SettingsPopup>
</dx:ASPxGridView>
<dx:ASPxGridViewExporter runat="server" ID="GridExporter" GridViewID="grid" />
<%--<div style="margin: 16px auto; width: 160px;"></div>--%>
<dx:ASPxCallbackPanel ID="PreviewPanel" runat="server" RenderMode="Div" Height="100%" CssClass="PreviewPanel" ClientInstanceName="ClientPreviewPanel" 
        ClientVisible="false" OnCallback="PreviewPanel_Callback" />
<dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" ClientVisible="false" ScrollBars="Vertical" ClientInstanceName="ClientFormPanel">
    <PanelCollection>
        <dx:PanelContent>  
        <dx:ASPxFormLayout ID="ContactForm" runat="server">
	        <Items>
		        <dx:LayoutGroup Caption="MainInfo" GroupBoxDecoration="HeadingLine">
			        <Items> 
                    <dx:LayoutItem Caption="Company" CaptionSettings-Location="Left">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="CmbCompany" runat="server" DataSourceID="dsCompany" 
                                            ValueField="Code" Width="200px"
                                            TextFormatString="{0}">
                                            <Columns>
                                                <dx:ListBoxColumn FieldName="Code"/>
                                                <dx:ListBoxColumn FieldName="Name"/>
                                            </Columns>
                                        </dx:ASPxComboBox>
                                <%--<dx:ASPxButton ID="btnBlueBall" RenderMode="Link" runat="server" Image-Url="~/Content/images/update.png" />--%>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Upload file" Name="Upload" CaptionSettings-Location="Left">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer>
                        <dx:ASPxUploadControl ID="Upload" runat="server" OnFileUploadComplete="Upload_FileUploadComplete" Width="200" ClientInstanceName="Upload" 
                            NullText="Click here to browse files..." UploadMode="Advanced" FileUploadMode="OnPageLoad" AutoStartUpload="True">
                            <AdvancedModeSettings EnableMultiSelect="True" EnableFileList="True" EnableDragAndDrop="True" />
                            <ValidationSettings AllowedFileExtensions=".xls,.xlsx"/>
                            <ClientSideEvents FileUploadComplete="OnFileUploadComplete" FilesUploadStart="OnFilesUploadStart" />
                        </dx:ASPxUploadControl>
                        </dx:LayoutItemNestedControlContainer>
                </LayoutItemNestedControlCollection>
                </dx:LayoutItem>

				    <dx:LayoutItem Caption="">
							        <LayoutItemNestedControlCollection>
								        <dx:LayoutItemNestedControlContainer runat="server">
                        <dx:ASPxGridView ID="testGrid" ClientInstanceName="testGrid" runat="server" Width="100%"   
                        OnDataBinding="testGrid_DataBinding" OnDataBound="testGrid_DataBound"
                        OnCustomCallback="testGrid_CustomCallback" KeyFieldName="ID" EnableRowsCache="false">
                            <SettingsSearchPanel ColumnNames="" Visible="false" />
		                    <Settings ShowGroupPanel="false" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="true" ShowStatusBar="Hidden"/>
		                    <SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true"/>
		                    <SettingsPager PageSize="50">
                                <PageSizeItemSettings Visible="true" ShowAllItem="true"  AllItemText="All Records" />
                            </SettingsPager>
		                    <Styles>
			                    <Row Cursor="pointer" />
		                    </Styles>
                            <SettingsContextMenu Enabled="true" />
                            </dx:ASPxGridView>
								        </dx:LayoutItemNestedControlContainer>
							        </LayoutItemNestedControlCollection>
						        </dx:LayoutItem>
                            </Items>
					        <ParentContainerStyle CssClass="InfoLayoutGroup" />
				        </dx:LayoutGroup>
			        </Items>
			        <SettingsItemCaptions Location="Top" />
		        </dx:ASPxFormLayout>  
            </dx:PanelContent>
        </PanelCollection>
        </dx:ASPxCallbackPanel>
        <dx:ASPxSpreadsheet ID="Spreadsheet" runat="server" ActiveTabIndex="0" ShowConfirmOnLosingChanges="false" 
            Visible="false">
        </dx:ASPxSpreadsheet>
        <dx:ASPxPopupControl ID="pcLogin" runat="server" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcLogin"
        Width="900px"
        HeaderText="Create Master" AllowDragging="True" PopupAnimationType="None" EnableViewState="False">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxPanel ID="Panel1" runat="server" DefaultButton="btOK">
                    <PanelCollection>
                        <dx:PanelContent runat="server"></dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <ContentStyle>
            <Paddings PaddingBottom="5px" />
        </ContentStyle>
    </dx:ASPxPopupControl>
</div>
<asp:SqlDataSource ID="ds" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"/>
<asp:SqlDataSource ID="dsMenu" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select * from MasMenu" UpdateCommand="update MasMenu set [Text]=@Text Where ID=@ID" 
    DeleteCommand="Delete MasMenu where ID=@ID">
    <UpdateParameters>
        <asp:Parameter Name="Text" Type="String" />
    </UpdateParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsCompany" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
    SelectCommand="select code,Name from MasCompany">
</asp:SqlDataSource>
<asp:SqlDataSource ID="dsMasPrice" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        DeleteCommand="Delete MasPrice Where ID=@ID" 
        SelectCommand="select * from MasPrice where Company in (select distinct value from dbo.FNC_SPLIT(@Bu,'|'))" 
        UpdateCommand="Update MasPrice set Company=@Company,SAPMaterial=@SAPMaterial,Name=@Name,Validto=@Validto,Price=@Price,Currency=@Currency,Unit=@Unit where ID=@ID"
        InsertCommand="Insert into MasPrice values(@Company,@SAPMaterial,@Name,@Validto,@Price,@Currency,@Unit)">
        <SelectParameters>
             <asp:ControlParameter ControlID="hBu" Name="BU" PropertyName="['BU']"/>
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Company" Type="String" />
            <asp:Parameter Name="SAPMaterial" Type="String" />
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="Validto" Type="DateTime" />
            <asp:Parameter Name="Price" Type="String" />
            <asp:Parameter Name="Currency" Type="String" />
            <asp:Parameter Name="Unit" Type="String" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="Company" Type="String" />
            <asp:Parameter Name="SAPMaterial" Type="String" />
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="Validto" Type="DateTime" />
            <asp:Parameter Name="Price" Type="String" />
            <asp:Parameter Name="Currency" Type="String" />
            <asp:Parameter Name="Unit" Type="String" />
        </InsertParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsMasPricePolicy" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        DeleteCommand="Delete MasPricePolicy Where ID=@ID" 
        SelectCommand="select * from MasPricePolicy">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsMasYield" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        DeleteCommand="Delete MasYield Where ID=@ID"
        SelectCommand="select * from MasYield where Company in (select distinct value from dbo.FNC_SPLIT(@Bu,'|'))" 
        InsertCommand="Insert Into MasYield values(@Company,@Material,@Name,@RawMaterial,@Description,@Yield)"
        UpdateCommand="Update MasYield set Company=@Company,Material=@Material,Name=@Name,RawMaterial=@RawMaterial,Description=@Description,Yield=@Yield where Id=@Id" >
        <SelectParameters>
             <asp:ControlParameter ControlID="hBu" Name="BU" PropertyName="['BU']"/>
        </SelectParameters>
        <InsertParameters>
            <asp:Parameter Name="Company" Type="String" />
            <asp:Parameter Name="Material" Type="String" />
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="RawMaterial" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Yield" Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="Company" Type="String" />
            <asp:Parameter Name="Material" Type="String" />
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="RawMaterial" Type="String" />
            <asp:Parameter Name="Description" Type="String" />
            <asp:Parameter Name="Yield" Type="String" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsulogin" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spselectulogin" SelectCommandType="StoredProcedure"
        DeleteCommand="Delete ulogin where au_id=@au_id"
        InsertCommand="Insert into ulogin values (@user_name,@Position,@userlevel,@FirstName,@LastName,@Email,@BU,0,@sublevel,@usertype)"  
        UpdateCommand="Update ulogin set [user_name]=@user_name,Position=@Position,Email=@Email,BU=@BU,IsResign=@IsResign,userlevel=@userlevel,sublevel=@sublevel,usertype=@usertype where au_id=@au_id">
        <SelectParameters>
             <asp:ControlParameter ControlID="hBu" Name="BU" PropertyName="['BU']"/>
             <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
        <InsertParameters>
            <asp:Parameter Name="user_name" Type="String" />
            <asp:Parameter Name="Position" Type="String" />
            <asp:Parameter Name="userlevel" Type="String" />
            <asp:Parameter Name="FirstName" Type="String" />
            <asp:Parameter Name="LastName" Type="String" />
            <asp:Parameter Name="Email" Type="String" />
            <asp:Parameter Name="BU" Type="String" />
            <asp:Parameter Name="sublevel" Type="String" />
            <asp:Parameter Name="usertype" Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="Position" Type="String" />
            <asp:Parameter Name="Email" Type="String" />
            <asp:Parameter Name="BU" Type="String" />
            <asp:Parameter Name="IsResign" Type="Boolean" />
            <asp:Parameter Name="userlevel" Type="String" />
            <asp:Parameter Name="sublevel" Type="String" />
            <asp:Parameter Name="user_name" Type="String" />
            <asp:Parameter Name="usertype" Type="String" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsApprovAssign" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetApprovAssign" SelectCommandType="StoredProcedure"
        InsertCommand="Insert into MasApprovAssign values (@empid,@Sublevel)"
        UpdateCommand="Update MasApprovAssign set Sublevel=@Sublevel Where Id=@Id"
        DeleteCommand="delete MasApprovAssign where Id=@Id" >
        <SelectParameters>
             <asp:ControlParameter ControlID="hBu" Name="BU" PropertyName="['BU']"/>
             <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Sublevel" Type="String" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="empid" Type="String" />
            <asp:Parameter Name="Sublevel" Type="String" />
        </InsertParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsSublevel" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasSublevel">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsSubType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('Raw Material & Ingredient;Primary Packaging;Secondary Packaging;Damage & Loss',';')">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsSAPMaterial" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from SAPMaterial">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsCurrency" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('THB;USD;',';')">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsExchangeRat" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasExchangeRat where Company in (select distinct value from dbo.FNC_SPLIT(@Bu,'|'))"
        UpdateCommand="Update MasExchangeRat set Company=@Company,Validfrom=@Validfrom,Validto=@Validto,Ratio=@Ratio,Currency=@Currency,Rate=@Rate,CurrencyTh=@CurrencyTh,ExchangeType=@ExchangeType where ID=@ID" 
        InsertCommand="Insert into MasExchangeRat values (@Company,@Validfrom,@Validto,@Ratio,@Currency,@Rate,@CurrencyTh,@ExchangeType)"
        DeleteCommand="Delete MasExchangeRat where ID=@ID">
       <SelectParameters>
             <asp:ControlParameter ControlID="hBu" Name="BU" PropertyName="['BU']"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsUserlevel" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select 0 as 'Id','User' as 'Name' union select 1,'KeyUser' union select 2,'Admin' union select 3,'CC'">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsCustomerPrice" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasCustomerPrice">
    </asp:SqlDataSource>
<asp:SqlDataSource ID="dsPrimary" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasPrimary">
    </asp:SqlDataSource>

<asp:SqlDataSource ID="dsusertype" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select 0 as 'Id','PF' as 'Name' union select 1,'HF'">
    </asp:SqlDataSource>
<asp:SqlDataSource ID="dsUnit" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('KG;G;Ton',';')">
    </asp:SqlDataSource>
<asp:SqlDataSource ID="dsReason" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasReason"
        UpdateCommand="Update MasReason set Description=@Description where ID=@ID" 
        InsertCommand="Insert into MasReason values (@Description)"
        DeleteCommand="Delete MasReason where ID=@ID"  >
    <UpdateParameters>
        <asp:Parameter Name="Description" Type="String" />
    </UpdateParameters>
    <InsertParameters>
        <asp:Parameter Name="Description" Type="String" />
    </InsertParameters>
    </asp:SqlDataSource>
<asp:SqlDataSource ID="dsFormulaByCode" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spUpdateFormulaByCode" SelectCommandType="StoredProcedure"
        UpdateCommand="Update TransFormulaHeader set Code=@Code,Name=@Name,RefSamples=@RefSamples where ID=@ID"
               DeleteCommand="Delete TransFormulaHeader where Id=@ID">
    <SelectParameters>
        <asp:SessionParameter Name="user" SessionField="user_name" Type="String" />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Name="Code" Type="String" />
        <asp:Parameter Name="ID" Type="Int32" />
        <asp:Parameter Name="Name" Type="String" />
        <asp:Parameter Name="RefSamples" Type="String" />
    </UpdateParameters>
    </asp:SqlDataSource>