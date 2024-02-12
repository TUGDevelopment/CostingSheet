CREATE PROCEDURE [dbo].[spupdateexch] 
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@ID nvarchar(max),
	@Validfrom nvarchar(max),
	@Validto nvarchar(max),
	@Ratio nvarchar(max),
	@Currency nvarchar(max),
	@Rate nvarchar(max),
	@CurrencyTh nvarchar(max),
	@ExchangeType  nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Update MasExchangeRat set Company=@Company,Validfrom=@Validfrom,Validto=@Validto,Ratio=@Ratio,
	Currency=@Currency,Rate=@Rate,CurrencyTh=@CurrencyTh,ExchangeType=@ExchangeType where ID=@ID
END
go

