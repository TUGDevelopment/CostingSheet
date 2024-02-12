-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetHistory]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@user nvarchar(max),
	@tablename nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Id int=16341,@tablename nvarchar(max)='TransCostingHeader'
    -- Insert statements for procedure here
	SELECT a.Id,
	a.RequestNo,
	a.Username,
	a.StatusApp,
	a.Remark,case when Title in('Approve','Accept') then '-' else a.Reason end 'Reason' ,a.tablename,
	format(a.CreateOn,'dd-MM-yyyy HH:mm:ss')CreateOn,b.Title from MasHistory a left join MasStepApproval b on b.Id=a.StatusApp
	where RequestNo=@Id and a.tablename=@tablename order by a.id desc
	 
END
go

