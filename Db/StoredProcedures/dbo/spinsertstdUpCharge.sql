-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spinsertstdUpCharge]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@UpchargeGroup nvarchar(max),
	@Upcharge nvarchar(max),
	@Price nvarchar(max),
	@Quantity nvarchar(max),
	@Currency nvarchar(max),
	@Result nvarchar(max),
	@SubID nvarchar(max),
	@RequestNo nvarchar(max),
	@stdPackSize nvarchar(max),
	@Mark nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @stdPackSize nvarchar(max)
	SET NOCOUNT ON; 
	if (@UpchargeGroup='Del' or @Mark='Del')
	delete TransUpCharge where ID=@ID
	else
	begin
    -- Insert statements for procedure here
	if(select count(ID) from TransUpCharge where ID=@ID and SubID=@SubID and
	RequestNo=@RequestNo)=0
	insert into TransUpCharge values(@UpchargeGroup,@Upcharge,@Price,@Quantity,@Currency,@Result,@stdPackSize,@SubID,
	@RequestNo) else
	update TransUpCharge set Price=@Price where ID=@ID
	end
END

 --select * from TransUpCharge where RequestNo =4
go

