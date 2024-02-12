-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spGetCostQuota2]
	-- Add the parameters for the stored procedure here
	@user nvarchar(max),
	@Id int,
	@Selected nvarchar(max)
AS
BEGIN
 --declare @user nvarchar(max)='fo5910155',@id int =0,@Selected nvarchar(max)=1
 if(@Selected=0) begin
	 declare @CurDate datetime = getdate()-10
		select RequestNo,ID,Customer,Brand,format(RequireDate,'dd-MM-yyyy')RequireDate
		from TransTechnical where dbo.fnc_checktype(concat(Requester,',',isnull(Assignee,'')),@user)>0
		and StatusApp=4 and DATEDIFF(DAY,getdate(),RequireDate)>-100
	 end else if(@Selected=1) begin
	 --declare @user_name nvarchar(max)='fo5910155',@id int =0
		declare @StatusApp nvarchar(max)
		set @StatusApp = concat('',(select StatusApp from TransQuotationHeader where id=@id))
		If Object_ID('tempdb..#find')  is not null  drop table #find
		select idx,ulevel,editor,Sublevel,SubApp into #find from dbo.FindULevel(@user)

		declare @temp tabletype;
		insert into @temp
		select * from(select editor from #find)#a --Development
		declare @editor nvarchar(max)= dbo.fnc_stuff(@temp)
		If Object_ID('tempdb..#TransCostingHeader')  is not null  drop table #TransCostingHeader
		 select * into #TransCostingHeader from TransCostingHeader where StatusApp=4 and [To] >= cast(getdate() as date)  
		If Object_ID('tempdb..#MasHistory')  is not null  drop table #MasHistory
		 select * into #MasHistory from MasHistory
		If Object_ID('tempdb..#TransTechnical')  is not null  drop table #TransTechnical
		select * into #TransTechnical from TransTechnical where StatusApp=4 and ID in (select RequestNo from #TransCostingHeader) 
		--where dbo.fnc_checktype(concat(requester,',',isnull(assignee,'')),@editor)>0 and StatusApp=4
	 if (@StatusApp='')begin
		 If Object_ID('tempdb..#table')  is not null  drop table #table
		 select distinct(ID) into #table from #TransCostingHeader 
		 where RequestNo in (select ID from #TransTechnical where @editor in (select value from dbo.FNC_SPLIT(concat(requester,',',isnull(assignee,'')),',')))
		If Object_ID('tempdb..#temp')  is not null  drop table #temp
		 select * into #temp from (select distinct(RequestNo) from #MasHistory where Username in (select distinct value from dbo.FNC_SPLIT(@editor,','))
			and tablename in ('3','TransCostingHeader') union select distinct(ID) from #table)#a  
			select ID,MarketingNumber as 'RequestNo' from #TransCostingHeader where ID in (select RequestNo from #temp) 
		end
		else 
		select ID,MarketingNumber as 'RequestNo' from #TransCostingHeader where ID in (
		select a.RequestNo from TransQuotation a where a.SubID=@Id)
	end
END
go

