-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spgettestcallstatus 
	-- Add the parameters for the stored procedure here
 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
    -- Insert statements for procedure here
	select * into #temp from TransTechnical  
	select * from #temp where StatusApp=1

	--select * from #temp where ID in (select RequestNo from TransCostingHeader ) and #temp.statusapp <> 4
	delete #temp where ID in (select RequestNo from TransCostingHeader )
	declare @table table (ID int,statusapp nvarchar(max))
	declare @ID nvarchar(max)
	declare cur_temp CURSOR FOR
	select ID from #temp --where StatusApp=1
	open cur_temp
	FETCH NEXT FROM cur_temp INTO @ID
	WHILE @@FETCH_STATUS = 0
	BEGIN
	declare @Max int
		set @Max = (select max(ID) from TransApprove where RequestNo=@ID and SubmitDate is not null and tablename=0)  
		if (select statusapp from TransApprove where id=@Max) = -1
		insert into @table  
		select RequestNo,statusapp from TransApprove where id=@Max

		FETCH NEXT FROM cur_temp INTO @ID
	END

	CLOSE cur_temp
	DEALLOCATE cur_temp

	select * from #temp where id in (select a.ID from @table a)
END
go

