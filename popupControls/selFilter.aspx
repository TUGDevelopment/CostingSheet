<%@ Page Language="C#" AutoEventWireup="true" CodeFile="selFilter.aspx.cs" Inherits="popupControls_selFilter" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .formLayout {
            max-width: 1300px;
            margin: auto;
        }
    </style>
    <script>
        window.onload = function () {
            var myDate = addDays(-365);
            ClientValidfrom.SetText(myDate);
            ClientValidto.SetText(currentdate());
            //alert("Got Called!!");
            //.PerformCallback('reload');
            //var UserType = Demo.getUrlVars()["UserType"];
        }
        function addDays(n) {
            var t = new Date();
            t.setDate(t.getDate() + n);
            var month = "0" + (t.getMonth() + 1);
            var date = "0" + t.getDate();
            month = month.slice(-2);
            date = date.slice(-2);
            var date = date + "/" + month + "/" + t.getFullYear();
            return date;
        }
        var monthNames = [
            "January", "February", "March", "April", "May", "June", "July",
            "August", "September", "October", "November", "December"
        ];
        var dayOfWeekNames = [
            "Sunday", "Monday", "Tuesday",
            "Wednesday", "Thursday", "Friday", "Saturday"
        ];
        function formatDate(date, patternStr) {
            if (!patternStr) {
                patternStr = 'M/d/yyyy';
            }
            var day = date.getDate(),
                month = date.getMonth(),
                year = date.getFullYear(),
                hour = date.getHours(),
                minute = date.getMinutes(),
                second = date.getSeconds(),
                miliseconds = date.getMilliseconds(),
                h = hour % 12,
                hh = twoDigitPad(h),
                HH = twoDigitPad(hour),
                mm = twoDigitPad(minute),
                ss = twoDigitPad(second),
                aaa = hour < 12 ? 'AM' : 'PM',
                EEEE = dayOfWeekNames[date.getDay()],
                EEE = EEEE.substr(0, 3),
                dd = twoDigitPad(day),
                M = month + 1,
                MM = twoDigitPad(M),
                MMMM = monthNames[month],
                MMM = MMMM.substr(0, 3),
                yyyy = year + "",
                yy = yyyy.substr(2, 2)
                ;
            // checks to see if month name will be used
            patternStr = patternStr
                .replace('hh', hh).replace('h', h)
                .replace('HH', HH).replace('H', hour)
                .replace('mm', mm).replace('m', minute)
                .replace('ss', ss).replace('s', second)
                .replace('S', miliseconds)
                .replace('dd', dd).replace('d', day)

                .replace('EEEE', EEEE).replace('EEE', EEE)
                .replace('yyyy', yyyy)
                .replace('yy', yy)
                .replace('aaa', aaa);
            if (patternStr.indexOf('MMM') > -1) {
                patternStr = patternStr
                    .replace('MMMM', MMMM)
                    .replace('MMM', MMM);
            }
            else {
                patternStr = patternStr
                    .replace('MM', MM)
                    .replace('M', M);
            }
            return patternStr;
        }
        function twoDigitPad(num) {
            return num < 10 ? "0" + num : num;
        }
        function onBtnCancelClick() {
            window.parent.HidePopupAndShowInfo('Cancel', '');
        }
        function onbtnSubmitClick() {
            debugger;
            //var result = s.GetRowKey(e.visibleIndex);
            var deto = ClientValidto.GetValue();
            var defrom = ClientValidfrom.GetValue();
            var result = [formatDate(defrom, 'yyyyMMdd'),
                formatDate(deto, 'yyyyMMdd'),
                tbMaterial.GetText(),
                tbRequestNo.GetText(),
                glQuotation.GetText(),
                checkBoxList.GetValue()].join('|');
            window.parent.HidePopupAndShowInfo('Client', result);
            //s.GetRowValues(e.visibleIndex, 'Material', OnGetRowId); 
            //window.opener.OnCloseSelector(key);
            
        }
        function currentdate() {
            var today = new Date();
            var dd = today.getDate() + 1;
            var mm = today.getMonth() + 1; //January is 0!
            var yyyy = today.getFullYear();
            if (dd < 10) {
                dd = '0' + dd;
            }
            if (mm < 10) {
                mm = '0' + mm;
            }
            var today = dd + '/' + mm + '/' + yyyy;
            return today;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding-top: 30px">
        <dx:ASPxFormLayout runat="server" ID="formLayout" CssClass="formLayout">
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="700" />
            <Items>
                <dx:LayoutGroup ShowCaption="False" ColCount="1" GroupBoxDecoration="None" UseDefaultPaddings="false" Paddings-PaddingTop="10">
                    <GroupBoxStyle>
                        <Caption Font-Bold="true" Font-Size="16" />
                    </GroupBoxStyle>
                                <Items>
                                <dx:LayoutItem Caption="Create From">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <table>
                                                <tr>
                                                    <td><dx:ASPxDateEdit runat="server" ID="defrom" ClientInstanceName="ClientValidfrom" 
                                               DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                                    </dx:ASPxDateEdit></td>
                                                    <td>&nbsp;To&nbsp;</td>
                                                    <td><dx:ASPxDateEdit runat="server" ID="deto" ClientInstanceName="ClientValidto"
                                                        DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy">
                                                    </dx:ASPxDateEdit></td>
                                                </tr>
                                            </table>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Quotation">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxGridLookup ID="glQuotation" runat="server" SelectionMode="Multiple" DataSourceID="dsQuotation" ClientInstanceName="glQuotation"
                                                KeyFieldName="Id" TextFormatString="{0}" MultiTextSeparator=", ">
                                                <Columns>
                                                    <dx:GridViewCommandColumn ShowSelectCheckbox="True" />
                                                    <dx:GridViewDataColumn FieldName="RequestNo" />
                                                    <dx:GridViewDataDateColumn FieldName="CreateOn" />
                                                    <dx:GridViewDataColumn FieldName="user_name" />
                                                </Columns>
                                                <GridViewProperties>
                                                    <Settings ShowFilterRow="true" ShowStatusBar="Visible" AutoFilterCondition="Contains"/>
                                                    <SettingsPager PageSize="7" EnableAdaptivity="true" />
                                                </GridViewProperties>
                                            </dx:ASPxGridLookup>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Material">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxTextBox runat="server" ID="tbMaterial" ClientInstanceName="tbMaterial"/>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Costing">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxTokenBox runat="server" ID="tbRequestNo" ClientInstanceName="tbRequestNo"/>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxRadioButtonList  ID="checkBoxList" runat="server" DataSourceID="dschecklist"
                                                ValueField="idx" TextField="value" RepeatColumns="4" RepeatLayout="Flow" >
                                                <CaptionSettings Position="Top" />
                                            </dx:ASPxRadioButtonList>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
 
                                <dx:LayoutItem Caption="">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <div class="buttonsContainer">
                                                <dx:ASPxButton ID="btnSubmit" runat="server" CssClass="submitButton" Text="Submit" Width="100">
                                                    <ClientSideEvents Click="onbtnSubmitClick" />
                                                </dx:ASPxButton>
                                                <dx:ASPxButton ID="btnCancel" runat="server" CssClass="cancelButton" Text="Cancel" AutoPostBack="false" Width="100">
                                                    <ClientSideEvents Click="onBtnCancelClick" />
                                                </dx:ASPxButton>
                                            </div>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
     
                            </Items>
                        </dx:LayoutGroup>
                    </Items>
                </dx:ASPxFormLayout>
        </div>
    </form>
    <asp:SqlDataSource ID="dsQuotation" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select Id,RequestNo,CreateOn,(select firstname +' '+lastname as fn from ulogin where user_name= CreateBy) as user_name from TransQuotationHeader">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="dschecklist" runat="server" ConnectionString="<%$ ConnectionStrings:CostingConnectionString %>"
        SelectCommand="select * from dbo.FNC_SPLIT('Send Complete,In Complete',',')">
    </asp:SqlDataSource>
 
</body>
</html>
