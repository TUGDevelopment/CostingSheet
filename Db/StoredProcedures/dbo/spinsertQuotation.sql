-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spinsertQuotation]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@Mark nvarchar(max),
	@Incoterm nvarchar(max),
	@Route nvarchar(max),
	@Size nvarchar(max),
	@Quantity nvarchar(max),
	@PaymentTerm nvarchar(max),
	@Commission nvarchar(max),
	@Interest nvarchar(max),
	@RequestNo nvarchar(max),
	@SubID	nvarchar(max),
	--@StatusApp nvarchar(max),
	@User nvarchar(max),
	@Freight nvarchar(max),
	@Insurance nvarchar(max),
	@Currency nvarchar(max),
	@ExchangeRate nvarchar(max),
	@Customer nvarchar(max),
	@ShipTo nvarchar(max),
	@Remark nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @ID nvarchar(max)='8918'
    -- Insert statements for procedure here
	if(@Mark='X')
	begin
	insert into TransQuotation
	SELECT Incoterm=@Incoterm
      ,[Route]=@Route
      ,[Size]=@Size
      ,Quantity=@Quantity
      ,PaymentTerm=@PaymentTerm
      ,Commission=@Commission
	  ,0
      ,Interest=@Interest
	  ,CreateBy=@User
	  ,Getdate()
	  ,null
	  ,null
	  ,@Freight,@Insurance,Currency=@Currency,ExchangeRate=@ExchangeRate,Remark=@Remark,
	  Customer=@Customer,ShipTo=@ShipTo,RequestNo=@RequestNo,SubID=@SubID
	SET @Id = (SELECT CAST(scope_identity() AS int))
	end
	else if(@Mark='U')
	update TransQuotation 
	set Incoterm=@Incoterm,
	Quantity=@Quantity,
	[Route]=@Route,
	[Size]=@Size,
	Freight=@Freight,
	Insurance=@Insurance,
	PaymentTerm=@PaymentTerm,
	Interest=@Interest,
	Commission=@Commission,Currency=@Currency,ExchangeRate=@ExchangeRate,Remark=@Remark,ModifyBy=@User,
	Customer=@Customer,ShipTo=@ShipTo,
	ModifyOn=getdate() where ID=@ID
	select @ID
	--select * from TransQuotation
END



go

