-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetrole]
	-- Add the parameters for the stored procedure here
	@user nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @user nvarchar(max)='Fo5910155'
    -- Insert statements for procedure here
	select top 1 convert(nvarchar(max),idx)idx from dbo.FindULevel(@user) where idx in(0,1,2,5,6,9,10)
END
go

