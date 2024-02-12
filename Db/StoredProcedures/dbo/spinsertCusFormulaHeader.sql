-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spinsertCusFormulaHeader]
	-- Add the parameters for the stored procedure here
@Id nvarchar(max)
      ,@Name nvarchar(max)
      ,@Customer nvarchar(max)
      ,@Code nvarchar(max)
      ,@RefSamples nvarchar(max)
      ,@Formula nvarchar(max)
      ,@IsActive nvarchar(max)
      ,@Costper nvarchar(max)
      ,@CostNo nvarchar(max)
      ,@Revised nvarchar(max)
      ,@MinPrice nvarchar(max)
      ,@UniqueColumn nvarchar(max)
      ,@NetWeight nvarchar(max)
      ,@Packaging nvarchar(max)
	  ,@Company nvarchar(max)
	  ,@ProductStyle nvarchar(max)
	  ,@FW nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	if (select count(*) from TransCusFormulaHeader where id=@id)=0 begin
	declare @runid nvarchar(max)
	set @runid= FORMAT(GetDate(), 'yyyyMM') +''+ 
	(SELECT format(isnull(max(right(RequestNo,5)),0)+1, '00000') FROM TransCusFormulaHeader
	where SUBSTRING(RequestNo,1,6)=FORMAT(GetDate(), 'yyyyMM'))
	insert into TransCusFormulaHeader
	values(@Name,@Customer,@Code,@RefSamples,@Formula,@IsActive,@Costper,@CostNo,@Revised,@MinPrice,@UniqueColumn,0,@NetWeight,@Packaging,@Company,@ProductStyle,@FW,@runid) 
	  SET @Id = (SELECT CAST(scope_identity() AS int))
	end else 
	update TransCusFormulaHeader set 
	Company=@Company,Name=@Name,
	Customer=@Customer,CostNo=@CostNo,RefSamples=@RefSamples,FW=@FW,Revised=@Revised,Code=@Code,Packaging=@Packaging,ProductStyle=@ProductStyle,NetWeight=@NetWeight where ID=@ID
	select @Id as ID
END
--select * from TransCusFormulaHeader
go

