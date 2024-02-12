-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spinsertCosting]
 @Id nvarchar(50),
 @RequestNo nvarchar(50),
 @MarketingNumber nvarchar(max),
 @RDNumber nvarchar(50),
 @Company nvarchar(50),
 @PackSize nvarchar(50),
 @CreateBy nvarchar(max),
 @NewID nvarchar(max),
 @Remark nvarchar(max),
 @CanSize nvarchar(max),
 @Packaging nvarchar(max),
 @StatusApp nvarchar(max),
 @Netweight nvarchar(max),
 @Customer nvarchar(max),
 @ExchangeRate nvarchar(max),
 @From datetime,
 @To datetime,
 @UserType nvarchar(max),
 @VarietyPack nvarchar(max)
 AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Company nvarchar(max)='101',@MarketingNumber nvarchar(max)='',@UserType nvarchar(max)=2,@Id nvarchar(max)=0
	declare @x datetime= GETDATE(),@runid nvarchar(max),@status nvarchar(max)
	--select CONVERT(int,CONVERT(CHAR(4), @x, 120))
	IF CONVERT(int,CONVERT(CHAR(4), @x, 120)) < 2500
	SET @x=DATEADD(yyyy,543,@x)
	--select FORMAT(@x, 'yy')
	set @status=(select StatusApp from TransCostingHeader where id=@Id)
	if len(@MarketingNumber)=0
	set @runid = (select top 1 NamingCode from MasPlant where Company=@Company and dbo.fnc_checktype(usertype,@UserType)>0)+FORMAT(@x, 'yy')+(
	select format(isnull(max(right(MarketingNumber,4)),0)+1, '0000') from  TransCostingHeader 
	where substring(MarketingNumber,3,2)=FORMAT(@x, 'yy')and Company=@Company)
	else
	set @runid=@MarketingNumber
	print @runid;
	SET NOCOUNT ON;
	if (SELECT count(*) FROM TransCostingHeader where Id=@Id)=0
	begin
	insert into TransCostingHeader
	select RequestNo=@RequestNo,
	MarketingNumber=@runid,
	RDNumber=@RDNumber,
	Company=@Company,
	PackSize=@PackSize,
	CreateOn=getdate(),
	CreateBy=@CreateBy,
	ModifyOn=Null,
	ModifyBy=Null,
	UniqueColumn=@NewID,
	Remark=@Remark,
	CanSize=@CanSize,
	Packaging=@Packaging,
	StatusApp=@StatusApp,
	Revised=0,
	ExchangeRate=@ExchangeRate,
	Netweight=@Netweight,
	IsActive=0,
	IsRefer=0,
	Customer=@Customer,
	[From]=@From,
	[To]=@To,
	UserType=@UserType,
	VarietyPack=@VarietyPack
 --SELECT SCOPE_IDENTITY();
	SET @ID = (SELECT CAST(scope_identity() AS int))
	--Set @Id =(SELECT ID FROM TransCostingHeader Where ID = (SELECT CAST(scope_identity() AS int)))
	end
	else
	begin
	if (@status=-1)
		update TransApprove set Statusapp=0,
		ActiveBy=null,
		SubmitDate=null where RequestNo=@Id and tablename=1

	UPDATE TransCostingHeader Set
	RequestNo=@RequestNo,
	MarketingNumber=@MarketingNumber,
	RDNumber=@RDNumber,
	Company=@Company,
	PackSize=@PackSize,
	ModifyOn=getdate(),
	ModifyBy=@CreateBy,
	--UniqueColumn=@NewID,
	[From]=@From,
	[To]=@To,
	Remark=@Remark,CanSize=@CanSize,Packaging=@Packaging,
	Netweight=@Netweight,
	ExchangeRate=@ExchangeRate,	Customer=@Customer,VarietyPack=@VarietyPack where ID=@Id
	end
	--#workflow
	if (@StatusApp='2') 
	and (select count(Id) from TransApprove where RequestNo=@ID and tablename=1)=0--send approve
	begin
		declare @Type nvarchar(1)=1
		if(@UserType<>0)
		set @Type =2
		Exec spcreateapprove @ID , @CreateBy,@Type,0
	end
	select * from TransCostingHeader where ID=@Id
END



go

