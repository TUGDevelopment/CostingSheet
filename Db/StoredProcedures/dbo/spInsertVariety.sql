
CREATE procedure spInsertVariety
@ID nvarchar(max),
@VarietyPack nvarchar(max),
@Code  nvarchar(max),
@Quantity nvarchar(max),
@SellingUnit nvarchar(max),
@TotalPack nvarchar(max),
@PackingStyle nvarchar(max),
@Result nvarchar(max),
@SiteId nvarchar(max),
@StatusApp nvarchar(max),
@RequestNo nvarchar(max)
as begin
insert into TransVarietyPack
select  VarietyPack=@VarietyPack,
      Code=@Code,
      Quantity=@Quantity,
      SellingUnit=@SellingUnit,
      TotalPack=@TotalPack,
      PackingStyle=@PackingStyle,
      Result=@Result,
      SiteId=@SiteId,
	  StatusApp=@StatusApp,
      RequestNo=@RequestNo
end
go

