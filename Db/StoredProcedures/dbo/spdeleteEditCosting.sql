-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spdeleteEditCosting] 
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@user nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @userlevel nvarchar(max)=(select userlevel from ulogin where [user_name]=@user)
	print @userlevel;
	if(@userlevel='1')
	DELETE TransEditCosting WHERE RequestNo = @Id
END


go

