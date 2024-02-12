-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[sploadCostingItems]
 @ID nvarchar(50),
 @RequestNo nvarchar(50),
 --@MarketingNumber nvarchar(max),
 --@RDNumber nvarchar(50),
 --@Company nvarchar(50),
 --@PackSize nvarchar(50),
 --@CreateBy nvarchar(max)
 @Component nvarchar(max),
 @SAPMaterial nvarchar(max),
 @Description nvarchar(max),
 @Quantity nvarchar(50),
 @PriceUnit nvarchar(50),
 @Amount nvarchar(50),
 @Per nvarchar(50),
 @Loss nvarchar(50),
 @SellingUnit nvarchar(50),
 @Formula nvarchar(max),
 @CreateBy nvarchar(50),
 @Mark nvarchar(max)
 AS
BEGIN
	declare @result nvarchar(50)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--update TransCostingHeader set StatusApp=2 where MarketingNumber='GP6322711'
	--select * from TransCosting where RequestNo=22809 order by ID 
	if(@Mark='D')
	delete TransCosting where ID=@ID
	else if(select count(Id) from TransCosting where Id=@Id and RequestNo=@RequestNo)>0
	update TransCosting set
	Component=@Component,
	SAPMaterial=@SAPMaterial,
	[Description]=@Description,
	Quantity=@Quantity,
	PriceUnit=@PriceUnit,
	Amount=@Amount,
	Per=@Per,
	SellingUnit=@SellingUnit,
	Loss=@Loss,
	ModifyOn=getdate(),
	ModifyBy=@CreateBy,
	Formula=@Formula where
	RequestNo=@RequestNo and ID=@Id
else
	if(@Mark='X')
	insert into TransCosting
	select 
	Component=@Component,
	SAPMaterial=@SAPMaterial,
	[Description]=@Description,
	Quantity=@Quantity,
	PriceUnit=@PriceUnit,
	Amount=@Amount,
	Per=@Per,
	SellingUnit=@SellingUnit,
	Loss=@Loss,
	CreateOn=getdate(),
	CreateBy=@CreateBy,
	ModifyOn=Null,
	ModifyBy=Null,
	Formula=@Formula,
	RequestNo=@RequestNo
 --SELECT SCOPE_IDENTITY();
	Set @result =(SELECT ID FROM TransCosting Where ID = (SELECT CAST(scope_identity() AS int)))
END

 
go

