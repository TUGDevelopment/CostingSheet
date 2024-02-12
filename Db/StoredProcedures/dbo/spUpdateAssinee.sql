-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spUpdateAssinee
	-- Add the parameters for the stored procedure here
	@Assign nvarchar(max),
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    Update TransTechnical set Modified=getdate(),
	assignee=@Assign where Id=@Id
END
go

