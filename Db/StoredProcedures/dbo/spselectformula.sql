-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spselectformula]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)='16265'
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select a.Company,a.MarketingNumber,a.RDNumber,a.PackSize,b.*,
	(case when (select count(Id) from MasPricePolicy c where c.Material=b.Material and c.Material in (
	select Material from MasFixFishCal	
	))>0 then 0 else 1 end)'Mark'
	,convert(float,replace((case when ISNUMERIC(b.priceofunit)=0 then '0' else 
	convert(float,replace(b.priceofunit,',','')) /(case when Currency='THB' then 
	(case when a.ExchangeRate='0' then '1' else a.ExchangeRate end) else '1' end) end),',',''))'ofunit'
	,a.ExchangeRate as 'Rate',(case when Currency='THB' then a.ExchangeRate else '1' end) 'XX',
	convert(float,(case when ISNUMERIC(replace(b.priceofunit,',',''))=0 then '0' else convert(float,replace(b.priceofunit,',','')) end))'ofunitxx'
	into #temp
	from TransFormula b left join TransCostingHeader a on a.ID = b.RequestNo where b.RequestNo = @Id

	select *,
	case when Mark=0 then(case Unit when 'KG' then ofunit * 1000 when 'MT' then ofunit end) else 0 end 'Fishprice' from #temp
END
 
go

