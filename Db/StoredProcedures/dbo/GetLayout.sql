CREATE PROCEDURE [dbo].[GetLayout]
	
	@UserID int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   SELECT UserLayout FROM Layout Where UserID = @UserID


END


go

