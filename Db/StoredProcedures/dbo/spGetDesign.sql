-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetDesign]
	-- Add the parameters for the stored procedure here
	@Primary nvarchar(max),
	@usertype nvarchar(max)
AS
BEGIN
    --declare @Primary nvarchar(max)='Can',@usertype nvarchar(max)='1,2'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	select ID,Name from(Select * from MasDesign Where dbo.fnc_checktype(usertype,@usertype)>0
	and @Primary in (select value from dbo.FNC_SPLIT(tcode,',')))#a
END

go

