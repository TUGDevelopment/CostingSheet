
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCostingItem]
	-- Add the parameters for the stored procedure here
	@param nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @param nvarchar(max)=1212
    -- Insert statements for procedure here
	select ID,Component as 'SubType',SAPMaterial as 'Code',Description as 'Name',
            Quantity,(case when PriceUnit is null or PriceUnit='' then '0' else PriceUnit end) as 'PriceUnit',
			Amount,Per,SellingUnit as 'Currency',Loss,Formula,'' as Mark from TransCosting where RequestNo=@param
END

go

