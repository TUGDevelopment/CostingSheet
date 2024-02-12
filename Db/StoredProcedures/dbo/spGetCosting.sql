 

CREATE PROCEDURE [dbo].[spGetCosting]
	@user nvarchar(max),
	@Id int,
	@Selected nvarchar(max)
AS
BEGIN
--declare @user nvarchar(max)='FO5910155'
declare @CurDate datetime = getdate()-10
select RequestNo,ID,Customer,Brand,format(RequireDate,'dd-MM-yyyy')RequireDate
 from TransTechnical where dbo.fnc_checktype(concat(Requester,',',isnull(Assignee,'')),@user)>0
and StatusApp=4 and DATEDIFF(DAY,getdate(),RequireDate)>-100
end
go

