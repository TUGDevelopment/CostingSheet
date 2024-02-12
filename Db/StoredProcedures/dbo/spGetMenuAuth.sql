-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetMenuAuth]
	-- Add the parameters for the stored procedure here
	@username nvarchar(max),
	@viewMode nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @username nvarchar(max)='FO5910155',@viewMode nvarchar(max)='adminform'
	declare @role nvarchar(max),@userlevel nvarchar(max)
	set @userlevel = (select userlevel from ulogin where [user_name]=@username and isnull(isResign,0)=0)
	set @role = (select case when userlevel='2' then '0,1,3,4,5' 
	when userlevel='1' then '0' when userlevel='3' then '1' end 'X' from ulogin where [user_name]=@username and isnull(isResign,0)=0)
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select * into #temp 
	from (select * from MasMenu where IsActive in (select value from dbo.FNC_SPLIT(@role,',')) union--0,1,
	select * from MasMenu where id=1)#a
	select * from #temp where isnull(URL,'')<>'' order by [Text]
END
go

