<%@ Page Language="C#" AutoEventWireup="true" CodeFile="popupsel.aspx.cs" Inherits="UserControls_popupsel" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body, html {
            padding: 0;
            margin: 0;
        }
    </style>
    <script type="text/javascript">
        window.onload = function () {
            //alert("Got Called!!");
            cp.PerformCallback('reload');
            //var UserType = Demo.getUrlVars()["UserType"];
        }
    function getUrlVars() {
        var vars = {};
        var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
            vars[key] = value;
        });
        return vars;
    }
        function OnItemDoubleClick(s, e) {
            debugger;
            var key = getUrlVars()["view"];
            var result = s.GetSelectedItem().text;
            window.parent.HidePopupAndShowInfo(key,result);

        } 
        function OnInit(s, e) {
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
            ClientlistBox.SetHeight(height);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="gridContainer" style="visibility: hidden">
        <dx:ASPxCallbackPanel ID="cp" runat="server" Width="100%" ClientInstanceName="cp" 
                    OnCallback="cp_Callback" >
                    <PanelCollection>
                    <dx:PanelContent runat="server">
                <dx:ASPxListBox runat="server" ID="listBox" Width="100%" 
                        OnDataBinding="listBox_DataBinding" ClientInstanceName="ClientlistBox"  
                        TextField="Name" ValueField="ID"
                        CallbackPageSize="15">
                    <FilteringSettings ShowSearchUI="true" />
                    <ClientSideEvents ItemDoubleClick="OnItemDoubleClick" Init="OnInit" EndCallback="OnEndCallback" />
                </dx:ASPxListBox>
                </dx:PanelContent>
                </PanelCollection>
                </dx:ASPxCallbackPanel>
    </div>
    </form>
</body>
</html>
