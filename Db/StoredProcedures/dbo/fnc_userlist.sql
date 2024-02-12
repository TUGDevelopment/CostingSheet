-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fnc_userlist](
	@Sublevel nvarchar(max),
	--@ApprovId nvarchar(max),
	@User nvarchar(max),
	@usertype nvarchar(max))
RETURNS  nvarchar(max)  
AS
BEGIN
	--declare @Sublevel nvarchar(max)=3,@User nvarchar(max)='fo5910155',@usertype nvarchar(max)=0

	declare @t  table(id nvarchar(max))
	DECLARE @id INT = @Sublevel
	;WITH ret AS(
			SELECT  *
			FROM    MasSublevel
			WHERE   ID = @ID
			UNION ALL
			SELECT  t.*
			FROM    MasSublevel t INNER JOIN
					ret r ON t.SubLevel = r.ID
	)
	insert into @t 
	SELECT  id FROM ret
	declare @count int= (select count(Id) from @t)
	if (@count>1)
	begin
	declare @temp  tabletype
	insert into @temp 
	select empid from(select empid,[FirstName]
      ,[LastName],a.Sublevel,a.Id from masApprovAssign a left join ulogin b on b.user_name=a.empid
	  where a.EmpId in (select [user_name] from ulogin where dbo.fnc_checktype(usertype,@usertype)>0) )#a where #a.Sublevel in (
	select c.Id from @t c)
	DECLARE @cols AS NVARCHAR(MAX);
		SET @cols = dbo.FNC_STUFF(@temp)
	end else
	set @cols=@User
	--print @cols;
RETURN @cols
END

go

