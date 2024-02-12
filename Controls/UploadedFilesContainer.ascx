<%@ Control Language="C#" %>
<script runat="server">
    
    public int Width { get; set; }
    public int Height { get; set; }
    public int NameColumnWidth { get; set; }
    public int SizeColumnWidth { get; set; }
    public string HeaderText { get; set; }
    public bool UseExtendedPopup { get; set; }
    protected void Page_Load(object sender, EventArgs e) {
        FilesRoundPanel.Width = Width;
        FilesRoundPanel.Height = Height;
        FilesRoundPanel.HeaderText = HeaderText;
    }
    protected string GetOptionsString() {
        return "'" + GetStyleAttributeValue(NameColumnWidth) + "', '" 
            + GetStyleAttributeValue(SizeColumnWidth) + "', " 
            +  UseExtendedPopup.ToString().ToLower();
    }
    protected string GetStyleAttributeValue(int width) {
        return width > 0 ? string.Format("width: {0}px; max-width: {0}px", width) : string.Empty;
    }
 </script>
<script type="text/javascript">
    Demo.FilesContainer(<%= GetOptionsString() %>);
</script>
<dx:ASPxRoundPanel ID="FilesRoundPanel" ClientInstanceName="FilesRoundPanel" runat="server">
    <PanelCollection>
        <dx:PanelContent runat="server">
            <table id="uploadedFilesContainer" class="uploadedFilesContainer">
                <tbody></tbody>
            </table>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxRoundPanel>

