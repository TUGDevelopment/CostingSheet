-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spselectrequest2]
@Id nvarchar(max),
@username nvarchar(max)
--@compa nvarchar(10)
AS
BEGIN
	--declare @Id nvarchar(max)='235',@username nvarchar(max)='fo5910155'
	--intial
	declare @table table(Id int IDENTITY(1,1),Name nvarchar(50))
	insert into @table
	select value from dbo.FNC_SPLIT('Product Spec.;Prototype Photo;Costing,CS Profile',';')
	declare @statusapp nvarchar(max),@editor nvarchar(max),@Requester nvarchar(max),@Receiver nvarchar(max),
	@requestNo nvarchar(max),@Copied nvarchar(max),@assignee nvarchar(max)
	---
	select @statusapp=statusapp,
	@Requester=Requester,@requestNo=RequestNo,@assignee=isnull(assignee,''),@Receiver=isnull(Receiver,'') 
	from TransTechnical where ID=@Id
	---
	 If Object_ID('tempdb..#temp')  is not null  drop table #temp
	 select * into #temp from dbo.FindULevel(@username) where idx in(0,1,2)
	 declare @mytable tabletype
	 insert into @mytable
	 select editor from #temp
	 declare @ulevel nvarchar(max)=dbo.fnc_stuff(@mytable)
	 print @ulevel;
    ---
	set @Copied =(select count(*) from TransTechnical 
	where RequestNo=@requestNo and StatusApp in (0,1,2)) 
	---authorization
	set @editor=case when @statusapp = 0 then
		(select case when 
		(select sum(value) from(select count(value)'value' from dbo.FNC_SPLIT(@ulevel,',') where value=@Requester union all
		select count(value) from dbo.FNC_SPLIT(@ulevel,',') where value=@assignee)#a) >0 and 
		(select count(*) from #temp where idx= @statusapp)>0 then 0 
		else  1 end)
	when @statusapp = 1 then
		(select case when (select count(value) from dbo.FNC_SPLIT(@ulevel,',') where value=@Receiver)>0
		and (select count(*) from #temp where idx= @statusapp)>0 then 0 
		else  1 end)
	when @statusapp=2 then
		(select case when (select count(*) from #temp where @statusapp = idx) >0 then 0 else  1 end) 
	when @statusapp in (-1,4) then 1 end
	print @editor;

	select ID,RequestNo,Company,Marketingnumber,Requester,
	petfoodtype,
	format(RequestDate,'dd-MM-yyyy')'Validfrom'
	,format(RequireDate,'dd-MM-yyyy')'Validto',PackSize
	,PetCategory
	,CompliedWith
    ,NutrientProfile
    ,Requestfor
    ,ProductType
    ,ProductStyle
	,ProductNote
    ,Media
    ,ChunkType
    ,NetWeight
    ,PackSize
    ,Packaging
    ,Material
    ,PackType
    ,PackDesign
    ,PackColor
    ,PackLid
    ,PackShape
    ,PackLacquer
    ,SellingUnit
	,format(CreateOn,'dd-MM-yyyy')'CreateOn'
    ,MarketingNumber
    ,RDNumber
    ,AcceptCostingRequest
    ,Modified
    ,isnull(Drainweight,'-')'Drainweight'
	,isnull(Notes,'')'Notes'
	,isnull(Customer,'')'Customer'
	,isnull(Customprice,'')'Customprice'
	,isnull(Brand,'')'Brand'
	,isnull(Destination,'')'Destination'
	,isnull(Country,'')'Country'
	,isnull(ESTVolume,'')'ESTVolume'
	,isnull(ESTLaunching,'')'ESTLaunching'
	,isnull(ESTFOB,'')'ESTFOB'
	,isnull(CustomerRequest,'')'CustomerRequest'
	,isnull(Ingredient,'|')'Ingredient'
	,isnull(Claims,'|')'Claims'
	,isnull(StatusApp,'0')'StatusApp'
	,isnull(VetOther,'|')'VetOther'
	,isnull(PhysicalSample,'|||')'PhysicalSample'
	,isnull(Receiver,'')'Receiver'
	,Revised
	,isnull(RequestType,0)'RequestType'
	,CONVERT(nvarchar(max), UniqueColumn) AS 'UniqueColumn'
	,SUBSTRING(RequestNo,4,1)'RequestType'
	,case StatusApp when 6 then 1 else @editor end as 'editor'
	,case when @Copied > 0 then 0 else 1 end as Copied--Copied 0 : OK ,1 : Not OK
	,CONVERT(nvarchar(max), NewID()) as 'CopiedID'
	 from TransTechnical  where ID=@Id
END


go

