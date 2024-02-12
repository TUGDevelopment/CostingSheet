-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spinsertQuotationCustom]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@Mark nvarchar(max),
	@ProductName nvarchar(max),
	@Customer nvarchar(max),
	@RD_ref nvarchar(max),
	@CostingNo nvarchar(max),
	@OfferPrice nvarchar(max),
	@subId nvarchar(max),
	@RequestNo nvarchar(max),
	@StatusApp nvarchar(max),
	@Material nvarchar(max),
	@ShipTo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@Mark = 'D')
	delete TransQuotationCustom where ID=@ID
    -- Insert statements for procedure here
	else if(@Mark = 'X')
	insert into TransQuotationCustom
	select 
	ProductName=@ProductName,
	Material=@Material,
	Customer=@Customer,
	ShipTo=@ShipTo,
	RD_ref=@RD_ref,
	CostingNo=@CostingNo,
	OfferPrice=@OfferPrice,
	StatusApp=@StatusApp,
	RequestNo=@RequestNo,
	SubID=@SubID
	else
	update TransQuotationCustom set OfferPrice=@OfferPrice ,StatusApp=@StatusApp
		where costingno=@CostingNo 
		and SubID=@subId 
		and Customer=@Customer 
		and ShipTo=@ShipTo
END
--select * from TransQuotationCustom
go

