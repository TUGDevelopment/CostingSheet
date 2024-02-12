CREATE PROCEDURE [dbo].[spGetFillWeight2] 
@Code nvarchar(max)
AS
BEGIN
 --declare @Code nvarchar(max)='3AAOCN2ABACN5INNRV'
If Object_ID('tempdb..#fw')  is not null  drop table #fw
declare @fw table(RowID int,Result float,Unit nvarchar(max),FishGroup nvarchar(max),SHD nvarchar(max),FishCert nvarchar(max))
declare @name nvarchar(max),@sql nvarchar(max),@skjgroup nvarchar(max)
select top 1 @name = isnull(name,'None'),@skjgroup=skjgroup from MasFishSpecies where sapcode = substring(@Code,3,2)
if (select count(*) from stdTunaSpecialFW where substring(Material,1,16)= substring(@Code,1,16))>0
set @sql=',FishGroup,0,0 from stdTunaSpecialFW where substring(Material,1,16)'
else if (select count(*)from stdTunaFixFW where substring(Material,1,16)= substring(@Code,1,16))>0 set @sql=',FishGroup,SHD,FishCert from stdTunaFixFW where substring(Material,1,16)'
else set @sql=',0,0,0 from StdFillWeight where substring(SapCodedigit,1,16)'
set @sql= concat('select ROW_NUMBER() OVER(ORDER BY Unit ASC) AS ID,replace(FillWeight,'','',''''),Unit ', @sql , '= substring('''+@Code+''',1,16)')
If Object_ID('tempdb..#tsap')  is not null  drop table #tsap
select MVKE_PRODH into #tsap from tsapMVKE where substring(MVKE_MATNR,1,16)= substring(@Code,1,16) 
and isnull(MVKE_PRODH,'')<>'' and SUBSTRING(isnull(MVKE_PRODH,''),1,1)='1' group by MVKE_PRODH
print @sql;
insert @fw EXEC sp_executesql @sql
SELECT *,convert(nvarchar(max),'') Yield,convert(nvarchar(max),'') Grading into #fw FROM  @fw
 --select * from #fw
declare @FishGroup nvarchar(max),@SHD nvarchar(100),@FishCert nvarchar(max),@RowID int
declare cur_fw CURSOR FOR
SELECT FishGroup,SHD,FishCert,RowID FROM  @fw
open cur_fw

FETCH NEXT FROM cur_fw INTO @FishGroup, @SHD, @FishCert,@RowID
WHILE @@FETCH_STATUS = 0
BEGIN
--declare @Code nvarchar(max)='3AAOCN2ABACN5INNRV'
declare @Styles nvarchar(max),
	@Grading nvarchar(max),
	@Yeild nvarchar(max)
	set @Grading= (select top 1 Grading from MasGrading2 where SAPCodeDigit in (select SUBSTRING(isnull(MVKE_PRODH,''),6,5) from #tsap))
	--print @Grading;
	exec sptunastdGrading @Code, @Result = @Grading OUTPUT;
	print @Grading
	--set @Grading=(select * from MasGrading2 where sapcodedigit = substring(@Code,14,1))
	set @Styles =case when @SHD='0' then (select GroupStyle from StandardStyle where sapcodedigit = substring(@Code,5,1)) else @SHD end
	set @FishGroup = case when @FishGroup='0' then (select FishGroup from MasFishSpecies where sapcode = substring(@Code,3,2)) else @FishGroup end
	set @FishCert = case when @FishCert='0' then (select FishCert from MasFishCert where sapcode = substring(@Code,15,2)) else @FishCert end
	exec sptunastdYield @FishGroup, @Grading, @Styles, @Code, @Result = @Yeild OUTPUT;
	update #fw set Yield=@Yeild,SHD=@Styles,FishGroup=@FishGroup,FishCert=isnull(@FishCert,'0'),Grading=@Grading where RowID=@RowID
	FETCH NEXT FROM cur_fw INTO @FishGroup, @SHD, @FishCert,@RowID
END

CLOSE cur_fw
DEALLOCATE cur_fw
--If Object_ID('tempdb..#tsap')  is not null  drop table #tsap
--select distinct MVKE_PRODH from #tsap from tsapMVKE where MVKE_MATNR=@Code 
--and isnull(MVKE_PRODH,'')<>'' and SUBSTRING(isnull(MVKE_PRODH,''),1,1)='1'
declare @GradingName nvarchar(max) = (select top 1 Grading from MasGrading2 where SAPCodeDigit in (select SUBSTRING(isnull(MVKE_PRODH,''),6,5) from #tsap))
--select *,@name as Name,@GradingName as Grading from #fw
select RowID,Result,Unit,FishGroup,SHD,FishCert,Yield,@name as Name,case when Grading='' then @GradingName else Grading end Grading,@skjgroup as 'skjgroup' from #fw
end
go

