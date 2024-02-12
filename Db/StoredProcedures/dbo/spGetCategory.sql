-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCategory]
	@usertype nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @usertype nvarchar(max)='0',	@plant nvarchar(max)='1021'
	--declare @BU nvarchar(max)=(select top 1 Company from MasPlant where Code=@plant)
      select * from MasPetCategory  where dbo.fnc_checktype(usertype,@usertype)>0
END
go

