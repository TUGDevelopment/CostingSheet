 
CREATE PROCEDURE [dbo].[spGetTopping]
	-- Add the parameters for the stored procedure here
	@group varchar(50)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		--declare @group nvarchar(max)='Q'
		select * from [DevQCAnalysis].dbo.transRawMaterial 
		--where productgroup = (case when @group like  '%non fish%' then 'PF (non fish base)' else 'PF (fish base)'end)
		where dbo.fnc_checktype( productgroup, @group)>0
		and ProductType='PF'

END
go

