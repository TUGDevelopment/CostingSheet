-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create Procedure [dbo].[spAssignee2]
	@usertype nvarchar(max),
	@user_name nvarchar(max),@Bu nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @usertype nvarchar(max)='1',	@user_name nvarchar(max)='mo540628',@BU nvarchar(max)='1031'
	declare @sublevel nvarchar(max),@userlist nvarchar(max),@position nvarchar(max)
	select @position=Position from ulogin where [user_name]=@user_name and IsResign=0
	declare @table table ([user_name] nvarchar(max),[Description] nvarchar(max))
	declare @Company nvarchar(max)= (select Company from MasPlant where Code=@Bu and usertype in (select value from dbo.FNC_SPLIT(@usertype,';'))) 
	if (@Company='' or @Company = null)
	set @Company = @Bu
	if(@position='USPN' or (select count(value) from dbo.FNC_SPLIT(@usertype,';') where value='1')>0) 
	set @userlist =dbo.fnc_getuser('1,2,3','102;103;1011;1012',@usertype)
	else set @userlist = dbo.fnc_getuser((case when @sublevel in ('1','2') then '3,4' else '1,2' end),@Company,@usertype)


	print @userlist;
	set @userlist = REPLACE(@userlist,@user_name,'')
    SELECT au_id,UPPER([user_name])'user_name',firstname +' '+ lastname as 'fn',BU,usertype,isnull(Position,'')'Position',Email 
	from ulogin where IsResign=0  and @usertype in (select value from dbo.FNC_SPLIT(usertype,';')) 
	and [user_name] 
	in (select value from dbo.FNC_SPLIT(@userlist,',')) --union select '','',CONCAT(@usertype,@user_name,@Bu),'','','',''
	--in (SELECT distinct EmpId FROM MasApprovAssign where Sublevel in (select value from dbo.FNC_SPLIT(@userlist,',')))
	--and @Company in (select value from dbo.FNC_SPLIT(Bu,','))
END

go

