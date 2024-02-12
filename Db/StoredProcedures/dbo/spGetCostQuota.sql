-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCostQuota]
	-- Add the parameters for the stored procedure here
	@user_name nvarchar(max),
	@Id int
AS
BEGIN
 --declare @user_name nvarchar(max)='FO5910155',@id int =0
 --select * from TransQuotationHeader where requestno='20200500001'
	declare @StatusApp nvarchar(max)
	set @StatusApp = concat('',(select StatusApp from TransQuotationHeader where id=@id))
	If Object_ID('tempdb..#find')  is not null  drop table #find
	select idx,ulevel,editor,Sublevel,SubApp into #find from dbo.FindULevel(@user_name)

	declare @temp tabletype;
	insert into @temp
	select * from(select editor from #find)#a --Development
	declare @editor nvarchar(max)= dbo.fnc_stuff(@temp)


	If Object_ID('tempdb..#TransCostingHeader')  is not null  drop table #TransCostingHeader
	select * into #TransCostingHeader from TransCostingHeader where StatusApp=4 --and [To] >= cast(getdate() as date)  
	If Object_ID('tempdb..#MasHistory')  is not null  drop table #MasHistory
	select * into #MasHistory from MasHistory
	If Object_ID('tempdb..#TransTechnical')  is not null  drop table #TransTechnical
	select * into #TransTechnical from TransTechnical where StatusApp=4 and ID in (select RequestNo from #TransCostingHeader) 
	--where dbo.fnc_checktype(concat(requester,',',isnull(assignee,'')),@editor)>0 and StatusApp=4

 	If Object_ID('tempdb..#TransFormulaHeader')  is not null  drop table #TransFormulaHeader
	select * into #TransFormulaHeader from TransFormulaHeader where RequestNo in (select ID from #TransCostingHeader) 

	If Object_ID('tempdb..#table')  is not null  drop table #table
	select distinct(ID) into #table from #TransCostingHeader 
	where RequestNo in (select ID from #TransTechnical where @editor in (select value from dbo.FNC_SPLIT(concat(requester,',',isnull(assignee,'')),',')))

	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select * into #temp from (select distinct(RequestNo) from #MasHistory where Username in (select distinct value from dbo.FNC_SPLIT(@editor,','))
		and tablename in ('3','TransCostingHeader') union select distinct(ID) from #table)#a  

	select concat('S',ID) ID,CostNo,RefSamples,Code,[Name],RequestNo,
		convert(bit,0)'IsActive',
		(select a.Revised from TransCostingHeader a where a.ID=#TransFormulaHeader.RequestNo )'Revised'--,'S' CostType
	from #TransFormulaHeader where RequestNo in (select RequestNo from #temp) 
	union select concat('V',ID),MarketingNumber,RDNumber,VarietyPack,'','',convert(bit,0)IsActive,Revised--,'V' 
	from #TransCostingHeader where isnull(VarietyPack,'')<>''
	--union
	--select ID,MarketingNumber,RequestNo,RDNumber,ExchangeRate,Company from #TransFormulaHeader where ID in (
	--select a.RequestNo from TransQuotation a where a.SubID=@Id)
END

go

