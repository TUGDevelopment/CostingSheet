-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spcopyCosting] 
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
		declare @x datetime= GETDATE(),@runid nvarchar(max),@Company nvarchar(max),@UserType nvarchar(max) 
	--select CONVERT(int,CONVERT(CHAR(4), @x, 120))
	IF CONVERT(int,CONVERT(CHAR(4), @x, 120)) < 2500
	SET @x=DATEADD(yyyy,543,@x)
	--select FORMAT(@x, 'yy')
	SELECT @Company=Company,@UserType=UserType from TransCostingHeader where ID=@Id
	set @runid = (select top 1 NamingCode from MasPlant where Company=@Company and dbo.fnc_checktype(usertype,@UserType)>0)+FORMAT(@x, 'yy')+(
	select format(isnull(max(right(MarketingNumber,4)),0)+1, '2000') from  TransCostingHeader 
	where substring(MarketingNumber,3,3)=FORMAT(@x, 'yy')+'2' and Company=@Company --and substring(MarketingNumber,5,1)='6'
	)
	print @runid;
	DECLARE @myid uniqueidentifier = NEWID();  
    -- Insert statements for procedure here
	--SELECT @StatusApp=StatusApp from TransCostingHeader where ID=@Id
	insert into TransCostingHeader 
	select RequestNo
			,MarketingNumber=@runid
			--,MarketingNumber=case when StatusApp='4' then @runid else MarketingNumber end
			,RDNumber
			,Company
			,PackSize
			,CreateOn=GETDATE()
			,CreateBy=@Requester
			,ModifyOn=null
			,ModifyBy=null
			,UniqueColumn=CONVERT(nvarchar(max), @myid)
			,Remark
			,CanSize
			,Packaging
			,2
			,Revised=
			case when StatusApp='4' then Revised+1 else
			'0' end,ExchangeRate,Netweight,0,@Id,Customer,[From],[To],UserType,VarietyPack from TransCostingHeader where ID=@Id
	SET @RequestNo = (SELECT CAST(scope_identity() AS int))
	--costingitems select * from TransCostingHeader
declare @fn nvarchar(max)
declare cur_table CURSOR FOR
SELECT value,@runid FROM dbo.FNC_SPLIT('TransFormula,TransFormulaHeader,TransCosting',',')
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
	WHERE t.name in (''+@fn+'') and c.name not in ('Id','RequestNo')
	ORDER BY c.column_id;

	--select * FROM TransFormulaHeader

	DECLARE @cols AS NVARCHAR(MAX),
		@query  AS NVARCHAR(MAX);
	SET @cols = STUFF((SELECT ',' + QUOTENAME(c.column_name) 
            FROM #temp c ORDER BY c.column_id
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
		if(@fn='TransFormulaHeader')
		set @query='insert '+@fn+ ' SELECT Name
		  ,Customer
		  ,Code
		  ,RefSamples
		  ,Formula
		  ,IsActive
		  ,Costper
		  ,'''+ @runid+'''+''''+'+'format(Formula,''00'')
		  ,Revised,MinPrice
		  ,'+@RequestNo+' from '+ @fn +' where RequestNo='+@Id
		else
		set @query='insert '+@fn+' select '+@cols+','+@RequestNo+' from '+@fn+' where RequestNo='+@Id
		print @query;
		execute(@query)
		FETCH NEXT FROM cur_table INTO @fn,@runid
END
CLOSE cur_table
DEALLOCATE cur_table
 
select @RequestNo 
END

 

go

