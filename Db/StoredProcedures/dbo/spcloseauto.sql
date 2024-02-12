CREATE PROCEDURE spcloseauto
	-- Add the parameters for the stored procedure here
AS
BEGIN
If Object_ID('tempdb..#temp')  is not null  drop table #temp
select a.*,0 as xx,
(select count(*) from TransEditCosting b where b.RequestNo=a.ID)
 as cou into #temp from TransTechnical a where RequestType=1 and StatusApp<>4 

declare @Id Int
declare cur_ CURSOR FOR

select Id from #temp

open cur_

FETCH NEXT FROM cur_ INTO @Id
WHILE @@FETCH_STATUS = 0
BEGIN
	declare @count int
	set @count = (select count(*) from TransEditCosting where RequestNo=@Id and (Result is null or Result ='0'))
	update #temp set xx=@count where ID=@Id
	FETCH NEXT FROM cur_ INTO @Id
END

CLOSE cur_
DEALLOCATE cur_

update TransTechnical set StatusApp=4 where id in (select Id from #temp where xx=0)
--select count(*) from TransEditCosting where RequestNo=1942 and (Result is null or Result ='0')
end
go

