-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create FUNCTION [dbo].[fnc_applist](
	@Sublevel nvarchar(max),
	@ApprovId nvarchar(max),
	@User nvarchar(max))
RETURNS  nvarchar(max)  
AS
BEGIN
	--declare @Sublevel nvarchar(max)=7,@User nvarchar(max)='CP4790028'
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
	select ApprovId from(select empid,b.ApprovId,a.Sublevel,a.Id 
	from masApprovAssign a left join MasSublevel b on b.Id=a.Sublevel)#a where #a.Sublevel in (
	select c.Id from @t c)
	DECLARE @cols AS NVARCHAR(MAX);
		SET @cols = dbo.FNC_STUFF(@temp)
	end else
	set @cols=@ApprovId
	--print @cols;
RETURN @cols
END

go

