-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spApproveStep2]
	-- Add the parameters for the stored procedure here
	@StatusApp nvarchar(max),
	@Id nvarchar(max),
	@User nvarchar(max),
	@table nvarchar(max),
	@remark nvarchar(max),
	@Reason nvarchar(max)
AS
BEGIN
	--declare @StatusApp nvarchar(1)='5',@Id nvarchar(max)='225',@User nvarchar(max)='fo5910155',@table nvarchar(max)='TransTechnical',@remark nvarchar(max),@Reason nvarchar(max)='9'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	declare @temp table(id int,RequestNo nvarchar(max),Requester nvarchar(max),CreateBy nvarchar(max),Assignee nvarchar(max))
	declare @sender Table(MailID nvarchar(max),MailTo nvarchar(max),MailCc nvarchar(max))
	declare @cols nvarchar(max)  
	--create Table #temp (ID nvarchar(max),Ref nvarchar(max),RequestNo nvarchar(max)) 
	SET NOCOUNT ON;
	declare @fn nvarchar(max),@RequestType nvarchar(max),@Ref nvarchar(max)='',@Bu nvarchar(max)
	,@status nvarchar(max)
	select @fn=Position,@Bu=BU from ulogin where [user_name]=@User and IsResign=0
	declare @Query nvarchar(max)
	---select * from TransApprove
	insert into MasHistory values (@Id,@User,@StatusApp,getdate(),@remark,@Reason,@table)
	if(@table='TransTechnical')	begin
	select @status=statusapp,@RequestType=RequestType from TransTechnical where Id=@Id
	if(@StatusApp=7)
	Update TransTechnical set Modified=getdate(),assignee=@Reason where Id=@Id
	if(@StatusApp=5)
	begin--declare @Reason nvarchar(max)='fo591014'
		if(@status='9' and (select count(*) from MasCompany where Code=substring(@Reason,1,3))>0)
		begin
		--declare @cols nvarchar(max)
			SET @cols = dbo.fnc_getuser(6,@Reason)
			set @cols = (select top 1 value from dbo.FNC_SPLIT(@cols,','))
			print @cols;
			set @Statusapp=1;set @Reason=@cols;
			Update TransTechnical set Assignee=@cols where Id=@Id
		end
		else if(@Reason ='9')
		begin
		--declare @cols nvarchar(max)
		 SET @cols = dbo.fnc_getuser(2,'X')
		 print @cols;
		 set @Statusapp=@Reason;--set @Reason=@cols;
		 Update TransTechnical set Assignee=@cols where Id=@Id
		end
		else
		Update TransTechnical set Receiver=@Reason where Id=@Id
	end
	UPDATE TransTechnical set StatusApp= (case when RequestType=2 and @StatusApp in(1,7) then 2 
	when @StatusApp in (5,7) then 
		(case when @status= 1 then 5 else 1 end)
	when @StatusApp in (3) then 
		(case when @status=9 then 1 when @status=10 then 9 else -1 end)
	else @StatusApp end) 
	Where ID=@Id
	if(@StatusApp <> 5) 
	UPDATE TransApprove Set StatusApp=(case when @StatusApp=3 then -1 else 1 end),
	ActiveBy=@User,
	SubmitDate=GETDATE()
	Where RequestNo=@Id and levelApp=@status and tablename=@RequestType
	---
	insert into @temp 
	select ID,RequestNo,Requester,isnull(Receiver,'')Receiver,isnull(Assignee,'')Assignee
		 from TransTechnical a where ID =@Id
	insert into @sender EXECUTE spsender @Id,@User,@Statusapp,@table,@status
	end
	else--TransCostingHeader
	begin
	set @status = isnull((select statusapp from TransCostingHeader where Id=@Id),'0')

	UPDATE TransCostingHeader set StatusApp= case when @StatusApp=3 then -1
	when @StatusApp= 1 and @Status in (2,-1) then 3
	when @StatusApp= 2 and @Status=3 then 0
	when @StatusApp= 2 and @status=0 then 8
	when @StatusApp= 4 and @status=8 then @StatusApp end Where ID=@Id

	UPDATE TransApprove Set StatusApp=case when @StatusApp=3 then -1 else 1 end,
	ActiveBy=@User,
	SubmitDate=GETDATE()
	Where RequestNo=@Id and levelApp=(case when @Status=-1 then 2 else @status end) 
	and tablename=1

	update TransApprove set StatusApp=0 where RequestNo=@Id 
	and tablename=1
	and levelApp<>(case when @Status=-1 then 2 else @status end)
	insert into @temp 
	select a.ID, a.MarketingNumber,CreateBy,b.RequestNo,0 from TransCostingHeader a left join TransTechnical b on b.ID=a.RequestNo
	where a.ID =@Id

 	insert into @sender EXECUTE spsender @Id,@User,@StatusApp,@table,@status
	end

	select b.*,a.MailTo,a.MailCc  from @sender a left join @temp b on b.ID=a.MailID
END

go

