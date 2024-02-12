

CREATE FUNCTION [dbo].[fnc_settype]
(
	@t nvarchar(max)
)
RETURNS nvarchar(max) 
AS
BEGIN
	-- Declare @t nvarchar(max)='0;1'
	DECLARE @Result nvarchar(max)
	declare @table tabletype;
	-- Add the T-SQL statements to compute the return value here
	insert into @table
	select ID from masusertype where name in (select value from dbo.FNC_SPLIT(@t,';')) 
	set @Result= replace(dbo.fnc_STUFF(@table),',',';') 
	--print @Result
	-- Return the result of the function
	RETURN @Result

END
go

