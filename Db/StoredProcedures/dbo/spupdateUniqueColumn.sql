/****** Script for SelectTopNRows command from SSMS  ******/
create PROCEDURE [dbo].[spupdateUniqueColumn]
AS
BEGIN
If Object_ID('tempdb..#temp')  is not null  drop table #temp
SELECT count(id)x, [UniqueColumn] into #temp
  FROM [CostingSheet].[dbo].[TransTechnical] group by UniqueColumn

  select * from TransTechnical where UniqueColumn in ( select #temp.UniqueColumn from #temp where x >1)
  and RequestType=0

  --select * from TransTechnical where UniqueColumn='6AEE3012-1CF5-4E25-AF51-24011D0E59D3'

  end
go

