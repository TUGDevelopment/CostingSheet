
CREATE FUNCTION [dbo].[loophole](@i int) RETURNS varchar(20) AS
  BEGIN
     DECLARE @sql varchar(MAX),
             @cmd varchar(4000)
     SELECT @sql = ' UPDATE rsci ' +
                   ' SET b = CASE ' + ltrim(str(@i + 1)) +
                   ' WHEN 1 THEN ''Ett'' WHEN 2 THEN ''Två''' +
                   ' WHEN 3 THEN ''Tre'' WHEN 4 THEN ''Fyra''' +
                   ' WHEN 5 THEN ''Fem'' WHEN 6 THEN ''Sex''' +
                   ' WHEN 7 THEN ''Sju'' WHEN 8 THEN ''Åtta''' +
                   ' WHEN 9 THEN ''Nio'' WHEN 10 THEN ''Tio'' END' +
                   ' WHERE a = ' + ltrim(str(@i + 1))
     SELECT @cmd = 'sqlcmd -S ' + @@servername + ' -d ' + db_name() +
                   ' -Q "' + @sql + '"'
     EXEC master..xp_cmdshell @cmd, 'no_output'
     RETURN (SELECT b FROM rsci WHERE a = @i)
  END
go

