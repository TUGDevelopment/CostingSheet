-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spGetclaim 
	-- Add the parameters for the stored procedure here
	@claim nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @claim nvarchar(max) ='Dolphin safe,MSC claim'
    -- Insert statements for procedure here
	 declare @table tabletype;
	 insert @table
	 select ID from Masclaims where name in (select value from dbo.FNC_SPLIT(@claim,','))

	 select dbo.fnc_stuff(@table) 'Name'
	 --select * from Masclaims

END
go

