USE [DevCostingSheet]
GO

/****** Object:  StoredProcedure [dbo].[spApproveStep]    Script Date: 8/24/2023 3:08:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER Procedure [dbo].[spApproveStep]
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
	--declare @StatusApp nvarchar(1)='9',@Id nvarchar(max)='1705',@Assign nvarchar(max)='',@User nvarchar(max)='MO630208',@table nvarchar(max)='TransTunaStd',@remark nvarchar(max),@Reason nvarchar(max)=''
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Value nvarchar(max) 
	--set @Value=(Select  Substring(@Reason, Charindex('|', @Reason)+1, LEN(@Reason))) 
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	declare @temp table(id int,RequestNo nvarchar(max),Requester nvarchar(max),CreateBy nvarchar(max),Assignee nvarchar(max),
	Revised nvarchar(max),StatusApp nvarchar(max),NamingCode nvarchar(max),UserType nvarchar(1),requestdate date)
	declare @sender Table(MailID nvarchar(max),MailTo nvarchar(max),MailCc nvarchar(max))
	declare @cols nvarchar(max),@Query nvarchar(max),@Subject nvarchar(max),@UniqueColumn nvarchar(max);
	declare @RequestNo int ,@Req nvarchar(max),@Requester nvarchar(max)
	declare @step nvarchar(max),@cc nvarchar(max) 
	--create Table #temp (ID nvarchar(max),Ref nvarchar(max),RequestNo nvarchar(max)) 
	SET NOCOUNT ON;
	declare @fn nvarchar(max),@RequestType nvarchar(max),@Ref nvarchar(max)='',@Bu nvarchar(max),@usertype nvarchar(max),@company nvarchar(max)
	,@status nvarchar(max),@idx nvarchar(max),@Requestfor nvarchar(max)
	--set @idx=(select idx from dbo.FindULevel(@User))
	select @fn=Position,@Bu=plant,@usertype=usertype from ulogin where [user_name]=@User and IsResign=0
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
		declare @Sublevel nvarchar(max)
		set @Sublevel=(select Sublevel from MasApprovAssign where EmpId=@Assign)
		if(@RequestType in (0,3))
		begin
			if(@StatusApp=1)--Assignee & Send Approve
				EXEC spUpdateAssinee @Assign,@Id
 --+++++++++++++++
			if(@StatusApp=5)--Assignee for R&D
					Update TransTechnical set Receiver=@Assign,StatusApp=5 where Id=@Id
			else if(@StatusApp=10)--Send Formula 
					Update TransTechnical set StatusApp=1 where Id=@Id
			else if(@StatusApp=9) --Request for Sample/spec product
					Update TransTechnical set StatusApp=4 where Id=@Id
			else if(@StatusApp=7) --Send to Lab RD
					Update TransTechnical set LabBy=@Assign,StatusApp=7 where Id=@Id
			else if(@StatusApp=3) --reject --
				begin
				if(@Sublevel in (1,2,3,4))--reject to requester
					Update TransTechnical set StatusApp=-1 where ID=@Id
				else if(@Sublevel in (5)) --reject to PIC
					Update TransTechnical set StatusApp=5 where ID=@Id
				else if(@Sublevel in (14)) --reject to PIC
					Update TransTechnical set StatusApp=5 where ID=@Id
				else if(@Sublevel in (13))--reject to PD
					Update TransTechnical set StatusApp=6 where ID=@Id
				else if(@Sublevel in (6,10)) --reject to RD
					Update TransTechnical set StatusApp=1 where ID=@Id
				end
			else if(@StatusApp=2 and (select Sublevel from MasApprovAssign where EmpId=@User) in (14)) --send back formula
				Update TransTechnical set StatusApp=5 where ID=@Id
			else if(@StatusApp=4 and (select Sublevel from MasApprovAssign where EmpId=@User) in (14)) --Receive Lab
				Update TransTechnical set StatusApp=7 where ID=@Id
			else if(@StatusApp =3 and @status=6)
				Update TransTechnical set StatusApp=8 where ID=@Id
			else
				UPDATE TransTechnical set StatusApp= @StatusApp Where ID=@Id
		end
		else if (@RequestType=2)--skip rd (rd=none) send to costing
			UPDATE TransTechnical set StatusApp= (case when @StatusApp in(1,7) then 2 
			when @StatusApp in (3) then -1
			else @StatusApp end) Where ID=@Id
 	
	if(@StatusApp in (0,1,2,4,5,6,7)) 
	EXEC spUpdateApprove @StatusApp,@User,@Id,@status,@RequestType

	---send mail---
	insert into @temp 
	select ID,RequestNo,Requester,isnull(Receiver,'')Receiver,isnull(Assignee,'')Assignee,Revised,StatusApp,@name,@usertype,getdate()
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
		select a.ID, a.RequestNo,CreateBy,'','',0,StatusApp,'',0,getdate() from TransQuotationHeader a where a.ID =@Id
 		insert into @sender EXECUTE spsender @Id,@User,@StatusApp,@table,@status
	end
	else if (@table='TransRequestForm')
	 begin
	 select  @status = isnull((statusapp),'0'),@Req=DocumentNo,@Requester=CreateBy from TransRequestForm where Id=@Id
	  if (@StatusApp in (4,5) and @status in (0,1)) begin
	   set @step=5
	   update TransRequestForm set Assignee=@Assign WHERE ID=@ID --insert into @sender values(@Id,@Requester,@User)
	  end 
	  else if (@StatusApp in (4) and @status=5) begin
	   set @step=17--insert into @sender values(@Id,@Requester,concat(@User,';',''))
	  end
	  else if (@StatusApp in (4) and @status=17) begin
	   set @step=18--insert into @sender values(@Id,@Requester,concat(@User,';',''))
	  end
	  else if (@StatusApp in (4) and @status=18) begin
	   set @step=4 --insert into @sender values(@Id,@Requester,concat(@User,';',''))
	  end
	  if (@StatusApp in (3))   
	   set @step ='-1'
  
	  update TransRequestForm set StatusApp= @step where ID=@Id
	  insert into @temp 
	  select a.ID, a.DocumentNo,CreateBy,'','',0,StatusApp,'',0,getdate() from TransRequestForm a where a.ID =@Id
	  insert into @sender EXECUTE spsender @Id,@User,@StatusApp,@table,@status
	 end
	else if (@table='TransTunaStd')
	begin	
	--select * from ulogin where Email='apichart.noidee@thaiunion.com'
	--if ((select count(Sublevel) from MasApprovAssign where EmpId in (@user) and Sublevel=15)>0 and @StatusApp = 8) begin
	--	set @StatusApp =9
	--	update TransTunaStdItems set IsAccept='level2' where IsAccept='level1' and RequestNo=@Id
	--end
		select  @status = isnull((statusapp),'0') from TransTunaStd where Id=@Id
		if  @StatusApp=3 set @step=-1
		else if @StatusApp in (8,9) and @status=0 set @step= @StatusApp--//offer more minprice send mgr mk
		else if @StatusApp in (9,4) and @status=8 set @step= @StatusApp
		else if (@StatusApp in (4,9) and @status=9) or (@StatusApp in (2,1) and @status=0) set @step= 4--//mgr cost app
		else  set @step=@StatusApp 

		UPDATE TransTunaStd set StatusApp= @step Where ID=@Id

		insert into @temp 
		select a.ID, RequestNo,CreateBy,'','',0,StatusApp,'',0,CreateOn from TransTunaStd a where a.ID =@Id
		--set @User=(case when @StatusApp = 8 and @reason <>'' then @reason else @User end)
		if @StatusApp = 8 and @Assign <>'' begin
			insert into @sender values(@Id,@Assign,@User)
		end else
 			insert into @sender EXECUTE spsender @Id,@User,@StatusApp,@table,@status

	end
	else if (@table='TransCostingHeader')
	begin
		select @status=isnull(statusapp,'0'),
		@UniqueColumn=CONVERT(nvarchar(max), UniqueColumn),@RequestNo=RequestNo,@usertype=UserType,@company=Company
		 from TransCostingHeader where Id=@Id
		if (select count(value) from dbo.FNC_SPLIT((dbo.fnc_getuser(9,@Company,@usertype)),',') )=0
			begin
			update TransCostingHeader set 
			StatusApp= case when @StatusApp= 1 then 0--3 send to mkt
			when @StatusApp=3 then -1
			when @StatusApp in (2,4) and @Status=3 then 4
			else 4 end Where ID=@Id

			set @Subject = (select [Subject] from MasStepApproval where Id=
			(case when @StatusApp in (1,4) and @status=2 and @usertype in (1,2,3,4,5,6,7,8) then 8
			else @StatusApp end))
			goto JumpToStep  
		end 
		else
		begin
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
		set @Subject = (select [Subject] from MasStepApproval where Id= (case when @StatusApp in (4) and @status=3 
		and @usertype in (1,2,3,4,5,6,7,8) then 8
			else @StatusApp end))
		JumpToStep:

		insert into @temp 
		select a.ID, a.MarketingNumber,CreateBy,b.RequestNo,isnull(b.Customer,''),b.Revised,a.StatusApp,dbo.fnc_NamingCode(a.UserType,a.Company),@usertype,getdate()
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
GO


