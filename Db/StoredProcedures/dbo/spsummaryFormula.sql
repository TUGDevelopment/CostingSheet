-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spsummaryFormula]
	-- Add the parameters for the stored procedure here
@Id nvarchar(max),
@username nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.* from TransCostingHeader a left join TransTechnical b on b.ID=a.RequestNo where a.ID=@Id
END


go

