create PROCEDURE [dbo].[spupdateNewID]
AS
BEGIN
declare @Id int
declare cur_Employee CURSOR FOR

SELECT  Id
FROM     TransTechnical
WHERE   RequestType=1 and UniqueColumn is null

open cur_Employee

FETCH NEXT FROM cur_Employee INTO @Id
WHILE @@FETCH_STATUS = 0
BEGIN
DECLARE @myid uniqueidentifier = NEWID();  
SELECT CONVERT(nvarchar(max), @myid) AS 'NEWID';  
	update TransTechnical 
	set UniqueColumn = (SELECT CONVERT(nvarchar(max), @myid))
	WHERE Id=@Id

	FETCH NEXT FROM cur_Employee INTO @Id
END

CLOSE cur_Employee
DEALLOCATE cur_Employee
end
go

