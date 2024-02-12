-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spTunaStdMedia]
	-- Add the parameters for the stored procedure here
	@Code nvarchar(max),
	@from datetime,
	@to datetime,
	@Group nvarchar(max)
AS
BEGIN
	--declare @Code nvarchar(max)='3AA2CBABJAMN54PER3',	@from datetime='2021-12-01',@to datetime='2022-01-01',@Group nvarchar(max)='media'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @Grading nvarchar(max)
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select * into #temp from StdTunaPrice where --@to between cast([From] as date) and cast ([To] as date)
		(@from between cast([From] as date) and cast ([To] as date) Or @to between cast([From] as date) and cast ([To] as date))
	--select * from #temp where code='4100951'
	if(@Group='Media') begin
	declare @Unitofmeasurement nvarchar(max)
	set @Unitofmeasurement = (select PackagingType from stdTunaPackaging where SApCodeDigit=substring(@Code,12,1)) 
	select a.*,(convert(float,a.MediaWeight)/1000)* b.Price as 'Result',b.Price,b.Currency,b.Unit as 'Unitofmeasurement' from StdFillWeightMedia a 
	left join #temp b on b.Code=a.Code and b.GroupType=a.GroupType where substring(SApCodeDigit,1,16)=substring(@Code,1,16) 
	--select * from StdFillWeightMedia 
	end
	else if(@Group='PKG') begin
		select a.*,b.Price,b.Currency,b.Unit,(select PackagingType from stdTunaPackaging where SApCodeDigit=substring(@Code,12,1)) 'Unitofmeasurement' from StandardPrimary  a 
		left join #temp b on b.Code=a.Code and b.GroupType=a.[Group] where substring(SApCodeDigit,1,16)=substring(@Code,1,16)
	end 
	else if(@Group='Labor') begin
	declare @CanSize nvarchar(max),@PackagingType nvarchar(max)
		select @Grading=Grading from MasGrading where sapcodedigit = substring(@Code,14,1)
		select @PackagingType=a.PackagingType from StdSAPPackaging a where SAPCodeDigit = substring(@Code,12,1)
		select * from(select top 1 'Laborcost' as Title,Cost,Currency,Unit,[From],[To],0 as StdPackSize,''Size,'' PackagingType from stdLaborcost where Grading=@Grading
		and Packaging=@PackagingType
		union select Title,Cost,Currency,Unit,[From],[To],StdPackSize,Size,PackagingType from StdOverheadCost where PackagingType=@PackagingType and CanSize = substring(@Code,9,3))#a 
		where @from between cast([From] as date) and cast ([To] as date) Or @to between cast([From] as date) and cast ([To] as date) order by case Title when 'Laborcost' then 1
		when 'FOH' then 2 when 'OOH' then 2 end asc
		--select * from StdOverheadCost 
	end
	else if (@Group='Style') begin
	declare @Size nvarchar(max),@currency nvarchar(max),@Unit nvarchar(max)
		select @Size=Size,@currency=Currency,@Unit=Unit from StdOverheadCost where CanSize = substring(@Code,9,3)
		select *,@Unit as 'Unit',@currency as 'Currency' from StdPackingStyle where SApCodeDigit=substring(@Code,17,2) and Size=@Size
	end
	else if (@Group='Margin') begin
		select * from StdTunaMargin where Customer='D001' 
		and( @from between cast([From] as date) and cast ([To] as date) Or @to between cast([From] as date) and cast ([To] as date))
	end
END
 
go

