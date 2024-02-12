-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spGetProductCategory] 
	-- Add the parameters for the stored procedure here
	@Bu nvarchar(max),
	@User nvarchar(max),
	@UserType nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @User nvarchar(max)='FO5910155',@Company nvarchar(max)='101',@UserType nvarchar(max)='3;4;5'
	SET NOCOUNT ON;
	if(select count(*) from dbo.FNC_SPLIT(@usertype,';'))>1
	--declare @UserType nvarchar(max)= (select Usertype from ulogin where [user_name]=@User)
	select Code,Title as 'Name' from MasPlant where Company=@Bu and dbo.fnc_checktype(@UserType,usertype)>0
	else
	select * from MasCompany where Code in (select distinct value from dbo.FNC_SPLIT(@Bu,'|')) order by Code
	
END
go

