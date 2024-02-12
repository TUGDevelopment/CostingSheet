-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCertificate_fee]
	-- Add the parameters for the stored procedure here
	@Code nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Code nvarchar(max)='3AAOCO2EXAP9SINNRN'
    -- Insert statements for procedure here
	declare @FishGroup nvarchar(max)
	set @FishGroup = (select top 1 FishGroup from stdTunaFixFW where substring(Material,1,16)= substring(@Code,1,16))
	select * from StdCertificate where FishGroup=@FishGroup
END
go

