CREATE PROCEDURE [dbo].[spbuildMapCosting] 
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
	/*SELECT * into #temp from TransEditCosting where RequestNo=@GeID 
	and SiteId=@SiteId
	DECLARE @cols AS NVARCHAR(MAX),
	@query  AS NVARCHAR(MAX);
	--SET @cols = STUFF((SELECT  ',' + QUOTENAME(formula) from #temp
	SET @cols = STUFF((SELECT distinct ',' + (Costingsheet) from #temp
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)') 
			,1,1,'')
	print @cols;*/

declare @index int = 1
declare @cur nvarchar(max),@SellingUnit nvarchar(max),@VarietyPack nvarchar(max)
declare cur_temp CURSOR FOR
--select distinct value from dbo.FNC_SPLIT(@cols,',')
SELECT Costingsheet,isnull(SellingUnit,0),isnull(VarietyPack,'') from TransEditCosting where RequestNo=@GeID 
	and SiteId=@SiteId
open cur_temp

FETCH NEXT FROM cur_temp INTO @cur,@SellingUnit,@VarietyPack
WHILE @@FETCH_STATUS = 0
BEGIN
	  declare @formula nvarchar(max),@PackSize nvarchar(max),@Packaging nvarchar(max),@NW nvarchar(max),@Unit nvarchar(max) 
	  select @formula=Formula,@ID=RequestNo from TransFormulaHeader where CostNo in (@cur)
	  select @packSize=PackSize,@Packaging=Packaging,@NW=(
	  case when CHARINDEX('|',Netweight)>0 then SUBSTRING('80|Grams',1,CHARINDEX('|',Netweight)-1) else '0' end)
	  from TransCostingHeader where id=@ID
	  --select top 10 * from TransFormulaHeader order by id desc
	  --select CHARINDEX('|','80|Grams'),SUBSTRING('80|Grams',1,CHARINDEX('|','80|Grams')-1)
	  insert into TransFormulaHeader
	  select Name,Customer,Code,RefSamples,@index,1,0,@runid+ format(@index,'00'),Revised,'','',@SellingUnit,RequestNo,isnull(Packaging,@Packaging),isnull(NW,@NW),
	  isnull([PackSize],@packSize),@NewID 
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
	if(len(@VarietyPack)>0) begin
	insert into TransCosting
	select Component,SAPMaterial,[Description],@SellingUnit,PriceUnit,(convert(float,@SellingUnit)*convert(float,PriceUnit)),Per,SellingUnit,Loss,getdate(),@user,null,null,@index,@NewID 
		FROM TransCosting where RequestNo=@ID and Formula in (@formula) --and lower(Component) in ('upcharge','primary packaging')
		--select * from TransCosting where lower(Component) in ('upcharge','primary packaging')
	end 
	else begin
	insert into TransCosting
	select Component,SAPMaterial,[Description],Quantity,PriceUnit,Amount,Per,SellingUnit,Loss,getdate(),@user,null,null,@index,@NewID 
		FROM TransCosting where RequestNo=@ID and Formula in (@formula)
	end
	update TransEditCosting set Result=@runid+''+ format(@index,'00') 
	where CostingSheet in (@cur)
	and RequestNo=@GeID
	set @index= @index + 1
	FETCH NEXT FROM cur_temp INTO @cur,@SellingUnit,@VarietyPack
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

--select * from  [dbo].[TransTechnical]  where ID=18461

go

