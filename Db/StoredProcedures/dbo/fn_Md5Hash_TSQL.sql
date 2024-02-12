CREATE FUNCTION [dbo].[fn_Md5Hash_TSQL](@S1 nvarchar(50))
RETURNS nvarchar(100)
BEGIN
declare
@result varbinary(4000),
@S2 varchar(100),
@X nvarchar(100);

SET @S2 = CONVERT( VARCHAR(100), @S1 );
SET @result = HASHBYTES('MD5', @S2);
SET @X = LOWER(CAST('' AS XML).value('xs:hexBinary(sql:variable("@result"))', 'NVARCHAR(100)') )

RETURN(@X);

END
go

