-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spApproveStep3]
	-- Add the parameters for the stored procedure here
	@StatusApp nvarchar(max),
	@Id nvarchar(max),
	@User nvarchar(max),
	@table nvarchar(max),
	@remark nvarchar(max),
	@Reason nvarchar(max),
	@Assign nvarchar(max)
AS
BEGIN
--@status system,@statusapp webbase send parameter
	--declare @StatusApp nvarchar(1)='1',@Id nvarchar(max)='402',@Assign nvarchar(max)='',
	--@User nvarchar(max)='fo5910155',@table nvarchar(max)='TransTechnical',@remark nvarchar(max),@Reason nvarchar(max)=''
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Value nvarchar(max) 
	--set @Value=(Select  Substring(@Reason, Charindex('|', @Reason)+1, LEN(@Reason))) 
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	declare @temp table(id int,RequestNo nvarchar(max),Requester nvarchar(max),CreateBy nvarchar(max),Assignee nvarchar(max),
	Revised nvarchar(max),StatusApp nvarchar(max),NamingCode nvarchar(max),UserType nvarchar(1))
	declare @sender Table(MailID nvarchar(max),MailTo nvarchar(max),MailCc nvarchar(max))
	declare @cols nvarchar(max),@Query nvarchar(max),@Subject nvarchar(max),@UniqueColumn nvarchar(max);
	--create Table #temp (ID nvarchar(max),Ref nvarchar(max),RequestNo nvarchar(max)) 
	SET NOCOUNT ON;
	declare @fn nvarchar(max),@RequestType nvarchar(max),@Ref nvarchar(max)='',@Bu nvarchar(max),@usertype nvarchar(max),@company nvarchar(max)
	,@status nvarchar(max),@idx nvarchar(max),@Requestfor nvarchar(max)
	--set @idx=(select idx from dbo.FindULevel(@User))
	select @fn=Position,@Bu=BU,@usertype=usertype from ulogin where [user_name]=@User and IsResign=0
	---#Technical request
	insert into MasHistory values (@Id,@User,@StatusApp,getdate(),@remark,@Reason+'|'+@Assign,@table)
	if(@table='TransTechnical')	
	begin
		select
		@status=statusapp,@company=Company,@usertype=usertype,
		@RequestType=RequestType,@Requestfor=Requestfor 
		from TransTechnical where Id=@Id
		declare @name nvarchar(max)
		set @name = (select [Subject] from MasUserType where ID=@usertype)
		set @Subject = (select [Subject] from MasStepApproval where Id=@StatusApp)

		if(@RequestType in (0,3))
		begin
			if(@StatusApp=7)--Assignee & Send Approve
				Update TransTechnical set Modified=getdate(),assignee=@Assign  where Id=@Id
 
			if(@StatusApp=5)--Assignee for R&D
				Update TransTechnical set Receiver=@Assign,StatusApp=5 where Id=@Id
			else if(@StatusApp=10)--Send Formula 
					Update TransTechnical set StatusApp=1 where Id=@Id
			else if(@StatusApp=9) --Request for Sample/spec product
					Update TransTechnical set StatusApp=4 where Id=@Id
			else if(@StatusApp=3)
				begin
				declare @Sublevel nvarchar(max)
				set @Sublevel=(select Sublevel from MasApprovAssign where EmpId=@Assign)
				if(@Sublevel in (1,2,3,4))
					Update TransTechnical set StatusApp=-1 where ID=@Id
				else if(@Sublevel in (5))
					begin
						Update TransTechnical set StatusApp=5 where ID=@Id
					end
				else if(@Sublevel in (6))
					begin
						Update TransTechnical set StatusApp=1 where ID=@Id
						Update TransApprove set StatusApp=0,ActiveBy=NULL,SubmitDate=NULL 
							Where RequestNo=@Id and levelApp=1 and tablename=@RequestType
					end
				if(@Sublevel in (5,6))
					begin
						select ID,RequestNo,Requester,isnull(Receiver,'')Receiver,isnull(Assignee,'')Assignee,
						@Assign as MailTo,'' as MailCc,@Subject 'Subject',@status 'StatusApp',Revised,@name 'NamingCode',@usertype 'UserType'
						from TransTechnical a where ID =@Id goto common
					end
				end
			else if(@StatusApp=2 and @RequestType=3)--Request for Sample product
				Update TransTechnical set StatusApp=4 where Id=@Id
			else
				UPDATE TransTechnical set StatusApp= @StatusApp Where ID=@Id
		end
		else if (@RequestType=2)
			UPDATE TransTechnical set StatusApp= (case when @StatusApp in(1,7) then 2 end) Where ID=@Id
 	
	if(@StatusApp in (0,1,2,4,5,6,7)) 
	UPDATE TransApprove Set StatusApp=1,ActiveBy=@User,
	SubmitDate=GETDATE()
	Where RequestNo=@Id and levelApp=@status and tablename=@RequestType

	---send mail---
	insert into @temp 
	select ID,RequestNo,Requester,isnull(Receiver,'')Receiver,isnull(Assignee,'')Assignee,Revised,StatusApp,@name,@usertype
		 from TransTechnical a where ID =@Id
	insert into @sender EXECUTE spsender @Id,@User,@Statusapp,@table,@status
	end
	else if (@table='TransQuotationHeader')	
	begin
	set @status = isnull((select statusapp from TransQuotationHeader where Id=@Id),'0')
	--if(@StatusApp= 1 and @Status in (2,-1))--clear status after reject
	--	Update TransApprove set StatusApp=0 where RequestNo=@Id and tablename='3' 
		UPDATE TransQuotationHeader set StatusApp= case when @StatusApp=3 then -1
				when @StatusApp in (2,1) and @status=0 then 4--//complete
				when @StatusApp = 8 and @status=0 then @StatusApp--//offer more minprice send mgr mk
				when @StatusApp = 9 and @status=8 then @StatusApp--//mgr mk app
				when @StatusApp = 4 and @status=9 then 4--//mgr cost app
				when @StatusApp = 4 and @status=8 then @StatusApp end Where ID=@Id

		--select * from TransCostingHeader order by id desc
		insert into @temp 
		select a.ID, a.RequestNo,CreateBy,'','',0,StatusApp,'',0 from TransQuotationHeader a where a.ID =@Id
 		insert into @sender EXECUTE spsender @Id,@User,@StatusApp,@table,@status
	end
	else if (@table='TransCostingHeader')
	begin
		declare @RequestNo int 
		select @status=isnull(statusapp,'0'),
		@UniqueColumn=CONVERT(nvarchar(max), UniqueColumn),@RequestNo=RequestNo,@usertype=UserType,@company=Company
		 from TransCostingHeader where Id=@Id
		if (@usertype in (1,2,3,4,5,6,7,8))
			begin
			update TransCostingHeader set 
			StatusApp= case when @StatusApp= 1 then 0--3 send to mkt
			when @StatusApp=3 then -1
			when @StatusApp in (2,4) and @Status=3 then 4
			else 4 end Where ID=@Id
			goto JumpToStep  
		end 
		else if(@usertype in (0)) begin
		if(@StatusApp= 1 and @Status in (2,-1)) begin--clear status after reject
			Update TransApprove set StatusApp=0 where RequestNo=@Id and tablename='1' 
		end
		UPDATE TransCostingHeader set StatusApp= case when @StatusApp=3 then -1
		when @StatusApp in (1,0) and @Status in (2,-1) then 3
		when @StatusApp in (2,4) and @Status=3 then 0
		when @StatusApp in (2,4) and @status=0 then 4--//
		when @StatusApp in (8,0) and @status=5 then 4--//Costing revised
		when @StatusApp in (4,0) and @status=8 then @StatusApp end Where ID=@Id

		UPDATE TransApprove Set StatusApp=case when @StatusApp=3 then -1 else 1 end,
		ActiveBy=@User,
		SubmitDate=GETDATE()
		Where RequestNo=@Id and levelApp=(case when @Status=-1 then 2 else @status end) 
		and tablename='1'
		end
		JumpToStep:
		set @Subject = (select [Subject] from MasStepApproval where Id=
		(case when @StatusApp in (1,2,4) and @status=0 and @usertype in (1,2,3,4,5,6,7,8) then 8
		else @StatusApp end))

		insert into @temp 
		select a.ID, a.MarketingNumber,CreateBy,b.RequestNo,isnull(b.Customer,''),b.Revised,a.StatusApp,dbo.fnc_NamingCode(a.UserType,a.Company),@usertype
		from TransCostingHeader a 
		left join TransTechnical b on b.ID=a.RequestNo
		where a.ID =@Id

		insert into @sender EXECUTE spsender @Id,@User,@StatusApp,@table,@status
	end
		
	select b.*,(case when len(a.MailTo)>0 then a.MailTo else @User end )'MailTo'
	,a.MailCc,@Subject 'Subject',StatusApp,Revised,
	@UniqueColumn 'UniqueColumn' from @sender a left join @temp b on b.ID=a.MailID
common:
END

--select * from MasStepApproval  where requestno=403
go

