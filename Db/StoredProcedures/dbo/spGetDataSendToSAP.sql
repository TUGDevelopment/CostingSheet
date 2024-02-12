-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetDataSendToSAP]
	-- Add the parameters for the stored procedure here
	@user nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @ID nvarchar(max)=10
    -- Insert statements for procedure here
	-- select * from TransQuotation where subid in (9,10)
	declare @dif int=360,@diffo int =10
	If Object_ID('tempdb..#TransCostingHeader')  is not null  drop table #TransCostingHeader
	select * into #TransCostingHeader from TransCostingHeader where StatusApp=4 and 
	((case when [ModifyOn] is not null then [ModifyOn] else CreateOn end)  between getdate()-@diffo and getdate())
	and UserType=0 and RequestNo <> 0 and [To] is not null
	If Object_ID('tempdb..#table')  is not null  drop table #table
	select a.*,b.[From],b.[To],b.CreateOn,b.ModifyOn into #table from TransFormulaHeader a left join #TransCostingHeader b on b.ID=a.RequestNo
	where a.IsActive=0 and a.requestno in (select c.ID from #TransCostingHeader c where c.requestno<>0 ) and isnull(Code,'')<>''
	--select * from TransQuotationHeader

	If Object_ID('tempdb..#t')  is not null  drop table #t
	select a.*,b.RequestNo as 'subRequestNo'
	,b.ID 'QuotaID',(select top 1 ExchangeRate from TransCostingHeader c where c.ID=b.RequestNo )Rate into #t 
	from TransQuotationHeader a left join TransQuotationItems b on a.ID=b.SubID 
	where ((case when a.ModifyOn is not null then a.ModifyOn else a.CreateOn end) between getdate()-@dif and getdate()) and
	a.StatusApp=4 and a.ID 
	in (select c.SubID from TransQuotationItems c where isnull(c.isactive,0) <>4) 
	--select * from #t 
	--Update t1 
	--	SET t1.Code=t2.Material,t1.Costper=(case when t2.Material is null then 0 else 1 end)
	--	FROM TransFormulaHeader t1 LEFT JOIN TransMapCosting t2 ON t1.CostNo=t2.Costing where t1.Costper=0 --and t1.RequestNo=@ID
	
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	SELECT #a.*,b.Customer as 'SoldTo',b.ShipTo,b.ID as 'SubID',b.Incoterm,b.QuotaID,
	b.PaymentTerm into #temp from (select a.[From],a.[To],a.ID from TransCostingHeader a 
	where a.RequestNo >0 and a.StatusApp not in (2,3) and a.ID in (select distinct subRequestNo from #t))#a 
	left join #t b on #a.ID=b.subRequestNo 
	--and #a.ID=@ID
	--select * from #temp
	--select * from #t a left join #temp b on a.SubID=b.ID and a.ID=b.subRequestNo
	If Object_ID('tempdb..#data')  is not null  drop table #data
	select b.RequestNo,b.Formula,
	(case when isnull(b.CostNo,'')<>'' then b.CostNo end) 'CostNo',
	b.Code,#a.MinPrice,#a.OfferPrice,
	isnull(#a.OfferPrice_Adjust,'0') 'PriceUpload',
	#a.SubID,#a.ID,#a.RequestNo 'IDSub',#a.CreateOn,#a.ModifyOn,#a.Currency,#a.ExchangeRate
	into #data
	from (select 
	   b.ID
      ,[Overprice]
      ,[Extracost]
      ,[subContainers]
      ,[CostingNo]
      ,[MinPrice]
      ,[OfferPrice]
      ,[OfferPrice_Adjust]
      ,[Formula]
      ,[IsActive]
      ,[ActionDate]
      ,b.StatusApp
      ,b.ExchangeRate as Rate
      ,b.RequestNo 
      ,[SubID]
	
	,x.RequestNo'RequestSub',x.Customer,x.ShipTo,x.CreateOn,x.ModifyOn,x.Currency,x.ExchangeRate from TransQuotationHeader x left join TransQuotationItems b on x.ID=b.SubID 
	where x.ID in (select distinct Id from #t) and isnull(b.isactive,0) <>4)#a 
	left join TransFormulaHeader b on #a.CostingNo=b.ID
	 
	--select * from TransFormulaHeader where ID=59505
	--select * from #table
	--select * from #data where id=1456
 
	delete #data where isnull(Code,'') =''
	if (select count(ID) from #data)>0 
	select *,'' as 'OfferPriceExch','' as 'MinPriceExch',(case substring(CostNo,1,2) when 'PF' then '103' when 'GP' then '102' end) as 'SalesOrg',
	Currency,'CAR' as 'SalesUnit','1' as 'PricingUnit'from ( 
	select b.ExchangeRate,
	(select distinct Rate from #t where #t.ID=b.SubID) as 'Rate',
	isnull((select distinct Currency from #t where #t.ID=b.SubID),'USD') as 'Currency',
	convert(float,b.PriceUpload) as PriceUpload,
	case when convert(float,b.PriceUpload)>0 then 	
	format(convert(float,b.PriceUpload),'###.00')
	when convert(float,b.OfferPrice)=0 then 	
	format(convert(float,b.MinPrice),'###.00') 
	else
	format(convert(float,b.OfferPrice),'###.00') end as 'OfferPrice',
	format(convert(float,b.MinPrice),'###.00') as 'MinPrice',
	a.Incoterm,
	a.PaymentTerm,
	b.Code,
	case when b.ID='' then b.CostNo else CONCAT(b.CostNo,'(',(select distinct RequestNo from #t where #t.ID=b.SubID),')')end 'CostNo',
	a.SoldTo,
	a.ShipTo,
	format(a.[From],'dd.MM.yyyy')RequestDate,
	format(a.[To],'dd.MM.yyyy')RequireDate,
	a.ID, b.ID 'subID',b.RequestNo,b.Formula,b.CreateOn,b.ModifyOn from #data b left join #temp a on 
	 a.SubID=b.SubID where b.ID in (select ID from #data where isnull(code,'')<>'') union
	 select '','','USD','','' as OfferPrice,format(convert(float,isnull(MinPrice,'')),'###.00') as 'MinPrice',
	 'FOB','',Code,CostNo,'','',format([from],'dd.MM.yyyy'),format([to],'dd.MM.yyyy'),ID,0,RequestNo,Formula,CreateOn,ModifyOn from #table)#a 
	 ORDER BY 
		CASE WHEN isnull(ModifyOn,'')='' THEN CreateOn else ModifyOn END asc
	 else
	 select *,'' as 'OfferPriceExch','' as 'MinPriceExch' from #data
END
go

