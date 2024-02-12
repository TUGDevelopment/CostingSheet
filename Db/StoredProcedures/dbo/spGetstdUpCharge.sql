-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spGetstdUpCharge
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 

    -- Insert statements for procedure here
	SELECT * from TransUpCharge where RequestNo=@ID
END
go

