-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spSummaryReportCost]
	-- Add the parameters for the stored procedure here
	@Keyword nvarchar(max),
	@UserNo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Keyword nvarchar(max)='X',@UserNo nvarchar(max)='mp001688'
	--declare @Keyword nvarchar(max)='CreateOn between ''2020-02-09'' and ''2023-03-10''',@UserNo nvarchar(max)='fo5910155'
	declare @usertype nvarchar(max)
	set @usertype =(select usertype from ulogin where [user_name]=@UserNo)
	If Object_ID('tempdb..#level')  is not null  drop table #level
	select * into #level from dbo.FindULevel(@UserNo) where idx in(0,1,2,5,9,10)
	declare @t tabletype 
	insert into @t
	select case when Sublevel in (6,7) then concat(ulevel,',',editor) else editor end from #level
	declare @editor nvarchar(max)=dbo.fnc_stuff(@t)
	declare @str nvarchar(max)='select ID from TransCostingHeader where (CreateBy In (select distinct value from dbo.FNC_SPLIT('''+@editor+''','','')) or ID in (select distinct(RequestNo) from MasHistory where Username'
	set @str= @str + ' in (select distinct value from dbo.FNC_SPLIT('''+@editor+''','','')))) and dbo.fnc_checktype(usertype,'''+@usertype+''')>0'
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	
	declare @sql NVARCHAR(MAX)
	declare @table Table(ID nvarchar(max))
	SET @sql = case when @Keyword='X' or @Keyword='' then 
	@str +' and (DATEDIFF(day,CreateOn,GETDATE()))<31' 
	else @str +' and ' +' ' + @Keyword end
	--print @sql;
	insert into @table EXEC sp_executesql @sql
 
    -- Insert statements for procedure here
	SELECT #a.*,isnull(b.CostNo,#a.MarketingNumber) as 'CostNo',isnull(b.Code,'')as 'Code',b.Name,isnull(b.ID,#a.ID) as 'Items',b.RefSamples into #temp 
	from(select b.RequestNo,a.ID,a.Packaging,a.Netweight,a.CanSize,a.Company,b.Requester,b.ID as 'trfid',a.Revised,b.CreateOn,a.CreateOn as 'CostStarted',
	b.Destination,a.StatusApp,a.Remark,a.CreateBy,a.MarketingNumber,a.UserType,a.Customer 
	from TransCostingHeader a left join TransTechnical b on a.RequestNo=b.ID where a.RequestNo > 0 and a.ID in (select * from @table) 
	and dbo.fnc_checktype(a.usertype,@usertype)>0
	)#a left join TransFormulaHeader b on #a.ID=b.RequestNo
	--select * from #temp
	--If Object_ID('tempdb..#ulog')  is not null  drop table #ulog
	--select * from  MasHistory where statusapp=1
	If Object_ID('tempdb..#appr')  is not null  drop table #appr
	select * into #appr from TransApprove where RequestNo in (select distinct trfid from #temp) and tablename='0' and levelApp in (1,2)
	If Object_ID('tempdb..#apprcost')  is not null  drop table #apprcost
	select * into #apprcost from TransApprove where RequestNo in (select distinct id from #temp) and tablename='1' and levelApp in (2,3)
	If Object_ID('tempdb..#history')  is not null  drop table #history
	select * into #history from MasHistory where RequestNo in (select distinct trfid from #temp) and tablename in ('1','TransTechnical') 
	--calculate
	If Object_ID('tempdb..#t')  is not null  drop table #t
	select #temp.*,
	case when substring(#temp.RequestNo,4,1)='1' then #temp.CreateOn else (select SubmitDate from #appr where levelApp=1 and tablename=0 and #appr.RequestNo=#temp.trfid) end as 'Received',
	case when substring(#temp.RequestNo,4,1)='1' then #temp.CostStarted else (select SubmitDate from #appr where levelApp=2 and tablename=0 and #appr.RequestNo=#temp.trfid) end as 'Started',
	(select SubmitDate from #apprcost where levelApp=2 and tablename=1 and #apprcost.RequestNo=#temp.ID) as 'Completed',
	(select SubmitDate from #apprcost where levelApp=3 and tablename=1 and #apprcost.RequestNo=#temp.ID) as 'SendMKT',
	(select h.CreateOn from #history h where h.StatusApp=5 and h.tablename in ('1','TransTechnical') and h.RequestNo=#temp.trfid) as 'RD_send',
	(select h.Username from #history h where h.StatusApp=5 and h.tablename in ('1','TransTechnical') and h.RequestNo=#temp.trfid) as 'RD',
	(select h.CreateOn from #history h where h.StatusApp=1 and h.tablename in ('1','TransTechnical') and h.RequestNo=#temp.trfid) as 'RDM_send',
	(select h.Username from #history h where h.StatusApp=1 and h.tablename in ('1','TransTechnical') and h.RequestNo=#temp.trfid) as 'RDM',
	(case when StatusApp in (-1) then (select h.Username from #history h where h.StatusApp in (3) and h.tablename in ('1','TransTechnical') and h.RequestNo=#temp.trfid) else '' end) as 'Remark_Reject',
	RequestNo as 'TRFWebBase',(select top 1 NamingCode from MasPlant b where dbo.fnc_checktype(b.usertype,#temp.usertype)>0 and b.Company=#temp.Company)as 'Plant',--u.FirstName +'.'+ substring(u.LastName,1,1) 'Marketing Name',
	#temp.Requester as 'MarketingName',
	(case when code=name then '' else code end)'ProductName' into #t from #temp 
	--left join #ulog u on #temp.Requester=u.[user_name] 
	--where Code <>''

	select Items as ID,
	RequestNo,
	CostNo as 'CostingNo',
	case when substring(code,1,1)='3' then Code end 'MaterialCode',
	RefSamples as 'TestNo',
	Plant,
	Packaging as 'Package',
	CanSize,
	(select top 1 value from dbo.FNC_SPLIT(#t.Netweight,'|')) as 'NW',
	Name as 'ProductName',
	Customer,
	upper(MarketingName)'MarketingName',
	Destination as 'Country',
	Received,
	[Started],
	Completed,
	case when Completed != '' or [Started] != '' then 
	(select convert(nvarchar(max),[Days])+'.'+ convert(nvarchar(max),[Hours]) from dbo.fnc_DateDif([Started],Completed)) end as 'Usedtime',
	StatusApp,
	Remark,
	upper(CreateBy) as 'By',
	SendMKT
	
	,case when SendMKT !='' or [Started]!='' then 
	(select convert(nvarchar(max),[Days])+'.'+ convert(nvarchar(max),[Hours]) from dbo.fnc_DateDif([Started],SendMKT))end as 'KPIUsedtime',
	RD_send,RD,RDM_send,RDM,trfid,Remark_Reject
	from #t
END
--select * from TransTechnical where requestno='10102200847'
--select * from MasStatusApp where levelapp in (0,2)
--select * from TransCostingHeader where marketingnumber='GP630308'
--update TransCostingHeader set Marketingnumber='HF630394' where id=17828
 

go

