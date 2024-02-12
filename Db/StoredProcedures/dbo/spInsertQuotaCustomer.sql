-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spInsertQuotaCustomer]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@SoldTo nvarchar(max),
	@ShipTo nvarchar(max),
	@Mark nvarchar(max),
	@SubID nvarchar(max),
	@RequestNo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@Mark='X')
	Insert into TransCustomer values
	(@SoldTo,@ShipTo,@SubID,@RequestNo)
	else
	update TransCustomer set SoldTo=@SoldTo,ShipTo=@ShipTo where ID=@Id
    -- Insert statements for procedure here
	-- SELECT * from TransCustomer
END
go

