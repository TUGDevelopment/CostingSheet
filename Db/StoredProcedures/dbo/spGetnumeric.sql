/****** Script for SelectTopNRows command from SSMS  ******/
create PROCEDURE [dbo].[spGetnumeric]
as
begin
If Object_ID('tempdb..#temp')  is not null  drop table #temp
SELECT  [Id]
      ,[Company]
      ,[Material]
      ,[Description]
      ,case when ISNUMERIC( [PriceStd1])=1 then 0 else 1 end xx
      ,case when ISNUMERIC( [PriceStd2])=1 then 0 else 1 end zz
      ,[PriceStd2]
      ,[Currency]
      ,[Unit]
      ,[From]
      ,[To]
      ,[IsActive] 
  FROM [CostingSheet].[dbo].[MasPriceStd]

  SELECT [Id]
      ,[Material]
      ,[Description]
      ,case when ISNUMERIC( [Jan])=1 then 0 else 1 end[Jan]
      ,case when ISNUMERIC( [Feb])=1 then 0 else 1 end[Feb]
      ,case when ISNUMERIC( [Mar])=1 then 0 else 1 end[Mar]
      ,case when ISNUMERIC( [Apr])=1 then 0 else 1 end[Apr]
      ,case when ISNUMERIC( [May])=1 then 0 else 1 end[May]
      ,case when ISNUMERIC( [Jun])=1 then 0 else 1 end[Jun]
      ,case when ISNUMERIC( [Jul])=1 then 0 else 1 end[Jul]
      ,case when ISNUMERIC( [Aug])=1 then 0 else 1 end[Aug]
      ,case when ISNUMERIC( [Sep])=1 then 0 else 1 end[Sep]
      ,case when ISNUMERIC( [Oct])=1 then 0 else 1 end[Oct]
      ,case when ISNUMERIC( [Nov])=1 then 0 else 1 end[Nov]
      ,case when ISNUMERIC( [Dec])=1 then 0 else 1 end[Dec]
      ,[Currency]
      ,[Unit]
      ,[From]
      ,[To]
      ,[IsActive] into #temp
  FROM [CostingSheet].[dbo].[MasPricePolicy]

  select * from #temp where Mar=1
  end
go

