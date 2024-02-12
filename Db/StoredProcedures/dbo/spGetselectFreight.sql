CREATE PROCEDURE [dbo].[spGetselectFreight]
	-- Add the parameters for the stored procedure here
	@Route nvarchar(max),
	@Container nvarchar(max),
	@Customer nvarchar(max),
	@ShipTo nvarchar(max)
AS
BEGIN
--declare @Route nvarchar(max)='ZAJP11',@Container nvarchar(max)='REFHC40'''
If Object_ID('tempdb..#temp')  is not null  drop table #temp
declare @aedat datetime 
SELECT * into #temp
  FROM [dbo].[zthsdt_appv_log] where zthsdt_appv_log_route=@Route
  and zthsdt_appv_log_zcontain_type=@Container
set @aedat=(select max(zthsdt_appv_log_aedat)'aedat' from #temp)
  --select * from #temp

  select zthsdt_appv_log_zmarket_cost as 'MKTCost' from #temp where  @aedat between ZTHSDT_APPV_LOG_ZFREIGHT_FR and ZTHSDT_APPV_LOG_ZFREIGHT_TO
  and (ZTHSDT_APPV_LOG_KUNNR = @Customer or ZTHSDT_APPV_LOG_KUNNR = @ShipTo)

end
go

