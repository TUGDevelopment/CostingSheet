-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spinsertFileSystem]
	-- Add the parameters for the stored procedure here
	@Name nvarchar(max),
	@IsFolder bit,
	@Data varbinary(MAX),
	@ParentID int,
	@LastUpdateBy nvarchar(max),
	@LastWriteTime datetime,
	@GCRecord uniqueidentifier,
	@table nvarchar(max) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @GCRecord uniqueidentifier='F43CBB0F-E04B-4C43-8CA5-4AD3E2175EB3',@LastUpdateBy nvarchar(max)='CP4790028',@table nvarchar(max)='XX'
	declare @usertype nvarchar(max)
	declare @StatusApp int,@FolderName nvarchar(max)
	declare @arr nvarchar(max) ='Marketing;RD;Costing'--1,2,3,'','','','',
	if(@table='TransCostingHeader')
		set @FolderName ='Costing center'
	else
	begin
	select @StatusApp=StatusApp,@usertype=usertype from TransTechnical where UniqueColumn=@GCRecord
	set @usertype =(select usertype from ulogin where [user_name]=@LastUpdateBy)
	If Object_ID('tempdb..#level')  is not null  drop table #level
	select * into #level from dbo.FindULevel(@LastUpdateBy) where idx in(0,1,2,5,6,7,9,10,14)
	set @StatusApp = (select top 1 idx from #level)
		set @FolderName = (select case when @StatusApp in (0,3,-1) then 'Marketing' 
			when @StatusApp in (1,2,5) then 'RD' when @StatusApp in (6) then 'PD'
			when @StatusApp in (7) then 'LAB' else 'Costing' end)
	end
	--print @FolderName;
	declare @ID nvarchar(max) = concat('0',(select top 1 convert(nvarchar(max),ID) 
	from FileSystem where GCRecord=@GCRecord and Name=@FolderName and IsFolder=1))
    --print @id;
	if (@ID = 0) begin
	Insert FileSystem(Name,IsFolder, ParentId,LastWriteTime, GCRecord, LastUpdateBy)
	values (@FolderName,1,1,@LastWriteTime, @GCRecord, @LastUpdateBy)
	SET @ParentID = (SELECT CAST(scope_identity() AS int))
	end
	else
	set @ParentID = @ID
	INSERT INTO FileSystem (Name, IsFolder, ParentId, Data, LastWriteTime, GCRecord, LastUpdateBy) 
	VALUES (@Name, @IsFolder, @ParentID, @Data, @LastWriteTime, @GCRecord, @LastUpdateBy);
END
go

