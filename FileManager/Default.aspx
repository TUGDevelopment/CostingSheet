<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <style type="text/css">
        .dxfm-uploadPanelTableBCell a {
        display:none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxFileManager ID="ASPxFileManager1" runat="server">
                <Settings RootFolder="~\Content\FileManager" ThumbnailFolder="~\Thumb\" />
                <SettingsEditing AllowDelete="true" AllowRename="true" AllowCreate="true" AllowMove="true" />
            </dx:ASPxFileManager>
        </div>
    </form>
</body>
</html>
