-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetTunaCal]
	-- Add the parameters for the stored procedure here
	@RequestNo nvarchar(max),
	@SubID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @RequestNo nvarchar(max)=845,	@SubID nvarchar(max)=36333
	SET NOCOUNT ON; 
	select [RowID]
      ,[Component]
      ,[Name]
      ,[Currency]
      ,convert(float,[Result]) as 'Result'
      ,convert(int,[Calcu]) as 'Calcu'
      ,[Quantity]
      ,[Price]
      ,[Unit]
      ,[BaseUnit]
      ,[SubID]
      ,[RequestNo] from TransTunaCal where RequestNo=@RequestNo and SubID=@SubID order by Calcu,RowID
END

--select sum(convert(float,[Result])) from TransTunaCal where Component in ('Media')
go

