-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spGetStructure
	-- Add the parameters for the stored procedure here
	@group varchar(50),
	@type nvarchar(250)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if (@type='RawMaterial')
		select * from [DevQCAnalysis].dbo.transRawMaterial where productgroup = (case when @group not like  '%non%' then 'PF (non fish base)' else 'PF (fish base)'end  )
		and ProductType='PF'
	if (@type='ContainerLid')
		select * from  [DevQCAnalysis].dbo.transCanSize where ProductType='PF'
END
go

