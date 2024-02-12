-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetPetFoodType]
	-- Add the parameters for the stored procedure here
	@usertype nvarchar(max) 
AS
BEGIN
--declare @usertype nvarchar(max)='1'
 select * from MasPetFoodType where dbo.fnc_checktype(usertype,@usertype)>0
 ORDER BY Name,
 CASE Name WHEN '-' THEN 99 else id END ASC
END
go

