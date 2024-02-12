
/****** json_Message  ******/
CREATE FUNCTION [dbo].[json_Message](@JSON NVARCHAR(MAX),@start int)
RETURNS NVARCHAR(MAX)
AS
	BEGIN
		set @start=dbo.json_Skip(@json,@start)
		if (@start=0) return '** END OF TEXT **'
		return substring(@json,@start,30)
	end

go

