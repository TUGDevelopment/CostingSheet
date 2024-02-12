-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spselectrequest]
@Id nvarchar(max),
@username nvarchar(max)
--@compa nvarchar(10)
AS
BEGIN
	--declare @Id nvarchar(max)='7969',@username nvarchar(max)='FP5910155'
	--intial
	declare @table table(Id int IDENTITY(1,1),Name nvarchar(50))
	insert into @table
	select value from dbo.FNC_SPLIT('Product Spec.;Prototype Photo;Costing,CS Profile',';')
	declare @statusapp nvarchar(max),@editor nvarchar(max),@Requester nvarchar(max),@Receiver nvarchar(max),
	@requestNo nvarchar(max),@Copied nvarchar(max),
	@assignee nvarchar(max),
	@usertype nvarchar(max),
	@Sublevel nvarchar(max)
	---
	select @statusapp=statusapp,
	@usertype=usertype,
	@Requester=Requester,@requestNo=RequestNo,@assignee=isnull(assignee,''),@Receiver=isnull(Receiver,'') 
	from TransTechnical where ID=@Id
	---print @statusapp;
	
	 If Object_ID('tempdb..#temp')  is not null  drop table #temp
	 select * into #temp from dbo.fnc_ULevel(@username) where idx in(0,1,2,6,5,7,8,9,10) and usertype=@usertype
	 declare @mytable tabletype
	 insert into @mytable
	 --select editor from #temp
	 select case when Sublevel=6 then concat(ulevel,',',isnull(editor,'')) else isnull(editor,'') end from #temp
	 declare @ulevel nvarchar(max)=dbo.fnc_stuff(@mytable)
	 --print @ulevel;
     --select * from #temp
	 delete @mytable; insert into @mytable
	 select SubApp from #temp
	 declare @SubApp nvarchar(max)=dbo.fnc_stuff(@mytable)
	 set @Copied =(select count(*) from TransTechnical 
	 where RequestNo=@requestNo and StatusApp in (0,1,2,5,6,7,8,9,10)) --approve Copied Status
	---authorization
	 set @editor=case when @statusapp in (0,9) then
		(select case when 
		(select sum(value) from(select count(value)'value' from dbo.FNC_SPLIT(@ulevel,',') where value=@Requester union all
		select count(value) from dbo.FNC_SPLIT(@ulevel,',') where value in (select a.value from dbo.FNC_SPLIT(@assignee,',')a))#a) >0 and 
		(select count(*) from #temp where idx= @statusapp)>0 then 0 
		else  1 end)
	when @statusapp in (1,5) then
		(select case when 
		(select count(value) from dbo.FNC_SPLIT(@ulevel,',') where value in (select a.value from dbo.FNC_SPLIT(@Receiver,',')a))>0
		and (select count(value) from dbo.FNC_SPLIT(@SubApp,',') where value= @statusapp)>0 then 0 
		else  1 end)
	when @statusapp in (2) then
		(select case when (select count(*) from #temp where @statusapp = idx) >0 then 0 else  1 end) 
	when @statusapp in (6,7,8) and (select count(*) from #temp where idx in (6,7,8)) >0 then
		0
	when @statusapp in (-1,4) then 1 end
	--print @editor;

	select ID,RequestNo,Company,Marketingnumber,Requester,
	petfoodtype,
	format(RequestDate,'dd-MM-yyyy')'Validfrom'
	,format(RequireDate,'dd-MM-yyyy')'Validto',PackSize
	,PetCategory
	--,(select a.ID from MasPetCategory a where a.Name=PetCategory 
	--	and a.Factory=Company and a.usertype=usertype)PetCategoryID
	,CompliedWith
    ,NutrientProfile
    ,Requestfor
    ,ProductType
    ,ProductStyle
	,ProductNote
    ,Media
	,Concave
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
	,concat(isnull(CustomerRequest,''),case when CHARINDEX('|',isnull(CustomerRequest,''))=0 then '|' else '' end) 'CustomerRequest'
	,isnull(Ingredient,'|')'Ingredient'
	,isnull(Claims,'|')'Claims'
	,isnull(StatusApp,'0')'StatusApp'
	,isnull(VetOther,'|')'VetOther'
	,isnull(PhysicalSample,'|||')'PhysicalSample'
	,replace(isnull(Receiver,''),',','')'Receiver'
	,Revised
	,isnull(RequestType,0)'RequestType'
	,CONVERT(nvarchar(max), UniqueColumn) AS 'UniqueColumn'
	,SUBSTRING(RequestNo,4,1)'RequestType'
	,@editor as 'editor'
--	,case StatusApp when 6 then 1 else @editor end as 'editor'
	,case when @Copied > 0 then 0 else 1 end as Copied--Copied 0 : OK ,1 : Not OK
	,CONVERT(nvarchar(max), NewID()) as 'CopiedID'
	,upper(isnull(assignee,'')) as 'assignee'
	,isnull(UserType,'')'UserType'
	,format(isnull(SampleDate,DATEADD(dy,7,CreateOn)),'dd-MM-yyyy')'SampleDate'
    ,concat(isnull(PrdRequirement,'') , case when CHARINDEX('|',isnull(PrdRequirement,''))=0 then '|' else '' end) 'PrdRequirement'
	,concat(isnull(SecInner,'') , case when CHARINDEX('|',isnull(SecInner,''))=0 then '|' else '' end) 'Inner'
	,(case when CHARINDEX('|',isnull(SecOuter,''))=0 then '|' else SecOuter end)'Outer'
	,(select Name from MasPetCategory a where a.ID=PetCategory)ProductCat
	 from TransTechnical  where ID=@Id
END

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
 
go

