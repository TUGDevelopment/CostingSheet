-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spgetselectcost]
	-- Add the parameters for the stored procedure here
	@param nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(len(@param)<18)
    -- Insert statements for procedure here
	select MarketingNumber from TransCostingHeader Where Id in (
	select RequestNo from TransFormulaHeader where Code=@param)
	else if Substring(@param,1, 2) ='3V'
		select MarketingNumber from TransCostingHeader where VarietyPack=@param
END
go

