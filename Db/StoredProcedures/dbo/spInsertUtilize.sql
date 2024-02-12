-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertUtilize]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),	
	@Result nvarchar(max),
	@MonthName nvarchar(max),
	@SubID nvarchar(max),
	@RequestNo nvarchar(max),
	@Mark nvarchar(max),
	@Cost nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 

	if (@Mark='Del')
		delete TransUtilize where ID=@ID
	else
	begin
    -- Insert statements for procedure here
	if(select count(ID) from TransUtilize where ID=@ID and SubID=@SubID and
	RequestNo=@RequestNo)=0
		insert into TransUtilize values(@Result,@MonthName,@Cost,@SubID,@RequestNo) else
		update TransUtilize set Result=@Result where ID=@ID
	end
END
go

