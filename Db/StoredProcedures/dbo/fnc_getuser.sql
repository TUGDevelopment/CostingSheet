CREATE FUNCTION [dbo].[fnc_getuser](
	@sublevel nvarchar(max),
	@plant nvarchar(max),
	@usertype nvarchar(max))
RETURNS  nvarchar(max)  
AS
BEGIN
	--declare @sublevel nvarchar(max)='1,2',@Bu nvarchar(max)='102'
	declare @temp TableType
	DECLARE @id nvarchar(max)
	DECLARE cur CURSOR FOR SELECT value FROM dbo.FNC_SPLIT(@plant,';')
	OPEN cur

	FETCH NEXT FROM cur INTO @id 

	WHILE @@FETCH_STATUS = 0 BEGIN
		insert into @temp
		SELECT EmpId
		FROM MasApprovAssign a inner join ulogin b on b.user_name=a.EmpId
		WHERE a.Sublevel in (select value from dbo.FNC_SPLIT(@sublevel,',')) 
		and dbo.fnc_checktype(b.Plant,@id)>0 and EmpId not in(select LocationName from @temp)  -- call your sp here
		and EmpId in (select user_name from ulogin where dbo.fnc_checktype(usertype,@usertype)>0)
		FETCH NEXT FROM cur INTO @id 
	END

	CLOSE cur    
	DEALLOCATE cur

	DECLARE @cols AS NVARCHAR(MAX);
		SET @cols = dbo.FNC_STUFF(@temp)
	--select @Cols;
RETURN @cols
END

go

