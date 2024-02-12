 
-- =============================================
CREATE PROCEDURE [dbo].[spGetCanSize]
	-- Add the parameters for the stored procedure here
	@group varchar(50)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		--declare @group varchar(50)='N'
		declare @n nvarchar(max)=(select name from [DevQCAnalysis].dbo.tblProductGroup where ProductGroup=@group)
		print @n;
		select * from  [DevQCAnalysis].dbo.transCanSize  
		where dbo.fnc_checktype( productgroup, @group)>0
		--where productgroup = (case when @group like '%non fish%' then 'PF (non fish base)' else 'PF (fish base)'end )
		and Packaging = (case when @n like  '%non can%' then 'Non Can' else 'Can'end )
		and ProductType='PF'

END
go

