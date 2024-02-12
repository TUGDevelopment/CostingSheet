-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spSummaryCostingsheet]
	-- Add the parameters for the stored procedure here
	@Keyword nvarchar(max),
	@UserNo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Keyword nvarchar(max)='CreateOn between ''2020-04-01'' and ''2020-04-20''',@UserNo nvarchar(max)='FP542117'
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	declare @usertype nvarchar(max)--='0;1;2;3;4;5;6;7;8'
	set @usertype =(select usertype from ulogin where [user_name]=@UserNo)
	declare @sql NVARCHAR(MAX)
	declare @table Table(ID nvarchar(max))
	declare @str nvarchar(max)='select ID from TransTechnical where dbo.fnc_checktype(usertype,'''+@usertype+''')>0'
	SET @sql = case when @Keyword='X' or @Keyword='' then @str +' and (DATEDIFF(day,CreateOn,GETDATE()))<31' 
	else @str + ' and ' +' ' + @Keyword end
	--print @sql;
	insert into @table EXEC sp_executesql @sql

	If Object_ID('tempdb..#Technical')  is not null  drop table #Technical
	select a.* into #Technical from TransTechnical a where a.ID in (select * from @table)
	--select * from transTechnical where requestno='10201900757'
    -- Insert statements for procedure here
	SELECT #a.*,isnull(b.CostNo,#a.MarketingNumber)'CostNo',isnull(b.Code,'')as 'Code',b.Name,b.ID as 'Items',b.RefSamples into #temp 
	from(select b.RequestNo,a.ID,a.Packaging,a.Netweight,a.CanSize,a.Company,b.Requester,b.ID as 'trfid',a.Revised,b.CreateOn,a.CreateOn as 'CostStarted',
	b.Destination,a.StatusApp,a.Remark,a.CreateBy,b.Requestfor,b.Customer ,a.usertype,a.MarketingNumber,b.PetFoodType,b.Company as 'ProposedFactory',[ESTVolume],
	[ESTLaunching],
	[ESTFOB]
	from #Technical b left join TransCostingHeader a on a.RequestNo=b.ID where a.RequestNo > 0 and a.usertype in (select value from dbo.FNC_SPLIT(@usertype,';'))
	)#a left join TransFormulaHeader b on #a.ID=b.RequestNo

	--select * from #temp
	--If Object_ID('tempdb..#ulog')  is not null  drop table #ulog
	--select * into #ulog from ulogin where IsResign=0
	If Object_ID('tempdb..#appr')  is not null  drop table #appr
	select * into #appr from TransApprove where RequestNo in (select distinct trfid from #temp) and tablename='0' and levelApp in (0,1,2,6)
	If Object_ID('tempdb..#apprcost')  is not null  drop table #apprcost
	select * into #apprcost from TransApprove where RequestNo in (select distinct id from #temp) and tablename in ('1','2') and levelApp in (2,3)
	--select * from #appr
	--calculate
	If Object_ID('tempdb..#t')  is not null  drop table #t
	select #temp.*,
	case when substring(#temp.RequestNo,4,1)='1' then #temp.CreateOn else (select SubmitDate from #appr where levelApp=0 and tablename=0 and #appr.RequestNo=#temp.trfid) end as 'Marketing',
	case when substring(#temp.RequestNo,4,1)='1' then #temp.CreateOn else (select SubmitDate from #appr where levelApp=1 and tablename=0 and #appr.RequestNo=#temp.trfid) end as 'Received',
	case when substring(#temp.RequestNo,4,1)='1' then #temp.CreateOn else (select SubmitDate from #appr where levelApp=6 and tablename=0 and #appr.RequestNo=#temp.trfid) end as 'PD',
	case when substring(#temp.RequestNo,4,1)='1' then #temp.CostStarted else (select SubmitDate from #appr where levelApp=2 and tablename=0 and #appr.RequestNo=#temp.trfid) end as 'Started',
	(select top 1 SubmitDate from #apprcost where levelApp=2 and tablename in (1,2) and #apprcost.RequestNo=#temp.ID) as 'Completed',
	(select SubmitDate from #apprcost where levelApp=3 and tablename in (1,2) and #apprcost.RequestNo=#temp.ID) as 'SendCostMKT',
	RequestNo as 'TRFWebBase',(select top 1 NamingCode from MasPlant b where dbo.fnc_checktype(b.usertype,#temp.usertype)>0 and b.Company=#temp.Company)as 'Plant',--u.FirstName +'.'+ substring(u.LastName,1,1) 'Marketing Name',
	#temp.Requester as 'MarketingName',
	(case when code=name then '' else code end)'ProductName' into #t from #temp 
	--select * from #t

	select trfid as ID,
	RequestNo,
	Requestfor,
	PetFoodType,
	ProposedFactory,
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
	Marketing,
	Received 'R&D',
	PD,
	[ESTVolume],
	[ESTLaunching],
	[ESTFOB],
	[Started],
	Completed,
	case when Completed != '' or [Started] != '' then 
	(select convert(nvarchar(max),[Days])+'.'+ convert(nvarchar(max),[Hours]) from dbo.fnc_DateDif([Started],Completed)) end as 'Usedtime',
	StatusApp,
	Remark,
	upper(CreateBy) as 'By',
	SendCostMKT
	,case when SendCostMKT !='' or [Started]!='' then 
	(select convert(nvarchar(max),[Days])+'.'+ convert(nvarchar(max),[Hours]) from dbo.fnc_DateDif([Started],SendCostMKT))end as 'KPIUsedtime'
	from #t
END
--select * from TransTechnical where id=7222
--select * from TransApprove where requestno=19344
--select * from mashistory where requestno=19344
--select * from(SELECT [user_name],firstname +' '+ lastname as fn from ulogin where IsResign=0 and firstname like 'manee%' )#a union select '0','None'
--select * from transcostingheader where marketingNumber='TU630049'

go

