-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spDelTechnical]
	@Id nvarchar(max),	
	@user nvarchar(max),	
	@StatusApp nvarchar(max),	
	@tablename nvarchar(max)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @user nvarchar(max)='FO5910155',@tablename nvarchar(max)='TransTechnical',@StatusApp nvarchar(max)='-1', @Id nvarchar(max)='8018'
	SET NOCOUNT ON;
	--select * from TransCostingHeader
    -- Insert statements for procedure here
	--if(select Statusapp from TransTechnical where ID=@Id)=0
	--begin
	insert into MasHistory values(@Id,@user,@StatusApp,GETDATE(),'Delete',NULL,@tablename)
	declare @RequestType nvarchar(max)
	set @RequestType=(select top 1 RequestType from TransTechnical where ID=@Id)
	delete TransTechnical where ID=@Id
	if @RequestType=0
		begin
			delete FileSystem where GCRecord in (select UniqueColumn from TransTechnical where ID=@Id)
			delete TransApprove where RequestNo=@Id
	end
	else
		begin
			delete TransEditCosting where RequestNo=@Id
	end
	--delete TransFormula where RequestNo=@Id
	delete TransProductList where RequestNo=@Id
	--end
END


go

