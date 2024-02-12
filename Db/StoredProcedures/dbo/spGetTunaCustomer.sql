/****** Script for SelectTopNRows command from SSMS  ******/
CREATE Procedure [dbo].[spGetTunaCustomer]
	-- Add the parameters for the stored procedure here
	--@Id nvarchar(max),
	@requestno nvarchar(max),
	@value nvarchar(max),
	@username nvarchar(max)
AS
BEGIN
--declare @username nvarchar(max)='FO5910155',	@requestno nvarchar(max)=1454,	@value nvarchar(max)='Customer'
--If Object_ID('tempdb..#temp')  is not null  drop table #temp
--declare @icount int  = (select count(*) from TransQuotationCustom where requestno=@requestno)
	declare @editor nvarchar(max),@statusapp nvarchar(max)
	If Object_ID('tempdb..#table')  is not null  drop table #table
	select a.* into #table from TransTunaStd a where a.ID=@requestno
	set @statusapp=(select statusapp from #table)
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	--select * into #temp from dbo.FindULevel(@username) where idx in(2,3,0,8) 
	select Sublevel into #temp from MasApprovAssign where EmpId=@username
	--select * from #temp
if @statusapp in (0,-1) 
	begin
		--declare @table tabletype
		--	insert into @table select editor from #temp where Sublevel in (3,4) 
		--declare @ulevel nvarchar(max)=dbo.fnc_stuff(@table)
		set @editor=case when (select count(*) from #table a
		where (a.CreateBy in (select distinct value from dbo.FNC_SPLIT(@username,','))
		)) >0 then 0 else  1 end
	end
	else if @statusapp in (2)
		set @editor=case when (select count(*) from #temp 
		where Sublevel=(4))>0 then 0 else  1 end
	else if @statusapp in (8) 
		set @editor=case when (select count(*) from #temp 
		where Sublevel=(15))>0 then 0 else  1 end
	else if @statusapp in (9)
		set @editor=case when (select count(*) from #temp 
		where Sublevel=(4))>0 then 0 else  1 end
print @editor;
if @value='Customer'
	SELECT ID,Material,RequestNo,
	Incoterm,
	[Route],
	isnull([Zone],'') as 'Zone',
	Size,Quantity,PaymentTerm,
	Interest,
	(select count(f.SubID) from TransStdFileDetails f where f.RequestNo=t.ID and SubID=0 and f.Result='Freight') as '_Freight',
	(select count(f.SubID) from TransStdFileDetails f where f.RequestNo=t.ID and SubID=0 and f.Result='ValidityDate') as '_ValidityDate',
	(select count(f.SubID) from TransStdFileDetails f where f.RequestNo=t.ID and SubID=0 and f.Result='ExchangeRate') as '_ExchangeRate',
	Freight,Insurance,Extracost,Currency,
	ExchangeRate,Remark,Customer,ShipTo,Quantity,StatusApp,'' as Mark ,''Upcharge,
	UniqueColumn,BillTo,
	format([From],'dd-MM-yyyy')'From',
	format([To],'dd-MM-yyyy')'To',
	format(ValidityDate,'dd-MM-yyyy')'ValidityDate',
	case when StatusApp in (4) then 1 else @editor end as 'editor',
	case when (select count(*) from #temp where Sublevel in (4,15))>0 and isnull(StatusApp,0) <> 0 then 0 else 1 end 'approv' 
	--1 as 'editor'
	FROM TransTunaStd t where ID=@requestno
end
 --select * from TransStdFileDetails

go

