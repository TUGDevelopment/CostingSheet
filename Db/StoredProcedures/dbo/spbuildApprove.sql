-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spbuildApprove]
	-- Add the parameters for the stored procedure here
	@user nvarchar(max) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @user nvarchar(max) ='MO630208'
	SET NOCOUNT ON;
	select * from(SELECT empid,   
	  isnull([L1],'') as 'L1', isnull([L2],(select LevelApprove from StdApprove where EMPID=[L1]))  as 'L2'
	FROM  
	(
	  SELECT Position, LevelApprove,empid   
	  FROM StdApprove
	) AS SourceTable  
	PIVOT  
	(  
	  max(LevelApprove)  
	  FOR Position IN ([L1], [L2])  
	) AS PivotTable) #a where EMPID=@user;
END
go

