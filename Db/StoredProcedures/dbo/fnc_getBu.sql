-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fnc_getBu](
	@Bu nvarchar(max),
	@IsBu nvarchar(max))
RETURNS  nvarchar(max)  
AS
BEGIN
	--declare @Bu nvarchar(max)='102',@IsBu nvarchar(max)='1'
	declare @temp TableType

	insert into @temp
	SELECT Code
	FROM MasCompany a
	WHERE a.Code in (case when @IsBu='1' then a.Code else @Bu end)
	
	DECLARE @cols AS NVARCHAR(MAX);
		SET @cols = dbo.FNC_STUFF(@temp)
	--select @Cols;
RETURN @cols
END


go

