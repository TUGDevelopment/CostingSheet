-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spDelDuplicate
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 If Object_ID('tempdb..#t')  is not null  drop table #t
	 select count(costno)c,costno,Formula,RequestNo into #t from TransFormulaHeader group by CostNo,Formula,RequestNo

	 delete from TransFormulaHeader 
	 WHERE EXISTS ( 
	 select * from (select *,(select max(id) from TransFormulaHeader c where c.CostNo=#t.CostNo and c.Formula=#t.Formula and c.RequestNo=#t.RequestNo)'ID'
	  from #t where c>10 )#a where #a.CostNo=TransFormulaHeader.CostNo and #a.Formula=TransFormulaHeader.Formula and #a.RequestNo=TransFormulaHeader.RequestNo
	  and #a.ID <> TransFormulaHeader.ID)--order by c desc 

	If Object_ID('tempdb..#t')  is not null  drop table #TransFormulaHeader
	 delete
	 --select * into #TransFormulaHeader from 
	 TransFormulaHeader where CostNo='GP61256401' and (id between 7190 and 7199)
 
	 --select * from #TransFormulaHeader where CostNo='GP61256401'
END
go

