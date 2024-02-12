-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetselectall]
	-- Add the parameters for the stored procedure here
	@Keyword nvarchar(max),
	@user_name nvarchar(max),
	@type nvarchar(max)
AS
BEGIN
 
	--declare @user_name nvarchar(max)='fo5910155',@type nvarchar(max)=0,@Keyword nvarchar(max)='',@RequestType nvarchar(max)='0'
	 declare @t Table(ID nvarchar(max))
	 declare @str nvarchar(max)='select ID from TransTechnical ',@sql NVARCHAR(MAX)
	 SET @sql = @str --+ case when len(@Keyword)>1 then 'where '+@Keyword else '' end
	 --print @sql;
	 insert into @t EXEC sp_executesql @sql
	 declare @StatusApp nvarchar(max),@sublevel nvarchar(max)
	 If Object_ID('tempdb..#tulogin')  is not null  drop table #tulogin
	 select [user_name],firstname +' ' + LastName as fullname into #tulogin from ulogin
	 If Object_ID('tempdb..#temp')  is not null  drop table #temp
	 select * into #temp from dbo.FindULevel(@user_name) where idx in(0,1,2,5,6,7,8,9,10)
	 --select * from #temp
	 If Object_ID('tempdb..#TransTechnical')  is not null  drop table #TransTechnical
	 select * into #TransTechnical from TransTechnical where ID in (select * from @t) 
	 declare @table tabletype;
	 delete @table
	 If Object_ID('tempdb..#Category')  is not null  drop table #Category
	 select * into #Category from MasPetCategory
	 --select * from #Category
	 declare @usertype nvarchar(max),@plant nvarchar(max),@factory nvarchar(max)--=dbo.fnc_stuff(@table)
	 select @usertype=usertype,@factory=Plant from ulogin where [user_name]=@user_name

	 --print @factory;
	 If Object_ID('tempdb..#t')  is not null  drop table #t
	 select a.ID,a.RequestNo,
	 --case when len(a.Company)=4 then substring(a.Company,1,3) else a.Company end as 'Company',case when len(a.Company)=4 then a.Company else '' end 'Plant' 
	 a.Company,MarketingNumber,PetCategory,upper(Requester)'Requester',
	 format(RequestDate,'dd-MM-yyyy')'RequestDate',
	 a.Customer,
	 a.Brand,
	 RIGHT(CONCAT('00', a.Revised), 2)'Revised',a.StatusApp
	 ,a.UniqueColumn,a.PackSize,isnull(a.RequestType,0)'RequestType'
	 ,format(a.CreateOn,'dd-MM-yyyy')'CreateOn'
	 ,upper(a.Receiver)'Receiver'
	 ,upper(isnull(a.assignee,''))'Assignee' 
	 ,isnull(UserType,0)'UserType'
	 into #t 
	 from #TransTechnical a 
	 --where StatusApp in (select case when idx=0 then a.StatusApp else idx end from #temp) --select * from #temp
	 where StatusApp in (select case when @type=1 then a.StatusApp else idx end from #temp 
	 union select case when (select count(*) from #temp where idx=2)>0 then 6 end)
	 --and a.Company in (select distinct value from dbo.FNC_SPLIT(@Bu,';')) 
	 and isnull(usertype,'0') in (select value from dbo.FNC_SPLIT(@usertype,';'))
	--select * from #temp
	 --delete @table
	 insert into @table
	 select case when Sublevel=6 then concat(ulevel,',',editor) else editor end from #temp
	 declare @editor nvarchar(max)=dbo.fnc_stuff(@table)
	 print @editor;
	 if(@type=1)
	 begin
	 	if(select count(*) from #temp where idx = 2)>0
		begin
		delete @table
		insert into @table
		select ulevel from #temp
		declare @ulevel nvarchar(max)=dbo.fnc_stuff(@table)
		print @ulevel;
		select *,case when #t.StatusApp=1 then
		(case when (select count(*) from MasHistory hs where hs.RequestNo=#t.ID and hs.StatusApp=10)>0 then 'Wait for approve formula' else 'Wait for assign RD PIC' end)
		when #t.StatusApp =7 then
		(case when (select count(*) from TransApprove ta where ta.RequestNo=#t.ID and ta.StatusApp=0 and ta.levelApp=7)>0 then 'Wait for Receive Lab' else 'Wait for submit' end)
		else
		(select st.Title from MasStatusApp st where st.Id=#t.StatusApp and st.levelapp in (0,2))end'StatusAppTitle',
		(select fullname from #tulogin where user_name=Receiver)'ReceiverName',
		(select fullname from #tulogin where user_name=Requester)'RequesterName',
		case SUBSTRING(RequestNo,4,1) when 1 then RequestNo+'00' else RequestNo+''+Revised end as 'Folio',
		SUBSTRING(RequestNo,4,1)'ReType',ID as 'Info' from #t where #t.StatusApp in (1,2,3,4,5,6) and #t.ID in 
		(select distinct(RequestNo) from MasHistory where Username in (select distinct value from dbo.FNC_SPLIT(@ulevel,','))
		and tablename='TransTechnical')

		and (Company in (select value from dbo.FNC_SPLIT(@factory,';')))
		end
		else
		select *,case when #t.StatusApp=1 then
		(case when (select count(*) from MasHistory hs where hs.RequestNo=#t.ID and hs.StatusApp=10)>0 then 'Wait for approve formula' else 'Wait for assign RD PIC' end)
		when #t.StatusApp =7 then
		(case when (select count(*) from TransApprove ta where ta.RequestNo=#t.ID and ta.StatusApp=0 and ta.levelApp=7)>0 then 'Wait for Receive Lab' else 'Wait for submit' end)
		else
		(select st.Title from MasStatusApp st where st.Id=#t.StatusApp and st.levelapp in (0,2))end'StatusAppTitle',
		(select fullname from #tulogin where user_name=Receiver)'ReceiverName',
		(select fullname from #tulogin where user_name=Requester)'RequesterName',
		case SUBSTRING(RequestNo,4,1) when 1 then RequestNo+'00' else RequestNo+''+Revised end as 'Folio',
		SUBSTRING(RequestNo,4,1)'ReType',ID as 'Info' from #t where #t.ID in
		(select distinct(RequestNo) from MasHistory where Username in (select distinct value from dbo.FNC_SPLIT(@editor,','))
		and tablename='TransTechnical'
		and (Company in (select value from dbo.FNC_SPLIT(@factory,';')))
		union 
		select Id from #t where (select count(value) from dbo.FNC_SPLIT(#t.Requester+','+isnull(#t.Assignee,''),',')
		where value in (select distinct value from dbo.FNC_SPLIT(@editor,',')))>0)
		and (Company in (select value from dbo.FNC_SPLIT(@factory,';')))
	 end
	 else if(select count(idx) from #temp where idx in (1,5))>0 begin
		declare @cols nvarchar(max)
		--delete @table 
		--insert into @table 
		--select Code from MasPlant where Company in (select value from dbo.FNC_SPLIT(@Bu,';'))
		--and dbo.fnc_checktype(@usertype,usertype)>0
		--set @plant = dbo.fnc_STUFF(@table)

		SET @cols = STUFF((select ',',Id from(SELECT Id,usertype from #Category where Receiver in (select value from dbo.FNC_SPLIT(@factory,';'))union 
			SELECT Id,usertype from #Category where dbo.fnc_checktype(@factory,factory)>0 and isnull(Receiver,'')='')#a 
			where dbo.fnc_checktype(@usertype,usertype)>0
		    FOR XML PATH(''), TYPE
			).value('.', 'NVARCHAR(MAX)') 
			,1,1,'')
		select *,case when #t.StatusApp=1 then
		(case when (select count(*) from MasHistory hs where hs.RequestNo=#t.ID and hs.StatusApp=10)>0 then 'Wait for approve formula' else 'Wait for assign RD PIC' end)
		when #t.StatusApp =7 then
		 (case when (select count(*) from TransApprove ta where ta.RequestNo=#t.ID and ta.StatusApp=0 and ta.levelApp=7)>0 then 'Wait for Receive Lab' else 'Wait for submit' end)
		else
		(select st.Title from MasStatusApp st where st.Id=#t.StatusApp and st.levelapp in (0,2))end'StatusAppTitle',
		(select fullname from #tulogin where user_name=Receiver)'ReceiverName',
		(select fullname from #tulogin where user_name=Requester)'RequesterName',
		case SUBSTRING(RequestNo,4,1) when 1 then RequestNo+'00' else RequestNo+''+Revised end as 'Folio',
		SUBSTRING(RequestNo,4,1)'ReType',ID as 'Info' from #t
		where #t.StatusApp in (1,5) 
		and Receiver in (select value from dbo.FNC_SPLIT(@editor,',')) 
		and PetCategory in (select value from dbo.FNC_SPLIT(@cols,','))
	 end 
	 --else
	 --if(select count(*) from #temp where idx = '2')>0
	 --select *,case SUBSTRING(RequestNo,4,1) when 1 then RequestNo+'00' else RequestNo+''+Revised end as 'Folio',
	 --SUBSTRING(RequestNo,4,1)'ReType',ID as 'Info' from #t
	 else
	 select *,case when #t.StatusApp=1 then
	 (case when (select count(*) from MasHistory hs where hs.RequestNo=#t.ID and hs.StatusApp=10)>0 then 'Wait for approve formula' else 'Wait for assign RD PIC' end)
	 when #t.StatusApp =7 then
	 (case when (select count(*) from TransApprove ta where ta.RequestNo=#t.ID and ta.StatusApp=0 and ta.levelApp=7)>0 then 'Wait for Receive Lab' else 'Wait for submit' end)
	 else
	 (select st.Title from MasStatusApp st where st.Id=#t.StatusApp and st.levelapp in (0,2))end'StatusAppTitle',
	 (select fullname from #tulogin where user_name=Receiver)'ReceiverName',
	 (select fullname from #tulogin where user_name=Requester)'RequesterName',
	 case SUBSTRING(RequestNo,4,1) when 1 then RequestNo+'00' else RequestNo+''+Revised end as 'Folio',
	 SUBSTRING(RequestNo,4,1)'ReType',ID as 'Info' from #t
	 --where (Requester in (select distinct value from dbo.FNC_SPLIT(@ulevel,','))
	 --or Receiver in (select distinct value from dbo.FNC_SPLIT(@ulevel,','))
	 --or Assignee in (select distinct value from dbo.FNC_SPLIT(@ulevel,',')))
	 where 0<(case 
	 when #t.StatusApp in (2,7,6) then 1
	 when #t.StatusApp in (0,1,5,8,9,10) then (select count(value) from dbo.FNC_SPLIT(@editor,',') 
	 where value in (select value from dbo.FNC_SPLIT(#t.Requester+','+ isnull(#t.Receiver,'')+','+isnull(#t.Assignee,''),','))) end)
	 and (Company in (select value from dbo.FNC_SPLIT(@factory,';')))
END
---select count(value) from dbo.FNC_SPLIT('2,3,4,5',',') where value in (select value from dbo.FNC_SPLIT('0',','))
--select * from transtechnical where id='7984'
go

