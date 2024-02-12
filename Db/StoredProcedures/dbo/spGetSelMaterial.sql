-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetSelMaterial]
	-- Add the parameters for the stored procedure here
	@bu nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @bu nvarchar(max)='1011'
    -- Insert statements for procedure here
	SELECT Material,Description from MasPriceStd where @bu like CONCAT(Company,'%') and IsActive=0
	 and cast(getdate() as date ) between [From] and [to] union
	select Material,Description from MasPricePolicy where IsActive=0
	 and cast(getdate() as date ) between [From] and [to] 
	 END

	 --select * from MasPricePolicy where Material='11M240000006'
go

