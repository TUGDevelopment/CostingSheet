-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetRequestNo]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@user nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Company nvarchar(max)='103',@user nvarchar(max)='FO5910155'
    -- Insert statements for procedure here	
	declare @type nvarchar(max)='0,2',@usertype nvarchar(max),@Plant nvarchar(max)
	select  @usertype =usertype,@Plant=Plant from ulogin where [user_name]=@user
	if(select count(value) from dbo.FNC_SPLIT(@usertype,';') where value=1)>0
	set @type ='0,1,2'
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select ID,RequestNo,MarketingNumber,format(RequireDate,'dd-MM-yyyy')'RequireDate',PackSize, Packaging, 
	RIGHT(CONCAT('00',Revised), 2)'Revised',UserType,RequestType,format(RequestDate,'dd-MM-yyyy')'RequestDate',isnull(NetWeight,'0|0')'NetWeight',
	DATEDIFF(day,RequireDate,GETDATE())x,Modified,Customer +'/'+ Destination as 'Customer' into #temp 
	--from TransTechnical Where Company in (select Code from MasPlant where company in (select value from dbo.FNC_SPLIT(@Company,';')) union select value from dbo.FNC_SPLIT(@Company,';')) 
	from TransTechnical Where dbo.fnc_checktype(Company,@Plant)>0
	and StatusApp = 4 --and id not in(select RequestNo from TransCostingHeader where Completed=1)	
	and usertype in (select value from dbo.FNC_SPLIT(@usertype,';')) 
	and RequestType in (select value from dbo.FNC_SPLIT(@type,','))
	and (DATEDIFF(day,Modified,GETDATE()))<360;--CAST(GETDATE() as DATE)

	--select *,(DATEDIFF(day,Modified,GETDATE())) from TransTechnical where statusapp=4
	declare @table table(
	ID int,
	RequestNo nvarchar(max),
	MarketingNumber nvarchar(max),
	RequireDate nvarchar(max),
	PackSize nvarchar(max), 
	Packaging nvarchar(max), 
	Revised int,
	UserType int,
	RequestType nvarchar(max),
	RequestDate nvarchar(max),Modified datetime,Customer nvarchar(max),NetWeight nvarchar(max)
	)
	declare @RequestNo nvarchar(50), @NetWeight nvarchar(100)
	declare cur_temp CURSOR FOR

	SELECT ID,(select top 1 value from dbo.FNC_SPLIT(NetWeight,'|'))'NetWeight' FROM #temp

	open cur_temp

	FETCH NEXT FROM cur_temp INTO @RequestNo, @NetWeight
	WHILE @@FETCH_STATUS = 0
	BEGIN
	
		insert into @table
		select b.ID,
		b.RequestNo,
		b.MarketingNumber,
		b.RequireDate,
		b.PackSize,
		b.Packaging,
		b.Revised,
		b.UserType,
		b.RequestType,
		b.RequestDate,b.Modified,Customer,
		value +'|'+ PARSENAME(REPLACE(b.NetWeight,'|','.'),1) from dbo.FNC_SPLIT(@NetWeight,',') a,#temp b
		where b.ID=@RequestNo
		FETCH NEXT FROM cur_temp INTO @RequestNo, @NetWeight
	END

	CLOSE cur_temp
	DEALLOCATE cur_temp
	--+++
	--If Object_ID('tempdb..#table')  is not null  drop table #table
	--select * into #table from @table  
	--delete @table
	--select * from #table
	--declare @ID nvarchar(50), @PackSize nvarchar(100),@countsize nvarchar(max),@NetWeight nvarchar(max)
	--declare cur_temp CURSOR FOR

	--SELECT ID,PackSize,(select count(value) from dbo.FNC_SPLIT(PackSize,',')) as 'countsize',
	--(select top 1 value from dbo.FNC_SPLIT(NetWeight,'|'))'NetWeight' FROM #temp
	--open cur_temp

	--FETCH NEXT FROM cur_temp INTO @ID, @PackSize,@countsize,@NetWeight
	--WHILE @@FETCH_STATUS = 0
	--BEGIN
	--	insert into @table
	--	select b.ID,
	--	b.RequestNo,
	--	b.MarketingNumber,
	--	b.RequireDate,
	--	convert(int,value),
	--	b.Packaging,
	--	b.Revised,
	--	b.RequestType,
	--	b.RequestDate,b.Modified,
	--	b.NetWeight from dbo.FNC_SPLIT(@PackSize,',') a, #temp b
	--where b.ID=@ID
	--FETCH NEXT FROM cur_temp INTO @ID, @PackSize,@countsize,@NetWeight
	--END

	--CLOSE cur_temp
	--DEALLOCATE cur_temp

	--DELETE a
	--FROM @table a
	--INNER JOIN TransCostingHeader b
	--  ON a.ID=b.RequestNo and a.PackSize=b.PackSize

	select ID,
	RequestNo,
	--MarketingNumber,
	RequireDate,
	isnull(PackSize,'0')'PackSize',
	isnull(Packaging,'')'Packaging',
	Revised,
	--isnull(UserType,'0')UserType,
	isnull(dbo.fnc_gettype(usertype),'0')Product,
	--RequestType,
	RequestDate,
	--Modified,
	isnull(Customer,'-')'Customer',
	isnull(NetWeight,'0')'NetWeight' from @table order by Modified,PackSize,NetWeight desc
	END

 
go

