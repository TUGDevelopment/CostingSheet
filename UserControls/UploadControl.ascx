<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UploadControl.ascx.cs" Inherits="UserControls_UploadControl" %>
<script type="text/javascript">
    function onFileUploadComplete(s, e) {
        if (e.callbackData) {
            debugger;
            var fileData = e.callbackData.split('|');
             ClientSpreadsheet.PerformCallback(CmbCompany.GetValue() + "|" +fileData);
        }
    }
    function OnInit(s, e) {
        //debugger;
        s.SetFullscreenMode(false);
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
        ClientSpreadsheet.SetHeight(height - 220);
    }
</script>
<dx:ASPxHiddenField ID="editor" runat="server" ClientInstanceName="editor" />
<dx:ASPxHiddenField ID="hfgetvalue" runat="server" ClientInstanceName="ClientNewID" />
<dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div"  ScrollBars="Vertical" ClientInstanceName="ClientFormPanel" >
    <PanelCollection>
        <dx:PanelContent>  
        <dx:ASPxFormLayout ID="ContactForm" runat="server" AlignItemCaptionsInAllGroups="True" UseDefaultPaddings="false">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
	        <Items>
		        <dx:LayoutGroup Caption="" GroupBoxDecoration="None" ColCount="1" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                    <GroupBoxStyle>
                        <Caption Font-Bold="true" Font-Size="10"/>
                    </GroupBoxStyle>
                    <Items>
                    <dx:LayoutItem Caption="att" Paddings-PaddingLeft="5">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxUploadControl ID="UploadControl" runat="server" ClientInstanceName="UploadControl" Width="200"
                                        NullText="Select multiple files..." UploadMode="Advanced" ShowUploadButton="false" ShowProgressPanel="True"
                                        OnFileUploadComplete="UploadControl_FileUploadComplete">
                                        <AdvancedModeSettings EnableMultiSelect="True" EnableFileList="True" EnableDragAndDrop="True" />
                                        <ValidationSettings MaxFileSize="4194304" AllowedFileExtensions=".jpg,.jpeg,.gif,.png,.xls,.xlsx,.pdf">
                                        </ValidationSettings>
                                    <ClientSideEvents FileUploadComplete="onFileUploadComplete" />
                                </dx:ASPxUploadControl>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="" Paddings-PaddingLeft="5">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxRadioButtonList ID="rbCostType" runat="server" ClientInstanceName="rbCostType">
                                    <Items>
                                        <dx:ListEditItem Text="Single" Value="0" Selected="true"/>
                                        <dx:ListEditItem Text="VarietyPack" Value="1" />
                                    </Items>
                                </dx:ASPxRadioButtonList>
                                 </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Company" Paddings-PaddingLeft="5">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <table>
                                    <tr>
                                        <td>
                                        <dx:ASPxComboBox ID="CmbCompany" runat="server" DataSourceID="dsCompany" ValueField="Code" Width="200px" ClientInstanceName="CmbCompany"
                                                                    TextFormatString="{0}">
                                                                    <Columns>
                                                                        <dx:ListBoxColumn FieldName="Code"/>
                                                                        <dx:ListBoxColumn FieldName="Name"/>
                                                                    </Columns>
                                                                </dx:ASPxComboBox>
                                        </td>
                                        <td>
                                            <dx:ASPxButton ID="btnImageAndText" runat="server" ClientVisible="false"
                                            Width="90px" Text="Cancel" AutoPostBack="false">
                                            <Image  Url="~/Content/Images/icons8-upload-16.jpg"></Image> 
                                        </dx:ASPxButton>
                                            <%--<dx:ASPxButton ID="btnBlueBall" runat="server" AutoPostBack="False" AllowFocus="False" RenderMode="Link" OnClick="btnBlueBall_Click" EnableTheming="False">
                                            <Image>
                                                <SpriteProperties CssClass="blueBall" HottrackedCssClass="blueBallHottracked" PressedCssClass="blueBallPressed" />
                                            </Image> 
                                        </dx:ASPxButton>--%>
                                        </td>
                                    </tr>
                                </table>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    <dx:LayoutItem Caption="" CaptionSettings-Location="Top" Paddings-PaddingLeft="5">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                            <div id="gridContainer" style="visibility: hidden">
                            <dx:ASPxSpreadsheet ID="Spreadsheet" runat="server" Width="100%" ActiveTabIndex="0" ShowConfirmOnLosingChanges="false" OnCallback="Spreadsheet_Callback" 
                                    ClientInstanceName="ClientSpreadsheet">
                                 <ClientSideEvents Init="OnInit" EndCallback="OnEndCallback" />
                            </dx:ASPxSpreadsheet></div>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                <%--<dx:LayoutItem>
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer>
                        <dx:ASPxGridView ID="testgrid" runat="server" OnDataBinding="testgrid_DataBinding" OnCustomCallback="testgrid_CustomCallback" ClientInstanceName="testgrid">
                            <SettingsPager PageSize="60" />
                        </dx:ASPxGridView>
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
    <asp:SqlDataSource ID="dsCompany" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasCompany">
    </asp:SqlDataSource>
