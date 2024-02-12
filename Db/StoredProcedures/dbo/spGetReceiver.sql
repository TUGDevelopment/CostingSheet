-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetReceiver]
	-- Add the parameters for the stored procedure here
	@plant nvarchar(max),
	@Category nvarchar(max)
AS
BEGIN
	--declare @plant nvarchar(max)='1011',@ID nvarchar(max)=0,@Category nvarchar(max)='1003'
	SET NOCOUNT ON;
	declare @statusapp nvarchar(max)=0,@utype nvarchar(max),@BU nvarchar(max)
	select @utype=usertype,@BU=isnull(Receiver,'') from MasPetCategory where ID=@Category
	declare @table table (ID nvarchar(max),Name nvarchar(max));
	if @BU in (102) begin
	insert into @table 
	SELECT [user_name],firstname +' '+ lastname as fn from ulogin where 
	[user_name] in (select empid from MasApprovAssign 
	--where Sublevel in (select value from dbo.FNC_SPLIT((case when @statusapp=0 then '5,6' else Sublevel end),',')))
	where Sublevel in (5,6))
	and IsResign=0 and @BU in (select value from dbo.FNC_SPLIT(BU,';')) and dbo.fnc_checktype(usertype,@utype)>0
	union select '0','None'
	end 
	else begin
	insert into @table 
	SELECT [user_name],firstname +' '+ lastname as fn from ulogin where 
	[user_name] in (select empid from MasApprovAssign 
	--where Sublevel in (select value from dbo.FNC_SPLIT((case when @statusapp=0 then '5,6' else Sublevel end),',')))
	where Sublevel in (5,6))
	and IsResign=0 and @plant in (select code from masplant where company in (select value from dbo.FNC_SPLIT(BU,';')))
	and dbo.fnc_checktype(usertype,@utype)>0
	--Bu in(select BU from ulogin where [user_name]=@username and IsResign=0)
	union select '0','None' 
	end
	select * from @table group by ID,[Name]
		ORDER BY 
	 CASE WHEN [Name]='None' THEN ID END Desc,
	 CASE WHEN len([Name])>1 THEN [Name] END ASC
END
go

