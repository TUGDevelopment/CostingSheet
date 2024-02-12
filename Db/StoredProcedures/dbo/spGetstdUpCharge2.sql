-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetstdUpCharge2]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@SubID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 
	--declare @ID nvarchar(max) =36143,	@SubID nvarchar(max)=828
    -- Insert statements for procedure here
	SELECT * from TransUpCharge where RequestNo=@ID
	and SubID=@SubID
END
go

