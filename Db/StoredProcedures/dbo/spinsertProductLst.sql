-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spinsertProductLst] 
	-- Add the parameters for the stored procedure here
	  @name nvarchar(max),
	  @Id nvarchar(max),
      @NetWeight nvarchar(max),
      @DWType nvarchar(max),
	  @RequestNo nvarchar(max),
	  @FixedFillWeight nvarchar(max),
	  @DW nvarchar(max),
	  @PW nvarchar(max),
	  @SaltContent nvarchar(max),
	  @TargetPrice nvarchar(max),
	  @Mark nvarchar(max),
	  @Efficiency nvarchar(max),
	  @Yield nvarchar(max),
	  @PackSize nvarchar(max),
	  @RDCode nvarchar(max)
 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--delete TransFormulaHeader where Formula=@formula and RequestNo=@RequestNo
		if (@Mark='D')
		delete TransProductList where Id=@Id
		else
		begin
		if (select count(*) from TransProductList where Id=@Id and RequestNo=@RequestNo)=0
		-- Insert statements for procedure here
		begin
			--select FORMAT(1,'00') 
			insert into TransProductList 
			values(@name,@NetWeight,@DW,@DWType,@PW,@FixedFillWeight,@SaltContent,@TargetPrice,@Efficiency,@Yield,@RDCode,@PackSize,@RequestNo)
			SET @ID = (SELECT CAST(scope_identity() AS int))
		end
		else 
		update TransProductList set Name=@name,NetWeight=@NetWeight,DWType=@DWType,SaltContent=@SaltContent,FixedFillWeight=@FixedFillWeight,DW=@DW,PW=@PW ,
		TargetPrice=@TargetPrice,Efficiency=@Efficiency,Yield=@Yield,PackSize=@PackSize,RdCode=@RDCode where ID=@Id and RequestNo=@RequestNo
		end
		select @ID as 'ID'
END


go

