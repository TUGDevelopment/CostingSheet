CREATE PROCEDURE [dbo].[spGetFormulaHeader] 
@id nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @id nvarchar(max)=18471
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select distinct formula,RequestNo into #temp from TransFormula where RequestNo=@id
	--select * from #temp
	select case when ID is null then ROW_NUMBER() OVER (ORDER BY #a.RequestNo) else ID end ID,
	#a.Formula,isnull(Name,'') Name,#a.Costno,isnull(RefSamples,'') RefSamples,
	case when substring(Code,1,1)='3' then Code else '' end Code,case when c.RequestNo is null then 'X' else '' end as Mark,
	--convert(bit,case when c.IsActive=0 then 0 else 1 end)'IsActive',
	isnull(MinPrice,'0')'MinPrice',
	(select count(*) from TransRequestForm r where r.Costno=c.ID) as RequestForm,
	isnull((select n.NetWeight from TransProductList n where n.Id=PID),NW) NetWeight,
	isnull((select p.PackSize from TransProductList p where p.Id=PID),PackSize) PackSize,
	(select p.FixedFillWeight from TransProductList p where p.Id=PID) FixedFillWeight,PID,isnull(c.SellingUnit,'') as 'SellingUnit',
	isnull(ref,'')'Ref',c.NW,Packaging,c.RequestNo
	from(
	select #temp.RequestNo,Formula,b.MarketingNumber+''+ RIGHT('00' + CAST(#temp.Formula as varchar), 2)  as Costno 
	from #temp left join TransCostingHeader b on #temp.RequestNo=b.ID where b.ID=@id)#a left join 
	TransFormulaHeader c on #a.Formula=c.Formula and #a.RequestNo=c.RequestNo order by Formula
end

go

