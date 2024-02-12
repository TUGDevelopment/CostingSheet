-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spinsertLaborOverhead]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@LBName nvarchar(max),
	@PackSize nvarchar(max),
	@LBType nvarchar(max),
	@LBRate nvarchar(max),
	@Currency nvarchar(max),
	@LBMin nvarchar(max),
	@LBMax nvarchar(max),
	@Unit nvarchar(max),
	@tcode nvarchar(max),
	@BU nvarchar(max),
	@Categoty nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Company nvarchar(max)='162',	@tcode nvarchar(max)='Can'	
	--declare @Company nvarchar(max)='102',@LBName nvarchar(max)='NW 5-15G',
	--@PackSize nvarchar(max)='24',
	--@LBType nvarchar(max)='Sachet',
	--@LBRate nvarchar(max)=1,
	--@Currency nvarchar(max)='THB',
	--@LBMin nvarchar(max)=5,
	--@LBMax nvarchar(max)=15,
	--@Unit nvarchar(max)='G',
	--@tcode nvarchar(max)='Can'
	SET NOCOUNT ON;
	declare @LBCode nvarchar(max),@LBOh nvarchar(max)
	set @LBOh=(select LBOh from MasPrimary where Name=@tcode)
    -- Insert statements for procedure here
	--select * from MasLaborOverhead
	select @LBCode=(max(convert(int,SUBSTRING(LBCode,3,3)))) from MasLaborOverhead where Company=@Company
	and SUBSTRING(LBCode,1,2) = @LBOh 
	--select @LBOh+''+format(isnull(@LBCode,0)+1,'000');
	Insert into MasLaborOverhead values(@Company,@LBOh+''+format(isnull(@LBCode,0)+1,'000'),@LBName,
 	@PackSize,
	@LBType,
	@LBRate,@Currency,@LBMin,@LBMax,@Unit,dbo.fnc_settype(@BU),@Categoty)
END


go

