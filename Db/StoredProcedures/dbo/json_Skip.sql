


/****** json_Skip  ******/
CREATE  FUNCTION [dbo].[json_Skip](@JSON NVARCHAR(MAX),@start int)
returns int
as
begin
	if (@start>=len(@json)) return 0
	while ((@start<=len(@json)) and (ascii(substring(@json,@start,1))<=32)) set @start=@start+1
	if (@start>len(@json)) return 0
	return @start
end

go

