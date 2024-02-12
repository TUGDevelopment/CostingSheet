-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spGetFreight]
	-- Add the parameters for the stored procedure here
	@Route nvarchar(max),
	@Container nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Route nvarchar(max)='ZAJP11',@Container nvarchar(max)='REFHC40'''
    -- Insert statements for procedure here
	print @container;
	SELECT * from MasFreight where [Route]=@Route and Container=@Container
END
go

