-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnc_checktype]
(
	@type nvarchar(max),
	@value nvarchar(max)
)
RETURNS nvarchar(max) 
AS
BEGIN
	-- Declare @type nvarchar(max)='0',@value nvarchar(max)='0,1,2'
	DECLARE @Result nvarchar(max)
	set @Result = (select count(value) from dbo.FNC_SPLIT(@value,
	dbo.fnc_symbol(@value)) where value in (select value from dbo.FNC_SPLIT(@type, dbo.fnc_symbol(@type))))
	RETURN @Result
END

go

