-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetFileSystem] 
	-- Add the parameters for the stored procedure here
	@GCRecord nvarchar(max),
	@username nvarchar(max),
	@tablename nvarchar(max)
AS
BEGIN
	--declare @GCRecord nvarchar(max)='baf97f42-7e81-481e-8c54-1f28bcb9ca09',@username nvarchar(max)='FO5910155', @tablename nvarchar(max)='TransTechnical'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements. 
	SET NOCOUNT ON;
	Declare @ID int=1,
	@userlevel nvarchar(1)= (select userlevel from ulogin where [user_name]=@username)
	--declare @Requester nvarchar(max)=(select Requester from TransTechnical where UniqueColumn=@GCRecord)
	--set @GCRecord nvarchar(max)=null
    -- Insert statements for procedure here
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select * into #temp from dbo.FindULevel(@username) 
	declare @table tabletype;delete @table
	insert into @table
	select editor from #temp
	declare @editor nvarchar(max)=dbo.fnc_stuff(@table)
	 --select * from #temp
	If Object_ID('tempdb..#table')  is not null  drop table #table
	select *,(select Sublevel from MasApprovAssign a where a.EmpId=#a.LastUpdateBy) Sublevel into #table from(
	select LastWriteTime,Name,ID,ParentID,IsFolder,Data,OptimisticLockField,GCRecord,SSMA_TimeStamp,LastUpdateBy from FileSystem where GCRecord=@GCRecord 
	--and LastUpdateBy in 
	--(select value from dbo.FNC_SPLIT(
	--(select case when idx=0 then @editor else LastUpdateBy end  from #temp),','))
	union select 
	LastWriteTime,Name,ID,ParentID,IsFolder,null,OptimisticLockField,GCRecord,SSMA_TimeStamp,LastUpdateBy from FileSystem where ID=@ID)#a
	if(select count(*) from #temp where idx=0)>0 and @tablename='TransTechnical'
	delete #table where Name not like '%.pdf' and IsFolder=0
	select * from #table
END
 
--select * from FileSystem where gcrecord='41D68E1D-8E4F-4985-8C01-05FF3C42B6B6'
--select * from TransTechnical where requestno='10102000265'
go

