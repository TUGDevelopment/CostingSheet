-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spRequestRate]
	-- Add the parameters for the stored procedure here
	@id nvarchar(max)
AS
BEGIN

--declare @id nvarchar(max) = '103|11M320000001|4391'
--exec spRequestRate @id
--set @company =102
--set @material= '2XA1N10000NN-1'
--set @request='1021600003'
If Object_ID('tempdb..#MasPricePolicy')  is not null  drop table #MasPricePolicy
select * into #MasPricePolicy from MasPricePolicy where IsActive=0  
declare @company nvarchar(50),
		@material nvarchar(50),
		@request nvarchar(50),@from date,@to date
DECLARE @tbl TABLE (Value INT,String VARCHAR(MAX))
INSERT INTO @tbl VALUES(1,@id);
--select @company=,@material,@request
select @company=company,@material=material,@request=request from 
(SELECT t3.Value,[1] as company,[2] as material,[3] as request
FROM @tbl as t1
CROSS APPLY [dbo].[DelimitedSplit8K](String,'|') as t2
PIVOT(MAX(Item) FOR ItemNumber IN ([1],[2],[3])) as t3) #a

	select @from=cast(RequestDate as date),@to=cast(RequireDate as date) from TransTechnical where ID=@request
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @int int,@dt nvarchar(50),@valid datetime,@diff int,@exchangeRat float,@customer nvarchar(50)
	select top 1 --@diff=((month(RequireDate) - month(RequestDate))+1), 
	@diff=DATEDIFF(month, RequestDate,RequireDate)+1,
	@valid=t.RequestDate
	,@customer=CustomPrice 
	from TransTechnical t where ID=@request
	declare @lastyear date
	if (year(@from)<>year(@to))
 	set @lastyear =(select dateadd(ms,-2,dateadd(year,0,dateadd(year,datediff(year,0,@from)+1,0))) )
	--set @from =(case when @customer=1 then (SELECT DATEADD(yy, DATEDIFF(yy, 0, @from), 0)) else @from end)
	--set @to = (case when @customer=1 then @from else @to end)
	if @Customer=1
	set @from= (DATEADD(yy, DATEDIFF(yy, 0, @to), 0))
	else
	set @to = (case when @to>@lastyear then @lastyear else @to end)
 
	set @exchangeRat=(select top 1 convert(float,Rate) * Ratio from MasExchangeRat where @from between Validfrom and Validto  
	and Company=substring(@company,1,3)and ExchangeType=@Customer ) 

	--declare @temp TableType
	--insert into @temp
	--select code from SMITEMMS 
	--	where code like case when substring(@material,1,1)='1' then STUFF(@material,3,1, '%') else @material end
	--SET @material = (select (case when dbo.FNC_STUFF(@temp)<> '' then  dbo.FNC_STUFF(@temp) else @material end))

	set @dt = @diff	
	set @int =(case when (select top 1 count(material) from #MasPricePolicy where Material in (select value from dbo.FNC_SPLIT(@material,',')
	)and IsActive=0  
	--and Customer=@customer 
	--and cast([From] as date) <= @from and cast ([To] as date)>=@to
	and (@from between cast([From] as date) and cast ([To] as date) Or @to between cast([From] as date) and cast ([To] as date))
	) > 0 then 1
	when (select count(material) from MasPriceStd where Company=@company and Material in (select value from dbo.FNC_SPLIT(@material,',')) 
	--and Customer=@customer
	--and cast([From] as date) <= @from and cast ([To] as date)>=@to
	and (@from between cast([From] as date) and cast ([To] as date) Or @to between cast([From] as date) and cast ([To] as date))
	) > 0 then 2 else 0 end )
	--print @int;
	DECLARE @MyTableVar table(  
    Company nvarchar(50),  
    material nvarchar(50),  
    Description nvarchar(max),  
    Name float,
	Currency nvarchar(50),
	Unit nvarchar(50),
	exchangeRat float);
	declare @t table (name nvarchar(max))  
	--select @int
	--Company,material,Description,(select * from @t),Currency,Unit,@exchangeRat
	if (@int = 1)
	begin 
		--select Company,material,Description,case when @dt < 4 then Q1
		--when @dt < 7 then Q2
		--when @dt < 10 then Q3 else Q4 end,Currency,Unit from tblPolicy where company=@company and Material=@material
		--select * from wf012
		--select left(DATENAME(month, '12:10:30.123'),3)
		--++++++++++++++++++++++++++++++++++++++++++++
		declare @loopCounter int = 0
		declare @str nvarchar(50) ='X'
		--declare @valid2 datetime
		while (@loopCounter <@diff)
		begin
			declare @query  AS NVARCHAR(MAX),@cols AS NVARCHAR(MAX)
			--set @cols = 'sum(' + @str + ') /' + convert(nvarchar(max),@loopCounter) 
			set @str = (select DATEADD(month,@loopCounter,@valid))
			--if @Str = 'X'
			--	begin 
			set @cols = (select left(DATENAME(month, @str),3))
			--	end
			--else
			--begin 
			--	set @str = @str +'+' + (select left(DATENAME(month, @valid2),3))
			--end
			--PRINT  @valid2
			--set @query = 'SELECT '+ @cols +' * (case when Currency=''USD'' then ' + convert(nvarchar(max),@exchangeRat) + ' else 1 end) from MasPricePolicy where Material in ('''+  
			declare @isactive nvarchar(max)=''
			if @Customer=2
			begin
			set @isactive ='IsActive=0 and'
			end
			set @query = 'SELECT '+ @cols +' from MasPricePolicy where '+ @isactive +' Material in ('''+  
			(select value from dbo.FNC_SPLIT(@material,',')) +N''') and cast(''' + convert(nvarchar(24),
			(DATEADD(month, @loopCounter,@from)), 101) + ''' as date) between cast([From] as date) and cast ([To] as date)'
			--select  @query
			INSERT INTO @t Exec (@query)
			--SELECT DATEADD(month, 2, '2017/08/25') AS DateAdd;
			SET @loopCounter  = @loopCounter  + 1
		end
		insert @MyTableVar
		select '',material,[Description],(select avg(CAST(name as float)) from @t) ,Currency,Unit,@exchangeRat 
		from MasPricePolicy where Material in (select value from dbo.FNC_SPLIT(@material,','))
		and IsActive=0  
		--and Customer=@customer
		--and cast([From] as date) <= @from and cast ([To] as date)>=@to
		and (@from between cast([From] as date) and cast ([To] as date) Or @to between cast([From] as date) and cast ([To] as date))
	end
	else if(@int = 2)
	begin 
		--insert @MyTableVar
		insert into @MyTableVar
 		select Company,material,[Description],case when @dt < 7 then PriceStd1
		else PriceStd2 end ,Currency,Unit,@exchangeRat from MasPriceStd  where company=@company and Material in (select value from dbo.FNC_SPLIT(@material,','))
		--and Customer=@customer
		--and cast([From] as date) <= @from and cast ([To] as date)>=@to
		and (@from between cast([From] as date) and cast ([To] as date) Or @to between cast([From] as date) and cast ([To] as date))
	end 
	--select * from @t
	select top 1
	Name as 'PriceOfUnit',Currency,Unit,case when Currency='THB' then 1 else exchangeRat end as ExchangeRate,
	(case when Currency='USD' then @exchangeRat*Name else Name end)/(case when Unit='Ton' then 1000000 else 1 end)'BaseUnit',
	(case when Unit='Ton' then 1000000 else 1 end) tonUnit,Company,Material,[Description] from @MyTableVar
END

--select * from MasPricePolicy where cast([From] as date)<='2017-09-17' and cast([To] as date) >='2017-09-17' 
 --SELECT sum(Dec+Jan+Feb+Mar+Apr+May+Jun+Jul+Aug+Sep+Oct+Nov) /12 from MasPricePolicy where Material='11M300000001'

 

go

