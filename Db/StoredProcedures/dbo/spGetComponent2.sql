-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
 
create PROCEDURE [dbo].[spGetComponent2]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@action nvarchar(max),
	@user_name nvarchar(max)
AS
BEGIN
	--declare @action nvarchar(max)=3,	@user_name nvarchar(max)='MO600431',@Id nvarchar(max)='557DC3AD-0E33-4595-8834-8D28CE1B6FB1'
	-- SET NOCOUNT ON added to prevent extra result sets from
	declare @sublevel nvarchar(max),@Bu nvarchar(max),@userlist nvarchar(max),
	@statusapp nvarchar(max),@position nvarchar(max),
	@CreateBy nvarchar(max),
	@RequestNo nvarchar(max),
	@usertype nvarchar(max),
	@Company nvarchar(max) =''

	select @statusapp =Statusapp,@CreateBy=Requester,@RequestNo=ID,@Company=Company,@usertype=usertype from TransTechnical where RequestType in (0,2) and UniqueColumn=@Id
	select @Bu=BU,@position=Position from ulogin where [user_name]=@user_name and IsResign=0
	select @sublevel=Sublevel  from MasApprovAssign where EmpId=@user_name 
	print @sublevel;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	-- interfering with SELECT statements.
	if (@Company<>'')
	set @Bu = (select Company from MasPlant where usertype=@usertype and Code=@Company)
	SET NOCOUNT ON;
	declare @table table ([user_name] nvarchar(max),[Description] nvarchar(max))
	SELECT [user_name],firstname +' '+ lastname as fn,userlevel,sublevel,BU into #temp from ulogin where IsResign=0
	and @usertype in (select value from dbo.FNC_SPLIT(usertype,';'))
	if(@action=5)
		if (@statusapp=9) begin
		insert into @table
		select Code,name from masCompany where Code in('102','103')
	end
	else begin
	set @userlist =dbo.fnc_getuser(5,@Bu,@usertype)--RDS(7)
	insert into @table
	SELECT [user_name],fn from #temp where [user_name] in (select value from dbo.FNC_SPLIT(@userlist,',')) --and BU=@Bu
	--union select '9','Stage Gate'
	--union select [user_name],firstname +' '+ lastname as fn from ulogin where [user_name]='FO5910155'
	--insert into @table values(0,'none')
	end
	if(@action=3)begin
		if(@statusapp=2)begin
		insert into @table
		SELECT [user_name],fn from #temp where [user_name] in ( select value from dbo.FNC_SPLIT(@CreateBy,',') union
		select ActiveBy from TransApprove where levelApp=1 and tablename=0 and RequestNo=@RequestNo)
		end
		if(@statusapp=1)begin
		insert into @table
		SELECT [user_name],fn from #temp where [user_name] in (
		select distinct Username from MasHistory where StatusApp in (1,7) and tablename='TransTechnical' and RequestNo=@RequestNo)
		end
		if(@statusapp=5)begin
		insert into @table
		SELECT [user_name],fn from #temp where [user_name] in (
		select distinct Username from MasHistory where StatusApp in (5) and tablename='TransTechnical' and RequestNo=@RequestNo)
		end
	end
	if(@action=6 or @action=7)
	if(@position='USPN') begin
		set @userlist =dbo.fnc_getuser('1,2,3','102;103',@usertype)
		insert into @table
		--select ROW_NUMBER() OVER(ORDER BY value ASC),value from dbo.FNC_SPLIT('CD','')
		SELECT [user_name],fn from #temp where [user_name] in
		(select value from dbo.FNC_SPLIT(@userlist,',')) and [user_name] <>@user_name end
	else	
	begin
	declare @sub nvarchar(max) = case when @sublevel in ('1','2') then '3,4' else '1,2' end
	set @userlist =dbo.fnc_getuser(@sub,@Bu,@usertype)
	insert into @table
	SELECT [user_name],fn from #temp where [user_name] in
		(select value from dbo.FNC_SPLIT(@userlist,',')) --and BU=@Bu
	end
	select [user_name],[Description] from(select * from @table 
	--union SELECT [user_name],firstname +' '+ lastname as fn from ulogin where [user_name]='MO330303'
	)#a
	order by [Description]
END



go

