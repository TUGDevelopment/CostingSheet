-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spStdSecPKGCost]
	-- Add the parameters for the stored procedure here
	@Customer nvarchar(max),
	@ShipTo nvarchar(max),
	@Material nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Customer nvarchar(max)='10026592',@ShipTo nvarchar(max)='10026592',@Material nvarchar(max)='3AAOCO24BALNN4NNS4'
		If Object_ID('tempdb..#StdSecPKGCost')  is not null  drop table #StdSecPKGCost
		SELECT *,convert(float,Amount)'Price' into #StdSecPKGCost from StdSecPKGCost where Material=@Material
    -- select * from StdSecPKGCost where material like '3AAOCO24BALNN4NN%'
	if(SELECT count(*) from #StdSecPKGCost where Material=@Material and Customer=@Customer and ShipTo=@ShipTo)>0
		SELECT AVG(Price)'Amount' from #StdSecPKGCost where Material=@Material and Customer=@Customer and ShipTo=@ShipTo
	else if(SELECT count(*) from #StdSecPKGCost where Material=@Material and Customer=@Customer)>0
		SELECT AVG(Price)'Amount' from #StdSecPKGCost where Material=@Material and Customer=@Customer
	else if(SELECT count(*) from #StdSecPKGCost where Material=@Material)>0
		SELECT AVG(Price)'Amount' from #StdSecPKGCost where Material=@Material

END
go

