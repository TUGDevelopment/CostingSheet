-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spUpdateQuotationfromJob]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@subID nvarchar(max),
	@MinPrice nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @ID nvarchar(max)='56154',	@subID nvarchar(max)='',	@MinPrice nvarchar(max)='11.02'
	    -- Insert statements for procedure here
	if(@subID='0')
		update TransFormulaHeader set IsActive=4,MinPrice=@MinPrice where ID=@ID
	else
		update TransQuotationItems set IsActive=4,ActionDate=GETDATE() where ID=@ID
	--select * from TransQuotationItems 
END
go

