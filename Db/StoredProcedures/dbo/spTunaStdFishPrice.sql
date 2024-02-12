-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spTunaStdFishPrice] 
	-- Add the parameters for the stored procedure here
	@Code nvarchar(max),
	@from datetime,
	@to datetime,
	@FishGroup nvarchar(max),
	@SHD nvarchar(max),
	@FishCert nvarchar(max),
	@cols nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Code nvarchar(max)='3AAOCO2EXAP9SINNRN',	@from datetime='2020-07-30',@to datetime='2020-08-06',@cols nvarchar(max)='Jul',@FishGroup nvarchar(max)='SKJNM',@FishCert nvarchar(max)='GEN',@SHD nvarchar(max)='None'
    -- Insert statements for procedure here
	--update StdPricePolicy set [To]='2020-12-31'
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	SELECT * into #temp from StdPricePolicy where
	(@from between cast([From] as date) and cast ([To] as date) Or @to between cast([From] as date) and cast ([To] as date)) and FishGroup=@FishGroup
	and FishCert=@FishCert and SHD=@SHD
	--declare @diff int =(SELECT DATEDIFF(month, @from, @to)+1 AS DateDiff)
	--declare @loopCounter int = 0
	--declare @str nvarchar(50) ='X'
	declare @t table (name nvarchar(max))  
	--while (@loopCounter <@diff)
	--begin
		declare @query  AS NVARCHAR(MAX)
	--	set @str = (select DATEADD(month,@loopCounter,@to))
	--	set @cols = (select left(DATENAME(month, @str),3))
		declare @tempexch table( Rate nvarchar(max),Currency nvarchar(max))
		insert @tempexch exec spTunaStdExchangeRat @from,@to

		set @query = 'SELECT '+ @cols +' as Result,Currency,Unit,Description from #temp'
		declare @result table(Result float,Currency nvarchar(max),Unit nvarchar(max),Name nvarchar(max))
		insert @result Exec (@query)
		--INSERT INTO @t Exec (@query)
	--	SET @loopCounter  = @loopCounter  + 1
	--end
		declare @Rate float = (select top 1 Rate from @tempexch)
		select case when Currency='THB' then Result / @Rate else Result end 'Result',Unit,Name,Currency from @result
	--select SUM(convert(float,name))/@loopCounter as 'Result' from @t
	--select * from StdPricePolicy
END

go

