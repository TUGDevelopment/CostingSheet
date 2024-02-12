-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCostingSheet2]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max)
AS
BEGIN
--declare @Id nvarchar(max)=122
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @RequestNo nvarchar(max)
    -- Insert statements for procedure here
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	SELECT @RequestNo=RDNumber from TransTechnical where ID=@Id

	select Id,MarketingNumber,PackSize,null ParentID into #temp from TransCostingHeader where Requestno=@RequestNo
	--group by MarketingNumber,PackSize order by MarketingNumber
	declare @table table (ID int,ParentID int,PackSize nvarchar(max),Material nvarchar(max),MarketingNumber nvarchar(max))
	insert into @table
	select ID,ParentID,PackSize,'' as Material,MarketingNumber from #temp union
	select --row_number() OVER (ORDER BY MarketingNumber) n,
	row_number() OVER (ORDER BY MarketingNumber) n,
	Id,PackSize,'' 
	,MarketingNumber+''+convert(nvarchar(max),FORMAT(formula,'00')) as 'MarketingNumber'
	--format(MarketingNumber+'/'+convert(nvarchar(max),formula),'0000000000')'costno' 
	from #temp a left join (
	select cast(formula as int)'formula',
	RequestNo from TransFormula where RequestNo in (select Id from #temp)
	group by formula,RequestNo)#a on #a.RequestNo=a.ID
	
	select * from @table order by MarketingNumber
END


--select * from TransTechnical

go

