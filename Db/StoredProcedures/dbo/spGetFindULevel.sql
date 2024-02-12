-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spGetFindULevel
	-- Add the parameters for the stored procedure here
	@user_name nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	If Object_ID('tempdb..#find')  is not null  drop table #find
	select idx,ulevel,editor,Sublevel,SubApp from dbo.FindULevel(@user_name)
END
go

