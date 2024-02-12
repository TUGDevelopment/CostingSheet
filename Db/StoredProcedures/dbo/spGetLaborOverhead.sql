-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetLaborOverhead]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@Packaging nvarchar(max),
	@NetWeight nvarchar(max),
	@PackSize nvarchar(max),
	@UserType nvarchar(max)
AS
BEGIN
	--declare @Company nvarchar(max)='103',	@Packaging nvarchar(max)='Pouch',@NetWeight nvarchar(max)=40,@PackSize nvarchar(max)=24,@UserType nvarchar(max)=0
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	If Object_ID('tempdb..#table')  is not null  drop table #table
	SET NOCOUNT ON;
	if(@Packaging='Cup')
	set @Packaging='Cup,Plastic container'
	select b.LBOh ,@NetWeight as 'NetWeight' into #table from MasPrimary b where b.Name in (select value  from dbo.FNC_SPLIT(@Packaging,',')) and dbo.fnc_checktype(b.usertype,@UserType)>0
	select b.ID,((b.LBRate / convert(float,b.PackSize))* @PackSize)LBRate,
	LBCode,
	LBName,
	@PackSize as 'PackSize',
	LBType,
	Currency
	from #table a left join MasLaborOverhead b on a.Lboh = substring(LBCode,1,2)
	and NetWeight between convert(float,lbmin) and convert(float,lbmax) 
	--and PackSize=@PackSize
	where Company=@Company order by LBName 
END
--select SUBSTRING('02001',1,2)
--select * from MasLaborOverhead


go

