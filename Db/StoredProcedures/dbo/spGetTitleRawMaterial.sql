
create PROCEDURE [dbo].[spGetTitleRawMaterial]
	-- Add the parameters for the stored procedure here
	@Code nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @table table(id int,material nvarchar(max),name nvarchar(max),old nvarchar(max))
	--insert into @table EXECUTE spGetsapMaterial2
	--declare @code nvarchar(max)='14L110000031'
    -- Insert statements for procedure here
	select top 1 [Description] as name from (
	SELECT [Description] from MasPricePolicy where material=@Code union select [Description] from MasPriceStd where material=@Code) #a
END
go

