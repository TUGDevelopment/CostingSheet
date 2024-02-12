-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spcalcuformula]
	@Id nvarchar(max),
	@formula nvarchar(max),
	@user nvarchar(max),
	@type nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)=18437,@formula nvarchar(max)=1,@user nvarchar(max)='fo5910155',@type nvarchar(max)=2
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select * into #temp from dbo.FindULevel(@user) where idx in(2,3,4,5)
	declare @userlevel nvarchar(max)=(select count(*) from #temp where idx in(2,3))
	declare @PackSize int,@Company nvarchar(max)
	select @PackSize=PackSize,@Company=Company from TransCostingHeader where ID=@Id
	--select * from #temp
	if(@Company='101')
	set @PackSize = (select isnull(PackSize,'') from TransProductList where Id in (
		select PID from TransFormulaHeader where RequestNo=@Id and Formula=@formula))
	print @userlevel;
	If Object_ID('tempdb..#table')  is not null  drop table #table
	SELECT Id as 'RowID'
		  ,case when Component='' or Component is null then '-' else Component end as 'Component'
		  ,SubType
		  ,Description
		  ,Material as 'Material'
		  ,convert(nvarchar(max),case when Result in ('NaN','Infinity','') 
				then '0' else Result end) 'Result'
		  ,case when Yield in ('NaN','Infinity','','0') 
				then '100' else Yield end 'Yield'
		  ,RawMaterial
		  ,Name
		  ,PriceOfUnit
		  ,Currency
		  ,Unit
		  ,convert(float,case when ExchangeRate in ('NaN','Infinity','') 
				then '1' else ExchangeRate end) 'ExchangeRate'
		  ,BaseUnit
		  ,case when PriceOfCarton in ('NaN','Infinity','') 
				then '0' else PriceOfCarton end 'PriceOfCarton'
		  ,convert(int,Formula)'Formula',IsActive,Remark,LBOh,replace(LBRate,',','')'LBRate','' as aValidate,RequestNo into #table
	  FROM TransFormula where Formula=@formula and RequestNo=@Id
	  --select * from #table
	if(@type='2')--Ingredient
	begin
	if (@userlevel=0)
	  SELECT SubType,sum(case when PriceOfCarton in ('NaN','Infinity','') then 0 else convert(numeric(8,3),
	  ((convert(float,Result)/1000) * @PackSize)/(CAST(CAST(Yield AS decimal(38,19)) / 100 AS float) ) * BaseUnit) end)'PriceOfCarton'
	  ,@userlevel 'userlevel'
	  FROM #table where Formula=@formula and RequestNo=@Id and Component <> 'Raw Material' 
	  group by SubType
	if(@userlevel=1 or @userlevel=2 or @userlevel=3 or @userlevel=4 or @userlevel=5 or @userlevel=6 or @userlevel=7 or @userlevel=8)
	  SELECT *
		,@userlevel 'userlevel'
		FROM #table where Formula=@formula and RequestNo=@Id and Component <> 'Raw Material' 
	end
	else if (@type='3')--LOH
	begin
		 declare @table table(SubType nvarchar(max),title nvarchar(max),LBRate float,userlevel nvarchar(20),
		 SellingUnit nvarchar(max))
		 declare @sum float = (select sum(convert(float,Result))'Result' from TransFormula where Formula=@formula and LBRate<>'' and RequestNo=@Id)
		 insert into @table (SubType,title,LBRate,userlevel,SellingUnit)
		 select SubType,'Labor & Overhead', case when @sum<>0 then 
		 sum(convert(float,a.LBRate)*convert(float,Result))/@sum else 0 end'LBRate',@userlevel 'userlevel',
		 '' FROM #table a 
		 --left join MasLaborOverhead b on b.LBCode=a.LBOh 
		 where Formula=@formula and RequestNo=@Id --and Component = 'Raw Material' 
		 and ISNUMERIC(a.LBRate)=1--and a.LBRate like '%[^0-9.]%' 
		 --and b.Company in (select Company from TransCostingHeader where id=@Id) 
		 group by SubType

		 declare @LBRate float =(select top 1 LBRate from @table)
		 insert into @table (title,LBRate,userlevel,SellingUnit)
		 select [Description],Quantity,
		 --case when SellingUnit ='%' then @LBRate * (convert(float,Quantity)/100) else Quantity end,
		 @userlevel,SellingUnit
		 from TransCosting where RequestNo=@Id and Component='Labor&Overhead' and Formula=@formula
		 ORDER BY 
			CASE WHEN SellingUnit='%' THEN 1 else 99 END Desc
		 select * from @table
	end

	--if(@userlevel=1)
	--	 select LBType+'('+LBName+','+PackSize+')' as'LBName',a.LBRate,@userlevel 'userlevel' FROM TransFormula a left join MasLaborOverhead b
	--	 on b.LBCode=a.LBOh where Formula=@formula and RequestNo=@Id and Component = 'Raw Material'  and len(a.LBOh)>0
	--	 and b.Company in (select Company from TransCostingHeader where id=@Id)
	  --group by SubType
END


go

