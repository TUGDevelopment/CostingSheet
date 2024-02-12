-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spGetUpdateIsactive
	-- Add the parameters for the stored procedure here
	@Param nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @Result nvarchar(max)
    -- Insert statements for procedure here
	set @Result=(SELECT count(Isactive) from TransFormulaHeader where IsActive=1 and RequestNo=@Param)
	select @Result
END
go

