CREATE PROCEDURE [dbo].[spUpdateulogin] 
	-- Add the parameters for the stored procedure here
	@user_name nvarchar(max),
	@Position nvarchar(max),
	@userlevel nvarchar(max),
	@FirstName nvarchar(max),
	@LastName nvarchar(max),
	@Email nvarchar(max),
	@BU nvarchar(max),
	@usertype nvarchar(max),
	@au_id nvarchar(max),
	@factory nvarchar(max),
	@IsResign bit,
	@emplevel nvarchar(max),
	@RequestType nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @usertype nvarchar(max)='HF;PF',@RequestType nvarchar(max)='Costing;Stage gate'
	declare @type nvarchar(max),@Separator nvarchar(max)=null,@Req nvarchar(max)=''
	set @type = dbo.fnc_set_value(@usertype,';')
	set @Req=isnull((select abc =STUFF(((SELECT DISTINCT  ',' + convert(nvarchar(max),a.ID)
                                         FROM      MasRequestType a
                                         WHERE  dbo.fnc_checktype( @RequestType ,convert(nvarchar(max), a.Name ))>0  FOR XML PATH(''))), 1, 1, '')),'')
    -- Insert statements for procedure here
	if(select count(*) from ulogin where [user_name]=@user_name)>0 
	Update ulogin set [user_name]=@user_name,Position=@Position,
	FirstName=@FirstName,
	LastName=@LastName,
	Email=@Email,BU=@BU,IsResign=@IsResign,userlevel=@userlevel,sublevel=@emplevel,Plant=@factory,usertype=@type,RequestType=@Req where au_id=@au_id
	else 	Insert into ulogin values (@user_name,@Position,@userlevel,@FirstName,
	@LastName,@Email,@BU,0,@emplevel,@type,@factory,@Req)
END
--select * from MasRequestType where firstname like '%vora%'
go

