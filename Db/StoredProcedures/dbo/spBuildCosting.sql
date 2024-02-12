-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spBuildCosting]
	@GeID nvarchar(max),
	@user nvarchar(max) ,
	@param nvarchar(max) 
AS
BEGIN
	--declare @GeID nvarchar(max)=29399,@user nvarchar(max)='fo5910155',@param nvarchar(max)='43335,43336,43337'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	declare	@Code nvarchar(max),
	@Costing nvarchar(max),
	@Company nvarchar(max),
	@CustomerPrice nvarchar(max),
	@VarietyPack nvarchar(max),
	@Units nvarchar(max),
    @TotalPack nvarchar(max),
    @PackingStyle nvarchar(max),
	@TechnicalID nvarchar(max),@request nvarchar(50),@from date,@to date,@UserType nvarchar(max)
	select top 1 @VarietyPack=t.VarietyPack,@Units=isnull([Units],''),@PackingStyle=isnull(PackingStyle,'') from TransEditCosting t
	where t.RequestNo=@GeID and isnull(t.Units,'')<>''
	update TransEditCosting set Result=case when Result is null  then '' else Result end,
	SiteId=case when len(SiteId)=1 then 0 
	when Result='' then 0 
	else SiteId end where RequestNo=@GeID
	insert into MasHistory values (@GeID,@User,2,getdate(),NULL,NULL,'TransTechnical')
	select @TechnicalID=b.ID,@Code=b.RequestNo,@Company=b.Company,
	@CustomerPrice=b.CustomPrice,@from=cast(RequestDate as date),@to=cast(RequireDate as date),@UserType=usertype from TransTechnical b where b.Id=@GeID
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	SET NOCOUNT ON;
	select #a.ID,MarketingNumber,CostNo,#a.PackSize,#a.Packaging,Netweight,b.Code,
	#a.Customer,CanSize,b.Formula into #temp
	from (select a.ID,a.Customer,a.PackSize,a.CanSize,a.Packaging,Netweight,a.MarketingNumber,a.StatusApp from TransCostingHeader a where a.id in 
	(select max(convert(int,b.RequestNo)) from TransFormulaHeader b group by b.Code)) #a
	left join TransFormulaHeader b on #a.ID=b.RequestNo where #a.StatusApp in (0,4,8) and CostNo !=''
	
	--select max(convert(int,b.RequestNo)),b.Code from TransFormulaHeader b where code in ('3JCCM822R2DB3PPSPL','3HAOM822R2DB3PPSPL','3HSSX822R2DB3PPSPL') group by b.Code

	--1.update costing
	If Object_ID('tempdb..#newtable')  is not null  drop table #newtable
	update t set t.CostingSheet= (case when t.CostingSheet='' or t.CostingSheet is null then
	(select top 1 #temp.CostNo from #temp where #temp.Id in(select max(b.id) from #temp b where b.Code= t.Material) and #temp.Code= t.Material) else t.CostingSheet end)
	--select t.CostingSheet, a.CostNo
	from TransEditCosting t
	where t.RequestNo=@GeID and isnull(t.CostingSheet,'')='' and isnull(t.SiteId,0) =0 and t.ID in (select value from dbo.FNC_SPLIT(@param,','))
	--2 maping
	select t.ID as 'tID',t.CostingSheet,b.*
	,0 as [siteId] into #newtable from TransEditCosting t 
	left join #temp b on t.CostingSheet = b.CostNo where t.RequestNo=@GeID and isnull(t.SiteId,0) =0 and t.ID in (select value from dbo.FNC_SPLIT(@param,','))
	--select * from #newtable
	declare @cur int,@Netweight nvarchar(max),@Packaging nvarchar(max),@PackSize nvarchar(max),@Customer nvarchar(max),@CanSize nvarchar(max)
	declare cur_temp CURSOR FOR
	--select value from dbo.FNC_SPLIT(@cols,',')
	--3 create 
	select count(t.CostingSheet)c,t.Netweight,t.Packaging,t.PackSize,t.Customer,t.CanSize from #newtable t
	group by t.Netweight,t.Packaging,t.PackSize,t.Customer,t.CanSize
	open cur_temp

	FETCH NEXT FROM cur_temp INTO @cur,@Netweight,@Packaging,@PackSize,@Customer,@CanSize
	WHILE @@FETCH_STATUS = 0
	BEGIN
	--declare @Company nvarchar(max)='1031',@UserType nvarchar(max)=1
	declare @x datetime= GETDATE(),@runid nvarchar(max),@NewID int ,@NamingCode nvarchar(max),@fix int,@month nvarchar(max)   = month(getdate())
	--select CONVERT(int,CONVERT(CHAR(4), @x, 120))
	IF CONVERT(int,CONVERT(CHAR(4), @x, 120)) < 2500
	SET @x=DATEADD(yyyy,543,@x)
	--select FORMAT(@x, 'yy')
	set @NamingCode = (select top 1 NamingCode from MasPlant where Code =@Company and dbo.fnc_checktype(usertype,@UserType)>0)
	--set @runid = @NamingCode+FORMAT(@x, 'yy')+(
	--select format(isnull(max(right(MarketingNumber,4)),0)+1, '0000') from  TransCostingHeader 
	--where substring(MarketingNumber,3,2)=FORMAT(@x, 'yy') and Company=@Company --and substring(MarketingNumber,5,1)='5'
	--)
	IF CONVERT(int,CONVERT(CHAR(4), @x, 120)) < 2500
	SET @x=DATEADD(yyyy,543,@x)
	--select FORMAT(@x, 'yy')
	set @fix = (select SUBSTRING(@month,1,1))
	if(@month='10' or @month='11' or @month='12')
		set @fix='1'
	set @runid = @NamingCode+FORMAT(@x, 'yy')+(
	select format(@fix	,'0')+''+format(isnull(max(right(MarketingNumber,3)),0)+1, '000') from  TransCostingHeader 
	where substring(MarketingNumber,3,3)=concat(FORMAT(@x, 'yy'),@fix) and requestno<>0 and 
	substring(MarketingNumber,1,2) = @NamingCode)

	print @runid;

	DECLARE @myid uniqueidentifier = NEWID(),@id nvarchar(max)
	--SELECT CONVERT(nvarchar(max), @myid) AS 'NEWID';
	print @myid;  
	set @id=(select top 1 Id from #newtable where Netweight=@Netweight and Packaging=@Packaging and PackSize=@PackSize 
	  and isnull(Customer,'')=@Customer and CanSize=@CanSize)
	--select * from TransCostingHeader
	insert into TransCostingHeader
	select
	@TechnicalID,@runid,RDNumber,Company,
	(case when len(@Units)>0 then @Units else  PackSize end),
	getdate(),@user,null,null,@myid,Remark,
	(case when len(@PackingStyle)>0 then @PackingStyle else  CanSize end),Packaging,2,0
      ,isnull((select top 1 Rate from MasExchangeRat where Company=@Company and ExchangeType=@CustomerPrice and @from between Validfrom and Validto),0) as 'ExchangeRate'
	  ,Netweight,0,0,customer ,@From
      ,@To
      ,@UserType,@VarietyPack --ExchangeRate
	  from TransCostingHeader where ID 
	  in (@id) 

	  Set @NewID =(SELECT CAST(scope_identity() AS int))
	  --Exec spcreateapprove @NewID ,@user,1
	  update #newtable set [siteId]=@NewID where Netweight=@Netweight and Packaging=@Packaging and PackSize=@PackSize
	  and isnull(Customer,'')=@Customer and CanSize=@CanSize

	FETCH NEXT FROM cur_temp INTO @cur,@Netweight,@Packaging,@PackSize,@Customer,@CanSize
	END

	CLOSE cur_temp
	DEALLOCATE cur_temp
	update t set t.SiteId=b.SiteId
	from TransEditCosting t left join #newtable b on t.ID=b.tID where t.RequestNo=@GeID and isnull(t.SiteId,0) =0
	and t.ID in (select value from dbo.FNC_SPLIT(@param,','))
	select * from #newtable
END

go

