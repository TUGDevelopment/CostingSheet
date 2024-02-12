-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spselectCostingHeader]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@username nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)='6465',@username nvarchar(max)='GPCUser02'

	declare @editor nvarchar(max),@statusapp nvarchar(max),@From datetime,@To datetime,@usertype nvarchar(max)
	If Object_ID('tempdb..#table')  is not null  drop table #table
	select a.statusapp,a.Company,isnull(b.Requester,'')Requester,b.RequestDate
      ,b.RequireDate,
	isnull(b.assignee,'')Assignee,a.UserType into #table from TransCostingHeader a left join TransTechnical b on b.ID=a.RequestNo where a.ID=@Id
	 (select @statusapp=statusapp,@From=RequestDate,@To=RequireDate from #table)
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	set @usertype = (select UserType from #table)
	select * into #temp from dbo.fnc_ULevel(@username) where idx in(2,3,0) and dbo.fnc_checktype(usertype,@usertype)>0

	--select * from #temp
	if @statusapp=0 Or @statusapp=8
	begin
		declare @table tabletype
			insert into @table--select editor from #temp
			select * from (--select empid from MasApprovAssign where Sublevel=3 union all
			select editor from #temp where Sublevel in (3,4))#a
		declare @ulevel nvarchar(max)=dbo.fnc_stuff(@table)
		print @ulevel;
		set @editor=case when (select count(*) from #table a
		where (dbo.fnc_checktype(concat(a.Requester,',',isnull(a.Assignee,'')),@ulevel) >0))>0 then 0 else  1 end
	end
	else
		set @editor=case when (select count(*) from #temp 
		where idx=(case when @statusapp in (5,2,-1) then 2 else @statusapp end))>0 then 0 else  1 end
	---
	print @editor;
	SET NOCOUNT ON;
	declare @Name nvarchar(max),@RefSamples nvarchar(max),@Code nvarchar(max)
	select @Name=name,@RefSamples=RefSamples,@Code=case when substring(isnull(Code,''),1,1)='3' then isnull(Code,'') else '' end from TransFormulaHeader where Formula=1 and RequestNo = @Id
	select CONVERT(nvarchar(max), a.UniqueColumn) AS 'UniqueColumn',convert(nvarchar(max),isnull(b.ID,'0'))'ID',a.Company,a.MarketingNumber,a.RDNumber,a.PackSize, 
    isnull(b.RequestNo,'-')'RequestNo',convert(nvarchar(max),a.ID)'Folio',a.Remark,a.CanSize,a.Packaging,--case when a.Completed = 1 then 'true' else 'false' end 'Completed'
    case when a.NetWeight='0' then '0|Grams' else a.Netweight end as 'Netweight',
	ExchangeRate,
	a.StatusApp,
	a.Packaging
	,case when a.StatusApp in (4) then 1 
	--when a.StatusApp in (-1) then 
	--(case when(select count(c.StatusApp) from TransApprove c where tablename=1 and levelApp=3 and c.StatusApp in(-1) and c.RequestNo=@Id)>0
	--then @editor else 1 end)
	else @editor end as 'editor'
	,@Name as 'Name'
	,@RefSamples as 'RefSamples'
	,@Code  as 'Code'
	,isnull(a.Customer,b.Customer+'/'+b.Destination) as 'Customer'
	,format(isnull([From],@From),'dd-MM-yyyy')'From'
	,format(isnull([To],@To),'dd-MM-yyyy')'To'
	,isnull(a.UserType,'0')'UserType'
	,isnull(a.VarietyPack,'0')'VarietyPack'
	from TransCostingHeader a left join TransTechnical b on b.ID = a.RequestNo where a.ID = @Id
END



go

