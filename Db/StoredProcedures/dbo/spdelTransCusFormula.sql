-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spdelTransCusFormula]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@list nvarchar(max)
 
AS
BEGIN
	--declare @Id nvarchar(max)='8035',	@list nvarchar(max)='1,2,3,4,5,6,7'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
    -- Insert statements for procedure here
	select * into #temp from TransCusFormula WHERE RequestNo = @Id

	delete #temp where ID in (select value from dbo.FNC_SPLIT(@list,','))
	delete TransCusFormula where Id in (select value from dbo.FNC_SPLIT(@list,',')) and RequestNo = @Id
END
go

