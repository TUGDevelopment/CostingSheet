CREATE PROCEDURE [dbo].[SaveLayout]
	-- Add the parameters for the stored procedure here
	@UserID int, 
	@UserLayout nvarchar(max)
AS
BEGIN
	
--	SET NOCOUNT ON;

   
	UPDATE Layout
SET UserLayout = @UserLayout
WHERE (UserID = @UserID)

END


go

