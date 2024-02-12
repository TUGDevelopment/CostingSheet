-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fnc_editor](
	@User nvarchar(max),
	@Usertype nvarchar(max))
RETURNS  nvarchar(max)  
AS
BEGIN
	--declare @User nvarchar(max)='FO5910155'
	declare @temp  table(empid nvarchar(max))

	insert into @temp
	SELECT dbo.fnc_userlist(b.id,@User,@Usertype)
	FROM MasApprovAssign a left join MasSublevel b on b.Id=a.Sublevel
	WHERE  EmpId=@User

	DECLARE @cols AS NVARCHAR(MAX);
		SET @cols = STUFF((SELECT distinct ',' + (convert(nvarchar(max),c.empid)) 
            FROM @temp c   
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
RETURN @cols
END


go

