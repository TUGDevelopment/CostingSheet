-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spselectformuladisplay]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)='1038'
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select b.Component,
	b.SubType,
	b.Description,
	b.Material,
	b.Result,
	b.Yield,
	b.PriceOfUnit,
    b.Currency,
    b.Unit,
    b.ExchangeRate,
    b.BaseUnit,
    case when b.PriceOfCarton='NaN' or b.PriceOfCarton='Infinity'
	or b.PriceOfCarton='' then '0' else b.PriceOfCarton end 'PriceOfCarton',
    Formula,
    IsActive into #temp from TransFormula b where b.RequestNo = @Id
	If Object_ID('tempdb..#table')  is not null  drop table #table
	select * into #table from #temp where Component ='Raw Material' 
	--select * from #temp
	--ingredients
	insert into #table(Component,SubType,[Description],Result,PriceOfCarton,Formula,IsActive)
	select Component,SubType,SubType,
	sum(convert(Numeric,Result))'Result',
	sum(convert(Numeric,PriceOfCarton))'PriceOfCarton',
    Formula,
    IsActive
	from #temp where Component <> 'Raw Material' 
	group by Component,subtype,Formula,IsActive
	select ROW_NUMBER() OVER (ORDER BY formula) AS Id,*,
	0'BahtCarton',
	0'usdCarton'  from #table
END


go

