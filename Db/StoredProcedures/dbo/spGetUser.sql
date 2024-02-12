-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetUser]
@usertype nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select * from(SELECT au_id,upper([user_name])'user_name',firstname +' '+ lastname as fn,Position,Email 
	from ulogin where dbo.fnc_checktype(@usertype,usertype)>0 )#a union select -1,'0','None','',''
END

go

