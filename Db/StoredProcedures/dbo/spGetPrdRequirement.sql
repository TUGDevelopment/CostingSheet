-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetPrdRequirement]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max)
AS
BEGIN
--declare @ID nvarchar(max)=1012
declare @usertype nvarchar(max)
set @usertype = (select usertype from MasPetCategory where ID=@ID)
select * from MasPrdRequirement where dbo.fnc_checktype(usertype,@usertype)>0
ORDER BY 
 CASE Name WHEN 'etc.' THEN 99 else id END ASC
END
go

