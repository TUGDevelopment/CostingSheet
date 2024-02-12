-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCostingMat]
	-- Add the parameters for the stored procedure here
	@Costing nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Costing nvarchar(max)='GP65300401'
    -- Insert statements for procedure here
	select * from (SELECT * from t1_TransMapCosting 
		union SELECT * from t2_TransMapCosting)#a where Costing=@Costing
END

go

