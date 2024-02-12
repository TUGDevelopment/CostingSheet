/****** Script for SelectTopNRows command from SSMS  ******/
CREATE PROCEDURE [dbo].[spGetWF]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max)
AS
BEGIN
--declare @ID nvarchar(max)=3
SELECT [ID]
      ,[RequestNo]
      ,[StatusApp]
      ,[Condition]
      ,[fn]
      ,[ActiveBy]
      ,[SubmitDate]
      ,[levelApp]
      ,[tablename]
  FROM TransApprove where tablename='0' and RequestNo=@ID
end
 --select * from TransApprove where RequestNo=138 and SubmitDate is not null
 --update TransApprove set SubmitDate=GETDATE() where RequestNo=122 and SubmitDate is null

go

