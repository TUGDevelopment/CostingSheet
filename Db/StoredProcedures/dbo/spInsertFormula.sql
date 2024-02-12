-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertFormula]
	-- Add the parameters for the stored procedure here
	@Id int,
	@Component nvarchar(max),
	@SubType nvarchar(max),
	@Description nvarchar(max),
	@Material nvarchar(max),
	@Result nvarchar(max),
	@Yield nvarchar(max),
	@RawMaterial nvarchar(max),
	@Name nvarchar(max),
	@PriceOfUnit nvarchar(max),
	@Currency nvarchar(max),
	@Unit nvarchar(max),
	@ExchangeRate nvarchar(max),
	@BaseUnit nvarchar(max),
	@PriceOfCarton nvarchar(max),
	@Formula nvarchar(max),
	@IsActive nvarchar(max),
	@RequestNo nvarchar(max),
	@user nvarchar(max),
	@Remark nvarchar(max),
	@LBOh nvarchar(max),
	@LBRate nvarchar(max),
	@Mark nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @user nvarchar(max) ='FO5910155'
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	--select * into #temp from dbo.FindULevel(@user)
	--declare @userlevel nvarchar(max) =(select idx from #temp where idx=0)
	--print @userlevel;
    -- Insert statements for procedure here
--if (select count(*) from TransFormula where ID=@Id)=0
--	begin
	--if(@user=0)
	----Isactive
	--	update TransFormula set ExchangeRate=case when Currency=@Currency and @ExchangeRate<>'' then @ExchangeRate else ExchangeRate end,
	--	Isactive=@Isactive
	--	where RequestNo=@RequestNo and Formula=@Formula
	--else
	if @RequestNo>0
		if(@Mark='D')
			delete TransFormula where ID=@ID
		else if(select count(Id) from TransFormula where Id=@Id and RequestNo=@RequestNo)>0
		update TransFormula set Component=@Component,SubType=@SubType,
		LBOh=@LBOh,
		LBRate=@LBRate,
		Description=@Description,	
		Material=@Material,
		Result=@Result,
		Yield=@Yield,
		RawMaterial=@RawMaterial,
		name=@Name,
		PriceOfUnit=@PriceOfUnit,
		Currency=@Currency,
		Unit=@Unit,
		ExchangeRate=@ExchangeRate,
		BaseUnit=@BaseUnit,
		PriceOfCarton=@PriceOfCarton,
		Formula=@Formula,
		IsActive=@IsActive,
		Remark=@Remark where Id=@Id
		else
		if(@Mark='X')
		insert into TransFormula values(@Component,
		@SubType,
		@Description,	
		@Material,
		@Result,
		@Yield,
		@RawMaterial,
		@Name,
		@PriceOfUnit,
		@Currency,
		@Unit,
		@ExchangeRate,
		@BaseUnit,
		@PriceOfCarton,
		@Formula,
		@IsActive,
		@Remark,
		@LBOh,
		@LBRate,
		@RequestNo 
			  )
		set @Id  = (SELECT CAST(scope_identity() AS int))
		--select @Id;
--end
END

 --update TransFormula set IsActive=0

go

