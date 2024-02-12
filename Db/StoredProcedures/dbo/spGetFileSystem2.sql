-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetFileSystem2] 
	-- Add the parameters for the stored procedure here
	@GCRecord nvarchar(max),
	@username nvarchar(max)
AS
BEGIN
	--declare @GCRecord nvarchar(max)='BA5785F2-4112-418E-8CF1-2AE918FC5A41',@username nvarchar(max)='fo5910155'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @ID int=1,
	@userlevel nvarchar(1)= (select userlevel from ulogin where [user_name]=@username)
	declare @Requester nvarchar(max)=(select Requester from TransTechnical where UniqueColumn=@GCRecord)
	--set @GCRecord nvarchar(max)=null
    -- Insert statements for procedure here
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select * into #temp from dbo.FindULevel(@username) 
	declare @table tabletype;delete @table
	insert into @table
	select editor from #temp
	declare @editor nvarchar(max)=dbo.fnc_stuff(@table)
	 --select * from #temp
	select Name,ID,ParentID,IsFolder,GCRecord from FileSystem where GCRecord=@GCRecord 
	--and LastUpdateBy in 
	--(select value from dbo.FNC_SPLIT(
	--(select case when idx=0 then @editor else LastUpdateBy end  from #temp),','))
	--union select * from FileSystem where ID=@ID
END
--select * from [dbo].[TransTechnical] where [UniqueColumn]='BA5785F2-4112-418E-8CF1-2AE918FC5A41'
--update FileSystem set IsFolder=0 where id in (41)

go

