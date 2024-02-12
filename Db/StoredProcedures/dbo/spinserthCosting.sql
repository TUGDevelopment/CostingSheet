-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spinserthCosting]
 @ID nvarchar(50),
 @RequestNo nvarchar(50),
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
	if(@Mark='D')
	delete TranshCosting where ID=@ID
	else if(select count(Id) from TranshCosting where Id=@Id and RequestNo=@RequestNo)>0
	update TranshCosting set
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
	insert into TranshCosting
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
	Set @result =(SELECT ID FROM TranshCosting Where ID = (SELECT CAST(scope_identity() AS int)))
END
 
go

