<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PdfViewer.ascx.cs" Inherits="PdfViewer" %>
<dx:ASPxLabel ID="lbErrorMessage" runat="server" ForeColor="Red">
</dx:ASPxLabel>
<dx:ASPxMenu ID="mnuToolbar" runat="server" ClientInstanceName="mnuToolbar" ApplyItemStyleToTemplates="True" ClientVisible="false">
    <Items>
        <dx:MenuItem Name="mnuPrint" Text="" ToolTip="Print the chart">
            <Image Url="~/Content/Images/BtnPrint.png" />
        </dx:MenuItem>
        <dx:MenuItem Name="mnuSaveToDisk" Text="" ToolTip="Export a chart and save it to the disk" BeginGroup="True">
            <Image Url="~/Content/Images/BtnSave.png" />
        </dx:MenuItem>
        <dx:MenuItem Name="mnuSaveToWindow" Text="" ToolTip="Export a chart and show it in a new window">
            <Image Url="~/Content/Images/BtnSaveWindow.png" />
        </dx:MenuItem>
    </Items>
    <ClientSideEvents ItemClick="function(s, e) {
        if (e.item.name == 'mnuPrint')
	        //chart.Print();
            dvDocument.GetViewer().Print();

        if (e.item.name == 'mnuSaveToDisk')
        {                       
            //chart.SaveToDisk('pdf');
            dvDocument.GetViewer().SaveToDisk('PDF');
            }
        if (e.item.name == 'mnuSaveToWindow')
            //chart.SaveToWindow('pdf');
            dvDocument.SaveToWindow('PDF');
    }" />
</dx:ASPxMenu>
<dx:ASPxDataView ID="dvDocument" runat="server" ClientInstanceName="dvDocument">
    <SettingsTableLayout ColumnCount="1" RowsPerPage="1" />
    <PagerSettings ShowNumericButtons="True">
        <AllButton Visible="True">
        </AllButton>
    </PagerSettings>
    <ItemTemplate>
        <dx:ASPxBinaryImage ID="bimPdfPage" runat="server" OnDataBinding="bimPdfPage_DataBinding">
        </dx:ASPxBinaryImage>
    </ItemTemplate>
    <ItemStyle>
        <Paddings Padding="0px" />
    </ItemStyle>
</dx:ASPxDataView>