-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertStdFileDetails]
	-- Add the parameters for the stored procedure here
	@user nvarchar(max),
	@Result nvarchar(max),
	@Name nvarchar(max),
	@Notes nvarchar(max),
	@Attached varbinary(MAX),
	@SubID nvarchar(max),
	@RequestNo nvarchar(max),
	@IsApprove nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--declare @Result nvarchar(max)='ValidityDate',@SubID nvarchar(max)=0,@RequestNo nvarchar(max)=237
	--declare @id int 
	--set @id=(select top 1 ID from TransStdFileDetails where SubID=@SubID and RequestNo=@RequestNo and Result=@Result)
	--print @id;
	--if(@id)=0 begin
	if(@Result='ExchangeRate' or @Result='Freight' or @Result='ValidityDate')
	set @SubID= 0
	insert into TransStdFileDetails values(
	getdate(),@user,@Result,@Name,@Notes,@Attached,@IsApprove,@SubID,@RequestNo)
	--end else
	--update TransStdFileDetails set Name=@Name,Notes=@Notes,Attached=@Attached where ID=@id
END
--select * from TransStdFileDetails
go

