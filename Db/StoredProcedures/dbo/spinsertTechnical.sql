-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spinsertTechnical] 
@RequestNo nvarchar(50),
@Requester nvarchar(250),
@Company nvarchar(50),
@From datetime,
@To datetime,
@PetCategory nvarchar(250),
@PetFoodType nvarchar(250),
@CompliedWith nvarchar(250),
@NutrientProfile nvarchar(250),
@Requestfor nvarchar(250),
@ProductType nvarchar(max), 
@ProductStyle nvarchar(250),
@Media nvarchar(250),
@ChunkType nvarchar(250),
@NetWeight nvarchar(250),
@PackSize nvarchar(50),
@Primary nvarchar(250),
@Material nvarchar(250),
@PackageType nvarchar(250),
@Design nvarchar(50),
@Color nvarchar(250),
@Lid nvarchar(250),
@PackagingShape nvarchar(250),
@Lacquer nvarchar(250),
@SellingUnit nvarchar(250),
@Marketingnumber nvarchar(250),
@Notes nvarchar(max),
@Drainweight nvarchar(max),
@Customer nvarchar(max),
@Brand  nvarchar(max),
@Destination nvarchar(max),
@Country nvarchar(max),
@ESTVolume nvarchar(max),
@ESTLaunching nvarchar(max),
@ESTFOB nvarchar(max),
@ProductNote nvarchar(max),
@CustomerRequest nvarchar(max),
@Ingredient nvarchar(max),
@Claims nvarchar(max),
@VetOther nvarchar(max),
@PhysicalSample nvarchar(max),
@Receiver nvarchar(max),
@RequestType nvarchar(max),
@NewID nvarchar(max),
@StatusApp nvarchar(max),
@RDNumber nvarchar(max),
@customprice nvarchar(max),
@Assignee nvarchar(max),
@usertype nvarchar(max),
@SampleDate datetime,
@PrdRequirement nvarchar(max),
@SecInner nvarchar(max),
@SecOuter nvarchar(max),
@Concave  nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Drainweight nvarchar(250)
	declare @Type nvarchar(max)=(case when @Receiver='0' then '2'
		else '0' end)
	if(@StatusApp=1)
	if(select count(*) from dbo.FNC_SPLIT(@Requestfor,';') where value in ('Costing','Line Extension'))=0 and @Type='0'
		set @Type =3;
	SET NOCOUNT ON;
	if(@RequestNo = '' or @RequestNo='0')
	BEGIN
	if(select count(ID)from TransTechnical where UniqueColumn=@NewID)>0 Goto ExitToJump
	declare @id nvarchar(max)
	set @id= substring(@Company,1,3) +'0'+ convert(nvarchar(2),right(year(getdate()),2)) +''+ 
	(SELECT format(isnull(max(right(RequestNo,5)),0)+1, '00000') FROM TransTechnical
	where SUBSTRING(RequestNo,5,2)=right(year(getdate()),2) and SUBSTRING(RequestNo,4,1)='0')
--	print @id
	set @usertype = (select usertype from MasPetCategory  
	where ID=@PetCategory)

	INSERT INTO TransTechnical
	select RequestNo=@id,
	Requester=@Requester,
	RequestDate=@From,
	RequireDate=@To, 
	Company=@Company,
	PetCategory=@PetCategory, 
	PetFoodType=@PetFoodType,
	CompliedWith=@CompliedWith,
	NutrientProfile=@NutrientProfile,
	Requestfor=@Requestfor,
	ProductType=@ProductType,
	ProductStyle=@ProductStyle,
	ProductNote=@ProductNote,
	Media=@Media,
	ChunkType=@ChunkType,
	NetWeight=@NetWeight,
	PackSize=@PackSize,
	Packaging=@Primary,
	Material=@Material,
	PackType=@PackageType,
	PackDesign=@Design,
	PackColor=@Color,
	PackLid=@Lid,
	PackShape=@PackagingShape,
	PackLacquer=@Lacquer,
	SellingUnit=@SellingUnit,
	CreateOn=getdate(),
	Marketingnumber=@Marketingnumber,
	RDNumber=@RDNumber,
	AcceptCostingRequest='',
	Modified=null,
	Drainweight=@Drainweight,
	Notes=@Notes,
	Customer=@Customer,
	customprice=@customprice,
	Brand=@Brand,  
	Destination=@Destination,
	Country=@Country,
	ESTVolume=@ESTVolume,
	ESTLaunching=@ESTLaunching,
	ESTFOB=@ESTFOB,
	CustomerRequest=@CustomerRequest,
	Ingredient=@Ingredient,
	Claims=@Claims,
	Concave=@Concave, 
	StatusApp=0,
	ReferenceNo=0,
	VetOther=@VetOther,
	PhysicalSample=@PhysicalSample,
	Receiver=@Receiver,
	RequestType=@Type,
	Revised=0,
	assignee=@Assignee,
	usertype=@usertype,
	SampleDate=@SampleDate,
	PrdRequirement=@PrdRequirement,
	SecInner=@SecInner,
	SecOuter=@SecOuter,
	null,
	UniqueColumn=@NewID
    --SELECT SCOPE_IDENTITY();
	--SET @RequestNo = (SELECT RequestNo FROM TransTechnical Where ID = (SELECT CAST(scope_identity() AS int)))
	SET @ID = (SELECT CAST(scope_identity() AS int))
	END
	ELSE  
	BEGIN
	if ((select StatusApp from TransTechnical where ID=@RequestNo)=0)
	UPDATE TransTechnical Set 
	Company= @Company,
	PetCategory=@PetCategory,
	Requestfor=@Requestfor,
	ProductType=@ProductType,
	RequestDate=@From, 
	RequireDate=@To,
	Packaging=@Primary,
	Modified=getdate(),
	Material=@Material,
	PackColor=@Color,
	PackLid=@Lid,
	PackLacquer=@Lacquer,
	PackDesign=@Design,
	SellingUnit=@SellingUnit,
	PackShape=@PackagingShape,
	Marketingnumber=@Marketingnumber,
	PetFoodType=@PetFoodType,
	CompliedWith=@CompliedWith,
	NutrientProfile=@NutrientProfile,
	ProductStyle=@ProductStyle,
	ProductNote=@ProductNote,
	Media=@Media,
	ChunkType=@ChunkType,
	NetWeight=@NetWeight,
	PackSize=@PackSize,
	PackType=@PackageType,
	Drainweight=@Drainweight,
	Notes=@Notes,
	Customer=@Customer,
	customprice=@customprice,
	Brand=@Brand,  
	Destination=@Destination,
	Country=@Country,
	ESTVolume=@ESTVolume,
	ESTLaunching=@ESTLaunching,
	ESTFOB=@ESTFOB,
	CustomerRequest=@CustomerRequest,
	Ingredient=@Ingredient,
	Claims=@Claims,
	Concave=@Concave, 
	VetOther=@VetOther,
	PhysicalSample=@PhysicalSample,
	Receiver=@Receiver,	
	RequestType=@Type,
	--UniqueColumn=@NewID,
	PrdRequirement=@PrdRequirement,
	SecInner=@SecInner,
	SecOuter=@SecOuter,
	--assignee=@Assignee,
	SampleDate=@SampleDate
	--DevelopFmBy=@DevelopFmBy
	Where ID=@RequestNo
	else
	UPDATE TransTechnical Set RequestDate=@From, 
	RequireDate=@To,Modified=GETDATE(),	customprice=@customprice Where ID=@RequestNo
	set @id=@RequestNo;
	END
	--#workflow
	if (@StatusApp='1' Or @StatusApp='7') 
	and (select count(Id) from TransApprove where RequestNo=@id and tablename=0)=0--send approve
	begin
		if(@Type=3) 
		set @usertype ='0' 
		else if (@usertype in (2,3,4,5,6,7,8,9)) set @usertype ='2'
		Exec spcreateapprove @ID,@Requester,@Type,@usertype
	end
	SELECT * from TransTechnical where ID=@ID;
ExitToJump:
END
--select * from TransTechnical where id=344
 
 


go

