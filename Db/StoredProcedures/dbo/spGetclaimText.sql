-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetclaimText] 
	-- Add the parameters for the stored procedure here
	@claim nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @claim nvarchar(max) ='1,2,4,5'
    -- Insert statements for procedure here
	 declare @table tabletype;
	 insert @table
	 select Name from Masclaims where ID in (select value from dbo.FNC_SPLIT(@claim,';'))
	 and ID  not in (0,1,2,3)

	 select dbo.fnc_stuff(@table) 'Name'
	 --select * from Masclaims

END
go

