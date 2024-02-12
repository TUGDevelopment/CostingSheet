-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetuserlevel]
	-- Add the parameters for the stored procedure here
	 @user nvarchar(max)  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @user nvarchar(max)  ='FO5910155'
    -- Insert statements for procedure here
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	declare @mytable tabletype
	insert into @mytable
	select isnull(idx,'') from dbo.FindULevel(@user)
	select dbo.fnc_stuff(@mytable) as idx;
END
go

