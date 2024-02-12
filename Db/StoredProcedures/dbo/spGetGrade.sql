-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetGrade]
	-- Add the parameters for the stored procedure here
	@group varchar(50)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @group nvarchar(max)='(H): PF Fish (non can) - CAT'
	SET NOCOUNT ON;
	--declare @group varchar(50)='P'
		select Code,[Description] from  [DevQCAnalysis].dbo.transGrade  
		--where productgroup like (case when @group like  '%non fish%' then '____non fish base)' else '____fish base)'end  )
		where dbo.fnc_checktype( productgroup, @group)>0
		and ProductType='PF' union select '-1',''
END
go

