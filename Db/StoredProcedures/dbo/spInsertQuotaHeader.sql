-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spInsertQuotaHeader]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@User nvarchar(max),
	@Incoterm	nvarchar(50),
	@Route	nvarchar(50),
	@Size	nvarchar(50),
	@PaymentTerm	nvarchar(50),
	@Interest	nvarchar(50),
	@Freight	nvarchar(50),
	@Insurance	nvarchar(50),
	@Remark	nvarchar(MAX),
	@Customer	nvarchar(50),
	@ShipTo	nvarchar(50),
	@StatusApp	nvarchar(50),
	@Commission nvarchar(max),
	@Currency nvarchar(max),
	@ExchangeRate nvarchar(50),
	@Brand nvarchar(max)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(SELECT count(*) from TransQuotationHeader where ID=@Id)=0
	begin
	declare @runid nvarchar(max)
	set @runid= FORMAT(GetDate(), 'yyyyMM') +''+ 
	(SELECT format(isnull(max(right(RequestNo,5)),0)+1, '00000') FROM TransQuotationHeader
	where SUBSTRING(RequestNo,1,6)=FORMAT(GetDate(), 'yyyyMM'))
	--SELECT FORMAT(GetDate(), 'yyyyMMdd')
	print @runid
	Insert into TransQuotationHeader values
	(@User,Getdate(),null,null,@Commission,@Brand,@Incoterm,
	@Route,
	@Size,
	@PaymentTerm,
	@Interest,
	@Freight,
	@Insurance,
	@Remark,
	@Customer,
	@ShipTo,
	0,@Currency,@ExchangeRate,@runid,NEWID())
	SET @Id = (SELECT CAST(scope_identity() AS int))
	end
	else
	update TransQuotationHeader set ModifyBy=@User,ModifyOn=Getdate(),Commission=@Commission,Brand=@Brand,Incoterm=@Incoterm,[Route]=@Route,Size=@Size,PaymentTerm=@PaymentTerm,
	Interest=@Interest,
	Freight=@Freight,
	Insurance=@Insurance,
	Remark=@Remark,
	Customer=@Customer,
	ShipTo=@ShipTo,
	Currency=@Currency,ExchangeRate=@ExchangeRate where ID=@Id
    -- Insert statements for procedure here
	-- SELECT * from TransQuotationHeader where ID=@Id
	select @Id
END

go

