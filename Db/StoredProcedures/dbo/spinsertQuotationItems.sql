-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spinsertQuotationItems] 
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@ExchangeRate nvarchar(max),
	@Overprice	nvarchar(50),
	@Extracost	nvarchar(50),
	@subContainers nvarchar(max),
	@RequestNo nvarchar(max),
	@CostingNo nvarchar(max),
	@Formula nvarchar(max),
	@SubID nvarchar(max),
	@MinPrice nvarchar(max),
	@OfferPrice nvarchar(max),
	@OfferPrice_Adjust nvarchar(max),
	@Mark nvarchar(50),
	@Code_Adjust nvarchar(50),
	@StatusApp nvarchar(max),
	@RequestType nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
			if(@Mark='D')
			delete TransQuotationItems where ID=@ID
	else if(select count(*) from TransQuotationItems where ID=@ID and SubID=@SubID)=0
	begin
		insert into TransQuotationItems
		select 
		--Commission=@Commission,
		Overprice=@Overprice,
		Extracost=@Extracost,
		subContainers=@subContainers,
		CostingNo=@CostingNo,
		MinPrice=@MinPrice,
		OfferPrice=@OfferPrice,
		OfferPrice_Adjust=@OfferPrice_Adjust,
		Code_Adjust=@Code_Adjust,
		Formula=@Formula,
		IsActive=0,
		null,
		StatusApp=@StatusApp,
		ExchangeRate=@ExchangeRate,
		RequestNo=@RequestNo,
		RequestType=@RequestType,
		SubID=@SubID
	end
	else 
		update TransQuotationItems set 
		--Commission=@Commission,
		Overprice=@Overprice,
		Extracost=@Extracost,
		subContainers=@subContainers,
		ExchangeRate=@ExchangeRate,
		StatusApp=@StatusApp,
		MinPrice=@MinPrice,
		OfferPrice=@OfferPrice,

		OfferPrice_Adjust=@OfferPrice_Adjust,Code_Adjust=@Code_Adjust where ID=@ID

		--select * from TransQuotationItems where id=32
		--select * from TransQuotation
END

go

