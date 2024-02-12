-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetPackagType]
	-- Add the parameters for the stored procedure here
	@Primary nvarchar(max),
	@usertype nvarchar(max)
AS
BEGIN
    --declare @Primary nvarchar(max)='Cup'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select ID,Name from(Select * from MasPackagType where dbo.fnc_checktype(usertype,@usertype)>0 and @Primary 
	in (select value from dbo.FNC_SPLIT(tcode,',')))#a
END
go

