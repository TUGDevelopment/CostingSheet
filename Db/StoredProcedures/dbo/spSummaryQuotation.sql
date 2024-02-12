CREATE PROCEDURE [dbo].[spSummaryQuotation]
	-- Add the parameters for the stored procedure here
	@UserNo nvarchar(max),
	@from datetime,
	@to datetime,
	@material nvarchar(max),
	@requestno nvarchar(max),
	@costing nvarchar(max),
	@statusapp nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--select * from ulogin where email like '%juthamard.aksorn@thaiunion.com%'
	--declare @UserNo nvarchar(max)='MO610314',@from datetime='20210220',@to datetime='20220324',@material nvarchar(max)='',@requestno nvarchar(max)='',@statusapp nvarchar(max)='2',@costing nvarchar(max)=''
	declare @Keyword nvarchar(max)='convert(nvarchar(max),cast([CreateOn] as date)) Between '''+ convert(nvarchar(max),cast(@from as date))+''' and '''+ convert(nvarchar(max),cast(@to as date))+''''
	--print @Keyword;
	If Object_ID('tempdb..#find')  is not null  drop table #find
	select * into #find from dbo.FindULevel(@UserNo)
	--select * from #find
	SET NOCOUNT ON;
	declare @temp tabletype;
	delete @temp
	insert into @temp
	select * from(select editor from #find)#a
	declare @editor nvarchar(max)= dbo.fnc_stuff(@temp)

	print @editor;
	declare @str nvarchar(max)
	declare @table Table(ID nvarchar(max),RequestNo nvarchar(max),CreateOn datetime)
	set @str=N'select ID,RequestNo,CreateOn from TransQuotationHeader where dbo.fnc_checktype(CreateBy,'''+@editor+''')>0'
	declare @sql NVARCHAR(MAX)
	SET @sql = CONCAT( @str ,' and requestno in (select LTRIM(RTRIM(value)) from dbo.FNC_SPLIT((case when '''+@requestno+'''<>'''' then '''+@requestno+''' else requestno end),'','')) and statusapp in (3,4,0,8,9) and ' , @Keyword ) 
	print @sql;
	insert into @table EXEC sp_executesql @sql
	If Object_ID('tempdb..#Qtemp')  is not null  drop table #Qtemp

    -- Insert statements for procedure here
	SELECT a.*,b.RequestNo as Quotation,b.Customer,b.ShipTo,b.StatusApp 'Status',b.Currency,
	b.Incoterm,
	(select CONCAT(Code +':',Value) from MasPaymentTerm where Code=b.PaymentTerm) as 'PaymentTerm'
	into #Qtemp from TransQuotationItems a left join TransQuotationHeader b on b.ID=a.SubID
	where a.SubID in (select ID from @table)
	
	If Object_ID('tempdb..#TransCostingHeader')  is not null  drop table #TransCostingHeader
	select b.ID,(select top 1 RequestNo from TransTechnical a where a.ID=b.RequestNo)TechnicalNo,
	(select top 1 RequestDate from TransTechnical a where a.ID=b.RequestNo)[Form],
	(select top 1 RequireDate from TransTechnical a where a.ID=b.RequestNo)[To] into #TransCostingHeader
	from TransCostingHeader b where id in (select RequestNo from #Qtemp) 
	--and b.StatusApp = case when (select count(*) from dbo.FNC_SPLIT(@statusapp,',') where value in (3))>0 then 4 else b.StatusApp end 
	--select * from TransQuotationHeader
	select * from(
	select ROW_NUMBER() OVER (
	ORDER BY #a.ID
   ) row_num,#a.ID,
	#a.Customer as 'Sold-To-Code',
	#a.Customer,
	#a.ShipTo as 'Ship-To-Code',
	#a.ShipTo,#a.Incoterm,#a.PaymentTerm,
	(case  SUBSTRING(c.CostNo,1,2) when 'PF' then '103' 
	when 'GP' then '102' end )'sale Org',
	(case when #a.RequestType='V' then (select h.VarietyPack from TransCostingHeader h where h.ID=#a.requestno) else
	c.Code end) as 'Code',c.Name,#a.OfferPrice_Adjust,format(#a.Form,'dd/MM/yyyy')'Form',format([To],'dd/MM/yyyy') as 'To',
	#a.MinPrice,
	#a.OfferPrice,
	
	#a.ActionDate,
	case when convert(nvarchar(max),#a.StatusApp)='A' then 'Below Cost' else '-' end 'StatusApp',
	case #a.IsActive when '4' then 'Send To complete' else '' end 'IsActive',
	#a.Quotation,
	#a.TechnicalNo 'requestno',
	
	
	#a.SubID,
	c.Formula,
	(select s.Title from MasStatusApp s where s.id=isnull(#a.Status,0) and s.levelapp in (3,2))'StatusName',
	(case when isnull(#a.Status,0)=4 then (select max(h.createon) from MasHistory h where h.tablename='TransQuotationHeader' and h.StatusApp in (1,4) and h.RequestNo=#a.SubID) else null end)  [complete process],
	isnull(c.CostNo,'') as CostNo,
	#a.Currency
	from(
	select a.*,b.TechnicalNo,b.Form,[To] from #Qtemp a left join #TransCostingHeader b on b.ID=a.RequestNo
	where isnull(a.IsActive,0) = case when (select count(*) from dbo.FNC_SPLIT(@statusapp,',') where value in (1))>0 then 4 
	when (select count(*) from dbo.FNC_SPLIT(@statusapp,',') where value in (2))>0 then 0 else a.IsActive end 
	)#a left join TransFormulaHeader c on c.ID=convert(int,#a.CostingNo)
	where isnull(#a.Quotation,'')<>'')#b where CostNo like '%' +  (case when len(@costing)>0 then @costing else CostNo end)  +'%' and Code like '%' + (case when len(@material)>0 then @material else Code end) +'%'
END
go

