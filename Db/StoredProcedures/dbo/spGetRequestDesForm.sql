-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetRequestDesForm]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@user nvarchar(max)
AS
BEGIN
	--declare @ID nvarchar(max)=2339
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--set @ID =141
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
    select *,'' Mark from TransRequestDesItems where requestno=@ID
END
 

go

