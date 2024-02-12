-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertTunaCal]
	-- Add the parameters for the stored procedure here
	@RowID nvarchar(max),	
 	@Component nvarchar(max),
	@Name nvarchar(max),
	@Currency nvarchar(max),
	@Result nvarchar(max),
	@Calcu nvarchar(max),
	@Quantity nvarchar(max),
	@Price nvarchar(max),
	@Unit nvarchar(max),
	@BaseUnit nvarchar(max),

	@SubID nvarchar(max),
	@RequestNo nvarchar(max),
	@Mark nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 

	if (@Mark='Del')
		delete TransTunaCal where RowID=@RowID
	else
	begin
    -- select * from TransTunaCal
	if(select count(RowID) from TransTunaCal where RowID=@RowID and SubID=@SubID and
	RequestNo=@RequestNo and Calcu=@Calcu)=0
		insert into TransTunaCal values(@Component,@Name,@Currency, @Result,@Calcu,@Quantity,@Price,@Unit,@BaseUnit,@SubID,@RequestNo) else
		update TransTunaCal set Result=@Result,Quantity=@Quantity,Price=@Price,Unit=@Unit,BaseUnit=@BaseUnit where RowID=@RowID
	end
END
go

