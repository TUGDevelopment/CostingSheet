-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spGetOldMaterial] 
	-- Add the parameters for the stored procedure here
	@Material nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Material nvarchar(max) ='xxx'
    -- Insert statements for procedure here
	SELECT code from SMITEMMS where CODE14DIGIT=@Material
END
go

