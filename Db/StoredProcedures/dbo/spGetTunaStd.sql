-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetTunaStd]
	-- Add the parameters for the stored procedure here
	@user nvarchar(max),
	@type nvarchar(max) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @user nvarchar(max)='fo5910155',	@type nvarchar(max) =0
	SET NOCOUNT ON;
	declare @temp tabletype;
	If Object_ID('tempdb..#find')  is not null  drop table #find
	--select idx,ulevel,editor,Sublevel,SubApp into #find from dbo.FindULevel(@user)
	--insert into @temp
	--select * from #find
	select Sublevel into #find from MasApprovAssign where EmpId=@user
	declare @editor nvarchar(max)=(select abc =STUFF(((SELECT DISTINCT  ';' + f.EMPID
                                         from StdApprove f where dbo.fnc_checktype(LevelApprove,@user)>0  FOR XML PATH(''))), 1, 1, ''))
	--declare @editor nvarchar(max)= dbo.fnc_stuff(@temp)
	if (@type=1)
	begin
	select *,convert(bit,Flag) as FlagColor,
	(select c.Name from MasCustomer c where c.Code=#a.Customer) as 'SoldToName',
	(select c.Name from MasCustomer c where (case when ISNUMERIC(c.Code)=0 then convert(int,c.code) else c.Code end) =#a.ShipTo) as 'ShipToName' from(
	select *,(select FirstName +' '+ LastName from ulogin where [user_name]=CreateBy)'Requester',
	CAST(COALESCE(NULL,0) AS BIT) 'IsPaid',0 as 'FileDetails'
	from TransTunaStd  a where a.ID in
	(select distinct(RequestNo) from MasHistory where Username in (select distinct value from dbo.FNC_SPLIT(@user,','))
		and tablename in ('4','TransTunaStd')) union
	select *,(select FirstName +' '+ LastName from ulogin where [user_name]=CreateBy)'Requester',--Invalid
	CAST(COALESCE(NULL,0) AS BIT) 'IsPaid',0 as 'FileDetails' 
	from TransTunaStd where StatusApp in (1) and createby=@user)#a

	end else begin
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select *,convert(bit,Flag) as FlagColor,(select FirstName +' '+ LastName from ulogin where [user_name]=CreateBy)'Requester',
	isnull((select count(*) from TransStdFileDetails t where t.Requestno = TransTunaStd.ID),0) as 'FileDetails',
	CAST(COALESCE(NULL,0) AS BIT) 'IsPaid' into #temp
	from TransTunaStd where StatusApp in (-1,0,2,8,9)
	if(select count(*) from #find where Sublevel = 4)>0
	select *,convert(bit,Flag) as FlagColor,(select c.Name from MasCustomer c where c.Code=#a.Customer) as 'SoldToName',
	(select c.Name from MasCustomer c where (case when ISNUMERIC(c.Code)=0 then convert(int,c.code) else c.Code end)=#a.ShipTo) as 'ShipToName'
	from (
	SELECT * from #temp
	where StatusApp in (-1,0) and createby=@user union
	SELECT *
			from #temp where StatusApp in (9) and CreateBy in (select EMPID from StdApprove where dbo.fnc_checktype(LevelApprove,@user)>0  union 
			select EMPID from StdApprove where dbo.fnc_checktype(LevelApprove,@editor)>0))#a

	else if(select count(*) from #find where Sublevel = 15)>0
	SELECT *,
	(select c.Name from MasCustomer c where c.Code=#a.Customer) as 'SoldToName',
	(select c.Name from MasCustomer c where (case when ISNUMERIC(c.Code)=0 then convert(int,c.code) else c.Code end)=#a.ShipTo) as 'ShipToName'
	from(SELECT * from #temp
	where StatusApp in (-1,0) and createby=@user union
	SELECT *
	from #temp where StatusApp in (2,8) and CreateBy in (select EMPID from StdApprove where dbo.fnc_checktype(LevelApprove,@user)>0))#a
	else 
	SELECT *,
	(select c.Name from MasCustomer c where c.Code=#a.Customer) as 'SoldToName',
	(select c.Name from MasCustomer c where (case when ISNUMERIC(c.Code)=0 then convert(int,c.code) else c.Code end)=#a.ShipTo) as 'ShipToName'
	from(SELECT * from #temp where StatusApp in (-1,0) and createby=@user)#a
	end
END
go

