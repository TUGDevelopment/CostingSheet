-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sptest]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

If Object_ID('tempdb..#temp')  is not null  drop table #temp
select *,REPLACE(RawMaterial,'-1','')'nn' into #temp from MasYield
SELECT [Id]
      ,[Company]
	  --,a.Material
      ,isnull(REPLACE(a.Material,b.OldCode,b.Material),a.Material)'Material'
      ,[Name]
      --,[RawMaterial]
	  ,isnull(b.Material,RawMaterial)'RawMaterial'
      ,[Description]
      ,[Yield]
  FROM #temp a left join SAPMaterial b on b.OldCode=a.nn
END


go

