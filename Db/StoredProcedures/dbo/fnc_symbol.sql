-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnc_symbol] 
(
	-- Add the parameters for the function here
	@value nvarchar(max)
)
RETURNS nvarchar(max) 
AS
BEGIN
	 declare @syntax nvarchar(max)=(SELECT CHARINDEX ( '|',  
       @value 
       COLLATE Latin1_General_CS_AS))
	 set @syntax =(select case when (CHARINDEX('|',isnull(@value,''))>0) then '|' 
	 when (CHARINDEX(';',isnull(@value,''))>0) then ';' 
	 when (CHARINDEX(',',isnull(@value,''))>0) then ',' else '|' end)

	-- Return the result of the function
	RETURN @syntax

END

go

