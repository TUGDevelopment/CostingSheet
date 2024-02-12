-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spGetCalculation2]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@Param nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--declare @ID nvarchar(max)=8973,@Param nvarchar(max)=1
	If Object_ID('tempdb..#Quota')  is not null  drop table #Quota
	select CAST(subContainers AS FLOAT)  sub,
	Formula,OfferPrice  into #Quota from TransQuotationItems where RequestNo=@ID
	--select * from #Quota
	declare @table  table(
	ID int IDENTITY(1,1) NOT NULL,
	ParentID int,
	Location nvarchar(max),
	Name nvarchar(max),
	Price float,
	Formula nvarchar(max))
	
	DECLARE @i int = 0,@max int
	set @max=(select max(Formula) from TransFormulaHeader where RequestNo=@ID)
	WHILE @i < @max 
	BEGIN
		SET @i = @i + 1
		--if(@Param=@i) begin DECLARE @i int = 1
		declare @a float =0
		declare @sub float=0,@offer float=0
		(select @sub=sub,@offer=OfferPrice from #Quota where Formula=@i)
		insert into @table 
		select 0,'','FOB Price',@a,@i union
		select 1,'','Commission',@a,@i union
		select 2,'','Interest',@a,@i union
		select 3,'','Overprice',@a,@i union
		select 4,'','Extracost',@a,@i union
		select 5,'','Total',@a,@i union
		select 6,'','Freight',@a,@i  union
		select 7,'','Insurance',@a,@i union
		select 8,'','CIF Price',@a,@i union
		select 9,'','subContainers',@sub,@i union
		select 10,'','Offer Price',@offer,@i 
--end 		else
--		insert into @table 
--		select 0,'','FOB Price',0,@i 
		/* your code*/
	END
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select * into #temp from @table

	--select * from #temp
	DECLARE @cols AS NVARCHAR(MAX),
		@query  AS NVARCHAR(MAX);

	SET @cols = STUFF((SELECT ',' + QUOTENAME(#a.formula) 
				FROM (select distinct formula from #temp)#a order by #a.Formula
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
							When ''FOB Price'' then 0
							When ''Commission'' then 1 
							When ''Interest'' then 2 
							When ''Overprice'' then 3 
							When ''Extracost'' then 4 
							When ''Total'' then 5 
							When ''Freight'' then 6 
							When ''Insurance'' then 7 
							When ''CIF Price'' then 8
							When ''subContainers'' then 9
							When ''Offer Price'' then 10 ELSE 99 END;';
	--print @query;
	execute(@query)
END
go

