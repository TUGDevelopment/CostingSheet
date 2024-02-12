-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spinsertstdApprove]
	-- Add the parameters for the stored procedure here
	@requestno nvarchar(max),
	@fn nvarchar(50),
	@Activeby nvarchar(max),
	@levelApp nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	insert into transdApprove values(@requestno,0,1,@fn,@Activeby,getdate(),@levelApp,4)
    -- Insert statements for procedure here
	--SELECT * from transapprove
END
go

