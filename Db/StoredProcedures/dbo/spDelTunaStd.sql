-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spDelTunaStd]
	@Id nvarchar(max),	
	@user nvarchar(max),	
	@StatusApp nvarchar(max),	
	@tablename nvarchar(max)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)='1021170000100'
	SET NOCOUNT ON;
	--select * from TransCostingHeader
    -- Insert statements for procedure here
	--if(select Statusapp from TransTechnical where ID=@Id)=0
	--begin
	insert into MasHistory values(@Id,@user,@StatusApp,GETDATE(),'Delete',NULL,@tablename)
	declare @Status nvarchar(max)
	set @Status=(select top 1 StatusApp from TransTunaStd where ID=@Id)
	if @Status=0
		begin
			delete TransTunaStd where ID=@Id
			delete TransTunaStdItems where RequestNo=@Id
			delete TransUpCharge where RequestNo=@Id
			delete TransUtilize where RequestNo=@Id
			delete TransTunaCal where RequestNo=@Id
	end
	--delete TransFormula where RequestNo=@Id
	--end
END




go

