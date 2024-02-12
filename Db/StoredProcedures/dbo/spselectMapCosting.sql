CREATE PROCEDURE [dbo].[spselectMapCosting] 
	@value nvarchar(max),
	@key nvarchar(1),
	@user nvarchar(max),
	@GeID nvarchar(max),
	@ID nvarchar(max),
	@SiteId nvarchar(max)
AS
BEGIN
	--declare @value nvarchar(max)='179',@key nvarchar(1)=1,@user nvarchar(max)='fo5910155',@GeID nvarchar(max)='255',@ID nvarchar(max)='7486',@SiteId nvarchar(max)='7522'
	declare @NewID int=0
	declare @result nvarchar(max)=(select isnull(Result,'0') from TransEditCosting where ID=@value)
	IF (@result <> '0') GOTO jumploop
	declare	@Code nvarchar(max),
	@Costing nvarchar(max),
	@Company nvarchar(3),
	@TechnicalID nvarchar(max)
	select @TechnicalID=b.ID,@Code=b.RequestNo,@Company=b.Company from TransTechnical b where b.Id=@GeID
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	SET NOCOUNT ON;
	declare @RequestNo nvarchar(max),@runid nvarchar(max)
	set @runid = (select MarketingNumber from TransCostingHeader where ID=@SiteId)

    -- Insert statements for procedure here
	set @RequestNo = @GeID
	set @NewID=@SiteId
	SELECT * into #temp from TransEditCosting where RequestNo=@GeID 
	and SiteId=@SiteId
		DECLARE @cols AS NVARCHAR(MAX),
		@query  AS NVARCHAR(MAX);
	--SET @cols = STUFF((SELECT  ',' + QUOTENAME(formula) from #temp
	SET @cols = STUFF((SELECT distinct ',' + (Costingsheet) from #temp
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)') 
			,1,1,'')
	print @cols;

declare @index int = 1
declare @cur nvarchar(max)
declare cur_temp CURSOR FOR
select distinct value from dbo.FNC_SPLIT(@cols,',')
open cur_temp

FETCH NEXT FROM cur_temp INTO @cur
WHILE @@FETCH_STATUS = 0
BEGIN
	  declare @formula nvarchar(max) 
	  select @formula=Formula,@ID=RequestNo from TransFormulaHeader where CostNo in (@cur)
	  --select * from TransFormulaHeader
	  insert into TransFormulaHeader
	  select Name,Customer,Code,RefSamples,@index,1,0,@runid+ format(@index,'00'),Revised,'',@NewID 
	  from TransFormulaHeader where CostNo in (@cur) and RequestNo=@ID

-- formula
	  insert into TransFormula
	  select Component,SubType,[Description],Material,Result,Yield,RawMaterial,Name
      ,PriceOfUnit
      ,Currency
      ,Unit
      ,ExchangeRate
      ,BaseUnit
      ,PriceOfCarton
      ,@index,IsActive,Remark,LBOh,LBRate,@NewID from TransFormula where RequestNo=@ID and Formula in (@formula)
-- costing
	insert into TransCosting
	select Component,
	SAPMaterial,
	[Description],
	Quantity,
	PriceUnit,
	Amount,
	Per,
	SellingUnit,
	Loss,
	getdate(),
	@user,
	null,
	null,
	@index,
	@NewID 
	FROM TransCosting where RequestNo=@ID and Formula in (@formula)
	
	update TransEditCosting set Result=@runid+''+ format(@index,'00') 
	where CostingSheet in (@cur)
	and RequestNo=@GeID
	set @index= @index + 1
	FETCH NEXT FROM cur_temp INTO @cur
END

CLOSE cur_temp
DEALLOCATE cur_temp
----update price
--	DECLARE @curid int,@Material nvarchar(max)
--	DECLARE cur CURSOR FOR SELECT Id,Material FROM TransFormula where RequestNo = @SiteId and Material <>''
--	OPEN cur

--	FETCH NEXT FROM cur INTO @curid,@Material
--	WHILE @@FETCH_STATUS = 0 BEGIN
--			DECLARE @MyTableVar table(
--	PriceOfUnit nvarchar(max), 
--	Currency nvarchar(50),	 
--	Unit nvarchar(50),
--	ExchangeRat float,
--	BaseUnit float,
--	tonUnit float,
--    Company nvarchar(50),  
--    Material nvarchar(50),  
--    [Description] nvarchar(max));
--	declare @param nvarchar(max)= @company +'|'+ @Material +'|'+ @geid
--	--declare @param nvarchar(max) = '103|14L110000009|255'
--	declare @sql nvarchar(max) ='exec dbo.spRequestRate'''+ @param +''''
--	insert into @MyTableVar exec(@sql)
--	update t set t.PriceOfUnit=b.PriceOfUnit
--	from TransFormula t left join @MyTableVar b on t.Material=b.Material where Id=@curid 
--	--select * from TransFormula  
--	-- call your sp here
--		FETCH NEXT FROM cur INTO @curid,@Material 
--	END

--	CLOSE cur    
--	DEALLOCATE cur
jumploop:
if (@NewID=0)
	set @NewID=@ID
select @NewID
END

--update TransEditCosting set Result=0 where ID=163
go

