-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create Procedure [dbo].[spGetReceiver2]
	-- Add the parameters for the stored procedure here
	@user_name nvarchar(max),
	@plant nvarchar(max),
	@ID nvarchar(max),
	@usertype nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @plant nvarchar(max)='10101',@ID nvarchar(max)=214,@usertype nvarchar(max)=2
	SET NOCOUNT ON;
	declare @statusapp nvarchar(max)=0
	if (@ID<>0)
	set @statusapp=(select Statusapp from TransTechnical where ID=@ID)
	--print @statusapp;
	declare @table table ([user_name] nvarchar(max),fn nvarchar(max));
	--set @plant = SUBSTRING(@plant,1,3)
    -- Insert statements for procedure here
	insert into @table 
	SELECT [user_name],firstname +' '+ lastname as fn from ulogin where 
	[user_name] in (select empid from MasApprovAssign 
	where Sublevel in (select value from dbo.FNC_SPLIT((case when @statusapp=0 then '5,6' else Sublevel end),',')))
	and IsResign=0 and @plant in (select code from masplant where company in (select value from dbo.FNC_SPLIT(BU,';')))
	and @usertype in (select value from dbo.FNC_SPLIT(usertype,';'))
	--Bu in(select BU from ulogin where [user_name]=@username and IsResign=0)
	union select '0','None' 
	--union SELECT [user_name],firstname +' '+ lastname as fn from ulogin where [user_name]='FO5910155'
	--if (select count(*) from ulogin where [user_name]=@username and userlevel in (0,2,3)) > 0
	--insert into @table select @ID,'None'
	select * from @table group by [user_name],fn 
		ORDER BY 
	 CASE WHEN fn='None' THEN user_name END Desc,
	 CASE WHEN len(fn)>1 THEN fn END ASC
END

--select * from TransTechnical order by id desc


go

