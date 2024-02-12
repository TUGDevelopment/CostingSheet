-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetulogin]
	-- Add the parameters for the stored procedure here
	@Bu nvarchar(max),
	@usertype nvarchar(max)
AS
BEGIN
	--declare @bu nvarchar(max)='102;101',@usertype nvarchar(max)='0'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @Mytable TABLE (au_id int, [user_name] nvarchar(max),Position nvarchar(max),userlevel nvarchar(max)
		,FirstName nvarchar(max),LastName nvarchar(max),Email nvarchar(max),BU nvarchar(max),IsResign bit,usertype nvarchar(max),sublevel nvarchar(max)) 
	select * from (select au_id,[user_name],
      Position,userlevel,FirstName,LastName,Email,
      BU,Plant as 'Factory',convert(bit,IsResign)'IsResign',replace(dbo.fnc_gettype(usertype),',',';') usertype,
	  isnull((select abc =STUFF(((SELECT DISTINCT  ',' + a.Name
                                         FROM      MasRequestType a
                                         WHERE dbo.fnc_checktype(RequestType ,convert(nvarchar(max), a.ID ))>0 FOR XML PATH(''))), 1, 1, '')),'') as 'RequestType'--,concat(FirstName,' ',LastName)'fn'
	  from ulogin where 
	  dbo.fnc_checktype(BU,@Bu)>0 and dbo.fnc_checktype(@usertype,usertype)>0)#a
	  order by FirstName,LastName
END
go

