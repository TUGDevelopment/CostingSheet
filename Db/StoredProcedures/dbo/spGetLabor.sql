-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetLabor]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@CostingNo nvarchar(max)
AS
BEGIN
	--declare @Company nvarchar(max)='101',	@CostingNo nvarchar(max)='8078'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	If Object_ID('tempdb..#table')  is not null  drop table #table
	SET NOCOUNT ON;
	select b.LBOh,convert(float,(select top 1 value from dbo.FNC_SPLIT(a.NetWeight,'|'))) as 'NetWeight' into #table from TransTechnical a left join MasPrimary b on
	b.Name=a.Packaging where a.ID=@CostingNo

	--select * from TransProductList where RequestNo=@CostingNo
	select b.ID,b.LBCode,b.LBName,LBRate from TransProductList a inner join MasLaborOverhead b on --substring(LBCode,1,2) = a.Lboh and 
	convert(float,NetWeight) between convert(float,lbmin) and convert(float,lbmax) 
	where  RequestNo=@CostingNo and Company=@Company group by b.ID,b.LBCode,b.LBName,LBRate
	order by LBName
END
--select SUBSTRING('02001',1,2)
--select * from MasLaborOverhead where 161 between convert(float,lbmin) and convert(float,lbmax) 


go

