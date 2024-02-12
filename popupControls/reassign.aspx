<%@ Page Language="C#" AutoEventWireup="true" CodeFile="reassign.aspx.cs" Inherits="UserControls_reassign" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        .container {
            display: table;
        }
        .contentButtons {
            padding-top:20px; 
            padding-bottom:10px;
        }
        .button {
                width:100% !important;
        }
        @media(min-width:790px) {
            .contentEditors, .contentButtons {
                display: table-cell;
                width: 33.33333333%;
            }
            .button {
                width:170px !important;
            }
            .contentEditors {
                vertical-align: top;
            }
            .contentButtons {
                vertical-align: middle;
                text-align: center;
            }
        }
    </style>
    <script type="text/javascript">
        function AddSelectedItems() {
            MoveSelectedItems(lbAvailable, lbChoosen);
            UpdateButtonState();
        }
        function AddAllItems() {
            MoveAllItems(lbAvailable, lbChoosen);
            UpdateButtonState();
        }
        function RemoveSelectedItems() {
            MoveSelectedItems(lbChoosen, lbAvailable);
            UpdateButtonState();
        }
        function RemoveAllItems() {
            MoveAllItems(lbChoosen, lbAvailable);
            UpdateButtonState();
        }
        function MoveSelectedItems(srcListBox, dstListBox) {
            srcListBox.BeginUpdate();
            dstListBox.BeginUpdate();
            var items = srcListBox.GetSelectedItems();
            for (var i = items.length - 1; i >= 0; i = i - 1) {
                dstListBox.AddItem(items[i].text, items[i].value);
                srcListBox.RemoveItem(items[i].index);
            }
            srcListBox.EndUpdate();
            dstListBox.EndUpdate();
            cp.PerformCallback("1");
        }
        function MoveAllItems(srcListBox, dstListBox) {
            srcListBox.BeginUpdate();
            var count = srcListBox.GetItemCount();
            for (var i = 0; i < count; i++) {
                var item = srcListBox.GetItem(i);
                dstListBox.AddItem(item.text, item.value);
            }
            srcListBox.EndUpdate();
            srcListBox.ClearItems();
            cp.PerformCallback("2");
        }
        function UpdateButtonState() {
            btnMoveAllItemsToRight.SetEnabled(lbAvailable.GetItemCount() > 0);
            btnMoveAllItemsToLeft.SetEnabled(lbChoosen.GetItemCount() > 0);
            btnMoveSelectedItemsToRight.SetEnabled(lbAvailable.GetSelectedItems().length > 0);
            btnMoveSelectedItemsToLeft.SetEnabled(lbChoosen.GetSelectedItems().length > 0);

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <dx:ASPxCallbackPanel ID="cp" runat="server" Width="100%" ClientInstanceName="cp" 
                    OnCallback="cp_Callback" />
    <dx:ASPxGlobalEvents ID="GlobalEvents" runat="server">
        <ClientSideEvents ControlsInitialized="function(s, e) { UpdateButtonState(); }" />
    </dx:ASPxGlobalEvents>
    <div class="container horizontal-center-aligned">
        <div class="contentEditors">
            <dx:ASPxListBox ID="lbAvailable" runat="server" ClientInstanceName="lbAvailable" DataSourceID="dsAssignee" ValueField="user_name" TextField="fn"
                Width="285" Height="240px" SelectionMode="CheckColumn" Caption="Available" EnableSynchronization="True">
                <CaptionSettings Position="Top" />
                <ClientSideEvents SelectedIndexChanged="function(s, e) { UpdateButtonState(); }" />
                <FilteringSettings ShowSearchUI="true" />
            </dx:ASPxListBox>
        </div>
        <div class="contentButtons">
            <div>
                <dx:ASPxButton ID="btnMoveSelectedItemsToRight" runat="server" ClientInstanceName="btnMoveSelectedItemsToRight" CssClass="button"
                    AutoPostBack="False" Text="Add >" ClientEnabled="False"
                    ToolTip="Add selected items">
                    <ClientSideEvents Click="function(s, e) { AddSelectedItems(); }" />
                </dx:ASPxButton>
            </div>
            <div class="TopPadding">
                <dx:ASPxButton ID="btnMoveAllItemsToRight" runat="server" ClientInstanceName="btnMoveAllItemsToRight" CssClass="button"
                    AutoPostBack="False" Text="Add All >>" ToolTip="Add all items">
                    <ClientSideEvents Click="function(s, e) { AddAllItems(); }" />
                </dx:ASPxButton>
            </div>
            <div style="height: 32px">
            </div>
            <div>
                <dx:ASPxButton ID="btnMoveSelectedItemsToLeft" runat="server" ClientInstanceName="btnMoveSelectedItemsToLeft" CssClass="button"
                    AutoPostBack="False" Text="< Remove" ClientEnabled="False"
                    ToolTip="Remove selected items">
                    <ClientSideEvents Click="function(s, e) { RemoveSelectedItems(); }" />
                </dx:ASPxButton>
            </div>
            <div class="TopPadding">
                <dx:ASPxButton ID="btnMoveAllItemsToLeft" runat="server" ClientInstanceName="btnMoveAllItemsToLeft" CssClass="button"
                    AutoPostBack="False" Text="<< Remove All" ClientEnabled="False"
                    ToolTip="Remove all items">
                    <ClientSideEvents Click="function(s, e) { RemoveAllItems(); }" />
                </dx:ASPxButton>
            </div>
        </div>
        <div class="contentEditors">
            <dx:ASPxListBox ID="lbChoosen" runat="server" ClientInstanceName="lbChoosen" Width="285" EnableSynchronization="True"
               DataSourceID="dsreassign" ValueField="user_name" TextField="fn" Height="240px" SelectionMode="CheckColumn" Caption="Chosen">
                <CaptionSettings Position="Top" />
                <ClientSideEvents SelectedIndexChanged="function(s, e) { UpdateButtonState(); }"></ClientSideEvents>
            </dx:ASPxListBox>
        </div>
    </div>
    </form>
    <asp:SqlDataSource ID="dsAssignee" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="spAssignee" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="usertype" SessionField="usertype" Type="String" />
            <asp:SessionParameter Name="BU" SessionField="BU" Type="String" />
            <asp:SessionParameter Name="ID" SessionField="NewID" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsreassign" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select user_name,firstname +' '+ lastname as fn from ulogin where user_name in
         (select value from dbo.FNC_SPLIT((select isnull(Assignee,'')n from TransTechnical where UniqueColumn=@ID),','))">
        <SelectParameters>
            <asp:SessionParameter Name="ID" SessionField="NewID" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
</body>
</html>
