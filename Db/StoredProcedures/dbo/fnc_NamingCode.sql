

CREATE FUNCTION [dbo].[fnc_NamingCode]
(
	@usertype nvarchar(max),
	@Company nvarchar(max)
)
RETURNS nvarchar(max) 
AS
BEGIN
	-- Declare @t nvarchar(max)='0;1'
	DECLARE @Result nvarchar(max)
	set @Result =(select top 1 NamingCode from MasPlant where dbo.fnc_checktype(@usertype, usertype) > 0 
	and dbo.fnc_checktype(Company, @Company) > 0)
	--print @Result
	-- Return the result of the function
	RETURN @Result

END
go

