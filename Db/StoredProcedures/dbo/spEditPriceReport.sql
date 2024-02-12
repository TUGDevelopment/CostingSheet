-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spEditPriceReport] 
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max)
AS
BEGIN
	--declare @Id nvarchar(max)=109
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
    -- Insert statements for procedure here
	select convert(nvarchar(max),Series) as 'Series' into #temp from TransEditCosting where RequestNo=@Id
	group by Series

	DECLARE @cols AS NVARCHAR(MAX),
		@query  AS NVARCHAR(MAX);

	SET @cols = STUFF((SELECT  '|' + (Series) 
				FROM #temp
				FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)') 
			,1,1,'')
	select @cols 'cols'
END


go

