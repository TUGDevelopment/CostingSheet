<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GetExcelFiles.aspx.cs" Inherits="UpdatePanel_Auto_Refresh_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="Css/Appearance.css" />
</head>
<body>
    <form id="form1" runat="server">
    <dx:ASPxHiddenField ID="hfgetvalue" runat="server" ClientInstanceName="ClientNewID" />
    <table>
        <tr>
            <td>Company:</td>
            <td>
            <dx:ASPxComboBox ID="CmbCompany" runat="server" DataSourceID="dsCompany" ValueField="Code" Width="200px"
                                        TextFormatString="{0}">
                                        <Columns>
                                            <dx:ListBoxColumn FieldName="Code"/>
                                            <dx:ListBoxColumn FieldName="Name"/>
                                        </Columns>
                                    </dx:ASPxComboBox>
            </td>
            <td colspan="2">
                <dx:ASPxButton ID="btnImageAndText" runat="server" ClientVisible="false"
                Width="90px" Text="Cancel" AutoPostBack="false">
                <Image  Url="../Content/Images/icons8-upload-16.jpg"></Image> 
            </dx:ASPxButton>
                <dx:ASPxButton ID="btnBlueBall" runat="server" AutoPostBack="False" AllowFocus="False" RenderMode="Link" OnClick="btSubmit_Click" EnableTheming="False">
                <Image>
                    <SpriteProperties CssClass="blueBall" HottrackedCssClass="blueBallHottracked" PressedCssClass="blueBallPressed" />
                </Image> 
            </dx:ASPxButton>
            </td>
        </tr>
    </table><br />
    <dx:ASPxSpreadsheet ID="Spreadsheet" runat="server" Width="100%" Height="340px" ActiveTabIndex="0" ShowConfirmOnLosingChanges="false" 
            ClientInstanceName="ClientSpreadsheet">
         <ClientSideEvents Init="function(s, e){ s.SetFullscreenMode(false); }" />
    </dx:ASPxSpreadsheet><br />
    <dx:ASPxGridView ID="testgrid" runat="server">
        <SettingsPager PageSize="60" />
    </dx:ASPxGridView>
    <asp:SqlDataSource ID="dsCompany" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from MasCompany">
    </asp:SqlDataSource>
    </form>
</body>
</html>
