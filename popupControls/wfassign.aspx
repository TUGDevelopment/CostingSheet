<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wfassign.aspx.cs" Inherits="UserControls_wfassign" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
    function OnGetSelectionButtonClick(s, e) {
    var g = glAssignee.GetGridView();
        g.GetSelectedFieldValues('user_name', OnGetSelectedFieldValues);
        //alert(glAssignee.GetValue());
        //if (ASPxClientEdit.ValidateGroup('entryGroup'))
        //    pcassign.Hide();
    }
    function OnGetSelectedFieldValues(selectedValues) {
        if (selectedValues.length == 0) return;
        s = "";
        for (i = 0; i < selectedValues.length; i++) {
            for (j = 0; j < selectedValues[i].length; j++) {
                s = s + selectedValues[i][j];
            }
            s += "\n";
        }
       //alert("Selected Products:\n" + s);
       // ClientgridData.PerformCallback('ValueChanged');
        load(s);
        }
        function load(result) {
            //alert('document is ready.');
            var myParam = location.search.split('id=')[1]
            window.parent.HidePopupAndShowInfo('Client',myParam +'|'+ result);
        }
        function OnFileUploaded(s, e) {
            console.log("e.fileName=" + e.fileName);
            console.log("e.folder=" + e.folder);
        }
      
        var postponedCallbackRequired = false;
        function OnSelectedFileChanged(s, e) {
            debugger;
            
            if (e.file) {
                alert(e.file);
                //if(!spreadsheet.InCallback())
                //    spreadsheet.PerformCallback();
                //else
                //    postponedCallbackRequired = true;
            }
        }
        function SetCurrentFolder() {
            fileManager.SetCurrentFolderPath('');
        }
        function SelectFirstFile() {
            fileManager.GetItems()[0].SetSelected('true');
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxFormLayout runat="server" ID="formLayout" Width="100%" Height="100%">
                <Items>
<%--                    <dx:LayoutItem Caption="Username" Name="Assignee">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxGridLookup ID="ASPxGridLookup1" runat="server" ClientInstanceName="glAssignee" DataSourceID="dsAssignee" SelectionMode="Multiple"
                                        Width="240px" KeyFieldName="user_name" TextFormatString="{0}" MultiTextSeparator=", " OnDataBound="ASPxGridLookup1_DataBound">
                                        <Columns>
                                        <dx:GridViewCommandColumn ShowSelectCheckbox="True" />
                                        <dx:GridViewDataColumn FieldName="fn" Caption="FullName" />
                                    </Columns>
                                    <GridViewProperties>
                                        <Templates>
                                            <StatusBar>
                                                <table class="OptionsTable" style="float: right">
                                                    <tr>
                                                        <td>
                                                            <dx:ASPxButton ID="btOK" runat="server" Text="OK" Width="80px" AutoPostBack="False" Style="float: left; margin-right: 8px">
                                                                <ClientSideEvents Click="function(s, e) {OnGetSelectionButtonClick(s, e); }" />
                                                            </dx:ASPxButton>
                                                            <dx:ASPxButton ID="btCancel" runat="server" Text="Cancel" Width="80px" AutoPostBack="False" Style="float: left; margin-right: 8px">
                                                                <ClientSideEvents Click="function(s, e) { window.parent.CloseHidePopup(); }" />
                                                            </dx:ASPxButton>
                                                            <dx:ASPxButton ID="Close" runat="server" AutoPostBack="false" Text="Close" ClientSideEvents-Click="CloseGridLookup" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </StatusBar>
                                        </Templates>
                                        <Settings ShowFilterRow="true" ShowStatusBar="Visible"/>
                                        <SettingsPager PageSize="7" EnableAdaptivity="true" />
                                    </GridViewProperties>
                                </dx:ASPxGridLookup>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
 
                    <dx:LayoutItem ShowCaption="False" Paddings-PaddingTop="19" Name="btnEvent">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxButton ID="btOK" runat="server" Text="OK" Width="80px" AutoPostBack="False" Style="float: left; margin-right: 8px">
                                    <ClientSideEvents Click="function(s, e) {OnGetSelectionButtonClick(s, e); }" />
                                </dx:ASPxButton>
                                <dx:ASPxButton ID="btCancel" runat="server" Text="Cancel" Width="80px" AutoPostBack="False" Style="float: left; margin-right: 8px">
                                    <ClientSideEvents Click="function(s, e) { window.parent.CloseHidePopup(); }" />
                                </dx:ASPxButton>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>--%>
                    <%--<dx:LayoutItem Caption="Type" CaptionSettings-Location="Left">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxCallbackPanel ID="cpCustomerRole" runat="server" Width="100%" ClientInstanceName="cpCustomerRole" OnCallback="cpCustomerRole_Callback" >
                                  <PanelCollection>
                                  <dx:PanelContent runat="server">
                                    <dx:ASPxCheckBoxList ID="cbRequest" runat="server" ValueField="idx" TextField="value" RepeatDirection="Horizontal" DataSourceID="dsRequest"  
                                           ClientInstanceName="ClientRequest" RepeatColumns="10" RepeatLayout="Table" Border-BorderWidth="0"/>
                                      </dx:PanelContent>
                                      </PanelCollection>
                                    </dx:ASPxCallbackPanel>
                                      </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>--%>
                    <dx:LayoutItem Caption="" Name="Attachment" CaptionSettings-Location="Top">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxFileManager ID="fileManager" runat="server" DataSourceID="ArtsDataSource" OnCustomCallback="fileManager_CustomCallback"  
                                     Height="240px" OnItemDeleting="fileManager_ItemDeleting" 
                                     OnFileUploading="fileManager_FileUploading"
                                      ClientInstanceName="fileManager">
                                        <ClientSideEvents FileUploaded="OnFileUploaded" SelectedFileChanged="OnSelectedFileChanged" 
                                            FilesUploading="SetCurrentFolder"/>
                                        <Settings ThumbnailFolder="~/Content/FileManager" InitialFolder="Salvador Dali\1936 - 1945" 
                                            AllowedFileExtensions=".doc, .docx,.jpg,.jpeg,.gif,.png,.xls,.xlsx,.pdf"/>
                                        <SettingsFileList View="Thumbnails">
                                            <ThumbnailsViewSettings ThumbnailWidth="100" ThumbnailHeight="100" />
                                        </SettingsFileList>
                                        <SettingsToolbar ShowPath="false" ShowRefreshButton="false" />
                                        <SettingsDataSource KeyFieldName="ID" ParentKeyFieldName="ParentID" NameFieldName="Name" IsFolderFieldName="IsFolder" 
                                            FileBinaryContentFieldName="Data" LastWriteTimeFieldName="LastWriteTime" />
                                        <SettingsEditing AllowCreate="false" AllowDelete="true" AllowMove="false" AllowRename="false" AllowDownload="true" />
                                        <SettingsBreadcrumbs Visible="true" ShowParentFolderButton="true" Position="Top" />
                                        <SettingsPermissions>
                                            <AccessRules>
                                                <dx:FileManagerFolderAccessRule Role="Administrator" Edit="Allow" Browse="Allow" />
                                            </AccessRules>
                                        </SettingsPermissions>
                                        <SettingsUpload UseAdvancedUploadMode="true" Enabled="true">
                                            <AdvancedModeSettings EnableMultiSelect="true"  />
                                            <ValidationSettings 
                                                MaxFileSize="10000000" 
                                                MaxFileSizeErrorText="The file you are trying to upload is larger than what is allowed (10 MB).">
                                            </ValidationSettings>
                                        </SettingsUpload>
					                <Settings EnableMultiSelect="true" />
                                    </dx:ASPxFileManager>
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
            </dx:ASPxFormLayout>
            
            <%--<dx:ASPxGridView ID="ASPxGridView1" ClientInstanceName="grid" runat="server" DataSourceID="dsAssignee" KeyFieldName="user_name" Width="100%">
            <Columns>
                <dx:GridViewCommandColumn ShowSelectCheckbox="true" />
                <dx:GridViewDataColumn FieldName="user_name" />
                <dx:GridViewDataColumn FieldName="fn" />
            </Columns>
            <SettingsSearchPanel Visible="true" />
        </dx:ASPxGridView>--%>
        </div>
    </form>
<%--    <asp:SqlDataSource ID="dsAssignee" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spAssignee" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="usertype" SessionField="usertype" Type="String" />
            <asp:SessionParameter Name="BU" SessionField="BU" Type="String" />
            <asp:SessionParameter Name="ID" SessionField="NewID" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>--%>
    <asp:SqlDataSource ID="ArtsDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetFileSystem" SelectCommandType="StoredProcedure" CancelSelectOnNullParameter="False"
        InsertCommand="spinsertFileSystem" InsertCommandType="StoredProcedure"
        UpdateCommand="spUpdateFileSystem" UpdateCommandType="StoredProcedure">
        <InsertParameters>
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="IsFolder" Type="Boolean" />
            <asp:Parameter Name="ParentID" Type="Int32" />
            <asp:Parameter Name="Data" DbType="Binary"/>
            <asp:Parameter Name="LastWriteTime" Type="DateTime" />
            <asp:SessionParameter Name="GCRecord" SessionField="NewID" Type="String" />
            <asp:SessionParameter Name="LastUpdateBy" SessionField="username" Type="String" />
            <asp:SessionParameter Name="table" SessionField="tablename"/>
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="IsFolder" Type="Boolean" />
            <asp:Parameter Name="ParentID" Type="Int32" />
            <asp:Parameter Name="Data" DbType="Binary" />
            <asp:Parameter Name="LastWriteTime" Type="DateTime" />
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
        <SelectParameters>
           <asp:SessionParameter Name="GCRecord" SessionField="NewID" Type="String" />
           <asp:SessionParameter Name="username" SessionField="username"/>
           <asp:SessionParameter Name="tablename" SessionField="tablename"/>
        </SelectParameters>
    </asp:SqlDataSource>
     <asp:SqlDataSource ID="dsRequest" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="select * from dbo.FNC_SPLIT('Upload Spce,Formula',',')" />
</body>
</html>
