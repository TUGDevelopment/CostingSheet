-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCostingSheet]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max)
AS
BEGIN
--declare @Id nvarchar(max)=195
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @RequestNo nvarchar(max)
    -- Insert statements for procedure here
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	SELECT @RequestNo=RDNumber from TransTechnical where ID=@Id
	print @RequestNo;
	select MarketingNumber,
	RDNumber,
	CanSize,@Id as 'RequestNo',ID from TransCostingHeader where Requestno=@RequestNo
END


--select format(1,'00')

go

