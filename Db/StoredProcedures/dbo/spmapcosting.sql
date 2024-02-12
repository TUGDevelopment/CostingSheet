 CREATE PROCEDURE [dbo].[spmapcosting]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max)
AS
BEGIN
 --declare @Id nvarchar(max)=4851
 If Object_ID('tempdb..#temp')  is not null  drop table #temp
 select formula,requestno into #temp from TransFormula where RequestNo=@Id group by
 formula,requestno

 If Object_ID('tempdb..#t')  is not null  drop table #t
 select #a.*,c.Formula as f,c.RequestNo as r into #t 
 from (
 select a.*,b.MarketingNumber,RDNumber
  from TransFormulaHeader a left join TransCostingHeader b on a.RequestNo=b.ID where a.RequestNo=@Id
  )#a left join #temp c on #a.Formula=c.Formula
  and #a.RequestNo=c.RequestNo 

  --delete TransFormulaHeader where RequestNo=0

  update a set a.costno=#t.MarketingNumber+''+ format(#t.Formula,'00')
  from TransFormulaHeader a left join #t on a.ID=#t.ID where a.ID in (select c.ID from #t c) and a.RequestNo=@Id
end
  --select * from TransFormulaHeader where Code='3HNNSO3SV2EPRJUCJR'
  --update TransFormulaHeader set CostNo='GP61259701' where code='3ICFS822L2S87XP2RU'
go

