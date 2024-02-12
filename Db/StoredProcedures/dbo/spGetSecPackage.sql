-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spGetSecPackage
	@Company nvarchar(max)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select ID,SAPMaterial,Name,Price,Currency,Unit from MasPrice 
	where Company=@Company and SAPMaterial like '5%' and substring(SAPMaterial,2,1) in ('F')
END
go

