-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spselectQuotaHeader]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@username nvarchar(max)
AS
BEGIN
	--declare @Id nvarchar(max)='30',@username nvarchar(max)='fo5910155'

	declare @editor nvarchar(max),@statusapp nvarchar(max)
	If Object_ID('tempdb..#table')  is not null  drop table #table
	select a.* into #table from TransQuotationHeader a where a.ID=@Id
	set @statusapp=(select statusapp from #table)
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select * into #temp from dbo.FindULevel(@username) where idx in(2,3,0,8) 
	--select * from #temp
	if @statusapp=0 
	begin
		declare @table tabletype
			insert into @table--select editor from #temp
			select * from (--select empid from MasApprovAssign where Sublevel=3 union all
			select editor from #temp where Sublevel in (3,4))#a
		declare @ulevel nvarchar(max)=dbo.fnc_stuff(@table)
		print @ulevel;
		set @editor=case when (select count(*) from #table a
		where (a.CreateBy in (select distinct value from dbo.FNC_SPLIT(@ulevel,','))
		)) >0 then 0 else  1 end
	end
	else if @statusapp=8 or @statusapp=9
		set @editor=case when (select count(*) from #temp 
		where Sublevel=(case when @statusapp=8 then 4 when @statusapp=9 then 9 else 0 end))>0 then 0 else  1 end
	else
		set @editor=case when (select count(*) from #temp 
		where idx=(case when @statusapp in (2,-1) then 2 else @statusapp end))>0 then 0 else  1 end
	---
	print @editor;

    -- Insert statements for procedure here
	SELECT *,case when a.StatusApp in (4) then 1 
	else @editor end as 'editor' from TransQuotationHeader a where Id=@Id
END


go

