using System;
using System.IO;
using System.Configuration;
using System.Data;
using DevExpress.Web;
using DevExpress.Web.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Collections.Specialized;

public partial class EditForm : MasterUserControl
{
    myClass myClass = new myClass();
    MyDataModule cs = new MyDataModule();
    string user_name = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
    string strConn = ConfigurationManager.ConnectionStrings["CostingConnectionString"].ConnectionString;
    string FilePath
    {
        get { return Page.Session["FilePath"] == null ? String.Empty : Page.Session["FilePath"].ToString(); }
        set { Page.Session["FilePath"] = value; }
    }
    private DataTable dataTable
    {
        get { return Page.Session["sessionKey"] == null ? null : (DataTable)Page.Session["sessionKey"]; }
        set { Page.Session["sessionKey"] = value; }
    }
    string selectedDataSource
    {
        get { return Page.Session["selectedDataSource"] == null ? String.Empty : Page.Session["selectedDataSource"].ToString(); }
        set { Page.Session["selectedDataSource"] = value; }
    }
    protected string SearchText { get { return Utils.GetSearchText(Page); } }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            Session.Clear();
            this.Session["user_name"] = user_name;
            this.Session["viewMode"] = Request.QueryString["viewMode"];// cs.user();
            //selectedDataSource = string.Format("{0}", Request.QueryString["role"]);
            //string strSQL = @"select Bu from ulogin where [user_name]='" + user_name + "'";
            hBu["BU"] = cs.GetData(user_name, "oBU");
            var edit = cs.GetData(user_name, "userlevel");
            usertp["usertype"] = string.Format("{0}", cs.GetData(user_name, "usertype"));
            //editor["Name"] = edit == "1" || edit =="2" ? true : false;
        }
        //int keys;
        //if (int.TryParse(Request.QueryString["viewMode"], out keys))
        //        Session["selectedDataSource"] = keys; 
        //Session["selectedDataSource"] = string.Format("{0}", Request.QueryString["viewMode"]);
        Update();
    }
    public override void Update()
    {
        //gridData.DataSource = dsgv;
        //DataTable dt = ((DataView)dsgv.Select(DataSourceSelectArguments.Empty)).Table;
        grid.DataBind();
    }
    protected void grid_DataBinding(object sender, EventArgs e)
    {
        //object o = selectedDataSource;
        //switch (string.Format("{0}", o))
        //{
        //    case "StdFillWeightMedia":
        //        (sender as ASPxGridView).KeyFieldName = "ID;Code;GroupType";
        //        break;
        //    default:
        //        (sender as ASPxGridView).KeyFieldName = "ID";
        //        break;
        //}
        (sender as ASPxGridView).DataSource = GetDataSource();
        AddColumns();
    }
    protected void grid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
    {
        DataRow dr = grid.GetDataRow(grid.FocusedRowIndex);
        //Session["selectedDataSource"] = Int32.Parse(e.Parameters);
        if (string.IsNullOrEmpty(e.Parameters))
            return;
        var args = e.Parameters.Split('|');
        if (args[0] == "Search")
        {
            string filter = args[0] == "Search" ? SearchText : string.Empty;
            //gridData.SearchPanelFilter = filter;
            //gridData.ExpandAll();
            if (filter == "")
            {
                grid.FilterExpression = "";
            }
            else
            {
                string fExpr = "";
                foreach (GridViewColumn column in grid.Columns)
                {
                    if (column is GridViewDataColumn)
                    {
                        if (fExpr == "")
                        {
                            fExpr = "[" + (column as GridViewDataColumn).FieldName + "] Like '%" + filter + "%'";
                        }
                        else
                        {
                            fExpr = fExpr + "OR " + "[" + (column as GridViewDataColumn).FieldName + "] Like '%" + filter + "%'";
                        }
                    }
                }
                grid.FilterExpression = fExpr;
            }
        }
        else if (args[0] == "reload")
        {
            selectedDataSource = string.Format("{0}", args[1].ToString());
        }
        else
        {
            grid.Columns.Clear();
            grid.AutoGenerateColumns = false;
        }
        grid.DataBind();
    }
    private SqlDataSource GetDataSource()
    {
        string strvalue = "";//string.Format(@"'{0}' in (select value from dbo.FNC_SPLIT(usertype,','))", usertp["usertype"]);
        strvalue=string.Format(@"(dbo.fnc_checktype(usertype,'{0}')> 0)", usertp["usertype"]);
        object o = selectedDataSource;
        switch (string.Format("{0}", o))
        {
            case "ProductGroup":
                lab.SelectCommand = "select ID,ProductGroup,Name from tblProductGroup where GroupType='F'";
                return lab;
            case "ContainerLid":
                lab.SelectCommand = "select ID,Code,ContainerType,LidType,ProductGroup,SUBSTRING(Code,1,1) as 'PrimaryType', ProductType from transContainerLid where ProductType='PF'";
                lab.UpdateCommand = "Update transContainerLid set ContainerType=@ContainerType,LidType=@LidType,Code=@Code where Id=@id";
                lab.InsertCommand = "insert into transContainerLid values(@Code,@ContainerType,@LidType,@ProductGroup,'PF','',null,getdate(),null,null,null,null)";
                lab.DeleteCommand = "Delete transContainerLid Where ID=@ID";
                return lab;
            case "Grade":
                lab.SelectCommand = "select ID,Code,Description,IsActive,ProductGroup from transGrade where ProductType='PF'";
                lab.UpdateCommand = "Update transGrade set Description=@Description,Code=@Code where Id=@id";
                lab.InsertCommand = "insert into transGrade values('',@Code,@Description,@ProductGroup,'PF',getdate(),null,null,null,null)";
                lab.DeleteCommand = "Delete transGrade Where ID=@ID";
                return lab;
            case "Brand":
                lab.SelectCommand = "select ID,Code,Description,IsActive from transCustomerBrand where ProductType='PF'";
                lab.UpdateCommand = "Update transCustomerBrand set Description=@Description,Code=@Code where Id=@id";
                lab.InsertCommand = "insert into transCustomerBrand values('',@Code,@Description,@ProductGroup,'PF',getdate(),null,null,null,null)";
                lab.DeleteCommand = "Delete transCustomerBrand Where ID=@ID";
                return lab;
            case "Style":
                lab.SelectCommand = "SELECT ID,ProductGroup,Code,Description,IsActive from transProductStyle where ProductType='PF'";
                lab.UpdateCommand = "Update transProductStyle set Description=@Description,Code=@Code where Id=@id";
                lab.InsertCommand = "insert into transProductStyle values('',@Code,@Description,@ProductGroup,'PF',getdate(),null,null,null,null)";
                lab.DeleteCommand = "Delete transProductStyle Where ID=@ID";
                return lab;
            case "RawMaterial":
                lab.SelectCommand = "SELECT ID,ProductGroup, ProductType,Code,Description,IsActive from transRawMaterial where ProductType='PF'";
                lab.UpdateCommand = "Update transRawMaterial set ProductGroup=@ProductGroup,Description=@Description,Code=@Code where Id=@id";
                lab.InsertCommand = "insert into transRawMaterial values(@ProductGroup,'PF','',@Code,@Description,getdate(),null,null,null)";
                lab.DeleteCommand = "Delete transRawMaterial Where ID=@ID";
                return lab;
            case "MediaType":
                lab.SelectCommand = "SELECT ID,MediaType,ProductGroup,Code,Description,ProductType,substring(Code,1,1) as Groups,'' as BaseType,IsActive from transMediaType where ProductType='PF'";
                lab.UpdateCommand = "Update transMediaType set ProductGroup=@ProductGroup,Description=@Description,Code=@Code where Id=@id";
                lab.InsertCommand = "insert into transMediaType values(@ProductGroup,@Code,@MediaType)";
                lab.DeleteCommand = "Delete transMediaType Where ID=@ID";
                return lab;
            case "CanSize":
                lab.SelectCommand = "select ID,Packaging,ProductGroup,Code,CanSize,PouchWidth,Type,NW,DWeight,ProductType,'' as BaseType,Media,NutritionType from transCanSize where ProductType='PF'";
                lab.InsertCommand = "insert into transCanSize (ProductGroup,Code,CanSize,PouchWidth,Type,NW,DWeight,Packaging,ProductType,IsActive,Media,NutritionType) values(@ProductGroup,@Code,@CanSize,@PouchWidth,@Type,@NW,@DWeight,@Packaging,@ProductType,1,@Media,@NutritionType)";
                lab.InsertCommand = "update transCanSize set NW=@NW,IsActive=@IsActive,Media=@Media,NutritionType=@NutritionType Where ID=@ID";
                lab.DeleteCommand = "Delete transCanSize Where ID=@ID";
                return lab;
            case "HierarchyBrand":
                lab.SelectCommand = "SELECT ID, Code, Name from HierBrand";
                lab.UpdateCommand = "Update HierBrand set Name=@Name,Code=@Code where ID=@ID";
                lab.InsertCommand = "insert into HierBrand values(@Code,@Name)";
                lab.DeleteCommand = "Delete HierBrand Where ID=@ID";
                return lab;
            case "HierarchyContainer":
                lab.SelectCommand = "SELECT ID, Code, Name ,Digit,ContainerType from HierContainer";
                lab.UpdateCommand = "Update HierContainer set Name=@Name,Code=@Code,Digit=@Digit,ContainerType=@ContainerType where ID=@ID";
                lab.InsertCommand = "insert into HierContainer values(@Code,@Name,@Digit,@ContainerType)";
                lab.DeleteCommand = "Delete HierContainer Where ID=@ID";
                return lab;
            case "HierarchyCorporate":
                lab.SelectCommand = "SELECT ID, Code, Name from HierCorporate";
                lab.UpdateCommand = "Update HierCorporate set Name=@Name,Code=@Code where ID=@ID";
                lab.InsertCommand = "insert into HierCorporate values(@Code,@Name)";
                lab.DeleteCommand = "Delete HierCorporate Where ID=@ID";
                return lab;
            case "HierarchyCustomer":
                lab.SelectCommand = "SELECT ID, Code, Name, Brand, ProductGroup from HierCustomer";
                lab.UpdateCommand = "Update HierCustomer set Name=@Name,Code=@Code,Brand=@Brand,ProductGroup=@ProductGroup where ID=@ID";
                lab.InsertCommand = "insert into HierCustomer values(@Code,@Name,@Brand,@ProductGroup)";
                lab.DeleteCommand = "Delete HierCustomer Where ID=@ID";
                return lab;
            case "HierarchyDivision":
                lab.SelectCommand = "SELECT ID, PH2, PH2Des,PDGROUP,PRODUCT_LINE,DIVISION from HierDivision";
                lab.UpdateCommand = "Update HierDivision set PH2=@PH2, PH2Des=@PH2Des,PDGROUP=@PDGROUP,PRODUCT_LINE=@PRODUCT_LINE,DIVISION=@DIVISION where ID=@ID";
                lab.InsertCommand = "insert into HierDivision values(@PH2, @PH2Des,@PDGROUP,@PRODUCT_LINE,@DIVISION)";
                lab.DeleteCommand = "Delete HierDivision Where ID=@ID";
                return lab;
            case "HierarchyMedia":
                lab.SelectCommand = "SELECT ID, Code, Name, Digit, Packaging, ProductGroup, Commercial from HierMedia";
                lab.UpdateCommand = "Update HierMedia set Name=@Name,Code=@Code,Digit=@Digit, Packaging=@Packaging, ProductGroup=@ProductGroup, Commercial=@Commercial where ID=@ID";
                lab.InsertCommand = "insert into HierMedia values(@Code,@Name,@Digit, @Packaging, @ProductGroup, @Commercial)";
                lab.DeleteCommand = "Delete HierMedia Where ID=@ID";
                return lab;
            case "HierarchyNutrition":
                lab.SelectCommand = "SELECT ID, Code, Name from HierNutrition";
                lab.UpdateCommand = "Update HierNutrition set Name=@Name,Code=@Code where ID=@ID";
                lab.InsertCommand = "insert into HierNutrition values(@Code,@Name)";
                lab.DeleteCommand = "Delete HierNutrition Where ID=@ID";
                return lab;
            case "HierarchyPackaging":
                lab.SelectCommand = "SELECT ID, Code, Name from HierPackaging";
                lab.UpdateCommand = "Update HierPackaging set Name=@Name,Code=@Code where ID=@ID";
                lab.InsertCommand = "insert into HierPackaging values(@Code,@Name)";
                lab.DeleteCommand = "Delete HierPackaging Where ID=@ID";
                return lab;
            case "HierarchyPetType":
                lab.SelectCommand = "SELECT ID, Code, Name from HierPetType";
                lab.UpdateCommand = "Update HierPetType set Name=@Name,Code=@Code where ID=@ID";
                lab.InsertCommand = "insert into HierPetType values(@Code,@Name)";
                lab.DeleteCommand = "Delete HierPetType Where ID=@ID";
                return lab;
            case "HierarchyProductGroup":
                lab.SelectCommand = "SELECT ID, Code, Name,Digit from HierProductGroup";
                lab.UpdateCommand = "Update HierProductGroup set Name=@Name,Code=@Code,Digit=@Digit where ID=@ID";
                lab.InsertCommand = "insert into HierProductGroup values(@Code,@Name,@Digit)";
                lab.DeleteCommand = "Delete HierProductGroup Where ID=@ID";
                return lab;
            case "HierarchyProductLine":
                lab.SelectCommand = "SELECT ID, Code, Name from HierProductLine";
                lab.UpdateCommand = "Update HierProductLine set Name=@Name,Code=@Code where ID=@ID";
                lab.InsertCommand = "insert into HierProductLine values(@Code,@Name)";
                lab.DeleteCommand = "Delete HierProductLine Where ID=@ID";
                return lab;
            case "HierarchyRawMaterials":
                lab.SelectCommand = "SELECT ID, Code, Name,Digit,ProductGroup from HierRawMaterials";
                lab.UpdateCommand = "Update HierRawMaterials set Name=@Name,Code=@Code,Digit=@Digit,ProductGroup=@ProductGroup where ID=@ID";
                lab.InsertCommand = "insert into HierRawMaterials values(@Code,@Name,@Digit,@ProductGroup)";
                lab.DeleteCommand = "Delete HierRawMaterials Where ID=@ID";
                return lab;
            case "HierarchyStyle":
                lab.SelectCommand = "SELECT ID, Code, Name,Digit,ProductGroup from HierStyle";
                lab.UpdateCommand = "Update HierStyle set Name=@Name,Code=@Code,Digit=@Digit,ProductGroup=@ProductGroup where ID=@ID";
                lab.InsertCommand = "insert into HierStyle values(@Code,@Name,@Digit,@ProductGroup)";
                lab.DeleteCommand = "Delete HierStyle Where ID=@ID";
                return lab;
            case "StdPKGStyleSpecial":
                ds.SelectCommand = "SELECT ID,[SAPCodeDigit],[PackSize],[LaborCost],[Currency] FROM StdPKGStyleSpecial";
                ds.UpdateCommand = "Update StdPKGStyleSpecial set PackSize=@PackSize,Currency=@Currency,Currency=@Currency where SAPCodeDigit=@ID";
                ds.InsertCommand = "insert into StdPKGStyleSpecial values(@SAPCodeDigit,@PackSize,@Currency,@Currency)";
                ds.DeleteCommand = "Delete StdPKGStyleSpecial Where ID=@ID";
                return ds;
            case "MasCustomer": case "stdCustomer":
                ds.SelectCommand = "SELECT ID,Code, Name, Custom, Zone from MasCustomer";
                ds.UpdateCommand = "Update MasCustomer set Name=@Name,Custom=@Custom,Zone=@Zone where ID=@ID";
                ds.InsertCommand = "insert into MasCustomer values(@Code,@Name,@Custom,@Zone)";
                ds.DeleteCommand = "Delete MasCustomer Where ID=@ID";
                return ds;
            case "StdAssignZone":
                ds.SelectCommand = "SELECT ID,a.EMPID,(select top 1 concat(b.firstname,' ',b.lastname) from ulogin b where b.[user_name]=a.empid)'name', isnull(a.Zone,'') as 'Zone' from StdAssignZone a";
                ds.UpdateCommand = "Update StdAssignZone set EMPID=@EMPID,Zone=@Zone where ID=@ID";
                ds.InsertCommand = "insert into StdAssignZone values(@EMPID,@Zone)";
                ds.DeleteCommand = "Delete StdAssignZone Where ID=@ID";
                return ds;
            case "StdApprove":
                ds.SelectCommand = @" SELECT ID,(select m.Sublevel from MasApprovAssign m where EmpId=a.empid) as 'Sublevel',Position,
                                    a.EMPID ,(select abc =STUFF(((SELECT DISTINCT  ';' + (select top 1 concat(b.firstname,' ',b.lastname) from ulogin b where b.[user_name]=f.value)
                                    FROM dbo.FNC_SPLIT(a.LevelApprove,';') f FOR XML PATH(''))), 1, 1, '')) as 'LevelApprove' from StdApprove a";
                ds.UpdateCommand = "Update StdApprove set EMPID=@EMPID,Position=@Position,LevelApprove=dbo.fnc_getstduser(@LevelApprove) where ID=@ID";
                ds.InsertCommand = "insert into StdApprove values(@EMPID,@Position,dbo.fnc_getstduser(@LevelApprove))";
                ds.DeleteCommand = "Delete StdApprove Where ID=@ID";
                return ds;
            case "StandardYield":
                ds.SelectCommand = "SELECT * from StandardYield";
                ds.UpdateCommand = "Update StandardYield set FishGroup=@FishGroup,Grading=@Grading,Yield=@Yield where ID=@ID";
                ds.InsertCommand = "insert into StandardYield values(@FishGroup,@Grading,@Yield)";
                ds.DeleteCommand = "Delete StandardYield Where ID=@ID";
                return ds;
            case "StdExchangeRat":
                ds.SelectCommand = "SELECT * from StdExchangeRat";
                ds.UpdateCommand = "Update StdExchangeRat set Validfrom=@Validfrom,Validto=@Validto,Ratio=@Ratio,Currency=@Currency,Rate=@Rate,CurrencyTh=@CurrencyTh where ID=@ID";
                ds.InsertCommand = "insert into StdExchangeRat values(@Validfrom,@Validto,@Ratio,@Currency,@Rate,@CurrencyTh)";
                ds.DeleteCommand = "Delete StdExchangeRat Where ID=@ID";
                return ds;
            case "StdSpcYield":
                return ds;
            case "StdTunaPrice":
                ds.SelectCommand = "SELECT ID,GroupType, GroupDescription, Code, Description, Currency, Unit, Price, [From], [To] FROM StdTunaPrice";
                ds.UpdateCommand = "Update StdTunaPrice set Description=@Description,Currency=@Currency,Unit=@Unit,Price=@Price,GroupType=@GroupType,GroupDescription=@GroupDescription,Code=@Code,[From]=@From,[To]=@To where ID =@ID";
                ds.InsertCommand = "insert into StdTunaPrice values( @GroupType,@GroupDescription,@Code,@Description,@Currency,@Unit,@Price,@From,@To)";
                ds.DeleteCommand = "Delete StdTunaPrice Where ID=@ID";
                return ds;
            case "StdOverheadCost":
                ds.SelectCommand = "SELECT ID, PackagingType,CanSize,Size,Title,Cost,Currency,Unit,[From],[To],IsActive,StdPackSize from StdOverheadCost";
                ds.UpdateCommand = "Update StdOverheadCost set PackagingType=@PackagingType,CanSize=@CanSize,Size=@Size,Title=@Title,Cost=@Cost,Currency=@Currency,Unit=@Unit,IsActive=@IsActive where ID=@ID";
                ds.InsertCommand = "insert into StdOverheadCost values(@PackagingType,@CanSize,@Size,@Title,@Cost,@Currency,@Unit,@From,@To,0,@StdPackSize)";
                ds.DeleteCommand = "Delete StdOverheadCost Where ID=@ID";
                return ds;
            case "StdLossPrimaryPKG":
                return ds;
            case "StdPackingStyle":
                ds.SelectCommand = "SELECT concat(SAPCodeDigit,'|',Size) as ID,SAPCodeDigit, Size, GroupStyle, StyleRef, Description, PackSize, LaborCost, SecPKGCost from StdPackingStyle";
                ds.UpdateCommand = "Update StdPackingStyle set GroupStyle=@GroupStyle,StyleRef=@StyleRef,Description=@Description, PackSize=@PackSize, LaborCost=@LaborCost, SecPKGCost=@SecPKGCost where ID=@ID and Size=@Size ";
                ds.InsertCommand = "insert into StdPackingStyle values(@SAPCodeDigit,@Size,@GroupStyle,@StyleRef,@Description, @PackSize, @LaborCost, @SecPKGCost)";
                ds.DeleteCommand = "spDelStdPackingStyle";
                ds.DeleteCommandType = SqlDataSourceCommandType.StoredProcedure;
                return ds;
            case "StdPackingStyle2":
                ds.SelectCommand = "SELECT concat(SAPCodeDigit,'|',Size) as ID,SAPCodeDigit, Size, GroupStyle, StyleRef, Description, PackSize, LaborCost, SecPKGCost from StdPackingStyle2";
                ds.UpdateCommand = "Update StdPackingStyle2 set GroupStyle=@GroupStyle,StyleRef=@StyleRef,Description=@Description, PackSize=@PackSize, LaborCost=@LaborCost, SecPKGCost=@SecPKGCost where ID=@ID and Size=@Size ";
                ds.InsertCommand = "insert into StdPackingStyle2 values(@SAPCodeDigit,@Size,@GroupStyle,@StyleRef,@Description, @PackSize, @LaborCost, @SecPKGCost)";
                ds.DeleteCommand = "spDelStdPackingStyle2";
                ds.DeleteCommandType = SqlDataSourceCommandType.StoredProcedure;
                return ds;
            case "StdSecPackingCost":
                return ds;
            case "StdPKGLaborCost":
                return ds;
            case "StdLossSecPKG":
                return ds;
            case "StandardUpcharge":
                ds.SelectCommand = "select ID, UpchargeGroup, Upcharge,[Value], Currency, Unit, StdPackSize,SapDigit from StandardUpcharge";
                ds.UpdateCommand = "Update StandardUpcharge set UpchargeGroup=@UpchargeGroup,Upcharge=@Upcharge,[Value]=@Value,Currency=@Currency,Unit=@Unit,StdPackSize=@StdPackSize,SapDigit=@SapDigit where ID=@ID";
                ds.InsertCommand = "insert into StandardUpcharge values(@UpchargeGroup,@Upcharge,@Value,@Currency,@Unit,@StdPackSize,@SapDigit)";
                ds.DeleteCommand = "Delete StandardUpcharge Where ID=@ID";
                return ds;
            case "StdCustomTitle":
                ds.SelectCommand = "SELECT ID, Material, Description, CustomTitle from StdCustomTitle";
                ds.UpdateCommand = "Update StdCustomTitle set Description=@Description,CustomTitle=@CustomTitle where ID=@ID";
                ds.InsertCommand = "insert into StdCustomTitle values(@Material,@Description,@CustomTitle)";
                ds.DeleteCommand = "Delete StdCustomTitle Where ID=@ID";
                return ds;
            case "StdCertificate":
                ds.SelectCommand = "SELECT ID, FishGroup, Certificate_fee, free from StdCertificate";
                ds.UpdateCommand = "Update StdCertificate set FishGroup=@FishGroup,Certificate_fee=@Certificate_fee,free=@free where ID=@ID";
                ds.InsertCommand = "insert into StdCertificate values(@FishGroup,@Certificate_fee,@free)";
                ds.DeleteCommand = "Delete StdCertificate Where ID=@ID";
                return ds;
            case "StdTunaSpeCase":
                ds.SelectCommand = "SELECT ID,MatCode, IsActive from StdTunaSpeCase";
                ds.UpdateCommand = "Update StdTunaSpeCase set IsActive=@IsActive where ID=@ID";
                ds.InsertCommand = "insert into StdTunaSpeCase values(@MatCode,@IsActive)";
                ds.DeleteCommand = "Delete StdTunaSpeCase Where ID=@ID";
                return ds;
            case "StdFillWeightMedia":
                //ds.SelectCommand = "SELECT CONCAT(SAPCodeDigit,'|',GroupType,'|',Code) as ID,SAPCodeDigit, Name, GroupType, GroupDescription, Code, CodeName, MediaWeight, Unit from StdFillWeightMedia";
                //ds.UpdateCommand = "Update StdFillWeightMedia set Name=@Name,GroupDescription=@GroupDescription,CodeName=@CodeName,MediaWeight=@MediaWeight,Unit=@Unit where ID=@ID and GroupType=@GroupType and Code=@Code";
                //ds.InsertCommand = "insert into StdFillWeightMedia values(@SAPCodeDigit,@Name,@GroupType,@GroupDescription,@Code,@CodeName,@MediaWeight,@Unit)";
                //ds.DeleteCommand = "Delete StdFillWeightMedia Where SAPCodeDigit=@ID and GroupType=@GroupType and Code=@Code";
                return dsStdFillWeightMedia;
            case "StandardPrimary":
                ds.SelectCommand = "SELECT concat(SAPCodeDigit,'|',Code) as ID,SAPCodeDigit, MatDescription, [Group], GroupDescription, Code, Description, Can_Size from StandardPrimary";
                ds.UpdateCommand = "Update StandardPrimary set MatDescription=@MatDescription,[Group]=@Group,GroupDescription=@GroupDescription,Description=@Description,Can_Size=@Can_Size where SAPCodeDigit=@SAPCodeDigit and Code=@Code";
                ds.InsertCommand = "insert into StandardPrimary values(@SAPCodeDigit,@MatDescription,@Group,@GroupDescription,@Code,@Description,@Can_Size)";
                ds.DeleteCommand = "spDelStandardPrimary";
                ds.DeleteCommandType= SqlDataSourceCommandType.StoredProcedure; 
                return ds;
            case "StdTunaFixFW":
                ds.SelectCommand = "SELECT ID,Material, Description, FishGroup, FishCert, SHD, FillWeight, Unit from StdTunaFixFW";
                ds.UpdateCommand = "Update StdTunaFixFW set FillWeight=@FillWeight,Description=@Description,Unit=@Unit where ID=@ID";
                ds.InsertCommand = "insert into StdTunaFixFW values(@Material,@Description,@FishGroup,@FishCert,@SHD,@FillWeight,@Unit)";
                ds.DeleteCommand = "Delete StdTunaFixFW Where ID=@ID";
                return ds;
            case "StdSecPKGCost":
                //ds.SelectCommand = "SELECT Material as ID,Material, Customer, ShipTo, Amount, Currency, Unit from StdSecPKGCost";
                //ds.UpdateCommand = "Update StdSecPKGCost set SAPCodeDigit=@SAPCodeDigit,Description=@Description,PackagingType=@PackagingType where ID=@ID and Customer=@Customer and ShipTo=@ShipTo";
                //ds.InsertCommand = "insert into StdSecPKGCost values(@Material,@Customer,@ShipTo,@Amount,@Currency,@Unit)";
                //ds.DeleteCommand = "Delete StdSecPKGCost Where ID=@ID and Customer=@Customer and ShipTo=@ShipTo";
                return dsStdSecPKGCost;
            case "StdSAPPackaging":
                ds.SelectCommand = "SELECT Id, SAPCodeDigit, Description, PackagingType from StdSAPPackaging";
                ds.UpdateCommand = "Update StdSAPPackaging set SAPCodeDigit=@SAPCodeDigit,Description=@Description,PackagingType=@PackagingType where ID=@ID";
                ds.InsertCommand = "insert into StdSAPPackaging values(@SAPCodeDigit,@Description,@PackagingType)";
                ds.DeleteCommand = "Delete StdSAPPackaging Where ID=@ID";
                return ds;
            case "stdGrading":
                ds.SelectCommand = "SELECT ID,SAPCodeDigit, Grading from MasGrading2";
                ds.UpdateCommand = "Update MasGrading2 set Grading=@Grading where ID=@ID";
                ds.InsertCommand = "insert into MasGrading2 values(@SAPCodeDigit,@Grading)";
                ds.DeleteCommand = "Delete MasGrading2 Where ID=@ID";
                return ds;
            case "StandardStyle":
                ds.SelectCommand = "SELECT ID,SAPCodeDigit, Name, GroupStyle from StandardStyle";
                ds.UpdateCommand = "Update StandardStyle set Name=@Name,GroupStyle=@GroupStyle where ID=@ID";
                ds.InsertCommand = "insert into StandardStyle values(@SAPCodeDigit,@Name,@GroupStyle)";
                ds.DeleteCommand = "Delete StandardStyle Where ID=@ID";
                return ds;
            case "FishSpecies":
                ds.SelectCommand = "SELECT * from MasFishSpecies";
                ds.UpdateCommand = "Update MasFishSpecies set Name=@Name,FishGroup=@FishGroup where SapCode=@ID";
                ds.InsertCommand = "insert into MasFishSpecies (SAPCode,Name,FishGroup)values(@SAPCode,@Name,@FishGroup)";
                ds.DeleteCommand = "Delete MasFishSpecies Where ID=@ID";
                return ds;
            case "FishCert":
                ds.SelectCommand = "SELECT ID,sapcode,Name, FishCert from MasFishCert";
                ds.UpdateCommand = "Update MasFishCert set Name=@Name,FishCert=@FishCert where ID=@ID";
                ds.InsertCommand = "insert into MasFishCert values(@SAPCode,@Name,@FishCert)";
                ds.DeleteCommand = "Delete MasFishCert Where ID=@ID";
                return ds;
            case "stdTunaSpecialFW":
                return dsstdTunaSpecialFW;
            case "StdPricePolicy":
                return dsStdPricePolicy;
            case "StdFillWeight":
                ds.SelectCommand = "SELECT ID,SapCodeDigit,Name,DW,NetWeight,FillWeight,Unit from StdFillWeight";
                ds.UpdateCommand = "Update StdFillWeight set FillWeight=@FillWeight,Name=@Name where ID=@ID";
                ds.InsertCommand = "insert into StdFillWeight values(@SapCodeDigit,@Name,@DW,@NetWeight,@FillWeight,@Unit)";
                ds.DeleteCommand = "Delete StdFillWeight Where ID=@ID";
                return ds;
            case "StdLaborCost":
                ds.SelectCommand = "SELECT * from StdLaborCost2";
                ds.UpdateCommand = "Update StdLaborCost2 set Packaging=@Packaging,Grading=@Grading,Title=@Title,Style=@Style,Currency=@Currency,Cost=@Cost,[From]=@From,[To]=@To,IsActive=@IsActive where Id=@id";
                ds.InsertCommand = "insert into StdLaborCost2 values(@Packaging,@Grading,@Title,@Style,@Cost,@Currency,@Unit,@From,@To,0)";
                ds.DeleteCommand = "Delete StdLaborCost2 Where ID=@ID";
                return ds; 
            case "Primary":
                return dsPrimary;

            case "WorkFlow":
                return dsWorkFlowapp;
            case "UserType":
                ds.SelectCommand = "SELECT * from MasUserType";
                ds.UpdateCommand = "Update MasUserType set name=@name,Subject=@Subject where Id=@id";
                ds.InsertCommand = "insert into MasUserType values(@name,@Subject)";
                ds.DeleteCommand = "Delete MasUserType Where ID=@ID";
                return ds;
            case "Material":
                ds.SelectCommand = string.Format("SELECT ID,Name,tcode,dbo.fnc_gettype(usertype)usertype from MasPackagType where {0}", strvalue);
                ds.UpdateCommand = "Update MasPackagType set name=@name,tcode=@tcode where Id=@id";
                ds.InsertCommand = "insert into MasPackagType values(@name,@tcode,dbo.fnc_settype(@usertype))";
                ds.DeleteCommand = "Delete MasPackagType Where ID=@ID";
                return ds;
            case "SellingUnit":
                ds.SelectCommand = string.Format("SELECT ID,Name,dbo.fnc_gettype(usertype)usertype from MasSellingUnit where {0}", strvalue);
                ds.UpdateCommand = "Update MasSellingUnit set name=@name,usertype=dbo.fnc_settype(@usertype)) where Id=@id";
                ds.InsertCommand = "insert into MasSellingUnit values(@name,dbo.fnc_settype(@usertype))";
                ds.DeleteCommand = "Delete MasSellingUnit Where ID=@ID";
                return ds;
            case "PetCategory":
                ds.SelectCommand = string.Format("select ID,Name,Factory,dbo.fnc_gettype(usertype)usertype,Receiver from MasPetCategory where {0}", strvalue);
                ds.UpdateCommand = "update MasPetCategory set name=@name,Factory=@Factory,usertype=dbo.fnc_settype(@usertype),Receiver=@Receiver where id=@id";
                ds.InsertCommand = "insert into MasPetCategory values(@name,dbo.fnc_settype(@usertype),@Factory,@Receiver)";
                ds.DeleteCommand = "Delete MasPetCategory Where ID=@ID";
                return ds;
            case "CompliedWith":
                ds.SelectCommand = string.Format("SELECT ID,Name,dbo.fnc_gettype(usertype)usertype from MasCompliedWith where {0}", strvalue);
                ds.UpdateCommand = "update MasCompliedWith set name=@name where id=@id";
                ds.InsertCommand = "insert into MasCompliedWith values(@name,dbo.fnc_settype(@usertype))";
                ds.DeleteCommand = "Delete MasCompliedWith Where ID=@ID";
                return ds;
            case "NutrientProfile":
                ds.SelectCommand = "SELECT * from MasNutrientProfile";
                ds.UpdateCommand = "update MasNutrientProfile set name=@name where id=@id";
                ds.InsertCommand = "insert into MasNutrientProfile values(@name)";
                ds.DeleteCommand = "Delete MasNutrientProfile Where ID=@ID";
                return ds;
            case "Color":
                ds.SelectCommand = string.Format("SELECT ID,Name,tcode,dbo.fnc_gettype(usertype)usertype from MasColor where {0}", strvalue);
                ds.UpdateCommand = "update MasColor set name=@name,tcode=@tcode,UserType=dbo.fnc_settype(@usertype) where id=@id";
                ds.InsertCommand = "insert into MasColor values(@name,@tcode,dbo.fnc_settype(@usertype))";
                ds.DeleteCommand = "Delete MasColor Where ID=@ID";
                return ds;
            case "ProductType":
                ds.SelectCommand = "Select * from MasProductType";
                ds.UpdateCommand = "Update MasProductType set Name=@Name where ID=@ID";
                ds.InsertCommand = "Insert into MasProductType values(@Name)";
                ds.DeleteCommand = "Delete MasProductType Where ID=@ID";
                return ds;
            case "Lid":
                ds.SelectCommand = string.Format("Select ID,Name,tcode,dbo.fnc_gettype(usertype)usertype from MasPFLid where {0}", strvalue);
                ds.UpdateCommand = "update MasPFLid set name=@name,tcode=@tcode,UserType=dbo.fnc_settype(@usertype) where id=@id";
                ds.InsertCommand = "insert into MasPFLid values(@name,@tcode,dbo.fnc_settype(@usertype))";
                ds.DeleteCommand = "Delete MasPFLid Where ID=@ID";
                return ds;
            case "Shape":
                ds.SelectCommand = string.Format("Select ID,Name,tcode,dbo.fnc_gettype(usertype)usertype from MasShape where {0}", strvalue);
                ds.UpdateCommand = "Update MasShape set Name=@Name,tcode=@tcode where ID=@ID";
                ds.InsertCommand = "Insert into MasShape values(@Name,@tcode,dbo.fnc_settype(@usertype))";
                ds.DeleteCommand = "Delete MasShape Where ID=@ID";
                return ds;
            case "Lacquer":
                ds.SelectCommand = string.Format("Select ID,Name,tcode,dbo.fnc_gettype(usertype)usertype from MasLacquer where {0}", strvalue);
                ds.UpdateCommand = "Update MasLacquer set Name=@Name,tcode=@tcode where ID=@ID";
                ds.InsertCommand = "Insert into MasLacquer values(@Name,@tcode,dbo.fnc_settype(@usertype))";
                ds.DeleteCommand = "Delete MasLacquer Where ID=@ID";
                return ds;
            case "Type":
                ds.SelectCommand = string.Format("Select ID,Name,tcode,dbo.fnc_gettype(usertype)usertype from MasPFType where {0}", strvalue);
                ds.UpdateCommand = "update MasPFType set name=@name,tcode=@tcode where id=@id";
                ds.InsertCommand = "insert into MasPFType values(@name,@tcode,dbo.fnc_settype(@usertype))";
                ds.DeleteCommand = "Delete MasPFType Where ID=@ID";
                return ds;
            case "Factory":
                ds.SelectCommand = string.Format("SELECT ID,Code,Title,Company,dbo.fnc_gettype(usertype)usertype,NamingCode from MasPlant where {0}", strvalue);
                ds.UpdateCommand = "update MasPlant set Code=@Code,Title=@Title,Company=@Company,UserType=dbo.fnc_settype(@usertype),NamingCode=@NamingCode Where ID=@ID";
                ds.InsertCommand = "insert into MasPlant values(@Code,@Title,@Company,dbo.fnc_settype(@usertype),@NamingCode)";
                ds.DeleteCommand = "Delete MasPlant Where ID=@ID";
                return ds;
            case "Design":
                ds.SelectCommand = string.Format("Select ID,Name,tcode,dbo.fnc_gettype(usertype)usertype from MasDesign where {0}", strvalue);
                ds.UpdateCommand = "update MasDesign set name=@name,tcode=@tcode,usertype=dbo.fnc_settype(@usertype) where id=@id";
                ds.InsertCommand = "insert into MasDesign values(@name,@tcode,dbo.fnc_settype(@usertype))";
                ds.DeleteCommand = "Delete MasDesign Where ID=@ID";
                return ds;
            case "ChunkType":
                ds.SelectCommand = "Select * from MasChunkType";
                ds.UpdateCommand = "Update MasChunkType set Name=@Name Where ID=@ID";
                ds.InsertCommand = "Insert into MasChunkType values(@Name)";
                ds.DeleteCommand = "Delete MasChunkType Where ID=@ID";
                return ds;
            case "Media":
                ds.SelectCommand = "Select * from MasMedia";
                ds.UpdateCommand = "Update MasMedia set Name=@Name Where ID=@ID";
                ds.InsertCommand = "Insert into MasMedia values(@Name)";
                ds.DeleteCommand = "Delete MasMedia Where ID=@ID";
                return ds;
            case "ProductStlye":
                ds.SelectCommand = "Select * from MasProductStyle";
                ds.UpdateCommand = "Update MasProductStyle set Name=@Name Where ID=@ID";
                ds.InsertCommand = "Insert into MasProductStyle values(@Name)";
                ds.DeleteCommand = "Delete MasProductStyle Where ID=@ID";
                return ds;
            case "RequestType":
                ds.SelectCommand = string.Format("Select ID,Name,dbo.fnc_gettype(usertype)usertype from MasRequestType where {0}", strvalue);
                ds.UpdateCommand = "Update MasRequestType set Name=@Name,usertype=dbo.fnc_settype(@usertype) Where ID=@ID";
                ds.InsertCommand = "Insert into MasRequestType values(@Name,dbo.fnc_settype(@usertype))";
                ds.DeleteCommand = "Delete MasRequestType Where ID=@ID";
                return ds;
            case "Customer":
                ds.SelectCommand = "Select * from MasCustomer";
                ds.UpdateCommand = "Update MasCustomer set Name=@Name,Code=@Code Where ID=@ID";
                ds.InsertCommand = "spinsertCustomer";
                ds.InsertCommandType = SqlDataSourceCommandType.StoredProcedure;
                ds.DeleteCommand = "Delete MasCustomer Where ID=@ID";
                return ds;
            case "PetFoodType":
                ds.SelectCommand = string.Format("select ID,Name,dbo.fnc_gettype(usertype)usertype from MasPetFoodType where {0}", strvalue);
                ds.UpdateCommand = "Update MasPetFoodType set Name=@Name,usertype=dbo.fnc_settype(@usertype) Where ID=@ID";
                ds.InsertCommand = "insert MasPetFoodType values(@Name,dbo.fnc_settype(@usertype))";
                ds.DeleteCommand = "Delete MasPetFoodType Where ID=@ID";
                return ds;
            case "EditCost":
                return dsFormulaByCode;
            case "VarietyPack":
                return dsVarietyPack;
            case "User":
                return dsulogin;
            case "upCost":
                List<string> _list = new List<string>();
                foreach (DataRow dr in dataTable.Rows)
                    _list.Add(dr["RequestNo"].ToString());
                var _value = String.Join(";", _list.ToArray());
                ds.SelectCommand = string.Format(@"select b.ID,MarketingNumber,b.RequestNo,isnull(Code,'') as 'Code' 
                from TransCostingHeader a left join TransFormulaHeader b on a.ID=b.RequestNo
                where a.id in (select distinct value from dbo.FNC_SPLIT('{0}','|'))", _value);//Id
                return ds;
            case "ExchangeRat":
                return dsExchangeRat;
            case "Reason":
                return dsReason;
            case "Role":
                return dsApprovAssign;
            case "Policy":
                return dsMasPricePolicy;
            case "Standard":
                ds.SelectCommand = string.Format(@"select [ID],[Company],[Material],[Description]
              ,[PriceStd1]
              ,[PriceStd2]
              ,[Currency]
              ,[Unit]
              ,[From]
              ,[To]
              ,[IsActive]
              ,replace(dbo.fnc_gettype(BU),',',';') usertype from MasPriceStd where Company in (select distinct value from dbo.FNC_SPLIT('{0}',';'))", hBu["BU"].ToString());
                ds.InsertCommand = "Insert into MasPriceStd values (@Company,@Material,@Description,@PriceStd1,@PriceStd2,@Currency,@Unit,@From,@To,0,dbo.fnc_set_value(@usertype,';'))";
                ds.UpdateCommand = @"Update MasPriceStd set Company=@Company,Material=@Material,Description=@Description,PriceStd1=@PriceStd1,
                PriceStd2=@PriceStd2,Currency=@Currency,Unit=@Unit,[From]=@From,[To]=@To,IsActive=@IsActive,BU=dbo.fnc_set_value(@usertype,';') where ID=@ID";
                ds.DeleteCommand = "Delete MasPriceStd Where ID=@ID";
                return ds;
            case "MasPrice":
                return dsMasPrice;
            case "Margin":
                ds.SelectCommand = string.Format(@"select a.*,b.Name as tcode from MasMargin a left join MasPrimary b on b.LBOh=SUBSTRING(MarginCode,1,2) 
                where a.Company in (select distinct value from dbo.FNC_SPLIT('{0}',';'))", hBu["BU"].ToString());
                ds.UpdateCommand = "Update MasMargin set MarginName=@MarginName,MarginRate=@MarginRate where ID=@ID";
                ds.InsertCommand = "spinsertMargin"; ds.InsertCommandType = SqlDataSourceCommandType.StoredProcedure;
                ds.DeleteCommand = "Delete MasMargin Where ID=@ID";
                return ds;
            case "OH_SG&A":
                ds.SelectCommand = string.Format(@"select [ID]
                  ,[Company]
                  ,[usertype] as 'BUtype'
                  ,(SELECT abc = STUFF((SELECT DISTINCT  ','+ convert(nvarchar(max),b.ID) +':'+ b.Name
                                         FROM      MasPetCategory b
                                         WHERE  b.ID in (select value from dbo.FNC_SPLIT(a.Category,';')) FOR XML PATH('')),1,1,'')) [Category]
                  ,[Name]
                  ,[Rate]
                  ,[Currency]
                  ,[Unit]
                  ,[Validfrom]
                  ,[ValidTo]
                  ,[DataType] from MasOHSGA a where {0}
                and a.Company in (select distinct value from dbo.FNC_SPLIT('{1}',';')) ",  strvalue ,hBu["BU"].ToString());
                ds.UpdateCommand = @"Update MasOHSGA set Company=@Company,Category=dbo.fnc_getbutype(@Category),Name=@Name,
                Currency=@Currency,
                Unit=@Unit,
                DataType=@DataType,
                usertype=@Butype,Rate=@Rate,Validfrom=@Validfrom,ValidTo=@ValidTo where ID=@ID";
                ds.InsertCommand = @"insert into MasOHSGA values(@Company
                  ,@Butype
                  ,dbo.fnc_getbutype(@Category)
                  ,@Name
                  ,@Rate
                  ,@Currency
                  ,@Unit
                  ,@Validfrom
                  ,@ValidTo
                  ,@DataType"; 
                ds.DeleteCommand = "Delete MasOHSGA Where ID=@ID";
                return ds;
            case "Labor":
                ds.SelectCommand = string.Format(@"select a.ID
                      ,[Company]
                      ,[LBCode]
                      ,[LBName]
                      ,[PackSize]
                      ,[LBType]
                      ,[LBRate]
                      ,[Currency]
                      ,[LBMin]
                      ,[LBMax]
                      ,[Unit]
                      ,dbo.fnc_gettype(a.BU) as usertype
                      ,[Category]
                  ,b.Name as tcode from MasLaborOverhead from MasLaborOverhead a left join MasPrimary b on b.LBOh=SUBSTRING(LBCode,1,2) 
                where a.Company in (select distinct value from dbo.FNC_SPLIT('{0}',';'))", hBu["BU"].ToString());
                ds.UpdateCommand = @"Update MasLaborOverhead set LBName=@LBName,LBRate=@LBRate,
                LBType=@LBType,Currency=@Currency,PackSize=@PackSize,Unit=@Unit,LBMin=@LBMin,LBMax=@LBMax where ID=@ID";
                //ds.InsertCommand = "Insert into MasLabor values (@Company,@LBCode,@LBName,@LBRate@Currency,@PackSiz)";
                ds.InsertCommand = "spinsertLaborOverhead"; 
                ds.InsertCommandType = SqlDataSourceCommandType.StoredProcedure;
                ds.DeleteCommand = "Delete MasLaborOverhead Where ID=@ID";
                return ds;
            case "DL":
                ds.SelectCommand = string.Format(@"select [ID]
                  ,[Company]
                  ,[BU] as 'BUtype'
                  ,(SELECT abc = STUFF((SELECT DISTINCT  ','+ convert(nvarchar(max),b.ID) +':'+ b.Name
                                         FROM      MasPetCategory b
                                         WHERE  b.ID in (select value from dbo.FNC_SPLIT(a.Category,',')) FOR XML PATH('')),1,1,'')) [Category]
                  ,[LBCode]
                  ,[LBName]
                  ,[LBRate]
                  ,[Currency]
                  ,[Unit] from MasLabor a  
                where a.Company in (select distinct value from dbo.FNC_SPLIT('{0}',';'))", hBu["BU"].ToString());
                ds.UpdateCommand = @"Update MasLabor set LBName=@LBName,LBRate=@LBRate,Category=dbo.fnc_getbutype(@Category),BU=@Butype,
                LBCode=@LBCode,Currency=@Currency,Unit=@Unit where ID=@ID";
                ds.InsertCommand = "Insert into MasLabor values (@Company,@Butype,dbo.fnc_getbutype(@Category),@LBCode,@LBName,@LBRate@Currency,@Unit)";
                ds.DeleteCommand = "Delete MasLabor Where ID=@ID";
                return ds;
            case "Loss":
                ds.SelectCommand = @"select [ID]
                  ,[PackageType]
                  ,[Loss]
                  ,[SubType]
                  ,[Validfrom]
                  ,[Validto]
                  ,replace(dbo.fnc_gettype(BU),',',';') usertype from MasPFLoss";
                ds.UpdateCommand = "Update MasPFLoss set PackageType=@PackageType,Loss=@Loss,SubType=@SubType,Validfrom=@Validfrom,Validto=@Validto,BU=dbo.fnc_set_value(@usertype,';') where Id=@ID";
                ds.InsertCommand = "Insert into MasPFLoss values (@PackageType,@Loss,@SubType,@Validfrom,@Validto,dbo.fnc_set_value(@usertype,';'))";
                ds.DeleteCommand = "Delete MasPFLoss Where ID=@ID";
                return ds;
            case "Company":
                ds.SelectCommand = "select ID,Code,Name from MasCompany";
                ds.UpdateCommand = "Update MasCompany set Code=@Code,Name=@Name where ID=@ID";
                ds.DeleteCommand = "Delete MasCompany Where ID=@ID";
                ds.InsertCommand = "insert into MasCompany values(@Code,@Name)";
                return ds;
            case "Yield":
                return dsMasYield;
            case "Workflow":
                ds.SelectCommand = "select * from MasWFapp";
                ds.UpdateCommand = "Update MasWFapp set Company=@Company,Userstatus=@Userstatus,fn=@fn,StatusApp=@StatusApp where ID=@ID";
                ds.InsertCommand = "Insert into MasWFapp values(@Company,@Userstatus,@fn,@StatusApp)";
                ds.DeleteCommand = "Delete MasWFapp Where ID=@ID";
                return ds;
            default:
                return null;
        }
    }
    private void AddColumns()
    {
        grid.Columns.Clear();
        if (GetDataSource() == null) return;
        DataView dw = (DataView)GetDataSource().Select(DataSourceSelectArguments.Empty);
        var values = new[] { "From", "To", "Validfrom", "Validto" };
        foreach (DataColumn c in dw.Table.Columns)
        {
            var str = c.ColumnName;
            if (c.ColumnName.Contains("tcode"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("DataType"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "value";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = cs.builditems("select value from dbo.FNC_SPLIT('OH;SG&A',';') ");
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("PackageType"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("GroupApp"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "fn";
                cb.PropertiesComboBox.ValueField = "ID";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = dsGroupFlow;
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("Currency") || c.ColumnName.Contains("SubType") || c.ColumnName.Contains("Unit"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "value";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.ToLower().Contains("usertype"))
            {
                //GridViewDataCheckColumn cc = new GridViewDataCheckColumn();
                //cc.FieldName = c.ColumnName;
                //grid.Columns.Add(cc);
                GridViewDataDropDownEditColumn col = new GridViewDataDropDownEditColumn();
                col = new GridViewDataDropDownEditColumn();
                ((DropDownEditProperties)col.PropertiesEdit).DropDownWindowTemplate = new MyComboBoxTemplate();
                col.FieldName = c.ColumnName;
                col.Caption = "Type";
                grid.Columns.Add(col);
            }
            else if (values.Any(str.Equals))
            {
                GridViewDataDateColumn date = new GridViewDataDateColumn();
                date.FieldName = c.ColumnName;
                date.PropertiesDateEdit.DisplayFormatString = "dd-MM-yyyy";
                date.PropertiesDateEdit.EditFormatString = "dd-MM-yyyy";
                grid.Columns.Add(date);
            }
            else if (c.ColumnName.Equals("BU"))
            {
                GridViewDataDropDownEditColumn col = new GridViewDataDropDownEditColumn();
                col = new GridViewDataDropDownEditColumn();
                ((DropDownEditProperties)col.PropertiesEdit).DropDownWindowTemplate = new MyDropDownWindow();
                col.FieldName = c.ColumnName;
                grid.Columns.Add(col);

            }
            else if (c.ColumnName.Equals("RequestType"))
            {
                GridViewDataDropDownEditColumn col = new GridViewDataDropDownEditColumn();
                col = new GridViewDataDropDownEditColumn();
                ((DropDownEditProperties)col.PropertiesEdit).DropDownWindowTemplate = new RequestTypeTemplate(dsRequestType);
                col.FieldName = c.ColumnName;
                grid.Columns.Add(col);

            }
            
            else if (c.ColumnName.Equals("Factory")|| c.ColumnName.Contains("Receiver"))
            {
                GridViewDataDropDownEditColumn col = new GridViewDataDropDownEditColumn();
                col = new GridViewDataDropDownEditColumn();
                ((DropDownEditProperties)col.PropertiesEdit).DropDownWindowTemplate = new MyPlantTemplate(dsPlant);
                col.FieldName = c.ColumnName;
                grid.Columns.Add(col);

            }
            //else if (c.ColumnName.Contains("UType"))
            //{
            //    GridViewDataDropDownEditColumn cb = new GridViewDataDropDownEditColumn();
            //    cb = new GridViewDataDropDownEditColumn();
            //    ((DropDownEditProperties)cb.PropertiesEdit).DropDownWindowTemplate = new MyCheckBoxList();
            //    cb.FieldName = c.ColumnName;
            //    grid.Columns.Add(cb);
            //}
            else if (c.ColumnName.Contains("Company"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.ValueField = "Code";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = dsCompany;
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.ToLower().Equals("butype"))
            {
                GridViewDataComboBoxColumn cbox = new GridViewDataComboBoxColumn();
                cbox.FieldName = c.ColumnName;
                cbox.PropertiesComboBox.Columns.Clear();
                cbox.PropertiesComboBox.TextField = "Name";
                cbox.PropertiesComboBox.ValueField = "Id";
                cbox.PropertiesComboBox.TextFormatString = "{0}";
                cbox.PropertiesComboBox.DataSource = dsusertype;
                grid.Columns.Add(cbox);
            }
            else if (c.ColumnName.ToLower() == "sublevel")
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.ValueField = "Id";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = dsSublevel  ;
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName == "userlevel")
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.ValueField = "Id";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource =  dsUserlevel;
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.ToLower().Equals("zone"))
            {
                GridViewDataDropDownEditColumn col = new GridViewDataDropDownEditColumn();
                col = new GridViewDataDropDownEditColumn();
                ((DropDownEditProperties)col.PropertiesEdit).DropDownWindowTemplate = new MyZoneTemplate();
                col.FieldName = "Zone";
                col.Caption = "Zone";
                grid.Columns.Add(col);
            }
            else if (c.ColumnName.Equals("LevelApprove"))
            {
                GridViewDataDropDownEditColumn col = new GridViewDataDropDownEditColumn();
                col = new GridViewDataDropDownEditColumn();
                ((DropDownEditProperties)col.PropertiesEdit).DropDownWindowTemplate = new MyCustomTemplate();
                col.PropertiesEdit.DisplayFormatString = "{0}";
                col.FieldName = "LevelApprove";
                col.Caption = "LevelApprove";
                grid.Columns.Add(col);
            }
            else if (c.ColumnName.Contains("PackageType") || c.ColumnName.Contains("tcode"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.ToLower().Contains("empid"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("user_name"));
                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("firstname"));
                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("lastname"));
                cb.PropertiesComboBox.TextField = "fn";
                cb.PropertiesComboBox.ValueField = "user_name";
                cb.PropertiesComboBox.TextFormatString = "{1} {2}";
                cb.PropertiesComboBox.Items.Insert(0, new DevExpress.Web.ListEditItem("empty", "",""));
                cb.PropertiesComboBox.NullText = "Select the data";
                cb.PropertiesComboBox.DataSource = dsulogin;
                cb.PropertiesComboBox.EnableCallbackMode = true;
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("Category"))
            {
                GridViewDataDropDownEditColumn cbCat = new GridViewDataDropDownEditColumn();
                cbCat = new GridViewDataDropDownEditColumn();
                ((DropDownEditProperties)cbCat.PropertiesEdit).DropDownWindowTemplate = new MyCategoryTemplate(usertp["usertype"].ToString());
                cbCat.FieldName = c.ColumnName;
                grid.Columns.Add(cbCat);
            }
            else if (c.ColumnName.Contains("emplevel"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.Caption = "ApproveLevel";
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("user_name"));
                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("firstname"));
                cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("lastname"));
                cb.PropertiesComboBox.ValueField = "user_name";
                cb.PropertiesComboBox.TextFormatString = "{1} {2}";
                cb.PropertiesComboBox.DataSource = dsulogin;
                cb.PropertiesComboBox.EnableCallbackMode = true;
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("Currency") || c.ColumnName.Contains("SubType") || c.ColumnName.Contains("Unit") || c.ColumnName.Contains("CurrencyTh"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "value";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Equals("From") || c.ColumnName.Equals("To") || c.ColumnName.Contains("Validfrom") || c.ColumnName.Contains("Validto"))
            {
                GridViewDataDateColumn colID = new GridViewDataDateColumn();
                colID.FieldName = c.ColumnName;
                colID.PropertiesDateEdit.EditFormat = EditFormat.Date;
                colID.PropertiesDateEdit.EditFormatString = "dd/MM/yyyy";
                colID.PropertiesEdit.DisplayFormatString = "dd/MM/yyyy";
                colID.PropertiesDateEdit.AllowNull = true;
                colID.PropertiesDateEdit.AllowUserInput = true;
                colID.PropertiesDateEdit.ConvertEmptyStringToNull = true;
                colID.PropertiesDateEdit.NullDisplayText = "";
                colID.PropertiesDateEdit.NullText = "";
                colID.PropertiesEdit.NullDisplayText = "";
                grid.Columns.Add(colID);
            }
            //else if (c.ColumnName.Contains("Factory"))
            //{
            //    GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
            //    cb.FieldName = c.ColumnName;
            //    cb.PropertiesComboBox.Columns.Clear();
            //    cb.PropertiesComboBox.TextField = "Name";
            //    cb.PropertiesComboBox.TextFormatString = "{0}";
            //    grid.Columns.Add(cb);
            //}
            else if (c.ColumnName.Contains("CustomerPrice") || c.ColumnName.Contains("ExchangeType"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.ValueField = "ID";
                cb.PropertiesComboBox.TextField = "Name";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = dsCustomerPrice;
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("MarginCode") || c.ColumnName.Contains("PercentMargin") || c.ColumnName.Contains("LBCode"))
            {
                GridViewDataTextColumn tc = new GridViewDataTextColumn();
                tc.FieldName = c.ColumnName;
                tc.EditFormSettings.Visible = DevExpress.Utils.DefaultBoolean.False;
                grid.Columns.Add(tc);
            }
            //else if (c.ColumnName.Contains("Receiver"))
            //{
            //    GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
            //    cb.FieldName = c.ColumnName;
            //    cb.PropertiesComboBox.Columns.Clear();
            //    cb.PropertiesComboBox.Columns.Add(new ListBoxColumn("Value"));
            //    cb.PropertiesComboBox.ValueField = "Value";
            //    cb.PropertiesComboBox.EnableCallbackMode = true;
            //    cb.PropertiesComboBox.CallbackPageSize = 10;
            //    cb.PropertiesComboBox.TextFormatString = "{0}";
            //    cb.PropertiesComboBox.DataSource = dsReceiver;
            //    grid.Columns.Add(cb);
            //}
            else if (c.ColumnName.Contains("IsResign") || c.ColumnName.Contains("IsActive"))
            {
                GridViewDataCheckColumn cc = new GridViewDataCheckColumn();
                cc.FieldName = c.ColumnName;
                grid.Columns.Add(cc);
            }
            else if (c.ColumnName == "BaseType")
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "value";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = cs.builditems("select value from dbo.FNC_SPLIT('Non-Fish Base;Fish Base',';') ");
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName == "Groups")
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.ValueField = "Code";
                cb.PropertiesComboBox.TextField = "Description";
                cb.PropertiesComboBox.TextFormatString = "{0}{1}";
                cb.PropertiesComboBox.DataSource = myClass.builditems("select Code,Description from tblToppingGroup ");
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName == "ProductGroup")
            {
                //GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                //cb.FieldName = c.ColumnName;
                //cb.PropertiesComboBox.Columns.Clear();
                //cb.PropertiesComboBox.TextField = "Name";
                //cb.PropertiesComboBox.ValueField = "ProductGroup";
                //cb.PropertiesComboBox.TextFormatString = "{0}";
                //cb.PropertiesComboBox.DataSource = dsProductGroup;
                //gv.Columns.Add(cb);
                GridViewDataDropDownEditColumn col = new GridViewDataDropDownEditColumn();
                ((DropDownEditProperties)col.PropertiesEdit).DropDownWindowTemplate = new MyDropDownWindow();
                col.FieldName = c.ColumnName;
                //col.Visible = false;
                col.EditFormSettings.Visible = DevExpress.Utils.DefaultBoolean.False;
                grid.Columns.Add(col);
            }
            else if (c.ColumnName == "CanSize")
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.Caption = "Container Type";

                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "Description";
                cb.PropertiesComboBox.ValueField = "Code";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = myClass.builditems("select Code,Description from MasfixdigitSize where IsActive='0' and ProductGroup='PF' "); ;
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName== "PrimaryType")
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "Description";
                cb.PropertiesComboBox.ValueField = "Code";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = myClass.builditems("select Code,Description from tblPrimaryPKG"); ;
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("ProductType"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "value";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = cs.builditems("select value from dbo.FNC_SPLIT('PF;HF',';') ");
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("NutritionType"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "value";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = cs.builditems("select value from dbo.FNC_SPLIT('Complementary;Completed;',';') ");
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("Packaging"))
            {
                GridViewDataComboBoxColumn cb = new GridViewDataComboBoxColumn();
                cb.FieldName = c.ColumnName;
                cb.PropertiesComboBox.Columns.Clear();
                cb.PropertiesComboBox.TextField = "value";
                cb.PropertiesComboBox.TextFormatString = "{0}";
                cb.PropertiesComboBox.DataSource = cs.builditems("select value from dbo.FNC_SPLIT('Can;Non Can',';') ");
                grid.Columns.Add(cb);
            }
            else if (c.ColumnName.Contains("Type"))
            {
                GridViewDataTextColumn tc = new GridViewDataTextColumn();
                tc.FieldName = c.ColumnName;
                tc.Caption = "Container Size";
                //tc.ReadOnly = true;
                grid.Columns.Add(tc);
            }
            else
                AddTextColumn(c.ColumnName);
        }
        grid.KeyFieldName = dw.Table.Columns[0].ColumnName;
        grid.Columns[0].Visible = false;
        AddCommandColumn();
    }
    private void AddTextColumn(string fieldName)
    {
        GridViewDataTextColumn c = new GridViewDataTextColumn();
        c.FieldName = fieldName;
        grid.Columns.Add(c);
    }
    private void AddCommandColumn()
    {
        SqlDataSource ds = (SqlDataSource)grid.DataSource;
        bool showColumn = !(String.IsNullOrEmpty(ds.UpdateCommand) && String.IsNullOrEmpty(ds.InsertCommand) &&
            String.IsNullOrEmpty(ds.DeleteCommand));

        if (showColumn)
        {
            GridViewCommandColumn c = new GridViewCommandColumn();
            grid.Columns.Add(c);
            //c.Name = "Editar";
            //c.Caption = "Editar";
            c.VisibleIndex = 0;
            c.ShowEditButton = !String.IsNullOrEmpty(ds.UpdateCommand);
            c.Width = 50;
            //c.ShowNewButtonInHeader = !String.IsNullOrEmpty(ds.InsertCommand);
            //c.ShowDeleteButton = !String.IsNullOrEmpty(ds.DeleteCommand);
            c.ShowCancelButton = true;
            c.ShowUpdateButton = true;
            //c.ButtonRenderMode = GridCommandButtonRenderMode.Image;
        }
        //grid.SettingsCommandButton.NewButton.Image.Url = "~/Content/images/AddRecord.gif";
        //grid.SettingsCommandButton.EditButton.Image.Url = "~/Content/images/Edit.gif";
        //grid.SettingsCommandButton.UpdateButton.Image.Url = "~/Content/images/update.png";
        //grid.SettingsCommandButton.CancelButton.Image.Url = "~/Content/images/cancel.png";
        //grid.SettingsEditing.Mode = GridViewEditingMode.Batch;
    }
    protected void grid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
    {
        object o = selectedDataSource;
        var values = new[] { "Company", "Currency", "Unit", "SubType", "PackageType", "CustomerPrice", "tcode", "empid", "CurrencyTh", "ExchangeType" };
        if (grid.IsNewRowEditing)
        {
            if (e.Column.FieldName == "IsActive" || e.Column.FieldName == "IsResign")
            {
                ASPxCheckBox cb = (ASPxCheckBox)e.Editor;
                cb.Enabled = false;
            }
        }
        if (e.Column.FieldName == "Custom")
        {
            if (grid.IsEditing || grid.IsNewRowEditing)
                e.Editor.ReadOnly = true;
        }
        if (values.Any(e.Column.FieldName.Contains))
        {
            ASPxComboBox combo = (ASPxComboBox)e.Editor;
            dsCompany.SelectParameters.Clear();
            dsCompany.DataBind();
            switch (e.Column.FieldName)
            {
                case "SubType":
                    combo.DataSource = dsSubType;
                    break;
                case "Company":
                    combo.DataSource = dsCompany;
                    break;
                case "Unit":
                    combo.DataSource = dsUnit;
                    break;
                case "CustomerPrice":
                case "ExchangeType":
                    combo.DataSource = dsCustomerPrice;
                    break;
                case "empid":
                    combo.SetClientSideEventHandler("SelectedIndexChanged", "OnSelectedIndexChanged");
                    combo.DataSource = dsulogin;
                    //combo.EnableIncrementalFiltering = false;
                    combo.EnableCallbackMode = true;
                    combo.CallbackPageSize = (!IsPostBack && !Page.IsCallback) ? 5 : 100;
                    break;
                case "tcode":
                case "PackageType":
                    combo.DataSource = dsPrimary;
                    break;
                case "Currency":
                case "CurrencyTh":
                    combo.DataSource = dsCurrency;
                    break;
            }
            combo.DataBind();
        }
        if (e.Column.FieldName == "BU")
        {
            ASPxDropDownEdit dropDownEdit = (ASPxDropDownEdit)e.Editor;
            dropDownEdit.ClientInstanceName = "dropDownEdit";
            string[] indexes = dropDownEdit.Text.Split(',');

            ASPxListBox listBox = (ASPxListBox)dropDownEdit.FindControl("ASPxListBox1");
            listBox.ClientInstanceName = "checkListBox";
            listBox.ClientSideEvents.SelectedIndexChanged = String.Format("function(s,e){{OnListBoxSelectionChanged(s,e,{0});}}", dropDownEdit.ClientID);
            dropDownEdit.ClientSideEvents.TextChanged = String.Format("function(s,e){{ SynchronizeListBoxValues(s,e,{0});}}", listBox.ClientID);
            dropDownEdit.ClientSideEvents.DropDown = String.Format("function(s,e){{ SynchronizeListBoxValues(s,e,{0});}}", listBox.ClientID);
            if (listBox == null) return;
            foreach (ListEditItem item in listBox.Items)
            {
                if (indexes.Contains<string>(item.Value.ToString()))
                    item.Selected = true;
            }
        }
        else if (e.Column.FieldName == "usertype")
        {
            ASPxDropDownEdit dropDownEdit = (ASPxDropDownEdit)e.Editor;
            dropDownEdit.ClientInstanceName = "dropDownEdit1";
            string[] indexes = dropDownEdit.Text.Split(',');

            ASPxListBox listBox = (ASPxListBox)dropDownEdit.FindControl("combo");
            listBox.ClientInstanceName = "checkListBox1";
            listBox.ClientSideEvents.SelectedIndexChanged = String.Format("function(s,e){{OnListBoxSelectionChanged1(s,e,{0});}}", dropDownEdit.ClientID);
            dropDownEdit.ClientSideEvents.TextChanged = String.Format("function(s,e){{ SynchronizeListBoxValues1(s,e,{0});}}", listBox.ClientID);
            dropDownEdit.ClientSideEvents.DropDown = String.Format("function(s,e){{ SynchronizeListBoxValues1(s,e,{0});}}", listBox.ClientID);
            if (listBox == null) return;
            foreach (ListEditItem item in listBox.Items)
            {
                if (indexes.Contains<string>(item.Value.ToString()))
                    item.Selected = true;
            }

        }
        else if (e.Column.FieldName == "Factory")
        {
            ASPxDropDownEdit dropDownEdit = (ASPxDropDownEdit)e.Editor;
            dropDownEdit.ClientInstanceName = "dropDownEdit2";
            string[] indexes = dropDownEdit.Text.Split(',');

            ASPxListBox listBox = (ASPxListBox)dropDownEdit.FindControl("lst");
            listBox.ClientInstanceName = "checkListBox2";
            listBox.ClientSideEvents.SelectedIndexChanged = String.Format("function(s,e){{OnListBoxSelectionChanged2(s,e,{0});}}", dropDownEdit.ClientID);
            dropDownEdit.ClientSideEvents.TextChanged = String.Format("function(s,e){{ SynchronizeListBoxValues2(s,e,{0});}}", listBox.ClientID);
            dropDownEdit.ClientSideEvents.DropDown = String.Format("function(s,e){{ SynchronizeListBoxValues2(s,e,{0});}}", listBox.ClientID);
            if (listBox == null) return;
            foreach (ListEditItem item in listBox.Items)
            {
                if (indexes.Contains<string>(item.Value.ToString()))
                    item.Selected = true;
            }
        }
        else if (e.Column.FieldName == "Category")
        {
            ASPxDropDownEdit dropDownEdit = (ASPxDropDownEdit)e.Editor;
            dropDownEdit.ClientInstanceName = "dropDownEdit3";
            string[] indexes = dropDownEdit.Text.Split(',');

            ASPxListBox listBox = (ASPxListBox)dropDownEdit.FindControl("cbCate");
            listBox.ClientInstanceName = "checkListBox3";
            listBox.ClientSideEvents.SelectedIndexChanged = String.Format("function(s,e){{OnListBoxSelectionChanged3(s,e,{0});}}", dropDownEdit.ClientID);
            dropDownEdit.ClientSideEvents.TextChanged = String.Format("function(s,e){{ SynchronizeListBoxValues3(s,e,{0});}}", listBox.ClientID);
            dropDownEdit.ClientSideEvents.DropDown = String.Format("function(s,e){{ SynchronizeListBoxValues3(s,e,{0});}}", listBox.ClientID);
            if (listBox == null) return;
            foreach (ListEditItem item in listBox.Items)
            {
                if (indexes.Contains<string>(item.Value.ToString()))
                    item.Selected = true;
            }

        }
        else if (e.Column.FieldName.ToLower() == "zone")
        {
            ASPxDropDownEdit dropDownEdit = (ASPxDropDownEdit)e.Editor;
            dropDownEdit.ClientInstanceName = "dropDownEdit4";
            string[] indexes = dropDownEdit.Text.Split(',');

            ASPxListBox listBox = (ASPxListBox)dropDownEdit.FindControl("combo");
            listBox.ClientInstanceName = "checkListBox4";
            listBox.ClientSideEvents.SelectedIndexChanged = String.Format("function(s,e){{OnListBoxSelectionChanged4(s,e,{0});}}", dropDownEdit.ClientID);
            dropDownEdit.ClientSideEvents.TextChanged = String.Format("function(s,e){{ SynchronizeListBoxValues4(s,e,{0});}}", listBox.ClientID);
            dropDownEdit.ClientSideEvents.DropDown = String.Format("function(s,e){{ SynchronizeListBoxValues4(s,e,{0});}}", listBox.ClientID);
            if (listBox == null) return;
            foreach (ListEditItem item in listBox.Items)
            {
                if (indexes.Contains<string>(item.Value.ToString()))
                    item.Selected = true;
            }

        }
        else if (e.Column.FieldName == "LevelApprove")
        {
            ASPxDropDownEdit dropDownEdit = (ASPxDropDownEdit)e.Editor;
            dropDownEdit.ClientInstanceName = "dropDownEdit5";
            string[] indexes = dropDownEdit.Text.Split(',');

            ASPxListBox listBox = (ASPxListBox)dropDownEdit.FindControl("combo");
            listBox.ClientInstanceName = "checkListBox5";
            listBox.ClientSideEvents.SelectedIndexChanged = String.Format("function(s,e){{OnListBoxSelectionChanged5(s,e,{0});}}", dropDownEdit.ClientID);
            dropDownEdit.ClientSideEvents.TextChanged = String.Format("function(s,e){{ SynchronizeListBoxValues5(s,e,{0});}}", listBox.ClientID);
            dropDownEdit.ClientSideEvents.DropDown = String.Format("function(s,e){{ SynchronizeListBoxValues5(s,e,{0});}}", listBox.ClientID);
            if (listBox == null) return;
            foreach (ListEditItem item in listBox.Items)
            {
                if (indexes.Contains<string>(item.Value.ToString()))
                    item.Selected = true;
            }

        }
        else if (e.Column.FieldName == "RequestType")
        {
            ASPxDropDownEdit dropDownEdit = (ASPxDropDownEdit)e.Editor;
            dropDownEdit.ClientInstanceName = "dropDownEdit6";
            string[] indexes = dropDownEdit.Text.Split(',');

            ASPxListBox listBox = (ASPxListBox)dropDownEdit.FindControl("lstRequest");
            listBox.ClientInstanceName = "checkListBox6";
            listBox.ClientSideEvents.SelectedIndexChanged = String.Format("function(s,e){{OnListBoxSelectionChanged6(s,e,{0});}}", dropDownEdit.ClientID);
            dropDownEdit.ClientSideEvents.TextChanged = String.Format("function(s,e){{ SynchronizeListBoxValues6(s,e,{0});}}", listBox.ClientID);
            dropDownEdit.ClientSideEvents.DropDown = String.Format("function(s,e){{ SynchronizeListBoxValues6(s,e,{0});}}", listBox.ClientID);
            if (listBox == null) return;
            foreach (ListEditItem item in listBox.Items)
            {
                if (indexes.Contains<string>(item.Value.ToString()))
                    item.Selected = true;
            }

        }
        else if (values.Any(e.Column.FieldName.Contains))
        {
            ASPxComboBox combo = (ASPxComboBox)e.Editor;
            dsCompany.SelectParameters.Clear();
            dsCompany.DataBind();
            switch (e.Column.FieldName)
            {
                case "Customer":
                case "ExchangeType":
                    combo.DataSource = dsCustomerPrice;
                    break;
                case "SubType":
                    combo.DataSource = dsSubType;
                    break;
                case "Currency":
                case "CurrencyTh":
                    combo.DataSource = dsCurrency;
                    break;
                case "empid":
                    combo.SetClientSideEventHandler("SelectedIndexChanged", "OnSelectedIndexChanged");
                    combo.DataSource = dsulogin;
                    combo.EnableCallbackMode = true;
                    combo.CallbackPageSize = (!IsPostBack && !Page.IsCallback) ? 5 : 100;
                    break;
                case "tcode":
                case "PackageType":
                    combo.DataSource = dsPrimary;
                    break;
                case "Unit":
                    combo.DataSource = dsUnit;
                    break;
            }
            combo.DataBind();
        }
        if (e.Editor is ASPxTextBox || e.Editor is ASPxComboBox)
        {
            e.Editor.Width = Unit.Pixel(200);
        }
    }
    protected void grid_DataBound(object sender, EventArgs e)
    {
        ASPxGridView g = sender as ASPxGridView;
        foreach (GridViewColumn column in g.Columns)
        {
            if (column is GridViewDataColumn)
            {
                ((GridViewDataColumn)column).Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            }
        }
    }
    protected void testGrid_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
    {
        object o = selectedDataSource;
        switch (string.Format("{0}", o))
        {
            case "HierarchyBrand":
            case "HierarchyCorporate":
            case "HierarchyNutrition":
            case "HierarchyPackaging":
            case "HierarchyPetType":
            case "HierarchyProductLine":
                lab.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                break;
            case "HierarchyProductGroup":
                lab.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                lab.SelectParameters.Add("Digit", e.NewValues["Digit"].ToString());
                break;
            case "HierarchyContainer":
                lab.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                lab.SelectParameters.Add("Digit", e.NewValues["Digit"].ToString());
                lab.SelectParameters.Add("ContainerType", e.NewValues["ContainerType"].ToString());
                break;
            case "HierarchyCustomer":
                lab.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                lab.SelectParameters.Add("Brand", e.NewValues["Brand"].ToString());
                lab.SelectParameters.Add("ProductGroup", e.NewValues["ProductGroup"].ToString());
                break;
            case "HierarchyDivision":
                lab.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                lab.SelectParameters.Add("PH2", e.NewValues["PH2"].ToString());
                lab.SelectParameters.Add("PH2Des", e.NewValues["PH2Des"].ToString());
                lab.SelectParameters.Add("PDGROUP", e.NewValues["PDGROUP"].ToString());
                lab.SelectParameters.Add("PRODUCT_LINE", e.NewValues["PRODUCT_LINE"].ToString());
                lab.SelectParameters.Add("DIVISION", e.NewValues["DIVISION"].ToString());
                break;
            case "HierarchyMedia":
                lab.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                lab.SelectParameters.Add("Digit", e.NewValues["Digit"].ToString());
                lab.SelectParameters.Add("Packaging", e.NewValues["Packaging"].ToString());
                lab.SelectParameters.Add("ProductGroup", e.NewValues["ProductGroup"].ToString());
                lab.SelectParameters.Add("Commercial", e.NewValues["Commercial"].ToString());
                break;
            case "HierarchyRawMaterials":
            case "HierarchyStyle":
                lab.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                lab.SelectParameters.Add("Digit", e.NewValues["Digit"].ToString());
                lab.SelectParameters.Add("ProductGroup", e.NewValues["ProductGroup"].ToString());
                break;
            case "ProductGroup":
                lab.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                lab.SelectParameters.Add("ProductGroup", e.NewValues["ProductGroup"].ToString());
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                break;
            case "CanSize":
                lab.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                lab.SelectParameters.Add("ProductGroup", e.NewValues["ProductGroup"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                lab.SelectParameters.Add("CanSize", e.NewValues["CanSize"].ToString());
                lab.SelectParameters.Add("PouchWidth", e.NewValues["PouchWidth"].ToString());
                lab.SelectParameters.Add("Type", e.NewValues["Zone"].ToString());
                lab.SelectParameters.Add("NW", e.NewValues["NW"].ToString());
                lab.SelectParameters.Add("NutritionType", e.NewValues["NutritionType"].ToString());
                lab.SelectParameters.Add("Media", e.NewValues["Media"].ToString());
                lab.SelectParameters.Add("DWeight", e.NewValues["DWeight"].ToString());
                lab.SelectParameters.Add("Packaging", e.NewValues["Packaging"].ToString());
                break;
            case "MasCustomer":
            case "stdCustomer":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("Custom", e.NewValues["Custom"].ToString());
                ds.SelectParameters.Add("Zone", e.NewValues["Zone"].ToString());
                break;
            case "StdPKGStyleSpecial":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("SAPCodeDigit", e.NewValues["SAPCodeDigit"].ToString());
                ds.SelectParameters.Add("PackSize", e.NewValues["PackSize"].ToString());
                ds.SelectParameters.Add("LaborCost", e.NewValues["LaborCost"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                break;
            case "StandardYield":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("FishGroup", e.NewValues["FishGroup"].ToString());
                ds.SelectParameters.Add("Grading", e.NewValues["Grading"].ToString());
                ds.SelectParameters.Add("Yield", e.NewValues["Yield"].ToString());
                break;
            case "StdExchangeRat":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Validfrom", e.NewValues["Validfrom"].ToString());
                ds.SelectParameters.Add("Validto", e.NewValues["Validto"].ToString());
                ds.SelectParameters.Add("Ratio", e.NewValues["Ratio"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Rate", e.NewValues["Rate"].ToString());
                ds.SelectParameters.Add("CurrencyTh", e.NewValues["CurrencyTh"].ToString());
                break;
            case "StdTunaPrice":
                ds.SelectParameters.Add("GroupType", e.NewValues["GroupType"].ToString());
                ds.SelectParameters.Add("GroupDescription", e.NewValues["GroupDescription"].ToString());
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("Price", e.NewValues["Price"].ToString());
                ds.SelectParameters.Add("From", e.NewValues["From"].ToString());
                ds.SelectParameters.Add("To", e.NewValues["To"].ToString());
                break;
            case "StandardUpcharge":
                ds.SelectParameters.Add("UpchargeGroup", e.NewValues["UpchargeGroup"].ToString());
                ds.SelectParameters.Add("Upcharge", e.NewValues["Upcharge"].ToString());
                ds.SelectParameters.Add("Value", e.NewValues["Value"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("StdPackSize", e.NewValues["StdPackSize"].ToString());
                ds.SelectParameters.Add("SapDigit", e.NewValues["SapDigit"].ToString());
                break;
            case "StdOverheadCost":
                ds.SelectParameters.Add("Cost", e.NewValues["Cost"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("IsActive", e.NewValues["IsActive"].ToString());
                break;
            case "StdPackingStyle":
                ds.SelectParameters.Add("GroupStyle", e.NewValues["GroupStyle"].ToString());
                ds.SelectParameters.Add("StyleRef", e.NewValues["StyleRef"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("PackSize", e.NewValues["PackSize"].ToString());
                ds.SelectParameters.Add("PackSize", e.NewValues["PackSize"].ToString());
                ds.SelectParameters.Add("LaborCost", e.NewValues["LaborCost"].ToString());
                ds.SelectParameters.Add("SecPKGCost", e.NewValues["SecPKGCost"].ToString());
                break;
            case "StdPackingStyle2":
                ds.SelectParameters.Add("GroupStyle", e.NewValues["GroupStyle"].ToString());
                ds.SelectParameters.Add("StyleRef", e.NewValues["StyleRef"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("PackSize", e.NewValues["PackSize"].ToString());
                ds.SelectParameters.Add("PackSize", e.NewValues["PackSize"].ToString());
                ds.SelectParameters.Add("LaborCost", e.NewValues["LaborCost"].ToString());
                ds.SelectParameters.Add("SecPKGCost", e.NewValues["SecPKGCost"].ToString());
                break;
            case "StandardPrimary":
                ds.UpdateParameters["MatDescription"].DefaultValue = e.NewValues["MatDescription"].ToString();
                ds.UpdateParameters["Group"].DefaultValue = e.NewValues["Group"].ToString();
                ds.UpdateParameters["GroupDescription"].DefaultValue = e.NewValues["GroupDescription"].ToString();
                ds.UpdateParameters["Can_Size"].DefaultValue = e.NewValues["Can_Size"].ToString();
                ds.UpdateParameters["SAPCodeDigit"].DefaultValue = e.NewValues["SAPCodeDigit"].ToString();
                ds.UpdateParameters["Code"].DefaultValue = e.NewValues["Code"].ToString();
                ds.UpdateParameters["Description"].DefaultValue = e.NewValues["Description"].ToString();
                ds.Update();
                e.Cancel = true;
                testGrid.CancelEdit();
                break;
            case "FishSpecies":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("FishGroup", e.NewValues["FishGroup"].ToString());
                break;
            case "FishCert":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("FishCert", e.NewValues["FishCert"].ToString());
                break;
            case "StdFillWeight":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("FillWeight", e.NewValues["FillWeight"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                break;
            case "StdLaborCost":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Packaging", e.NewValues["Packaging"].ToString());
                ds.SelectParameters.Add("Grading", e.NewValues["Grading"].ToString());
                ds.SelectParameters.Add("Style", e.NewValues["Style"].ToString());
                ds.SelectParameters.Add("Title", e.NewValues["Title"].ToString());
                ds.SelectParameters.Add("Cost", e.NewValues["Cost"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("From", e.NewValues["From"].ToString());
                ds.SelectParameters.Add("To", e.NewValues["To"].ToString());
                ds.SelectParameters.Add("IsActive", e.NewValues["IsActive"].ToString());
                break;
            case "PetCategory":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("Factory", e.NewValues["Factory"].ToString());
                ds.SelectParameters.Add("Receiver", e.NewValues["Receiver"].ToString());
				ds.SelectParameters.Add("usertype", e.NewValues["usertype"].ToString());
                break;
            case "RequestType":case "SellingUnit":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("usertype", e.NewValues["usertype"].ToString());
                break;
            case "PetFoodType":
            case "CompliedWith":
            case "NutrientProfile":
            case "ProductType":
            case "Media":
            case "ProductStlye":
            case "ChunkType":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                break;
            case "UserType":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("Subject", e.NewValues["Subject"].ToString());
                break;
            case "Color":
            case "Shape":
            case "Lacquer":
            case "Material":
            case "Lid":
            case "Design":
            case "Type":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("tcode", e.NewValues["tcode"].ToString());
                ds.SelectParameters.Add("usertype", e.NewValues["usertype"].ToString());
                break;
            case "Factory":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Title", e.NewValues["Title"].ToString());
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("Usertype", e.NewValues["Usertype"].ToString());
                ds.SelectParameters.Add("NamingCode", e.NewValues["NamingCode"].ToString());
                break;
            case "Customer":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("Custom", e.NewValues["Custom"].ToString());
                break;
            case "Company":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                break;
            case "Loss":
                //"Insert into MasPFLoss values @PackageType,@Loss,@SubType";
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("PackageType", e.NewValues["PackageType"].ToString());
                ds.SelectParameters.Add("Loss", e.NewValues["Loss"].ToString());
                ds.SelectParameters.Add("SubType", e.NewValues["SubType"].ToString());
                ds.SelectParameters.Add("Validfrom", e.NewValues["Validfrom"].ToString());
                ds.SelectParameters.Add("Validto", e.NewValues["Validto"].ToString());
                
                break;
            case "Labor":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("LBType", e.NewValues["LBType"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("LBName", e.NewValues["LBName"].ToString());
                ds.SelectParameters.Add("LBRate", e.NewValues["LBRate"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("PackSize", e.NewValues["PackSize"].ToString());
                ds.SelectParameters.Add("LBMin", e.NewValues["LBMin"].ToString());
                ds.SelectParameters.Add("LBMax", e.NewValues["LBMax"].ToString());
                ds.SelectParameters.Add("usertype", e.NewValues["usertype"].ToString());
                ds.SelectParameters.Add("Categoty", e.NewValues["Categoty"].ToString());
                ds.SelectParameters.Add("isactive", e.NewValues["isactive"].ToString());
                break;
            case "Margin":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("MarginName", e.NewValues["MarginName"].ToString());
                ds.SelectParameters.Add("MarginRate", e.NewValues["MarginRate"].ToString());
                break;
            case "Workflow":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("Userstatus", e.NewValues["Userstatus"].ToString());
                ds.SelectParameters.Add("fn", e.NewValues["fn"].ToString());
                ds.SelectParameters.Add("StatusApp", e.NewValues["StatusApp"].ToString());
                break;
            case "Standard":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("Material", e.NewValues["Material"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("PriceStd1", e.NewValues["PriceStd1"].ToString());
                ds.SelectParameters.Add("PriceStd2", e.NewValues["PriceStd2"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("From", e.NewValues["From"].ToString());
                ds.SelectParameters.Add("To", e.NewValues["To"].ToString());
                ds.SelectParameters.Add("IsActive", e.NewValues["IsActive"].ToString());
                break;
            case "OH_SG&A":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("BUtype", e.NewValues["Butype"].ToString());
                ds.SelectParameters.Add("Category", e.NewValues["Category"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("Validfrom", e.NewValues["Validfrom"].ToString());
                ds.SelectParameters.Add("ValidTo", e.NewValues["ValidTo"].ToString());
                ds.SelectParameters.Add("Rate", e.NewValues["Rate"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("DataType", e.NewValues["DataType"].ToString());
                break;
            case "DL":
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("BUtype", e.NewValues["Butype"].ToString());
                ds.SelectParameters.Add("Category", e.NewValues["Category"].ToString());
                ds.SelectParameters.Add("LBCode", e.NewValues["LBCode"].ToString());
                ds.SelectParameters.Add("LBName", e.NewValues["LBName"].ToString());
                ds.SelectParameters.Add("LBRate", e.NewValues["LBRate"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                break;
            case "StdApprove":
                ds.SelectParameters.Add("EMPID", e.NewValues["EMPID"].ToString());
                ds.SelectParameters.Add("Position", e.NewValues["Position"].ToString());
                ds.SelectParameters.Add("LevelApprove", e.NewValues["LevelApprove"].ToString());
                break;
        }
    }
    OrderedDictionary buildCode(OrderedDictionary values)
    {
        object o = selectedDataSource;
        if (string.Format("{0}", o).Equals("CanSize"))
        {
            //string a = MyToDataTable.Increment("X");
            SqlParameter[] param = { new SqlParameter("@ProductGroup", string.Format("{0}", values["ProductGroup"])),
        new SqlParameter("@Packaging", string.Format("{0}", values["Packaging"])),
        new SqlParameter("@CanSize", string.Format("{0}", values["CanSize"])),
        new SqlParameter("@ProductType", string.Format("{0}", values["ProductType"]))};
            DataTable result = myClass.GetRelatedResources("spGetCanSize", param);
            if (result.Rows.Count > 0)
            {
                values["CanSize"] = myClass.ReadItems("select Description from [MasfixdigitSize] where IsActive='0' and Code='" + string.Format("{0}"
                    , values["CanSize"]) + "' and ProductGroup='PF' and Packaging='" + string.Format("{0}", values["Packaging"]) + "'");

                int a = 0;
                string id = result.Rows[0]["Code"].ToString();
                for (int i = id.Length - 1; i >= 0; i--)
                {
                    if (string.Format("{0}", id[i]) == "Z")
                    {
                        var text = string.Format("{0}", id[i]);
                        if (text.All(Char.IsNumber))
                        {
                            string d = (Convert.ToInt32(text) + 1).ToString();
                            values["Code"] = string.Format("{0}{1}2", values["CanSize"], d);
                            return values;
                        }
                        else
                        {
                            values["Code"] = string.Format("{0}{1}2", values["CanSize"], MyToDataTable.Increment(string.Format("{0}", id[1])));
                            return values;
                        }
                    }
                    else if (string.Format("{0}", id[i]) == "9")
                    {
                        values["Code"] = string.Format("{0}A", result.Rows[0]["Code"].ToString().Substring(0, 2));
                        return values;
                    }
                    else
                    {
                        values["Code"] = result.Rows[0]["Code"].ToString().Substring(0, 2) + MyToDataTable.Increment(string.Format("{0}", id[i]));
                        return values;
                    }
                }

            }
        }
        else if (string.Format("{0}", o).Equals("MediaType"))
        {
            SqlParameter[] param = { new SqlParameter("@ProductGroup", string.Format("{0}", values["ProductGroup"])),
        new SqlParameter("@CanSize", string.Format("{0}", values["Groups"])),
        new SqlParameter("@ProductType", string.Format("{0}", values["ProductType"]))};
            DataTable result = myClass.GetRelatedResources("spGetMediaType", param); if (result.Rows.Count > 0)
            {
                int a = 0;
                string id = result.Rows[0]["Code"].ToString();
                for (int i = id.Length - 1; i >= 0; i--)
                {
                    if (string.Format("{0}", id[i]) == "Z")
                    {
                        var text = string.Format("{0}", id[i]);
                        if (text.All(Char.IsNumber))
                        {
                            string d = (Convert.ToInt32(text) + 1).ToString();
                            values["Code"] = string.Format("{0}{1}2", values["Groups"], d);
                            return values;
                        }
                        else
                        {
                            values["Code"] = string.Format("{0}{1}2", values["Groups"], MyToDataTable.Increment(string.Format("{0}", id[1])));
                            return values;
                        }
                    }
                    else if (string.Format("{0}", id[i]) == "9")
                    {
                        values["Code"] = string.Format("{0}A", result.Rows[0]["Code"].ToString().Substring(0, 2));
                        return values;
                    }
                    else
                    {
                        values["Code"] = result.Rows[0]["Code"].ToString().Substring(0, 2) + MyToDataTable.Increment(string.Format("{0}", id[i]));
                        return values;
                    }
                }

            }
        }
        else if (string.Format("{0}", o).Equals("ContainerLid"))
        {
            SqlParameter[] param = { new SqlParameter("@ProductGroup", string.Format("{0}", values["ProductGroup"])),
        new SqlParameter("@CanSize", string.Format("{0}", values["PrimaryType"])),
        new SqlParameter("@ProductType", string.Format("{0}", values["ProductType"]))};
            DataTable result = myClass.GetRelatedResources("spGetContainerLid", param);
            if (result.Rows.Count > 0)
            {
                string id = result.Rows[0]["Code"].ToString().Substring(0,1);
                values["Code"] = string.Format("{0}{1}", values["PrimaryType"], MyToDataTable.Increment(string.Format("{0}", id)));
                return values;
            }
        }
        return values;
    }
    OrderedDictionary buildProductType(OrderedDictionary values)
    {
        buildCode(values);
        SqlParameter[] param = {new SqlParameter("@package", string.Format("{0}", values["Packaging"])),
                        new SqlParameter("@type",string.Format("{0}",values["ProductType"])=="HF"?"H":"F"),//10
                        new SqlParameter("@fishbase", string.Format("{0}", values["BaseType"]).ToLower().Contains("Non-Fish Base")?"non fish": "fish")};
        DataTable sqldt = myClass.GetRelatedResources("spgetproductgroup2", param);
        if (sqldt.Rows.Count > 0)
        {
            DataRow _row = sqldt.Select().FirstOrDefault();
            values["ProductGroup"] = string.Format("{0}", _row["name"]);
        }

        return values;
    }
    protected void testGrid_RowInserting(object sender, ASPxDataInsertingEventArgs e)
    {
        ASPxGridView gv = sender as ASPxGridView;
        object o = selectedDataSource;
        switch (string.Format("{0}", o))
        {
            case "ProductGroup":
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("ProductGroup", e.NewValues["ProductGroup"].ToString());
                break;
            case "ContainerLid":
                lab.SelectParameters.Add("ProductGroup", string.Format("{0}", buildProductType(e.NewValues)));
                lab.SelectParameters.Add("Code", string.Format("{0}", e.NewValues["Code"]));
                lab.SelectParameters.Add("ContainerType", string.Format("{0}", e.NewValues["ContainerType"]));
                lab.SelectParameters.Add("LidType", string.Format("{0}", e.NewValues["LidType"]));
                break;
            case "Grade":case "Brand":case "Style":
                lab.SelectParameters.Add("ProductGroup", string.Format("{0}", buildProductType(e.NewValues)));
                lab.SelectParameters.Add("Code", string.Format("{0}", e.NewValues["Code"]));
                lab.SelectParameters.Add("Description", string.Format("{0}", e.NewValues["Description"]));
                lab.SelectParameters.Add("IsActive", string.Format("{0}", e.NewValues["IsActive"]));
                break;
            case "HierarchyBrand": case "HierarchyCorporate":case "HierarchyNutrition":case "HierarchyPackaging":case "HierarchyPetType":case "HierarchyProductLine":
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                break;
            case "HierarchyProductGroup":
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                lab.SelectParameters.Add("Digit", e.NewValues["Digit"].ToString());
                break;
            case "HierarchyContainer":
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                lab.SelectParameters.Add("Digit", e.NewValues["Digit"].ToString());
                lab.SelectParameters.Add("ContainerType", e.NewValues["ContainerType"].ToString());
                break;
            case "HierarchyCustomer":
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                lab.SelectParameters.Add("Brand", e.NewValues["Brand"].ToString());
                lab.SelectParameters.Add("ProductGroup", e.NewValues["ProductGroup"].ToString());
                break;
            case "HierarchyDivision":
                lab.SelectParameters.Add("PH2", e.NewValues["PH2"].ToString());
                lab.SelectParameters.Add("PH2Des", e.NewValues["PH2Des"].ToString());
                lab.SelectParameters.Add("PDGROUP", e.NewValues["PDGROUP"].ToString());
                lab.SelectParameters.Add("PRODUCT_LINE", e.NewValues["PRODUCT_LINE"].ToString());
                lab.SelectParameters.Add("DIVISION", e.NewValues["DIVISION"].ToString());
                break;
            case "HierarchyMedia":
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                lab.SelectParameters.Add("Digit", e.NewValues["Digit"].ToString());
                lab.SelectParameters.Add("Packaging", e.NewValues["Packaging"].ToString());
                lab.SelectParameters.Add("ProductGroup", e.NewValues["ProductGroup"].ToString());
                lab.SelectParameters.Add("Commercial", e.NewValues["Commercial"].ToString());
                break;
            case "HierarchyRawMaterials":case "HierarchyStyle":
                lab.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                lab.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                lab.SelectParameters.Add("Digit", e.NewValues["Digit"].ToString());
                lab.SelectParameters.Add("ProductGroup", e.NewValues["ProductGroup"].ToString());
                break;
            case "CanSize":
                //object StateID = e.NewValues["CanSize"];
                //GridViewDataComboBoxColumn column = (GridViewDataComboBoxColumn)gv.Columns["CanSize"];
                
                lab.SelectParameters.Add("ProductGroup", string.Format("{0}", buildProductType(e.NewValues)));
                lab.SelectParameters.Add("ProductType", string.Format("{0}", e.NewValues["ProductType"]));
                lab.SelectParameters.Add("Code", string.Format("{0}", e.NewValues["Code"]));
                lab.SelectParameters.Add("CanSize", string.Format("{0}", e.NewValues["CanSize"]));
                lab.SelectParameters.Add("PouchWidth", string.Format("{0}", e.NewValues["PouchWidth"]));
                lab.SelectParameters.Add("Type", string.Format("{0}", e.NewValues["Type"]));
                
                lab.SelectParameters.Add("NW", string.Format("{0}", e.NewValues["NW"]));
                lab.SelectParameters.Add("NutritionType", e.NewValues["NutritionType"].ToString());
                lab.SelectParameters.Add("Media", e.NewValues["Media"].ToString());
                lab.SelectParameters.Add("DWeight", string.Format("{0}", e.NewValues["DWeight"]));
                lab.SelectParameters.Add("Packaging", string.Format("{0}", e.NewValues["Packaging"]));
                break;
            case "MediaType":
                lab.SelectParameters.Add("ProductGroup", string.Format("{0}", buildProductType(e.NewValues)));
                lab.SelectParameters.Add("OldCode", string.Format("{0}", e.NewValues["OldCode"]));
                lab.SelectParameters.Add("Code", string.Format("{0}", e.NewValues["Code"]));
                lab.SelectParameters.Add("Description", string.Format("{0}", e.NewValues["Description"]));
                
                lab.SelectParameters.Add("ProductType", string.Format("{0}", e.NewValues["ProductType"]));
                lab.SelectParameters.Add("MediaType", string.Format("{0}", e.NewValues["MediaType"]));
                break;
            case "RawMaterial":
                lab.SelectParameters.Add("Code", string.Format("{0}", e.NewValues["Code"]));
                lab.SelectParameters.Add("Description", string.Format("{0}", e.NewValues["Description"]));
                lab.SelectParameters.Add("ProductGroup", string.Format("{0}", buildProductType(e.NewValues)));
                break;
            case "StdApprove":
                ds.SelectParameters.Add("EMPID", e.NewValues["EMPID"].ToString());
                ds.SelectParameters.Add("Position", e.NewValues["Position"].ToString());
                ds.SelectParameters.Add("LevelApprove", e.NewValues["LevelApprove"].ToString());
                break;
            case "StdFillWeightMedia":
                ds.SelectParameters.Add("SAPCodeDigit", e.NewValues["SAPCodeDigit"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("GroupType", e.NewValues["GroupType"].ToString());
                ds.SelectParameters.Add("GroupDescription", e.NewValues["GroupDescription"].ToString());
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("CodeName", e.NewValues["CodeName"].ToString());
                ds.SelectParameters.Add("MediaWeight", e.NewValues["MediaWeight"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                break;
            case "StandardPrimary":
                ds.SelectParameters.Add("SAPCodeDigit", e.NewValues["SAPCodeDigit"].ToString());
                ds.SelectParameters.Add("MatDescription", e.NewValues["MatDescription"].ToString());
                ds.SelectParameters.Add("Group", e.NewValues["Group"].ToString());
                ds.SelectParameters.Add("GroupDescription", e.NewValues["GroupDescription"].ToString());
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("Can_Size", e.NewValues["Can_Size"].ToString());
                break;
            case "StdPKGStyleSpecial":
                ds.SelectParameters.Add("SAPCodeDigit", e.NewValues["SAPCodeDigit"].ToString());
                ds.SelectParameters.Add("PackSize", e.NewValues["PackSize"].ToString());
                ds.SelectParameters.Add("LaborCost", e.NewValues["LaborCost"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                break;
            case "MasCustomer":case "stdCustomer":
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("Custom", e.NewValues["Custom"].ToString());
                ds.SelectParameters.Add("Zone", e.NewValues["Zone"].ToString());
                break;
            case "StandardYield":
                ds.SelectParameters.Add("FishGroup", e.NewValues["FishGroup"].ToString());
                ds.SelectParameters.Add("Grading", e.NewValues["Grading"].ToString());
                ds.SelectParameters.Add("Yield", e.NewValues["Yield"].ToString());
                break;
            case "StdExchangeRat":
                ds.SelectParameters.Add("Validfrom", e.NewValues["Validfrom"].ToString());
                ds.SelectParameters.Add("Validto", e.NewValues["Validto"].ToString());
                ds.SelectParameters.Add("Ratio", e.NewValues["Ratio"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Rate", e.NewValues["Rate"].ToString());
                ds.SelectParameters.Add("CurrencyTh", e.NewValues["CurrencyTh"].ToString());
                break;
            case "StdSpcYield":
                break;
            case "StdTunaPrice":
                ds.SelectParameters.Add("GroupType", e.NewValues["GroupType"].ToString());
                ds.SelectParameters.Add("GroupDescription", e.NewValues["GroupDescription"].ToString());
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("Price", e.NewValues["Price"].ToString());
                ds.SelectParameters.Add("From", e.NewValues["From"].ToString());
                ds.SelectParameters.Add("To", e.NewValues["To"].ToString());
                break;
            case "StdOverheadCost":
                ds.SelectParameters.Add("PackagingType", e.NewValues["PackagingType"].ToString());
                ds.SelectParameters.Add("CanSize", e.NewValues["CanSize"].ToString());
                ds.SelectParameters.Add("Size", e.NewValues["Size"].ToString());
                ds.SelectParameters.Add("Title", e.NewValues["Title"].ToString());

                ds.SelectParameters.Add("Cost", e.NewValues["Cost"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("IsActive", e.NewValues["IsActive"].ToString());

                ds.SelectParameters.Add("StdPackSize", e.NewValues["StdPackSize"].ToString());
                ds.SelectParameters.Add("From", e.NewValues["From"].ToString());
                ds.SelectParameters.Add("To", e.NewValues["To"].ToString());
                break;
            case "StdLossPrimaryPKG":
                break;
            case "StdCustomTitle":
                ds.SelectParameters.Add("Material", e.NewValues["Material"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("CustomTitle", e.NewValues["CustomTitle"].ToString());
                break;
            case "StdCertificate":
                ds.SelectParameters.Add("FishGroup", e.NewValues["FishGroup"].ToString());
                ds.SelectParameters.Add("Certificate_fee", e.NewValues["Certificate_fee"].ToString());
                ds.SelectParameters.Add("free", e.NewValues["free"].ToString());
                break;
            case "StdPackingStyle":
                ds.SelectParameters.Add("SAPCodeDigit", e.NewValues["SAPCodeDigit"].ToString());
                ds.SelectParameters.Add("Size", e.NewValues["Size"].ToString());
                ds.SelectParameters.Add("GroupStyle", e.NewValues["GroupStyle"].ToString());
                ds.SelectParameters.Add("StyleRef", e.NewValues["StyleRef"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("PackSize", e.NewValues["PackSize"].ToString());
                ds.SelectParameters.Add("LaborCost", e.NewValues["LaborCost"].ToString());
                ds.SelectParameters.Add("SecPKGCost", e.NewValues["SecPKGCost"].ToString());
                break;
            case "StdPackingStyle2":
                ds.SelectParameters.Add("SAPCodeDigit", e.NewValues["SAPCodeDigit"].ToString());
                ds.SelectParameters.Add("Size", e.NewValues["Size"].ToString());
                ds.SelectParameters.Add("GroupStyle", e.NewValues["GroupStyle"].ToString());
                ds.SelectParameters.Add("StyleRef", e.NewValues["StyleRef"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("PackSize", e.NewValues["PackSize"].ToString());
                ds.SelectParameters.Add("LaborCost", e.NewValues["LaborCost"].ToString());
                ds.SelectParameters.Add("SecPKGCost", e.NewValues["SecPKGCost"].ToString());
                break;
            case "StdSecPackingCost":
                break;
            case "StdPKGLaborCost":
                break;
            case "StdLossSecPKG":
                break;
            case "StandardUpcharge":
                ds.SelectParameters.Add("UpchargeGroup", e.NewValues["UpchargeGroup"].ToString());
                ds.SelectParameters.Add("Upcharge", e.NewValues["Upcharge"].ToString());
                ds.SelectParameters.Add("Value", e.NewValues["Value"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("StdPackSize", e.NewValues["StdPackSize"].ToString());
                ds.SelectParameters.Add("SapDigit", e.NewValues["SapDigit"].ToString());
                break;
            case "StdTunaSpeCase":
                ds.SelectParameters.Add("Material", e.NewValues["Material"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("FishGroup", e.NewValues["FishGroup"].ToString());
                ds.SelectParameters.Add("FishCert", e.NewValues["FishCert"].ToString());
                ds.SelectParameters.Add("SHD", e.NewValues["SHD"].ToString());
                ds.SelectParameters.Add("FillWeight", e.NewValues["FillWeight"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                break;
            case "StdTunaFixFW":
                ds.SelectParameters.Add("Material", e.NewValues["Material"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("FishGroup", e.NewValues["FishGroup"].ToString());
                ds.SelectParameters.Add("FishCert", e.NewValues["FishCert"].ToString());
                ds.SelectParameters.Add("SHD", e.NewValues["SHD"].ToString());
                ds.SelectParameters.Add("FillWeight", e.NewValues["FillWeight"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                break;
            //case "StdSecPKGCost":
            //    ds.SelectParameters.Add("Material", e.NewValues["Material"].ToString());
            //    ds.SelectParameters.Add("Customer", e.NewValues["Customer"].ToString());
            //    ds.SelectParameters.Add("ShipTo", e.NewValues["ShipTo"].ToString());
            //    ds.SelectParameters.Add("Amount", e.NewValues["Amount"].ToString());
            //    ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
            //    ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
            //    break;
            case "StdSAPPackaging":
                ds.SelectParameters.Add("SAPCodeDigit", e.NewValues["SAPCodeDigit"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("PackagingType", e.NewValues["PackagingType"].ToString());
                break;
            case "stdGrading":
                ds.SelectParameters.Add("SAPCodeDigit", e.NewValues["SAPCodeDigit"].ToString());
                ds.SelectParameters.Add("Grading", e.NewValues["Grading"].ToString());
                break;
            case "StandardStyle":
                ds.SelectParameters.Add("SAPCode", e.NewValues["SAPCode"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("GroupStyle", e.NewValues["GroupStyle"].ToString());
                break;
            case "FishCert":
                ds.SelectParameters.Add("SAPCode", e.NewValues["SAPCode"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("FishCert", e.NewValues["FishCert"].ToString());
                break;
            case "FishSpecies":
                ds.SelectParameters.Add("SAPCode", e.NewValues["SAPCode"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("FishGroup", e.NewValues["FishGroup"].ToString());
                break;
            case "StdFillWeight":
                ds.SelectParameters.Add("SapCodeDigit", e.NewValues["SapCodeDigit"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("DW", e.NewValues["DW"].ToString());
                ds.SelectParameters.Add("NetWeight", e.NewValues["NetWeight"].ToString());
                ds.SelectParameters.Add("FillWeight", e.NewValues["FillWeight"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                break;
            case "OH_SG&A":
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("BUtype", e.NewValues["BUtype"].ToString());
                ds.SelectParameters.Add("Category", e.NewValues["Category"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("Validfrom", e.NewValues["Validfrom"].ToString());
                ds.SelectParameters.Add("ValidTo", e.NewValues["ValidTo"].ToString());
                ds.SelectParameters.Add("Rate", e.NewValues["Rate"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("DataType", e.NewValues["DataType"].ToString());
                break;
            case "DL":
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("BUtype", e.NewValues["Butype"].ToString());
                ds.SelectParameters.Add("Category", e.NewValues["Category"].ToString());
                ds.SelectParameters.Add("LBCode", e.NewValues["LBCode"].ToString());
                ds.SelectParameters.Add("LBName", e.NewValues["LBName"].ToString());
                ds.SelectParameters.Add("LBRate", e.NewValues["LBRate"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                break;
            case "StdLaborCost":
                ds.SelectParameters.Add("Packaging", e.NewValues["Packaging"].ToString());
                ds.SelectParameters.Add("Grading", e.NewValues["Grading"].ToString());
                ds.SelectParameters.Add("Style", e.NewValues["Style"].ToString());
                ds.SelectParameters.Add("Title", e.NewValues["Title"].ToString());
                ds.SelectParameters.Add("Cost", e.NewValues["Cost"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("From", e.NewValues["From"].ToString());
                ds.SelectParameters.Add("To", e.NewValues["To"].ToString());
                ds.SelectParameters.Add("IsActive", e.NewValues["IsActive"].ToString());
                break;
            case "PetCategory":
                ds.SelectParameters.Add("usertype", e.NewValues["usertype"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("Factory", e.NewValues["Factory"].ToString());
                ds.SelectParameters.Add("Receiver", e.NewValues["Receiver"].ToString());
                break;
            case "PetFoodType":case "CompliedWith":case "RequestType":case "SellingUnit":
                ds.SelectParameters.Add("usertype", e.NewValues["usertype"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                break;
            case "UserType":
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("Subject", e.NewValues["Subject"].ToString());
                break;
            case "NutrientProfile":
            case "ProductType":
            case "Media":
            case "ProductStlye":
            case "ChunkType":
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                break;
            case "Color":
            case "Material":
            case "Lacquer":
            case "Lid":
            case "Type":
            case "Design":
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("tcode", e.NewValues["tcode"].ToString());
                ds.SelectParameters.Add("usertype", e.NewValues["usertype"].ToString());
                break;
            case "Factory":
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Title", e.NewValues["Title"].ToString());
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("Usertype", e.NewValues["Usertype"].ToString());
                ds.SelectParameters.Add("NamingCode", e.NewValues["NamingCode"].ToString());
                break;
            case "Customer":
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                ds.SelectParameters.Add("Custom", e.NewValues["Custom"].ToString());
                break;
            case "Company":
                ds.SelectParameters.Add("Code", e.NewValues["Code"].ToString());
                ds.SelectParameters.Add("Name", e.NewValues["Name"].ToString());
                break;
            case "Loss":
                ds.SelectParameters.Add("PackageType", e.NewValues["PackageType"].ToString());
                ds.SelectParameters.Add("Loss", e.NewValues["Loss"].ToString());
                ds.SelectParameters.Add("SubType", e.NewValues["SubType"].ToString());
                ds.SelectParameters.Add("Validfrom", e.NewValues["Validfrom"].ToString());
                ds.SelectParameters.Add("Validto", e.NewValues["Validto"].ToString());
                break;
            case "Labor":
                //ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("tcode", e.NewValues["tcode"].ToString());
                ds.SelectParameters.Add("LBName", e.NewValues["LBName"].ToString());
                ds.SelectParameters.Add("LBRate", e.NewValues["LBRate"].ToString());
                ds.SelectParameters.Add("LBType", e.NewValues["LBType"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("PackSize", e.NewValues["PackSize"].ToString());
                ds.SelectParameters.Add("LBMin", e.NewValues["LBMin"].ToString());
                ds.SelectParameters.Add("LBMax", e.NewValues["LBMax"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("usertype", e.NewValues["usertype"].ToString());
                ds.SelectParameters.Add("Categoty", e.NewValues["Categoty"].ToString());
                ds.SelectParameters.Add("isactive",e.NewValues["isactive"].ToString());
                break;
            case "Margin":
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("MarginName", e.NewValues["MarginName"].ToString());
                ds.SelectParameters.Add("MarginRate", e.NewValues["MarginRate"].ToString());
                ds.SelectParameters.Add("tcode", e.NewValues["tcode"].ToString());
                ds.SelectParameters.Add("usertype", e.NewValues["usertype"].ToString());
                break;
            case "Standard":
                ds.SelectParameters.Add("Company", e.NewValues["Company"].ToString());
                ds.SelectParameters.Add("Material", e.NewValues["Material"].ToString());
                ds.SelectParameters.Add("Description", e.NewValues["Description"].ToString());
                ds.SelectParameters.Add("PriceStd1", e.NewValues["PriceStd1"].ToString());
                ds.SelectParameters.Add("PriceStd2", e.NewValues["PriceStd2"].ToString());
                ds.SelectParameters.Add("Currency", e.NewValues["Currency"].ToString());
                ds.SelectParameters.Add("Unit", e.NewValues["Unit"].ToString());
                ds.SelectParameters.Add("From", e.NewValues["From"].ToString());
                ds.SelectParameters.Add("To", e.NewValues["To"].ToString());
                break;
            default:
                break;
        }
        List<string> list = new List<string>();
        string MailTo = "";
        string MailCc = String.Join(",", list.ToArray());
        if (new[] { "CanSize", "MediaType" }.Any(string.Format("create master {0}", o).Equals))
            cs.sendemail(MailTo, MailCc, string.Format("{0}", e.NewValues["Code"].ToString()), string.Format("create master {0}", o));
        gv.CancelEdit();
        e.Cancel = true;
    }
    protected void testGrid_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
    {
        object o = selectedDataSource;
        ASPxGridView g = sender as ASPxGridView;
        int i = g.FindVisibleIndexByKeyValue(e.Keys[g.KeyFieldName]);
        var args = e.Keys[g.KeyFieldName].ToString().Split('|');
        switch (string.Format("{0}", o))
        {
            case "StandardPrimary":
                //SqlParameter[] param = { new SqlParameter("@ID", string.Format("{0}",args[0])),
                //    new SqlParameter("@GroupType", string.Format("{0}",args[1])),
                //    new SqlParameter("@Code", string.Format("{0}",args[2]))};
                //    cs.DelExecuteNonQuery("Delete StdFillWeightMedia Where SAPCodeDigit = @ID and GroupType = @GroupType and Code = @Code", param);
                ds.SelectParameters.Add("SAPCodeDigit", e.Values["SAPCodeDigit"].ToString());
                ds.SelectParameters.Add("Code", e.Values["Code"].ToString());
 
                //ds.SelectParameters.Add("ID", string.Format("{0}", args[0]));
                //ds.SelectParameters.Add("GroupType", string.Format("{0}", args[1]));
                //ds.SelectParameters.Add("Code", string.Format("{0}", args[2]));
                break;
            default:
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                break;
        }
        //ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
    }
    protected void grid_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
    {
        if (e.MenuType == GridViewContextMenuType.Rows)
        {
            e.Items.Clear();
            var item = e.CreateItem("Refresh", "Refresh");
            item.Image.Url = @"~/Content/Images/Refresh.png";
            e.Items.Add(item);

            item = e.CreateItem("Export", "Export");
            item.BeginGroup = true;
            item.Image.Url = @"~/Content/Images/if_sign-out_59204.png";
            e.Items.Insert(e.Items.IndexOfCommand(GridViewContextMenuCommand.Custom), item);
            AddMenuSubItem(item, "PDF", "ExportToPDF", "~/Content/Images/pdf.gif", true);
            AddMenuSubItem(item, "XLS", "ExportToXLS", "~/Content/Images/excel.gif", true);
        }
    }
    private static void AddMenuSubItem(GridViewContextMenuItem parentItem, string text, string name, string iconID, bool isPostBack)
    {
        var exportToXlsItem = parentItem.Items.Add(text, name);
        exportToXlsItem.Image.Url = iconID;
    }

    protected void grid_ContextMenuItemClick(object sender, ASPxGridViewContextMenuItemClickEventArgs e)
    {
        switch (e.Item.Name)
        {
            case "ExportToPDF":
                GridExporter.WritePdfToResponse();
                break;
            case "ExportToXLS":
                GridExporter.WriteXlsToResponse();
                break;
        }
    }
    public class MyDropDownWindow : ITemplate
    {
        void ITemplate.InstantiateIn(Control container)
        {
            ASPxListBox lb = new ASPxListBox();
            lb.ID = "ASPxListBox1";
            container.Controls.Add(lb);
            lb.Width = Unit.Percentage(100);
            lb.SelectionMode = ListEditSelectionMode.CheckColumn;
            //lb.ClientSideEvents.SelectedIndexChanged = "function(s,e) { OnSelectedIndexChanged(s,e); }";
            lb.DataBinding += lb_DataBinding;
        }
        void lb_DataBinding(object sender, EventArgs e)
        {
            ASPxListBox lb = (ASPxListBox)sender;
            lb.DataSource = GetDataSource();
        }
        private List<string> GetDataSource()
        {
            MyDataModule mycla = new MyDataModule();
            DataTable table = mycla.builditems("select Code from MasCompany");
            List<string> list = table.AsEnumerable()
                           .Select(r => r.Field<string>("Code"))
                           .ToList();
            return list;

        }
    }
    class MyComboBoxTemplate : ITemplate
    {
        private List<string> data()
        {
            MyDataModule mycla = new MyDataModule();
            DataTable table = mycla.builditems("select Name from MasUserType");
            List<string> list = table.AsEnumerable()
                           .Select(r => r.Field<string>("Name"))
                           .ToList();
            return list;
            //return new List<string>(new string[] { "101", "102", "103", "901" });
        }
        public void InstantiateIn(Control container)
        {
            ASPxListBox lst = new ASPxListBox();
            //GridViewDataItemTemplateContainer gridContainer = (GridViewDataItemTemplateContainer)container;
            lst.ID = "combo";
            lst.Width = Unit.Percentage(100);
            lst.DataSource = data();
            lst.Border.BorderWidth = 0;
            lst.SelectionMode = ListEditSelectionMode.CheckColumn;
            //combo.DropDownStyle = DropDownStyle.DropDown;
            //combo.IncrementalFilteringMode = IncrementalFilteringMode.StartsWith;
            container.Controls.Add(lst);
        }
    }
    class MyZoneTemplate : ITemplate
    {
        private List<string> data()
        {
            MyDataModule mycla = new MyDataModule();
            DataTable table = mycla.builditems("select distinct [Zone] 'value' from MasCustomer where Zone is not null order by Zone");
            List<string> list = table.AsEnumerable()
                           .Select(r => r.Field<string>("value"))
                           .ToList();
            return list;
            //return new List<string>(new string[] { "101", "102", "103", "901" });
        }
        public void InstantiateIn(Control container)
        {
            ASPxListBox lst = new ASPxListBox();
            //GridViewDataItemTemplateContainer gridContainer = (GridViewDataItemTemplateContainer)container;
            lst.ID = "combo";
            lst.Width = Unit.Percentage(100);
            lst.DataSource = data();
            lst.Border.BorderWidth = 0;
            lst.SelectionMode = ListEditSelectionMode.CheckColumn;
            //combo.DropDownStyle = DropDownStyle.DropDown;
            //combo.IncrementalFilteringMode = IncrementalFilteringMode.StartsWith;
            container.Controls.Add(lst);
        }
    }
    class MyCustomTemplate : ITemplate
    {
        private List<string> data()
        {
            MyDataModule mycla = new MyDataModule();
            DataTable table = mycla.builditems("select user_name, CONCAT(firstname, ' ', lastname)Name from ulogin where dbo.fnc_checktype(usertype,'1')>0 order by firstname,lastname");
            List<string> list = table.AsEnumerable()
                           .Select(r => r.Field<string>("Name"))
                           .ToList();
            return list;
            //return new List<string>(new string[] { "101", "102", "103", "901" });
        }
        public void InstantiateIn(Control container)
        {
            ASPxListBox lst = new ASPxListBox();
            //GridViewDataItemTemplateContainer gridContainer = (GridViewDataItemTemplateContainer)container;
            lst.ID = "combo";
            lst.Width = Unit.Percentage(100);
            lst.DataSource = data();
            lst.Border.BorderWidth = 0;
            lst.SelectionMode = ListEditSelectionMode.CheckColumn;
            //combo.DropDownStyle = DropDownStyle.DropDown;
            //combo.IncrementalFilteringMode = IncrementalFilteringMode.StartsWith;
            container.Controls.Add(lst);
        }
    }
    class MyCategoryTemplate : ITemplate
    {
        string usertype;
        public MyCategoryTemplate(string dsf)
        {
            this.usertype = dsf;
        }
        private List<string> data()
        {
            MyDataModule mycla = new MyDataModule();
            DataTable table = mycla.builditems(@"select convert(nvarchar(max),ID) +':'+Name as 'CateName' from MasPetCategory where dbo.fnc_checktype('" + 
                usertype.ToString() + "',usertype)>0");
            List<string> list = table.AsEnumerable()
                           .Select(r => r.Field<string>("CateName"))
                           .ToList();
            return list;
            //return new List<string>(new string[] { "101", "102", "103", "901" });
        }
        public void InstantiateIn(Control container)
        {
            ASPxListBox lst = new ASPxListBox();
            //GridViewDataItemTemplateContainer gridContainer = (GridViewDataItemTemplateContainer)container;
            lst.ID = "cbCate";
            lst.Width = Unit.Percentage(100);
            lst.DataSource = data();
            lst.Border.BorderWidth = 0;
            lst.SelectionMode = ListEditSelectionMode.CheckColumn;
            //combo.DropDownStyle = DropDownStyle.DropDown;
            //combo.IncrementalFilteringMode = IncrementalFilteringMode.StartsWith;
            container.Controls.Add(lst);
        }
    }
    public class LookupTemplate : ITemplate
    {
        ASPxGridLookup gridLookupOptions = new ASPxGridLookup();

        public LookupTemplate()
        {
            gridLookupOptions.ID = "gridLookupOptions";
            gridLookupOptions.ClientInstanceName = "gridLookupOptions";

            //gridLookupOptions.GridViewProperties.SettingsBehavior.AllowFocusedRow = true;
            //gridLookupOptions.GridViewProperties.SettingsBehavior.AllowSelectByRowClick = true;
            //gridLookupOptions.GridViewProperties.SettingsBehavior.AllowSelectSingleRowOnly = true;

            //gridLookupOptions.Width = Unit.Pixel(100);

            gridLookupOptions.AutoGenerateColumns = true;
            gridLookupOptions.SelectionMode = GridLookupSelectionMode.Multiple;
            gridLookupOptions.TextFormatString = "{0}";
            gridLookupOptions.MultiTextSeparator = " ";

            //gridLookupOptions.Caption = "Select options";
            gridLookupOptions.ClientSideEvents.GotFocus = "function(s,e) { s.ShowDropDown(); }";

        }

        public void InstantiateIn(Control container)
        {

            GridViewEditItemTemplateContainer gridContainer = (GridViewEditItemTemplateContainer)container;
            container.Controls.Add(gridLookupOptions);
        }
    }
    class RequestTypeTemplate : ITemplate
    {
        SqlDataSource dsf;
        public RequestTypeTemplate(SqlDataSource dsRequestType)
        {
            this.dsf = dsRequestType;
        }
        private List<string> data()
        {
            MyDataModule mycla = new MyDataModule();
            DataTable table = ((DataView)this.dsf.Select(DataSourceSelectArguments.Empty)).Table;
            List<string> list = table.AsEnumerable()
                           .Select(r => r.Field<string>("Name"))
                           .ToList();
            return list;
        }

        public void InstantiateIn(Control container)
        {
            ASPxListBox lst = new ASPxListBox();
            //GridViewDataItemTemplateContainer gridContainer = (GridViewDataItemTemplateContainer)container;
            lst.ID = "lstRequest";
            lst.Width = Unit.Percentage(100);
            lst.DataSource = data();
            lst.Border.BorderWidth = 0;
            lst.SelectionMode = ListEditSelectionMode.CheckColumn;
            //combo.DropDownStyle = DropDownStyle.DropDown;
            //combo.IncrementalFilteringMode = IncrementalFilteringMode.StartsWith;
            container.Controls.Add(lst);
        }
    }
    class MyPlantTemplate : ITemplate
    {
        SqlDataSource dsplant;
        public MyPlantTemplate(SqlDataSource dsf)
        {
           this.dsplant = dsf;
        }
        private List<string> data()
        {
            MyDataModule mycla = new MyDataModule();
            DataTable table = ((DataView)this.dsplant.Select(DataSourceSelectArguments.Empty)).Table;
            List<string> list = table.AsEnumerable()
                           .Select(r => r.Field<string>("Code"))
                           .ToList();
            return list;
        }
 
        public void InstantiateIn(Control container)
        {
             ASPxListBox lst = new ASPxListBox();
            //GridViewDataItemTemplateContainer gridContainer = (GridViewDataItemTemplateContainer)container;
            lst.ID = "lst";
            lst.Width = Unit.Percentage(100);
            lst.DataSource = data();
            lst.Border.BorderWidth = 0;
            lst.SelectionMode = ListEditSelectionMode.CheckColumn;
            //combo.DropDownStyle = DropDownStyle.DropDown;
            //combo.IncrementalFilteringMode = IncrementalFilteringMode.StartsWith;
            container.Controls.Add(lst);
        }
    }
    //public class MyCheckBoxList : ITemplate
    //{
    //    void ITemplate.InstantiateIn(Control container)
    //    {
    //        ASPxListBox cb = new ASPxListBox();
    //        cb.ID = "ASPxListBox_XX";
    //        container.Controls.Add(cb);
    //        cb.Width = Unit.Percentage(50);
    //        cb.SelectionMode = ListEditSelectionMode.CheckColumn;
    //        cb.DataBinding += cb_DataBinding;
    //    }
    //    void cb_DataBinding(object sender, EventArgs e)
    //    {
    //        ASPxListBox cb = (ASPxListBox)sender;
    //        cb.DataSource = GetDataSource();
    //    }
    //    private List<string> GetDataSource()
    //    {
    //        return new List<string>(new string[] { "0", "1" });
    //    }
    //}

    protected void grid_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
    {
        object o = selectedDataSource;
        ASPxGridView g = sender as ASPxGridView;
        int i = g.FindVisibleIndexByKeyValue(e.Keys[g.KeyFieldName]);
        var args = e.Keys[g.KeyFieldName].ToString().Split('|');
        switch (string.Format("{0}", o))
        {
            case "XXX":
                //SqlParameter[] param = { new SqlParameter("@ID", string.Format("{0}",args[0])),
                //    new SqlParameter("@GroupType", string.Format("{0}",args[1])),
                //    new SqlParameter("@Code", string.Format("{0}",args[2]))};
                //    cs.DelExecuteNonQuery("Delete StdFillWeightMedia Where SAPCodeDigit = @ID and GroupType = @GroupType and Code = @Code", param);
                ds.DeleteParameters["Code"].DefaultValue = e.Values["Code"].ToString();
                ds.DeleteParameters["SAPCodeDigit"].DefaultValue = e.Values["SAPCodeDigit"].ToString();
                ds.Delete();
                e.Cancel = true;
                //ds.SelectParameters.Add("ID", string.Format("{0}", args[0]));
                //ds.SelectParameters.Add("GroupType", string.Format("{0}", args[1]));
                //ds.SelectParameters.Add("Code", string.Format("{0}", args[2]));
                break;
            default:
                ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
                break;
        }
        //ds.SelectParameters.Add("ID", string.Format("{0}", e.Keys[0]));
    }
}