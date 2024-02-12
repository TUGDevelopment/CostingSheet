-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCompliedWith]
	-- Add the parameters for the stored procedure here
	@usertype nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @usertype nvarchar(max)='2'
 	If Object_ID('tempdb..#temp')  is not null  drop table #temp
		 select ID,Name,usertype from MasCompliedWith where dbo.fnc_checktype(usertype,@usertype)>0
		 --order by Name 

END
go

