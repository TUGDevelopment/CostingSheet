-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create FUNCTION [dbo].[fnc_ULevel](@User nvarchar(max)
)
RETURNS @Mytbl  TABLE (idx smallint, ulevel nvarchar(max),editor nvarchar(max)--,Bu nvarchar(max)
,Sublevel nvarchar(max),SubApp nvarchar(max),usertype nvarchar(max))  
AS
BEGIN
declare @usertype nvarchar(max)
set @usertype =(select usertype from ulogin where [user_name]=@User)
declare @idx nvarchar(max)
declare cur_Employee CURSOR FOR

SELECT value from dbo.FNC_SPLIT(@usertype,';')

open cur_Employee

FETCH NEXT FROM cur_Employee INTO @idx
WHILE @@FETCH_STATUS = 0
BEGIN
insert into @Mytbl 
	select c.ApprovId,
	dbo.fnc_getuser(c.Id,#a.BU,@idx),
	dbo.fnc_userlist(c.Id,@User,@idx),
	--dbo.fnc_getBu(Bu,c.IsBu),
	c.Id,dbo.fnc_applist(c.Id,c.ApprovId,@User),@idx from(select empid,[FirstName]
      ,[LastName],a.Sublevel,b.BU from masApprovAssign a left join ulogin b on b.user_name=a.empid 
	  where a.empid=@User --and b.BU=@Bu
	  )#a
	  left join MasSublevel c on c.Id=#a.Sublevel

	FETCH NEXT FROM cur_Employee INTO @idx
END

CLOSE cur_Employee
DEALLOCATE cur_Employee
 
RETURN 
END


go

