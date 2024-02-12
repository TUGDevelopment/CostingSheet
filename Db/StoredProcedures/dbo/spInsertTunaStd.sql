-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertTunaStd]
 @ID nvarchar(max)
--      ,@Material nvarchar(max)
      ,@Incoterm nvarchar(max)
      ,@Route nvarchar(max)
      ,@Size nvarchar(max)
      ,@Quantity nvarchar(max)
      ,@PaymentTerm nvarchar(max)
      --,@Commission nvarchar(max)
      ,@StatusApp nvarchar(max)
      ,@Interest nvarchar(max)
      ,@User nvarchar(max)
      --,@CreateOn datetime
      --,@ModifyBy nvarchar(max)
      --,@ModifyOn datetime
      ,@Freight nvarchar(max)
      ,@Insurance nvarchar(max)
      ,@Currency nvarchar(max)
      ,@ExchangeRate nvarchar(max)
      ,@Remark nvarchar(max)
      ,@Customer nvarchar(max)
      ,@ShipTo nvarchar(max)
	  ,@from datetime
	  ,@to datetime
      ,@ValidityDate Datetime

      ,@BillTo nvarchar(max)
	  ,@Zone nvarchar(max)
      ,@RequestNo nvarchar(max)
      ,@NewID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(select count(ID)from TransTunaStd where UniqueColumn=@NewID)=0 begin
	declare @runid nvarchar(max)
	set @runid= FORMAT(GetDate(), 'yyyyMM') +''+ 
	(SELECT format(isnull(max(right(RequestNo,5)),0)+1, '00000') FROM TransTunaStd
	where SUBSTRING(RequestNo,1,6)=FORMAT(GetDate(), 'yyyyMM'))
	--SELECT FORMAT(GetDate(), 'yyyyMMdd')
	--print @runid
	--select * from TransTunaStd
	insert into TransTunaStd
	select null,
		Incoterm=@Incoterm,
		[Route]=@Route,
		Size=@Size,
		Quantity=@Quantity,
		PaymentTerm=@PaymentTerm,
		--Commission=@Commission,
		StatusApp=@StatusApp,
		Interest=@Interest,
		CreateBy=@User,
		CreateOn=GETDATE(),
		null,null,
		Freight=@Freight,
		Insurance=@Insurance,
		Currency=@Currency,
		ExchangeRate=@ExchangeRate,
		Remark=@Remark,
		Customer=@Customer,
		ShipTo=@ShipTo,null,
		[From]=@from,
		[To]=@to,
		ValidityDate=@ValidityDate,
		BillTo=@BillTo,
		[Zone]=@Zone,
		[Flag]=0,
		RequestNo=@runid,
		UniqueColumn=@NewID
		SET @ID = (SELECT CAST(scope_identity() AS int))
	end
	else begin
	if (select ExchangeRate from TransTunaStd where id=@id)<>@ExchangeRate
		insert into StdHistory values(@Id,@user,(select ExchangeRate from TransTunaStd where id=@id),getdate(),'ExchangeRate',@ExchangeRate,null)

	update TransTunaStd set ModifyBy=@User,
		ModifyOn=Getdate(),
		Incoterm=@Incoterm,
		[Route]=@Route,
		Size=@Size,
		Quantity=@Quantity,
		PaymentTerm=@PaymentTerm,
		--Commission=@Commission,
		--StatusApp=@StatusApp,
		Interest=@Interest,
		Freight=@Freight,
		Insurance=@Insurance,
		Currency=@Currency,
		ExchangeRate=@ExchangeRate,
		Remark=@Remark,
		Customer=@Customer,
		[From]=@from,
		[To]=@to,
		ValidityDate=@ValidityDate,
		BillTo=@BillTo,
		[Zone]=@Zone,
		ShipTo=@ShipTo where ID=@Id

		end
    -- Insert statements for procedure here
	SELECT * from TransTunaStd where ID=@ID;
END

--select * from StdHistory
go

