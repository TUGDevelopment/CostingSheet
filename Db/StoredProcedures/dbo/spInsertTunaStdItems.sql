-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertTunaStdItems]
	-- Add the parameters for the stored procedure here
	   @ID nvarchar(max)
      ,@Material nvarchar(max)
      ,@Utilize nvarchar(max)
      ,@From datetime
	  ,@To datetime
	  ,@RawMaterial nvarchar(max)
      ,@subContainers nvarchar(max)
      ,@Media nvarchar(max)
      ,@Packaging nvarchar(max)
      ,@LOHCost nvarchar(max)
      ,@PackingStyle nvarchar(max)
      ,@Upcharge nvarchar(max)
      ,@RequestNo nvarchar(max)
	  ,@Commission nvarchar(max)
      ,@OverPrice nvarchar(max)
      ,@Pacifical nvarchar(max)
      ,@MSC nvarchar(max)
      ,@Margin nvarchar(max)
      ,@MinPrice nvarchar(max)
      ,@OfferPrice nvarchar(max)
	  ,@Mark nvarchar(max)
	  ,@name nvarchar(max)
	  ,@OverType nvarchar(max)
	  ,@IsAccept nvarchar(max)
	  ,@PackSize nvarchar(max)
	  ,@Yield nvarchar(max)
	  ,@FillWeight nvarchar(max)
	  ,@SecPackaging nvarchar(max)
	  ,@SpecialFishPrice nvarchar(max)
	  ,@MarginBid nvarchar(max)
	  ,@Bidprice nvarchar(max)
	  ,@Authorized_price nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		if(@Mark='D') 
		delete TransTunaStdItems where ID=@ID
		else begin
		if(select count(ID)from TransTunaStdItems where ID=@ID and RequestNo=@RequestNo)=0 begin
			insert into TransTunaStdItems
			select Material=@Material,[name]=@name,
			Utilize=@Utilize,
			[From]=@From,
			[To]=@To,
			RawMaterial=@RawMaterial,
			SpecialFishPrice=@SpecialFishPrice,
			PackSize=@PackSize,
			Yield=@Yield,
			FillWeight=@FillWeight,
			subContainers=@subContainers,
			Media=@Media,
			Packaging=@Packaging,
			LOHCost=@LOHCost,
			PackingStyle=@PackingStyle,
			SecPackaging=@SecPackaging,
			Upcharge=@Upcharge,
			Commission=@Commission,
			OverPrice=@OverPrice,
			OverType=@OverType,
			Pacifical=@Pacifical,
			MSC=@MSC,
			Margin=@Margin,
			Authorized_price=@Authorized_price,
			Bidprice=@Bidprice,
			MarginBid=@MarginBid,
			MinPrice=@MinPrice,
			OfficePrice=@OfferPrice,
			IsAccept=@IsAccept,
			RequestNo=@RequestNo
			SET @ID = (SELECT CAST(scope_identity() AS int))
		end else
		update TransTunaStdItems set Material=@Material,[name]=@name,
			Utilize=@Utilize,
			[From]=@From,
			[To]=@To,
			RawMaterial=@RawMaterial,
			SpecialFishPrice=@SpecialFishPrice,
			PackSize=@PackSize,
			Yield=@Yield,
			FillWeight=@FillWeight,
			SecPackaging=@SecPackaging,
			subContainers=@subContainers,
			Media=@Media,
			Packaging=@Packaging,
			LOHCost=@LOHCost,
			PackingStyle=@PackingStyle,
			Upcharge=@Upcharge,
			Commission=@Commission,
			OverPrice=@OverPrice,
			OverType=@OverType,
			Pacifical=@Pacifical,
			MSC=@MSC,
			IsAccept =@IsAccept,
			Margin=@Margin,
			Authorized_price=@Authorized_price,
			Bidprice=@Bidprice,
			MarginBid=@MarginBid,
			MinPrice=@MinPrice,
			OfferPrice=@OfferPrice where ID=@ID
		end
		select @ID as 'ID'
		--select * from TransTunaStdItems
END

go

