-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spGetCompany] 
	-- Add the parameters for the stored procedure here
	@Bu nvarchar(max), 
	@ID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	select * from MasCompany where Code=(case when len(@ID)=3 then @ID else Code end) and Id <> 7 and Code in 
	(select distinct value from dbo.FNC_SPLIT(@Bu,'|')) order by Code
END
go

