-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetUtilize2]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@RequestNo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 
	--declare @ID nvarchar(max) =36143,	@RequestNo nvarchar(max)=828
    -- Insert statements for procedure here
	SELECT ID as 'RowID',
	CAST(Result as DECIMAL(9,2)) Result,
	[MonthName],Cost,SubID,RequestNo,''Mark,0 as Calcu from TransUtilize where SubID=@ID
	and RequestNo=@RequestNo
END
go

