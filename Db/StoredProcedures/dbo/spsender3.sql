--/****** Script for SelectTopNRows command from SSMS  ******/
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create Procedure [dbo].[spsender3]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@User nvarchar(max),
	@StatusApp nvarchar(max),
	@table nvarchar(max),
	@AppStatus nvarchar(max)
AS
BEGIN
	--declare @Id nvarchar(max)=6725,@user nvarchar(max)='FP562871',@statusapp nvarchar(max)=1,@table nvarchar(max)='TransTechnical',@AppStatus nvarchar(max)=1
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	Create Table #temp (MailID int,MailTo nvarchar(max),MailCc nvarchar(max)) 
	declare @RequestType nvarchar(max),@Receiver nvarchar(max),@Requester nvarchar(max),@assignee nvarchar(max),@Company nvarchar(max)
	declare @mytable tabletype
	DECLARE @cols AS NVARCHAR(MAX),@Cc nvarchar(max),@usertype nvarchar(max)
	select @usertype=usertype from ulogin where [user_name]=@User and IsResign=0
    -- Insert statements for procedure here
	if(@table='TransTechnical')
	begin
	SELECT @RequestType=isnull(RequestType,0),@Receiver=isnull(Receiver,''),@Company=SUBSTRING(Company,1,3),
	@Requester=Requester,@assignee=isnull(Assignee,'') from TransTechnical where Id=@Id
	if @RequestType=0
	begin		
		if (@statusapp in (1,6,7,10))
		begin
		SET @cols = dbo.fnc_getuser(6,@Company,@usertype)
		if (@StatusApp=1)
			if(@AppStatus=5)
			insert into #temp values (@Id,@cols,'')
			else
			insert into #temp values (@Id,@cols,@Receiver+','+@assignee)
		else if (@StatusApp=10)
			insert into #temp values (@Id,@cols,'')
		if(@StatusApp in (6,7))
		insert into #temp 
		values (@Id,@Receiver,case when @assignee='' then @Requester else @assignee+','+@Requester+','+@cols end)
		end
		if @statusapp=2
		begin
			SET @cols = dbo.fnc_getuser(7,@Company,@usertype)
			insert into #temp values(@Id,@cols,dbo.fnc_getuser(8,@Company,@usertype))
		end
		if (@statusapp in (3,4))--accept
		begin
		SET @cols = dbo.fnc_getuser(6,@Company,@usertype)
		if len(@Requester)>0 and len(@assignee)>0
			SET @cols = @cols +','+ dbo.fnc_getuser(2,@Company,@usertype)
		insert into #temp 
		values(@Id,@Requester,(case when @assignee='' then @Receiver else @assignee+','+@Receiver end)+','+@cols)
		end
		if (@statusapp in (5,9))
		insert into #temp 
		values(@Id,@Receiver,case when @assignee='' then @Requester else @Requester+','+@assignee end)
		--if @StatusApp=10
		--insert into #temp values(
		--@Id,
		--dbo.fnc_getuser(15,@Company),
		--@Requester)
	end
	if @RequestType=1
	begin
		if @statusapp=2
		begin
			SET @cols = dbo.fnc_getuser(7,@Company,@usertype)
			insert into #temp values(@Id,@cols,dbo.fnc_getuser(8,@Company,@usertype)+','+@assignee)
		end
		if @StatusApp in (4,-1) 
		begin
			insert into #temp values(@Id,@Requester+','+@assignee,'')
		end
	end
	if @RequestType=2
	begin
		if (@statusapp in (1,7))
		begin
		SET @cols = dbo.fnc_getuser(7,@Company,@usertype)
		insert into #temp values(@Id,@cols,dbo.fnc_getuser(8,@Company,@usertype))
		end
		if (@statusapp in (3,4))--accept
		insert into #temp values (@Id,@Requester,@assignee)
		end
	end
	else if (@table='TransQuotationHeader')
	begin
		if(@StatusApp=8 and @AppStatus=0)--mgr marketing apv
		begin
		--declare @mytable tabletype,@cols nvarchar(max)
			insert into @mytable
			select empid from MasApprovAssign where Sublevel=4
			SET @cols = dbo.fnc_stuff(@mytable)
			insert into #temp values(@Id,@cols,'')
		end
		if(@StatusApp=9 and @AppStatus=8)--mgr costing apv
		begin
			insert into @mytable
			select empid from MasApprovAssign where Sublevel=11
			SET @cols = dbo.fnc_stuff(@mytable)
			insert into #temp values(@Id,@cols,'')
		end
		if(select isnull(statusapp,0) from TransQuotationHeader where ID=@Id)=4
		begin
		If Object_ID('tempdb..#t')  is not null  drop table #t
		select distinct RequestNo into #t from TransQuotationItems where SubID=@Id
		insert into @mytable
		SELECT distinct ',' + (c.ActiveBy) 
            FROM TransApprove c where  c.tablename='1' and c.RequestNo=@Id and c.ActiveBy !=''
		SET @cols = dbo.fnc_STUFF(@mytable)
		--If Object_ID('tempdb..#table')  is not null  drop table #table
		delete @mytable;insert into @mytable
		select * from(select Receiver from TransTechnical where ID in (select RequestNo from TransCostingHeader where ID in (select * from #t))
		union select [user_name] from ulogin where usertype=@usertype and [user_name] in(select [EmpId] from MasApprovAssign where Sublevel=6) and IsResign=0)#a

		set @Receiver = dbo.fnc_STUFF(@mytable)
		insert into #temp values(@Id,@cols,@Receiver)
		end
	end
	else if @table='TransCostingHeader'
	begin
	declare @folio nvarchar(max),@status nvarchar(max)
	SELECT @Company=SUBSTRING(Company,1,3),@folio=RequestNo,@usertype=usertype,
		@status=StatusApp from TransCostingHeader where Id=@Id
		if (@statusapp=3)--Reject
		begin
		--If Object_ID('tempdb..#col')  is not null  drop table #col
		insert into @mytable
		select [user_name] from ulogin where usertype=@usertype and IsResign=0 and [user_name] in (select value from dbo.FNC_SPLIT(
		(select CreateBy+';'+ isnull(ModifyBy,'')  from TransCostingHeader b where b.Id=@Id),';'))
		set @cols = dbo.fnc_stuff(@mytable)
		declare @c nvarchar(max)= dbo.fnc_getuser(8,@Company,@usertype) 
		print @c;
		insert into #temp values(@Id,@cols,@c)
		end
		if(@statusapp=1 and @status=3)--send approve
		begin
			SET @cols = dbo.fnc_getuser(9,@Company,@usertype)
			insert into #temp values(@Id,@cols,'')
		end
		if(@statusapp in (2,4) and @status=0)--send marketing 
		begin
			set @Cc =dbo.fnc_getuser(7,@Company,@usertype);
			--declare @folio int =4205,@cols nvarchar(max)
			set @cols=(select Requester +','+ isnull(assignee,'') +
			 (case when substring(Company,1,3)='103' then ','+ isnull(Receiver,'') else '' end) from TransTechnical where ID=@folio)
			if len(@cols)>0
			begin
			declare @ActiveBy nvarchar(max)= (select top 1 ActiveBy from TransApprove where RequestNo=@folio and levelApp=1 and tablename=0)
			set @cols = (case when right(@cols,1)=',' then @cols  else @cols +','  end) +''+ case when len(@ActiveBy) >0 then @ActiveBy else '' end
			end
			--print @cols;
			insert into #temp values(@Id,@cols,@Cc)
		end

		if(@Status=4 and @StatusApp<>8)
		begin
		insert into @mytable
		SELECT distinct ',' + (c.ActiveBy) 
            FROM TransApprove c where  c.tablename='1' and c.RequestNo=@Id and c.ActiveBy !=''
		SET @cols = dbo.fnc_STUFF(@mytable)
		--If Object_ID('tempdb..#table')  is not null  drop table #table
		delete @mytable;insert into @mytable
		select * from(select Receiver from TransTechnical where ID=@folio 
		union select [user_name] from ulogin where usertype=@usertype and [user_name] in(select [EmpId] from MasApprovAssign where Sublevel=6)
		and BU like '%'+@Company+'%' and IsResign=0)#a

		set @Receiver = dbo.fnc_STUFF(@mytable)
		insert into #temp values(@Id,@cols,@Receiver)
		end
	end
	else if @table='XX'
	begin
	SELECT @Company=SUBSTRING(Company,1,3),@folio=RequestNo,
		@status=StatusApp from TransCostingHeader where Id=@Id
		select @cols = Requester +','+ isnull(assignee,'') ,
			@Cc = '' from TransTechnical where ID=@folio
		insert into #temp values(@Id,@cols,'')
		end
	select * from #temp
END

--exec spsender 9052,'FO5910155','8','TransCostingHeader','8'

go

