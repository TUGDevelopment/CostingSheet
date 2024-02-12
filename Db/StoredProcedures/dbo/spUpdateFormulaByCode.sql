CREATE PROCEDURE [dbo].[spUpdateFormulaByCode]
	-- Add the parameters for the stored procedure here
	@user nvarchar(max)
AS
BEGIN
--declare @user nvarchar(max)='fo5910155'
declare @Bu nvarchar(max)
set @Bu = (select BU from ulogin where [user_name]=@user)
If Object_ID('tempdb..#temp')  is not null  drop table #temp
select * into #temp from TransCostingHeader c where c.RequestNo>0 and
Company in (select value from dbo.FNC_SPLIT(@Bu,';'))

SELECT a.ID,Name,Code,RefSamples,Formula,CostNo,b.Customer as '_Cus',b.CanSize
  FROM TransFormulaHeader a left join #temp b on b.ID=a.RequestNo
   where  a.RequestNo in (select c.ID from #temp c where c.RequestNo>0)
and isnull(a.CostNo,'')<>''
end
go

