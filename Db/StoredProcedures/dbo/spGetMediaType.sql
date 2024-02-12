-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetMediaType]
	-- Add the parameters for the stored procedure here
	@group varchar(50)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @group varchar(50)='P'
		select * from  [DevQCAnalysis].dbo.transMediaType 
		-- where productgroup = (case when @group like '%non fish%' then 'PF (non fish base)' else 'PF (fish base)'end  )
		where dbo.fnc_checktype( productgroup, @group)>0
		and ProductType='PF'

END
go

