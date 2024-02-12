-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spCopyTechnical] 
@RequestNo nvarchar(50),
@Requester nvarchar(250),
@RequestType nvarchar(max),
@usertype nvarchar(max),
@myid uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @RequestNo nvarchar(50)='171',@Requester nvarchar(250)='fo5910155',@RequestType nvarchar(250)='0'
	SET NOCOUNT ON;
	declare @Drainweight nvarchar(250),@Company nvarchar(max)
	declare @Revised nvarchar(max),@ID nvarchar(max)='0',@Copy nvarchar(max),@Count int,@X nvarchar(max)
	set @copy = (select RequestNo from TransTechnical where ID=@RequestNo)
	--DECLARE @myid uniqueidentifier = NEWID();  
	set @myid =(SELECT CONVERT(nvarchar(max), @myid));
	select @Revised=Revised+1,@X=RequestNo,@Company=substring(Company,1,3),@usertype=usertype from TransTechnical where ID=@RequestNo
	set @Count =(select count(*)from TransTechnical where RequestNo=@X and StatusApp not in (4,-1))
	print @Count;
	if(@Count>0) goto ExitToJump
	--if (select StatusApp from TransTechnical where ID=@RequestNo)=4 and @Count=0
	if @RequestType='0'
	begin
	set @Revised =(select MAX(Revised)+1 from TransTechnical where RequestNo=@copy)
	--set @Company=(select Company from transtechnical where RequestNo=@RequestNo)
	--set @id= @Company +''+ convert(nvarchar(2),right(year(getdate()),2)) +''+ 
	--(SELECT format(isnull(max(right(RequestNo,5)),0)+1, '00000') FROM TransTechnical
	--where SUBSTRING(RequestNo,4,2)=right(year(getdate()),2))
	--	print @id
	--end 
	declare @Newid nvarchar(max)
	--set @Newid= @Company + @RequestType + convert(nvarchar(2),right(year(getdate()),2)) +''+ 
	set @Newid= @Company + '0' + convert(nvarchar(2),right(year(getdate()),2)) +''+ 
	(SELECT format(isnull(max(right(RequestNo,5)),0)+1, '00000') FROM TransTechnical
	where SUBSTRING(RequestNo,5,2)=right(year(getdate()),2) )--and SUBSTRING(RequestNo,4,1)='0')
	print @Newid
	if @RequestType='2'
	set @copy=@Newid
	INSERT INTO TransTechnical
	select @copy,
	@Requester,
	RequestDate,
	RequireDate, 
	Company,
	PetCategory, 
	PetFoodType,
	CompliedWith,
	NutrientProfile,
	Requestfor,
	ProductType,
	ProductStyle,
	ProductNote,
	Media,
	ChunkType,
	NetWeight,
	PackSize,
	Packaging,
	Material,
	PackType,
	PackDesign,
	PackColor,
	PackLid,
	PackShape,
	PackLacquer,
	SellingUnit,
	CreateOn=getdate(),
	Marketingnumber,
	RDNumber= case when @RequestType='2' then @RequestNo else '' end,
	AcceptCostingRequest='',
	Null,
	Drainweight,
	Notes,
	Customer,
	CustomPrice,
	Brand,  
	Destination,
	Country,
	ESTVolume,
	ESTLaunching,
	ESTFOB,
	CustomerRequest,
	Ingredient,
	Claims,
	StatusApp=0,
	ReferenceNo=0,
	VetOther,
	PhysicalSample,
	Receiver,
	@RequestType,
	case when @RequestType='0' then @Revised else 0 end,
	Assignee,
	@usertype,
	SampleDate,
	PrdRequirement,
	SecInner,
	SecOuter,
	null,
	@myid from TransTechnical where ID=@RequestNo;
    --SELECT SCOPE_IDENTITY();
	SET @ID = (SELECT CAST(scope_identity() AS int))
	--workflow
	--Exec spcreateapprove @ID , @Requester,@RequestType
	--SET @ID =(select ID from TransTechnical where UniqueColumn=@myid)
	if(select count(*) from TransProductList where RequestNo=@RequestNo)>0 begin
	declare @fn nvarchar(max)
	declare cur_table CURSOR FOR
	SELECT value FROM dbo.FNC_SPLIT('TransProductList',',')
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

		DECLARE @cols AS NVARCHAR(MAX),
			@query  AS NVARCHAR(MAX);
		SET @cols = STUFF((SELECT ',' + QUOTENAME(c.column_name) 
				FROM #temp c ORDER BY c.column_id
				FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)') 
			,1,1,'')
			set @query='insert '+@fn+' select '+@cols+','+@ID+' from '+@fn+' where RequestNo='+@Id
			print @query;
			execute(@query)
			FETCH NEXT FROM cur_table INTO @fn
	END
	CLOSE cur_table
	DEALLOCATE cur_table
	end
	select @ID 
	end 
	else begin
ExitToJump:
	Set @ID=0
	end
	select @ID as 'ID'
END
--
 


go

