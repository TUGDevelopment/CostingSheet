-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sptunastdGrading] 
	-- Add the parameters for the stored procedure here
	@Code nvarchar(max),
	@Result nvarchar(max) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @code nvarchar(max)='3AYOCN3FCA7NVLNNRU'
	SET NOCOUNT ON;	
	declare @Grading nvarchar(max),@Styles nvarchar(max)
	--	declare @FishGroup nvarchar(max)='SKJMPA',	@Grading nvarchar(max)='PREMIUM',	@Styles nvarchar(max)='None',	@Code nvarchar(max)='3AA7CBBHBALSS4OC5H'
	if(select count(isnull(@Grading,'')))=1 begin
		if(select count(PH2Des) from stdGradingL1 where ID = substring(@Code,14,1) and PH2Des<>'STANDARD')>0
			set @Grading =(select PH2Des from stdGradingL1 where ID = substring(@Code,14,1))
		else if (select count(*) from stdGradingL2 where ID = substring(@Code,5,1) and TunaM2<>'101')>0 begin
			set @Styles =(select TunaM2 from stdGradingL2 where ID = substring(@Code,5,1))
			set @Grading =(select GradeDes from stdGradingL3 where ID = @Styles)
			end
		else if (select count(*) from stdGradingL2 where ID = substring(@Code,5,1) and TunaM2='101')>0
			 if(substring(@Code,9,1)='D')
				set @Grading =(select GradeDes from stdGradingL5 where ID = substring(@Code,9,2))
			else if(substring(@Code,9,1)<>'D')
				set @Grading =(select top 1 GradeDes from stdGradingL4 where SAPDigit = substring(@Code,9,1))	
	end
	--print @Yeild
	set @Grading = (case when @Grading = 'PREMIUM' then 'Canada' else @Grading end)
	select @Result=@Grading
END
 
go

