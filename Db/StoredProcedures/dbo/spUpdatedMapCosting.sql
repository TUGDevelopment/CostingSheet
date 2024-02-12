-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spUpdatedMapCosting
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
    -- Insert statements for procedure here
	select Material,Costing,CreateOn into #temp from (SELECT * from t1_TransMapCosting union select * from t2_TransMapCosting)#a
	group by Material,Costing,CreateOn
	--select * from #temp
	delete #temp where substring(material,17,2)='00'
	
	delete TransMapCosting where Costing in (select costing from #temp)
	insert into TransMapCosting (Material,Costing,CreateOn)
	select * from #temp

	Update t SET 
       t.Code=t2.Material
	FROM TransFormulaHeader t LEFT JOIN 
       #temp t2 ON 
       t2.Costing=t.CostNo where t.CostNo in (select Costing from #temp)
END
go

