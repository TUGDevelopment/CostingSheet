-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetPrimary] 
	-- Add the parameters for the stored procedure here
	@usertype nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @usertype nvarchar(max)='1'
	select ROW_NUMBER() OVER(ORDER BY Name DESC) AS ID,Name,LBOh   from (
	select Name,LBOh from MasPrimary where dbo.fnc_checktype(usertype,@usertype)>0 group by Name,LBOh )#a 
END




go

