-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spGetMaterialYield]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@RawMaterial nvarchar(max),
	@Material nvarchar(max)

AS
BEGIN
	--declare @Company nvarchar(max)='102',@RawMaterial nvarchar(max)='4100330'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	If Object_ID('tempdb..#table')  is not null  drop table #table
	SET NOCOUNT ON;
	Select ID,Company,Material,Name,convert(float, Yield) * 100 as 'Yield',RawMaterial  From MasYield a  
	where a.RawMaterial in (@RawMaterial) and Company=@Company and Material=@Material
END
 


go

