CREATE PROCEDURE [dbo].[spGetProductFormula]
@Id nvarchar(max)
AS
BEGIN
--declare @id nvarchar(max)=53042
select 
 a.ID
      ,[Name]
      ,a.Customer
      ,[Code]
      ,[RefSamples]
      ,[Formula]
      ,a.IsActive
      ,[Costper]
      ,[CostNo]
      ,a.Revised
      ,[MinPrice]
      ,[PID]
      ,[SellingUnit]
      ,[Ref]
      ,case when isnull(a.Packaging,'')!='' then isnull(a.Packaging,'') else b.Packaging end as Packaging
      ,case when isnull(a.PackSize,'')!='' then isnull(a.PackSize,'') else b.PackSize end as 'PackSize'
      ,a.RequestNo

,isnull(Code,'') as 'Material',b.UserType,case when isnull([NW],'')!='' then isnull([NW],'') else (select top 1 value from dbo.FNC_SPLIT( b.Netweight,'|')) end as 'NW'
--,(select w.Name from MasPrimary w where w.ID=a.Packaging)pack
from TransFormulaHeader a left join TransCostingHeader b on b.ID=a.RequestNo where a.Requestno=@Id
end

 
go

