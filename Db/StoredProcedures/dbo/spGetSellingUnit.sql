-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spGetSellingUnit 
	-- Add the parameters for the stored procedure here
	@usertype nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select ID,Name from MasSellingUnit where dbo.fnc_checktype(usertype,@usertype)>0
	union select 0,''
END
go

