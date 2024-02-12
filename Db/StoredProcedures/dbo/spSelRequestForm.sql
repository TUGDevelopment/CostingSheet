-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spSelRequestForm]
	-- Add the parameters for the stored procedure here
	@username nvarchar(max),
	@Id nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Id nvarchar(max)=11,@username nvarchar(max)='FO5910155'
    -- Insert statements for procedure here
	If Object_ID('tempdb..#find')  is not null  drop table #find
	select idx,ulevel,editor,Sublevel,SubApp into #find from dbo.FindULevel(@username)
	SELECT a.*,
	case when (select count(SubApp) from #find where (select count(*) from  dbo.FNC_SPLIT( subapp,',') where value =a.statusapp )>0)>0 then 0 else 1 end as 'editor',CONVERT(nvarchar(max), UniqueColumn) AS 'NewID',
	(select t.RequestNo from TransTechnical t where t.id=a.requestno) trf_requestno,(select f.CostNo from TransFormulaHeader f where f.id=a.Costno) costingno,
	--(select concat(ProductGroup,';', p.Name) from [DevQCAnalysis].dbo.tblProductGroup p where p.ProductGroup=a.ProductGroup collate Thai_CI_AS and GroupType='F')'ProductGroupName',
	(select  Customer from TransTechnical where id=a.requestno )'Customer',
	concat('3', ProductGroup,RawMaterial,StyleofPack,MediaType,NW,ContainerLid,Grade,Zone,'00') as Code,a.Country,a.PetType,a.Division,a.Nutrition,a.PackingStyle
	from TransRequestForm a
	where ID=@Id  --and Requester=@username
END

--select Assignee from TransRequestForm

go

