-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetRequestForm]
	-- Add the parameters for the stored procedure here
	@user_name nvarchar(max),
	@type nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @user_name nvarchar(max)='TO660411',@type nvarchar(max)='1'
    -- Insert statements for procedure here	
	declare @StatusApp nvarchar(max),@sublevel nvarchar(max),@plant nvarchar(max),@usertype nvarchar(max)
	declare @table tabletype;delete @table
	If Object_ID('tempdb..#tulogin')  is not null  drop table #tulogin
	select [user_name],firstname +' ' + LastName as fullname,Plant,usertype into #tulogin from ulogin
	select @plant=u.Plant,@usertype=usertype from #tulogin u where u.user_name=@user_name
	
	If Object_ID('tempdb..#auth')  is not null  drop table #auth
	 select aa.*,
		(select abc =STUFF(((SELECT DISTINCT  ',' + EmpId
		FROM MasApprovAssign a left join ulogin b on b.user_name=a.EmpId
		WHERE a.Sublevel in (select value from dbo.FNC_SPLIT(aa.Sublevel,',')) 
		and dbo.fnc_checktype(b.Plant,@plant)>0 
		and EmpId in (select user_name from ulogin where dbo.fnc_checktype(usertype,@usertype)>0)   FOR XML PATH(''))), 1, 1, '')) ulevel ,
		dbo.fnc_userlist(aa.Sublevel,@user_name,@usertype) editor into #auth from dbo.fnc_approv(@user_name) aa where aa.idx in(0,1,2,5,6,7,8,9,10)

	insert into @table
	select case when Sublevel=6 then concat(ulevel,',',editor) else editor end from #auth
	declare @editor nvarchar(max)=dbo.fnc_stuff(@table)
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
    -- Insert statements for procedure here
	SELECT a.ID,a.RequestNo,a.Requester,a.CreateOn,DocumentNo,Costno,RefSamples,a.StatusApp,a.assignee,t.Receiver,
	--(case StatusApp when '0' then 'Waiting to accept' else 'Process' end ) as'StatusApp' ,
	concat('2', ProductGroup,RawMaterial,StyleofPack,MediaType,NW,ContainerLid,Grade,Zone,'00') as Result,
	(select f.CostNo from TransFormulaHeader f where f.ID=a.Costno) as 'CostName',--(select t.RequestNo from TransTechnical t where t.ID=a.RequestNo) as 'RequestName' 
	t.RequestNo as 'RequestName',t.Company into #temp
	from TransRequestForm a left join TransTechnical t on t.ID=a.RequestNo
	--where Requester= case when (select count(*) from  MasApprovAssign where EmpId=@user_name and Sublevel in (1,2,3,4))>0 then @user_name else Requester end
	--and RequestNo in (select h.RequestNo from MasHistory h where h.Username=@user_name union select t.ID from TransTechnical t 
	--where (select count(value) from dbo.FNC_SPLIT(CONCAT(t.Requester,';',isnull(t.assignee,'')),';') where value=@user_name)>0)
	If Object_ID('tempdb..#find')  is not null  drop table #find
	select idx,ulevel,editor,Sublevel,SubApp into #find from dbo.FindULevel(@user_name)
	if @type = 0
		if(select count(*) from #find where idx in (5))>0 begin
			select *,0 editor from #temp where assignee = @user_name
		end
		else if(select count(*) from #find where idx in (1))>0 begin
		declare @cols nvarchar(max)
		delete @table 
		insert into @table 
		select Code from MasPlant where Code in (select value from dbo.FNC_SPLIT(@plant,';'))
		and dbo.fnc_checktype(@usertype,usertype)>0
		set @plant = dbo.fnc_STUFF_Value(@table,';')--dbo.fnc_STUFF(@table)

		select *,0 editor from #temp where Receiver in (select value from dbo.FNC_SPLIT(@editor,',')) 
		and Company in (select distinct value from dbo.FNC_SPLIT(@plant,';'))

		end else
		select *,0 editor from #temp
		where (select count(SubApp) from #find where (select count(*) from  dbo.FNC_SPLIT( subapp,',') where value =#temp.statusapp )>0)>0
		--select * from TransRequestForm where EmpId='Fo5910155'
	else begin

	select *,1 editor from #temp
	where ID in (select h.RequestNo from MasHistory h where h.Username=@user_name and tablename='TransRequestForm') or
	Requester=@user_name
	end
END


go

