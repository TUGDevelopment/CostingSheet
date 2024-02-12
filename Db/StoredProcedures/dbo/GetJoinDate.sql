CREATE FUNCTION [dbo].[GetJoinDate](@empID as nvarchar(500))
RETURNS Varchar(8000)
AS
BEGIN
DECLARE @RESULT AS NVARCHAR(500)

Select @result=JoinDateQuery from NewHireEmployee where empid= @empID 

return @RESULT
END
go

