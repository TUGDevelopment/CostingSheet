CREATE PROCEDURE [dbo].[spGetElement] @tablename as nvarchar(max) AS
BEGIN
--DECLARE @tablename as varchar(50)='(select top 0 * from TransStdComponent)#a'
--SELECT @tablename = tablename FROM tables WHERE tableid = @tableid
--EXEC('SELECT * FROM ' + @tablename + ' WHERE elementid = ' + @elementid)
EXEC('SELECT * FROM ' + @tablename)
END

--SELECT TABLE_NAME
--FROM INFORMATION_SCHEMA.TABLES
--WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='CostingSheet'

go

