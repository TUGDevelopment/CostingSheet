CREATE FUNCTION [dbo].[usp_listFields](@schema VARCHAR(50), @table VARCHAR(50))
RETURNS @query TABLE (
    FieldName VARCHAR(255)
    )
BEGIN
    INSERT @query
    SELECT
        'SELECT ''' + @table+'~'+RTRIM(COLUMN_NAME)+'~''+CONVERT(VARCHAR, COUNT(*)) '+
    'FROM '+@schema+'.'+@table+' '+
          ' WHERE isnull("'+RTRIM(COLUMN_NAME)+'",'''')<>'''' UNION'
    FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table and TABLE_SCHEMA = @schema
    RETURN
END
go

