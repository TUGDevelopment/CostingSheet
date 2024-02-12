-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetLacquer]
	-- Add the parameters for the stored procedure here
	@Primary nvarchar(max),
	@usertype nvarchar(max)
AS
BEGIN
    --declare @Primary nvarchar(max)='Can',@usertype nvarchar(max)='0,1'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    select ID,Name,UserType from(Select * from MasLacquer where dbo.fnc_checktype(usertype,@usertype)>0 and @Primary
    in (select value from dbo.FNC_SPLIT(tcode,',')))#a
END

go

