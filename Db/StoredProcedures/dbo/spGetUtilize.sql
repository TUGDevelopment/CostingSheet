-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetUtilize]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max)
	--@RequestNo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 

    -- Insert statements for procedure here
	SELECT ID,
	CAST(Result as DECIMAL(9,2)) Result,
	[MonthName],Cost,SubID,RequestNo,''Mark from TransUtilize where RequestNo=@ID
END
go

