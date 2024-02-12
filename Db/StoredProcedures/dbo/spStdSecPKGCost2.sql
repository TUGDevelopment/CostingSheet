-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spStdSecPKGCost2]
	-- Add the parameters for the stored procedure here
	@Customer nvarchar(max),
	@ShipTo nvarchar(max),
	@toDate date,
	@Material nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Customer nvarchar(max)='10026592',@ShipTo nvarchar(max)='10026592',@Material nvarchar(max)='3AAOCO24BALNN4NNS4'
		If Object_ID('tempdb..#StdSecPKGCost')  is not null  drop table #StdSecPKGCost
		SELECT *,convert(float,Amount)'Price' into #StdSecPKGCost from StdSecPKGCost where Material=@Material and @toDate between [from] and [to]
    -- select * from StdSecPKGCost where material like '3AAOCO24BALNN4NN%'
	if(SELECT count(*) from #StdSecPKGCost where Material=@Material and Customer=@Customer and ShipTo=@ShipTo and @toDate between [from] and [to])>0
		SELECT AVG(Price)'Amount' from #StdSecPKGCost where Material=@Material and Customer=@Customer and ShipTo=@ShipTo and @toDate between [from] and [to]
	else if(SELECT count(*) from #StdSecPKGCost where Material=@Material and Customer=@Customer and @toDate between [from] and [to])>0
		SELECT AVG(Price)'Amount' from #StdSecPKGCost where Material=@Material and Customer=@Customer and @toDate between [from] and [to]
	else if(SELECT count(*) from #StdSecPKGCost where Material=@Material and @toDate between [from] and [to])>0
		SELECT AVG(Price)'Amount' from #StdSecPKGCost where Material=@Material and @toDate between [from] and [to]

END
go

