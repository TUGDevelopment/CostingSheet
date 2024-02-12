<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MSCharts.aspx.cs" Inherits="DBExample_MSCharts" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>FusionCharts - Simple</title>
    <!-- FusionCharts script tag -->
    <script type="text/javascript" src="../fusioncharts/fusioncharts.js"></script>
    <!-- End -->
    <style type="text/css">
    .header   {
         background-color:#3E3E3E;        
         font-family:Calibri;
         color:White;
         text-align:center;  
         line-height:25px;     
    }
    .rowstyle    {             
         font-family:Calibri;
         line-height:25px;
    }
    #wizHeader li .prevStep
    {
        background-color: #669966;
    }
    #wizHeader li .prevStep:after
    {
        border-left-color:#669966 !important;
    }
    #wizHeader li .currentStep
    {
        background-color: #C36615;
    }
    #wizHeader li .currentStep:after
    {
        border-left-color: #C36615 !important;
    }
    #wizHeader li .nextStep
    {
        background-color:#C2C2C2;
    }
    #wizHeader li .nextStep:after
    {
        border-left-color:#C2C2C2 !important;
    }
    #wizHeader
    {
        list-style: none;
        overflow: hidden;
        font: 18px Helvetica, Arial, Sans-Serif;
        margin: 0px;
        padding: 0px;
    }
    #wizHeader li
    {
        float: left;
    }
    #wizHeader li a
    {
        color: white;
        text-decoration: none;
        padding: 10px 0 10px 55px;
        background: brown; /* fallback color */
        background: hsla(34,85%,35%,1);
        position: relative;
        display: block;
        float: left;
    }
    #wizHeader li a:after
    {
        content: " ";
        display: block;
        width: 0;
        height: 0;
        border-top: 50px solid transparent; /* Go big on the size, and let overflow hide */
        border-bottom: 50px solid transparent;
        border-left: 30px solid hsla(34,85%,35%,1);
        position: absolute;
        top: 50%;
        margin-top: -50px;
        left: 100%;
        z-index: 2;
    }
    #wizHeader li a:before
    {
        content: " ";
        display: block;
        width: 0;
        height: 0;
        border-top: 50px solid transparent;
        border-bottom: 50px solid transparent;
        border-left: 30px solid white;
        position: absolute;
        top: 50%;
        margin-top: -50px;
        margin-left: 1px;
        left: 100%;
        z-index: 1;
    }        
    #wizHeader li:first-child a
    {
        padding-left: 10px;
    }
    #wizHeader li:last-child 
    {
        padding-right: 50px;
    }
    #wizHeader li a:hover
    {
        background: #FE9400;
    }
    #wizHeader li a:hover:after
    {
        border-left-color: #FE9400 !important;
    }        
    .content
    {
        height:150px;           
        padding-top:75px;
        text-align:center;
        background-color:#F9F9F9;
        font-size:48px;
    }
    .hidden { display: none; }
    .visible { display: inline; }
    .image {
            max-width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align:center">
<%--        <asp:Wizard ID="Wizard1" runat="server" DisplaySideBar="false">
           <WizardSteps>
               <asp:WizardStep ID="WizardStep1" runat="server" Title="Step 1">
                <div class="content">This is Step 1</div>
               </asp:WizardStep>
               <asp:WizardStep ID="WizardStep2" runat="server" Title="Step 2">
                   <div class="content">This is Step 2</div>
               </asp:WizardStep>
               <asp:WizardStep ID="WizardStep3" runat="server" Title="Step 3">
                  <div class="content">This is Step 3</div>
               </asp:WizardStep>
               <asp:WizardStep ID="WizardStep4" runat="server" Title="Step 4">
                   <div class="content">This is Step 4</div>
               </asp:WizardStep>
           <asp:WizardStep ID="WizardStep5" runat="server" Title="Step 5">
                   <div class="content">This is Step 5</div>
               </asp:WizardStep>
          </WizardSteps>
           <HeaderTemplate>
               <ul id="wizHeader">
                   <asp:Repeater ID="SideBarList" runat="server">
                       <ItemTemplate>
                           <li><a class="<%# GetClassForWizardStep(Container.DataItem) %>" title="<%#Eval("Name")%>">
                               <%# Eval("Name")%></a> </li>
                       </ItemTemplate>
                   </asp:Repeater>
               </ul>
           </HeaderTemplate>
       </asp:Wizard>--%>
        <asp:Wizard ID="Wizard1" runat="server" DisplaySideBar="false" OnActiveStepChanged="Wizard1_ActiveStepChanged">
            <WizardSteps>
            </WizardSteps>
            <StartNavigationTemplate>
                <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Visible="false"
                Text="Next" />
            </StartNavigationTemplate>
            <StepNavigationTemplate>
            <asp:Button ID="StepPreviousButton" runat="server" CausesValidation="False"
                CommandName="MovePrevious" Text="Previous" Visible="False" />
            <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Visible="false"
                Text="Next" />
        </StepNavigationTemplate>
   <FinishNavigationTemplate>
         <asp:Button ID="StepPreviousButton" runat="server" CausesValidation="False"
                CommandName="MovePrevious" Text="Previous" Visible="False" />
            <asp:Button ID="Finish" runat="server" CommandName="MoveComplete" Visible="false"
                Text="Finish" />
        </FinishNavigationTemplate>
            <HeaderTemplate>
               <ul id="wizHeader">
                   <asp:Repeater ID="SideBarList" runat="server">
                       <ItemTemplate>
                           <li><a class="<%# GetClassForWizardStep(Container.DataItem) %>" title="<%#Eval("Name")%>">
                               <%# Eval("Name")%></a> </li>
                       </ItemTemplate>
                   </asp:Repeater>
               </ul>
           </HeaderTemplate>
        </asp:Wizard>
        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
        <dx:aspxgridview id="grid" runat="server" Visible="false" 
            Settings-ShowGroupPanel="true"></dx:aspxgridview>         
    </div>
    </form>
</body>
</html>
