-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetQuotationItems]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@requestno nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @requestno nvarchar(max)=31,@Id nvarchar(max)=0
    -- Insert statements for procedure here	
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	SELECT a.Name,isnull(z.Code_Adjust,a.Code) as 'Code',a.RefSamples,
	case when isnull(RequestType,'')='V' then (select b.MarketingNumber from TransCostingHeader b where b.ID=z.RequestNo) else
	a.CostNo end as CostNo,a.RequestNo,'' as Mark,	
	isnull(Overprice,'')'Overprice',
	isnull(Extracost,'')'Extracost',
	isnull(z.subContainers,'0') as 'SubContainers',
	isnull(z.MinPrice,'0') as 'MinPrice',
	isnull(z.ExchangeRate, (select t.ExchangeRate from TransCostingHeader t where t.ID=z.requestno) ) as 'ExchangeRate',
	case when z.OfferPrice ='' or z.OfferPrice is null  then '0' else z.OfferPrice end  as 'OfferPrice',
	case when OfferPrice_Adjust ='' or OfferPrice_Adjust is null  then '0' else OfferPrice_Adjust end as OfferPrice_Adjust,

	--'0' as 'Freight',
	--'0' as 'Insurance',
	isnull(RequestType,'') as RequestType,
	isnull(z.SubID,'') as SubID,
	isnull(z.CostingNo,'') as CostingNo,isnull(z.ID,0) as ID into #temp
	 from  TransQuotationItems z left join TransFormulaHeader a 
	on a.ID=z.CostingNo  where z.SubID=@requestno 

 select *,
	case when OfferPrice_Adjust ='0' then '0' else 
	(case when (convert(float,OfferPrice) - convert(float,OfferPrice_Adjust)) > 0.04 then 'A'end) end as 'StatusApp',

	case when OfferPrice_Adjust ='0' then '0' else (convert(float,OfferPrice) - convert(float,OfferPrice_Adjust)) end as 'Diff',
	case when OfferPrice_Adjust ='0' then '0' else
    ((convert(float,OfferPrice) - convert(float,OfferPrice_Adjust)) / convert(float,OfferPrice) * 100) end as 'PercentDiff',
	isnull(OfferPrice,'0') as OfferPriceExch,isnull(MinPrice,'0') as MinPriceExch from #temp
	--update TransFormulaHeader set isactive=1 where requestno=6443 
	--select * from #temp where requestno=8918 
END
go

