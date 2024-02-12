/****** Script for SelectTopNRows command from SSMS  ******/
CREATE Procedure [dbo].[spGetQuotaCustomer]
	-- Add the parameters for the stored procedure here
	--@Id nvarchar(max),
	@requestno nvarchar(max),
	@value nvarchar(max)
AS
BEGIN
--declare @Id nvarchar(max)=7,	@requestno nvarchar(max)=20,	@value nvarchar(max)='Customer'
--If Object_ID('tempdb..#temp')  is not null  drop table #temp
--declare @icount int  = (select count(*) from TransQuotationCustom where requestno=@requestno)
if @value='Customer'
SELECT ID,--isnull((select MarketingNumber from TransCostingHeader a where a.ID= t.RequestNo),'') as Costing,
'' as CostingNo,
RequestNo,
Incoterm,
[Route],
Size,Quantity,PaymentTerm,
Commission,
Interest,
Freight,Insurance,'' Overprice,'' Extracost,Currency,
ExchangeRate,Remark,Customer,ShipTo,'' as SubContainers,'' as MinPrice,'' as OfferPrice,'' as PriceUpload,StatusApp,'' as Mark 
FROM TransQuotation t where SubId=@requestno
else
select --ROW_NUMBER() OVER(ORDER BY ProductName ASC) AS RowID,
ID,
ProductName,
isnull(Material,'') as Material,
isnull(Customer,'') as Customer,
isnull(ShipTo,'') as ShipTo,
RD_ref,
CostingNo,
OfferPrice,SubID,StatusApp,RequestNo,
'' as Mark,
''Incoterm,
''[Route],
''[Size],
''[Diff],
''PercentDiff,
''PaymentTerm,
''Commission,
''Interest,
''Freight,
''Insurance,
''ExchangeRate,
'' as MinPrice
 from TransQuotationCustom where SubId=@requestno
--if(@icount=0)
--select * from TransQuotationCustom where costingno in (select costno from TransFormulaHeader where RequestNo=@requestno)
end
 
go

