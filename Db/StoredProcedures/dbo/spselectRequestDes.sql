-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spselectRequestDes]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@username nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @ID nvarchar(max)=840,@username nvarchar(max)='fo5910155'
	SET NOCOUNT ON;
	declare @editor nvarchar(max),@StatusApp nvarchar(max)
	set @editor =(select case when count(*)>0 then 0 else 1 end from ulogin where [user_name]=@username and userlevel=2) 
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select * into #temp from dbo.FindULevel(@username) where idx in(0,1,2,5,9,10)
    -- Insert statements for procedure here
	SELECT ID,
	RequestNo,
	Remark,
	Company,
 
	assignee,
	Destination,
	CONVERT(nvarchar(max), UniqueColumn) AS 'UniqueColumn',
	format(RequestDate,'dd-MM-yyyy')'Validfrom',
	format(RequireDate,'dd-MM-yyyy')'Validto'
	,StatusApp as 'StatusApp',
 
	format(CreateOn,'dd-MM-yyyy')'CreateOn',
	isnull((case when StatusApp in (2) then
		(select case when (select count(*) from #temp where StatusApp = idx) >0 then 0 else  1 end)
	when StatusApp in (4,-1) then 1 
	end),0) as 'editor'
	from TransRequestDesForm where ID=@ID
END
go

