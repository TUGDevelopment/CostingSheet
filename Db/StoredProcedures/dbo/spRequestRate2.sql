-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spRequestRate2]
	-- Add the parameters for the stored procedure here
	@id nvarchar(max)
AS
BEGIN

--declare @id nvarchar(max) = '101|4130241|99'
--set @company =102
--set @material= '2XA1N10000NN-1'
--set @request='1021600003'
declare @company nvarchar(50),
		@material nvarchar(50),
		@request nvarchar(50),@from date,@to date
DECLARE @tbl1 TABLE (Value INT,String VARCHAR(MAX))
INSERT INTO @tbl1 VALUES(1,@id);
--select @company=,@material,@request
select @company=company,@material=material,@request=request from 
(SELECT t3.Value,[1] as company,[2] as material,[3] as request
FROM @tbl1 as t1
CROSS APPLY [dbo].[DelimitedSplit8K](String,'|') as t2
PIVOT(MAX(Item) FOR ItemNumber IN ([1],[2],[3])) as t3) #a

	select @from=cast(RequestDate as date),@to=cast(RequireDate as date) from TransTechnical where ID=@request
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @int int,@dt nvarchar(50),@valid datetime,@diff int,@exchangeRat float
	set @diff = (select top 1 (month(RequireDate) - month(RequestDate))+1 from TransTechnical where ID=@request)
	select @valid=t.RequestDate,@exchangeRat=(select top 1 Rate from MasExchangeRat where t.CreateOn between Validfrom and Validto  and Company=t.Company) from TransTechnical t where ID=@request
	set @dt = (select top 1 (month(RequireDate) - month(RequestDate))+1 from TransTechnical where ID=@request)
	set @int =(case when (select top 1 count(material) from MasPricePolicy where company=@company and Material=@material and cast([From] as date) <= @from
	and cast ([To] as date)>=@to) > 0 then 1
	when (select count(material) from MasPriceStd where Company=@company and Material=@material
	and cast([From] as date) <= @from
	and cast ([To] as date)>=@to) > 0 then 2 else 0 end )
	DECLARE @MyTableVar table(  
    Company nvarchar(50),  
    material nvarchar(50),  
    Description nvarchar(max),  
    Name float,
	Currency nvarchar(50),
	Unit nvarchar(50),
	exchangeRat float);  
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
		declare @valid2 datetime
		while (@loopCounter <@diff)
		begin
			set @valid2 = (select DATEADD(month,@loopCounter,@valid))
			if @Str = 'X'
				begin 
					set @str = (select left(DATENAME(month, @valid2),3))
				end
			else
			begin 
				set @str = @str +'+' + (select left(DATENAME(month, @valid2),3))
			end
			--PRINT  @valid2
			SET @loopCounter  = @loopCounter  + 1
		end
 
		declare @query  AS NVARCHAR(MAX),@cols AS NVARCHAR(MAX)
		set @cols = 'sum(' + @str + ') /' + convert(nvarchar(max),@loopCounter) 
		set @query = 'SELECT '+ @cols +' from MasPricePolicy where Material='''+@material + N''''
		--select  @query
		declare @t table (name nvarchar(max))
		insert @t (name)
		execute(@query)
		insert @MyTableVar
		select Company,material,[Description],(select * from @t),Currency,Unit,@exchangeRat from MasPricePolicy where company=@company and Material=@material
		and cast([From] as date) <= @from and cast ([To] as date)>=@to
	end
	else if(@int = 2)
	begin 
		--insert @MyTableVar
		insert into @MyTableVar
 		select Company,material,[Description],case when @dt < 7 then PriceStd1
		else PriceStd2 end ,Currency,Unit,@exchangeRat from MasPriceStd  where company=@company and Material=@material
		and cast([From] as date) <= @from and cast ([To] as date)>=@to
	end 
	select top 1
	Name as 'PriceOfUnit',Currency,Unit,case when Currency='THB' then 1 else exchangeRat end as ExchangeRate,
	(case when Currency='USD' then @exchangeRat*Name else Name end)/(case when Unit='Ton' then 1000000 else 1 end)'BaseUnit',
	(case when Unit='Ton' then 1000000 else 1 end) tonUnit,Company,Material,[Description] from @MyTableVar
END

--select * from MasPricePolicy where cast([From] as date)<='2017-09-17' and cast([To] as date) >='2017-09-17' 
 

go

