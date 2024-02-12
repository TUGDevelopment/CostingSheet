-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetApprovAssign] 
	-- Add the parameters for the stored procedure here
	@UserType nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @UserType nvarchar(max)='0;3;7'
	SET NOCOUNT ON;
	declare @Mytable TABLE (id int, empid nvarchar(max)
		,FirstName nvarchar(max),LastName nvarchar(max),sublevel nvarchar(max),UserType nvarchar(max))
 
		insert into @Mytable
		select Id,empid,firstname,lastname,a.Sublevel,b.usertype from MasApprovAssign a left join ulogin b on b.[user_name]=a.empid
		where empid not in (select EmpId from @Mytable) order by a.sublevel -- call your sp here
	select Id,empid,firstname,lastname,Sublevel from @Mytable where (dbo.fnc_checktype(usertype,@UserType)> 0)
END
go

