create FUNCTION [dbo].[RemoveAllSpaces]
(
    @InputStr varchar(8000)
)
RETURNS varchar(8000)
AS
BEGIN
declare @ResultStr varchar(8000)
set @ResultStr = @InputStr
while charindex(' ', @ResultStr) > 0
    set @ResultStr = replace(@InputStr, ' ', '')

return @ResultStr
END
go

