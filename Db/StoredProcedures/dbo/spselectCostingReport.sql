-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spselectCostingReport]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@username nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)='18430',@username nvarchar(max)='fo5910155'
	SET NOCOUNT ON;
	declare @usertype nvarchar(max)
	select @usertype=usertype from ulogin where [user_name]=@username and IsResign=0
	declare @temp table (ID int)
	declare @Company nvarchar(max)
	declare @RequestType nvarchar(max),@CostingNo nvarchar(max),@Requester nvarchar(max),@RequestNo nvarchar(max)
	If Object_ID('tempdb..#table')  is not null  drop table #table
	select b.RequestType,a.MarketingNumber,a.RequestNo,a.Company,isnull(b.Requester,'')Requester,isnull(b.assignee,'')Assignee,CreateBy,
	b.RequestDate,b.RequireDate
	into #table
	from TransCostingHeader a left join TransTechnical b on b.ID = a.RequestNo where a.ID = @Id

	select @RequestType=RequestType,@CostingNo=MarketingNumber,@RequestNo=RequestNo,@Company=Company from #table
	set @Requester =dbo.fnc_getuser(3,@Company,@usertype)+','+ dbo.fnc_getuser(4,@Company,@usertype)--CD(3)Group level
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
	select ID from TransCostingHeader where ID in (@Id)
	end
	else
	insert into @temp values (@Id)
	select CONVERT(nvarchar(max), a.UniqueColumn) AS 'UniqueColumn',convert(nvarchar(max),isnull(b.ID,'0'))'ID',a.Company,
	case when @RequestType=1 then @CostingNo else a.MarketingNumber end as 'MarketingNumber',a.RDNumber,a.PackSize, 
    isnull(b.RequestNo,'-')'RequestNo',convert(nvarchar(max),a.ID)'Folio',a.Remark,a.CanSize,a.Packaging,--case when a.Completed = 1 then 'true' else 'false' end 'Completed',
	isnull(@Requester,'-') as 'Requester',
	isnull(@createby,'-') as 'createby',
    a.NetWeight,
	isnull((case when b.CustomPrice=1 then 'US Pet Nutrition / ' else '' end)+''+
	--isnull(b.Customer,(select top 1 isnull(Customer,'') from TransFormulaHeader t where t.RequestNo=a.ID))+''+isnull(' / '+b.Destination,''),'') as 'Customer',
	isnull(a.Customer,b.Customer+''+isnull(' / '+b.Destination,'')),'') as 'Customer',
	ExchangeRate,
	a.StatusApp,
	b.RequestDate,b.RequireDate,
	--b.Packaging,
	(select u.Name from MasUserType u where u.ID=b.UserType)'UserType',
	format(b.RequireDate,'MMM-yyyy') as 'RequireDate',format(a.[To],'MMM-yyyy') as 'To'
	,case a.StatusApp when 4 then 1 else 0 end as 'editor' from TransCostingHeader a left join TransTechnical b on b.ID = a.RequestNo 
	where a.ID in (select ID from @temp)
END



go

