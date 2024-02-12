CREATE FUNCTION [dbo].[_tmp_func](@orderID NVARCHAR(50))
RETURNS INT
AS
BEGIN
DECLARE @sql varchar(4000), @cmd varchar(4000)
SELECT @sql = 'INSERT INTO _ord (ord_Code) VALUES (''' + @orderID + ''') '
SELECT @cmd = 'sqlcmd -S ' + @@servername +
              ' -d ' + db_name() + ' -Q "' + @sql + '"'
EXEC master..xp_cmdshell @cmd, 'no_output'
RETURN 1
END
go

