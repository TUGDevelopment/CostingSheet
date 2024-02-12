-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create FUNCTION [dbo].[fnc_Approv](@User nvarchar(max)
)
RETURNS @Mytbl  TABLE (idx smallint,Sublevel nvarchar(max),SubApp nvarchar(max))  
AS
BEGIN
	--declare @User nvarchar(max)='FO5910155'
 	insert into @Mytbl 
			select ApprovId,#a.Sublevel,dbo.fnc_applist(c.Id,c.ApprovId,@User) subapp  from(select a.Sublevel
			from masApprovAssign a left join ulogin b on b.user_name=a.empid 
			where a.empid=@User --and b.BU=@Bu
			)#a left join MasSublevel c on c.Id=#a.Sublevel
 
RETURN 
END
go

