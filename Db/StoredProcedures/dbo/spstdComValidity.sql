-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spstdComValidity]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @d as datetime = DATEADD(DAY,1, cast(getdate() as date))
	print @d
    -- Insert statements for procedure here
	update TransTunaStd set StatusApp=1 where ID in (
	SELECT t.ID from TransTunaStd t where cast(
	(DATEADD(DAY,1, cast(t.validityDate as date)))
	 as date)<@d) and StatusApp=0
END


--update TransTunaStd set StatusApp=0 where StatusApp=''
go

