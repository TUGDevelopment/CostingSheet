<%@ Control Language="C#" AutoEventWireup="true" CodeFile="test.ascx.cs" Inherits="UserControls_test" %>
<dx:ASPxCallbackPanel ID="FormPanel" runat="server" RenderMode="Div" 
        ScrollBars="Vertical" ClientInstanceName="ClientCostFormPanel">
        <PanelCollection>
            <dx:PanelContent>  
            <dx:ASPxFormLayout runat="server" ID="formLayout" CssClass="formLayout">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
            <Items>
                <dx:LayoutGroup Caption="" ColCount="3" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                    <Paddings PaddingTop="10px"></Paddings>
                    <GroupBoxStyle>
                        <Caption Font-Bold="true" Font-Size="10"/>
                    </GroupBoxStyle>
                    <Items>
                        <dx:LayoutItem Caption="Company Code">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
        <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" KeyFieldName="Id"
            OnCellEditorInitialize="ASPxGridView1_CellEditorInitialize">
            <Columns>
                <dx:GridViewCommandColumn ShowNewButtonInHeader="true" ShowEditButton="true" />
            </Columns>
        </dx:ASPxGridView>

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
        <asp:SqlDataSource ID="dsYield" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="Select * From MasYield">
    </asp:SqlDataSource>