-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetShape]
	-- Add the parameters for the stored procedure here
	@usertype nvarchar(max),
	@Primary nvarchar(max)
AS
BEGIN
    --declare @Primary nvarchar(max)='Cup',@usertype nvarchar(max)='0'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select * from MasShape where @Primary
    in (select value from dbo.FNC_SPLIT(tcode,','))  and dbo.fnc_checktype(usertype,@usertype)>0
END
go

