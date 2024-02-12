--/****** Script for SelectTopNRows command from SSMS  ******/
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spsender]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@User nvarchar(max),
	@StatusApp nvarchar(max),
	@table nvarchar(max),
	@AppStatus nvarchar(max)
AS
BEGIN
	--declare @Id nvarchar(max)=7985,@user nvarchar(max)='FO5910155',@statusapp nvarchar(max)=7,@table nvarchar(max)='TransTechnical',@AppStatus nvarchar(max)=7
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	Create Table #temp (MailID int,MailTo nvarchar(max),MailCc nvarchar(max)) 
	declare @RequestType nvarchar(max),@Receiver nvarchar(max),
	@Requester nvarchar(max),
	@assignee nvarchar(max),
	@Company nvarchar(max),
	@Requestfor nvarchar(max),
	@Category nvarchar(max),
	@LabBy nvarchar(max)
	declare @mytable tabletype
	declare @folio nvarchar(max),@status nvarchar(max)
	declare @cols AS nvarchar(MAX),@Cc nvarchar(max),@usertype nvarchar(max)
	select @usertype=usertype from ulogin where [user_name]=@User and IsResign=0
    -- Insert statements for procedure here
	if(@table='TransTechnical')
	begin
	SELECT @RequestType=isnull(RequestType,0),@Receiver=isnull(Receiver,''),
	@Company=Company,@usertype=usertype,
	@Category=PetCategory,
	@Requester=Requester,@assignee=isnull(Assignee,''),
	@Requestfor=Requestfor,
	@LabBy=LabBy from TransTechnical where Id=@Id

	if @RequestType in (0,3)
	begin
		if(@StatusApp in (1,4,5,9,10))	--case tuf using rd tum 
		set @Company = (select case when isnull(Receiver,'')='' then @Company else
		Receiver end from MasPetCategory where ID=@Category)
		if (@statusapp in (1,6,7,9,10))
		begin
		SET @cols = dbo.fnc_getuser(6,@Company,@usertype)
			if (@StatusApp=1)
				if(@AppStatus=5)
				insert into #temp values (@Id,@cols,'')
				else
				insert into #temp values (@Id,@cols,@Receiver+','+@assignee)
			else if (@Statusapp in (6))begin--send pd
					SET @cols = dbo.fnc_getuser(13,@Company,@usertype)
					insert into #temp values(@Id,@cols,dbo.fnc_getuser(8,@Company,@usertype))
				end
			else if(@StatusApp in (7)) begin--send pic rd lab
				--SET @cols = dbo.fnc_getuser(14,@Company,@usertype)
				insert into #temp 
				values (@Id,@LabBy,'')
				end
			else if(@StatusApp=9)
					insert into #temp values(@Id,case when @assignee='' then @Requester else @Requester+','+@assignee end,@cols)
			else if(@StatusApp=10)
					insert into #temp values (@Id,@cols,'')
			end
		else if @statusapp=2 --send by rdm 
			if(@RequestType=3) --request sample product
				insert into #temp 
				values(@Id,case when @assignee='' then @Requester else @Requester+','+@assignee end,@cols)
			else
			begin--request costing
				SET @cols = dbo.fnc_getuser(7,@Company,@usertype)
				insert into #temp values(@Id,@cols,dbo.fnc_getuser(8,@Company,@usertype))
			end
		else if (@statusapp in (3))
			begin
			declare @RejectId nvarchar(max),@Reason nvarchar(max)  
			set @RejectId = (select max(Id) from mashistory where requestno=@Id and username=@User and tablename=@table and statusapp=@statusapp)
			set @Reason = (select Reason from mashistory where ID=@rejectId)
				if (select count(*) from dbo.fnc_split(@reason,'|'))>1 begin
					set @requester =(select value from dbo.fnc_split(@reason,'|') where idx=2) 
					insert into #temp values(@Id,@Requester,@user)
				end 
			end
		else if (@statusapp in (4))--accept by costint team
			begin
			if (@AppStatus in (7)) begin--Receive Lab && Submit
				SET @cols = dbo.fnc_getuser(14,@Company,@usertype)
				insert into #temp values (@Id,@Receiver,@cols)
			end
			else begin
				delete @mytable;
				insert into @mytable
				select Username from MasHistory where RequestNo=@Id and tablename=@table
				SET @cols = dbo.fnc_stuff(@mytable)
				--SET @cols = dbo.fnc_getuser(6,@Company,@usertype)
					if len(@Requester)>0 and len(@assignee)>0
						SET @cols = @cols +','+ dbo.fnc_getuser(2,@Company,@usertype)
				insert into #temp 
				values(@Id,@Requester,(case when @assignee='' then @Receiver else @assignee+','+@Receiver end)+','+@cols)
			end
		end
		else if (@statusapp in (5))--assign marking & project
			insert into #temp 
			values(@Id,@Receiver,case when @assignee='' then @Requester else @Requester+','+@assignee end)
	end
	else if @RequestType=1
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
	else if @RequestType=2
	begin
		if (@statusapp in (1,7))--send to costing team
		begin
			SET @cols = dbo.fnc_getuser(7,@Company,@usertype)
			insert into #temp values(@Id,@cols,dbo.fnc_getuser(8,@Company,@usertype))
		end
		else if (@statusapp in (3,4))--accept
			insert into #temp values (@Id,@Requester,@assignee)
		end
	end
	else if (@table='TransQuotationHeader')
	begin
		if(@StatusApp=8 and @AppStatus=0)--mgr marketing apv
		begin
			insert into @mytable
			select empid from MasApprovAssign where Sublevel=4
			SET @cols = dbo.fnc_stuff(@mytable)
			insert into #temp values(@Id,@cols,'')
		end
		else if(@StatusApp=9 and @AppStatus=8)--mgr costing apv
		begin
			insert into @mytable
			select empid from MasApprovAssign where Sublevel=11
			SET @cols = dbo.fnc_stuff(@mytable)
			insert into #temp values(@Id,@cols,'')
		end
		else if(select isnull(statusapp,0) from TransQuotationHeader where ID=@Id)=4
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
		else if (@statusapp in (3)) begin
			insert into @mytable
			select Username from MasHistory where RequestNo=@Id and tablename='TransQuotationHeader'
			SET @cols = dbo.fnc_stuff(@mytable)
			insert into #temp values(@Id,@cols,'')
		end
	end
	else if @table='TransCostingHeader'
	begin
	SELECT @Company=Company,@folio=RequestNo,@usertype=usertype,
		@status=StatusApp from TransCostingHeader where Id=@Id
		if (@statusapp=3)--Reject
		begin
			--If Object_ID('tempdb..#col')  is not null  drop table #col
			insert into @mytable
			select [user_name] from ulogin where dbo.fnc_checktype(usertype,@usertype)>0 and IsResign=0 and [user_name] in (select value from dbo.FNC_SPLIT(
			(select CreateBy+';'+ isnull(ModifyBy,'')  from TransCostingHeader b where b.Id=@Id),';'))
			set @cols = dbo.fnc_stuff(@mytable)
			declare @c nvarchar(max)= dbo.fnc_getuser(8,@Company,@usertype) 
			insert into #temp values(@Id,@cols,@c)
		end
		else if(@statusapp=1 and @status=3)--send approve
		begin
			declare @Requestno nvarchar(max) 
			set @Requestno=(select requestno from TransCostingHeader b where b.Id=@Id)
			set @Requester=(select concat(Requester,',',isnull(Assignee,'')) from TransTechnical where Id=@Requestno)
			declare @temp TableType
			insert into @temp
			select distinct sublevel from ulogin where dbo.fnc_checktype([user_name],@Requester)>0
			DECLARE @sublevel AS NVARCHAR(MAX);
			if(select count(*) from @temp)>10 
			begin
			SET @sublevel = dbo.FNC_STUFF(@temp)
			insert into @temp
			select value from dbo.FNC_SPLIT((dbo.fnc_getuser(9,@Company,@usertype)),',') 
			where dbo.fnc_checktype(@sublevel,value)>0
			SET @cols = dbo.FNC_STUFF(@temp)
			end 
			else
			SET @cols = dbo.fnc_getuser(9,@Company,@usertype)
			insert into #temp values(@Id,@cols,'')
		end
		else if(@statusapp in (2,4) and @status=0)--send marketing 
		begin
			set @Cc =dbo.fnc_getuser(7,@Company,@usertype)
			set @cols=(select Requester +','+ isnull(assignee,'') +
			 (case when substring(Company,1,3)='103' then ','+ isnull(Receiver,'') else '' end) from TransTechnical where ID=@folio)
			if len(@cols)>0
			begin
				declare @ActiveBy nvarchar(max)= (select top 1 ActiveBy from TransApprove where RequestNo=@folio and levelApp=1 and tablename=0)
				set @cols = (case when right(@cols,1)=',' then @cols  else @cols +','  end) +''+ case when len(@ActiveBy) >0 then @ActiveBy else '' end
			end--print @cols;
			insert into #temp values(@Id,@cols,@Cc)
		end
		else if(@Status=4 and @StatusApp<>8)
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
	--else if @usertype in (1,2,3,4,5,6,7,8)
	--begin
	--	select @cols = Requester +','+ isnull(assignee,'') ,
	--		@Cc = '' from TransTechnical where ID=@folio
	--	insert into #temp values(@Id,@cols,'')
	--	end
	end
	select * from #temp
END

--select * from ulogin where user_name in ('MO531136','MO531136')
--exec spsender 9052,'FO5910155','8','XX','8'

go

