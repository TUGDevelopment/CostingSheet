 CREATE PROCEDURE [dbo].[spGetCustomerSyc]
AS
BEGIN

  If Object_ID('tempdb..#temp')  is not null  drop table #temp
  select * into #temp from [192.168.1.193].[CostingSheet].dbo.MasCustomer a where convert(int,a.code) not in  (select convert(int,b.code) from MasCustomer b) 
  insert into MasCustomer (Code,Name,Custom)
  select code,Name,custom from #temp --where Code like '%10035361%'
end
--select a,count(a) b from (
--select convert(int,code) a from MasCustomer)#a group by a
--delete MasCustomer where isnull(zone,'')=''
go

