-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spSummaryCosting2]
	-- Add the parameters for the stored procedure here
	@Keyword nvarchar(max),
	@UserNo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.	
	--declare @Keyword nvarchar(max)='[CreateOn] Between ''2020-03-01'' and ''2020-03-31''',@UserNo nvarchar(max)='FO5910155'
 	declare @usertype nvarchar(max)
	set @usertype =(select usertype from ulogin where [user_name]=@UserNo)
	declare @str nvarchar(max)
	set @str='select ID,RequestNo,ExchangeRate,CreateOn from TransCostingHeader where RequestNo <>0 and dbo.fnc_checktype(usertype,'''+@usertype+''')>0'
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
	if(select count(*) from #ULevel where SubApp in (0,1))>0
	begin
		insert into @table
		select ID,RequestNo,ExchangeRate,CreateOn from TransCostingHeader where statusapp in (0) and RequestNo in (
		select ID from TransTechnical a 
		where (a.Requester in (@UserNo) or (select count(value) from dbo.FNC_SPLIT(a.Assignee,',') where value = @UserNo)>0)) 
	end
	else
	begin
	SET @sql = case when @Keyword='X' or @Keyword='' then 
	@str +' and statusapp in (3,4,0,8) and (DATEDIFF(day,CreateOn,GETDATE()))<31' 
	else @str +' and statusapp in (3,4,0,8) and ' +' ' + @Keyword end
	--print @sql;
	insert into @table EXEC sp_executesql @sql
	end
	--declare @Id nvarchar(max)='8918'
	select *,(case when ISNUMERIC(LBRate)=0 then '0' else replace(LBRate,',','') end)'LBRatex',
	(case when ISNUMERIC(Result)=0 then '0' else replace(Result,',','') end)'Resultx',
	(case when ISNUMERIC(priceofcarton)=0 then '0' else replace(priceofcarton,',','') end)'priceofcartonx',
	(case when ISNUMERIC(priceofunit)=0 then '0' else replace(priceofunit,',','') end)'priceofunitx'
	 into #TransFormula from TransFormula where RequestNo in (select ID from @table)
	select * into #TransCosting from TransCosting where RequestNo in (select ID from @table)
	select * into #TransTechnical from TransTechnical where ID in (select RequestNo from @table)
	select * into #TransFormulaHeader from TransFormulaHeader where RequestNo in (select ID from @table)
	select * into #table from @table

	select Formula,RequestNo,sum(
	convert(float,a.LBRatex)*convert(float,Resultx))/((select sum(convert(float,x.Resultx)) 
	from #TransFormula x where x.Formula=a.Formula and x.LBRate<>'' and x.Resultx<>'0' and RequestNo=a.RequestNo))'LBRate' 
	into #LBRate FROM #TransFormula a where  ISNUMERIC(a.LBRate)=1 and a.Resultx<>'0' group by a.Formula,a.RequestNo
	--select * from #LBRate where Result='0'
	--select a.*,(case when c.material is null then '1' else '0' end) as 'Mark'  into #MasPricePolicy 
	--from #TransFormula a left join MasPricePolicy c on a.Material=c.Material where c.Material in (select Material from #TransFormula)

	select * into #temp from (select Id,RequestNo,Formula,convert(float,LTRIM(RTRIM(priceofcartonx))) as 'minprice',
	(case when Component in ('Raw Material') then 'RM' else 'Ing' end) as 'CompoType',Unit from #TransFormula where RequestNo in (select ID from @table) union 
	select Id,RequestNo,Formula,convert(float,
	(case when ISNUMERIC(LTRIM(RTRIM(priceofunitx)))=0 then '0' else convert(float,LTRIM(RTRIM(priceofunitx))) /(case when Currency='THB' then 
	(select ExchangeRate from @table x where x.ID=#TransFormula.RequestNo) else 1 end) end))'ofunit',
	(case when #TransFormula.Material in ('11M1SL004001','11M1YL004001','11M210000001','11M1B0000002') then 'X' else '1' end)'Fishprice',
	Unit from #TransFormula where RequestNo in (select ID from #table) union
	--select * from #TransFormula where RequestNo in (select ID from @table) and LBRate <> '' group by Id,RequestNo,Formula union
	
	select id,RequestNo,Formula,(case when Component in ('margin') then 
	convert(float,(case when ISNUMERIC(LTRIM(RTRIM(per))) = 0 then '0' else LTRIM(RTRIM(per)) end)) else 
	convert(float,(isnull((case when ISNUMERIC(LTRIM(RTRIM(Quantity))) = 0 then '0' else LTRIM(RTRIM(Quantity)) end),'0'))) * 
	convert(float,(isnull((case when ISNUMERIC(LTRIM(RTRIM(priceUnit))) = 0 then '0' else LTRIM(RTRIM(priceUnit)) end),'0'))) end),Component,'' 
	from #TransCosting where RequestNo in (select ID from @table)
	 )#a
	--select * from #TransCosting
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
	--#a.LOH,
	#a.UpCharge,#a.Fishprice into #total from (
	select RequestNo,Formula, cast((sum(case when CompoType in ('RM') then minprice else 0 end)) as float)as 'RM',
	cast((sum(case when CompoType in ('Ing') then minprice else 0 end)) as float) as 'Ing',
	cast((sum(case when CompoType in ('margin') then minprice else 0 end)) as float) as 'Margin',
	cast((sum(case when CompoType in ('primary packaging') then minprice else 0 end)) as float) as 'PrimaryPkg',
	cast((sum(case when CompoType in ('secondary packaging') then minprice else 0 end)) as float) as 'SecondaryPkg',
	--cast((sum(case when CompoType in ('LOH') then minprice else 0 end)) as float) as 'LOH',
	cast((sum(case when CompoType in ('upcharge') then minprice else 0 end)) as float) as 'UpCharge',
	cast((max(case when CompoType in ('X') then 
	(case Unit when 'KG' then minprice * 1000 when 'MT' then minprice end)
	 else 0 end)) as float) as 'Fishprice' from #temp where RequestNo in (select ID from @table)
	group by RequestNo,Formula)#a left join #TransFormulaHeader b on #a.RequestNo=b.RequestNo and #a.Formula=b.Formula 
	--select ISNUMERIC('0.63x')
	--select NEWID()
	If Object_ID('tempdb..#tempClass')  is not null  drop table #tempClass
	select a.RequestNo,a.Formula,a.Amount,b.Description as 'Name' into #tempClass from #TransCosting a left join MaterialClass b on substring(a.SAPMaterial,2,1)=b.Id 
	where RequestNo in (select distinct #total.RequestNo from #total) and LOWER(component) in ('secondary packaging') 
	
 
	declare @cols nvarchar(max),@query nvarchar(max)
	SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.Name) 
            FROM #tempClass c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
If Object_ID('tempdb..##MyTable')  is not null  drop table ##MyTable 
set @query = N'SELECT requestno,Formula,' + @cols + ' into ##MyTable from 
            (
                select requestno,Formula,Amount,Name

                from #tempClass
           ) x
            pivot 
            (
                 min(Amount)
                for Name in (' + @cols + ')
            ) p '

execute (@Query)
--EXEC dbo.spDynamicPivot @Query = 'SELECT * FROM ##MyTable', @AggregateFunction = 'min(Amount)', @ValueColumn = 'Name', @ResultTable = '#MyTable'

	select #b.*,##MyTable.* from(select 
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
	#a.PerMargin,#a.ExchangeRate,#a.Fishprice,#a.CreateOn,upper(#a.name)name
	,c.RequestNo 'TechnicalNo',c.RequestDate,c.RequireDate from (select 
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
	isnull((select LBRate from #LBRate where #LBRate.Formula=a.Formula and #LBRate.RequestNo=a.RequestNo),'0') as 'LOH',
	a.UpCharge,
	RM+Ing as 'RmIng',
	convert(float,0) as Loss,
	convert(float,0) as 'Margin',
	convert(float,0)  as 'FOB',
	a.Margin as 'PerMargin',ExchangeRate,a.Fishprice,b.CreateOn,isnull(dbo.fnc_getcd(b.RequestNo),'')'Name',b.RequestNo TechnicalID
	--,format(isnull(((select sum(convert(float,isnull(p.PriceOfUnit,0))) from #MasPricePolicy p where p.Formula=#total.Formula and p.RequestNo=#total.RequestNo) * 1000)/
	--(case when len(ExchangeRate)=0 or ExchangeRate = null then 1 else convert(float,ExchangeRate) end),'0') ,'#,###.00')as 'Fishprice'	
	from #total a left join #table b on a.RequestNo=b.ID)#a left join #TransTechnical c on c.ID=#a.TechnicalID)#b
	left join ##MyTable on ##MyTable.RequestNo=#b.RequestNo and ##MyTable.Formula=#b.Formula
END

go

