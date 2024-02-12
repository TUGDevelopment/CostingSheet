-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spcopyCostingsheet] 
	-- Add the parameters for the stored procedure here
	@RequestNo nvarchar(max), 
	@Requester nvarchar(max),
	@CostingNo nvarchar(max)
AS
BEGIN
	--declare @CostingNo nvarchar(max)='GP600008',@RequestNo nvarchar(max)='1047',@Requester nvarchar(max)='fo5910155'
	set @CostingNo = RTRIM(LTRIM(@CostingNo))
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @table table(costingno nvarchar(max),formula nvarchar(max),n int)
	declare @ID int,@runid nvarchar(max),@x datetime= GETDATE();
	IF CONVERT(int,CONVERT(CHAR(4), @x, 120)) < 2500
	SET @x=DATEADD(yyyy,543,@x) 
    -- Insert statements for procedure here
	if len(@CostingNo)=8
	begin
	declare @n nvarchar(max)=(select id from TransCostingHeader where MarketingNumber= @CostingNo
	and StatusApp <>7)
	--insert into @table(costingno)values(@CostingNo)
	insert into @table(formula,n)
	select formula,requestno from TransFormula where RequestNo=@n
	group by Formula,RequestNo
	update @table set costingno=@CostingNo
	end
	else if len(@CostingNo)=10
	insert into @table values(
	left(@CostingNo,8),
	substring(@costingno,9,2),0)
	DECLARE @myid uniqueidentifier = NEWID();  
	update t set n=a.ID
	from @table t left join TransCostingHeader a on a.MarketingNumber=t.costingno
	set @runid = (select fn from MasCompany where substring(Code,1,3) in (select substring(company,1,3) from TransCostingHeader where MarketingNumber=left(@CostingNo,8)))+FORMAT(@x, 'yy')+(
	select format(isnull(max(right(MarketingNumber,4)),0)+1, '0000') from  TransCostingHeader 
	where substring(MarketingNumber,3,2)=FORMAT(@x, 'yy'))
	print @runid;
	--select * from @table
	insert into TransCostingHeader
	select RequestNo=@RequestNo
      ,MarketingNumber=@runid
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
      ,StatusApp=0
      ,Revised
      ,ExchangeRate
	  ,Netweight
      ,IsActive,0,Customer,[From]
      ,[To]
      ,[UserType] from TransCostingHeader where ID 
	in (select n from @table) and StatusApp <>7
	SET @ID = (SELECT CAST(scope_identity() AS int))
		Exec spcreateapprove @ID ,@Requester ,1
--Formula
	insert into TransFormula
	SELECT Component
      ,SubType
      ,[Description]
      ,Material
      ,Result
      ,Yield
      ,RawMaterial
      ,Name
      ,PriceOfUnit
      ,Currency
      ,Unit
      ,ExchangeRate
      ,BaseUnit
      ,PriceOfCarton
      ,a.Formula
      ,IsActive
      ,Remark
      ,LBOh
      ,LBRate
      ,RequestNo=@ID
  FROM TransFormula a inner join @table b on b.n=a.RequestNo and cast(b.formula as int)=a.Formula --and IsActive=0
  --Margin,Upcharge
  insert into TransCosting
  SELECT Component
      ,SAPMaterial
      ,[Description]
      ,Quantity
      ,PriceUnit
      ,Amount
      ,Per
      ,SellingUnit
      ,Loss
      ,CreateOn=getdate()
      ,CreateBy=@Requester
      ,ModifyOn=null
      ,ModifyBy=null
      ,a.Formula
      ,RequestNo=@ID
  FROM TransCosting a inner join @table b on b.n=a.RequestNo and cast(b.formula as int)=a.Formula 
  select @ID
END
 


go

