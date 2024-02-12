<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LoadContentOnDemand.aspx.cs" Inherits="popupControls_LoadContentOnDemend" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        table
        {
            max-width:none;
            background-color:transparent;
            border-collapse:collapse;
            border-spacing:0;
        }

        .table
        {
            width:auto;
            height:auto;
            margin-bottom:20px;
        }

        .table th,.table td
        {
            width:auto;
            height:auto;
            padding:8px;
            line-height:20px;
            text-align:left;
            vertical-align:top;
            border-top:1px solid #dddddd;
        }

        .table th
        {
            width:auto;
            height:auto;
            font-weight:bold;
        }

        .table thead th
        {
            vertical-align:bottom;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Panel ID="Panel1" runat="server"/>
            <asp:GridView ID="GridView1" runat="server" DataSourceID="dsgv" AutoGenerateColumns="false" GridLines="None"
                CssClass="table table-bordered table-striped"
                EmptyDataText = "No files uploaded">
                <Columns>
                    <asp:BoundField DataField="Result" HeaderText="Topic"/>
                    <asp:BoundField DataField="Notes" HeaderText="Notes"/>
                    
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("ID") %>' runat="server" OnClick="ViewDetails">
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
   <asp:SqlDataSource ID="dsgv" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="spGetstdAttached" SelectCommandType="StoredProcedure">
       <SelectParameters>
           <asp:QueryStringParameter Name="Id" QueryStringField="Id" />
       </SelectParameters>
   </asp:SqlDataSource>
</body>
</html>
