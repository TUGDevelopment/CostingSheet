-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spcopyCostingRevised2] 
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@Requester nvarchar(max),
	@Per nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)=1226,@Per nvarchar(max)=0,@Requester nvarchar(max)='FO5910155'
	SET NOCOUNT ON;
	declare @RequestNo nvarchar(max),@Revised nvarchar(max)
	DECLARE @myid uniqueidentifier = NEWID();  
    -- Insert statements for procedure here
	if(select count(*) from TransCostingHeader where StatusApp=0)>0 
	begin
	SELECT @Revised= max(Revised)+1 from TransCostingHeader where ID=@Id
	insert into TransCostingHeader 
	select RequestNo
			,MarketingNumber
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
			,StatusApp=(case @per when 0 then 4 else 8 end)
			,Revised=@Revised,ExchangeRate,Netweight,0 from TransCostingHeader where ID=@Id
	SET @RequestNo = (SELECT CAST(scope_identity() AS int))
	--workflow
	Insert into TransApprove
	SELECT RequestNo=@RequestNo
      ,StatusApp
      ,Condition
      ,fn
      ,ActiveBy
      ,SubmitDate
      ,levelApp=(case  when @per=1 and levelApp=0 then 8  else levelApp end)
      ,tablename
	FROM TransApprove where RequestNo=@Id

	--flage status edit margin
	update TransCostingHeader set StatusApp=4 ,IsActive=1 where ID=@Id
	--costingitems
	declare @fn nvarchar(max)
	declare cur_table CURSOR FOR
	SELECT value FROM dbo.FNC_SPLIT('TransFormula,TransFormulaHeader,TransCosting',',')
	open cur_table
	FETCH NEXT FROM cur_table INTO @fn
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

		--select c.* FROM sys.tables AS t
		--INNER JOIN sys.columns c ON t.OBJECT_ID = c.OBJECT_ID WHERE t.name='TransFormula'

		DECLARE @cols AS NVARCHAR(MAX),
			@query  AS NVARCHAR(MAX);
		SET @cols = STUFF((SELECT ',' + QUOTENAME(c.column_name) 
				FROM #temp c ORDER BY c.column_id
				FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)') 
			,1,1,'')
			set @query='insert '+@fn+' select '+@cols+','+@RequestNo+' from '+@fn+' where RequestNo='+@Id
			print @query;
			execute(@query)
			FETCH NEXT FROM cur_table INTO @fn
	END
	CLOSE cur_table
	DEALLOCATE cur_table
	end
select @RequestNo 
END

--select * from Trans where ID='237'
--select * from MailData order by MailID

go

