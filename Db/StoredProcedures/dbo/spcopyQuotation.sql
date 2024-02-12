-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spcopyQuotation] 
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@Requester nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)=14413,@Requester nvarchar(max)='FO5910155'
	SET NOCOUNT ON;
	declare @RequestNo nvarchar(max)
	--add new
	declare @x datetime= GETDATE(),@runid nvarchar(max)
	--select CONVERT(int,CONVERT(CHAR(4), @x, 120))
	IF CONVERT(int,CONVERT(CHAR(4), @x, 120)) < 2500
	SET @x=DATEADD(yyyy,543,@x)
	--select FORMAT(@x, 'yy')
	set @runid= FORMAT(GetDate(), 'yyyyMM') +''+ 
	(SELECT format(isnull(max(right(RequestNo,5)),0)+1, '00000') FROM TransQuotationHeader
	where SUBSTRING(RequestNo,1,6)=FORMAT(GetDate(), 'yyyyMM'))
	print @runid;
	DECLARE @myid uniqueidentifier = NEWID();  
    -- Insert statements for procedure here
	--SELECT @StatusApp=StatusApp from TransCostingHeader where ID=@Id
	insert into TransQuotationHeader 
	select @Requester,Getdate(),null,null
		  ,[Commission]
		  ,[Brand]
		  ,[Incoterm]
		  ,[Route]
		  ,[Size]
		  ,[PaymentTerm]
		  ,[Interest]
		  ,[Freight]
		  ,[Insurance]
		  ,[Remark]
		  ,[Customer]
		  ,[ShipTo]
		  ,0
		  ,[Currency]
		  ,ExchangeRate
		  ,@runid,NEWID() from TransQuotationHeader where ID=@Id
	SET @RequestNo = (SELECT CAST(scope_identity() AS int))

declare @fn nvarchar(max)
declare cur_table CURSOR FOR
SELECT value,@runid FROM dbo.FNC_SPLIT('TransQuotationItems',',')
open cur_table
FETCH NEXT FROM cur_table INTO @fn,@runid
WHILE @@FETCH_STATUS = 0
BEGIN
If Object_ID('tempdb..#temp')  is not null  drop table #temp
	SELECT t.name AS table_name,
	SCHEMA_NAME(schema_id) AS schema_name,
	c.name AS column_name,column_id into #temp
	FROM sys.tables AS t
	INNER JOIN sys.columns c ON t.OBJECT_ID = c.OBJECT_ID
	WHERE t.name in (''+@fn+'') and c.name not in ('Id','SubID')
	ORDER BY c.column_id;

	DECLARE @cols AS NVARCHAR(MAX),
		@query  AS NVARCHAR(MAX);
	SET @cols = STUFF((SELECT ',' + QUOTENAME(c.column_name) 
            FROM #temp c ORDER BY c.column_id
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
		set @query='insert '+@fn+' select '+@cols+','+@RequestNo+' from '+ @fn +' where SubID='+@Id
		print @query;
		execute(@query)
		--reset 
		update TransQuotationItems set IsActive=0,ActionDate=null where SubID=@Id
		FETCH NEXT FROM cur_table INTO @fn,@runid
END
CLOSE cur_table
DEALLOCATE cur_table
 
select @RequestNo 
END

 

go

