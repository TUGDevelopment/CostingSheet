-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spselectQuotation]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@username nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)='9068',@username nvarchar(max)='FO5910155'
	declare @editor nvarchar(max),@statusapp nvarchar(max)
	If Object_ID('tempdb..#table')  is not null  drop table #table
	select a.statusapp,a.Company,isnull(b.Requester,'')Requester,
	isnull(b.assignee,'')Assignee into #table from TransCostingHeader a 
	left join TransTechnical b on b.ID=a.RequestNo where a.ID=@Id
	set @statusapp=(select statusapp from #table)
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select * into #temp from dbo.FindULevel(@username) where idx in(2,3,0,8,9,12) 
	--select * from #temp
	if @statusapp=0 Or @statusapp=8 
	begin
		declare @table tabletype
			insert into @table--select editor from #temp
			select * from (--select empid from MasApprovAssign where Sublevel=3 union all
			select editor from #temp where Sublevel in (case @statusapp when 0 then (3) when 8 then (4) end))#a
		declare @ulevel nvarchar(max)=dbo.fnc_stuff(@table)
		print @ulevel;
		set @editor=case when (select count(*) from #table a
		where (a.Requester in (select distinct value from dbo.FNC_SPLIT(@ulevel,','))
		or a.Assignee in (select distinct value from dbo.FNC_SPLIT(@ulevel,',')))) >0 then 0 else  1 end
	end
	else if (@statusapp=9)
		set @editor=case when (select count(*) from #temp where idx in (12))>0 then 0 else  1 end
	else
		set @editor=case when (select count(*) from #temp 
		where idx=(case when @statusapp in (2,-1) then 2 else @statusapp end))>0 then 0 else  1 end
	---
	print @editor;
	SET NOCOUNT ON;
	declare @Name nvarchar(max),@RefSamples nvarchar(max)
	select @Name=name,@RefSamples=RefSamples from TransFormulaHeader where Formula=1 and RequestNo = @Id
	If Object_ID('tempdb..#Quota')  is not null  drop table #Quota
	select * into #Quota from TransQuotation c where c.RequestNo=@Id 
	--select * from #Quota
	select #a.*,
	c.Incoterm,
	c.Freight,
	c.Interest,
	c.PaymentTerm,
	c.Quantity,
	c.[Route],
	c.Size,
	c.Customer,
	c.CustomerWE,
	isnull(c.Remark,'') as 'Notes',
	c.Commission,isnull(Insurance,'') as 'Insurance',
	isnull(c.ID,0) as 'CostingNo' from(
	select CONVERT(nvarchar(max), 
	a.UniqueColumn) AS 'UniqueColumn',
	convert(nvarchar(max),isnull(b.ID,'0'))'ID',
	a.Company,
	a.MarketingNumber,
	a.RDNumber,
	a.PackSize, 
    isnull(b.RequestNo,'-')'RequestNo',
	convert(nvarchar(max),a.ID)'Folio',
	a.Remark,
	a.CanSize,
	a.Packaging,--case when a.Completed = 1 then 'true' else 'false' end 'Completed'
    case when a.NetWeight='0' then '0|Grams' else a.Netweight end as 'Netweight',
	ExchangeRate,
	a.StatusApp,
	case when a.StatusApp in (4) then 1 
	--when a.StatusApp in (-1) then 
	--(case when(select count(c.StatusApp) 
	--from TransApprove c where tablename=1 and levelApp=3 and c.StatusApp in(-1) and c.RequestNo=@Id)>0
	--then @editor else 1 end)
	else @editor end as 'editor'
	,@Name as 'Name'
	,@RefSamples as 'RefSamples'
	,format(RequestDate,'dd-MM-yyyy')'Validfrom'
	,format(RequireDate,'dd-MM-yyyy')'Validto'
	from TransCostingHeader a left join TransTechnical b on b.ID = a.RequestNo 
	where a.ID = @Id)#a left join #Quota c on #a.Folio=c.RequestNo
END

go

