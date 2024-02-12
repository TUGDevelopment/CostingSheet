<%@ Page Language="C#" AutoEventWireup="true" CodeFile="product_list.aspx.cs" Inherits="popupControls_product_list" %>

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
        var curentEditingIndex;
        function gv_BatchEditEndEditing(s, e) {
            debugger;
            window.setTimeout(function () {
                var count_value = GetChangesCount(s.batchEditApi);
                if (count_value > 0) {
                    s.UpdateEdit();
                    SetButtonsVisibility('visible');
                }
            }, 0);
        }
        function GetChangesCount(batchApi) {
            var updatedCount = batchApi.GetUpdatedRowIndices().length;
            var deletedCount = batchApi.GetDeletedRowIndices().length;
            var insertedCount = batchApi.GetInsertedRowIndices().length;

            return updatedCount + deletedCount + insertedCount;
        }
        function OnRowDblClick(s, e) {
            var result = s.GetRowKey(e.visibleIndex);
            window.parent.HidePopupAndShowInfo('Client', result);
            //s.GetRowValues(e.visibleIndex, 'Material', OnGetRowId); 
            //window.opener.OnCloseSelector(key);
            //window.close();
        }
        function OnInit(s, e) {
            debugger;
            AdjustSize();
            document.getElementById("gridContainer").style.visibility = "";
            SetButtonsVisibility('hidden');
        }
        function OnEndCallback(s, e) {
            AdjustSize();
            //SetButtonsVisibility(s);
        }
        function OnControlsInitialized(s, e) {
            ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
                AdjustSize();
            });
        }
        function AdjustSize() {
            var height = Math.max(0, document.documentElement.clientHeight);
            gv.SetHeight(height);
        }
        function OnFileUploadComplete(s, e) {
            debugger;
            gv.PerformCallback('upload|1');
            SetButtonsVisibility('visible');
            //gv.batchEditApi.EndEdit();
            //gv.batchEditApi.SetCellValue(1, "Name", e.callbackData);
        }
        var isPreviewChangesVisible = false;
        function SetButtonsVisibility(s) {
            var statusBar = gv.GetMainElement().getElementsByClassName("StatusBarWithButtons")[0].getElementsByTagName("td")[0];
            statusBar.style.visibility = s;
            //if (!s.batchEditApi.HasChanges())
            //    statusBar.style.visibility = "hidden";
            //else {
            //    statusBar.style.visibility = "visible";
            //}
        }
        function onPreviewChangesClick(s, e) {
            if (isPreviewChangesVisible) {
                s.SetText("Show changes");
                gv.batchEditApi.HideChangesPreview();
            }
            else {
                s.SetText("Hide preview");
                gv.batchEditApi.ShowChangesPreview();
            }
            isPreviewChangesVisible = !isPreviewChangesVisible;
        }
        function OnCustomButtonClick(s, e) {
            if (e.buttonID == "deleteButton") {
                s.DeleteRow(e.visibleIndex);
                //SetButtonsVisibility(s);
            }
        }
        //function OnBatchEditEndEditing(s, e) {
        //    window.setTimeout(function () { SetButtonsVisibility(s); }, 0);
        //}
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="gridContainer" style="visibility: hidden">
               <dx:ASPxFormLayout runat="server" ID="formLayout" ClientInstanceName="formLayout" CssClass="formLayout" AlignItemCaptionsInAllGroups="true">
                    <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
                    <Items>
                        <dx:LayoutGroup Caption="" Name="layoutname" ColCount="4" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                            <GroupBoxStyle>
                                <Caption Font-Bold="true" Font-Size="10"/>
                            </GroupBoxStyle>
                            <GridSettings StretchLastItem="true" WrapCaptionAtWidth="660">
                                <Breakpoints>
                                    <dx:LayoutBreakpoint MaxWidth="500" ColumnCount="1" Name="S" />
                                    <dx:LayoutBreakpoint MaxWidth="800" ColumnCount="2" Name="M" />
                                </Breakpoints>
                            </GridSettings>
                            <Items>
                            <dx:LayoutItem Caption="Selected" Name="Upload" Width="100%" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxUploadControl ID="UpCode" runat="server" Width="200" ClientInstanceName="UpCode" OnFileUploadComplete="UpCode_FileUploadComplete"
                                    NullText="Click here to browse files..." UploadMode="Advanced" FileUploadMode="OnPageLoad" AutoStartUpload="True">
                                    <ValidationSettings AllowedFileExtensions=".xls,.xlsx">
                                    </ValidationSettings>
                                    <ClientSideEvents FileUploadComplete="OnFileUploadComplete" />
                                </dx:ASPxUploadControl>
                                </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="" Name="Product_List" Width="100%" VerticalAlign="Middle">
                            <SpanRules>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="S"></dx:SpanRule>
                                <dx:SpanRule ColumnSpan="1" RowSpan="1" BreakpointName="M"></dx:SpanRule>
                            </SpanRules>
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxGridView runat="server" ID="gv" Width="100%" KeyFieldName="Id" 
                                        ClientInstanceName="gv" OnCustomCallback="gv_CustomCallback" OnBatchUpdate="gv_BatchUpdate"
                                    OnDataBinding="gv_DataBinding" OnCustomButtonCallback="gv_CustomButtonCallback" 
                                    OnCustomButtonInitialize="gv_CustomButtonInitialize" 
                                    OnDataBound="gv_DataBound">
                                        <ClientSideEvents RowDblClick="OnRowDblClick"  Init="OnInit" EndCallback="OnEndCallback" 
                                            BatchEditEndEditing="gv_BatchEditEndEditing" BatchEditStartEditing="function(s, e) {
                                            curentEditingIndex = e.visibleIndex; }" CustomButtonClick="OnCustomButtonClick"/>
                                        <Columns>
                                            <dx:GridViewCommandColumn  ShowClearFilterButton="true" Width="65px" ButtonRenderMode="Image"
                                                FixedStyle="Left" ShowNewButtonInHeader="true" ShowEditButton="true" ShowDeleteButton="true">
                                                <CustomButtons>
                                                    <dx:GridViewCommandColumnCustomButton ID="Edit">
                                                        <Image ToolTip="Remove" Url="~/Content/Images/Cancel.gif"/>
                                                    </dx:GridViewCommandColumnCustomButton>
                                                </CustomButtons>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <HeaderTemplate>
                                                    <dx:ASPxButton runat="server" RenderMode="Link" AutoPostBack="false">
                                                        <Image ToolTip="Insert" Url="~/Content/Images/icons8-plus-math-filled-16.png" />
                                                        <ClientSideEvents Click="function(s,e){ gv.AddNewRow(); }" />
                                                    </dx:ASPxButton>
                                                </HeaderTemplate>
                                            </dx:GridViewCommandColumn>
                                            <dx:GridViewDataColumn FieldName="RowID" ReadOnly="true" Width="5%" EditFormSettings-Visible="False" />
                                            <dx:GridViewDataTextColumn FieldName="Name" Caption="ProductName" Width="45%"/>
                                            <dx:GridViewDataTextColumn FieldName="NetWeight" Caption="N/W (g)" />
                                            <dx:GridViewDataTextColumn FieldName="DW" Caption="D/W (g)" />
                                            <dx:GridViewDataComboBoxColumn FieldName="DWType">
                                                <PropertiesComboBox DataSourceID="dsDWeight" ValueField="ID" TextField="Name" />
                                            </dx:GridViewDataComboBoxColumn>
                                            <dx:GridViewDataTextColumn FieldName="FixedFillWeight" Caption="Fixed F/W (g)" />
                                            <dx:GridViewDataTextColumn FieldName="PW" Caption="P/W (g)" />
                                            <dx:GridViewDataTextColumn FieldName="TargetPrice" />
                                            <dx:GridViewDataTextColumn FieldName="SaltContent" />
                                            <dx:GridViewDataTextColumn FieldName="Efficiency" Width="0px" CellStyle-CssClass="unitPriceColumn" HeaderStyle-CssClass="unitPriceColumn" />
                                            <dx:GridViewDataTextColumn FieldName="Yield" Width="0px" CellStyle-CssClass="unitPriceColumn" HeaderStyle-CssClass="unitPriceColumn"/>
                                            <dx:GridViewDataColumn FieldName="Mark" Width="0px" CellStyle-CssClass="unitPriceColumn" HeaderStyle-CssClass="unitPriceColumn"/>
                                        </Columns>
                                        <SettingsBehavior AllowFixedGroups="true" AutoExpandAllGroups="true" />
                                        <SettingsPager PageSize="20" />
                                        <SettingsContextMenu Enabled="true" />
                                        <Settings ShowVerticalScrollBar="true" VerticalScrollableHeight="0"/>
                                        <SettingsExport EnableClientSideExportAPI="true" ExcelExportMode="DataAware" />
                                        <Templates>
                                            <StatusBar>
                                                <div style="text-align: right">
                                                    <dx:ASPxButton ID="btnPrevChanges" runat="server" RenderMode="Outline" Text="Preview changes" ClientVisible="false" 
                                                        AutoPostBack="false">
                                                        <ClientSideEvents Click="onPreviewChangesClick" />
                                                    </dx:ASPxButton>
                                                    <dx:ASPxButton ID="btnSave" runat="server" RenderMode="Outline" Text="Save changes" AutoPostBack="false">
                                                        <ClientSideEvents Click="function(s, e){ gv.PerformCallback('SaveMail|1');
                                                            SetButtonsVisibility('hidden'); }" />
                                                    </dx:ASPxButton>
                                                    <dx:ASPxButton ID="btnCancel" runat="server" RenderMode="Outline" Text="Cancel changes" AutoPostBack="false">
                                                        <ClientSideEvents Click="function(s, e){ gv.CancelEdit();}" />
                                                    </dx:ASPxButton>
                                                </div>
                                            </StatusBar>
                                        </Templates>
                                        <SettingsEditing Mode="Batch" />
                                        <Styles>
                                            <StatusBar CssClass="StatusBarWithButtons">
                                            </StatusBar>
                                        </Styles>
                                    </dx:ASPxGridView>
                        </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:LayoutGroup>
            </Items>
        </dx:ASPxFormLayout>
        </div>
    </form>
   <asp:SqlDataSource ID="dsnamelist" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="select ROW_NUMBER() OVER(ORDER BY Id) AS RowID ,*,''Mark from TransProductList where RequestNo=@Id">
       <SelectParameters>
           <asp:Parameter Name="Id" />
       </SelectParameters>
   </asp:SqlDataSource>
    <asp:SqlDataSource ID="dsDWeight" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
       SelectCommand="select * from MasDWeight where dbo.fnc_checktype(usertype,@usertp)>0">
        <SelectParameters>
            <asp:QueryStringParameter Name="usertp" QueryStringField="usertype" Type="Single" />
        </SelectParameters>
    </asp:SqlDataSource>
</body>
</html>
