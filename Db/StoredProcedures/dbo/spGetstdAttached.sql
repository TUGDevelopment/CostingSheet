-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetstdAttached]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Id nvarchar(max)=0
    -- Insert statements for procedure here
	select * from transstdFileDetails where requestno= @Id
	--union select '1' truncate table transstdFileDetails
END
--select * from TransStdFileDetails where id=958
go

