-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spselectsummary] 
	-- Add the parameters for the stored procedure here
	@username nvarchar(max),
	@Id nvarchar(max)
AS
BEGIN
	--declare @Id nvarchar(max)='1040'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select a.MarketingNumber,a.RDNumber,
	 b.* into #temp from TransCostingHeader a left join TransFormula b on b.RequestNo=a.ID
	 where a.ID=@Id
	DECLARE @cols AS NVARCHAR(MAX),
		@query  AS NVARCHAR(MAX);

	SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(a.formula) 
				FROM #temp a 
				FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)') 
			,1,1,'')

	print @cols;
	set @query = 'SELECT ROW_NUMBER() OVER (ORDER BY Material) AS RowID,Component,SubType,Description,Material,' + @cols + ' from 
				(
					select Component,SubType,Description,Material,Result,formula
					from #temp
			   ) x
				pivot 
				(
					 max(Result)
					for formula in (' + @cols + ')
				) p '
	execute(@query)
END


go

