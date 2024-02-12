-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spinsertFormulaCost] 
	-- Add the parameters for the stored procedure here
	@name nvarchar(max),
	@Customer nvarchar(max),
	@Code nvarchar(max),
	@RefSamples nvarchar(max),
	@formula nvarchar(max),
	@RequestNo nvarchar(max),
	@Id nvarchar(max),
	@Costper nvarchar(max),
	@CostNo nvarchar(max),
	@Revised nvarchar(max),
	@MinPrice nvarchar(max),
	@Mark nvarchar(max),
	@IsActive nvarchar(max),
	@SellingUnit nvarchar(max),
	@ref nvarchar(max),
	@Packaging nvarchar(max),
	@nw nvarchar(max),
	@PackSize nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--delete TransFormulaHeader where Formula=@formula and RequestNo=@RequestNo
	if (select count(*) from TransFormulaHeader where ID=@Id and RequestNo=@RequestNo)=0
    -- Insert statements for procedure here
	begin
		--declare @RequestNo nvarchar(max)=1393,@formula nvarchar(max)=1,@CostNo nvarchar(max)=0
		if(@CostNo='0' or @CostNo='')
		set @CostNo=(select MarketingNumber +''+ format(convert(int,@formula),'00') 
					from TransCostingHeader where ID=@RequestNo)

		--select FORMAT(1,'00') 
		print @CostNo;
		if(@formula<>0)
		insert into TransFormulaHeader 
		values(@name,@Customer,@Code,@RefSamples,@formula,0,@Costper,@CostNo,@Revised,@MinPrice,0,@SellingUnit,@ref,@Packaging,@nw,@PackSize,@RequestNo)
	end
	else 
	update TransFormulaHeader set Name=@name,RefSamples=@RefSamples,IsActive=@IsActive,Code=@Code,MinPrice=@MinPrice,SellingUnit=@SellingUnit,[ref]=@ref,Packaging=@Packaging,
		nw=@nw,PackSize=@PackSize
		where ID=@Id and RequestNo=@RequestNo
END

--select * from TransFormulaHeader where RequestNo=1217
--update TransCostingHeader set StatusApp=2 where id=6443
go

