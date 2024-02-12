-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spUpdateApprove]
	-- Add the parameters for the stored procedure here
	@StatusApp nvarchar(max),
	@User nvarchar(max),
	@Id nvarchar(max),
	@status nvarchar(max),
	@RequestType nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(select Sublevel from MasApprovAssign where EmpId=@User) in (14) 
		UPDATE TransApprove Set StatusApp=1,ActiveBy=@User,
	SubmitDate=GETDATE()
	Where RequestNo=@Id and levelApp=case when @StatusApp=4 then 7 when @StatusApp=2 then 8 else @status end and tablename=@RequestType
	else if(@StatusApp in (0,1,2,4,5,6,7)) 
	UPDATE TransApprove Set StatusApp=1,ActiveBy=@User,
	SubmitDate=GETDATE()
	Where RequestNo=@Id and levelApp=@status and tablename=@RequestType
END
go

