-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spInsertComponent
	-- Add the parameters for the stored procedure here
	@Component nvarchar(max),
	@Result nvarchar(max),
	@Price nvarchar(max),
	@Unit nvarchar(max),
	@SubID nvarchar(max),
	@RequestNo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	insert into transstdcomponent values(
	@Component,@Result,@Price,@Unit,@SubID,@RequestNo)
END
go

