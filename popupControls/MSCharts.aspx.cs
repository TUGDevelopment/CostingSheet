using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
// Use the `FusionCharts.Charts` namespace to be able to use classes and methods required to create charts.
using FusionCharts.Charts;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

public partial class DBExample_MSCharts : System.Web.UI.Page
{
    MyDataModule cs = new MyDataModule();
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        string value =  Request.QueryString["id"];
        //Literal1.Text = value;
        //Response.Write(value);
        if(Request.QueryString["view"] == "4")
            fusioncharts(value);
        //BindData(value);
        else
        Wizard1.PreRender += new EventHandler(Wizard1_PreRender);
        //if (!IsPostBack) 
    }
    protected void Wizard1_PreRender(object sender, EventArgs e)
    {
        Repeater SideBarList = Wizard1.FindControl("HeaderContainer").FindControl("SideBarList") as Repeater;
        SideBarList.DataSource = Wizard1.WizardSteps;
        SideBarList.DataBind();

    }
    private void AddSteps()
    {
        List<WizardStep> steps = (List<WizardStep>)Session["some"];
        foreach (var step in steps)
        {
            Wizard1.WizardSteps.Add(step);
        }
        int o = 0;
        string value = Request.QueryString["id"];
        string active = cs.ReadItems(@"select count(*)d from TransApprove where tablename in ('0','3') and StatusApp=1 and RequestNo=" + string.Format("{0}", value));
        int.TryParse(active.ToString(), out o);
        Wizard1.ActiveStepIndex = o - 1;
    }
    protected void Page_PreInit(object sender, EventArgs e)
    {
        string value = Request.QueryString["id"];
        List<WizardStep> steps = new List<WizardStep>();
        DataTable dt = cs.builditems(@"select * from TransApprove where tablename in ('0','3') and RequestNo=" + string.Format("{0}", value));
        DataRow[] rows = dt.Select("StatusApp=1");
        if (rows.Length==0)
            return; 
        if (dt.Rows.Count == 0)
            return;
        foreach(DataRow dr in dt.Rows)
        {
            WizardStep Step1 = new WizardStep();
            Step1.Title = dr["fn"].ToString();
            steps.Add(Step1);
        }
        //WizardStep Step1 = new WizardStep();
        //Step1.Title = "some";
        //steps.Add(Step1);

        //WizardStep Step2 = new WizardStep();
        //Step2.Title = "some2";
        //steps.Add(Step2);

        //WizardStep Step3 = new WizardStep();
        //Step3.Title = "some3";
        //steps.Add(Step3);
        Session["some"] = steps;
        AddSteps();
    }

    protected string GetClassForWizardStep(object wizardStep)
    {
        WizardStep step = wizardStep as WizardStep;

        if (step == null)
        {
            return "";
        }
        int stepIndex = Wizard1.WizardSteps.IndexOf(step);

        if (stepIndex < Wizard1.ActiveStepIndex)
        {
            return "prevStep";
        }
        else if (stepIndex > Wizard1.ActiveStepIndex)
        {
            return "nextStep";
        }
        else
        {
            return "currentStep";
        }
    }
    protected void BindData(string value)
    {
        //DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        //ds.ReadXml(Server.MapPath("EmployeeDetails.xml"));
        SqlParameter[] param = {new SqlParameter("@Id", string.Format("{0}", value)) };
        DataTable table = new DataTable();
        dt = cs.GetRelatedResources("spGetWF", param);

        grid.DataSource = dt;
        grid.DataBind();
    }
    void fusioncharts(string value)
    {
        // Create the `jsonData` StringBuilder object to store the data fetched 
        //from the database as a string.
        SqlParameter[] param = { new SqlParameter("@Keyword", string.Format("{0}", "X")),
        new SqlParameter("@UserNo", string.Format("{0}", User.Identity.Name.Replace(@"THAIUNION\", @"")))};
        DataTable table = cs.GetRelatedResources("spSummaryReport", param);
        StringBuilder jsonData = new StringBuilder();
        bool ReqDatasetComma = false, ReqComma = false;

        // Initialize the chart-level attributes and append them to the 
        //`jsonData` StringBuilder object.
        jsonData.Append("{" +
            "'chart': {" +
                // add chart level attrubutes
                "'caption': 'WF Output report'," +
                "'subCaption': 'By Quantity'," +
                "'formatNumberScale': '0'," +
                "'rotatelabels': '1'," +
                "'showvalues': '0'," +
                "'showBorder': '1'" +
            "},");

        // Initialize the Categories object.
        jsonData.Append("'categories': [" +
            "{" +
                "'category': [");
        var distinctNames = (from row in table.AsEnumerable()
                             select row.Field<string>("CD")).Distinct();
        foreach (var name in distinctNames) { 
                if (ReqComma)
                {
                    jsonData.Append(",");
                }
                else
                {
                    ReqComma = true;
                }
                jsonData.AppendFormat("{{" +
                        // category level attributes
                        "'label': '{0}'" +
                    "}}", cs.ReadItems(@"select FirstName +' '+ substring(LastName,1,1) from ulogin where [user_name]='"+ name +"'"));
        }
        //Close the database connection.
        //Close the catgories object.
        jsonData.Append("]" +
                "}" +
            "],");
        // Initialize the Dataset object.
        jsonData.Append("'dataset': [");
        //Fetch all details for the three factories from the Factory_Master table
        // and store the result in the `factoryquery2` variable.
        string factoryquery2 = "select ID,fn from TransApprove where RequestNo="+ string.Format("{0}", value);
        //Establish the database connection.
        using (SqlConnection con = new SqlConnection(strConn))
        {
            SqlCommand command = new SqlCommand(factoryquery2, con);
            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            // Iterate through the results in the `factoryquery2` variable to fetch the 
            // factory  name and factory id.
            while (reader.Read())
            {
                if (ReqDatasetComma)
                {
                    jsonData.Append(",");
                }
                else
                {
                    ReqDatasetComma = true;
                }
                // Append the factory name as the value for the `seriesName` attribute.
                jsonData.AppendFormat("{{" +
                        // dataset level attributes
                        "'seriesname': '{0}'," +
                        "'data': [", reader.GetString(1).ToString());
                // Based on the factory id, fetch the quantity produced by each factory on each day 
                // from the factory_output table.
                // Store the results in the `factoryquery3` string object.
                //string factoryquery3 = "select convert(nvarchar(max),levelApp)q from TransApprove where RequestNo=152 and ID=" + reader.GetInt32(0);
                DataTable dt = cs.builditems(@"select convert(nvarchar(max),isnull(levelApp,0))q from TransApprove where tablename in ('0','3')
                and RequestNo=" + string.Format("{0}", value));
                //Establish the database connection.
                    ReqComma = false;
                foreach (DataRow dr in dt.Rows)
                {
                    // Iterate through the results in the `factoryquery3` object and fetch the quantity details 
                    // for each factory.
                    // Append the quantity details as the the value for the `<set>` element.
                    if (ReqComma)
                    {
                        jsonData.Append(",");
                    }
                    else
                    {
                        ReqComma = true;
                    }
                    jsonData.AppendFormat("{{" +
                        // data set attributes
                        "'value': '{0}'" +
                    "}}", dr[0].ToString());
                }
                // Close individual dataset object.
                jsonData.Append("]" +
                    "}");
            }
            // Close the database connection.
            con.Close();
        }
        // Close the JSON object.
        jsonData.Append("]" +
            "}");

        // Initialize chart - MSLine Chart data pulling from database
        Chart factoryOutput = new Chart("stackedbar3d", "myChart", "600", "350", "json", jsonData.ToString());
        // Render the chart
        Literal1.Text = factoryOutput.Render();
    }

    protected void Wizard1_ActiveStepChanged(object sender, EventArgs e)
    {
        if (Wizard1.ActiveStepIndex == 1)
        {
            Wizard1.StepNextButtonStyle.CssClass = "hidden";
        }
        else
        {
            Wizard1.StepPreviousButtonStyle.CssClass = "visible";
            Wizard1.StepNextButtonStyle.CssClass = "visible";
        }
    }
}
