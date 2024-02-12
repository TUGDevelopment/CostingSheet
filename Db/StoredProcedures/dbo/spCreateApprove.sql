-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spCreateApprove]
	-- Add the parameters for the stored procedure here
	@Id int,
	@Requester  nvarchar(max),
	@Type nvarchar(1),
	@usertype nvarchar(1)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Type nvarchar(1)=0,@Id int=6895,	@Requester  nvarchar(max)='FO6110057',@usertype nvarchar(1)=0
	declare @stausapp nvarchar(max)
	set @stausapp=(case when @Type=1 then 2 else @Type end)
	Insert into TransApprove(RequestNo,StatusApp,Condition,fn,ActiveBy,SubmitDate,levelApp,tablename)
	select @id,0,1,fn,case when StatusApp=@stausapp then @Requester else null end
	,case when StatusApp=@stausapp then getdate() else null end,StatusApp,@Type from MasWorkFlowapp where Userstatus=@Type and 
	dbo.fnc_checktype(usertype,@usertype)>0

	--select * from MasWorkFlowapp
	--select * from transapprove where requestno=6895
END
 


go

