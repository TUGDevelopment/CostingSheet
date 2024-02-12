-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spDelCusFormulaHD]
	@Id nvarchar(max),	
	@user nvarchar(max),	
	@StatusApp nvarchar(max),	
	@tablename nvarchar(max)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @user nvarchar(max)='FO5910155',@tablename nvarchar(max)='TransCusFormulaHeader',@StatusApp nvarchar(max)='-1', @Id nvarchar(max)='2'
	SET NOCOUNT ON;
	--select * from TransCostingHeader
    -- Insert statements for procedure here
	--if(select Statusapp from TransTechnical where ID=@Id)=0
	--begin
	insert into MasHistory values(@Id,@user,@StatusApp,GETDATE(),'Delete',NULL,@tablename)
	if (select count(*) from TransCusFormula where SubID=@Id and isnull(RequestNo,'')='')=0
		begin
			delete TransCusFormulaHeader where ID=@Id
			delete TransCusFormula where SubID=@Id
	end
END


go

