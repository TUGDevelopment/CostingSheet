create FUNCTION [dbo].[fnc_getbutype]
(
	@t nvarchar(max)
)
RETURNS nvarchar(max) 
AS
BEGIN
	-- Declare @t nvarchar(max)='1010:TUNA SALADA;1011:AMBIENT PRODUCT BY TU RD'
	DECLARE @Result nvarchar(max)
	declare @table tabletype;
	--set @t=replace(@t,',',';')
	-- Add the T-SQL statements to compute the return value here
	insert into @table
	select ID from MasPetCategory where convert(nvarchar(max),ID) +':'+Name in (select value from dbo.FNC_SPLIT(@t,';')) 
	set @Result = dbo.fnc_STUFF(@table)
	--print @Result
	-- Return the result of the function
	RETURN @Result

END
 
go

