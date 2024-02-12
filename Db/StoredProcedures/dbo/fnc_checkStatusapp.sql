-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
create FUNCTION [dbo].[fnc_checkStatusapp]
(
	@ID nvarchar(max)
)
RETURNS nvarchar(max) 
AS
BEGIN
	-- Declare @type nvarchar(max)='0',@value nvarchar(max)='0,1,2'
	DECLARE @Result nvarchar(max)
	set @Result = concat('', (select StatusApp from TransCostingHeader where ID=@ID))
	RETURN @Result
END

go

