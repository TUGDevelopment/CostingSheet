-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sptunastdYield] 
	-- Add the parameters for the stored procedure here
	@FishGroup nvarchar(max),
	@Grading nvarchar(max),
	@Styles nvarchar(max),
	@Code nvarchar(max),
	@Result nvarchar(max) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--	declare @FishGroup nvarchar(max)='SKJMPA',	@Grading nvarchar(max)='PREMIUM',	@Styles nvarchar(max)='None',	@Code nvarchar(max)='3AA7CBBHBALSS4OC5H'
	
	declare @Yeild nvarchar(max)
	if(@Styles='SHD')
		set @Yeild ='100';
	else
		set @Yeild = concat('',(select Yield from StdspcYield where substring(Material,1,16)=substring(@Code,1,16)))
	if(@Yeild='')
		set @Yeild = concat('',(select Yield from StandardYield where FishGroup=@FishGroup and Grading=@Grading))

	--print @Yeild
	select @Result=@Yeild
END
 
go

