-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spSummaryReport]
	-- Add the parameters for the stored procedure here
	@Keyword nvarchar(max),
	@UserNo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Keyword nvarchar(max)='',@UserNo nvarchar(max)='fo5910155'
	SET NOCOUNT ON;
	If Object_ID('tempdb..#tt')  is not null  drop table #tt
	declare @usertype nvarchar(max)
	set @usertype =(select usertype from ulogin where [user_name]=@UserNo)
 	select * into #tt from TransTechnical  where usertype in (select value from dbo.FNC_SPLIT(@usertype,';'))
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
    -- Insert statements for procedure here
	SELECT ID,RequestNo,isnull(Customer,'-')Customer,
	isnull(Destination,'-')Destination,
	--(case when (select count(Sublevel) from dbo.FindULevel(Requester) where Sublevel in (1,2))>0 then isnull(Assignee,'')+'|'+Requester
	--else Requester +''+ isnull(+'|'+Assignee,'')end)name,
	Requester +''+ replace(isnull(+'|'+Assignee,''),',','|') name,
	Company,Notes,StatusApp,CreateOn,upper(Requester)'Requester',UserType into #temp
	from #tt 

	--select * from #temp
	If Object_ID('tempdb..#t')  is not null  drop table #t
	select Packaging,Formula,a.RequestNo into #t from TransCostingHeader a left join TransFormula b 
	on b.RequestNo=a.ID where a.RequestNo in (select ID from #temp) and a.usertype in (select value from dbo.FNC_SPLIT(@usertype,';')) group by a.Packaging,b.Formula,a.RequestNo	
	If Object_ID('tempdb..#formula')  is not null  drop table #formula
	CREATE TABLE #formula(
	RequestNo INT primary key ,
	formula int
	);
	insert into #formula
	select RequestNo,count(formula) formula from #t group by RequestNo
	If Object_ID('tempdb..#query')  is not null  drop table #query
	select ID,
	RequestNo,
	upper(replace(name,'|',',')) as 'name',
	charindex('|',name) as strindex,
	upper(case when charindex('|',name)=0 then name else SUBSTRING(name,0,charindex('|',name))end) as CD, 
	upper(replace((case when charindex('|',name)=0 then '' else SUBSTRING(name,charindex('|',name)+1,len(name)+1) end),'|',',')) as PMC,
	--Customer+'/'+@Keyword as 'Customer',
	Customer as 'Customer',
	Destination,
    dbo.RemoveAllSpaces(replace((case when len(Company)=3 then (select Name from MasCompany where Code=#temp.Company) 
	else (select Title from MasPlant where Code=#temp.Company and dbo.fnc_checktype(usertype,#temp.usertype)>0) end),'Plant','')) Company,
	Notes,
	(select count(#formula.formula) from #formula where #formula.RequestNo=#temp.ID) as formula,
	StatusApp,CreateOn,Requester into #query
	from #temp 
	--select * from #formula
	declare @sql NVARCHAR(MAX)
	SET @sql = case when @Keyword='X' or @Keyword='' then 'select * from #query where (DATEDIFF(day,CreateOn,GETDATE()))<31' 
	else 'select * from #query where' +' ' + @Keyword end
	print @sql;
	EXEC sp_executesql @sql
END
--select (DATEDIFF(day,'2017-11-01',GETDATE()))
go

