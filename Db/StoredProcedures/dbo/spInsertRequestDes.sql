-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertRequestDes] 
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@RequestNo nvarchar(max),
	@Company nvarchar(max),
	@Remark nvarchar(max),
	@user nvarchar(max),
	@From nvarchar(max),
	@To nvarchar(max),
 
	@Assignee nvarchar(max),
	@NewID nvarchar(max),
	@StatusApp nvarchar(max),
	@Destination nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @ID nvarchar(max)='4961',@StatusApp nvarchar(max)='4',@Company nvarchar(max)='102',@Remark nvarchar(max)='',@user nvarchar(max)='MP001414'
	SET NOCOUNT ON;
	insert into MasHistory values (@Id,@User,@StatusApp,getdate(),@remark,null,'TransTechnical')
	if(select count(Id) from TransRequestDesForm where ID=@ID)>0
	update TransRequestDesForm set Remark=@Remark,
	RequestDate=@From,
	StatusApp=(case when StatusApp=2 then 4 else @StatusApp end),
	RequireDate=@To,Destination=@Destination,Company=@Company,Assignee=@Assignee where Id=@ID
	else
	begin
	set @RequestNo= substring(@Company,1,3) +'1'+ convert(nvarchar(2),right(year(getdate()),2)) +''+ 
	(SELECT format(isnull(max(right(RequestNo,5)),0)+1, '00000') FROM TransRequestDesForm
	where SUBSTRING(RequestNo,5,2)=right(year(getdate()),2) and SUBSTRING(RequestNo,4,1)='1')
	print @RequestNo;
	insert into TransRequestDesForm(RequestNo,
	Remark,
	CreateBy,
	CreateOn,
	StatusApp,
	RequestDate,
	RequireDate,UniqueColumn,Destination,Company,Assignee) values
	(@RequestNo,@Remark,@user,getdate(),@StatusApp,@From,@To,@NewID,@Destination,@Company,@Assignee)
	set @Id  = (SELECT CAST(scope_identity() AS int)) 
	end 
	select ID,
	RequestNo,
	StatusApp,
	format(RequestDate,'dd-MM-yyy') as 'Form',
	format(RequireDate,'dd-MM-yyy') as 'To',Company,Assignee from TransRequestDesForm where ID=@Id
END
--select * from TransRequestDesForm order by id desc
--truncate table transeditprice

go

