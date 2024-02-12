 create PROCEDURE [dbo].[spGetCommission] 
@Customer nvarchar(max)
AS
BEGIN

 select * from masCommission where Customer=@Customer

end
go

