<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditForm.ascx.cs" Inherits="EditForm" %>
<script type="text/javascript">
    function OnInit(s, e) {
        //debugger;
        //var edit = editor.Get("Name");
        //grid.SetVisible(edit);
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
        grid.SetHeight(height - 120);
    }
    function OnFileUploadComplete(s, e) {
        //testGrid.PerformCallback();
        //testGrid.PerformCallback('upload|1');
        alert("test");
    }
    var lastCountry = null;
    function OnSelectedIndexChanged(s, e) {
        debugger;
        lastCountry = s.GetSelectedItem().GetColumnText(1);//s.GetText().toString();
        //grid.SetEditValue('Description', lastCountry);
        grid.SetEditValue('firstname', lastCountry);
        grid.SetEditValue('lastname', s.GetSelectedItem().GetColumnText(2));
        //alert(lastCountry);
    }
    function OnChanged(s, e) {
        lastCountry = s.GetSelectedItem().GetColumnText(1);//s.GetText().toString();
        grid.SetEditValue('Name', lastCountry);
        //alert(lastCountry);
    }
    function OnContextMenuItemClick(sender, args) {
        debugger;
        if (args.objectType == "row") {
            if (args.item.name == "ExportToPDF" || args.item.name == "ExportToXLS") {
                args.processOnServer = true;
                args.usePostBack = true;
            }
        }
    }
    function SynchronizeListBoxValues(dropDown, args, text) {
        //debugger;
        var texts = dropDown.GetText().split(";");
        var values = GetValuesByTexts(texts, text, checkListBox);
        checkListBox.SelectValues(values);
        UpdateText(text, checkListBox, dropDownEdit); // for remove non-existing texts
        if (values != "")
            dropDown.isValid = false;
    }
    function OnListBoxSelectionChanged(s, e, text) {
        UpdateText(text, checkListBox, dropDownEdit);
        Session["FirstName"] 
    }
    function UpdateText(text, ListBox, dropDown) {
        var selectedItems = null;
        selectedItems =  ListBox.GetSelectedItems();
        dropDown.SetText(GetSelectedItemsText(selectedItems));
    }

    function SynchronizeListBoxValues1(dropDown, args, text) {
        //debugger;
        var texts = dropDown.GetText().split(";");
        var values = GetValuesByTexts(texts, text ,checkListBox1);
        checkListBox1.SelectValues(values);
        UpdateText(text, checkListBox1, dropDownEdit1); // for remove non-existing texts
        if (values != "")
            dropDown.isValid = false;
    }
    function OnListBoxSelectionChanged1(s, e, text) {
        UpdateText(text, checkListBox1, dropDownEdit1);
    }
     function SynchronizeListBoxValues2(dropDown, args, text) {
        //debugger;
        var texts = dropDown.GetText().split(";");
        var values = GetValuesByTexts(texts, text ,checkListBox2);
        checkListBox2.SelectValues(values);
        UpdateText(text, checkListBox2, dropDownEdit2); // for remove non-existing texts
        if (values != "")
            dropDown.isValid = false;
    }
    function OnListBoxSelectionChanged2(s, e, text) {
        UpdateText(text, checkListBox2, dropDownEdit2);
    }
    function SynchronizeListBoxValues3(dropDown, args, text) {
        //debugger;
        var texts = dropDown.GetText().split(";");
        var values = GetValuesByTexts(texts, text, checkListBox3);
        checkListBox3.SelectValues(values);
        UpdateText(text, checkListBox3, dropDownEdit3); // for remove non-existing texts
        if (values != "")
            dropDown.isValid = false;
    }
    function OnListBoxSelectionChanged3(s, e, text) {
        UpdateText(text, checkListBox3, dropDownEdit3);
    }
    function SynchronizeListBoxValues4(dropDown, args, text) {
        //debugger;
        var texts = dropDown.GetText().split(";");
        var values = GetValuesByTexts(texts, text, checkListBox4);
        checkListBox4.SelectValues(values);
        UpdateText(text, checkListBox4, dropDownEdit4); // for remove non-existing texts
        if (values != "")
            dropDown.isValid = false;
    }
    function OnListBoxSelectionChanged4(s, e, text) {
        UpdateText(text, checkListBox4, dropDownEdit4);
    }

    function SynchronizeListBoxValues5(dropDown, args, text) {
        //debugger;
        var texts = dropDown.GetText().split(";");
        var values = GetValuesByTexts(texts, text, checkListBox5);
        checkListBox5.SelectValues(values);
        UpdateText(text, checkListBox5, dropDownEdit5); // for remove non-existing texts
        if (values != "")
            dropDown.isValid = false;
    }
    function OnListBoxSelectionChanged5(s, e, text) {
        UpdateText(text, checkListBox5, dropDownEdit5);
    }
    function SynchronizeListBoxValues6(dropDown, args, text) {
        //debugger;
        var texts = dropDown.GetText().split(";");
        var values = GetValuesByTexts(texts, text, checkListBox6);
        checkListBox6.SelectValues(values);
        UpdateText(text, checkListBox6, dropDownEdit6); // for remove non-existing texts
        if (values != "")
            dropDown.isValid = false;
    }
    function OnListBoxSelectionChanged6(s, e, text) {
        UpdateText(text, checkListBox6, dropDownEdit6);
    }
    function GetSelectedItemsText(items) {
        var texts = [];
        for (var i = 0; i < items.length; i++)
            //if (items[i].index != 0)
            texts.push(items[i].text);
        return texts.join(";");
    }
    function GetValuesByTexts(texts, text , ListBox) {
        var actualValues = [];
        var item;
        for (var i = 0; i < texts.length; i++) {
            item = ListBox.FindItemByText(texts[i]);
            if (item != null)
                actualValues.push(item.value);
        }
        return actualValues;
    }

    function OnSelectedmaster(s, e) {
        debugger;
        var name = s.GetSelectedItem().GetColumnText(0);
         grid.PerformCallback('reload|' + name);
    }
</script>
<dx:ASPxHiddenField ID="hBu" runat="server" ClientInstanceName="hBu"/>
<dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
<dx:ASPxHiddenField ID="usertp" runat="server" ClientInstanceName="usertp" />
<div id="gridContainer" style="visibility: hidden"></div>
<dx:ASPxGridView runat="server" ID="grid" ClientInstanceName="grid" OnCustomCallback="grid_CustomCallback" OnCellEditorInitialize="grid_CellEditorInitialize"
OnFillContextMenuItems="grid_FillContextMenuItems" OnContextMenuItemClick="grid_ContextMenuItemClick" KeyFieldName="ID"
OnDataBinding="grid_DataBinding" Width="100%" EnableViewState="false" OnRowDeleting="grid_RowDeleting"
OnDataBound="grid_DataBound" 
Border-BorderWidth="0">
<Paddings PaddingTop="0px" Padding="1px" PaddingBottom="0px" PaddingLeft="0px" />
<Toolbars>
    <dx:GridViewToolbar ItemAlign="left">
        <Items>
            <dx:GridViewToolbarItem>
                <Template>
                    <dx:ASPxComboBox runat="server" ID="CmbSelect" ClientInstanceName="ClientSelect" ValueField="ID" Caption="Selected" 
                        DataSourceID="dsMaster" TextFormatString="{0}[{1}]" Width="340px">
                        <Columns>
                            <dx:ListBoxColumn FieldName="Text" />
                            <dx:ListBoxColumn FieldName="URL" />
                        </Columns>
                        <ClientSideEvents SelectedIndexChanged="OnSelectedmaster" />
                    </dx:ASPxComboBox>
                </Template>
            </dx:GridViewToolbarItem>
            <dx:GridViewToolbarItem Text="Export to" BeginGroup="true">
                <Image IconID="actions_download_16x16office2013"></Image>
                <Items>
                    <dx:GridViewToolbarItem  Command="ExportToPdf" Text="PDF" Image-IconID="export_exporttopdf_16x16office2013" />
                    <dx:GridViewToolbarItem  Command="ExportToXlsx" Text="XLSX" Image-IconID="export_exporttoxlsx_16x16office2013" />
                    <dx:GridViewToolbarItem  Command="ExportToXls" Text="XLS" Image-IconID="export_exporttoxls_16x16office2013" />
                </Items>
            </dx:GridViewToolbarItem>
            <dx:GridViewToolbarItem Command="Refresh" BeginGroup="true"></dx:GridViewToolbarItem>
        </Items>
    </dx:GridViewToolbar>
</Toolbars>
<ClientSideEvents Init="OnInit" EndCallback="OnEndCallback" ContextMenuItemClick="function(s,e) { OnContextMenuItemClick(s, e); }"/>
<SettingsSearchPanel ColumnNames="" Visible="false" />
<Settings ShowGroupPanel="false" VerticalScrollBarMode="Auto" VerticalScrollableHeight="0" ShowGroupedColumns="True" GridLines="Vertical" ShowFooter="true" />
<SettingsBehavior AutoExpandAllGroups="true" EnableRowHotTrack="True" ColumnResizeMode="Control" EnableCustomizationWindow="true" AllowFocusedRow="true"/>
<SettingsPager PageSize="50">
<PageSizeItemSettings Visible="true" ShowAllItem="true"  AllItemText="All Records" />
</SettingsPager>
<Styles>
	<Row Cursor="pointer" />
</Styles>
<SettingsContextMenu Enabled="true" />
<SettingsEditing EditFormColumnCount="1" Mode="PopupEditForm"/>
<SettingsPopup>
        <EditForm Modal="true" AllowResize="true"/>
</SettingsPopup>
<%--    <SettingsCommandButton EditButton-Image-Url="~/Content/images/Edit.gif" EditButton-RenderMode="Image"
    UpdateButton-Image-Url="~/Content/images/update.png" UpdateButton-RenderMode="Image"
    CancelButton-Image-Url="~/Content/images/cancel.png" CancelButton-RenderMode="Image"  />--%>
<SettingsCommandButton EditButton-Image-Url="~/Content/images/icons8-edit-16.png" EditButton-RenderMode="Image"
    UpdateButton-RenderMode="Button"  CancelButton-RenderMode="Button"  />
</dx:ASPxGridView>
<dx:ASPxGridViewExporter runat="server" ID="GridExporter" GridViewID="grid" />
<dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div"  ScrollBars="Vertical" ClientInstanceName="ClientFormPanel" ClientVisible="false" >
    <PanelCollection>
        <dx:PanelContent>  
        <dx:ASPxFormLayout ID="ASPxFormLayout1" runat="server" AlignItemCaptionsInAllGroups="True" UseDefaultPaddings="false">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
	        <Items>
		        <dx:LayoutGroup Caption="MainInfo" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                    <GroupBoxStyle>
                        <Caption Font-Bold="true" Font-Size="10"/>
                    </GroupBoxStyle>
                    <Items>
                    <%--<dx:LayoutItem Caption="&nbsp;Selected" Paddings-PaddingLeft="5">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                            <dx:ASPxComboBox runat="server" ID="CmbMaster" ClientInstanceName="ClientMaster" ValueField="ID" 
                              DataSourceID="dsMaster" TextFormatString="{0}" Width="340px">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="Text" />
                                    <dx:ListBoxColumn FieldName="URL" />
                                </Columns>
                                <ClientSideEvents SelectedIndexChanged="OnSelectedmaster" />
                            </dx:ASPxComboBox>
                            </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="" CaptionSettings-Location="Top" Paddings-PaddingLeft="5">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                            
                            
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>--%>
                    </Items>
                </dx:LayoutGroup>
            </Items>
        </dx:ASPxFormLayout>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxCallbackPanel>
<div style="margin: 16px auto; width: 160px;">
<dx:ASPxPopupControl ID="pcLogin" runat="server" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcLogin"
        Width="900px"
        HeaderText="Create Master" AllowDragging="True" PopupAnimationType="None" EnableViewState="False">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxPanel ID="Panel1" runat="server" DefaultButton="btOK">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                        <dx:ASPxFormLayout ID="ContactForm" runat="server" ColCount="3">
						<Items>
							<dx:LayoutGroup Caption="MainInfo" GroupBoxDecoration="HeadingLine">
								<Items>
									<dx:LayoutItem Caption="">
												<LayoutItemNestedControlCollection>
													<dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxGridView ID="testGrid" ClientInstanceName="testGrid" runat="server" Width="100%"
                                    OnRowUpdating="testGrid_RowUpdating"
                                    OnRowDeleting="testGrid_RowDeleting"
                                    OnRowInserting="testGrid_RowInserting"           
                                    EnableRowsCache="false">
                                        <SettingsBehavior AllowSort="false" AllowGroup="false" AllowFocusedRow="True" AutoExpandAllGroups="true" 
                                        EnableRowHotTrack="True" ColumnResizeMode="Control" />
		                            <SettingsPager AlwaysShowPager="true" />
		                            <Styles>
			                            <Row Cursor="pointer" />
		                            </Styles>
                                    </dx:ASPxGridView>
									</dx:LayoutItemNestedControlContainer>
												</LayoutItemNestedControlCollection>
											</dx:LayoutItem>
											<dx:LayoutItem Caption="">
												<LayoutItemNestedControlCollection>
													<dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxButton ID="btOK" runat="server" Text="OK" Width="80px" AutoPostBack="False" style="float: left; margin-right: 8px">
                                        </dx:ASPxButton>
                                        <dx:ASPxButton ID="btCancel" runat="server" Text="Cancel" Width="80px" AutoPostBack="False" style="float: left; margin-right: 8px">
                                        </dx:ASPxButton>
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
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <ContentStyle>
            <Paddings PaddingBottom="5px" />
        </ContentStyle>
    </dx:ASPxPopupControl>
</div>
    <asp:SqlDataSource ID="dsCompany" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasCompany">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsPrimary" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select ID,Name,dbo.fnc_gettype(usertype)usertype,LBOh from MasPrimary where dbo.fnc_checktype(@userType,usertype)>0"
        UpdateCommand="update MasPrimary set Name=@Name,LBOh=@LBOh,usertype=dbo.fnc_settype(@usertype) where ID=@ID"
         InsertCommand="insert into MasPrimary values(@Name,@LBOh,dbo.fnc_settype(@usertype))"
        DeleteCommand="Delete MasPrimary Where ID=@ID">
        <SelectParameters>
            <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="LBOh" Type="String" />
            <asp:Parameter Name="usertype" Type="String" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="LBOh" Type="String" />
            <asp:Parameter Name="usertype" Type="String" />
        </InsertParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsSubType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('Raw Material & Ingredient;Primary Packaging;Secondary Packaging',';')">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsUnit" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select value from dbo.FNC_SPLIT('KG;G;Ton;MT;Case;gram;Can',';')">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="ds" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"/>
    <asp:SqlDataSource ID="dsMenu" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasMenu" UpdateCommand="update MasMenu set [Text]=@Text Where ID=@ID" 
        DeleteCommand="Delete MasMenu where ID=@ID">
        <UpdateParameters>
            <asp:Parameter Name="Text" Type="String" />
        </UpdateParameters>
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
        <asp:SqlDataSource ID="dsStdPricePolicy" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            DeleteCommand="Delete StdPricePolicy Where ID=@ID" 
            InsertCommand="insert into StdPricePolicy values(@FishGroup,@Description,@Currency,@Unit,
             @Jan,@Feb,@Mar,@Apr,@may,@jun,@jul,@aug,@sep,@oct,@nov,@dec,@From,@To,@FishCert,@SHD,0)" 
            SelectCommand="select * from StdPricePolicy" 
            UpdateCommand="UPDATE StdPricePolicy SET [Jan] = @Jan,[Feb]=@Feb
	              ,[Mar]=@Mar
                  ,[Apr]=@Apr
                  ,[May]=@may
                  ,[Jun]=@jun
                  ,[Jul]=@jul
                  ,[Aug]=@aug
                  ,[Sep]=@sep
                  ,[Oct]=@oct
                  ,[Nov]=@nov
                  ,[Dec]=@dec
	              ,[Description]=@Description,Currency=@Currency,Unit=@Unit
                  ,[From]=@From,[To]=@To,FishCert=@FishCert,[SHD]=@SHD,[IsActive]=@IsActive WHERE ID=@ID">
            <UpdateParameters>
              <asp:Parameter Name="FishCert" Type="String" />
              <asp:Parameter Name="SHD" Type="String" />
              <asp:Parameter Name="FishGroup" Type="String" />
              <asp:Parameter Name="Description" Type="String" />
              <asp:Parameter Name="Jan" Type="String" />
              <asp:Parameter Name="Feb" Type="String" />
              <asp:Parameter Name="Mar" Type="String" />
              <asp:Parameter Name="Apr" Type="String" />
              <asp:Parameter Name="May" Type="String" />
              <asp:Parameter Name="Jun" Type="String" />
              <asp:Parameter Name="Jul" Type="String" />
              <asp:Parameter Name="Aug" Type="String" />
              <asp:Parameter Name="Sep" Type="String" />
              <asp:Parameter Name="Oct" Type="String" />
              <asp:Parameter Name="Nov" Type="String" />
              <asp:Parameter Name="Dec" Type="String" />
              <asp:Parameter Name="Currency" Type="String" />
              <asp:Parameter Name="Unit" Type="String" />
              <asp:Parameter Name="From" Type="DateTime" />
              <asp:Parameter Name="To" Type="DateTime" />
              <asp:Parameter Name="IsActive" Type="Boolean" />
            </UpdateParameters>
            <InsertParameters>
              <asp:Parameter Name="FishCert" Type="String" />
              <asp:Parameter Name="SHD" Type="String" />
              <asp:Parameter Name="FishGroup" Type="String" />
              <asp:Parameter Name="Description" Type="String" />
              <asp:Parameter Name="Jan" Type="String" />
              <asp:Parameter Name="Feb" Type="String" />
              <asp:Parameter Name="Mar" Type="String" />
              <asp:Parameter Name="Apr" Type="String" />
              <asp:Parameter Name="May" Type="String" />
              <asp:Parameter Name="Jun" Type="String" />
              <asp:Parameter Name="Jul" Type="String" />
              <asp:Parameter Name="Aug" Type="String" />
              <asp:Parameter Name="Sep" Type="String" />
              <asp:Parameter Name="Oct" Type="String" />
              <asp:Parameter Name="Nov" Type="String" />
              <asp:Parameter Name="Dec" Type="String" />
              <asp:Parameter Name="Currency" Type="String" />
              <asp:Parameter Name="Unit" Type="String" />
              <asp:Parameter Name="From" Type="DateTime" />
              <asp:Parameter Name="To" Type="DateTime" />
              <asp:Parameter Name="IsActive" Type="Boolean" />
            </InsertParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsMasPricePolicy" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            DeleteCommand="Delete MasPricePolicy Where ID=@ID" InsertCommand="insert into MasPricePolicy values(@Material,@Description,
             @Jan,@Feb,@Mar,@Apr,@may,@jun,@jul,@aug,@sep,@oct,@nov,@dec,@Currency,@Unit,@From,@To,0,@Company,@BU)" 
            SelectCommand="select * from MasPricePolicy" 
            UpdateCommand="UPDATE MasPricePolicy SET [Jan] = @Jan,[Feb]=@Feb
	              ,[Mar]=@Mar
                  ,[Apr]=@Apr
                  ,[May]=@may
                  ,[Jun]=@jun
                  ,[Jul]=@jul
                  ,[Aug]=@aug
                  ,[Sep]=@sep
                  ,[Oct]=@oct
                  ,[Nov]=@nov
                  ,[Dec]=@dec
	              ,[From]=@From,[To]=@To,[Description]=@Description,Material=@Material,Currency=@Currency,Unit=@Unit,IsActive=@IsActive WHERE ID=@Id">
            <UpdateParameters>
              <asp:Parameter Name="Material" Type="String" />
              <asp:Parameter Name="Description" Type="String" />
              <asp:Parameter Name="Jan" Type="String" />
              <asp:Parameter Name="Feb" Type="String" />
              <asp:Parameter Name="Mar" Type="String" />
              <asp:Parameter Name="Apr" Type="String" />
              <asp:Parameter Name="May" Type="String" />
              <asp:Parameter Name="Jun" Type="String" />
              <asp:Parameter Name="Jul" Type="String" />
              <asp:Parameter Name="Aug" Type="String" />
              <asp:Parameter Name="Sep" Type="String" />
              <asp:Parameter Name="Oct" Type="String" />
              <asp:Parameter Name="Nov" Type="String" />
              <asp:Parameter Name="Dec" Type="String" />
              <asp:Parameter Name="Currency" Type="String" />
              <asp:Parameter Name="Unit" Type="String" />
              <asp:Parameter Name="From" Type="DateTime" />
              <asp:Parameter Name="To" Type="DateTime" />
              <asp:Parameter Name="IsActive" Type="Boolean" />
            </UpdateParameters>
            <InsertParameters>
              <asp:Parameter Name="Material" Type="String" />
              <asp:Parameter Name="Description" Type="String" />
              <asp:Parameter Name="Jan" Type="String" />
              <asp:Parameter Name="Feb" Type="String" />
              <asp:Parameter Name="Mar" Type="String" />
              <asp:Parameter Name="Apr" Type="String" />
              <asp:Parameter Name="May" Type="String" />
              <asp:Parameter Name="Jun" Type="String" />
              <asp:Parameter Name="Jul" Type="String" />
              <asp:Parameter Name="Aug" Type="String" />
              <asp:Parameter Name="Sep" Type="String" />
              <asp:Parameter Name="Oct" Type="String" />
              <asp:Parameter Name="Nov" Type="String" />
              <asp:Parameter Name="Dec" Type="String" />
              <asp:Parameter Name="Currency" Type="String" />
              <asp:Parameter Name="Unit" Type="String" />
              <asp:Parameter Name="From" Type="DateTime" />
              <asp:Parameter Name="To" Type="DateTime" />
              <asp:Parameter Name="IsActive" Type="Boolean" />
              <asp:Parameter Name="Company" Type="String" />
              <asp:Parameter Name="BU" Type="String" />
            </InsertParameters>
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
            SelectCommand="spGetulogin" SelectCommandType="StoredProcedure"
            DeleteCommand="Delete ulogin where au_id=@au_id"
            InsertCommand="spUpdateulogin" InsertCommandType="StoredProcedure"
            UpdateCommand="spInsertulogin" UpdateCommandType="StoredProcedure">
            <SelectParameters>
                 <asp:ControlParameter ControlID="hBu" Name="BU" PropertyName="['BU']"/>
                 <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
            </SelectParameters>
            <InsertParameters>
                <asp:Parameter Name="au_id" Type="String" />
                <asp:Parameter Name="Position" Type="String" />
                <asp:Parameter Name="Email" Type="String" />
                <asp:Parameter Name="BU" Type="String" />
                <asp:Parameter Name="IsResign" Type="Boolean" />
                <asp:Parameter Name="userlevel" Type="String" />
                <asp:Parameter Name="FirstName" Type="String" />
                <asp:Parameter Name="LastName" Type="String" />
                <asp:Parameter Name="user_name" Type="String" />
                <asp:Parameter Name="usertype" Type="String" />
                <asp:Parameter Name="factory" Type="String" />
                <asp:Parameter Name="emplevel" Type="String" />
                <asp:Parameter Name="RequestType" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="au_id" Type="String" />
                <asp:Parameter Name="Position" Type="String" />
                <asp:Parameter Name="Email" Type="String" />
                <asp:Parameter Name="BU" Type="String" />
                <asp:Parameter Name="IsResign" Type="Boolean" />
                <asp:Parameter Name="userlevel" Type="String" />
                <asp:Parameter Name="FirstName" Type="String" />
                <asp:Parameter Name="LastName" Type="String" />
                <asp:Parameter Name="user_name" Type="String" />
                <asp:Parameter Name="usertype" Type="String" />
                <asp:Parameter Name="factory" Type="String" />
                <asp:Parameter Name="emplevel" Type="String" />
                <asp:Parameter Name="RequestType" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsApprovAssign" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="spGetApprovAssign" SelectCommandType="StoredProcedure"
            InsertCommand="Insert into MasApprovAssign values (@empid,@Sublevel)"
            UpdateCommand="Update MasApprovAssign set Sublevel=@Sublevel Where Id=@Id"
            DeleteCommand="delete MasApprovAssign where Id=@Id">
            <SelectParameters>
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
        <asp:SqlDataSource ID="dsSAPMaterial" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="select * from SAPMaterial">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsPetCategory" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="select * from MasPetCategory where dbo.fnc_checktype(@userType,usertype)>0">
            <SelectParameters>
                <asp:ControlParameter ControlID="usertp" Name="usertype" PropertyName="['usertype']"/>
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsCurrency" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="select value from dbo.FNC_SPLIT('THB;USD;',';')">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsExchangeRat" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="select * from MasExchangeRat where Company in (select distinct convert(nvarchar(3),value) from dbo.FNC_SPLIT(@Bu,'|'))"
            UpdateCommand="spupdateexch" UpdateCommandType="StoredProcedure" 
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
        <asp:SqlDataSource ID="dsusertype" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
                SelectCommand="select * from masusertype">
            </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsReason" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
                SelectCommand="select * from MasReason"
                UpdateCommand="Update MasReason set Description=@Description,Type=@Type where ID=@ID" 
                InsertCommand="Insert into MasReason values (@Description,@Type)"
                DeleteCommand="Delete MasReason where ID=@ID"  >
            <UpdateParameters>
                <asp:Parameter Name="Description" Type="String" />
                <asp:Parameter Name="Type" Type="String" />
            </UpdateParameters>
            <InsertParameters>
                <asp:Parameter Name="Description" Type="String" />
                <asp:Parameter Name="Type" Type="String" />
            </InsertParameters>
            </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsVarietyPack" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
                SelectCommand="spUpdateVarietyPack" SelectCommandType="StoredProcedure"
                UpdateCommand="Update TransCostingHeader set MarketingNumber=@MarketingNumber,RDNumber=@RDNumber,VarietyPack=@VarietyPack Where ID=@ID"
                DeleteCommand="Delete TransCostingHeader where Id=@ID">
        <SelectParameters>
            <asp:SessionParameter Name="user" SessionField="user_name" Type="String" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="MarketingNumber" Type="String" />
            <asp:Parameter Name="ID" Type="Int32" />
            <asp:Parameter Name="VarietyPack" Type="String" />
            <asp:Parameter Name="RDNumber" Type="String" />
        </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsFormulaByCode" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
                SelectCommand="spUpdateFormulaByCode2" SelectCommandType="StoredProcedure"
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
        <asp:SqlDataSource ID="dsMaster" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="spGetMenuAuth" SelectCommandType="StoredProcedure">
            <SelectParameters>
            <asp:SessionParameter Name="username" SessionField="user_name" Type="String" />
            <asp:SessionParameter Name="viewMode" SessionField="viewMode" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsPlant" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="spGetFactory2" SelectCommandType="StoredProcedure">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsWorkFlowapp" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="spGetWorkFlowapp" SelectCommandType="StoredProcedure" 
            UpdateCommand="spUpdateWorkFlow" UpdateCommandType="StoredProcedure">
            <UpdateParameters>
                <asp:Parameter Name="ID" />
                <asp:Parameter Name="Sublevel" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsGroupFlow" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="select * from MasGroupFlow">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsstdTunaSpecialFW" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="select *, Material as ID from stdTunaSpecialFW"
            UpdateCommand="Update TransFormulaHeader set FillWeight=@FillWeight,Unit=@Unit,FishGroup=@FishGroup where Material=@ID"
            DeleteCommand="Delete TransFormulaHeader where Material=@ID"
            InsertCommand="insert into stdTunaSpecialFW values (@Material,@Description,@FillWeight,@Unit,@FishGroup" >
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsStdFillWeightMedia" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="SELECT CONCAT(SAPCodeDigit,'|', Code ,'|', GroupType) AS ID,SAPCodeDigit, Name, GroupType, GroupDescription, Code, CodeName, MediaWeight, Unit from StdFillWeightMedia"
            UpdateCommand="spUpFillWeightMedia" UpdateCommandType="StoredProcedure"
            DeleteCommand="spDelFillWeightMedia" DeleteCommandType="StoredProcedure"
            InsertCommand="insert into StdFillWeightMedia values(@SAPCodeDigit,@Name,@GroupType,@GroupDescription,@Code,@CodeName,@MediaWeight,@Unit)">
            <InsertParameters>
                <asp:Parameter Name="SAPCodeDigit" Type="String" />
                <asp:Parameter Name="MediaWeight" Type="String" />
                <asp:Parameter Name="Unit" Type="String" />
                <asp:Parameter Name="GroupType" Type="String" />
                <asp:Parameter Name="Code" Type="String" />
                <asp:Parameter Name="CodeName" Type="String" />
                <asp:Parameter Name="GroupDescription" Type="String" />
                <asp:Parameter Name="Name" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="MediaWeight" Type="String" />
                <asp:Parameter Name="Unit" Type="String" />
                <asp:Parameter Name="CodeName" Type="String" />
                <asp:Parameter Name="GroupDescription" Type="String" />
                <asp:Parameter Name="Name" Type="String" />
            </UpdateParameters>
            <DeleteParameters>
                <asp:Parameter Name="ID" />
            </DeleteParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsStdSecPKGCost" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="SELECT ID,Material, Customer, ShipTo, Amount, Currency, Unit,[From],[To] from StdSecPKGCost"
            UpdateCommand="Update StdSecPKGCost set Amount=@Amount,Currency=@Currency,Unit=@Unit,Material=@Material,Customer=@Customer,ShipTo=@ShipTo,[from]=@from,[to]=@to where ID=@ID" 
            DeleteCommand="Delete StdSecPKGCost where ID=@ID" DeleteCommandType="StoredProcedure"
            InsertCommand="insert into StdSecPKGCost values(@Material,@Customer,@ShipTo,@Amount,@Currency,@Unit,@from,@to)">
            <InsertParameters>
                <asp:Parameter Name="Material" Type="String" />
                <asp:Parameter Name="Customer" Type="String" />
                <asp:Parameter Name="ShipTo" Type="String" />
                <asp:Parameter Name="Amount" Type="String" />
                <asp:Parameter Name="Currency" Type="String" />
                <asp:Parameter Name="Unit" Type="String" />
                <asp:Parameter Name="From" Type="DateTime" />
                <asp:Parameter Name="To" Type="DateTime" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="ShipTo" Type="String" />
                <asp:Parameter Name="Material" Type="String" />
                <asp:Parameter Name="Customer" Type="String" />
                <asp:Parameter Name="Amount" Type="String" />
                <asp:Parameter Name="Currency" Type="String" />
                <asp:Parameter Name="Unit" Type="String" />
                <asp:Parameter Name="From" Type="DateTime" />
                <asp:Parameter Name="To" Type="DateTime" />
            </UpdateParameters>
            <DeleteParameters>
                <asp:Parameter Name="ID" />
            </DeleteParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dszone" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="select distinct Zone as 'value' from MasCustomer order by Zone">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="dsRequestType" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
            SelectCommand="select Name from MasRequestType group by name">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="lab" runat="server" ConnectionString="<%$ ConnectionStrings:LabConnectionString %>"/>