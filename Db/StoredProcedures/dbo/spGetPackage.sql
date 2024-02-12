-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetPackage]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@Company nvarchar(3),
	@type nvarchar(max),@from date,@to date
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)='8078',@Company nvarchar(3)='1011',@type nvarchar(max)='3',@from date='20220526',@to date='20220828'
 
	SET NOCOUNT ON;
	declare @customer nvarchar(max),@valid datetime,@diff int,@usertype nvarchar(max)
	--select ID,SAPMaterial,Name,Price,Currency,Unit from MasPrice where Company=@Company and SAPMaterial like '5%' and substring(SAPMaterial,2,1) in ('F')
	select top 1 --@diff=((month(RequireDate) - month(RequestDate))+1), 
	@diff=DATEDIFF(month, RequestDate,RequireDate) + 1,
	--@from=cast(RequestDate as date),@to=cast(RequireDate as date),
	@valid=t.RequestDate,@customer=CustomPrice,@usertype=UserType from TransTechnical t where t.ID=@Id  
    -- Insert statements for procedure here
	set @from =(case when @customer=1 then (SELECT DATEADD(yy, DATEDIFF(yy, 0, @from), 0)) else @from end)
	--set @to = (case when @customer=1 then @from else @to end)
	declare @lastyear date =(select dateadd(ms,-2,dateadd(year,0,dateadd(year,datediff(year,0,getdate())+1,0))) )
	set @to = case when @customer=1 then @to else (case when @to>@lastyear then @lastyear else @to end) end
	--select DATEDIFF(month, RequestDate,RequireDate),* from TransTechnical where id=1104
	if(substring(@Company,1,3)='101') begin
		select ID,Company,Material,[Description],
		Jan,
		feb,
		Mar,
		Apr,
		May,
		Jun,
		Jul,
		Aug,
		Sep,
		Oct,
		Nov,
		[Dec],Currency,Unit,[From],[To] from MasPricePolicy  
		where company=@company and Material like '5%' and dbo.fnc_checktype(isnull(BU,''),@usertype)>0 and (--@from between cast([From] as date) and cast ([To] as date) Or 
		@to between cast([From] as date) and cast ([To] as date)) 
		--select * from MasPricePolicy
		end
	else if(@type=0)
	begin 
		select * from(
 		select ID,Company,Material,[Description],case when @diff < 7 then PriceStd1
		else PriceStd2 end as 'Price' ,Currency,Unit,[From],[To] from MasPriceStd  
		where company=@company and Material like '5%' and ISNUMERIC(substring(Material,2,1))= @type
		--and Customer=@customer
		--and cast([From] as date) <= @from and cast ([To] as date)>=@to
		and (--@from between cast([From] as date) and cast ([To] as date) Or 
		@to between cast([From] as date) and cast ([To] as date)))#a union
		select ID,Company,Material,[Description],case when @diff < 7 then PriceStd1
		else PriceStd2 end as 'Price' ,Currency,Unit,[From],[To] from MasPriceStd  
		where company=@company and Material like '5122%'
		and (--@from between cast([From] as date) and cast ([To] as date) Or 
		@to between cast([From] as date) and cast ([To] as date)) end
	else  
	begin
		If Object_ID('tempdb..#temp')  is not null  drop table #temp	
 		select ID,Company,Material,[Description],case when @diff < 7 then PriceStd1
		else PriceStd2 end as 'Price' ,Currency,Unit,[From],[To] into #temp from MasPriceStd  
		where company=@company and Material like '5%' and ISNUMERIC(substring(Material,2,1))= @type
		and (--@from between cast([From] as date) and cast ([To] as date) Or 
		@to between cast([From] as date) and cast ([To] as date))

		delete #temp where Material like '5122%'
		select  * from #temp
	end
END

go

