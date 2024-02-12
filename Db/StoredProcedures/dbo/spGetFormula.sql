-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetFormula]
	-- Add the parameters for the stored procedure here
	@Data nvarchar(max),
	@user nvarchar(max)
AS
BEGIN
	--select * from TransCostingHeader
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Data nvarchar(max)=14602,@user nvarchar(max)='FO5910155'
	--select * from TransFormula where RequestNo=@Data
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
    -- Insert statements for procedure here
	If Object_ID('tempdb..#find')  is not null  drop table #find
	select * into #find from dbo.FindULevel(@user) where idx in(2,3,0)
	declare @PackSize int =(select PackSize from TransCostingHeader where ID=@Data)
	SELECT ID
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
		  ,case when PriceOfCarton in ('NaN','Infinity','','0') 
				then '0' 
				when convert(float,PriceOfCarton)=0 then '0' else PriceOfCarton end 'PriceOfCarton'
				--else convert(nvarchar(max),case when Currency='USD' then convert(float,PriceOfCarton)/convert(float,case when ExchangeRate='0' then '1' else convert(nvarchar(max),ExchangeRate) end) 
				--else PriceOfCarton end) end 'PriceOfCarton'
		  ,convert(int,Formula)'Formula',IsActive,Remark,LBOh,LBRate,'' as aValidate into #temp
	  FROM TransFormula where RequestNo=@Data
	  if (select count(*) from #find where idx in (0))>0
	  begin
	  	If Object_ID('tempdb..#table')  is not null  drop table #table
		select * into #table from #temp where Component ='Raw Material' 
		--select * from #temp
		--ingredients
		insert into #table(Component,SubType,[Description],GUnit,Yield,PriceOfCarton,Formula,IsActive,SAPMaterial,Remark,LBOh,LBRate,aValidate)
		select '',SubType,SubType,
		sum(convert(float,GUnit))'GUnit',
		sum(convert(float,Yield))'Yield',
		sum(convert(float,(GUnit/1000) * @PackSize)/(CAST(CAST(case when Yield='0' then '1' else Yield end AS decimal(38,19)) / 100 AS float)) * BaseUnit)'PriceOfCarton',
		Formula,
		IsActive,'','','','','0'
		from #temp where Component <> 'Raw Material' 
		group by subtype,Formula,IsActive
		select convert(int,ROW_NUMBER() OVER (ORDER BY formula)) as 'ID',Component
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
		  ,PriceOfCarton
		  ,Formula,IsActive,Remark,LBOh,LBRate,'0' as aValidate,'' as Mark
	 from #table
	 end
	 else
	 select *,'' as Mark from #temp
END


go

