-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetEditCosting] 
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@user nvarchar(max)
AS
BEGIN
	--declare @ID nvarchar(max)=2339
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--set @ID =141
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
    -- Insert statements for procedure here
	select a.ID,MarketingNumber,CostNo,b.PackSize,b.Packaging,Netweight,b.Code,b.Customer,CanSize,b.Formula into #temp
	from TransCostingHeader a left join TransFormulaHeader b on a.ID=b.RequestNo where a.StatusApp in (0,4,8) and CostNo !=''
	If Object_ID('tempdb..#newtable')  is not null  drop table #newtable
	SELECT a.ID
	  ,ROW_NUMBER() OVER(ORDER BY Series ASC) AS RowID
      ,RequestNo
      ,a.Material as 'Code'

      ,
	  case when a.CostingSheet='' or a.CostingSheet is null then
	  (case when len(Material)=18 then (
	  select top 1 t.CostNo from #temp t where t.Id in(select max(b.id) from #temp b where b.Code= a.Material) and t.Code= a.Material
	  ) 
	  when (SUBSTRING(Material,1,2)='3V') then
		(select max(b.MarketingNumber) from TransCostingHeader b where b.VarietyPack=a.Material)
	  else a.CostingSheet end) else a.CostingSheet end 'CostingSheet'
      ,isnull(Result,0) as 'Result' --,b.RDNumber
      ,Series--,a.Material as 'Code',
	  ,'' as StatusApp
	  ,SiteId
	  ,SellingUnit 
	  ,Units 
	  ,TotalPack	 
	  ,PackingStyle
	  ,VarietyPack
	 into #newtable FROM TransEditCosting a where RequestNo=@ID
	--select * from #newtable
	--1 maping costing
	update t set t.CostingSheet= 
	(case when t.CostingSheet='' or t.CostingSheet is null then a.CostNo else t.CostingSheet end)
	from #newtable t left join #temp a on t.CostingSheet = a.CostNo 
	where t.RequestNo=@ID and isnull(t.SiteId,0)=0 

	select ID,
	RowID,
	Code,
	CostingSheet,
	Result,
	Series,(case when 
	(select count(ID)'X' from #temp where #temp.CostNo=#newtable.CostingSheet)>0 then 'X' else '' end) as Mark,
	StatusApp,
	SiteId,
	SellingUnit, 
	Units,
	TotalPack,	 
	PackingStyle,
	VarietyPack,
	RequestNo
	from #newtable
END
 

go

