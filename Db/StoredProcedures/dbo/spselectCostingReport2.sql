-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spselectCostingReport2]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@username nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)='1218'
	SET NOCOUNT ON;
	
	declare @temp table (ID int)
	declare @Company nvarchar(max)
	declare @RequestType nvarchar(max),@CostingNo nvarchar(max),@Requester nvarchar(max),@RequestNo nvarchar(max)
	If Object_ID('tempdb..#table')  is not null  drop table #table
	select b.RequestType,a.MarketingNumber,a.RequestNo,a.Company,isnull(b.Requester,'')Requester,isnull(b.assignee,'')Assignee,CreateBy into #table
	from TransCostingHeader a left join TransTechnical b on b.ID = a.RequestNo where a.ID = @Id


	select @RequestType=RequestType,@CostingNo=MarketingNumber,@RequestNo=RequestNo,@Company=Company from #table
	set @Requester =dbo.fnc_getuser(3,@Company)+','+ dbo.fnc_getuser(4,@Company)--CD(3)Group level
	print @Requester;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	set @Requester=(select top 1 'K. '+FirstName from ulogin where [user_name] in (select value from dbo.FNC_SPLIT(@Requester,',') where value in
		(select value from dbo.FNC_SPLIT((select requester+';'+ assignee from #table),';'))))

	declare @createby nvarchar(max)
	set @createby =(select CreateBy from #table)--dbo.fnc_getuser(7,@Company)--CCST(7)
	set @createby=STUFF((SELECT distinct ',' + 'K. '+(c.FirstName) 
            FROM ulogin c where [user_name] in (select value from dbo.FNC_SPLIT(@createby,','))
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
	print @RequestType;
	if @RequestType=1
	begin
	declare @CostingSheet nvarchar(max)
	insert into @temp
	select ID from TransCostingHeader where MarketingNumber in
	(select substring(CostingSheet,1,8) from TransEditCosting where substring(Result,1,8)=@CostingNo)
	end
	else
	insert into @temp values (@Id)
	select CONVERT(nvarchar(max), a.UniqueColumn) AS 'UniqueColumn',convert(nvarchar(max),b.ID)'ID',a.Company,
	case when @RequestType=1 then @CostingNo else a.MarketingNumber end as 'MarketingNumber',a.RDNumber,a.PackSize, 
    b.RequestNo,convert(nvarchar(max),a.ID)'Folio',a.Remark,a.CanSize,a.Packaging,--case when a.Completed = 1 then 'true' else 'false' end 'Completed',
	isnull(@Requester,'') as 'Requester',
	@createby as 'createby',
    a.NetWeight,
	b.Customer,
	ExchangeRate,
	a.StatusApp,
	b.Packaging,
	format(b.RequireDate,'MMM-yyyy') as 'RequireDate'
	,case a.StatusApp when 4 then 1 else 0 end as 'editor' from TransCostingHeader a left join TransTechnical b on b.ID = a.RequestNo 
	where a.ID in (select ID from @temp)
END


go

