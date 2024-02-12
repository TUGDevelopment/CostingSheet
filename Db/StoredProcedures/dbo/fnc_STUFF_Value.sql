create FUNCTION [dbo].[fnc_STUFF_Value](@TableName TableType READONLY,@DELIMITER CHAR(1))
RETURNS NVARCHAR(max)
AS
BEGIN
DECLARE @cols AS NVARCHAR(MAX);
		SET @cols = STUFF((SELECT distinct @DELIMITER + (c.LocationName) 
            FROM @TableName c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
RETURN @cols
END
go

