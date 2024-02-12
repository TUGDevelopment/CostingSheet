-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spmaterialreport]
	-- Add the parameters for the stored procedure here
	--@user_name nvarchar(max)
AS
BEGIN
--declare @user_name nvarchar(max)='fo5910155'
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	select top 100 a.ID,b.RequestNo,b.MarketingNumber,a.Material,a.RDNumber,a.Costing 
	into #temp from TransMapCosting a left join TransCostingHeader b
	on b.MarketingNumber = substring(a.Costing,1,8)

	select top 100 --row_number() OVER (ORDER BY (SELECT 1))ID,
	a.ID,
	a.MarketingNumber,b.RequestNo,a.RDNumber,b.Company,a.Material,a.Costing
	from #temp a  left join TransTechnical b on b.ID=a.RequestNo
END


go

