-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spDelQuota]
	@Id nvarchar(max),
	@user nvarchar(max),
	@tablename nvarchar(max),
	@StatusApp nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	--declare @Id nvarchar(max)=5,@user nvarchar(max)='fo5910155',@tablename nvarchar(max)='TransQuotationHeader',@StatusApp nvarchar(max)='-1'
	SET NOCOUNT ON;
	--select * from MasHistory
	insert into MasHistory values(@Id,@user,@StatusApp,getdate(),'Del','',@tablename)
    -- Insert statements for procedure here
	delete TransQuotationHeader where ID=@Id
	delete TransQuotation where SubID=@Id
	delete TransQuotationCustom where SubID=@Id
	delete TransQuotationItems where SubID=@Id
END



go

