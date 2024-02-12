-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spSummaryCosting]
	-- Add the parameters for the stored procedure here
	@Keyword nvarchar(max),
	@UserNo nvarchar(max)
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.	
	--declare @Keyword nvarchar(max)='CreateOn between ''2020-11-01'' and ''2021-09-29''',@UserNo nvarchar(max)='MO590736' 
 	declare @usertype nvarchar(max)
	set @usertype =(select top 1 usertype from ulogin where [user_name]=@UserNo and IsResign=0)
	declare @str nvarchar(max)
	set @str='select a.ID,a.RequestNo,ExchangeRate,CreateOn from TransCostingHeader a left join TransCusFormulaHeader b on b.RequestNo=a.ID
	where a.RequestNo <> 0 and usertype in (select value from dbo.FNC_SPLIT('''+@usertype+''','';''))'
	SET NOCOUNT ON;
	If Object_ID('tempdb..#ULevel')  is not null  drop table #ULevel
	select * into #ULevel from dbo.FindULevel(@UserNo) where idx in(0,1,2,5,9,10)

	If Object_ID('tempdb..#table')  is not null  drop table #table
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	If Object_ID('tempdb..#TransFormula')  is not null  drop table #TransFormula
	If Object_ID('tempdb..#TransCosting')  is not null  drop table #TransCosting
	if OBJECT_ID('tempdb..#TransFormulaHeader') is not null drop table #TransFormulaHeader
	if OBJECT_ID('tempdb..#LBRate') is not null drop table #LBRate
	if OBJECT_ID('tempdb..#TransTechnical') is not null drop table #TransTechnical
    -- Insert statements for procedure here
	--update TransCosting set per=0 where per = '' and Component in ('margin')
	If Object_ID('tempdb..#query')  is not null  drop table #query
	declare @sql NVARCHAR(MAX)
	declare @table Table(ID nvarchar(max),RequestNo nvarchar(max), ExchangeRate float,CreateOn datetime)
	SET @sql = case when @Keyword='X' or @Keyword='' then 
	@str +' and a.statusapp in (3,4,0,8) and (DATEDIFF(day,CreateOn,GETDATE()))<31' 
	when @Keyword='Z' then @str +' and a.statusapp in (4) and DATEADD(DAY, -30, getdate())<[To]' 
	else @str +' and a.statusapp in (3,4,0,8) and ' +' ' + @Keyword end
	print @sql;
	insert into @table EXEC sp_executesql @sql
	if(select count(*) from #ULevel where dbo.fnc_checktype(SubApp,'0,1')>0)>0
	begin
		If Object_ID('tempdb..#tsql')  is not null  drop table #tsql
		select *into #tsql from @table
		delete @table

		insert into @table
		select ID,RequestNo,ExchangeRate,CreateOn from TransCostingHeader where statusapp in (0,4) 
		and RequestNo <> 0 
		and RequestNo in (
		select ID from TransTechnical a 
		where (a.Requester in (select value from dbo.FNC_SPLIT((select editor from #ULevel),',')) or (select count(value) from dbo.FNC_SPLIT(a.Assignee,',') where value = @UserNo)>0)
		and ID in (select q.RequestNo from #tsql q)
		) 
	end
	--declare @Id nvarchar(max)='8918'
	select * into #TransCosting from TransCosting where RequestNo in (select ID from @table)
	select * into #TransFormulaHeader from TransFormulaHeader where RequestNo in (select ID from @table)
	select *,(case when ISNUMERIC(LBRate)=0 then '0' else replace(LBRate,',','') end)'LBRatex',
	(case when ISNUMERIC(Result)=0 then '0' else replace(Result,',','') end)'Resultx',
	(case when ISNUMERIC(priceofcarton)=0 then '0' else replace(
	(case when ISNUMERIC(Yield)=1 and ISNUMERIC(Result)=1 and (convert(float,Yield)/100) >0 then
	((convert(float,Result)/1000)* (
	select h.PackSize from TransCostingHeader h where h.ID = TransFormula.RequestNo) /(convert(float,Yield)/100) ) *  convert(float,replace(BaseUnit,',','')) 
	else priceofcarton end)
	,',','') end)'priceofcartonx',
	(case when ISNUMERIC(priceofunit)=0 then '0' else replace(priceofunit,',','') end)'priceofunitx'
	 into #TransFormula from TransFormula where RequestNo in (select ID from @table)

	select * into #TransTechnical from TransTechnical where ID in (select RequestNo from @table)
	 
	select * into #table from @table
	select Formula,RequestNo,sum(
	convert(float,a.LBRatex)*convert(float,Resultx))/((select sum(convert(float,x.Resultx)) 
	from #TransFormula x where x.Formula=a.Formula and x.LBRate<>'' and x.Resultx<>'0' and RequestNo=a.RequestNo))'LBRate' 
	into #LBRate FROM #TransFormula a where  ISNUMERIC(a.LBRate)=1 and a.Resultx<>'0' group by a.Formula,a.RequestNo
	--select * from #LBRate where Result='0'
	--select a.*,(case when c.material is null then '1' else '0' end) as 'Mark'  into #MasPricePolicy 
	--from #TransFormula a left join MasPricePolicy c on a.Material=c.Material where c.Material in (select Material from #TransFormula)
	
	select * into #temp from (select Id,RequestNo,Formula,convert(float,LTRIM(RTRIM(priceofcartonx))) as 'minprice',
	(case when Component in 
	('Raw Material') then 'RM' else 'Ing' end) as 'CompoType',Unit from #TransFormula where RequestNo in (select ID from @table) union 
	select Id,RequestNo,Formula,convert(float,
	(case when ISNUMERIC(LTRIM(RTRIM(priceofunitx)))=0 then '0' else convert(float,LTRIM(RTRIM(priceofunitx))) /(case when Currency='THB' then 
	(select top 1 ExchangeRate from @table x where x.ID=#TransFormula.RequestNo) else 1 end) end))'ofunit',
	(case when #TransFormula.Material in ('11M1SL004001','11M1YL004001','11M210000001','11M1B0000002') then 'X' else '1' end)'Fishprice',
	Unit from #TransFormula where RequestNo in (select ID from #table) union all
	--select * from #TransFormula where RequestNo in (select ID from @table) and LBRate <> '' group by Id,RequestNo,Formula union
	
	select id,RequestNo,Formula,(case when Component in ('margin') then 
	convert(float,(case when ISNUMERIC(LTRIM(RTRIM(per))) = 0 then '0' else LTRIM(RTRIM(per)) end)) 
	when Component='Labor&Overhead' and Description='Labelling & WH DL' then Quantity
	when Component='Labor&Overhead' and Description='LOH Factor Adjustment' then Quantity
	else 
	convert(float,(isnull((case when ISNUMERIC(LTRIM(RTRIM(Quantity))) = 0 then '0' else LTRIM(RTRIM(Quantity)) end),'0'))) * 
	convert(float,(isnull((case when ISNUMERIC(LTRIM(RTRIM(priceUnit))) = 0 then '0' else LTRIM(RTRIM(priceUnit)) end),'0'))) end),Component,Description 
	from #TransCosting where RequestNo in (select ID from @table)
	 )#a
	--select * from TransCosting where Component='Labor&Overhead'
	--select * from #temp where Compotype='Labor&Overhead'
	--select cast((sum(case when CompoType in ('RM') then minprice else 0 end)) as decimal(10,2))  from #temp where minprice is null
	If Object_ID('tempdb..#total')  is not null  drop table #total
	select #a.RequestNo,#a.Formula,b.ID,Customer
	,CostNo,
	case when substring(b.code,1,1)='3' then Code end 'MaterialCode',
	b.RefSamples,
	b.Name as 'ProductName',
	#a.RM,
	#a.Ing,
	#a.Margin,
	#a.PrimaryPkg,
	#a.SecondaryPkg,
	#a.[Labelling & WH DL],
	#a.[LOH Factor Adjustment],
	--#a.LOH,
	#a.UpCharge,#a.Fishprice into #total from (
	select RequestNo,Formula, cast((sum(case when CompoType in ('RM') then minprice else 0 end)) as float)as 'RM',
	cast((sum(case when CompoType in ('Ing') then minprice else 0 end)) as float) as 'Ing',
	cast((sum(case when CompoType in ('margin') then minprice else 0 end)) as float) as 'Margin',
	cast((sum(case when CompoType in ('primary packaging') then minprice else 0 end)) as float) as 'PrimaryPkg',
	cast((sum(case when CompoType in ('secondary packaging') then minprice else 0 end)) as float) as 'SecondaryPkg',
--	cast((sum(case when CompoType in ('LOH') then minprice else 0 end)) as float) as 'LOH',
	cast((sum(case when CompoType in ('Labor&Overhead') and Unit='Labelling & WH DL' then minprice else 0 end)) as float) as 'Labelling & WH DL',
	cast((sum(case when CompoType in ('Labor&Overhead') and Unit='LOH Factor Adjustment' then minprice else 0 end)) as float) as 'LOH Factor Adjustment',
	cast((sum(case when CompoType in ('upcharge') then minprice else 0 end)) as float) as 'UpCharge',
	cast((max(case when CompoType in ('X') then 
	(case Unit when 'KG' then minprice * 1000 when 'MT' then minprice end)
	 else 0 end)) as float) as 'Fishprice' from #temp where RequestNo in (select ID from @table)
	group by RequestNo,Formula)#a left join #TransFormulaHeader b on #a.RequestNo=b.RequestNo and #a.Formula=b.Formula 
	--select ISNUMERIC('0.63x')
	--select *,len([LOH Factor Adjustment]),[LOH Factor Adjustment]/100 from #total
 	If Object_ID('tempdb..#a')  is not null  drop table #a
	select 
	a.RequestNo,--b.RequestNo as 'XX',
	--(select #TransTechnical.RequestNo from #TransTechnical where #TransTechnical.ID=b.RequestNo) as 'TechnicalNo',
	Formula,
	a.ID,
	CostNo,Customer,
	--isnull((case when Customer='' or Customer is null then 
	--(select #TransTechnical.Customer from #TransTechnical where #TransTechnical.ID=b.RequestNo) else Customer end),'') 'Customer',
	isnull(MaterialCode,'') 'MaterialCode',
	ProductName,
	a.RefSamples,
	a.RM,
	a.Ing,
	a.PrimaryPkg,
	a.SecondaryPkg,
	(isnull((select LBRate from #LBRate where #LBRate.Formula=a.Formula and #LBRate.RequestNo=a.RequestNo),'0')
	+ (case when len(a.[LOH Factor Adjustment])>0 then (a.[LOH Factor Adjustment]/100)*
	(isnull((select LBRate from #LBRate where #LBRate.Formula=a.Formula and #LBRate.RequestNo=a.RequestNo),'0')) else 1 end)+a.[Labelling & WH DL])	as 'LOH',
	--0 as 'LOH',
	a.UpCharge,
	RM+Ing as 'RmIng',
	convert(float,0) as Loss,
	convert(float,0) as 'Margin',
	convert(float,0)  as 'FOB',
	convert(float,0)  as 'FOBUSD',
	a.Margin as 'PerMargin',ExchangeRate,a.Fishprice,b.CreateOn,isnull(dbo.fnc_getcd(b.RequestNo),'')'Name',b.RequestNo as ReqNo
	--,format(isnull(((select sum(convert(float,isnull(p.PriceOfUnit,0))) from #MasPricePolicy p 
	--where p.Formula=#total.Formula and p.RequestNo=#total.RequestNo) * 1000)/
	--(case when len(ExchangeRate)=0 or ExchangeRate = null then 1 else convert(float,ExchangeRate) end),'0') ,'#,###.00')as 'Fishprice'	
	into #a
	from #total a left join #table b on a.RequestNo=b.ID where len(isnull(a.MaterialCode,'')) >= (case when 
	@Keyword='Z' then 18 else 0 end)
	delete #a where ID is null
	
	--select #b.c from(select count(ID) c from #a group by ID)#b where #b.c>1 is null
	select 
	ROW_NUMBER() OVER (
	ORDER BY #a.RequestNo
   ) row_num,
	#a.RequestNo,
	#a.Formula,
	#a.ID,
	#a.CostNo,
	isnull((case when #a.Customer='' or #a.Customer is null then 
	c.Customer else #a.Customer end),'') 'Customer',
	#a.MaterialCode,
	#a.ProductName,
	#a.RefSamples,
	#a.RM,
	#a.Ing,
	#a.PrimaryPkg,
	#a.SecondaryPkg,
	#a.LOH,
	#a.UpCharge,
	#a.RmIng,
	#a.Loss,
	#a.Margin,
	#a.FOB,
	#a.FOBUsd,
	#a.PerMargin,#a.ExchangeRate,#a.Fishprice,#a.CreateOn,upper(#a.name)name
	,c.RequestNo 'TechnicalNo',c.RequestDate,c.RequireDate from #a 
	left join #TransTechnical c on c.ID=#a.ReqNo
END

go

