-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetFactory] 
	-- Add the parameters for the stored procedure here
	@username nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @username nvarchar(max)='FO5910155'
	declare @userType nvarchar(max),@BU nvarchar(max)
	select @userType=userType,@BU=BU from ulogin where [user_name]=@username

	select * from(
	select code,Title from MasPlant where dbo.fnc_checktype(Company,@BU)> 0 and dbo.fnc_checktype(usertype,@userType)>0 group by code,Title)#a 
    -- Insert statements for procedure here
 
END
go

