-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spTunaStdMedia2]
	-- Add the parameters for the stored procedure here
	@Code nvarchar(max),
	@from datetime,
	@to datetime,
	@Group nvarchar(max)
AS
BEGIN
	--declare @Code nvarchar(max)='3AAOCN2NSAON5IUUS4',	@from datetime='2023-01-01',@to datetime='2023-02-28',@Group nvarchar(max)='Labor',@request nvarchar(max)=0
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @Grading nvarchar(max)
	declare @Unitofmeasurement nvarchar(max),@stdPackSize nvarchar(max),@PackSize nvarchar(max),@pSize nvarchar(max)
	declare @Size nvarchar(max),@currency nvarchar(max),@Unit nvarchar(max)
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
 
	select * into #temp from StdTunaPrice where --@to between cast([From] as date) and cast ([To] as date)
	(@to between cast([From] as date) and cast ([To] as date))
	--select * from StdTunaPrice where code='3AA2CBABJAMN54PER3'
	if(@Group='Media') begin
	set @Unitofmeasurement = (select PackagingType from stdTunaPackaging where SApCodeDigit=substring(@Code,12,1)) 
	(select top 1 @stdPackSize=stdPackSize,@pSize=Size from StdOverheadCost where CanSize = substring(@Code,9,3) and PackagingType in 
	(select a.PackagingType from StdSAPPackaging a where SAPCodeDigit = substring(@Code,12,1)))
	set @PackSize = (select top 1 PackSize from StdPackingStyle where SApCodeDigit=substring(@Code,17,2) and Size=@pSize)

	select a.*,
	case when a.Unit ='Gram' then 
	(convert(float,a.MediaWeight)/1000)* b.Price else
	((((convert(float,a.MediaWeight))* b.Price )/convert(float,@stdPackSize))* convert(float,@PackSize))
	end as 'Result',isnull(b.Price,0) as 'Price',b.Currency,b.Unit as 'Unitofmeasurement' from StdFillWeightMedia a 
	left join #temp b on b.Code=a.Code and b.GroupType=a.GroupType where substring(SApCodeDigit,1,16)=substring(@Code,1,16) 
	--select * from StdOverheadCost
	end
	else if(@Group='Media2') begin
	set @Unitofmeasurement = (select PackagingType from stdTunaPackaging where SApCodeDigit=substring(@Code,12,1)) 
	(select top 1 @stdPackSize=stdPackSize,@pSize=Size from StdOverheadCost where CanSize = substring(@Code,9,3) and PackagingType in 
	(select a.PackagingType from StdSAPPackaging a where SAPCodeDigit = substring(@Code,12,1)))
	set @PackSize = (select top 1 PackSize from StdPackingStyle2 where SApCodeDigit=substring(@Code,17,2) and Size=@pSize)

	select a.*,
	case when a.Unit ='Gram' then 
	(convert(float,a.MediaWeight)/1000)* b.Price else
	((((convert(float,a.MediaWeight))* b.Price )/convert(float,@stdPackSize))* convert(float,@PackSize))
	end as 'Result',isnull(b.Price,0) as 'Price',b.Currency,b.Unit as 'Unitofmeasurement' from StdFillWeightMedia a 
	left join #temp b on b.Code=a.Code and b.GroupType=a.GroupType where substring(SApCodeDigit,1,16)=substring(@Code,1,16) 
	--select * from StdOverheadCost
	end
	else if(@Group='PKG') begin
		select a.*,b.Price,b.Currency,b.Unit,(select PackagingType from stdTunaPackaging where SApCodeDigit=substring(@Code,12,1)) 'Unitofmeasurement' from StandardPrimary  a 
		left join #temp b on b.Code=a.Code and b.GroupType=a.[Group] where substring(SApCodeDigit,1,16)=substring(@Code,1,16)
	end 
	else if(@Group='Labor') begin
	If Object_ID('tempdb..#tsap')  is not null  drop table #tsap
	select MVKE_PRODH into #tsap from tsapMVKE where MVKE_MATNR=@Code 
	and isnull(MVKE_PRODH,'')<>'' and SUBSTRING(isnull(MVKE_PRODH,''),1,1)='1' group by MVKE_PRODH
	declare @GradingName nvarchar(max) = (select top 1 Grading from MasGrading2 where SAPCodeDigit in (select SUBSTRING(isnull(MVKE_PRODH,''),6,5) from #tsap))
	exec sptunastdGrading @Code, @Result = @GradingName OUTPUT;
	declare @GroupStyle nvarchar(max) = (select GroupStyle from StandardStyle where sapcodedigit = substring(@Code,5,1))
	declare @CanSize nvarchar(max),@PackagingType nvarchar(max)
		select @Grading=Grading from MasGrading where sapcodedigit = substring(@Code,14,1)
		select @PackagingType=a.PackagingType from StdSAPPackaging a where SAPCodeDigit = substring(@Code,12,1)
		select * from(select top 1 'Laborcost' as Title,Cost,Currency,Unit,[From],[To],0 as StdPackSize,''Size,'' PackagingType from stdLaborcost2 where Grading=@GradingName
		and Style=@GroupStyle and Packaging =@PackagingType and @to between cast([From] as date) and cast ([To] as date) order by cost desc
		union select Title,Cost,Currency,Unit,[From],[To],StdPackSize,Size,PackagingType from StdOverheadCost where PackagingType=@PackagingType and CanSize = substring(@Code,9,3)
		union select Title,Cost,Currency,'KG per case',ValidFrom,[ValidTo],'0','0','' from StdColdStCost where 	Packaging =@PackagingType	
		)#a 
		where @to between cast([From] as date) and cast ([To] as date) order by case Title when 'Laborcost' then 1
		when 'Cold storage' then 2
		when 'FOH' then 3 when 'OOH' then 3 end asc

		--select * from StdPackingStyle2 
	end
	else if (@Group='Style') begin
		select @Size=Size,@currency=Currency,@Unit=Unit from StdOverheadCost where CanSize = substring(@Code,9,3)
		select *,@Unit as 'Unit',@currency as 'Currency','Packing style (Upcharge) - Standard' name from StdPackingStyle where SApCodeDigit=substring(@Code,17,2) and Size=@Size 
	end
	else if (@Group='Style2') begin
		select @Size=Size,@currency=Currency,@Unit=Unit from StdOverheadCost where CanSize = substring(@Code,9,3)
		select [SAPCodeDigit]
      ,[Size]
      ,[GroupStyle]
      ,[StyleRef]
      ,[Description]
      ,[PackSize]
      ,'0' as [LaborCost]
      ,[SecPKGCost],@Unit as 'Unit',@currency as 'Currency', CONCAT('Packing style (Upcharge) - Standard',' ',Type) name from StdPackingStyle2 where SApCodeDigit=substring(@Code,17,2) and Size=@Size union
		select substring(SAPCodeDigit,17,2),'','','',Description,packsize,laborcost,Currency,@Unit as 'Unit',Currency,'Packing style (Additional Upcharge) - '+SAPCodeDigit from StdPKGStyleSpecial where SAPCodeDigit=@Code
	end
	else if (@Group='Margin') begin
		select * from StdTunaMargin where Customer='D001' 
		and(@to between cast([From] as date) and cast ([To] as date))
	end
	else if (@Group='upcharge') begin
		select * from StandardUpcharge where isnull(sapdigit,'')=substring(@Code,12,2)
	end
END
go

