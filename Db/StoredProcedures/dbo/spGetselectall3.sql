-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create Procedure [dbo].[spGetselectall3]
	-- Add the parameters for the stored procedure here
	@user_name nvarchar(max),
	@type nvarchar(max)
AS
BEGIN
	--declare @user_name nvarchar(max)='FO5910155',@type nvarchar(max)=1
	 declare @StatusApp nvarchar(max),@sublevel nvarchar(max)
	 If Object_ID('tempdb..#temp')  is not null  drop table #temp
	 select * into #temp from dbo.FindULevel(@user_name) where idx in(0,1,2,5,9,10)
	 --select * from #temp
	 declare @table tabletype;delete @table
	 --insert into @table select Bu from #temp
	 declare @Bu nvarchar(max),@usertype nvarchar(max)--=dbo.fnc_stuff(@table)
	 select @Bu=Bu,@usertype=usertype from ulogin where [user_name]=@user_name
	 --print @usertype;
	 If Object_ID('tempdb..#t')  is not null  drop table #t
	 select a.ID,a.RequestNo,case when len(a.Company)=4 then substring(a.Company,1,3) else a.Company end as 'Company',
	 case when len(a.Company)=4 then a.Company else '' end 'Plant' ,MarketingNumber,PetCategory,upper(Requester)'Requester',
	 format(RequestDate,'dd-MM-yyyy')'RequestDate',
	 a.customer,
	 a.Brand,
	 RIGHT(CONCAT('00', a.Revised), 2)'Revised',a.StatusApp
	 ,a.UniqueColumn,a.PackSize,isnull(a.RequestType,0)'RequestType'
	 ,format(a.CreateOn,'dd-MM-yyyy')'CreateOn'
	 ,upper(a.Receiver)'Receiver'
	 ,upper(isnull(a.assignee,''))'Assignee' 
	 ,isnull(UserType,0)'UserType'
	 into #t 
	 from TransTechnical a 
	 --where StatusApp in (select case when idx=0 then a.StatusApp else idx end from #temp) 
	 where StatusApp in (select case when @type=1 then a.StatusApp else idx end from #temp 
	 union select case when (select count(*) from #temp where idx=2)>0 then 6 end)
	 --and a.Company in (select distinct value from dbo.FNC_SPLIT(@Bu,';')) 
	 and (a.Company in 
	 (select Code from MasPlant where Company in
	 (select distinct value from dbo.FNC_SPLIT(@Bu,';')) union select distinct value from dbo.FNC_SPLIT(@Bu,';')))
	 and isnull(usertype,'0') in (select value from dbo.FNC_SPLIT(@usertype,';'))
	--select * from #t 
	 delete @table
	 insert into @table
	 select case when Sublevel=6 then concat(ulevel,',',editor) else editor end from #temp
	 declare @editor nvarchar(max)=dbo.fnc_stuff(@table)
	 --print @editor;
	 if(@type=1)
	 begin
	 	if(select count(*) from #temp where idx = 2)>0
		begin
		delete @table
		insert into @table
		select ulevel from #temp
		declare @ulevel nvarchar(max)=dbo.fnc_stuff(@table)
		print @ulevel;
		select *,case SUBSTRING(RequestNo,4,1) when 1 then RequestNo+'00' else RequestNo+''+Revised end as 'Folio',
		SUBSTRING(RequestNo,4,1)'ReType',ID as 'Info' from #t where #t.StatusApp in (1,2,3,4,5,6) and #t.ID in 
		(select distinct(RequestNo) from MasHistory where Username in (select distinct value from dbo.FNC_SPLIT(@ulevel,','))
		and tablename='TransTechnical')
		end
		else
		select *,case SUBSTRING(RequestNo,4,1) when 1 then RequestNo+'00' else RequestNo+''+Revised end as 'Folio',
		SUBSTRING(RequestNo,4,1)'ReType',ID as 'Info' from #t where #t.ID in
		(select distinct(RequestNo) from MasHistory where Username in (select distinct value from dbo.FNC_SPLIT(@editor,','))
		and tablename='TransTechnical'
		union 
		select Id from #t where (select count(value) from dbo.FNC_SPLIT(#t.Requester+','+isnull(#t.Assignee,''),',')
		where value in (select distinct value from dbo.FNC_SPLIT(@editor,',')))>0)
	 end
	 --else
	 --if(select count(*) from #temp where idx = '2')>0
	 --select *,case SUBSTRING(RequestNo,4,1) when 1 then RequestNo+'00' else RequestNo+''+Revised end as 'Folio',
	 --SUBSTRING(RequestNo,4,1)'ReType',ID as 'Info' from #t
	 else
	 select *,case SUBSTRING(RequestNo,4,1) when 1 then RequestNo+'00' else RequestNo+''+Revised end as 'Folio',
	 SUBSTRING(RequestNo,4,1)'ReType',ID as 'Info' from #t
	 --where (Requester in (select distinct value from dbo.FNC_SPLIT(@ulevel,','))
	 --or Receiver in (select distinct value from dbo.FNC_SPLIT(@ulevel,','))
	 --or Assignee in (select distinct value from dbo.FNC_SPLIT(@ulevel,',')))
	 where 0<(case 
	 when #t.StatusApp in (2,6) then 1
	 when #t.StatusApp in (0,1,5,9,10) then (select count(value) from dbo.FNC_SPLIT(@editor,',') 
	 where value in (select value from dbo.FNC_SPLIT(#t.Requester+','+ isnull(#t.Receiver,'')+','+isnull(#t.Assignee,''),','))) end)
END
---select count(value) from dbo.FNC_SPLIT('2,3,4,5',',') where value in (select value from dbo.FNC_SPLIT('0',','))

go

