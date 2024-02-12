<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrintMails.aspx.cs" Inherits="MailMerge" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Print Costing</title>
        <script>
            function Spreadsheet1_onCustomCommandExecuted(s, e) {
                debugger;
            if (e.commandName == "emailExcel") {
                pcConfirm4.Show();
                //clb.PerformCallback();
            }
            if (e.commandName == "downloadExcel") {
                clb_export_xlsx.PerformCallback();
            }
        }
    </script>
	<link rel="icon" href="Content/LogoThaiUnion.png"/>
</head>
<body>
    <form id="form1" runat="server">
    <dx:ASPxHiddenField ID="hfusername" runat="server" />
<%--    <dx:ReportToolbar ID="ReportToolbar1" runat="server" ShowDefaultButtons="False" ReportViewerID="ReportViewer1">
            <Items>
                <dx:ReportToolbarButton ItemKind="Search" />
                <dx:ReportToolbarSeparator />
                <dx:ReportToolbarButton ItemKind="PrintReport" />
                <dx:ReportToolbarButton ItemKind="PrintPage" />
                <dx:ReportToolbarSeparator />
                <dx:ReportToolbarButton Enabled="False" ItemKind="FirstPage" />
                <dx:ReportToolbarButton Enabled="False" ItemKind="PreviousPage" />
                <dx:ReportToolbarLabel ItemKind="PageLabel" />
                <dx:ReportToolbarComboBox ItemKind="PageNumber" Width="65px">
                </dx:ReportToolbarComboBox>
                <dx:ReportToolbarLabel ItemKind="OfLabel" />
                                   
                <dx:ReportToolbarTextBox ItemKind="PageCount" />
                <dx:ReportToolbarButton ItemKind="NextPage" />
                <dx:ReportToolbarButton ItemKind="LastPage" />
                <dx:ReportToolbarSeparator />
                <dx:ReportToolbarButton ItemKind="SaveToDisk" />
                <dx:ReportToolbarButton ItemKind="SaveToWindow" />
                <dx:ReportToolbarComboBox ItemKind="SaveFormat" Width="70px">
                    <Elements>
                        <dx:ListElement Value="pdf" />
                        <dx:ListElement Value="xls" />
                        <dx:ListElement Value="xlsx" />
                        <dx:ListElement Value="csv" />
                    </Elements>
                </dx:ReportToolbarComboBox>
            </Items>
            <Styles>
                <LabelStyle>
                    <Margins MarginLeft='3px' MarginRight='3px' />
                </LabelStyle>
            </Styles>
        </dx:ReportToolbar>
    <dx:ReportViewer ID="ReportViewer1" runat="server" 
                OnUnload="ReportViewer1_Unload"
                Report="<%# new WebApplication1.XtraReport4() %>"   
                ReportName="WebApplication1.XtraReport4"  >
                </dx:ReportViewer>
    <dx:PdfViewer ID="viewer" runat="server" Visible="false" />--%>
    <dx:ASPxSpreadsheet ID="Spreadsheet" runat="server" Width="100%" Height="740px" ActiveTabIndex="0" 
        ShowConfirmOnLosingChanges="false" RibbonMode="Ribbon" WorkDirectory="~/App_Data/WorkDirectory"
            ClientInstanceName="ClientSpreadsheet" OnCallback="ASPxSpreadsheet1_Callback" 
                                CustomCommandExecuted   ="Spreadsheet1_onCustomCommandExecuted">
         <ClientSideEvents Init="function(s, e){ s.SetFullscreenMode(true); }" />
        <SettingsDialogs >
                <SaveFileDialog DisplaySectionMode="ShowDownloadSection"  />
            </SettingsDialogs>
            <RibbonTabs>
                <dx:SRFileTab>
                    <Groups>
                        <dx:SRFileCommonGroup Text="File features">
                            <Items>
                                
                                <dx:RibbonButtonItem LargeImage-IconID="mail_emailtemplate_32x32office2013" Name="downloadExcel" Text="Download xlsx (nuevo)" Size="Large" >
                                    <LargeImage IconID="actions_download_32x32office2013"></LargeImage>
                                </dx:RibbonButtonItem>
                                <dx:SRFileSaveAsCommand Text="Download (.xlsx)" >
                                    <LargeImage IconID="actions_download_32x32office2013"></LargeImage>
                                </dx:SRFileSaveAsCommand>
                                <dx:SRFilePrintCommand Text="Print/Export PDF"></dx:SRFilePrintCommand>

                                <dx:RibbonButtonItem LargeImage-IconID="mail_emailtemplate_32x32office2013" Name="emailExcel"  Text="Send by e-mail" Size="Large" >
                                    <LargeImage IconID="mail_emailtemplate_32x32office2013"></LargeImage>
                                </dx:RibbonButtonItem>

                            </Items>
                        </dx:SRFileCommonGroup>
                    </Groups>
                </dx:SRFileTab>
                <dx:SRHomeTab>
                    <Groups>
                        <dx:SRUndoGroup>
                            <Items>
                                <dx:SRFileUndoCommand>
                                </dx:SRFileUndoCommand>
                                <dx:SRFileRedoCommand>
                                </dx:SRFileRedoCommand>
                            </Items>
                        </dx:SRUndoGroup>
                        <dx:SRClipboardGroup>
                            <Items>
                                <dx:SRPasteSelectionCommand>
                                </dx:SRPasteSelectionCommand>
                                <dx:SRCutSelectionCommand>
                                </dx:SRCutSelectionCommand>
                                <dx:SRCopySelectionCommand>
                                </dx:SRCopySelectionCommand>
                            </Items>
                        </dx:SRClipboardGroup>
                        <dx:SRFontGroup>
                            <Items>
                                <dx:SRFormatFontNameCommand>
                                    <PropertiesComboBox NullText="(Font Name)" Width="130px">
                                    </PropertiesComboBox>
                                </dx:SRFormatFontNameCommand>
                                <dx:SRFormatFontSizeCommand>
                                    <PropertiesComboBox NullText="(Font Size)" Width="60px">
                                    </PropertiesComboBox>
                                </dx:SRFormatFontSizeCommand>
                                <dx:SRFormatIncreaseFontSizeCommand>
                                </dx:SRFormatIncreaseFontSizeCommand>
                                <dx:SRFormatDecreaseFontSizeCommand>
                                </dx:SRFormatDecreaseFontSizeCommand>
                                <dx:SRFormatFontBoldCommand>
                                </dx:SRFormatFontBoldCommand>
                                <dx:SRFormatFontItalicCommand>
                                </dx:SRFormatFontItalicCommand>
                                <dx:SRFormatFontUnderlineCommand>
                                </dx:SRFormatFontUnderlineCommand>
                                <dx:SRFormatFontStrikeoutCommand>
                                </dx:SRFormatFontStrikeoutCommand>
                                <dx:SRFormatBordersCommand>
                                </dx:SRFormatBordersCommand>
                                <dx:SRFormatFillColorCommand AutomaticColorItemValue="Automatic">
                                </dx:SRFormatFillColorCommand>
                                <dx:SRFormatFontColorCommand AutomaticColorItemValue="Automatic">
                                </dx:SRFormatFontColorCommand>
                                <dx:SRFormatBorderLineColorCommand AutomaticColorItemValue="Automatic">
                                </dx:SRFormatBorderLineColorCommand>
                            </Items>
                        </dx:SRFontGroup>
                        <dx:SRAlignmentGroup>
                            <Items>
                                <dx:SRFormatAlignmentTopCommand>
                                </dx:SRFormatAlignmentTopCommand>
                                <dx:SRFormatAlignmentMiddleCommand>
                                </dx:SRFormatAlignmentMiddleCommand>
                                <dx:SRFormatAlignmentBottomCommand>
                                </dx:SRFormatAlignmentBottomCommand>
                                <dx:SRFormatAlignmentLeftCommand>
                                </dx:SRFormatAlignmentLeftCommand>
                                <dx:SRFormatAlignmentCenterCommand>
                                </dx:SRFormatAlignmentCenterCommand>
                                <dx:SRFormatAlignmentRightCommand>
                                </dx:SRFormatAlignmentRightCommand>
                                <dx:SRFormatDecreaseIndentCommand>
                                </dx:SRFormatDecreaseIndentCommand>
                                <dx:SRFormatIncreaseIndentCommand>
                                </dx:SRFormatIncreaseIndentCommand>
                                <dx:SRFormatWrapTextCommand>
                                </dx:SRFormatWrapTextCommand>
                                <dx:SREditingMergeCellsGroupCommand>
                                </dx:SREditingMergeCellsGroupCommand>
                            </Items>
                        </dx:SRAlignmentGroup>
                        <dx:SRNumberGroup>
                            <Items>
                                <dx:SRFormatNumberAccountingCommand>
                                </dx:SRFormatNumberAccountingCommand>
                                <dx:SRFormatNumberPercentCommand>
                                </dx:SRFormatNumberPercentCommand>
                                <dx:SRFormatNumberCommaStyleCommand>
                                </dx:SRFormatNumberCommaStyleCommand>
                                <dx:SRFormatNumberIncreaseDecimalCommand>
                                </dx:SRFormatNumberIncreaseDecimalCommand>
                                <dx:SRFormatNumberDecreaseDecimalCommand>
                                </dx:SRFormatNumberDecreaseDecimalCommand>
                            </Items>
                        </dx:SRNumberGroup>
                        <dx:SRCellsGroup>
                            <Items>
                                <dx:SRFormatInsertCommand>
                                </dx:SRFormatInsertCommand>
                                <dx:SRFormatRemoveCommand>
                                </dx:SRFormatRemoveCommand>
                                <dx:SRFormatFormatCommand>
                                </dx:SRFormatFormatCommand>
                            </Items>
                        </dx:SRCellsGroup>
                        <dx:SREditingGroup>
                            <Items>
                                <dx:SRFormatAutoSumCommand>
                                </dx:SRFormatAutoSumCommand>
                                <dx:SRFormatFillCommand>
                                </dx:SRFormatFillCommand>
                                <dx:SRFormatClearCommand>
                                </dx:SRFormatClearCommand>
                                <dx:SREditingSortAndFilterCommand>
                                </dx:SREditingSortAndFilterCommand>
                                <dx:SREditingFindAndSelectCommand>
                                </dx:SREditingFindAndSelectCommand>
                            </Items>
                        </dx:SREditingGroup>
                        <dx:SRStylesGroup>
                            <Items>
                                <dx:SRFormatAsTableCommand>
                                </dx:SRFormatAsTableCommand>
                            </Items>
                        </dx:SRStylesGroup>
                    </Groups>
                </dx:SRHomeTab>
                <dx:SRInsertTab>
                    <Groups>
                        <dx:SREditingGroup>
                            <Items>
                                <dx:SRFormatAutoSumCommand>
                                </dx:SRFormatAutoSumCommand>
                                <dx:SRFormatFillCommand>
                                </dx:SRFormatFillCommand>
                                <dx:SRFormatClearCommand>
                                </dx:SRFormatClearCommand>
                                <dx:SREditingSortAndFilterCommand>
                                </dx:SREditingSortAndFilterCommand>
                                <dx:SREditingFindAndSelectCommand>
                                </dx:SREditingFindAndSelectCommand>
                            </Items>
                        </dx:SREditingGroup>
                        <dx:SRTablesGroup>
                            <Items>
                                <dx:SRInsertTableCommand>
                                </dx:SRInsertTableCommand>
                            </Items>
                        </dx:SRTablesGroup>
                        <dx:SRIllustrationsGroup>
                            <Items>
                                <dx:SRFormatInsertPictureCommand>
                                </dx:SRFormatInsertPictureCommand>
                            </Items>
                        </dx:SRIllustrationsGroup>
                        <dx:SRChartsGroup>
                            <Items>
                                <dx:SRInsertChartColumnCommand>
                                </dx:SRInsertChartColumnCommand>
                                <dx:SRInsertChartLinesCommand>
                                </dx:SRInsertChartLinesCommand>
                                <dx:SRInsertChartPiesCommand>
                                </dx:SRInsertChartPiesCommand>
                                <dx:SRInsertChartBarsCommand>
                                </dx:SRInsertChartBarsCommand>
                                <dx:SRInsertChartAreasCommand>
                                </dx:SRInsertChartAreasCommand>
                                <dx:SRInsertChartScattersCommand>
                                </dx:SRInsertChartScattersCommand>
                                <dx:SRInsertChartOthersCommand>
                                </dx:SRInsertChartOthersCommand>
                            </Items>
                        </dx:SRChartsGroup>
                        <dx:SRLinksGroup>
                            <Items>
                                <dx:SRFormatInsertHyperlinkCommand>
                                </dx:SRFormatInsertHyperlinkCommand>
                            </Items>
                        </dx:SRLinksGroup>
                    </Groups>
                </dx:SRInsertTab>
                <dx:SRPageLayoutTab>
                    <Groups>
                        <dx:SRPageSetupGroup>
                            <Items>
                                <dx:SRPageSetupMarginsCommand>
                                </dx:SRPageSetupMarginsCommand>
                                <dx:SRPageSetupOrientationCommand>
                                </dx:SRPageSetupOrientationCommand>
                                <dx:SRPageSetupPaperKindCommand>
                                </dx:SRPageSetupPaperKindCommand>
                            </Items>
                        </dx:SRPageSetupGroup>
                        <dx:SRPrintGroup>
                            <Items>
                                <dx:SRPrintGridlinesCommand>
                                </dx:SRPrintGridlinesCommand>
                                <dx:SRPrintHeadingsCommand>
                                </dx:SRPrintHeadingsCommand>
                            </Items>
                        </dx:SRPrintGroup>
                    </Groups>
                </dx:SRPageLayoutTab>
                <dx:SRDataTab>
                    <Groups>
                        <dx:SRDataSortAndFilterGroup>
                            <Items>
                                <dx:SRDataSortAscendingCommand>
                                </dx:SRDataSortAscendingCommand>
                                <dx:SRDataSortDescendingCommand>
                                </dx:SRDataSortDescendingCommand>
                                <dx:SRDataFilterToggleCommand ShowText="True">
                                </dx:SRDataFilterToggleCommand>
                                <dx:SRDataFilterClearCommand>
                                </dx:SRDataFilterClearCommand>
                                <dx:SRDataFilterReApplyCommand>
                                </dx:SRDataFilterReApplyCommand>
                            </Items>
                        </dx:SRDataSortAndFilterGroup>
                        <dx:SRDataToolsGroup>
                            <Items>
                                <dx:SRDataToolsDataValidationGroupCommand>
                                </dx:SRDataToolsDataValidationGroupCommand>
                            </Items>
                        </dx:SRDataToolsGroup>
                    </Groups>
                </dx:SRDataTab>
                <dx:SRViewTab>
                    <Groups>
                        <dx:SRShowGroup>
                            <Items>
                                <dx:SRViewShowGridlinesCommand>
                                </dx:SRViewShowGridlinesCommand>
                            </Items>
                        </dx:SRShowGroup>
                        <dx:SRViewGroup>
                            <Items>
                                <dx:SRFullScreenCommand>
                                </dx:SRFullScreenCommand>
                            </Items>
                        </dx:SRViewGroup>
                        <dx:SRWindowGroup>
                            <Items>
                                <dx:SRViewFreezePanesGroupCommand>
                                </dx:SRViewFreezePanesGroupCommand>
                            </Items>
                        </dx:SRWindowGroup>
                    </Groups>
                </dx:SRViewTab>
            </RibbonTabs>
            <SettingsDocumentSelector>
                <CommonSettings AllowedFileExtensions=".pdf, .xlsx, .xlsm, .xls, .xltx, .xltm, .xlt, .txt" />
                <EditingSettings AllowCopy="false" />
            </SettingsDocumentSelector>
    </dx:ASPxSpreadsheet>
    
    </form>
</body>
</html>
