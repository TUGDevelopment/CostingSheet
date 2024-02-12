-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetTunaCalculation] 
	-- Add the parameters for the stored procedure here
	@Param int,
	@Code nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Code nvarchar(max)='3AAOCM2MBAZL5IJWVU',@Param int=0
    -- Insert statements for procedure here
	declare @PackingStyle nvarchar(max),
	@SecPKGStyle nvarchar(max),
	@stdPackSize nvarchar(max),
	@GroupPKGStyle nvarchar(max),
	@Loss2ndPKG nvarchar(max),
	@PKGLaborCost nvarchar(max),
	@PackSize  nvarchar(max)
	--+++++++++++++++++++++
	select @stdPackSize=PackSize,@PackingStyle=StyleRef,@GroupPKGStyle=GroupStyle from StdPackingStyle where sapcodedigit = substring(@Code,17,2)
	declare @LaborCost nvarchar(max),@Loss1stPKG nvarchar(max) 
	--set @LaborCost= (select top 1 b.Cost from StdOverheadCost a left join StandardLOHGroup b on b.LOHGroup =a.LOHGroup where sapcodedigit = substring(@Code,9,3))
	--select * from StdSAPPackaging
	--select * from StdTunaPackaging where SAPCodeDigit = substring(@Code,9,3)
	print @GroupPKGStyle;
	If Object_ID('tempdb..#StdSAPPackaging')  is not null  drop table #StdSAPPackaging
	set @Loss1stPKG = (select b.Loss1stPKG from StdSAPPackaging a left join StdLossPrimaryPKG b on upper(b.LossType) = upper(a.packagingType)
	 where SAPCodeDigit = substring(@Code,9,3))
	set @SecPKGStyle =(select Cost from StdSecPackingCost where PKGStyle=@PackingStyle)
	set @Loss2ndPKG = (select LossSecPKG from StdLossSecPKG where GroupPKGStyle=@GroupPKGStyle)
	set @PKGLaborCost =(select PackingStyleCost from StdPKGLaborCost where GroupPKGStyle=@GroupPKGStyle)
	select @PackSize=StdPackSize from StdOverheadCost where CanSize = substring(@Code,9,3)
	--declare @Margin nvarchar(max),@Loss nvarchar(max)
	--select @Margin=Margin,@Loss=Loss from StdMarginLoss where Grading=@GradingName and (getdate() between [From] and [To]) and Isactive=0
	declare @table table(RowID int,Name nvarchar(max),Result float,Unit nvarchar(max),OrderIndex int,colsName nvarchar(max))
	insert into @table
	SELECT RowID,Name,isnull(
	case when Name='stdPackSize' then @stdPackSize
	when Name='Pack size'then @PackSize
	when Name='Labor cost' then @LaborCost
	when Name='% Loss 1st PKG' then @Loss1stPKG
	when Name='2nd PKG' then @SecPKGStyle
	when Name='%Loss 2nd PKG' then @Loss2ndPKG
	when Name='Labor packing' then @PKGLaborCost
	else Result end,0) 'Result',Unit,OrderIndex,colsName from StdCalculation 
	union select concat('21',ROW_NUMBER() OVER(ORDER BY GroupType)),GroupDescription,MediaWeight,Unit,3,'MediaWeight'
	from StdFillWeightMedia b where SapCodedigit= substring(@Code,1,16)
	--+++++++++++++++++++++
	select * from @table order by OrderIndex
END


go

