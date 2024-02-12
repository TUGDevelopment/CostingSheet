-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fnc_GetLabActive]( 
	-- Add the parameters for the stored procedure here
	@SubID nvarchar(max))
	RETURNS  nvarchar(max)  
AS
BEGIN
declare @sql nvarchar(max)
			if(select count(*) from TransApprove where fn ='Receive Lab' and RequestNo=@SubID and StatusApp<>1)>0
			SET @sql = '''4'', ''3'''
			else if (select count(*) from TransApprove where fn ='Submit' and RequestNo=@SubID and StatusApp<>1)>0
			SET @sql = '''2'''
RETURN @sql
END
go

