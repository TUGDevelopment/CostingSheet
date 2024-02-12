-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spAssignDocument]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(50),
	@Assignee nvarchar(max),
	@Type nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(50)=214,@Assignee nvarchar(max)='MO520826',@Type nvarchar(max)=7
	SET NOCOUNT ON;
	if(@Type=6 Or @Type=7)
	Update TransTechnical set Modified=getdate(),assignee=@Assignee where Id=@Id 
	if(@Type=5)
	Update TransTechnical set Receiver=@Assignee where Id=@Id

	SELECT 
	RequestNo,
	(case when @Type=6 then Assignee when @Type=5 or @Type=7 then Receiver end)'Assignee',
	(case when @Type=7 then isnull(Assignee,'')+','+ isnull(Requester,'') else 
	Requester end) 'Requester'
	 FROM TransTechnical where Id=@Id
END



go

