-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
create FUNCTION [dbo].[fnc_getstduser]
(
	@value nvarchar(max)
)
RETURNS nvarchar(max) 
AS
BEGIN
	-- Declare @type nvarchar(max)='0',@value nvarchar(max)='0,1,2'
	DECLARE @Result nvarchar(max)
	set @Result =(select abc =STUFF(((SELECT DISTINCT  ';' + (select top 1 b.[user_name] from ulogin b where concat(b.firstname,' ',b.lastname)=f.value)
											 FROM dbo.FNC_SPLIT(@value,';') f FOR XML PATH(''))), 1, 1, ''))
	RETURN @Result
END

go

