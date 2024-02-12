-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetSapMaterial2]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--select * from massapmaterial
    -- Insert statements for procedure here
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	SELECT  code as Material,description1 as 'Description',isnull(old_code,'-')'old_code' into #temp
	from [192.168.1.195].[IHUBT01-01_IHUBRE].[dbo].SMITEMMS where substring(code,1,1) in ('3') and len(code)=18 --and isnull(f_delete,'') <> 'X'

	--delete  #temp where substring(#temp.Material,1,16) in (select substring(MatCode,1,16) COLLATE Latin1_General_CI_AS from StdTunaSpeCase)
	select ROW_NUMBER() OVER(ORDER BY Material ASC) AS Id,Material,[Description],'' old_code from(
	select * from #temp where #temp.Material in (select distinct a.Material COLLATE Latin1_General_CI_AS from StdSecPKGCost a) union
	select b.Material collate Thai_CI_AS,(select top 1 a.Description from StdCustomTitle a where a.Material=b.Material) collate Thai_CI_AS,'' from StdSecPKGCost b)#a
	where [Description]<>''
	group by Material,[Description]
END
go

