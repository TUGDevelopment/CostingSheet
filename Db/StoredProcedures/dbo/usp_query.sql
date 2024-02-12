  create PROC [dbo].[usp_query] (
    @table NVARCHAR(128)
)
AS
BEGIN
 
    DECLARE @sql NVARCHAR(MAX);
    -- construct SQL
    SET @sql = N'SELECT * FROM ' + @table;
    -- execute the SQL
    EXEC sp_executesql @sql;
    
END;
go

