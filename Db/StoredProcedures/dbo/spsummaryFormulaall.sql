-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spsummaryFormulaall]
	-- Add the parameters for the stored procedure here
@user_name nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.ID,a.Company,
	a.MarketingNumber,
	a.RDNumber,
	a.StatusApp,
	a.CanSize,
	a.PackSize,
	b.Customer,
	b.RequestNo from TransCostingHeader a left join TransTechnical b on b.ID=a.RequestNo --where a.ID=1040
END


go

