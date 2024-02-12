<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomPager.ascx.cs" Inherits="UserControls_CustomPager" %>
 <div style="text-align: right;">
                <table>
                    <tr>
                        <td>Items per page:
                        </td>
                        <td>
                            <dx:ASPxComboBox ID="cbRecords" runat="server" ToolTip="Items Per Page" ValueType="System.Int32"
                                Width="50px" OnInit="cbRecords_Init">
                                <ClientSideEvents SelectedIndexChanged="function (s, e) { grid.PerformCallback(s.GetValue()); }"
                                    Init="function (s, e) { s.SetValue(grid.cpPageSize); }" />
                            </dx:ASPxComboBox>
                        </td>
                        <td>
                            <dx:ASPxHyperLink ID="lnkFirstPage" runat="server" Text="<<" NavigateUrl="javascript:void(0);">
                                <ClientSideEvents Click="function (s, e) { grid.GotoPage(0); }" />
                            </dx:ASPxHyperLink>
                        </td>
                        <td>
                            <dx:ASPxHyperLink ID="lnkPrevPage" runat="server" Text="<" NavigateUrl="javascript:void(0);">
                                <ClientSideEvents Click="function (s, e) { grid.PrevPage(); }" />
                            </dx:ASPxHyperLink>
                        </td>
                        <td>
                            <dx:ASPxComboBox ID="cbPage" runat="server" ToolTip="Current Page" ValueType="System.Int32"
                                Width="50px" OnInit="cbPage_Init">
                                <ClientSideEvents SelectedIndexChanged="function (s, e) { grid.GotoPage(parseInt(s.GetValue()) - 1); }"
                                    Init="function (s, e) { s.SetValue(grid.GetPageIndex()+1); }" />
                            </dx:ASPxComboBox>
                        </td>
                        <td>
                            <dx:ASPxHyperLink ID="lnkNextPage" runat="server" Text=">" NavigateUrl="javascript:void(0);">
                                <ClientSideEvents Click="function (s, e) { grid.NextPage(); }" />
                            </dx:ASPxHyperLink>
                        </td>
                        <td>
                            <dx:ASPxHyperLink ID="lnkLastPage" runat="server" Text=">>" NavigateUrl="javascript:void(0);">
                                <ClientSideEvents Click="function (s, e) { grid.GotoPage(grid.GetPageCount()-1); }" />
                            </dx:ASPxHyperLink>
                        </td>
                    </tr>
                </table>
            </div>     