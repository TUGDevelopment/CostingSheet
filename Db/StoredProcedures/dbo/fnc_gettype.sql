CREATE FUNCTION [dbo].[fnc_gettype]
(
	@t nvarchar(max)
)
RETURNS nvarchar(max) 
AS
BEGIN
	-- Declare @t nvarchar(max)='0;1'
	DECLARE @Result nvarchar(max)
	declare @table tabletype;
	--set @t=replace(@t,',',';')
	-- Add the T-SQL statements to compute the return value here
	insert into @table
	select Name from masusertype where id in (select value from dbo.FNC_SPLIT(@t,';')) 
	set @Result = dbo.fnc_STUFF(@table)
	--print @Result
	-- Return the result of the function
	RETURN @Result

END
 
go

