<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccessControl.ascx.cs" Inherits="UserControls_adminControls_AccessControl" %>
<%--<dx:ASPxButton ID="closeButton" runat="server" AutoPostBack="false" Text="Close">
                                                    <ClientSideEvents Click="function(s,e) { ClientFormPanel.SetVisible(true); }" />
                                                </dx:ASPxButton>--%>
    <dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" ClientVisible="false" ScrollBars="Vertical" ClientInstanceName="ClientFormPanel">
        <ClientSideEvents Init="Demo.Init" />
        <PanelCollection>
            <dx:PanelContent>  
    <dx:ASPxFormLayout runat="server" ID="formLayout" ClientInstanceName="formLayout" CssClass="formLayout">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
            <Items>
                <dx:LayoutGroup Caption="Marketing PF Technical Request Form" ColCount="3" GroupBoxDecoration="HeadingLine" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                    <GroupBoxStyle>
                        <Caption Font-Bold="true" Font-Size="10"/>
                    </GroupBoxStyle>
                    <Items>
                        <dx:LayoutItem Caption="Request No">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
    <dx:ASPxFileManager ID="FileManager" runat="server" DataSourceID="ArtsDataSource" Height="240px">
        <Settings ThumbnailFolder="~/Content/FileManager" InitialFolder="Salvador Dali\1936 - 1945" 
            AllowedFileExtensions=".jpg,.jpeg,.gif,.rtf,.txt,.avi,.png,.mp3,.xml,.doc,.pdf" />
        <SettingsDataSource KeyFieldName="ID" ParentKeyFieldName="ParentID" NameFieldName="Name" IsFolderFieldName="IsFolder" FileBinaryContentFieldName="Data" LastWriteTimeFieldName="LastWriteTime" />
        <SettingsEditing AllowCreate="true" AllowDelete="true" AllowMove="true" AllowRename="true" />
        <SettingsPermissions>
            <AccessRules>
                <dx:FileManagerFolderAccessRule Path="" Edit="Deny" />
                <dx:FileManagerFileAccessRule Path="*.xml" Edit="Deny" />
                <dx:FileManagerFolderAccessRule Path="System" Browse="Deny" />
                
                <dx:FileManagerFolderAccessRule Path="Documents" Role="DocumentManager" EditContents="Allow" />
                
                <dx:FileManagerFolderAccessRule Path="" Role="MediaModerator" Upload="Deny" />
                <dx:FileManagerFolderAccessRule Path="Music" Role="MediaModerator" EditContents="Allow" Upload="Allow" />
                <dx:FileManagerFolderAccessRule Path="Video" Role="MediaModerator" EditContents="Allow" Upload="Allow" />
        
                <dx:FileManagerFolderAccessRule Role="Administrator" Edit="Allow" Browse="Allow" />
            </AccessRules>
        </SettingsPermissions>
    </dx:ASPxFileManager>
    <br />
    <p class="Note">
        <strong>Allowed Extensions</strong>: .jpg, .jpeg, .gif, .rtf, .txt, .avi, .png, .mp3, .xml, .doc, .pdf
    </p>

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
<asp:SqlDataSource ID="ArtsDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spGetFileSystem" SelectCommandType="StoredProcedure" CancelSelectOnNullParameter="False"
        DeleteCommand="DELETE FROM FileSystem WHERE ID = @Id" OnInserting="ArtsDataSource_Inserting" OnUpdating="ArtsDataSource_Updating"
        InsertCommand="INSERT INTO FileSystem (Name, IsFolder, ParentId, Data, LastWriteTime, GCRecord, LastUpdateBy) VALUES (@Name, @IsFolder, @ParentID, @Data, @LastWriteTime, @GCRecord, @LastUpdateBy)"
        UpdateCommand="UPDATE FileSystem SET Name = @Name, IsFolder = @IsFolder, ParentID = @ParentID, Data = @Data, LastWriteTime = @LastWriteTime, LastUpdateBy = @LastUpdateBy WHERE (Id = @Id)">
        <InsertParameters>
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="IsFolder" Type="Boolean" />
            <asp:Parameter Name="ParentID" Type="Int32" />
            <asp:Parameter Name="Data" DbType="Binary"/>
            <asp:Parameter Name="LastWriteTime" Type="DateTime" />
            <asp:Parameter Name="GCRecord" Type="String" />
            <asp:Parameter Name="LastUpdateBy" Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="Name" Type="String" />
            <asp:Parameter Name="IsFolder" Type="Boolean" />
            <asp:Parameter Name="ParentID" Type="Int32" />
            <asp:Parameter Name="Data" DbType="Binary" />
            <asp:Parameter Name="LastWriteTime" Type="DateTime" />
            <asp:Parameter Name="LastUpdateBy" Type="String"/>
            <asp:Parameter Name="Id" Type="Int32" />
        </UpdateParameters>
        <DeleteParameters>
            <asp:Parameter Name="Id" Type="Int32" />
        </DeleteParameters>
        <SelectParameters>
           <asp:Parameter Name="GCRecord" DefaultValue="3C985E3B-F003-4596-B1BC-0CAB4050324F"  />
           <asp:Parameter Name="username" DefaultValue="fo5910155" />
        </SelectParameters>
</asp:SqlDataSource>