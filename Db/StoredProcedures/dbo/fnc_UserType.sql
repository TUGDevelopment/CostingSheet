
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create FUNCTION [dbo].[fnc_UserType](@usertype nvarchar(max),@User nvarchar(max)
)
RETURNS @Mytbl  TABLE (idx smallint, ulevel nvarchar(max),editor nvarchar(max)--,Bu nvarchar(max)
,Sublevel nvarchar(max),SubApp nvarchar(max))  
AS
BEGIN
	--declare @usertype nvarchar(max)='1',@User nvarchar(max)='FO5910155'
		insert into @Mytbl 
		select c.ApprovId,
		dbo.fnc_getuser(c.Id,#a.plant,@usertype),
		dbo.fnc_userlist(c.Id,@User,@usertype),
		--dbo.fnc_getBu(Bu,c.IsBu),
		c.Id,dbo.fnc_applist(c.Id,c.ApprovId,@User) from(select empid,[FirstName]
			,[LastName],a.Sublevel,b.BU,b.Plant from masApprovAssign a left join ulogin b on b.user_name=a.empid 
			where a.empid=@User --and b.BU=@Bu
			)#a
			left join MasSublevel c on c.Id=#a.Sublevel
 
 
RETURN 
END


go

