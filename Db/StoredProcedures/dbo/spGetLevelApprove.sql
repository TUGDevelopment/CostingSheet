
CREATE PROCEDURE [dbo].[spGetLevelApprove]
	@Id nvarchar(max),
	@table nvarchar(max),
	@username nvarchar(max),
	@zone nvarchar(max)
AS
BEGIN

--declare @Id nvarchar(max),@table nvarchar(max)='level1',	@username nvarchar(max)='MO630208',	@zone nvarchar(max)='USA'
declare @temp table(empid nvarchar(30),l1 nvarchar(max),l2 nvarchar(max))
insert into @temp
exec spbuildApprove @username
--select * from @temp
If Object_ID('tempdb..#ulogin')  is not null  drop table #ulogin
select * into #ulogin from ulogin
declare @position nvarchar(max)= (select top 1 Sublevel from MasApprovAssign b where b.EmpId=@username)
if(@table='level1') begin
	declare @countemp int =(select count(a.empid)
	from StdAssignZone a where a.empid in (select value from dbo.FNC_SPLIT((select s.LevelApprove from StdApprove s where s.EMPID=@username),';')) and a.zone like '%'+@zone+'%')
	----------------
	if @position = 15
	select distinct user_name as empid,concat(b.firstname,' ',b.lastname)'name' from #ulogin b where b.[user_name] in (
		select l2 from @temp)
	else
		select distinct empid,(select top 1 concat(b.firstname,' ',b.lastname) from #ulogin b where b.[user_name]=#a.empid)'name' from(
		select a.empid
		from StdAssignZone a where a.empid in (select value from dbo.FNC_SPLIT((select s.LevelApprove from StdApprove s where s.EMPID=@username),';')) and a.zone like 
		case when @countemp >0 then '%'+@zone+'%' else a.Zone end 
		union 	select a.empid 
		from StdAssignZone a where a.zone like '%'+@zone+'%' union 
		select case when l1='' then l2 else l1 end from @temp)#a where EMPID<>'' 
		
	end
if(@table='level2') begin
	declare @level nvarchar(max)= (select s.LevelApprove from StdApprove s where s.EMPID=@username)
	select distinct empid,(select top 1 concat(b.firstname,' ',b.lastname) from #ulogin b where b.[user_name]=#a.empid)'name' from(
	select a.empid
	from StdAssignZone a where a.empid in (select value from dbo.FNC_SPLIT((select top 1 s.LevelApprove from StdApprove s where s.EMPID in (select value from dbo.FNC_SPLIT(@level,';'))),';'))
	union select LevelApprove from StdApprove where EMPID=@username and Position='L2'
	)#a
	end
end

--select * from StdApprove

--select * from StdAssignZone
go

