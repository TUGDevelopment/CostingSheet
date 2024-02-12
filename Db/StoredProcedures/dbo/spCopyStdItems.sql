-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spCopyStdItems] 
	-- Add the parameters for the stored procedure here
	@RequestNo nvarchar(max),
	@SubIDNew nvarchar(max),
	@Id nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

declare @fn nvarchar(max)
declare cur_temp CURSOR FOR
SELECT value FROM dbo.FNC_SPLIT('TransUpCharge,TransUtilize',',')
open cur_temp
FETCH NEXT FROM cur_temp INTO @fn
WHILE @@FETCH_STATUS = 0
BEGIN
If Object_ID('tempdb..#temp')  is not null  drop table #temp
	SELECT t.name AS table_name,
	SCHEMA_NAME(schema_id) AS schema_name,
	c.name AS column_name,column_id into #temp
	FROM sys.tables AS t
	INNER JOIN sys.columns c ON t.OBJECT_ID = c.OBJECT_ID
	WHERE t.name in (''+@fn+'') and c.name not in ('Id','SubID','RequestNo')
	ORDER BY c.column_id;

	DECLARE @cols AS NVARCHAR(MAX),
		@query  AS NVARCHAR(MAX);
	SET @cols = STUFF((SELECT ',' + QUOTENAME(c.column_name) 
            FROM #temp c ORDER BY c.column_id
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
		set @query='insert '+@fn+' select '+@cols+','+@SubIDNew+','+@RequestNo+' from '+ @fn +' where SubID='+@Id
		print @query;
		execute(@query)
		FETCH NEXT FROM cur_temp INTO @fn
END
CLOSE cur_temp
DEALLOCATE cur_temp
END
go

