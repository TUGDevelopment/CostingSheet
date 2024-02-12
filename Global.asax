<%@ Application Language="C#" %>
<script RunAt="server">
    void Application_PreRequestHandlerExecute(object sender, EventArgs e) {
        var t =   Utils.CurrentTheme;
        DevExpress.Web.ASPxWebControl.GlobalTheme = t;
    }
    void Application_Start(Object sender, EventArgs e) {
        DevExpress.Web.Internal.DemoUtils.RegisterDemo("WebMailClient");
        DevExpress.Web.ASPxWebControl.CallbackError += new EventHandler(CallbackError);
    }

    void CallbackError(object sender, EventArgs e) {
        // Logging exceptions occur on callback events of DevExpress ASP.NET controls. 
        // To learn more, see http://www.devexpress.com/Support/Center/Example/Details/E2398      
    }
</script>