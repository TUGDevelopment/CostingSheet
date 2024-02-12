-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spselectcosting]
	-- Add the parameters for the stored procedure here
	@user_name nvarchar(max),
	@type nvarchar(max)
AS
BEGIN
	--declare @user_name nvarchar(max)='fo5910155',@type nvarchar(max)=0
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.//declare @Id nvarchar(max) ='1'
	declare @UserType nvarchar(max), @Bu nvarchar(max)--=dbo.fnc_stuff(@temp)
	select @Bu = Plant,@UserType=usertype from ulogin where [user_name]=@user_name
	If Object_ID('tempdb..#find')  is not null  drop table #find
	select idx,ulevel,editor,Sublevel,SubApp into #find from dbo.FindULevel(@user_name)
	SET NOCOUNT ON;
	declare @temp tabletype;delete @temp
	--insert into @temp select * from #find
	If Object_ID('tempdb..#TransCostingHeader')  is not null  drop table #TransCostingHeader
	select * into #TransCostingHeader from TransCostingHeader where RequestNo>0 and isnull(userType,'0') in (select value from dbo.FNC_SPLIT(@UserType,';'))
	If Object_ID('tempdb..#TransTechnical')  is not null  drop table #TransTechnical
	select * into #TransTechnical from TransTechnical
	--print @Bu;
	--select * from #TransCostingHeader
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select a.ID,a.Company,a.MarketingNumber,a.RDNumber,a.PackSize,a.UniqueColumn,
	isnull(b.RequestNo,'-')'RequestNo',a.Remark,a.RequestNo 'folio',
	a.StatusApp,
	RIGHT(CONCAT('00', a.Revised), 2)'Revised',
	isnull(b.Requester,'')'Requester',
	isnull(b.assignee,'')'Assignee',
	a.Packaging,
	a.CanSize,
	(case when b.CustomPrice=1 then 'US Pet Nutrition / ' else '' end)+''+
	isnull(b.Customer,'')+''+isnull(' / '+b.Destination,'') as 'Customer',
	--isnull(b.customer,(select top 1 o.Customer from TransFormulaHeader o where o.RequestNo=a.ID)) as 'Customer',
	b.Destination,
	a.ExchangeRate,
	replace(a.Netweight,'|',',')'NetWeight',
	format(a.CreateOn,'dd-MM-yyyy')'CreateOn', 
	CreateBy,0 as Formula--(select count(*) from TransFormulaHeader t where t.RequestNo=a.ID) as Formula
	into #temp from #TransCostingHeader a left join #TransTechnical b on a.RequestNo=b.ID
	where a.StatusApp in (select case when @type=1 then a.StatusApp
	when Sublevel=7 and @type=0 then
	--(select value from dbo.FNC_SPLIT('5,2,-1',',') where value =a.StatusApp)
	(select value from dbo.FNC_SPLIT('2',',') where value =a.StatusApp)
	when Sublevel=4 and @type=0 then (select value from dbo.FNC_SPLIT('0,8',',') where value =a.StatusApp) 
	when Sublevel=1 or Sublevel=2 and @type=0 then '0'
	else idx end from #find)
	and b.Company in (select distinct value from dbo.FNC_SPLIT(@Bu,';')) 

	--declare @tsub TableType
	--insert into @tsub
	--select [user_name] from ulogin where sublevel=@user_name and IsResign=0
	--DECLARE @cols AS NVARCHAR(MAX);
	--	SET @cols = dbo.FNC_STUFF(@tsub)

	--and a.RequestNo <> 0
	--select * from #temp order by marketingnumber
	--where a.StatusApp in (select case when idx=2 then a.StatusApp
	--when Sublevel=4 then 
	--(select value from dbo.FNC_SPLIT('0,8',',') where value =a.StatusApp) else idx end from #find) 
	if (@type=1)
	begin
		If Object_ID('tempdb..#table')  is not null  drop table #table
		select * into #table from #TransCostingHeader where RequestNo in (select ID from #TransTechnical where Requester=@user_name 
		OR @user_name in (select value from dbo.FNC_SPLIT(isnull(Assignee,''),',') where value =@user_name))

		delete @temp
		insert into @temp
		select * from(select editor from #find)#a --Development
		declare @editor nvarchar(max)= dbo.fnc_stuff(@temp)
		print @editor;
		if(select count(*) from #find where idx = 2)>0
		select a.* from #temp a
		else if (select count(*) from #find where SubLevel in (1,2))>0
		select b.ID,
			a.Company,
			b.CostNo as MarketingNumber,
			a.RequestNo,
			a.Revised,
			a.StatusApp,
			b.RefSamples as RDNumber,
			a.PackSize,
			a.CanSize,
			a.Customer,
			a.Destination,
			a.Requester,
			a.NetWeight,
			a.CreateOn,
			a.UniqueColumn from #temp a inner join TransFormulaHeader b on b.RequestNo=a.ID 
		where (select count(value) from dbo.FNC_SPLIT(concat(requester,',',isnull(assignee,'')),',') where value=@user_name)>0
		else
		select a.* from #temp a where a.ID in
		(select distinct(RequestNo) from MasHistory where Username in (select distinct value from dbo.FNC_SPLIT(@editor,','))
		and tablename='TransCostingHeader' union select #table.ID from #table) 
	end
	else
	--select * from TransCostingHeader
	if(select count(*) from #find where idx = 0)>0
	begin
	delete @temp
		insert into @temp
		select * from(select editor from #find where Sublevel in (3,4) --union all
		--select empid from MasApprovAssign where Sublevel=3
		)#a --Development
	declare @ulevel nvarchar(max)= dbo.fnc_stuff(@temp)
	--print @ulevel;
	select a.* from #temp a where
		--where (Requester in (select distinct value from dbo.FNC_SPLIT(@ulevel,',')) 
		--or a.Assignee in (select distinct value from dbo.FNC_SPLIT(@ulevel,',')))
	0<(case 
	 when (select count(value) from dbo.FNC_SPLIT(@ulevel,',') 
	 where value in (select value from dbo.FNC_SPLIT(a.Requester+'|'+ replace(isnull(a.Assignee,''),',','|'),'|')))>0 then 1 else 0 end)
	end
	else
		if(select count(*) from #find where idx = 3)>0 begin
		--if(len(@cols)>8)
		If Object_ID('tempdb..#comm')  is not null  drop table #comm
		select a.*,(select (case when isnull(b.sublevel,'')='' or isnull(b.sublevel,'')='0' then @user_name else b.sublevel end) 
		from ulogin b where b.user_name=a.Requester and IsResign=0) 'sublevel' into #comm 
		from #temp a 
		select * from #comm where sublevel=@user_name
		end 

		else if (select count(*) from #find where SubLevel in (1,2))>0
			select b.ID,
			a.Company,
			b.CostNo as MarketingNumber,
			a.RequestNo,
			a.Revised,
			a.StatusApp,
			b.RefSamples as RDNumber,
			a.PackSize,
			a.CanSize,
			a.Customer,
			a.Destination,
			a.Requester,
			a.NetWeight,
			a.CreateOn,
			a.UniqueColumn
			from #temp a inner join TransFormulaHeader b on b.RequestNo=a.ID
			where (select count(value) from dbo.FNC_SPLIT(concat(requester,',',isnull(assignee,'')),',') where value=@user_name)>0
		else 
		select a.* from #temp a 
END


go

