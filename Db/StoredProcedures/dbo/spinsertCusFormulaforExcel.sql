 
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spinsertCusFormulafortrf]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@Component nvarchar(max)
	,@SubType nvarchar(max)
      ,@Description nvarchar(max)
      ,@Material nvarchar(max)
      ,@Result nvarchar(max)
      ,@Yield nvarchar(max)
      ,@RawMaterial nvarchar(max)
      ,@Name nvarchar(max)
      ,@PriceOfUnit nvarchar(max)
      ,@Currency nvarchar(max)
      ,@Unit nvarchar(max)
      ,@ExchangeRate nvarchar(max)
      ,@BaseUnit nvarchar(max)
      ,@PriceOfCarton nvarchar(max)
      ,@Formula int
      ,@IsActive nvarchar(max)
      ,@Remark nvarchar(max)
      ,@LBOh nvarchar(max)
      ,@LBRate nvarchar(max)
      ,@RequestNo nvarchar(max),@AdjustPrice nvarchar(max),@SubID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	if(select count(*) from TransCusFormula where Id=@Id and RequestNo=@RequestNo and Formula=@Formula)=0
	insert into TransCusFormula
	 select [Component]=@Component
      ,[SubType]=@SubType
      ,[Description]=@Description
      ,[Material]=@Material
      ,[Result]=@Result
      ,[Yield]=@Yield
      ,[RawMaterial]=@RawMaterial
      ,[Name]=@Name
      ,[PriceOfUnit]=@PriceOfUnit
	  ,AdjustPrice=@AdjustPrice
      ,[Currency]=@Currency
      ,[Unit]=@Unit
      ,[ExchangeRate]=@ExchangeRate
      ,[BaseUnit]=@BaseUnit
      ,[PriceOfCarton]=@PriceOfCarton
      ,[Formula]=@Formula
      ,[IsActive]=@IsActive
      ,[Remark]=@Remark
      ,[LBOh]=@LBOh
      ,[LBRate]=@LBRate
	  ,[SubID] =@SubID
      ,[RequestNo] =@RequestNo
	  else
	 update TransCusFormula
	 set [Component]=@Component
      ,[SubType]=@SubType
      ,[Description]=@Description
      ,[Material]=@Material
      ,[Result]=@Result
      ,[Yield]=@Yield
      ,[RawMaterial]=@RawMaterial
      ,[Name]=@Name
      ,[PriceOfUnit]=@PriceOfUnit
	  ,[AdjustPrice]=@AdjustPrice
      ,[Currency]=@Currency
      ,[Unit]=@Unit
      ,[ExchangeRate]=@ExchangeRate
      ,[BaseUnit]=@BaseUnit
      ,[PriceOfCarton]=@PriceOfCarton
      ,[IsActive]=@IsActive
      ,[Remark]=@Remark
      ,[LBOh]=@LBOh
      ,[LBRate]=@LBRate where Id=@Id
END
go

