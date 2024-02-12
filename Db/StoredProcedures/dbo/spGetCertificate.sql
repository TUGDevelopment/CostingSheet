-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCertificate]
	-- Add the parameters for the stored procedure here
	@FishGroup nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @FishGroup nvarchar(max)='SKJMPL'
    -- Insert statements for procedure here
	select * from StdCertificate where FishGroup = @FishGroup
END
go

