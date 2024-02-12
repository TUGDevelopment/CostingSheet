-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spcopyTunaStd] 
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@Requester nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--select * from TransTunaStd where RequestNo='20201000001'
	--declare @Id nvarchar(max)=69,@Requester nvarchar(max)='FO5910155'
	SET NOCOUNT ON;
	declare @RequestNo nvarchar(max)
	--add new
	declare @x datetime= GETDATE(),@runid nvarchar(max)
	--select CONVERT(int,CONVERT(CHAR(4), @x, 120))
	IF CONVERT(int,CONVERT(CHAR(4), @x, 120)) < 2500
	SET @x=DATEADD(yyyy,543,@x)
	--select FORMAT(@x, 'yy')
	set @runid= FORMAT(GetDate(), 'yyyyMM') +''+ 
	(SELECT format(isnull(max(right(RequestNo,5)),0)+1, '00000') FROM TransTunaStd
	where SUBSTRING(RequestNo,1,6)=FORMAT(GetDate(), 'yyyyMM'))
	print @runid;
	DECLARE @myid uniqueidentifier = NEWID();  
    -- Insert statements for procedure here
	--SELECT * from TransTunaStd where ID=@Id
	insert into TransTunaStd 
	select [Material]
      ,[Incoterm]
      ,[Route]
      ,[Size]
      ,[Quantity]
      ,[PaymentTerm]
      ,0
      ,[Interest]
      ,@Requester
      ,Getdate(),null,null
      ,[Freight]
      ,[Insurance]
      ,[Currency]
      ,[ExchangeRate]
      ,[Remark]
      ,[Customer]
      ,[ShipTo]
      ,[Extracost]
	  ,[From]
	  ,[To]
	  ,[ValidityDate]
	  ,[billto]
	  ,[Zone]
	  ,0
	  ,@runid,NEWID() from TransTunaStd where ID=@Id
	SET @RequestNo = (SELECT CAST(scope_identity() AS int))

declare @RowID nvarchar(max)
declare cur_table CURSOR FOR
SELECT Id,@RequestNo FROM TransTunaStdItems where RequestNo=@Id
open cur_table
FETCH NEXT FROM cur_table INTO @RowID,@RequestNo
WHILE @@FETCH_STATUS = 0
BEGIN
If Object_ID('tempdb..#temp')  is not null  drop table #temp
	SELECT t.name AS table_name,
	SCHEMA_NAME(schema_id) AS schema_name,
	c.name AS column_name,column_id into #temp
	FROM sys.tables AS t
	INNER JOIN sys.columns c ON t.OBJECT_ID = c.OBJECT_ID
	WHERE t.name in ('TransTunaStdItems') and c.name not in ('Id','RequestNo')
	ORDER BY c.column_id;
	--select * from TransTunaStdItems
	DECLARE @cols AS NVARCHAR(MAX),@SubIDNew NVARCHAR(MAX),@query  AS NVARCHAR(MAX);
		insert TransTunaStdItems select [Material],[Name]
      ,[Utilize]
      ,[From]
      ,[To]
      ,[RawMaterial]
	  ,SpecialFishPrice
	  ,PackSize
	  ,Yield
	  ,FillWeight
      ,[SubContainers]
      ,[Media]
      ,[Packaging]
      ,[LOHCost]
      ,[PackingStyle]
	  ,SecPackaging
      ,[Upcharge]
      ,[Commission]
      ,[OverPrice]
      ,[OverType]
      ,[Pacifical]
      ,[MSC]
      ,[Margin]
	  ,[Authorized_price]
	  ,[Bidprice]
	  ,[Margin]
      ,[MinPrice]
      ,[OfferPrice],'',@RequestNo from TransTunaStdItems where Id=@RowID
		SET @SubIDNew= (SELECT CAST(scope_identity() AS int))
		exec spCopyStdItems @RequestNo,@SubIDNew,@RowID 
		FETCH NEXT FROM cur_table INTO @RowID,@RequestNo
END
CLOSE cur_table
DEALLOCATE cur_table
 
select @RequestNo 
END

--select * from TransTunaStdItems 

go

