-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetFormulaQuo]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@username nvarchar(max)
AS
BEGIN
	--select * from TransCostingHeader where MarketingNumber like 'GP627308'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @ID nvarchar(max)=15871,@username nvarchar(max)='fo5910155'
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
    -- Insert statements for procedure here
	If Object_ID('tempdb..#find')  is not null  drop table #find
	select * into #find from dbo.FindULevel(@username) where idx in(2,3,0)
	declare @Exchangerate float
	set @Exchangerate=(select case when ExchangeRate='0' then 1 else convert(float,ExchangeRate) end from TransCostingHeader where id=@ID)
	print @exchangerate;
	SELECT Id as 'RowID'
		  ,case when Component='' or Component is null then '-' else Component end as 'Component'
		  ,SubType
		  ,Description
		  ,Material as 'SAPMaterial'
		  ,convert(float,case when Result in ('NaN','Infinity','') 
				then '0' else Result end) 'GUnit'
		  ,case when Yield in ('NaN','Infinity','') 
				then '0' else Yield end 'Yield'
		  ,RawMaterial
		  ,Name
		  ,PriceOfUnit
		  ,Currency
		  ,Unit
		  ,ExchangeRate
		  ,BaseUnit
		  ,isnull(case when PriceOfCarton in ('NaN','Infinity','') 
				then '0' else PriceOfCarton end,'') 'PriceOfCarton'
		  ,convert(int,Formula)'Formula',IsActive,Remark,LBOh,LBRate,'' as aValidate into #temp
	  FROM TransFormula where RequestNo=@ID
	  --if (select count(*) from #find where idx in (0))>0
	  --begin
	  	If Object_ID('tempdb..#table')  is not null  drop table #table
		select * into #table from #temp 
		--where Component ='Raw Material' 
		--select * from #temp
		--ingredients
		--insert into #table(Component,SubType,[Description],GUnit,Yield,PriceOfCarton,Formula,IsActive,SAPMaterial,Remark,LBOh,LBRate,aValidate)
		--select '',SubType,SubType,
		--sum(convert(float,GUnit))'GUnit',
		--sum(convert(float,Yield))'Yield',
		--sum(convert(float,PriceOfCarton))'PriceOfCarton',
		--Formula,
		--IsActive,'','','','','0'
		--from #temp where Component <> 'Raw Material' 
		--group by subtype,Formula,IsActive
		insert into #table (Component,PriceOfCarton,aValidate,Formula,Currency,ExchangeRate) 
			select 'Labor & Overhead',isnull(LBRate,''),'',Formula,'THB','1' from #table where len(LBRate) >0
		select convert(int,ROW_NUMBER() OVER (ORDER BY formula)) as 'RowID',Component
		  ,SubType
		  ,Description
		  ,SAPMaterial
		  ,GUnit
		  ,Yield
		  ,RawMaterial
		  ,Name
		  ,PriceOfUnit
		  ,Currency
		  ,Unit
		  ,ExchangeRate
		  ,BaseUnit
		  ,isnull(PriceOfCarton,'') as 'PriceOfCarton'
		  ,Formula,IsActive,Remark,LBOh,LBRate,'0' as aValidate,PriceOfCarton/@Exchangerate as 'PriceOfCartonusd','' as 'Per',
		  case when Component='Labor & Overhead' then 'L' else 'R' end as mark,@Exchangerate as 'Rate'
	 from #table union
	 select 90001+ID,Component,'',
	 Description,
	 SAPMaterial,
	 Quantity,
	 '','','',
	 PriceUnit,SellingUnit,'','','',
	 case when Component in ('primary packaging','secondary packaging') then convert(float,Quantity) * convert(float,PriceUnit) else Amount end
	 ,Formula,0,'','','','',Amount/@Exchangerate as 'PriceOfCartonusd',Per,'X',@Exchangerate as 'Rate' from TransCosting where RequestNo=@ID 
	 --end
END 
go

