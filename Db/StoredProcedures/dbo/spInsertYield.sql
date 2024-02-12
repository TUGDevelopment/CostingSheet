-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertYield]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@Material nvarchar(max),
	@Name nvarchar(max),
	@RawMaterial nvarchar(max),
	@Description nvarchar(max),
	@Yield nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--select * from MasYield where substring(rawmaterial,1,1)='1'
    -- Insert statements for procedure here
	insert into MasYield values
	(@Company,
	@Material,
	@Name,
	@RawMaterial,
	@Description,
	@Yield)
END
go

