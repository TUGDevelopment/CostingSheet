-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sptemp]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @id int

	DECLARE cur CURSOR FOR select RequestNo from TransFormulaHeader where RequestNo = '10146'

	OPEN cur

	FETCH NEXT FROM cur INTO @id 

	WHILE @@FETCH_STATUS = 0 BEGIN
		EXEC spDelDocument @id  -- call your sp here
		FETCH NEXT FROM cur INTO @id 
	END

	CLOSE cur    
	DEALLOCATE cur
END
go

