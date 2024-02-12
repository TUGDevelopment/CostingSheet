-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetContainerLid]
	-- Add the parameters for the stored procedure here
	@group varchar(50)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @group nvarchar(max)='(G): PF Fish (can) - CAT'
	SET NOCOUNT ON;
		select * from  [DevQCAnalysis].dbo.transContainerLid  
		--where productgroup like (case when @group like  '%non%' then '____non fish base)' else '____fish base)'end  )
		where dbo.fnc_checktype( productgroup, @group)>0
		and ProductType='PF'
END
 
go

