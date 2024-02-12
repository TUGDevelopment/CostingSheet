-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetDescription]
	-- Add the parameters for the stored procedure here
	@Code nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @code nvarchar(max)='3AA8LN2CBACN5IUUSJ'
	declare @table table(id int,material nvarchar(max),name nvarchar(max),old nvarchar(max))
	insert into @table EXECUTE spGetsapMaterial2
    -- Insert statements for procedure here
	SELECT top 1 name from @table where material=@Code and [name] <>''
END
go

