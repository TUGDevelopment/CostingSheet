-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create Procedure [dbo].[spUpdateFileSystem]
	-- Add the parameters for the stored procedure here
	@Name nvarchar(max),
	@IsFolder bit,
	@Data varbinary(MAX),
	@ParentID int,
	@LastWriteTime datetime,
	@Id int 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	UPDATE FileSystem SET Name = @Name, IsFolder = @IsFolder, ParentID = @ParentID, Data = @Data, 
	LastWriteTime = @LastWriteTime WHERE (Id = @Id)
END

go

