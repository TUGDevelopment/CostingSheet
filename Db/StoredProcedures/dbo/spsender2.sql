--/****** Script for SelectTopNRows command from SSMS  ******/
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spsender2]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@User nvarchar(max),
	@StatusApp nvarchar(max),
	@table nvarchar(max)
AS
BEGIN
	--declare @Id nvarchar(max)=273,@user nvarchar(max)='MO600321',@statusapp nvarchar(max)=1,@table nvarchar(max)='TransTechnical'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	Create Table #temp (MailID int,MailTo nvarchar(max),MailCc nvarchar(max)) 
	declare @RequestType nvarchar(max),@Receiver nvarchar(max),@Requester nvarchar(max),@assignee nvarchar(max),@Company nvarchar(max)
	declare @mytable tabletype
	DECLARE @cols AS NVARCHAR(MAX);
    -- Insert statements for procedure here
	if(@table='TransTechnical')
	begin
	SELECT @RequestType=isnull(RequestType,0),@Receiver=isnull(Receiver,''),@Company=SUBSTRING(Company,1,3),
	@Requester=Requester,@assignee=isnull(Assignee,'') from TransTechnical where Id=@Id
	if @RequestType=0
	begin		
		if (@statusapp=1 or @StatusApp=7)
		begin
		SET @cols = dbo.fnc_getuser(6,@Company)
		if (@StatusApp=1)
		insert into #temp values (@Id,@Receiver,@cols)
		if(@StatusApp=7)
		insert into #temp values (@Id,@Receiver,case when @assignee='' then @Requester else @assignee+','+@Requester+','+@cols end)
		end
		if @statusapp=2
		begin
			SET @cols = dbo.fnc_getuser(7,@Company)
			insert into #temp values(@Id,@cols,dbo.fnc_getuser(8,@Company))
		end
		if @statusapp=3 or @statusapp=4--accept
		begin
		SET @cols = dbo.fnc_getuser(6,@Company)
		if len(@Requester)>0 and len(@assignee)>0
			SET @cols = @cols +','+ dbo.fnc_getuser(2,@Company)
		insert into #temp values(@Id,@Requester,(case when @assignee='' then @Receiver else @assignee+','+@Receiver end)+','+@cols)
		end
		if @statusapp=5
		insert into #temp values(@Id,@Receiver,case when @assignee='' then @Requester else @Requester+','+@assignee end)
	end
	if @RequestType=2
	begin
		if @statusapp=1 or @statusapp=7
		begin
		SET @cols = dbo.fnc_getuser(7,@Company)
		insert into #temp values(@Id,@cols,dbo.fnc_getuser(8,@Company))
		end
		if @statusapp=3 or @statusapp=4--accept
		insert into #temp values (@Id,@Requester,@assignee)
		end
	end
	else
	begin
	declare @folio nvarchar(max),@status nvarchar(max)
	SELECT @Company=SUBSTRING(Company,1,3),@folio=RequestNo,
		@status=StatusApp from TransCostingHeader where Id=@Id
		if (@statusapp=3)--Reject
		begin
		--If Object_ID('tempdb..#col')  is not null  drop table #col
		insert into @mytable
		select [user_name] from ulogin where IsResign=0 and [user_name] in (select value from dbo.FNC_SPLIT(
		(select CreateBy+';'+ isnull(ModifyBy,'')  from TransCostingHeader b where b.Id=@Id),';'))
		set @cols = dbo.fnc_stuff(@mytable)
		declare @c nvarchar(max)= dbo.fnc_getuser(8,@Company) 
		print @c;
		insert into #temp values(@Id,@cols,@c)
		end
		if(@statusapp=1 and @status=3)--send approve
		begin
			SET @cols = dbo.fnc_getuser(9,@Company)
			insert into #temp values(@Id,@cols,'')
		end
		if(@statusapp=2 and @status=0)--send marketing 
		begin
			declare @creater nvarchar(max)
			--set @r =(select RequestNo from TransCostingHeader where ID=@Id)
			set @creater=(select Requester +';'+ isnull(assignee,'') from TransTechnical where ID=@folio)
			set @cols = (select empid from MasApprovAssign where Sublevel=3 and empid in
			(select value from dbo.FNC_SPLIT(@creater,';')))
			insert into #temp values(@Id,@cols,'')
		end
		if(@statusapp=2 and @Status=8)--mgr marketing apv
		begin
			insert into @mytable
			select empid from MasApprovAssign where Sublevel=4
			SET @cols = dbo.fnc_stuff(@mytable)
			insert into #temp values(@Id,@cols,'')
		end
		if(@Status=4)
		begin
		insert into @mytable
		SELECT distinct ',' + (c.ActiveBy) 
            FROM TransApprove c where  c.tablename=1 and c.RequestNo=@Id and c.ActiveBy !=''
		SET @cols = dbo.fnc_STUFF(@mytable)
		--If Object_ID('tempdb..#table')  is not null  drop table #table
		delete @mytable;insert into @mytable
		select * from(select Receiver from TransTechnical where ID=@folio 
		union select [user_name] from ulogin where [user_name] in(select [EmpId] from MasApprovAssign where Sublevel=6)
		and BU=@Company and IsResign=0)#a

		set @Receiver = dbo.fnc_STUFF(@mytable)
		insert into #temp values(@Id,@cols,@Receiver)
		end
	end
	select * from #temp
END


go

