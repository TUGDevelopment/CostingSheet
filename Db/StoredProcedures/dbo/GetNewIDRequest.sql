CREATE PROCEDURE [dbo].[GetNewIDRequest]
AS
BEGIN
DECLARE @myid uniqueidentifier = NEWID();  
SELECT CONVERT(nvarchar(max), @myid) AS 'NEWID';  
--update TransTechnical set UniqueColumn=@myid where id=3954
END

go

