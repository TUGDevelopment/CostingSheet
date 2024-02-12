-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spDelformula]
	@Id nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)=195
	SET NOCOUNT ON;
	--select * from TransCostingHeader where id=1225
	--select * from TransFormulaHeader where RequestNo=1225
	declare @Formula nvarchar(max),@RequestNo nvarchar(max)
	select @RequestNo=RequestNo,@Formula=Formula from TransFormulaHeader where ID=@Id
	--declare @Formula nvarchar(max)=6,@RequestNo nvarchar(max)=1225
	delete TransFormulaHeader where ID=@Id
	delete TransCosting where RequestNo=@RequestNo and Formula=@Formula 
	delete TransFormula where RequestNo=@RequestNo and Formula=@Formula
	--declare @RequestNo nvarchar(max)=1225
	If Object_ID('tempdb..#Costing')  is not null  drop table #Costing
	select * into #Costing from TransCosting where RequestNo=@RequestNo
	If Object_ID('tempdb..#Formula')  is not null  drop table #Formula
	select * into #Formula from TransFormula where RequestNo=@RequestNo

	
	declare @i int=0
	declare @idx nvarchar(max),@for nvarchar(max),@Req nvarchar(max)
	declare cur_Employee CURSOR FOR

	SELECT id,Formula,RequestNo from TransFormulaHeader where RequestNo=@RequestNo

	open cur_Employee

	FETCH NEXT FROM cur_Employee INTO @idx,@for,@Req
	WHILE @@FETCH_STATUS = 0
	BEGIN
		set @i=@i+1
		DECLARE @cols AS NVARCHAR(MAX);
		SET @cols = STUFF((SELECT distinct ',' + (convert(nvarchar(max),c.ID)) 
            FROM #Costing c where c.RequestNo=@Req and c.Formula=@for
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
		update TransFormulaHeader set Formula= @i,CostNo= CONCAT(substring(CostNo,1,8),'', format(convert(int,@i),'00')) where ID=@idx

		update TransCosting set Formula=@i where id in (select value from dbo.FNC_SPLIT(@cols,','))
		SET @cols = STUFF((SELECT distinct ',' + (convert(nvarchar(max),c.ID))  
            FROM #Formula c where c.RequestNo=@Req and c.Formula=@for
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
		update TransFormula set Formula=@i where id in (select value from dbo.FNC_SPLIT(@cols,','))
		FETCH NEXT FROM cur_Employee INTO @idx,@for,@Req
	END

	CLOSE cur_Employee
	DEALLOCATE cur_Employee
END
 
go

