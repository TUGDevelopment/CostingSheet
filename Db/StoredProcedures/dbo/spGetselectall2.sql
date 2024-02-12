-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spGetselectall2]
	-- Add the parameters for the stored procedure here
	@user_name nvarchar(max)--,
	--@editor nvarchar(max)
AS
BEGIN
--declare @user_name nvarchar(max)='Fo5910155'
	 declare @userlevel nvarchar(max),@StatusApp nvarchar(max),@sublevel nvarchar(max)
	 If Object_ID('tempdb..#temp')  is not null  drop table #temp
	 select * into #temp from dbo.FindULevel(@user_name) where idx in(0,1,2)
	 --select * from #temp
	 If Object_ID('tempdb..#t')  is not null  drop table #t
	 select a.ID,a.RequestNo,a.Company,MarketingNumber,PetCategory,Requester,
	 format(RequestDate,'dd-MM-yyyy')'RequestDate',
	 a.customer,
	 a.Brand,
	 RIGHT(CONCAT('00', a.Revised), 2)'Revised',a.StatusApp
	 ,a.UniqueColumn,a.PackSize,isnull(a.RequestType,0)'RequestType'
	 ,format(a.CreateOn,'dd-MM-yyyy')'CreateOn'
	 ,a.Receiver
	 ,isnull(a.assignee,'')'Assignee' 
	 into #t 
	 from TransTechnical a 
	 where StatusApp in (select case when idx=0 then a.StatusApp else idx end from #temp) 
	 and substring(a.Company,1,3) in (select case when idx=0 then substring(a.Company,1,3) else Bu end  from #temp) 
	
	 declare @table tabletype
	 insert into @table
	 select editor from #temp
	 declare @ulevel nvarchar(max)=dbo.fnc_stuff(@table)
	 print @ulevel;
	 if(select count(*) from #temp where idx = '2')>0
	 select *,case SUBSTRING(RequestNo,4,1) when 1 then RequestNo+'00' else RequestNo+''+Revised end as 'Folio',
	 SUBSTRING(RequestNo,4,1)'ReType',ID as 'Info' from #t
	 else
	 select *,case SUBSTRING(RequestNo,4,1) when 1 then RequestNo+'00' else RequestNo+''+Revised end as 'Folio',
	 SUBSTRING(RequestNo,4,1)'ReType',ID as 'Info' from #t
	 where (Requester in (select distinct value from dbo.FNC_SPLIT(@ulevel,','))
	 or Receiver in (select distinct value from dbo.FNC_SPLIT(@ulevel,','))
	 or Assignee in (select distinct value from dbo.FNC_SPLIT(@ulevel,',')))
END

go

