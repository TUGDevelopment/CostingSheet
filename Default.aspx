<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SideHolder" Runat="Server">
 <script type="text/javascript">
     //if (location.protocol == 'http:')
     //    location.href = location.href.replace(/^http:/, 'https:');
     //function ChangeUrl(title, url) {
     //    if (typeof (history.pushState) != "undefined") {
     //        var obj = { Title: title, Url: url };
     //        history.pushState(obj, obj.Title, obj.Url);
     //    } else {
     //        alert("Browser does not support HTML5.");
     //    }
     //}
     //function updateURL(title) {
     //    if (history.pushState) {
     //        var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname + '?viewMode=CostingEditForm';
     //        window.history.pushState({ path: newurl }, '', newurl);
     //    }
     //}
     $(function () {
         var name=getUrlVars()["viewMode"].replace('Form','');
         NavigationMenu.GetItem(0).SetText(name);
     });    
     function getUrlVars() {
         var vars = {};
         var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi,
             function (m, key, value) {
                 vars[key] = value;
             });
         return vars;
     }
     var postponedCallbackRequired = false;
     function OnNodeClick(s, e) {
         
         //alert(s.GetSelectedNode().name);
         //updateURL(s.GetSelectedNode().name);
         //ChangeUrl('Page1', 'Default.aspx?viewMode=CostingEditForm');
         //var item = s.GetSelectedItem();
         var url = new URL(window.location.href);
         //var url = document.URL
         var name = s.GetSelectedNode().name;
         if (name == '') return;
         document.cookie = s.GetSelectedNode().GetText();
         //window.location = '/Default.aspx?viewMode=' + name;
         window.location = '/webapi/Default.aspx?viewMode=' + name;
         if (CallbackPanel.InCallback())
             postponedCallbackRequired = true;
         else {
             //s.GetSelectedNodeValues('text', setValues);
             CallbackPanel.PerformCallback(s.GetSelectedNode().GetText());
         }
     }
     function OnEndCallback(s, e) {
         if (postponedCallbackRequired) {
             CallbackPanel.PerformCallback();
             postponedCallbackRequired = false;
         }
     }
     function OnExpandedChanged(s, e) {
         //if (e.node.GetExpanded()) {
         //    e.node.SetImageUrl('Content/Images/opened_folder.png');
         //}
         //else {
         //    e.node.SetImageUrl('Content/Images/closed_folder.png');
         //}
     }
     function SelectedItemChanged(e) {
         
         var url = new URL(window.location.href);
         var name = e.item.name;
         if (name == '') return;
         
         //window.location = '/Default.aspx?viewMode=' + name;
         window.location = '/webapi/Default.aspx?viewMode=' + name;
         if (CallbackPanel.InCallback())
             postponedCallbackRequired = true;
         else {
             CallbackPanel.PerformCallback(e.item.name);
         }
     }
 </script>
     <%--<dx:ASPxTreeView runat="server" ID="treeView" ClientInstanceName="treeView" AllowSelectNode="True" CssClass="MailTree">
        <Nodes>
            <dx:TreeViewNode Text="Menu" Name="Inbox" Expanded="True" Image-SpriteProperties-CssClass="Sprite_Inbox">
                <Nodes>
                    <dx:TreeViewNode Text="Technical" Name="CustomerEditForm" Image-SpriteProperties-CssClass="Sprite_ASP" />
                    <dx:TreeViewNode Text="Costing Sheet" Name="CostingEditForm" Image-SpriteProperties-CssClass="Sprite_Announcements" />
                    <dx:TreeViewNode Text="TunaStandard" Name="CalculationForm" Image-SpriteProperties-CssClass="Sprite_DevExtreme" />
                    <dx:TreeViewNode Text="Quotation" Name="QuotationForm" Image-SpriteProperties-CssClass="Sprite_Frameworks" />
                </Nodes>
            </dx:TreeViewNode>
            <dx:TreeViewNode Text="Sent Items" Name="Sent Items" Image-SpriteProperties-CssClass="Sprite_SentItems" />
            <dx:TreeViewNode Text="Drafts" Name="Drafts" Image-SpriteProperties-CssClass="Sprite_Drafts" />
        </Nodes>
        <ClientSideEvents NodeClick="OnNodeClick"   ExpandedChanged="OnExpandedChanged"/>
    </dx:ASPxTreeView>
        <dx:ASPxNavBar ID="ASPxNavBar1" runat="server" EnableViewState="false" Width="320" EnableAnimation="true" EnableTheming="false"
            AllowSelectItem="True">
            <ClientSideEvents ItemClick="function(s, e) { SelectedItemChanged(e); }" />
        <Groups>
            <dx:NavBarGroup ItemBulletStyle="Square" Text="New features">
                <Items>
                    <dx:NavBarItem Text="New Request"  Name="CustomerEditForm"/>
                    <dx:NavBarItem Text="Update Cost" Name="UpdatePriceForm" />
                    <dx:NavBarItem Text="Costing Sheet" Name="CostingEditForm" />
                    <dx:NavBarItem Text="Quotation" Name="QuotationForm"/>
                    <dx:NavBarItem Text="TunaStandard" Name="CalculationForm" />
                </Items>
            </dx:NavBarGroup>
            <dx:NavBarGroup Text="Report">
                <Items>
                    <dx:NavBarItem Text="Technical" Name="TechnicalReport&mode=spSummaryReport"/>
                    <dx:NavBarItem Text="SummaryCosting" Name="TechnicalReport&mode=spSummaryCosting" />
                    <dx:NavBarItem Text="NPP Pricing" Name="TechnicalReport&mode=spSummaryCustomer" />
                    <dx:NavBarItem Text="KPI Costing" Name="TechnicalReport&mode=spSummaryReportCost" />
                    <dx:NavBarItem Text="SummaryRequest" Name="TechnicalReport&mode=spSummaryTechnical" />
					<dx:NavBarItem Text="CostingSheet" Name="TechnicalReport&mode=spSummaryCostingsheet" />
                    <dx:NavBarItem Text="Quotation" Name="TechnicalReport&mode=spSummaryQuotation" />
                </Items>
            </dx:NavBarGroup>
            <dx:NavBarGroup Text="Setup">
                <Items>
                    <dx:NavBarItem Text="Master" Name="EditForm"/>
                    <dx:NavBarItem Text="UploadExcel" Name="UploadControl"/>
                </Items>
            </dx:NavBarGroup>
        </Groups>
        <ItemStyle Height="20px" HorizontalAlign="Left">
            <Paddings PaddingTop="0px" PaddingRight="12px" PaddingBottom="0px" PaddingLeft="0px">
            </Paddings>
        </ItemStyle>
        <GroupHeaderStyleCollapsed HorizontalAlign="Left">
            <BorderBottom BorderStyle="Solid"></BorderBottom>
            <BackgroundImage Repeat="RepeatX" ImageUrl="~/Content/Images/nbHeaderBackCollapsed.gif">
            </BackgroundImage>
        </GroupHeaderStyleCollapsed>
        <GroupHeaderStyle BackColor="#8FC4FE" ForeColor="White" HorizontalAlign="Left">
            <Paddings PaddingTop="4px" PaddingRight="9px" PaddingBottom="5px" PaddingLeft="12px">
            </Paddings>
            <BorderBottom BorderStyle="None"></BorderBottom>
            <BackgroundImage Repeat="RepeatX" ImageUrl="~/Content/Images/nbHeaderBack.gif"></BackgroundImage>
        </GroupHeaderStyle>
        <BackgroundImage Repeat="RepeatX" ImageUrl="~/Content/Images/nbBack.gif"></BackgroundImage>
        <ExpandImage Url="~/Content/Images/nbExpand.gif" Height="13px" Width="13px">
        </ExpandImage>
        <CollapseImage Url="~/Content/Images/nbCollapse.gif" Height="13px" Width="13px">
        </CollapseImage>
        <GroupContentStyle BackColor="White" ForeColor="#1473D2" HorizontalAlign="Left">
            <Paddings PaddingTop="13px" Padding="1px" PaddingBottom="20px" PaddingLeft="15px"></Paddings>
        </GroupContentStyle>
    </dx:ASPxNavBar>--%>
    <dx:ASPxTreeView runat="server" ID="treeView" ClientInstanceName="treeView" AllowSelectNode="True" CssClass="MailTree" 
        ImageUrlField="NodeTypeImage">
        <Nodes>
            <%--<dx:TreeViewNode Text="Menu" Expanded="True">
                <Nodes>--%>
                    <dx:TreeViewNode Text="Technical">
                        <Nodes>
                            <dx:TreeViewNode Text="New Request"  Name="CustomerEditForm"/>
                            <dx:TreeViewNode Text="Update Cost" Name="UpdatePriceForm" />
                        </Nodes>
                    </dx:TreeViewNode>
                    <dx:TreeViewNode Text="Costing Sheet" Name="CostingEditForm" />
                    <dx:TreeViewNode Text="Quotation" Name="QuotationForm"/>
                    <dx:TreeViewNode Text="Formula" Name="formulaForm" />
                    <dx:TreeViewNode Text="TunaStandard" Name="CalculationForm2" />
                    <dx:TreeViewNode Text="RequestForm">
                        <Nodes>
                            <dx:TreeViewNode Text="New Request" Name="RequestEditForm"/>
                            <dx:TreeViewNode Text="Copy Destination" Name="RequestDesForm" />
                        </Nodes>
                    </dx:TreeViewNode>
                    <dx:TreeViewNode Text="Report">
                        <Nodes>
                            <dx:TreeViewNode Text="Technical" Name="TechnicalReport&mode=spSummaryReport"/>
                            <dx:TreeViewNode Text="SummaryCosting" Name="TechnicalReport&mode=spSummaryCosting" />
                            <dx:TreeViewNode Text="NPP Pricing" Name="TechnicalReport&mode=spSummaryCustomer" />
                            <dx:TreeViewNode Text="KPI Costing" Name="TechnicalReport&mode=spSummaryReportCost" />
                            <dx:TreeViewNode Text="SummaryRequest" Name="TechnicalReport&mode=spSummaryTechnical" />
							<dx:TreeViewNode Text="CostingSheet" Name="TechnicalReport&mode=spSummaryCostingsheet" />
                            <dx:TreeViewNode Text="SummaryQuotation" Name="ReportQuotation&mode=spSummaryQuotation" />
                        </Nodes>
                    </dx:TreeViewNode>
                    <dx:TreeViewNode Text="Setup" Name="EditForm"/>
                    <dx:TreeViewNode Text="UploadExcel" Name="UploadControl&mode=oldcost"/>
                    <dx:TreeViewNode Text="ChangeCostExc" Name="UploadControl&mode=update"/>
             <%--   </Nodes>
            </dx:TreeViewNode>--%>
        </Nodes>
       <Styles>
            <NodeImage Paddings-PaddingTop="3px">
            </NodeImage>
        </Styles>
        <ClientSideEvents NodeClick="OnNodeClick"
            ExpandedChanged="OnExpandedChanged"/>
    </dx:ASPxTreeView>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainHolder" Runat="Server">
<dx:ASPxCallbackPanel ID="cp" runat="server" 
    ClientInstanceName="CallbackPanel" OnCallback="cp_Callback">
    <ClientSideEvents EndCallback="OnEndCallback" />
    <PanelCollection>
        <dx:PanelContent ID="pc" runat="server">
        <div id="MasterContainer" runat="server"/>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxCallbackPanel>
</asp:Content>