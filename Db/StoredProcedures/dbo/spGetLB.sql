-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetLB]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@SubID nvarchar(max)

AS
BEGIN
	--declare @Company nvarchar(max)='1012',	@SubID nvarchar(max)='2785' 
 
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	declare @Packaging nvarchar(max),
	@NetWeight nvarchar(max),
	@PackSize nvarchar(max),
	@UserType nvarchar(max),
	@RequestNo nvarchar(max),
	@BU nvarchar(max)

	select @NetWeight=NetWeight,@PackSize=PackSize,@RequestNo=RequestNo from TransProductList where id=@subid
	select @UserType=UserType,@BU=PetCategory from TransTechnical where id=@RequestNo
	print @UserType;
	SET NOCOUNT ON;
	if(@Packaging='Cup')
	set @Packaging='Cup,Plastic container'
 
	If Object_ID('tempdb..#table')  is not null  drop table #table
	--select id,LBMax,ISNUMERIC(LBMax)'LBMaxx' into #table from MasLaborOverhead
	--select #table.id from #table where LBMaxx='0'
	--select * from MasLaborOverhead where id=14232

	select b.ID,b.LBRate,--((b.LBRate / convert(float,b.PackSize))* @PackSize)LBRate,
	LBCode,
	LBName,
	@PackSize as 'PackSize',
	LBType,
	Currency
	from MasLaborOverhead b where b.PackSize = @PackSize
	and @Company like Company+'%'
	and @NetWeight between 
	(case when unit='KG'  then convert(float,lbmin) * 1000 else 
	convert(float,lbmin) end) and (case when unit='KG'  then convert(float,lbmax) * 1000 else convert(float,lbmax) end) 
	--and PackSize=@PackSize
	and dbo.fnc_checktype(@UserType,isnull(BU,''))>0
	and dbo.fnc_checktype(@BU,isnull(Category,''))>0
	 order by LBName 
END
--select SUBSTRING('02001',1,2)
--delete MasLaborOverhead where company='101' and  (LBMax = '1003;1004')
--select convert(float,'0.08')
 



go

