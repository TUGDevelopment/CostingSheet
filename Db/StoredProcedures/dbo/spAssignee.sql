-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spAssignee]
	@usertype nvarchar(max),@Bu nvarchar(max),@ID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @ID nvarchar(max)='C459DD4D-B930-4925-8343-EEB8B3584137',	@usertype nvarchar(max)='1',@BU nvarchar(max)='101',@t nvarchar(1)='0'
	declare @user_name nvarchar(max)
	declare @userlist nvarchar(max)
	set @userlist =dbo.fnc_getuser('1,2,3,4',@Bu,@usertype)
	declare @able table(Empid nvarchar(max))
	insert into @able
	select value from dbo.FNC_SPLIT(@userlist,';')
	declare @assign nvarchar(max)
	select top 1  @assign =concat('', isnull(Assignee,'')),@user_name=Requester from TransTechnical where UniqueColumn =@ID 
	set @user_name = concat('', @user_name);
	SELECT au_id,UPPER([user_name])'user_name',firstname +' '+ lastname as fn,usertype,Position,Email,bu
	from ulogin where IsResign=0 and dbo.fnc_checktype(usertype,@usertype)>0 and substring(@BU,1,3) in (select value from dbo.FNC_SPLIT(bu,';')) and [user_name]<>@user_name and
	[user_name] not in (select value from dbo.FNC_SPLIT(@assign,','))
END
go

