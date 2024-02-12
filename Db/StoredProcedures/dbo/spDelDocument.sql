-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spDelDocument]
	@Id nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)=18529
	SET NOCOUNT ON;
	--select * from TransEditCosting
	declare @RequestType nvarchar(max),@RequestNo nvarchar(max)
	select top 1 @RequestType=RequestType,@RequestNo=a.RequestNo from TransCostingHeader a inner join TransTechnical b
	on b.ID=a.RequestNo where a.ID=@Id
	if (@RequestType=1)
		begin
			update TransEditCosting set Result=0,SiteId=NULL,CostingSheet = (
			case when Material<>'' then '' else CostingSheet end) where SiteId=@Id
			if(select count(Result) from TransEditCosting 
				where RequestNo=@RequestNo and isnull(result,'0')='0')=0
				update TransTechnical set StatusApp=2 where ID=@RequestNo
		end
    -- Insert statements for procedure here
	delete TransCosting where RequestNo=@Id
	delete TransCostingHeader where ID=@Id
	delete TransFormula where RequestNo=@Id
	delete TransFormulaHeader where RequestNo=@Id
	delete TransApprove where RequestNo=@Id
	delete TranshCosting where RequestNo=@Id
END
go

