<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JavaScript.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body
        {
            font-family: Arial;
            font-size: 10pt;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server">
</asp:ScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
    <asp:Label ID="lblTime" runat="server" />
    <asp:Button ID="btnSubmit" runat="server" OnClick="GetTime" style = "display:none"/>
</ContentTemplate>
</asp:UpdatePanel>
<script type="text/javascript">
window.onload = function () {
    setInterval(function () {
        document.getElementById("<%=btnSubmit.ClientID %>").click();
    }, 1000);
};
</script>
    </form>
</body>
</html>
