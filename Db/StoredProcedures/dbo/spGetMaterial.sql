-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetMaterial]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@RawMaterial nvarchar(max)
AS
BEGIN
	--declare @Company nvarchar(max)='102',@RawMaterial nvarchar(max)='4100330'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	If Object_ID('tempdb..#table')  is not null  drop table #table
	SET NOCOUNT ON;
	select * into #table from(
	Select ID,Company,Material,Name,convert(float, Yield) * 100 as 'Yield',RawMaterial  From MasYield a  
	where a.RawMaterial in (@RawMaterial) and Company=@Company union
	select 0,@Company,'','','','')#a
	select * from #table
END
 


go

