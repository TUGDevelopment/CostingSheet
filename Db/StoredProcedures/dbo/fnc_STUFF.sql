CREATE FUNCTION [dbo].[fnc_STUFF](@TableName TableType READONLY)
RETURNS NVARCHAR(max)
AS
BEGIN
DECLARE @cols AS NVARCHAR(MAX);
		SET @cols = STUFF((SELECT distinct ',' + (c.LocationName) 
            FROM @TableName c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
RETURN @cols
END
go

