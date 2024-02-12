Create FUNCTION [dbo].[fnc_getusertype]
(
	@u nvarchar(max)
)
RETURNS nvarchar(max) 
AS
BEGIN
	-- Declare @t nvarchar(max)='0;1'
	DECLARE @Result nvarchar(max)
	set @Result = (select usertype from ulogin where [user_name] in (select value from dbo.FNC_SPLIT(@u,';'))) 
	--print @Result
	-- Return the result of the function
	RETURN @Result

END
 
go

