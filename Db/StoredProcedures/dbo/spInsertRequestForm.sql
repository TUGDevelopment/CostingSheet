
CREATE Procedure [dbo].[spInsertRequestForm]
	@ID int,
	@Requester nvarchar(50),
	@DocumentNo nvarchar(50),
	@Costno nvarchar(max),
	--@Customer nvarchar(250),
	@Result nvarchar(250),
	@ProductGroup nvarchar(50),
	@RawMaterial nvarchar(250),
	@StyleofPack nvarchar(50),
	@MediaType nvarchar(50),
	@NW nvarchar(50),
	@ContainerLid nvarchar(50),
	@Grade nvarchar(50),
	@Zone nvarchar(50),
	@StatusApp nvarchar(50),
	@Remark nvarchar(max),
	@CreateBy nvarchar(50),
	@RequestNo nvarchar(50),
	@RequestDate datetime,
	@Country nvarchar(max),
	@PetType nvarchar(max),
	@assignee nvarchar(max),
	@Division nvarchar(max),
	@Nutrition nvarchar(max),
	@RefSamples nvarchar(max),
	@Destination nvarchar(max),
	@PackingStyle nvarchar(max)
 AS
BEGIN
 
	SET NOCOUNT ON;
	if (select count(*) from TransRequestForm where id=@ID)=0 begin
	declare @runid nvarchar(max)
	set @runid= FORMAT(GetDate(), 'yyyyMM') +''+ 
	(SELECT format(isnull(max(right(RequestNo,5)),0)+1, '00000') FROM TransRequestForm
	where SUBSTRING(RequestNo,1,6)=FORMAT(GetDate(), 'yyyyMM'))
	--select * from TransRequestForm
	insert into TransRequestForm  
	select [Requester]=@Requester
      ,[RequestNo]=@RequestNo
	  ,[Costno]=@Costno
	  ,[RefSamples]=@RefSamples
	  ,[Destination]=@Destination
      ,[Result]=@Result
      ,[ProductGroup]=@ProductGroup
      ,[RawMaterial]=@RawMaterial
      ,[StyleofPack]=@StyleofPack
      ,[MediaType]=@MediaType
      ,[NW]=@NW
      ,[ContainerLid]=@ContainerLid
      ,[Grade]=@Grade
      ,[Zone]=@Zone
      ,0
	  ,[RequestDate]=@RequestDate

	  ,[Country]=@Country
	  ,[PetType]=@PetType
	  ,[Division]=@Division
	  ,[Nutrition]=@Nutrition
	  ,[PackingStyle]=@PackingStyle
      ,[Remark]=@Remark
      ,[CreateBy]=@CreateBy
      ,[CreateOn]=GETDATE()
      ,[ModifyBy]=null
      ,[ModifyOn]=null
      ,[DocumentNo]=@runid
	  ,[assignee]=@assignee
      ,[UniqueColumn]=NEWID()
	  SET @Id = (SELECT CAST(scope_identity() AS int))
	  end else
	  update TransRequestForm set 
	   [ProductGroup]=@ProductGroup
      ,[RawMaterial]=@RawMaterial
      ,[StyleofPack]=@StyleofPack
      ,[MediaType]=@MediaType
      ,[NW]=@NW
      ,[ContainerLid]=@ContainerLid
      ,[Grade]=@Grade
      ,[Zone]=@Zone
	  ,[PackingStyle]=@PackingStyle
	  ,[RefSamples]=@RefSamples
	  ,[Destination]=@Destination
	  ,[Country]=@Country
	  ,[PetType]=@PetType
	  ,[Division]=@Division
	  ,[Nutrition]=@Nutrition
	  ,ModifyBy=@CreateBy,ModifyOn=getdate() where id=@ID
	  select @Id
end
go

