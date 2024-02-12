-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create Procedure [dbo].[spGetCalculation3]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@Param nvarchar(max)
	--@RequestNo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--declare @ID nvarchar(max)=0,@Param nvarchar(max)=1218,@RequestNo nvarchar(max)=1230
	--select * from TransQuotationItems where RequestNo=@ID and SubID=@Param
	--If Object_ID('tempdb..#Quota')  is not null  drop table #Quota
	--select MinPrice,CAST(subContainers AS FLOAT)  sub,
	--Formula,case when len(OfferPrice)=0 then '0' else OfferPrice end OfferPrice
	--into #Quota from TransQuotationItems where RequestNo=@ID and SubID=@Param
	--select * from #Quota
	declare @table  table(
	ID int IDENTITY(1,1) NOT NULL,
	ParentID int,
	Location nvarchar(max),
	Name nvarchar(max),
	Price float,
	Formula nvarchar(max))
	
	DECLARE @i int = 0,@max int
	set @max=(select max(Formula) from TransFormulaHeader where RequestNo=@Param)
	WHILE @i < @max 
	BEGIN
		SET @i = @i + 1
		--if(@Param=@i) begin DECLARE @i int = 1
		declare @a float =0
		declare @sub float=0,@offer float=0,@min float=0 
		--(select @min=minprice,@sub=sub,@offer=OfferPrice from #Quota where Formula=@i)
		insert into @table 
		select 0,'','MinPrice',@min,@i union
		select 3,'','Overprice',@a,@i union
		select 4,'','Extracost',@a,@i union
		select 8,'','SubContainers',@sub,@i union
		select 9,'','OfferPrice',@offer,@i 
		--select 10,'','Upload Min/Offer Price',@PriceUpload,@i 
	END
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select * into #temp from @table

	--select * from #temp
	DECLARE @cols AS NVARCHAR(MAX),
		@query  AS NVARCHAR(MAX);

	SET @cols = STUFF((SELECT ',' + QUOTENAME(#a.formula) 
				FROM (select distinct formula from #temp)#a order by #a.Formula + 0 ASC
				FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)') 
			,1,1,'')

	print @cols;
	set @query = 'SELECT ROW_NUMBER() OVER(ORDER BY Name ASC) AS RowID,Name,' + @cols + ' from 
				(
					select Name,Price,formula from #temp 
			   ) x
				pivot 
				(
					 max(Price)
					for formula in (' + @cols + ')
				) p order by CASE Name 
							When ''MinPrice'' then 0
							When ''Overprice'' then 3
							When ''Extracost'' then 4
							When ''SubContainers'' then 8
							When ''OfferPrice'' then 9 ELSE 99 END;';
	--print @query;
	execute(@query)
END

 

go

