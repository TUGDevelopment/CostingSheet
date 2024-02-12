-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetstdAttached2]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	select 
	ID,
	LastWriteTime,
	LastUpdateBy,
	Result,
	Name,
	Notes,
	'' as Attached,
	SubID,
	RequestNo,
	'' as IsApprove,
	10 as Calcu from transstdFileDetails where RequestNo= @Id
	--union select '1' truncate table transstdFileDetails
END
go

