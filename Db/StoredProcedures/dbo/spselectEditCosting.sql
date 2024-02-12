-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spselectEditCosting]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@Series nvarchar(max),
	@username nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)='5011',@Series nvarchar(max)='1'
	SET NOCOUNT ON;
	declare @temp table(ID nvarchar(max))--,Result nvarchar(max),formalu int)
	insert into @temp
	select b.ID--,a.Result,convert(int,substring(a.result,9,2)) 
	from TransEditCosting a left join TransCostingHeader b on b.MarketingNumber=substring(a.result,1,8)
	where a.RequestNo=@Id and Series=@Series and b.StatusApp=0
	group by b.ID
	select * from @temp

	--select CONVERT(nvarchar(max), a.UniqueColumn) AS 'UniqueColumn',convert(nvarchar(max),b.ID)'ID',a.Company,a.MarketingNumber,a.RDNumber,a.PackSize, 
	--   b.RequestNo,convert(nvarchar(max),a.ID)'Folio',a.Remark,a.CanSize,case when a.Completed = 1 then 'true' else 'false' end 'Completed'
	--   ,NetWeight,
	--ExchangeRate,
	--a.StatusApp,
	--b.Packaging
	--,case a.StatusApp when 4 then 1 else 0 end as 'editor' from TransCostingHeader a left join TransTechnical b on b.ID = a.RequestNo where a.ID --= @Id
	--in (select ID from @temp)
END


go

