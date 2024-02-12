/****** Script for SelectTopNRows command from SSMS  ******/
CREATE Procedure  [dbo].[spGetCostFormula]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@t nvarchar(max)
AS
BEGIN
--select * from Mascompany
--declare @Company nvarchar(max)='103',@t nvarchar(max)='1',@usertype nvarchar(max)=1
If Object_ID('tempdb..#temp')  is not null  drop table #temp
--declare @c nvarchar(max)=(select case when @usertype='1' then hf else fn end from Mascompany where Code=@Company)
--print @c;
SELECT ID,CostNo,isnull(Code,'-') as 'Material',Name,RequestNo into #temp
FROM TransFormulaHeader where isnull(CostNo,'')<>'' 
--and (case when len(isnull(CostNo,''))>0 then substring(isnull(CostNo,''),1,2) end ) in (@c) 
--and isnull(code,'')<>''
and RequestNo in (select Id from TransCostingHeader where StatusApp in (0,4) and Company=@Company)
--group by CostNo,Code

if (@t='0') begin
If Object_ID('tempdb..#t')  is not null  drop table #t
select Material,Name, costno into #t from #temp where requestno in (select max(requestno) from #temp c where c.Material=#temp.Material)

select #a.*,#t.CostNo,#t.Name from(
select distinct Material from #temp where Material not in ('','-') and substring(isnull(Material,''),1,1)='3' )#a left join
#t on #t.Material=#a.Material
order by #a.Material desc
end
if (@t='1')
select * from #temp order by CostNo desc
end
 
  
go

